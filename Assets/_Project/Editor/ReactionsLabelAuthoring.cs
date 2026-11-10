using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using Tecaverso.Labs.Chemistry.Reactions;
using Object=UnityEngine.Object;

namespace Tecaverso.Editor
{
    /// <summary>One-time local correction: physical labels and fixed shelf signage. Never rebuilds the room.</summary>
    public static class ReactionsLabelAuthoring
    {
        const string Folder="Assets/_Project/Laboratories/Chemistry/Reactions";
        static Material LabelMaterial(string id)
        {
            string path=Folder+"/Art/Labels/"+id+".png";
            var importer=(TextureImporter)AssetImporter.GetAtPath(path);
            if(!importer)throw new Exception("Generate label textures before migrating.");
            importer.mipmapEnabled=true;importer.wrapMode=TextureWrapMode.Clamp;importer.anisoLevel=4;importer.maxTextureSize=512;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.SaveAndReimport();
            string matPath=Folder+"/Art/Labels/"+id+".mat";
            var mat=AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if(!mat){mat=new Material(AssetDatabase.LoadAssetAtPath<Material>(Folder+"/Materials/Grafite.mat"));AssetDatabase.CreateAsset(mat,matPath);}
            mat.SetColor("_BaseColor",Color.white);mat.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(path));mat.SetFloat("_Smoothness",.15f);mat.SetFloat("_Metallic",0);EditorUtility.SetDirty(mat);return mat;
        }
        static Mesh MakeBand()
        {
            const string path=Folder+"/Art/Labels/CurvedLabel.asset";
            var existing=AssetDatabase.LoadAssetAtPath<Mesh>(path);if(existing)return existing;
            var v=new List<Vector3>();var uv=new List<Vector2>();var tri=new List<int>();
            for(int i=0;i<=32;i++)for(int j=0;j<2;j++)
            {
                float a=Mathf.Lerp(-60,60,i/32f)*Mathf.Deg2Rad,y=j==0?-.195f:-.025f;
                float r=.1100f+(y+.232f)/.246f*.0032f+.0008f;
                v.Add(new Vector3(Mathf.Sin(a)*r,y,-Mathf.Cos(a)*r));uv.Add(new Vector2(i/32f,j));
                if(i<32&&j==0){int k=i*2;tri.AddRange(new[]{k,k+1,k+2,k+2,k+1,k+3});}
            }
            var mesh=new Mesh{name="Rótulo curvo aderente"};mesh.SetVertices(v);mesh.SetUVs(0,uv);mesh.SetTriangles(tri,0);mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,path);return mesh;
        }
        static void AddLabel(Transform parent,Mesh mesh,Material material)
        {
            var g=new GameObject("Rótulo impresso",typeof(MeshFilter),typeof(MeshRenderer));g.transform.SetParent(parent,false);
            g.GetComponent<MeshFilter>().sharedMesh=mesh;g.GetComponent<Renderer>().sharedMaterial=material;
        }
        public static void Upgrade()
        {
            var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();var b=Object.FindFirstObjectByType<ReactionsBench>();
            if(Application.isPlaying||scene.path!=ReactionsAuthoring.ScenePath||!b)throw new Exception("Open Reactions outside Play Mode.");
            if(b.instruments.Any(t=>t.transform.Find("Rótulo impresso")))throw new Exception("Already migrated; use targeted edits.");
            System.IO.Directory.CreateDirectory("Temp/ReactionsMigration");EditorSceneManager.SaveScene(scene,"Temp/ReactionsMigration/Reactions-before-labels.unity",true);
            var band=MakeBand();
            foreach(var t in b.instruments)
            {
                if(t.spoon)
                {
                    var plate=new GameObject("Fórmula no cabo",typeof(MeshFilter),typeof(MeshRenderer));plate.transform.SetParent(t.transform,false);
                    // Surface of the existing handle, not a floating card; no new collider.
                    var mesh=new Mesh{name="Impressão no cabo"};
                    mesh.vertices=new[]{new Vector3(-.012f,.0103f,-.045f),new Vector3(-.012f,.0103f,.065f),new Vector3(.012f,.0103f,-.045f),new Vector3(.012f,.0103f,.065f)};
                    mesh.uv=new[]{new Vector2(0,0),new Vector2(1,0),new Vector2(0,1),new Vector2(1,1)};mesh.triangles=new[]{0,1,2,2,1,3};mesh.RecalculateNormals();mesh.RecalculateBounds();
                    string path=Folder+"/Art/Labels/HandlePrint.asset";var saved=AssetDatabase.LoadAssetAtPath<Mesh>(path);if(!saved){AssetDatabase.CreateAsset(mesh,path);saved=mesh;}else Object.DestroyImmediate(mesh);
                    plate.GetComponent<MeshFilter>().sharedMesh=saved;plate.GetComponent<Renderer>().sharedMaterial=LabelMaterial(t.reagentId+"-spoon");
                    if(t.label){var old=t.label.transform.parent.gameObject;t.label=null;Object.DestroyImmediate(old);}EditorUtility.SetDirty(t);continue;
                }
                AddLabel(t.transform,band,LabelMaterial(t.reagentId));
                var panel=(RectTransform)t.label.transform.parent;panel.SetParent(b.transform,true);panel.name="Identificação fixa · "+t.reagentId;
                panel.rotation=Quaternion.identity;panel.localScale=Vector3.one*.001f;panel.sizeDelta=new Vector2(430,130);
                if(t.indicator)panel.position=new Vector3(1.24f,.85f,-.274f);
                else{int i=Array.FindIndex(ReactionWorkbench.Reagents,r=>r.Id==t.reagentId),k=i%3;panel.position=new Vector3(t.home.position.x,.79f+k*.25f,1.080f+k*.58f);}
                t.label.fontSize=20;t.label.enableAutoSizing=false;t.label.overflowMode=TextOverflowModes.Ellipsis;t.label.raycastTarget=false;
                EditorUtility.SetDirty(t);
            }
            EditorUtility.SetDirty(b);EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
        }
    }
}
