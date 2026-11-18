using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Tecaverso.Labs.Biology.Dna
{
    public sealed class DnaSocket : XRSocketInteractor
    {
        public DnaLab lab;
        public int index;
        DnaBase accepting;

        public override bool CanHover(IXRHoverInteractable item) =>
            item is DnaBase piece && piece.lab == lab && !lab.Modal && base.CanHover(item);
        public override bool CanSelect(IXRSelectInteractable item) =>
            accepting != null && ReferenceEquals(item, accepting) && !lab.Modal && base.CanSelect(item);

        // The socket never steals from the hand. After release it registers the
        // valid docking gesture; the base then stays kinematic and freely re-grabbable.
        public void Seat(DnaBase piece)
        {
            if (!interactionManager) return;
            accepting = piece;
            try
            {
                interactionManager.SelectEnter((IXRSelectInteractor)this, (IXRSelectInteractable)piece);
                interactionManager.SelectExit((IXRSelectInteractor)this, (IXRSelectInteractable)piece);
            }
            finally { accepting = null; }
        }
    }
}
