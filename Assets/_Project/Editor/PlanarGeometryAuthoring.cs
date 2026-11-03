using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using Tecaverso.Labs.Shapes;
using Tecaverso.Labs.PlanarGeometry;
using Tecaverso.UI;
using Object = UnityEngine.Object;

namespace Tecaverso.Editor
{
    /// <summary>First installation only. Existing scenes must be refined in place.</summary>
    public static class PlanarGeometryAuthoring
    {
        public const string Folder = "Assets/_Project/Laboratories/Mathematics/PlanarGeometry";
        public const string ScenePath = Folder + "/Scenes/PlanarGeometry.unity";
        const string Solids = "Assets/_Project/Laboratories/Mathematics/GeometricShapes";
        static TMP_FontAsset regular, bold;
        static readonly Color Blue = new Color32(40,78,160,255), Ink = new Color32(22,22,22,255), Lavender = new Color32(232,230,254,255);

        [MenuItem("Tecaverso/Instalar Geometria plana (uma vez)")]
        public static void Create()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || Enumerable.Range(0, SceneManager.sceneCount).Any(i => SceneManager.GetSceneAt(i).isDirty))
                throw new InvalidOperationException("Saia de Play Mode e preserve as cenas não salvas antes da instalação.");
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) != null)
                throw new InvalidOperationException("A cena já existe. Edite os objetos existentes; não reinstale.");
            var catalog = AssetDatabase.LoadAssetAtPath<Tecaverso.Hub.HubCatalog>("Assets/_Project/Hub/HubCatalog.asset");
            var math = catalog.disciplines.First(d => d.title == "Matemática");
            if (math.contents.Length >= 6 || math.contents.Any(c => c.scenePath == ScenePath))
                throw new InvalidOperationException("Catálogo cheio ou entrada já cadastrada.");
            var source = SceneManager.GetSceneByPath(Solids + "/Scenes/GeometricShapes.unity");
            bool openedSource = !source.isLoaded;
            if (openedSource) source = EditorSceneManager.OpenScene(Solids + "/Scenes/GeometricShapes.unity", OpenSceneMode.Additive);
            SceneManager.SetActiveScene(source);
            var sourceGallery = source.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<SolidGallery>()).Single();
            var ambientMode = RenderSettings.ambientMode; var ambientSky = RenderSettings.ambientSkyColor;
            var ambientEquator = RenderSettings.ambientEquatorColor; var ambientGround = RenderSettings.ambientGroundColor;
            var ambientIntensity = RenderSettings.ambientIntensity; var skybox = RenderSettings.skybox;
            regular = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Regular SDF.asset");
            bold = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Bold SDF.asset");
            if (!regular || !bold) throw new InvalidOperationException("Fontes da aplicação não encontradas.");
            Ensure(Folder + "/Scenes"); Ensure(Folder + "/Meshes"); Ensure(Folder + "/Materials");
            var surface = AssetDatabase.LoadAssetAtPath<Material>(Solids + "/Materials/Gallery blue.mat");
            var edge = AssetDatabase.LoadAssetAtPath<Material>(Solids + "/Materials/Gallery edges.mat");
            var vertex = AssetDatabase.LoadAssetAtPath<Material>(Solids + "/Materials/Gallery vertices.mat");
            var dimension = MakeMaterial("Auxiliary measures", new Color(1,.76f,.28f));
            var cut = MakeMaterial("Removed rectangle", new Color(1,.34f,.43f));
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            SceneManager.SetActiveScene(scene);
            RenderSettings.ambientMode = ambientMode; RenderSettings.ambientSkyColor = ambientSky;
            RenderSettings.ambientEquatorColor = ambientEquator; RenderSettings.ambientGroundColor = ambientGround;
            RenderSettings.ambientIntensity = ambientIntensity; RenderSettings.skybox = skybox;
            foreach (string name in new[] { "XR Origin (XR Rig)", "XR Interaction Manager", "EventSystem", "Directional Light", "TecaversoStandardRoom", "Shared learning rendering" })
            {
                var original = source.GetRootGameObjects().First(g => g.name == name);
                var clone = Object.Instantiate(original); clone.name = original.name; SceneManager.MoveGameObjectToScene(clone, scene);
            }
            var root = new GameObject("Planar Gallery"); root.SetActive(false);
            var gallery = root.AddComponent<PlanarGallery>(); var exhibits = new List<PlanarExhibit>();
            var definitions = PlanarCatalog.Create();
            for (int index = 0; index < definitions.Count; index++)
            {
                var definition = definitions[index]; var shape = definition.shape;
                var exhibit = Child(root.transform, shape.name).gameObject.AddComponent<PlanarExhibit>();
                Position(exhibit.transform, index); exhibit.circle = definition.IsCircle; exhibit.vertexCount = definition.VertexCount;
                var geometry = Child(exhibit.transform, "Geometry"); geometry.localScale = Vector3.one * .8f;
                Render(geometry, "Surface", Surface(shape.vertices), surface, index + "-surface");
                exhibit.vertexMarkers = Child(geometry, "Vertices").gameObject;
                if (!definition.IsCircle) Render(exhibit.vertexMarkers.transform, "Markers", Markers(shape.vertices), vertex, index + "-vertices");
                exhibit.contourGeometry = Child(geometry, "Contours").gameObject;
                var outline = new LineMesh(.012f); outline.Loop(shape.vertices, false, .014f);
                Render(exhibit.contourGeometry.transform, "Outline", outline.Mesh(), edge, index + "-outline");
                var caption = Object.Instantiate(sourceGallery.exhibits[0].caption.gameObject, exhibit.transform).GetComponent<RectTransform>();
                caption.name = "Name and area"; caption.localPosition = new Vector3(0,1.48f,0); caption.localRotation = Quaternion.identity;
                caption.sizeDelta = new Vector2(1000,500);
                var texts = caption.GetComponentsInChildren<TMP_Text>();
                string[] values = { shape.name, definition.Classification + "\n" + definition.Counts, definition.formula, definition.legend };
                float[] heights = { 94,90,80,130 }; float[] sizes = { 43,29,44,29 };
                for (int j=0;j<texts.Length;j++) { texts[j].text = values[j]; texts[j].fontSize = sizes[j]; texts[j].GetComponent<LayoutElement>().preferredHeight = texts[j].GetComponent<LayoutElement>().minHeight = heights[j]; }
                var perimeter = Canvas(exhibit.transform, "Perimeter", new Vector3(0,-1.05f,0), new Vector2(960,100));
                Text(perimeter, definition.perimeterFormula, 35, true, Vector2.zero, perimeter.sizeDelta);
                exhibit.perimeterLabel = perimeter.gameObject.AddComponent<CanvasGroup>();
                exhibit.dimensionGeometry = Child(geometry, "Auxiliary geometry").gameObject;
                var labels = Canvas(geometry, "Measure labels", Vector3.back * .055f, new Vector2(1400,1400), false);
                exhibit.dimensionLabels = labels.gameObject.AddComponent<CanvasGroup>();
                var lines = new LineMesh();
                foreach (var d in definition.dimensions)
                {
                    lines.Line(d.start, d.end, .012f, true);
                    if (d.rightAngle)
                    {
                        var u = (d.end-d.start).normalized * .11f; var v = d.perpendicular.normalized * .11f;
                        lines.Line(d.start+u, d.start+u+v, .009f); lines.Line(d.start+u+v, d.start+v, .009f);
                    }
                    var point = (d.start+d.end)/2 + d.labelOffset;
                    var label = Text(labels, d.symbol, 64, true, new Vector2(point.x,point.y)*500, new Vector2(130,100));
                    var backdrop = ChildRect(labels, "Measure backing"); backdrop.anchoredPosition = label.rectTransform.anchoredPosition; backdrop.sizeDelta = new Vector2(130,100);
                    var image = backdrop.gameObject.AddComponent<Image>(); image.color = Lavender; image.raycastTarget = false;
                    backdrop.SetSiblingIndex(label.transform.GetSiblingIndex());
                }
                Render(exhibit.dimensionGeometry.transform, "Dimensions", lines.Mesh(), dimension, index + "-dimensions");
                for (int j=0;j<definition.auxiliaryLoops.Length;j++)
                {
                    var loop = new LineMesh(j == 0 ? .034f : .042f); loop.Loop(definition.auxiliaryLoops[j], true, .018f);
                    Render(exhibit.dimensionGeometry.transform, j == 0 ? "Outer rectangle" : "Removed rectangle", loop.Mesh(), j == 0 ? dimension : cut, index + "-aux-" + j);
                }
                exhibit.ShowVertices(false); exhibit.ShowContours(true); exhibit.ShowDimensions(false); exhibits.Add(exhibit);
            }
            gallery.exhibits = exhibits.ToArray();
            var controls = Object.Instantiate(sourceGallery.verticesToggle.transform.parent.parent.gameObject, root.transform).GetComponent<RectTransform>();
            controls.name = "Planar controls"; controls.localPosition = new Vector3(-2.4f,1.6f,-3.6f); controls.localRotation = Quaternion.identity;
            controls.sizeDelta = new Vector2(850,580);
            controls.GetComponentsInChildren<TMP_Text>().First().text = "GEOMETRIA PLANA";
            var toggles = controls.GetComponentsInChildren<Toggle>();
            string[] titles = { "Exibir vértices", "Destacar contornos", "Exibir medidas auxiliares" };
            for (int i=0;i<3;i++)
            {
                toggles[i].name = titles[i]; toggles[i].GetComponentsInChildren<TMP_Text>().Single().text = titles[i];
                toggles[i].onValueChanged = new Toggle.ToggleEvent(); toggles[i].SetIsOnWithoutNotify(i == 1);
            }
            gallery.verticesToggle = toggles[0]; gallery.contoursToggle = toggles[1]; gallery.dimensionsToggle = toggles[2];
            gallery.backButton = controls.GetComponentInChildren<Button>(); gallery.backButton.onClick = new Button.ButtonClickedEvent();
            // The source UI is cloned with its XRI raycaster and full-row toggle hit targets.
            root.SetActive(true);
            math.contents = math.contents.Concat(new[] { new Tecaverso.Hub.HubCatalog.Content {
                title = "Geometria plana", description = "Explore figuras regulares, irregulares e o círculo. Observe áreas, contornos e medidas auxiliares.",
                objective = "Relacionar lados, vértices, perímetro e fórmulas de área.", icon = math.icon, scenePath = ScenePath } }).ToArray();
            EditorUtility.SetDirty(catalog);
            UnityEngine.Canvas.ForceUpdateCanvases();
            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = EditorBuildSettings.scenes.Concat(new[] { new EditorBuildSettingsScene(ScenePath,true) }).ToArray();
            AssetDatabase.SaveAssets();
            if (openedSource) EditorSceneManager.CloseScene(source, true);
            // Keep only the new room loaded to avoid duplicate rigs in an additive Play session.
            for (int i=SceneManager.sceneCount-1;i>=0;i--) { var other=SceneManager.GetSceneAt(i); if (other!=scene) EditorSceneManager.CloseScene(other,true); }
        }
        static void Position(Transform t, int index)
        {
            int[] left={0,1,2,3}, right={7,8,9,10}, back={4,5,6,11,12};
            int k=Array.IndexOf(left,index);
            if(k>=0){t.localPosition=new Vector3(-5.45f,1.55f,-4+k*2.5f);t.localRotation=Quaternion.Euler(0,-90,0);return;}
            k=Array.IndexOf(right,index);
            if(k>=0){t.localPosition=new Vector3(5.45f,1.55f,3.5f-k*2.5f);t.localRotation=Quaternion.Euler(0,90,0);return;}
            k=Array.IndexOf(back,index);t.localPosition=new Vector3((k-2)*2.2f,1.55f,5.45f);
        }
        static void Ensure(string path)
        { if(AssetDatabase.IsValidFolder(path))return;int split=path.LastIndexOf('/');Ensure(path[..split]);AssetDatabase.CreateFolder(path[..split],path[(split+1)..]); }
        static Material MakeMaterial(string name,Color color)
        { var path=Folder+"/Materials/"+name+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);if(m)return m;m=new Material(Shader.Find("Universal Render Pipeline/Unlit"));m.SetColor("_BaseColor",color);AssetDatabase.CreateAsset(m,path);return m; }
        static Transform Child(Transform parent,string name)
        {var t=new GameObject(name).transform;t.SetParent(parent,false);return t;}
        static RectTransform ChildRect(Transform parent,string name)
        {var t=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();t.SetParent(parent,false);return t;}
        static void Render(Transform parent,string name,Mesh mesh,Material material,string assetName)
        {
            mesh.name=assetName;var path=Folder+"/Meshes/"+assetName+".asset";AssetDatabase.CreateAsset(mesh,path);
            var t=Child(parent,name);t.gameObject.AddComponent<MeshFilter>().sharedMesh=mesh;t.gameObject.AddComponent<MeshRenderer>().sharedMaterial=material;
        }
        static Mesh Surface(Vector3[] points)
        {
            var front=ShapeGeometry.Triangulate(points);int n=points.Length;
            var vertices=points.Concat(points).ToArray();var indices=new List<int>();
            for(int i=0;i<front.Length;i+=3){indices.AddRange(new[]{front[i],front[i+1],front[i+2],front[i]+n,front[i+2]+n,front[i+1]+n});}
            var m=new Mesh{vertices=vertices,triangles=indices.ToArray()};m.RecalculateNormals();m.RecalculateBounds();return m;
        }
        static Mesh Markers(Vector3[] points)
        {
            var temp=GameObject.CreatePrimitive(PrimitiveType.Sphere);var primitive=temp.GetComponent<MeshFilter>().sharedMesh;
            var mesh=new Mesh();mesh.CombineMeshes(points.Select(v=>new CombineInstance{mesh=primitive,transform=Matrix4x4.TRS(v,Quaternion.identity,Vector3.one*.09f)}).ToArray());
            Object.DestroyImmediate(temp);return mesh;
        }
        static RectTransform Canvas(Transform parent,string name,Vector3 position,Vector2 size,bool backing=true)
        {
            var r=ChildRect(parent,name);r.localPosition=position;r.localScale=Vector3.one*.002f;r.sizeDelta=size;
            r.gameObject.AddComponent<Canvas>().renderMode=RenderMode.WorldSpace;
            if(backing){var image=r.gameObject.AddComponent<Image>();image.color=Lavender;image.raycastTarget=false;}return r;
        }
        static TMP_Text Text(RectTransform parent,string value,float size,bool strong,Vector2 position,Vector2 dimensions)
        {
            var r=ChildRect(parent,value);r.anchoredPosition=position;r.sizeDelta=dimensions;
            var t=r.gameObject.AddComponent<TextMeshProUGUI>();t.text=value;t.font=strong?bold:regular;t.fontSize=size;t.color=strong?Blue:Ink;
            t.alignment=TextAlignmentOptions.Center;t.raycastTarget=false;t.overflowMode=TextOverflowModes.Ellipsis;return t;
        }
        sealed class LineMesh
        {
            readonly List<Vector3> vertices=new();readonly List<int> triangles=new();
            readonly float layerDepth;
            public LineMesh(float layerDepth=.026f){this.layerDepth=layerDepth;}
            public void Line(Vector3 a,Vector3 b,float width,bool dashed=false)
            {
                if(dashed){int n=Mathf.Max(1,Mathf.CeilToInt(Vector3.Distance(a,b)/.12f));for(int i=0;i<n;i++)Line(Vector3.Lerp(a,b,(float)i/n),Vector3.Lerp(a,b,(i+.62f)/n),width);return;}
                var offset=Vector3.Cross(Vector3.forward,b-a).normalized*width/2;
                foreach(float depth in new[]{-layerDepth,layerDepth})
                {
                    int i=vertices.Count;var z=Vector3.forward*depth;
                    vertices.AddRange(new[]{a-offset+z,a+offset+z,b+offset+z,b-offset+z});
                    triangles.AddRange(new[]{i,i+1,i+2,i,i+2,i+3,i,i+2,i+1,i,i+3,i+2});
                }
            }
            public void Loop(Vector3[] points,bool dashed,float width)
            {for(int i=0;i<points.Length;i++)Line(points[i],points[(i+1)%points.Length],width,dashed);}
            public Mesh Mesh(){var m=new Mesh();m.SetVertices(vertices);m.SetTriangles(triangles,0);m.RecalculateNormals();m.RecalculateBounds();return m;}
        }
    }
}
