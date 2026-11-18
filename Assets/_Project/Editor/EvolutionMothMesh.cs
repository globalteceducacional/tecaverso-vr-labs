using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tecaverso.Editor
{
    // One instanced mesh per insect. Wings, thorax, abdomen, legs and antennae
    // share a draw. The vertex mask keeps the body pigment separate from the wings.
    internal sealed class EvolutionMothMesh
    {
        readonly List<Vector3> vertices = new List<Vector3>();
        readonly List<Vector2> uv = new List<Vector2>();
        readonly List<Color32> mask = new List<Color32>();
        readonly List<int> triangles = new List<int>();

        public static Mesh GetOrCreate(string path)
        {
            var existing = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (existing) return existing;
            var model = new EvolutionMothMesh();
            for (int side = -1; side <= 1; side += 2)
            {
                model.Wing(side, new Vector3(.028f,.014f,-.018f), new[] {
                    new Vector3(.005f,.013f,.009f), new Vector3(.061f,.008f,-.010f), new Vector3(.058f,.006f,-.031f),
                    new Vector3(.044f,.005f,-.045f), new Vector3(.030f,.006f,-.049f), new Vector3(.017f,.008f,-.042f),
                    new Vector3(.006f,.010f,-.031f) });
                model.Wing(side, new Vector3(.037f,.023f,.012f), new[] {
                    new Vector3(.005f,.023f,.025f), new Vector3(.048f,.020f,.040f), new Vector3(.089f,.012f,.046f),
                    new Vector3(.096f,.007f,.040f), new Vector3(.084f,.006f,.010f), new Vector3(.070f,.006f,-.016f),
                    new Vector3(.051f,.008f,-.020f), new Vector3(.009f,.015f,-.014f) });
                Vector3 legBase = new Vector3(side*.005f,.009f,.006f);
                for (int leg = 0; leg < 3; leg++)
                {
                    var joint = legBase + new Vector3(side*.014f,.003f,(leg-1)*.023f);
                    model.Stem(legBase, joint,.0016f,.0013f);
                    model.Stem(joint, joint + new Vector3(side*.012f,-.011f,(leg-1)*.010f),.0013f,.0007f);
                }
                var brow = new Vector3(side*.003f,.025f,.031f);
                var bend = new Vector3(side*.011f,.031f,.044f);
                model.Stem(brow,bend,.0014f,.0012f);
                model.Stem(bend,new Vector3(side*.023f,.033f,.056f),.0012f,.0005f);
            }
            var tail = new Vector3(0,.013f,-.041f);
            var abdomen = new Vector3(0,.015f,-.018f);
            var waist = new Vector3(0,.017f,-.003f);
            var chest = new Vector3(0,.020f,.012f);
            var neck = new Vector3(0,.021f,.022f);
            var head = new Vector3(0,.022f,.028f);
            model.Stem(tail, abdomen,.001f,.006f);
            model.Stem(abdomen,waist,.006f,.005f);
            model.Stem(waist,chest,.005f,.009f);
            model.Stem(chest,neck,.009f,.005f);
            model.Stem(neck,head,.005f,.006f);
            model.Stem(head,new Vector3(0,.022f,.034f),.006f,.001f);
            var mesh = new Mesh { name = "Mariposa · asas e anatomia externa" };
            mesh.SetVertices(model.vertices);
            mesh.SetUVs(0,model.uv);
            mesh.SetColors(model.mask);
            mesh.SetTriangles(model.triangles,0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            AssetDatabase.CreateAsset(mesh,path);
            return mesh;
        }

        int Add(Vector3 p, bool body)
        {
            vertices.Add(p);
            uv.Add(new Vector2(p.x,p.z) * 10f);
            mask.Add(body ? new Color32(255,255,255,255) : new Color32(0,0,0,255));
            return vertices.Count - 1;
        }
        void Wing(int side, Vector3 center, Vector3[] ring)
        {
            center.x *= side;
            int mid = Add(center,false);
            int first = mid + 1;
            foreach (Vector3 point in ring) Add(new Vector3(point.x*side,point.y,point.z),false);
            for (int i=0;i<ring.Length;i++)
            {
                int a=first+i, b=first+(i+1)%ring.Length;
                triangles.Add(mid);
                triangles.Add(side > 0 ? a : b);
                triangles.Add(side > 0 ? b : a);
            }
        }
        void Stem(Vector3 a, Vector3 b, float radiusA, float radiusB)
        {
            const int sides=8;
            Vector3 axis=(b-a).normalized;
            Vector3 x=Vector3.Cross(axis,Vector3.up).normalized;
            Vector3 y=Vector3.Cross(axis,x);
            int first=vertices.Count;
            for(int i=0;i<sides;i++)
            {
                float angle=i*Mathf.PI*2f/sides;
                Vector3 outwards=x*Mathf.Cos(angle)+y*Mathf.Sin(angle);
                Add(a+radiusA*outwards,true);
                Add(b+radiusB*outwards,true);
            }
            for(int i=0;i<sides;i++)
            {
                int ia=first+2*i, ib=first+2*((i+1)%sides);
                triangles.AddRange(new[]{ia,ib,ia+1,ib,ib+1,ia+1});
            }
        }
    }
}
