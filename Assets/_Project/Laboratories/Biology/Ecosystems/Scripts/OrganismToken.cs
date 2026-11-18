using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Tecaverso.Labs.Biology.Ecosystems
{
    public sealed class OrganismToken : XRGrabInteractable
    {
        public EcosystemsLab lab;
        public Transform home;
        public int role;
        public bool Docked { get; private set; }
        public bool Held => isSelected && interactorsSelecting.Exists(i=>i is not XRSocketInteractor);
        bool pendingRelease;
        int lastApproach=-1;

        public override bool IsHoverableBy(IXRHoverInteractor interactor) =>
            lab && !lab.Modal && base.IsHoverableBy(interactor);

        public override bool IsSelectableBy(IXRSelectInteractor interactor) =>
            lab && !lab.Modal && base.IsSelectableBy(interactor);

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            if(args.interactorObject is XRSocketInteractor)return;
            pendingRelease=false; lastApproach=-1; Docked=false;
            lab.PickedUp(this);
        }
        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            if(args.interactorObject is not XRSocketInteractor)pendingRelease=true;
        }
        void LateUpdate()
        {
            if(Held)
            {
                int candidate=lab.NearSocket(transform.position);
                if(candidate!=lastApproach && candidate>=0)lab.Preview(this,candidate);
                lastApproach=candidate;
            }
            else if(pendingRelease)
            {
                pendingRelease=false;
                lab.Release(this);
            }
        }
        public void Place(bool dock)
        {
            Docked=dock; pendingRelease=false; lastApproach=-1;
            var anchor=dock?lab.sockets[role].attachTransform:home;
            var rb=GetComponent<Rigidbody>();
            rb.position=anchor.position; rb.rotation=anchor.rotation;
            transform.SetPositionAndRotation(anchor.position,anchor.rotation);
        }
    }
}
