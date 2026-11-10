using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Tecaverso.Labs.Chemistry.Reactions
{
    public sealed class ReagentSocket : XRSocketInteractor
    {
        public ReactionsBench bench;
        public int slot;
        public override bool CanHover(IXRHoverInteractable interactable)
            =>interactable is ReagentTool t&&!t.spoon&&!t.indicator&&bench&&!bench.Modal&&base.CanHover(interactable);
        public override bool CanSelect(IXRSelectInteractable interactable)
            =>interactable is ReagentTool t&&!t.Held&&bench&&bench.CanDock(slot,t)&&base.CanSelect(interactable);
        protected override void OnHoverEntered(HoverEnterEventArgs args)
        {
            base.OnHoverEntered(args);
            if(args.interactableObject is ReagentTool t)bench.ExplainDock(slot,t);
        }
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {base.OnSelectEntered(args);if(args.interactableObject is ReagentTool t)bench.Dock(slot,t);}
        protected override void OnSelectExited(SelectExitEventArgs args)
        {base.OnSelectExited(args);if(bench)bench.Undock(slot);}
        public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase phase)
        {
            base.ProcessInteractor(phase);
            if(phase!=XRInteractionUpdateOrder.UpdatePhase.Dynamic||!bench||bench.Modal||!socketActive)return;
            foreach(var item in interactablesHovered)if(item is ReagentTool t&&!bench.CanDock(slot,t))bench.ExplainDock(slot,t);
        }
    }
}
