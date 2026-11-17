using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Tecaverso.Labs.Biology.Cells
{
    public sealed class CellStructure : XRGrabInteractable
    {
        public CellsLab lab;
        public CellDisplay display;
        public string structureId, title;
        [TextArea] public string function, context;
        public bool detachable;
        public Transform home;
        public CellReturnSocket socket;
        public TMPro.TMP_Text caption;
        public LineRenderer selectionOutline;
        public LineRenderer homeOutline;
        public LineRenderer captionLeader;
        [System.NonSerialized] public bool docked = true;
        bool releasePending;
        float unattended;
        public bool Held => isSelected && interactorsSelecting.Exists(i => !(i is XRSocketInteractor));

        public override bool IsSelectableBy(IXRSelectInteractor interactor) =>
            lab != null && !lab.Modal && (interactor is XRSocketInteractor || !lab.OtherHeld(this)) && base.IsSelectableBy(interactor);

        protected override void Awake()
        {
            base.Awake();
            trackPosition = trackRotation = detachable;
            throwOnDetach = false;
            movementType = MovementType.Kinematic;
        }
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            if (args.interactorObject is XRSocketInteractor) return;
            releasePending = false; unattended = 0;
            if (detachable) docked = false;
            lab.Inspect(this);
        }
        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            if (!(args.interactorObject is XRSocketInteractor)) releasePending = true;
        }
        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase phase)
        {
            base.ProcessInteractable(phase);
            if (phase == XRInteractionUpdateOrder.UpdatePhase.Late && Held && detachable && home)
                transform.position = home.position + Vector3.ClampMagnitude(transform.position - home.position, .4f);
        }
        void LateUpdate()
        {
            if (!lab || !home) return;
            if (captionLeader)
            {
                captionLeader.enabled = caption.gameObject.activeSelf && !lab.Modal;
                if (captionLeader.enabled)
                {
                    var anchor = caption.transform.parent.position;
                    captionLeader.SetPosition(0, anchor + Vector3.right * (anchor.x > display.transform.position.x ? -.19f : .19f));
                    captionLeader.SetPosition(1, colliders.Count > 0 ? colliders[0].bounds.center : transform.position);
                }
            }
            if (homeOutline) homeOutline.enabled = detachable && !docked && !lab.Modal;
            if (releasePending && !Held) { releasePending = false; lab.Release(this); }
            if (Held) return;
            if (docked || !detachable)
            {
                transform.SetPositionAndRotation(home.position, home.rotation);
                unattended = 0;
            }
            else if (!lab.Modal)
            {
                unattended += Time.unscaledDeltaTime;
                if (transform.position.y < .5f || Vector3.Distance(transform.position, home.position) > .45f || unattended > 15)
                    lab.Return(this, true);
            }
        }
        public void RestorePose()
        {
            docked = true; releasePending = false; unattended = 0;
            transform.SetPositionAndRotation(home.position, home.rotation);
            var body = GetComponent<Rigidbody>();
            if (body && !body.isKinematic) { body.linearVelocity = Vector3.zero; body.angularVelocity = Vector3.zero; }
        }
    }
}
