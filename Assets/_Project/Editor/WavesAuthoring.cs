using System;
using System.Collections.Generic;
using System.Linq;
using Tecaverso.Hub;
using Tecaverso.Labs.Waves;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Object=UnityEngine.Object;

namespace Tecaverso.Editor
{
    public static class WavesAuthoring
    {
        public const string Folder="Assets/_Project/Laboratories/Physics/Waves",ScenePath=Folder+"/Scenes/Waves.unity";
        static TMP_FontAsset regular,bold;
        static Material navy,blue,metal,pearl,cyan,orange;
        static WavesLab lab;
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

        static WaveKnob Knob(Transform parent,string name,Vector3 position,float value)
        {
            var t=Node(name,parent,position);t.localRotation=Quaternion.Euler(90,0,0);
            var handle=Lathe("Rotary grip",t,new[]{(-.07f,.14f),(.03f,.14f),(.06f,.12f)},blue);
            Shape("Indicator",handle,PrimitiveType.Sphere,new Vector3(0,-.075f,.1f),Vector3.one*.025f,pearl);
            var c=handle.gameObject.AddComponent<SphereCollider>();c.radius=.15f;
            var k=t.gameObject.AddComponent<WaveKnob>();k.lab=lab;k.handle=handle;k.minAngle=-135;k.maxAngle=135;k.positionTrackedRadius=.08f;k.colliders.Add(c);k.value=value;return k;
        }
        static WaveSlide Slide(Transform parent,string name,Vector3 position,bool pulse)
        {
            var t=Node(name,parent,position);var body=Lathe("Grip",t,new[]{(-.09f,.045f),(-.07f,.07f),(.07f,.07f),(.09f,.045f)},pulse?blue:orange);
            var c=t.gameObject.AddComponent<BoxCollider>();c.size=new Vector3(.17f,.22f,.18f);
            var rb=t.gameObject.AddComponent<Rigidbody>();rb.isKinematic=true;rb.useGravity=false;
            var s=t.gameObject.AddComponent<WaveSlide>();s.lab=lab;s.pulse=pulse;s.coordinate=pulse?0:position.x;s.colliders.Add(c);s.useDynamicAttach=true;s.trackRotation=false;s.throwOnDetach=false;s.movementType=XRBaseInteractable.MovementType.Instantaneous;return s;
        }
        static void Apparatus(Transform root)
        {
            foreach(float x in new[]{-3f,3f})
            {
                var p=Node("Instrument pedestal",root,new Vector3(x,0,1));
                Lathe("Foot",p,new[]{(0f,.42f),(.08f,.44f),(.14f,.32f)},navy);
                Lathe("Telescopic column",p,new[]{(.14f,.16f),(.95f,.16f),(.98f,.10f),(1.85f,.10f)},metal);
                Lathe("Blue collar",p,new[]{(.85f,.175f),(.9f,.175f)},blue);
            }
            lab.apparatus=Node("Height adjustable apparatus",root,new Vector3(-3,1.2f,1));var a=lab.apparatus;
            var left=Node("Oscillator housing",a,new Vector3(-.15f,0,0));
            var motor=Lathe("Motor casing",left,new[]{(-.20f,.22f),(-.16f,.26f),(.16f,.26f),(.20f,.20f)},navy);motor.localRotation=Quaternion.Euler(90,0,0);
            lab.rod=Node("Driver rod",a,new Vector3(0,0,-.08f));Shape("Piston",lab.rod,PrimitiveType.Cylinder,Vector3.zero,new Vector3(.05f,.18f,.05f),metal);
            lab.pullAssembly=Node("Manual input",a,Vector3.zero).gameObject;lab.pull=Slide(lab.pullAssembly.transform,"XR pulse handle",new Vector3(-.18f,0,-.42f),true);
            WorldText(lab.pullAssembly.transform,"PUXE E SOLTE",new Vector3(-.25f,-.32f,-.4f),new Vector2(1.1f,.2f),1.5f);
            lab.knobAssembly=Node("Periodic controls",a,Vector3.zero).gameObject;
            Shape("Control face",lab.knobAssembly.transform,PrimitiveType.Cube,new Vector3(-.28f,-.15f,-.48f),new Vector3(.75f,1.05f,.12f),navy);
            lab.amplitudeKnob=Knob(lab.knobAssembly.transform,"XR amplitude",new Vector3(-.28f,.08f,-.58f),.70588f);
            lab.frequencyKnob=Knob(lab.knobAssembly.transform,"XR frequency",new Vector3(-.28f,-.38f,-.58f),.41176f);
            lab.amplitudeLabel=WorldText(lab.knobAssembly.transform,"AMPLITUDE",new Vector3(-.28f,.30f,-.59f),new Vector2(.7f,.21f),1.25f);
            lab.frequencyLabel=WorldText(lab.knobAssembly.transform,"FREQUÊNCIA",new Vector3(-.28f,-.16f,-.59f),new Vector2(.7f,.21f),1.25f);
            var sw=Node("Generator switch",a,new Vector3(.30f,-.4f,-.42f));
            Lathe("Switch base",sw,new[]{(-.07f,.12f),(.04f,.12f)},navy);
            lab.switchHandle=Node("Rocker",sw,Vector3.zero);Shape("Switch lever",lab.switchHandle,PrimitiveType.Capsule,new Vector3(0,.11f,0),new Vector3(.08f,.12f,.08f),blue);
            var sc=sw.gameObject.AddComponent<BoxCollider>();sc.center=new Vector3(0,.10f,0);sc.size=Vector3.one*.25f;
            lab.powerSwitch=sw.gameObject.AddComponent<WaveSwitch>();lab.powerSwitch.lab=lab;lab.powerSwitch.colliders.Add(sc);
            lab.switchLabel=WorldText(a,"GERADOR DESLIGADO",new Vector3(.4f,-.65f,-.46f),new Vector2(1.2f,.2f),1.2f);
            lab.fixedEnd=Node("Fixed clamp",a,new Vector3(6,0,0)).gameObject;
            Shape("Clamp back",lab.fixedEnd.transform,PrimitiveType.Cube,Vector3.zero,new Vector3(.12f,.3f,.2f),navy);
            var screw=Lathe("Clamp screw",lab.fixedEnd.transform,new[]{(0f,.045f),(.18f,.045f),(.18f,.075f),(.22f,.075f)},metal);screw.localRotation=Quaternion.Euler(90,0,0);
            lab.freeEnd=Node("Free termination",a,Vector3.zero).gameObject;
            Shape("Vertical rail",lab.freeEnd.transform,PrimitiveType.Cylinder,new Vector3(6,0,.04f),new Vector3(.045f,.45f,.045f),metal);
            lab.freeRing=Lathe("Sliding ring",lab.freeEnd.transform,new[]{(-.05f,.09f),(.05f,.09f),(.05f,.04f),(-.05f,.04f),(-.05f,.09f)},orange,false);lab.freeRing.localPosition=new Vector3(6,0,0);
            lab.absorber=Node("Absorbing module",a,new Vector3(6,0,0)).gameObject;
            var damper=Lathe("Damping cartridge",lab.absorber.transform,new[]{(-.25f,.12f),(-.2f,.16f),(.2f,.16f),(.25f,.12f)},blue);damper.localRotation=Quaternion.Euler(0,0,90);
            lab.endLabel=WorldText(a,"EXTREMIDADE FIXA",new Vector3(5.8f,.65f,0),new Vector2(1.7f,.25f),1.6f);
            var rope=Node("Numerical rope",a,Vector3.zero);lab.rope=rope.gameObject.AddComponent<MeshFilter>();lab.rope.sharedMesh=CreateRopePreview();rope.gameObject.AddComponent<MeshRenderer>().sharedMaterial=cyan;
            var points=new List<WavePoint>();foreach(int i in new[]{40,80,120,160,200})
            {
                var t=Shape("Track point "+i,a,PrimitiveType.Sphere,new Vector3(i*.025f,0,0),Vector3.one*.095f,orange,true);
                var p=t.gameObject.AddComponent<WavePoint>();p.lab=lab;p.sampleIndex=i;points.Add(p);
            }
            lab.points=points.ToArray();lab.markers=new Transform[12];for(int i=0;i<12;i++)lab.markers[i]=WorldText(a,"NÓ",Vector3.zero,new Vector2(.7f,.2f),1.5f).transform;
            lab.ruler=Node("Spatial ruler",a,new Vector3(0,-.48f,-.20f)).gameObject;
            Shape("Ruler rail",lab.ruler.transform,PrimitiveType.Cube,new Vector3(3,0,0),new Vector3(6,.035f,.05f),pearl);
            for(int i=0;i<=24;i++){float x=i*.25f;Shape("Tick",lab.ruler.transform,PrimitiveType.Cube,new Vector3(x,0,-.025f),new Vector3(.012f,i%4==0?.14f:.07f,.02f),blue);if(i%4==0)WorldText(lab.ruler.transform,(i/4)+" m",new Vector3(x,-.15f,0),new Vector2(.4f,.18f),1.3f);}
            lab.cursorA=Slide(lab.ruler.transform,"Cursor A",new Vector3(1,0,-.12f),false);lab.cursorB=Slide(lab.ruler.transform,"Cursor B",new Vector3(3,0,-.12f),false);
            WorldText(root,"ONDULATÓRIA · CORDA VIBRANTE",new Vector3(0,3.4f,3),new Vector2(7,.5f),5f);
        }
        static void Interface(Transform root)
        {
            var canvas=Panel(root,"Waves controls",new Vector3(-4.65f,1.75f,.2f),new Vector2(1000,1150),out lab.controlsPage);var page=lab.controlsPage.transform;
            Label(page,"ONDULATÓRIA",40,60,Blue).font=bold;
            var modes=Row(page,"Experiences",75);lab.modeButtons=new[]{Button(modes,"PULSO"),Button(modes,"PERIÓDICA"),Button(modes,"ESTACIONÁRIA")};
            lab.instruction=Label(page,"Instrução",25,110);
            var ends=Row(page,"Termination selection",65);ends.gameObject.AddComponent<CanvasGroup>();lab.endRow=ends.gameObject;lab.endButtons=new[]{Button(ends,"FIXA"),Button(ends,"LIVRE"),Button(ends,"ABSORVENTE")};
            Label(page,"Terminação: escolha antes do ensaio.\nReinicie para alterar; a estacionária usa fixa.",22,65);
            lab.metrics=Label(page,"Métricas",27,140,Blue);
            var actions=Row(page,"Time controls",65);lab.pauseButton=Button(actions,"PAUSAR");lab.stepButton=Button(actions,"PASSO 0,05 s");lab.resetButton=Button(actions,"REINICIAR");
            var aids=Row(page,"Observation aids",60);lab.rulerButton=Button(aids,"RÉGUA");lab.pointsButton=Button(aids,"PONTOS");lab.nodesButton=Button(aids,"NÓS");
            var height=Row(page,"Height",60);lab.lowerButton=Button(height,"BAIXAR APARELHO");lab.raiseButton=Button(height,"ELEVAR APARELHO");
            lab.status=Label(page,"Estado",23,95,Blue);
            lab.backButton=Button(Row(page,"Navigation",65),"VOLTAR AO HUB");
            var modal=Rect(canvas,"Confirmation");Stretch(modal);modal.gameObject.AddComponent<Image>().color=Lavender;modal.gameObject.AddComponent<CanvasGroup>();lab.dialog=modal.gameObject;
            var body=Rect(modal,"Content");body.anchorMin=body.anchorMax=new Vector2(.5f,.5f);body.sizeDelta=new Vector2(850,350);var layout=body.gameObject.AddComponent<VerticalLayoutGroup>();layout.spacing=24;layout.childControlHeight=layout.childControlWidth=true;layout.childForceExpandHeight=false;
            lab.dialogText=Label(body,"Confirmar?",32,180);var confirm=Row(body,"Actions",70);lab.confirmButton=Button(confirm,"CONFIRMAR");lab.cancelButton=Button(confirm,"CANCELAR");Tecaverso.UI.UIVisibility.Set(modal.gameObject,false);
            Panel(root,"Wave observation",new Vector3(4.65f,1.75f,.2f),new Vector2(1000,1000),out lab.analysisPage);var analysis=lab.analysisPage.transform;
            Label(analysis,"OBSERVAÇÃO E MEDIÇÃO",34,60,Blue).font=bold;
            lab.measurement=Label(analysis,"Distância",27,100);
            Label(analysis,"Selecione um ponto laranja na corda.\nA curva abaixo acompanha somente esse ponto.",24,85);
            lab.pointLabel=Label(analysis,"Ponto selecionado",25,85,Blue);
            var region=Rect(analysis,"Displacement graph");var le=region.gameObject.AddComponent<LayoutElement>();le.minHeight=le.preferredHeight=280;
            lab.trace=region.gameObject.AddComponent<WaveTrace>();lab.trace.raycastTarget=false;
            lab.graphScale=Label(analysis,"Eixos",23,80);
            Label(analysis,"PERFIL DA CORDA: deslocamento × posição\nGRÁFICO DO PONTO: deslocamento × tempo\nv = λf   ·   T = 1/f",24,120,Blue);
            Label(analysis,"Modelo linear com amortecimento leve.\nManoplas bloqueadas durante a pausa.",22,65);
        }
        public static Mesh CreateRopePreview()
        {
            Ensure(Folder+"/Meshes");string path=Folder+"/Meshes/RopePreview.asset";
            var existing=AssetDatabase.LoadAssetAtPath<Mesh>(path);if(existing!=null)return existing;
            var v=new Vector3[16];var n=new Vector3[16];var indices=new List<int>();
            for(int ring=0;ring<2;ring++)for(int j=0;j<8;j++){float angle=j*Mathf.PI/4;int i=ring*8+j;n[i]=new Vector3(0,Mathf.Cos(angle),Mathf.Sin(angle));v[i]=new Vector3(ring*6,0,0)+n[i]*.022f;}
            for(int j=0;j<8;j++){int b=(j+1)%8;indices.AddRange(new[]{j,b,b+8,j,b+8,j+8});}
            var mesh=new Mesh{name="Rope preview"};mesh.vertices=v;mesh.normals=n;mesh.triangles=indices.ToArray();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,path);return mesh;
        }
        public static string Create()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Exit Play Mode.");
            if(AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath)!=null)throw new InvalidOperationException("Scene exists: edit in place.");
            var template=SceneManager.GetSceneByPath("Assets/_Project/Laboratories/Physics/Thermology/Scenes/Thermology.unity");
            if(!template.IsValid()||!template.isLoaded)template=EditorSceneManager.OpenScene("Assets/_Project/Laboratories/Physics/Thermology/Scenes/Thermology.unity",OpenSceneMode.Additive);
            Ensure(Folder+"/Scenes");Ensure(Folder+"/Materials");regular=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Regular SDF.asset");bold=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Bold SDF.asset");
            navy=Mat("Instrument navy",new Color(.04f,.08f,.14f));blue=Mat("Control blue",Blue);metal=Mat("Steel",new Color(.55f,.65f,.72f),.7f);pearl=Mat("Ruler ivory",new Color(.85f,.9f,1));cyan=Mat("Rope cyan",new Color(.45f,.9f,1),.4f,true);orange=Mat("Tracking amber",new Color(1,.64f,.12f),.4f,true);
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);SceneManager.SetActiveScene(scene);
            foreach(var name in new[]{"XR Origin (XR Rig)","XR Interaction Manager","EventSystem","Directional Light"}){var c=Object.Instantiate(template.GetRootGameObjects().Single(o=>o.name==name));c.name=name;SceneManager.MoveGameObjectToScene(c,scene);if(name=="XR Origin (XR Rig)")c.transform.position=new Vector3(0,0,-4);}
            PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Environments/StandardRoom/TecaversoStandardRoom.prefab"),scene);
            var volume=new GameObject("Shared learning post processing").AddComponent<Volume>();volume.isGlobal=true;volume.sharedProfile=AssetDatabase.LoadAssetAtPath<VolumeProfile>("Assets/_Project/Shared/Rendering/TecaversoLearningProfile.asset");
            var root=new GameObject("Waves Lab").transform;lab=root.gameObject.AddComponent<WavesLab>();Apparatus(root);Interface(root);Unity3DControlVisuals.Knob(lab.amplitudeKnob,2.7f);Unity3DControlVisuals.Knob(lab.frequencyKnob,2.7f);Unity3DControlVisuals.Lever(lab.powerSwitch,1.3f);foreach(var slide in new[]{lab.pull,lab.cursorA,lab.cursorB})Unity3DControlVisuals.SliderGrip(slide.transform,3.6f);EditorSceneManager.SaveScene(scene,ScenePath);
            var catalog=AssetDatabase.LoadAssetAtPath<HubCatalog>("Assets/_Project/Hub/HubCatalog.asset");var entry=catalog.disciplines.First(d=>d.title=="Física").contents.Single(c=>c.title=="Ondas"||c.title=="Ondulatória");entry.title="Ondulatória";entry.scenePath=ScenePath;entry.description="Produza pulsos e ondas em uma corda. Explore reflexão, frequência, comprimento de onda e ondas estacionárias.";entry.objective="Relacionar propagação, oscilação e superposição por observação e medição.";EditorUtility.SetDirty(catalog);
            var scenes=EditorBuildSettings.scenes.ToList();if(!scenes.Any(s=>s.path==ScenePath))scenes.Add(new EditorBuildSettingsScene(ScenePath,true));EditorBuildSettings.scenes=scenes.ToArray();AssetDatabase.SaveAssets();
            foreach(var other in Enumerable.Range(0,SceneManager.sceneCount).Select(SceneManager.GetSceneAt).Where(s=>s!=scene&&!s.isDirty).ToArray())EditorSceneManager.CloseScene(other,true);
            return "Waves scene created and registered in Hub.";
        }
    }
}
