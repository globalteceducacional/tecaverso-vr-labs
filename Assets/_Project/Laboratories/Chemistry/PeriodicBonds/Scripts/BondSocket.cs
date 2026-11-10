using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Tecaverso.Labs.Chemistry.PeriodicBonds
{
    public sealed class BondSocket : XRSocketInteractor
    {
        public PeriodicBondsLab lab;
        public int index;
        public override bool CanSelect(IXRSelectInteractable interactable)
        {
            var piece = interactable.transform.GetComponent<AtomPiece>();
            return piece && lab && (piece.dockIndex == index || lab.CanDock(index, piece)) && base.CanSelect(interactable);
        }
    }
}
