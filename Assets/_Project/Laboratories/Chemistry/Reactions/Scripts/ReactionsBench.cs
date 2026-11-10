using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Tecaverso.UI;

namespace Tecaverso.Labs.Chemistry.Reactions
{
    /// <summary>Physical A/B selection and compact consultation. Uses the existing liquid/VFX assets.</summary>
    public sealed class ReactionsBench : MonoBehaviour
    {
        public ReagentTool[] instruments;
        public ReactionThermometer thermometer;
        public ReagentSocket[] sockets;
        public TMP_Text[] socketLabels;
        public TMP_Text status,details,confirmationText;
        public Button consult,reset,back,close,confirm,cancel;
        public GameObject sheet,confirmation;
        public CanvasGroup commandGroup;
        public ReactionWorkbench Model {get;private set;}
        public bool Modal=>confirmation&&UIVisibility.IsVisible(confirmation);
        public bool SheetOpen=>sheet&&UIVisibility.IsVisible(sheet);
        public bool AnyHeld=>instruments.Any(t=>t&&t.Held)||(thermometer&&thermometer.Held);
        ReactionsLab lab;
        bool pendingExit,restoreSheet,leaving,resetting;
        string notice;
        float noticeUntil;
        double lastExtent;
        float lastPrecipitation=-100;
        float modalStarted;
        readonly Dictionary<ReagentTool,float> abandoned=new Dictionary<ReagentTool,float>();
        MaterialPropertyBlock block;
        public static readonly Color ClearLiquid=new Color(.92f,.92f,.92f,1);

        public void Initialize(ReactionsLab owner)
        {
            lab=owner;block=new MaterialPropertyBlock();Model=new ReactionWorkbench();lab.SetBenchSession(null);
            UIVisibility.Set(sheet,false);UIVisibility.Set(confirmation,false);
            consult.onClick.AddListener(OpenSheet);close.onClick.AddListener(CloseSheet);
            reset.onClick.AddListener(RequestReset);back.onClick.AddListener(RequestExit);
            confirm.onClick.AddListener(Confirm);cancel.onClick.AddListener(Cancel);
            if(thermometer)thermometer.selectEntered.AddListener(_=>CloseSheet());
            foreach(var t in instruments)
            {
                abandoned[t]=0;
                t.selectEntered.AddListener(e=>{if(!(e.interactorObject is XRSocketInteractor))CloseSheet();});
                if(t.indicator)t.activated.AddListener(_=>UseIndicator(t));
            }
            ResetPresentation();Refresh();
        }
        public bool CanDock(int slot,ReagentTool t)
        {
            if(Model==null||Modal||resetting||!t||t.spoon||t.indicator||ReactionWorkbench.Find(t.reagentId)==null)return false;
            if(Model.Ready)return t.reagentId==(slot==0?Model.A:Model.B);
            return slot==0?(Model.A==null||Model.A==t.reagentId):ReactionWorkbench.Recipe(Model.A,t.reagentId)!=null;
        }
        public void ExplainDock(int slot,ReagentTool t)
        {
            if(CanDock(slot,t))Notify("Solte o recipiente sobre o suporte "+(slot==0?"A":"B")+" para encaixar.");
            else Notify(Model.Ready?"O par já está definido. Use Nova mistura para escolher outros reagentes.":slot==1&&Model.A==null?"Escolha o reagente A primeiro.":"Esta combinação não está disponível neste laboratório.");
        }
        public void Dock(int slot,ReagentTool t)
        {
            if(!CanDock(slot,t))return;
            if(!Model.Ready)
            {
                if(slot==0)Model.SelectA(t.reagentId);
                else if(Model.SelectB(t.reagentId)){lab.SetBenchSession(Model.Session);ResetPresentation();}
            }
            Notify(Model.Ready?"Par pronto. Retire os recipientes e dose A e B no béquer.":"A escolhido. Encaixe em B um dos parceiros destacados.");Refresh();
        }
        public void Undock(int slot){if(Model==null)return;if(slot==0&&!Model.Ready){Model.RemoveA();Notify("Escolha um reagente no suporte A.");}Refresh();}
        public void Notify(string text){notice=text;noticeUntil=Time.unscaledTime+4;}
        public void OpenSheet(){if(Modal||AnyHeld)return;UpdateDetails();UIVisibility.Set(sheet,true);Refresh();}
        public void CloseSheet(){UIVisibility.Set(sheet,false);}
        public void RequestReset()=>Request(false);
        public void RequestExit()=>Request(true);
        void Request(bool exit)
        {
            if(Modal||leaving||resetting)return;
            if(AnyHeld){Notify("Devolva os instrumentos antes de reiniciar ou sair.");return;}
            if(!Model.Used){if(exit)Leave();else ResetAll();return;}
            pendingExit=exit;restoreSheet=SheetOpen;CloseSheet();
            confirmationText.text=exit?"Voltar ao Hub?\nA mistura atual será descartada.":"Iniciar nova mistura?\nO conteúdo será descartado e todos os estoques serão repostos.";
            confirm.GetComponentInChildren<TMP_Text>().text=exit?"Sair":"Confirmar reinício";
            commandGroup.interactable=commandGroup.blocksRaycasts=false;
            modalStarted=Time.time;Model.Session.Paused=true;foreach(var t in instruments)t.StopEffects();lab.bubbles.Pause();lab.precipitate.Pause();
            UIVisibility.Set(confirmation,true);Refresh();
        }
        public void Cancel()
        {
            if(Modal)lastPrecipitation+=Time.time-modalStarted;
            UIVisibility.Set(confirmation,false);commandGroup.interactable=commandGroup.blocksRaycasts=true;
            if(Model.Session!=null)Model.Session.Paused=false;
            if(lab.bubbles.isPaused)lab.bubbles.Play();if(lab.precipitate.isPaused)lab.precipitate.Play();
            if(restoreSheet)UIVisibility.Set(sheet,true);restoreSheet=false;Refresh();
        }
        public void Confirm()
        {
            if(!Modal||AnyHeld)return;bool exit=pendingExit;Cancel();if(exit)Leave();else ResetAll();
        }
        public void ResetAll()
        {
            if(AnyHeld||resetting)return;
            resetting=true;commandGroup.interactable=commandGroup.blocksRaycasts=false;
            foreach(var socket in sockets)
            {
                socket.socketActive=false;
                foreach(var selected in socket.interactablesSelected.ToArray())socket.interactionManager.SelectExit(socket,selected);
            }
            Model.Reset();lab.SetBenchSession(null);CloseSheet();ResetPresentation();
            foreach(var t in instruments){t.StopEffects();t.ReturnHome();abandoned[t]=0;}
            if(thermometer)thermometer.ReturnHome();
            StartCoroutine(FinishReset());
            Notify("Escolha A e depois B. Estoques completos.");Refresh();
        }
        System.Collections.IEnumerator FinishReset()
        {
            // Let XRI finish detach and PhysX clear stale socket overlaps before enabling selection again.
            yield return new WaitForFixedUpdate();yield return new WaitForEndOfFrame();
            foreach(var t in instruments)t.ReturnHome();if(thermometer)thermometer.ReturnHome();Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();
            resetting=false;foreach(var socket in sockets)socket.socketActive=true;
            commandGroup.interactable=commandGroup.blocksRaycasts=true;Refresh();
        }
        void Leave(){leaving=true;back.interactable=false;SceneManager.LoadSceneAsync("Assets/_Project/Hub/Scenes/Hub.unity");}
        void UseIndicator(ReagentTool t)
        {
            if(Modal||!t.Held)return;
            if(!lab.ReceiverHit(t.mouth.position,out _)){Notify("Use o conta-gotas sobre a abertura do béquer.");return;}
            if(!Model.AddIndicator()){Notify("Indicador disponível somente para HCl + NaOH.");return;}
            Notify("Fenolftaleína adicionada em traço. Indicador presente.");Refresh();
        }
        public void UpdateTool(ReagentTool t,float dt)
        {
            if(Model==null||resetting)return;
            if(t.isSelected)abandoned[t]=0;
            else{abandoned[t]+=dt;if(abandoned[t]>10||t.transform.position.y<.15f){t.ReturnHome();abandoned[t]=0;}}
            bool flowing=false;
            if(t.Held&&!Modal&&!t.indicator&&!t.solidPot)
            {
                float upright=Vector3.Dot(t.transform.up,Vector3.up);
                if(t.spoon&&upright>.7f)
                {
                    var pot=instruments.FirstOrDefault(p=>p.solidPot&&p.reagentId==t.reagentId);
                    if(pot&&Vector3.Distance(t.mouth.position,pot.collectionPoint.position)<.16f)
                    {
                        string reason=Model.BlockReason(t.reagentId);
                        if(reason!=null)Notify(reason);else if(Model.LoadSpoon(t.reagentId)>0)Notify("Dose coletada. Incline a colher sobre o béquer.");
                    }
                }
                if(upright<.4f&&(t.spoon?Model.Spoon(t.reagentId)>0:t.liquid&&t.liquid.ReachesMouth(t.mouth.position)))
                {
                    string reason=Model.BlockReason(t.reagentId);
                    if(reason!=null)Notify(reason);
                    else
                    {
                        bool hit=lab.ReceiverHit(t.mouth.position,out Vector3 end);
                        var transfer=t.spoon?Model.PourPowder(t.reagentId,dt*.8,hit):Model.PourLiquid(t.reagentId,dt*40*Mathf.Clamp01(1-upright),hit);
                        flowing=transfer.Removed>0;
                        if(flowing)
                        {
                            t.DrawStream(end);
                            Notify(transfer.Spilled>0?"Perda registrada. Dose sobre a abertura; o béquer comporta 500 mL.":transfer.Reacted>0?Model.Session.Definition.Observation:"Reagente recebido. Adicione o outro do par.");
                        }
                    }
                }
            }
            if(t.stream)t.stream.enabled=flowing&&!t.spoon;
            if(t.droplets)
            {
                var emission=t.droplets.emission;emission.enabled=flowing;
                if(flowing&&!t.droplets.isPlaying)t.droplets.Play();
                if(!flowing&&t.droplets.isPlaying)t.droplets.Stop(true,ParticleSystemStopBehavior.StopEmitting);
            }
        }
        void ResetPresentation()
        {
            lastExtent=0;lastPrecipitation=-100;lab.bubbles.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);lab.precipitate.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
            lab.receiver.ResetMotion();lab.sediment.gameObject.SetActive(false);
        }
        public void Refresh()
        {
            if(Model==null||!lab)return;
            for(int i=0;i<2;i++){var r=ReactionWorkbench.Find(i==0?Model.A:Model.B);socketLabels[i].text=(i==0?"A":"B")+"\n"+(r==null?"Encaixe um reagente":r.Formula+" · "+r.Name);}
            foreach(var t in instruments)
            {
                if(t.indicator){t.label.text="INDICADOR\nFenolftaleína · traço\nHCl + NaOH\nAtivar sobre o béquer";if(t.liquid)t.liquid.paused=Modal;continue;}
                var r=ReactionWorkbench.Find(t.reagentId);if(r==null)continue;
                bool partner=!Model.Ready&&Model.A!=null&&ReactionWorkbench.Recipe(Model.A,r.Id)!=null;
                if(t.spoon)
                {
                    double grams=Model.Spoon(r.Id);if(t.label)t.label.text=$"{r.Formula} · COLHER\n{grams:0.00} g";
                    if(t.powderVisual)t.powderVisual.gameObject.SetActive(grams>.001);
                }
                else
                {
                    t.label.text=$"{r.Formula} · {r.Name}\n"+(r.Solid?$"Estoque: {Model.Stock(r.Id):0.00} g · Colher: {Model.Spoon(r.Id):0.00} g":$"0,10 mol/L · Estoque: {Model.Stock(r.Id):0.0} mL")+(partner?"\n✓ B disponível":"");
                    t.label.color=partner?new Color(.06f,.22f,.48f):new Color(.08f,.08f,.08f);
                    var rim=t.transform.Find("Aro de identificação");if(rim&&rim.TryGetComponent<Renderer>(out var renderer))
                    {renderer.GetPropertyBlock(block);block.SetColor("_BaseColor",partner?new Color(.30f,.83f,1):new Color(.157f,.306f,.627f));renderer.SetPropertyBlock(block);}
                    if(t.solidPot&&t.powderVisual){t.powderVisual.gameObject.SetActive(Model.Stock(r.Id)>.001);t.powderVisual.localScale=new Vector3(.16f,.05f*(float)(Model.Stock(r.Id)/6),.16f);}
                    if(t.liquid&&!t.solidPot){t.liquid.fill=(float)(Model.Stock(r.Id)/300)*.9f;t.liquid.paused=Modal;}
                }
            }
            reset.GetComponentInChildren<TMP_Text>().text=Model.Used?"Nova mistura":"Trocar par";reset.interactable=Model.A!=null&&!Modal;
            string baseline=Model.Ready?"Par definido. Dose A e B; consulte para entender o resultado.":Model.A==null?"1. Escolha A → 2. Escolha B\nEncaixe os recipientes nos suportes.":"Encaixe em B um dos parceiros destacados.";
            status.text=Time.unscaledTime<noticeUntil?notice:baseline;
            UIVisibility.Set(status.transform.parent.gameObject,!SheetOpen&&!Modal);
            PresentReaction();if(SheetOpen)UpdateDetails();
        }
        void PresentReaction()
        {
            var s=Model.Session;
            lab.receiver.fill=s==null?0:(float)(s.VolumeMl/500)*.9f;lab.receiver.paused=Modal;
            lab.receiver.tint=ClearLiquid;lab.receiver.opacity=.16f;
            if(s==null){lab.sediment.gameObject.SetActive(false);return;}
            var d=s.Definition;
            float pink=s.IndicatorPresent&&!double.IsNaN(s.PH)?Mathf.InverseLerp(8.2f,10,(float)s.PH):0;
            lab.receiver.tint=Color.Lerp(ClearLiquid,new Color(.9f,.08f,.46f),pink);
            lab.receiver.opacity=Mathf.Lerp(.16f,.50f,pink);
            if(d.Kind==ReactionKind.BlueSolution){float blue=Mathf.Clamp01((float)(s.ExtentMol/Math.Max(.00001,s.VolumeMl*.001))/.1f);lab.receiver.tint=Color.Lerp(ClearLiquid,new Color(.12f,.48f,.91f),blue);lab.receiver.opacity=Mathf.Lerp(.16f,.55f,blue);}
            bool precip=d.Kind==ReactionKind.Precipitation&&s.ExtentMol>1e-9;
            if(precip)
            {
                if(s.ExtentMol>lastExtent)lastPrecipitation=Time.time;
                // Transient suspension clears, leaving a distinct solid deposit in clear supernatant.
                float cloud=Mathf.Exp(-((Modal?modalStarted:Time.time)-lastPrecipitation)/3f)*Mathf.Clamp01((float)s.ExtentMol*180);
                lab.receiver.tint=Color.Lerp(ClearLiquid,Color.white,cloud);lab.receiver.opacity=.16f+.25f*cloud;
            }
            double residue=d.PowderSecond?s.RemainingMol[1]*d.SolidMolarMass:0;
            lab.sediment.gameObject.SetActive(precip||residue>1e-6);
            if(precip||residue>1e-6)
            {
                float h=Mathf.Clamp(precip?(float)s.ExtentMol*2:(float)residue*.006f,.002f,.04f);
                lab.sediment.localScale=new Vector3(.40f,h,.40f);lab.sediment.localPosition=new Vector3(0,-.317f+h,0);
                var renderer=lab.sediment.GetComponent<Renderer>();renderer.GetPropertyBlock(block);block.SetColor("_BaseColor",d.Kind==ReactionKind.BlueSolution?new Color(.04f,.04f,.04f):new Color(.92f,.94f,.95f));renderer.SetPropertyBlock(block);
            }
            if(!Modal&&s.ExtentMol>lastExtent)
            {
                int count=Mathf.Clamp(Mathf.CeilToInt((float)(s.ExtentMol-lastExtent)*15000),1,24);
                var effect=d.Kind==ReactionKind.Gas?lab.bubbles:d.Kind==ReactionKind.Precipitation?lab.precipitate:null;
                if(effect)
                {
                    float bottom=lab.receiver.transform.position.y-.317f,surface=lab.receiver.SurfaceY;
                    float y=Mathf.Clamp(surface-.10f,bottom+.006f,Mathf.Max(bottom+.006f,surface-.005f));
                    effect.transform.position=new Vector3(lab.opening.position.x,y,lab.opening.position.z);
                    var main=effect.main;main.startLifetime=d.Kind==ReactionKind.Gas?Mathf.Clamp((surface+.06f-y)/.23f,.08f,1.1f):Mathf.Clamp((y-bottom)/.07f,.04f,1.6f);
                    if(!effect.isPlaying)effect.Play();effect.Emit(count);
                }
                lastExtent=s.ExtentMol;
            }
        }
        void UpdateDetails()
        {
            if(!Model.Ready){details.text="DOIS REAGENTES POR MISTURA\n\nEncaixe o primeiro recipiente em A. Os parceiros disponíveis recebem destaque.\n\nEncaixe um deles em B; depois retire os frascos e dose no béquer.\n\nPode alternar doses de A e B. Para outros reagentes, use Nova mistura.\n\n12 reagentes · 12 receitas locais. Combinações não cadastradas são limites deste modelo.";return;}
            var s=Model.Session;var d=s.Definition;var text=new StringBuilder();
            text.AppendLine(d.Title.ToUpperInvariant()).AppendLine(d.Observation).AppendLine();
            for(int i=0;i<d.Species.Length;i++){if(i>0)text.Append(i==2?" → ":" + ");if(d.Stoichiometry[i]!=1)text.Append(d.Stoichiometry[i]+" ");text.Append(d.Species[i].Formula);}text.AppendLine().AppendLine();
            text.AppendLine($"Volume recebido: {s.VolumeMl:0.0} / 500 mL");
            for(int i=0;i<2;i++)text.AppendLine($"{d.Species[i].Formula}: {(i==1&&d.PowderSecond?s.AddedMol[i]*d.SolidMolarMass:s.AddedMol[i]*10000):0.00} {(i==1&&d.PowderSecond?"g":"mL")} recebidos");
            if(d==ReactionDefinition.All[9])
            {
                text.AppendLine($"Restam {(2*s.RemainingMol[0]-s.RemainingMol[1])*1000:0.00} mmol de equivalentes (ácidos + / básicos −).");
                text.AppendLine("Balanço de neutralização completa; espécies bissulfato/sulfato e pH não modelados.");
            }
            else
            {
                string excess=s.ExtentMol==0?"Sem reação ainda":s.RemainingMol[0]>1e-8?"Limitante: "+d.Species[1].Formula:s.RemainingMol[1]>1e-8?"Limitante: "+d.Species[0].Formula:"Proporção estequiométrica · sem excesso";
                text.AppendLine(excess);
                for(int i=0;i<2;i++)if(s.RemainingMol[i]>1e-8)text.AppendLine($"Restante {d.Species[i].Formula}: {s.RemainingMol[i]*1000:0.00} mmol");
            }
            for(int i=0;i<s.ProductsMol.Length;i++)text.AppendLine($"{d.Species[i+2].Formula}: {s.ProductsMol[i]*1000:0.00} mmol {(d.Species[i+2].Formula=="CO₂"?"liberados":"formados")}");
            if(d.Kind==ReactionKind.Precipitation)text.AppendLine($"CaCO₃: {s.ExtentMol*ReactionSession.CarbonateMolarMass:0.000} g");
            if(d.SupportsPH)text.AppendLine($"pH ideal (25 °C): {(double.IsNaN(s.PH)?"—":s.PH.ToString("0.00"))} · indicador {(s.IndicatorPresent?"presente":"não adicionado")}");
            if(s.HasThermalModel)text.AppendLine("Temperatura: use a sonda. Estimativa sem perdas térmicas; reagentes a 25 °C.");
            else text.AppendLine("Modelo térmico indisponível para este par.");
            text.AppendLine($"Perdas: {s.SpilledMl:0.0} mL + {s.SpilledPowder:0.00} g");
            text.Append("Modelo estequiométrico; não calcula cinética.\nÁgua formada exclui solvente; sais aquosos dissociados.");details.text=text.ToString();
        }
    }
}
