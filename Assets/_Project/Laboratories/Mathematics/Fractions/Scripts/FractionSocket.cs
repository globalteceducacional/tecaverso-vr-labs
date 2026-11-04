using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
namespace Tecaverso.Labs.Fractions
{
    // The coordinator requests selection only on release, after exact capacity/type checks.
    // A socket never steals an object from a hand or selects an already mounted piece.
    public sealed class FractionSocket:XRSocketInteractor
    {
        [System.NonSerialized] public FractionPiece candidate;
        public override bool CanSelect(IXRSelectInteractable interactable)=>candidate!=null&&ReferenceEquals(interactable,candidate)&&!candidate.Held&&base.CanSelect(interactable);
    }
}
