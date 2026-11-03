using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tecaverso.Labs.Shapes
{
    [Serializable] public sealed class ShapeData
    {
        public string name; public Vector3[] vertices; public int[][] faces;
        public bool planar; public string curved=""; public float radius=1,height=2;
        public ShapeData Copy()=>new ShapeData{name=name,vertices=(Vector3[])vertices.Clone(),faces=faces.Select(f=>(int[])f.Clone()).ToArray(),planar=planar,curved=curved,radius=radius,height=height};
        public (int,int)[] Edges=>faces.SelectMany(f=>f.Select((a,i)=>(Math.Min(a,f[(i+1)%f.Length]),Math.Max(a,f[(i+1)%f.Length])))).Distinct().ToArray();
        public float Area=>curved=="sphere"?4*Mathf.PI*radius*radius:curved=="circle"?Mathf.PI*radius*radius:curved=="cylinder"?2*Mathf.PI*radius*(radius+height):curved=="cone"?Mathf.PI*radius*(radius+Mathf.Sqrt(radius*radius+height*height)):faces.Sum(f=>ShapeGeometry.FaceArea(f.Select(i=>vertices[i]).ToArray()));
        public float Perimeter=>curved=="circle"?2*Mathf.PI*radius:Edges.Sum(e=>Vector3.Distance(vertices[e.Item1],vertices[e.Item2]));
        public float Volume
        {
            get
            {
                if(planar)return 0;if(curved=="sphere")return 4f/3*Mathf.PI*radius*radius*radius;
                if(curved=="cylinder"||curved=="cone")return Mathf.PI*radius*radius*height/(curved=="cone"?3:1);
                float sum=0;foreach(var f in faces)for(int i=1;i<f.Length-1;i++)sum+=Vector3.Dot(vertices[f[0]],Vector3.Cross(vertices[f[i]],vertices[f[i+1]]))/6;return Mathf.Abs(sum);
            }
        }
    }
    public static class ShapeGeometry
    {
        public static float Cross(Vector2 a,Vector2 b)=>a.x*b.y-a.y*b.x;
        public static float SignedArea(Vector3[] v){float a=0;for(int i=0;i<v.Length;i++)a+=Cross(v[i],v[(i+1)%v.Length]);return a/2;}
        public static float FaceArea(Vector3[] v){var sum=Vector3.zero;for(int i=0;i<v.Length;i++)sum+=Vector3.Cross(v[i],v[(i+1)%v.Length]);return sum.magnitude/2;}
        public static float Angle(Vector3[] v,int i)
        {
            Vector2 before=v[(i+v.Length-1)%v.Length]-v[i],after=v[(i+1)%v.Length]-v[i];float a=Vector2.Angle(before,after);
            return Cross(before,after)*Mathf.Sign(SignedArea(v))>0?360-a:a;
        }
        public static bool Regular(Vector3[] v)=>Enumerable.Range(0,v.Length).All(i=>Mathf.Abs(Vector3.Distance(v[i],v[(i+1)%v.Length])-Vector3.Distance(v[0],v[1]))<.005f&&Mathf.Abs(Angle(v,i)-Angle(v,0))<.2f);
        public static bool Convex(Vector3[] v)=>Enumerable.Range(0,v.Length).All(i=>Angle(v,i)<180-.01f);
        public static bool ValidPolygon(Vector3[] v)
        {
            if(v.Length<3||v.Length>12||v.Any(p=>!float.IsFinite(p.x)||!float.IsFinite(p.y)||Mathf.Abs(p.z)>.001f)||Mathf.Abs(SignedArea(v))<.01f)return false;
            for(int i=0;i<v.Length;i++)
            {
                Vector2 a=v[i],b=v[(i+1)%v.Length];if((a-b).magnitude<.05f)return false;
                if(Mathf.Abs(Cross(v[(i+v.Length-1)%v.Length]-v[i],b-a))<.001f)return false;
                for(int j=i+1;j<v.Length;j++)
                {
                    if(j==i+1||(i==0&&j==v.Length-1))continue;Vector2 c=v[j],d=v[(j+1)%v.Length];
                    float p=Cross(b-a,c-a),q=Cross(b-a,d-a),r=Cross(d-c,a-c),s=Cross(d-c,b-c);
                    if(p*q<=0&&r*s<=0&&Mathf.Max(Mathf.Min(a.x,b.x),Mathf.Min(c.x,d.x))<=Mathf.Min(Mathf.Max(a.x,b.x),Mathf.Max(c.x,d.x))&&Mathf.Max(Mathf.Min(a.y,b.y),Mathf.Min(c.y,d.y))<=Mathf.Min(Mathf.Max(a.y,b.y),Mathf.Max(c.y,d.y)))return false;
                }
            }
            return true;
        }
        public static int[] Triangulate(Vector3[] v)
        {
            // Ear clipping preserves concave outlines. Project the planar face onto its own basis.
            var n=Vector3.Cross(v[1]-v[0],v[2]-v[0]).normalized;var u=(v[1]-v[0]).normalized;var w=Vector3.Cross(n,u);
            var p=v.Select(x=>new Vector2(Vector3.Dot(x,u),Vector3.Dot(x,w))).ToArray();float area=0;for(int i=0;i<p.Length;i++)area+=Cross(p[i],p[(i+1)%p.Length]);float sign=Mathf.Sign(area);
            var ids=Enumerable.Range(0,v.Length).ToList();var result=new List<int>();int guard=v.Length*v.Length;
            while(ids.Count>3&&guard-->0){bool found=false;for(int i=0;i<ids.Count;i++){int a=ids[(i+ids.Count-1)%ids.Count],b=ids[i],c=ids[(i+1)%ids.Count];if(Cross(p[b]-p[a],p[c]-p[b])*sign<.00001f)continue;
                    bool contains=ids.Any(k=>k!=a&&k!=b&&k!=c&&Cross(p[b]-p[a],p[k]-p[a])*sign>=0&&Cross(p[c]-p[b],p[k]-p[b])*sign>=0&&Cross(p[a]-p[c],p[k]-p[c])*sign>=0);
                    if(contains)continue;result.AddRange(new[]{a,b,c});ids.RemoveAt(i);found=true;break;}if(!found)throw new ArgumentException("Face cannot be triangulated");}
            result.AddRange(ids);return result.ToArray();
        }
        public static ShapeData Polygon(int n,float r=1)
        {
            n=Mathf.Clamp(n,3,12);var v=Enumerable.Range(0,n).Select(i=>new Vector3(Mathf.Cos(2*Mathf.PI*i/n+Mathf.PI/2),Mathf.Sin(2*Mathf.PI*i/n+Mathf.PI/2),0)*r).ToArray();
            return new ShapeData{name=new[]{"Triângulo","Quadrado","Pentágono","Hexágono","Heptágono","Octógono","Eneágono","Decágono","Undecágono","Dodecágono"}[n-3],planar=true,vertices=v,faces=new[]{Enumerable.Range(0,n).ToArray()}};
        }
        public static ShapeData Hull(string name,Vector3[] v)
        {
            var faces=new List<int[]>();var keys=new HashSet<string>();
            for(int a=0;a<v.Length;a++)for(int b=a+1;b<v.Length;b++)for(int c=b+1;c<v.Length;c++)
            {
                var normal=Vector3.Cross(v[b]-v[a],v[c]-v[a]);if(normal.sqrMagnitude<1e-8f)continue;normal.Normalize();var distances=v.Select(p=>Vector3.Dot(p-v[a],normal)).ToArray();
                if(distances.Any(d=>d>.0001f)&&distances.Any(d=>d<-.0001f))continue;if(distances.Any(d=>d>.0001f))normal=-normal;
                var ids=Enumerable.Range(0,v.Length).Where(i=>Mathf.Abs(distances[i])<.0001f).ToArray();if(!keys.Add(string.Join(",",ids)))continue;
                var center=ids.Aggregate(Vector3.zero,(s,i)=>s+v[i])/ids.Length;var x=(v[ids[0]]-center).normalized;var y=Vector3.Cross(normal,x);
                faces.Add(ids.OrderBy(i=>Mathf.Atan2(Vector3.Dot(v[i]-center,y),Vector3.Dot(v[i]-center,x))).ToArray());
            }
            return new ShapeData{name=name,vertices=v,faces=faces.ToArray()};
        }
        public static ShapeData Prism(int n,bool pyramid=false)
        {
            var ring=Polygon(n).vertices.Select(p=>new Vector3(p.x,-.7f,p.y)).ToArray();return Hull(pyramid?"Pirâmide de base quadrada":n==3?"Prisma triangular":"Prisma pentagonal",ring.Concat(pyramid?new[]{new Vector3(0,.7f,0)}:ring.Select(p=>p+Vector3.up*1.4f)).ToArray());
        }
        public static List<ShapeData> Catalog()
        {
            var list=new List<ShapeData>();for(int n=3;n<=8;n++)list.Add(Polygon(n));
            var rectangle=Polygon(4);rectangle.name="Retângulo";rectangle.vertices=new[]{new Vector3(-1,-.6f,0),new Vector3(1,-.6f,0),new Vector3(1,.6f,0),new Vector3(-1,.6f,0)};list.Add(rectangle);
            var concave=Polygon(5);concave.name="Polígono côncavo";concave.vertices=new[]{new Vector3(-1,-1,0),new Vector3(1,-1,0),new Vector3(.2f,0,0),new Vector3(1,1,0),new Vector3(-1,1,0)};list.Add(concave);
            list.Add(Curved("circle"));list.Add(Hull("Cubo",(from x in new[]{-1,1} from y in new[]{-1,1} from z in new[]{-1,1} select new Vector3(x,y,z)*.65f).ToArray()));
            list.Add(Hull("Tetraedro",new[]{new Vector3(1,1,1),new Vector3(1,-1,-1),new Vector3(-1,1,-1),new Vector3(-1,-1,1)}.Select(p=>p*.6f).ToArray()));
            list.Add(Hull("Octaedro",new[]{Vector3.up,Vector3.down,Vector3.right,Vector3.left,Vector3.forward,Vector3.back}));
            float t=(1+Mathf.Sqrt(5))/2;var ico=new List<Vector3>();foreach(int a in new[]{-1,1})foreach(int b in new[]{-1,1}){ico.Add(new Vector3(0,a,b*t).normalized);ico.Add(new Vector3(a,b*t,0).normalized);ico.Add(new Vector3(b*t,0,a).normalized);}var ic=Hull("Icosaedro",ico.ToArray());
            list.Add(Hull("Dodecaedro",ic.faces.Select(f=>f.Aggregate(Vector3.zero,(s,i)=>s+ic.vertices[i]).normalized).ToArray()));list.Add(ic);
            list.Add(Prism(3));list.Add(Prism(5));list.Add(Prism(4,true));list.Add(Curved("sphere"));list.Add(Curved("cylinder"));list.Add(Curved("cone"));return list;
        }
        public static ShapeData Curved(string kind)
        {
            var v=new List<Vector3>();var f=new List<int[]>();const int segments=32;
            int rings=kind=="sphere"?17:kind=="circle"?1:2;
            for(int j=0;j<rings;j++)for(int i=0;i<segments;i++)
            {float a=2*Mathf.PI*i/segments;float phi=Mathf.PI*j/(rings-1);float r=kind=="sphere"?Mathf.Max(.0001f,Mathf.Sin(phi)):kind=="cone"&&j==1?.0001f:1;float y=kind=="sphere"?Mathf.Cos(phi):j*2-1;v.Add(kind=="circle"?new Vector3(Mathf.Cos(a),Mathf.Sin(a),0):new Vector3(r*Mathf.Cos(a),y,r*Mathf.Sin(a)));}
            if(kind=="circle")f.Add(Enumerable.Range(0,segments).ToArray());else{for(int j=0;j<rings-1;j++)for(int i=0;i<segments;i++)f.Add(new[]{j*segments+i,j*segments+(i+1)%segments,(j+1)*segments+(i+1)%segments,(j+1)*segments+i});if(kind!="sphere"){f.Add(Enumerable.Range(0,segments).Reverse().ToArray());if(kind=="cylinder")f.Add(Enumerable.Range(segments,segments).ToArray());}}
            return new ShapeData{name=kind=="circle"?"Círculo":kind=="sphere"?"Esfera":kind=="cone"?"Cone":"Cilindro",planar=kind=="circle",curved=kind,vertices=v.ToArray(),faces=f.ToArray()};
        }
        public static Vector3[][] Unfold(ShapeData shape,float amount)
        {
            var result=new Vector3[shape.faces.Length][];var matrices=new Matrix4x4[shape.faces.Length];var done=new bool[shape.faces.Length];
            int root=Enumerable.Range(0,shape.faces.Length).OrderBy(i=>shape.faces[i].Average(k=>shape.vertices[k].y)).First();done[root]=true;matrices[root]=Matrix4x4.identity;var queue=new Queue<int>();queue.Enqueue(root);
            Vector3 Normal(int i){var f=shape.faces[i];return Vector3.Cross(shape.vertices[f[1]]-shape.vertices[f[0]],shape.vertices[f[2]]-shape.vertices[f[0]]).normalized;}
            while(queue.Count>0){int parent=queue.Dequeue();for(int child=0;child<shape.faces.Length;child++){if(done[child])continue;var edge=shape.faces[parent].Intersect(shape.faces[child]).ToArray();if(edge.Length!=2)continue;
                    var pivot=shape.vertices[edge[0]];var axis=(shape.vertices[edge[1]]-pivot).normalized;float angle=Vector3.SignedAngle(Normal(child),Normal(parent),axis);
                    matrices[child]=matrices[parent]*Matrix4x4.Translate(pivot)*Matrix4x4.Rotate(Quaternion.AngleAxis(angle*Mathf.Clamp01(amount),axis))*Matrix4x4.Translate(-pivot);done[child]=true;queue.Enqueue(child);}}
            for(int i=0;i<result.Length;i++)result[i]=shape.faces[i].Select(k=>matrices[i].MultiplyPoint3x4(shape.vertices[k])).ToArray();return result;
        }
    }
}
