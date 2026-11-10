using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
namespace Tecaverso.Interaction
{
    // Scene entry and XR registration cover runtime-created atoms, points, cables and fraction pieces.
    // Serialized controls/templates are also covered by the editor migration. No per-frame object search.
    public sealed class InteractionMotionBootstrap : MonoBehaviour
    {
        readonly List<XRInteractionManager> managers=new();
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] static void Install(){var go=new GameObject("Shared interaction motion");DontDestroyOnLoad(go);go.AddComponent<InteractionMotionBootstrap>();}
        void Awake(){SceneManager.sceneLoaded+=Loaded;}
        void Loaded(Scene s,LoadSceneMode mode)
        {
            foreach(var root in s.GetRootGameObjects()){
                foreach(var c in root.GetComponentsInChildren<Selectable>(true))Ensure(c);
                foreach(var x in root.GetComponentsInChildren<XRBaseInteractable>(true))Ensure(x);
                foreach(var m in root.GetComponentsInChildren<XRInteractionManager>(true))if(!managers.Contains(m)){managers.Add(m);m.interactableRegistered+=Registered;}
            }
        }
        void Registered(InteractableRegisteredEventArgs e){if(e.interactableObject is XRBaseInteractable x)Ensure(x);}
        public static void Ensure(Selectable c)
        {
            if(c.GetComponent<ControlMotion>()||c.GetComponent<Tecaverso.Labs.ObliqueLaunch.ButtonTweenFeedback>()||c.GetComponent<Tecaverso.Labs.ObliqueLaunch.SliderTweenFeedback>())return;
            c.gameObject.AddComponent<ControlMotion>();
        }
        public static void Ensure(XRBaseInteractable x)
        {
            if(x.GetComponent<XRInteractionMotion>()||x.GetComponent<Tecaverso.Labs.Chemistry.PeriodicBonds.PeriodicElementCube>()||x.GetType().Name.Contains("Teleportation"))return;
            x.gameObject.AddComponent<XRInteractionMotion>();
        }
        void OnDestroy(){SceneManager.sceneLoaded-=Loaded;foreach(var m in managers)if(m)m.interactableRegistered-=Registered;}
    }
}
