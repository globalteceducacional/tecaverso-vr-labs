using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Content.Interaction;
using Tecaverso.Labs.Probability;
using Tecaverso.Labs.PlanarGeometry;
using Object=UnityEngine.Object;
namespace Tecaverso.Editor
{
    public static class ProbabilityAuthoring
    {
        public const string Folder="Assets/_Project/Laboratories/Mathematics/Probability";
        public const string ScenePath=Folder+"/Scenes/Probability.unity";
        static TMP_FontAsset regular,bold;static Button buttonTemplate;static Toggle toggleTemplate;
        static readonly Color Blue=new Color32(40,78,160,255),Ink=new Color32(22,22,22,255),Lavender=new Color32(232,230,254,255);
        public static void Create()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode||Enumerable.Range(0,SceneManager.sceneCount).Any(i=>SceneManager.GetSceneAt(i).isDirty))throw new Exception("Preserve as cenas não salvas antes de instalar.");
            if(AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath))throw new Exception("Cena já existe; editar pontualmente.");
            Ensure(Folder+"/Scenes");Ensure(Folder+"/Materials");
            var source=EditorSceneManager.OpenScene("Assets/_Project/Laboratories/Mathematics/PlanarGeometry/Scenes/PlanarGeometry.unity",OpenSceneMode.Additive);
            var gallery=source.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<PlanarGallery>()).Single();buttonTemplate=gallery.backButton;toggleTemplate=gallery.verticesToggle;
            regular=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Regular SDF.asset");bold=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Bold SDF.asset");
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);SceneManager.SetActiveScene(scene);
            foreach(string name in new[]{"XR Origin (XR Rig)","XR Interaction Manager","EventSystem","Directional Light","TecaversoStandardRoom","Shared learning rendering"}){var copy=Object.Instantiate(source.GetRootGameObjects().Single(r=>r.name==name));copy.name=name;SceneManager.MoveGameObjectToScene(copy,scene);}
            var root=new GameObject("Probability laboratory");root.SetActive(false);var lab=root.AddComponent<ProbabilityLab>();lab.stations=new ProbabilityStation[3];
            var blue=Mat("Probability blue",Blue);var orange=Mat("Probability orange",new Color(1,.55f,.15f));var dark=Mat("Instrument graphite",new Color(.06f,.09f,.15f));var white=Mat("Instrument silver",new Color(.7f,.78f,.85f));
            for(int k=0;k<3;k++)
            {
                string title=new[]{"MOEDA","DADO","URNA"}[k];var station=Child(root.transform,title).gameObject.AddComponent<ProbabilityStation>();lab.stations[k]=station;station.lab=lab;station.kind=(Experiment)k;
                station.transform.localPosition=k==0?new Vector3(-4.8f,0,0):k==1?new Vector3(0,0,4.8f):new Vector3(4.8f,0,0);station.transform.localRotation=Quaternion.Euler(0,k==0?-90:k==1?0:90,0);
                Label(station.transform,"Title",title,new Vector3(0,4.65f,0),new Vector2(1700,150),64,true,true);
                var chart=Child(station.transform,"Frequency chart");chart.localPosition=new Vector3(0,2.35f,0);Box(chart,"Backing",new Vector3(0,.8f,.15f),new Vector3(3.5f,2.7f,.08f),dark);
                int n=k==1?6:2;station.bars=new Transform[n];station.theoryMarks=new Transform[n];station.categories=new TMP_Text[n];
                for(int i=0;i<n;i++)
                {
                    float x=(i-(n-1)/2f)*.48f;var bar=GameObject.CreatePrimitive(PrimitiveType.Cube);bar.name="Frequency "+i;bar.transform.SetParent(chart,false);bar.transform.localPosition=new Vector3(x,.3f,0);bar.GetComponent<Renderer>().sharedMaterial=blue;Object.DestroyImmediate(bar.GetComponent<Collider>());station.bars[i]=bar.transform;
                    var mark=GameObject.CreatePrimitive(PrimitiveType.Cube);mark.name="Theory "+i;mark.transform.SetParent(chart,false);mark.transform.localPosition=new Vector3(x,.3f,-.06f);mark.transform.localScale=new Vector3(.38f,.025f,.02f);mark.GetComponent<Renderer>().sharedMaterial=orange;Object.DestroyImmediate(mark.GetComponent<Collider>());station.theoryMarks[i]=mark.transform;
                    station.categories[i]=Label(chart,"Category "+i,"",new Vector3(x,-.08f,-.08f),new Vector2(225,290),27,false,true);
                }
                for(int y=0;y<=4;y++){Box(chart,"Scale "+y,new Vector3(0,.3f+y*.35f,.05f),new Vector3(3,.008f,.01f),white);Label(chart,"Percent "+y,(y*25)+"%",new Vector3(-1.6f,.3f+y*.35f,-.03f),new Vector2(180,70),26,false,true);}
                station.summary=Label(station.transform,"Statistics","",new Vector3(2.45f,2.2f,-.1f),new Vector2(700,800),34,false,true);
                var control=Panel(station.transform,"Controls",new Vector3(-2.5f,2.1f,-.2f),new Vector2(750,k==2?1850:1400),true);station.controls=control.gameObject;
                var layout=control.gameObject.AddComponent<VerticalLayoutGroup>();layout.padding=new RectOffset(18,18,18,18);layout.spacing=12;layout.childControlWidth=layout.childControlHeight=true;layout.childForceExpandHeight=false;
                RowText(control,"Sorteador computacional. A força da mão não altera as chances.",28,false,120);
                station.once=Button(control,"Sortear uma vez",80);var row=Row(control,80);station.batches=new[]{Button(row,"10"),Button(row,"100"),Button(row,"1000")};station.run=Button(control,"Executar lote",80);
                var actions=Row(control,80);station.pause=Button(actions,"Pausar");station.stop=Button(actions,"Encerrar");station.status=RowText(control,"",29,false,140);
                station.theoretical=Toggle(control,"Exibir probabilidades",true);RowText(control,"Frequência = ocorrências / tentativas\nLotes: animações representativas.",27,false,120);
                if(k==2){station.composition=RowText(control,"",28,false,170);var ab=Row(control,80);station.config=new[]{Button(ab,"A −"),Button(ab,"A +"),Button(ab,"B −"),Button(ab,"B +")};station.replacement=Toggle(control,"Com reposição",true);}
                station.reset=Button(control,k==2?"Nova série / repor urna":"Nova série",80);
                Box(station.transform,"Instrument base",new Vector3(0,.65f,-1.1f),new Vector3(1.5f,1.1f,.75f),dark);
                station.lever=Child(station.transform,"Unity lever").gameObject.AddComponent<XRLever>();station.lever.transform.localPosition=new Vector3(-.5f,1.25f,-1.2f);Unity3DControlVisuals.Lever(station.lever,1.4f);station.lever.value=false;
                var display=GameObject.CreatePrimitive(k==0?PrimitiveType.Cylinder:k==1?PrimitiveType.Cube:PrimitiveType.Sphere);display.name="Result specimen";display.transform.SetParent(station.transform,false);display.transform.localPosition=new Vector3(.3f,1.5f,-1.1f);display.transform.localScale=k==0?new Vector3(.4f,.035f,.4f):Vector3.one*.3f;display.GetComponent<Renderer>().sharedMaterial=k==0?orange:white;station.display=display.transform;
                station.result=Label(station.transform,"Last result","Último resultado: —",new Vector3(.3f,1.9f,-1.2f),new Vector2(850,90),32,true,true);
                if(k==0){Label(display.transform,"Cara","CARA",Vector3.up*1.02f,new Vector2(450,150),54,true,true).transform.parent.localRotation=Quaternion.Euler(90,0,0);Label(display.transform,"Coroa","COROA",Vector3.down*1.02f,new Vector2(450,150),54,true,true).transform.parent.localRotation=Quaternion.Euler(-90,0,0);station.resultRotations=new[]{Quaternion.Euler(-90,0,0),Quaternion.Euler(90,0,0)};}
                else if(k==1){var directions=new[]{Vector3.back,Vector3.right,Vector3.forward,Vector3.left,Vector3.up,Vector3.down};station.resultRotations=new Quaternion[6];for(int i=0;i<6;i++){var face=Label(display.transform,"Face "+(i+1),(i+1).ToString(),directions[i]*.51f,new Vector2(400,400),180,true,true).transform.parent;face.localRotation=Quaternion.LookRotation(-directions[i]);station.resultRotations[i]=Quaternion.FromToRotation(directions[i],Vector3.back);}}
                else
                {
                    station.resultRotations=new[]{Quaternion.identity,Quaternion.identity};var rb=display.AddComponent<Rigidbody>();rb.isKinematic=true;rb.useGravity=false;display.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>().throwOnDetach=false;
                    station.population=new Transform[20];for(int i=0;i<20;i++){var ball=GameObject.CreatePrimitive(PrimitiveType.Sphere);ball.name=i<10?"Categoria A":"Categoria B";ball.transform.SetParent(station.transform,false);ball.transform.localPosition=new Vector3(-.65f+(i%5)*.27f,.35f+(i/5)*.22f,.35f);ball.transform.localScale=Vector3.one*.14f;ball.GetComponent<Renderer>().sharedMaterial=i<10?blue:orange;Object.DestroyImmediate(ball.GetComponent<Collider>());station.population[i]=ball.transform;}
                    Box(station.transform,"Opaque draw chamber",new Vector3(.9f,1.2f,-.8f),new Vector3(.4f,.65f,.5f),blue);Label(station.transform,"Population legend","Composição da urna · A azul / B laranja",new Vector3(0,.1f,-.1f),new Vector2(1600,90),28,true,true);
                }
            }
            var nav=Panel(root.transform,"Navigation",new Vector3(0,1.5f,-4),new Vector2(900,220),true);var navLayout=nav.gameObject.AddComponent<VerticalLayoutGroup>();navLayout.childControlWidth=navLayout.childControlHeight=true;navLayout.childForceExpandWidth=true;navLayout.childForceExpandHeight=false;navLayout.padding=new RectOffset(24,24,24,24);lab.back=Button(nav,"Voltar ao Hub",100);
            var modal=Panel(root.transform,"Confirmation",new Vector3(0,2,0),new Vector2(1100,700),true);lab.modal=modal.gameObject;var ml=modal.gameObject.AddComponent<VerticalLayoutGroup>();ml.padding=new RectOffset(32,32,32,32);ml.spacing=24;ml.childControlWidth=ml.childControlHeight=true;ml.childForceExpandWidth=true;ml.childForceExpandHeight=false;lab.message=RowText(modal,"",42,true,280);lab.confirm=Button(modal,"Confirmar",100);lab.cancel=Button(modal,"Cancelar",100);Tecaverso.UI.UIVisibility.Set(lab.modal,false);
            root.SetActive(true);UnityEngine.Canvas.ForceUpdateCanvases();
            var catalog=AssetDatabase.LoadAssetAtPath<Tecaverso.Hub.HubCatalog>("Assets/_Project/Hub/HubCatalog.asset");var math=catalog.disciplines.Single(d=>d.title=="Matemática");var entry=math.contents.Single(c=>c.title=="Medidas");entry.title="Probabilidade e estatística";entry.description="Experimente moeda, dado e urna. Compare probabilidades e frequências.";entry.objective="Investigar aleatoriedade, amostras e medidas estatísticas.";entry.scenePath=ScenePath;EditorUtility.SetDirty(catalog);
            EditorSceneManager.SaveScene(scene,ScenePath);EditorBuildSettings.scenes=EditorBuildSettings.scenes.Concat(new[]{new EditorBuildSettingsScene(ScenePath,true)}).ToArray();AssetDatabase.SaveAssets();
            for(int i=SceneManager.sceneCount-1;i>=0;i--){var other=SceneManager.GetSceneAt(i);if(other!=scene)EditorSceneManager.CloseScene(other,true);}
        }

        public static void FinishInstruments()
        {
            var lab=Object.FindFirstObjectByType<ProbabilityLab>();
            if(EditorApplication.isPlaying||lab.transform.Find("Instrument finish installed"))throw new Exception("Use somente uma vez, fora de Play Mode.");
            regular=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Regular SDF.asset");bold=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Bold SDF.asset");Ensure(Folder+"/Meshes");
            var glass=new Material(Shader.Find("Universal Render Pipeline/Lit")){name="Instrument glass",renderQueue=3000};glass.SetColor("_BaseColor",new Color(.35f,.72f,1,.12f));glass.SetFloat("_Surface",1);glass.SetFloat("_SrcBlend",5);glass.SetFloat("_DstBlend",10);glass.SetFloat("_ZWrite",0);glass.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");AssetDatabase.CreateAsset(glass,Folder+"/Materials/Instrument glass.mat");
            var silver=AssetDatabase.LoadAssetAtPath<Material>(Folder+"/Materials/Instrument silver.mat");var blue=AssetDatabase.LoadAssetAtPath<Material>(Folder+"/Materials/Probability blue.mat");var orange=AssetDatabase.LoadAssetAtPath<Material>(Folder+"/Materials/Probability orange.mat");
            foreach(var s in lab.stations)
            {
                Label(s.transform,"Chart legend","Azul: frequência observada · Laranja: probabilidade teórica",new Vector3(0,4.25f,-.1f),new Vector2(1700,100),27,false,true);
                if(s.kind==Experiment.Dice){var profile=Enumerable.Range(0,17).Select(i=>new Vector2(.57f*Mathf.Cos(i*Mathf.PI/32),.57f*Mathf.Sin(i*Mathf.PI/32))).ToArray();Lathe(s.transform,"Dice dome",new Vector3(.3f,1.27f,-1.1f),profile,glass);}
                if(s.kind==Experiment.Coin)Lathe(s.transform,"Coin launcher",new Vector3(.3f,1.13f,-1.1f),new[]{new Vector2(.28f,0),new Vector2(.28f,.06f),new Vector2(.12f,.10f),new Vector2(.12f,.25f),new Vector2(.24f,.28f)},silver);
                if(s.kind==Experiment.Urn)
                {
                    Lathe(s.transform,"Urn vessel",new Vector3(0,.18f,.35f),new[]{new Vector2(.7f,0),new Vector2(.85f,.12f),new Vector2(.85f,1),new Vector2(.65f,1.2f)},glass);
                    Lathe(s.transform,"Urn rim",new Vector3(0,1.35f,.35f),new[]{new Vector2(.64f,0),new Vector2(.71f,.02f),new Vector2(.71f,.08f),new Vector2(.64f,.1f)},silver);
                    s.categoryMaterials=new[]{blue,orange};s.specimenLabel=Label(s.display,"Category symbol","—",new Vector3(0,0,-.52f),new Vector2(300,220),100,true,true);
                    for(int i=0;i<s.population.Length;i++)Label(s.population[i],"Symbol",i<10?"A":"B",new Vector3(0,0,-.53f),new Vector2(350,250),140,true,true);
                }
            }
            var urn=lab.stations[2];var offset=new Vector3(1.5f,.9f,-1.45f);foreach(var ball in urn.population)ball.localPosition+=offset;urn.transform.Find("Urn vessel").localPosition+=offset;urn.transform.Find("Urn rim").localPosition+=offset;Child(lab.transform,"Instrument finish installed");EditorSceneManager.MarkSceneDirty(lab.gameObject.scene);EditorSceneManager.SaveScene(lab.gameObject.scene);AssetDatabase.SaveAssets();
        }
        static void Lathe(Transform parent,string name,Vector3 position,Vector2[] profile,Material material)
        {
            const int sides=48;var vertices=new System.Collections.Generic.List<Vector3>();var triangles=new System.Collections.Generic.List<int>();
            for(int j=0;j<profile.Length;j++)for(int i=0;i<sides;i++){float a=i*Mathf.PI*2/sides;vertices.Add(new Vector3(profile[j].x*Mathf.Cos(a),profile[j].y,profile[j].x*Mathf.Sin(a)));}
            for(int j=0;j<profile.Length-1;j++)for(int i=0;i<sides;i++){int a=j*sides+i,b=j*sides+(i+1)%sides,c=a+sides,d=b+sides;triangles.AddRange(new[]{a,c,b,b,c,d});}
            var mesh=new Mesh{name=name};mesh.SetVertices(vertices);mesh.SetTriangles(triangles,0);mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,Folder+"/Meshes/"+name+".asset");var t=Child(parent,name);t.localPosition=position;t.gameObject.AddComponent<MeshFilter>().sharedMesh=mesh;t.gameObject.AddComponent<MeshRenderer>().sharedMaterial=material;
        }
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

