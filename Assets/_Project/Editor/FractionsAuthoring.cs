using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Tecaverso.Labs.Fractions;
using Tecaverso.Labs.PlanarGeometry;
using Object=UnityEngine.Object;
namespace Tecaverso.Editor
{
    public static class FractionsAuthoring
    {
        public const string Folder="Assets/_Project/Laboratories/Mathematics/Fractions";
        public const string ScenePath=Folder+"/Scenes/Fractions.unity";
        static TMP_FontAsset regular,bold;static Button buttonTemplate;static Toggle toggleTemplate;
        static readonly Color Blue=new Color32(40,78,160,255),Ink=new Color32(22,22,22,255),Lavender=new Color32(232,230,254,255);
        public static void Create()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode||Enumerable.Range(0,SceneManager.sceneCount).Any(i=>SceneManager.GetSceneAt(i).isDirty))throw new Exception("Preserve as cenas não salvas antes de instalar.");
            if(AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath))throw new Exception("Cena já existe; editar pontualmente.");
            Ensure(Folder+"/Scenes");Ensure(Folder+"/Materials");Ensure(Folder+"/Prefabs");
            var source=EditorSceneManager.OpenScene("Assets/_Project/Laboratories/Mathematics/PlanarGeometry/Scenes/PlanarGeometry.unity",OpenSceneMode.Additive);
            var gallery=source.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<PlanarGallery>()).Single();buttonTemplate=gallery.backButton;toggleTemplate=gallery.verticesToggle;
            regular=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Regular SDF.asset");bold=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Bold SDF.asset");
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);SceneManager.SetActiveScene(scene);
            foreach(string name in new[]{"XR Origin (XR Rig)","XR Interaction Manager","EventSystem","Directional Light","TecaversoStandardRoom","Shared learning rendering"}){var copy=Object.Instantiate(source.GetRootGameObjects().Single(r=>r.name==name));copy.name=name;SceneManager.MoveGameObjectToScene(copy,scene);}
            var root=new GameObject("Fractions and proportions laboratory");root.SetActive(false);var lab=root.AddComponent<FractionsLab>();
            var blue=Mat("Fraction blue",Blue);var orange=Mat("Recipe orange",new Color(.88f,.37f,.08f));var dark=Mat("Graphite",new Color(.04f,.07f,.12f));var white=Mat("Division white",Color.white);var mint=Mat("Snap preview",new Color(.2f,.8f,.6f));
            lab.templates=new FractionPiece[11];lab.dispensers=new Transform[11];lab.origins=new Transform[4];lab.sockets=new FractionSocket[4];lab.totals=new TMP_Text[2];
            var rack=Child(root.transform,"Fraction dispensers");rack.localPosition=new Vector3(-4.2f,0,0);rack.localRotation=Quaternion.Euler(0,-90,0);
            Label(rack,"Rack title","PEGUE UMA PEÇA\nReposição automática · Até 6 peças soltas",new Vector3(0,2.85f,0),new Vector2(1900,220),42,true,true);
            for(int k=0;k<11;k++)
            {
                bool token=k>=9;int d=token?1:FractionSession.Denominators[k];float width=token?.2f:1.2f/d;
                Material mat=token?(k==9?blue:orange):Mat("Denominator "+d,Color.Lerp(Blue,new Color(.05f,.6f,.65f),k/10f));
                var go=new GameObject(token?"Token "+(k==9?"A":"B"):"Fraction 1 over "+d);go.SetActive(false);
                var rb=go.AddComponent<Rigidbody>();rb.isKinematic=true;rb.useGravity=false;
                var p=go.AddComponent<FractionPiece>();p.kind=k;p.throwOnDetach=false;p.useDynamicAttach=true;p.movementType=UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable.MovementType.Kinematic;
                var visual=GameObject.CreatePrimitive(token?PrimitiveType.Cylinder:PrimitiveType.Cube);visual.name="Visual";visual.transform.SetParent(go.transform,false);visual.transform.localScale=token?new Vector3(.2f,.04f,.2f):new Vector3(width,.22f,.1f);if(token)visual.transform.localRotation=Quaternion.Euler(90,0,0);visual.GetComponent<Renderer>().sharedMaterial=mat;Object.DestroyImmediate(visual.GetComponent<Collider>());
                var collider=go.AddComponent<BoxCollider>();collider.size=new Vector3(Mathf.Max(width,.1f),.24f,.13f);p.colliders.Add(collider);
                var text=Label(go.transform,"Fraction label",token?(k==9?"A":"B"):d==1?"1":"1/"+d,new Vector3(0,0,-.065f),new Vector2(token?180:Mathf.Max(88,width*500),100),token?46:25,true,false,Color.white);
                p.divisions=Child(go.transform,"Division marks").gameObject;
                if(!token){for(int edge=-1;edge<=1;edge+=2)Box(p.divisions.transform,"Boundary",new Vector3(edge*(width/2-.006f),0,-.057f),new Vector3(.009f,.22f,.008f),white);}
                var prefab=PrefabUtility.SaveAsPrefabAsset(go,Folder+"/Prefabs/"+go.name+".prefab");lab.templates[k]=prefab.GetComponent<FractionPiece>();Object.DestroyImmediate(go);
                if(!token){var position=new Vector3((k%3-1)*1.36f,2.15f-(k/3)*.6f,-.15f);var sourcePoint=Child(rack,"Dispenser "+d);sourcePoint.localPosition=position;lab.dispensers[k]=sourcePoint;Box(rack,"Shelf "+d,position+new Vector3(0,-.18f,.12f),new Vector3(1.28f,.08f,.38f),dark);Label(rack,"Source label "+d,d==1?"1 inteiro":"1/"+d,position+Vector3.up*.23f,new Vector2(500,100),34,true,true);}
            }
            Label(root.transform,"Title","FRAÇÕES E PROPORÇÕES",new Vector3(0,3.8f,2.5f),new Vector2(2300,170),64,true,true);
            Label(root.transform,"Assembly instruction","Pegue uma barra e solte perto da ponta livre.\nRetire qualquer peça para subtrair; as demais se alinham.",new Vector3(0,3.45f,2.5f),new Vector2(2300,180),32,false,true);
            for(int r=0;r<2;r++)
            {
                var origin=Child(root.transform,"Line "+(r==0?"A":"B"));origin.localPosition=new Vector3(-1.2f,r==0?2.05f:1.15f,2.5f);lab.origins[r]=origin;
                Box(origin,"Backing",new Vector3(1.2f,0,.1f),new Vector3(2.5f,.32f,.1f),dark);
                for(int i=0;i<=2;i++){Box(origin,"Whole divider "+i,new Vector3(i*1.2f,0,-.01f),new Vector3(.012f,.42f,.02f),white);Label(origin,"Whole "+i,i.ToString(),new Vector3(i*1.2f,.32f,0),new Vector2(100,80),30,true,true);}
                lab.totals[r]=Label(root.transform,"Total "+r,"",new Vector3(0,r==0?2.85f:.65f,2.5f),new Vector2(2100,290),32,false,true);
            }
            lab.comparison=Label(root.transform,"Comparison","",new Vector3(0,1.75f,2.45f),new Vector2(1800,80),32,true,true);
            var line=Child(root.transform,"Equality endpoints").gameObject.AddComponent<LineRenderer>();line.useWorldSpace=true;line.positionCount=2;line.startWidth=line.endWidth=.018f;line.sharedMaterial=mint;lab.equivalenceLine=line;
            var recipeRoot=Child(root.transform,"Recipe station");recipeRoot.localPosition=new Vector3(4,0,.4f);recipeRoot.localRotation=Quaternion.Euler(0,90,0);
            Label(recipeRoot,"Recipe title","RECEITA · 2 A PARA 3 B",new Vector3(0,3.15f,0),new Vector2(2100,180),50,true,true);
            lab.recipe=Label(recipeRoot,"Recipe readings","",new Vector3(0,2.35f,0),new Vector2(2100,600),36,false,true);
            for(int cat=0;cat<2;cat++){var origin=Child(recipeRoot,"Receptacle "+cat);origin.localPosition=new Vector3(cat==0?-.8f:.8f,1.1f,-.2f);lab.origins[cat+2]=origin;Box(origin,"Receptacle backing",new Vector3(0,.15f,.1f),new Vector3(1.35f,.65f,.1f),dark);Box(origin,"Lip",new Vector3(0,-.17f,-.05f),new Vector3(1.35f,.07f,.3f),cat==0?blue:orange);Label(origin,"Category",cat==0?"CATEGORIA A":"CATEGORIA B",new Vector3(0,.58f,0),new Vector2(650,100),32,true,true);var sourcePoint=Child(recipeRoot,"Token supply "+cat);sourcePoint.localPosition=new Vector3(cat==0?-1.65f:1.65f,1.25f,-.3f);lab.dispensers[cat+9]=sourcePoint;Label(sourcePoint,"Take token",cat==0?"PEGAR A":"PEGAR B",Vector3.up*.3f,new Vector2(400,100),28,true,true);}
            var recipeControls=Panel(recipeRoot,"Recipe controls",new Vector3(0,.55f,-.1f),new Vector2(1800,250),true);Layout(recipeControls);var choices=Row(recipeControls,85);lab.multipliers=new[]{Button(choices,"1×"),Button(choices,"2×"),Button(choices,"3×")};lab.clearRecipe=Button(recipeControls,"Limpar receita",85);
            for(int i=0;i<4;i++){var socket=Child(root.transform,"XR socket "+i).gameObject.AddComponent<FractionSocket>();socket.transform.position=lab.origins[i].position;var col=socket.gameObject.AddComponent<SphereCollider>();col.isTrigger=true;col.radius=.65f;socket.attachTransform=Child(socket.transform,"Attach");socket.showInteractableHoverMeshes=false;lab.sockets[i]=socket;}
            var controls=Panel(root.transform,"Options",new Vector3(-2.8f,1.8f,-2),new Vector2(1500,650),true);controls.localRotation=Quaternion.Euler(0,-45,0);Layout(controls);lab.divisions=Toggle(controls,"Exibir divisões",true);lab.decimals=Toggle(controls,"Exibir decimal e porcentagem",false);lab.equivalence=Toggle(controls,"Mostrar equivalência",false);var actions=Row(controls,85);lab.undo=Button(actions,"Desfazer");lab.recover=Button(actions,"Recolher peças soltas");var navigation=Row(controls,85);lab.clearRows=Button(navigation,"Limpar montagem");lab.back=Button(navigation,"Voltar ao Hub");lab.status=RowText(controls,"Pegue uma peça no dispensador lateral.",28,false,100);lab.controls=new[]{controls.gameObject,recipeControls.gameObject};
            lab.returnArea=Child(root.transform,"Return area");lab.returnArea.localPosition=new Vector3(-3,.8f,-3.5f);Box(lab.returnArea,"Tray",Vector3.zero,new Vector3(.65f,.12f,.6f),blue);Label(lab.returnArea,"Return label","DEVOLVER",Vector3.up*.3f,new Vector2(400,100),32,true,true);
            var ghost=GameObject.CreatePrimitive(PrimitiveType.Cube);ghost.name="Snap preview";ghost.transform.SetParent(root.transform,false);Object.DestroyImmediate(ghost.GetComponent<Collider>());ghost.GetComponent<Renderer>().sharedMaterial=mint;ghost.SetActive(false);lab.preview=ghost.transform;
            var modal=Panel(root.transform,"Confirmation",new Vector3(0,1.7f,-2),new Vector2(1500,650),true);Layout(modal);lab.modal=modal.gameObject;lab.message=RowText(modal,"",38,true,300);lab.confirm=Button(modal,"Confirmar",90);lab.cancel=Button(modal,"Cancelar",90);Tecaverso.UI.UIVisibility.Set(lab.modal,false);
            root.SetActive(true);Canvas.ForceUpdateCanvases();
            var catalog=AssetDatabase.LoadAssetAtPath<Tecaverso.Hub.HubCatalog>("Assets/_Project/Hub/HubCatalog.asset");var math=catalog.disciplines.Single(d=>d.title=="Matemática");if(math.contents.Length>=6)throw new Exception("A disciplina já tem seis conteúdos.");math.contents=math.contents.Concat(new[]{new Tecaverso.Hub.HubCatalog.Content{title="Frações e proporções",description="Monte barras fracionárias e explore receitas proporcionais.",objective="Comparar frações equivalentes, porcentagens e razões.",scenePath=ScenePath,icon=math.icon}}).ToArray();EditorUtility.SetDirty(catalog);
            EditorSceneManager.SaveScene(scene,ScenePath);EditorBuildSettings.scenes=EditorBuildSettings.scenes.Concat(new[]{new EditorBuildSettingsScene(ScenePath,true)}).ToArray();AssetDatabase.SaveAssets();
            for(int i=SceneManager.sceneCount-1;i>=0;i--){var other=SceneManager.GetSceneAt(i);if(other!=scene&&!other.isDirty)EditorSceneManager.CloseScene(other,true);}
        }
        static void Layout(RectTransform panel){var g=panel.gameObject.AddComponent<VerticalLayoutGroup>();g.padding=new RectOffset(24,24,24,24);g.spacing=8;g.childControlWidth=g.childControlHeight=true;g.childForceExpandWidth=true;g.childForceExpandHeight=false;}
        static void Ensure(string path){if(AssetDatabase.IsValidFolder(path))return;int i=path.LastIndexOf('/');Ensure(path[..i]);AssetDatabase.CreateFolder(path[..i],path[(i+1)..]);}
        static Transform Child(Transform parent,string name){var t=new GameObject(name).transform;t.SetParent(parent,false);return t;}
        static RectTransform Rect(Transform parent,string name){var t=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();t.SetParent(parent,false);return t;}
        static void Stretch(RectTransform t){t.anchorMin=Vector2.zero;t.anchorMax=Vector2.one;t.offsetMin=t.offsetMax=Vector2.zero;}
        static Material Mat(string name,Color color){var m=new Material(Shader.Find("Universal Render Pipeline/Unlit")){name=name};m.SetColor("_BaseColor",color);AssetDatabase.CreateAsset(m,Folder+"/Materials/"+name+".mat");return m;}
        static void Box(Transform parent,string name,Vector3 position,Vector3 size,Material mat)
        {var go=GameObject.CreatePrimitive(PrimitiveType.Cube);go.name=name;go.transform.SetParent(parent,false);go.transform.localPosition=position;go.transform.localScale=size;go.GetComponent<Renderer>().sharedMaterial=mat;}
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


