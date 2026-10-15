using System;
using System.Linq;
using Tecaverso.Hub;
using Tecaverso.Labs.InclinedPlane;
using Tecaverso.Labs.ObliqueLaunch;
using Tecaverso.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Object=UnityEngine.Object;

namespace Tecaverso.Editor
{
    /// <summary>Creates a new lab once. Never rebuilds or saves the source experiment.</summary>
    public static class InclinedPlaneAuthoring
    {
        public const string Folder="Assets/_Project/Laboratories/Physics/InclinedPlane";
        public const string ScenePath=Folder+"/Scenes/InclinedPlane.unity";
        const string SourcePath=LanExperimentSession.ExperimentPath;
        static readonly Color Blue=new Color32(40,78,160,255), Navy=new Color32(7,56,111,255), Ink=new Color32(22,22,22,255), Surface=new Color32(211,209,232,255);
        static TMP_FontAsset regular,bold;
        static Slider sliderTemplate;
        static Button buttonTemplate;
        static Toggle toggleTemplate;
        static T Ref<T>(Object source,string name) where T:Object => (T)new SerializedObject(source).FindProperty(name).objectReferenceValue;
        static void Assign(Object source,string name,Object value)
        {var so=new SerializedObject(source);so.FindProperty(name).objectReferenceValue=value;so.ApplyModifiedPropertiesWithoutUndo();}
        static void FolderAt(string path)
        {if(AssetDatabase.IsValidFolder(path))return;int split=path.LastIndexOf('/');FolderAt(path.Substring(0,split));AssetDatabase.CreateFolder(path.Substring(0,split),path.Substring(split+1));}

        public static string Create()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Exit Play Mode first.");
            if(AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath)!=null)throw new InvalidOperationException("InclinedPlane already exists. Edit its objects; do not rebuild.");
            var openHub=SceneManager.GetSceneByPath("Assets/_Project/Hub/Scenes/Hub.unity");
            if(openHub.isLoaded&&openHub.isDirty)throw new InvalidOperationException("Save the authored Hub changes before installing a new catalog entry.");
            FolderAt(Folder+"/Scenes");FolderAt(Folder+"/Prefabs");FolderAt(Folder+"/Materials");
            var source=SceneManager.GetSceneByPath(SourcePath);
            if(!source.isLoaded)source=EditorSceneManager.OpenScene(SourcePath,OpenSceneMode.Additive);
            var sourceRoots=source.GetRootGameObjects();
            var sourcePanel=sourceRoots.SelectMany(r=>r.GetComponentsInChildren<ObliqueLaunchPanel>(true)).Single();
            sliderTemplate=Ref<Slider>(sourcePanel,"angle");buttonTemplate=Ref<Button>(sourcePanel,"fire");toggleTemplate=Ref<Toggle>(sourcePanel,"showVectors");
            regular=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Regular SDF.asset");
            bold=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Bold SDF.asset");
            var sourceArrow=sourceRoots.SelectMany(r=>r.GetComponentsInChildren<VectorArrowView>(true)).First();
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);
            SceneManager.SetActiveScene(scene);
            foreach(string name in new[]{"XR Origin (XR Rig)","XR Interaction Manager","EventSystem","Directional Light"})
            {
                var original=sourceRoots.Single(r=>r.name==name);
                var copy=Object.Instantiate(original);copy.name=original.name;
            }
            var room=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Environments/StandardRoom/TecaversoStandardRoom.prefab"),scene);
            room.transform.localScale=new Vector3(1.5f,1.6f,1);
            PrefabUtility.RecordPrefabInstancePropertyModifications(room.transform);
            var root=new GameObject("Inclined Plane Lab");
            var graphite=EnvironmentMaterial("Structural Graphite");var pearl=EnvironmentMaterial("Pearl Panels");var blue=EnvironmentMaterial("Blue Wall Base");
            float length=InclinedPlaneModel.RampLength;
            var pivot=new GameObject("Ramp Pivot").transform;pivot.SetParent(root.transform,false);pivot.position=new Vector3(7.5f,1.25f,0);
            Cube("Ramp surface",pivot,new Vector3(-length*.5f,-.1f,0),new Vector3(length,.2f,1.4f),pearl);
            Cube("Front blue rail",pivot,new Vector3(-length*.5f,-.16f,-.72f),new Vector3(length,.16f,.08f),blue);
            Cube("Back blue rail",pivot,new Vector3(-length*.5f,-.16f,.72f),new Vector3(length,.16f,.08f),blue);
            for(int i=0;i<=length*2;i++)Cube("Ruler tick "+i,pivot,new Vector3(-.5f*i,.012f,-.59f),new Vector3(.022f,.015f,i%2==0?.24f:.12f),graphite,false);
            var support=Cube("Adjustable support",root.transform,Vector3.zero,Vector3.one,graphite).transform;
            Cube("Hinge support",root.transform,new Vector3(7.5f,.6f,0),new Vector3(.28f,1.2f,.8f),graphite);
            Cube("Apparatus base",root.transform,new Vector3(7.5f-length*.5f,.08f,0),new Vector3(length+1,.16f,2),blue);
            var block=new GameObject("Block").transform;block.SetParent(root.transform,false);
            PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Folder+"/Prefabs/WoodenCargoCrate.prefab"),block);
            // Reuse the existing cylinder/cone geometry, saved as a prefab with explicit physical scaling.
            var arrowTemplate=Object.Instantiate(sourceArrow.gameObject);arrowTemplate.name="Force Arrow";
            arrowTemplate.transform.SetPositionAndRotation(Vector3.zero,Quaternion.identity);arrowTemplate.transform.localScale=Vector3.one;
            foreach(var collider in arrowTemplate.GetComponentsInChildren<Collider>(true))Object.DestroyImmediate(collider);
            arrowTemplate.SetActive(true);
            // Scene-local meshes do not survive saving a separate prefab. Use a persistent asset.
            arrowTemplate.transform.Find("Head").GetComponent<MeshFilter>().sharedMesh=AssetDatabase.LoadAssetAtPath<Mesh>(Folder+"/Meshes/ForceArrowCone.asset");
            var arrowPrefab=PrefabUtility.SaveAsPrefabAsset(arrowTemplate,Folder+"/Prefabs/ForceArrow.prefab");Object.DestroyImmediate(arrowTemplate);
            var forcesRoot=new GameObject("Forces on block");forcesRoot.transform.SetParent(root.transform,false);
            var arrows=new VectorArrowView[6];var labels=new TMP_Text[6];
            string[] names={"Peso","Normal","Atrito","Resultante","Peso paralelo","Peso perpendicular"};
            Color[] colors={new Color32(255,208,70,255),new Color32(62,211,156,255),new Color32(240,88,100,255),new Color32(90,160,255,255),new Color32(255,225,150,255),new Color32(255,225,150,255)};
            for(int i=0;i<arrows.Length;i++)
            {
                var arrow=(GameObject)PrefabUtility.InstantiatePrefab(arrowPrefab,forcesRoot.transform);arrow.name=names[i];arrows[i]=arrow.GetComponent<VectorArrowView>();
                var material=new Material(sourceArrow.GetComponentInChildren<Renderer>(true).sharedMaterial);material.name=names[i];material.SetColor("_BaseColor",colors[i]);
                AssetDatabase.CreateAsset(material,Folder+"/Materials/Force"+i+".mat");
                foreach(var renderer in arrow.GetComponentsInChildren<Renderer>(true))
                { renderer.sharedMaterial=material;PrefabUtility.RecordPrefabInstancePropertyModifications(renderer); }
                var text=new GameObject(names[i]+" value",typeof(TextMeshPro)).GetComponent<TextMeshPro>();
                text.transform.SetParent(forcesRoot.transform,false);text.font=bold;text.fontSize=1.3f;text.color=colors[i];
                text.rectTransform.sizeDelta=new Vector2(2.3f,.3f);text.alignment=TextAlignmentOptions.Center;text.raycastTarget=false;labels[i]=text;
            }
            var forces=forcesRoot.AddComponent<InclinedPlaneForcesView>();forces.Bind(arrows,labels);
            var panel=CreatePanel(root.transform,sourcePanel.transform);
            var lab=root.AddComponent<InclinedPlaneLab>();lab.Bind(panel,forces,pivot,block,support);
            lab.Model.Configure(panel.Parameters);lab.Present();
            var canvas=panel.GetComponent<Canvas>();Canvas.ForceUpdateCanvases();
            foreach(var layout in canvas.GetComponentsInChildren<LayoutGroup>(true).Reverse())LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)layout.transform);
            EditorSceneManager.SaveScene(scene,ScenePath);
            var scenes=EditorBuildSettings.scenes.ToList();if(!scenes.Any(s=>s.path==ScenePath)){scenes.Add(new EditorBuildSettingsScene(ScenePath,true));EditorBuildSettings.scenes=scenes.ToArray();}
            InstallCatalogAndPagination();AssetDatabase.SaveAssets();
            Selection.activeGameObject=root;
            return "Created InclinedPlane scene, force arrow prefab and Hub entry. Source experiment untouched.";
        }
        static Material EnvironmentMaterial(string name)=>AssetDatabase.LoadAssetAtPath<Material>("Assets/_Project/Laboratories/Physics/ObliqueLaunch/Materials/Environment/"+name+".mat");
        static GameObject Cube(string name,Transform parent,Vector3 position,Vector3 scale,Material material,bool collision=true)
        {
            var go=GameObject.CreatePrimitive(PrimitiveType.Cube);go.name=name;go.transform.SetParent(parent,false);go.transform.localPosition=position;go.transform.localScale=scale;
            go.GetComponent<Renderer>().sharedMaterial=material;if(!collision)Object.DestroyImmediate(go.GetComponent<Collider>());return go;
        }
        static RectTransform Rect(Transform parent,string name,float x,float y,float w,float h)=>ObliqueLaunchFigmaUIBuilder.Rect(parent,name,x,y,w,h);
        static void Stretch(RectTransform r){r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;}
        static void Size(GameObject go,float w,float h)
        {var e=go.GetComponent<LayoutElement>()??go.AddComponent<LayoutElement>();e.minWidth=0;e.preferredWidth=w;e.minHeight=e.preferredHeight=h;e.flexibleWidth=e.flexibleHeight=0;}
        static RectTransform Flow(Transform parent,string name,float x,float y,float w,float h,bool horizontal,float gap)
        {
            var r=Rect(parent,name,x,y,w,h);HorizontalOrVerticalLayoutGroup g=horizontal?r.gameObject.AddComponent<HorizontalLayoutGroup>():r.gameObject.AddComponent<VerticalLayoutGroup>();
            g.spacing=gap;g.childControlHeight=g.childControlWidth=true;g.childForceExpandHeight=g.childForceExpandWidth=false;g.childAlignment=TextAnchor.UpperLeft;Size(r.gameObject,w,h);return r;
        }
        static TMP_Text Text(Transform parent,string value,float w,float h,int size=20,Color? color=null,bool strong=false)
        {
            var t=ObliqueLaunchFigmaUIBuilder.Text(parent,value,0,0,w,h,size,strong?bold:regular,color??Ink);
            t.textWrappingMode=TextWrappingModes.Normal;t.overflowMode=TextOverflowModes.Truncate;Size(t.gameObject,w,h);return t;
        }
        static RectTransform Card(Transform parent,string name,float x,float y,float w,float h)
        {
            var image=ObliqueLaunchFigmaUIBuilder.Image(parent,name,x,y,w,h,Navy,true);
            var inner=ObliqueLaunchFigmaUIBuilder.Image(image.transform,"Surface",2,2,w-4,h-4,Surface);
            inner.raycastTarget=false;return image.rectTransform;
        }
        static Button Button(Transform parent,string label,float width=300,float height=64)
        {
            var b=Object.Instantiate(buttonTemplate,parent);b.name=label;b.onClick=new Button.ButtonClickedEvent();b.interactable=true;
            b.transform.localScale=Vector3.one;b.transform.localRotation=Quaternion.identity;var r=(RectTransform)b.transform;r.sizeDelta=new Vector2(width,height);r.anchoredPosition=Vector2.zero;
            var t=b.GetComponentInChildren<TMP_Text>(true);t.text=label;t.fontSize=20;Stretch(t.rectTransform);Size(b.gameObject,width,height);
            foreach(var g in b.GetComponents<CanvasGroup>()){g.alpha=1;g.interactable=g.blocksRaycasts=true;}
            return b;
        }
        static Toggle Toggle(Transform parent,string label,bool initial,float width)
        {
            var t=Object.Instantiate(toggleTemplate,parent);t.name=label;t.onValueChanged=new Toggle.ToggleEvent();t.SetIsOnWithoutNotify(initial);
            t.transform.localScale=Vector3.one;((RectTransform)t.transform).sizeDelta=new Vector2(width,48);
            var text=t.GetComponentInChildren<TMP_Text>(true);text.text=label;text.fontSize=20;text.rectTransform.sizeDelta=new Vector2(width-48,40);
            Size(t.gameObject,width,48);return t;
        }
        static InclinedPlanePanel CreatePanel(Transform parent,Transform original)
        {
            var go=new GameObject("Inclined Plane Controls",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster),typeof(TrackedDeviceGraphicRaycaster));
            go.transform.SetParent(parent,false);go.transform.SetPositionAndRotation(original.position,original.rotation);go.transform.localScale=original.lossyScale;
            ((RectTransform)go.transform).sizeDelta=new Vector2(720,1480);go.GetComponent<Canvas>().renderMode=RenderMode.WorldSpace;
            var root=Rect(go.transform,"Inclined Plane UI",0,0,720,1480);root.anchorMin=root.anchorMax=root.pivot=new Vector2(.5f,.5f);root.anchoredPosition=Vector2.zero;root.localScale=Vector3.one*.64f;
            var pages=Rect(root,"Pages",0,0,720,1480);Stretch(pages);
            var tabs=Flow(pages,"Tabs",0,0,720,64,true,16);
            var parameterTab=Button(tabs,"PARÂMETROS",352);var analysisTab=Button(tabs,"ANÁLISE",352);
            var parameters=Card(pages,"Parameters",0,80,720,1280);var analysisPage=Card(pages,"Analysis",0,80,720,1280);
            var stack=Flow(parameters,"Parameters content",48,32,624,1216,false,16);
            Text(stack,"PLANO INCLINADO",624,36,24,Ink,true);
            Text(stack,"Libere a caixa de carga e observe as forças.\nReinicie para ajustar a rampa.",624,52);
            var presetRow=Flow(stack,"Surface presets",0,0,624,48,true,12);
            var presets=new[]{Button(presetRow,"SEM ATRITO",200,48),Button(presetRow,"BAIXO ATRITO",200,48),Button(presetRow,"ALTO ATRITO",200,48)};
            var fields=Flow(stack,"Parameter fields",0,0,624,528,false,12);
            string[] names={"INCLINAÇÃO","MASSA","ATRITO ESTÁTICO · μs","ATRITO CINÉTICO · μk","GRAVIDADE"};
            float[] min={0,1,0,0,5},max={60,10,1,.5f,20},initial={30,3,.5f,.3f,9.81f};
            var sliders=new Slider[5];var values=new TMP_Text[5];
            for(int i=0;i<5;i++)
            {
                var field=Flow(fields,names[i]+" field",0,0,624,96,false,0);
                var caption=Flow(field,"Label and value",0,0,624,32,true,16);
                Text(caption,names[i],408,32,20,Ink,true);values[i]=Text(caption,"",200,32,22,Blue,true);values[i].alignment=TextAlignmentOptions.MidlineRight;
                var s=Object.Instantiate(sliderTemplate,field);s.name=names[i];s.onValueChanged=new Slider.SliderEvent();s.minValue=min[i];s.maxValue=max[i];s.wholeNumbers=i==0;s.SetValueWithoutNotify(initial[i]);
                s.transform.localScale=Vector3.one;((RectTransform)s.transform).sizeDelta=new Vector2(624,64);Size(s.gameObject,624,64);
                foreach(Transform child in s.transform){var r=child as RectTransform;if(r!=null)r.sizeDelta=new Vector2(624,r.sizeDelta.y);}
                sliders[i]=s;
            }
            var visibility=Flow(stack,"Vector options",0,0,624,104,false,8);
            var row=Flow(visibility,"Force visibility",0,0,624,48,true,16);
            var vectors=Toggle(row,"FORÇAS",true,304);var resultant=Toggle(row,"RESULTANTE",false,304);
            var components=Toggle(visibility,"DECOMPOR PESO · substitui a seta P",false,624);
            var actions=Flow(stack,"Simulation actions",0,0,624,64,true,12);
            var release=Button(actions,"LIBERAR",200);var pause=Button(actions,"PAUSAR",200);var reset=Button(actions,"REINICIAR",200);
            var status=Text(stack,"",624,64,20,Navy,true);
            var metrics=Text(stack,"",624,64,22,Navy);
            var scale=Text(stack,"",624,44,18,Navy);
            var detail=Flow(analysisPage,"Analysis content",48,32,624,1216,false,24);
            Text(detail,"FORÇAS NO BLOCO",624,36,24,Ink,true);
            var readout=Text(detail,"",624,450,22);
            Text(detail,"P = mg     N = mg cos(θ)\nPpar = mg sen(θ)     Pperp = −N\n\nEquilíbrio: Fat = Ppar ≤ μsN\nDescida: Fat = μkN\na = g [sen(θ) − μk cos(θ)]",624,260,24,Blue,true);
            Text(detail,"AMARELO · peso    VERDE · normal\nVERMELHO · atrito    AZUL · resultante\n\nPpar e Pperp decompõem o peso.\nNão são forças adicionais.\n\nModelo: bloco sem rolamento, rampa rígida\ne coeficientes constantes (μk ≤ μs).\nPercurso útil: 9,20 m. Sem réplicas temporais.",624,300,20);
            var back=Button(pages,"VOLTAR AO HUB",720,72);var br=(RectTransform)back.transform;br.anchorMin=br.anchorMax=br.pivot=new Vector2(0,1);br.anchoredPosition=new Vector2(0,-1380);
            var dialog=Card(root,"Leave confirmation",0,0,720,1480);var dialogStack=Flow(dialog,"Confirmation content",48,420,624,400,false,24);
            Text(dialogStack,"Voltar ao Hub?",624,64,32,Ink,true);Text(dialogStack,"A tentativa atual será descartada.",624,96,24);
            var dialogActions=Flow(dialogStack,"Confirmation actions",0,0,624,64,true,24);
            var cancel=Button(dialogActions,"CONTINUAR",300);var confirm=Button(dialogActions,"VOLTAR",300);
            var panel=go.AddComponent<InclinedPlanePanel>();panel.Bind(sliders,values,new[]{release,pause,reset,parameterTab,analysisTab,back,confirm,cancel},presets,new[]{vectors,resultant,components},
                new[]{parameters.gameObject,analysisPage.gameObject,dialog.gameObject,pages.gameObject},new[]{status,metrics,readout,pause.GetComponentInChildren<TMP_Text>(true),scale});
            UIVisibility.Set(parameters.gameObject,true);UIVisibility.Set(analysisPage.gameObject,false);UIVisibility.Set(dialog.gameObject,false);parameterTab.interactable=false;
            return panel;
        }
        static void InstallCatalogAndPagination()
        {
            var catalog=AssetDatabase.LoadAssetAtPath<HubCatalog>("Assets/_Project/Hub/HubCatalog.asset");
            var physics=catalog.disciplines.First(d=>d.title=="Física");
            if(!physics.contents.Any(c=>c.scenePath==ScenePath))
            {
                var contents=physics.contents.ToList();contents.Insert(1,new HubCatalog.Content{title="Plano inclinado",description="Investigue peso, normal e atrito em um bloco sobre uma rampa. Disponível na exploração individual.",objective="Relacionar inclinação, equilíbrio, atrito e aceleração.",icon=AssetDatabase.LoadAssetAtPath<Sprite>(Folder+"/InclinedPlaneIcon.png")??contents[0].icon,scenePath=ScenePath});physics.contents=contents.ToArray();EditorUtility.SetDirty(catalog);
            }
            const string prefabPath="Assets/_Project/Hub/HubUI.prefab";
            var prefab=PrefabUtility.LoadPrefabContents(prefabPath);
            try{Pagination(prefab.GetComponent<HubMenuController>());PrefabUtility.SaveAsPrefabAsset(prefab,prefabPath);}
            finally{PrefabUtility.UnloadPrefabContents(prefab);}
            const string hubPath="Assets/_Project/Hub/Scenes/Hub.unity";
            var hub=SceneManager.GetSceneByPath(hubPath);bool opened=!hub.isLoaded;
            if(opened)hub=EditorSceneManager.OpenScene(hubPath,OpenSceneMode.Additive);
            foreach(var menu in hub.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<HubMenuController>(true)))Pagination(menu);
            EditorSceneManager.SaveScene(hub);if(opened)EditorSceneManager.CloseScene(hub,true);
        }
        static void Pagination(HubMenuController menu)
        {
            var view=Ref<GameObject>(menu,"contentsView");if(view.transform.Find("Content pagination")!=null)return;
            var row=Flow(view.transform,"Content pagination",290,944,1140,64,true,24);
            var previous=Button(row,"ANTERIORES",330);var label=Text(row,"1 / 2",432,64,24,Ink,true);label.alignment=TextAlignmentOptions.Center;
            var next=Button(row,"PRÓXIMOS",330);
            Assign(menu,"previousPage",previous);Assign(menu,"nextPage",next);Assign(menu,"pageLabel",label);Assign(menu,"pagination",row.gameObject);
            UIVisibility.Set(row.gameObject,false);EditorUtility.SetDirty(menu);
        }
    }
}
