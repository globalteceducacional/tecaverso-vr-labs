using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Tecaverso.Hub;
using Tecaverso.Labs.Chemistry.AtomicModels;
using Object=UnityEngine.Object;

namespace Tecaverso.Editor
{
    public static class AtomicModelsAuthoring
    {
        const string Folder="Assets/_Project/Laboratories/Chemistry/AtomicModels";
        const string ScenePath=Folder+"/Scenes/AtomicModels.unity";
        const string Source="Assets/_Project/Laboratories/Mathematics/PlanarGeometry/Scenes/PlanarGeometry.unity";
        static TMP_FontAsset regular,bold;
        static Material blue,ink,pearl,purple,red,neutron,electron,cloud,cyan;

        [MenuItem("Tecaverso/Refinar orientação do museu atômico")]
        public static void RefineExistingLayout()
        {
            var scene=EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Single);
            var lab=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<AtomicModelsLab>(true)).Single();
            var exhibits=Object.FindObjectsByType<AtomicControl>(FindObjectsInactive.Include,FindObjectsSortMode.None).Where(c=>c.gameObject.scene==scene && c.action=="exhibit").ToArray();
            foreach(var exhibit in exhibits)
            {
                exhibit.transform.localRotation=exhibit.transform.localPosition.z>0?Quaternion.identity:Quaternion.Euler(0,180,0);
                var label=exhibit.transform.Find("Nome");if(label)label.localRotation=Quaternion.identity;
                var rings=exhibit.GetComponentsInChildren<Transform>(true).Where(t=>t.name=="Nível visual").ToArray();
                for(int i=0;i<rings.Length;i++)
                {
                    var ring=rings[i];var mesh=ring.GetComponent<MeshRenderer>();if(mesh)mesh.enabled=false;
                    var collider=ring.GetComponent<Collider>();if(collider)collider.enabled=false;
                    ring.localScale=Vector3.one;
                    var line=ring.GetComponent<LineRenderer>();if(!line)line=ring.gameObject.AddComponent<LineRenderer>();
                    line.useWorldSpace=false;line.loop=true;line.positionCount=80;line.startWidth=line.endWidth=.013f;
                    line.sharedMaterial=Mat("Órbitas",new Color(.38f,.8f,1f));
                    float radius=.34f+i*.13f;
                    for(int k=0;k<80;k++){float a=k*Mathf.PI*2/80;line.SetPosition(k,new Vector3(Mathf.Cos(a)*radius,Mathf.Sin(a)*radius,0));}
                }
                if(exhibit.exhibitTitle=="Modelo quântico" && !exhibit.transform.Find("Nuvem qualitativa"))
                {
                    var mist=new GameObject("Nuvem qualitativa").transform;mist.SetParent(exhibit.transform,false);mist.localPosition=new Vector3(0,0,-.27f);
                    var material=Mat("Nuvem translúcida",new Color(.40f,.72f,1f,.32f));material.SetFloat("_Surface",1);material.SetFloat("_SrcBlend",5);material.SetFloat("_DstBlend",10);material.SetFloat("_ZWrite",0);material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");material.SetOverrideTag("RenderType","Transparent");material.renderQueue=3000;EditorUtility.SetDirty(material);
                    for(int i=0;i<11;i++)
                    {
                        float a=i*2.399963f;var point=Sphere(mist,"Região provável "+i,new Vector3(Mathf.Cos(a)*(.15f+.014f*i),Mathf.Sin(a)*(.14f+.012f*i),-.015f*i),Vector3.one*(.16f+.01f*(i%3)),material);
                        point.GetComponent<Collider>().enabled=false;
                    }
                }
            }
            lab.consultationTitle.transform.localPosition=new Vector3(0,.55f,.16f);
            lab.consultationText.transform.localPosition=new Vector3(0,-.02f,.16f);
            lab.confirmation.transform.localPosition=new Vector3(0,.20f,.13f);
            var viewButton=Object.FindObjectsByType<AtomicControl>(FindObjectsInactive.Include,FindObjectsSortMode.None).FirstOrDefault(c=>c.gameObject.scene==scene && c.action=="view");
            if(viewButton){var position=viewButton.transform.localPosition;Object.DestroyImmediate(viewButton.gameObject);CreateViewLever(lab.transform,lab,position);}
            if(!lab.viewStatus)lab.viewStatus=Text(lab.transform,"Estado da vista","CAMADAS · órbitas didáticas",new Vector3(0,1.10f,-1.65f),.12f,Color.white);
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
            Debug.Log("Vitrines e fichas orientadas para o corredor central.");
        }

        public static void Validate()
        {
            if(!Application.isPlaying || SceneManager.GetActiveScene().path!=ScenePath)throw new InvalidOperationException("Abra AtomicModels em Play Mode. A validação reinicia apenas a montagem em runtime.");
            var scene=SceneManager.GetActiveScene();
            var lab=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<AtomicModelsLab>(true)).Single();
            lab.ConfirmClear();lab.SetView(false);
            var controls=Object.FindObjectsByType<AtomicControl>(FindObjectsInactive.Include,FindObjectsSortMode.None).Where(c=>c.gameObject.scene==scene).ToArray();
            if(controls.Count(c=>c.action=="exhibit")!=10 || controls.Count(c=>c.action=="change")!=6)throw new Exception("Inventário XR incompleto: vitrines="+controls.Count(c=>c.action=="exhibit")+", ajustes="+controls.Count(c=>c.action=="change")+", total="+controls.Length);
            if(lab.protonVisuals.Length!=20 || lab.neutronVisuals.Length!=24 || lab.electronVisuals.Length!=20 || lab.orbitVisuals.Length!=4)throw new Exception("Inventário de partículas incompleto.");
            for(int i=0;i<6;i++){lab.Change(0,1);lab.Change(1,1);lab.Change(2,1);}
            if(!lab.result.text.Contains("Carbono") || !lab.result.text.Contains("A 12") || !lab.result.text.Contains("neutro"))throw new Exception("C-12 inconsistente.");
            lab.Change(1,1);lab.Change(2,-1);
            if(!lab.result.text.Contains("A 13") || !lab.result.text.Contains("cátion +1"))throw new Exception("Isótopo/íon inconsistente.");
            lab.ToggleView();if(!lab.cloud || !lab.cloudVisuals[0].activeSelf || lab.orbitVisuals[0].activeSelf)throw new Exception("Alternância Camadas/Nuvem inconsistente.");
            lab.RequestClear();if(!lab.IsConfirming || !Tecaverso.UI.UIVisibility.IsVisible(lab.confirmationRoot))throw new Exception("Confirmação ausente.");
            lab.Change(0,1);if(lab.protons!=6 || lab.rotation.enabled)throw new Exception("Modal não bloqueia a oficina.");
            lab.CancelClear();if(lab.protons!=6)throw new Exception("Cancelar alterou a montagem.");
            lab.RequestClear();lab.ConfirmClear();if(lab.protons+lab.neutrons+lab.electrons!=0)throw new Exception("Limpar falhou.");
            lab.SetView(false);for(int i=0;i<30;i++){lab.Change(0,1);lab.Change(1,1);lab.Change(2,1);}
            if(lab.protons!=20 || lab.neutrons!=24 || lab.electrons!=20)throw new Exception("Limites inconsistentes.");
            for(int i=0;i<4;i++)lab.Change(1,-1);
            if(!lab.result.text.Contains("Cálcio") || !lab.result.text.Contains("A 40") || lab.orbitVisuals.Any(g=>!g.activeSelf))throw new Exception("Ca40 inconsistente.");
            if(controls.Where(c=>c.action=="exhibit").Any(c=>!c.exhibitImage))throw new Exception("Ilustração local ausente.");
            lab.ConfirmClear();lab.Change(0,-1);if(lab.protons!=0)throw new Exception("Contagem negativa.");
            var catalog=AssetDatabase.LoadAssetAtPath<HubCatalog>("Assets/_Project/Hub/HubCatalog.asset");
            if(!catalog.disciplines.First(d=>d.title=="Química").contents.Any(c=>c.scenePath==ScenePath))throw new Exception("Hub não aponta para a sala.");
            Debug.Log("VALIDAÇÃO ATÔMICA: inventário, C-12, C-13 cátion, vistas, confirmação e Hub OK.");
        }

        [MenuItem("Tecaverso/Instalar Modelos atômicos (uma vez)")]
        public static void Create()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode || Enumerable.Range(0,SceneManager.sceneCount).Any(i=>SceneManager.GetSceneAt(i).isDirty))throw new InvalidOperationException("Salve as cenas abertas antes da instalação.");
            if(AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath))throw new InvalidOperationException("A cena já existe; refine sem regenerá-la.");
            Ensure(Folder+"/Scenes");Ensure(Folder+"/Materials");
            regular=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Regular SDF.asset");
            bold=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Bold SDF.asset");
            blue=Mat("Azul",new Color(.16f,.31f,.63f));ink=Mat("Azul escuro",new Color(.035f,.07f,.18f));
            pearl=Mat("Pérola",new Color(.86f,.88f,.98f));purple=Mat("Violeta",new Color(.46f,.28f,.91f));
            red=Mat("Prótons",new Color(.95f,.26f,.33f));neutron=Mat("Nêutrons",new Color(.67f,.78f,.91f));
            electron=Mat("Elétrons",new Color(.25f,.9f,1f));cloud=Mat("Nuvem",new Color(.35f,.69f,1f));cyan=Mat("Órbitas",new Color(.38f,.8f,1f));
            var source=SceneManager.GetSceneByPath(Source);bool opened=!source.isLoaded;
            if(opened)source=EditorSceneManager.OpenScene(Source,Application.isBatchMode?OpenSceneMode.Single:OpenSceneMode.Additive);
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);SceneManager.SetActiveScene(scene);
            foreach(string name in new[]{"XR Origin (XR Rig)","XR Interaction Manager","EventSystem","Directional Light","TecaversoStandardRoom","Shared learning rendering"})
            {var copy=Object.Instantiate(source.GetRootGameObjects().First(g=>g.name==name));copy.name=name;SceneManager.MoveGameObjectToScene(copy,scene);}
            var root=new GameObject("Modelos atômicos · museu e oficina");root.SetActive(false);
            var lab=root.AddComponent<AtomicModelsLab>();
            Text(root.transform,"Título","MODELOS ATÔMICOS · ESTRUTURA DA MATÉRIA",new Vector3(0,3.35f,3.35f),.30f,Color.white);
            Text(root.transform,"Orientação","Explore as vitrines. Monte um átomo no centro com os controles + e −.",new Vector3(0,2.95f,3.35f),.18f,Color.white);

            // Physical museum: first wall is historical, opposite wall is particle structure.
            string[] titles={"Leucipo e Demócrito","Dalton · 1803","Thomson · 1904","Rutherford · 1911","Bohr · 1913","Modelo quântico"};
            string[] bodies={
                "PROPOSTA  Átomos como unidades indivisíveis da matéria.\nEVIDÊNCIA  Hipótese filosófica, sem experimento.\nAVANÇO  Pensar a matéria em unidades.\nLIMITE  Não descrevia partículas nem estrutura interna.",
                "PROPOSTA  Esfera maciça e indivisível para cada elemento.\nEVIDÊNCIA  Leis ponderais das reações.\nAVANÇO  Explicou proporções fixas.\nLIMITE  Não conhecia elétrons nem núcleo.",
                "PROPOSTA  Elétrons em uma esfera de carga positiva.\nEVIDÊNCIA  Raios catódicos revelaram o elétron.\nAVANÇO  O átomo tem partes internas.\nLIMITE  Não explica o espalhamento de partículas alfa.",
                "PROPOSTA  Núcleo pequeno e positivo; átomo quase vazio.\nEVIDÊNCIA  Espalhamento na lâmina de ouro.\nAVANÇO  Identificou o núcleo.\nLIMITE  Não explica a estabilidade e espectros.",
                "PROPOSTA  Elétrons em níveis de energia discretos.\nEVIDÊNCIA  Espectro do hidrogênio.\nAVANÇO  Explicou linhas espectrais simples.\nLIMITE  Órbitas circulares não são trajetórias quânticas reais.",
                "PROPOSTA  Orbitais descrevem distribuição de probabilidade.\nEVIDÊNCIA  Espectroscopia e teoria quântica.\nAVANÇO  Explica átomos multieletrônicos.\nLIMITE  A nuvem desta sala é apenas qualitativa."
            };
            for(int i=0;i<6;i++) Exhibit(root.transform,lab,titles[i],bodies[i],new Vector3(-3.7f+i*1.48f,1.68f,3.05f),i,Quaternion.identity);
            Exhibit(root.transform,lab,"Chadwick · 1932","DESCOBERTA  O nêutron foi identificado em 1932.\nPAPEL  Explicou parte da massa nuclear e a existência de isótopos.\nEsta placa é um marco histórico, não um modelo atômico separado.",new Vector3(3.75f,1.05f,2.98f),6,Quaternion.identity);
            string[] particleNames={"Elétron · e⁻","Próton · p⁺","Nêutron · n⁰"};
            string[] particleBodies={
                "COMPOSIÇÃO  Partícula elementar; sem quarks conhecidos.\nDESCOBERTA  J. J. Thomson, 1897.\nPAPEL  Compõe a eletrosfera e determina a carga do íon.\nLIMITE  Esta esfera não representa tamanho real.",
                "COMPOSIÇÃO  Dois quarks up e um down (uud), ligados por glúons.\nDESCOBERTA  Núcleo por Rutherford; próton identificado depois.\nPAPEL  Determina o número atômico Z.\nLIMITE  Quarks e glúons aqui são esquemáticos.",
                "COMPOSIÇÃO  Um quark up e dois down (udd), ligados por glúons.\nDESCOBERTA  James Chadwick, 1932.\nPAPEL  Altera o isótopo e o número de massa A.\nLIMITE  Não avaliamos estabilidade nuclear."
            };
            for(int i=0;i<3;i++)Exhibit(root.transform,lab,particleNames[i],particleBodies[i],new Vector3(-2.7f+i*2.7f,1.65f,-3.05f),7+i,Quaternion.Euler(0,180,0));

            // Workshop: anchored model, local physical controls and fixed readout.
            Cylinder(root.transform,"Plataforma central",new Vector3(0,.58f,0),new Vector3(2.4f,.17f,2.4f),ink);
            Cylinder(root.transform,"Anel de identidade",new Vector3(0,.69f,0),new Vector3(2.25f,.035f,2.25f),blue);
            var pivot=new GameObject("Átomo · giro sem translação").transform;pivot.SetParent(root.transform);pivot.localPosition=new Vector3(0,1.98f,0);
            var rb=pivot.gameObject.AddComponent<Rigidbody>();rb.isKinematic=true;rb.useGravity=false;
            var grabCollider=pivot.gameObject.AddComponent<SphereCollider>();grabCollider.radius=1.65f;grabCollider.isTrigger=true;
            var grab=pivot.gameObject.AddComponent<XRGrabInteractable>();grab.trackPosition=false;grab.trackRotation=false;
            lab.rotation=pivot.gameObject.AddComponent<AtomicRotateInteractable>();lab.atomPivot=pivot;
            lab.protonVisuals=new GameObject[20];lab.neutronVisuals=new GameObject[24];lab.electronVisuals=new GameObject[20];lab.cloudVisuals=new GameObject[20];lab.orbitVisuals=new GameObject[4];
            for(int i=0;i<44;i++)
            {
                float a=i*2.399963f;float radius=.06f+.052f*Mathf.Sqrt(i);var pos=new Vector3(Mathf.Cos(a)*radius,((i%5)-2)*.065f,Mathf.Sin(a)*radius);
                var item=Sphere(pivot,(i<20?"Próton ":"Nêutron ")+(i+1),pos,Vector3.one*.13f,i<20?red:neutron);
                if(i<20)lab.protonVisuals[i]=item;else lab.neutronVisuals[i-20]=item;
            }
            int electronIndex=0;
            for(int s=0;s<4;s++)
            {
                int count=new[]{2,8,8,2}[s];float radius=.52f+s*.29f;Quaternion tilt=Quaternion.Euler(new[]{0,28,-31,52}[s],s*23,12);
                var orbit=new GameObject("Camada "+(s+1));orbit.transform.SetParent(pivot,false);var line=orbit.AddComponent<LineRenderer>();line.useWorldSpace=false;line.loop=true;line.positionCount=96;line.startWidth=line.endWidth=.015f;line.sharedMaterial=cyan;
                for(int p=0;p<96;p++){float a=p*Mathf.PI*2/96;line.SetPosition(p,tilt*new Vector3(Mathf.Cos(a)*radius,0,Mathf.Sin(a)*radius));}
                lab.orbitVisuals[s]=orbit;
                for(int j=0;j<count;j++)
                {
                    float a=j*Mathf.PI*2/count;lab.electronVisuals[electronIndex]=Sphere(pivot,"Elétron "+(electronIndex+1),tilt*new Vector3(Mathf.Cos(a)*radius,0,Mathf.Sin(a)*radius),Vector3.one*.10f,electron);
                    lab.cloudVisuals[electronIndex]=Sphere(pivot,"Probabilidade qualitativa "+(electronIndex+1),tilt*new Vector3(Mathf.Cos(a)*radius,.13f*Mathf.Sin(a*3),Mathf.Sin(a)*radius),Vector3.one*.23f,cloud);
                    electronIndex++;
                }
            }
            lab.result=Text(root.transform,"Resultado fixo","Nenhum elemento\nZ 0   A 0   neutro\np⁺ 0    n⁰ 0    e⁻ 0",new Vector3(2.7f,1.8f,.7f),.22f,Color.white);
            lab.warning=Text(root.transform,"Aviso de limite","",new Vector3(2.7f,1.17f,.7f),.16f,new Color(1,.76f,.3f));lab.warning.gameObject.SetActive(false);
            Cylinder(root.transform,"Console de partículas",new Vector3(-2.9f,.8f,-.05f),new Vector3(1.8f,.12f,1.5f),ink);
            for(int row=0;row<3;row++)
            {
                Text(root.transform,"Contagem "+row,new[]{"PRÓTON  p⁺","NÊUTRON  n⁰","ELÉTRON  e⁻"}[row],new Vector3(-3.45f,1.18f,-.65f+row*.55f),.16f,Color.white);
                Control(root.transform,lab,"change","−",new Vector3(-2.75f,1.04f,-.65f+row*.55f),row,-1);
                Control(root.transform,lab,"change","+",new Vector3(-2.25f,1.04f,-.65f+row*.55f),row,1);
            }
            CreateViewLever(root.transform,lab,new Vector3(.6f,.89f,-1.62f));
            lab.viewStatus=Text(root.transform,"Estado da vista","CAMADAS · órbitas didáticas",new Vector3(0,1.10f,-1.65f),.12f,Color.white);
            Control(root.transform,lab,"recenter","RECENTRALIZAR",new Vector3(-.6f,.89f,-1.62f));
            Control(root.transform,lab,"clear","LIMPAR",new Vector3(1.7f,.89f,-1.62f));
            Control(root.transform,lab,"back","VOLTAR AO HUB",new Vector3(3.4f,.95f,-3.25f));
            var consult=new GameObject("Ficha contextual · uma por vez");consult.transform.SetParent(root.transform);consult.transform.localPosition=new Vector3(0,1.6f,-2.15f);lab.consultationRoot=consult;
            Box(consult.transform,"Fundo da ficha",new Vector3(0,0,.08f),new Vector3(3.1f,1.75f,.10f),pearl);
            lab.consultationTitle=Text(consult.transform,"Título da ficha","",new Vector3(0,.55f,.16f),.20f,new Color(.16f,.31f,.63f));
            lab.consultationText=Text(consult.transform,"Conteúdo da ficha","",new Vector3(0,-.02f,.16f),.12f,new Color(.035f,.07f,.18f));
            Control(consult.transform,lab,"close","FECHAR",new Vector3(1.1f,-.7f,-.12f));consult.SetActive(false);
            var confirm=new GameObject("Confirmação de limpeza");confirm.transform.SetParent(root.transform);confirm.transform.localPosition=new Vector3(0,1.7f,-1.2f);lab.confirmationRoot=confirm;
            Box(confirm.transform,"Fundo da confirmação",Vector3.zero,new Vector3(2.7f,.85f,.12f),pearl);
            lab.confirmation=Text(confirm.transform,"Pergunta","Limpar a montagem atual?",new Vector3(0,.20f,.13f),.16f,new Color(.035f,.07f,.18f));
            Control(confirm.transform,lab,"cancel","CANCELAR",new Vector3(-.65f,-.26f,-.11f));
            Control(confirm.transform,lab,"confirm","LIMPAR",new Vector3(.65f,-.26f,-.11f));confirm.SetActive(false);
            root.SetActive(true);
            EditorSceneManager.SaveScene(scene,ScenePath);
            var catalog=AssetDatabase.LoadAssetAtPath<HubCatalog>("Assets/_Project/Hub/HubCatalog.asset");
            var chemistry=catalog.disciplines.First(d=>d.title=="Química");var entry=chemistry.contents.First(c=>c.title=="Estrutura da matéria");
            entry.title="Modelos atômicos";entry.description="Explore a história dos modelos atômicos e monte átomos com prótons, nêutrons e elétrons.";
            entry.objective="Relacionar número atômico, isótopos, carga e modelos históricos.";entry.scenePath=ScenePath;EditorUtility.SetDirty(catalog);
            EditorBuildSettings.scenes=EditorBuildSettings.scenes.Concat(new[]{new EditorBuildSettingsScene(ScenePath,true)}).ToArray();AssetDatabase.SaveAssets();
            if(opened)EditorSceneManager.CloseScene(source,true);
            Debug.Log("Modelos atômicos instalado e registrado no Hub.");
        }
        static void Ensure(string path){if(AssetDatabase.IsValidFolder(path))return;int split=path.LastIndexOf('/');Ensure(path[..split]);AssetDatabase.CreateFolder(path[..split],path[(split+1)..]);}
        static Material Mat(string name,Color c){string p=Folder+"/Materials/"+name+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(p);if(m)return m;m=new Material(Shader.Find("Universal Render Pipeline/Lit"));m.SetColor("_BaseColor",c);AssetDatabase.CreateAsset(m,p);return m;}
        static GameObject Box(Transform p,string n,Vector3 pos,Vector3 scale,Material m){var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=n;g.transform.SetParent(p,false);g.transform.localPosition=pos;g.transform.localScale=scale;g.GetComponent<Renderer>().sharedMaterial=m;return g;}
        static GameObject Cylinder(Transform p,string n,Vector3 pos,Vector3 scale,Material m){var g=GameObject.CreatePrimitive(PrimitiveType.Cylinder);g.name=n;g.transform.SetParent(p,false);g.transform.localPosition=pos;g.transform.localScale=scale;g.GetComponent<Renderer>().sharedMaterial=m;return g;}
        static GameObject Sphere(Transform p,string n,Vector3 pos,Vector3 scale,Material m){var g=GameObject.CreatePrimitive(PrimitiveType.Sphere);g.name=n;g.transform.SetParent(p,false);g.transform.localPosition=pos;g.transform.localScale=scale;g.GetComponent<Renderer>().sharedMaterial=m;g.GetComponent<Collider>().enabled=false;return g;}
        static TMP_Text Text(Transform p,string n,string value,Vector3 pos,float size,Color color)
        {var t=new GameObject(n).AddComponent<TextMeshPro>();t.transform.SetParent(p,false);t.transform.localPosition=pos;t.transform.localRotation=Quaternion.Euler(0,180,0);t.font=regular;t.fontSize=1;t.text=value;t.color=color;t.alignment=TextAlignmentOptions.Center;t.textWrappingMode=TextWrappingModes.Normal;t.rectTransform.sizeDelta=new Vector2(14,7);t.transform.localScale=Vector3.one*size;return t;}
        static AtomicControl Control(Transform p,AtomicModelsLab lab,string action,string title,Vector3 pos,int kind=0,int delta=0)
        {
            var root=new GameObject("Controle físico · "+title);root.transform.SetParent(p,false);root.transform.localPosition=pos;
            Box(root.transform,"Base",Vector3.zero,new Vector3(title.Length>3?.8f:.34f,.11f,.30f),blue);
            var cap=Box(root.transform,"Cap",new Vector3(0,.09f,0),new Vector3(title.Length>3?.74f:.28f,.075f,.24f),purple);cap.GetComponent<Collider>().enabled=false;
            var collider=root.AddComponent<BoxCollider>();collider.size=new Vector3(title.Length>3?.9f:.42f,.25f,.38f);
            root.AddComponent<XRSimpleInteractable>();var control=root.AddComponent<AtomicControl>();control.lab=lab;control.action=action;control.kind=kind;control.delta=delta;
            Text(root.transform,"Rótulo",title,new Vector3(0,.17f,0),title.Length>3?.055f:.12f,Color.white).transform.localRotation=Quaternion.Euler(90,0,0);
            return control;
        }
        static void CreateViewLever(Transform p,AtomicModelsLab lab,Vector3 pos)
        {
            var g=new GameObject("Seletor físico · Camadas / Nuvem");g.transform.SetParent(p,false);g.transform.localPosition=pos;
            var lever=g.AddComponent<XRLever>();Unity3DControlVisuals.Lever(lever,1.5f);lever.value=false;
            var bridge=g.AddComponent<AtomicViewLever>();bridge.lab=lab;
            Text(g.transform,"Posições","CAMADAS       NUVEM",new Vector3(0,.36f,0),.065f,Color.white).transform.localRotation=Quaternion.Euler(90,0,0);
        }
        static void Exhibit(Transform p,AtomicModelsLab lab,string title,string body,Vector3 position,int kind,Quaternion rotation)
        {
            var root=new GameObject("Vitrine · "+title);root.transform.SetParent(p,false);root.transform.localPosition=position;root.transform.localRotation=rotation;
            Box(root.transform,"Placa de fundo",Vector3.zero,new Vector3(1.25f,1.55f,.13f),ink);
            Text(root.transform,"Nome",title,new Vector3(0,.60f,-.09f),.105f,Color.white).transform.localRotation=Quaternion.identity;
            Cylinder(root.transform,"Pedestal",new Vector3(0,-.52f,-.28f),new Vector3(.44f,.10f,.40f),blue);
            if(kind==0||kind==1)Sphere(root.transform,"Matéria indivisível",new Vector3(0,0,-.27f),Vector3.one*.48f,kind==0?purple:blue);
            else if(kind==2){Sphere(root.transform,"Carga positiva",new Vector3(0,0,-.27f),Vector3.one*.50f,purple);for(int i=0;i<5;i++)Sphere(root.transform,"Elétron embebido",new Vector3(Mathf.Cos(i*1.26f)*.19f,Mathf.Sin(i*1.26f)*.19f,-.55f),Vector3.one*.06f,electron);}
            else if(kind==3||kind==4||kind==5){Sphere(root.transform,"Núcleo",new Vector3(0,0,-.27f),Vector3.one*.19f,red);for(int i=0;i<(kind==3?1:3);i++){var ring=Cylinder(root.transform,"Nível visual",new Vector3(0,0,-.27f),new Vector3(.40f+i*.12f,.008f,.40f+i*.12f),cyan);ring.transform.localRotation=Quaternion.Euler(70+i*18,0,0);ring.GetComponent<Collider>().enabled=false;}}
            else if(kind==6)Sphere(root.transform,"Nêutron descoberto",new Vector3(0,0,-.27f),Vector3.one*.35f,neutron);
            else if(kind==7)Sphere(root.transform,"Elétron",new Vector3(0,0,-.27f),Vector3.one*.33f,electron);
            else for(int i=0;i<3;i++)Sphere(root.transform,"Quark "+i,new Vector3((i-1)*.20f,(i==1?.12f:-.12f),-.27f),Vector3.one*.24f,i==0?red:kind==8?purple:neutron);
            var collider=root.AddComponent<BoxCollider>();collider.size=new Vector3(1.28f,1.55f,.75f);collider.center=new Vector3(0,0,-.25f);
            root.AddComponent<XRSimpleInteractable>();var control=root.AddComponent<AtomicControl>();control.lab=lab;control.action="exhibit";control.exhibitTitle=title;control.exhibitBody=body;
        }
    }
}
