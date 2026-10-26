using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Tecaverso.Labs.Thermology
{
    public sealed class ThermalLoad : XRGrabInteractable
    {
        public ThermologyLab lab;
        public float pressureKpa = 100;
        Vector3 homePosition;
        Quaternion homeRotation;
        bool homeReady;
        protected override void Awake() { base.Awake(); homePosition=transform.position; homeRotation=transform.rotation; homeReady=true; }
        public override bool IsSelectableBy(IXRSelectInteractor interactor)
            => lab != null && (interactor is ThermalLoadSocket || lab.CanConfigureLoad) && base.IsSelectableBy(interactor);
        public void ReturnHome()
        {
            if(!homeReady)return;
            transform.SetPositionAndRotation(homePosition,homeRotation);
            var body=GetComponent<Rigidbody>();if(!body.isKinematic){body.linearVelocity=Vector3.zero;body.angularVelocity=Vector3.zero;}
        }
    }
}
