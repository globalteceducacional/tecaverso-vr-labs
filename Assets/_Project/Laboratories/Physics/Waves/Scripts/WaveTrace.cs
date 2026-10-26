using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Tecaverso.Labs.Waves
{
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class WaveTrace:MaskableGraphic
    {
        readonly List<Vector2> samples=new();
        public float Range {get;private set;}=.35f;
        public void Clear(){samples.Clear();SetVerticesDirty();}
        public void Record(float time,float displacement)
        {
            if(samples.Count>0&&time-samples[^1].x<.025f)return;
            samples.Add(new Vector2(time,displacement));while(samples.Count>1&&time-samples[0].x>8)samples.RemoveAt(0);
            Range=.35f;foreach(var s in samples)Range=Mathf.Max(Range,Mathf.Abs(s.y)*1.1f);SetVerticesDirty();
        }
        static void Line(VertexHelper v,Vector2 a,Vector2 b,float width,Color color)
        {
            var d=(b-a).normalized;var n=new Vector2(-d.y,d.x)*width*.5f;int i=v.currentVertCount;
            v.AddVert(a-n,color,Vector2.zero);v.AddVert(a+n,color,Vector2.zero);v.AddVert(b+n,color,Vector2.zero);v.AddVert(b-n,color,Vector2.zero);v.AddTriangle(i,i+1,i+2);v.AddTriangle(i,i+2,i+3);
        }
        protected override void OnPopulateMesh(VertexHelper v)
        {
            v.Clear();var r=rectTransform.rect;var grid=new Color(.15f,.3f,.5f,.25f);
            for(int i=0;i<=8;i++)Line(v,new Vector2(r.xMin+i*r.width/8,r.yMin),new Vector2(r.xMin+i*r.width/8,r.yMax),1,grid);
            Line(v,new Vector2(r.xMin,0),new Vector2(r.xMax,0),2,Color.gray);
            if(samples.Count<2)return;float end=samples[^1].x;
            Vector2 Map(Vector2 p)=>new Vector2(r.xMax+(p.x-end)*r.width/8,p.y/Range*r.height*.5f);
            for(int i=1;i<samples.Count;i++)Line(v,Map(samples[i-1]),Map(samples[i]),3,new Color(.15f,.31f,.63f));
        }
    }
}
