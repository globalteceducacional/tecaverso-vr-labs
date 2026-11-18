using System.Collections.Generic;
using Tecaverso.Interaction;
using Tecaverso.Labs.Biology.Ecosystems;
using Tecaverso.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Object=UnityEngine.Object;

namespace Tecaverso.Editor
{
    public static class EcosystemsAuthoring
    {
        public const string Folder="Assets/_Project/Laboratories/Biology/Ecosystems";
        public const string ScenePath=Folder+"/Scenes/Ecosystems.unity";
        const float Surface=1.08f;
        static readonly float[] X={-.71f,-.015f,.66f};
        static Vector3 V(float x,float y,float z)=>new(x,y,z);
        static Material graphite, basalt, loam, moss, trim, porcelain, specimen, gold, aqua, heatInk;
        static Transform Child(Transform parent,string name,Vector3 p)
        {
            var t=new GameObject(name).transform; t.SetParent(parent,false); t.localPosition=p; return t;
        }
        static Material Mat(string name,int rgb,string shader="Universal Render Pipeline/Lit")
        {
            string path=Folder+"/Art/"+name+".mat"; var m=AssetDatabase.LoadAssetAtPath<Material>(path);
            if(m)return m;
            m=new Material(Shader.Find(shader)){name=name};
            var c=new Color32((byte)(rgb>>16),(byte)(rgb>>8),(byte)rgb,255);
            if(m.HasProperty("_BaseColor"))m.SetColor("_BaseColor",c);
            if(m.HasProperty("_Tint"))m.SetColor("_Tint",c);
            if(m.HasProperty("_Smoothness"))m.SetFloat("_Smoothness",.21f);
            AssetDatabase.CreateAsset(m,path);return m;
        }
        static GameObject Primitive(Transform root,string name,PrimitiveType type,Vector3 at,Vector3 size,Material material)
        {
            var obj=GameObject.CreatePrimitive(type);obj.name=name;obj.transform.SetParent(root,false);
            obj.transform.localPosition=at;obj.transform.localScale=size;
            obj.GetComponent<MeshRenderer>().sharedMaterial=material;
            Object.DestroyImmediate(obj.GetComponent<Collider>());return obj;
        }
        static Renderer Shape(Transform root,string name,Vector3 at,Mesh mesh,Material m)
        {
            Transform part=Child(root,name,at);
            part.gameObject.AddComponent<MeshFilter>().sharedMesh=mesh;
            var r=part.gameObject.AddComponent<MeshRenderer>();r.sharedMaterial=m;return r;
        }
        static Mesh SaveMesh(string name,List<Vector3> v,List<int> tris,List<Vector2> uv=null)
        {
            string path=Folder+"/Art/"+name+".asset";
            var saved=AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if(saved)return saved;
            var mesh=new Mesh{name=name};mesh.SetVertices(v);mesh.SetTriangles(tris,0);
            if(uv!=null)mesh.SetUVs(0,uv);mesh.RecalculateNormals();mesh.RecalculateBounds();
            AssetDatabase.CreateAsset(mesh,path);return mesh;
        }
        static Mesh Slab()
        {
            var vs=new List<Vector3>();var tris=new List<int>();
            var profile=new List<Vector2>{new(-.46f,-.5f),new(.46f,-.5f),new(.5f,-.42f),new(.5f,.42f),new(.46f,.5f),new(-.46f,.5f),new(-.5f,.42f),new(-.5f,-.42f)};
            for(int i=0;i<8;i++)
            {
                var a=profile[i];var b=profile[(i+1)%8];int k=vs.Count;
                vs.Add(V(a.x,-.5f,a.y));vs.Add(V(a.x,.5f,a.y));vs.Add(V(b.x,.5f,b.y));vs.Add(V(b.x,-.5f,b.y));
                tris.AddRange(new[]{k,k+1,k+2,k,k+2,k+3});
                k=vs.Count;vs.Add(Vector3.up*.5f);vs.Add(V(b.x,.5f,b.y));vs.Add(V(a.x,.5f,a.y));
                tris.AddRange(new[]{k,k+1,k+2});
            }
            return SaveMesh("Perfil da bancada",vs,tris);
        }
        static void Slab(Transform root,string name,Vector3 p,Vector3 s,Material m)
        {
            Shape(root,name,p,Slab(),m).transform.localScale=s;
        }
        static void Label(Transform root,string title,Vector3 p,float width,float size=27,float pitch=0)
        {
            var t=BiologyWorldUI.Caption(root,title,p,new Vector2(width/.0015f,80),size);
            t.transform.parent.localRotation=Quaternion.Euler(pitch,0,0);
        }

        [MenuItem("Tecaverso/Biologia/Criar Ecossistemas e relações")]
        public static void Create()
        {
            var scene=BiologyRoomAuthoring.Begin(ScenePath);
            graphite=Mat("Basalto",0x303e54);basalt=Mat("Xisto",0x515c68);loam=Mat("Húmus",0x83604a);moss=Mat("Prado",0x526e59);
            trim=Mat("Aro de latão",0xc5ae77);porcelain=Mat("Porcelana verde",0x9cb3a1);
            specimen=Mat("Espécimes pintados",0xffffff,"Tecaverso/Biology/FieldSpecimens");
            gold=Mat("Energia solar",0xfbd77c,"Tecaverso/Biology/TrophicRibbon");
            heatInk=Mat("Calor",0xf7ac85,"Tecaverso/Biology/TrophicRibbon");
            aqua=Mat("Matéria",0x68e3cf,"Tecaverso/Biology/TrophicRibbon");
            var root=new GameObject("Ecossistemas · pradaria terrestre");root.SetActive(false);
            var lab=root.AddComponent<EcosystemsLab>();var t=root.transform;
            Slab(t,"Coluna recuada",V(0,.46f,.26f),V(1.7f,.92f,.69f),graphite);
            Slab(t,"Estrutura do diorama",V(0,.925f,.25f),V(2.67f,.105f,1.65f),basalt);
            Slab(t,"Horizonte mineral",V(0,1.004f,.51f),V(2.61f,.055f,1.07f),loam);
            Slab(t,"Camada orgânica",V(0,1.045f,.51f),V(2.60f,.027f,1.07f),graphite);
            Slab(t,"Prado",V(0,1.069f,.51f),V(2.59f,.022f,1.07f),moss);
            Shape(t,"Margem do campo",V(0,Surface,0),EcosystemsSpecimens.Get("Margem do campo",4),specimen);
            Label(t,"Ecossistemas e relações",V(0,1.94f,1.25f),2.4f,53);
            Label(t,"CADEIA ALIMENTAR TERRESTRE",V(0,1.81f,1.25f),1.7f,25);
            BuildChain(lab);BuildFlows(lab);Interface(lab);RefineCaptions(t);
            root.SetActive(true);
            if(!EditorSceneManager.SaveScene(scene))throw new System.IO.IOException("Falha ao salvar Ecossistemas.");
            AssetDatabase.SaveAssets();
        }
        static void RefineCaptions(Transform t)
        {
            void Caption(string original,string text,float y,float size)
            {
                var label=t.Find(original).GetComponentInChildren<TMPro.TMP_Text>();
                label.text=text;label.fontSize=size;
                var rect=(RectTransform)label.transform.parent;
                var p=rect.anchoredPosition3D;p.y=y;rect.anchoredPosition3D=p;
            }
            Caption("CADEIA ALIMENTAR TERRESTRE","CADEIA TERRESTRE · ESCALAS DIDÁTICAS",1.81f,26);
            Caption("Energia → alimento     ·     Calor → ambiente","Energia na alimentação · calor dissipado",1.62f,27);
            Caption("Matéria → solo → produtor","Nutrientes voltam ao produtor",1.62f,26);
            Caption("Decompositores","Decompositores",1.51f,26);
            Caption("Consultar","Consultar",1.45f,21);
            foreach(string name in new[]{"Decompositores","Consultar"})
            {
                var rect=(RectTransform)t.Find(name);
                var p=rect.anchoredPosition3D;p.x=1.22f;rect.anchoredPosition3D=p;
            }
        }
        public static void RefinePresentation()
        {
            if(Application.isPlaying)throw new System.InvalidOperationException("Ajuste em Edit Mode.");
            var scene=UnityEngine.SceneManagement.SceneManager.GetSceneByPath(ScenePath);
            bool opened=!scene.isLoaded;
            if(opened)scene=EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Additive);
            try
            {
                foreach(var root in scene.GetRootGameObjects())
                    if(root.TryGetComponent<EcosystemsLab>(out var lab))RefineCaptions(lab.transform);
                EcosystemsSpecimens.Get("Margem do campo",4,true);
                EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
            }
            finally { if(opened)EditorSceneManager.CloseScene(scene,true); }
        }
        static void BuildChain(EcosystemsLab lab)
        {
            var t=lab.transform;
            lab.tokens=new OrganismToken[3];lab.sockets=new TrophicSocket[3];
            string[] roleTitles={"Produtor","Herbívoro","Predador"};
            for(int i=0;i<3;i++)
            {
                var slot=Child(t,"Encaixe · "+roleTitles[i],V(X[i],Surface+.010f,.26f));
                Primitive(slot,"Borda do receptáculo",PrimitiveType.Cylinder,V(0,.003f,0),V(.34f,.003f,.34f),trim);
                Primitive(slot,"Berço",PrimitiveType.Cylinder,V(0,.009f,0),V(.313f,.0035f,.313f),graphite);
                var snap=Child(slot,"Encaixe da ficha",V(0,.013f,0));
                var socket=slot.gameObject.AddComponent<TrophicSocket>();socket.attachTransform=snap;socket.showInteractableHoverMeshes=false;
                var trigger=slot.gameObject.AddComponent<SphereCollider>();trigger.isTrigger=true;trigger.radius=.16f;
                lab.sockets[i]=socket;
                var title=Primitive(t,"Placa · "+roleTitles[i],PrimitiveType.Cube,V(X[i],1.43f,.51f),V(.51f,.075f,.025f),graphite);
                Label(t,roleTitles[i],title.transform.position+Vector3.back*.015f,.49f,27);
                // Display the organisms in a different order from their trophic roles.
                var home=Child(t,"Bandeja · "+EcosystemsLab.Names[i],V(X[(i+1)%3],.995f,-.31f));
                Primitive(home,"Base de exposição",PrimitiveType.Cylinder,V(0,-.011f,0),V(.323f,.007f,.323f),porcelain);
                Label(t,EcosystemsLab.Names[i],V(home.position.x,.994f,-.506f),.55f,25,25);
                var piece=Child(t,EcosystemsLab.Names[i],home.position);
                Primitive(piece,"Ficha",PrimitiveType.Cylinder,V(0,.009f,0),V(.282f,.009f,.282f),loam);
                var model=Shape(piece,"Espécime · "+EcosystemsLab.Names[i],V(0,.018f,0),EcosystemsSpecimens.Get(EcosystemsLab.Names[i],i),specimen);
                if(i==1 || i==2)model.transform.localRotation=Quaternion.Euler(0,20,0);
                var body=piece.gameObject.AddComponent<Rigidbody>();body.useGravity=false;body.isKinematic=true;
                var collider=piece.gameObject.AddComponent<BoxCollider>();collider.center=V(0,.111f,0);collider.size=V(.31f,.22f,.24f);
                var token=piece.gameObject.AddComponent<OrganismToken>();token.lab=lab;token.home=home;token.role=i;
                token.throwOnDetach=false;token.movementType=XRGrabInteractable.MovementType.Kinematic;token.useDynamicAttach=true;
                var feedback=piece.gameObject.AddComponent<XRInteractionMotion>();feedback.visualTargets=new[]{model};
                lab.tokens[i]=token;socket.target=token;
            }
            var mushroom=Child(t,"Decompositores · consultar",V(1.02f,Surface+.003f,.68f));
            var relief=Shape(mushroom,"Fungos em madeira morta",Vector3.zero,EcosystemsSpecimens.Get("Fungos sapróbios",3),specimen);
            var col=mushroom.gameObject.AddComponent<BoxCollider>();col.center=V(0,.085f,0);col.size=V(.30f,.18f,.22f);
            lab.decomposers=mushroom.gameObject.AddComponent<XRSimpleInteractable>();
            mushroom.gameObject.AddComponent<XRInteractionMotion>().visualTargets=new[]{relief};
            Label(t,"Decompositores",V(1.04f,1.33f,.73f),.50f,22);
            Label(t,"Consultar",V(1.04f,1.28f,.73f),.46f,18);
            Primitive(t,"Sol · disco",PrimitiveType.Cylinder,V(-1.155f,Surface+.015f,.29f),V(.115f,.008f,.115f),trim);
            for(int i=0;i<10;i++)
            {
                float angle=Mathf.PI*2*i/10;
                var ray=Primitive(t,"Raio solar",PrimitiveType.Cube,V(-1.155f+.085f*Mathf.Sin(angle),Surface+.013f,.29f+.085f*Mathf.Cos(angle)),V(.013f,.008f,.028f),trim);
                ray.transform.localRotation=Quaternion.Euler(0,angle*Mathf.Rad2Deg,0);
            }
            Label(t,"Luz solar",V(-1.16f,1.25f,.42f),.31f,21);
        }
        // Curved, surface-mounted ribbons. Their UV distance is in metres for uniform motion.
        static Renderer Ribbon(Transform root,string name,Material material,float width,params Vector2[] knots)
        {
            var vs=new List<Vector3>();var uv=new List<Vector2>();var tris=new List<int>();
            var points=new List<Vector2>();
            for(int i=0;i<knots.Length-1;i++)
            {
                var p0=knots[Mathf.Max(0,i-1)];var p1=knots[i];var p2=knots[i+1];var p3=knots[Mathf.Min(knots.Length-1,i+2)];
                for(int j=0;j<10;j++)
                {
                    float u=j/10f;
                    points.Add(.5f*((2*p1)+(-p0+p2)*u+(2*p0-5*p1+4*p2-p3)*u*u+(-p0+3*p1-3*p2+p3)*u*u*u));
                }
            }
            points.Add(knots[^1]);
            float travel=0;
            for(int j=0;j<points.Count;j++)
            {
                var p=points[j];var dir=(points[Mathf.Min(j+1,points.Count-1)]-points[Mathf.Max(0,j-1)]).normalized;
                Vector2 side=new Vector2(-dir.y,dir.x)*width*.5f;
                void Add(Vector2 v,float across){vs.Add(V(v.x,0,v.y));uv.Add(new Vector2(travel,across));}
                if(j>0)travel+=Vector2.Distance(points[j-1],p);
                Add(p+side,0);Add(p-side,1);
                int a=vs.Count-4;
                if(j>0)tris.AddRange(new[]{a,a+2,a+1,a+1,a+2,a+3});
                if(j!=points.Count-1)continue;
                int h=vs.Count;Add(p+side*2.8f,0);Add(p-side*2.8f,1);Add(p+dir*width*2.9f,.5f);tris.AddRange(new[]{h,h+1,h+2});
            }
            var r=Shape(root,name,V(0,Surface+.007f,0),SaveMesh(name,vs,tris,uv),material);
            r.shadowCastingMode=ShadowCastingMode.Off;r.enabled=false;return r;
        }
        static Vector2 P(float x,float z)=>new(x,z);
        static void BuildFlows(EcosystemsLab lab)
        {
            var t=lab.transform;
            lab.energy=new[]{
                Ribbon(t,"Luz para o capim",gold,.020f,P(-1.055f,.28f),P(-.99f,.26f),P(-.905f,.26f)),
                Ribbon(t,"Capim para gafanhoto",gold,.024f,P(-.54f,.26f),P(-.38f,.225f),P(-.235f,.26f)),
                Ribbon(t,"Gafanhoto para sapo",gold,.024f,P(.16f,.26f),P(.30f,.225f),P(.435f,.26f))};
            lab.heat=new Renderer[3];var materials=new List<Renderer>();
            for(int i=0;i<3;i++)
            {
                float x=X[i];
                lab.heat[i]=Ribbon(t,"Calor · "+EcosystemsLab.Names[i],heatInk,.013f,P(x+.076f,.115f),P(x+.105f,.078f),P(x+.14f,.06f),P(x+.152f,.034f));
                materials.Add(Ribbon(t,"Resíduos · "+EcosystemsLab.Names[i],aqua,.015f,P(x+.07f,.405f),P(x+.094f,.47f),P(x+.147f,.53f),P(x+.235f,.54f)));
            }
            materials.Add(Ribbon(t,"Resíduos para decompositores",aqua,.016f,P(-.48f,.54f),P(.05f,.54f),P(.78f,.54f),P(.935f,.60f)));
            materials.Add(Ribbon(t,"Nutrientes do solo para o capim",aqua,.022f,P(1.01f,.80f),P(.88f,.875f),P(-.39f,.875f),P(-.90f,.77f),P(-.89f,.52f),P(-.81f,.43f)));
            lab.matter=materials.ToArray();
            Label(t,"Energia → alimento     ·     Calor → ambiente",V(-.55f,1.65f,1f),1.15f,22);
            Label(t,"Matéria → solo → produtor",V(.68f,1.65f,1f),.98f,22);
        }
        static void Interface(EcosystemsLab lab)
        {
            var t=lab.transform;
            var lever=Child(t,"Exibir fluxo",V(1.12f,1.01f,-.33f));
            lab.flowLever=lever.gameObject.AddComponent<XRLever>();Unity3DControlVisuals.Lever(lab.flowLever,.82f);
            lab.flowLever.value=false;
            Label(t,"Exibir fluxo",V(1.13f,1.005f,-.52f),.35f,22,25);
            var rotation=Quaternion.Euler(28,0,0);
            var controls=BiologyWorldUI.Panel(t,"E1 · Controles do diorama",V(0,.81f,-.68f),new Vector2(940,170),.0013f,rotation);
            lab.controls=controls.gameObject;var body=BiologyWorldUI.Body(controls,15);
            lab.status=BiologyWorldUI.Text(body,EcosystemsLab.Invitation,60,23);
            var row=BiologyWorldUI.Rect(body,"Ações");BiologyWorldUI.Height(row,58);
            var layout=row.gameObject.AddComponent<HorizontalLayoutGroup>();layout.spacing=12;layout.childControlWidth=layout.childControlHeight=true;layout.childForceExpandWidth=true;
            lab.restore=BiologyWorldUI.Button(row,"Restaurar diorama");lab.back=BiologyWorldUI.Button(row,"Voltar ao Hub");
            var pos=V(1.82f,1.43f,.32f);var orientation=Quaternion.Euler(0,17,0);
            var q=BiologyWorldUI.Panel(t,"Q1 · Matéria e energia",pos,new Vector2(600,460),.00135f,orientation);lab.consultation=q.gameObject;
            var info=BiologyWorldUI.Body(q,23);
            BiologyWorldUI.Text(info,"Decomposição",54,34,true);
            BiologyWorldUI.Text(info,"Fungos e bactérias decompõem restos e resíduos de todos os níveis da cadeia. Nutrientes minerais voltam ao solo e podem ser absorvidos pelo capim.",144,25);
            BiologyWorldUI.Text(info,"A luz fornece energia ao produtor. Parte da energia é transferida na alimentação e parte se dissipa como calor em cada nível.",116,24);
            lab.close=BiologyWorldUI.Button(info,"Fechar");
            var x=BiologyWorldUI.Panel(t,"X1 · Confirmação",pos,new Vector2(600,285),.00135f,orientation);lab.confirmation=x.gameObject;
            var actions=BiologyWorldUI.Body(x);lab.question=BiologyWorldUI.Text(actions,"Restaurar o diorama?",78,28,true);
            lab.cancel=BiologyWorldUI.Button(actions,"Cancelar");lab.confirm=BiologyWorldUI.Button(actions,"Restaurar");lab.confirmLabel=lab.confirm.GetComponentInChildren<TMPro.TMP_Text>();
            UIVisibility.Set(q.gameObject,false);UIVisibility.Set(x.gameObject,false);
        }
    }
}
