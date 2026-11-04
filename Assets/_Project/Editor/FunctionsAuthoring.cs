using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Tecaverso.Labs.Functions;
using Tecaverso.Labs.PlanarGeometry;
using Object=UnityEngine.Object;

namespace Tecaverso.Editor
{
    /// <summary>One-time installation. Never use this to update an existing authored room.</summary>
    public static class FunctionsAuthoring
    {
        public const string Folder="Assets/_Project/Laboratories/Mathematics/Functions";
        public const string ScenePath=Folder+"/Scenes/Functions.unity";
        const string Source="Assets/_Project/Laboratories/Mathematics/PlanarGeometry/Scenes/PlanarGeometry.unity";
        static TMP_FontAsset regular,bold;
        static Button buttonTemplate;static Toggle toggleTemplate;
        static readonly Color Blue=new Color32(40,78,160,255),Ink=new Color32(22,22,22,255),Lavender=new Color32(232,230,254,255);

        [MenuItem("Tecaverso/Instalar Funções e gráficos (uma vez)")]
        public static void Create()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode||Enumerable.Range(0,SceneManager.sceneCount).Any(i=>SceneManager.GetSceneAt(i).isDirty))
                throw new InvalidOperationException("Preserve as cenas não salvas e saia de Play Mode antes da instalação.");
            if(AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath))throw new InvalidOperationException("A sala já existe; edite os objetos em vez de reinstalar.");
            var catalog=AssetDatabase.LoadAssetAtPath<Tecaverso.Hub.HubCatalog>("Assets/_Project/Hub/HubCatalog.asset");
            var math=catalog.disciplines.Single(d=>d.title=="Matemática");
            var entry=math.contents.Single(c=>c.title.StartsWith("Funções"));
            if(!string.IsNullOrEmpty(entry.scenePath))throw new InvalidOperationException("Funções já possui uma cena.");
            Ensure(Folder+"/Scenes");Ensure(Folder+"/Meshes");Ensure(Folder+"/Materials");
            regular=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Regular SDF.asset");
            bold=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Bold SDF.asset");
            var source=SceneManager.GetSceneByPath(Source);if(!source.isLoaded)source=EditorSceneManager.OpenScene(Source,OpenSceneMode.Additive);
            SceneManager.SetActiveScene(source);
            var gallery=source.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<PlanarGallery>()).Single();
            buttonTemplate=gallery.backButton;toggleTemplate=gallery.verticesToggle;
            var ambientMode=RenderSettings.ambientMode;var sky=RenderSettings.ambientSkyColor;var equator=RenderSettings.ambientEquatorColor;
            var ground=RenderSettings.ambientGroundColor;var intensity=RenderSettings.ambientIntensity;var skybox=RenderSettings.skybox;
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);SceneManager.SetActiveScene(scene);
            RenderSettings.ambientMode=ambientMode;RenderSettings.ambientSkyColor=sky;RenderSettings.ambientEquatorColor=equator;
            RenderSettings.ambientGroundColor=ground;RenderSettings.ambientIntensity=intensity;RenderSettings.skybox=skybox;
            foreach(var name in new[]{"XR Origin (XR Rig)","XR Interaction Manager","EventSystem","Directional Light","TecaversoStandardRoom","Shared learning rendering"})
            {
                var clone=Object.Instantiate(source.GetRootGameObjects().Single(r=>r.name==name));clone.name=name;SceneManager.MoveGameObjectToScene(clone,scene);
            }
            var root=new GameObject("Functions laboratory");root.SetActive(false);var lab=root.AddComponent<FunctionsLab>();
            var active=Mat("Active curve",new Color(.2f,.84f,1));var reference=Mat("Reference curve",new Color(1,.65f,.18f));
            var grid=Mat("Grid",new Color(.22f,.30f,.42f));var axis=Mat("Axes",new Color(.7f,.8f,.95f));
            var point=Mat("Marker",new Color(1,.42f,.28f));var backing=Mat("Graph backing",new Color(.025f,.045f,.085f));
            var plot=Child(root.transform,"Cartesian graph").gameObject.AddComponent<FunctionPlot>();lab.plot=plot;
            plot.transform.localPosition=new Vector3(0,2.4f,4.6f);
            Box(plot.transform,"Graph backing",new Vector3(0,0,.15f),new Vector3(6.35f,4.35f,.10f),backing);
            var gridLines=new PlotLines();for(int i=-6;i<=6;i++)if(i!=0)gridLines.Line(new Vector2(i*.5f,-2),new Vector2(i*.5f,2),.012f);
            for(int i=-4;i<=4;i++)if(i!=0)gridLines.Line(new Vector2(-3,i*.5f),new Vector2(3,i*.5f),.012f);
            plot.grid=MeshObject(plot.transform,"Grid",grid,gridLines,.05f).gameObject;
            var axes=new PlotLines();axes.Line(new Vector2(-3,0),new Vector2(3,0),.016f);axes.Line(new Vector2(0,-2),new Vector2(0,2),.016f);
            axes.Arrow(new Vector2(3,0),Vector2.right,.12f);axes.Arrow(new Vector2(0,2),Vector2.up,.12f);
            MeshObject(plot.transform,"Axes",axis,axes,.02f);
            for(int x=-6;x<=6;x++)Label(plot.transform,"X "+x,x.ToString(),new Vector3(x*.5f,-.14f,-.04f),new Vector2(140,80),30,false,false,Color.white);
            for(int y=-4;y<=4;y++)if(y!=0)Label(plot.transform,"Y "+y,y.ToString(),new Vector3(-.14f,y*.5f,-.04f),new Vector2(130,80),30,false,false,Color.white);
            Label(plot.transform,"X title","x",new Vector3(3.1f,.12f,-.04f),new Vector2(100,80),38,true,false,Color.white);
            Label(plot.transform,"Y title","y = f(x)",new Vector3(.35f,2.06f,-.04f),new Vector2(400,80),30,true,false,Color.white);
            plot.curve=MeshObject(plot.transform,"Active curve",active,FunctionPlot.PolynomialLines(new Polynomial(FunctionFamily.Affine),false),-.025f);
            plot.referenceCurve=MeshObject(plot.transform,"Reference curve",reference,new PlotLines(),-.045f);plot.referenceCurve.gameObject.SetActive(false);
            plot.projections=MeshObject(plot.transform,"Marker projections",point,new PlotLines(),-.065f);
            var marker=GameObject.CreatePrimitive(PrimitiveType.Sphere);marker.name="Grab point P";marker.transform.SetParent(plot.transform,false);marker.transform.localScale=Vector3.one*.15f;
            marker.GetComponent<Renderer>().sharedMaterial=point;var body=marker.AddComponent<Rigidbody>();body.isKinematic=true;body.useGravity=false;
            plot.marker=marker.AddComponent<FunctionMarker>();plot.marker.lab=lab;plot.marker.throwOnDetach=false;plot.marker.trackRotation=false;
            plot.marker.attachEaseInTime=0;plot.marker.useDynamicAttach=true;
            plot.marker.movementType=UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable.MovementType.Instantaneous;
            plot.markerLabel=Label(plot.transform,"Point coordinates","P (0; 0)",new Vector3(0,.4f,-.14f),new Vector2(750,90),34,true,true);
            plot.featureMarkers=new Transform[3];plot.featureLabels=new TMP_Text[3];
            for(int i=0;i<3;i++)
            {
                var feature=Child(plot.transform,i==2?"Vertex":"Root "+(i+1));
                var sphere=GameObject.CreatePrimitive(PrimitiveType.Sphere);sphere.transform.SetParent(feature,false);sphere.transform.localScale=Vector3.one*.10f;
                Object.DestroyImmediate(sphere.GetComponent<Collider>());sphere.GetComponent<Renderer>().sharedMaterial=reference;
                plot.featureMarkers[i]=feature;plot.featureLabels[i]=Label(feature,"Analytic feature","",Vector3.down*.24f,new Vector2(750,90),30,true,true);
                feature.gameObject.SetActive(false);
            }
            lab.expression=Label(root.transform,"Current expression","CURVA ATIVA · f(x) = x",new Vector3(0,5.05f,4.55f),new Vector2(2800,170),68,true,true);
            lab.referenceText=Label(root.transform,"Reference expression","Referência: nenhuma curva guardada",new Vector3(0,4.78f,4.53f),new Vector2(2800,100),36,true,true);
            Label(root.transform,"Graph scale","ESCALA FIXA · 1 unidade = 0,5 m · arraste P sobre a curva",new Vector3(0,.20f,4.45f),new Vector2(2800,90),34,true,true);

            // A compact, purpose-specific console, with reused full-row XR controls.
            var console=Panel(root.transform,"Functions controls",new Vector3(-4.3f,2.15f,3.7f),new Vector2(900,1700),true);
            var content=Rect(console,"Content");Stretch(content);lab.content=content.gameObject;
            var layout=content.gameObject.AddComponent<VerticalLayoutGroup>();layout.padding=new RectOffset(24,24,24,24);layout.spacing=14;layout.childControlWidth=true;layout.childControlHeight=true;layout.childForceExpandHeight=false;
            RowText(content,"FUNÇÕES E GRÁFICOS",42,true,90);
            var family=Row(content,78);lab.affine=Button(family,"Afim");lab.quadratic=Button(family,"Quadrática");
            lab.familyText=RowText(content,"",32,false,135);
            lab.gridToggle=Toggle(content,"Exibir grade",true);lab.featuresToggle=Toggle(content,"Exibir raízes e vértice",false);
            lab.analysis=RowText(content,"",30,false,200);
            lab.coordinate=RowText(content,"",36,true,110);
            var markerRow=Row(content,76);lab.markerMinus=Button(markerRow,"x − 0,1");lab.markerPlus=Button(markerRow,"x + 0,1");
            lab.saveReference=Button(content,"Guardar referência",76);lab.removeReference=Button(content,"Remover referência",76);
            lab.restore=Button(content,"Restaurar função",76);lab.undo=Button(content,"Desfazer restauração",76);
            lab.back=Button(content,"Voltar ao Hub",84);
            var modal=Rect(console,"Confirmation");Stretch(modal);modal.gameObject.AddComponent<Image>().color=Lavender;lab.modal=modal.gameObject;
            var modalLayout=modal.gameObject.AddComponent<VerticalLayoutGroup>();modalLayout.padding=new RectOffset(40,40,350,40);modalLayout.spacing=28;modalLayout.childControlWidth=true;modalLayout.childControlHeight=true;modalLayout.childForceExpandHeight=false;
            lab.modalText=RowText(modal,"",42,true,380);lab.confirm=Button(modal,"Confirmar",100);lab.cancel=Button(modal,"Cancelar",100);
            modal.gameObject.AddComponent<CanvasGroup>();content.gameObject.AddComponent<CanvasGroup>();Tecaverso.UI.UIVisibility.Set(modal.gameObject,false);

            lab.coefficients=new FunctionsLab.CoefficientControl[3];
            for(int i=0;i<3;i++)
            {
                var station=Child(root.transform,"Coefficient "+(char)('a'+i));station.localPosition=new Vector3(4.2f,0,3-i*1.8f);station.localRotation=Quaternion.Euler(0,90,0);
                var knob=Child(station,"Unity dial").gameObject.AddComponent<FunctionKnob>();knob.lab=lab;knob.index=i;
                knob.transform.localPosition=new Vector3(0,1.05f,0);knob.transform.localRotation=Quaternion.Euler(90,0,0);
                knob.minAngle=-135;knob.maxAngle=135;knob.clampedMotion=true;Unity3DControlVisuals.Knob(knob,3.5f);
                Box(station,"Instrument support",new Vector3(0,.67f,.16f),new Vector3(.65f,.8f,.45f),backing);
                var board=Panel(station,"Coefficient readout",new Vector3(0,1.65f,0),new Vector2(700,300),true);
                var group=board.gameObject.AddComponent<VerticalLayoutGroup>();group.padding=new RectOffset(16,16,16,16);group.spacing=10;group.childControlHeight=true;group.childControlWidth=true;group.childForceExpandHeight=false;
                var value=RowText(board,"",46,true,80);var steps=Row(board,80);var minus=Button(steps,"− 0,1");var plus=Button(steps,"+ 0,1");
                RowText(board,"Gire para ajustar  ·  −  ↻  +",27,false,65);
                lab.coefficients[i]=new FunctionsLab.CoefficientControl{knob=knob,value=value,minus=minus,plus=plus,station=station.gameObject};
            }
            // Refresh once for an authored initial preview; persist generated meshes so the saved scene is complete.
            lab.Refresh();
            foreach(var filter in new[]{plot.curve,plot.referenceCurve,plot.projections})
                if(filter.sharedMesh&&!AssetDatabase.Contains(filter.sharedMesh))AssetDatabase.CreateAsset(filter.sharedMesh,Folder+"/Meshes/Initial "+filter.name+".asset");
            root.SetActive(true);UnityEngine.Canvas.ForceUpdateCanvases();
            entry.title="Funções e gráficos";entry.description="Transforme funções afins e quadráticas e observe como seus gráficos mudam.";
            entry.objective="Relacionar coeficientes, raízes, vértice e coordenadas ao gráfico de uma função.";entry.scenePath=ScenePath;EditorUtility.SetDirty(catalog);
            EditorSceneManager.SaveScene(scene,ScenePath);
            EditorBuildSettings.scenes=EditorBuildSettings.scenes.Concat(new[]{new EditorBuildSettingsScene(ScenePath,true)}).ToArray();AssetDatabase.SaveAssets();
            for(int i=SceneManager.sceneCount-1;i>=0;i--){var other=SceneManager.GetSceneAt(i);if(other!=scene)EditorSceneManager.CloseScene(other,true);}
        }
        static void Ensure(string path){if(AssetDatabase.IsValidFolder(path))return;int i=path.LastIndexOf('/');Ensure(path[..i]);AssetDatabase.CreateFolder(path[..i],path[(i+1)..]);}
        static Transform Child(Transform parent,string name){var t=new GameObject(name).transform;t.SetParent(parent,false);return t;}
        static RectTransform Rect(Transform parent,string name){var t=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();t.SetParent(parent,false);return t;}
        static void Stretch(RectTransform t){t.anchorMin=Vector2.zero;t.anchorMax=Vector2.one;t.offsetMin=t.offsetMax=Vector2.zero;}
        static Material Mat(string name,Color color){var m=new Material(Shader.Find("Universal Render Pipeline/Unlit")){name=name};m.SetColor("_BaseColor",color);AssetDatabase.CreateAsset(m,Folder+"/Materials/"+name+".mat");return m;}
        static void Box(Transform parent,string name,Vector3 position,Vector3 size,Material mat)
        {var go=GameObject.CreatePrimitive(PrimitiveType.Cube);go.name=name;go.transform.SetParent(parent,false);go.transform.localPosition=position;go.transform.localScale=size;go.GetComponent<Renderer>().sharedMaterial=mat;}
        static MeshFilter MeshObject(Transform parent,string name,Material mat,PlotLines lines,float depth)
        {var t=Child(parent,name);t.localPosition=Vector3.forward*depth;var mesh=new Mesh{name=name};lines.Fill(mesh);AssetDatabase.CreateAsset(mesh,Folder+"/Meshes/"+name+".asset");var f=t.gameObject.AddComponent<MeshFilter>();f.sharedMesh=mesh;t.gameObject.AddComponent<MeshRenderer>().sharedMaterial=mat;return f;}
        static RectTransform Panel(Transform parent,string name,Vector3 position,Vector2 size,bool interactive)
        {var t=Rect(parent,name);t.localPosition=position;t.localScale=Vector3.one*.002f;t.sizeDelta=size;t.gameObject.AddComponent<Canvas>().renderMode=RenderMode.WorldSpace;var img=t.gameObject.AddComponent<Image>();img.color=Lavender;img.raycastTarget=interactive;if(interactive)t.gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();return t;}
        static TMP_Text Label(Transform parent,string name,string text,Vector3 position,Vector2 size,float fontSize,bool strong,bool backing,Color? color=null)
        {
            var canvas=Panel(parent,name,position,size,false);if(!backing)Object.DestroyImmediate(canvas.GetComponent<Image>());
            var r=Rect(canvas,"Text");Stretch(r);r.offsetMin=new Vector2(10,4);r.offsetMax=new Vector2(-10,-4);
            return Configure(r,text,fontSize,strong,color);
        }
        static TMP_Text Configure(RectTransform r,string text,float fontSize,bool strong,Color? color=null)
        {var t=r.gameObject.AddComponent<TextMeshProUGUI>();t.font=strong?bold:regular;t.text=text;t.fontSize=fontSize;t.color=color??(strong?Blue:Ink);t.alignment=TextAlignmentOptions.Center;t.raycastTarget=false;t.overflowMode=TextOverflowModes.Ellipsis;return t;}
        static void Height(GameObject go,float height){var e=go.GetComponent<LayoutElement>()??go.AddComponent<LayoutElement>();e.minHeight=e.preferredHeight=height;e.flexibleHeight=0;e.flexibleWidth=1;}
        static TMP_Text RowText(Transform parent,string text,float size,bool strong,float height)
        {var r=Rect(parent,"Text "+text);Height(r.gameObject,height);return Configure(r,text,size,strong);}
        static RectTransform Row(Transform parent,float height)
        {var r=Rect(parent,"Row");Height(r.gameObject,height);var g=r.gameObject.AddComponent<HorizontalLayoutGroup>();g.spacing=16;g.childControlWidth=g.childControlHeight=true;g.childForceExpandWidth=true;g.childForceExpandHeight=false;return r;}
        static Button Button(Transform parent,string text,float height=76)
        {
            var b=Object.Instantiate(buttonTemplate,parent);b.name=text;b.onClick=new Button.ButtonClickedEvent();Height(b.gameObject,height);
            var t=b.GetComponentInChildren<TMP_Text>();t.text=text;t.fontSize=32;return b;
        }
        static Toggle Toggle(Transform parent,string text,bool value)
        {var t=Object.Instantiate(toggleTemplate,parent);t.name=text;t.onValueChanged=new Toggle.ToggleEvent();t.SetIsOnWithoutNotify(value);t.group=null;Height(t.gameObject,76);var label=t.GetComponentInChildren<TMP_Text>();label.text=text;label.fontSize=32;StyleToggle(t);return t;}
        public static void StyleToggle(Toggle t)
        {
            t.graphic.color=Blue;
            var background=t.graphic.transform.parent.GetComponent<Image>();background.color=Color.white;
            var outline=background.GetComponent<Outline>()??background.gameObject.AddComponent<Outline>();outline.effectColor=Blue;outline.effectDistance=new Vector2(2,-2);
        }
    }
}
