using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
namespace Tecaverso.Labs.Fractions
{
    public sealed class FractionPiece:XRGrabInteractable
    {
        public FractionsLab lab;
        public int kind;
        public GameObject divisions;
        [System.NonSerialized] public bool supply;
        [System.NonSerialized] public int dock=-1;
        [System.NonSerialized] public Vector3 destination;
        [System.NonSerialized] public Quaternion orientation;
        bool releasePending;
        float abandoned;
        public bool Held=>isSelected && interactorsSelecting.Exists(i=>!(i is XRSocketInteractor));
        public override bool IsSelectableBy(IXRSelectInteractor interactor)=>lab!=null&&!lab.Modal&&(!supply||lab.LooseCount<6)&&base.IsSelectableBy(interactor);
        protected override void OnSelectEntered(SelectEnterEventArgs args){base.OnSelectEntered(args);if(args.interactorObject is XRSocketInteractor)return;releasePending=false;abandoned=0;lab.Take(this);}
        protected override void OnSelectExited(SelectExitEventArgs args){base.OnSelectExited(args);if(!(args.interactorObject is XRSocketInteractor)&&isActiveAndEnabled)releasePending=true;}
        void LateUpdate()
        {
            if(!lab||lab.Modal)return;
            if(releasePending&&!Held){releasePending=false;lab.Release(this);}
            if(Held)return;
            if(supply||dock>=0){transform.position=Vector3.MoveTowards(transform.position,destination,Time.deltaTime*3);transform.rotation=Quaternion.RotateTowards(transform.rotation,orientation,Time.deltaTime*600);abandoned=0;}
            else {abandoned+=Time.deltaTime;if(abandoned>8||transform.position.y<.1f)lab.Return(this);}
        }
    }
}
