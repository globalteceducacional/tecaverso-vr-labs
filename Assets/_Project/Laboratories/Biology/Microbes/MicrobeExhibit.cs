using Tecaverso.Interaction;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Tecaverso.Labs.Biology.Microbes
{
    public sealed class MicrobeExhibit : XRSimpleInteractable
    {
        public MicrobesLab gallery;
        public string title, category, description, note;
        public Transform turntable;
        public XRKnob dial;
        public GameObject annotation;

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            gallery.Inspect(this);
        }

        void LateUpdate()
        {
            var rotation = Quaternion.Euler(0, dial.value * 360f, 0);
            turntable.localRotation = MotionPreferences.Reduced ? rotation :
                Quaternion.Slerp(turntable.localRotation, rotation, 1 - Mathf.Exp(-18 * Time.unscaledDeltaTime));
        }

        public void Highlight(bool selected)
        {
            InteractionMotionBootstrap.Ensure(this);
            GetComponent<XRInteractionMotion>().SetPersistentSelected(selected);
        }
    }
}
