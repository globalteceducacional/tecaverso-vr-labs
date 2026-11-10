using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Tecaverso.Labs.Chemistry.Reactions;
using Tecaverso.Labs.PlanarGeometry;
using Tecaverso.Liquids;
using Object=UnityEngine.Object;

namespace Tecaverso.Editor
{
    /// <summary>Initial installation only. Never regenerate an authored room to update its controls.</summary>
    public static class ReactionsAuthoring
    {
        public const string Folder="Assets/_Project/Laboratories/Chemistry/Reactions";
        public const string ScenePath=Folder+"/Scenes/Reactions.unity";
        const string Sample="Assets/ThirdParty/UnityLiquidSample/";
        static TMP_FontAsset regular,bold;
        static Button buttonTemplate;static Toggle toggleTemplate;
        static Material glass,blue,dark,metal,white,liquidMaterial,particleMaterial;
        static Mesh bottleMesh,bottleLiquid,tankMesh,tankLiquid;
        static readonly Color Blue=new Color32(40,78,160,255),Lavender=new Color32(232,230,254,255);

        public static void Create()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode||Enumerable.Range(0,SceneManager.sceneCount).Any(i=>SceneManager.GetSceneAt(i).isDirty))throw new Exception("Preserve as cenas não salvas antes da instalação.");
            if(AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath))throw new Exception("A cena já existe. Faça alterações pontuais.");
            foreach(var folder in new[]{"Scenes","Materials","Prefabs","Meshes"})Ensure(Folder+"/"+folder);
            var source=EditorSceneManager.OpenScene("Assets/_Project/Laboratories/Mathematics/PlanarGeometry/Scenes/PlanarGeometry.unity",OpenSceneMode.Additive);
            var template=source.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<PlanarGallery>()).Single();
            buttonTemplate=template.backButton;toggleTemplate=template.verticesToggle;
            regular=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Regular SDF.asset");
            bold=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Bold SDF.asset");
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);SceneManager.SetActiveScene(scene);
            foreach(string name in new[]{"XR Origin (XR Rig)","XR Interaction Manager","EventSystem","Directional Light","TecaversoStandardRoom","Shared learning rendering"})
            {var copy=Object.Instantiate(source.GetRootGameObjects().Single(g=>g.name==name));copy.name=name;SceneManager.MoveGameObjectToScene(copy,scene);}
            Materials();
            bottleMesh=MeshAt("BottleChunk.fbx","Bottle_Chunk");bottleLiquid=MeshAt("BottleChunk.fbx","Bottle_ChunkLiquid");
            tankMesh=MeshAt("TankCylinder.fbx","Tank_Cylinder");tankLiquid=MeshAt("TankCylinder.fbx","Tank_CylinderLiquid");
            var root=new GameObject("Reações químicas · exploração individual");root.SetActive(false);
            var lab=root.AddComponent<ReactionsLab>();
            lab.title=Label(root.transform,"Título","REAÇÕES QUÍMICAS",new Vector3(0,3.75f,2.9f),new Vector2(2400,150),54,true);
            Label(root.transform,"Instrução","PEGUE · INCLINE · OBSERVE\nTrês reações cadastradas · soluções didáticas a 0,10 mol/L",new Vector3(0,3.38f,2.9f),new Vector2(2200,180),30,false);

            // Open central station: students can reach the mouth and walk around its sides.
            var reactor=Child(root.transform,"Béquer de reação");reactor.localPosition=new Vector3(0,1.16f,.5f);
            Cylinder(root.transform,"Base do reator",new Vector3(0,.58f,.5f),new Vector3(.72f,.58f,.72f),dark,true);
            Cylinder(root.transform,"Aro azul",new Vector3(0,.91f,.5f),new Vector3(.76f,.025f,.76f),blue,false);
            var glassBody=MeshObject(reactor,"Béquer · vidro",tankMesh,glass);glassBody.localScale=Vector3.one*3;
            lab.receiver=Liquid(reactor,tankLiquid,Vector3.one*3);lab.receiver.fill=0;
            lab.opening=Child(reactor,"Abertura receptora");lab.opening.localPosition=new Vector3(0,.27f,0);lab.openingRadius=.165f;
            // Only the walls/base collide; the opening remains accessible to the physical spoon.
            Cylinder(reactor,"Fundo físico",new Vector3(0,-.27f,0),new Vector3(.36f,.008f,.36f),metal,true);
            for(int i=0;i<12;i++)
            {
                float angle=i*Mathf.PI/6;var wall=Box(reactor,"Parede física",new Vector3(Mathf.Sin(angle)*.18f,0,Mathf.Cos(angle)*.18f),new Vector3(.095f,.52f,.012f),null,true);
                wall.localRotation=Quaternion.Euler(0,i*30,0);
            }
            lab.sediment=Cylinder(reactor,"CaCO₃ depositado",new Vector3(0,-.225f,0),new Vector3(.30f,.001f,.30f),white,false);lab.sediment.gameObject.SetActive(false);
            for(int i=1;i<=5;i++)
            {
                float y=-.24f+.432f*i/5;
                Box(reactor,"Marca "+i*100+" mL",new Vector3(.07f,y,-.182f),new Vector3(.04f,.003f,.003f),white,false);
                Label(reactor,"Volume "+i*100,i*100+"",new Vector3(.12f,y,-.19f),new Vector2(80,35),20,false,Color.white,.001f);
            }
            lab.bubbles=Particles(reactor,"CO₂ · bolhas",new Color(.72f,.92f,1,.7f),.022f,.23f,1.1f);
            lab.bubbles.transform.rotation=Quaternion.LookRotation(Vector3.up);
            lab.precipitate=Particles(reactor,"CaCO₃ · suspensão",Color.white,.014f,.07f,1.6f);
            lab.precipitate.transform.rotation=Quaternion.LookRotation(Vector3.down);
            lab.readings=Label(root.transform,"Leituras","0 / 500 mL",new Vector3(0,1.98f,1.4f),new Vector2(1650,330),32,false);
            lab.hint=Label(root.transform,"Orientação contextual","Pegue um frasco e incline sobre o béquer.",new Vector3(0,.62f,-.12f),new Vector2(1200,240),28,false);

            lab.tools=new ReagentTool[3];
            for(int i=0;i<2;i++)
            {
                var home=Child(root.transform,"Retorno frasco "+i);home.localPosition=new Vector3(i==0?-.7f:.7f,1.2f,.25f);
                Cylinder(root.transform,"Suporte frasco "+i,new Vector3(home.position.x,.45f,.25f),new Vector3(.36f,.45f,.36f),dark,true);
                Cylinder(root.transform,"Bandeja retorno "+i,new Vector3(home.position.x,.925f,.25f),new Vector3(.38f,.025f,.38f),blue,true);
                lab.tools[i]=BottlePrefab(i,root.transform);lab.tools[i].home=home;lab.tools[i].lab=lab;lab.tools[i].transform.SetPositionAndRotation(home.position,home.rotation);
            }
            var powder=Child(root.transform,"Bicarbonato e colher");powder.localPosition=new Vector3(1.25f,0,-.55f);lab.powderStation=powder.gameObject;
            Cylinder(powder,"Suporte do sólido",new Vector3(0,.45f,0),new Vector3(.45f,.45f,.45f),dark,true);
            var dish=MeshObject(powder,"Cuba de sólido",tankMesh,glass);dish.localPosition=new Vector3(0,1,0);dish.localScale=new Vector3(2.8f,.9f,2.8f);
            lab.powderStockVisual=Cylinder(powder,"Bicarbonato em pó",new Vector3(0,.97f,0),new Vector3(.29f,.018f,.29f),white,false);
            lab.powderDish=Child(powder,"Zona de coleta");lab.powderDish.localPosition=new Vector3(0,1.02f,0);
            lab.powderLabel=Label(powder,"Rótulo do sólido","NaHCO₃ · 6 g",new Vector3(0,1.62f,.1f),new Vector2(600,210),28,false);
            lab.tools[2]=SpoonPrefab(root.transform);lab.tools[2].lab=lab;
            lab.tools[2].home=Child(root.transform,"Retorno colher");lab.tools[2].home.localPosition=new Vector3(1.15f,1.16f,-.83f);lab.tools[2].transform.position=lab.tools[2].home.position;
            CreateControls(root.transform,lab);
            CreateMolecularView(root.transform,lab);
            FinishInitialLayout(lab);
            root.SetActive(true);Canvas.ForceUpdateCanvases();
            var catalog=AssetDatabase.LoadAssetAtPath<Tecaverso.Hub.HubCatalog>("Assets/_Project/Hub/HubCatalog.asset");
            var content=catalog.disciplines.Single(d=>d.title=="Química").contents.Single(c=>c.title.StartsWith("Reações"));
            content.scenePath=ScenePath;content.description="Incline frascos, dose reagentes e observe gás, precipitado e neutralização.";content.objective="Relacionar evidências, reagente limitante e conservação dos átomos.";EditorUtility.SetDirty(catalog);
            EditorSceneManager.SaveScene(scene,ScenePath);EditorBuildSettings.scenes=EditorBuildSettings.scenes.Concat(new[]{new EditorBuildSettingsScene(ScenePath,true)}).ToArray();AssetDatabase.SaveAssets();
            EditorSceneManager.CloseScene(source,true);
            for(int i=SceneManager.sceneCount-1;i>=0;i--){var other=SceneManager.GetSceneAt(i);if(other!=scene&&!other.isDirty)EditorSceneManager.CloseScene(other,true);}
        }
        static void Materials()
        {
            blue=Material("Azul",Blue);dark=Material("Grafite",new Color(.035f,.055f,.09f));metal=Material("Metal",new Color(.5f,.58f,.64f));white=Material("Sólido branco",new Color(.92f,.94f,.95f));
            glass=Material("Vidraria",new Color(.65f,.86f,.95f,.13f));glass.SetFloat("_Surface",1);glass.SetFloat("_SrcBlend",5);glass.SetFloat("_DstBlend",10);glass.SetFloat("_ZWrite",0);glass.SetFloat("_Smoothness",.8f);glass.SetFloat("_Cull",2);glass.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");glass.renderQueue=3000;glass.SetOverrideTag("RenderType","Transparent");EditorUtility.SetDirty(glass);
            var shader=AssetDatabase.LoadAssetAtPath<Shader>("Assets/_Project/Shared/Liquids/TransparentLiquid.shadergraph");if(!shader)throw new Exception("Variante transparente do Shader Graph Unity não encontrada.");
            liquidMaterial=new Material(shader){name="Líquido Unity · instâncias por PropertyBlock"};AssetDatabase.CreateAsset(liquidMaterial,Folder+"/Materials/Liquid.mat");
            particleMaterial=new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit")){name="Partículas de reação"};particleMaterial.SetColor("_BaseColor",Color.white);AssetDatabase.CreateAsset(particleMaterial,Folder+"/Materials/Particles.mat");
            ConfigureParticleMaterial(particleMaterial);
        }
        public static void ConfigureParticleMaterial(Material mat)
        {
            const string path=Folder+"/Materials/SoftDroplet.asset";
            var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if(!texture)
            {
                texture=new Texture2D(64,64,TextureFormat.RGBA32,false){name="Soft circular droplet",wrapMode=TextureWrapMode.Clamp};
                for(int y=0;y<64;y++)for(int x=0;x<64;x++)
                {float r=Vector2.Distance(new Vector2(x+.5f,y+.5f),new Vector2(32,32))/32;float alpha=1-Mathf.SmoothStep(.65f,1,r);float light=Mathf.Lerp(1,.65f,r);texture.SetPixel(x,y,new Color(light,light,light,alpha));}
                texture.Apply();AssetDatabase.CreateAsset(texture,path);
            }
            mat.SetTexture("_BaseMap",texture);mat.SetFloat("_Surface",1);mat.SetFloat("_SrcBlend",5);mat.SetFloat("_DstBlend",10);mat.SetFloat("_ZWrite",0);mat.SetFloat("_Cull",0);mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");mat.renderQueue=3000;mat.SetOverrideTag("RenderType","Transparent");EditorUtility.SetDirty(mat);
        }
        static void FinishInitialLayout(ReactionsLab lab)
        {
            // Applied only during Create(), never used as an updater of authored scenes.
            var root=lab.transform;var reactor=lab.opening.parent;
            reactor.localPosition=new Vector3(0,1.2f,.5f);reactor.Find("Béquer · vidro").localScale=Vector3.one*4;lab.receiver.transform.localScale=Vector3.one*4;
            lab.opening.localPosition=new Vector3(0,.36f,0);lab.openingRadius=.225f;
            root.Find("Base do reator").localPosition=new Vector3(0,.42f,.5f);root.Find("Base do reator").localScale=new Vector3(.72f,.42f,.72f);root.Find("Aro azul").localPosition=new Vector3(0,.86f,.5f);
            reactor.Find("Fundo físico").localPosition=new Vector3(0,-.36f,0);reactor.Find("Fundo físico").localScale=new Vector3(.48f,.008f,.48f);
            foreach(Transform t in reactor)
            {
                if(t.name=="Parede física"){t.localPosition=new Vector3(t.localPosition.x*4/3,0,t.localPosition.z*4/3);t.localScale=new Vector3(.125f,.70f,.012f);}
                if(t.name.StartsWith("Marca ")||t.name.StartsWith("Volume "))t.localPosition*=4f/3;
                if(t.name.StartsWith("Volume "))Object.DestroyImmediate(t.GetComponent<Image>());
            }
            lab.controls.transform.localScale=Vector3.one*.0014f;lab.controls.transform.localPosition=new Vector3(-1.6f,1.65f,-.1f);
            lab.equationControls.transform.localScale=Vector3.one*.0014f;lab.equationControls.transform.localPosition=new Vector3(3.1f,1.8f,.7f);lab.equationControls.transform.localRotation=Quaternion.Euler(0,60,0);
            lab.modal.transform.SetPositionAndRotation(lab.controls.transform.position-Vector3.forward*.02f,lab.controls.transform.rotation);lab.modal.transform.localScale=Vector3.one*.0014f;
            lab.readings.transform.parent.localPosition=new Vector3(0,.55f,.05f);lab.readings.transform.parent.localScale=Vector3.one*.0012f;
            lab.hint.transform.parent.localPosition=new Vector3(0,.22f,-.03f);lab.hint.transform.parent.localScale=Vector3.one*.0012f;
            lab.molecularView.transform.localPosition=new Vector3(0,2.65f,3);
            foreach(var tool in lab.tools.Where(t=>!t.spoon))
            {
                tool.transform.Find("Vidraria Unity").localScale=Vector3.one*2.3f;tool.liquid.transform.localScale=Vector3.one*2.3f;tool.mouth.localPosition=new Vector3(0,.27f,0);
                var collider=tool.GetComponent<CapsuleCollider>();collider.height=.55f;collider.radius=.11f;
                tool.label.transform.parent.localScale=Vector3.one*.0008f;tool.label.transform.parent.localPosition=new Vector3(0,-.02f,-.122f);
            }
            lab.powderLabel.transform.parent.localScale=Vector3.one*.0014f;lab.powderLabel.transform.parent.localPosition=new Vector3(0,1.45f,.1f);
            lab.title.transform.parent.localPosition=new Vector3(0,3.6f,3);root.Find("Instrução").localPosition=new Vector3(0,3.27f,3);
            CalibrateGraduations(lab);
        }
        public static void CalibrateGraduations(ReactionsLab lab)
        {
            var reactor=lab.opening.parent;
            for(int i=1;i<=5;i++)
            {
                lab.receiver.fill=i*.18f;lab.receiver.Refresh(0);
                float y=lab.receiver.SurfaceY-reactor.position.y;
                var mark=reactor.Find("Marca "+i*100+" mL");var text=reactor.Find("Volume "+i*100);
                mark.localPosition=new Vector3(.07f,y,-.245f);text.localPosition=new Vector3(.13f,y,-.25f);
            }
            lab.receiver.fill=0;lab.receiver.Refresh(0);
        }
        static ReagentTool BottlePrefab(int index,Transform parent)
        {
            var go=new GameObject("Frasco "+(index==0?"A":"B"));go.SetActive(false);
            var tool=SetupTool(go,index,false);
            var shell=MeshObject(go.transform,"Vidraria Unity",bottleMesh,glass);shell.localScale=Vector3.one*2;
            tool.liquid=Liquid(go.transform,bottleLiquid,Vector3.one*2);tool.liquid.fill=.9f;
            tool.mouth=Child(go.transform,"Bico · saída");tool.mouth.localPosition=new Vector3(0,.235f,0);
            var collider=go.AddComponent<CapsuleCollider>();collider.radius=.095f;collider.height=.48f;tool.colliders.Add(collider);
            Cylinder(go.transform,"Aro de identificação",new Vector3(0,-.1f,0),new Vector3(.19f,.025f,.19f),blue,false);
            tool.label=Label(go.transform,"Reagente",index==0?"CH₃COOH\nÁcido acético diluído\n0,10 mol/L · 300 mL":"Segundo reagente",new Vector3(0,.025f,-.106f),new Vector2(380,190),25,false,null,.001f);
            return SaveTool(go,tool,parent);
        }
        static ReagentTool SpoonPrefab(Transform parent)
        {
            var go=new GameObject("Colher dosadora");go.SetActive(false);var tool=SetupTool(go,1,true);
            var vertices=new List<Vector3>();var triangles=new List<int>();
            // Closed bowl shell: concave inner profile, outer underside and rim are one mesh.
            Vector2[] profile={new Vector2(0,-.018f),new Vector2(.025f,-.014f),new Vector2(.05f,.005f),new Vector2(.065f,.027f),new Vector2(.069f,.027f),new Vector2(.055f,0),new Vector2(.028f,-.021f),new Vector2(0,-.024f)};
            for(int j=0;j<profile.Length;j++)for(int i=0;i<=32;i++){float a=i*Mathf.PI*2/32;vertices.Add(new Vector3(Mathf.Cos(a)*profile[j].x,profile[j].y,Mathf.Sin(a)*profile[j].x));}
            for(int j=0;j<profile.Length-1;j++)for(int i=0;i<32;i++){int a=j*33+i,b=a+33;triangles.AddRange(new[]{a,a+1,b,b,a+1,b+1});}
            var mesh=new Mesh{name="Colher · concha modelada"};mesh.SetVertices(vertices);mesh.SetTriangles(triangles,0);mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,Folder+"/Meshes/SpoonBowl.asset");
            var bowl=MeshObject(go.transform,"Concha",mesh,metal);bowl.localPosition=Vector3.forward*.18f;
            var handle=GameObject.CreatePrimitive(PrimitiveType.Capsule);handle.name="Cabo azul";handle.transform.SetParent(go.transform,false);handle.transform.localPosition=Vector3.forward*.015f;handle.transform.localRotation=Quaternion.Euler(90,0,0);handle.transform.localScale=new Vector3(.035f,.15f,.02f);handle.GetComponent<Renderer>().sharedMaterial=blue;Object.DestroyImmediate(handle.GetComponent<Collider>());
            var col=go.AddComponent<BoxCollider>();col.center=new Vector3(0,0,.07f);col.size=new Vector3(.14f,.055f,.38f);tool.colliders.Add(col);
            tool.mouth=Child(go.transform,"Concha · saída");tool.mouth.localPosition=new Vector3(0,.01f,.18f);
            tool.powderVisual=Cylinder(bowl,"Dose de sólido",Vector3.zero,new Vector3(.085f,.008f,.085f),white,false);
            tool.label=Label(go.transform,"Massa na colher","COLHER · 0 g",new Vector3(0,.1f,.02f),new Vector2(360,70),24,false,null,.001f);
            return SaveTool(go,tool,parent);
        }
        static ReagentTool SetupTool(GameObject go,int source,bool spoon)
        {
            var rb=go.AddComponent<Rigidbody>();rb.useGravity=false;rb.isKinematic=true;
            var tool=go.AddComponent<ReagentTool>();tool.source=source;tool.spoon=spoon;tool.throwOnDetach=false;tool.useDynamicAttach=true;
            tool.movementType=UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable.MovementType.Kinematic;
            tool.stream=Child(go.transform,"Fluxo contínuo").gameObject.AddComponent<LineRenderer>();tool.stream.useWorldSpace=true;tool.stream.positionCount=2;tool.stream.startWidth=tool.stream.endWidth=.009f;tool.stream.sharedMaterial=white;tool.stream.enabled=false;
            tool.droplets=Particles(go.transform,spoon?"Grãos em queda":"Gotas",spoon?Color.white:new Color(.55f,.8f,.93f),spoon?.01f:.012f,1.8f,1);
            var shape=tool.droplets.shape;shape.radius=.008f;var emission=tool.droplets.emission;emission.rateOverTime=spoon?70:35;
            return tool;
        }
        static ReagentTool SaveTool(GameObject go,ReagentTool tool,Transform parent)
        {
            go.SetActive(true);var prefab=PrefabUtility.SaveAsPrefabAsset(go,Folder+"/Prefabs/"+go.name+".prefab");Object.DestroyImmediate(go);
            return ((GameObject)PrefabUtility.InstantiatePrefab(prefab,parent)).GetComponent<ReagentTool>();
        }
        static LiquidPresentation Liquid(Transform parent,Mesh mesh,Vector3 scale)
        {
            var visual=MeshObject(parent,"Líquido · Shader Graph Unity",mesh,liquidMaterial);visual.localScale=scale;
            var liquid=visual.gameObject.AddComponent<LiquidPresentation>();liquid.surface=visual.GetComponent<Renderer>();liquid.localBounds=mesh.bounds;liquid.volumeSamples=VolumeSamples(mesh);return liquid;
        }
        static Vector3[] VolumeSamples(Mesh mesh)
        {
            // Deterministic volume quadrature. Ray parity works for either triangle winding.
            var vertices=mesh.vertices;var tris=mesh.triangles;var bounds=mesh.bounds;var samples=new List<Vector3>();
            Vector3 direction=new Vector3(1,.0173f,.0091f).normalized;
            for(int y=0;y<24;y++)for(int x=0;x<12;x++)for(int z=0;z<12;z++)
            {
                var p=bounds.min+Vector3.Scale(bounds.size,new Vector3((x+.5f)/12,(y+.5f)/24,(z+.5f)/12));int intersections=0;
                for(int i=0;i<tris.Length;i+=3)
                {
                    var a=vertices[tris[i]];var e1=vertices[tris[i+1]]-a;var e2=vertices[tris[i+2]]-a;var h=Vector3.Cross(direction,e2);float det=Vector3.Dot(e1,h);if(Mathf.Abs(det)<1e-9f)continue;
                    float inv=1/det;var s=p-a;float u=inv*Vector3.Dot(s,h);if(u<0||u>1)continue;var q=Vector3.Cross(s,e1);float v=inv*Vector3.Dot(direction,q);if(v<0||u+v>1)continue;if(inv*Vector3.Dot(e2,q)>1e-7f)intersections++;
                }
                if((intersections&1)==1)samples.Add(p);
            }
            if(samples.Count<100)throw new Exception("Malha de líquido não fechada: "+mesh.name);
            return samples.ToArray();
        }
        static void CreateControls(Transform root,ReactionsLab lab)
        {
            var controls=Panel(root,"Escolha e operação",new Vector3(-1.85f,1.65f,-.15f),new Vector2(930,1060),true);controls.localRotation=Quaternion.Euler(0,-25,0);Layout(controls);lab.controls=controls.gameObject.AddComponent<CanvasGroup>();
            RowText(controls,"ESCOLHA UMA EXPERIÊNCIA",32,true,90);lab.cases=ReactionDefinition.All.Select(d=>Button(controls,d.Title,100)).ToArray();
            lab.molecularToggle=Toggle(controls,"Ver composição e quantidades",true);
            var row=Row(controls,90);lab.pause=Button(row,"Pausar",90);lab.reset=Button(row,"Reiniciar",90);
            lab.recover=Button(controls,"Recolher instrumentos soltos",90);lab.back=Button(controls,"Voltar ao Hub",90);
            RowText(controls,"Pegue com o grip; incline para dosar.\nInstrumentos soltos retornam após 10 s.\nPausa mantém a navegação disponível.",27,false,170);
            var modal=Panel(root,"Confirmar reinício",new Vector3(-1.85f,1.65f,-.18f),new Vector2(930,700),true);modal.localRotation=controls.localRotation;Layout(modal);lab.modal=modal.gameObject;
            lab.modalText=RowText(modal,"",34,true,350);lab.confirm=Button(modal,"Descartar e reiniciar",100);lab.cancel=Button(modal,"Continuar experiência",100);modal.gameObject.SetActive(false);
        }
        static void CreateMolecularView(Transform root,ReactionsLab lab)
        {
            var view=Child(root,"Visor de composição");view.localPosition=new Vector3(0,2.5f,2.9f);lab.molecularView=view.gameObject;
            Label(view,"Legenda molecular","COMPOSIÇÃO · modelos simbólicos, não geometria molecular\nH branco · C grafite · O vermelho · Na violeta · Cl verde · Ca azul",new Vector3(0,.3f,0),new Vector2(2200,150),27,false);
            lab.atomGroups=new Transform[5];lab.speciesLabels=new TMP_Text[5];lab.amountLabels=new TMP_Text[5];
            lab.atomMaterials=new[]{Material("Átomo H",Color.white),Material("Átomo C",new Color(.16f,.18f,.22f)),Material("Átomo O",new Color(.8f,.12f,.12f)),Material("Átomo Na",new Color(.6f,.28f,.8f)),Material("Átomo Cl",new Color(.2f,.65f,.25f)),Material("Átomo Ca",new Color(.25f,.5f,.85f))};
            var sphere=GameObject.CreatePrimitive(PrimitiveType.Sphere);lab.atomMesh=sphere.GetComponent<MeshFilter>().sharedMesh;Object.DestroyImmediate(sphere);
            for(int i=0;i<5;i++)
            {
                var card=Child(view,"Espécie "+i);card.localPosition=new Vector3((i-2)*.77f,-.28f,0);
                lab.speciesLabels[i]=Label(card,"Espécie","",new Vector3(0,.12f,0),new Vector2(370,130),25,true);
                lab.atomGroups[i]=Child(card,"Átomos simbólicos");lab.atomGroups[i].localPosition=new Vector3(0,-.13f,-.1f);
                lab.amountLabels[i]=Label(card,"Quantidade","",new Vector3(0,-.35f,0),new Vector2(370,100),26,false);
            }
            Label(view,"Limite do modelo","Quantidades em mmol · água formada não inclui o solvente.\nSais em solução estão dissociados; esferas representam a composição.\nCO₂ liberado continua contabilizado. Efeitos não representam cinética real.",new Vector3(0,-.92f,0),new Vector2(2200,190),25,false);
            var panel=Panel(root,"Balanceamento",new Vector3(2.65f,1.9f,.5f),new Vector2(1130,1150),true);panel.localRotation=Quaternion.Euler(0,35,0);Layout(panel);lab.equationControls=panel.gameObject.AddComponent<CanvasGroup>();
            RowText(panel,"BALANCEIE A EQUAÇÃO",36,true,90);RowText(panel,"Primeiras 2 linhas: reagentes → demais: produtos.\nAjustar números não altera a mistura dosada.",26,false,110);
            lab.coefficientLabels=new TMP_Text[5];lab.minus=new Button[5];lab.plus=new Button[5];
            for(int i=0;i<5;i++)
            {
                var row=Row(panel,90);lab.minus[i]=Button(row,"−",90);lab.minus[i].GetComponent<LayoutElement>().flexibleWidth=.3f;
                lab.coefficientLabels[i]=RowText(row,"1",30,true,90);lab.coefficientLabels[i].GetComponent<LayoutElement>().flexibleWidth=2;
                lab.plus[i]=Button(row,"+",90);lab.plus[i].GetComponent<LayoutElement>().flexibleWidth=.3f;
            }
            lab.balance=RowText(panel,"",28,false,300);
        }
        static Mesh MeshAt(string path,string name)=>AssetDatabase.LoadAllAssetsAtPath(Sample+path).OfType<Mesh>().Single(m=>m.name==name);
        static void Ensure(string path){if(AssetDatabase.IsValidFolder(path))return;int i=path.LastIndexOf('/');Ensure(path[..i]);AssetDatabase.CreateFolder(path[..i],path[(i+1)..]);}
        static Transform Child(Transform p,string name){var t=new GameObject(name).transform;t.SetParent(p,false);return t;}
        static Transform MeshObject(Transform p,string name,Mesh mesh,Material mat){var t=Child(p,name);t.gameObject.AddComponent<MeshFilter>().sharedMesh=mesh;t.gameObject.AddComponent<MeshRenderer>().sharedMaterial=mat;return t;}
        static Material Material(string name,Color color){var m=new Material(Shader.Find("Universal Render Pipeline/Lit")){name=name};m.SetColor("_BaseColor",color);m.SetFloat("_Smoothness",.35f);AssetDatabase.CreateAsset(m,Folder+"/Materials/"+name+".mat");return m;}
        static Transform Primitive(PrimitiveType type,Transform p,string name,Vector3 position,Vector3 size,Material mat,bool collider)
        {var go=GameObject.CreatePrimitive(type);go.name=name;go.transform.SetParent(p,false);go.transform.localPosition=position;go.transform.localScale=size;if(mat)go.GetComponent<Renderer>().sharedMaterial=mat;else Object.DestroyImmediate(go.GetComponent<Renderer>());if(!collider)Object.DestroyImmediate(go.GetComponent<Collider>());return go.transform;}
        static Transform Cylinder(Transform p,string n,Vector3 pos,Vector3 size,Material m,bool c)=>Primitive(PrimitiveType.Cylinder,p,n,pos,size,m,c);
        static Transform Box(Transform p,string n,Vector3 pos,Vector3 size,Material m,bool c)=>Primitive(PrimitiveType.Cube,p,n,pos,size,m,c);
        static ParticleSystem Particles(Transform parent,string name,Color color,float size,float speed,float life)
        {
            var ps=Child(parent,name).gameObject.AddComponent<ParticleSystem>();ps.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
            var main=ps.main;main.playOnAwake=false;main.loop=true;main.maxParticles=180;main.startLifetime=life;main.startSize=size;main.startSpeed=speed;main.startColor=color;main.simulationSpace=ParticleSystemSimulationSpace.World;
            var emission=ps.emission;emission.enabled=false;emission.rateOverTime=0;var shape=ps.shape;shape.shapeType=ParticleSystemShapeType.Cone;shape.angle=0;shape.radius=.10f;
            var renderer=ps.GetComponent<ParticleSystemRenderer>();renderer.sharedMaterial=particleMaterial;renderer.renderMode=ParticleSystemRenderMode.Billboard;return ps;
        }
        static RectTransform Rect(Transform p,string name){var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(p,false);return r;}
        static RectTransform Panel(Transform p,string name,Vector3 pos,Vector2 size,bool interactive,float scale=.002f)
        {var r=Rect(p,name);r.localPosition=pos;r.localScale=Vector3.one*scale;r.sizeDelta=size;r.gameObject.AddComponent<Canvas>().renderMode=RenderMode.WorldSpace;var image=r.gameObject.AddComponent<Image>();image.color=Lavender;image.raycastTarget=interactive;if(interactive)r.gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();return r;}
        static TMP_Text Configure(RectTransform r,string text,float size,bool strong,Color? color=null)
        {var t=r.gameObject.AddComponent<TextMeshProUGUI>();t.font=strong?bold:regular;t.text=text;t.fontSize=size;t.color=color??(strong?Blue:new Color(.08f,.08f,.08f));t.alignment=TextAlignmentOptions.Center;t.raycastTarget=false;t.overflowMode=TextOverflowModes.Ellipsis;return t;}
        static TMP_Text Label(Transform p,string name,string text,Vector3 pos,Vector2 size,float font,bool strong,Color? color=null,float scale=.002f)
        {var panel=Panel(p,name,pos,size,false,scale);var r=Rect(panel,"Texto");r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=new Vector2(8,4);r.offsetMax=new Vector2(-8,-4);return Configure(r,text,font,strong,color);}
        static void Height(GameObject go,float h){var e=go.GetComponent<LayoutElement>()??go.AddComponent<LayoutElement>();e.minHeight=e.preferredHeight=h;e.flexibleHeight=0;e.flexibleWidth=1;}
        static void Layout(RectTransform panel){var g=panel.gameObject.AddComponent<VerticalLayoutGroup>();g.padding=new RectOffset(24,24,24,24);g.spacing=12;g.childControlWidth=g.childControlHeight=true;g.childForceExpandWidth=true;g.childForceExpandHeight=false;}
        static RectTransform Row(Transform p,float h){var r=Rect(p,"Linha");Height(r.gameObject,h);var g=r.gameObject.AddComponent<HorizontalLayoutGroup>();g.spacing=12;g.childControlWidth=g.childControlHeight=true;g.childForceExpandWidth=true;g.childForceExpandHeight=false;return r;}
        static TMP_Text RowText(Transform p,string s,float size,bool boldText,float h){var r=Rect(p,"Texto");Height(r.gameObject,h);return Configure(r,s,size,boldText);}
        static Button Button(Transform p,string s,float h){var b=Object.Instantiate(buttonTemplate,p);b.name=s;b.onClick=new Button.ButtonClickedEvent();Height(b.gameObject,h);var t=b.GetComponentInChildren<TMP_Text>();t.text=s;t.fontSize=30;return b;}
        static Toggle Toggle(Transform p,string s,bool value){var t=Object.Instantiate(toggleTemplate,p);t.name=s;t.group=null;t.onValueChanged=new Toggle.ToggleEvent();t.SetIsOnWithoutNotify(value);Height(t.gameObject,90);t.GetComponentInChildren<TMP_Text>().text=s;t.GetComponentInChildren<TMP_Text>().fontSize=29;FractionsAuthoring.StyleToggle(t);return t;}

        /// <summary>One-time, scoped migration of existing objects. Never recreates room, rig or liquid assets.</summary>
        public static void UpgradeToBench()
        {
            var scene=SceneManager.GetActiveScene();
            if(EditorApplication.isPlayingOrWillChangePlaymode||scene.isDirty||scene.path!=ScenePath)throw new Exception("Abra Reactions, fora de Play, com a cena salva.");
            var lab=Object.FindFirstObjectByType<ReactionsLab>();if(!lab||lab.bench)throw new Exception("Sala ausente ou já migrada; faça ajustes pontuais.");
            System.IO.Directory.CreateDirectory("Temp/ReactionsMigration");System.IO.File.Copy(ScenePath,"Temp/ReactionsMigration/Reactions-before-bench.unity",true);
            regular=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Regular SDF.asset");
            bold=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Bold SDF.asset");
            buttonTemplate=lab.back;
            blue=AssetDatabase.LoadAssetAtPath<Material>(Folder+"/Materials/Azul.mat");dark=AssetDatabase.LoadAssetAtPath<Material>(Folder+"/Materials/Grafite.mat");white=AssetDatabase.LoadAssetAtPath<Material>(Folder+"/Materials/Sólido branco.mat");
            var black=AssetDatabase.LoadAssetAtPath<Material>(Folder+"/Materials/Sólido preto.mat");if(!black)black=Material("Sólido preto",new Color(.04f,.04f,.04f));
            var root=lab.transform;var group=Child(root,"Mesa de reagentes · seleção A B");var bench=group.gameObject.AddComponent<ReactionsBench>();lab.bench=bench;
            var oldTools=lab.tools.ToArray();var instruments=new List<ReagentTool>();
            Transform Table(string name,Vector3 position,Vector3 size)
            {
                var mesh=UnityEngine.ProBuilder.ShapeGenerator.GenerateCube(UnityEngine.ProBuilder.PivotLocation.Center,size);mesh.name=name;mesh.transform.SetParent(group,false);mesh.transform.localPosition=position;mesh.GetComponent<Renderer>().sharedMaterial=dark;
                if(!mesh.GetComponent<Collider>())mesh.gameObject.AddComponent<BoxCollider>();return mesh.transform;
            }
            Table("Tampo esquerdo",new Vector3(-1.38f,.88f,1),new Vector3(.65f,.08f,2.5f));
            Table("Tampo direito",new Vector3(1.38f,.88f,1),new Vector3(.65f,.08f,2.5f));
            Table("Tampo ao fundo",new Vector3(0,.88f,2.1f),new Vector3(3.4f,.08f,.65f));
            foreach(float x in new[]{-1.45f,1.45f})foreach(float z in new[]{0f,2.1f})Table("Pé da mesa",new Vector3(x,.44f,z),new Vector3(.10f,.88f,.10f));
            for(int i=0;i<2;i++)Table("Apoio do suporte "+i,new Vector3(i==0?-.63f:.63f,.88f,.16f),new Vector3(.6f,.08f,.52f));
            string[] groups={"ÁCIDOS","BASES","SAIS","ÓXIDOS"};
            Vector3[] headings={new Vector3(-1.38f,1.82f,.70f),new Vector3(-.77f,1.82f,2.15f),new Vector3(.77f,1.82f,2.15f),new Vector3(1.38f,1.82f,.70f)};
            for(int g=0;g<4;g++)Label(group,"Grupo "+groups[g],groups[g]+" · 3",headings[g],new Vector2(550,90),38,true,null,.001f);
            for(int i=0;i<ReactionWorkbench.Reagents.Length;i++)
            {
                var reagent=ReactionWorkbench.Reagents[i];int g=i/3,k=i%3;
                Vector3 position=g==0?new Vector3(-1.38f,1.20f,.05f+k*.57f):g==1?new Vector3(-1.15f+k*.46f,1.20f,2.1f):g==2?new Vector3(.23f+k*.46f,1.20f,2.1f):new Vector3(1.38f,1.20f,.05f+k*.57f);
                var tool=Object.Instantiate(oldTools[0],group);tool.name=reagent.Formula+" · "+reagent.Name;tool.reagentId=reagent.Id;tool.lab=lab;tool.solidPot=reagent.Solid;tool.source=0;tool.spoon=false;
                tool.home=Child(group,"Retorno · "+reagent.Id);tool.home.localPosition=position;tool.transform.SetPositionAndRotation(tool.home.position,tool.home.rotation);
                var panel=(RectTransform)tool.label.transform.parent;panel.sizeDelta=new Vector2(420,230);panel.localScale=Vector3.one*.0008f;tool.label.fontSize=25;
                tool.label.text=reagent.Formula+"\n"+reagent.Name+"\n"+(reagent.Solid?"Sólido · 6 g":"0,10 mol/L · 300 mL");
                if(reagent.Solid)
                {
                    tool.liquid.gameObject.SetActive(false);
                    tool.powderVisual=Cylinder(tool.transform,"Estoque sólido",new Vector3(0,-.12f,0),new Vector3(.16f,.05f,.16f),reagent.Id=="cuo"?black:white,false);
                    tool.collectionPoint=Child(tool.transform,"Zona de coleta");tool.collectionPoint.localPosition=new Vector3(0,.28f,0);
                    var spoon=Object.Instantiate(oldTools[2],group);spoon.name="Colher · "+reagent.Formula;spoon.reagentId=reagent.Id;spoon.lab=lab;
                    spoon.home=Child(group,"Retorno colher · "+reagent.Id);spoon.home.localPosition=position+new Vector3(g==3?-.20f:.20f,-.25f,-.10f);spoon.transform.SetPositionAndRotation(spoon.home.position,spoon.home.rotation);
                    spoon.label.text=reagent.Formula+" · COLHER\n0,00 g";spoon.label.fontSize=23;((RectTransform)spoon.label.transform.parent).sizeDelta=new Vector2(400,100);
                    spoon.powderVisual.GetComponent<Renderer>().sharedMaterial=reagent.Id=="cuo"?black:white;spoon.powderVisual.gameObject.SetActive(false);
                    var main=spoon.droplets.main;main.startColor=reagent.Id=="cuo"?Color.black:Color.white;instruments.Add(spoon);
                }
                tool.StopEffects();instruments.Add(tool);
            }
            var indicator=Object.Instantiate(oldTools[0],group);indicator.name="Conta-gotas · indicador";indicator.reagentId="indicator";indicator.indicator=true;indicator.lab=lab;
            indicator.home=Child(group,"Retorno indicador");indicator.home.localPosition=new Vector3(.97f,1.12f,1.52f);indicator.transform.localScale=Vector3.one*.65f;indicator.transform.SetPositionAndRotation(indicator.home.position,indicator.home.rotation);
            indicator.label.text="INDICADOR\nFenolftaleína · traço\nHCl + NaOH\nAtivar sobre o béquer";indicator.liquid.fill=.55f;instruments.Add(indicator);
            // A nozzle distinguishes the auxiliary dropper without changing the sampled glass/liquid meshes.
            var nozzle=Cylinder(indicator.transform,"Bico do conta-gotas",new Vector3(0,.30f,0),new Vector3(.035f,.045f,.035f),blue,false);indicator.mouth.localPosition=new Vector3(0,.35f,0);
            bench.instruments=instruments.ToArray();lab.tools=bench.instruments;
            bench.sockets=new ReagentSocket[2];bench.socketLabels=new TMP_Text[2];
            for(int i=0;i<2;i++)
            {
                var dock=Child(group,"Suporte "+(i==0?"A":"B"));dock.localPosition=new Vector3(i==0?-.63f:.63f,1.20f,.16f);
                Cylinder(dock,"Base do encaixe",new Vector3(0,-.25f,0),new Vector3(.32f,.025f,.32f),blue,true);
                var trigger=dock.gameObject.AddComponent<BoxCollider>();trigger.isTrigger=true;trigger.size=new Vector3(.33f,.5f,.33f);
                var socket=dock.gameObject.AddComponent<ReagentSocket>();socket.bench=bench;socket.slot=i;socket.showInteractableHoverMeshes=false;
                socket.attachTransform=Child(dock,"Encaixe");socket.recycleDelayTime=.1f;bench.sockets[i]=socket;
                bench.socketLabels[i]=Label(dock,"Identificação",(i==0?"A":"B")+"\nEncaixe um reagente",new Vector3(0,-.12f,-.22f),new Vector2(420,150),30,true,null,.001f);
            }
            var commands=Panel(group,"Comandos essenciais",new Vector3(0,1.04f,-.55f),new Vector2(1150,130),true,.001f);Layout(commands);bench.commandGroup=commands.gameObject.AddComponent<CanvasGroup>();
            var commandRow=Row(commands,80);bench.consult=Button(commandRow,"Consultar",80);bench.reset=Button(commandRow,"Trocar par",80);bench.back=Button(commandRow,"Voltar ao Hub",80);
            foreach(var b in new[]{bench.consult,bench.reset,bench.back})b.GetComponentInChildren<TMP_Text>().fontSize=25;
            bench.status=Label(group,"Situação local","Escolha A, depois B.\nEncaixe os recipientes nos suportes.",new Vector3(-.85f,1.68f,.35f),new Vector2(750,220),27,false,null,.001f);
            var sheet=Panel(group,"Ficha opcional",new Vector3(2.25f,1.90f,.4f),new Vector2(950,1150),true,.001f);sheet.localRotation=Quaternion.Euler(0,35,0);Layout(sheet);bench.sheet=sheet.gameObject;
            bench.details=RowText(sheet,"Consulte o par escolhido.",27,false,950);bench.details.alignment=TextAlignmentOptions.TopLeft;
            bench.close=Button(sheet,"Fechar ficha",85);
            var modal=Panel(group,"Confirmar nova mistura",sheet.localPosition-Vector3.forward*.01f,new Vector2(950,620),true,.001f);modal.localRotation=sheet.localRotation;Layout(modal);bench.confirmation=modal.gameObject;
            bench.confirmationText=RowText(modal,"Iniciar nova mistura?",34,true,340);var actions=Row(modal,100);bench.cancel=Button(actions,"Cancelar",100);bench.confirm=Button(actions,"Confirmar reinício",100);
            foreach(var b in new[]{bench.cancel,bench.confirm})b.GetComponentInChildren<TMP_Text>().fontSize=26;
            Tecaverso.UI.UIVisibility.Set(sheet.gameObject,false);Tecaverso.UI.UIVisibility.Set(modal.gameObject,false);
            // Remove the obsolete presentation only, retaining the existing reactor, rendering, rig and environment.
            var obsolete=new[]{lab.controls.gameObject,lab.equationControls.gameObject,lab.molecularView,lab.modal,lab.readings.transform.parent.gameObject,lab.hint.transform.parent.gameObject,lab.powderStation};
            foreach(var obj in obsolete)Object.DestroyImmediate(obj);
            foreach(var tool in oldTools){if(tool.home)Object.DestroyImmediate(tool.home.gameObject);Object.DestroyImmediate(tool.gameObject);}
            foreach(string name in new[]{"Suporte frasco 0","Suporte frasco 1","Bandeja retorno 0","Bandeja retorno 1"}){var t=root.Find(name);if(t)Object.DestroyImmediate(t.gameObject);}
            lab.title.text="REAÇÕES QUÍMICAS";root.Find("Instrução").GetComponentInChildren<TMP_Text>().text="Escolha dois reagentes nos suportes A e B. Depois, misture e observe.\n12 reagentes · 12 receitas · exploração individual";
            foreach(var selectable in group.GetComponentsInChildren<Selectable>(true))Tecaverso.Interaction.InteractionMotionBootstrap.Ensure(selectable);
            foreach(var tool in instruments)Tecaverso.Interaction.InteractionMotionBootstrap.Ensure(tool);
            EditorUtility.SetDirty(lab);Canvas.ForceUpdateCanvases();EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
        }
    }
}
