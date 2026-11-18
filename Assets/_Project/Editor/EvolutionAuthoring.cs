using System;
using System.Linq;
using Tecaverso.Interaction;
using Tecaverso.Labs.Biology.Evolution;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Tecaverso.Editor
{
    public static class EvolutionAuthoring
    {
        public const string Folder = "Assets/_Project/Laboratories/Biology/Evolution";
        public const string ScenePath = Folder + "/Scenes/Evolution.unity";
        const string Art = Folder + "/Art/";
        const string OfficialButton = "Assets/Samples/XR Interaction Toolkit/3.5.1/Starter Assets/DemoAssets/Prefabs/Interactables/Push Button.prefab";
        const string Lit = "Universal Render Pipeline/Lit";
        const float StageHeight = 1.155f, ReadoutHeight = 1.735f, SupportHeight = 1.005f, SupportDepth = .48f;
        const float CountFont = 32f, MessageFont = 27f;

        public static void Create()
        {
            var button = AssetDatabase.LoadAssetAtPath<GameObject>(OfficialButton);
            if (!button || !Shader.Find("Tecaverso/Biology/Bark") || !Shader.Find("Tecaverso/Biology/MothPigment"))
                throw new InvalidOperationException("Importe os shaders da sala e o botão oficial do XRI antes da autoria.");
            var scene = BiologyRoomAuthoring.Begin(ScenePath);
            var root = new GameObject("Evolução · terrário de mariposas");
            root.SetActive(false);
            var lab = root.AddComponent<EvolutionLab>();
            var graphite = Material("Grafite",new Color32(36,48,68,255));
            var metal = Material("Aro",new Color32(91,111,128,255));
            lab.stepAvailable = Material("Tecla primária",new Color32(40,78,160,255));
            lab.stepUnavailable = metal;
            var accent = Material("Biologia",new Color32(62,211,156,255));
            CreateHabitat(lab,graphite,accent,metal);
            CreateConsole(lab,button,graphite,metal);
            CreateDisplay(lab,graphite);
            CreateConsultation(lab);
            lab.PresentInitial();
            root.SetActive(true);
            if (!EditorSceneManager.SaveScene(scene,ScenePath)) throw new InvalidOperationException("Falha ao salvar o terrário.");
            AssetDatabase.SaveAssets();
        }

        static void CreateHabitat(EvolutionLab lab, Material graphite, Material accent, Material metal)
        {
            var stage = Child(lab.transform,"Amostra de casca",new Vector3(0,StageHeight,.52f));
            stage.localRotation = Quaternion.Euler(-22,0,0);
            Box(stage,"Estrutura", new Vector3(0,-.085f,0),new Vector3(2.19f,.14f,1.32f),graphite);
            Box(stage,"Filete Biologia",new Vector3(0,-.034f,0),new Vector3(2.197f,.014f,1.327f),accent);
            Box(stage,"Moldura",new Vector3(0,-.015f,0),new Vector3(2.18f,.025f,1.31f),metal);
            lab.lightBark = BarkMaterial("Casca clara",new Color32(182,162,131,255),new Color32(125,107,87,255));
            lab.darkBark = BarkMaterial("Casca escura",new Color32(85,83,89,255),new Color32(50,52,61,255));
            lab.bark = Box(stage,"Superfície de casca",Vector3.zero,new Vector3(2.13f,.026f,1.26f),lab.lightBark);
            for (int side=-1;side<=1;side+=2)
                Box(lab.transform,"Suporte",new Vector3(.77f*side,SupportHeight*.5f,.52f),new Vector3(.20f,SupportHeight,SupportDepth),graphite);
            lab.lightMoth = Pigment("Asa clara",new Color32(232,207,158,255),new Color32(153,127,91,255),new Color32(108,87,66,255));
            lab.darkMoth = Pigment("Asa escura",new Color32(64,64,72,255),new Color32(116,109,114,255),new Color32(48,44,48,255));
            var mesh = EvolutionMothMesh.GetOrCreate(Art+"Mariposa.asset");
            lab.moths = new MeshRenderer[SelectionPopulation.Size];
            for (int i=0;i<lab.moths.Length;i++)
            {
                int row=i/6, column=i%6;
                float x=(column-2.5f)*.336f + Mathf.Sin(2.61f*i+.7f)*.046f;
                float z=(row-1.5f)*.291f + Mathf.Cos(2.14f*i)*.036f;
                var moth = Child(stage,$"Mariposa {i+1:00}",new Vector3(x,.015f,z));
                moth.localRotation=Quaternion.Euler(0,(i*73+row*11)%360,0);
                moth.gameObject.AddComponent<MeshFilter>().sharedMesh=mesh;
                lab.moths[i]=moth.gameObject.AddComponent<MeshRenderer>();
            }
        }

        static void CreateConsole(EvolutionLab lab, GameObject buttonPrefab, Material graphite, Material metal)
        {
            var deck = Child(lab.transform,"Borda de comando",new Vector3(0,.87f,-.335f));
            deck.localRotation=Quaternion.Euler(-24,0,0);
            Box(deck,"Tampo",new Vector3(0,-.035f,0),new Vector3(2.19f,.07f,.42f),graphite);
            var leverRoot = Child(deck,"Ambiente",new Vector3(-.79f,.005f,.045f));
            leverRoot.localRotation=Quaternion.Euler(0,90,0);
            lab.habitat=leverRoot.gameObject.AddComponent<XRLever>();
            Unity3DControlVisuals.Lever(lab.habitat,.85f);
            lab.habitat.value=false;
            DeckCaption(deck,"Clara     /     Escura",new Vector3(-.79f,.003f,-.142f),265,22);

            var key=(GameObject)PrefabUtility.InstantiatePrefab(buttonPrefab,deck);
            key.name="Avançar geração · botão XR";
            key.transform.localPosition=new Vector3(-.29f,.102f,.025f);
            key.transform.localRotation=Quaternion.identity;
            key.transform.localScale=Vector3.one*1.18f;
            lab.step=key.GetComponent<XRSimpleInteractable>();
            var faces=key.GetComponentsInChildren<MeshRenderer>();
            lab.stepCap=faces.Single(r=>r.name=="EmergencyStop");
            foreach (var face in faces)
                face.sharedMaterials = face.sharedMaterials.Select((_,i)=>face==lab.stepCap && i==0 ? lab.stepAvailable : metal).ToArray();
            var motion=key.GetComponent<XRInteractionMotion>()??key.AddComponent<XRInteractionMotion>();
            motion.visualTargets=new[]{lab.stepCap};
            lab.stepCaption=DeckCaption(deck,"Avançar geração",new Vector3(-.29f,.003f,-.142f),245,22);

            var panel=BiologyWorldUI.Panel(deck,"C2 · Registro e consulta",new Vector3(.495f,.005f,-.007f),
                new Vector2(735,214),.00145f,Quaternion.Euler(90,0,0));
            lab.actions=panel.gameObject;
            var body=BiologyWorldUI.Body(panel,10);
            var first=Row(body,"Registro",86);
            lab.save=BiologyWorldUI.Button(first,"Guardar referência");
            lab.restore=BiologyWorldUI.Button(first,"Restaurar população");
            var second=Row(body,"Consulta e saída",86);
            lab.about=BiologyWorldUI.Button(second,"Sobre a seleção");
            lab.back=BiologyWorldUI.Button(second,"Voltar ao Hub");
        }

        static TMP_Text DeckCaption(Transform deck,string title,Vector3 point,float width,float font)
        {
            var text=BiologyWorldUI.Caption(deck,title,point,new Vector2(width,60),font);
            text.transform.parent.localRotation=Quaternion.Euler(90,0,0);
            return text;
        }

        static void CreateDisplay(EvolutionLab lab, Material graphite)
        {
            var sign=Child(lab.transform,"Visor fixo",new Vector3(0,ReadoutHeight,1.145f));
            Box(sign,"Fundo do visor",Vector3.zero,new Vector3(2.19f,.61f,.055f),graphite);
            BiologyWorldUI.Caption(sign,"Evolução e seleção natural",new Vector3(0,.218f,-.03f),new Vector2(1400,74),40);
            lab.counts=BiologyWorldUI.Caption(sign,"População",new Vector3(-.52f,.018f,-.031f),new Vector2(670,114),CountFont);
            lab.reference=BiologyWorldUI.Caption(sign,"Referência",new Vector3(.55f,.018f,-.031f),new Vector2(640,114),CountFont);
            lab.status=BiologyWorldUI.Caption(sign,"Guarde uma referência e avance uma geração.",
                new Vector3(0,-.194f,-.031f),new Vector2(1380,85),MessageFont,false);
        }

        static void CreateConsultation(EvolutionLab lab)
        {
            var point=new Vector3(1.78f,1.35f,.52f);
            var direction=Quaternion.Euler(0,17,0);
            var card=BiologyWorldUI.Panel(lab.transform,"Q1 · Seleção natural",point,new Vector2(680,525),.00165f,direction);
            lab.consultation=card.gameObject;
            var content=BiologyWorldUI.Body(card,24);
            BiologyWorldUI.Text(content,"Seleção natural",68,37,true);
            var text=BiologyWorldUI.Text(content,
                "A camuflagem favorece mariposas com pigmentação semelhante à casca. Elas contribuem mais para a geração seguinte.\n\n" +
                "O modelo usa pesos 1 e 0,7 para calcular 24 descendentes. O arredondamento pode manter uma contagem igual. " +
                "A cor é herdada. O modelo simplifica a reprodução e omite mutações e deriva genética.",302,24);
            text.alignment=TextAlignmentOptions.TopLeft;
            lab.close=BiologyWorldUI.Button(content,"Fechar consulta",64);

            var dialog=BiologyWorldUI.Panel(lab.transform,"X1 · Restaurar",point,new Vector2(680,304),.00165f,direction);
            lab.confirmation=dialog.gameObject;
            var confirm=BiologyWorldUI.Body(dialog,24);
            BiologyWorldUI.Text(confirm,"Restaurar população?",60,35,true);
            BiologyWorldUI.Text(confirm,"A casca volta a ser clara, a G0 retorna com 12 de cada fenótipo e a referência é apagada.",82,25);
            var row=Row(confirm,"Decisão",70);
            lab.cancel=BiologyWorldUI.Button(row,"Cancelar");
            lab.confirm=BiologyWorldUI.Button(row,"Restaurar");
        }
        // Targeted correction after reviewing the front and close views. Child poses and serialized
        // XR references stay intact. This also applies the same measurements at first authoring.
        public static void RefineViewingClearance()
        {
            var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (Application.isPlaying || scene.path!=ScenePath)
                throw new InvalidOperationException("Abra a sala Evolution em Edit Mode.");
            var lab=scene.GetRootGameObjects().Select(g=>g.GetComponent<EvolutionLab>()).Single(l=>l);
            var stage=lab.bark.transform.parent;
            var sign=lab.counts.transform.parent.parent;
            stage.localPosition=new Vector3(0,StageHeight,.52f);
            sign.localPosition=new Vector3(0,ReadoutHeight,1.145f);
            foreach(Transform support in lab.transform)
            {
                if(support.name!="Suporte") continue;
                var anchor=support.localPosition;
                anchor.y=SupportHeight*.5f;
                support.localPosition=anchor;
                support.localScale=new Vector3(.20f,SupportHeight,SupportDepth);
            }
            lab.counts.fontSize=lab.reference.fontSize=CountFont;
            lab.status.fontSize=MessageFont;
            EditorSceneManager.MarkSceneDirty(scene);
            if(!EditorSceneManager.SaveScene(scene)) throw new InvalidOperationException("Falha ao salvar o ajuste de visibilidade.");
        }
        static RectTransform Row(Transform parent,string title,float height)
        {
            var row=BiologyWorldUI.Rect(parent,title);
            BiologyWorldUI.Height(row,height);
            var layout=row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing=12;
            layout.childControlWidth=layout.childControlHeight=true;
            layout.childForceExpandWidth=layout.childForceExpandHeight=true;
            return row;
        }
        static Transform Child(Transform parent,string name,Vector3 position)
        {
            var child=new GameObject(name).transform;
            child.SetParent(parent,false);
            child.localPosition=position;
            return child;
        }
        static MeshRenderer Box(Transform parent,string name,Vector3 p,Vector3 size,Material material)
        {
            var box=GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.name=name;
            box.transform.SetParent(parent,false);
            box.transform.localPosition=p;
            box.transform.localScale=size;
            var renderer=box.GetComponent<MeshRenderer>();
            renderer.sharedMaterial=material;
            return renderer;
        }
        static Material Material(string name,Color color,string shaderName=Lit)
        {
            string path=Art+name+".mat";
            var m=AssetDatabase.LoadAssetAtPath<Material>(path);
            if(m) return m;
            m=new Material(Shader.Find(shaderName)){ name=name,enableInstancing=true };
            m.SetColor("_BaseColor",color);
            AssetDatabase.CreateAsset(m,path);
            return m;
        }
        static Material BarkMaterial(string name,Color baseColor,Color grain)
        {
            var material=Material(name,baseColor,"Tecaverso/Biology/Bark");
            material.SetColor("_GrainColor",grain);
            EditorUtility.SetDirty(material);
            return material;
        }
        static Material Pigment(string name,Color wings,Color marks,Color body)
        {
            var material=Material(name,wings,"Tecaverso/Biology/MothPigment");
            material.SetColor("_MarkColor",marks);
            material.SetColor("_BodyColor",body);
            EditorUtility.SetDirty(material);
            return material;
        }
    }
}
