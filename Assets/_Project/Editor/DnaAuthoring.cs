using System;
using System.Collections.Generic;
using System.Linq;
using Tecaverso.Labs.Biology.Dna;
using Tecaverso.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Object = UnityEngine.Object;

namespace Tecaverso.Editor
{
    public static class DnaAuthoring
    {
        public const string Folder = "Assets/_Project/Laboratories/Biology/Dna";
        public const string ScenePath = Folder + "/Scenes/Dna.unity";
        const string Models = "Assets/_Project/Art/Biology/Dna/Models/";
        static Material graphite, rail, enamel, socketMaterial;
        static Material[] inks;
        static Mesh tileMesh;
        const string Symbols = "ATGCU";

        [MenuItem("Tecaverso/Biologia/Instalar DNA e RNA (uma vez)")]
        public static void Create()
        {
            var strand = AssetDatabase.LoadAssetAtPath<GameObject>(Models + "dna_strand_set.FBX");
            var chromosome = AssetDatabase.LoadAssetAtPath<GameObject>(Models + "Chromosome FBX.FBX");
            if (!strand || !chromosome) throw new InvalidOperationException("Importe os dois modelos canônicos de DNA antes da autoria.");
            var helixSource = strand.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name.IndexOf("DNA_01", StringComparison.OrdinalIgnoreCase) >= 0 && t.GetComponentsInChildren<MeshFilter>(true).Length > 1);
            if (!helixSource) throw new InvalidOperationException("Variante DNA_01 ausente no modelo.");
            var room = BiologyRoomAuthoring.Begin(ScenePath);
            CellsAssets.Ensure(Folder + "/Materials");
            graphite = AssetDatabase.LoadAssetAtPath<Material>("Assets/_Project/Laboratories/Biology/Cells/Materials/Grafite.mat");
            rail = AssetDatabase.LoadAssetAtPath<Material>("Assets/_Project/Laboratories/Biology/Anatomy/Materials/Acento Biologia.mat");
            enamel = Mat("Esmalte azul-marinho", new Color32(43, 67, 94, 255));
            socketMaterial = Mat("Aro dos encaixes", new Color32(170, 195, 202, 255));
            inks = new[] { Mat("A · âmbar",new Color32(164,91,16,255)), Mat("T · azul", new Color32(33,101,166,255)),
                Mat("G · verde",new Color32(30,119,89,255)), Mat("C · violeta",new Color32(130,74,151,255)), Mat("U · terracota",new Color32(166,65,64,255)) };
            tileMesh = TileMesh();
            var root = new GameObject("DNA, RNA e informação");
            root.SetActive(false);
            var lab = root.AddComponent<DnaLab>();
            var targets = new List<DnaTarget>();
            var board = MakeBench(lab.transform);
            BuildKit(lab, board, targets);
            lab.helix = Exhibit(lab, targets, helixSource.gameObject, true);
            Exhibit(lab, targets, chromosome, false);
            lab.targets = targets.ToArray();
            MakeInterface(lab);
            FinishConsole(lab);
            root.SetActive(true);
            if (!EditorSceneManager.SaveScene(room)) throw new InvalidOperationException("A cena de DNA não pôde ser salva.");
            AssetDatabase.SaveAssets();
        }

        static Transform MakeBench(Transform parent)
        {
            var board = Node(parent,"Bancada de pareamento", new Vector3(0,.99f,0));
            board.localRotation = Quaternion.Euler(48,0,0);
            Solid(board, "Tampo inclinado", Vector3.forward*.029f, new Vector3(1.62f,.81f,.056f), graphite);
            Solid(board, "Faixa Biologia",new Vector3(0,-.402f,-.002f),new Vector3(1.62f,.009f,.012f),rail);
            foreach (float x in new[]{-.61f,.61f})
                Solid(parent,"Pé da bancada",new Vector3(x,.44f,.06f),new Vector3(.055f,.88f,.073f),graphite);
            Caption(parent,"DNA, RNA e informação",new Vector3(0,1.88f,.57f),920,65,39);
            return board;
        }
        static void BuildKit(DnaLab lab, Transform board, List<DnaTarget> targets)
        {
            lab.sockets = new DnaSocket[4];
            lab.pieces = new DnaBase[Symbols.Length];
            const float top = .185f, bottom = -.049f;
            Caption(board,"FITA MOLDE · DNA",new Vector3(0,.331f,-.008f),550,44,18);
            Caption(board,"3'",new Vector3(-.684f,top,-.005f),70,55,31);
            Caption(board,"5'",new Vector3(.684f,top,-.005f),70,55,31);
            Caption(board,"5'",new Vector3(-.684f,bottom,-.005f),70,55,31);
            Caption(board,"3'",new Vector3(.684f,bottom,-.005f),70,55,31);
            Solid(board,"Suporte da fita molde",new Vector3(0,top,.001f),new Vector3(1.22f,.044f,.027f),enamel);
            Solid(board,"Suporte da fita complementar",new Vector3(0,bottom,.001f),new Vector3(1.22f,.044f,.027f),enamel);
            for (int i = 0; i < 4; i++)
            {
                float x = (i-1.5f)*.285f;
                char symbol = DnaPairing.Template[i];
                var mold = Node(board, "Molde " + symbol, new Vector3(x,top,-.021f));
                TokenVisual(mold, symbol);
                Solid(board,"Ligação entre posições",new Vector3(x,(top+bottom)*.5f,-.002f),new Vector3(.012f,.20f,.011f),socketMaterial);
                var ring = Node(board,"Encaixe " + (i+1),new Vector3(x,bottom,-.018f));
                var rim = Tile(ring, "Borda", socketMaterial);
                rim.localScale = new Vector3(1.2f,1.2f,.64f);
                var well = Tile(ring, "Berço", enamel);
                well.localScale = new Vector3(1.08f,1.07f,.65f);
                well.localPosition = Vector3.back*.005f;
                Caption(ring,(i+1).ToString(),Vector3.back*.019f,80,48,22);
                var hit = ring.gameObject.AddComponent<BoxCollider>();
                hit.size = new Vector3(.172f,.138f,.028f);
                var target = ring.gameObject.AddComponent<DnaTarget>();
                target.colliders.Clear(); target.colliders.Add(hit);
                target.lab = lab; target.targetIndex=i;targets.Add(target);
                var mount = Node(ring,"Assento",Vector3.back*.027f);
                var trigger = mount.gameObject.AddComponent<SphereCollider>();trigger.radius=.098f;trigger.isTrigger=true;
                var socket = mount.gameObject.AddComponent<DnaSocket>();
                socket.lab=lab;socket.index=i;socket.attachTransform=mount;socket.showInteractableHoverMeshes=false;
                lab.sockets[i]=socket;
            }
            // The T and U occupy the same physical tray well in their respective activities.
            const string order="AGCT";
            for (int i=0;i<Symbols.Length;i++)
            {
                char symbol=Symbols[i];
                float x=(order.IndexOf(symbol=='U'?'T':symbol)-1.5f)*.285f;
                var home=Node(board,"Bandeja · "+symbol,new Vector3(x,-.291f,-.030f));
                if(symbol!='U')
                {
                    var cradle=Tile(home,"Rebaixo da bandeja",enamel);
                    cradle.localPosition=Vector3.forward*.025f;cradle.localScale=new Vector3(1.17f,1.17f,1);
                }
                var piece=Node(lab.transform,"Base "+symbol,Vector3.zero);
                piece.SetPositionAndRotation(home.position,home.rotation);
                var body=piece.gameObject.AddComponent<Rigidbody>();body.isKinematic=true;body.useGravity=false;
                var c=piece.gameObject.AddComponent<BoxCollider>();c.size=new Vector3(.161f,.142f,.042f);
                var visual=Node(piece,"Peça esmaltada",Vector3.zero);
                TokenVisual(visual,symbol);
                var grab=piece.gameObject.AddComponent<DnaBase>();
                grab.lab=lab;grab.tray=home;grab.symbol=symbol.ToString();grab.visual=visual;grab.throwOnDetach=false;
                grab.movementType=XRBaseInteractable.MovementType.Kinematic;grab.colliders.Clear();grab.colliders.Add(c);
                lab.pieces[i]=grab;piece.gameObject.SetActive(symbol!='U');
            }
            Caption(board,"BASES",new Vector3(-.686f,-.291f,-.005f),100,48,19);
            lab.modeCaption=Caption(board,"DNA    0/4",new Vector3(.682f,-.291f,-.005f),146,48,19);
        }

        static Transform Exhibit(DnaLab lab,List<DnaTarget> targets,GameObject source,bool isHelix)
        {
            float x=isHelix?-1.51f:1.51f;
            var parent=lab.transform;
            Solid(parent,"Coluna",new Vector3(x,.49f,.68f),new Vector3(.49f,.49f,.49f),graphite,PrimitiveType.Cylinder);
            Solid(parent,"Filete",new Vector3(x,.986f,.68f),new Vector3(.71f,.012f,.71f),rail,PrimitiveType.Cylinder);
            Solid(parent,"Base de observação",new Vector3(x,1.024f,.68f),new Vector3(.77f,.029f,.77f),graphite,PrimitiveType.Cylinder);
            var pivot=Node(parent,isHelix?"Hélice de DNA":"Cromossomo",new Vector3(x,isHelix?1.62f:1.53f,.68f));
            var holder=Node(pivot,"Modelo legado",Vector3.zero);
            var obj=Object.Instantiate(source,holder,false);
            var renderers=obj.GetComponentsInChildren<MeshRenderer>(true);
            if(renderers.Length==0) throw new InvalidOperationException("Modelo sem malhas: "+source.name);
            foreach(var renderer in renderers)
                renderer.sharedMaterials=renderer.sharedMaterials.Select(m=>ExhibitMaterial(isHelix,renderer.name,m)).ToArray();
            var bounds=BoundsOf(renderers);
            // Present the long axis upright and the broad side of each specimen to the visitor.
            if(bounds.size.z>bounds.size.y && bounds.size.z>bounds.size.x)holder.localRotation=Quaternion.Euler(-90,0,0);
            else if(bounds.size.x>bounds.size.y)holder.localRotation=Quaternion.Euler(0,0,90);
            bounds=BoundsOf(renderers);
            if(bounds.size.z>bounds.size.x)holder.localRotation=Quaternion.AngleAxis(90,Vector3.up)*holder.localRotation;
            bounds=BoundsOf(renderers);
            holder.localScale*= (isHelix?1.06f:.88f)/Mathf.Max(bounds.size.x,bounds.size.y,bounds.size.z);
            holder.position+=pivot.position-BoundsOf(renderers).center;
            var collider=pivot.gameObject.AddComponent<BoxCollider>();
            collider.size=Vector3.Max(BoundsOf(renderers).size,Vector3.one*.11f);
            var target=pivot.gameObject.AddComponent<DnaTarget>();target.lab=lab;target.targetIndex=isHelix?4:5;
            target.colliders.Clear();target.colliders.Add(collider);targets.Add(target);
            Caption(parent,isHelix?"Dupla hélice de DNA":"Cromossomo duplicado",new Vector3(x,2.30f,.68f),700,60,31);
            Caption(parent,"AMPLIAÇÃO DIDÁTICA",new Vector3(x,2.212f,.68f),620,48,18);
            if(isHelix)
            {
                var support=Node(parent,"Giro da hélice",new Vector3(x,.97f,.27f));
                support.localRotation=Quaternion.Euler(48,0,0);
                Solid(support,"Apoio inclinado",Vector3.forward*.02f,new Vector3(.31f,.24f,.04f),enamel);
                var knob=Node(support,"Manopla",new Vector3(0,.025f,-.012f));knob.localRotation=Quaternion.Euler(90,0,0);
                lab.dial=knob.gameObject.AddComponent<XRKnob>();lab.dial.minAngle=0;lab.dial.maxAngle=360;
                Unity3DControlVisuals.Knob(lab.dial,1.45f);lab.dial.value=0;
                Caption(support,"Girar hélice",new Vector3(0,-.083f,-.013f),218,32,17);
            }
            return pivot;
        }

        static Material ExhibitMaterial(bool isHelix,string mesh,Material imported)
        {
            if(!isHelix) return mesh.IndexOf("DNA",StringComparison.OrdinalIgnoreCase)>=0?inks[2]:Mat("Cromatina",new Color32(110,80,162,255));
            string name=imported?imported.name:"";
            if(name.StartsWith("A_",StringComparison.OrdinalIgnoreCase))return inks[0];
            if(name.StartsWith("T_",StringComparison.OrdinalIgnoreCase))return inks[1];
            if(name.StartsWith("G_mtl1",StringComparison.OrdinalIgnoreCase)||name.StartsWith("C_",StringComparison.OrdinalIgnoreCase))return inks[3];
            if(name.StartsWith("G_",StringComparison.OrdinalIgnoreCase))return inks[2];
            return name.IndexOf("backbone_L",StringComparison.OrdinalIgnoreCase)>=0?socketMaterial:
                Mat("Espinha da hélice",new Color32(80,156,177,255));
        }
        static Bounds BoundsOf(Renderer[] renderers)
        {
            var b=renderers[0].bounds;
            foreach(var renderer in renderers)b.Encapsulate(renderer.bounds);
            return b;
        }

        static void MakeInterface(DnaLab lab)
        {
            var r=lab.transform;
            var p=BiologyWorldUI.Panel(r,"C1 · Controle das fitas",new Vector3(0,1.47f,.47f),new Vector2(1020,180),.0015f,Quaternion.Euler(12,0,0));
            lab.controls=p.gameObject;
            var body=BiologyWorldUI.Body(p,14);var row=Row(body,62);
            lab.dna=BiologyWorldUI.Button(row,"DNA · ativo");
            lab.rna=BiologyWorldUI.Button(row,"RNA");
            lab.restore=BiologyWorldUI.Button(row,"Restaurar fitas");
            lab.back=BiologyWorldUI.Button(row,"Voltar ao Hub");
            lab.status=BiologyWorldUI.Text(body,"Pegue uma base da bandeja e solte-a em frente à letra complementar.",68,24);
            var where=new Vector3(2.60f,1.49f,-.13f);
            var rotation=Quaternion.Euler(0,32,0);
            var q=BiologyWorldUI.Panel(r,"Q1 · Consulta",where,new Vector2(560,610),.00165f,rotation);
            lab.consultation=q.gameObject;var inner=BiologyWorldUI.Body(q,22);
            lab.queryTitle=BiologyWorldUI.Text(inner,"",83,35,true);
            lab.queryBody=BiologyWorldUI.Text(inner,"",385,25,false,new Color32(22,22,22,255));
            lab.queryBody.alignment=TextAlignmentOptions.TopLeft;
            lab.close=BiologyWorldUI.Button(inner,"Fechar consulta",66);UIVisibility.Set(q.gameObject,false);
            var c=BiologyWorldUI.Panel(r,"X1 · Restaurar",where,new Vector2(560,300),.00165f,rotation);
            lab.confirmation=c.gameObject;var dialog=BiologyWorldUI.Body(c,24);
            BiologyWorldUI.Text(dialog,"Restaurar as duas fitas?",77,33,true);
            BiologyWorldUI.Text(dialog,"As bases encaixadas voltarão à bandeja.",64,24);
            var buttons=Row(dialog,63);lab.cancel=BiologyWorldUI.Button(buttons,"Cancelar");lab.confirm=BiologyWorldUI.Button(buttons,"Restaurar");
            UIVisibility.Set(c.gameObject,false);
        }
        static Transform Row(Transform parent,int height)
        {
            var r=BiologyWorldUI.Rect(parent,"Ações");
            BiologyWorldUI.Height(r,height);var layout=r.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing=10;layout.childControlWidth=layout.childControlHeight=true;layout.childForceExpandWidth=true;return r;
        }

        // Both the initial installation and the focused scene update use the same mount.
        static void FinishConsole(DnaLab lab)
        {
            lab.status.font=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Bold SDF.asset");
            lab.status.fontSize=26;
            EditorUtility.SetDirty(lab.status);
            const string mountName="Suporte dos controles de pareamento";
            if(lab.transform.Find(mountName))return;
            var board=lab.transform.Find("Bancada de pareamento");
            var frame=Node(lab.transform,mountName,Vector3.zero);
            var surface=(RectTransform)lab.controls.transform;
            var body=board.Find("Tampo inclinado").GetComponent<Renderer>().sharedMaterial;
            Solid(frame,"Chapa traseira",surface.TransformPoint(new Vector3(0,0,8)),new Vector3(1.552f,.293f,.018f),body);
            frame.Find("Chapa traseira").rotation=surface.rotation;
            foreach(float x in new[]{-.58f,.58f})
            {
                var lower=board.TransformPoint(new Vector3(x,.34f,.046f));
                var upper=surface.position+surface.rotation*new Vector3(x,-.091f,.026f);
                var direction=upper-lower;
                var name=x<0?"Montante esquerdo":"Montante direito";
                Solid(frame,name,(lower+upper)*.5f,new Vector3(.036f,direction.magnitude,.032f),body);
                frame.Find(name).rotation=Quaternion.FromToRotation(Vector3.up,direction);
            }
        }
        public static void RefineConsole()
        {
            if(Application.isPlaying)throw new InvalidOperationException("O ajuste da bancada requer Edit Mode.");
            var room=SceneManager.GetSceneByPath(ScenePath);
            bool opened=!room.isLoaded;
            if(opened)room=EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Additive);
            try
            {
                if(room.isDirty)throw new InvalidOperationException("A cena de DNA tem edições pendentes.");
                var lab=room.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<DnaLab>(true)).Single();
                FinishConsole(lab);
                EditorSceneManager.MarkSceneDirty(room);
                if(!EditorSceneManager.SaveScene(room))throw new InvalidOperationException("O ajuste da bancada não pôde ser salvo.");
            }
            finally{if(opened)EditorSceneManager.CloseScene(room,true);}
        }
        static TMP_Text Caption(Transform parent,string content,Vector3 position,int width,int height,int font) =>
            BiologyWorldUI.Caption(parent,content,position,new Vector2(width,height),font);
        static void TokenVisual(Transform root,char symbol)
        {
            Tile(root,"Pastilha",inks[Symbols.IndexOf(symbol)]);
            Caption(root,symbol.ToString(),Vector3.back*.024f,97,80,52);
        }
        static Transform Tile(Transform parent,string name,Material material)
        {
            var t=Node(parent,name,Vector3.zero);
            t.gameObject.AddComponent<MeshFilter>().sharedMesh=tileMesh;
            t.gameObject.AddComponent<MeshRenderer>().sharedMaterial=material;return t;
        }
        static Mesh TileMesh()
        {
            string path=Folder+"/Materials/Pastilha chanfrada.asset";
            var existing=AssetDatabase.LoadAssetAtPath<Mesh>(path);if(existing)return existing;
            Vector2[] shape={new(-.066f,-.062f),new(.049f,-.062f),new(.080f,-.031f),new(.080f,.032f),
                new(.049f,.062f),new(-.066f,.062f),new(-.080f,.046f),new(-.080f,-.046f)};
            var vertices=new List<Vector3>();var indices=new List<int>();
            // A flat front and a bevel around the thumb-sized token keep each letter readable from oblique angles.
            foreach(var layer in new[]{new Vector2(.92f,-.020f),new Vector2(1,-.013f),new Vector2(1,.013f),new Vector2(.92f,.020f)})
                foreach(var p in shape)vertices.Add(new Vector3(p.x*layer.x,p.y*layer.x,layer.y));
            int n=shape.Length;
            for(int i=1;i<n-1;i++) { indices.AddRange(new[]{0,i+1,i,3*n,3*n+i,3*n+i+1}); }
            for(int layer=0;layer<3;layer++)
                for(int i=0;i<n;i++)
                {
                    int a=layer*n+i,b=layer*n+(i+1)%n;
                    indices.AddRange(new[]{a,b,b+n,a,b+n,a+n});
                }
            var mesh=new Mesh{name="Pastilha chanfrada"};mesh.SetVertices(vertices);mesh.SetTriangles(indices,0);mesh.RecalculateNormals();mesh.RecalculateBounds();
            AssetDatabase.CreateAsset(mesh,path);return mesh;
        }
        static Transform Node(Transform parent,string name,Vector3 position)
        {
            var t=new GameObject(name).transform;t.SetParent(parent,false);t.localPosition=position;return t;
        }
        static void Solid(Transform parent,string name,Vector3 position,Vector3 scale,Material material,PrimitiveType type=PrimitiveType.Cube)
        {
            var g=GameObject.CreatePrimitive(type);g.name=name;g.transform.SetParent(parent,false);
            g.transform.localPosition=position;g.transform.localScale=scale;g.GetComponent<Renderer>().sharedMaterial=material;
        }
        static Material Mat(string name,Color color)
        {
            var path=Folder+"/Materials/"+name+".mat";var found=AssetDatabase.LoadAssetAtPath<Material>(path);if(found)return found;
            var mat=new Material(Shader.Find("Universal Render Pipeline/Lit")){name=name};mat.SetColor("_BaseColor",color);mat.SetFloat("_Smoothness",.34f);AssetDatabase.CreateAsset(mat,path);return mat;
        }
    }
}
