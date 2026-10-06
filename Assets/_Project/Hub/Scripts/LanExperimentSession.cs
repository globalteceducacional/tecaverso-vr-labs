using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tecaverso.Labs.ObliqueLaunch;
using Tecaverso.UI;
using Unity.Netcode;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tecaverso.Hub
{
    /// <summary>Local additive presentation; network objects and the connected XR rig remain in the Hub.
    /// The experiment contains no NetworkObjects: its analytical state is replicated separately.</summary>
    public sealed class LanExperimentSession : MonoBehaviour
    {
        public const string ExperimentPath = "Assets/_Project/Laboratories/Physics/ObliqueLaunch/Scenes/ObliqueLaunch.unity";
        LanLobbyController lobby;
        readonly List<GameObject> suspended = new();
        XROrigin rig;
        Vector3 rigPosition;
        Quaternion rigRotation;
        Scene hubScene;
        bool transitioning;
        ProjectileLanSync replication;
        public bool IsTransitioning => transitioning;
        public void Bind(LanLobbyController value) { lobby=value; lobby.Changed+=Reconcile; Reconcile(); }
        void OnDestroy() { if(lobby!=null) lobby.Changed-=Reconcile; }
        void Reconcile()
        {
            if(transitioning) return;
            bool wanted=lobby.State==LanLobbyController.ConnectionState.Lobby && lobby.Snapshot.experimentActive;
            bool loaded=SceneManager.GetSceneByPath(ExperimentPath).isLoaded;
            if(wanted&&!loaded) StartCoroutine(Enter());
            else if(!wanted&&loaded) StartCoroutine(Exit());
        }
        IEnumerator Enter()
        {
            transitioning=true;
            hubScene=SceneManager.GetActiveScene();
            rig=FindFirstObjectByType<XROrigin>();
            if(rig!=null) { rigPosition=rig.transform.position; rigRotation=rig.transform.rotation; }
            var operation=SceneManager.LoadSceneAsync(ExperimentPath,LoadSceneMode.Additive);
            if(operation==null) { transitioning=false; lobby.ReturnToLobby(); yield break; }
            yield return operation;
            var scene=SceneManager.GetSceneByPath(ExperimentPath);
            var roots=scene.GetRootGameObjects();
            // Keep the already-networked player rig and input devices. Adopt the lab's authored spawn pose.
            foreach(var labRig in roots.SelectMany(r=>r.GetComponentsInChildren<XROrigin>(true)))
            {
                if(rig!=null) rig.transform.SetPositionAndRotation(labRig.transform.position,labRig.transform.rotation);
                labRig.gameObject.SetActive(false);
            }
            foreach(var es in roots.SelectMany(r=>r.GetComponentsInChildren<UnityEngine.EventSystems.EventSystem>(true)))
                es.gameObject.SetActive(false);
            // Explicit presentation-only roots: never disable the transport, avatars, input or voice manager.
            foreach(var root in hubScene.GetRootGameObjects())
                if(root.activeSelf && (root.name=="Environment" || root.name=="Tecaverso Standard Room" ||
                    root.name=="Tecaverso Hub UI" || root.name=="Connection Canvas"))
                { suspended.Add(root); if(root.GetComponent<Canvas>()!=null)UIVisibility.Set(root,false);else root.SetActive(false); }
            SceneManager.SetActiveScene(scene);
            var lab=roots.SelectMany(r=>r.GetComponentsInChildren<ObliqueLaunchLab>(true)).FirstOrDefault();
            if(lab==null) { transitioning=false; lobby.Leave(); Reconcile(); yield break; }
            replication=lab.gameObject.AddComponent<ProjectileLanSync>();
            replication.Bind(lobby,lab);
            LanLabSessionBar.Create(lab.Panel,lobby);
            lobby.ReportLabReady();
            transitioning=false;
            Reconcile(); // A disconnect or return may have arrived during the asynchronous load.
        }
        IEnumerator Exit()
        {
            transitioning=true;
            if(replication!=null) { replication.enabled=false; Destroy(replication); }
            if(hubScene.IsValid()&&hubScene.isLoaded) SceneManager.SetActiveScene(hubScene);
            yield return SceneManager.UnloadSceneAsync(ExperimentPath);
            foreach(var root in suspended) if(root!=null) { if(root.GetComponent<Canvas>()!=null)UIVisibility.Set(root,true);else root.SetActive(true); }
            suspended.Clear();
            if(rig!=null) rig.transform.SetPositionAndRotation(rigPosition,rigRotation);
            transitioning=false;
            Reconcile();
        }
    }
}
