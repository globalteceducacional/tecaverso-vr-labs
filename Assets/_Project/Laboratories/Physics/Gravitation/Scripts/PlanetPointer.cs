using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Tecaverso.Labs.Gravitation
{
    [RequireComponent(typeof(XRSimpleInteractable))]
    public sealed class PlanetPointer : MonoBehaviour
    {
        public PlanetInspection inspection;
        public int index;
        XRSimpleInteractable target;
        void OnEnable()
        {
            target=GetComponent<XRSimpleInteractable>();
            target.hoverEntered.AddListener(Enter);
            target.hoverExited.AddListener(Exit);
            target.selectEntered.AddListener(Select);
        }
        void OnDisable()
        {
            target.hoverEntered.RemoveListener(Enter);
            target.hoverExited.RemoveListener(Exit);
            target.selectEntered.RemoveListener(Select);
            if(inspection!=null)inspection.HideHover(index);
        }
        void Enter(HoverEnterEventArgs _) { if(inspection!=null)inspection.ShowHover(index); }
        void Exit(HoverExitEventArgs _) { if(!target.isHovered&&inspection!=null)inspection.HideHover(index); }
        void Select(SelectEnterEventArgs _) { if(inspection!=null)inspection.Focus(index); }
    }
}
