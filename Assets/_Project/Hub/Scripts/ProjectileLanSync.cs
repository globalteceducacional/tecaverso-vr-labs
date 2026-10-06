using System;
using System.Collections.Generic;
using Tecaverso.Labs.ObliqueLaunch;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Tecaverso.Hub
{
    /// <summary>Host-authoritative, bounded analytical history. Visual preferences remain per-device.</summary>
    public sealed class ProjectileLanSync : MonoBehaviour
    {
        const string Message = "Tecaverso.Projectile.State.v1";
        [Serializable] public sealed class Settings
        {
            public float angle, speed, height, mass, gravity;
            public Settings() { }
            public Settings(LaunchParameters p) { angle=p.Angle; speed=p.Speed; height=p.Height; mass=p.Mass; gravity=p.Gravity; }
            public LaunchParameters Value => new(angle,speed,height,mass,gravity);
        }
        [Serializable] public sealed class Shot { public int id; public Settings settings; public float time; public SimulationState state; }
        [Serializable] public sealed class StatePacket
        {
            public int revision, sequence;
            public double serverTime;
            public Settings controls;
            public Shot[] shots;
        }
        readonly List<Shot> history=new();
        LanLobbyController lobby;
        ObliqueLaunchLab lab;
        NetworkManager manager;
        StatePacket received;
        int revision, serial, sequence, appliedRevision=-1, appliedShot=-1, receivedSequence=-1;
        float nextSend;
        bool bound;
        public void Bind(LanLobbyController service,ObliqueLaunchLab experiment)
        {
            lobby=service; manager=service.Manager; lab=experiment; bound=true;
            lab.Simulation.RemoteDriven=!lobby.IsHost;
            lab.SetControlAuthority(lobby.EveryoneInLab);
            manager.CustomMessagingManager.RegisterNamedMessageHandler(Message,Receive);
            if(lobby.IsHost) { lab.Launched+=Launched; lab.ResetRequested+=ResetHistory; lab.Simulation.StateChanged+=StateChanged; }
        }
        void OnDisable()
        {
            if(!bound) return;
            lab.Launched-=Launched; lab.ResetRequested-=ResetHistory;
            lab.Simulation.StateChanged-=StateChanged;
            manager.CustomMessagingManager?.UnregisterNamedMessageHandler(Message);
            bound=false;
        }
        void Launched()
        {
            history.Add(new Shot { id=++serial, settings=new Settings(lab.Simulation.Parameters), state=SimulationState.Running });
            if(history.Count>5) history.RemoveAt(0);
            Send();
        }
        void ResetHistory() { revision++; history.Clear(); Send(); }
        void StateChanged(SimulationState state)
        { if(state==SimulationState.Paused || state==SimulationState.Complete) Send(); }
        void LateUpdate()
        {
            if(!bound || !manager.IsListening) return;
            lab.SetControlAuthority(lobby.EveryoneInLab);
            if(lobby.IsHost)
            {
                if(Time.unscaledTime>=nextSend) { nextSend=Time.unscaledTime+.1f; Send(); }
            }
            else if(received?.shots?.Length>0)
            {
                var shot=received.shots[received.shots.Length-1];
                // At most 250 ms of extrapolation; stalled links must not run indefinitely.
                float elapsed=shot.state==SimulationState.Running ? Mathf.Clamp((float)(manager.ServerTime.Time-received.serverTime),0,.25f) : 0;
                Advance(shot.state==SimulationState.Running?Mathf.Max(lab.Simulation.Current.Time,shot.time+elapsed):shot.time,shot.state);
            }
        }
        void Send()
        {
            if(!bound || !manager.IsListening || !lobby.IsHost) return;
            if(history.Count>0) { var shot=history[history.Count-1]; shot.time=lab.Simulation.Current.Time; shot.state=lab.Simulation.State; }
            var packet=new StatePacket { revision=revision,sequence=++sequence,serverTime=manager.ServerTime.Time,
                controls=new Settings(lab.Panel.Parameters), shots=history.ToArray() };
            var payload=new FixedString4096Bytes(JsonUtility.ToJson(packet));
            using var writer=new FastBufferWriter(4096,Allocator.Temp);
            writer.WriteValueSafe(payload);
            foreach(var id in manager.ConnectedClientsIds)
                if(id!=manager.LocalClientId) manager.CustomMessagingManager.SendNamedMessage(Message,id,writer,NetworkDelivery.ReliableFragmentedSequenced);
        }
        void Receive(ulong sender,FastBufferReader reader)
        {
            if(!bound || lobby.IsHost || sender!=NetworkManager.ServerClientId) return;
            StatePacket packet;
            try { reader.ReadValueSafe(out FixedString4096Bytes payload); packet=JsonUtility.FromJson<StatePacket>(payload.ToString()); }
            catch(Exception e) when(e is ArgumentException || e is OverflowException) { return; }
            if(!Valid(packet) || packet.sequence<=receivedSequence) return;
            receivedSequence=packet.sequence;
            if(appliedRevision!=packet.revision)
            { lab.ClearRemoteHistory(); appliedRevision=packet.revision; appliedShot=-1; }
            // If the client missed more than five launches, discard its obsolete local history first.
            if(packet.shots.Length>0 && appliedShot>=0 && appliedShot<packet.shots[0].id-1)
            { lab.ClearRemoteHistory(); appliedShot=-1; }
            foreach(var shot in packet.shots)
            {
                if(shot.id>appliedShot)
                {
                    lab.BeginRemoteLaunch(shot.settings.Value);
                    Advance(shot.time,shot.state);
                    appliedShot=shot.id;
                }
                else if(shot.id==appliedShot) Advance(shot.state==SimulationState.Running?Mathf.Max(lab.Simulation.Current.Time,shot.time):shot.time,shot.state);
            }
            lab.ApplyRemoteParameters(packet.controls.Value);
            received=packet;
        }
        void Advance(float target,SimulationState state)
        {
            if(Mathf.Approximately(lab.Simulation.Current.Time,target)&&lab.Simulation.State==state)return;
            // Feed the same ordered samples into the existing trajectory/snapshot/apex views.
            float time=lab.Simulation.Current.Time;
            for(int steps=0;time+.05f<target && steps<300;steps++)
            { time+=.05f; lab.Simulation.ApplyTime(time,SimulationState.Running); }
            lab.Simulation.ApplyTime(target,state);
        }
        public static bool Valid(StatePacket p)
        {
            if(p==null || p.controls==null || p.shots==null || p.shots.Length>5 || !Finite(p.controls) ||
                double.IsNaN(p.serverTime)||double.IsInfinity(p.serverTime)) return false;
            int last=-1;
            foreach(var shot in p.shots)
            {
                if(shot==null||shot.settings==null||!Finite(shot.settings)||shot.id<=last||
                    float.IsNaN(shot.time)||float.IsInfinity(shot.time)||shot.time<0||shot.time>15||
                    shot.state<SimulationState.Running||shot.state>SimulationState.Complete) return false;
                last=shot.id;
            }
            return true;
        }
        static bool Finite(Settings p) => float.IsFinite(p.angle)&&float.IsFinite(p.speed)&&float.IsFinite(p.height)&&float.IsFinite(p.mass)&&float.IsFinite(p.gravity);
    }
}
