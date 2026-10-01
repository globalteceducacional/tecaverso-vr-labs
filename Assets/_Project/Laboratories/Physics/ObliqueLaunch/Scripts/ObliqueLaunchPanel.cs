using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tecaverso.Labs.ObliqueLaunch
{
    public sealed class ObliqueLaunchPanel : MonoBehaviour
    {
        [SerializeField] Slider angle, speed, height, mass, gravity;
        [SerializeField] TMP_Text angleValue, speedValue, heightValue, massValue, gravityValue, metrics, pauseLabel;
        [SerializeField] Button fire, pause, reset;
        bool hooked;
        public event System.Action FireRequested, PauseRequested, ResetRequested;
        public LaunchParameters Parameters => new(angle.value, speed.value, height.value, mass.value, gravity.value);
        public void Bind(Slider a, Slider s, Slider h, Slider m, Slider g, TMP_Text av, TMP_Text sv, TMP_Text hv, TMP_Text mv, TMP_Text gv, TMP_Text metricText, TMP_Text pauseText, Button fireButton, Button pauseButton, Button resetButton)
        { angle=a; speed=s; height=h; mass=m; gravity=g; angleValue=av; speedValue=sv; heightValue=hv; massValue=mv; gravityValue=gv; metrics=metricText; pauseLabel=pauseText; fire=fireButton; pause=pauseButton; reset=resetButton; }
        void Awake() => Hook();
        void Hook()
        {
            if (hooked) return;
            hooked = true;
            angle.onValueChanged.AddListener(_=>RefreshValues()); speed.onValueChanged.AddListener(_=>RefreshValues()); height.onValueChanged.AddListener(_=>RefreshValues()); mass.onValueChanged.AddListener(_=>RefreshValues()); gravity.onValueChanged.AddListener(_=>RefreshValues());
            fire.onClick.AddListener(()=>FireRequested?.Invoke()); pause.onClick.AddListener(()=>PauseRequested?.Invoke()); reset.onClick.AddListener(()=>ResetRequested?.Invoke()); RefreshValues();
        }
        void RefreshValues()
        { angleValue.text=$"{angle.value:0}°"; speedValue.text=$"{speed.value:0.0} m/s"; heightValue.text=$"{height.value:0.0} m"; massValue.text=$"{mass.value:0.0} kg"; gravityValue.text=$"{gravity.value:0.00} m/s²"; }
        public void ShowSample(in FlightSample s) => metrics.text=$"t = {s.Time:0.00} s\nY = {s.Position.y:0.00} m\nYmax = {s.MaximumHeight:0.00} m";
        public void ShowState(SimulationState state)
        { pauseLabel.text=state==SimulationState.Paused?"RETOMAR":"PAUSAR"; pause.interactable=state==SimulationState.Running||state==SimulationState.Paused; fire.interactable=state==SimulationState.Idle||state==SimulationState.Complete; }
    }
}
