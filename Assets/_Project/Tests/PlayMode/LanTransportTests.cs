using System.Collections;
using System.Net;
using System.Net.Sockets;
using NUnit.Framework;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.TestTools;

public class LanTransportTests
{
    NetworkManager host;
    NetworkManager client;

    NetworkManager CreatePeer(string name, ushort port)
    {
        var go = new GameObject(name);
        var transport = go.AddComponent<UnityTransport>();
        transport.SetConnectionData("127.0.0.1", port, "0.0.0.0");
        var manager = go.AddComponent<NetworkManager>();
        manager.NetworkConfig = new NetworkConfig
        {
            NetworkTransport = transport,
            NetworkTopology = NetworkTopologyTypes.DistributedAuthority,
            UseCMBService = false,
            AutoSpawnPlayerPrefabClientSide = false,
            EnableSceneManagement = false,
            ForceSamePrefabs = false
        };
        return manager;
    }

    [UnityTest]
    public IEnumerator DirectUdpConnectsDisconnectsAndReconnectsWithoutCloud()
    {
        ushort port;
        using (var socket = new UdpClient(0)) port = (ushort)((IPEndPoint)socket.Client.LocalEndPoint).Port;
        host = CreatePeer("LAN test host", port);
        client = CreatePeer("LAN test client", port);
        Assert.That(host.StartHost(), Is.True);
        Assert.That(client.StartClient(), Is.True);
        var deadline = Time.realtimeSinceStartup + 10;
        while (!client.IsConnectedClient && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(client.IsConnectedClient, Is.True, "Direct UDP client must complete the handshake.");
        Assert.That(host.DistributedAuthorityMode, Is.True);
        client.Shutdown();
        deadline = Time.realtimeSinceStartup + 5;
        while ((client.IsListening || client.ShutdownInProgress) && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(client.StartClient(), Is.True);
        deadline = Time.realtimeSinceStartup + 10;
        while (!client.IsConnectedClient && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(client.IsConnectedClient, Is.True, "A disconnected peer must be able to reconnect.");
    }

    [UnityTearDown]
    public IEnumerator Cleanup()
    {
        if (client != null) client.Shutdown();
        if (host != null) host.Shutdown();
        yield return null;
        yield return null;
        if (client != null) Object.Destroy(client.gameObject);
        if (host != null) Object.Destroy(host.gameObject);
        yield return null;
    }
}
