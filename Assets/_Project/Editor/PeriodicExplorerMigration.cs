using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Tecaverso.Hub;
using Tecaverso.Labs.Chemistry.PeriodicBonds;
using Object=UnityEngine.Object;
namespace Tecaverso.Editor
{
    public static class PeriodicExplorerMigration
    {
        const string Folder="Assets/_Project/Laboratories/Chemistry/PeriodicBonds";
        static TMP_FontAsset font;
        static Color blue=new Color(.157f,.306f,.627f);
        static Material tile,edge;
        public static void Run()
        {
            if(EditorApplication.isPlaying)throw new Exception("Saia do Play Mode.");
            var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if(scene.path!=Folder+"/Scenes/PeriodicBonds.unity")throw new Exception("Abra PeriodicBonds.");
            if(scene.GetRootGameObjects().Any(g=>g.GetComponent<PeriodicTableExplorer>()))throw new Exception("Migração já aplicada.");
            // Preserve existing authorial changes before replacing only the obsolete laboratory root.
            EditorSceneManager.SaveScene(scene);
            font=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Regular SDF.asset");
            tile=Mat("Cubos tabela",new Color(.22f,.32f,.6f));edge=Mat("Contorno tabela",new Color(.95f,.8f,.25f));
            var root=new GameObject("Tabela periódica 3D");var lab=root.AddComponent<PeriodicTableExplorer>();
            lab.data=AssetDatabase.LoadAssetAtPath<TextAsset>(Folder+"/Data/PubChemPeriodicTable.json");
            var data=JsonUtility.FromJson<PeriodicTableExplorer.Dataset>(lab.data.text);
            if(data.Table.Row.Length!=118)throw new Exception("Base incompleta.");
            lab.cubes=new PeriodicElementCube[118];
            foreach(var e in PeriodicElementCatalog.All)
            {
                bool series=e.group==0;
                float x=series?-3.4f+(e.atomicNumber-(e.IsLanthanide?57:89))*.46f:-4.08f+(e.group-1)*.48f;
                float y=series?(e.IsLanthanide?1.12f:.64f):4.65f-(e.period-1)*.47f;
                var go=GameObject.CreatePrimitive(PrimitiveType.Cube);go.name=e.atomicNumber+" · "+e.name;
                go.transform.SetParent(root.transform,false);go.transform.localPosition=new Vector3(x,y,4.25f);go.transform.localScale=new Vector3(.41f,.40f,.32f);
                var r=go.GetComponent<Renderer>();r.sharedMaterial=tile;
                var c=go.AddComponent<PeriodicElementCube>();c.number=e.atomicNumber;c.explorer=lab;c.surface=r;
                string category=data.Table.Row[e.atomicNumber-1].Cell[15];
                c.color=category.Contains("Noble")?new Color(.42f,.27f,.64f):category.Contains("Halogen")?new Color(.12f,.44f,.48f):category.Contains("Nonmetal")?new Color(.13f,.43f,.29f):e.group==0?new Color(.48f,.24f,.43f):new Color(.16f,.32f,.62f);
                var initial=new MaterialPropertyBlock();initial.SetColor("_BaseColor",c.color);r.SetPropertyBlock(initial);
                var outline=new GameObject("Contorno de foco");outline.transform.SetParent(go.transform,false);c.outline=outline;
                for(int k=0;k<4;k++){
                    var line=GameObject.CreatePrimitive(PrimitiveType.Cube);line.transform.SetParent(outline.transform,false);
                    line.transform.localPosition=new Vector3(k<2?(k==0?-.51f:.51f):0,k>=2?(k==2?-.51f:.51f):0,-.52f);
                    line.transform.localScale=k<2?new Vector3(.025f,1.04f,.025f):new Vector3(1.04f,.025f,.025f);line.GetComponent<Renderer>().sharedMaterial=edge;Object.DestroyImmediate(line.GetComponent<Collider>());
                }outline.SetActive(false);
                Text3(root.transform,e.atomicNumber+"\n<size=155%><b>"+e.symbol+"</b></size>\n"+e.name,new Vector3(x,y,4.075f),new Vector2(.39f,.39f),.53f);
                lab.cubes[e.atomicNumber-1]=c;
            }
            Text3(root.transform,"TABELA PERIÓDICA · 118 ELEMENTOS",new Vector3(0,5.08f,4.05f),new Vector2(8.5f,.35f),1.8f);
            Text3(root.transform,"APONTE E SELECIONE UM CUBO PARA EXPLORAR",new Vector3(0,5.4f,4.05f),new Vector2(8.5f,.25f),.9f);
            Text3(root.transform,"57–71\nLantanídeos ↓",new Vector3(-3.12f,2.3f,4.05f),new Vector2(.43f,.4f),.46f);
            Text3(root.transform,"89–103\nActinídeos ↓",new Vector3(-3.12f,1.83f,4.05f),new Vector2(.43f,.4f),.46f);
            var panel=new GameObject("Ficha do elemento",typeof(RectTransform),typeof(Canvas),typeof(TrackedDeviceGraphicRaycaster));panel.transform.SetParent(root.transform,false);
            var rect=(RectTransform)panel.transform;rect.position=new Vector3(4.65f,2.2f,.3f);rect.rotation=Quaternion.Euler(0,65,0);rect.localScale=Vector3.one*.0022f;rect.sizeDelta=new Vector2(1500,1550);panel.GetComponent<Canvas>().renderMode=RenderMode.WorldSpace;
            panel.AddComponent<Image>().color=new Color(.91f,.9f,.99f);
            lab.heading=Label(rect,"TABELA PERIÓDICA",new Vector2(0,675),new Vector2(1380,110),52);
            lab.pages=new Button[4];string[] titles={"Visão geral","Propriedades","Na natureza","Fontes"};
            for(int i=0;i<4;i++)lab.pages[i]=Button(rect,titles[i],new Vector2(-540+i*360,555),new Vector2(340,85));
            lab.pageLabel=Label(rect,"VISÃO GERAL",new Vector2(0,445),new Vector2(1380,65),30);
            lab.body=Label(rect,"Aponte para um cubo e pressione o gatilho.\n\nTodos os 118 elementos podem ser consultados.",new Vector2(0,-25),new Vector2(1340,820),33);lab.body.alignment=TextAlignmentOptions.TopLeft;
            lab.clear=Button(rect,"Limpar seleção",new Vector2(-360,-665),new Vector2(660,95));lab.back=Button(rect,"Voltar ao Hub",new Vector2(360,-665),new Vector2(660,95));
            var old=scene.GetRootGameObjects().FirstOrDefault(g=>g.name=="Tabela periódica e ligações");if(old)Object.DestroyImmediate(old);
            var catalog=AssetDatabase.LoadAssetAtPath<HubCatalog>("Assets/_Project/Hub/HubCatalog.asset");
            foreach(var d in catalog.disciplines)foreach(var content in d.contents)if(content.scenePath==scene.path){content.title="Tabela periódica";content.description="Explore 118 elementos em uma tabela 3D e consulte suas propriedades.";content.objective="Relacionar organização periódica, propriedades e ocorrência dos elementos.";}
            EditorUtility.SetDirty(catalog);AssetDatabase.SaveAssets();EditorSceneManager.SaveScene(scene);
            Selection.activeGameObject=root;
        }
        static Material Mat(string name,Color color){string path=Folder+"/Materials/"+name+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);if(!m){m=new Material(Shader.Find("Universal Render Pipeline/Lit"));m.SetColor("_BaseColor",color);AssetDatabase.CreateAsset(m,path);}return m;}
        static TMP_Text Text3(Transform parent,string text,Vector3 pos,Vector2 size,float fs){var t=new GameObject(text).AddComponent<TextMeshPro>();t.transform.SetParent(parent,false);t.transform.localPosition=pos;t.rectTransform.sizeDelta=size;t.font=font;t.text=text;t.fontSize=fs;t.color=Color.white;t.alignment=TextAlignmentOptions.Center;t.textWrappingMode=TextWrappingModes.Normal;return t;}
        static RectTransform Rect(Transform p,string name,Vector2 pos,Vector2 size){var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(p,false);r.anchoredPosition=pos;r.sizeDelta=size;return r;}
        static TMP_Text Label(Transform p,string text,Vector2 pos,Vector2 size,float fs){var r=Rect(p,text,pos,size);var t=r.gameObject.AddComponent<TextMeshProUGUI>();t.font=font;t.text=text;t.fontSize=fs;t.color=new Color(.086f,.086f,.086f);t.alignment=TextAlignmentOptions.Center;t.raycastTarget=false;t.overflowMode=TextOverflowModes.Truncate;return t;}
        static Button Button(Transform p,string text,Vector2 pos,Vector2 size){var r=Rect(p,text,pos,size);r.gameObject.AddComponent<Image>().color=blue;var b=r.gameObject.AddComponent<Button>();Label(r,text,Vector2.zero,size,29).color=Color.white;return b;}
    }
}
