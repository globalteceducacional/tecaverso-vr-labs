using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tecaverso.Labs.Cartesian
{
    public enum ConstructionKind { Segment, Line, Ray, Triangle, Plane, Box, Sphere, Tetrahedron }
    [Serializable] public sealed class CartesianPoint
    {
        public int id; public Vector3 position; public bool lockX,lockY,lockZ; public int plane=-1;
        public string Name => id<26?((char)('A'+id)).ToString():"P"+(id+1);
        public CartesianPoint Copy() => (CartesianPoint)MemberwiseClone();
    }
    [Serializable] public sealed class CartesianConstruction
    {
        public int id; public ConstructionKind kind; public int[] points;
        public CartesianConstruction Copy()=>new CartesianConstruction{id=id,kind=kind,points=(int[])points.Clone()};
    }
    public sealed class CartesianModel
    {
        public const float Limit=5, Epsilon=.001f;
        public const int MaxPoints=24,MaxConstructions=16;
        public readonly List<CartesianPoint> Points=new();
        public readonly List<CartesianConstruction> Constructions=new();
        public int Revision {get;private set;}
        public bool CanUndo=>undo.Count>0;public bool CanRedo=>redo.Count>0;
        int nextPoint,nextConstruction;
        sealed class State {public CartesianPoint[] p;public CartesianConstruction[] c;public int np,nc;}
        readonly List<State> undo=new(),redo=new();State gesture;
        State Snapshot()=>new State{p=Points.Select(p=>p.Copy()).ToArray(),c=Constructions.Select(c=>c.Copy()).ToArray(),np=nextPoint,nc=nextConstruction};
        void Restore(State s){Points.Clear();Points.AddRange(s.p.Select(p=>p.Copy()));Constructions.Clear();Constructions.AddRange(s.c.Select(c=>c.Copy()));nextPoint=s.np;nextConstruction=s.nc;Revision++;}
        void Push(State s){undo.Add(s);if(undo.Count>64)undo.RemoveAt(0);redo.Clear();}
        void Save(){if(gesture==null)Push(Snapshot());}
        public void BeginGesture(){if(gesture==null)gesture=Snapshot();}
        public void EndGesture(){if(gesture==null)return;if(!Same(gesture,Snapshot()))Push(gesture);gesture=null;}
        static bool Same(State a,State b)=>a.p.Length==b.p.Length&&a.c.Length==b.c.Length&&a.p.Zip(b.p,(x,y)=>x.id==y.id&&x.position==y.position).All(x=>x);
        public void Undo(){EndGesture();if(!CanUndo)return;var s=undo[^1];undo.RemoveAt(undo.Count-1);redo.Add(Snapshot());Restore(s);}
        public void Redo(){EndGesture();if(!CanRedo)return;var s=redo[^1];redo.RemoveAt(redo.Count-1);undo.Add(Snapshot());Restore(s);}
        public CartesianPoint Point(int id)=>Points.FirstOrDefault(p=>p.id==id);
        public CartesianConstruction Construction(int id)=>Constructions.FirstOrDefault(c=>c.id==id);
        public CartesianPoint AddPoint(int plane=-1)
        {if(Points.Count>=MaxPoints)return null;Save();var p=new CartesianPoint{id=nextPoint++,plane=plane};Points.Add(p);Revision++;return p;}
        public static bool Finite(Vector3 v)=>float.IsFinite(v.x)&&float.IsFinite(v.y)&&float.IsFinite(v.z);
        public bool Move(int id,Vector3 position,float step=0)
        {
            var p=Point(id);if(p==null||!Finite(position))return false;
            for(int i=0;i<3;i++){position[i]=Mathf.Clamp(position[i],-Limit,Limit);if(step>0)position[i]=Mathf.Round(position[i]/step)*step;}
            if(p.lockX)position.x=p.position.x;if(p.lockY)position.y=p.position.y;if(p.lockZ)position.z=p.position.z;
            if(p.plane>=0)position[p.plane]=0;
            if((position-p.position).sqrMagnitude<1e-10f)return false;
            Save();p.position=position;Revision++;return true;
        }
        public void ToggleLock(int id,int axis){var p=Point(id);if(p==null)return;Save();if(axis==0)p.lockX=!p.lockX;if(axis==1)p.lockY=!p.lockY;if(axis==2)p.lockZ=!p.lockZ;Revision++;}
        // Common clamped delta preserves shape, including shared vertices and axis locks.
        public Vector3 Translate(int[] ids,Vector3 delta)
        {
            if(!Finite(delta))return Vector3.zero;
            var ps=ids.Distinct().Select(Point).Where(p=>p!=null).ToArray();if(ps.Length==0)return Vector3.zero;
            for(int axis=0;axis<3;axis++)
            {
                if(ps.Any(p=>p.plane==axis||(axis==0?p.lockX:axis==1?p.lockY:p.lockZ))){delta[axis]=0;continue;}
                delta[axis]=Mathf.Clamp(delta[axis],ps.Max(p=>-Limit-p.position[axis]),ps.Min(p=>Limit-p.position[axis]));
            }
            if(delta.sqrMagnitude<1e-10f)return Vector3.zero;Save();foreach(var p in ps)p.position+=delta;Revision++;return delta;
        }
        // Apply only to XR point dragging; exact numeric edits and rigid group moves bypass it.
        public static Vector3 Magnetize(Vector3 position,bool enabled)
        {
            if(!enabled)return position;
            for(int axis=0;axis<3;axis++)
            {
                float nearest=Mathf.Round(position[axis]);
                if(Mathf.Abs(position[axis]-nearest)<=.12f)position[axis]=nearest;
            }
            return position;
        }
        public static int Required(ConstructionKind k)=>k==ConstructionKind.Tetrahedron?4:k==ConstructionKind.Triangle||k==ConstructionKind.Plane?3:2;
        public bool Validate(ConstructionKind kind,int[] ids,out string error)
        {
            error="";if(ids.Length!=Required(kind)||ids.Distinct().Count()!=ids.Length||ids.Any(i=>Point(i)==null)){error="Selecione pontos diferentes para completar a construção.";return false;}
            var v=ids.Select(i=>Point(i).position).ToArray();
            if((v[1]-v[0]).magnitude<Epsilon){error="Os pontos coincidem. Afaste um deles.";return false;}
            if(kind==ConstructionKind.Box&&Enumerable.Range(0,3).Any(i=>Mathf.Abs(v[1][i]-v[0][i])<Epsilon)){error="A caixa precisa de largura, altura e profundidade maiores que zero.";return false;}
            if(v.Length>=3&&Vector3.Cross(v[1]-v[0],v[2]-v[0]).magnitude<Epsilon){error="Pontos alinhados. Mova o terceiro ponto para definir a superfície.";return false;}
            if(v.Length==4&&Mathf.Abs(Vector3.Dot(Vector3.Cross(v[1]-v[0],v[2]-v[0]),v[3]-v[0]))<Epsilon){error="Pontos coplanares. Mova o quarto ponto para formar um volume.";return false;}
            return true;
        }
        public CartesianConstruction Add(ConstructionKind kind,int[] ids,out string error)
        {
            if(Constructions.Count>=MaxConstructions){error="Limite de 16 construções. Exclua uma para continuar.";return null;}
            if(!Validate(kind,ids,out error))return null;Save();var c=new CartesianConstruction{id=nextConstruction++,kind=kind,points=(int[])ids.Clone()};Constructions.Add(c);Revision++;return c;
        }
        public int Dependencies(int point)=>Constructions.Count(c=>c.points.Contains(point));
        public void DeletePoint(int id){if(Point(id)==null)return;Save();Points.RemoveAll(p=>p.id==id);Constructions.RemoveAll(c=>c.points.Contains(id));Revision++;}
        public void DeleteConstruction(int id){if(Construction(id)==null)return;Save();Constructions.RemoveAll(c=>c.id==id);Revision++;}
        public void Clear(){if(Points.Count==0&&Constructions.Count==0)return;Save();Points.Clear();Constructions.Clear();nextPoint=nextConstruction=0;Revision++;}
        public Vector3[] Vertices(CartesianConstruction c)=>c.points.Select(id=>Point(id).position).ToArray();
        public static float TriangleArea(Vector3 a,Vector3 b,Vector3 c)=>Vector3.Cross(b-a,c-a).magnitude*.5f;
        public static float TetraVolume(Vector3 a,Vector3 b,Vector3 c,Vector3 d)=>Mathf.Abs(Vector3.Dot(Vector3.Cross(b-a,c-a),d-a))/6;
        public static bool ClipLine(Vector3 origin,Vector3 direction,bool ray,out Vector3 a,out Vector3 b)
        {
            float lo=ray?0:float.NegativeInfinity,hi=float.PositiveInfinity;a=b=origin;
            for(int i=0;i<3;i++){if(Mathf.Abs(direction[i])<1e-7f){if(Mathf.Abs(origin[i])>Limit)return false;continue;}float x=(-Limit-origin[i])/direction[i],y=(Limit-origin[i])/direction[i];lo=Mathf.Max(lo,Mathf.Min(x,y));hi=Mathf.Min(hi,Mathf.Max(x,y));}
            if(lo>hi||!float.IsFinite(lo)||!float.IsFinite(hi))return false;a=origin+direction*lo;b=origin+direction*hi;return true;
        }
    }
}
