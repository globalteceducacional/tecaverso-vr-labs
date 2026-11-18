using System;
using System.Linq;
using Tecaverso.Labs.Biology.Microbes;
using Tecaverso.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Tecaverso.Editor
{
    public static class MicrobesAuthoring
    {
        public const string Folder = "Assets/_Project/Laboratories/Biology/Microbes";
        public const string ScenePath = Folder + "/Scenes/Microbes.unity";
        const string Art = "Assets/_Project/Art/Biology/";
        static Material graphite, accent;
        readonly struct Spec
        {
            public readonly string file, title, category, feature, description, note;
            public readonly Color32 tint;
            public Spec(string file, string title, string category, string feature, string description, string note, Color32 tint)
            {
                this.file=file; this.title=title; this.category=category; this.feature=feature;
                this.description=description; this.note=note; this.tint=tint;
            }
        }

        static readonly Spec[] Catalogue = {
            new("Bacillus bacteria", "Modelo Bacillus", "Bactéria · acervo", "Superfície externa", "O nome Bacillus vem do acervo. Observe a superfície desta maquete e compare seu relevo com os outros modelos bacterianos.", "Na descrição da forma, bacilo significa bastonete. Esta maquete artística não permite confirmar gênero ou espécie.", new(188,148,98,255)),
            new("Bacterium", "Bactéria genérica", "Bactéria", "Organização celular", "Uma bactéria é uma célula procariótica. Seu material genético ocupa uma região do citoplasma.", "Compare sua organização celular com a do vírus na galeria.", new(83,173,163,255)),
            new("Green bacterium", "Bactéria verde", "Bactéria", "Contorno celular", "A superfície delimita uma célula inteira. O interior reúne as estruturas que sustentam seu metabolismo.", "O verde é uma escolha artística do modelo.", new(121,171,91,255)),
            new("Salmonella", "Salmonella", "Bactéria · acervo", "Apêndices de superfície", "O acervo usa Salmonella como nome desta representação bacteriana. Observe o corpo alongado e seus apêndices.", "A identificação vem do acervo. A cor e a silhueta são ilustrativas.", new(203,140,93,255)),
            new("Virus", "Vírus genérico", "Vírus", "Partícula viral", "Os vírus são acelulares. Uma partícula viral contém material genético associado a proteínas.", "O modelo ilustra a superfície. Cor e forma não determinam a espécie.", new(187,102,134,255)),
            new("Light Virus", "Vírus luminoso", "Vírus", "Projeções de superfície", "Compare o relevo desta representação com o outro modelo viral. Ambos representam entidades acelulares.", "Os detalhes claros são um recurso visual do acervo.", new(132,134,197,255)),
            new("Bacteriophage Virus", "Bacteriófago", "Vírus", "Cabeça e cauda", "Bacteriófagos são vírus que têm bactérias como hospedeiras. Neste modelo, observe a cabeça e a cauda.", "O diorama lateral ilustra um ciclo lítico genérico em três momentos.", new(143,121,189,255)),
            new("Flagellas", "Flagelo", "Estrutura celular", "Filamento", "O flagelo pode contribuir para o deslocamento de uma célula. Observe o filamento isolado.", "Esta vitrine mostra uma parte da célula em ampliação independente.", new(205,167,95,255)),
            new("Chromosome", "Cromossomo", "Estrutura celular", "Cromátides-irmãs", "Observe as duas cromátides-irmãs unidas neste cromossomo eucariótico duplicado e condensado.", "A forma em X corresponde a um estado específico do cromossomo.", new(116,153,205,255))
        };

        [MenuItem("Tecaverso/Biologia/Instalar Microrganismos (uma vez)")]
        public static void Create()
        {
            var models = Catalogue.Select(s => AssetDatabase.LoadAssetAtPath<GameObject>(Art +
                (s.file == "Chromosome" ? "Dna" : "Microbes") + "/Models/" + s.file + " FBX.FBX")).ToArray();
            if (models.Any(model => !model)) throw new InvalidOperationException("Há modelos ausentes na galeria.");
            var scene = BiologyRoomAuthoring.Begin(ScenePath);
            CellsAssets.Ensure(Folder + "/Materials");
            graphite = AssetDatabase.LoadAssetAtPath<Material>("Assets/_Project/Laboratories/Biology/Cells/Materials/Grafite.mat");
            accent = AssetDatabase.LoadAssetAtPath<Material>("Assets/_Project/Laboratories/Biology/Anatomy/Materials/Acento Biologia.mat");
            var root = new GameObject("Galeria de microrganismos");
            root.SetActive(false);
            var lab = root.AddComponent<MicrobesLab>();
            lab.exhibits = new MicrobeExhibit[models.Length];
            for (int i = 0; i < models.Length; i++) lab.exhibits[i] = BuildExhibit(lab, models[i], i);
            Label(root.transform, "Microrganismos e vírus", new(0,2.34f,3), 1200, 85, 52);
            Label(root.transform, "Ampliação independente em cada vitrine", new(0,2.22f,3),1100,55,26);
            BuildConsole(lab);
            MicrobesDiorama.Create(lab, graphite, accent);
            root.SetActive(true);
            if (!EditorSceneManager.SaveScene(scene, ScenePath)) throw new InvalidOperationException("Falha ao salvar galeria.");
        }

        static MicrobeExhibit BuildExhibit(MicrobesLab lab, GameObject asset, int index)
        {
            var spec = Catalogue[index];
            float angle = Mathf.Lerp(-76,76,index / 8f);
            var stand = Node(lab.transform, spec.title, new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad)*4.55f,0,
                Mathf.Cos(angle * Mathf.Deg2Rad)*4.55f - 1.55f));
            stand.localRotation = Quaternion.Euler(0,angle,0);
            Shape(stand,"Coluna",PrimitiveType.Cylinder,new(0,.46f,0),new(.30f,.46f,.30f),graphite);
            Shape(stand,"Mesa de observação",PrimitiveType.Cylinder,new(0,.966f,0),new(.88f,.052f,.88f),graphite);
            Shape(stand,"Friso",PrimitiveType.Cylinder,new(0,.925f,0),new(.87f,.008f,.87f),accent);
            var holder = Node(stand,"Peça",new(0,1.45f,0));
            var item = holder.gameObject.AddComponent<MicrobeExhibit>();
            item.gallery = lab;
            item.title = spec.title; item.category = spec.category; item.description = spec.description; item.note = spec.note;
            var hit = holder.gameObject.AddComponent<SphereCollider>(); hit.radius = .31f; item.colliders.Add(hit);
            item.turntable = Node(holder,"Giro",Vector3.zero);
            var model = (GameObject)PrefabUtility.InstantiatePrefab(asset,item.turntable);
            var surfaces = model.GetComponentsInChildren<Renderer>(true);
            Bounds GetBounds()
            {
                var b = surfaces[0].bounds;
                foreach (var r in surfaces) b.Encapsulate(r.bounds);
                return b;
            }
            // Fit in unrotated gallery axes; each model retains its imported orientation and proportions.
            stand.localRotation = Quaternion.identity;
            if (spec.file == "Chromosome")
                model.transform.localRotation = Quaternion.Euler(0,0,90) * Quaternion.Euler(90,0,0) * model.transform.localRotation;
            var b = GetBounds();
            model.transform.localScale *= .61f / Mathf.Max(b.size.x, b.size.y, b.size.z);
            model.transform.position += item.turntable.position - GetBounds().center;
            stand.localRotation = Quaternion.Euler(0,angle,0);
            foreach (var r in surfaces)
            {
                r.sharedMaterials = r.sharedMaterials.Select(m => Surface(spec,m ? m.name : "")).ToArray();
                PrefabUtility.RecordPrefabInstancePropertyModifications(r);
            }
            PrefabUtility.RecordPrefabInstancePropertyModifications(model.transform);
            var dialMount = Node(stand,"Girar exemplar",new(0,1.025f,-.32f));
            dialMount.localRotation = Quaternion.Euler(145,0,0);
            item.dial = dialMount.gameObject.AddComponent<XRKnob>();
            item.dial.minAngle = 0; item.dial.maxAngle = 360;
            Unity3DControlVisuals.Knob(item.dial,1.2f); item.dial.value = 0;
            var plaque = Node(stand,"Identificação",new(0,.84f,-.435f));
            Shape(plaque,"Placa",PrimitiveType.Cube,Vector3.zero,new(.79f,.18f,.016f),graphite);
            Label(plaque,spec.title,new(0,.034f,-.011f),520,65,35);
            Label(plaque,spec.category,new(0,-.050f,-.011f),520,40,21);
            item.annotation = Label(stand,spec.feature,new(0,1.87f,0),590,70,29).transform.parent.gameObject;
            item.annotation.SetActive(false);
            return item;
        }

        static Material Surface(Spec spec,string slot)
        {
            Color color = spec.tint;
            if (slot.IndexOf("blue",StringComparison.OrdinalIgnoreCase)>=0) color = new Color32(98,158,191,255);
            else if (slot.IndexOf("red",StringComparison.OrdinalIgnoreCase)>=0) color = new Color32(194,101,123,255);
            else if (slot == "Green") color = new Color32(105,166,132,255);
            else if (slot == "Orange" || slot == "Yellow") color = new Color32(208,165,100,255);
            else if (slot == "Lines" || slot.Contains("chromasome")) color = new Color32(76,90,126,255);
            return MaterialFor("Tono " + ColorUtility.ToHtmlStringRGB(color),color);
        }

        internal static Material MaterialFor(string name,Color color)
        {
            string path = Folder + "/Materials/" + name + ".mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material) return material;
            material = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name=name };
            material.SetColor("_BaseColor",color); material.SetFloat("_Smoothness",.28f);
            AssetDatabase.CreateAsset(material,path);
            return material;
        }

        static void BuildConsole(MicrobesLab lab)
        {
            var parent=lab.transform;
            var tilt=Quaternion.Euler(46,0,0);
            Shape(parent,"Suporte do comando",PrimitiveType.Cylinder,new(-.90f,.44f,-.83f),new(.20f,.44f,.20f),graphite);
            var console=BiologyWorldUI.Panel(parent,"Comandos da galeria",new(-.90f,.96f,-.82f),new(610,230),.00175f,tilt);
            var body=BiologyWorldUI.Body(console,14);
            var row=BiologyWorldUI.Rect(body,"Visualização"); BiologyWorldUI.Height(row,55);
            var layout=row.gameObject.AddComponent<HorizontalLayoutGroup>();layout.spacing=8;
            layout.childControlWidth=layout.childControlHeight=layout.childForceExpandWidth=true;
            lab.labels=BiologyWorldUI.Toggle(row,"Rótulos de estruturas");lab.labels.SetIsOnWithoutNotify(false);
            lab.restore=BiologyWorldUI.Button(row,"Restaurar galeria");
            lab.status=BiologyWorldUI.Text(body,"Selecione um exemplar. Gire a manopla para examinar o outro lado.",65,23);
            lab.back=BiologyWorldUI.Button(body,"Voltar ao Hub",50);
            var position=new Vector3(-1.66f,1.48f,-.67f);
            var rotation=Quaternion.Euler(0,-25,0);
            var query=BiologyWorldUI.Panel(parent,"Consulta",position,new(635,540),.0018f,rotation);
            lab.query=query.gameObject;var content=BiologyWorldUI.Body(query,18);
            lab.queryTitle=BiologyWorldUI.Text(content,"Exemplar",65,36,true);
            lab.queryText=BiologyWorldUI.Text(content,"",345,26,true);lab.queryText.alignment=TextAlignmentOptions.TopLeft;
            lab.close=BiologyWorldUI.Button(content,"Fechar",58);
            var modal=BiologyWorldUI.Panel(parent,"Confirmar restauração",position,new(505,310),.0018f,rotation);
            lab.confirmation=modal.gameObject;var actions=BiologyWorldUI.Body(modal,20);
            BiologyWorldUI.Text(actions,"Restaurar a galeria?",56,34,true);
            BiologyWorldUI.Text(actions,"Os giros, os rótulos e a etapa do fago voltarão ao início.",70,23);
            lab.cancel=BiologyWorldUI.Button(actions,"Cancelar",56);lab.confirm=BiologyWorldUI.Button(actions,"Restaurar",56);
            UIVisibility.Set(lab.query,false);UIVisibility.Set(lab.confirmation,false);
        }

        internal static Transform Node(Transform parent,string name,Vector3 local)
        {
            var t=new GameObject(name).transform;t.SetParent(parent,false);t.localPosition=local;return t;
        }
        internal static TMP_Text Label(Transform parent,string text,Vector3 local,float width,float height,float font)
            => BiologyWorldUI.Caption(parent,text,local,new(width,height),font);
        internal static Transform Shape(Transform parent,string name,PrimitiveType type,Vector3 local,Vector3 size,Material material)
        {
            var g=GameObject.CreatePrimitive(type);g.name=name;g.transform.SetParent(parent,false);
            g.transform.localPosition=local;g.transform.localScale=size;g.GetComponent<Renderer>().sharedMaterial=material;
            UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());
            return g.transform;
        }
    }
}
