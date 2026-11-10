using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
namespace Tecaverso.Interaction
{
    // A local presentation frame: no Rigidbody, collider, domain value or scientific mesh is animated.
    [DisallowMultipleComponent,RequireComponent(typeof(XRBaseInteractable))]
    public sealed class XRInteractionMotion : MonoBehaviour
    {
        XRBaseInteractable xr;LineRenderer line;float amount,pulse;bool hover,selected;static Material material;
        readonly Vector3[] points=new Vector3[16];
        static readonly int[] order={0,1,3,2,0,4,5,1,5,7,3,7,6,2,6,4};
        void Awake()
        {
            xr=GetComponent<XRBaseInteractable>();xr.firstHoverEntered.AddListener(Enter);xr.lastHoverExited.AddListener(Exit);xr.firstSelectEntered.AddListener(Select);xr.lastSelectExited.AddListener(Release);
            if(!material){material=new Material(Shader.Find("Universal Render Pipeline/Unlit"));material.SetColor("_BaseColor",new Color(.18f,.65f,1));}
            var go=new GameObject("XR motion outline");go.transform.SetParent(transform,false);line=go.AddComponent<LineRenderer>();line.sharedMaterial=material;line.positionCount=16;line.useWorldSpace=true;line.loop=false;line.enabled=false;
            line.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;line.receiveShadows=false;
        }
        void Enter(HoverEnterEventArgs _){hover=true;}void Exit(HoverExitEventArgs _){hover=false;}
        void Select(SelectEnterEventArgs _){selected=true;pulse=.22f;}void Release(SelectExitEventArgs _){selected=false;pulse=.22f;}
        void LateUpdate()
        {
            pulse=Mathf.Max(0,pulse-Time.unscaledDeltaTime);float target=hover||selected||pulse>0?1:0;
            amount=MotionPreferences.Reduced?target:Mathf.MoveTowards(amount,target,Time.unscaledDeltaTime/.12f);
            line.enabled=amount>.001f;if(!line.enabled)return;
            Bounds bounds=new Bounds(transform.position,Vector3.zero);bool found=false;
            foreach(var collider in xr.colliders){if(!collider||!collider.enabled)continue;if(!found){bounds=collider.bounds;found=true;}else bounds.Encapsulate(collider.bounds);}
            if(!found){line.enabled=false;return;}
            float grow=MotionPreferences.Reduced?.008f:.008f+Mathf.Sin(pulse/.22f*Mathf.PI)*.012f;
            bounds.Expand(grow);var min=bounds.min;var max=bounds.max;
            for(int i=0;i<16;i++){int k=order[i];points[i]=new Vector3((k&1)==0?min.x:max.x,(k&2)==0?min.y:max.y,(k&4)==0?min.z:max.z);}
            line.SetPositions(points);line.widthMultiplier=.004f*amount;
        }
        void OnDisable(){hover=selected=false;amount=pulse=0;if(line)line.enabled=false;}
    }
}
