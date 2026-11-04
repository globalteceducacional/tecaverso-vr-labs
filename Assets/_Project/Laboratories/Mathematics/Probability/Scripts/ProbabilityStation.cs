using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.XR.Content.Interaction;
using Tecaverso.UI;
namespace Tecaverso.Labs.Probability
{
    public sealed class ProbabilityStation:MonoBehaviour
    {
        public Experiment kind;
        public ProbabilityLab lab;
        public XRLever lever;
        public Transform[] bars, theoryMarks, population;
        public TMP_Text[] categories;
        public TMP_Text result,summary,status,composition;
        public Button once,run,pause,stop,reset;
        public Button[] batches,config;
        public Toggle theoretical,replacement;
        public GameObject controls;
        public Transform display;
        public Quaternion[] resultRotations;
        public TMP_Text specimenLabel;
        public Material[] categoryMaterials;
        public ProbabilitySeries Series { get; private set; }
        public int Remaining { get; private set; }
        public bool Paused { get; private set; }
        int batch=10,requested,completed;
        float tick,animation;
        Quaternion finalRotation;
        bool armed=true;
        Vector3 trayPosition;
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
        readonly List<Action> cleanup=new();
        void Awake(){Series=new ProbabilitySeries(kind);trayPosition=display.position;grab=display.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();}
        void OnEnable()
        {
            Bind(once,Single);Bind(run,StartBatch);Bind(pause,TogglePause);Bind(stop,()=>{Remaining=0;Refresh();});Bind(reset,()=>lab.Ask("Iniciar nova série nesta estação?",()=>{Remaining=0;completed=0;requested=0;animation=0;Series.Reset();result.text="Último resultado: —";if(specimenLabel)specimenLabel.text="—";Refresh();},Series.Total>0,controls.transform));
            for(int i=0;i<batches.Length;i++){int size=new[]{10,100,1000}[i];Bind(batches[i],()=>{batch=size;Refresh();});}
            UnityAction activated=()=>{if(armed){armed=false;Single();}};UnityAction rearm=()=>armed=true;
            lever.onLeverActivate.AddListener(activated);lever.onLeverDeactivate.AddListener(rearm);cleanup.Add(()=>{lever.onLeverActivate.RemoveListener(activated);lever.onLeverDeactivate.RemoveListener(rearm);});
            UnityAction<bool> theory=v=>Refresh();theoretical.onValueChanged.AddListener(theory);cleanup.Add(()=>theoretical.onValueChanged.RemoveListener(theory));
            if(kind==Experiment.Urn)
            {
                for(int i=0;i<4;i++){int index=i;Bind(config[i],()=>{Series.Configure(Series.InitialA+(index<2?(index==0?-1:1):0),Series.InitialB+(index>=2?(index==2?-1:1):0),Series.Replacement);Refresh();});}
                UnityAction<bool> replace=v=>{Series.Configure(Series.InitialA,Series.InitialB,v);Refresh();};replacement.onValueChanged.AddListener(replace);cleanup.Add(()=>replacement.onValueChanged.RemoveListener(replace));
            }
            Refresh();
        }
        void Bind(Button b,UnityAction action){b.onClick.AddListener(action);cleanup.Add(()=>b.onClick.RemoveListener(action));}
        void OnDisable(){foreach(var action in cleanup)action();cleanup.Clear();}
        public void Suspend(){if(Remaining>0){Paused=true;Refresh();}}
        public void Single(){if(lab.Modal||Remaining>0||animation>0||!Series.CanDraw||grab&&grab.isSelected)return;lab.Activate(this);Present(Series.Draw());Refresh();}
        public void StartBatch(){if(lab.Modal||Remaining>0||!Series.CanDraw)return;lab.Activate(this);requested=batch;completed=0;Remaining=batch;Paused=false;Refresh();}
        public void TogglePause(){if(Remaining==0||lab.Modal)return;if(Paused)lab.Activate(this);Paused=!Paused;Refresh();}
        void Update()
        {
            if(grab&&grab.isSelected)animation=0;
            if(animation>0){animation-=Time.deltaTime;display.localRotation=animation>0?Quaternion.Euler(0,animation*720,animation*360):finalRotation;}
            if(Remaining==0||Paused||lab.Modal)return;
            if(grab&&grab.isSelected){Suspend();status.text="Solte a bola para continuar o lote.";return;}
            if(Camera.main&&Vector3.Distance(Camera.main.transform.position,transform.position)>6){Suspend();return;}
            tick+=Time.deltaTime;if(tick<.12f)return;tick=0;
            int latest=-1;for(int i=0;i<Math.Max(1,requested/40)&&Remaining>0;i++)
            {latest=Series.Draw();if(latest<0){Remaining=0;break;}completed++;Remaining--;}
            if(latest>=0)Present(latest);if(!Series.CanDraw)Remaining=0;Refresh();
        }
        string Name(int i)=>kind==Experiment.Coin?(i==0?"Cara":"Coroa"):kind==Experiment.Urn?(i==0?"A":"B"):(i+1).ToString();
        void Present(int i){if(i<0)return;if(grab){display.position=trayPosition;if(categoryMaterials.Length>i)display.GetComponent<Renderer>().sharedMaterial=categoryMaterials[i];if(specimenLabel)specimenLabel.text=Name(i);}result.text="Último resultado: "+Name(i);animation=Remaining>0?.1f:.65f;finalRotation=resultRotations.Length>i?resultRotations[i]:Quaternion.identity;}
        public void Refresh()
        {
            if(Series==null)return;
            for(int i=0;i<bars.Length;i++)
            {
                float fraction=Series.Total==0?0:(float)Series.Counts[i]/Series.Total;
                bars[i].localScale=new Vector3(.28f,Mathf.Max(.001f,fraction*1.4f),.06f);var p=bars[i].localPosition;p.y=.3f+fraction*.7f;bars[i].localPosition=p;bars[i].gameObject.SetActive(Series.Total>0&&fraction>0);
                double chance=Series.Chance(i);bool mark=theoretical.isOn&&!double.IsNaN(chance)&&!(kind==Experiment.Urn&&!Series.Replacement);theoryMarks[i].gameObject.SetActive(mark);p=theoryMarks[i].localPosition;p.y=.3f+(float)(double.IsNaN(chance)?0:chance)*1.4f;theoryMarks[i].localPosition=p;
                categories[i].text=Name(i)+"\n"+Series.Counts[i]+"\n"+(Series.Total==0?"—":(100*fraction).ToString("0.0",System.Globalization.CultureInfo.GetCultureInfo("pt-BR"))+"%")+(theoretical.isOn?"\nP: "+(double.IsNaN(chance)?"—":(chance*100).ToString("0.0",System.Globalization.CultureInfo.GetCultureInfo("pt-BR"))+"%") : "");
            }
            summary.text="Tentativas: "+Series.Total+" / 10.000\nRecentes: "+string.Join(" · ",Series.Recent.Select(Name));
            if(kind==Experiment.Dice&&Series.Total>0)summary.text+="\nMédia: "+Series.Mean.ToString("0.00",System.Globalization.CultureInfo.GetCultureInfo("pt-BR"))+" · Mediana: "+Series.Median.ToString("0.0",System.Globalization.CultureInfo.GetCultureInfo("pt-BR"))+"\nModa(s): "+(Series.Counts.Where(n=>n>0).Distinct().Count()==1?"sem moda única":string.Join(", ",Series.Modes));
            if(kind==Experiment.Dice&&Series.Total==0)summary.text+="\nMédia: — · Mediana: — · Moda: —";
            if(kind==Experiment.Dice)summary.text+="\nMédia = soma dos valores / tentativas";
            if(kind==Experiment.Urn)summary.text+="\nP(A) = bolas A / total disponível";
            status.text=Remaining>0?(Paused?"PAUSADO · ":"EM LOTE · ")+completed+" / "+requested:"Pronto · "+completed+" resultados no último lote";
            if(!Series.CanDraw)status.text=Series.Total>=10000?"Limite atingido. Inicie nova série.":"Urna vazia · "+completed+" resultados no lote. Reponha com Nova série.";
            once.interactable=Remaining==0&&Series.CanDraw;run.interactable=once.interactable;pause.interactable=stop.interactable=Remaining>0;pause.GetComponentInChildren<TMP_Text>().text=Paused?"Continuar":"Pausar";
            for(int i=0;i<batches.Length;i++){int n=new[]{10,100,1000}[i];batches[i].GetComponentInChildren<TMP_Text>().text=n+(batch==n?" ✓":"");batches[i].interactable=Remaining==0;}
            if(kind==Experiment.Urn){replacement.SetIsOnWithoutNotify(Series.Replacement);replacement.interactable=Series.Total==0;foreach(var b in config)b.interactable=Series.Total==0;composition.text="Inicial A:B = "+Series.InitialA+":"+Series.InitialB+" · Restam "+Series.RemainingA+":"+Series.RemainingB+"\n"+(Series.Total>0?"Nova série libera a configuração.":"Configure as categorias A e B.")+(Series.Replacement?"":"\nP refere-se à próxima retirada.");for(int i=0;i<population.Length;i++)population[i].gameObject.SetActive(i<10?i<Series.RemainingA:i-10<Series.RemainingB);}
        }
    }
}
