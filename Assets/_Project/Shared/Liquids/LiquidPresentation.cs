using System;
using UnityEngine;
using Tecaverso.Interaction;

namespace Tecaverso.Liquids
{
    /// <summary>Adapter for Unity's LiquidWScript graph. Samples approximate equal volumes, not molecules.</summary>
    public sealed class LiquidPresentation : MonoBehaviour
    {
        public Renderer surface;
        public Vector3[] volumeSamples;
        public Bounds localBounds;
        [Range(0,1)] public float fill;
        public Color tint = new Color(.45f,.75f,.88f,1);
        [Range(0,1)] public float opacity=.65f;
        public bool paused;
        MaterialPropertyBlock block;
        float[] heights;
        Vector3 previousPosition;
        Quaternion previousRotation;
        Vector2 impulse;
        float phase, min, range = 1, cut, wx, wz;
        Quaternion renderedRotation;
        Vector3 renderedScale;
        Color renderedTint;
        float renderedFill=-1, renderedX, renderedZ, renderedOpacity;
        public float SurfaceY => transform.position.y + min + cut * range;

        void OnEnable()
        {
            block ??= new MaterialPropertyBlock();
            heights = new float[volumeSamples.Length];
            ResetMotion();
        }
        public void ResetMotion()
        {
            previousPosition=transform.position;previousRotation=transform.rotation;renderedFill=-1;
            impulse=Vector2.zero;phase=wx=wz=0;
        }
        void OnDisable(){ResetMotion();if(surface)surface.SetPropertyBlock(null);}
        void LateUpdate()=>Refresh(Time.deltaTime);
        public void Refresh(float dt)
        {
            if(!surface||volumeSamples.Length==0)return;
            block ??= new MaterialPropertyBlock();
            if(heights==null||heights.Length!=volumeSamples.Length)heights=new float[volumeSamples.Length];
            if(!paused && dt>0 && !MotionPreferences.Reduced)
            {
                var movement=transform.position-previousPosition;
                var turn=transform.rotation*Quaternion.Inverse(previousRotation);
                turn.ToAngleAxis(out float angle,out Vector3 axis);
                if(angle>180)angle-=360;
                var spin=angle<.001f&&angle>-.001f?Vector3.zero:axis*angle;
                impulse*=Mathf.Exp(-4*dt);
                impulse+=new Vector2(movement.x+spin.z*.002f,movement.z-spin.x*.002f)*.5f;
                impulse=Vector2.ClampMagnitude(impulse,.07f);
                phase+=dt*10;wx=impulse.x*Mathf.Sin(phase);wz=impulse.y*Mathf.Sin(phase);
            }
            if(MotionPreferences.Reduced){impulse=Vector2.zero;wx=wz=0;}
            previousPosition=transform.position;previousRotation=transform.rotation;
            if(renderedFill==fill&&renderedTint==tint&&renderedRotation==transform.rotation&&renderedScale==transform.lossyScale&&renderedX==wx&&renderedZ==wz&&renderedOpacity==opacity)return;
            renderedFill=fill;renderedTint=tint;renderedRotation=transform.rotation;renderedScale=transform.lossyScale;renderedX=wx;renderedZ=wz;renderedOpacity=opacity;
            // The graph remaps world Y first, then adds X/Z wobble. Match that exact equation.
            min=float.PositiveInfinity;float max=float.NegativeInfinity;
            for(int i=0;i<8;i++)
            {
                var p=localBounds.center+Vector3.Scale(localBounds.extents,new Vector3((i&1)==0?-1:1,(i&2)==0?-1:1,(i&4)==0?-1:1));
                float y=transform.TransformVector(p).y;min=Mathf.Min(min,y);max=Mathf.Max(max,y);
            }
            range=Mathf.Max(.001f,max-min);
            for(int i=0;i<heights.Length;i++)heights[i]=PlaneValue(transform.TransformPoint(volumeSamples[i]));
            Array.Sort(heights);
            float index=Mathf.Clamp01(fill)*(heights.Length-1);
            int lo=Mathf.FloorToInt(index),hi=Mathf.Min(lo+1,heights.Length-1);
            cut=Mathf.Lerp(heights[lo],heights[hi],index-lo);
            surface.enabled=fill>.0001f;
            block.SetVector("_ObjectHeight",new Vector4(min,max,0,0));
            block.SetFloat("_FillVolume",fill>=.9999f?2:cut);
            block.SetFloat("_WobbleX",wx);block.SetFloat("_WobbleZ",wz);
            block.SetFloat("_Opacity",opacity);
            block.SetColor("_OutsideColor",tint);block.SetColor("_InsideColor",Color.Lerp(tint,Color.white,.25f));
            surface.SetPropertyBlock(block);
        }
        float PlaneValue(Vector3 point){var p=point-transform.position;return (p.y-min)/range+wx*p.x+wz*p.z;}
        public bool ReachesMouth(Vector3 mouth)=>fill>.0001f && PlaneValue(mouth)<=cut+.035f;
    }
}
