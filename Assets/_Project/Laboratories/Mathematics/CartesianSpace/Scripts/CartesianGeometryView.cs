using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Tecaverso.Labs.ObliqueLaunch;

namespace Tecaverso.Labs.Cartesian
{
    public sealed class CartesianGeometryView : MonoBehaviour
    {
        public CartesianLab lab;public int id;public bool construction;public CartesianGrab handle;public TMP_Text label;
        public CartesianConstruction previewDefinition;
        CartesianConstruction Definition=>previewDefinition??lab.Model.Construction(id);
        readonly List<GameObject> drawn=new();readonly List<Mesh> meshes=new();
        int cursor; int signature=int.MinValue;
        public void Rebuild()
        {
            int hash=System.HashCode.Combine(lab.Unit,lab.origin.position,lab.SelectedPoint,lab.SelectedConstruction,lab.Pinned,lab.ShowPosition,lab.ShowComponents,lab.ShowProjections);
            hash=System.HashCode.Combine(hash,lab.Analysis,lab.IsHighlighted(id,construction));
            if(construction){var c=Definition;if(c!=null)hash=System.HashCode.Combine(hash,c.kind);if(c!=null)foreach(var p in lab.Model.Vertices(c))hash=System.HashCode.Combine(hash,p);}else hash=System.HashCode.Combine(hash,lab.Model.Point(id)?.position);
            if(signature==hash)return;signature=hash;cursor=0;Build();for(int i=cursor;i<drawn.Count;i++)drawn[i].SetActive(false);
        }
        void Build()
        {

            if(!construction)
            {
                var p=lab.Model.Point(id);if(p==null)return;handle.transform.position=lab.World(p.position);
                label.transform.position=handle.transform.position+Vector3.up*.22f;label.text=p.Name+(lab.SelectedPoint==id||lab.Pinned==id?" = "+CartesianLab.V(p.position):"");
                Tint(handle.GetComponent<Renderer>(),lab.IsHighlighted(id,false)?lab.selectedColor:lab.pointColor);
                if(lab.SelectedPoint!=id&&lab.Pinned!=id)return;
                if(lab.ShowPosition)Arrow(Vector3.zero,p.position,lab.selectedColor);
                if(lab.ShowComponents)for(int axis=0;axis<3;axis++){var end=Vector3.zero;end[axis]=p.position[axis];Arrow(Vector3.zero,end,lab.axisColors[axis]);}
                if(lab.ShowProjections)
                {
                    var v=p.position;for(int axis=0;axis<3;axis++){var on=v;on[axis]=0;Dashed(v,on,lab.axisColors[axis]);for(int j=0;j<3;j++)if(j!=axis){var foot=on;foot[j]=0;Dashed(on,foot,lab.axisColors[axis]);}}
                }
                return;
            }
            var c=Definition;if(c==null)return;var points=lab.Model.Vertices(c);var center=points.Aggregate(Vector3.zero,(sum,v)=>sum+v)/points.Length;
            handle.transform.position=lab.World(center);label.transform.position=handle.transform.position+Vector3.up*.20f;
            bool valid=lab.Model.Validate(c.kind,c.points,out _);var color=valid?(lab.IsHighlighted(id,true)?lab.selectedColor:new Color(.35f,.8f,1)):new Color(1,.3f,.3f);
            label.text=(previewDefinition!=null?"PRÉVIA · ":"")+CartesianLab.Kind(c.kind)+" "+(id+1)+(valid?"":" · inválida");Tint(handle.GetComponent<Renderer>(),color);
            if(!valid){for(int i=1;i<points.Length;i++)Dashed(points[0],points[i],color);return;}
            var a=points[0];var b=points[1];
            switch(c.kind)
            {
                case ConstructionKind.Segment:Line(new[]{a,b},color);break;
                case ConstructionKind.Line:case ConstructionKind.Ray:
                    if(CartesianModel.ClipLine(a,b-a,c.kind==ConstructionKind.Ray,out var start,out var end))
                    {Line(new[]{start,end},color);var d=(end-start).normalized;Arrow(end-d*.35f,end,color);if(c.kind==ConstructionKind.Line)Arrow(start+d*.35f,start,color);}break;
                case ConstructionKind.Triangle:Surface(points,new[]{0,1,2},color);Line(new[]{a,b,points[2],a},color);break;
                case ConstructionKind.Plane:
                    var n=Vector3.Cross(b-a,points[2]-a).normalized;var poly=PlanePolygon(a,n);
                    if(poly.Length>=3){var indices=new List<int>();for(int i=1;i<poly.Length-1;i++)indices.AddRange(new[]{0,i,i+1});Surface(poly,indices.ToArray(),color);Line(poly.Concat(new[]{poly[0]}).ToArray(),color);}
                    if(lab.SelectedConstruction==id&&lab.Analysis)Arrow(center,center+n*1.5f,new Color(1,.8f,.3f));break;
                case ConstructionKind.Box:
                    var min=Vector3.Min(a,b);var max=Vector3.Max(a,b);var box=new Vector3[8];for(int i=0;i<8;i++)box[i]=new Vector3((i&1)==0?min.x:max.x,(i&2)==0?min.y:max.y,(i&4)==0?min.z:max.z);
                    Surface(box,new[]{0,2,3,0,3,1,4,5,7,4,7,6,0,1,5,0,5,4,2,6,7,2,7,3,0,4,6,0,6,2,1,3,7,1,7,5},color);
                    for(int i=0;i<8;i++)for(int axis=0;axis<3;axis++)if((i&(1<<axis))==0)Line(new[]{box[i],box[i|(1<<axis)]},color);break;
                case ConstructionKind.Sphere:
                    float radius=Vector3.Distance(a,b);var vs=new List<Vector3>();var ts=new List<int>();const int rings=16,sides=32;
                    for(int r=0;r<=rings;r++)for(int s=0;s<=sides;s++){float theta=Mathf.PI*r/rings,phi=2*Mathf.PI*s/sides;vs.Add(a+radius*new Vector3(Mathf.Sin(theta)*Mathf.Cos(phi),Mathf.Cos(theta),Mathf.Sin(theta)*Mathf.Sin(phi)));}
                    for(int r=0;r<rings;r++)for(int s=0;s<sides;s++){int i=r*(sides+1)+s;ts.AddRange(new[]{i,i+1,i+sides+1,i+1,i+sides+2,i+sides+1});}Surface(vs.ToArray(),ts.ToArray(),color);Line(new[]{a,b},color);
                    for(int axis=0;axis<3;axis++){var circle=new Vector3[65];for(int i=0;i<65;i++){var v=Vector3.zero;v[(axis+1)%3]=Mathf.Cos(i*Mathf.PI/32)*radius;v[(axis+2)%3]=Mathf.Sin(i*Mathf.PI/32)*radius;circle[i]=a+v;}Line(circle,color);}break;
                case ConstructionKind.Tetrahedron:Surface(points,new[]{0,2,1,0,1,3,0,3,2,1,2,3},color);for(int i=0;i<4;i++)for(int j=i+1;j<4;j++)Line(new[]{points[i],points[j]},color);break;
            }
        }
        // Intersect the infinite plane with the mathematical viewing cube.
        public static Vector3[] PlanePolygon(Vector3 origin,Vector3 normal)
        {
            var corners=new Vector3[8];for(int i=0;i<8;i++)corners[i]=new Vector3((i&1)==0?-5:5,(i&2)==0?-5:5,(i&4)==0?-5:5);
            var hits=new List<Vector3>();for(int i=0;i<8;i++)for(int k=0;k<3;k++)if((i&(1<<k))==0)
            {var a=corners[i];var b=corners[i|(1<<k)];float da=Vector3.Dot(normal,a-origin),db=Vector3.Dot(normal,b-origin);if(Mathf.Abs(da)<.0001f)hits.Add(a);if(Mathf.Abs(db)<.0001f)hits.Add(b);if(da*db<0)hits.Add(Vector3.Lerp(a,b,da/(da-db)));}
            var unique=new List<Vector3>();foreach(var p in hits)if(!unique.Any(v=>(v-p).sqrMagnitude<.00001f))unique.Add(p);if(unique.Count<3)return unique.ToArray();
            var center=unique.Aggregate(Vector3.zero,(s,v)=>s+v)/unique.Count;var u=(unique[0]-center).normalized;var vaxis=Vector3.Cross(normal,u);return unique.OrderBy(p=>Mathf.Atan2(Vector3.Dot(p-center,vaxis),Vector3.Dot(p-center,u))).ToArray();
        }
        GameObject Node(string name){GameObject go;if(cursor<drawn.Count && drawn[cursor].name==name)go=drawn[cursor];else{go=new GameObject(name);go.transform.SetParent(transform,false);if(cursor<drawn.Count){Destroy(drawn[cursor]);drawn[cursor]=go;}else drawn.Add(go);}cursor++;go.SetActive(true);return go;}
        void Line(Vector3[] points,Color color)
        {var go=Node("Geometry line");var line=go.GetComponent<LineRenderer>();if(line==null)line=go.AddComponent<LineRenderer>();line.sharedMaterial=lab.lineMaterial;line.useWorldSpace=true;line.positionCount=points.Length;line.SetPositions(points.Select(lab.World).ToArray());line.widthMultiplier=.013f;line.startColor=line.endColor=color;line.numCapVertices=3;}
        void Dashed(Vector3 a,Vector3 b,Color color){float distance=Vector3.Distance(a,b);int count=Mathf.CeilToInt(distance/.22f);for(int i=0;i<count;i++)Line(new[]{Vector3.Lerp(a,b,(float)i/count),Vector3.Lerp(a,b,Mathf.Min(1,(i+.48f)/count))},color);}
        void Arrow(Vector3 a,Vector3 b,Color color)
        {
            if((b-a).sqrMagnitude<1e-8f)return;var root=Node("Vector");root.transform.position=lab.World(a);
            if(root.transform.childCount>0){var arrowView=root.GetComponent<VectorArrowView>();foreach(var r in root.GetComponentsInChildren<Renderer>())Tint(r,color);arrowView.SetScaledVector(b-a,lab.Unit);return;}
            var shaft=GameObject.CreatePrimitive(PrimitiveType.Cylinder);shaft.transform.SetParent(root.transform,false);Destroy(shaft.GetComponent<Collider>());shaft.GetComponent<Renderer>().sharedMaterial=lab.solidMaterial;Tint(shaft.GetComponent<Renderer>(),color);
            var head=new GameObject("Cone",typeof(MeshFilter),typeof(MeshRenderer));head.transform.SetParent(root.transform,false);head.GetComponent<MeshFilter>().sharedMesh=lab.coneMesh;head.GetComponent<Renderer>().sharedMaterial=lab.solidMaterial;Tint(head.GetComponent<Renderer>(),color);
            var arrow=root.AddComponent<VectorArrowView>();arrow.Bind(shaft.transform,head.transform);arrow.SetScaledVector(b-a,lab.Unit);
        }
        void Surface(Vector3[] vertices,int[] indices,Color color)
        {
            var go=Node("Mathematical surface");var filter=go.GetComponent<MeshFilter>();if(filter==null)filter=go.AddComponent<MeshFilter>();var renderer=go.GetComponent<MeshRenderer>();if(renderer==null)renderer=go.AddComponent<MeshRenderer>();var mesh=filter.sharedMesh;if(mesh==null){mesh=new Mesh{name="Cartesian surface"};meshes.Add(mesh);filter.sharedMesh=mesh;}mesh.Clear();mesh.vertices=vertices.Select(lab.World).ToArray();mesh.triangles=indices;mesh.RecalculateNormals();mesh.RecalculateBounds();
            go.transform.SetPositionAndRotation(Vector3.zero,Quaternion.identity);renderer.sharedMaterial=lab.surfaceMaterial;color.a=.14f;Tint(renderer,color);
        }
        public static void Tint(Renderer r,Color c){var block=new MaterialPropertyBlock();block.SetColor("_BaseColor",c);block.SetColor("_Color",c);r.SetPropertyBlock(block);}
        void LateUpdate(){if(label!=null&&Camera.main!=null)label.transform.rotation=Quaternion.LookRotation(label.transform.position-Camera.main.transform.position);}
        void OnDestroy(){foreach(var mesh in meshes)Destroy(mesh);}
    }
}
