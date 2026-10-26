using System;
using System.Collections.Generic;
using System.Linq;
using Tecaverso.Hub;
using Tecaverso.Labs.Thermology;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Object=UnityEngine.Object;

namespace Tecaverso.Editor
{
    public static class ThermologyAuthoring
    {
        public const string Folder="Assets/_Project/Laboratories/Physics/Thermology";
        public const string ScenePath=Folder+"/Scenes/Thermology.unity";
        static TMP_FontAsset regular,bold;
        static Material navy,blue,metal,pearl,glass,heat,particleMat;
        static ThermologyLab lab;
        static readonly Color Blue=new Color32(40,78,160,255),Ink=new Color32(22,22,22,255),Lavender=new Color32(232,230,254,255);
        static void Ensure(string path){if(AssetDatabase.IsValidFolder(path))return;int k=path.LastIndexOf('/');Ensure(path[..k]);AssetDatabase.CreateFolder(path[..k],path[(k+1)..]);}
        static Material Mat(string name,Color color,float smooth=.4f,bool emission=false)
        {
            string path=Folder+"/Materials/"+name+".mat";var existing=AssetDatabase.LoadAssetAtPath<Material>(path);if(existing!=null)return existing;
            var m=new Material(Shader.Find("Universal Render Pipeline/Lit")){name=name};m.SetColor("_BaseColor",color);m.SetFloat("_Smoothness",smooth);
            if(emission){m.EnableKeyword("_EMISSION");m.SetColor("_EmissionColor",color*.4f);}AssetDatabase.CreateAsset(m,path);return m;
        }
        static Transform Node(string name,Transform parent,Vector3 point)
        {var t=new GameObject(name).transform;t.SetParent(parent,false);t.localPosition=point;return t;}
        static Transform Shape(string name,Transform parent,PrimitiveType primitive,Vector3 point,Vector3 scale,Material mat,bool collider=false)
        {
            var go=GameObject.CreatePrimitive(primitive);go.name=name;go.transform.SetParent(parent,false);go.transform.localPosition=point;go.transform.localScale=scale;
            go.GetComponent<Renderer>().sharedMaterial=mat;if(!collider)Object.DestroyImmediate(go.GetComponent<Collider>());return go.transform;
        }
        // Surface of revolution. Ring order and triangle winding are outward; caps are separate hard faces.
        static Transform Lathe(string name,Transform parent,(float y,float r)[] profile,Material mat,bool caps=true,int sides=48)
        {
            var vertices=new List<Vector3>();var faces=new List<Face>();
            for(int row=0;row<profile.Length;row++)for(int k=0;k<sides;k++)
            {float a=2*Mathf.PI*k/sides;vertices.Add(new Vector3(Mathf.Cos(a)*profile[row].r,profile[row].y,Mathf.Sin(a)*profile[row].r));}
            for(int row=0;row<profile.Length-1;row++)for(int k=0;k<sides;k++)
            {int a=row*sides+k,b=row*sides+(k+1)%sides,c=b+sides,d=a+sides;faces.Add(new Face(new[]{a,c,b,a,d,c}){smoothingGroup=1});}
            if(caps)
            {
                int bottom=vertices.Count;vertices.Add(new Vector3(0,profile[0].y,0));int top=vertices.Count;vertices.Add(new Vector3(0,profile[^1].y,0));
                for(int k=0;k<sides;k++){int next=(k+1)%sides;faces.Add(new Face(new[]{bottom,k,next}));faces.Add(new Face(new[]{top,(profile.Length-1)*sides+next,(profile.Length-1)*sides+k}));}
            }
            var pb=ProBuilderMesh.Create(vertices,faces);pb.name=name;pb.transform.SetParent(parent,false);pb.GetComponent<Renderer>().sharedMaterial=mat;pb.ToMesh();pb.Refresh();return pb.transform;
        }
        static TMP_Text WorldText(Transform parent,string value,Vector3 point,Vector2 size,float fontSize=2.3f)
        {
            var t=Node(value,parent,point).gameObject.AddComponent<TextMeshPro>();t.font=bold;t.fontSize=fontSize*.4f;t.color=new Color(.82f,.91f,1);t.text=value;t.alignment=TextAlignmentOptions.Center;
            t.rectTransform.sizeDelta=size;t.raycastTarget=false;t.overflowMode=TextOverflowModes.Overflow;return t;
        }
        static void Apparatus(Transform root)
        {
            var baseRoot=Node("Gas chamber",root,new Vector3(.8f,.85f,1.8f));
            Lathe("Machined base",baseRoot,new[]{(-.85f,.96f),(-.74f,1.02f),(-.19f,.88f),(-.08f,.96f),(0f,.96f)},navy);
            Shape("Base collision",baseRoot,PrimitiveType.Cylinder,new Vector3(0,-.42f,0),new Vector3(1.92f,.42f,1.92f),navy,true).GetComponent<Renderer>().enabled=false;
            Lathe("Thermal jacket",baseRoot,new[]{(-.10f,.98f),(-.04f,.98f),(0f,.89f)},heat);lab.jacket=baseRoot.Find("Thermal jacket").GetComponent<Renderer>();lab.jacket.transform.localPosition=Vector3.up*.008f;
            Lathe("Transparent cylinder",baseRoot,new[]{(.015f,.765f),(1.60f,.765f)},glass,false);
            Lathe("Upper rim",baseRoot,new[]{(1.59f,.81f),(1.65f,.81f),(1.65f,.75f),(1.59f,.75f),(1.59f,.81f)},metal,false);
            foreach(float x in new[]{-.92f,.92f})Shape("Piston guide",baseRoot,PrimitiveType.Cylinder,new Vector3(x,.82f,.28f),new Vector3(.045f,.88f,.045f),metal,true);
            var p=Node("XR piston",baseRoot,new Vector3(0,.875f,0));
            Lathe("Piston head",p,new[]{(-.035f,.72f),(-.015f,.745f),(.035f,.745f),(.06f,.71f)},metal);
            Lathe("Piston seal",p,new[]{(-.025f,.752f),(-.005f,.752f)},blue);
            Shape("Handle arm",p,PrimitiveType.Cube,new Vector3(0,.02f,-.82f),new Vector3(.08f,.07f,.42f),metal);
            var grip=Shape("Blue piston grip",p,PrimitiveType.Capsule,new Vector3(0,.02f,-1.03f),new Vector3(.12f,.24f,.12f),blue);grip.localRotation=Quaternion.Euler(0,0,90);
            var body=p.gameObject.AddComponent<Rigidbody>();body.isKinematic=true;body.useGravity=false;
            var collider=p.gameObject.AddComponent<BoxCollider>();collider.center=new Vector3(0,.02f,-1.03f);collider.size=new Vector3(.55f,.20f,.22f);
            lab.piston=p.gameObject.AddComponent<ThermalPiston>();lab.piston.lab=lab;lab.piston.colliders.Add(collider);lab.piston.trackRotation=false;lab.piston.throwOnDetach=false;lab.piston.movementType=XRBaseInteractable.MovementType.Instantaneous;lab.piston.useDynamicAttach=true;
            WorldText(baseRoot,"PUXADOR · ISOTÉRMICA",new Vector3(0,-.22f,-1.04f),new Vector2(1.8f,.25f),2.0f);
            var socketNode=Node("Calibrated load socket",p,new Vector3(0,.08f,.10f));
            var sc=socketNode.gameObject.AddComponent<SphereCollider>();sc.radius=.25f;sc.isTrigger=true;
            socketNode.gameObject.AddComponent<Rigidbody>().isKinematic=true;
            var socket=socketNode.gameObject.AddComponent<ThermalLoadSocket>();socket.lab=lab;socket.showInteractableHoverMeshes=false;socket.interactionLayers=(InteractionLayerMask)4;
            socket.attachTransform=Node("Load alignment",socketNode,Vector3.zero);lab.loadSocket=socket;
            Lathe("Socket blue outline",socketNode,new[]{(0f,.27f),(.015f,.27f),(.015f,.24f),(0f,.24f),(0f,.27f)},blue,false);
            var latch=Node("Volume lock pivot",baseRoot,new Vector3(.90f,.875f,-.12f));
            Shape("Lock bolt",latch,PrimitiveType.Cube,new Vector3(-.14f,0,0),new Vector3(.35f,.09f,.12f),blue);lab.lockBar=latch;
            var cloud=Node("Representative molecules",baseRoot,Vector3.zero);var system=cloud.gameObject.AddComponent<ParticleSystem>();var main=system.main;main.playOnAwake=false;main.startSpeed=0;main.maxParticles=96;
            var em=system.emission;em.enabled=false;var sr=system.GetComponent<ParticleSystemRenderer>();sr.renderMode=ParticleSystemRenderMode.Mesh;
            var sphere=GameObject.CreatePrimitive(PrimitiveType.Sphere);sr.mesh=sphere.GetComponent<MeshFilter>().sharedMesh;Object.DestroyImmediate(sphere);sr.sharedMaterial=particleMat;sr.shadowCastingMode=ShadowCastingMode.Off;sr.receiveShadows=false;
            lab.particles=cloud.gameObject.AddComponent<GasParticles>();lab.particles.lab=lab;
            for(int v=15;v<=45;v+=5)
            {
                float y=v*.035f;Shape("Volume graduation "+v,baseRoot,PrimitiveType.Cube,new Vector3(.84f,y,-.17f),new Vector3(.12f,.015f,.02f),pearl);
                WorldText(baseRoot,v+" L",new Vector3(1.05f,y,-.19f),new Vector2(.40f,.16f),1.5f);
            }
            lab.volumeLabel=WorldText(baseRoot,"VOLUME",new Vector3(0,1.98f,-.05f),new Vector2(1.8f,.45f),2.3f);
            var gauge=Node("Pressure gauge",root,new Vector3(2.15f,1.6f,1.1f));
            var disc=Lathe("Pressure dial",gauge,new[]{(-.04f,.29f),(.04f,.29f)},pearl);disc.localRotation=Quaternion.Euler(90,0,0);
            var needle=Node("Pressure needle",gauge,new Vector3(0,0,-.065f));Shape("Needle",needle,PrimitiveType.Cube,new Vector3(0,.1f,0),new Vector3(.018f,.21f,.012f),blue);lab.pressureNeedle=needle;
            for(int k=0;k<9;k++){float a=(-135+k*33.75f)*Mathf.Deg2Rad;Shape("Gauge tick",gauge,PrimitiveType.Sphere,new Vector3(Mathf.Sin(a)*.24f,Mathf.Cos(a)*.24f,-.055f),Vector3.one*.015f,navy);}
            lab.pressureLabel=WorldText(gauge,"PRESSÃO",new Vector3(0,-.44f,-.08f),new Vector2(1.15f,.4f),2.0f);
            var thermo=Node("Thermometer",baseRoot,new Vector3(-1.15f,.85f,-.1f));
            Shape("Thermometer case",thermo,PrimitiveType.Capsule,Vector3.zero,new Vector3(.18f,.48f,.16f),pearl);
            lab.temperatureColumn=Shape("Temperature column",thermo,PrimitiveType.Cube,new Vector3(0,-.2f,-.08f),new Vector3(.07f,.3f,.035f),heat);
            WorldText(thermo,"K",new Vector3(0,.52f,-.08f),new Vector2(.2f,.18f),1.7f);
            var dial=Node("XR thermal dial",root,new Vector3(.3f,1.15f,.3f));dial.localRotation=Quaternion.Euler(90,0,0);
            var handle=Lathe("Rotary thermal handle",dial,new[]{(-.05f,.20f),(.05f,.20f),(.09f,.16f)},blue);Shape("Dial indicator",handle,PrimitiveType.Sphere,new Vector3(0,-.065f,.13f),Vector3.one*.045f,pearl);
            var kc=handle.gameObject.AddComponent<SphereCollider>();kc.radius=.21f;
            lab.knob=dial.gameObject.AddComponent<ThermalKnob>();lab.knob.lab=lab;lab.knob.handle=handle;lab.knob.minAngle=-135;lab.knob.maxAngle=135;lab.knob.value=.25f;lab.knob.positionTrackedRadius=.1f;lab.knob.colliders.Add(kc);
            Shape("Thermal console stand",root,PrimitiveType.Cylinder,new Vector3(.3f,.55f,.3f),new Vector3(.17f,.55f,.17f),navy,true);
            lab.temperatureLabel=WorldText(root,"TEMPERATURA",new Vector3(.3f,1.57f,.17f),new Vector2(1.65f,.5f),1.45f);
            WorldText(root,"GIRE PARA AQUECER / RESFRIAR",new Vector3(.3f,.88f,.1f),new Vector2(1.7f,.18f),1.45f);
            lab.heatIn=Arrow(baseRoot,"Heat enters",new Vector3(-.80f,.15f,-.2f),true);
            lab.heatOut=Arrow(baseRoot,"Heat leaves",new Vector3(-.80f,.15f,-.2f),false);
            lab.heatLabel=WorldText(baseRoot,"CALOR",new Vector3(-.1f,-.48f,-.98f),new Vector2(1.8f,.24f),1.8f);
            var weights=new List<ThermalLoad>();
            for(int i=0;i<3;i++)
            {
                var w=Node("Calibrated load "+(90+i*20)+" kPa",root,new Vector3(2.0f+i*.65f,1.0f,-.1f));
                Lathe("Load body",w,new[]{(0f,.20f),(.03f,.23f),(.16f,.23f),(.20f,.17f)},navy);
                Shape("Load grip",w,PrimitiveType.Capsule,new Vector3(0,.25f,0),new Vector3(.10f,.08f,.10f),blue);
                var wc=w.gameObject.AddComponent<BoxCollider>();wc.center=new Vector3(0,.12f,0);wc.size=new Vector3(.46f,.30f,.46f);
                var weightBody=w.gameObject.AddComponent<Rigidbody>();weightBody.useGravity=false;weightBody.isKinematic=true;var weight=w.gameObject.AddComponent<ThermalLoad>();weight.lab=lab;weight.pressureKpa=90+i*20;weight.throwOnDetach=false;weight.interactionLayers=(InteractionLayerMask)5;weight.movementType=XRBaseInteractable.MovementType.Instantaneous;weights.Add(weight);
                WorldText(w,(90+i*20)+" kPa",new Vector3(0,.10f,-.24f),new Vector2(.44f,.18f),1.7f);
            }
            lab.weights=weights.ToArray();Shape("Load tray",root,PrimitiveType.Cube,new Vector3(2.65f,.94f,-.1f),new Vector3(2f,.09f,.65f),metal,true);
            Shape("Tray support",root,PrimitiveType.Cylinder,new Vector3(2.65f,.45f,-.1f),new Vector3(.25f,.45f,.25f),navy,true);
            lab.loadLabel=WorldText(root,"CARGAS CALIBRADAS",new Vector3(2.65f,.68f,-.45f),new Vector2(2.15f,.24f),1.375f);
            WorldText(root,"TERMologia · CÂMARA DE GÁS IDEAL".ToUpperInvariant(),new Vector3(.5f,3.7f,4.4f),new Vector2(8,.65f),5f);
        }
        static GameObject Arrow(Transform parent,string name,Vector3 position,bool inward)
        {
            var root=Node(name,parent,position);root.localRotation=Quaternion.Euler(0,0,inward?-90:90);
            Lathe("Heat arrow",root,new[]{(-.26f,.035f),(.12f,.035f),(.12f,.10f),(.28f,.002f)},heat);return root.gameObject;
        }
        static RectTransform Rect(Transform parent,string name)
        {var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);return r;}
        static void Stretch(RectTransform r){r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;}
        static TMP_Text Label(Transform parent,string text,float size,float height,Color? color=null)
        {
            var r=Rect(parent,text);var le=r.gameObject.AddComponent<LayoutElement>();le.preferredHeight=height;le.minHeight=height;
            var t=r.gameObject.AddComponent<TextMeshProUGUI>();t.font=regular;t.fontSize=size;t.color=color??Ink;t.text=text;t.raycastTarget=false;t.overflowMode=TextOverflowModes.Overflow;return t;
        }
        static RectTransform Row(Transform parent,string name,float height)
        {
            var r=Rect(parent,name);var le=r.gameObject.AddComponent<LayoutElement>();le.preferredHeight=height;le.minHeight=height;
            var layout=r.gameObject.AddComponent<HorizontalLayoutGroup>();layout.spacing=12;layout.childControlWidth=layout.childControlHeight=true;layout.childForceExpandWidth=true;layout.childForceExpandHeight=true;return r;
        }
        static Button Button(Transform parent,string text)
        {
            var r=Rect(parent,text);r.gameObject.AddComponent<LayoutElement>().flexibleWidth=1;
            var image=r.gameObject.AddComponent<Image>();image.color=Blue;var b=r.gameObject.AddComponent<Button>();b.targetGraphic=image;
            var label=Rect(r,"Label");Stretch(label);label.offsetMin=new Vector2(6,2);label.offsetMax=new Vector2(-6,-2);
            var t=label.gameObject.AddComponent<TextMeshProUGUI>();t.font=bold;t.fontSize=22;t.text=text;t.color=Color.white;t.alignment=TextAlignmentOptions.Center;t.raycastTarget=false;t.overflowMode=TextOverflowModes.Ellipsis;return b;
        }
        static RectTransform Panel(Transform parent,string name,Vector3 position,Vector2 size,out GameObject page)
        {
            var canvas=Rect(parent,name);canvas.position=position;canvas.localScale=Vector3.one*.002f;canvas.sizeDelta=size;
            canvas.gameObject.AddComponent<Canvas>().renderMode=RenderMode.WorldSpace;canvas.gameObject.AddComponent<CanvasScaler>();canvas.gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();
            canvas.gameObject.AddComponent<Image>().color=Lavender;
            var content=Rect(canvas,"Page");Stretch(content);content.gameObject.AddComponent<CanvasGroup>();
            var layout=content.gameObject.AddComponent<VerticalLayoutGroup>();layout.padding=new RectOffset(28,28,28,28);layout.spacing=12;layout.childControlWidth=layout.childControlHeight=true;layout.childForceExpandHeight=false;layout.childForceExpandWidth=true;
            page=content.gameObject;return canvas;
        }
        static void Interface(Transform root)
        {
            var canvas=Panel(root,"Thermology controls",new Vector3(-1.65f,1.85f,1.05f),new Vector2(820,1180),out lab.controlsPage);var page=lab.controlsPage.transform;
            Label(page,"TERMOLOGIA",38,60,Blue).font=bold;
            lab.invariant=Label(page,"GRANDEZA CONSTANTE",23,55,Blue);
            var modes=Row(page,"Process selection",70);lab.modes=new[]{Button(modes,"ISOTÉRMICA"),Button(modes,"ISOBÁRICA"),Button(modes,"ISOMÉTRICA")};
            lab.instruction=Label(page,"Instrução",24,105);
            lab.values=Label(page,"Estado do gás",30,120,Blue);
            Label(page,"A pressão no seu próximo movimento…",23,50);
            var prediction=Row(page,"Pressure prediction",60);lab.predictions=new[]{Button(prediction,"DIMINUI"),Button(prediction,"FICA IGUAL"),Button(prediction,"AUMENTA")};
            lab.feedback=Label(page,"Previsão",22,75);
            var actions=Row(page,"Experiment actions",65);lab.startButton=Button(actions,"INICIAR");lab.pauseButton=Button(actions,"PAUSAR");lab.resetButton=Button(actions,"REINICIAR");
            var toggle=Row(page,"Particle density",45);var box=Rect(toggle,"Toggle");box.gameObject.AddComponent<LayoutElement>().preferredWidth=42;
            var bg=box.gameObject.AddComponent<Image>();bg.color=Color.white;var check=Rect(box,"Check");Stretch(check);check.offsetMin=Vector2.one*8;check.offsetMax=-Vector2.one*8;var mark=check.gameObject.AddComponent<Image>();mark.color=Blue;
            lab.reducedParticles=box.gameObject.AddComponent<Toggle>();lab.reducedParticles.targetGraphic=bg;lab.reducedParticles.graphic=mark;
            var caption=Label(toggle,"Reduzir partículas visíveis",22,45);caption.GetComponent<LayoutElement>().flexibleWidth=10;
            var back=Row(page,"Hub navigation",65);lab.backButton=Button(back,"VOLTAR AO HUB");lab.status=Label(page,"Estado",21,55,Blue);
            var modal=Rect(canvas,"Confirm return");Stretch(modal);modal.gameObject.AddComponent<Image>().color=Lavender;modal.gameObject.AddComponent<CanvasGroup>();lab.leaveDialog=modal.gameObject;
            var body=Rect(modal,"Dialog body");body.anchorMin=body.anchorMax=new Vector2(.5f,.5f);body.sizeDelta=new Vector2(700,350);
            var ml=body.gameObject.AddComponent<VerticalLayoutGroup>();ml.spacing=32;ml.childControlHeight=ml.childControlWidth=true;ml.childForceExpandHeight=false;
            Label(body,"Voltar ao Hub?\nO ensaio atual será descartado.",30,180);var buttons=Row(body,"Confirm actions",70);lab.confirmButton=Button(buttons,"VOLTAR");lab.cancelButton=Button(buttons,"CANCELAR");
            Tecaverso.UI.UIVisibility.Set(modal.gameObject,false);

            Panel(root,"Work and state analysis",new Vector3(3.55f,2.12f,2.15f),new Vector2(820,1100),out lab.analysisPage);var analysis=lab.analysisPage.transform;
            Label(analysis,"TRABALHO DE UM GÁS",34,55,Blue).font=bold;
            Label(analysis,"P × V · a área representa o trabalho",24,45);
            var region=Rect(analysis,"Graph region");region.gameObject.AddComponent<LayoutElement>().preferredHeight=390;
            var plot=Rect(region,"PV plot");plot.anchorMin=new Vector2(0,0);plot.anchorMax=Vector2.one;plot.offsetMin=new Vector2(80,60);plot.offsetMax=new Vector2(-20,-30);lab.graph=plot.gameObject.AddComponent<GasPVGraph>();lab.graph.raycastTarget=false;
            void Tick(string value,Vector2 anchor,Vector2 offset,Vector2 size)
            {
                var r=Rect(region,value);r.anchorMin=r.anchorMax=anchor;r.anchoredPosition=offset;r.sizeDelta=size;var t=r.gameObject.AddComponent<TextMeshProUGUI>();t.font=regular;t.fontSize=20;t.color=Ink;t.text=value;t.alignment=TextAlignmentOptions.Center;t.raycastTarget=false;
            }
            for(int k=0;k<=5;k++)Tick((k*10).ToString(),new Vector2(0,0),new Vector2(80+k*132.8f,40),new Vector2(60,26));
            for(int k=0;k<=4;k++)Tick((k*60).ToString(),new Vector2(0,0),new Vector2(40,60+k*75),new Vector2(70,26));
            Tick("Volume (L)",new Vector2(.5f,0),new Vector2(0,5),new Vector2(240,28));Tick("Pressão (kPa)",new Vector2(0,1),new Vector2(130,-5),new Vector2(230,28));
            lab.workLabel=Label(analysis,"Trabalho",27,95,Blue);
            Label(analysis,"Expansão: W > 0     Compressão: W < 0\nIsométrica: W = 0\n1 kPa × 1 L = 1 J",23,110);
            Label(analysis,"GÁS IDEAL · PV = nRT · n = 1 mol\nIsotérmica: W = nRT ln(Vf/Vi)\nIsobárica: W = P ΔV",23,110);
            Label(analysis,"Escala ampliada. Partículas representativas.\nA velocidade visual cresce com √T.\nValores calculados em kelvin; calor qualitativo.",21,100);
        }
        public static string Create()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Exit Play Mode.");
            if(AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath)!=null)throw new InvalidOperationException("Thermology already exists. Edit existing objects.");
            var template=SceneManager.GetSceneByPath("Assets/_Project/Laboratories/Physics/ResistorCircuits/Scenes/ResistorCircuits.unity");
            if(!template.IsValid()||!template.isLoaded)template=EditorSceneManager.OpenScene("Assets/_Project/Laboratories/Physics/ResistorCircuits/Scenes/ResistorCircuits.unity",OpenSceneMode.Additive);
            Ensure(Folder+"/Scenes");Ensure(Folder+"/Materials");
            regular=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Regular SDF.asset");bold=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Bold SDF.asset");
            navy=Mat("Instrument navy",new Color(.05f,.10f,.19f));blue=Mat("Control blue",Blue,.7f);metal=Mat("Machined silver",new Color(.55f,.66f,.77f),.72f);pearl=Mat("Instrument pearl",new Color(.83f,.88f,.94f));heat=Mat("Thermal jacket",new Color(.2f,.45f,.75f),.4f,true);
            glass=Mat("Chamber glass",new Color(.5f,.8f,1,.075f),.85f);glass.SetFloat("_Surface",1);glass.SetFloat("_SrcBlend",(float)BlendMode.SrcAlpha);glass.SetFloat("_DstBlend",(float)BlendMode.OneMinusSrcAlpha);glass.SetFloat("_ZWrite",0);glass.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");glass.renderQueue=3000;glass.SetOverrideTag("RenderType","Transparent");EditorUtility.SetDirty(glass);
            string particlePath=Folder+"/Materials/Molecules.mat";particleMat=new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));particleMat.SetColor("_BaseColor",Color.white);AssetDatabase.CreateAsset(particleMat,particlePath);
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);SceneManager.SetActiveScene(scene);
            foreach(var name in new[]{"XR Origin (XR Rig)","XR Interaction Manager","EventSystem","Directional Light"})
            {var copy=Object.Instantiate(template.GetRootGameObjects().Single(o=>o.name==name));copy.name=name;SceneManager.MoveGameObjectToScene(copy,scene);if(name=="XR Origin (XR Rig)")copy.transform.position=new Vector3(0,0,-4);}
            PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Environments/StandardRoom/TecaversoStandardRoom.prefab"),scene);
            var volume=new GameObject("Shared learning post processing").AddComponent<Volume>();volume.isGlobal=true;volume.sharedProfile=AssetDatabase.LoadAssetAtPath<VolumeProfile>("Assets/_Project/Shared/Rendering/TecaversoLearningProfile.asset");
            var root=new GameObject("Thermology Lab").transform;lab=root.gameObject.AddComponent<ThermologyLab>();Apparatus(root);Interface(root);Unity3DControlVisuals.Knob(lab.knob,3.5f);
            EditorSceneManager.SaveScene(scene,ScenePath);
            var catalog=AssetDatabase.LoadAssetAtPath<HubCatalog>("Assets/_Project/Hub/HubCatalog.asset");var entry=catalog.disciplines.First(d=>d.title=="Física").contents.Single(c=>c.title=="Termologia");
            entry.scenePath=ScenePath;entry.description="Explore pressão, volume e temperatura com um pistão, partículas e instrumentos XR. Compare transformações e o trabalho do gás.";EditorUtility.SetDirty(catalog);
            var settings=EditorBuildSettings.scenes.ToList();if(!settings.Any(s=>s.path==ScenePath))settings.Add(new EditorBuildSettingsScene(ScenePath,true));EditorBuildSettings.scenes=settings.ToArray();
            AssetDatabase.SaveAssets();
            foreach(var other in Enumerable.Range(0,SceneManager.sceneCount).Select(SceneManager.GetSceneAt).Where(s=>s!=scene&&!s.isDirty).ToArray())EditorSceneManager.CloseScene(other,true);
            return "Created Thermology scene, XR apparatus, UI and Hub entry.";
        }
    }
}


