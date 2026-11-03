using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Tecaverso.Labs.Shapes;
using Object = UnityEngine.Object;

namespace Tecaverso.Editor
{
    /// <summary>One-time, scoped migration of the retired study UI. Never rebuilds the room or XR rig.</summary>
    public static class ShapesAuthoring
    {
        public const string Folder = "Assets/_Project/Laboratories/Mathematics/GeometricShapes";
        public const string ScenePath = Folder + "/Scenes/GeometricShapes.unity";
        static TMP_FontAsset regular, bold;
        static readonly Color Blue = new Color32(40, 78, 160, 255);
        static readonly Color Ink = new Color32(22, 22, 22, 255);
        static readonly Color Lavender = new Color32(232, 230, 254, 255);

        [MenuItem("Tecaverso/Migrar Formas para galeria simples (uma vez)")]
        public static void SimplifyGallery()
        {
            var scene = SceneManager.GetActiveScene();
            if (EditorApplication.isPlayingOrWillChangePlaymode || scene.path != ScenePath || scene.isDirty)
                throw new InvalidOperationException("Abra a cena Formas Geométricas salva, fora de Play Mode.");
            if (Object.FindFirstObjectByType<SolidGallery>() != null)
                throw new InvalidOperationException("Galeria já migrada. Edite os objetos existentes; não reinstale.");
            var old = Object.FindFirstObjectByType<ShapesLab>();
            if (old == null) throw new InvalidOperationException("Lab anterior não encontrado.");
            regular = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Regular SDF.asset");
            bold = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Bold SDF.asset");
            if (!regular || !bold) throw new InvalidOperationException("Fontes do projeto não encontradas.");
            var definitions = SolidGalleryCatalog.Create();
            Ensure(Folder + "/Meshes");
            var surface = Material("Gallery blue", new Color(.12f, .32f, .7f), false);
            var edge = Material("Gallery edges", new Color(.25f, .85f, 1), true);
            var vertex = Material("Gallery vertices", new Color(1, .64f, .16f), true);
            var root = old.gameObject;
            Undo.RegisterFullObjectHierarchyUndo(root, "Simplificar galeria");
            for (int i = root.transform.childCount - 1; i >= 0; i--) Undo.DestroyObjectImmediate(root.transform.GetChild(i).gameObject);
            Undo.DestroyObjectImmediate(old);
            var gallery = root.AddComponent<SolidGallery>();
            root.name = "Solid Gallery";
            var exhibits = new List<SolidExhibit>();
            for (int i = 0; i < definitions.Count; i++)
            {
                var def = definitions[i]; var data = def.shape;
                var exhibit = new GameObject(data.name).AddComponent<SolidExhibit>();
                exhibit.transform.SetParent(root.transform, false);
                int[] rear = { 1, 0, 4, 6, 8, 9, 10 }, front = { 2, 3, 5, 7 };
                int rearIndex = Array.IndexOf(rear, i);
                exhibit.transform.localPosition = rearIndex >= 0
                    ? new Vector3((rearIndex - 3) * 1.85f, 1.75f, 4.25f)
                    : new Vector3((Array.IndexOf(front, i) - 1.5f) * 3, 1.4f, .3f);
                exhibit.geometry = Child(exhibit.transform, "Geometry");
                exhibit.geometry.localScale = Vector3.one * .8f;
                exhibit.geometry.localRotation = Quaternion.Euler(0, 20, 0);
                var solidMesh = Surface(data);
                Render(exhibit.geometry, "Surface", SaveMesh(solidMesh, i + "-surface"), surface);
                exhibit.vertices = Child(exhibit.geometry, "Vertices").gameObject;
                exhibit.edges = Child(exhibit.geometry, "Edges").gameObject;
                if (data.curved == "")
                {
                    exhibit.faceCount = data.faces.Length; exhibit.edgeCount = data.Edges.Length; exhibit.vertexCount = data.vertices.Length;
                    Render(exhibit.edges.transform, "Geometric edges", SaveMesh(Edges(data), i + "-edges"), edge);
                    Render(exhibit.vertices.transform, "Geometric vertices", SaveMesh(Vertices(data), i + "-vertices"), vertex);
                }
                exhibit.ShowVertices(false); exhibit.ShowEdges(true);
                Collider target;
                if (data.curved == "sphere")
                    target = exhibit.geometry.gameObject.AddComponent<SphereCollider>();
                else
                {
                    var collider = exhibit.geometry.gameObject.AddComponent<MeshCollider>();
                    collider.sharedMesh = solidMesh; collider.convex = true; target = collider;
                }
                exhibit.hoverTarget = exhibit.geometry.gameObject.AddComponent<XRSimpleInteractable>();
                exhibit.hoverTarget.colliders.Add(target);
                var caption = Panel(exhibit.transform, "Name and volume", new Vector3(0, 1.35f, 0), new Vector2(860, 450), false);
                exhibit.caption = caption;
                var body = Body(caption, 24);
                bool regularSolid = new[] { "Tetraedro", "Octaedro", "Dodecaedro", "Icosaedro" }.Contains(data.name);
                Label(body, data.name + (regularSolid ? " regular" : ""), 108, 43, bold, Ink);
                string topology = data.curved == "sphere" ? "Superfície curva" : $"{exhibit.faceCount} faces · {exhibit.edgeCount} arestas · {exhibit.vertexCount} vértices";
                Label(body, topology, 48, 27, regular, Ink);
                Label(body, def.formula, 84, 43, bold, Blue);
                Label(body, def.legend, 110, 29, regular, Ink);
                exhibits.Add(exhibit);
            }
            gallery.exhibits = exhibits.ToArray();
            var controls = Panel(root.transform, "Gallery controls", new Vector3(-4.8f, 1.65f, -3), new Vector2(720, 580), true);
            var content = Body(controls, 32);
            Label(content, "FORMAS GEOMÉTRICAS", 70, 32, bold, Ink);
            gallery.verticesToggle = Toggle(content, "Exibir vértices", false);
            gallery.edgesToggle = Toggle(content, "Exibir arestas", true);
            gallery.rotationToggle = Toggle(content, "Girar sólidos", true);
            var button = Rect(content, "Voltar ao Hub"); Height(button, 88);
            var background = button.gameObject.AddComponent<Image>(); background.color = Blue;
            gallery.backButton = button.gameObject.AddComponent<Button>(); gallery.backButton.targetGraphic = background;
            var text = Label(button, "VOLTAR AO HUB", 88, 32, bold, Color.white);
            Object.DestroyImmediate(text.GetComponent<LayoutElement>()); Stretch(text.rectTransform);
            var catalog = AssetDatabase.LoadAssetAtPath<Tecaverso.Hub.HubCatalog>("Assets/_Project/Hub/HubCatalog.asset");
            var entry = catalog.disciplines.SelectMany(d => d.contents).First(c => c.scenePath == ScenePath);
            entry.description = "Caminhe entre os sólidos, observe suas fórmulas de volume e destaque vértices e arestas.";
            entry.objective = "Reconhecer sólidos e relacionar faces, arestas, vértices e fórmulas de volume.";
            EditorUtility.SetDirty(catalog);
            Canvas.ForceUpdateCanvases();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
        }

        static void Ensure(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            int i = path.LastIndexOf('/'); Ensure(path[..i]); AssetDatabase.CreateFolder(path[..i], path[(i + 1)..]);
        }
        static Material Material(string name, Color color, bool unlit)
        {
            var path = Folder + "/Materials/" + name + ".mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat != null) return mat;
            mat = new Material(Shader.Find(unlit ? "Universal Render Pipeline/Unlit" : "Universal Render Pipeline/Lit"));
            mat.name = name; mat.SetColor("_BaseColor", color);
            if (!unlit) { mat.SetFloat("_Smoothness", .32f); mat.SetFloat("_Cull", 2); }
            AssetDatabase.CreateAsset(mat, path); return mat;
        }
        static Transform Child(Transform parent, string name)
        { var t = new GameObject(name).transform; t.SetParent(parent, false); return t; }
        static Mesh SaveMesh(Mesh mesh, string name)
        { mesh.name = name; AssetDatabase.CreateAsset(mesh, Folder + "/Meshes/" + name + ".asset"); return mesh; }
        static void Render(Transform parent, string name, Mesh mesh, Material material)
        {
            var t = Child(parent, name); t.gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
            t.gameObject.AddComponent<MeshRenderer>().sharedMaterial = material;
        }
        static Mesh Surface(ShapeData data)
        {
            var vertices = new List<Vector3>(); var triangles = new List<int>();
            foreach (var face in data.faces)
            {
                int offset = vertices.Count; vertices.AddRange(face.Select(i => data.vertices[i]));
                for (int i = 1; i < face.Length - 1; i++)
                {
                    var a = vertices[offset]; var b = vertices[offset + i]; var c = vertices[offset + i + 1];
                    bool outward = Vector3.Dot(Vector3.Cross(b - a, c - a), (a + b + c) / 3) > 0;
                    triangles.Add(offset); triangles.Add(offset + (outward ? i : i + 1)); triangles.Add(offset + (outward ? i + 1 : i));
                }
            }
            var mesh = new Mesh(); mesh.SetVertices(vertices); mesh.SetTriangles(triangles, 0); mesh.RecalculateNormals(); mesh.RecalculateBounds();
            if (data.curved == "sphere") mesh.normals = vertices.Select(p => p.normalized).ToArray();
            return mesh;
        }
        static Mesh Edges(ShapeData data)
        {
            var vertices = new List<Vector3>(); var triangles = new List<int>();
            foreach (var (ia, ib) in data.Edges)
            {
                var a = data.vertices[ia]; var b = data.vertices[ib];
                var rotation = Quaternion.FromToRotation(Vector3.up, b - a); int offset = vertices.Count;
                for (int end = 0; end < 2; end++) for (int k = 0; k < 6; k++)
                { float angle = k * Mathf.PI / 3; vertices.Add((end == 0 ? a : b) + rotation * new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * .014f); }
                for (int k = 0; k < 6; k++)
                { int n = (k + 1) % 6; triangles.AddRange(new[] { offset + k, offset + k + 6, offset + n, offset + n, offset + k + 6, offset + n + 6 }); }
            }
            var mesh = new Mesh(); mesh.SetVertices(vertices); mesh.SetTriangles(triangles, 0); mesh.RecalculateNormals(); mesh.RecalculateBounds(); return mesh;
        }
        static Mesh Vertices(ShapeData data)
        {
            var temp = GameObject.CreatePrimitive(PrimitiveType.Sphere); var sphere = temp.GetComponent<MeshFilter>().sharedMesh;
            var instances = data.vertices.Select(v => new CombineInstance { mesh = sphere, transform = Matrix4x4.TRS(v, Quaternion.identity, Vector3.one * .09f) }).ToArray();
            var mesh = new Mesh(); mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; mesh.CombineMeshes(instances); Object.DestroyImmediate(temp); return mesh;
        }
        static RectTransform Rect(Transform parent, string name)
        { var r = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>(); r.SetParent(parent, false); return r; }
        static void Stretch(RectTransform r)
        { r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one; r.offsetMin = r.offsetMax = Vector2.zero; }
        static void Height(RectTransform r, float h)
        { var e = r.gameObject.AddComponent<LayoutElement>(); e.minHeight = e.preferredHeight = h; }
        static RectTransform Panel(Transform parent, string name, Vector3 position, Vector2 size, bool interactive)
        {
            var r = Rect(parent, name); r.localPosition = position; r.localScale = Vector3.one * .002f; r.sizeDelta = size;
            r.gameObject.AddComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            if (interactive) r.gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();
            var background = r.gameObject.AddComponent<Image>(); background.color = Lavender; background.raycastTarget = false;
            return r;
        }
        static RectTransform Body(RectTransform panel, int margin)
        {
            var body = Rect(panel, "Content"); Stretch(body); body.offsetMin = Vector2.one * margin; body.offsetMax = -Vector2.one * margin;
            var layout = body.gameObject.AddComponent<VerticalLayoutGroup>(); layout.spacing = 12;
            layout.childControlHeight = layout.childControlWidth = true; layout.childForceExpandWidth = true; layout.childForceExpandHeight = false;
            return body;
        }
        static TMP_Text Label(Transform parent, string text, float height, float size, TMP_FontAsset font, Color color)
        {
            var r = Rect(parent, text); Height(r, height); var label = r.gameObject.AddComponent<TextMeshProUGUI>();
            label.text = text; label.font = font; label.fontSize = size; label.color = color; label.alignment = TextAlignmentOptions.Center;
            label.raycastTarget = false; label.overflowMode = TextOverflowModes.Ellipsis;
            return label;
        }
        static Toggle Toggle(Transform parent, string label, bool state)
        {
            var r = Rect(parent, label); Height(r, 96);
            var background = r.gameObject.AddComponent<Image>(); background.color = Color.white;
            var toggle = r.gameObject.AddComponent<Toggle>(); toggle.targetGraphic = background;
            var box = Rect(r, "Box"); box.anchorMin = box.anchorMax = new Vector2(0, .5f); box.anchoredPosition = new Vector2(48, 0); box.sizeDelta = new Vector2(48, 48);
            box.gameObject.AddComponent<Image>().color = Blue;
            var check = Rect(box, "Checked"); Stretch(check); check.offsetMin = Vector2.one * 10; check.offsetMax = -Vector2.one * 10;
            var mark = check.gameObject.AddComponent<Image>(); mark.color = Color.white; mark.raycastTarget = false;
            toggle.graphic = mark; toggle.isOn = state;
            var text = Label(r, label, 96, 34, regular, Ink); Object.DestroyImmediate(text.GetComponent<LayoutElement>()); Stretch(text.rectTransform);
            text.rectTransform.offsetMin = new Vector2(92, 0); text.rectTransform.offsetMax = new Vector2(-16, 0); text.alignment = TextAlignmentOptions.MidlineLeft;
            return toggle;
        }
    }
}
