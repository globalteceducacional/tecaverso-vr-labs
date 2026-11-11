using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Tecaverso.Interaction;
namespace Tecaverso.Labs.Chemistry.AtomicModels
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public sealed class AtomicRotateInteractable : MonoBehaviour
    {
        XRGrabInteractable grab;
        IXRSelectInteractor hand;
        Quaternion lastHand;
        Vector3 momentum;
        bool recentering;
        void Awake()
        {
            grab=GetComponent<XRGrabInteractable>();grab.trackPosition=false;grab.trackRotation=false;
            grab.selectEntered.AddListener(Grab);grab.selectExited.AddListener(Release);
        }
        void OnDestroy(){if(grab){grab.selectEntered.RemoveListener(Grab);grab.selectExited.RemoveListener(Release);}}
        void Grab(SelectEnterEventArgs a){recentering=false;hand=a.interactorObject;lastHand=hand.GetAttachTransform(grab).rotation;momentum=Vector3.zero;}
        void Release(SelectExitEventArgs _){hand=null;if(MotionPreferences.Reduced)momentum=Vector3.zero;}
        void Update()
        {
            if(recentering){transform.localRotation=Quaternion.Slerp(transform.localRotation,Quaternion.identity,1-Mathf.Exp(-12*Time.unscaledDeltaTime));if(Quaternion.Angle(transform.localRotation,Quaternion.identity)<.1f){transform.localRotation=Quaternion.identity;recentering=false;}return;}
            if(hand!=null)
            {
                Quaternion now=hand.GetAttachTransform(grab).rotation;
                Quaternion delta=now*Quaternion.Inverse(lastHand);delta.ToAngleAxis(out float angle,out Vector3 axis);
                if(angle>180)angle-=360;
                if(axis.sqrMagnitude>.01f && Mathf.Abs(angle)<50){transform.rotation=delta*transform.rotation;momentum=axis*angle/Mathf.Max(Time.deltaTime,.001f);}
                lastHand=now;
            }
            else if(!MotionPreferences.Reduced && momentum.sqrMagnitude>.01f)
            {transform.Rotate(momentum*Time.deltaTime,Space.World);momentum=Vector3.Lerp(momentum,Vector3.zero,Time.deltaTime*5f);}
        }
        void OnDisable(){hand=null;momentum=Vector3.zero;recentering=false;}
        public void Recenter(){momentum=Vector3.zero;recentering=!MotionPreferences.Reduced;if(!recentering)transform.localRotation=Quaternion.identity;}
    }
}

