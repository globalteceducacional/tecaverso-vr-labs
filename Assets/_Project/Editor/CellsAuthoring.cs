using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Tecaverso.Hub;
using Tecaverso.Labs.Biology.Cells;
using Object = UnityEngine.Object;

namespace Tecaverso.Editor
{
    public static class CellsAuthoring
    {
        public const string Folder = "Assets/_Project/Laboratories/Biology/Cells";
        public const string ScenePath = Folder + "/Scenes/Cells.unity";
        const string Source = "Assets/_Project/Laboratories/Mathematics/PlanarGeometry/Scenes/PlanarGeometry.unity";
        static readonly Color Paper = new Color32(232,230,254,255), Blue = new Color32(40,78,160,255), Ink = new Color32(7,56,111,255);
        static TMP_FontAsset regular, bold;
        static Material pedestal, accent, selectedMaterial;

        [MenuItem("Tecaverso/Instalar Células (uma vez)")]
        public static void Create()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || Enumerable.Range(0,SceneManager.sceneCount).Any(i=>SceneManager.GetSceneAt(i).isDirty))
                throw new InvalidOperationException("A instalação exige Edit Mode e cenas sem alterações pendentes.");
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath)) throw new InvalidOperationException("A sala já existe. Use ajustes pontuais.");
            CellsAssets.Ensure(Folder + "/Scenes"); CellsAssets.Ensure(Folder + "/Materials");
            regular = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Regular SDF.asset");
            bold = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Bold SDF.asset");
            pedestal = Material("Grafite",new Color(.08f,.12f,.19f)); accent=Material("Azul",Blue);
            selectedMaterial=Material("Seleção",new Color(1,.84f,.40f),true);
            var source = SceneManager.GetSceneByPath(Source); bool opened = !source.isLoaded;
            if (opened) source=EditorSceneManager.OpenScene(Source,OpenSceneMode.Additive);
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive); SceneManager.SetActiveScene(scene);
            foreach (string name in new[]{"XR Origin (XR Rig)","XR Interaction Manager","EventSystem","Directional Light","TecaversoStandardRoom","Shared learning rendering"})
            {
                var clone=Object.Instantiate(source.GetRootGameObjects().Single(g=>g.name==name)); clone.name=name; SceneManager.MoveGameObjectToScene(clone,scene);
                if (name=="XR Origin (XR Rig)") { clone.transform.position=new Vector3(0,0,-2.9f); clone.transform.rotation=Quaternion.identity; }
            }
            var root=new GameObject("Células · comparação e exploração"); root.SetActive(false);
            var lab=root.AddComponent<CellsLab>(); var parts=new List<CellStructure>();
            lab.displays=new[]{Display(root.transform,lab,"Animal_Cell",new Vector3(-.83f,1.78f,.9f),parts),Display(root.transform,lab,"Plant_Cell",new Vector3(.83f,1.78f,.9f),parts)};
            lab.structures=parts.ToArray();
            CreateUI(root.transform,lab);
            LayoutCaptions(lab);
            root.SetActive(true);
            EditorSceneManager.SaveScene(scene,ScenePath);
            RegisterCatalog();
            if (!EditorBuildSettings.scenes.Any(s=>s.path==ScenePath)) EditorBuildSettings.scenes=EditorBuildSettings.scenes.Concat(new[]{new EditorBuildSettingsScene(ScenePath,true)}).ToArray();
            AssetDatabase.SaveAssets();
            if(opened)EditorSceneManager.CloseScene(source,true);
            EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Single);
        }
        static CellDisplay Display(Transform parent,CellsLab lab,string model,Vector3 center,List<CellStructure> parts)
        {
            bool plant=model=="Plant_Cell";
            var root=Child(parent,plant?"Célula vegetal":"Célula animal");
            var display=root.gameObject.AddComponent<CellDisplay>(); display.lab=lab;
            Cylinder(root,"Suporte elevado",new Vector3(center.x,.57f,center.z),new Vector3(.72f,.57f,.72f),pedestal);
            Cylinder(root,"Tampo",new Vector3(center.x,1.18f,center.z),new Vector3(1.15f,.045f,1.15f),pedestal);
            Cylinder(root,"Faixa de identidade",new Vector3(center.x,1.135f,center.z),new Vector3(1.1f,.015f,1.1f),accent);
            var pivot=Child(root,"Giro ancorado"); pivot.localPosition=center; display.pivot=pivot;
            var asset=AssetDatabase.LoadAssetAtPath<GameObject>(CellsAssets.Folder+"/Prefabs/"+model+".prefab");
            var visual=(GameObject)PrefabUtility.InstantiatePrefab(asset,pivot); visual.transform.localPosition=Vector3.zero;
            PrefabUtility.UnpackPrefabInstance(visual,PrefabUnpackMode.Completely,InteractionMode.AutomatedAction);
            visual.transform.localRotation=Quaternion.Euler(-15,0,0)*visual.transform.localRotation;
            var bounds=BoundsOf(visual.transform); visual.transform.localScale*=1f/Mathf.Max(bounds.size.x,bounds.size.y,bounds.size.z);
            bounds=BoundsOf(visual.transform); visual.transform.position+=pivot.position-bounds.center;
            Caption(root,plant?"Célula vegetal":"Célula animal",new Vector3(center.x,2.49f,center.z),new Vector2(600,90),42);
            Caption(root,"MODELO AMPLIADO · CORES DIDÁTICAS",new Vector3(center.x,1.13f,center.z-.58f),new Vector2(610,50),22);
            var dial=Child(root,"Anel de rotação"); dial.localPosition=new Vector3(center.x,1.15f,center.z-.72f); dial.localRotation=Quaternion.Euler(45,0,0);
            display.ring=dial.gameObject.AddComponent<XRKnob>(); display.ring.minAngle=0;display.ring.maxAngle=360;display.ring.clampedMotion=true;
            Unity3DControlVisuals.Knob(display.ring,2.1f);
            display.ring.value=0;
            Caption(root,"Girar célula",new Vector3(center.x,1.01f,center.z-.84f),new Vector2(400,60),22);
            AddPart(visual.transform,"Nucleo",plant?"vegetal.nucleo":"animal.nucleo","Núcleo",true,lab,display,parts,
                "Abriga a maior parte do DNA e participa do controle da expressão gênica.","Presente nas duas células representadas.",new Vector3(.13f,.19f,-.08f));
            AddPart(visual.transform,"Mitocondria_1",plant?"vegetal.mitocondria":"animal.mitocondria","Mitocôndria",true,lab,display,parts,
                "Participa da respiração celular e da produção de ATP.","Células vegetais também possuem mitocôndrias e realizam respiração celular.",new Vector3(-.16f,.14f,-.08f));
            if (plant)
            {
                AddPart(visual.transform,"Cloroplasto_1","vegetal.cloroplasto","Cloroplasto",true,lab,display,parts,"Realiza fotossíntese.","Representado nesta célula vegetal; não faz parte da célula animal.",new Vector3(.2f,.15f,-.08f));
                AddPart(visual.transform,"Vacuolo","vegetal.vacuolo","Vacúolo central",true,lab,display,parts,"Armazena substâncias e contribui para o turgor da célula.","Grande compartimento característico do modelo vegetal apresentado.",new Vector3(0,.3f,-.1f));
                AddPart(visual.transform,"ParedeCelular","vegetal.parede","Parede celular",false,lab,display,parts,"Contribui para sustentação e proteção.","É externa à membrana plasmática. A parede vegetal contém celulose.",new Vector3(-.22f,-.24f,-.09f));
            }
            else AddPart(visual.transform,"MembranaCelular","animal.membrana","Membrana plasmática",false,lab,display,parts,"Delimita a célula e participa do controle das trocas com o meio.","As duas células possuem membrana plasmática; ela não é a parede celular.",new Vector3(.24f,-.24f,-.09f));
            return display;
        }
        static void AddPart(Transform visual,string original,string id,string title,bool detachable,CellsLab lab,CellDisplay display,List<CellStructure> parts,string function,string context,Vector3 labelOffset)
        {
            var source=visual.Find(original); if(!source)throw new InvalidOperationException("Estrutura semântica não encontrada: "+original);
            var bounds=BoundsOf(source);
            var wrapper=new GameObject(id).transform; wrapper.SetParent(visual,false); wrapper.SetPositionAndRotation(bounds.center,source.rotation);
            source.SetParent(wrapper,true);
            var home=Child(visual,"Origem · "+id); home.SetPositionAndRotation(wrapper.position,wrapper.rotation);
            var body=wrapper.gameObject.AddComponent<Rigidbody>(); body.isKinematic=true;body.useGravity=false;
            var collider=wrapper.gameObject.AddComponent<BoxCollider>();
            var local=LocalBounds(wrapper);collider.center=local.center;collider.size=Vector3.Max(local.size,new Vector3(.045f,.045f,.045f));
            if(!detachable)
            {
                // A small patch on the shell remains selectable without intercepting every internal organelle.
                collider.center=local.center+new Vector3(local.extents.x*.83f,0,0);
                collider.size=new Vector3(.065f,local.size.y*.30f,.075f);
            }
            var part=wrapper.gameObject.AddComponent<CellStructure>();part.lab=lab;part.display=display;part.structureId=id;part.title=title;part.detachable=detachable;
            part.function=function;part.context=context;part.home=home;part.colliders.Clear();part.colliders.Add(collider);part.throwOnDetach=false;
            ConfigureGrab(part);
            part.caption=Caption(home,title,labelOffset,new Vector2(390,65),29);part.caption.transform.parent.rotation=Quaternion.identity;
            var labelCanvas=part.caption.transform.parent;labelCanvas.localScale*=.0012f/labelCanvas.lossyScale.x;
            if(detachable)
            {
                var socketRoot=Child(home,"Encaixe original");var trigger=socketRoot.gameObject.AddComponent<SphereCollider>();trigger.isTrigger=true;trigger.radius=.1f;
                var socket=socketRoot.gameObject.AddComponent<CellReturnSocket>();socket.target=part;socket.attachTransform=home;socket.showInteractableHoverMeshes=false;part.socket=socket;
                DockHint(part);
            }
            parts.Add(part);
        }
        static void CreateUI(Transform root,CellsLab lab)
        {
            var controls=Panel(root,"Controles compactos",new Vector3(0,.82f,-.35f),new Vector2(1080,200),.00105f); lab.controlsRoot=controls.gameObject;
            var body=Body(controls,18);var row=Rect(body,"Ações");Height(row,65);var layout=row.gameObject.AddComponent<HorizontalLayoutGroup>();layout.spacing=16;layout.childControlWidth=layout.childControlHeight=true;layout.childForceExpandWidth=true;
            lab.labels=Toggle(row,"Exibir rótulos");lab.restore=Button(row,"Recompor células");lab.back=Button(row,"Voltar ao Hub");
            lab.status=Label(body,"",87,25,Ink);
            var consult=Panel(root,"Consulta lateral",new Vector3(2.56f,1.78f,.65f),new Vector2(620,740),.0016f);lab.consultationRoot=consult.gameObject;
            var content=Body(consult,28);lab.consultationTitle=Label(content,"",90,38,Blue);lab.consultationTitle.font=bold;
            lab.consultationBody=Label(content,"",478,28,new Color32(22,22,22,255));lab.consultationBody.alignment=TextAlignmentOptions.TopLeft;
            lab.close=Button(content,"Fechar consulta");Height((RectTransform)lab.close.transform,65);
            var confirm=Panel(root,"Confirmação local",consult.localPosition,new Vector2(620,370),.0016f);lab.confirmationRoot=confirm.gameObject;
            var cb=Body(confirm,28);lab.confirmationText=Label(cb,"",160,31,Ink);
            lab.cancel=Button(cb,"Cancelar");Height((RectTransform)lab.cancel.transform,65);
            lab.confirm=Button(cb,"Recompor");Height((RectTransform)lab.confirm.transform,65);lab.confirmLabel=lab.confirm.GetComponentInChildren<TMP_Text>();
            Tecaverso.UI.UIVisibility.Set(consult.gameObject,false);Tecaverso.UI.UIVisibility.Set(confirm.gameObject,false);
        }
        static Transform Child(Transform p,string name){var t=new GameObject(name).transform;t.SetParent(p,false);return t;}
        static Material Material(string name,Color color,bool unlit=false){string path=Folder+"/Materials/"+name+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);if(m)return m;m=new Material(Shader.Find(unlit?"Universal Render Pipeline/Unlit":"Universal Render Pipeline/Lit")){name=name};m.SetColor("_BaseColor",color);AssetDatabase.CreateAsset(m,path);return m;}
        static void Cylinder(Transform parent,string name,Vector3 position,Vector3 scale,Material material){var g=GameObject.CreatePrimitive(PrimitiveType.Cylinder);g.name=name;g.transform.SetParent(parent,false);g.transform.localPosition=position;g.transform.localScale=scale;g.GetComponent<Renderer>().sharedMaterial=material;}
        static Bounds BoundsOf(Transform t){var r=t.GetComponentsInChildren<Renderer>(true);var b=r[0].bounds;foreach(var x in r.Skip(1))b.Encapsulate(x.bounds);return b;}
        static Bounds LocalBounds(Transform t){var b=new Bounds();bool first=true;foreach(var f in t.GetComponentsInChildren<MeshFilter>(true)){var m=f.sharedMesh.bounds;for(int i=0;i<8;i++){var p=m.center+Vector3.Scale(m.extents,new Vector3((i&1)==0?-1:1,(i&2)==0?-1:1,(i&4)==0?-1:1));p=t.InverseTransformPoint(f.transform.TransformPoint(p));if(first){b=new Bounds(p,Vector3.zero);first=false;}else b.Encapsulate(p);}}return b;}
        static RectTransform Rect(Transform p,string name){var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(p,false);return r;}
        static void Height(RectTransform r,float value){var e=r.GetComponent<LayoutElement>();if(!e)e=r.gameObject.AddComponent<LayoutElement>();e.minHeight=e.preferredHeight=value;}
        static RectTransform Panel(Transform p,string name,Vector3 position,Vector2 size,float scale){var r=Rect(p,name);r.localPosition=position;r.localScale=Vector3.one*scale;r.sizeDelta=size;r.gameObject.AddComponent<Canvas>().renderMode=RenderMode.WorldSpace;r.gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();var image=r.gameObject.AddComponent<Image>();image.color=Paper;image.raycastTarget=false;return r;}
        static RectTransform Body(RectTransform p,int margin){var r=Rect(p,"Conteúdo");r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=Vector2.one*margin;r.offsetMax=-Vector2.one*margin;var l=r.gameObject.AddComponent<VerticalLayoutGroup>();l.spacing=12;l.childControlWidth=l.childControlHeight=true;l.childForceExpandHeight=false;l.childForceExpandWidth=true;return r;}
        static TMP_Text Label(Transform p,string value,float height,float size,Color color){var r=Rect(p,"Texto");Height(r,height);var t=r.gameObject.AddComponent<TextMeshProUGUI>();t.font=regular;t.fontSize=size;t.color=color;t.text=value;t.raycastTarget=false;t.alignment=TextAlignmentOptions.Center;t.textWrappingMode=TextWrappingModes.Normal;t.overflowMode=TextOverflowModes.Ellipsis;return t;}
        static TMP_Text Caption(Transform p,string value,Vector3 position,Vector2 size,float fontSize){var r=Rect(p,value);r.localPosition=position;r.localRotation=Quaternion.identity;r.localScale=Vector3.one*.0015f;r.sizeDelta=size;r.gameObject.AddComponent<Canvas>().renderMode=RenderMode.WorldSpace;var t=Label(r,value,size.y,fontSize,Color.white);var rt=t.rectTransform;rt.anchorMin=Vector2.zero;rt.anchorMax=Vector2.one;rt.offsetMin=rt.offsetMax=Vector2.zero;t.font=bold;return t;}
        static Button Button(Transform p,string value){var r=Rect(p,value);Height(r,65);var image=r.gameObject.AddComponent<Image>();image.color=Blue;var b=r.gameObject.AddComponent<Button>();b.targetGraphic=image;var text=Label(r,value,65,26,Color.white);text.font=bold;text.rectTransform.anchorMin=Vector2.zero;text.rectTransform.anchorMax=Vector2.one;text.rectTransform.offsetMin=new Vector2(8,3);text.rectTransform.offsetMax=new Vector2(-8,-3);return b;}
        static Toggle Toggle(Transform p,string value){var r=Rect(p,value);var image=r.gameObject.AddComponent<Image>();image.color=Blue;var toggle=r.gameObject.AddComponent<Toggle>();toggle.targetGraphic=image;var mark=Rect(r,"Marca");mark.anchorMin=new Vector2(0,.5f);mark.anchorMax=mark.anchorMin;mark.anchoredPosition=new Vector2(22,0);mark.sizeDelta=new Vector2(23,23);toggle.graphic=mark.gameObject.AddComponent<Image>();toggle.graphic.color=new Color32(62,211,156,255);var text=Label(r,value,65,26,Color.white);text.rectTransform.anchorMin=Vector2.zero;text.rectTransform.anchorMax=Vector2.one;text.rectTransform.offsetMin=new Vector2(42,3);text.rectTransform.offsetMax=new Vector2(-5,-3);toggle.SetIsOnWithoutNotify(false);return toggle;}

        public static void RefinePresentation()
        {
            if(Application.isPlaying || SceneManager.GetActiveScene().path!=ScenePath)throw new InvalidOperationException("Abra Cells em Edit Mode.");
            var lab=Object.FindFirstObjectByType<CellsLab>();
            foreach(var d in lab.displays)
            {
                d.ring.value=0;d.pivot.localRotation=Quaternion.identity;
                var visual=d.pivot.GetChild(0);
                var original=AssetDatabase.LoadAssetAtPath<GameObject>(CellsAssets.Folder+"/Prefabs/"+visual.name+".prefab");
                visual.localRotation=Quaternion.Euler(-15,0,0)*original.transform.localRotation;
                var renderers=visual.GetComponentsInChildren<MeshRenderer>(true);var bounds=renderers[0].bounds;
                foreach(var r in renderers.Skip(1))bounds.Encapsulate(r.bounds);
                visual.position+=d.pivot.position-bounds.center;
                var caption=d.GetComponentsInChildren<TMP_Text>(true).FirstOrDefault(t=>t.text.StartsWith("Gire a célula")||t.text=="Girar célula");
                if(caption){caption.text="Girar célula";caption.transform.parent.localPosition=new Vector3(d.pivot.localPosition.x,1.01f,d.pivot.localPosition.z-.84f);}
                EditorUtility.SetDirty(d.ring);
            }
            lab.controlsRoot.transform.localPosition=new Vector3(0,.82f,-.35f);lab.controlsRoot.transform.localScale=Vector3.one*.00105f;
            foreach(var part in lab.structures)
            {
                ConfigureGrab(part);
                if(part.detachable)DockHint(part);
                var caption=part.caption.transform.parent;caption.rotation=Quaternion.identity;
                caption.localScale*=.0012f/caption.lossyScale.x;
                Vector3 offset=part.structureId.Contains("nucleo")?new Vector3(.13f,.19f,-.08f):part.structureId.Contains("mitocondria")?new Vector3(-.16f,.14f,-.08f):part.structureId.Contains("cloroplasto")?new Vector3(.20f,.15f,-.08f):part.structureId.Contains("vacuolo")?new Vector3(0,.3f,-.1f):new Vector3(.24f,-.24f,-.09f);
                caption.position=part.home.position+offset;
            }
            LayoutCaptions(lab);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }
        public static void RefineCaptions()
        {
            if(Application.isPlaying || SceneManager.GetActiveScene().path!=ScenePath)throw new InvalidOperationException("Abra Cells em Edit Mode.");
            LayoutCaptions(Object.FindFirstObjectByType<CellsLab>());
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }
        static void LayoutCaptions(CellsLab lab)
        {
            foreach(var d in lab.displays)
            {
                float x=d.pivot.localPosition.x;
                var labels=d.GetComponentsInChildren<TMP_Text>(true);
                var dial=labels.First(t=>t.text=="Girar célula");dial.transform.parent.localPosition=new Vector3(x,.91f,d.pivot.localPosition.z-.84f);
                var legend=labels.First(t=>t.text.StartsWith("MODELO AMPLIADO"));legend.transform.parent.localPosition=new Vector3(x,2.37f,d.pivot.localPosition.z);
                var parts=lab.structures.Where(p=>p.display==d).OrderByDescending(p=>p.home.position.y).ToArray();
                for(int i=0;i<parts.Length;i++)
                {
                    var p=parts[i];var canvas=(RectTransform)p.caption.transform.parent;
                    canvas.SetParent(d.transform,true);canvas.localRotation=Quaternion.identity;canvas.localScale=Vector3.one*.0012f;canvas.sizeDelta=new Vector2(360,90);
                    p.caption.fontSize=24;
                    var line=canvas.GetComponent<LineRenderer>();if(!line)line=canvas.gameObject.AddComponent<LineRenderer>();
                    line.sharedMaterial=Material("Rótulos",new Color(.58f,.82f,1),true);line.useWorldSpace=true;line.positionCount=2;line.widthMultiplier=.0015f;
                    line.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;line.enabled=false;p.captionLeader=line;
                    canvas.anchoredPosition3D=new Vector3(x<0?-1.59f:1.59f,(parts.Length>3?2.20f:2.12f)-i*.22f,.25f);
                }
            }
            var position=lab.consultationRoot.transform.localPosition;position.x=2.56f;
            ((RectTransform)lab.consultationRoot.transform).anchoredPosition3D=position;
            ((RectTransform)lab.confirmationRoot.transform).anchoredPosition3D=position;
        }
        static void ConfigureGrab(CellStructure part)
        {
            var general=part.GetComponent<UnityEngine.XR.Interaction.Toolkit.Transformers.XRGeneralGrabTransformer>();
            if(!general)general=part.gameObject.AddComponent<UnityEngine.XR.Interaction.Toolkit.Transformers.XRGeneralGrabTransformer>();
            var limit=part.GetComponent<CellGrabLimit>();if(!limit)limit=part.gameObject.AddComponent<CellGrabLimit>();
            limit.home=part.home;limit.detachable=part.detachable;
            part.addDefaultGrabTransformers=false;
            part.startingSingleGrabTransformers.Clear();part.startingSingleGrabTransformers.Add(general);part.startingSingleGrabTransformers.Add(limit);
        }
        static void DockHint(CellStructure part)
        {
            var found=part.home.Find("Indicação de encaixe");var root=found?found:Child(part.home,"Indicação de encaixe");
            var line=root.GetComponent<LineRenderer>();if(!line)line=root.gameObject.AddComponent<LineRenderer>();
            line.sharedMaterial=Material("Encaixe",new Color(.35f,.85f,1),true);line.useWorldSpace=false;line.loop=true;line.positionCount=32;line.widthMultiplier=.0018f;
            for(int i=0;i<32;i++){float a=i*Mathf.PI*2/32;line.SetPosition(i,new Vector3(Mathf.Cos(a)*.045f,0,Mathf.Sin(a)*.045f));}
            line.enabled=false;part.homeOutline=line;
        }
        public static void RegisterCatalog()
        {
            var catalog=AssetDatabase.LoadAssetAtPath<HubCatalog>("Assets/_Project/Hub/HubCatalog.asset");var biology=catalog.disciplines.Single(d=>d.title=="Biologia");
            var old=biology.contents;
            string[] names={"Células animal e vegetal","Corpo humano e sistemas","Microrganismos e vírus","DNA, RNA e informação","Ecossistemas e relações","Evolução e seleção natural"};
            string[] descriptions={"Compare duas células ampliadas. Consulte, separe e reencaixe organelas.","Explore órgãos e sistemas em um mesmo corpo. Em desenvolvimento.","Compare bactérias, vírus e estruturas celulares. Em desenvolvimento.","Explore complementaridade entre DNA e RNA. Em desenvolvimento.","Investigue cadeias alimentares, energia e matéria. Em desenvolvimento.","Compare características de populações entre gerações. Em desenvolvimento."};
            if(old.Length>6)throw new InvalidOperationException("Catálogo modificado externamente; revisar antes de consolidar.");
            var entries=new List<HubCatalog.Content>();
            for(int i=0;i<names.Length;i++)
            {
                var existing=old.FirstOrDefault(c=>c.title==names[i] || (i==0 && c.title=="Células") || (i==1 && c.title=="Corpo humano") || (i==4 && c.title=="Ecossistemas"));
                var c=existing??new HubCatalog.Content{icon=biology.icon};c.title=names[i];c.description=descriptions[i];
                if(i==0){c.scenePath=ScenePath;c.objective="Relacionar estruturas celulares, funções e diferenças entre células animal e vegetal.";}
                else if(existing==null)c.objective=descriptions[i].Replace(" Em desenvolvimento.","");
                entries.Add(c);
            }
            biology.contents=entries.ToArray();EditorUtility.SetDirty(catalog);AssetDatabase.SaveAssets();
        }
    }
}
