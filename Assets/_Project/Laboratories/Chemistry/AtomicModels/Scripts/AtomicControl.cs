using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
namespace Tecaverso.Labs.Chemistry.AtomicModels
{
    public sealed class AtomicControl : MonoBehaviour
    {
        public AtomicModelsLab lab;
        public int kind, delta;
        public string action;
        public string exhibitTitle, exhibitBody;
        public Texture2D exhibitImage;
        XRSimpleInteractable interactable;
        bool held;
        float nextRepeat;
        Vector3 rest;
        Transform cap;
        void Awake()
        {
            interactable=GetComponent<XRSimpleInteractable>();
            if(interactable){interactable.selectEntered.AddListener(OnSelect);interactable.selectExited.AddListener(OnRelease);}
            cap=transform.Find("Cap");if(cap)rest=cap.localPosition;
        }
        void OnDestroy(){if(interactable){interactable.selectEntered.RemoveListener(OnSelect);interactable.selectExited.RemoveListener(OnRelease);}}
        void OnSelect(SelectEnterEventArgs _){Activate();held=true;nextRepeat=Time.unscaledTime+.5f;}
        void OnRelease(SelectExitEventArgs _){held=false;}
        void OnDisable(){held=false;if(cap)cap.localPosition=rest;}
        void Update()
        {
            if(held && action=="change" && Time.unscaledTime>=nextRepeat){Activate();nextRepeat=Time.unscaledTime+.18f;}
            if(cap)cap.localPosition=Tecaverso.Interaction.MotionPreferences.Reduced?rest:Vector3.Lerp(cap.localPosition,rest+(held?Vector3.down*.025f:Vector3.zero),Time.unscaledDeltaTime*18f);
        }
        public void Activate()
        {
            if(!lab)return;
            switch(action)
            {
                case "change":lab.Change(kind,delta);break;
                case "view":lab.ToggleView();break;
                case "recenter":lab.Recenter();break;
                case "clear":lab.RequestClear();break;
                case "confirm":lab.ConfirmClear();break;
                case "cancel":lab.CancelClear();break;
                case "close":lab.CloseConsultation();break;
                case "back":lab.Back();break;
                case "exhibit":lab.OpenConsultation(exhibitTitle,exhibitBody,transform,exhibitImage);break;
            }
        }
    }

}

