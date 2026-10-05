using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tecaverso.Labs.ObliqueLaunch
{
    /// <summary>Presentation only: switches Figma views and observes the existing simulation.</summary>
    public sealed class ProjectileAnalysisPanel : MonoBehaviour
    {
        [SerializeField] ObliqueLaunchSimulation simulation;
        [SerializeField] GameObject parametersView, analysisView;
        [SerializeField] Button parametersTab, analysisTab;
        [SerializeField] TMP_Text range, maximumHeight, telemetry;
        static readonly CultureInfo Portuguese = CultureInfo.GetCultureInfo("pt-BR");

        public void Bind(ObliqueLaunchSimulation source, GameObject parameters, GameObject analysis,
            Button parametersButton, Button analysisButton, TMP_Text rangeText, TMP_Text heightText, TMP_Text telemetryText)
        {
            simulation=source; parametersView=parameters; analysisView=analysis;
            parametersTab=parametersButton; analysisTab=analysisButton;
            range=rangeText; maximumHeight=heightText; telemetry=telemetryText;
        }

        void OnEnable()
        {
            parametersTab.onClick.AddListener(ShowParameters);
            analysisTab.onClick.AddListener(ShowAnalysis);
            simulation.SampleChanged+=ShowSample;
            simulation.StateChanged+=OnStateChanged;
            ShowParameters();
            ShowSample(simulation.Current);
        }

        void OnDisable()
        {
            parametersTab.onClick.RemoveListener(ShowParameters);
            analysisTab.onClick.RemoveListener(ShowAnalysis);
            simulation.SampleChanged-=ShowSample;
            simulation.StateChanged-=OnStateChanged;
        }

        public void ShowParameters() => Select(false);
        public void ShowAnalysis() => Select(true);
        void Select(bool analysis)
        {
            parametersView.SetActive(!analysis); analysisView.SetActive(analysis);
            parametersTab.interactable=analysis; analysisTab.interactable=!analysis;
        }
        void OnStateChanged(SimulationState state) => ShowSample(simulation.Current);
        void ShowSample(FlightSample sample)
        {
            bool idle=simulation.State==SimulationState.Idle;
            range.text=(idle?0f:Mathf.Abs(sample.Position.x-simulation.Origin.x)).ToString("0.0",Portuguese)+" m";
            maximumHeight.text=(idle?0f:sample.MaximumHeight).ToString("0.0",Portuguese)+" m";
            string state=simulation.State switch {
                SimulationState.Running=>"EM VOO · alcance percorrido",
                SimulationState.Paused=>"PAUSADO · alcance percorrido",
                SimulationState.Complete=>"CONCLUÍDO · alcance total",
                _=>"PRONTO PARA DISPARAR" };
            telemetry.text=state+"\n"+string.Format(Portuguese,"t = {0:0.00} s    ·    y = {1:0.00} m",idle?0f:sample.Time,idle?0f:sample.Position.y);
        }
    }
}
