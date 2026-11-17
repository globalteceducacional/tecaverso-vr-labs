using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Tecaverso.Labs.Biology.Cells
{
    // Identity filter: a socket never steals a piece from a hand or accepts another organelle.
    public sealed class CellReturnSocket : XRSocketInteractor
    {
        public CellStructure target;
        [System.NonSerialized] public bool capture;
        public override bool CanHover(IXRHoverInteractable item) =>
            target && ReferenceEquals(item, target) && target.detachable && base.CanHover(item);
        public override bool CanSelect(IXRSelectInteractable item) =>
            capture && target && ReferenceEquals(item, target) && !target.Held && base.CanSelect(item);
    }
}
