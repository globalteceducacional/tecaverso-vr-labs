using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Tecaverso.Labs.Biology.Ecosystems
{
    public sealed class TrophicSocket : XRSocketInteractor
    {
        public OrganismToken target;
        bool accepting;

        public override bool CanHover(IXRHoverInteractable item) =>
            ReferenceEquals(item,target) && target && !target.lab.Modal && base.CanHover(item);

        public override bool CanSelect(IXRSelectInteractable item) =>
            accepting && ReferenceEquals(item,target) && base.CanSelect(item);

        public void Seat(OrganismToken piece)
        {
            if(piece!=target || piece.Held || !interactionManager)return;
            accepting=true;
            try
            {
                interactionManager.SelectEnter((IXRSelectInteractor)this,(IXRSelectInteractable)piece);
                interactionManager.SelectExit((IXRSelectInteractor)this,(IXRSelectInteractable)piece);
            }
            finally { accepting=false; }
        }
    }
}
