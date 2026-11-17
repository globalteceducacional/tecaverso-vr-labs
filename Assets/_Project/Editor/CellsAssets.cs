using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tecaverso.Editor
{
    // One-time migration of geometry only. No legacy rig, callouts, physics or scripts.
    public static class CellsAssets
    {
        public const string Folder = "Assets/_Project/Art/Biology/Cells";
        [Serializable] public class Map { public Cell[] cells; }
        [Serializable] public class Cell { public string name; public Node[] nodes; }
        [Serializable] public class Reference { public long fileID; public string guid; }
        [Serializable] public class Node
        {
            public string id, go, parent, name;
            public Vector3 position, scale;
            public Quaternion rotation;
            public Reference mesh;
            public Reference[] materials;
        }
        static readonly Dictionary<string,Object> resolved = new();
        static readonly Dictionary<string,Material> materials = new();
        public static void Prepare()
        {
            Ensure(Folder + "/Prefabs"); Ensure(Folder + "/Materials");
            var map = JsonUtility.FromJson<Map>(AssetDatabase.LoadAssetAtPath<TextAsset>(Folder + "/GeometryMap.json").text);
            foreach (var cell in map.cells)
            {
                string path = Folder + "/Prefabs/" + cell.name + ".prefab";
                if (AssetDatabase.LoadAssetAtPath<GameObject>(path)) continue;
                var preview = UnityEditor.SceneManagement.EditorSceneManager.NewPreviewScene();
                var nodes = cell.nodes.ToDictionary(n => n.id, n => {
                    var g = new GameObject(n.name);
                    UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(g, preview);
                    return g.transform;
                });
                foreach (var n in cell.nodes)
                {
                    var t = nodes[n.id];
                    if (nodes.TryGetValue(n.parent, out var parent)) t.SetParent(parent, false);
                    t.localPosition = n.position; t.localRotation = n.rotation; t.localScale = n.scale;
                    if (n.mesh == null || n.mesh.fileID == 0) continue;
                    var mesh = Resolve(n.mesh) as Mesh;
                    if (!mesh) throw new InvalidOperationException("Legacy mesh not resolved: " + cell.name + "/" + n.name);
                    t.gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
                    t.gameObject.AddComponent<MeshRenderer>().sharedMaterials = n.materials.Select(MaterialFor).ToArray();
                }
                var root = nodes.Values.Single(t => !t.parent);
                PrefabUtility.SaveAsPrefabAsset(root.gameObject, path);
                Object.DestroyImmediate(root.gameObject);
                UnityEditor.SceneManagement.EditorSceneManager.ClosePreviewScene(preview);
            }
            AssetDatabase.SaveAssets();
        }
        static Object Resolve(Reference r)
        {
            string key = r.guid + ":" + r.fileID;
            if (resolved.TryGetValue(key, out var value)) return value;
            foreach (var item in AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GUIDToAssetPath(r.guid)))
            {
                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(item, out string guid, out long id) && id == r.fileID)
                    return resolved[key] = item;
            }
            return null;
        }
        static Material MaterialFor(Reference reference)
        {
            string key = reference.guid + ":" + reference.fileID;
            if (materials.TryGetValue(key, out var existing)) return existing;
            var source = Resolve(reference) as Material;
            if (!source) throw new InvalidOperationException("Legacy material not resolved: " + key);
            string path = Folder + "/Materials/" + reference.fileID + ".mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (!material)
            {
                material = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = source.name };
                material.SetColor("_BaseColor", source.HasProperty("_Color") ? source.GetColor("_Color") : source.color);
                if (source.HasProperty("_MainTex")) material.SetTexture("_BaseMap", source.GetTexture("_MainTex"));
                if (source.HasProperty("_BumpMap"))
                {
                    material.SetTexture("_BumpMap", source.GetTexture("_BumpMap"));
                    if (source.GetTexture("_BumpMap")) material.EnableKeyword("_NORMALMAP");
                }
                material.SetFloat("_Smoothness", .28f);
                AssetDatabase.CreateAsset(material, path);
            }
            return materials[key] = material;
        }
        public static object Describe()
        {
            return new[] { "Animal_Cell", "Plant_Cell" }.Select(name => {
                var asset = AssetDatabase.LoadAssetAtPath<GameObject>(Folder + "/Prefabs/" + name + ".prefab");
                return new { name, rootRotation = asset.transform.localEulerAngles,
                    parts = asset.GetComponentsInChildren<MeshRenderer>(true).Select(r => new {
                        path = AnimationUtility.CalculateTransformPath(r.transform, asset.transform),
                        center = r.bounds.center, size = r.bounds.size,
                        colors = r.sharedMaterials.Select(m => new { m.name, color = m.GetColor("_BaseColor") }).ToArray()
                    }).ToArray() };
            }).ToArray();
        }
        public static void Ensure(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            int split = path.LastIndexOf('/'); Ensure(path[..split]);
            AssetDatabase.CreateFolder(path[..split], path[(split + 1)..]);
        }
    }
}
