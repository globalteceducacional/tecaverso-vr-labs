using System.Collections.Generic;
using Tecaverso.Interaction;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Tecaverso.Labs.Biology.Anatomy
{
    public sealed class AnatomyOrgan : XRSimpleInteractable
    {
        public AnatomyLab lab;
        public int system;
        public string title;
        [TextArea] public string function, context;
        public Renderer[] surfaces;

        // One canonical source mesh identifies each removable unit, including the
        // multi-mesh brain, larynx and esophagus. Distributed networks remain in situ.
        static readonly HashSet<string> LiftableMeshes = new()
        {
            "Larynx_External", "Trachea", "Lung_Left", "Lung_Right", "Diaphragm",
            "Heart", "Esophagus_Torso", "Stomach", "Small_Inestines", "Colon", "Liver",
            "Pancreas", "Gallbladder", "Lobe_Parietal_Right", "Skeleton_Skull",
            "Skeleton_Pelvis", "Skeleton_Rib_Cage", "Kidney_Left", "Kidney_Right",
            "Urinary_Ureter_Left", "Urinary_Ureter_Right", "Bladder", "Spleen", "Thymus"
        };
        const float PullThreshold = .02f;
        const float ReturnDuration = .30f;

        public bool IsLiftable
        {
            get
            {
                if (surfaces == null) return false;
                foreach (var surface in surfaces)
                    if (surface && LiftableMeshes.Contains(surface.name)) return true;
                return false;
            }
        }

        public bool IsDisplaced { get; private set; }
        public Bounds WorldBounds
        {
            get
            {
                var bounds = surfaces[0].bounds;
                for (int i = 1; i < surfaces.Length; i++) bounds.Encapsulate(surfaces[i].bounds);
                return bounds;
            }
        }

        Transform gripPoint, hand;
        Vector3 handStart, contactStart, restPosition, returnContact;
        Quaternion handStartRotation, objectStartRotation, restRotation, returnRotation;
        bool returning;
        float returnElapsed;

        bool Available => lab && lab.ActiveSystem == system && (!lab.InspectionOrgan || lab.InspectionOrgan == this);
        public override bool IsHoverableBy(IXRHoverInteractor interactor) =>
            Available && !(interactor is XRSocketInteractor) && base.IsHoverableBy(interactor);
        public override bool IsSelectableBy(IXRSelectInteractor interactor) =>
            Available && !(interactor is XRSocketInteractor) && (!isSelected || interactorsSelecting.Contains(interactor))
            && base.IsSelectableBy(interactor);

        // Imported meshes share the body pivot. The XR ray must attach to the
        // surface touched by the student, even when that pivot is at the feet.
        public override Transform GetAttachTransform(IXRInteractor interactor) =>
            gripPoint ? gripPoint : base.GetAttachTransform(interactor);

        protected override void OnSelectEntering(SelectEnterEventArgs args)
        {
            if (!(args.interactorObject is XRSocketInteractor))
            {
                var anchor = args.interactorObject.GetAttachTransform(this);
                if (!gripPoint)
                {
                    gripPoint = new GameObject("Ponto de pega XR").transform;
                    gripPoint.SetParent(transform, false);
                }
                var bounds = WorldBounds;
                gripPoint.position = bounds.ClosestPoint(anchor.position);
                if (args.interactorObject is XRRayInteractor ray &&
                    ray.TryGetCurrent3DRaycastHit(out var hit) && colliders.Contains(hit.collider))
                    gripPoint.position = hit.point;

                if (IsLiftable)
                {
                    if (!IsDisplaced) transform.GetLocalPositionAndRotation(out restPosition, out restRotation);
                    returning = false;
                    hand = anchor;
                    handStart = hand.position;
                    handStartRotation = hand.rotation;
                    contactStart = gripPoint.position;
                    objectStartRotation = transform.rotation;
                }
            }
            base.OnSelectEntering(args);
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            if (!(args.interactorObject is XRSocketInteractor)) lab.Inspect(this);
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            hand = null;
            if (!IsDisplaced) return;
            if (args.isCanceled || MotionPreferences.Reduced || !isActiveAndEnabled)
            {
                Restore();
                return;
            }
            returning = true;
            returnElapsed = 0f;
            returnContact = transform.parent.InverseTransformPoint(gripPoint.position);
            returnRotation = transform.localRotation;
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase phase)
        {
            base.ProcessInteractable(phase);
            if (phase != XRInteractionUpdateOrder.UpdatePhase.Dynamic) return;
            if (returning)
            {
                returnElapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(returnElapsed / ReturnDuration);
                if (t >= 1f || MotionPreferences.Reduced) { Restore(); return; }
                float eased = Mathf.SmoothStep(0f, 1f, t);
                var rotation = Quaternion.Slerp(returnRotation, restRotation, eased);
                // Interpolate the contact point rather than the distant FBX pivot.
                // Everything here is relative to the rotating body.
                var offset = Vector3.Scale(transform.localScale, gripPoint.localPosition);
                var homeContact = restPosition + restRotation * offset;
                var contact = Vector3.Lerp(returnContact, homeContact, eased);
                transform.SetLocalPositionAndRotation(contact - rotation * offset, rotation);
                return;
            }
            if (!hand || !isSelected) return;
            var delta = hand.position - handStart;
            if (!IsDisplaced)
            {
                if (delta.sqrMagnitude < PullThreshold * PullThreshold || !lab.BeginInspection(this)) return;
                IsDisplaced = true;
            }
            transform.rotation = hand.rotation * Quaternion.Inverse(handStartRotation) * objectStartRotation;
            transform.position += contactStart + delta - gripPoint.position;
            // No gravity, ejection or collision impulses between anatomical pieces.
            float floorPenetration = lab.bodyPivot.position.y + .005f - WorldBounds.min.y;
            if (floorPenetration > 0f) transform.position += Vector3.up * floorPenetration;
        }

        void Restore()
        {
            if (!IsDisplaced) return;
            transform.SetLocalPositionAndRotation(restPosition, restRotation);
            IsDisplaced = returning = false;
            if (lab) lab.EndInspection(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            hand = null;
            Restore();
        }
    }
}
