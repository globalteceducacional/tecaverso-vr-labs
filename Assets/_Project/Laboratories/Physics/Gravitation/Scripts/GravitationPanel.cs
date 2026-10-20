using System;
using System.Globalization;
using Tecaverso.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Tecaverso.Labs.Gravitation
{
    public enum GravityMode { Attraction, Satellite, SolarSystem }

    public sealed class GravitationPanel : MonoBehaviour
    {
        public Slider[] controls;
        public TMP_Text[] values, captions;
        public Button[] modes, bodies, actions;
        public Toggle[] options;
        public GameObject[] pages;
        public TMP_Text status, metrics, analysis, scale, formula, explanation, launchLabel, pauseLabel;
        public GameObject solarControls;
        public Button[] solarViews;
        public TMP_Text solarGuide, introduction;
        public Toggle realScale;
        public bool InnerSystem { get; private set; }
        public GravityMode Mode { get; private set; } = GravityMode.Satellite;
        public int BodyIndex { get; private set; }
        public event Action SettingsChanged, ViewChanged, LaunchRequested, PauseRequested, ResetRequested;
        readonly UnityAction[] modeCallbacks=new UnityAction[3], bodyCallbacks=new UnityAction[3];
        static readonly CultureInfo Pt=CultureInfo.GetCultureInfo("pt-BR");
        static readonly string[] FieldNames={"ALTITUDE","VELOCIDADE INICIAL","MASSA DO SATÉLITE","MASSA DO ASTRO","ESCALA DE TEMPO"};
        bool loading, initialized;
        GravityMode? visibleMode;
        public double Rate => controls[4].value;
        public bool ForceVisible => options[0].isOn;
        public bool VelocityVisible => options[1].isOn;
        public bool PathsVisible => options[2].isOn;

        void Awake()
        {
            initialized=true;
            if(realScale!=null)realScale.onValueChanged.AddListener(VisibilityChanged);
            solarViews[0].onClick.AddListener(ShowCompleteSystem);
            solarViews[1].onClick.AddListener(ShowInnerSystem);
            for(int i=0;i<3;i++)
            {
                int index=i;
                modeCallbacks[i]=()=>SelectMode((GravityMode)index);
                bodyCallbacks[i]=()=>SelectBody(index);
                modes[i].onClick.AddListener(modeCallbacks[i]); bodies[i].onClick.AddListener(bodyCallbacks[i]);
            }
            for(int i=0;i<4;i++)controls[i].onValueChanged.AddListener(ParameterChanged);
            controls[4].onValueChanged.AddListener(RateChanged);
            foreach(var toggle in options)toggle.onValueChanged.AddListener(VisibilityChanged);
            actions[0].onClick.AddListener(Launch); actions[1].onClick.AddListener(Pause); actions[2].onClick.AddListener(ResetAttempt);
            actions[3].onClick.AddListener(ShowParameters); actions[4].onClick.AddListener(ShowAnalysis);
            actions[5].onClick.AddListener(OpenLeave); actions[6].onClick.AddListener(Leave); actions[7].onClick.AddListener(CancelLeave);
            ShowParameters(); UIVisibility.Set(pages[2],false);
        }
        void OnDestroy()
        {
            if(!initialized)return;
            if(realScale!=null)realScale.onValueChanged.RemoveListener(VisibilityChanged);
            solarViews[0].onClick.RemoveListener(ShowCompleteSystem);
            solarViews[1].onClick.RemoveListener(ShowInnerSystem);
            for(int i=0;i<3;i++){modes[i].onClick.RemoveListener(modeCallbacks[i]);bodies[i].onClick.RemoveListener(bodyCallbacks[i]);}
            for(int i=0;i<4;i++)controls[i].onValueChanged.RemoveListener(ParameterChanged);
            controls[4].onValueChanged.RemoveListener(RateChanged);
            foreach(var toggle in options)toggle.onValueChanged.RemoveListener(VisibilityChanged);
            actions[0].onClick.RemoveListener(Launch);actions[1].onClick.RemoveListener(Pause);actions[2].onClick.RemoveListener(ResetAttempt);
            actions[3].onClick.RemoveListener(ShowParameters);actions[4].onClick.RemoveListener(ShowAnalysis);
            actions[5].onClick.RemoveListener(OpenLeave);actions[6].onClick.RemoveListener(Leave);actions[7].onClick.RemoveListener(CancelLeave);
        }
        public void SelectMode(GravityMode mode)
        {
            Mode=mode;
            controls[4].maxValue=mode==GravityMode.SolarSystem?365:1000;
            controls[4].SetValueWithoutNotify(mode==GravityMode.SolarSystem?30:300);
            SettingsChanged?.Invoke();
        }
        void ShowCompleteSystem()=>SelectSolarView(false);
        void ShowInnerSystem()=>SelectSolarView(true);
        public void SelectSolarView(bool inner){InnerSystem=inner;ViewChanged?.Invoke();}
        void UpdateModeLayout()
        {
            if(visibleMode==Mode)return;
            visibleMode=Mode;
            bool solar=Mode==GravityMode.SolarSystem;
            UIVisibility.Set(bodies[0].transform.parent.gameObject,!solar,true);
            UIVisibility.Set(solarControls,solar,true);
            for(int i=0;i<4;i++)UIVisibility.Set(controls[i].transform.parent.gameObject,!solar,true);
            var fields=controls[0].transform.parent.parent.GetComponent<LayoutElement>();
            fields.minHeight=fields.preferredHeight=solar?96:528;
            UIVisibility.Set(options[0].transform.parent.gameObject,!solar,true);
            if(realScale!=null)UIVisibility.Set(realScale.gameObject,solar,true);
            options[2].GetComponentInChildren<TMP_Text>(true).text=solar?"EXIBIR ÓRBITAS":"TRAJETÓRIAS";
            var optionsLayout=options[2].transform.parent.GetComponent<LayoutElement>();
            optionsLayout.minHeight=optionsLayout.preferredHeight=104;
            introduction.text=solar?"Aponte para ler; selecione para ver de perto.\nOu escolha um planeta na lista abaixo.":
                Mode==GravityMode.Attraction?"Compare a atração ao mudar massa e distância.\nAs posições ficam fixas neste modo.":
                "Coloque um satélite em órbita.\nTrocar modo ou astro reinicia a tentativa.";
        }
        void SelectBody(int index) { BodyIndex=index; controls[3].SetValueWithoutNotify(1); SettingsChanged?.Invoke(); }
        void ParameterChanged(float _) => SettingsChanged?.Invoke();
        void RateChanged(float _) => ViewChanged?.Invoke();
        void VisibilityChanged(bool _) => ViewChanged?.Invoke();
        void Launch()=>LaunchRequested?.Invoke();
        void Pause()=>PauseRequested?.Invoke();
        void ResetAttempt()=>ResetRequested?.Invoke();
        void ShowParameters(){UIVisibility.Set(pages[0],true);UIVisibility.Set(pages[1],false);actions[3].interactable=false;actions[4].interactable=true;}
        void ShowAnalysis(){UIVisibility.Set(pages[0],false);UIVisibility.Set(pages[1],true);actions[3].interactable=true;actions[4].interactable=false;}
        void OpenLeave(){UIVisibility.Set(pages[2],true);UIVisibility.SetInteraction(pages[3],false);}
        void CancelLeave(){UIVisibility.Set(pages[2],false);UIVisibility.SetInteraction(pages[3],true);}
        void Leave()
        {
            const string hub="Assets/_Project/Hub/Scenes/Hub.unity";
            if(loading)return;
            if(!Application.CanStreamedLevelBeLoaded(hub)){status.text="Hub indisponível.";CancelLeave();return;}
            loading=true;actions[6].interactable=false;SceneManager.LoadSceneAsync(hub);
        }
        public void Show(OrbitalModel model,bool solarRunning,bool solarPaused,double days)
        {
            UpdateModeLayout();
            bool solar=Mode==GravityMode.SolarSystem, attraction=Mode==GravityMode.Attraction;
            bool editable=model.State==OrbitState.Ready;
            for(int i=0;i<3;i++){modes[i].interactable=i!=(int)Mode;bodies[i].interactable=!solar&&editable&&i!=BodyIndex;}
            for(int i=0;i<4;i++)controls[i].interactable=!solar&&editable&&(i!=1||!attraction);
            for(int i=0;i<5;i++)captions[i].text=FieldNames[i];
            if(solar)captions[4].text="DIAS POR SEGUNDO";
            controls[4].interactable=!attraction;
            options[0].interactable=!solar;options[1].interactable=!solar&&!attraction;
            launchLabel.text=solar?"INICIAR":"LANÇAR";
            actions[0].interactable=solar?!solarRunning:!attraction&&editable;
            actions[1].interactable=solar?solarRunning:model.State==OrbitState.Running||model.State==OrbitState.Paused;
            pauseLabel.text=(solar?solarPaused:model.State==OrbitState.Paused)?"RETOMAR":"PAUSAR";
            values[0].text=(controls[0].value*model.Body.RadiusKm).ToString("N0",Pt)+" km";
            values[1].text=controls[1].value.ToString("0.00",Pt)+" × vc";
            values[2].text=controls[2].value.ToString("N0",Pt)+" kg";
            values[3].text=controls[3].value.ToString("0.00",Pt)+" × M";
            values[4].text=Rate.ToString("0",Pt)+(solar?" dias/s":" s/s");
            if(solar)
            {
                solarViews[0].interactable=InnerSystem;solarViews[1].interactable=!InnerSystem;
                solarViews[0].GetComponentInChildren<TMP_Text>(true).text=InnerSystem?"SISTEMA COMPLETO":"COMPLETO · ATIVO";
                solarViews[1].GetComponentInChildren<TMP_Text>(true).text=InnerSystem?"ROCHOSOS · ATIVO":"AMPLIAR ROCHOSOS";
                bool exact=realScale!=null&&realScale.isOn;
                solarGuide.text=(InnerSystem?"DISTÂNCIAS · 1 m = 0,25 UA":"DISTÂNCIAS · 1 m = 5 UA")+
                    "\nDIÂMETROS · 1 m = "+SolarScale.BodyKmPerMetre(InnerSystem,exact).ToString("N0",Pt)+" km"+
                    "\nMesmo fator para Sol e todos os planetas."+
                    "\nVER DE PERTO · escolha um planeta";
                status.text=solarRunning?(solarPaused?"SISTEMA SOLAR · PAUSADO":"SISTEMA SOLAR · EM MOVIMENTO"):"SISTEMA SOLAR · Pronto para iniciar.";
                metrics.text="Tempo simulado: "+days.ToString("N1",Pt)+" dias\n"+(InnerSystem?"4 rochosos em foco · relógio compartilhado":"8 planetas · relógio compartilhado");
                scale.text=exact?"ESCALA ÚNICA · corpos e distâncias.\nLosangos são alvos, não tamanhos dos astros.":
                    "DUAS ESCALAS · diâmetros ampliados juntos 53,4×.\nLosangos são alvos, não tamanhos dos astros.";
                analysis.text="DISTÂNCIA ORBITAL · UA / PERÍODO · dias\n";
                foreach(var planet in PlanetOrbit.Planets)analysis.text+=planet.Name+" · "+planet.SemiMajorAU.ToString("0.00",Pt)+" UA / "+planet.PeriodDays.ToString("N0",Pt)+" d\n";
                formula.text="Órbitas keplerianas em torno do Sol.\nÁreas iguais em tempos iguais.\n\nA distância ao Sol varia ao longo da órbita.\nAs linhas mostram as órbitas completas.";
                explanation.text="TAMANHOS E DISTÂNCIAS\nSol / Júpiter: diâmetro 9,73× maior.\nModo didático: todos os diâmetros ampliados\n53,4× em relação à escala das distâncias.\nEscala única real: sem ampliação dos corpos.\nAlvos de seleção não representam diâmetros.\nA inspeção isolada usa ampliação individual.\nÓrbitas coplanares; fases ilustrativas.\nTexturas: Solar System Scope · CC BY 4.0\nDiâmetros e órbitas: NASA / JPL";
                return;
            }
            string body=model.Body.Name+(Math.Abs(model.MassMultiplier-1)>.001?" · PERSONALIZADO":"");
            status.text=attraction?"ATRAÇÃO · Posições fixas para estudo.":model.State switch {
                OrbitState.Ready=>"PRONTO · "+body,
                OrbitState.Running=>"EM VOO · Reinicie para ajustar.",
                OrbitState.Paused=>"PAUSADO · Vetores preservados.",
                OrbitState.Collision=>"COLISÃO · Valores antes do impacto.",
                OrbitState.Escaped=>"ESCAPE · Saiu da área de observação.",
                _=>"ÓRBITA LIGADA · Excedeu a área visível."
            };
            double speed=attraction?0:model.SpeedKm;
            metrics.text=string.Format(Pt,"F = {0:N1} N    v = {1:0.00} km/s\nAltitude = {2:N0} km    t = {3:N0} s",
                model.ForceNewtons,speed,Math.Max(0,model.RadiusKm-model.Body.RadiusKm),model.ElapsedSeconds);
            scale.text=string.Format(Pt,"1 m no modelo = {0:N0} km · satélite ampliado\nF: 1 m = {1:N0} N · v: 1 m = {2:0.00} km/s",
                model.Body.RadiusKm/GravitationLab.PlanetRadius,model.SurfaceForce/.8,model.VelocityUnit);
            analysis.text=string.Format(Pt,
                "{0}\nRaio do astro: {1:N0} km\nDistância entre centros: {2:N0} km\nMassa do satélite: {3:N0} kg\nForça gravitacional: {4:N1} N\n\nVelocidade circular local: {5:0.00} km/s\nVelocidade de escape local: {6:0.00} km/s\n\n{7}",
                body,model.Body.RadiusKm,model.RadiusKm,model.SatelliteMass,model.ForceNewtons,model.CircularSpeedKm,model.EscapeSpeedKm,
                attraction?"Sem integração do movimento neste modo.":model.SpecificEnergy>=0?"Energia orbital não negativa: trajetória aberta.":"Energia orbital negativa: órbita ligada;\npode interceptar a superfície do astro.");
            formula.text="F = G M m / r²      r = R + altitude\nvc = √(G M / r)      ve = √2 vc\n\nA gravidade muda a velocidade.\nA força aponta para o centro do astro.";
            explanation.text="AMARELO · força     AZUL · velocidade\nVetores com escalas independentes.\n\nAstro esférico fixo; satélite sem propulsão.\nSem atmosfera ou atração de outros corpos.\nTrajetória: até 2.048 pontos, sem cópias.\n\nTexturas: Solar System Scope · CC BY 4.0\nDados: NASA / JPL";
        }
    }
}
