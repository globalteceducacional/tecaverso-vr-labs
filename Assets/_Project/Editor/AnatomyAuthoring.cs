using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Tecaverso.Hub;
using Tecaverso.Labs.Biology.Anatomy;
using Object=UnityEngine.Object;

namespace Tecaverso.Editor
{
    public static class AnatomyAuthoring
    {
        public const string Folder="Assets/_Project/Laboratories/Biology/Anatomy";
        public const string ScenePath=Folder+"/Scenes/Anatomy.unity";
        const string Source="Assets/_Project/Laboratories/Biology/Cells/Scenes/Cells.unity";
        static Material graphite,accent;
        public static void Create()
        {
            if(Application.isPlaying||Enumerable.Range(0,SceneManager.sceneCount).Any(i=>SceneManager.GetSceneAt(i).isDirty))throw new InvalidOperationException("A criação requer Edit Mode e cenas sem alterações pendentes.");
            if(AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath))throw new InvalidOperationException("A sala já existe; faça ajustes pontuais.");
            var visualAsset=AssetDatabase.LoadAssetAtPath<GameObject>(AnatomyAssets.Folder+"/Prefabs/AnatomyVisual.prefab");if(!visualAsset)throw new InvalidOperationException("Prepare a arte primeiro.");
            CellsAssets.Ensure(Folder+"/Scenes");CellsAssets.Ensure(Folder+"/Materials");
            graphite=Material("Grafite",new Color(.075f,.11f,.18f));accent=Material("Acento Biologia",new Color32(62,211,156,255));
            var source=SceneManager.GetSceneByPath(Source);bool opened=!source.isLoaded;if(opened)source=EditorSceneManager.OpenScene(Source,OpenSceneMode.Additive);
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);SceneManager.SetActiveScene(scene);
            foreach(string name in new[]{"XR Origin (XR Rig)","XR Interaction Manager","EventSystem","Directional Light","TecaversoStandardRoom","Shared learning rendering"})
            {
                var clone=Object.Instantiate(source.GetRootGameObjects().Single(g=>g.name==name));clone.name=name;SceneManager.MoveGameObjectToScene(clone,scene);
                if(name=="XR Origin (XR Rig)"){clone.transform.position=new Vector3(0,0,-3);clone.transform.rotation=Quaternion.identity;}
            }
            var root=new GameObject("Corpo humano · sistemas e percursos");root.SetActive(false);var lab=root.AddComponent<AnatomyLab>();
            var pivot=Child(root.transform,"Giro ancorado");pivot.localPosition=new Vector3(0,0,1.55f);lab.bodyPivot=pivot;
            var visual=(GameObject)PrefabUtility.InstantiatePrefab(visualAsset,pivot);PrefabUtility.UnpackPrefabInstance(visual,PrefabUnpackMode.Completely,InteractionMode.AutomatedAction);
            visual.transform.localRotation=Quaternion.Euler(0,180,0)*visual.transform.localRotation;
            lab.skinRoot=visual.transform.Find("skin").gameObject;var skinRenderer=lab.skinRoot.GetComponentInChildren<MeshRenderer>();
            float factor=1.7f/skinRenderer.bounds.size.y;visual.transform.localScale*=factor;var bounds=skinRenderer.bounds;
            visual.transform.position+=new Vector3(pivot.position.x-bounds.center.x,.18f-bounds.min.y,pivot.position.z-bounds.center.z);
            var skinMaterial=new Material(skinRenderer.sharedMaterial){name="Pele contextual"};var skinColor=skinMaterial.GetColor("_BaseColor");skinColor.a=.12f;skinMaterial.SetColor("_BaseColor",skinColor);skinMaterial.SetFloat("_Surface",1);skinMaterial.SetFloat("_SrcBlend",(float)BlendMode.SrcAlpha);skinMaterial.SetFloat("_DstBlend",(float)BlendMode.OneMinusSrcAlpha);skinMaterial.SetFloat("_ZWrite",0);skinMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");skinMaterial.renderQueue=3000;AssetDatabase.CreateAsset(skinMaterial,Folder+"/Materials/Pele contextual.mat");skinRenderer.sharedMaterial=skinMaterial;skinRenderer.shadowCastingMode=ShadowCastingMode.Off;
            var map=AnatomyAssets.ReadMap();var parts=new List<AnatomyOrgan>();lab.systems=new GameObject[9];
            for(int i=0;i<9;i++)
            {
                var system=visual.transform.Find(AnatomyDefinitions.Ids[i]);lab.systems[i]=system.gameObject;
                var definitions=AnatomyDefinitions.Parts(AnatomyDefinitions.Ids[i],map.groups.Single(g=>g.id==AnatomyDefinitions.Ids[i]).names);
                foreach(var definition in definitions)
                {
                    var wrapper=Child(system,definition.title);var organ=wrapper.gameObject.AddComponent<AnatomyOrgan>();organ.lab=lab;organ.system=i;organ.title=definition.title;organ.function=definition.function;organ.context=definition.context;
                    organ.distanceCalculationMode=XRBaseInteractable.DistanceCalculationMode.ColliderPosition;organ.colliders.Clear();
                    foreach(var name in definition.meshes)
                    {
                        var mesh=system.Find(name);if(!mesh)throw new InvalidOperationException("Peça canônica ausente: "+name);
                        mesh.SetParent(wrapper,true);var collider=mesh.gameObject.AddComponent<MeshCollider>();collider.sharedMesh=mesh.GetComponent<MeshFilter>().sharedMesh;collider.convex=false;organ.colliders.Add(collider);
                    }
                    organ.surfaces=wrapper.GetComponentsInChildren<Renderer>();parts.Add(organ);
                }
            }
            lab.organs=parts.ToArray();
            Cylinder(root.transform,"Base anatômica",new Vector3(0,.08f,1.55f),new Vector3(1.2f,.08f,1.2f),graphite);
            Cylinder(root.transform,"Identidade Biologia",new Vector3(0,.165f,1.55f),new Vector3(1.17f,.008f,1.17f),accent);
            CreatePath(lab);CreateUI(lab,root.transform);
            for(int i=0;i<9;i++)lab.systems[i].SetActive(i==0);
            root.SetActive(true);lab.Choose(0);EditorSceneManager.SaveScene(scene,ScenePath);
            var catalog=AssetDatabase.LoadAssetAtPath<HubCatalog>("Assets/_Project/Hub/HubCatalog.asset");var entry=catalog.disciplines.Single(d=>d.title=="Biologia").contents.Single(c=>c.title=="Corpo humano e sistemas");entry.scenePath=ScenePath;entry.description="Explore nove sistemas no mesmo corpo. Consulte estruturas e observe quatro percursos esquemáticos.";entry.objective="Relacionar estruturas, funções e posições dos sistemas do corpo humano.";EditorUtility.SetDirty(catalog);
            if(!EditorBuildSettings.scenes.Any(s=>s.path==ScenePath))EditorBuildSettings.scenes=EditorBuildSettings.scenes.Concat(new[]{new EditorBuildSettingsScene(ScenePath,true)}).ToArray();
            AssetDatabase.SaveAssets();if(opened)EditorSceneManager.CloseScene(source,true);EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Single);
        }
        static void CreateUI(AnatomyLab lab,Transform root)
        {
            var console=Child(root,"Console inclinado");console.localPosition=new Vector3(-1.03f,.89f,-.8f);console.localRotation=Quaternion.Euler(45,0,0);
            Cube(console,"Tampo",Vector3.zero,new Vector3(1.22f,.73f,.055f),graphite);Cube(console,"Faixa azul",new Vector3(0,-.37f,0),new Vector3(1.22f,.025f,.06f),Material("Azul",BiologyWorldUI.Blue));
            Cube(root,"Apoio do console",new Vector3(-1.03f,.39f,-.65f),new Vector3(.23f,.78f,.32f),graphite);
            var panel=BiologyWorldUI.Panel(root,"C1 · Sistemas",console.localPosition+console.localRotation*Vector3.back*.04f,new Vector2(720,430),.0016f,console.localRotation);
            var body=BiologyWorldUI.Body(panel,18);var grid=BiologyWorldUI.Rect(body,"Nove sistemas");BiologyWorldUI.Height(grid,190);
            var layout=grid.gameObject.AddComponent<GridLayoutGroup>();layout.constraint=GridLayoutGroup.Constraint.FixedColumnCount;layout.constraintCount=3;layout.cellSize=new Vector2(221,58);layout.spacing=new Vector2(10,8);
            lab.systemButtons=AnatomyDefinitions.Titles.Select(title=>BiologyWorldUI.Button(grid,title)).ToArray();
            var row=BiologyWorldUI.Rect(body,"Preferência e percurso");BiologyWorldUI.Height(row,58);var horizontal=row.gameObject.AddComponent<HorizontalLayoutGroup>();horizontal.spacing=10;horizontal.childControlWidth=horizontal.childControlHeight=true;horizontal.childForceExpandWidth=true;
            lab.skin=BiologyWorldUI.Toggle(row,"Pele (contexto)");lab.process=BiologyWorldUI.Button(row,"Observar percurso");lab.processLabel=lab.process.GetComponentInChildren<TMP_Text>();
            lab.status=BiologyWorldUI.Text(body,"Seleção atual: Respiratório. Aponte para consultar.",48,20);
            lab.back=BiologyWorldUI.Button(body,"Voltar ao Hub",48);
            var dial=Child(root,"Girar corpo");lab.ring=dial.gameObject.AddComponent<XRKnob>();lab.ring.minAngle=0;lab.ring.maxAngle=360;Unity3DControlVisuals.Knob(lab.ring,1.7f);lab.ring.value=0;
            var dialCaption=BiologyWorldUI.Caption(root,"Girar corpo",Vector3.zero,new Vector2(220,50),22);
            MountDial(console,lab.ring,dialCaption.transform.parent);
            BiologyWorldUI.Caption(root,"CORPO HUMANO",new Vector3(0,2.24f,1.55f),new Vector2(900,60),37);
            lab.systemTitle=BiologyWorldUI.Caption(root,"SISTEMA RESPIRATÓRIO",new Vector3(0,2.12f,1.55f),new Vector2(850,60),27);
            lab.legend=BiologyWorldUI.Caption(root,"Ar: inspiração e expiração · percurso esquemático, sem escala de velocidade.",new Vector3(0,2.4f,1.55f),new Vector2(1600,90),20,false);
            lab.selectedLabel=BiologyWorldUI.Caption(root,"",new Vector3(-.78f,1.5f,.35f),new Vector2(350,100),25);
            var leader=Child(root,"R1 · Ligação com a estrutura");lab.leader=leader.gameObject.AddComponent<LineRenderer>();lab.leader.useWorldSpace=true;lab.leader.positionCount=2;lab.leader.widthMultiplier=.0016f;lab.leader.sharedMaterial=Material("Linha de consulta",new Color(.6f,.83f,1),"Universal Render Pipeline/Unlit");lab.leader.enabled=false;
            var query=BiologyWorldUI.Panel(root,"Q1 · Consulta",new Vector3(1.6f,1.51f,.05f),new Vector2(590,750),.0016f,Quaternion.identity);lab.consultation=query.gameObject;
            var content=BiologyWorldUI.Body(query,26);lab.queryTitle=BiologyWorldUI.Text(content,"",115,34,true);lab.queryBody=BiologyWorldUI.Text(content,"",460,28,false,new Color32(22,22,22,255));lab.queryBody.alignment=TextAlignmentOptions.TopLeft;
            lab.close=BiologyWorldUI.Button(content,"Fechar consulta",62);Tecaverso.UI.UIVisibility.Set(query.gameObject,false);
        }
        public static void MountDial(Transform console,XRKnob dial,Transform caption)
        {
            const string mountName="Extensão do tampo · giro";
            if(!console.Find(mountName))
            {
                var material=console.Find("Tampo").GetComponent<Renderer>().sharedMaterial;
                Cube(console,mountName,new Vector3(.80f,0,0),new Vector3(.39f,.43f,.055f),material);
            }
            // The original Unity mesh protrudes along local -Y; the reading
            // surface faces console-local -Z, shared by the caption and grip.
            dial.transform.SetPositionAndRotation(console.TransformPoint(new Vector3(.80f,.065f,-.031f)),
                console.rotation*Quaternion.Euler(90,0,0));
            var label=(RectTransform)caption;
            label.rotation=console.rotation;
            label.anchoredPosition3D=label.parent.InverseTransformPoint(console.TransformPoint(new Vector3(.80f,-.147f,-.038f)));
        }
        static void CreatePath(AnatomyLab lab)
        {
            var node=Child(lab.bodyPivot,"Percursos anatômicos");
            lab.path=node.gameObject.AddComponent<AnatomyPath>();
            lab.path.surfaceMaterial=AssetDatabase.LoadAssetAtPath<Material>(Folder+"/Materials/Percursos.mat");
            if(!lab.path.surfaceMaterial)throw new InvalidOperationException("Material dos percursos ausente.");
        }
        static Transform Child(Transform parent,string name){var t=new GameObject(name).transform;t.SetParent(parent,false);return t;}
        static Material Material(string name,Color color,string shader="Universal Render Pipeline/Lit")
        {
            string path=Folder+"/Materials/"+name+".mat";var material=AssetDatabase.LoadAssetAtPath<Material>(path);if(material)return material;
            material=new Material(Shader.Find(shader)){name=name};material.SetColor("_BaseColor",color);AssetDatabase.CreateAsset(material,path);return material;
        }
        static void Cube(Transform parent,string name,Vector3 position,Vector3 scale,Material material){Primitive(parent,name,position,scale,material,PrimitiveType.Cube);}
        static void Cylinder(Transform parent,string name,Vector3 position,Vector3 scale,Material material){Primitive(parent,name,position,scale,material,PrimitiveType.Cylinder);}
        static void Primitive(Transform parent,string name,Vector3 position,Vector3 scale,Material material,PrimitiveType type)
        {var g=GameObject.CreatePrimitive(type);g.name=name;g.transform.SetParent(parent,false);g.transform.localPosition=position;g.transform.localScale=scale;g.GetComponent<Renderer>().sharedMaterial=material;}
    }
}
