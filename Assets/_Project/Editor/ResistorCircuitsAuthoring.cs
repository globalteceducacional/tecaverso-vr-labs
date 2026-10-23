using System;
using System.Collections.Generic;
using System.Linq;
using Tecaverso.Hub;
using Tecaverso.Labs.ResistorCircuits;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Object=UnityEngine.Object;

namespace Tecaverso.Editor
{
    public static class ResistorCircuitsAuthoring
    {
        const string Folder="Assets/_Project/Laboratories/Physics/ResistorCircuits";
        const string ScenePath=Folder+"/Scenes/ResistorCircuits.unity";
        static readonly Color Blue=new Color32(40,78,160,255),Navy=new Color32(7,56,111,255),Graphite=new Color32(27,38,55,255),Pearl=new Color32(211,209,232,255),Ink=new Color32(22,22,22,255);
        static readonly List<CircuitTerminal> Terminals=new();
        static readonly List<CircuitPart> Parts=new();
        static readonly List<FlexibleCable> Cables=new();
        static TMP_FontAsset font;
        static Material bodyMat,terminalMat,redMat,blackMat,bulbMat,flowMat;
        static CircuitLab lab;
        static void Ensure(string path){if(AssetDatabase.IsValidFolder(path))return;int p=path.LastIndexOf('/');Ensure(path[..p]);AssetDatabase.CreateFolder(path[..p],path[(p+1)..]);}
        static Material Mat(string name,Color color,bool emissive=false)
        {
            var path=Folder+"/Materials/"+name+".mat";
            var mat=AssetDatabase.LoadAssetAtPath<Material>(path);
            if(mat!=null)return mat;
            mat=new Material(Shader.Find("Universal Render Pipeline/Lit")){name=name};mat.SetColor("_BaseColor",color);
            if(emissive){mat.EnableKeyword("_EMISSION");mat.SetColor("_EmissionColor",color*.65f);}
            AssetDatabase.CreateAsset(mat,path);return mat;
        }
        static Transform Shape(string name,Transform parent,PrimitiveType primitive,Vector3 position,Vector3 scale,Material material,bool collider=true)
        {
            var go=GameObject.CreatePrimitive(primitive);go.name=name;go.transform.SetParent(parent,false);go.transform.localPosition=position;go.transform.localScale=scale;
            go.GetComponent<Renderer>().sharedMaterial=material;if(!collider)Object.DestroyImmediate(go.GetComponent<Collider>());
            return go.transform;
        }
        static TMP_Text Text(Transform parent,string value,Vector3 position,float width=.8f,float height=.35f,float size=3,Color? color=null)
        {
            var text=new GameObject(value,typeof(TextMeshPro)).GetComponent<TextMeshPro>();text.transform.SetParent(parent,false);text.transform.localPosition=position;
            text.rectTransform.sizeDelta=new Vector2(width,height);text.font=font;text.fontSize=size;text.color=color??Color.white;
            text.alignment=TextAlignmentOptions.Center;text.text=value;text.raycastTarget=false;return text;
        }
        static CircuitTerminal Terminal(CircuitPart part,string name,Vector3 position,Material color)
        {
            var t=Shape(name,part.transform,PrimitiveType.Sphere,position,Vector3.one*.18f,color);
            t.GetComponent<Collider>().isTrigger=true;
            t.gameObject.AddComponent<Rigidbody>().isKinematic=true;
            var socket=t.gameObject.AddComponent<XRSocketInteractor>();socket.showInteractableHoverMeshes=false;socket.interactionLayers=(UnityEngine.XR.Interaction.Toolkit.InteractionLayerMask)2;
            var target=new GameObject("Plug alignment").transform;target.SetParent(t,false);target.localPosition=Vector3.zero;socket.attachTransform=target;
            var terminal=t.gameObject.AddComponent<CircuitTerminal>();terminal.lab=lab;terminal.id=Terminals.Count;Terminals.Add(terminal);return terminal;
        }
        static CircuitPart Part(Transform root,string name,ElectricalRole role,Vector3 position,float ohms,Material color,bool grabbable)
        {
            var go=new GameObject(name);go.transform.SetParent(root,false);go.transform.localPosition=position;
            var part=go.AddComponent<CircuitPart>();part.role=role;part.resistance=ohms;part.lab=lab;Parts.Add(part);
            Shape("Housing",go.transform,PrimitiveType.Cube,Vector3.zero,new Vector3(.95f,.45f,.4f),color);
            part.a=Terminal(part,"A",new Vector3(-.58f,0,0),redMat);part.b=Terminal(part,"B",new Vector3(.58f,0,0),blackMat);
            if(grabbable)
            {
                var rb=go.AddComponent<Rigidbody>();rb.useGravity=false;rb.isKinematic=false;rb.mass=1;
                var box=go.AddComponent<BoxCollider>();box.size=new Vector3(.95f,.45f,.4f);
                var grab=go.AddComponent<XRGrabInteractable>();grab.movementType=XRBaseInteractable.MovementType.Kinematic;
            }
            Text(go.transform,name,new Vector3(0,.65f,0),2.5f,.5f,1);
            return part;
        }
        static FlexibleCable Cable(Transform rack,int index,Material color)
        {
            var root=new GameObject("Cable "+(index+1));root.transform.SetParent(rack,false);
            var cable=root.AddComponent<FlexibleCable>();
            CableEnd End(string name,Vector3 position)
            {
                var tip=Shape(name,root.transform,PrimitiveType.Cylinder,position,new Vector3(.12f,.14f,.12f),color);
                tip.localRotation=Quaternion.Euler(90,0,0);
                tip.gameObject.AddComponent<Rigidbody>().useGravity=false;
                var grab=tip.gameObject.AddComponent<XRGrabInteractable>();grab.movementType=XRBaseInteractable.MovementType.Kinematic;grab.interactionLayers=(UnityEngine.XR.Interaction.Toolkit.InteractionLayerMask)3;
                return tip.gameObject.AddComponent<CableEnd>();
            }
            cable.first=End("Plug A",new Vector3(0,1.2f,index*.3f));
            cable.second=End("Plug B",new Vector3(.3f,1.2f,index*.3f));
            LineRenderer Line(string name,Material material,float width)
            {
                var line=new GameObject(name).AddComponent<LineRenderer>();line.transform.SetParent(root.transform,false);
                line.useWorldSpace=true;line.material=material;line.widthMultiplier=width;line.numCapVertices=3;line.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;return line;
            }
            cable.sheath=Line("Flexible sheath",color,.045f);cable.flow=Line("Current indicator",flowMat,.018f);
            Cables.Add(cable);return cable;
        }
        static Button Button(Transform parent,string label,Vector2 location,Vector2 size)
        {
            var rect=new GameObject(label,typeof(RectTransform),typeof(Image),typeof(Button)).GetComponent<RectTransform>();rect.SetParent(parent,false);
            rect.anchorMin=rect.anchorMax=new Vector2(.5f,.5f);rect.sizeDelta=size;rect.anchoredPosition=location;rect.GetComponent<Image>().color=Blue;
            var text=new GameObject("Label",typeof(RectTransform),typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();text.transform.SetParent(rect,false);
            text.rectTransform.anchorMin=Vector2.zero;text.rectTransform.anchorMax=Vector2.one;text.rectTransform.offsetMin=text.rectTransform.offsetMax=Vector2.zero;
            text.font=font;text.fontSize=24;text.color=Color.white;text.text=label;text.alignment=TextAlignmentOptions.Center;text.raycastTarget=false;
            return rect.GetComponent<Button>();
        }
        static GameObject Panel(Transform root)
        {
            var go=new GameObject("Circuit instructions",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster),typeof(TrackedDeviceGraphicRaycaster));
            go.transform.SetParent(root,false);go.transform.position=new Vector3(-4,2.2f,-3.7f);go.transform.rotation=Quaternion.Euler(0,25,0);go.transform.localScale=Vector3.one*.0024f;
            var canvas=go.GetComponent<Canvas>();canvas.renderMode=RenderMode.WorldSpace;var rect=(RectTransform)go.transform;rect.sizeDelta=new Vector2(760,900);
            var bg=go.AddComponent<Image>();bg.color=Pearl;
            var title=new GameObject("Title",typeof(RectTransform),typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();title.transform.SetParent(go.transform,false);
            title.rectTransform.sizeDelta=new Vector2(680,90);title.rectTransform.anchoredPosition=new Vector2(0,370);title.font=font;title.fontSize=39;title.color=Ink;title.text="ASSOCIAÇÃO DE RESISTORES";title.alignment=TextAlignmentOptions.Center;
            var guide=new GameObject("Guide",typeof(RectTransform),typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();guide.transform.SetParent(go.transform,false);
            guide.rectTransform.sizeDelta=new Vector2(660,350);guide.rectTransform.anchoredPosition=new Vector2(0,100);guide.font=font;guide.fontSize=26;guide.color=Ink;
            guide.text="1. Pegue as pontas dos cabos e encaixe nos terminais.\n2. Construa um caminho fechado da bateria à lâmpada e de volta.\n3. Feche o interruptor. Compare série e paralelo.\n4. Ligue o amperímetro em série e o voltímetro em paralelo.";
            var status=new GameObject("Circuit state",typeof(RectTransform),typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();status.transform.SetParent(go.transform,false);
            status.rectTransform.sizeDelta=new Vector2(660,70);status.rectTransform.anchoredPosition=new Vector2(0,-235);status.font=font;status.fontSize=22;status.color=Blue;
            status.text="CIRCUITO ABERTO";status.alignment=TextAlignmentOptions.Center;lab.summary=status;
            lab.resetButton=Button(go.transform,"SOLTAR CABOS",new Vector2(0,-190),new Vector2(650,68));
            lab.backButton=Button(go.transform,"VOLTAR AO HUB",new Vector2(0,-365),new Vector2(650,68));
            var page=new GameObject("Controls page",typeof(RectTransform),typeof(CanvasGroup));page.transform.SetParent(go.transform,false);
            var pageRect=(RectTransform)page.transform;pageRect.anchorMin=Vector2.zero;pageRect.anchorMax=Vector2.one;pageRect.offsetMin=pageRect.offsetMax=Vector2.zero;
            foreach(var child in new Transform[]{title.transform,guide.transform,status.transform,lab.resetButton.transform,lab.backButton.transform})child.SetParent(page.transform,false);
            var modal=new GameObject("Confirm return",typeof(RectTransform),typeof(Image),typeof(CanvasGroup));modal.transform.SetParent(go.transform,false);
            var mr=(RectTransform)modal.transform;mr.anchorMin=Vector2.zero;mr.anchorMax=Vector2.one;mr.offsetMin=mr.offsetMax=Vector2.zero;modal.GetComponent<Image>().color=Pearl;
            var question=new GameObject("Question",typeof(RectTransform),typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();question.transform.SetParent(modal.transform,false);
            question.rectTransform.sizeDelta=new Vector2(650,220);question.rectTransform.anchoredPosition=new Vector2(0,80);question.font=font;question.fontSize=32;question.color=Ink;question.text="Voltar ao Hub?\nA montagem atual será descartada.";question.alignment=TextAlignmentOptions.Center;
            lab.confirmButton=Button(modal.transform,"VOLTAR",new Vector2(-165,-150),new Vector2(300,70));
            lab.cancelButton=Button(modal.transform,"CANCELAR",new Vector2(165,-150),new Vector2(300,70));
            lab.leaveDialog=modal;lab.pages=page;return go;
        }
        public static string Create()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Exit Play Mode first.");
            if(AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath)!=null)throw new InvalidOperationException("Scene exists. Edit it; do not rebuild.");
            for(int i=0;i<SceneManager.sceneCount;i++)if(SceneManager.GetSceneAt(i).isDirty)throw new InvalidOperationException("Save authored scene changes first.");
            Ensure(Folder+"/Scenes");Ensure(Folder+"/Materials");
            font=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Bold SDF.asset");
            bodyMat=Mat("Instrument graphite",Graphite);terminalMat=Mat("Terminal metal",new Color(.55f,.65f,.75f));redMat=Mat("Positive red",new Color(.85f,.18f,.22f));blackMat=Mat("Negative blue",new Color(.12f,.37f,.72f));
            bulbMat=Mat("Bulb glass",new Color(.95f,.82f,.6f),true);flowMat=Mat("Current blue",new Color(.3f,.8f,1),true);
            var source=SceneManager.GetActiveScene();var roots=source.GetRootGameObjects();
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);SceneManager.SetActiveScene(scene);
            foreach(var name in new[]{"XR Origin (XR Rig)","XR Interaction Manager","EventSystem","Directional Light"})
            {var original=roots.Single(r=>r.name==name);var copy=Object.Instantiate(original);copy.name=name;}
            var room=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Environments/StandardRoom/TecaversoStandardRoom.prefab"),scene);
            room.transform.localScale=new Vector3(1.1f,1,1.1f);PrefabUtility.RecordPrefabInstancePropertyModifications(room.transform);
            var root=new GameObject("Resistor Circuits Lab").transform;lab=root.gameObject.AddComponent<CircuitLab>();Terminals.Clear();Parts.Clear();Cables.Clear();
            Text(root,"OFICINA DE CIRCUITOS",new Vector3(0,3.8f,5),8,.8f,.7f,new Color(.75f,.86f,1));
            var sourcePart=Part(root,"BATERIA 9 V",ElectricalRole.Source,new Vector3(-4,1.3f,-1),0,Mat("Brand blue",Blue),true);
            Text(sourcePart.transform,"+",new Vector3(-.57f,.18f,-.16f),.3f,.3f,.5f);
            Text(sourcePart.transform,"−",new Vector3(.57f,.18f,-.16f),.3f,.3f,.5f);
            var switchPart=Part(root,"INTERRUPTOR",ElectricalRole.Switch,new Vector3(-2,1.3f,-1),0,bodyMat,false);
            var handle=Shape("Handle",switchPart.transform,PrimitiveType.Cube,new Vector3(0,.37f,0),new Vector3(.12f,.45f,.12f),terminalMat);
            var toggle=handle.gameObject.AddComponent<XRSimpleInteractable>();handle.gameObject.AddComponent<SwitchHandle>().part=switchPart;handle.localRotation=Quaternion.Euler(32,0,0);
            var lamp=Part(root,"LÂMPADA",ElectricalRole.Lamp,new Vector3(4,1.3f,4),12,bodyMat,false);
            var bulb=Shape("Filament bulb",lamp.transform,PrimitiveType.Sphere,new Vector3(0,.52f,0),Vector3.one*.55f,bulbMat);
            lamp.lampRenderer=bulb.GetComponent<Renderer>();lamp.lampLight=bulb.gameObject.AddComponent<Light>();lamp.lampLight.type=LightType.Point;lamp.lampLight.range=3;lamp.lampLight.enabled=false;
            for(int i=0;i<3;i++)
            {
                var r=Part(root,$"R{i+1}",ElectricalRole.Resistor,new Vector3(-3.5f+i*2.7f,1.3f,2.3f),new[]{10f,20f,30f}[i],bodyMat,true);
                r.display=Text(r.transform,$"{r.resistance:0} Ω",new Vector3(0,.02f,-.23f),1.5f,.5f,.28f,new Color(.85f,.92f,1));
            }
            for(int i=0;i<2;i++)
            {
                var junction=Part(root,$"DERIVAÇÃO {i+1}",ElectricalRole.Junction,new Vector3(-1.5f+i*2.9f,1.3f,4.2f),0,Mat("Deep blue",Navy),false);
                junction.additionalTerminals=new[]{Terminal(junction,"C",new Vector3(0,.38f,0),terminalMat),Terminal(junction,"D",new Vector3(0,-.38f,0),terminalMat)};
            }
            var meter=Part(root,"AMPERÍMETRO",ElectricalRole.Ammeter,new Vector3(-4,1.3f,5),.01f,Mat("Brand blue",Blue),true);
            meter.display=Text(meter.transform,"0,00 A",new Vector3(0,0,-.23f),1.5f,.5f,.28f);
            var volt=Part(root,"VOLTÍMETRO",ElectricalRole.Voltmeter,new Vector3(4,1.3f,5),0,Mat("Brand blue",Blue),true);
            volt.display=Text(volt.transform,"0,00 V",new Vector3(0,0,-.23f),1.5f,.5f,.28f);
            foreach(var part in Parts)
            {
                var pedestal=Shape("Instrument pedestal",root,PrimitiveType.Cylinder,new Vector3(part.transform.localPosition.x,.55f,part.transform.localPosition.z),new Vector3(.6f,.55f,.6f),bodyMat);
            }
            var rack=new GameObject("Cable rack").transform;rack.SetParent(root,false);rack.position=new Vector3(-5.3f,0,-3.5f);
            for(int i=0;i<14;i++)Cable(rack,i,i%2==0?redMat:blackMat);
            Shape("Cable storage rail",root,PrimitiveType.Cube,new Vector3(-5.2f,1,-1.45f),new Vector3(.6f,.15f,4.8f),bodyMat);
            Panel(root);
            lab.terminals=Terminals.ToArray();lab.parts=Parts.ToArray();lab.cables=Cables.ToArray();
            EditorSceneManager.SaveScene(scene,ScenePath);
            ResistorCircuitsPresentation.Configure();
            ResistorCircuitsMeshModels.Configure();
            var catalog=AssetDatabase.LoadAssetAtPath<HubCatalog>("Assets/_Project/Hub/HubCatalog.asset");
            var entry=catalog.disciplines.First(d=>d.title=="Física").contents.Single(c=>c.title=="Eletricidade");
            entry.title="Associação de resistores";entry.description="Monte circuitos em série e paralelo com fios e instrumentos XR.";
            entry.objective="Relacionar resistência, tensão, corrente e potência da lâmpada.";entry.scenePath=ScenePath;
            EditorUtility.SetDirty(catalog);AssetDatabase.SaveAssetIfDirty(catalog);
            var scenes=EditorBuildSettings.scenes.ToList();scenes.Add(new EditorBuildSettingsScene(ScenePath,true));EditorBuildSettings.scenes=scenes.ToArray();
            foreach(var other in Enumerable.Range(0,SceneManager.sceneCount).Select(SceneManager.GetSceneAt).Where(s=>s!=scene&&!s.isDirty).ToArray())EditorSceneManager.CloseScene(other,true);
            AssetDatabase.SaveAssets();return "Created Resistor Circuits scene and Hub entry.";
        }
    }
}
