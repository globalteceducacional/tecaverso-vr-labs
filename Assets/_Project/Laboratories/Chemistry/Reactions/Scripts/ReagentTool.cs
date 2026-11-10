using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Tecaverso.Liquids;

namespace Tecaverso.Labs.Chemistry.Reactions
{
    public sealed class ReagentTool : XRGrabInteractable
    {
        public ReactionsLab lab;
        public string reagentId;
        public bool solidPot,indicator;
        public Transform collectionPoint;
        public int source;
        public bool spoon;
        public Transform mouth, home, powderVisual;
        public LiquidPresentation liquid;
        public TMP_Text label;
        public LineRenderer stream;
        public ParticleSystem droplets;
        float abandoned;
        public bool Held=>isSelected&&interactorsSelecting.Exists(i=>!(i is XRSocketInteractor));
        public override bool IsSelectableBy(IXRSelectInteractor interactor)=>lab&&!lab.Modal&&base.IsSelectableBy(interactor);
        void Update()
        {
            if(Application.isPlaying&&lab&&lab.bench){lab.bench.UpdateTool(this,Mathf.Min(Time.deltaTime,.05f));return;}
            if(!Application.isPlaying||!lab||lab.Session==null)return;
            bool flowing=false;float dt=Mathf.Min(Time.deltaTime,.05f);
            if(Held){abandoned=0;}else{abandoned+=dt;if(abandoned>10||transform.position.y<.15f)ReturnHome();}
            if(Held&&!lab.Modal&&!lab.Session.Paused)
            {
                float upright=Vector3.Dot(transform.up,Vector3.up);
                if(spoon && upright>.7f && Vector3.Distance(mouth.position,lab.powderDish.position)<.16f)lab.Session.LoadSpoon();
                if(upright<.4f && (spoon?lab.Session.SpoonGrams>0:liquid.ReachesMouth(mouth.position)))
                {
                    bool hit=lab.ReceiverHit(mouth.position,out Vector3 end);
                    var transfer=spoon?lab.Session.PourPowder(dt*.8,hit):lab.Session.PourLiquid(source,dt*40*Mathf.Clamp01(1-upright),hit);
                    flowing=transfer.Removed>0;
                    if(flowing){lab.Reacted(transfer);DrawStream(end);}
                }
            }
            stream.enabled=flowing&&!spoon;
            var emission=droplets.emission;emission.enabled=flowing;
            if(flowing&&!droplets.isPlaying)droplets.Play();
            if(!flowing&&droplets.isPlaying)droplets.Stop(true,ParticleSystemStopBehavior.StopEmitting);
        }
        public void DrawStream(Vector3 end)
        {
            stream.SetPosition(0,mouth.position);stream.SetPosition(1,end);
            droplets.transform.position=mouth.position;
            droplets.transform.rotation=Quaternion.LookRotation(Vector3.down);
            var main=droplets.main;main.startLifetime=Mathf.Clamp((mouth.position.y-end.y)/1.8f,.05f,1.5f);
        }
        public void ReturnHome()
        {
            if(isSelected||!home)return;
            transform.SetPositionAndRotation(home.position,home.rotation);abandoned=0;
            if(TryGetComponent<Rigidbody>(out var body)){body.position=home.position;body.rotation=home.rotation;body.linearVelocity=Vector3.zero;body.angularVelocity=Vector3.zero;}
            if(liquid)liquid.ResetMotion();
        }
        public void StopEffects(){if(stream)stream.enabled=false;if(droplets)droplets.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);}
        protected override void OnDisable(){StopEffects();base.OnDisable();}
    }
}
