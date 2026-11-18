using DG.Tweening;
using Tecaverso.Interaction;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Tecaverso.Labs.Biology.Dna
{
    public sealed class DnaBase : XRGrabInteractable
    {
        public DnaLab lab;
        public string symbol;
        public Transform tray, visual;
        public char Letter => symbol[0];
        public bool InUse => held || releasePending;
        bool held, releasePending;
        int nearest = -1;
        Tween settle;

        public override bool IsSelectableBy(IXRSelectInteractor hand) =>
            lab && !lab.Modal && (hand is XRSocketInteractor || lab.CanLift(this)) && base.IsSelectableBy(hand);
        protected override void OnSelectEntered(SelectEnterEventArgs e)
        {
            base.OnSelectEntered(e);
            if (e.interactorObject is XRSocketInteractor) return;
            StopSettle();
            held = true;
            nearest = -1;
            releasePending = false;
            lab.Lift(this);
        }
        protected override void OnSelectExited(SelectExitEventArgs e)
        {
            base.OnSelectExited(e);
            if (e.interactorObject is XRSocketInteractor) return;
            held = false;
            releasePending = !e.isCanceled;
        }
        void LateUpdate()
        {
            if (!lab || lab.Modal) return;
            if (held)
            {
                if (lab.OutsideWorkArea(transform.position) && interactorsSelecting.Count > 0)
                {
                    interactionManager.SelectExit(interactorsSelecting[0], (IXRSelectInteractable)this);
                    return;
                }
                int next = lab.Nearest(transform.position);
                if (next >= 0 && next != nearest) lab.Preview(this, next);
                nearest = next;
            }
            else if (releasePending)
            {
                releasePending = false;
                lab.Release(this);
            }
        }
        public void Anchor(Transform target, bool animate)
        {
            StopSettle();
            Vector3 previous = visual.position;
            transform.SetPositionAndRotation(target.position, target.rotation);
            var body = GetComponent<Rigidbody>();
            body.position = target.position;
            body.rotation = target.rotation;
            if (animate && !MotionPreferences.Reduced && (previous - target.position).sqrMagnitude < .25f)
            {
                visual.position = previous;
                settle = visual.DOLocalMove(Vector3.zero, .14f).SetEase(Ease.OutQuad).SetUpdate(true);
            }
        }
        void StopSettle()
        {
            settle?.Kill();
            if (visual) visual.localPosition = Vector3.zero;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            held = releasePending = false;
            StopSettle();
        }
    }
}
