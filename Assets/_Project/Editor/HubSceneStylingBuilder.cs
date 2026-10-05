using System;
using System.IO;
using System.Linq;
using Tecaverso.Hub;
using Tecaverso.Labs.ObliqueLaunch;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;
using static Tecaverso.Editor.ObliqueLaunchFigmaUIBuilder;

namespace Tecaverso.Editor
{
    public static class HubSceneStylingBuilder
    {
        const string Art="Assets/_Project/UI/Art/Figma/Hub/";
        const string RoomFolder="Assets/_Project/Environments/StandardRoom";
        const string Materials="Assets/_Project/Laboratories/Physics/ObliqueLaunch/Materials/Environment/";
        const string LabScene="Assets/_Project/Laboratories/Physics/ObliqueLaunch/Scenes/ObliqueLaunch.unity";
        static readonly Color Ink=Hex("161616"), Blue=Hex("284EA0"), Navy=Hex("07386F"), Surface=Hex("D3D1E8"), Background=Hex("E8E6FE");
        static TMP_FontAsset regular,bold,heavy;
        static Mesh cube;

        [MenuItem("Tecaverso/Hub/Apply Standard Room and Figma UI")]
        public static void Apply()
        {
            var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if(Application.isPlaying||scene.name!="Hub") throw new InvalidOperationException("Open Hub in Edit Mode first.");
            if(GameObject.Find("Tecaverso Hub UI")!=null) throw new InvalidOperationException("Hub already styled. Edit the existing objects instead.");
            regular=Font("Regular"); bold=Font("Bold"); heavy=Font("ExtraBold");
            foreach(var path in Directory.GetFiles(Art,"*.png"))
            {
                var importer=(TextureImporter)AssetImporter.GetAtPath(path.Replace('\\','/'));
                importer.textureType=TextureImporterType.Sprite; importer.spriteImportMode=SpriteImportMode.Single;
                importer.alphaIsTransparency=true; importer.mipmapEnabled=false; importer.maxTextureSize=1024;
                importer.textureCompression=TextureImporterCompression.Compressed; importer.SaveAndReimport();
            }
            var catalog=CreateCatalog();
            CreateRoom();
            CreateUI(catalog);
            // Repair the stale scene GUID in the existing enabled entry; never reorder startup scenes.
            var entries=EditorBuildSettings.scenes;
            for(int i=0;i<entries.Length;i++)
                if(entries[i].path==LabScene) entries[i]=new EditorBuildSettingsScene(LabScene,entries[i].enabled);
            EditorBuildSettings.scenes=entries;
            EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
        }

        static HubCatalog CreateCatalog()
        {
            const string path="Assets/_Project/Hub/HubCatalog.asset";
            var existing=AssetDatabase.LoadAssetAtPath<HubCatalog>(path); if(existing!=null) return existing;
            var catalog=ScriptableObject.CreateInstance<HubCatalog>();
            HubCatalog.Content Entry(string title,string icon,string body,string goal,string scene="") => new HubCatalog.Content{title=title,icon=Icon(icon),description=body,objective=goal,scenePath=scene};
            catalog.disciplines=new[]{
                new HubCatalog.Discipline{title="Física",description="Movimento, energia, ondas e fenômenos do mundo físico.",accent=Blue,icon=Icon("Physics"),contents=new[]{
                    Entry("Movimento e forças","ProjectileMotion","Explore o lançamento oblíquo: varie o ângulo, a velocidade inicial, a altura e a gravidade e observe a trajetória e os vetores.","Relacionar velocidade, gravidade, alcance e altura máxima.",LabScene),
                    Entry("Eletricidade","Electricity","Prévia: explore cargas elétricas, circuitos e transferência de energia.","Relacionar tensão, corrente e resistência."),
                    Entry("Óptica","Optics","Prévia: observe como a luz se propaga e interage com espelhos e lentes.","Investigar reflexão e refração da luz."),
                    Entry("Termodinâmica","Thermodynamics","Prévia: investigue temperatura, calor e transformações de energia.","Compreender trocas de calor entre sistemas."),
                    Entry("Ondas","Waves","Prévia: observe oscilações, propagação e interferência de ondas.","Relacionar amplitude, frequência e comprimento de onda."),
                    Entry("Gravitação","Gravity","Prévia: explore a atração gravitacional e o movimento dos corpos celestes.","Compreender massa, distância e atração gravitacional.")}},
                new HubCatalog.Discipline{title="Química",description="Matéria, propriedades, reações e transformações.",accent=Hex("7648E8"),icon=Icon("Chemistry"),contents=new[]{
                    Entry("Estrutura da matéria","Chemistry","Mockup: explore átomos, moléculas e propriedades da matéria.","Reconhecer modelos e estruturas da matéria."),
                    Entry("Reações químicas","Chemistry","Mockup: observe reagentes e produtos em transformações químicas.","Identificar evidências de uma reação química."),
                    Entry("Soluções","Chemistry","Mockup: prepare misturas e compare concentrações.","Relacionar soluto, solvente e concentração.")}},
                new HubCatalog.Discipline{title="Biologia",description="Vida, organismos, sistemas e ecossistemas.",accent=Hex("3ED39C"),icon=Icon("Biology"),contents=new[]{
                    Entry("Células","Biology","Mockup: conheça estruturas e funções celulares.","Identificar organelas e suas funções."),
                    Entry("Corpo humano","Biology","Mockup: explore os sistemas do corpo humano.","Relacionar órgãos e sistemas."),
                    Entry("Ecossistemas","Biology","Mockup: observe organismos e relações ecológicas.","Compreender interações nos ecossistemas.")}},
                new HubCatalog.Discipline{title="Matemática",description="Formas, relações, medidas e raciocínio lógico.",accent=Hex("E7811B"),icon=Icon("Mathematics"),contents=new[]{
                    Entry("Geometria","Mathematics","Mockup: explore formas geométricas no espaço.","Relacionar formas, áreas e volumes."),
                    Entry("Funções","Mathematics","Mockup: observe relações entre variáveis e gráficos.","Interpretar o comportamento de funções."),
                    Entry("Medidas","Mathematics","Mockup: compare grandezas e unidades de medida.","Estimar e medir grandezas no espaço.")}}
            };
            AssetDatabase.CreateAsset(catalog,path); return catalog;
        }

        static void CreateUI(HubCatalog catalog)
        {
            var go=new GameObject("Tecaverso Hub UI",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster),typeof(TrackedDeviceGraphicRaycaster));
            Undo.RegisterCreatedObjectUndo(go,"Create Figma Hub UI");
            var canvas=go.GetComponent<Canvas>(); canvas.renderMode=RenderMode.WorldSpace; canvas.worldCamera=Camera.main;
            var rt=(RectTransform)go.transform; rt.sizeDelta=new Vector2(2560,1440);
            rt.position=new Vector3(0,1.95f,3.2f); rt.localScale=Vector3.one*.002f;
            var page1=Image(rt,"HUB 1 - Discipline Selection",0,0,2560,1440,Background,true).rectTransform;
            var page2=Image(rt,"HUB 2 - Content Selection",0,0,2560,1440,Background,true).rectTransform;
            Label(page1,"Escolher disciplina",500,150,1560,130,100,heavy,Ink,true);
            var disciplineButtons=new Button[4];
            for(int i=0;i<4;i++)
            {
                var d=catalog.disciplines[i];
                var card=Panel(page1,"Discipline - "+d.title,300+i*480,440,430,650,i==0?8:2,i==0?Blue:Navy,Surface,out var fill);
                Image(card,"Color strip",0,0,430,18,d.accent);
                Picture(card,d.icon,105,28,220,220);
                Label(card,d.title,0,285,430,55,38,bold,Ink,true);
                var body=Label(card,d.description,40,370,350,130,24,regular,Ink,true); body.textWrappingMode=TextWrappingModes.Normal;
                Label(card,"Abrir disciplina",0,565,430,40,22,bold,Blue,true);
                disciplineButtons[i]=Clickable(card,fill);
            }
            var heading=Label(page2,"Física",290,130,1600,130,100,heavy,Ink);
            var sub=Label(page2,"SELECIONE O CONTEÚDO DA SIMULAÇÃO",295,260,1975,40,24,bold,Blue);
            var cards=new HubMenuController.ContentCard[6];
            for(int i=0;i<6;i++)
            {
                var c=catalog.disciplines[0].contents[i];
                var card=Panel(page2,"Content "+i,290+i%3*390,360+i/3*280,360,250,i==0?7:2,Navy,i==0?Blue:Surface,out var fill);
                var icon=Picture(card,c.icon,105,0,150,150);
                var label=Label(card,c.title,20,150,320,70,24,bold,i==0?Color.white:Ink,true); label.textWrappingMode=TextWrappingModes.Normal;
                cards[i]=new HubMenuController.ContentCard{button=Clickable(card,fill),surface=fill,icon=icon,label=label};
            }
            Panel(page2,"Detail visual",1560,360,710,410,2,Navy,Blue,out _);
            var detailIcon=Picture(page2,catalog.disciplines[0].contents[0].icon,1765,390,330,330);
            var detail=Label(page2,"Movimento e forças",1560,825,710,65,42,bold,Ink);
            var description=Label(page2,catalog.disciplines[0].contents[0].description,1560,905,710,135,26,regular,Ink);
            description.textWrappingMode=TextWrappingModes.Normal;
            var objective=Label(page2,"OBJETIVO  ·  "+catalog.disciplines[0].contents[0].objective,1560,1055,710,95,22,bold,Blue);
            objective.textWrappingMode=TextWrappingModes.Normal;
            var launch=Action(page2,"INICIAR SIMULAÇÃO",1560,1165,710,84,out var launchText);
            var status=Label(page2,"Disponível · Lançamento oblíquo",290,1175,1190,90,26,regular,Navy);
            status.textWrappingMode=TextWrappingModes.Normal;
            // VR-accessible commands replace the desktop-only Figma key prompts.
            Label(page1,"APONTE E PRESSIONE O GATILHO PARA ABRIR UMA DISCIPLINA",1090,1332,1380,50,24,regular,Ink);
            Label(page2,"APONTE E PRESSIONE O GATILHO PARA SELECIONAR",1360,1332,1150,50,24,regular,Ink);
            var back=Action(rt,"VOLTAR ÀS DISCIPLINAS",90,1310,650,82,out _);
            var controller=go.AddComponent<HubMenuController>();
            controller.Bind(catalog,page1.gameObject,page2.gameObject,disciplineButtons,cards,back,launch,heading,sub,detail,description,objective,launchText,status,detailIcon);
            page2.gameObject.SetActive(false); back.gameObject.SetActive(false);
            // Preserve the existing LAN controls; move their small canvas beside the main board.
            var connection=GameObject.Find("Connection Canvas");
            if(connection!=null){Undo.RecordObject(connection.transform,"Place LAN controls beside Hub");connection.transform.position=new Vector3(-3.5f,1.35f,2.8f);connection.transform.rotation=Quaternion.Euler(0,-25,0);}
            PrefabUtility.SaveAsPrefabAssetAndConnect(go,"Assets/_Project/Hub/HubUI.prefab",InteractionMode.AutomatedAction);
        }

        static void CreateRoom()
        {
            Directory.CreateDirectory(RoomFolder); AssetDatabase.Refresh();
            string meshPath=RoomFolder+"/UnitCube.asset";
            cube=AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
            if(cube==null)
            {
                var shape=ShapeGenerator.GenerateCube(PivotLocation.Center,Vector3.one);
                cube=UnityEngine.Object.Instantiate(shape.GetComponent<MeshFilter>().sharedMesh); cube.name="Standard Room Cube";
                AssetDatabase.CreateAsset(cube,meshPath); UnityEngine.Object.DestroyImmediate(shape.gameObject);
            }
            var room=new GameObject("Tecaverso Standard Room"); Undo.RegisterCreatedObjectUndo(room,"Create standard Hub room");
            Material wall=Mat("Pearl Panels"),dark=Mat("Structural Graphite"),plinth=Mat("Blue Wall Base"),floor=Mat("Midnight Floor"),blue=Mat("Electric Blue"),white=Mat("White Light");
            Box(room.transform,"Floor",new Vector3(0,-.15f,0),new Vector3(14,.3f,14),floor,true);
            Box(room.transform,"Ceiling",new Vector3(0,7.1f,0),new Vector3(14,.2f,14),wall,true);
            for(int side=0;side<4;side++)
            {
                var section=new GameObject("Wall "+side).transform;section.SetParent(room.transform,false);section.localRotation=Quaternion.Euler(0,side*90,0);
                Box(section,"Wall shell",new Vector3(0,3.5f,7),new Vector3(14,7,.3f),wall,true);
                for(int i=0;i<4;i++)
                {
                    float x=-5.25f+i*3.5f;
                    Box(section,"Pearl panel",new Vector3(x,3.85f,6.79f),new Vector3(3.42f,5.3f,.18f),wall);
                    Box(section,"Blue plinth",new Vector3(x,.6f,6.75f),new Vector3(3.42f,1.2f,.25f),plinth);
                    Box(section,"Panel seam",new Vector3(x-1.75f,3.5f,6.77f),new Vector3(.045f,7,.2f),dark);
                }
                Box(section,"Upper graphite rail",new Vector3(0,6.55f,6.7f),new Vector3(14,.65f,.35f),dark);
                Box(section,"Upper blue strip",new Vector3(0,6.55f,6.49f),new Vector3(14,.12f,.06f),blue);
                Box(section,"Lower blue strip",new Vector3(0,.1f,6.58f),new Vector3(14,.05f,.06f),blue);
                foreach(float x in new[]{-6.65f,6.65f})
                {
                    Box(section,"Corner pier",new Vector3(x,3.5f,6.5f),new Vector3(.3f,7,.5f),wall);
                    Box(section,"Corner light",new Vector3(x,.6f,6.21f),new Vector3(.06f,1.05f,.04f),blue);
                }
                foreach(float x in new[]{-3.5f,3.5f})
                {
                    Box(section,"Lamp housing",new Vector3(x,6.05f,6.48f),new Vector3(1.25f,.14f,.34f),dark);
                    Box(section,"Lamp diffuser",new Vector3(x,5.97f,6.46f),new Vector3(1.08f,.05f,.28f),white);
                }
            }
            for(int i=0;i<2;i++)
            {
                var lamp=new GameObject("Soft wall wash");lamp.transform.SetParent(room.transform,false);
                lamp.transform.localPosition=new Vector3(i==0?-4:4,5.8f,5.8f);lamp.transform.localRotation=Quaternion.Euler(50,0,0);
                var light=lamp.AddComponent<Light>();light.type=LightType.Spot;light.range=12;light.spotAngle=100;light.intensity=4;
                light.color=new Color(.72f,.8f,1);light.shadows=LightShadows.None;
            }
            var old=GameObject.Find("Environment");
            if(old!=null) foreach(Transform child in old.transform)
                if(child.GetComponent<Light>()==null){Undo.RecordObject(child.gameObject,"Archive template environment");child.gameObject.SetActive(false);}
            var props=GameObject.Find("NetworkInteractables");if(props!=null){Undo.RecordObject(props,"Hide template props");props.SetActive(false);}
            RenderSettings.ambientMode=AmbientMode.Trilight;RenderSettings.ambientSkyColor=new Color(.57f,.65f,.83f);
            RenderSettings.ambientEquatorColor=new Color(.32f,.39f,.55f);RenderSettings.ambientGroundColor=new Color(.11f,.15f,.24f);
            PrefabUtility.SaveAsPrefabAssetAndConnect(room,RoomFolder+"/TecaversoStandardRoom.prefab",InteractionMode.AutomatedAction);
        }
        static void Box(Transform parent,string name,Vector3 position,Vector3 size,Material material,bool collider=false)
        {
            var go=new GameObject(name,typeof(MeshFilter),typeof(MeshRenderer));go.transform.SetParent(parent,false);
            go.transform.localPosition=position;go.transform.localScale=size;go.isStatic=true;
            go.GetComponent<MeshFilter>().sharedMesh=cube;go.GetComponent<MeshRenderer>().sharedMaterial=material;
            if(collider)go.AddComponent<BoxCollider>();
        }
        static RectTransform Panel(Transform parent,string name,float x,float y,float w,float h,int border,Color edge,Color fill,out Image surface)
        {
            var outer=Image(parent,name,x,y,w,h,edge,true).rectTransform;
            surface=Image(outer,"Surface",border,border,w-border*2,h-border*2,fill);return outer;
        }
        static TMP_Text Label(Transform parent,string value,float x,float y,float w,float h,float size,TMP_FontAsset font,Color color,bool center=false)
        {
            var text=Text(parent,value,x,y,w,h,size,font,color);if(center)text.alignment=TextAlignmentOptions.Top;return text;
        }
        static Image Picture(Transform parent,Sprite sprite,float x,float y,float w,float h)
        {
            var image=Image(parent,sprite.name,x,y,w,h,Color.white);image.sprite=sprite;image.preserveAspect=true;return image;
        }
        static Button Clickable(RectTransform rt,Image target)
        {
            var button=rt.gameObject.AddComponent<Button>();button.targetGraphic=target;button.navigation=new Navigation{mode=Navigation.Mode.None};
            var colors=button.colors;colors.highlightedColor=new Color(.88f,.93f,1);colors.pressedColor=new Color(.7f,.8f,1);button.colors=colors;
            rt.gameObject.AddComponent<ButtonTweenFeedback>();return button;
        }
        static Button Action(Transform parent,string title,float x,float y,float w,float h,out TMP_Text label)
        {
            var im=Image(parent,title,x,y,w,h,Blue,true);var button=Clickable(im.rectTransform,im);
            label=Label(im.transform,title,0,0,w,h,28,bold,Color.white);label.alignment=TextAlignmentOptions.Center;return button;
        }
        static Sprite Icon(string name)=>AssetDatabase.LoadAssetAtPath<Sprite>(Art+name+".png");
        static TMP_FontAsset Font(string weight)=>AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-"+weight+" SDF.asset");
        static Material Mat(string name)=>AssetDatabase.LoadAssetAtPath<Material>(Materials+name+".mat");
        static Color Hex(string value){ColorUtility.TryParseHtmlString("#"+value,out var color);return color;}
    }
}
