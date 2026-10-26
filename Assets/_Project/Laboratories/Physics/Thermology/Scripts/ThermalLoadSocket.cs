using System.Linq;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Tecaverso.Labs.Thermology
{
    public sealed class ThermalLoadSocket : XRSocketInteractor
    {
        public ThermologyLab lab;
        public override bool CanSelect(IXRSelectInteractable interactable)
            => interactable is ThermalLoad && (interactablesSelected.Contains(interactable) || lab.CanConfigureLoad) && base.CanSelect(interactable);
        public override bool CanHover(IXRHoverInteractable interactable)
            => interactable is ThermalLoad && lab.CanConfigureLoad && base.CanHover(interactable);
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            if(args.interactableObject is ThermalLoad weight) lab.SetLoad(weight.pressureKpa);
        }
        protected override void OnSelectExited(SelectExitEventArgs args)
        { base.OnSelectExited(args); if(lab!=null)lab.SetLoad(100); }
    }
}
