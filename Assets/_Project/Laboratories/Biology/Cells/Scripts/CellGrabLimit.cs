using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

namespace Tecaverso.Labs.Biology.Cells
{
    // Constrain the XR target before physics movement, including near/far ray grabs.
    public sealed class CellGrabLimit : XRBaseGrabTransformer
    {
        public Transform home;
        public bool detachable;
        protected override RegistrationMode registrationMode => RegistrationMode.Single;
        public override void Process(XRGrabInteractable grab, XRInteractionUpdateOrder.UpdatePhase phase, ref Pose pose, ref Vector3 scale)
        {
            if (!home) return;
            pose.position = detachable ? home.position + Vector3.ClampMagnitude(pose.position - home.position, .4f) : home.position;
            if (!detachable) pose.rotation = home.rotation;
            scale = grab.transform.localScale;
        }
    }
}
