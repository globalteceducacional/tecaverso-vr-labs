using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Tecaverso.Labs.Thermology
{
    public sealed class ThermalPiston : XRGrabInteractable
    {
        public ThermologyLab lab;
        public override bool IsSelectableBy(IXRSelectInteractor interactor) => lab != null && lab.CanDragPiston && base.IsSelectableBy(interactor);
        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase phase)
        {
            base.ProcessInteractable(phase);
            if (lab == null) return;
            if (isSelected && lab.CanDragPiston) lab.MovePiston(transform.localPosition.y);
            transform.localPosition = new Vector3(0, lab.GasHeight, 0);
            transform.localRotation = Quaternion.identity;
        }
    }
}
