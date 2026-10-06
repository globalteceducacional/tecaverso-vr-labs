using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Object=UnityEngine.Object;

// Reflection is confined to this integration test: the existing lab is in Assembly-CSharp,
// which cannot be referenced by an asmdef without migrating the entire existing project.
public class LanLobbyExperimentTests
{
    const string Path="Assets/_Project/Laboratories/Physics/ObliqueLaunch/Scenes/ObliqueLaunch.unity";
    const BindingFlags Flags=BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic;
    NetworkManager host,client;
    GameObject replica;
    static Type Runtime(string name)=>Type.GetType(name+", Assembly-CSharp",true);
    static object Get(object target,string name)=>target.GetType().GetProperty(name,Flags).GetValue(target);
    static object Field(object target,string name)=>target.GetType().GetField(name,Flags).GetValue(target);
    static object Call(object target,string name,params object[] args)=>target.GetType().GetMethod(name,Flags).Invoke(target,args);
    NetworkManager Peer(string name,ushort port,out Component lobby)
    {
        var go=new GameObject(name);var transport=go.AddComponent<UnityTransport>();transport.SetConnectionData("127.0.0.1",port,"0.0.0.0");
        var manager=go.AddComponent<NetworkManager>();
        manager.NetworkConfig=new NetworkConfig {NetworkTransport=transport,NetworkTopology=NetworkTopologyTypes.DistributedAuthority,
            UseCMBService=false,AutoSpawnPlayerPrefabClientSide=false,EnableSceneManagement=false,ForceSamePrefabs=false,ConnectionApproval=true,
            ConnectionData=Encoding.UTF8.GetBytes("{\"protocol\":1,\"name\":\"Aluno\",\"version\":\""+Application.version+"\"}")};
        lobby=go.AddComponent(Runtime("Tecaverso.Hub.LanLobbyController"));((Behaviour)lobby).enabled=false;
        Call(lobby,"Bind",manager,null);Call(lobby,"Hook");
        manager.ConnectionApprovalCallback=(Action<NetworkManager.ConnectionApprovalRequest,NetworkManager.ConnectionApprovalResponse>)
            Delegate.CreateDelegate(typeof(Action<NetworkManager.ConnectionApprovalRequest,NetworkManager.ConnectionApprovalResponse>),lobby,lobby.GetType().GetMethod("Approve",Flags));
        return manager;
    }
    static IEnumerator Until(Func<bool> predicate,string message)
    {
        float deadline=Time.realtimeSinceStartup+12;
        while(!predicate()&&Time.realtimeSinceStartup<deadline)yield return null;
        Assert.IsTrue(predicate(),message);
    }
    [UnityTest]
    public IEnumerator DirectLanReplicatesLobbyLateJoinPauseAndReset()
    {
        ushort port;using(var socket=new UdpClient(0))port=(ushort)((IPEndPoint)socket.Client.LocalEndPoint).Port;
        yield return SceneManager.LoadSceneAsync(Path,LoadSceneMode.Additive);
        var labType=Runtime("Tecaverso.Labs.ObliqueLaunch.ObliqueLaunchLab");
        var lab=SceneManager.GetSceneByPath(Path).GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren(labType,true)).First();
        replica=Object.Instantiate(lab.gameObject);replica.name="LAN test observer";
        var observer=replica.GetComponent(labType);
        host=Peer("LAN integration host",port,out var hostLobby);
        Assert.IsTrue(host.StartHost());
        var snapshot=Get(hostLobby,"Snapshot");snapshot.GetType().GetField("experimentActive").SetValue(snapshot,true);Call(hostLobby,"ReportLabReady");
        var syncType=Runtime("Tecaverso.Hub.ProjectileLanSync");
        Call(lab.gameObject.AddComponent(syncType),"Bind",hostLobby,lab);
        var parameters=Activator.CreateInstance(Runtime("Tecaverso.Labs.ObliqueLaunch.LaunchParameters"),45f,18f,2f,3f,9.81f);
        var sim=Get(lab,"Simulation");var simState=Runtime("Tecaverso.Labs.ObliqueLaunch.SimulationState");
        for(int i=0;i<6;i++){Call(lab,"BeginRemoteLaunch",parameters);Call(sim,"ApplyTime",14f,Enum.Parse(simState,"Running"));}
        client=Peer("LAN integration observer",port,out var clientLobby);
        Assert.IsTrue(client.StartClient());
        yield return Until(()=>client.IsConnectedClient,"Local UDP handshake failed.");
        Call(hostLobby,"Publish");
        yield return Until(()=>((Array)Field(Get(clientLobby,"Snapshot"),"participants")).Length==2,"Lobby snapshot missing.");
        Call(clientLobby,"SetReady",true);
        yield return Until(()=>((Array)Field(Get(hostLobby,"Snapshot"),"participants")).Cast<object>().All(p=>(bool)Field(p,"ready")),"Ready state missing.");
        Call(clientLobby,"ReportLabReady");
        Call(replica.AddComponent(syncType),"Bind",clientLobby,observer);
        var trace=replica.GetComponentInChildren(Runtime("Tecaverso.Labs.ObliqueLaunch.ProjectileTrajectoryLine"),true);
        yield return Until(()=>(int)Get(trace,"RecordCount")==5,"Late join did not rebuild five launches.");
        Assert.IsFalse((bool)Field(observer,"controlAuthority"),"Observers must not operate the experiment.");
        var observerSim=Get(observer,"Simulation");
        Assert.AreEqual("Complete",Get(observerSim,"State").ToString());
        Call(lab,"BeginRemoteLaunch",parameters);Call(sim,"ApplyTime",.8f,Enum.Parse(simState,"Running"));Call(sim,"TogglePause");
        yield return Until(()=>Get(observerSim,"State").ToString()=="Paused","Pause was not replicated.");
        Assert.That((float)Field(Get(observerSim,"Current"),"Time"),Is.EqualTo(.8f).Within(.001f));
        Call(sim,"TogglePause");
        yield return Until(()=>Get(observerSim,"State").ToString()=="Running","Resume was not replicated.");
        Call(lab,"ClearRemoteHistory");
        yield return Until(()=>(int)Get(trace,"RecordCount")==0,"Reset did not clear observer history.");
        Assert.AreEqual("Idle",Get(observerSim,"State").ToString());
    }
    [UnityTearDown]
    public IEnumerator Cleanup()
    {
        if(client!=null)client.Shutdown();if(host!=null)host.Shutdown();yield return null;yield return null;
        if(replica!=null)Object.Destroy(replica);if(client!=null)Object.Destroy(client.gameObject);if(host!=null)Object.Destroy(host.gameObject);
        yield return null;
        var scene=SceneManager.GetSceneByPath(Path);if(scene.IsValid()&&scene.isLoaded)yield return SceneManager.UnloadSceneAsync(scene);
    }
}
