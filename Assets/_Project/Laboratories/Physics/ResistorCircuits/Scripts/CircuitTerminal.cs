using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Tecaverso.Labs.ResistorCircuits
{
    public sealed class CircuitTerminal : MonoBehaviour
    {
        public int id;
        public CircuitLab lab;
        XRSocketInteractor socket;
        void OnEnable(){socket=GetComponent<XRSocketInteractor>();socket.selectEntered.AddListener(Connected);socket.selectExited.AddListener(Disconnected);}
        void OnDisable(){if(socket==null)return;socket.selectEntered.RemoveListener(Connected);socket.selectExited.RemoveListener(Disconnected);}
        void Connected(SelectEnterEventArgs args)
        {
            if(args.interactableObject.transform.TryGetComponent(out CableEnd end))end.terminal=this;
            lab.Recalculate();
        }
        void Disconnected(SelectExitEventArgs args)
        {
            if(args.interactableObject.transform.TryGetComponent(out CableEnd end)&&end.terminal==this)end.terminal=null;
            lab.Recalculate();
        }
    }
}
