using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Tecaverso.Labs.ResistorCircuits
{
    public sealed class FlexibleCable : MonoBehaviour
    {
        public CableEnd first, second;
        public LineRenderer sheath, flow;
        public float current;
        const int Samples=20;
        readonly Transform[] markers=new Transform[3];
        void Awake()
        {
            for(int i=0;i<markers.Length;i++)
            {
                var marker=GameObject.CreatePrimitive(PrimitiveType.Sphere);marker.name="Conventional current marker";
                marker.transform.SetParent(transform,false);marker.transform.localScale=Vector3.one*.11f;
                Destroy(marker.GetComponent<Collider>());marker.GetComponent<Renderer>().sharedMaterial=flow.sharedMaterial;
                markers[i]=marker.transform;
            }
        }
        void LateUpdate()
        {
            if(first==null||second==null)return;
            var a=first.transform.position;var b=second.transform.position;
            float slack=Mathf.Max(.12f,Vector3.Distance(a,b)*.16f);
            sheath.positionCount=Samples;flow.positionCount=Samples;
            for(int i=0;i<Samples;i++)
            {
                float t=i/(float)(Samples-1);var p=Vector3.Lerp(a,b,t)+Vector3.down*(Mathf.Sin(t*Mathf.PI)*slack);
                sheath.SetPosition(i,p);flow.SetPosition(i,p);
            }
            flow.enabled=Mathf.Abs(current)>1e-4f;
            for(int i=0;i<markers.Length;i++)
            {
                markers[i].gameObject.SetActive(flow.enabled);
                if(!flow.enabled)continue;
                float t=Mathf.Repeat((i+.5f)/markers.Length+Time.time*Mathf.Sign(current)*Mathf.Clamp(Mathf.Abs(current)*.16f,.08f,.6f),1);
                markers[i].position=Vector3.Lerp(a,b,t)+Vector3.down*(Mathf.Sin(t*Mathf.PI)*slack);
            }
        }
    }
}
