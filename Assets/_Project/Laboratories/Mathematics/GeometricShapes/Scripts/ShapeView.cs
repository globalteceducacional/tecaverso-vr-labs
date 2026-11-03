using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
namespace Tecaverso.Labs.Shapes
{
    public class ShapeView:MonoBehaviour
    {
        public ShapesLab lab; public ShapeData data; public bool gallery,rotating,regular=true; public float open;
        public bool showVertices,showEdges=true,showFaces=true,transparent,angles,diagonals,symmetry;
        public readonly HashSet<string> counted=new();
        Transform visual; ShapeGrab grab; TMP_Text label; float hoverUntil,speed;int hovered;
        readonly List<Mesh> meshes=new();
        public void Initialize(ShapesLab owner,ShapeData shape,bool collection)
        {
            lab=owner;data=shape.Copy();regular=data.planar&&data.curved==""&&ShapeGeometry.Regular(data.vertices);gallery=collection;rotating=collection;
            var rb=gameObject.AddComponent<Rigidbody>();rb.isKinematic=true;rb.useGravity=false;
            var box=gameObject.AddComponent<BoxCollider>();box.size=Vector3.one*2.1f;if(data.planar)box.size=new Vector3(2.2f,2.2f,.12f);
            if(collection){var simple=gameObject.AddComponent<XRSimpleInteractable>();simple.colliders.Add(box);simple.selectEntered.AddListener(_=>{if(!lab.Modal)lab.Select(this);});simple.hoverEntered.AddListener(_=>Hover(true));simple.hoverExited.AddListener(_=>Hover(false));}
            else{grab=gameObject.AddComponent<ShapeGrab>();grab.view=this;grab.useDynamicAttach=true;grab.throwOnDetach=false;grab.movementType=XRBaseInteractable.MovementType.Instantaneous;grab.colliders.Add(box);}
            label=lab.WorldLabel(transform,data.name,new Vector3(0,1.35f,0),.34f);Rebuild();
        }
        public void Hover(bool enter){hovered=Mathf.Max(0,hovered+(enter?1:-1));hoverUntil=Time.unscaledTime+1;}
        void Update(){float target=rotating&&!lab.Modal&&hovered==0&&Time.unscaledTime>hoverUntil&&open==0&&(gallery||lab.mode==0)?12:0;speed=Mathf.MoveTowards(speed,target,Time.deltaTime*45);if(speed!=0)transform.Rotate(data.planar?Vector3.forward:Vector3.up,speed*Time.deltaTime,Space.Self);if(label!=null&&Camera.main!=null)label.transform.rotation=Camera.main.transform.rotation;}
        void ClearVisual(){if(visual!=null){visual.gameObject.SetActive(false);Destroy(visual.gameObject);}foreach(var m in meshes)Destroy(m);meshes.Clear();}
        public void UpdateDraggedGeometry()
        {
            if(visual==null)return;
            var mesh=visual.GetComponentInChildren<MeshFilter>();if(mesh!=null&&data.planar){mesh.sharedMesh.vertices=data.vertices;var tri=ShapeGeometry.Triangulate(data.vertices);mesh.sharedMesh.triangles=tri;mesh.sharedMesh.RecalculateBounds();}
            foreach(var g in visual.GetComponentsInChildren<ShapeGrab>())if(g.vertex>=0&&!g.isSelected)g.transform.localPosition=data.vertices[g.vertex];
            var lines=visual.GetComponentsInChildren<LineRenderer>();for(int i=0;i<Mathf.Min(data.vertices.Length,lines.Length);i++){lines[i].SetPosition(0,data.vertices[i]);lines[i].SetPosition(1,data.vertices[(i+1)%data.vertices.Length]);}
        }
        public void Rebuild()
        {
            ClearVisual();visual=new GameObject("Geometry").transform;visual.SetParent(transform,false);
            var polygons=data.curved!=""?new[]{data.vertices}:open>0?ShapeGeometry.Unfold(data,open):data.faces.Select(f=>f.Select(i=>data.vertices[i]).ToArray()).ToArray();
            for(int f=0;f<polygons.Length;f++)
            {
                var v=polygons[f];var go=new GameObject("Face "+(f+1));go.transform.SetParent(visual,false);var mesh=new Mesh();mesh.vertices=v;
                int[] triangles=data.curved!=""?data.faces.SelectMany(face=>Enumerable.Range(1,face.Length-2).SelectMany(i=>new[]{face[0],face[i],face[i+1]})).ToArray():ShapeGeometry.Triangulate(v);mesh.triangles=triangles;mesh.RecalculateNormals();mesh.RecalculateBounds();meshes.Add(mesh);
                go.AddComponent<MeshFilter>().sharedMesh=mesh;var renderer=go.AddComponent<MeshRenderer>();renderer.sharedMaterial=transparent?lab.glass:lab.surface;
                var color=data.curved!=""?new Color(.22f,.48f,.85f):Color.Lerp(new Color(.12f,.32f,.65f),new Color(.35f,.64f,.9f),(f%5)/5f);
                Tint(renderer,color);renderer.enabled=showFaces;
                if(!gallery&&lab.mode==1&&showFaces&&data.curved==""){var collider=go.AddComponent<MeshCollider>();collider.sharedMesh=mesh;Element(go,"Face",f,collider);}
                if(showEdges&&data.curved=="")for(int i=0;i<v.Length;i++)Line(v[i],v[(i+1)%v.Length],Color.cyan,.012f);
            }
            if(data.curved=="circle"&&showEdges)for(int i=0;i<data.vertices.Length;i++)Line(data.vertices[i],data.vertices[(i+1)%data.vertices.Length],Color.cyan,.012f);
            if(!gallery&&open==0&&data.curved=="")
            {
                if(showVertices||lab.mode==2)for(int i=0;i<data.vertices.Length;i++)
                {var go=GameObject.CreatePrimitive(PrimitiveType.Sphere);go.name="Vertex "+i;go.transform.SetParent(visual,false);go.transform.localPosition=data.vertices[i];go.transform.localScale=Vector3.one*.11f;go.GetComponent<Renderer>().sharedMaterial=lab.marker;
                    if(lab.mode==2&&data.planar){var rb=go.AddComponent<Rigidbody>();rb.isKinematic=true;rb.useGravity=false;var g=go.AddComponent<ShapeGrab>();g.view=this;g.vertex=i;g.trackRotation=false;g.useDynamicAttach=true;g.throwOnDetach=false;g.movementType=XRBaseInteractable.MovementType.Instantaneous;g.colliders.Add(go.GetComponent<Collider>());}else if(lab.mode==1)Element(go,"Vértice",i,go.GetComponent<Collider>());else go.GetComponent<Collider>().enabled=false;}
                if(showEdges&&lab.mode==1){var edges=data.Edges;for(int i=0;i<edges.Length;i++){var e=edges[i];var go=new GameObject("Edge target");go.transform.SetParent(visual,false);go.transform.localPosition=(data.vertices[e.Item1]+data.vertices[e.Item2])/2;go.transform.localRotation=Quaternion.FromToRotation(Vector3.up,data.vertices[e.Item2]-data.vertices[e.Item1]);var c=go.AddComponent<CapsuleCollider>();c.radius=.035f;c.height=Vector3.Distance(data.vertices[e.Item1],data.vertices[e.Item2]);Element(go,"Aresta",i,c);}}
            }
            if(data.planar&&data.curved=="")
            {
                                if(showEdges)for(int i=0;i<data.vertices.Length;i++){var a=data.vertices[i];var b=data.vertices[(i+1)%data.vertices.Length];lab.WorldLabel(visual,Vector3.Distance(a,b).ToString("0.00")+" u",(a+b)/2+Vector3.up*.08f,.13f);}
                if(angles)for(int i=0;i<data.vertices.Length;i++){
                    var p=data.vertices[i];float angle=ShapeGeometry.Angle(data.vertices,i);lab.WorldLabel(visual,angle.ToString("0.0")+"°",p*.8f,.16f);
                    var direction=(data.vertices[(i+1)%data.vertices.Length]-p).normalized;float sign=Mathf.Sign(ShapeGeometry.SignedArea(data.vertices));
                    for(int k=0;k<24;k++)Line(p+Quaternion.AngleAxis(sign*angle*k/24,Vector3.forward)*direction*.16f,p+Quaternion.AngleAxis(sign*angle*(k+1)/24,Vector3.forward)*direction*.16f,Color.yellow,.006f);
                }
                if(diagonals)for(int i=0;i<data.vertices.Length;i++)for(int j=i+2;j<data.vertices.Length;j++)if(i!=0||j!=data.vertices.Length-1)Line(data.vertices[i],data.vertices[j],new Color(1,.7f,.25f),.006f);
                if(symmetry)foreach(var axis in SymmetryAxes())Line(-axis*1.4f,axis*1.4f,Color.yellow,.009f);
            }
            if(!gallery&&lab.mode==1&&data.curved!=""){var target=new GameObject("Curved surface target");target.transform.SetParent(visual,false);var collider=target.AddComponent<SphereCollider>();collider.radius=data.radius;Element(target,"Superfície",0,collider);}
            if(grab!=null){GetComponent<BoxCollider>().enabled=lab.mode==0&&open==0;grab.trackPosition=lab.mode==0;grab.trackRotation=lab.mode==0;}
        }
        IEnumerable<Vector3> SymmetryAxes()
        {
            var center=data.vertices.Aggregate(Vector3.zero,(a,b)=>a+b)/data.vertices.Length;var axes=new List<Vector3>();
            foreach(var p in data.vertices.Concat(data.vertices.Select((v,i)=>(v+data.vertices[(i+1)%data.vertices.Length])/2)))
            {var d=(p-center).normalized;if(d.sqrMagnitude<.5f||axes.Any(a=>Mathf.Abs(Vector3.Dot(a,d))>.999f))continue;if(data.vertices.All(v=>data.vertices.Any(q=>Vector3.Distance(q,center+2*Vector3.Dot(v-center,d)*d-(v-center))<.005f)))axes.Add(d);}return axes;
        }
        void Element(GameObject go,string kind,int index,Collider c){var x=go.AddComponent<XRSimpleInteractable>();x.colliders.Add(c);var e=go.AddComponent<ShapeElement>();e.view=this;e.kind=kind;e.index=index;}
        void Line(Vector3 a,Vector3 b,Color color,float width){var go=new GameObject("Line");go.transform.SetParent(visual,false);var l=go.AddComponent<LineRenderer>();l.useWorldSpace=false;l.sharedMaterial=lab.lines;l.startColor=l.endColor=color;l.widthMultiplier=width;l.positionCount=2;l.SetPositions(new[]{a,b});}
        public static void Tint(Renderer r,Color color){var block=new MaterialPropertyBlock();block.SetColor("_BaseColor",color);r.SetPropertyBlock(block);}
        public void Inspect(string kind,int index,bool select)
        {
            if(select){counted.Add(kind+index);if(kind=="Vértice")lab.vertexIndex=index;}string measure="";if(kind=="Superfície")measure=data.curved=="sphere"?" · curva, sem vértices ou arestas poliedrais":data.curved=="circle"?" · círculo, não é polígono":data.curved=="cylinder"?" · duas bases circulares e superfície lateral curva":" · base circular, superfície lateral curva e ápice";
            if(kind=="Aresta"){var e=data.Edges[index];measure=" · "+Vector3.Distance(data.vertices[e.Item1],data.vertices[e.Item2]).ToString("0.00")+" u";}
            if(kind=="Face")measure=data.curved!=""?" · superfície curva / base":" · "+data.faces[index].Length+" lados · "+ShapeGeometry.FaceArea(data.faces[index].Select(i=>data.vertices[i]).ToArray()).ToString("0.00")+" u²";
            lab.detail.text=kind+" "+(index+1)+measure+"\nElementos visitados: "+counted.Count;
        }
        public void Edit(int index,Vector3 target)
        {
            target.z=0;target.x=Mathf.Clamp(target.x,-2,2);target.y=Mathf.Clamp(target.y,-2,2);if(lab.magnetic){target.x=Mathf.Abs(target.x-Mathf.Round(target.x))<.12f?Mathf.Round(target.x):target.x;target.y=Mathf.Abs(target.y-Mathf.Round(target.y))<.12f?Mathf.Round(target.y):target.y;}
            var next=(Vector3[])data.vertices.Clone();if(regular){var center=next.Aggregate(Vector3.zero,(a,b)=>a+b)/next.Length;var old=next[index]-center;float scale=Mathf.Clamp((target-center).magnitude/old.magnitude,.5f,2);var rotation=Quaternion.FromToRotation(old,target-center);next=next.Select(v=>center+rotation*(v-center)*scale).ToArray();}else next[index]=target;
            if(!ShapeGeometry.ValidPolygon(next)){lab.detail.text="Movimento inválido: evite cruzar lados ou achatar a figura.";return;}
            data.vertices=next;UpdateDraggedGeometry();lab.Refresh();lab.QueueRebuild(this);
        }
        public bool CanUnfold=>data.name=="Cubo"||data.name=="Prisma triangular"||data.name=="Pirâmide de base quadrada";
        void OnDestroy(){foreach(var m in meshes)if(m!=null)Destroy(m);}
    }
}
