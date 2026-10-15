using System;
using System.Globalization;
using Tecaverso.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Tecaverso.Labs.InclinedPlane
{
    public sealed class InclinedPlanePanel : MonoBehaviour
    {
        [SerializeField] Slider[] controls;
        [SerializeField] TMP_Text[] values;
        [SerializeField] Button release, pause, reset, parametersTab, analysisTab, back, confirmBack, cancelBack;
        [SerializeField] Button[] presets;
        [SerializeField] Toggle vectors, resultant, components;
        [SerializeField] GameObject parametersPage, analysisPage, leaveDialog, pages;
        [SerializeField] TMP_Text status, metrics, analysis, pauseLabel, scaleLabel;
        static readonly CultureInfo Portuguese=CultureInfo.GetCultureInfo("pt-BR");
        bool loading, hooked;
        bool presented;
        InclineParameters previousParameters;
        InclineState previousState;
        public event Action ParametersChanged, ReleaseRequested, PauseRequested, ResetRequested, VisibilityChanged;
        public InclineParameters Parameters => new(controls[0].value,controls[1].value,controls[4].value,controls[2].value,controls[3].value);
        public bool VectorsVisible => vectors.isOn;
        public bool ResultantVisible => resultant.isOn;
        public bool ComponentsVisible => components.isOn;
        public void Bind(Slider[] sliders,TMP_Text[] valueLabels,Button[] actions,Button[] surfacePresets,Toggle[] options,
            GameObject[] views,TMP_Text[] texts)
        {
            controls=sliders;values=valueLabels;
            release=actions[0];pause=actions[1];reset=actions[2];parametersTab=actions[3];analysisTab=actions[4];
            back=actions[5];confirmBack=actions[6];cancelBack=actions[7];presets=surfacePresets;
            vectors=options[0];resultant=options[1];components=options[2];
            parametersPage=views[0];analysisPage=views[1];leaveDialog=views[2];pages=views[3];
            status=texts[0];metrics=texts[1];analysis=texts[2];pauseLabel=texts[3];scaleLabel=texts[4];
        }
        void Awake()
        {
            if(hooked)return;hooked=true;
            foreach(var slider in controls)slider.onValueChanged.AddListener(OnParameterChanged);
            release.onClick.AddListener(Release);pause.onClick.AddListener(Pause);reset.onClick.AddListener(ResetLab);
            parametersTab.onClick.AddListener(ShowParameters);analysisTab.onClick.AddListener(ShowAnalysis);
            back.onClick.AddListener(OpenLeave);cancelBack.onClick.AddListener(CloseLeave);confirmBack.onClick.AddListener(ReturnToHub);
            vectors.onValueChanged.AddListener(OnVisibility);resultant.onValueChanged.AddListener(OnVisibility);components.onValueChanged.AddListener(OnVisibility);
            presets[0].onClick.AddListener(NoFriction);presets[1].onClick.AddListener(LowFriction);presets[2].onClick.AddListener(HighFriction);
            RefreshValues();ShowParameters();UIVisibility.Set(leaveDialog,false);
        }
        void OnDestroy()
        {
            if(!hooked)return;
            foreach(var slider in controls)slider.onValueChanged.RemoveListener(OnParameterChanged);
            release.onClick.RemoveListener(Release);pause.onClick.RemoveListener(Pause);reset.onClick.RemoveListener(ResetLab);
            parametersTab.onClick.RemoveListener(ShowParameters);analysisTab.onClick.RemoveListener(ShowAnalysis);
            back.onClick.RemoveListener(OpenLeave);cancelBack.onClick.RemoveListener(CloseLeave);confirmBack.onClick.RemoveListener(ReturnToHub);
            vectors.onValueChanged.RemoveListener(OnVisibility);resultant.onValueChanged.RemoveListener(OnVisibility);components.onValueChanged.RemoveListener(OnVisibility);
            presets[0].onClick.RemoveListener(NoFriction);presets[1].onClick.RemoveListener(LowFriction);presets[2].onClick.RemoveListener(HighFriction);
        }
        void Release()=>ReleaseRequested?.Invoke();
        void Pause()=>PauseRequested?.Invoke();
        void ResetLab()=>ResetRequested?.Invoke();
        void OnVisibility(bool _) => VisibilityChanged?.Invoke();
        void OnParameterChanged(float _)
        {
            // Surface model requires kinetic friction <= static friction.
            if(controls[3].value>controls[2].value)controls[3].SetValueWithoutNotify(controls[2].value);
            controls[3].maxValue=controls[2].value;
            RefreshValues();ParametersChanged?.Invoke();
        }
        void NoFriction()=>Preset(0,0);
        void LowFriction()=>Preset(.25f,.15f);
        void HighFriction()=>Preset(.65f,.45f);
        void Preset(float s,float k)
        {
            controls[2].SetValueWithoutNotify(s);controls[3].maxValue=s;controls[3].SetValueWithoutNotify(k);
            OnParameterChanged(0);
        }
        void RefreshValues()
        {
            var p=Parameters;
            values[0].text=p.Angle.ToString("0",Portuguese)+"°";
            values[1].text=p.Mass.ToString("0.0",Portuguese)+" kg";
            values[2].text=p.StaticFriction.ToString("0.00",Portuguese);
            values[3].text=p.KineticFriction.ToString("0.00",Portuguese);
            values[4].text=p.Gravity.ToString("0.00",Portuguese)+" m/s²";
        }
        void ShowParameters(){UIVisibility.Set(parametersPage,true);UIVisibility.Set(analysisPage,false);parametersTab.interactable=false;analysisTab.interactable=true;}
        void ShowAnalysis(){UIVisibility.Set(parametersPage,false);UIVisibility.Set(analysisPage,true);parametersTab.interactable=true;analysisTab.interactable=false;}
        void OpenLeave(){UIVisibility.Set(leaveDialog,true);UIVisibility.SetInteraction(pages,false);}
        void CloseLeave(){UIVisibility.Set(leaveDialog,false);UIVisibility.SetInteraction(pages,true);}
        void ReturnToHub()
        {
            const string hub="Assets/_Project/Hub/Scenes/Hub.unity";
            if(loading)return;
            if(!Application.CanStreamedLevelBeLoaded(hub)){status.text="Hub indisponível. Verifique as cenas da aplicação.";CloseLeave();return;}
            loading=true;confirmBack.interactable=false;SceneManager.LoadSceneAsync(hub);
        }
        public void Show(InclinedPlaneModel model,float forceScale)
        {
            metrics.text=string.Format(Portuguese,"s = {0:0.00} m    v = {1:0.00} m/s\na = {2:0.00} m/s²",model.Distance,model.Speed,model.Forces.Acceleration);
            var p=model.Parameters;
            if(presented && previousState==model.State && previousParameters.Angle==p.Angle && previousParameters.Mass==p.Mass &&
                previousParameters.Gravity==p.Gravity && previousParameters.StaticFriction==p.StaticFriction && previousParameters.KineticFriction==p.KineticFriction) return;
            presented=true;previousState=model.State;previousParameters=p;
            RefreshValues();
            bool editable=model.State==InclineState.Ready;
            foreach(var control in controls)control.interactable=editable;
            foreach(var preset in presets)preset.interactable=editable;
            release.interactable=editable;
            pause.interactable=model.State==InclineState.Sliding||model.State==InclineState.Paused;
            pauseLabel.text=model.State==InclineState.Paused?"RETOMAR":"PAUSAR";
            status.text=model.State switch {
                InclineState.Ready=>"PREVISÃO AO LIBERAR · "+(model.Forces.Slides?"O bloco deslizará.":"O bloco ficará em equilíbrio."),
                InclineState.Equilibrium=>"EM EQUILÍBRIO · Reinicie para ajustar.",
                InclineState.Sliding=>"DESLIZANDO · Parâmetros bloqueados.",
                InclineState.Paused=>"PAUSADO · Forças preservadas.",
                _=>"FIM DO PERCURSO · Estado final congelado."
            };
            var f=model.Forces;
            analysis.text=string.Format(Portuguese,
                "PESO · P = {0:0.00} N\nNORMAL · N = {1:0.00} N\nATRITO {2} · Fat = {3:0.00} N\nRESULTANTE · ΣF = {4:0.00} N\n\nLimite estático: μsN = {5:0.00} N\nPeso paralelo: Ppar = {6:0.00} N\nÂngulo crítico: {7:0.0}°\n\n{8}",
                f.Weight.magnitude,f.Normal.magnitude,f.Slides?"CINÉTICO":"ESTÁTICO",f.Friction.magnitude,f.Resultant.magnitude,f.StaticLimit,f.ParallelWeight.magnitude,
                Mathf.Atan(model.Parameters.StaticFriction)*Mathf.Rad2Deg,
                model.State==InclineState.Complete?"Valores imediatamente antes do fim.\nA parada não simula uma colisão.":"Atrito estático ajusta-se até μsN.\nComponentes de P não são forças extras.");
            scaleLabel.text=string.Format(Portuguese,"Escala comum às forças: 1 m de seta = {0:0.0} N",1f/forceScale);
        }
    }
}
