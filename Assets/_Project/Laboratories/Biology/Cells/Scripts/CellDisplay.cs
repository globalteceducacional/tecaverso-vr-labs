using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using Tecaverso.Interaction;

namespace Tecaverso.Labs.Biology.Cells
{
    public sealed class CellDisplay : MonoBehaviour
    {
        public CellsLab lab;
        public Transform pivot;
        public XRKnob ring;
        float desired;
        void OnEnable() { ring.onValueChange.AddListener(Rotate); }
        void OnDisable() { if (ring) ring.onValueChange.RemoveListener(Rotate); }
        void Rotate(float value) { desired = value * 360; }
        void Update()
        {
            bool blocked = lab.Modal || lab.Inspecting || lab.HasLoose || lab.HasHeld;
            if (ring.enabled == blocked) ring.enabled = !blocked;
            if (blocked) return;
            var goal = Quaternion.Euler(0, desired, 0);
            pivot.localRotation = MotionPreferences.Reduced ? goal : Quaternion.Slerp(pivot.localRotation, goal, 1 - Mathf.Exp(-12 * Time.unscaledDeltaTime));
        }
        public void Restore() { desired = 0; ring.value = 0; pivot.localRotation = Quaternion.identity; }
    }
}
