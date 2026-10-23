using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Tecaverso.Labs.ResistorCircuits
{
    public sealed class SwitchHandle : MonoBehaviour
    {
        public CircuitPart part;
        XRSimpleInteractable interactable;
        void OnEnable(){interactable=GetComponent<XRSimpleInteractable>();interactable.selectEntered.AddListener(Select);}
        void OnDisable(){if(interactable!=null)interactable.selectEntered.RemoveListener(Select);}
        void Select(SelectEnterEventArgs _){part.ToggleSwitch();transform.localRotation=Quaternion.Euler(part.switchClosed?-32:32,0,0);}
    }
}
