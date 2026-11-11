using UnityEngine;
using UnityEngine.XR.Content.Interaction;

namespace Tecaverso.Labs.Chemistry.AtomicModels
{
    [RequireComponent(typeof(XRLever))]
    public sealed class AtomicViewLever : MonoBehaviour
    {
        public AtomicModelsLab lab;
        XRLever lever;
        void Awake(){lever=GetComponent<XRLever>();}
        void OnEnable()
        {
            lever.onLeverActivate.AddListener(ShowCloud);
            lever.onLeverDeactivate.AddListener(ShowShells);
            if(lab)lab.SetView(lever.value);
        }
        void OnDisable(){lever.onLeverActivate.RemoveListener(ShowCloud);lever.onLeverDeactivate.RemoveListener(ShowShells);}
        void ShowCloud(){if(lab)lab.SetView(true);}
        void ShowShells(){if(lab)lab.SetView(false);}
    }
}
