using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object=UnityEngine.Object;

namespace Tecaverso.Editor
{
    public static class AnatomyAssets
    {
        public const string Folder="Assets/_Project/Art/Biology/Anatomy";
        public const string ModelPath=Folder+"/Models/Male_Full_Body_Anatomy_and_Skin_fbx.FBX";
        [Serializable]public class Map {public Group[] groups;public Part[] parts;public MaterialData[] materials;public TextureData[] textures;}
        [Serializable]public class Group {public string id,sourcePrefab;public string[] names;}
        [Serializable]public class Part {public string name;public string[] materials;}
        [Serializable]public class MaterialData {public string guid,name,albedo,normal;public Color color;public float smoothness,bumpScale;}
        [Serializable]public class TextureData {public string guid,source;public bool normal;}
        static Map map;
        static int next;
        public static string Status {get;private set;}="Idle";
        public static Map ReadMap()=>JsonUtility.FromJson<Map>(AssetDatabase.LoadAssetAtPath<TextAsset>(Folder+"/AnatomyGeometry.json").text);
        public static void StartPreparation()
        {
            if(Application.isPlaying)throw new InvalidOperationException("Use Edit Mode.");
            if(AssetDatabase.LoadAssetAtPath<GameObject>(Folder+"/Prefabs/AnatomyVisual.prefab"))throw new InvalidOperationException("Arte já preparada; não regenerar alterações autorais.");
            CellsAssets.Ensure(Folder+"/Textures");CellsAssets.Ensure(Folder+"/Materials");CellsAssets.Ensure(Folder+"/Prefabs");
            map=ReadMap();next=0;EditorApplication.update-=Tick;EditorApplication.update+=Tick;Status="Preparing 0 / "+map.textures.Length;
        }
        static void Tick()
        {
            try
            {
                if(EditorApplication.isCompiling||EditorApplication.isUpdating)return;
                for(int i=0;i<3&&next<map.textures.Length;i++,next++)ExportTexture(map.textures[next]);
                Status="Preparing "+next+" / "+map.textures.Length;
                if(next<map.textures.Length)return;
                EditorApplication.update-=Tick;BuildVisual();Status="Ready: "+map.groups.Length+" groups / "+map.parts.Length+" meshes / "+map.textures.Length+" textures";
            }
            catch(Exception e){EditorApplication.update-=Tick;Status="ERROR: "+e.Message;Debug.LogException(e);}
        }
        static string TexturePath(string guid)=>Folder+"/Textures/"+guid+".png";
        static void ExportTexture(TextureData entry)
        {
            string output=TexturePath(entry.guid);if(File.Exists(output))return;
            string source=Path.GetFullPath(entry.source);var original=new Texture2D(2,2,TextureFormat.RGBA32,false,entry.normal);
            RenderTexture temporary=null;var previous=RenderTexture.active;Texture2D resized=null;
            try
            {
                if(!ImageConversion.LoadImage(original,File.ReadAllBytes(source),true))throw new InvalidOperationException("PNG inválido: "+source);
                int limit=entry.normal?512:1024;float factor=Mathf.Min(1,limit/(float)Mathf.Max(original.width,original.height));
                int width=Mathf.Max(4,Mathf.RoundToInt(original.width*factor)),height=Mathf.Max(4,Mathf.RoundToInt(original.height*factor));
                temporary=RenderTexture.GetTemporary(width,height,0,RenderTextureFormat.ARGB32,entry.normal?RenderTextureReadWrite.Linear:RenderTextureReadWrite.sRGB);
                Graphics.Blit(original,temporary);RenderTexture.active=temporary;
                resized=new Texture2D(width,height,TextureFormat.RGBA32,false,entry.normal);resized.ReadPixels(new Rect(0,0,width,height),0,0);resized.Apply();
                File.WriteAllBytes(output,resized.EncodeToPNG());
            }
            finally{RenderTexture.active=previous;if(temporary)RenderTexture.ReleaseTemporary(temporary);Object.DestroyImmediate(original);if(resized)Object.DestroyImmediate(resized);}
            AssetDatabase.ImportAsset(output,ImportAssetOptions.ForceSynchronousImport);
            var importer=(TextureImporter)AssetImporter.GetAtPath(output);importer.textureType=entry.normal?TextureImporterType.NormalMap:TextureImporterType.Default;
            importer.sRGBTexture=!entry.normal;importer.maxTextureSize=entry.normal?512:1024;importer.mipmapEnabled=true;importer.isReadable=false;importer.textureCompression=TextureImporterCompression.Compressed;
            importer.filterMode=FilterMode.Trilinear;importer.anisoLevel=2;importer.SaveAndReimport();
        }
        static Material MaterialFor(MaterialData entry)
        {
            string path=Folder+"/Materials/"+entry.name+".mat";var material=AssetDatabase.LoadAssetAtPath<Material>(path);if(material)return material;
            material=new Material(Shader.Find("Universal Render Pipeline/Lit")){name=entry.name};material.SetColor("_BaseColor",entry.color);material.SetFloat("_Smoothness",entry.smoothness);
            if(!string.IsNullOrEmpty(entry.albedo))material.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(TexturePath(entry.albedo)));
            if(!string.IsNullOrEmpty(entry.normal))
            {
                material.SetTexture("_BumpMap",AssetDatabase.LoadAssetAtPath<Texture2D>(TexturePath(entry.normal)));material.SetFloat("_BumpScale",entry.bumpScale);material.EnableKeyword("_NORMALMAP");
            }
            AssetDatabase.CreateAsset(material,path);return material;
        }
        static void BuildVisual()
        {
            var source=AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);
            var meshes=source.GetComponentsInChildren<MeshFilter>(true).ToDictionary(x=>x.name);
            var definitions=map.parts.ToDictionary(x=>x.name);var materials=map.materials.ToDictionary(x=>x.guid,MaterialFor);
            var preview=EditorSceneManager.NewPreviewScene();var root=new GameObject("AnatomyVisual");UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(root,preview);
            root.transform.localRotation=source.transform.localRotation;root.transform.localScale=source.transform.localScale;
            try
            {
                foreach(var group in map.groups)
                {
                    var groupObject=new GameObject(group.id);groupObject.transform.SetParent(root.transform,false);
                    foreach(var name in group.names)
                    {
                        var original=meshes[name];if(original.transform.parent!=source.transform)throw new InvalidOperationException("Hierarquia FBX alterada; revisar pose canônica: "+name);
                        var part=new GameObject(name);part.transform.SetParent(groupObject.transform,false);
                        part.transform.localPosition=original.transform.localPosition;part.transform.localRotation=original.transform.localRotation;part.transform.localScale=original.transform.localScale;
                        part.AddComponent<MeshFilter>().sharedMesh=original.sharedMesh;part.AddComponent<MeshRenderer>().sharedMaterials=definitions[name].materials.Select(g=>materials[g]).ToArray();
                    }
                }
                PrefabUtility.SaveAsPrefabAsset(root,Folder+"/Prefabs/AnatomyVisual.prefab");AssetDatabase.SaveAssets();
            }
            finally{Object.DestroyImmediate(root);EditorSceneManager.ClosePreviewScene(preview);}
        }
    }
}
