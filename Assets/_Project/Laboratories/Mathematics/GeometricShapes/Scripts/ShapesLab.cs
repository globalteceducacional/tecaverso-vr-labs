using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Tecaverso.UI;
namespace Tecaverso.Labs.Shapes
{
    public sealed class ShapesLab:MonoBehaviour
    {
        public Material surface,glass,lines,marker;public TMP_FontAsset font;
        public TMP_InputField xInput,yInput,radiusInput;public int vertexIndex;public TMP_Text detail,status,comparison;public Transform[] stations;public GameObject[] pages;
        public GameObject modal;public TMP_Text modalText;public Transform sliderHandle;
        public int mode;public bool magnetic=true;public bool Modal=>modal!=null&&modal.GetComponent<CanvasGroup>().alpha>0;
        public ShapeView selected;public ShapeView[] study=new ShapeView[2];
        List<ShapeData> catalog;Action pending;readonly HashSet<ShapeView> rebuild=new();
        readonly Stack<State[]> undo=new(),redo=new();
        class State {public ShapeData data;public Vector3 position;public Quaternion rotation;public float open;public bool regular;}
        void Start(){Wire();catalog=ShapeGeometry.Catalog();UIVisibility.Set(modal,false);BuildGallery();Page(0);Refresh();}
        void Wire()
        {
                        var buttons=GetComponentsInChildren<Button>(true);
            foreach(var field in new[]{xInput,yInput,radiusInput})if(field!=null)field.onSelect.AddListener(_=>{var keyboard=UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard.GlobalNonNativeKeyboard.instance;if(keyboard!=null)keyboard.ShowKeyboard(field);});
            if(xInput!=null)xInput.onEndEdit.AddListener(s=>NumericVertex(0,s));if(yInput!=null)yInput.onEndEdit.AddListener(s=>NumericVertex(1,s));if(radiusInput!=null)radiusInput.onEndEdit.AddListener(s=>{if(float.TryParse(s.Replace(',','.'),System.Globalization.NumberStyles.Float,System.Globalization.CultureInfo.InvariantCulture,out float r)&&r>=.25f&&r<=2&&selected!=null&&!selected.gallery){float current=selected.data.curved!=""?selected.data.radius:selected.data.vertices.Max(v=>v.magnitude);Size(r/current);}});
            void Bind(string name,Action action){var b=buttons.First(x=>x.name==name);b.onClick.AddListener(()=>{if(!Modal)action();});}
            Bind("COLEÇÃO",()=>Page(0));Bind("INVESTIGAR",()=>Page(1));Bind("DESAFIOS",()=>Page(2));Bind("ESTUDAR EM A",()=>Study(0));Bind("ESTUDAR EM B",()=>Study(1));
            Bind("MANIPULAR",()=>SetMode(0));Bind("INSPECIONAR",()=>SetMode(1));Bind("EDITAR",()=>SetMode(2));
            string[] names={"ROTAÇÃO","VÉRTICES","ARESTAS / LADOS","FACES","TRANSPARÊNCIA","ÂNGULOS","DIAGONAIS","SIMETRIA","MANTER REGULAR","MAGNÉTICO"};for(int i=0;i<names.Length;i++){int k=i;Bind(names[i],()=>Toggle(k));}
            Bind("PLANIFICAR / FECHAR",Unfold);Bind("ALINHAR A/B",Align);Bind("RESTAURAR POSIÇÃO",Restore);Bind("LADOS −",()=>Sides(-1));Bind("LADOS +",()=>Sides(1));Bind("TAMANHO −",()=>Size(.9f));Bind("TAMANHO +",()=>Size(1.1f));
            Bind("DESFAZER",()=>History(false));Bind("REFAZER",()=>History(true));Bind("LIMPAR ESTUDO",Clear);Bind("VOLTAR AO HUB",Back);
            for(int i=0;i<4;i++){int k=i;Bind("ATIVIDADE "+(i+1),()=>Challenge(k));}
            buttons.First(b=>b.name=="CONFIRMAR").onClick.AddListener(()=>Confirm(true));buttons.First(b=>b.name=="CANCELAR").onClick.AddListener(()=>Confirm(false));
        }
        void NumericVertex(int axis,string text)
        {
            if(Modal||selected==null||selected.gallery||!selected.data.planar||selected.data.curved!="")return;
            if(!float.TryParse(text.Replace(',','.'),System.Globalization.NumberStyles.Float,System.Globalization.CultureInfo.InvariantCulture,out float value)||!float.IsFinite(value)||Mathf.Abs(value)>2){status.text="Coordenada entre −2 e 2.";return;}
            Save();bool old=magnetic;magnetic=false;var p=selected.data.vertices[Mathf.Clamp(vertexIndex,0,selected.data.vertices.Length-1)];p[axis]=value;selected.Edit(Mathf.Clamp(vertexIndex,0,selected.data.vertices.Length-1),p);magnetic=old;
        }
        void BuildGallery(){int flat=0,solid=0;foreach(var d in catalog){int index=d.planar?flat++:solid++;var pos=new Vector3(d.planar?-4.8f:4.8f,1.5f+(index/5)*1.8f,-2.5f+(index%5)*1.7f);var v=Create(d,true,pos);v.transform.localScale=Vector3.one*.5f;if(d.planar)v.transform.rotation=Quaternion.Euler(0,-65,0);}}
        ShapeView Create(ShapeData d,bool gallery,Vector3 at){var go=new GameObject(d.name);go.transform.SetParent(transform);go.transform.position=at;var v=go.AddComponent<ShapeView>();v.Initialize(this,d,gallery);return v;}
        public TMP_Text WorldLabel(Transform parent,string text,Vector3 at,float size){var t=new GameObject(text).AddComponent<TextMeshPro>();t.transform.SetParent(parent,false);t.transform.localPosition=at;t.font=font;t.text=text;t.fontSize=size*4;t.alignment=TextAlignmentOptions.Center;t.rectTransform.sizeDelta=new Vector2(3,.65f);t.overflowMode=TextOverflowModes.Ellipsis;t.raycastTarget=false;return t;}
        public void Select(ShapeView v){if(Modal)return;selected=v;Refresh();}
        public void Page(int page){if(Modal)return;for(int i=0;i<pages.Length;i++)UIVisibility.Set(pages[i],i==page);}
        public void Study(int slot){if(selected==null||Modal)return;var data=selected.data.Copy();Action apply=()=>{Save();if(study[slot]!=null)Destroy(study[slot].gameObject);study[slot]=Create(data,false,stations[slot].position);study[slot].transform.localScale=Vector3.one*.6f;selected=study[slot];mode=0;Refresh();};if(study[slot]!=null)Ask("Substituir a peça de estudo?",apply);else apply();}
        public void SetMode(int value){if(Modal)return;if(value==2&&(selected==null||!selected.data.planar||selected.data.curved!=""||selected.gallery)){status.text="Para editar, leve um polígono para A ou B.";return;}mode=value;foreach(var v in study)if(v!=null)v.Rebuild();Refresh();}
        public void Toggle(int option){if(Modal||selected==null)return;var v=selected;switch(option){case 0:v.rotating=!v.rotating;break;case 1:v.showVertices=!v.showVertices;break;case 2:v.showEdges=!v.showEdges;break;case 3:v.showFaces=!v.showFaces;break;case 4:v.transparent=!v.transparent;break;case 5:v.angles=!v.angles;break;case 6:v.diagonals=!v.diagonals;break;case 7:v.symmetry=!v.symmetry;break;case 8:if(!v.data.planar||v.data.curved!="")return;if(!v.regular&&!ShapeGeometry.Regular(v.data.vertices)){status.text="Restaure um polígono regular antes de travar a regularidade.";return;}v.regular=!v.regular;break;case 9:magnetic=!magnetic;break;}v.Rebuild();Refresh();}
        public void Open(float value){if(Modal||selected==null||selected.gallery||!selected.CanUnfold)return;selected.open=Mathf.Clamp01(value);selected.rotating=false;selected.Rebuild();Refresh();}
        public void Unfold(){if(selected==null||selected.gallery||!selected.CanUnfold){status.text="Planificação: cubo, prisma triangular ou pirâmide quadrada em A/B.";return;}Save();mode=1;int slot=Array.IndexOf(study,selected);selected.transform.SetPositionAndRotation(stations[slot].position,Quaternion.identity);Open(selected.open>0?0:1);}
        public void Align(){if(Modal)return;Save();for(int i=0;i<2;i++)if(study[i]!=null){study[i].transform.SetPositionAndRotation(stations[i].position,Quaternion.identity);study[i].rotating=false;}Refresh();}
        public void Restore(){if(selected==null||selected.gallery||Modal)return;Save();int i=Array.IndexOf(study,selected);selected.transform.SetPositionAndRotation(stations[i].position,Quaternion.identity);selected.open=0;selected.Rebuild();Refresh();}
        public void Sides(int delta){if(Modal||selected==null||selected.gallery||!selected.data.planar||selected.data.curved!="")return;Save();selected.data=ShapeGeometry.Polygon(Mathf.Clamp(selected.data.vertices.Length+delta,3,12));selected.regular=true;selected.Rebuild();Refresh();}
        public void Size(float factor){if(Modal||selected==null||selected.gallery)return;float radius=selected.data.vertices.Max(v=>v.magnitude);if(radius*factor<.25f||radius*factor>2.2f)return;Save();selected.data.vertices=selected.data.vertices.Select(v=>v*factor).ToArray();selected.data.radius*=factor;selected.data.height*=factor;selected.Rebuild();Refresh();}
        public void QueueRebuild(ShapeView v){rebuild.Add(v);}
        void LateUpdate(){foreach(var v in rebuild.ToArray())if(v!=null&&!v.GetComponentsInChildren<ShapeGrab>().Any(g=>g.isSelected)){v.Rebuild();rebuild.Remove(v);Refresh();}}
        State[] Capture()=>study.Select(v=>v==null?null:new State{data=v.data.Copy(),position=v.transform.position,rotation=v.transform.rotation,open=v.open,regular=v.regular}).ToArray();
        public void Save(){undo.Push(Capture());redo.Clear();}
        public void History(bool forward){if(Modal)return;var from=forward?redo:undo;var to=forward?undo:redo;if(from.Count==0)return;to.Push(Capture());var snapshot=from.Pop();for(int i=0;i<2;i++){if(study[i]!=null)Destroy(study[i].gameObject);study[i]=null;if(snapshot[i]==null)continue;var s=snapshot[i];study[i]=Create(s.data,false,s.position);study[i].transform.localScale=Vector3.one*.6f;study[i].transform.rotation=s.rotation;study[i].open=s.open;study[i].regular=s.regular;study[i].Rebuild();}selected=study.FirstOrDefault(v=>v!=null);Refresh();}
        public void Clear(){Ask("Remover as duas peças de estudo?",()=>{Save();foreach(var v in study)if(v!=null)Destroy(v.gameObject);study=new ShapeView[2];selected=null;Refresh();});}
        public void Back(){Ask("Voltar ao Hub e encerrar este estudo?",()=>SceneManager.LoadSceneAsync("Assets/_Project/Hub/Scenes/Hub.unity"));}
        void Ask(string text,Action action){if(Modal)return;pending=action;modalText.text=text;foreach(var p in pages){var c=p.GetComponent<CanvasGroup>();c.interactable=c.blocksRaycasts=false;}UIVisibility.Set(modal,true);}
        public void Confirm(bool yes){var action=pending;pending=null;UIVisibility.Set(modal,false);foreach(var p in pages){var c=p.GetComponent<CanvasGroup>();c.interactable=c.blocksRaycasts=c.alpha>0;}if(yes)action?.Invoke();}
        string Describe(ShapeView v){if(v==null)return "Selecione uma peça da galeria.";var d=v.data;string s=d.name+"\n";if(d.curved!="")s+=d.curved=="circle"?"Figura plana · não é polígono":"Corpo redondo · superfície curva";else s+=d.planar?$"{d.vertices.Length} lados · {(ShapeGeometry.Regular(d.vertices)?"Regular":"Irregular")} · {(ShapeGeometry.Convex(d.vertices)?"Convexo":"Côncavo")}":$"{d.vertices.Length} vértices · {d.Edges.Length} arestas · {d.faces.Length} faces";s+=$"\nÁrea: {d.Area:0.00} u²";s+=d.planar?$" · Perímetro: {d.Perimeter:0.00} u":$"\nVolume fechado: {d.Volume:0.00} u³";return s;}
        public void Refresh(){UpdateControls();detail.text=Describe(selected);comparison.text="A · "+Describe(study[0])+"\n\nB · "+Describe(study[1]);if(selected!=null)status.text=$"{new[]{"Manipular","Inspecionar","Editar"}[mode]} · Rotação {(selected.rotating?"ligada":"desligada")}\nVértices {(selected.showVertices?"sim":"não")} · Arestas {(selected.showEdges?"sim":"não")} · Faces {(selected.showFaces?"sim":"não")}\nMagnético {(magnetic?"ligado":"desligado")} · Manter regular {(selected.regular?"sim":"não")}";}
        void UpdateControls()
        {
            string[] names={"ROTAÇÃO","VÉRTICES","ARESTAS / LADOS","FACES","TRANSPARÊNCIA","ÂNGULOS","DIAGONAIS","SIMETRIA","MANTER REGULAR","MAGNÉTICO"};
            if(selected==null)return;var v=selected;bool polygon=v.data.planar&&v.data.curved=="";var states=new[]{v.rotating,v.showVertices,v.showEdges,v.showFaces,v.transparent,v.angles,v.diagonals,v.symmetry,v.regular,magnetic};
            foreach(var b in GetComponentsInChildren<Button>(true)){int i=Array.IndexOf(names,b.name);if(i>=0){b.GetComponentInChildren<TMP_Text>().text=(states[i]?"● ":"○ ")+names[i];b.interactable=i==0||i==3||i==4||(i==1||i==2?v.data.curved=="":polygon);}
                if(b.name=="PLANIFICAR / FECHAR")b.interactable=!v.gallery&&v.CanUnfold;
                if(b.name=="EDITAR")b.interactable=!v.gallery&&polygon&&v.open==0;
            }
            if(xInput!=null){xInput.interactable=yInput.interactable=polygon&&!v.gallery;if(polygon){var p=v.data.vertices[Mathf.Clamp(vertexIndex,0,v.data.vertices.Length-1)];if(!xInput.isFocused)xInput.SetTextWithoutNotify(p.x.ToString("0.###"));if(!yInput.isFocused)yInput.SetTextWithoutNotify(p.y.ToString("0.###"));}}
            if(radiusInput!=null){radiusInput.interactable=!v.gallery;if(!radiusInput.isFocused)radiusInput.SetTextWithoutNotify((v.data.curved!=""?v.data.radius:v.data.vertices.Max(p=>p.magnitude)).ToString("0.###"));}
        }
        public void Challenge(int i){status.text=new[]{"Transforme um polígono regular em irregular e compare lados e ângulos.","Conte os 8 vértices, 12 arestas e 6 faces de um cubo.","Compare áreas de figuras com o mesmo perímetro.","Planifique um prisma e identifique suas duas bases."}[i];}
    }
}
