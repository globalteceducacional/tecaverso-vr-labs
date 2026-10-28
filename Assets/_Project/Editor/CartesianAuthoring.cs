using System;
using System.Linq;
using Tecaverso.Hub;
using Tecaverso.Labs.Cartesian;
using Tecaverso.Labs.ObliqueLaunch;
using Tecaverso.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Object=UnityEngine.Object;

namespace Tecaverso.Editor
{
    // First installation only. Existing authored scenes must be edited in place.
    public static class CartesianAuthoring
    {
        public const string Folder="Assets/_Project/Laboratories/Mathematics/CartesianSpace",ScenePath=Folder+"/Scenes/CartesianSpace.unity";
        static CartesianLab lab;static TMP_FontAsset regular,bold;
        static readonly Color Blue=new Color32(40,78,160,255),Ink=new Color32(22,22,22,255),Lavender=new Color32(232,230,254,255);
        static void Ensure(string path){if(AssetDatabase.IsValidFolder(path))return;int i=path.LastIndexOf('/');Ensure(path[..i]);AssetDatabase.CreateFolder(path[..i],path[(i+1)..]);}
        static RectTransform Rect(Transform parent,string name){var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);return r;}
        static void Stretch(RectTransform r){r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;}
        static void Height(RectTransform r,float h){var le=r.gameObject.AddComponent<LayoutElement>();le.minHeight=le.preferredHeight=h;}
        static void Vertical(RectTransform r){var l=r.gameObject.AddComponent<VerticalLayoutGroup>();l.spacing=12;l.childControlWidth=l.childControlHeight=true;l.childForceExpandWidth=true;l.childForceExpandHeight=false;}
        static RectTransform Row(Transform parent,string name,float height=64){var r=Rect(parent,name);Height(r,height);var l=r.gameObject.AddComponent<HorizontalLayoutGroup>();l.spacing=12;l.childControlWidth=l.childControlHeight=true;l.childForceExpandWidth=true;return r;}
        static TMP_Text Label(Transform parent,string text,float h=64,float fontSize=26)
        {var r=Rect(parent,text);Height(r,h);var t=r.gameObject.AddComponent<TextMeshProUGUI>();t.font=regular;t.fontSize=fontSize;t.color=Ink;t.text=text;t.raycastTarget=false;t.overflowMode=TextOverflowModes.Ellipsis;return t;}
        static Button Button(Transform parent,string text)
        {var r=Rect(parent,text);r.gameObject.AddComponent<LayoutElement>().flexibleWidth=1;var image=r.gameObject.AddComponent<Image>();image.color=Blue;var b=r.gameObject.AddComponent<Button>();b.targetGraphic=image;var colors=b.colors;colors.highlightedColor=new Color(.8f,.9f,1);colors.pressedColor=new Color(.6f,.7f,.9f);b.colors=colors;var t=Label(r,text,64,24);Object.DestroyImmediate(t.GetComponent<LayoutElement>());Stretch(t.rectTransform);t.rectTransform.offsetMin=new Vector2(8,4);t.rectTransform.offsetMax=new Vector2(-8,-4);t.alignment=TextAlignmentOptions.Center;t.font=bold;t.color=Color.white;return b;}
        static RectTransform Panel(string name,Vector3 position,Vector2 size)
        {var r=Rect(lab.transform,name);r.position=position;r.localScale=Vector3.one*.0018f;r.sizeDelta=size;var cv=r.gameObject.AddComponent<Canvas>();cv.renderMode=RenderMode.WorldSpace;r.gameObject.AddComponent<CanvasScaler>();r.gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();r.gameObject.AddComponent<Image>().color=Lavender;return r;}
        static RectTransform Content(Transform canvas,string name)
        {var r=Rect(canvas,name);Stretch(r);r.offsetMin=new Vector2(28,28);r.offsetMax=new Vector2(-28,-28);Vertical(r);r.gameObject.AddComponent<CanvasGroup>();return r;}
        static TMP_InputField Input(Transform parent)
        {
            var r=Rect(parent,"Coordinate input");r.gameObject.AddComponent<LayoutElement>().preferredWidth=180;r.gameObject.AddComponent<Image>().color=Color.white;
            var field=r.gameObject.AddComponent<TMP_InputField>();field.targetGraphic=r.GetComponent<Image>();var viewport=Rect(r,"Viewport");Stretch(viewport);viewport.offsetMin=new Vector2(12,6);viewport.offsetMax=new Vector2(-12,-6);viewport.gameObject.AddComponent<RectMask2D>();
            var text=Label(viewport,"0",60,28);Object.DestroyImmediate(text.GetComponent<LayoutElement>());Stretch(text.rectTransform);text.alignment=TextAlignmentOptions.MidlineLeft;field.textViewport=viewport;field.textComponent=(TextMeshProUGUI)text;field.contentType=TMP_InputField.ContentType.Standard;field.characterLimit=8;return field;
        }
        static void Interface()
        {
            var canvas=Panel("Cartesian tools",new Vector3(-3.6f,1.8f,-.3f),new Vector2(1000,1480));lab.mainCanvas=canvas;var body=Content(canvas,"Main");
            Label(body,"ESPAÇO CARTESIANO",58,38).font=bold;var row=Row(body,"Tabs");lab.tabs=new[]{Button(row,"CRIAR"),Button(row,"VISUALIZAR"),Button(row,"ATIVIDADES")};
            lab.instruction=Label(body,"Instruções",105,27);var slot=Rect(body,"Pages");Height(slot,620);lab.pages=new GameObject[3];
            for(int i=0;i<3;i++){var p=Rect(slot,"Page "+i);Stretch(p);Vertical(p);p.gameObject.AddComponent<CanvasGroup>();lab.pages[i]=p.gameObject;}
            var create=lab.pages[0].transform;lab.create=Button(Row(create,"Point"),"CRIAR PONTO NA ORIGEM");Label(create,"LINHAS · SUPERFÍCIES · SÓLIDOS",48,23);
            lab.tools=new Button[8];for(int r=0;r<4;r++){row=Row(create,"Tools "+r,72);for(int col=0;col<2;col++){int i=r*2+col;lab.tools[i]=Button(row,CartesianLab.Kind((ConstructionKind)i).ToUpperInvariant());}}
            lab.cancel=Button(Row(create,"Cancel"),"CANCELAR CONSTRUÇÃO");Label(create,"Escolha a ferramenta e selecione os pontos.\nO marcador central seleciona a figura.",90,25);
            var visual=lab.pages[1].transform;lab.viewButtons=new Button[7];string[] names={"COMPONENTES X/Y/Z","VETOR POSIÇÃO","PROJEÇÕES","MAGNÉTICO: LIGADO","GRADE XZ\nVISÍVEL","GRADE XY\nVISÍVEL","GRADE YZ\nVISÍVEL"};
            for(int r=0;r<4;r++){row=Row(visual,"View "+r);for(int col=0;col<2&&r*2+col<7;col++){int i=r*2+col;lab.viewButtons[i]=Button(row,names[i]);}}
            row=Row(visual,"Step");lab.stepButton=Button(row,"PASSO −/+: 0,5 u");Label(visual,"NOVOS PONTOS · liberdade de movimento",52,24);row=Row(visual,"Plane");lab.planeButtons=new[]{Button(row,"3D"),Button(row,"XY"),Button(row,"XZ"),Button(row,"YZ")};row=Row(visual,"Height");lab.lower=Button(row,"BAIXAR ESPAÇO");lab.raise=Button(row,"ELEVAR ESPAÇO");
            var activities=lab.pages[2].transform;lab.challengeButtons=new Button[5];string[] challenges={"EXPLORAÇÃO LIVRE","1 · LOCALIZAR PONTO","2 · COMPRIMENTO","3 · ÁREA DO TRIÂNGULO","4 · VOLUME DA CAIXA"};for(int i=0;i<5;i++)lab.challengeButtons[i]=Button(Row(activities,"Challenge "+i,56),challenges[i]);lab.challengeText=Label(activities,"Atividade",270,25);
            lab.status=Label(body,"Estado",126,25);row=Row(body,"History");lab.undo=Button(row,"DESFAZER");lab.redo=Button(row,"REFAZER");lab.clear=Button(Row(body,"Clear"),"LIMPAR ESPAÇO");lab.back=Button(Row(body,"Navigation"),"VOLTAR AO HUB");
            var modal=Rect(canvas,"Confirmation");Stretch(modal);modal.gameObject.AddComponent<Image>().color=Lavender;modal.gameObject.AddComponent<CanvasGroup>();lab.dialog=modal.gameObject;var modalBody=Content(modal,"Dialog content");lab.dialogText=Label(modalBody,"Confirmar",260,32);row=Row(modalBody,"Actions",80);lab.confirm=Button(row,"CONFIRMAR");lab.dismiss=Button(row,"CANCELAR");UIVisibility.Set(modal.gameObject,false);
            var context=Panel("Selection details",new Vector3(3.5f,2.05f,-.8f),new Vector2(960,1320));lab.contextCanvas=context;var content=Content(context,"Selection page");lab.contextPage=content.gameObject;
            Label(content,"SELEÇÃO E ANÁLISE",58,34).font=bold;lab.details=Label(content,"Selecione um elemento",320,26);
            var coordinates=Rect(content,"Coordinates");Height(coordinates,240);Vertical(coordinates);coordinates.gameObject.AddComponent<CanvasGroup>();lab.coordinateSection=coordinates.gameObject;
            lab.fields=new TMP_InputField[3];lab.lockButtons=new Button[3];lab.minusButtons=new Button[3];lab.plusButtons=new Button[3];lab.coordinateLabels=new TMP_Text[3];
            for(int i=0;i<3;i++){row=Row(coordinates,"Axis "+i,72);lab.coordinateLabels[i]=Label(row,"XYZ"[i].ToString(),72,30);lab.coordinateLabels[i].GetComponent<LayoutElement>().preferredWidth=40;lab.fields[i]=Input(row);lab.minusButtons[i]=Button(row,"−");lab.plusButtons[i]=Button(row,"+");lab.lockButtons[i]=Button(row,"TRAVAR");}
            row=Row(content,"Inspect");lab.pin=Button(row,"FIXAR COMPARAÇÃO");lab.analysis=Button(row,"ANÁLISE");lab.moveGroup=Button(Row(content,"Translate"),"MOVER CONJUNTO");lab.delete=Button(Row(content,"Delete"),"EXCLUIR SELEÇÃO");row=Row(content,"Cycle");lab.nextPoint=Button(row,"PRÓXIMO PONTO");lab.nextConstruction=Button(row,"PRÓXIMA FIGURA");Label(content,"Agarrar vértice: muda a forma.\nMover conjunto: preserva dimensões.\nUnidades matemáticas (u), áreas (u²) e volumes (u³).",130,25);
        }
        static Material Material(string name,Color color,bool transparent=false,bool line=false)
        {
            var mat=new Material(Shader.Find(line?"Universal Render Pipeline/Particles/Unlit":"Universal Render Pipeline/Unlit")){name=name};mat.SetColor("_BaseColor",color);
            if(transparent){mat.SetFloat("_Surface",1);mat.SetFloat("_SrcBlend",(float)BlendMode.SrcAlpha);mat.SetFloat("_DstBlend",(float)BlendMode.OneMinusSrcAlpha);mat.SetFloat("_ZWrite",0);mat.SetFloat("_Cull",0);mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");mat.renderQueue=3000;}
            AssetDatabase.CreateAsset(mat,Folder+"/Materials/"+name+".mat");return mat;
        }
        static CartesianGrab Prefab(string name,bool construction)
        {
            var go=GameObject.CreatePrimitive(construction?PrimitiveType.Cube:PrimitiveType.Sphere);go.name=name;go.transform.localScale=Vector3.one*(construction?.10f:.14f);go.GetComponent<Collider>().isTrigger=false;go.GetComponent<Renderer>().sharedMaterial=lab.solidMaterial;
            var rb=go.AddComponent<Rigidbody>();rb.isKinematic=true;rb.useGravity=false;var grab=go.AddComponent<CartesianGrab>();grab.useDynamicAttach=true;grab.trackRotation=false;grab.throwOnDetach=false;grab.movementType=XRBaseInteractable.MovementType.Instantaneous;grab.colliders.Add(go.GetComponent<Collider>());var prefab=PrefabUtility.SaveAsPrefabAsset(go,Folder+"/Prefabs/"+name+".prefab");Object.DestroyImmediate(go);return prefab.GetComponent<CartesianGrab>();
        }
        static void Line(Transform parent,string name,Vector3 a,Vector3 b,Color color,float width=.008f)
        {var go=new GameObject(name);go.transform.SetParent(parent,false);var l=go.AddComponent<LineRenderer>();l.useWorldSpace=false;l.sharedMaterial=lab.lineMaterial;l.startColor=l.endColor=color;l.widthMultiplier=width;l.positionCount=2;l.SetPositions(new[]{a,b});}
        static void WorldText(Transform parent,string text,Vector3 at,float size=.7f)
        {var go=new GameObject(text);go.transform.SetParent(parent,false);go.transform.localPosition=at;var t=go.AddComponent<TextMeshPro>();t.font=bold;t.fontSize=size;t.text=text;t.color=Color.white;t.alignment=TextAlignmentOptions.Center;t.rectTransform.sizeDelta=size>=2?new Vector2(9,.6f):new Vector2(1.7f,.25f);t.overflowMode=TextOverflowModes.Ellipsis;}
        static void Space()
        {
            lab.origin=new GameObject("Mathematical origin").transform;lab.origin.SetParent(lab.transform);lab.origin.position=new Vector3(0,2.7f,1.5f);lab.grids=new GameObject[3];
            for(int axis=0;axis<3;axis++)
            {
                var axisMaterial=Material("Axis"+axis,lab.axisColors[axis]);var a=Vector3.zero;var b=Vector3.zero;a[axis]=-5*lab.Unit;b[axis]=5*lab.Unit;Line(lab.origin,"Axis "+axis,a,b,lab.axisColors[axis],.019f);
                for(int i=-5;i<=5;i++){var at=Vector3.zero;at[axis]=i*lab.Unit;var perpendicular=axis==1?Vector3.right:Vector3.up;Line(lab.origin,"Tick",at-perpendicular*.035f,at+perpendicular*.035f,lab.axisColors[axis]);if(i!=0)WorldText(lab.origin,i.ToString(),at-perpendicular*.14f,.5f);}
                foreach(int sign in new[]{-1,1}){var head=new GameObject("Axis arrow",typeof(MeshFilter),typeof(MeshRenderer));head.transform.SetParent(lab.origin,false);var dir=Vector3.zero;dir[axis]=sign;head.transform.localPosition=dir*(5*lab.Unit);head.transform.localRotation=Quaternion.FromToRotation(Vector3.up,dir);head.transform.localScale=new Vector3(.07f,.16f,.07f);head.GetComponent<MeshFilter>().sharedMesh=lab.coneMesh;head.GetComponent<MeshRenderer>().sharedMaterial=axisMaterial;}
                var end=Vector3.zero;end[axis]=5*lab.Unit+.35f;WorldText(lab.origin,"XYZ"[axis].ToString(),end,1.3f);
            }
            WorldText(lab.origin,"O",new Vector3(-.13f,-.16f,0));
            for(int g=0;g<3;g++){var go=new GameObject(new[]{"Grid XZ","Grid XY","Grid YZ"}[g]);go.transform.SetParent(lab.origin,false);lab.grids[g]=go;int a=g==2?1:0,b=g==1?1:2;for(int i=-5;i<=5;i++)for(int j=0;j<2;j++){var start=Vector3.zero;var end=Vector3.zero;start[j==0?a:b]=end[j==0?a:b]=i*lab.Unit;start[j==0?b:a]=-5*lab.Unit;end[j==0?b:a]=5*lab.Unit;Line(go.transform,"Grid line",start,end,new Color(.28f,.42f,.55f),.006f);}go.SetActive(true);}
        }
        public static string Create()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Exit Play Mode.");if(AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath)!=null)throw new InvalidOperationException("Scene exists. Edit it in place.");
            for(int i=0;i<SceneManager.sceneCount;i++)if(SceneManager.GetSceneAt(i).isDirty)throw new InvalidOperationException("Preserve unsaved scenes before creating another lab.");
            var template=EditorSceneManager.OpenScene("Assets/_Project/Laboratories/Physics/Waves/Scenes/Waves.unity",OpenSceneMode.Additive);Ensure(Folder+"/Scenes");Ensure(Folder+"/Materials");Ensure(Folder+"/Prefabs");
            regular=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Regular SDF.asset");bold=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Bold SDF.asset");
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);SceneManager.SetActiveScene(scene);
            foreach(var name in new[]{"XR Origin (XR Rig)","XR Interaction Manager","EventSystem","Directional Light"}){var go=Object.Instantiate(template.GetRootGameObjects().Single(o=>o.name==name));go.name=name;SceneManager.MoveGameObjectToScene(go,scene);if(name=="XR Origin (XR Rig)")go.transform.position=new Vector3(0,0,-4);foreach(var ray in go.GetComponentsInChildren<XRRayInteractor>(true)){ray.manipulateAttachTransform=true;ray.useForceGrab=false;ray.translateSpeed=1.5f;}}
            var room=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Environments/StandardRoom/TecaversoStandardRoom.prefab"),scene);room.transform.localScale=new Vector3(1,1.3f,1);
            var volume=new GameObject("Shared learning rendering").AddComponent<Volume>();volume.isGlobal=true;volume.sharedProfile=AssetDatabase.LoadAssetAtPath<VolumeProfile>("Assets/_Project/Shared/Rendering/TecaversoLearningProfile.asset");
            if(!Object.FindObjectsByType<UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard.GlobalNonNativeKeyboard>(FindObjectsInactive.Include,FindObjectsSortMode.None).Any(k=>k.gameObject.scene==scene))PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Samples/XR Interaction Toolkit/3.5.1/Spatial Keyboard/Prefabs/XRI Global Keyboard Manager.prefab"),scene);
            lab=new GameObject("Cartesian Space Lab").AddComponent<CartesianLab>();lab.font=regular;lab.lineMaterial=Material("Geometry lines",Color.white,false,true);lab.solidMaterial=Material("Control points",Color.white);lab.surfaceMaterial=Material("Mathematical surfaces",new Color(.3f,.7f,1,.14f),true);lab.coneMesh=AssetDatabase.LoadAssetAtPath<Mesh>("Assets/_Project/Laboratories/Physics/InclinedPlane/Meshes/ForceArrowCone.asset");lab.pointPrefab=Prefab("Cartesian point",false);lab.constructionPrefab=Prefab("Construction handle",true);Space();Interface();
            WorldText(lab.transform,"MATEMÁTICA · ESPAÇO CARTESIANO",new Vector3(0,4.8f,4),2);
            for(int i=0;i<lab.pages.Length;i++)UIVisibility.Set(lab.pages[i],i==0);UIVisibility.Set(lab.coordinateSection,false,true);Canvas.ForceUpdateCanvases();EditorSceneManager.SaveScene(scene,ScenePath);
            var catalog=AssetDatabase.LoadAssetAtPath<HubCatalog>("Assets/_Project/Hub/HubCatalog.asset");var entry=catalog.disciplines.First(d=>d.title=="Matemática").contents.First(c=>c.title=="Geometria");entry.title="Espaço cartesiano";entry.description="Crie pontos, retas, superfícies e sólidos. Manipule coordenadas e investigue medidas no espaço.";entry.objective="Relacionar coordenadas, vetores, áreas e volumes por construção e observação.";entry.scenePath=ScenePath;EditorUtility.SetDirty(catalog);
            var scenes=EditorBuildSettings.scenes.ToList();scenes.Add(new EditorBuildSettingsScene(ScenePath,true));EditorBuildSettings.scenes=scenes.ToArray();AssetDatabase.SaveAssets();foreach(var other in Enumerable.Range(0,SceneManager.sceneCount).Select(SceneManager.GetSceneAt).Where(s=>s!=scene&&!s.isDirty).ToArray())EditorSceneManager.CloseScene(other,true);return "Cartesian lab created and registered.";
        }
    }
}
