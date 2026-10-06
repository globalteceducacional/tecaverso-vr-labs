using System;
using System.Linq;
using Tecaverso.Hub;
using Tecaverso.Labs.ObliqueLaunch;
using Tecaverso.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object=UnityEngine.Object;

namespace Tecaverso.Editor
{
    /// <summary>Idempotent structural migration; no room builders and no world-space transform edits.</summary>
    public static class UILayoutMigration
    {
        static T Ref<T>(Object source,string name) where T:Object => new SerializedObject(source).FindProperty(name).objectReferenceValue as T;
        static RectTransform Rect(Transform parent,string name,float x,float y,float width,float height)
            => ObliqueLaunchFigmaUIBuilder.Rect(parent,name,x,y,width,height);
        static LayoutElement Size(GameObject go,float width,float height)
        {
            if(!go.TryGetComponent<LayoutElement>(out var item))item=go.AddComponent<LayoutElement>();
            item.minWidth=0;item.minHeight=height;item.preferredWidth=width;item.preferredHeight=height;item.flexibleWidth=0;item.flexibleHeight=0;return item;
        }
        static RectTransform Flow(Transform parent,string name,float x,float y,float w,float h,bool horizontal,float gap,params GameObject[] children)
        {
            var root=Rect(parent,name,x,y,w,h);
            HorizontalOrVerticalLayoutGroup group=horizontal?root.gameObject.AddComponent<HorizontalLayoutGroup>():root.gameObject.AddComponent<VerticalLayoutGroup>();
            group.spacing=gap;group.childAlignment=TextAnchor.UpperLeft;group.childControlWidth=true;group.childControlHeight=true;
            group.childForceExpandWidth=false;group.childForceExpandHeight=false;
            foreach(var child in children)
            {
                var r=(RectTransform)child.transform;var size=r.rect.size;
                r.SetParent(root,false);r.localScale=Vector3.one;r.localRotation=Quaternion.identity;
                Size(child,horizontal?size.x:w,horizontal?h:size.y);
            }
            return root;
        }
        static void Stretch(RectTransform rect,float inset=0)
        { rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=Vector2.one*inset;rect.offsetMax=-Vector2.one*inset; }
        static void StretchHorizontal(RectTransform rect)
        {
            var parent=(RectTransform)rect.parent;float left=rect.anchoredPosition.x;float top=rect.anchoredPosition.y;
            float width=rect.rect.width,height=rect.rect.height;
            rect.anchorMin=new Vector2(0,1);rect.anchorMax=Vector2.one;rect.pivot=new Vector2(0,1);
            rect.sizeDelta=new Vector2(width-parent.rect.width,height);rect.anchoredPosition=new Vector2(left,top);
        }
        static void Grid(Transform parent,string name,float x,float y,Vector2 cell,Vector2 gap,int columns,GameObject[] cards)
        {
            int rows=Mathf.CeilToInt(cards.Length/(float)columns);
            var root=Rect(parent,name,x,y,columns*cell.x+(columns-1)*gap.x,rows*cell.y+(rows-1)*gap.y);
            var grid=root.gameObject.AddComponent<GridLayoutGroup>();grid.cellSize=cell;grid.spacing=gap;grid.constraint=GridLayoutGroup.Constraint.FixedColumnCount;grid.constraintCount=columns;
            foreach(var card in cards) { card.transform.SetParent(root,false);Size(card,cell.x,cell.y); }
        }
        static void FinalizeUI(GameObject root)
        {
            foreach(var feedback in root.GetComponentsInChildren<ButtonTweenFeedback>(true)) { feedback.EnsureVisual();EditorUtility.SetDirty(feedback); }
            foreach(var text in root.GetComponentsInChildren<TMP_Text>(true))
            { text.raycastTarget=false;text.overflowMode=TextOverflowModes.Ellipsis; }
            foreach(var image in root.GetComponentsInChildren<Image>(true))
                if(image.name=="Surface") { image.raycastTarget=false;Stretch(image.rectTransform,2); }
            Canvas.ForceUpdateCanvases();
            foreach(var group in root.GetComponentsInChildren<LayoutGroup>(true).Reverse())LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)group.transform);
        }
        static void Menu(GameObject root)
        {
            var menu=root.GetComponent<HubMenuController>();if(menu==null)return;
            foreach(var hint in root.GetComponentsInChildren<TMP_Text>(true).Where(t=>t.text.StartsWith("APONTE E PRESSIONE")))
            { hint.rectTransform.anchoredPosition=new Vector2(hint.rectTransform.anchoredPosition.x,-1264);hint.rectTransform.sizeDelta=new Vector2(hint.rectTransform.sizeDelta.x,36); }
            if(root.transform.Find("Navigation Footer")!=null)return;
            var one=Ref<GameObject>(menu,"disciplinesView");var two=Ref<GameObject>(menu,"contentsView");
            Grid(one.transform,"Discipline Grid",300,440,new Vector2(430,650),new Vector2(50,32),4,one.transform.Cast<Transform>().Where(t=>t.name.StartsWith("Discipline - ")).Select(t=>t.gameObject).ToArray());
            Grid(two.transform,"Content Grid",290,360,new Vector2(360,250),new Vector2(30,30),3,two.transform.Cast<Transform>().Where(t=>t.name.StartsWith("Content ")).Select(t=>t.gameObject).ToArray());
            var back=Ref<Button>(menu,"back");
            var footer=Flow(root.transform,"Navigation Footer",90,1310,2380,88,true,32,back.gameObject);
            footer.anchorMin=Vector2.zero;footer.anchorMax=new Vector2(1,0);footer.pivot=Vector2.zero;footer.offsetMin=new Vector2(90,42);footer.offsetMax=new Vector2(-90,130);
            var spacer=Rect(footer,"Flexible space",0,0,0,88);Size(spacer.gameObject,0,88).flexibleWidth=1;
            UIVisibility.Set(back.gameObject,false,true);UIVisibility.Set(one,true);UIVisibility.Set(two,false);
            Stretch((RectTransform)one.transform);Stretch((RectTransform)two.transform);
            FinalizeUI(root);
        }
        static void Entry(HubEntryController ui)
        {
            if(ui.entryPage.transform.Find("Entry Cards")!=null)return;
            var cards=new[]{ui.solo.transform.parent,ui.create.transform.parent,ui.join.transform.parent};
            Flow(ui.entryPage.transform,"Entry Cards",96,480,2256,520,true,48,cards.Select(t=>t.gameObject).ToArray());
            foreach(var card in cards)
            {
                var content=card.Cast<Transform>().Where(t=>t.name!="Surface").Select(t=>t.gameObject).ToArray();
                var stack=Flow(card,"Card Content",48,48,624,424,false,16,content);
                float[] heights={72,64,128,88};
                for(int i=0;i<content.Length;i++)Size(content[i],i==0?72:624,heights[i]);
                StretchHorizontal(stack);
            }
            var fields=ui.nickname.transform.parent;
            var fieldNodes=fields.Cast<Transform>().Where(t=>t.name!="Surface").Select(t=>t.gameObject).ToArray();
            Flow(fields,"Form Fields",48,48,884,584,false,16,fieldNodes);
            Flow(ui.formPage.transform,"Form Actions",96,1232,980,88,true,24,ui.formBack.gameObject,ui.submit.gameObject);
            var contentPanel=ui.chooseContent.transform.parent;
            Flow(contentPanel,"Lobby Actions",48,440,984,208,false,24,ui.chooseContent.gameObject,ui.start.gameObject,ui.ready.gameObject);
            UIVisibility.Set(ui.ready.gameObject,false,true);
            Flow(ui.retry.transform.parent,"Error Actions",48,520,1504,88,true,24,ui.retry.gameObject,ui.errorBack.gameObject);
            Flow(ui.cancelLeave.transform.parent,"Confirmation Actions",48,304,1264,88,true,24,ui.cancelLeave.gameObject,ui.confirmLeave.gameObject);
            var footer=ui.menu.transform.Find("Navigation Footer");ui.selectionBack.transform.SetParent(footer,false);Size(ui.selectionBack.gameObject,704,88);
            var dialog=(RectTransform)ui.cancelLeave.transform.parent.parent;
            dialog.anchorMin=dialog.anchorMax=dialog.pivot=new Vector2(.5f,.5f);dialog.anchoredPosition=Vector2.zero;
            foreach(var page in new[]{ui.entryPage,ui.formPage,ui.lobbyPage,ui.errorPage,ui.connectingPage,ui.leaveDialog})
            { Stretch((RectTransform)page.transform);UIVisibility.Set(page,page==ui.entryPage); }
            Stretch((RectTransform)ui.transform);
            UIVisibility.Set(ui.selectionBack.gameObject,false,true);ui.menu.Hide();
            // A role's controls share a column, not coincident manually positioned rectangles.
            FinalizeUI(ui.gameObject);FinalizeUI(footer.gameObject);
        }
        static void Laboratory(ObliqueLaunchPanel panel)
        {
            var root=panel.transform.Find("Figma Experiment UI");if(root==null||root.Find("Tab Bar")!=null)return;
            var tabs=root.Cast<Transform>().Where(t=>t.GetComponent<Button>()!=null).Select(t=>t.gameObject).ToArray();
            Flow(root,"Tab Bar",50,0,620,64,true,20,tabs);
            var motion=root.Find("Experiment - Projectile Motion");
            var title=motion.Cast<Transform>().First(t=>t.name=="PARÂMETROS").gameObject;
            var fieldRoot=Rect(motion,"Parameter Fields",0,0,514,528);
            var fieldStack=fieldRoot.gameObject.AddComponent<VerticalLayoutGroup>();fieldStack.spacing=12;fieldStack.childControlWidth=fieldStack.childControlHeight=true;fieldStack.childForceExpandHeight=fieldStack.childForceExpandWidth=false;
            string[] keys={"angle","speed","height","mass","gravity"};
            foreach(var key in keys)
            {
                var slider=Ref<Slider>(panel,key);var value=Ref<TMP_Text>(panel,key+"Value");
                var label=motion.Cast<Transform>().Select(t=>t.GetComponent<TMP_Text>()).Where(t=>t!=null&&t!=value).OrderBy(t=>Mathf.Abs(t.rectTransform.anchoredPosition.y-value.rectTransform.anchoredPosition.y)).First();
                var field=Rect(fieldRoot,key+" Field",0,0,514,96);Size(field.gameObject,514,96);
                var caption=Flow(field,"Label and value",0,0,514,32,true,16,label.gameObject,value.gameObject);
                Size(label.gameObject,348,32);Size(value.gameObject,150,32);
                Flow(field,"Field content",0,0,514,96,false,0,caption.gameObject,slider.gameObject);
            }
            var pause=Ref<Button>(panel,"pause");var toggle=Ref<Toggle>(panel,"showVectors");var fire=Ref<Button>(panel,"fire");var reset=Ref<Button>(panel,"reset");
            var options=Flow(motion,"Playback and visibility",0,0,514,52,true,24,pause.gameObject,toggle.gameObject);
            Size(pause.gameObject,245,52);Size(toggle.gameObject,245,52);
            var actions=Flow(motion,"Launch Actions",0,0,514,72,true,24,fire.gameObject,reset.gameObject);
            Size(fire.gameObject,245,72);Size(reset.gameObject,245,72);
            var hint=motion.Cast<Transform>().First(t=>t.name.StartsWith("Arraste"));var metrics=Ref<TMP_Text>(panel,"metrics");
            var stack=Flow(motion,"Parameters Content",58,40,514,934,false,16,title,fieldRoot.gameObject,options.gameObject,actions.gameObject,hint.gameObject,metrics.gameObject);
            Size(hint.gameObject,514,40);StretchHorizontal(stack);
            var analysis=root.Find("Experiment - Projectile Analysis");
            var formulas=analysis.Cast<Transform>().Where(t=>t.name.StartsWith("Formula ")).Select(t=>t.gameObject).ToArray();
            Flow(analysis,"Formula Stack",48,548,620,336,false,24,formulas);
            var legends=analysis.Cast<Transform>().Where(t=>t.GetComponent<Image>()!=null&&t.name!="Surface").ToArray();
            foreach(var icon in legends)
            {
                var rt=(RectTransform)icon;var label=analysis.Cast<Transform>().Select(t=>t.GetComponent<TMP_Text>()).Where(t=>t!=null).OrderBy(t=>Mathf.Abs(t.rectTransform.anchoredPosition.y-rt.anchoredPosition.y)).FirstOrDefault();
                if(label!=null&&Mathf.Abs(label.rectTransform.anchoredPosition.y-rt.anchoredPosition.y)<10)
                    Flow(analysis,icon.name+" Legend",48,-rt.anchoredPosition.y,620,32,true,8,icon.gameObject,label.gameObject);
            }
            UIVisibility.Set(motion.gameObject,true);UIVisibility.Set(analysis.gameObject,false);
            FinalizeUI(root.gameObject);
        }
        public static string ApplyAll()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Exit Play Mode first.");
            const string prefabPath="Assets/_Project/Hub/HubUI.prefab";
            var prefab=PrefabUtility.LoadPrefabContents(prefabPath);
            try { Menu(prefab);PrefabUtility.SaveAsPrefabAsset(prefab,prefabPath); }
            finally { PrefabUtility.UnloadPrefabContents(prefab); }
            var original=SceneManager.GetActiveScene();int updated=0;
            foreach(var path in new[]{"Assets/_Project/Hub/Scenes/Hub.unity",LanExperimentSession.ExperimentPath})
            {
                var scene=SceneManager.GetSceneByPath(path);bool opened=!scene.isLoaded;
                if(opened)scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Additive);
                var canvases=scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<Canvas>(true)).ToArray();
                var poses=canvases.Select(c=>(c.transform.position,c.transform.rotation,c.transform.lossyScale)).ToArray();
                foreach(var root in scene.GetRootGameObjects())
                {
                    foreach(var ui in root.GetComponentsInChildren<HubEntryController>(true)){Menu(ui.menu.gameObject);Entry(ui);}
                    foreach(var panel in root.GetComponentsInChildren<ObliqueLaunchPanel>(true))Laboratory(panel);
                }
                for(int i=0;i<canvases.Length;i++)if(poses[i]!=(canvases[i].transform.position,canvases[i].transform.rotation,canvases[i].transform.lossyScale))throw new InvalidOperationException("Canvas pose changed; review before saving.");
                EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);updated++;
                if(opened)EditorSceneManager.CloseScene(scene,true);
            }
            SceneManager.SetActiveScene(original);return "Migrated "+updated+" scenes and HubUI prefab. Canvas world poses preserved.";
        }
    }
}
