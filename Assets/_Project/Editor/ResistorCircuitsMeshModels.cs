using System;
using System.Collections.Generic;
using System.Linq;
using Tecaverso.Labs.ResistorCircuits;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.ProBuilder;
using Object = UnityEngine.Object;

namespace Tecaverso.Editor
{
    /// <summary>Editable ProBuilder surfaces for the existing XR circuit parts. Electrical roots and sockets are untouched.</summary>
    public static class ResistorCircuitsMeshModels
    {
        const string Materials = "Assets/_Project/Laboratories/Physics/ResistorCircuits/Materials/";
        static readonly string[] MaterialNames = {"Device navy", "Brushed metal", "Resistor ceramic", "Contact gold", "Indicator red", "Indicator orange", "Soft rubber", "Bulb opal glass", "Instrument LCD", "Battery label blue", "Resistor brown"};
        static Material[] materials;
        const int Navy=0, Metal=1, Ceramic=2, Gold=3, Red=4, Orange=5, Rubber=6, Glass=7, Lcd=8, Blue=9, Brown=10;

        sealed class Builder
        {
            readonly List<Vector3> vertices=new();
            readonly List<Face> faces=new();
            void Quad(Vector3 a,Vector3 b,Vector3 c,Vector3 d,int material)
            {
                int i=vertices.Count;vertices.Add(a);vertices.Add(b);vertices.Add(c);vertices.Add(d);
                var face=new Face(new[]{i,i+2,i+1,i,i+3,i+2}){submeshIndex=material};faces.Add(face);
            }
            void Triangle(Vector3 a,Vector3 b,Vector3 c,int material)
            {
                int i=vertices.Count;vertices.Add(a);vertices.Add(b);vertices.Add(c);
                faces.Add(new Face(new[]{i,i+2,i+1}){submeshIndex=material});
            }
            void Cap(Vector3 center,IList<Vector3> ring,int material,bool top)
            {
                for(int i=0;i<ring.Count;i++)
                {
                    var a=ring[i];var b=ring[(i+1)%ring.Count];
                    if(top)Triangle(center,a,b,material);else Triangle(center,b,a,material);
                }
            }
            public void Loft(IList<(float y,float radius,int material)> profile,int sides,Vector3 offset,bool alongX=false)
            {
                var rings=new List<Vector3[]>();
                foreach(var p in profile)
                {
                    var ring=new Vector3[sides];
                    for(int k=0;k<sides;k++)
                    {
                        float angle=k*Mathf.PI*2/sides;
                        var v=new Vector3(Mathf.Cos(angle)*p.radius,p.y,Mathf.Sin(angle)*p.radius);
                        ring[k]=offset+(alongX?new Vector3(v.y,v.x,v.z):v);
                    }
                    rings.Add(ring);
                }
                for(int i=0;i<rings.Count-1;i++)for(int k=0;k<sides;k++)
                {
                    int next=(k+1)%sides;
                    Quad(rings[i][k],rings[i][next],rings[i+1][next],rings[i+1][k],profile[i].material);
                }
                var bottom=alongX?offset+Vector3.right*profile[0].y:offset+Vector3.up*profile[0].y;
                var top=alongX?offset+Vector3.right*profile[^1].y:offset+Vector3.up*profile[^1].y;
                Cap(bottom,rings[0],profile[0].material,false);Cap(top,rings[^1],profile[^1].material,true);
            }
            public void RoundedCase(Vector3 center,float width,float height,float depth,float corner,int material)
            {
                var outline=new List<Vector2>();
                for(int quadrant=0;quadrant<4;quadrant++)
                {
                    float angle0=quadrant*90f;
                    float cx=(quadrant==0||quadrant==3?1:-1)*(width*.5f-corner);
                    float cz=(quadrant<2?1:-1)*(depth*.5f-corner);
                    for(int step=0;step<=3;step++)
                    {
                        float a=(angle0+step*30f)*Mathf.Deg2Rad;
                        outline.Add(new Vector2(cx+Mathf.Cos(a)*corner,cz+Mathf.Sin(a)*corner));
                    }
                }
                var bottom=outline.Select(p=>center+new Vector3(p.x,-height*.5f,p.y)).ToArray();
                var top=outline.Select(p=>center+new Vector3(p.x,height*.5f,p.y)).ToArray();
                for(int i=0;i<outline.Count;i++){int j=(i+1)%outline.Count;Quad(bottom[i],bottom[j],top[j],top[i],material);}
                Cap(center-Vector3.up*height*.5f,bottom,material,false);
                Cap(center+Vector3.up*height*.5f,top,material,true);
            }
            public void Box(Vector3 center,Vector3 size,int material)
            {
                var x=size.x*.5f;var y=size.y*.5f;var z=size.z*.5f;
                var p=new[]{new Vector3(-x,-y,-z),new Vector3(x,-y,-z),new Vector3(x,y,-z),new Vector3(-x,y,-z),new Vector3(-x,-y,z),new Vector3(x,-y,z),new Vector3(x,y,z),new Vector3(-x,y,z)};
                int[,] q={{0,1,2,3},{5,4,7,6},{4,0,3,7},{1,5,6,2},{3,2,6,7},{4,5,1,0}};
                for(int k=0;k<6;k++)Quad(center+p[q[k,0]],center+p[q[k,1]],center+p[q[k,2]],center+p[q[k,3]],material);
            }
            public ProBuilderMesh Create(Transform parent,string name)
            {
                var pb=ProBuilderMesh.Create(vertices,faces);pb.name=name;pb.transform.SetParent(parent,false);
                pb.GetComponent<Renderer>().sharedMaterials=materials;pb.ToMesh();pb.Refresh();
                return pb;
            }
        }

        static void Battery(Builder b)
        {
            b.RoundedCase(new Vector3(0,.14f,0),.72f,.73f,.44f,.065f,Navy);
            b.RoundedCase(new Vector3(0,.52f,0),.735f,.055f,.455f,.05f,Metal);
            b.RoundedCase(new Vector3(0,.10f,-.225f),.62f,.40f,.018f,.02f,Blue);
            b.Loft(new[]{(.54f,.085f,Metal),(.58f,.085f,Metal),(.61f,.066f,Metal),(.62f,.066f,Metal)},16,new Vector3(-.18f,0,0));
            b.Loft(new[]{(.54f,.066f,Metal),(.58f,.066f,Metal),(.61f,.055f,Metal),(.62f,.055f,Metal)},16,new Vector3(.18f,0,0));
            foreach(float side in new[]{-1f,1f})b.Loft(new[]{(-.50f,.075f,side<0?Red:Rubber),(-.43f,.075f,side<0?Red:Rubber),(-.40f,.04f,Metal)},16,Vector3.zero,true);
        }
        static void Resistor(Builder b,CircuitPart p)
        {
            int first=p.resistance<15?Brown:p.resistance<25?Red:Orange;
            var bands=new[]{first,Rubber,Rubber,Gold};
            var profile=new List<(float,float,int)>
            {
                (-.57f,.022f,Metal),(-.48f,.022f,Metal),(-.43f,.07f,Metal),(-.37f,.075f,Metal),(-.35f,.13f,Ceramic),(-.25f,.15f,Ceramic)
            };
            for(int i=0;i<4;i++)
            {
                float x=-.23f+i*.14f;profile.Add((x,.15f,bands[i]));profile.Add((x+.028f,.15f,Ceramic));
            }
            profile.AddRange(new[]{(.29f,.15f,Ceramic),(.35f,.13f,Ceramic),(.37f,.075f,Metal),(.43f,.07f,Metal),(.48f,.022f,Metal),(.57f,.022f,Metal)});
            b.Loft(profile,20,Vector3.zero,true);
        }
        static void Lamp(Builder b)
        {
            b.Loft(new[]{(-.14f,.23f,Navy),(-.12f,.28f,Navy),(.06f,.28f,Navy),(.14f,.24f,Metal),(.23f,.24f,Metal),(.28f,.16f,Metal),(.35f,.16f,Metal)},24,Vector3.zero);
            for(int i=0;i<3;i++)b.Loft(new[]{(.15f+i*.045f,.246f,Metal),(.17f+i*.045f,.246f,Metal)},24,Vector3.zero);
        }
        static void Bulb(Builder b)
        {
            b.Loft(new[]{(.32f,.15f,Glass),(.40f,.23f,Glass),(.47f,.32f,Glass),(.65f,.36f,Glass),(.80f,.30f,Glass),(.93f,.19f,Glass),(1.01f,.06f,Glass),(1.02f,.01f,Glass)},32,Vector3.zero);
        }
        static void Meter(Builder b,bool volt)
        {
            b.RoundedCase(Vector3.zero,.98f,.61f,.46f,.075f,Rubber);
            b.RoundedCase(new Vector3(0,.08f,-.239f),.80f,.39f,.022f,.035f,Metal);
            b.RoundedCase(new Vector3(0,.08f,-.255f),.72f,.31f,.012f,.02f,Lcd);
            b.Loft(new[]{(-.31f,.10f,Rubber),(-.28f,.10f,Rubber),(-.26f,.075f,Metal)},20,new Vector3(0,-.19f,-.25f),true);
            foreach(float side in new[]{-1f,1f})b.Loft(new[]{(-.66f,.11f,side<0?Red:Rubber),(-.52f,.11f,side<0?Red:Rubber)},16,new Vector3(0,0,0),true);
        }
        static void Junction(Builder b)
        {
            b.RoundedCase(Vector3.zero,.76f,.65f,.36f,.045f,Navy);
            b.Box(new Vector3(0,0,-.19f),new Vector3(.12f,.48f,.027f),Metal);
            b.Box(new Vector3(0,0,-.206f),new Vector3(.53f,.07f,.028f),Metal);
        }
        static void Switch(Builder b)
        {
            b.RoundedCase(new Vector3(0,-.12f,0),.92f,.19f,.48f,.07f,Navy);
            b.Loft(new[]{(-.025f,.25f,Metal),(.09f,.25f,Metal),(.13f,.17f,Metal)},24,Vector3.zero);
            foreach(float x in new[]{-.35f,.35f})b.Loft(new[]{(-.015f,.038f,Metal),(.005f,.038f,Metal)},12,new Vector3(x,0,-.17f));
        }
        static void Lever(Transform part)
        {
            var pivot=part.Find("XR lever/Lever pivot");if(pivot==null)return;
            var previous=pivot.Find("ProBuilder lever");if(previous!=null)Object.DestroyImmediate(previous.gameObject);
            var b=new Builder();
            b.Loft(new[]{(0f,.045f,Metal),(.36f,.045f,Metal),(.38f,.11f,Rubber),(.51f,.12f,Rubber),(.57f,.075f,Rubber)},20,Vector3.zero);
            b.Create(pivot,"ProBuilder lever");
            var shaft=pivot.Find("Chrome shaft");if(shaft!=null)Object.DestroyImmediate(shaft.gameObject);
            var grip=pivot.Find("Insulated grip");
            if(grip!=null)
            {
                var renderer=grip.GetComponent<MeshRenderer>();if(renderer!=null)Object.DestroyImmediate(renderer);
                var filter=grip.GetComponent<MeshFilter>();if(filter!=null)Object.DestroyImmediate(filter);
            }
        }

        [MenuItem("Tecaverso/Labs/Model Circuit Components with ProBuilder")]
        public static string Configure()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Exit Play Mode first.");
            var lab=Object.FindFirstObjectByType<CircuitLab>();if(lab==null)throw new InvalidOperationException("Open the circuit lab scene.");
            materials=MaterialNames.Select(n=>AssetDatabase.LoadAssetAtPath<Material>(Materials+n+".mat")).ToArray();
            if(materials.Any(m=>m==null))throw new InvalidOperationException("Circuit materials are missing.");
            foreach(var part in lab.parts)
            {
                var root=part.transform.Find("Visual model");if(root==null)continue;
                foreach(var name in new[]{"ProBuilder model","ProBuilder bulb"})
                {
                    var previous=root.Find(name);if(previous!=null)Object.DestroyImmediate(previous.gameObject);
                }
                var b=new Builder();
                switch(part.role)
                {
                    case ElectricalRole.Source:Battery(b);break;
                    case ElectricalRole.Resistor:Resistor(b,part);break;
                    case ElectricalRole.Lamp:Lamp(b);break;
                    case ElectricalRole.Ammeter:Meter(b,false);break;
                    case ElectricalRole.Voltmeter:Meter(b,true);break;
                    case ElectricalRole.Junction:Junction(b);break;
                    case ElectricalRole.Switch:Switch(b);break;
                    default:continue;
                }
                var mesh=b.Create(root,"ProBuilder model");
                foreach(var renderer in root.GetComponentsInChildren<MeshRenderer>(true))
                    renderer.enabled=renderer==mesh.GetComponent<Renderer>()||renderer.GetComponent<TextMeshPro>()!=null;
                if(part.role==ElectricalRole.Source)
                    foreach(var label in root.GetComponentsInChildren<TextMeshPro>(true))
                        label.transform.localPosition=new Vector3(label.transform.localPosition.x,label.transform.localPosition.y,-.26f);
                if(part.role==ElectricalRole.Lamp)
                {
                    if(part.lampRenderer!=null)part.lampRenderer.enabled=false;
                    var original=part.transform.Find("Filament bulb");
                    if(original!=null)
                    {
                        var renderer=original.GetComponent<MeshRenderer>();if(renderer!=null)Object.DestroyImmediate(renderer);
                        var filter=original.GetComponent<MeshFilter>();if(filter!=null)Object.DestroyImmediate(filter);
                        var collider=original.GetComponent<Collider>();if(collider!=null)Object.DestroyImmediate(collider);
                    }
                    var bulb=new Builder();Bulb(bulb);
                    part.lampRenderer=bulb.Create(root,"ProBuilder bulb").GetComponent<Renderer>();
                    if(part.lampLight!=null)part.lampLight.transform.localPosition=new Vector3(0,.68f,0);
                }
                foreach(var renderer in root.GetComponentsInChildren<MeshRenderer>(true).ToArray())
                    if(renderer.GetComponent<ProBuilderMesh>()==null&&renderer.GetComponent<TextMeshPro>()==null)
                        Object.DestroyImmediate(renderer.gameObject);
                if(part.role==ElectricalRole.Switch)Lever(part.transform);
                EditorUtility.SetDirty(part);
            }
            foreach(var control in lab.GetComponentsInChildren<UnityEngine.XR.Content.Interaction.XRLever>(true))Unity3DControlVisuals.Lever(control,2.2f);
            EditorSceneManager.MarkSceneDirty(lab.gameObject.scene);
            EditorSceneManager.SaveScene(lab.gameObject.scene);
            return "Created editable ProBuilder meshes for 10 circuit devices; XR roots and sockets retained.";
        }
    }
}
