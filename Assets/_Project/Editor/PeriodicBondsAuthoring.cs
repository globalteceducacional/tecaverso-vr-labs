using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Tecaverso.Hub;
using Tecaverso.Labs.Chemistry.PeriodicBonds;
using Object = UnityEngine.Object;

namespace Tecaverso.Editor
{
    /// <summary>Instala somente a cena nova; nunca reconstrói cenas existentes.</summary>
    public static class PeriodicBondsAuthoring
    {
        const string Folder = "Assets/_Project/Laboratories/Chemistry/PeriodicBonds";
        const string ScenePath = Folder + "/Scenes/PeriodicBonds.unity";
        const string SourcePath = "Assets/_Project/Laboratories/Mathematics/PlanarGeometry/Scenes/PlanarGeometry.unity";
        static readonly Color Purple = new Color32(118,72,232,255), Blue = new Color32(40,78,160,255);
        static readonly Color Lavender = new Color32(232,230,254,255), Ink = new Color32(22,22,22,255);
        static TMP_FontAsset regular, bold;

        [MenuItem("Tecaverso/Instalar Tabela periódica e ligações (uma vez)")]
        public static void Create()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) throw new InvalidOperationException("Saia do Play Mode antes de instalar a sala.");
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath)) throw new InvalidOperationException("A cena já existe. Refine os objetos existentes.");
            regular = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Regular SDF.asset");
            bold = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Bold SDF.asset");
            if (!regular || !bold) throw new InvalidOperationException("Fontes Montserrat ausentes.");
            Ensure(Folder + "/Scenes"); Ensure(Folder + "/Materials");
            var source = SceneManager.GetSceneByPath(SourcePath);
            bool openedSource = !source.isLoaded;
            if (openedSource) source = EditorSceneManager.OpenScene(SourcePath, OpenSceneMode.Additive);
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            SceneManager.SetActiveScene(scene);
            foreach (string name in new[] { "XR Origin (XR Rig)", "XR Interaction Manager", "EventSystem", "Directional Light", "TecaversoStandardRoom", "Shared learning rendering" })
            {
                var original = source.GetRootGameObjects().First(g => g.name == name);
                var clone = Object.Instantiate(original); clone.name = name; SceneManager.MoveGameObjectToScene(clone, scene);
            }
            var root = new GameObject("Tabela periódica e ligações"); root.SetActive(false);
            var lab = root.AddComponent<PeriodicBondsLab>();
            var materials = new Material[BondCatalog.Active.Length];
            for (int i = 0; i < materials.Length; i++) materials[i] = Material("Átomo " + BondCatalog.Symbols[i], BondCatalog.Colors[i]);
            var pedestal = Material("Pedestais violeta", new Color(.13f,.10f,.25f));
            var light = Material("Traços de ligação", new Color(.69f,.9f,1f));
            var electron = Material("Elétrons", new Color(1f,.83f,.2f));

            // Three observation zones: periodic table, assembly, and enlarged result.
            PeriodicTableBoardAuthoring.Build(root.transform,lab);

            var anchors = new Transform[materials.Length]; var templates = new AtomPiece[materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                var stand = Primitive(root.transform,"Suporte " + BondCatalog.Symbols[i], PrimitiveType.Cylinder,
                    new Vector3(-4.15f, .77f, -2.6f + i * 1.2f), new Vector3(.58f,.11f,.58f), pedestal);
                Label3D(stand.transform, BondCatalog.Symbols[i], new Vector3(0,.32f,-.34f), .25f, Color.white);
                anchors[i] = Child(root.transform,"Reposição " + BondCatalog.Symbols[i]);
                anchors[i].position = stand.transform.position + Vector3.up * .3f;
                var atom = GameObject.CreatePrimitive(PrimitiveType.Sphere); atom.name = "Modelo " + BondCatalog.Symbols[i];
                atom.transform.SetParent(root.transform,false); atom.transform.position = anchors[i].position;
                atom.transform.localScale = Vector3.one * .27f; atom.GetComponent<Renderer>().sharedMaterial = materials[i];
                var rb = atom.AddComponent<Rigidbody>(); rb.useGravity = false; rb.isKinematic = true;
                var grab = atom.AddComponent<XRGrabInteractable>(); grab.movementType = XRBaseInteractable.MovementType.Instantaneous;
                var piece = atom.AddComponent<AtomPiece>(); piece.element = (ElementKind)i; piece.lab = lab;
                Label3D(atom.transform,BondCatalog.Symbols[i],new Vector3(0,0,-.52f),.55f,i==0?Ink:Color.white);
                atom.SetActive(false); templates[i] = piece;
            }
            lab.atomPrefabs = templates; lab.supplyAnchors = anchors;

            Primitive(root.transform,"Base de montagem",PrimitiveType.Cube,new Vector3(0,.78f,.15f),new Vector3(3.75f,.15f,1.1f),pedestal);
            var sockets = new BondSocket[BondAssembly.SlotCount];
            for (int i = 0; i < sockets.Length; i++)
            {
                var socketRoot = Child(root.transform,"Encaixe " + (i+1));
                socketRoot.position = new Vector3(-1.2f + i*.8f,1.06f,.15f);
                var trigger = socketRoot.gameObject.AddComponent<SphereCollider>(); trigger.radius = .22f; trigger.isTrigger = true;
                var socket = socketRoot.gameObject.AddComponent<BondSocket>(); socket.lab = lab; socket.index = i;
                var marker = Primitive(socketRoot,"Anel guia",PrimitiveType.Cylinder,Vector3.down*.13f,new Vector3(.28f,.025f,.28f),light);
                marker.GetComponent<Collider>().enabled = false;
                sockets[i] = socket;
            }
            lab.sockets = sockets;

            Primitive(root.transform,"Base da vitrine",PrimitiveType.Cylinder,new Vector3(3.75f,.85f,.15f),new Vector3(1.8f,.15f,1.8f),pedestal);
            var displayRoot = Child(root.transform,"Estrutura ampliada · girar com pega XR");
            displayRoot.position = new Vector3(3.75f,2.04f,.15f);
            var displayRb = displayRoot.gameObject.AddComponent<Rigidbody>(); displayRb.isKinematic = true; displayRb.useGravity = false;
            var displayCollider = displayRoot.gameObject.AddComponent<SphereCollider>(); displayCollider.radius = 1.05f;
            var displayGrab = displayRoot.gameObject.AddComponent<XRGrabInteractable>(); displayGrab.trackPosition = false;
            var display = displayRoot.gameObject.AddComponent<BondDisplay>(); display.model = Child(displayRoot,"Modelo");
            display.atomMaterials = materials; display.bondMaterial = light; display.electronMaterial = electron; display.font = bold;
            lab.display = display;

            var panel = Canvas(root.transform,"Controles e leitura",new Vector3(-2.9f,1.65f,-3.2f),Quaternion.identity,new Vector2(1700,570));
            Background(panel,Lavender);
            Label(panel,"LIGAÇÕES QUÍMICAS",new Vector2(0,235),new Vector2(1550,70),52,true,Purple);
            lab.assemblyInfo = Label(panel,"MONTAGEM",new Vector2(-380,105),new Vector2(850,65),31,true,Ink);
            lab.resultInfo = Label(panel,"Encaixe átomos para investigar uma estrutura.",new Vector2(350,60),new Vector2(760,230),27,false,Ink);
            lab.statusInfo = Label(panel,"Pegue átomos nos suportes e encaixe na base central.",new Vector2(0,-115),new Vector2(1550,70),26,false,Blue);
            lab.electronsToggle = Toggle(panel,"Exibir elétrons de valência",new Vector2(-475,-215));
            lab.dismantleButton = Button(panel,"Desmontar",new Vector2(190,-215),new Vector2(320,70),Blue,27);
            lab.backButton = Button(panel,"Voltar ao Hub",new Vector2(575,-215),new Vector2(400,70),Blue,27);
            root.SetActive(true);
            EditorSceneManager.SaveScene(scene,ScenePath);
            var catalog = AssetDatabase.LoadAssetAtPath<HubCatalog>("Assets/_Project/Hub/HubCatalog.asset");
            var chemistry = catalog.disciplines.First(d=>d.title=="Química");
            chemistry.contents = chemistry.contents.Concat(new[]{new HubCatalog.Content {
                title="Tabela periódica e ligações", description="Selecione elementos, monte moléculas e compare uma rede iônica.",
                objective="Relacionar elétrons de valência, composição e tipos de ligação.", icon=chemistry.icon, scenePath=ScenePath
            }}).ToArray();
            EditorUtility.SetDirty(catalog);
            EditorBuildSettings.scenes = EditorBuildSettings.scenes.Concat(new[]{new EditorBuildSettingsScene(ScenePath,true)}).ToArray();
            AssetDatabase.SaveAssets();
            if(openedSource) EditorSceneManager.CloseScene(source,true);
            Debug.Log("Sala de Tabela periódica e ligações instalada. Cenas abertas do usuário foram preservadas.");
        }
        static void Ensure(string path)
        { if(AssetDatabase.IsValidFolder(path))return;int split=path.LastIndexOf('/');Ensure(path[..split]);AssetDatabase.CreateFolder(path[..split],path[(split+1)..]); }
        static Material Material(string name,Color color)
        {
            string path=Folder+"/Materials/"+name+".mat";
            var m=AssetDatabase.LoadAssetAtPath<Material>(path);if(m)return m;
            m=new Material(Shader.Find("Universal Render Pipeline/Lit"));m.SetColor("_BaseColor",color);AssetDatabase.CreateAsset(m,path);return m;
        }
        static Transform Child(Transform parent,string name)
        { var t=new GameObject(name).transform;t.SetParent(parent,false);return t; }
        static GameObject Primitive(Transform parent,string name,PrimitiveType type,Vector3 position,Vector3 scale,Material material)
        {var go=GameObject.CreatePrimitive(type);go.name=name;go.transform.SetParent(parent,false);go.transform.localPosition=position;go.transform.localScale=scale;go.GetComponent<Renderer>().sharedMaterial=material;return go;}
        static RectTransform Canvas(Transform parent,string name,Vector3 position,Quaternion rotation,Vector2 size)
        {
            var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);
            r.localPosition=position;r.localRotation=rotation;r.localScale=Vector3.one*.0022f;r.sizeDelta=size;
            r.gameObject.AddComponent<UnityEngine.Canvas>().renderMode=RenderMode.WorldSpace;
            r.gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();return r;
        }
        static void Background(RectTransform parent,Color color)
        {var image=parent.gameObject.AddComponent<Image>();image.color=color;image.raycastTarget=false;}
        static RectTransform Rect(Transform parent,string name,Vector2 pos,Vector2 size)
        {var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);r.anchoredPosition=pos;r.sizeDelta=size;return r;}
        static TMP_Text Label(RectTransform parent,string value,Vector2 pos,Vector2 size,float fontSize,bool strong,Color color)
        {var r=Rect(parent,value,pos,size);var t=r.gameObject.AddComponent<TextMeshProUGUI>();t.font=strong?bold:regular;t.text=value;t.fontSize=fontSize;t.color=color;t.alignment=TextAlignmentOptions.Center;t.raycastTarget=false;return t;}
        static void Label3D(Transform parent,string value,Vector3 localPosition,float scale,Color color)
        {var t=new GameObject("Símbolo " + value).AddComponent<TextMeshPro>();t.transform.SetParent(parent,false);t.transform.localPosition=localPosition;t.transform.localScale=Vector3.one*scale;t.font=bold;t.fontSize=4;t.text=value;t.alignment=TextAlignmentOptions.Center;t.color=color;}
        static Button Button(RectTransform parent,string title,Vector2 pos,Vector2 size,Color color,float fontSize)
        {var r=Rect(parent,title,pos,size);var image=r.gameObject.AddComponent<Image>();image.color=color;var b=r.gameObject.AddComponent<Button>();Label(r,title,Vector2.zero,size,fontSize,true,Color.white);return b;}
        static Toggle Toggle(RectTransform parent,string title,Vector2 pos)
        {
            var r=Rect(parent,title,pos,new Vector2(570,70));var bg=r.gameObject.AddComponent<Image>();bg.color=Color.white;
            var toggle=r.gameObject.AddComponent<Toggle>();
            var box=Rect(r,"Caixa",new Vector2(-235,0),new Vector2(48,48));var boxImage=box.gameObject.AddComponent<Image>();boxImage.color=Purple;
            var tick=Rect(box,"Marca",Vector2.zero,new Vector2(26,26));var tickImage=tick.gameObject.AddComponent<Image>();tickImage.color=Color.white;
            toggle.targetGraphic=boxImage;toggle.graphic=tickImage;toggle.SetIsOnWithoutNotify(false);
            Label(r,title,new Vector2(35,0),new Vector2(440,60),24,true,Ink);return toggle;
        }
    }
}
