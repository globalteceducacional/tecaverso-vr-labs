using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Tecaverso.Labs.Chemistry.PeriodicBonds;
using Object = UnityEngine.Object;

namespace Tecaverso.Editor
{
    /// <summary>Local board migration. The room, XR rig, sockets, vitrine and user objects are untouched.</summary>
    public static class PeriodicTableBoardAuthoring
    {
        const string ScenePath="Assets/_Project/Laboratories/Chemistry/PeriodicBonds/Scenes/PeriodicBonds.unity";
        static readonly Color Purple=new Color32(118,72,232,255), Ink=new Color32(22,22,22,255);
        static readonly Color Lavender=new Color32(232,230,254,255), Tile=new Color32(203,204,226,255);
        static TMP_FontAsset regular,bold;

        [MenuItem("Tecaverso/Atualizar tabela periódica completa (uma vez)")]
        public static void Upgrade()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Saia do Play Mode.");
            var scene=SceneManager.GetSceneByPath(ScenePath);
            bool opened=!scene.isLoaded;
            if(opened)scene=EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Additive);
            if(scene.isDirty)throw new InvalidOperationException("A sala tem alterações não salvas. Preserve-as antes de atualizar a tabela.");
            var previous=SceneManager.GetActiveScene();SceneManager.SetActiveScene(scene);
            try
            {
                var root=scene.GetRootGameObjects().First(g=>g.name=="Tabela periódica e ligações").transform;
                var lab=root.GetComponent<PeriodicBondsLab>();
                Build(root,lab);
                EditorSceneManager.SaveScene(scene);
                Debug.Log("Tabela periódica atualizada: 118 elementos consultáveis; cinco com peças de montagem.");
            }
            finally
            {
                if(previous.isLoaded && previous!=scene)SceneManager.SetActiveScene(previous);
                if(opened)EditorSceneManager.CloseScene(scene,true);
            }
        }

        public static void Build(Transform root,PeriodicBondsLab lab)
        {
            regular=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Regular SDF.asset");
            bold=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Bold SDF.asset");
            if(!regular||!bold)throw new InvalidOperationException("Fontes Montserrat ausentes.");
            var board=Board(root,"Tabela periódica",new Vector3(0,2.5f,4.65f),Quaternion.identity,new Vector2(2940,1420));
            var side=Board(root,"Lantanídeos e actinídeos",new Vector3(5.1f,2.3f,1.3f),Quaternion.Euler(0,90,0),new Vector2(2650,650));
            Label(board,"TABELA PERIÓDICA · 118 ELEMENTOS",new Vector2(0,655),new Vector2(2850,70),50,true,Purple);
            Label(board,"ROXO: PEÇAS DISPONÍVEIS PARA LIGAÇÕES",new Vector2(0,590),new Vector2(2800,55),28,false,Ink);
            for(int group=1;group<=18;group++)
                Label(board,group.ToString(),new Vector2(-1320+(group-1)*155,530),new Vector2(100,45),24,true,Purple);
            for(int period=1;period<=7;period++)
                Label(board,period.ToString(),new Vector2(-1450,470-(period-1)*143),new Vector2(55,110),29,true,Purple);
            var buttons=new Button[PeriodicElementCatalog.All.Length];
            foreach(var element in PeriodicElementCatalog.All)
            {
                if(element.group==0)continue;
                var position=new Vector2(-1320+(element.group-1)*155,470-(element.period-1)*143);
                buttons[element.atomicNumber-1]=TileButton(board,element,position);
            }
            Placeholder(board,"57–71\nLANT. →",new Vector2(-1320+2*155,470-5*143));
            Placeholder(board,"89–103\nACT. →",new Vector2(-1320+2*155,470-6*143));
            lab.elementInfo=Label(board,"Selecione um elemento para ver nome, número e posição.",
                new Vector2(0,-565),new Vector2(2820,190),29,false,Ink);

            Label(side,"LANTANÍDEOS · ACTINÍDEOS",new Vector2(0,275),new Vector2(2450,70),47,true,Purple);
            for(int i=57;i<=71;i++)buttons[i-1]=TileButton(side,PeriodicElementCatalog.All[i-1],new Vector2(-1120+(i-57)*160,85));
            for(int i=89;i<=103;i++)buttons[i-1]=TileButton(side,PeriodicElementCatalog.All[i-1],new Vector2(-1120+(i-89)*160,-75));
            lab.secondaryElementInfo=Label(side,"Selecione um elemento desta série.",
                new Vector2(0,-260),new Vector2(2500,140),27,false,Ink);
            if(buttons.Any(b=>!b))throw new InvalidOperationException("A tabela deve conter 118 botões.");
            lab.tableButtons=buttons;
        }

        static RectTransform Board(Transform root,string name,Vector3 position,Quaternion rotation,Vector2 size)
        {
            var existing=root.Find(name);
            RectTransform rect;
            if(existing)rect=existing.GetComponent<RectTransform>();
            else
            {
                rect=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();
                rect.SetParent(root,false);
                rect.gameObject.AddComponent<Canvas>().renderMode=RenderMode.WorldSpace;
                rect.gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();
            }
            for(int i=rect.childCount-1;i>=0;i--)Object.DestroyImmediate(rect.GetChild(i).gameObject);
            rect.localPosition=position;rect.localRotation=rotation;rect.localScale=Vector3.one*.0022f;rect.sizeDelta=size;
            var image=rect.GetComponent<Image>() ?? rect.gameObject.AddComponent<Image>();
            image.color=Lavender;image.raycastTarget=false;
            return rect;
        }
        static RectTransform Rect(Transform parent,string name,Vector2 position,Vector2 size)
        {
            var rect=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent,false);rect.anchoredPosition=position;rect.sizeDelta=size;return rect;
        }
        static TMP_Text Label(Transform parent,string value,Vector2 position,Vector2 size,int fontSize,bool strong,Color color)
        {
            var rect=Rect(parent,value,position,size);
            var label=rect.gameObject.AddComponent<TextMeshProUGUI>();label.text=value;label.font=strong?bold:regular;
            label.fontSize=fontSize;label.color=color;label.alignment=TextAlignmentOptions.Center;
            label.raycastTarget=false;label.enableWordWrapping=true;return label;
        }
        static Button TileButton(RectTransform parent,PeriodicElement element,Vector2 position)
        {
            var rect=Rect(parent,element.atomicNumber+" "+element.symbol,position,new Vector2(140,126));
            bool active=Array.IndexOf(BondCatalog.Symbols,element.symbol)>=0;
            var image=rect.gameObject.AddComponent<Image>();image.color=active?Purple:Tile;
            var button=rect.gameObject.AddComponent<Button>();
            var ink=active?Color.white:Ink;
            Label(rect,element.atomicNumber.ToString(),new Vector2(-42,43),new Vector2(48,29),18,false,ink);
            Label(rect,element.symbol,new Vector2(0,-7),new Vector2(130,68),46,true,ink);
            return button;
        }
        static void Placeholder(RectTransform parent,string value,Vector2 position)
        {
            var rect=Rect(parent,value,position,new Vector2(140,126));
            var image=rect.gameObject.AddComponent<Image>();image.color=new Color32(170,170,196,255);
            Label(rect,value,Vector2.zero,rect.sizeDelta,24,true,Ink);
        }
    }
}
