using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tecaverso.Labs.Thermology
{
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class GasPVGraph : MaskableGraphic
    {
        readonly List<Vector2> history=new();
        IdealGasModel model;
        public void Record(IdealGasModel gas,bool reset=false)
        {
            model=gas;if(reset)history.Clear();
            var p=new Vector2((float)gas.Volume,(float)gas.Pressure);
            if(history.Count==0||(history[^1]-p).sqrMagnitude>.02f){history.Add(p);if(history.Count>256)history.RemoveAt(1);}
            SetVerticesDirty();
        }
        Vector2 Map(float volume,float pressure)=>new Vector2(rectTransform.rect.xMin+volume/50f*rectTransform.rect.width,rectTransform.rect.yMin+pressure/240f*rectTransform.rect.height);
        static void Quad(VertexHelper vh,Vector2 a,Vector2 b,Vector2 c,Vector2 d,Color color)
        {
            int n=vh.currentVertCount;vh.AddVert(a,color,Vector2.zero);vh.AddVert(b,color,Vector2.zero);vh.AddVert(c,color,Vector2.zero);vh.AddVert(d,color,Vector2.zero);
            vh.AddTriangle(n,n+1,n+2);vh.AddTriangle(n,n+2,n+3);
        }
        static void Line(VertexHelper vh,Vector2 a,Vector2 b,float width,Color color)
        {var d=(b-a).normalized;var n=new Vector2(-d.y,d.x)*width*.5f;Quad(vh,a-n,a+n,b+n,b-n,color);}
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();var grid=new Color(.15f,.30f,.52f,.18f);
            for(int i=0;i<=5;i++)Line(vh,Map(i*10,0),Map(i*10,240),1,grid);
            for(int i=0;i<=4;i++)Line(vh,Map(0,i*60),Map(50,i*60),1,grid);
            Line(vh,Map(0,240),Map(0,0),3,new Color(.03f,.20f,.4f));Line(vh,Map(0,0),Map(50,0),3,new Color(.03f,.20f,.4f));
            if(model==null)return;
            if(model.Process!=GasProcess.Isochoric)
            {
                float start=(float)model.InitialVolume,end=(float)model.Volume;
                Color fill=model.Work>=0?new Color(.10f,.48f,.95f,.25f):new Color(.95f,.43f,.15f,.25f);
                for(int i=0;i<40;i++)
                {
                    float x0=Mathf.Lerp(start,end,i/40f),x1=Mathf.Lerp(start,end,(i+1)/40f);
                    Quad(vh,Map(x0,0),Map(x0,(float)model.PressureAtVolume(x0)),Map(x1,(float)model.PressureAtVolume(x1)),Map(x1,0),fill);
                }
            }
            for(int i=1;i<history.Count;i++)Line(vh,Map(history[i-1].x,history[i-1].y),Map(history[i].x,history[i].y),3,new Color(.15f,.31f,.63f));
            var point=Map((float)model.Volume,(float)model.Pressure);Quad(vh,point+new Vector2(-5,-5),point+new Vector2(-5,5),point+new Vector2(5,5),point+new Vector2(5,-5),new Color(.9f,.30f,.08f));
        }
    }
}
