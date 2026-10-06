using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecaverso.Networking;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using XRMultiplayer;

namespace Tecaverso.Hub
{
    /// <summary>Host-authoritative lobby on the template's direct UDP transport.</summary>
    public sealed class LanLobbyController : MonoBehaviour
    {
        public enum ConnectionState { Offline, Connecting, Lobby, Failed }
        const string SnapshotMessage = "Tecaverso.Lobby.Snapshot.v1", ReadyMessage = "Tecaverso.Lobby.Ready.v1", LabReadyMessage = "Tecaverso.Lobby.LabReady.v1";
        [SerializeField] NetworkManager manager;
        [SerializeField] HubCatalog catalog;
        readonly Dictionary<ulong, LanLobbyProtocol.Participant> members = new();
        readonly Dictionary<ulong, LanLobbyProtocol.JoinRequest> reservations = new();
        public event Action Changed;
        public ConnectionState State { get; private set; }
        public LanLobbyProtocol.Snapshot Snapshot { get; private set; } = new();
        public string ErrorCode { get; private set; }
        public string Address { get; private set; }
        public bool IsHost => manager != null && manager.IsHost;
        public ulong LocalId => manager != null ? manager.LocalClientId : ulong.MaxValue;
        public NetworkManager Manager => manager;
        public bool EveryoneInLab => IsHost && Snapshot.experimentActive && members.Count>0 && members.Values.All(p=>p.labReady);
        public bool CanStart => IsHost && State == ConnectionState.Lobby && Snapshot.supportsMultiplayer &&
            !Snapshot.experimentActive && members.Count > 0 && reservations.Count == 0 && members.Values.All(p => p.ready);
        bool intentionalStop, registered, hooked;
        float connectionDeadline, nextSnapshot;
        string requestedRoom;

        public void Bind(NetworkManager networkManager, HubCatalog hubCatalog) { manager = networkManager; catalog = hubCatalog; }
        void Start()
        {
            Hook();
            if(GetComponent<LanExperimentSession>()==null) gameObject.AddComponent<LanExperimentSession>().Bind(this);
        }
        void Hook()
        {
            if (hooked || manager == null) return;
            manager.OnClientConnectedCallback += Connected;
            manager.OnClientDisconnectCallback += Disconnected;
            manager.OnClientStopped += Stopped;
            manager.OnTransportFailure += TransportFailed;
            hooked = true;
        }
        void OnDestroy()
        {
            if (!hooked || manager == null) return;
            manager.OnClientConnectedCallback -= Connected;
            manager.OnClientDisconnectCallback -= Disconnected;
            manager.OnClientStopped -= Stopped;
            manager.OnTransportFailure -= TransportFailed;
            if (manager.ConnectionApprovalCallback == Approve) manager.ConnectionApprovalCallback = null;
            UnregisterMessages();
        }
        public bool Host(string nickname, string room)
        {
            if (!Prepare(nickname)) return false;
            requestedRoom = LanLobbyProtocol.CleanName(room, 40);
            if (requestedRoom.Length == 0) requestedRoom = "Tecaverso Labs • LAN";
            Address = string.Join(" / ", LanAddress.GetLocalAddresses());
            if (Address.Length == 0) Address = "Sem IPv4 LAN disponível (loopback: 127.0.0.1)";
            if (!XRINetworkGameManager.Instance.HostLocalConnection()) { Fail("HOST_FAILED"); return false; }
            RegisterMessages();
            // StartHost can invoke the local connection callback synchronously.
            if (!members.ContainsKey(manager.LocalClientId)) Connected(manager.LocalClientId);
            XRINetworkGameManager.ConnectedRoomName.Value = requestedRoom;
            return true;
        }
        public bool Join(string nickname, string address)
        {
            if (!LanAddress.TryNormalize(address, out var normalized)) { Fail("INVALID_IP", false); return false; }
            if (!Prepare(nickname)) return false;
            Address = normalized;
            ((UnityTransport)manager.NetworkConfig.NetworkTransport).SetConnectionData(normalized, LanAddress.DefaultPort);
            if (!XRINetworkGameManager.Instance.JoinLocalConnection()) { Fail("CONNECTION_FAILED"); return false; }
            RegisterMessages();
            return true;
        }
        bool Prepare(string nickname)
        {
            Hook();
            if (manager == null || XRINetworkGameManager.Instance == null ||
                manager.IsListening || manager.ShutdownInProgress || State == ConnectionState.Connecting) return false;
            var name = LanLobbyProtocol.CleanName(nickname);
            if (name.Length == 0) { Fail("INVALID_NAME", false); return false; }
            if (!(manager.NetworkConfig.NetworkTransport is UnityTransport)) { Fail("TRANSPORT_FAILED", false); return false; }
            if (manager.ConnectionApprovalCallback != null && manager.ConnectionApprovalCallback != Approve)
            { Fail("APPROVAL_IN_USE", false); return false; }
            members.Clear(); reservations.Clear(); Snapshot = new(); registered = false;
            intentionalStop = false; ErrorCode = null;
            manager.NetworkConfig.ConnectionApproval = true;
            manager.NetworkConfig.ConnectionData = Encoding.UTF8.GetBytes(JsonUtility.ToJson(new LanLobbyProtocol.JoinRequest
            { name = name, version = Application.version }));
            manager.ConnectionApprovalCallback = Approve;
            XRINetworkGameManager.LocalPlayerName.Value = name;
            State = ConnectionState.Connecting; connectionDeadline = Time.realtimeSinceStartup + 12f;
            Changed?.Invoke();
            return true;
        }
        void Approve(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            LanLobbyProtocol.JoinRequest data = null;
            if (request.Payload != null && request.Payload.Length <= 512)
            {
                try { data = JsonUtility.FromJson<LanLobbyProtocol.JoinRequest>(Encoding.UTF8.GetString(request.Payload)); }
                catch (ArgumentException) { }
            }
            string rejection = LanLobbyProtocol.Validate(data, Application.version, members.Count + reservations.Count);
            response.Approved = rejection == null;
            response.CreatePlayerObject = response.Approved && !manager.NetworkConfig.AutoSpawnPlayerPrefabClientSide;
            response.Pending = false; response.Reason = rejection ?? "";
            if (response.Approved) { data.name = LanLobbyProtocol.CleanName(data.name); reservations[request.ClientNetworkId] = data; }
        }
        void Connected(ulong id)
        {
            RegisterMessages();
            if (IsHost)
            {
                bool host = id == manager.LocalClientId;
                reservations.TryGetValue(id, out var request);
                reservations.Remove(id);
                members[id] = new LanLobbyProtocol.Participant
                { id = id, host = host, ready = host, name = host ? XRINetworkGameManager.LocalPlayerName.Value : request?.name ?? "Participante" };
                Snapshot.room = requestedRoom;
                if (string.IsNullOrEmpty(Snapshot.contentId)) SelectFirstContent();
                State = ConnectionState.Lobby; Publish();
            }
            // Clients enter the lobby only after receiving its authoritative snapshot.
        }
        void RegisterMessages()
        {
            if (registered || manager == null || manager.CustomMessagingManager == null) return;
            manager.CustomMessagingManager.RegisterNamedMessageHandler(SnapshotMessage, ReceiveSnapshot);
            manager.CustomMessagingManager.RegisterNamedMessageHandler(ReadyMessage, ReceiveReady);
            manager.CustomMessagingManager.RegisterNamedMessageHandler(LabReadyMessage, ReceiveLabReady);
            registered = true;
        }
        void UnregisterMessages()
        {
            if (!registered || manager == null || manager.CustomMessagingManager == null) return;
            manager.CustomMessagingManager.UnregisterNamedMessageHandler(SnapshotMessage);
            manager.CustomMessagingManager.UnregisterNamedMessageHandler(ReadyMessage); registered = false;
            manager.CustomMessagingManager.UnregisterNamedMessageHandler(LabReadyMessage);
        }
        void Update()
        {
            if (State == ConnectionState.Connecting && Time.realtimeSinceStartup >= connectionDeadline) Fail("TIMEOUT");
            // Also covers a client whose messaging handlers were registered after its connect callback.
            if (IsHost && State == ConnectionState.Lobby && Time.realtimeSinceStartup >= nextSnapshot)
            { nextSnapshot = Time.realtimeSinceStartup + 2f; Publish(); }
        }
        void Publish()
        {
            Snapshot.participants = members.Values.OrderByDescending(p => p.host).ThenBy(p => p.id).ToArray();
            Changed?.Invoke();
            if (!registered) return;
            var payload = new FixedString4096Bytes(JsonUtility.ToJson(Snapshot));
            using var writer = new FastBufferWriter(4096, Allocator.Temp);
            writer.WriteValueSafe(payload);
            foreach (var id in manager.ConnectedClientsIds)
                if (id != manager.LocalClientId) manager.CustomMessagingManager.SendNamedMessage(SnapshotMessage, id, writer, NetworkDelivery.ReliableFragmentedSequenced);
        }
        void ReceiveSnapshot(ulong sender, FastBufferReader reader)
        {
            if (IsHost || sender != NetworkManager.ServerClientId || intentionalStop || State == ConnectionState.Failed) return;
            try
            {
                reader.ReadValueSafe(out FixedString4096Bytes payload);
                var snapshot = JsonUtility.FromJson<LanLobbyProtocol.Snapshot>(payload.ToString());
                if (snapshot?.participants == null || snapshot.participants.Length > LanLobbyProtocol.Capacity ||
                    !snapshot.participants.Any(p => p.id == manager.LocalClientId)) return;
                Snapshot = snapshot; State = ConnectionState.Lobby; ErrorCode = null; Changed?.Invoke();
            }
            catch (Exception e) when (e is ArgumentException || e is OverflowException) { Fail("INVALID_SNAPSHOT"); }
        }
        public void SetReady(bool ready)
        {
            if (State != ConnectionState.Lobby || IsHost || !registered) return;
            using var writer = new FastBufferWriter(1, Allocator.Temp);
            writer.WriteValueSafe(ready);
            manager.CustomMessagingManager.SendNamedMessage(ReadyMessage, NetworkManager.ServerClientId, writer);
        }
        void ReceiveReady(ulong sender, FastBufferReader reader)
        {
            if (!IsHost || reader.Length-reader.Position != 1 || !members.TryGetValue(sender, out var member) || member.host) return;
            reader.ReadValueSafe(out bool value); member.ready = value; Publish();
        }
        public void ReportLabReady()
        {
            if(!Snapshot.experimentActive || !registered) return;
            if(IsHost) { if(members.TryGetValue(LocalId,out var member)) member.labReady=true; Publish(); }
            else { using var writer=new FastBufferWriter(1,Allocator.Temp); writer.WriteValueSafe(true);
                manager.CustomMessagingManager.SendNamedMessage(LabReadyMessage,NetworkManager.ServerClientId,writer); }
        }
        void ReceiveLabReady(ulong sender, FastBufferReader reader)
        {
            if(!IsHost || !Snapshot.experimentActive || reader.Length-reader.Position!=1 || !members.TryGetValue(sender,out var member)) return;
            reader.ReadValueSafe(out bool ready); member.labReady=ready; Publish();
        }
        public void StartExperiment()
        {
            if(!CanStart) return;
            if(!Application.CanStreamedLevelBeLoaded(Snapshot.contentId)) { Fail("SCENE_UNAVAILABLE"); return; }
            foreach(var member in members.Values) member.labReady=false;
            Snapshot.experimentActive=true; Publish();
        }
        public void ReturnToLobby()
        {
            if(!IsHost) { Leave(); return; }
            Snapshot.experimentActive=false;
            foreach(var member in members.Values) { member.labReady=false; member.ready=member.host; }
            Publish();
        }
        void SelectFirstContent()
        {
            if (catalog == null) return;
            var content = catalog.disciplines.SelectMany(d => d.contents).FirstOrDefault(c => !string.IsNullOrEmpty(c.scenePath));
            if (content != null) SetContent(content);
        }
        public void SelectContent(HubCatalog.Content content)
        {
            if (!IsHost || State != ConnectionState.Lobby || Snapshot.experimentActive || content == null || string.IsNullOrEmpty(content.scenePath) ||
                !catalog.disciplines.Any(d => d.contents.Contains(content))) return;
            SetContent(content);
            foreach (var member in members.Values) member.ready = member.host;
            Publish();
        }
        void SetContent(HubCatalog.Content content)
        {
            Snapshot.contentId = content.scenePath; Snapshot.contentTitle = content.title;
            Snapshot.supportsMultiplayer = content.scenePath == LanExperimentSession.ExperimentPath;
        }
        void Disconnected(ulong id)
        {
            reservations.Remove(id);
            if (IsHost) { members.Remove(id); if (!intentionalStop) Publish(); }
            else if (id == manager.LocalClientId && !intentionalStop)
                Fail(string.IsNullOrEmpty(manager.DisconnectReason) ? State == ConnectionState.Lobby ? "HOST_LOST" : "CONNECTION_FAILED" : manager.DisconnectReason, false);
        }
        void Stopped(bool wasHost)
        {
            registered = false; reservations.Clear(); members.Clear();
            if (!intentionalStop && State != ConnectionState.Failed) Fail(State == ConnectionState.Lobby ? "HOST_LOST" : "CONNECTION_FAILED", false);
        }
        void TransportFailed() => Fail("TRANSPORT_FAILED");
        void Fail(string code, bool shutdown = true)
        {
            ErrorCode = code; State = ConnectionState.Failed;
            if (shutdown && manager != null && (manager.IsListening || manager.IsClient))
            { intentionalStop = true; XRINetworkGameManager.Instance?.LeaveLocalConnection(); }
            Changed?.Invoke();
        }
        public void Leave()
        {
            intentionalStop = true; UnregisterMessages();
            if (manager != null && (manager.IsListening || manager.IsClient)) XRINetworkGameManager.Instance?.LeaveLocalConnection();
            members.Clear(); reservations.Clear(); Snapshot = new(); ErrorCode = null; State = ConnectionState.Offline; Changed?.Invoke();
        }
    }
}
