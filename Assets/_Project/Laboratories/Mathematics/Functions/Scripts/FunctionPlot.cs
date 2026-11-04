using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Tecaverso.Labs.Functions
{
    /// <summary>Only data geometry is redrawn. The room, labels, UI and XR objects are authored scene assets.</summary>
    public sealed class FunctionPlot : MonoBehaviour
    {
        public MeshFilter curve, referenceCurve, projections;
        public GameObject grid;
        public FunctionMarker marker;
        public TMP_Text markerLabel;
        public Transform[] featureMarkers;
        public TMP_Text[] featureLabels;
        Mesh currentMesh,referenceMesh,projectionMesh;
        public void Draw(FunctionSession state,bool showGrid,bool features)
        {
            grid.SetActive(showGrid);
            DrawPolynomial(curve,ref currentMesh,state.Current,false);
            referenceCurve.gameObject.SetActive(state.Reference.HasValue);
            if(state.Reference.HasValue)DrawPolynomial(referenceCurve,ref referenceMesh,state.Reference.Value,true);
            foreach(var t in featureMarkers)t.gameObject.SetActive(false);
            if(features)
            {
                int i=0;foreach(double x in FunctionMath.Roots(state.Current))
                    if(FunctionMath.Visible(x,0))Feature(i++,new Vector2((float)x,0),"Raiz ("+FunctionsLab.Number(x)+"; 0)");
                if(FunctionMath.Vertex(state.Current,out var v)&&FunctionMath.Visible(v.x,v.y))Feature(2,v,"Vértice ("+FunctionsLab.Number(v.x)+"; "+FunctionsLab.Number(v.y)+")");
            }
            DrawMarker(state);
        }
        void Feature(int i,Vector2 p,string label)
        {
            featureMarkers[i].gameObject.SetActive(true);
            featureMarkers[i].localPosition=new Vector3(p.x*.5f,p.y*.5f,-.07f);
            featureLabels[i].text=label;
            // Stagger equal/double roots and the vertex; keep text inside the viewport horizontally.
            float dy=i==2?.30f:i==0?-.24f:-.48f;
            float labelY=Mathf.Clamp(p.y*.5f+dy,-1.7f,1.7f);
            featureLabels[i].transform.parent.localPosition=new Vector3(Mathf.Clamp(p.x*.5f,-2.3f,2.3f)-p.x*.5f,labelY-p.y*.5f,-.06f);
        }
        public void DrawMarker(FunctionSession state)
        {
            double x=state.MarkerX,y=state.Current.Evaluate(x);bool visible=FunctionMath.Visible(x,y);
            marker.gameObject.SetActive(visible); markerLabel.transform.parent.gameObject.SetActive(visible);
            var lines=new PlotLines();
            if(visible)
            {
                var point=new Vector3((float)x*.5f,(float)y*.5f,-.1f);
                // XRGrabInteractable temporarily unparents held objects. Always place P in the plot's world frame.
                marker.transform.position=transform.TransformPoint(point);marker.transform.rotation=transform.rotation;
                markerLabel.text="P ("+FunctionsLab.Number(x)+"; "+FunctionsLab.Number(y)+")";
                // Put the coordinate card beside P, away from the vertex card above it.
                float labelX=point.x>1.5f?point.x-.85f:point.x+.85f;
                markerLabel.transform.parent.localPosition=new Vector3(Mathf.Clamp(labelX,-2.3f,2.3f),Mathf.Clamp(point.y+.12f,-1.7f,1.8f),-.14f);
                lines.Line(new Vector2(point.x,0),point,.008f,true);
                lines.Line(new Vector2(0,point.y),point,.008f,true);
            }
            Assign(projections,ref projectionMesh,lines);
        }
        public static PlotLines PolynomialLines(Polynomial p,bool dashed)
        {
            var lines=new PlotLines();
            foreach(var range in FunctionMath.Intervals(p))
            {
                if(Mathf.Abs(range.y-range.x)<.00001f)
                {
                    var point=new Vector2(range.x*.5f,(float)p.Evaluate(range.x)*.5f);
                    lines.Line(point-Vector2.right*.015f,point+Vector2.right*.015f,.027f);continue;
                }
                int n=Math.Max(2,(int)Math.Ceiling((range.y-range.x)/.025));
                for(int i=0;i<n;i++)
                {
                    double x=range.x+(range.y-range.x)*i/n, x1=range.x+(range.y-range.x)*(i+1)/n;
                    // Fixed x dashes make the reference visibly distinct from the active solid curve.
                    if(!dashed || i%8<5)lines.Line(new Vector2((float)x*.5f,(float)p.Evaluate(x)*.5f),new Vector2((float)x1*.5f,(float)p.Evaluate(x1)*.5f),dashed?.016f:.027f);
                }
                foreach(float x in new[]{range.x,range.y})
                {
                    var p0=new Vector2(x*.5f,(float)p.Evaluate(x)*.5f);
                    float sign=x==range.x?-1:1;
                    var direction=new Vector2(sign,(float)(2*p.A*x+p.B)*sign).normalized;
                    lines.Arrow(p0,direction,.075f);
                }
            }
            return lines;
        }
        static void DrawPolynomial(MeshFilter filter,ref Mesh mesh,Polynomial p,bool dashed)=>Assign(filter,ref mesh,PolynomialLines(p,dashed));
        static void Assign(MeshFilter filter,ref Mesh mesh,PlotLines lines)
        { if(!mesh){mesh=new Mesh{name="Function dynamic geometry"};mesh.MarkDynamic();filter.sharedMesh=mesh;}lines.Fill(mesh); }
        void OnDestroy(){if(currentMesh)Destroy(currentMesh);if(referenceMesh)Destroy(referenceMesh);if(projectionMesh)Destroy(projectionMesh);}
    }

    public sealed class PlotLines
    {
        readonly List<Vector3> vertices=new();readonly List<int> triangles=new();
        public void Line(Vector2 a,Vector2 b,float width,bool dashed=false)
        {
            if(Vector2.Distance(a,b)<.00001f)return;
            if(dashed){int n=Mathf.Max(1,Mathf.CeilToInt(Vector2.Distance(a,b)/.09f));for(int i=0;i<n;i++)Line(Vector2.Lerp(a,b,(float)i/n),Vector2.Lerp(a,b,(i+.58f)/n),width);return;}
            Vector2 d=(b-a).normalized;Vector2 side=new Vector2(-d.y,d.x)*width/2;
            int k=vertices.Count;vertices.Add(a-side);vertices.Add(a+side);vertices.Add(b+side);vertices.Add(b-side);
            triangles.AddRange(new[]{k,k+1,k+2,k,k+2,k+3,k,k+2,k+1,k,k+3,k+2});
        }
        public void Arrow(Vector2 tip,Vector2 direction,float size)
        {
            var side=new Vector2(-direction.y,direction.x)*size*.45f;int k=vertices.Count;
            vertices.Add(tip);vertices.Add(tip-direction*size+side);vertices.Add(tip-direction*size-side);
            triangles.AddRange(new[]{k,k+1,k+2,k,k+2,k+1});
        }
        public void Fill(Mesh m){m.Clear();m.SetVertices(vertices);m.SetTriangles(triangles,0);m.RecalculateNormals();m.RecalculateBounds();}
    }
}
