using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
namespace Tecaverso.Labs.Waves
{
    public sealed class WaveSlide:XRGrabInteractable
    {
        public WavesLab lab;
        public bool pulse;
        public float coordinate;
        Vector3 origin;
        protected override void Awake(){base.Awake();origin=transform.localPosition;}
        public override bool IsSelectableBy(IXRSelectInteractor interactor)=>lab!=null&&!lab.Modal&&(!pulse||lab.CanDrive&&lab.Mode==0)&&base.IsSelectableBy(interactor);
        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase phase)
        {
            base.ProcessInteractable(phase);
            if(lab==null)return;
            if(isSelected)
            {
                coordinate=pulse?Mathf.Clamp(transform.localPosition.y-origin.y,-.30f,.30f):Mathf.Clamp(transform.localPosition.x,0,6);
                if(pulse)lab.Pull(coordinate);
            }
            else if(pulse)coordinate=lab.DriverDisplacement;
            transform.localPosition=pulse?origin+Vector3.up*coordinate:new Vector3(coordinate,origin.y,origin.z);
            transform.localRotation=Quaternion.identity;
        }
    }
}
