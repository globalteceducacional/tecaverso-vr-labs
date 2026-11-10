using System;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Tecaverso.Liquids;
using Tecaverso.UI;

namespace Tecaverso.Labs.Chemistry.Reactions
{
    public sealed class ReactionsLab : MonoBehaviour
    {
        public ReactionsBench bench;
        public void SetBenchSession(ReactionSession session){Session=session;lastVisualExtent=0;}
        public ReagentTool[] tools;
        public LiquidPresentation receiver;
        public Transform opening,powderDish,sediment,powderStockVisual;
        public float openingRadius=.22f;
        public GameObject powderStation;
        public ParticleSystem bubbles,precipitate;
        public Button[] cases,minus,plus;
        public Button pause,reset,recover,back,confirm,cancel;
        public Toggle molecularToggle;
        public GameObject molecularView,modal;
        public CanvasGroup controls,equationControls;
        public TMP_Text title,readings,hint,balance,modalText,powderLabel;
        public TMP_Text[] coefficientLabels,speciesLabels,amountLabels;
        public Transform[] atomGroups;
        public Material[] atomMaterials;
        public Mesh atomMesh;
        public ReactionSession Session {get;private set;}
        public bool Modal=>bench?bench.Modal:UIVisibility.IsVisible(modal);
        int[] coefficients;
        int pending=-1;
        float refreshTimer;
        double lastVisualExtent;
        bool leaving;
        static readonly Color ClearLiquid=new Color(.53f,.79f,.88f,1);

        void Awake()
        {
            if(bench){bench.Initialize(this);return;}
            Session=new ReactionSession();
            for(int i=0;i<cases.Length;i++){int k=i;cases[i].onClick.AddListener(()=>RequestCase(k));}
            for(int i=0;i<plus.Length;i++){int k=i;plus[i].onClick.AddListener(()=>ChangeCoefficient(k,1));minus[i].onClick.AddListener(()=>ChangeCoefficient(k,-1));}
            pause.onClick.AddListener(TogglePause);reset.onClick.AddListener(()=>RequestCase((int)Session.Definition.Kind));
            recover.onClick.AddListener(Recover);back.onClick.AddListener(Back);
            confirm.onClick.AddListener(ConfirmReset);cancel.onClick.AddListener(CancelReset);
            molecularToggle.onValueChanged.AddListener(ShowMolecules);
            modal.SetActive(false);SetCase(0);
        }
        void Update()
        {
            refreshTimer+=Time.deltaTime;
            if(refreshTimer>=.1f){refreshTimer=0;Refresh();}
        }
        public bool ReceiverHit(Vector3 origin,out Vector3 end)
        {
            float dy=origin.y-opening.position.y;
            var offset=origin-opening.position;offset.y=0;
            bool hit=dy>=0&&dy<=1.2f&&offset.sqrMagnitude<=openingRadius*openingRadius;
            end=hit?new Vector3(origin.x,Mathf.Min(opening.position.y,receiver.SurfaceY),origin.z):new Vector3(origin.x,Mathf.Max(.06f,origin.y-1.8f),origin.z);
            return hit;
        }
        public void Reacted(ReactionSession.Transfer transfer)
        {
            if(transfer.Spilled>0)hint.text="Derramamento registrado. Aponte o bico para a abertura; perdas não reagem.";
            else if(transfer.Accepted>0)hint.text=transfer.Reacted>0?Session.Definition.Observation:"Reagente recebido. Adicione o outro para observar a transformação.";
        }
        public void TogglePause()
        {
            Session.Paused=!Session.Paused;
            if(Session.Paused){foreach(var t in tools)t.StopEffects();bubbles.Pause();precipitate.Pause();}
            else{bubbles.Play();precipitate.Play();}
            Refresh();
        }
        public void RequestCase(int kind)
        {
            if(bench){bench.RequestReset();return;}
            if(Modal)return;
            if(tools.Any(t=>t.Held)){hint.text="Solte os frascos e a colher antes de reiniciar ou trocar de caso.";return;}
            if(!Session.Used){SetCase(kind);return;}
            pending=kind;modalText.text="Reiniciar a experiência?\nA mistura e as perdas atuais serão descartadas; os estoques serão repostos.";
            controls.interactable=controls.blocksRaycasts=false;equationControls.interactable=equationControls.blocksRaycasts=false;
            UIVisibility.Set(modal,true);
        }
        public void ConfirmReset(){if(pending<0)return;int k=pending;CancelReset();SetCase(k);}
        public void CancelReset(){pending=-1;UIVisibility.Set(modal,false);controls.interactable=controls.blocksRaycasts=true;equationControls.interactable=equationControls.blocksRaycasts=true;}
        void SetCase(int kind)
        {
            Session.Reset((ReactionKind)kind);lastVisualExtent=0;
            coefficients=Enumerable.Repeat(1,Session.Definition.Species.Length).ToArray();
            powderStation.SetActive(kind==0);tools[1].gameObject.SetActive(kind!=0);tools[2].gameObject.SetActive(kind==0);
            foreach(var t in tools){t.StopEffects();t.ReturnHome();}
            bubbles.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);precipitate.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
            title.text="REAÇÕES QUÍMICAS · "+Session.Definition.Title.ToUpperInvariant();
            hint.text=kind==0?"Pegue o frasco e incline sobre o béquer. Passe a colher no bicarbonato e vire-a sobre a abertura.":"Pegue um frasco e incline sobre o béquer. Misture o segundo e compare as quantidades restantes.";
            BuildAtoms();RefreshBalance();Refresh();ShowMolecules(molecularToggle.isOn);
        }
        public void Recover(){foreach(var t in tools)t.ReturnHome();hint.text="Instrumentos soltos recolhidos. A mistura e os estoques foram preservados.";}
        public void ShowMolecules(bool show)=>molecularView.SetActive(show);
        public void ChangeCoefficient(int index,int delta)
        {
            if(Modal||index>=coefficients.Length)return;
            coefficients[index]=Mathf.Clamp(coefficients[index]+delta,1,9);RefreshBalance();
        }
        void RefreshBalance()
        {
            var d=Session.Definition;var text=new StringBuilder();
            for(int i=0;i<coefficientLabels.Length;i++)
            {
                bool valid=i<coefficients.Length;
                coefficientLabels[i].transform.parent.gameObject.SetActive(valid);
                if(valid){coefficientLabels[i].text=$"{coefficients[i]} {d.Species[i].Formula}";minus[i].interactable=coefficients[i]>1;plus[i].interactable=coefficients[i]<9;}
            }
            foreach(string e in d.Elements)
            {
                int left=0,right=0;for(int i=0;i<coefficients.Length;i++){if(i<2)left+=coefficients[i]*d.AtomCount(i,e);else right+=coefficients[i]*d.AtomCount(i,e);}
                text.Append($"{e}: {left} / {right}   ");
            }
            balance.text=(d.IsBalanced(coefficients)?"BALANCEADA · átomos conservados":"AJUSTE OS COEFICIENTES · átomos: reagentes / produtos")+"\n"+text;
        }
        public void Refresh()
        {
            if(bench){bench.Refresh();return;}
            if(Session==null)return;
            var d=Session.Definition;
            foreach(var t in tools)
            {
                if(t.liquid){t.liquid.fill=(float)(Session.SourceMl[t.source]/ReactionSession.StockMl)*.9f;t.liquid.paused=Session.Paused||Modal;}
                if(t.spoon){t.label.text=$"COLHER · {Session.SpoonGrams:0.00} g";t.powderVisual.gameObject.SetActive(Session.SpoonGrams>.001);}
                else t.label.text=$"{d.Species[t.source].Formula}\n{d.Species[t.source].Name}\n0,10 mol/L · {Session.SourceMl[t.source]:0.0} mL";
            }
            powderLabel.text=$"NaHCO₃ · bicarbonato\nEstoque: {Session.PowderStock:0.00} g\nPasse a colher aqui · até 1 g";
            if(powderStockVisual){powderStockVisual.gameObject.SetActive(Session.PowderStock>.001);powderStockVisual.localScale=new Vector3(.29f,.018f*(float)(Session.PowderStock/ReactionSession.PowderGrams),.29f);}
            receiver.fill=(float)(Session.VolumeMl/ReactionSession.CapacityMl)*.9f;receiver.paused=Session.Paused||Modal;
            float pink=d.Kind==ReactionKind.Neutralization&&!double.IsNaN(Session.PH)?Mathf.InverseLerp(8.2f,10,(float)Session.PH):0;
            receiver.tint=Color.Lerp(ClearLiquid,new Color(.9f,.08f,.46f),pink);
            bool solid=d.Kind==ReactionKind.Precipitation&&Session.ExtentMol>1e-9;
            if(solid)receiver.tint=Color.Lerp(ClearLiquid,new Color(.9f,.92f,.94f),Mathf.Clamp01((float)Session.ExtentMol*180));
            sediment.gameObject.SetActive(solid);
            if(solid){float h=Mathf.Clamp((float)Session.ExtentMol*2,.002f,.04f);sediment.localScale=new Vector3(.40f,h,.40f);sediment.localPosition=new Vector3(0,-.317f+h,0);}
            if(!Session.Paused&&!Modal&&Session.ExtentMol>lastVisualExtent)
            {
                int count=Mathf.Clamp(Mathf.CeilToInt((float)(Session.ExtentMol-lastVisualExtent)*15000),1,24);
                var effect=d.Kind==ReactionKind.Gas?bubbles:d.Kind==ReactionKind.Precipitation?precipitate:null;
                if(effect)
                {
                    float bottom=receiver.transform.position.y-.317f;
                    float surface=receiver.SurfaceY;
                    float y=Mathf.Clamp(surface-.10f,bottom+.006f,Mathf.Max(bottom+.006f,surface-.005f));
                    effect.transform.position=new Vector3(opening.position.x,y,opening.position.z);
                    // In a narrow beaker, trajectories must stay vertical; particles are visual, not collision chemistry.
                    var main=effect.main;
                    if(d.Kind==ReactionKind.Gas)main.startLifetime=Mathf.Clamp((surface+.06f-y)/.23f,.08f,1.1f);
                    else main.startLifetime=Mathf.Clamp((y-bottom)/.07f,.04f,1.6f);
                    if(!effect.isPlaying)effect.Play();effect.Emit(count);
                }
                lastVisualExtent=Session.ExtentMol;
            }
            string excess=Session.ExtentMol<=1e-10?"Sem reação ainda":Session.RemainingMol[0]>1e-8?$"Excesso: {d.Species[0].Formula} · limitante: {d.Species[1].Formula}":Session.RemainingMol[1]>1e-8?$"Excesso: {d.Species[1].Formula} · limitante: {d.Species[0].Formula}":"Proporção estequiométrica · sem excesso";
            readings.text=$"{Session.VolumeMl:0.0} / 500 mL  ·  {(Session.Paused?"PAUSADO":"EM EXPLORAÇÃO")}\nPerdas: {Session.SpilledMl:0.0} mL + {Session.SpilledPowder:0.00} g\n{excess}";
            if(d.Kind==ReactionKind.Neutralization)readings.text+=$"\nFenolftaleína no béquer · pH: {(double.IsNaN(Session.PH)?"—":Session.PH.ToString("0.00"))}";
            if(solid)readings.text+=$"\nCaCO₃ formado: {Session.ExtentMol*ReactionSession.CarbonateMolarMass:0.000} g";
            pause.GetComponentInChildren<TMP_Text>().text=Session.Paused?"Continuar":"Pausar";
            for(int i=0;i<cases.Length;i++)cases[i].GetComponentInChildren<TMP_Text>().text=(i==(int)d.Kind?"• ":"")+ReactionDefinition.All[i].Title;
            for(int i=0;i<amountLabels.Length;i++)if(i<d.Species.Length)
            {
                double amount=i<2?Session.RemainingMol[i]:Session.ProductsMol[i-2];
                amountLabels[i].text=$"{amount*1000:0.00} mmol\n{(i<2?"RESTANTE":d.Kind==ReactionKind.Gas&&i==4?"LIBERADO":"FORMADO")}";
            }
        }
        void BuildAtoms()
        {
            var d=Session.Definition;
            string[] elements={"H","C","O","Na","Cl","Ca"};
            for(int i=0;i<atomGroups.Length;i++)
            {
                foreach(Transform child in atomGroups[i])Destroy(child.gameObject);
                bool valid=i<d.Species.Length;atomGroups[i].parent.gameObject.SetActive(valid);
                if(!valid)continue;
                speciesLabels[i].text=$"{d.Species[i].Formula}\n{d.Species[i].Name}";
                int k=0;
                foreach(var atom in d.Species[i].Atoms)for(int n=0;n<atom.Value;n++)
                {
                    var go=new GameObject(atom.Key,typeof(MeshFilter),typeof(MeshRenderer));go.transform.SetParent(atomGroups[i],false);
                    go.transform.localPosition=new Vector3((k%4-1.5f)*.075f,(k/4)*.075f,0);go.transform.localScale=Vector3.one*.055f;
                    go.GetComponent<MeshFilter>().sharedMesh=atomMesh;go.GetComponent<MeshRenderer>().sharedMaterial=atomMaterials[Array.IndexOf(elements,atom.Key)];k++;
                }
            }
        }
        public void Back(){if(bench){bench.RequestExit();return;}if(leaving||Modal)return;leaving=true;back.interactable=false;SceneManager.LoadSceneAsync("Assets/_Project/Hub/Scenes/Hub.unity");}
    }
}
