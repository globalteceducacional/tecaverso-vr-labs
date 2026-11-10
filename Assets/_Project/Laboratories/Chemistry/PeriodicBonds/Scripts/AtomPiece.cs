using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Tecaverso.Labs.Chemistry.PeriodicBonds
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public sealed class AtomPiece : MonoBehaviour
    {
        public ElementKind element;
        public PeriodicBondsLab lab;
        public int dockIndex = -1;
        XRGrabInteractable grab;
        bool used;
        void Awake() => grab = GetComponent<XRGrabInteractable>();
        void OnEnable() => grab.selectEntered.AddListener(OnGrab);
        void OnDisable() => grab.selectEntered.RemoveListener(OnGrab);
        void OnGrab(SelectEnterEventArgs args)
        {
            if (!used) { used = true; lab.Restock(element); }
        }
    }
}
