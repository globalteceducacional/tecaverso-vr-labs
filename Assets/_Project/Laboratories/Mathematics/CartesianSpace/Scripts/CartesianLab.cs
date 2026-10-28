using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Tecaverso.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;

namespace Tecaverso.Labs.Cartesian
{
    public sealed class CartesianLab : MonoBehaviour
    {
        public Transform origin,contextCanvas,mainCanvas;public float Unit=.5f;
        public CartesianGrab pointPrefab,constructionPrefab;public Material lineMaterial,solidMaterial,surfaceMaterial;public Mesh coneMesh;public TMP_FontAsset font;
        public Color pointColor=new Color(1,.55f,.12f),selectedColor=new Color(1,.85f,.35f);public Color[] axisColors={new Color(.95f,.2f,.25f),new Color(.2f,.85f,.45f),new Color(.2f,.55f,1)};
        public GameObject[] pages,grids;public GameObject dialog,contextPage,coordinateSection;public Button[] tabs,tools,viewButtons,planeButtons,lockButtons,minusButtons,plusButtons,challengeButtons;
        public TMP_InputField[] fields;public Button create,cancel,undo,redo,back,clear,confirm,dismiss,delete,pin,moveGroup,analysis,nextPoint,nextConstruction,stepButton,lower,raise;
        public TMP_Text instruction,status,details,dialogText,challengeText;public TMP_Text[] coordinateLabels;
        public CartesianModel Model {get;}=new();
        public int SelectedPoint {get;private set;}=-1;public int SelectedConstruction {get;private set;}=-1;public int Pinned {get;private set;}=-1;
        public bool Modal {get;private set;}public bool Analysis {get;private set;}public bool Snap {get;private set;}=true;public float Step {get;private set;}=.5f;
        public bool ShowComponents=true,ShowPosition=true,ShowProjections=true;
        public bool MovingGroup {get;private set;}
        public ConstructionKind? Tool {get;private set;}
        public readonly List<int> Pending=new();
        readonly Dictionary<int,CartesianGeometryView> pointViews=new(),constructionViews=new();
        CartesianGrab activeGrab;int revision=-1,plane=-1,activity;int hoverPoint=-1,hoverConstruction=-1;Action confirmation;bool setting,loading;float checkTimer;
        LineRenderer preview;CartesianGeometryView previewView;AudioSource audioSource;AudioClip selectSound;
        static readonly CultureInfo Pt=CultureInfo.GetCultureInfo("pt-BR");
        public static string N(float value)=>value.ToString("0.##",Pt);
        public static string V(Vector3 v)=>"("+N(v.x)+"; "+N(v.y)+"; "+N(v.z)+")";
        public static string Kind(ConstructionKind k)=>new[]{"Segmento","Reta","Semirreta","Triângulo","Plano","Caixa","Esfera","Tetraedro"}[(int)k];
        public Vector3 World(Vector3 p)=>origin.position+p*Unit;public Vector3 Coordinate(Vector3 p)=>(p-origin.position)/Unit;
        public Vector3 ConstructionCenter(int id){var c=Model.Construction(id);return c==null?origin.position:World(Model.Vertices(c).Aggregate(Vector3.zero,(s,p)=>s+p)/c.points.Length);}
        void Awake()
        {
            for(int i=0;i<tabs.Length;i++){int n=i;tabs[i].onClick.AddListener(()=>Page(n));}
            for(int i=0;i<tools.Length;i++){int n=i;tools[i].onClick.AddListener(()=>BeginTool((ConstructionKind)n));}
            create.onClick.AddListener(CreatePoint);cancel.onClick.AddListener(CancelTool);undo.onClick.AddListener(()=>History(false));redo.onClick.AddListener(()=>History(true));
            back.onClick.AddListener(()=>Ask("Voltar ao Hub? As construções desta sessão serão descartadas.",()=>{loading=true;SceneManager.LoadSceneAsync("Assets/_Project/Hub/Scenes/Hub.unity");}));
            clear.onClick.AddListener(()=>Ask("Limpar todos os pontos e construções? Você poderá desfazer.",()=>{Model.Clear();SelectedPoint=SelectedConstruction=Pinned=-1;Pending.Clear();Tool=null;Sync();}));
            confirm.onClick.AddListener(()=>{var action=confirmation;CloseModal();action?.Invoke();});dismiss.onClick.AddListener(CloseModal);
            delete.onClick.AddListener(DeleteSelected);pin.onClick.AddListener(()=>{Pinned=Pinned==SelectedPoint?-1:SelectedPoint;Dirty();});
            moveGroup.onClick.AddListener(()=>{MovingGroup=!MovingGroup;status.text=MovingGroup?"Agarre o marcador central. Pontos compartilhados também se moverão.":"Selecione um vértice para editar a forma.";Dirty();});
            analysis.onClick.AddListener(()=>{Analysis=!Analysis;Dirty();});nextPoint.onClick.AddListener(()=>Cycle(false));nextConstruction.onClick.AddListener(()=>Cycle(true));
            stepButton.onClick.AddListener(()=>{Step=Step==.5f?1:Step==1?.1f:.5f;Present();});lower.onClick.AddListener(()=>Height(-.2f));raise.onClick.AddListener(()=>Height(.2f));
            for(int axis=0;axis<3;axis++)
            {
                int a=axis;fields[a].onEndEdit.AddListener(value=>Edit(a,value));fields[a].onSelect.AddListener(_=>{if(GlobalNonNativeKeyboard.instance!=null)GlobalNonNativeKeyboard.instance.ShowKeyboard(fields[a]);});
                lockButtons[a].onClick.AddListener(()=>{if(activeGrab!=null)return;Model.ToggleLock(SelectedPoint,a);Dirty();});minusButtons[a].onClick.AddListener(()=>Increment(a,-Step));plusButtons[a].onClick.AddListener(()=>Increment(a,Step));
            }
            for(int i=0;i<planeButtons.Length;i++){int index=i;planeButtons[i].onClick.AddListener(()=>{plane=new[]{-1,2,1,0}[index];status.text="Restrição aplicada somente aos novos pontos.";Present();});}
            for(int i=0;i<viewButtons.Length;i++){int index=i;viewButtons[i].onClick.AddListener(()=>ToggleView(index));}
            for(int i=0;i<challengeButtons.Length;i++){int index=i;challengeButtons[i].onClick.AddListener(()=>{activity=index;CheckChallenge();Present();});}
            preview=new GameObject("Construction preview").AddComponent<LineRenderer>();preview.transform.SetParent(transform);preview.sharedMaterial=lineMaterial;preview.startColor=preview.endColor=selectedColor;preview.widthMultiplier=.018f;
            audioSource=gameObject.AddComponent<AudioSource>();audioSource.spatialBlend=0;audioSource.volume=.08f;var samples=new float[1600];for(int i=0;i<samples.Length;i++)samples[i]=Mathf.Sin(i*2*Mathf.PI*700/24000)*(1-(float)i/samples.Length)*.15f;selectSound=AudioClip.Create("Selection feedback",samples.Length,1,24000,false);selectSound.SetData(samples,0);
            UIVisibility.Set(dialog,false);Page(0);Sync();status.text="Crie um ponto na origem. Agarre para mover; use o analógico para aproximar ou afastar.";
        }
        void Update(){if(Model.Revision!=revision)Sync();checkTimer+=Time.unscaledDeltaTime;if(checkTimer>.3f){checkTimer=0;CheckChallenge();}}
        public void CreatePoint(){if(Modal||activeGrab!=null||loading)return;var p=Model.AddPoint(plane);if(p==null){status.text="Limite de 24 pontos. Exclua um para continuar.";return;}SelectedPoint=p.id;SelectedConstruction=-1;MovingGroup=false;Sync();PlaceContext();status.text=p.Name+" criado em "+V(p.position)+". Agarre para posicionar.";}
        public void Select(int id,bool construction)
        {
            if(Modal||loading)return;SelectedPoint=construction?-1:id;SelectedConstruction=construction?id:-1;
            if(!construction&&Tool.HasValue)
            {
                if(Pending.Contains(id)){status.text="Esse ponto já foi escolhido. Selecione outro.";return;}
                Pending.Add(id);if(Pending.Count==CartesianModel.Required(Tool.Value))
                {var result=Model.Add(Tool.Value,Pending.ToArray(),out var error);if(result==null){Pending.RemoveAt(Pending.Count-1);status.text=error;}else{SelectedPoint=-1;SelectedConstruction=result.id;Pending.Clear();Tool=null;status.text="Construção criada. Mova seus vértices para investigar.";}}
            }
            audioSource.PlayOneShot(selectSound);Dirty();PlaceContext();
        }
        public void BeginTool(ConstructionKind kind){if(Modal||activeGrab!=null)return;Tool=kind;Pending.Clear();MovingGroup=false;if(SelectedPoint>=0)Pending.Add(SelectedPoint);status.text="Selecione os pontos no espaço. Cancelar encerra a ferramenta.";Dirty();}
        public void CancelTool(){if(Modal)return;Tool=null;Pending.Clear();Dirty();}
        public bool BusyOther(CartesianGrab grab)=>activeGrab!=null&&activeGrab!=grab;
        public bool BeginDrag(CartesianGrab grab)
        {
            if(Modal||activeGrab!=null||Tool.HasValue||(grab.construction&&!MovingGroup)||(!grab.construction&&SelectedPoint!=grab.id))return false;
            activeGrab=grab;Model.BeginGesture();Present();return true;
        }
        public void EndDrag(CartesianGrab grab){if(activeGrab!=grab)return;Model.EndGesture();activeGrab=null;Dirty();}
        public bool IsHighlighted(int id,bool construction)
        {
            if(!construction)return id==SelectedPoint||id==Pinned||id==hoverPoint||Pending.Contains(id)||(SelectedConstruction>=0&&Model.Construction(SelectedConstruction)?.points.Contains(id)==true);
            return id==SelectedConstruction||id==hoverConstruction||(SelectedPoint>=0&&Model.Construction(id)?.points.Contains(SelectedPoint)==true);
        }
        public void Hover(int id,bool construction,bool value){if(Modal)return;if(construction)hoverConstruction=value?id:-1;else hoverPoint=value?id:-1;Dirty();}
        void ToggleView(int i){if(Modal)return;if(i==0)ShowComponents=!ShowComponents;if(i==1)ShowPosition=!ShowPosition;if(i==2)ShowProjections=!ShowProjections;if(i==3){Snap=!Snap;status.text=Snap?"Magnético ligado: aproximações de até 0,12 u encaixam nos inteiros. Digitação permanece exata.":"Magnético desligado: movimento livre, inclusive valores fracionários.";}if(i>=4)grids[i-4].SetActive(!grids[i-4].activeSelf);Dirty();}
        void Height(float delta){if(activeGrab!=null)return;var position=origin.position;position.y=Mathf.Clamp(position.y+delta,CartesianModel.Limit*Unit+.2f,3.2f);origin.position=position;Dirty();}
        void Edit(int axis,string value)
        {
            if(setting||Modal||activeGrab!=null)return;var p=Model.Point(SelectedPoint);if(p==null)return;
            if(!float.TryParse(value.Replace(',','.'),NumberStyles.Float,CultureInfo.InvariantCulture,out float number)||!float.IsFinite(number)||Mathf.Abs(number)>CartesianModel.Limit){status.text="Informe um número entre −5 e 5.";Present();return;}
            var position=p.position;position[axis]=number;Model.Move(p.id,position);Sync();
        }
        void Increment(int axis,float amount){if(Modal||activeGrab!=null)return;var p=Model.Point(SelectedPoint);if(p==null)return;var v=p.position;v[axis]+=amount;Model.Move(p.id,v);Sync();}
        void History(bool forward){if(Modal||activeGrab!=null)return;Tool=null;Pending.Clear();if(forward)Model.Redo();else Model.Undo();Sync();}
        void Cycle(bool construction){if(Modal||activeGrab!=null)return;var ids=construction?Model.Constructions.Select(c=>c.id).ToArray():Model.Points.Select(p=>p.id).ToArray();if(ids.Length==0)return;int current=construction?SelectedConstruction:SelectedPoint;Select(ids[(Array.IndexOf(ids,current)+1)%ids.Length],construction);}
        void DeleteSelected()
        {
            if(activeGrab!=null)return;if(SelectedPoint>=0){int id=SelectedPoint;Ask("Excluir "+Model.Point(id).Name+" e "+Model.Dependencies(id)+" construção(ões) dependente(s)?",()=>{Model.DeletePoint(id);SelectedPoint=-1;if(Pinned==id)Pinned=-1;Pending.Clear();Tool=null;Sync();});}
            else if(SelectedConstruction>=0){int id=SelectedConstruction;Ask("Excluir esta construção? Seus pontos serão preservados.",()=>{Model.DeleteConstruction(id);SelectedConstruction=-1;Sync();});}
        }
        void Ask(string message,Action action){if(activeGrab!=null||Modal)return;Modal=true;confirmation=action;dialogText.text=message;foreach(var p in pages){var cg=p.GetComponent<CanvasGroup>();cg.interactable=cg.blocksRaycasts=false;}UIVisibility.Set(contextPage,false);foreach(var tab in tabs)tab.interactable=false;UIVisibility.Set(dialog,true);}
        void CloseModal(){Modal=false;confirmation=null;UIVisibility.Set(dialog,false);Page(currentPage);Present();}
        int currentPage;
        void Page(int page){if(Modal)return;currentPage=page;for(int i=0;i<pages.Length;i++){UIVisibility.Set(pages[i],i==page);Mark(tabs[i],i==page);}Present();}
        void PlaceContext(){contextCanvas.position=new Vector3(3.5f,2.05f,-.8f);}
        public void Dirty(){revision=-1;Sync();}
        public void Sync()
        {
            if(pointPrefab==null)return;
            if(Model.Point(hoverPoint)==null)hoverPoint=-1;
            if(Model.Construction(hoverConstruction)==null)hoverConstruction=-1;
            Pending.RemoveAll(id=>Model.Point(id)==null);
            if(Model.Point(SelectedPoint)==null)SelectedPoint=-1;if(Model.Construction(SelectedConstruction)==null)SelectedConstruction=-1;if(Model.Point(Pinned)==null)Pinned=-1;
            UpdateViews(pointViews,Model.Points.Select(p=>p.id).ToArray(),false);UpdateViews(constructionViews,Model.Constructions.Select(c=>c.id).ToArray(),true);revision=Model.Revision;
            if(preview!=null){var ids=Pending.ToList();if(hoverPoint>=0&&!ids.Contains(hoverPoint))ids.Add(hoverPoint);preview.positionCount=Tool.HasValue?ids.Count:0;if(Tool.HasValue)preview.SetPositions(ids.Select(id=>World(Model.Point(id).position)).ToArray());}
            UpdatePreview();Present();
        }
        void UpdatePreview()
        {
            var ids=Pending.ToList();if(hoverPoint>=0&&!ids.Contains(hoverPoint))ids.Add(hoverPoint);
            bool complete=Tool.HasValue&&ids.Count==CartesianModel.Required(Tool.Value);
            if(!complete){if(previewView!=null)previewView.gameObject.SetActive(false);return;}
            if(previewView==null)
            {
                var go=new GameObject("Construction preview geometry");go.transform.SetParent(transform);previewView=go.AddComponent<CartesianGeometryView>();previewView.lab=this;previewView.id=-1;previewView.construction=true;
                previewView.handle=Instantiate(constructionPrefab,go.transform);previewView.handle.enabled=false;previewView.handle.GetComponent<Collider>().enabled=false;previewView.handle.GetComponent<Renderer>().enabled=false;
                var label=new GameObject("Preview label").AddComponent<TextMeshPro>();label.transform.SetParent(go.transform);label.font=font;label.fontSize=.65f;label.color=selectedColor;label.alignment=TextAlignmentOptions.Center;label.rectTransform.sizeDelta=new Vector2(2.5f,.3f);label.overflowMode=TextOverflowModes.Ellipsis;previewView.label=label;
            }
            previewView.gameObject.SetActive(true);previewView.previewDefinition=new CartesianConstruction{id=-1,kind=Tool.Value,points=ids.ToArray()};previewView.Rebuild();
        }
        void UpdateViews(Dictionary<int,CartesianGeometryView> views,int[] ids,bool construction)
        {
            foreach(var id in views.Keys.Where(id=>!ids.Contains(id)).ToArray()){Destroy(views[id].gameObject);views.Remove(id);}
            foreach(int id in ids)
            {
                if(!views.TryGetValue(id,out var view))
                {
                    var go=new GameObject((construction?"Construction ":"Point ")+id);go.transform.SetParent(transform,false);view=go.AddComponent<CartesianGeometryView>();view.lab=this;view.id=id;view.construction=construction;
                    view.handle=Instantiate(construction?constructionPrefab:pointPrefab,go.transform);view.handle.lab=this;view.handle.id=id;view.handle.construction=construction;
                    var text=new GameObject("Scientific label").AddComponent<TextMeshPro>();text.transform.SetParent(go.transform);text.font=font;text.fontSize=.75f;text.color=Color.white;text.alignment=TextAlignmentOptions.Center;text.rectTransform.sizeDelta=new Vector2(2.4f,.28f);text.overflowMode=TextOverflowModes.Ellipsis;text.raycastTarget=false;view.label=text;views.Add(id,view);
                }
                view.Rebuild();
            }
        }
        static void Mark(Button b,bool selected){b.GetComponent<Image>().color=selected?new Color32(7,56,111,255):new Color32(40,78,160,255);var label=b.GetComponentInChildren<TMP_Text>();string s=label.text.Replace("● ","");label.text=(selected?"● ":"")+s;}
        void Present()
        {
            if(instruction==null)return;instruction.text=Tool.HasValue?$"{Kind(Tool.Value)} · {Pending.Count}/{CartesianModel.Required(Tool.Value)} pontos\nEscolhidos: {string.Join(", ",Pending.Select(id=>Model.Point(id)?.Name))}":"Crie pontos e construa a partir deles.\nY é a altura · limites: −5 a 5 · 1 u = 0,5 m";
            bool busy=activeGrab!=null;create.interactable=!busy&&Model.Points.Count<CartesianModel.MaxPoints;undo.interactable=!busy&&Model.CanUndo;redo.interactable=!busy&&Model.CanRedo;foreach(var b in tools)b.interactable=!busy;
            cancel.interactable=Tool.HasValue&&!busy;back.interactable=clear.interactable=!busy;stepButton.GetComponentInChildren<TMP_Text>().text="PASSO −/+: "+N(Step)+" u";
            for(int i=0;i<planeButtons.Length;i++)Mark(planeButtons[i],plane==new[]{-1,2,1,0}[i]);
            var states=new[]{ShowComponents,ShowPosition,ShowProjections,Snap,grids[0].activeSelf,grids[1].activeSelf,grids[2].activeSelf};for(int i=0;i<viewButtons.Length;i++)Mark(viewButtons[i],states[i]);viewButtons[3].GetComponentInChildren<TMP_Text>().text="MAGNÉTICO: "+(Snap?"LIGADO":"DESLIGADO");for(int g=0;g<3;g++)viewButtons[g+4].GetComponentInChildren<TMP_Text>().text="GRADE "+new[]{"XZ","XY","YZ"}[g]+"\n"+(grids[g].activeSelf?"VISÍVEL":"OCULTA");
            for(int i=0;i<tools.Length;i++)Mark(tools[i],Tool==(ConstructionKind)i);
            for(int i=0;i<challengeButtons.Length;i++)Mark(challengeButtons[i],activity==i);
            var point=Model.Point(SelectedPoint);var construction=Model.Construction(SelectedConstruction);bool has=point!=null||construction!=null;UIVisibility.Set(contextPage,!Modal);
            UIVisibility.Set(coordinateSection,point!=null,true);pin.interactable=point!=null;delete.interactable=has&&!busy;moveGroup.interactable=construction!=null&&!busy;Mark(moveGroup,MovingGroup);Mark(analysis,Analysis);Mark(pin,point!=null&&Pinned==point.id);
            if(point!=null)
            {
                setting=true;for(int axis=0;axis<3;axis++){bool locked=point.plane==axis||(axis==0?point.lockX:axis==1?point.lockY:point.lockZ);if(!fields[axis].isFocused)fields[axis].SetTextWithoutNotify(N(point.position[axis]));fields[axis].interactable=!busy&&!locked;minusButtons[axis].interactable=plusButtons[axis].interactable=!busy&&!locked;lockButtons[axis].interactable=!busy&&point.plane!=axis;Mark(lockButtons[axis],locked);}setting=false;
                details.text=$"Ponto {point.Name} = {V(point.position)}\nDistância à origem: {N(point.position.magnitude)} u\n{Model.Dependencies(point.id)} construção(ões) vinculada(s).";
                if(Analysis)details.text+="\nOP = "+V(point.position)+"\nComponentes: x·i + y·j + z·k";
            }
            else details.text=construction!=null?Describe(construction):"Selecione um ponto ou o marcador central de uma construção.\n\nPróximo ponto alterna inclusive entre pontos coincidentes.";
        }
        public string Describe(CartesianConstruction c)
        {
            var v=Model.Vertices(c);var a=v[0];var b=v[1];var d=b-a;string s=Kind(c.kind)+" "+(c.id+1)+" · "+string.Join(", ",c.points.Select(id=>Model.Point(id).Name));
            if(!Model.Validate(c.kind,c.points,out var error))return s+"\n"+error;
            switch(c.kind)
            {
                case ConstructionKind.Segment:s+=$"\nComprimento: {N(d.magnitude)} u\nPonto médio: {V((a+b)/2)}";break;
                case ConstructionKind.Line:case ConstructionKind.Ray:s+="\nDiretor: "+V(d)+"\nProlongamento além da área visível.";break;
                case ConstructionKind.Triangle:s+=$"\nÁrea: {N(CartesianModel.TriangleArea(a,b,v[2]))} u²\nPerímetro: {N(d.magnitude+Vector3.Distance(b,v[2])+Vector3.Distance(v[2],a))} u\nLados: {N(d.magnitude)}; {N(Vector3.Distance(b,v[2]))}; {N(Vector3.Distance(v[2],a))} u";break;
                case ConstructionKind.Plane:s+="\nPlano ilimitado · recorte de visualização.\nTrês pontos não alinhados definem o plano.";break;
                case ConstructionKind.Box:s+=$"\nDimensões: {N(Mathf.Abs(d.x))} × {N(Mathf.Abs(d.y))} × {N(Mathf.Abs(d.z))} u\nVolume: {N(Mathf.Abs(d.x*d.y*d.z))} u³";break;
                case ConstructionKind.Sphere:s+=$"\nRaio: {N(d.magnitude)} u\nÁrea: {N(4*Mathf.PI*d.sqrMagnitude)} u²\nVolume: {N(4f/3*Mathf.PI*Mathf.Pow(d.magnitude,3))} u³";break;
                case ConstructionKind.Tetrahedron:s+=$"\nVolume: {N(CartesianModel.TetraVolume(a,b,v[2],v[3]))} u³";break;
            }
            if(Analysis)
            {
                if(c.kind<=ConstructionKind.Ray)s+="\nP(t) = "+V(a)+" + t "+V(d)+(c.kind==ConstructionKind.Segment?"\n0 ≤ t ≤ 1":c.kind==ConstructionKind.Ray?"\nt ≥ 0":"\nt real");
                if(c.kind==ConstructionKind.Plane||c.kind==ConstructionKind.Triangle){var n=Vector3.Cross(d,v[2]-a).normalized;s+="\nNormal: "+V(n)+$"\n{N(n.x)}x + {N(n.y)}y + {N(n.z)}z = {N(Vector3.Dot(n,a))}";}
            }
            return s;
        }
        void CheckChallenge()
        {
            if(challengeText==null)return;bool done=false;string prompt;
            switch(activity)
            {
                case 1:prompt="Localize um ponto em (2; 1; −3).\nTolerância: 0,1 u de distância.";done=Model.Points.Any(p=>Vector3.Distance(p.position,new Vector3(2,1,-3))<=.1f);break;
                case 2:prompt="Construa um segmento de comprimento 5 u.\nTolerância: ±0,1 u.";done=Model.Constructions.Any(c=>c.kind==ConstructionKind.Segment&&Mathf.Abs(Vector3.Distance(Model.Point(c.points[0]).position,Model.Point(c.points[1]).position)-5)<=.1f);break;
                case 3:prompt="Crie um triângulo com área 6 u².\nTolerância: ±0,1 u².";done=Model.Constructions.Any(c=>c.kind==ConstructionKind.Triangle&&Mathf.Abs(CartesianModel.TriangleArea(Model.Point(c.points[0]).position,Model.Point(c.points[1]).position,Model.Point(c.points[2]).position)-6)<=.1f);break;
                case 4:prompt="Construa uma caixa de volume 8 u³.\nTolerância: ±0,1 u³.";done=Model.Constructions.Any(c=>{if(c.kind!=ConstructionKind.Box)return false;var d=Model.Point(c.points[1]).position-Model.Point(c.points[0]).position;return Mathf.Abs(Mathf.Abs(d.x*d.y*d.z)-8)<=.1f;});break;
                default:prompt="Exploração livre\nCrie, conecte e manipule. As medidas acompanham os pontos.\n\nAgarre um ponto com o grip. No raio, mova o analógico para ajustar a distância.\nSelecione o marcador central para consultar uma construção.";break;
            }
            string text=prompt+(activity>0?(done?"\n\nOBJETIVO ATINGIDO":"\n\nEm investigação…"):"");if(challengeText.text!=text)challengeText.text=text;
        }
        void OnDestroy(){if(selectSound!=null)Destroy(selectSound);}
    }
}
