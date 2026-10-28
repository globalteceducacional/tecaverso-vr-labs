using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Tecaverso.Labs.Cartesian
{
    public sealed class CartesianGrab : XRGrabInteractable
    {
        public CartesianLab lab;public int id;public bool construction;
        Vector3 previous;bool dragging;
        public override bool IsSelectableBy(IXRSelectInteractor interactor)=>lab!=null&&!lab.Modal&&!lab.BusyOther(this)&&base.IsSelectableBy(interactor);
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);lab.Select(id,construction);dragging=lab.BeginDrag(this);trackPosition=dragging;previous=transform.position;
        }
        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase phase)
        {
            base.ProcessInteractable(phase);if(phase!=XRInteractionUpdateOrder.UpdatePhase.Late||!dragging||!isSelected)return;
            if(construction){var c=lab.Model.Construction(id);if(c!=null)lab.Model.Translate(c.points,(transform.position-previous)/lab.Unit);}
            else lab.Model.Move(id,CartesianModel.Magnetize(lab.Coordinate(transform.position),lab.Snap));
            lab.Sync();previous=construction?lab.ConstructionCenter(id):lab.World(lab.Model.Point(id).position);transform.position=previous;
        }
        protected override void OnSelectExited(SelectExitEventArgs args){base.OnSelectExited(args);if(dragging)lab.EndDrag(this);dragging=false;trackPosition=true;}
        protected override void OnHoverEntered(HoverEnterEventArgs args){base.OnHoverEntered(args);lab?.Hover(id,construction,true);}
        protected override void OnHoverExited(HoverExitEventArgs args){base.OnHoverExited(args);lab?.Hover(id,construction,false);}
    }
}
