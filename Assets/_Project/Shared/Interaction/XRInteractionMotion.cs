using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Tecaverso.Interaction
{
    // State only. The URP feature outlines visible geometry without touching its materials or pose.
    [DisallowMultipleComponent,RequireComponent(typeof(XRBaseInteractable))]
    public sealed class XRInteractionMotion : MonoBehaviour
    {
        public Transform visualRoot;
        public Renderer[] visualTargets;
        public bool PersistentSelected { get; private set; }
        public float Amount { get; private set; }
        public IReadOnlyList<Renderer> Surfaces => surfaces;
        public static readonly List<XRInteractionMotion> Instances=new();
        readonly List<Renderer> surfaces=new();
        readonly HashSet<IXRHoverInteractor> pointers=new();
        readonly HashSet<IXRSelectInteractor> hands=new();
        XRBaseInteractable xr;
        bool hovered,held;
        float pulse;
        public Color HighlightColor
        {
            get
            {
                var color=PersistentSelected?new Color(1,.76f,.36f):new Color(.30f,.72f,1);
                color.a=Amount*(held?.38f:PersistentSelected?.92f:.72f);
                if(!MotionPreferences.Reduced)color.a=Mathf.Clamp01(color.a+pulse*.45f*Amount);
                return color;
            }
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]static void ResetInstances()=>Instances.Clear();
        void Awake()
        {
            xr=GetComponent<XRBaseInteractable>();
            xr.hoverEntered.AddListener(Enter);xr.hoverExited.AddListener(Exit);
            xr.selectEntered.AddListener(Select);xr.selectExited.AddListener(Release);
        }
        void OnEnable()
        {
            Instances.Add(this);
            if(!xr || !xr.isActiveAndEnabled)return;
            foreach(var pointer in xr.interactorsHovering)if(!(pointer is XRSocketInteractor))pointers.Add(pointer);
            foreach(var hand in xr.interactorsSelecting)if(!(hand is XRSocketInteractor))hands.Add(hand);
            hovered=pointers.Count>0;held=hands.Count>0;
        }
        void Start()=>RefreshVisuals();
        public void RefreshVisuals()
        {
            surfaces.Clear();if(!xr)xr=GetComponent<XRBaseInteractable>();
            Transform root=visualRoot;
            if(!root)
            {
                if(xr is XRKnob knob)root=knob.handle;
                else if(xr is XRLever lever)root=lever.handle;
            }
            if(!root)root=transform;
            var candidates=visualTargets!=null&&visualTargets.Length>0?visualTargets:root.GetComponentsInChildren<Renderer>(true);
            foreach(var r in candidates)
            {
                if(!r || !(r is MeshRenderer || r is SkinnedMeshRenderer) || r.GetComponent<TMP_Text>())continue;
                if(r.GetComponentInParent<XRBaseInteractable>()!=xr)continue;
                var liquid=r.GetComponentInParent<Tecaverso.Liquids.LiquidPresentation>();
                if(liquid && liquid.surface==r)continue;
                if(r.gameObject.name.IndexOf("label",System.StringComparison.OrdinalIgnoreCase)>=0 || r.gameObject.name.IndexOf("rótulo",System.StringComparison.OrdinalIgnoreCase)>=0)continue;
                if(!surfaces.Contains(r))surfaces.Add(r);
            }
        }
        public void SetPersistentSelected(bool value)
        {
            if(PersistentSelected==value)return;
            PersistentSelected=value;if(value)RefreshVisuals();
        }
        void Enter(HoverEnterEventArgs e)
        {
            if(e.interactorObject is XRSocketInteractor)return;
            pointers.Add(e.interactorObject);hovered=true;RefreshVisuals();
        }
        void Exit(HoverExitEventArgs e){pointers.Remove(e.interactorObject);hovered=pointers.Count>0;}
        void Select(SelectEnterEventArgs e)
        {
            if(e.interactorObject is XRSocketInteractor)return;
            hands.Add(e.interactorObject);held=true;pulse=.22f;RefreshVisuals();
        }
        void Release(SelectExitEventArgs e){hands.Remove(e.interactorObject);held=hands.Count>0;}
        void LateUpdate()
        {
            if(!xr || !xr.enabled){ResetFeedback();return;}
            if(!hovered&&!held&&!PersistentSelected&&Amount<=0)return;
            pulse=Mathf.Max(0,pulse-Time.unscaledDeltaTime);
            float target=hovered||held||PersistentSelected?1:0;
            Amount=MotionPreferences.Reduced?target:Mathf.MoveTowards(Amount,target,Time.unscaledDeltaTime/.12f);
        }
        public void ResetFeedback(){pointers.Clear();hands.Clear();hovered=held=PersistentSelected=false;Amount=pulse=0;}
        void OnDisable(){Instances.Remove(this);ResetFeedback();}
        void OnDestroy()
        {
            if(!xr)return;
            xr.hoverEntered.RemoveListener(Enter);xr.hoverExited.RemoveListener(Exit);
            xr.selectEntered.RemoveListener(Select);xr.selectExited.RemoveListener(Release);
        }
    }
}
