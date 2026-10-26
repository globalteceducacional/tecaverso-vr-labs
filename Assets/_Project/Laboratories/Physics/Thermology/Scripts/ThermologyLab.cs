using System;
using System.Globalization;
using System.Linq;
using Tecaverso.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Tecaverso.Labs.Thermology
{
    public enum GasRunState { Ready, Running, Paused }

    public sealed class ThermologyLab : MonoBehaviour
    {
        public ThermalPiston piston;
        public ThermalKnob knob;
        public ThermalLoadSocket loadSocket;
        public ThermalLoad[] weights;
        public GasParticles particles;
        public GasPVGraph graph;
        public Transform lockBar, pressureNeedle, temperatureColumn;
        public Renderer jacket;
        public GameObject heatIn,heatOut;
        public TMP_Text values,instruction,invariant,status,feedback,workLabel,loadLabel,temperatureLabel,pressureLabel,volumeLabel,heatLabel;
        public Button[] modes,predictions;
        public Button startButton,pauseButton,resetButton,backButton,confirmButton,cancelButton;
        public Toggle reducedParticles;
        public GameObject controlsPage,analysisPage,leaveDialog;
        public IdealGasModel Model { get; } = new IdealGasModel();
        public GasRunState State { get; private set; }
        public bool Running => State==GasRunState.Running&&!modal&&!loading;
        public bool CanDragPiston => Running&&Model.Process==GasProcess.Isothermal;
        public bool CanHeat => Running&&Model.Process!=GasProcess.Isothermal;
        public bool CanConfigureLoad => State==GasRunState.Ready&&Model.Process==GasProcess.Isobaric&&!modal&&!loading;
        public float GasHeight => (float)Model.Volume*.035f;
        public float TargetTemperature { get; private set; }=300;
        bool modal,loading,settingKnob,observed;
        float pressurePreset=100,heatRemaining;
        int prediction=-2;
        MaterialPropertyBlock block;
        static readonly CultureInfo Pt=CultureInfo.GetCultureInfo("pt-BR");
        static readonly Color Blue=new Color32(40,78,160,255),Pale=new Color32(211,209,232,255);
        static string N(double value,string format="0,0") => value.ToString(format=="0,0"?"0.0":format,Pt);

        void Awake()
        {
            block=new MaterialPropertyBlock();
            for(int i=0;i<modes.Length;i++){int index=i;modes[i].onClick.AddListener(()=>ChooseMode(index));}
            for(int i=0;i<predictions.Length;i++){int sign=i-1;predictions[i].onClick.AddListener(()=>Predict(sign));}
            startButton.onClick.AddListener(StartRun);pauseButton.onClick.AddListener(TogglePause);resetButton.onClick.AddListener(ResetRun);
            backButton.onClick.AddListener(OpenLeave);cancelButton.onClick.AddListener(CancelLeave);confirmButton.onClick.AddListener(ReturnToHub);
            reducedParticles.onValueChanged.AddListener(value=>particles.reduced=value);
            knob.onValueChange.AddListener(SetThermalTarget);
            UIVisibility.Set(leaveDialog,false);ResetRun();
        }
        void OnDestroy(){if(knob!=null)knob.onValueChange.RemoveListener(SetThermalTarget);}
        void Update()
        {
            if(CanHeat)
            {
                double previous=Model.Temperature;
                if(Model.SetTemperature(Mathf.MoveTowards((float)previous,TargetTemperature,35f*Time.deltaTime)))
                {Heat(Model.Temperature>previous);Present();}
            }
            if(Running&&heatRemaining>0)
            {
                heatRemaining-=Time.deltaTime;
                if(heatRemaining<=0){heatIn.SetActive(false);heatOut.SetActive(false);heatLabel.text="SEM TROCA DE CALOR";SetJacket(new Color(.20f,.45f,.75f));}
            }
            piston.transform.localPosition=new Vector3(0,GasHeight,0);
        }
        public void ChooseMode(int mode)
        {
            if(State!=GasRunState.Ready||modal||mode<0||mode>2||Model.Process==(GasProcess)mode)return;
            ReleaseHands();
            if(loadSocket.hasSelection)
                foreach(var selected in loadSocket.interactablesSelected.ToArray())loadSocket.interactionManager.SelectExit(loadSocket,selected);
            foreach(var weight in weights)weight.ReturnHome();
            pressurePreset=100;Model.Reset((GasProcess)mode,pressurePreset);ResetRun();
        }
        public void SetLoad(float pressure)
        {
            if(State!=GasRunState.Ready||Model.Process!=GasProcess.Isobaric)return;
            pressurePreset=Mathf.Clamp(pressure,90,130);Model.Reset(Model.Process,pressurePreset);ResetRun();
        }
        public void Predict(int sign)
        {if(State!=GasRunState.Ready||modal)return;prediction=Mathf.Clamp(sign,-1,1);observed=false;feedback.text="Hipótese registrada. Inicie e faça um movimento.";Present();}
        public void StartRun()
        {
            if(modal||loading||State==GasRunState.Running||(Model.Process==GasProcess.Isobaric&&!loadSocket.hasSelection))return;
            State=GasRunState.Running;Present();
        }
        public void TogglePause()
        {
            if(modal||State==GasRunState.Ready)return;
            ReleaseHands();State=State==GasRunState.Running?GasRunState.Paused:GasRunState.Running;Present();
        }
        public void ResetRun()
        {
            ReleaseHands();State=GasRunState.Ready;Model.Reset(Model.Process,pressurePreset);TargetTemperature=300;
            foreach(var weight in weights)
                if(!weight.interactorsSelecting.Any(hand=>hand is ThermalLoadSocket))weight.ReturnHome();
            settingKnob=true;knob.value=.25f;settingKnob=false;
            prediction=-2;observed=false;heatRemaining=0;heatIn.SetActive(false);heatOut.SetActive(false);
            heatLabel.text="SEM TROCA DE CALOR";SetJacket(new Color(.20f,.45f,.75f));
            feedback.text="Antes de iniciar, preveja a pressão no seu próximo movimento.";
            if(particles!=null)particles.ResetCloud();graph.Record(Model,true);Present();
        }
        public void MovePiston(float localHeight)
        {
            if(!CanDragPiston)return;
            double previous=Model.Volume;
            if(Model.SetVolume(localHeight/.035f)){Heat(Model.Volume>previous);Present();}
        }
        public void SetThermalTarget(float normalized)
        {
            if(settingKnob||!CanHeat)return;
            TargetTemperature=Mathf.Lerp(250,450,Mathf.Clamp01(normalized));Present();
        }
        void Heat(bool enters)
        {
            heatRemaining=.6f;heatIn.SetActive(enters);heatOut.SetActive(!enters);
            heatLabel.text=enters?"CALOR ENTRA NO GÁS":"CALOR SAI DO GÁS";
            SetJacket(enters?new Color(1,.35f,.10f):new Color(.10f,.65f,1));
        }
        void SetJacket(Color color)
        {block.SetColor("_BaseColor",color);block.SetColor("_EmissionColor",color*.5f);jacket.SetPropertyBlock(block);}
        void ReleaseHands()
        {
            if(piston!=null&&piston.interactionManager!=null)
                foreach(var hand in piston.interactorsSelecting.ToArray())piston.interactionManager.SelectExit(hand,piston);
            if(knob!=null&&knob.interactionManager!=null)
                foreach(var hand in knob.interactorsSelecting.ToArray())knob.interactionManager.SelectExit(hand,knob);
            if(weights!=null)foreach(var weight in weights)
                if(weight.interactionManager!=null)foreach(var hand in weight.interactorsSelecting.ToArray())
                    if(!(hand is ThermalLoadSocket))weight.interactionManager.SelectExit(hand,weight);
        }
        public void Present()
        {
            int mode=(int)Model.Process;
            values.text=$"P  {N(Model.Pressure)} kPa\nV  {N(Model.Volume)} L\nT  {N(Model.Temperature)} K  /  {N(Model.Temperature-273.15)} °C";
            invariant.text=mode==0?"TEMPERATURA CONSTANTE · 300 K":mode==1?$"PRESSÃO CONSTANTE · {N(Model.InitialPressure)} kPa":"VOLUME CONSTANTE · 25 L";
            instruction.text=mode==0?"Pegue o puxador azul e mova o pistão.\nA jaqueta mantém 300 K durante o movimento.":mode==1?"Encaixe uma carga antes de iniciar.\nGire a manopla: o pistão acompanha o aquecimento.":"Gire a manopla para aquecer ou resfriar.\nA trava mantém o volume: observe a pressão.";
            status.text=State==GasRunState.Ready?"PREPARAÇÃO · escolha o modo e faça sua previsão":State==GasRunState.Paused?"PAUSADO · observe o estado e o gráfico":"EM EXPLORAÇÃO · manipule o aparelho";
            loadLabel.text=mode==1?(loadSocket.hasSelection?$"CARGA ENCAIXADA · {N(pressurePreset,"0")} kPa":"ISOBÁRICA · ENCAIXE UMA CARGA") : "CARGAS USADAS SOMENTE NA ISOBÁRICA";
            pressureLabel.text=$"PRESSÃO\n{N(Model.Pressure)} kPa";
            volumeLabel.text=$"VOLUME\n{N(Model.Volume)} L";
            temperatureLabel.text=mode==0?"RESERVATÓRIO · 300 K\nManopla bloqueada na isotérmica":$"TEMPERATURA · {N(Model.Temperature)} K\nALVO · {N(TargetTemperature)} K";
            workLabel.text=$"TRABALHO DO GÁS  {N(Model.Work)} J\n"+(Math.Abs(Model.Work)<.05?"Sem variação líquida de volume":Model.Work>0?"Expansão · trabalho positivo":"Compressão · trabalho negativo");
            lockBar.localEulerAngles=new Vector3(0,0,mode==2?0:85);
            pressureNeedle.localEulerAngles=new Vector3(0,0,135-(float)Model.Pressure/240f*270);
            float fill=Mathf.InverseLerp(250,450,(float)Model.Temperature);temperatureColumn.localScale=new Vector3(.07f,.1f+fill*.6f,.035f);
            temperatureColumn.localPosition=new Vector3(0,-.35f+(.1f+fill*.6f)*.5f,-.08f);
            for(int i=0;i<modes.Length;i++){modes[i].interactable=State==GasRunState.Ready;modes[i].GetComponent<Image>().color=i==mode?Blue:Pale;modes[i].GetComponentInChildren<TMP_Text>().color=i==mode?Color.white:Color.black;}
            for(int i=0;i<predictions.Length;i++){predictions[i].interactable=State==GasRunState.Ready;predictions[i].GetComponent<Image>().color=prediction==i-1?Blue:Pale;predictions[i].GetComponentInChildren<TMP_Text>().color=prediction==i-1?Color.white:Color.black;}
            startButton.interactable=State!=GasRunState.Running&&(mode!=1||loadSocket.hasSelection);
            startButton.GetComponentInChildren<TMP_Text>().text=State==GasRunState.Paused?"CONTINUAR":"INICIAR";
            pauseButton.interactable=State==GasRunState.Running;
            if(!observed&&State!=GasRunState.Ready&&prediction!=-2&&(Math.Abs(Model.Volume-Model.InitialVolume)>.8||Math.Abs(Model.Temperature-Model.InitialTemperature)>5))
            {
                int actual=Math.Abs(Model.Pressure-Model.InitialPressure)<.1?0:Model.Pressure>Model.InitialPressure?1:-1;
                feedback.text=(actual==prediction?"Previsão confirmada. ":"Compare com sua previsão. ")+"A pressão "+(actual==0?"permaneceu constante.":actual>0?"aumentou.":"diminuiu.");observed=true;
            }
            graph.Record(Model);
        }
        void OpenLeave(){ReleaseHands();modal=true;UIVisibility.Set(leaveDialog,true);UIVisibility.SetInteraction(controlsPage,false);UIVisibility.SetInteraction(analysisPage,false);}
        void CancelLeave(){modal=false;UIVisibility.Set(leaveDialog,false);UIVisibility.SetInteraction(controlsPage,true);UIVisibility.SetInteraction(analysisPage,true);}
        void ReturnToHub()
        {
            const string hub="Assets/_Project/Hub/Scenes/Hub.unity";
            if(loading)return;
            if(!Application.CanStreamedLevelBeLoaded(hub)){feedback.text="Hub indisponível.";CancelLeave();return;}
            loading=true;SceneManager.LoadSceneAsync(hub);
        }
    }
}
