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
        [SerializeField] Toggle showVectors;
        bool hooked;
        bool controlAuthority = true;
        SimulationState displayedState;
        public void SetControlAuthority(bool value) { controlAuthority=value; ShowState(displayedState); }
        public void ApplyParameters(LaunchParameters p)
        {
            angle.SetValueWithoutNotify(p.Angle); speed.SetValueWithoutNotify(p.Speed);
            height.SetValueWithoutNotify(p.Height); mass.SetValueWithoutNotify(p.Mass); gravity.SetValueWithoutNotify(p.Gravity);
            RefreshValues();
        }
        public bool VectorsVisible { get; private set; } = true;
        public event System.Action<bool> VectorVisibilityChanged;
        public event System.Action FireRequested, PauseRequested, ResetRequested;
        static readonly System.Globalization.CultureInfo Portuguese = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");
        public void BindVectorToggle(Toggle toggle) => showVectors=toggle;
        public LaunchParameters Parameters => new(angle.value, speed.value, height.value, mass.value, gravity.value);
        public void Bind(Slider a, Slider s, Slider h, Slider m, Slider g, TMP_Text av, TMP_Text sv, TMP_Text hv, TMP_Text mv, TMP_Text gv, TMP_Text metricText, TMP_Text pauseText, Button fireButton, Button pauseButton, Button resetButton)
        { angle=a; speed=s; height=h; mass=m; gravity=g; angleValue=av; speedValue=sv; heightValue=hv; massValue=mv; gravityValue=gv; metrics=metricText; pauseLabel=pauseText; fire=fireButton; pause=pauseButton; reset=resetButton; }
        void Awake()
        {
            speed.minValue=0f; speed.maxValue=30f; speed.wholeNumbers=true;
            gravity.minValue=5f; gravity.maxValue=20f; gravity.wholeNumbers=false;
            CreateVectorToggle();
            VectorsVisible=showVectors.isOn;
            showVectors.onValueChanged.AddListener(OnVectorVisibilityChanged);
            Hook();
        }
        void Start() => OnVectorVisibilityChanged(showVectors.isOn);
        void OnVectorVisibilityChanged(bool value)
        { VectorsVisible=value; VectorVisibilityChanged?.Invoke(value); }
        public void CreateVectorToggle()
        {
            if(showVectors!=null) return;
            var go=new GameObject("Show Vectors Toggle",typeof(RectTransform),typeof(UnityEngine.UI.Image),typeof(Toggle));
            go.transform.SetParent(transform,false);
            var rect=go.GetComponent<RectTransform>(); rect.sizeDelta=new Vector2(650,48); rect.anchoredPosition=new Vector2(0,-330);
            go.GetComponent<UnityEngine.UI.Image>().color=new Color(.15f,.22f,.3f);
            var check=new GameObject("Check",typeof(RectTransform),typeof(UnityEngine.UI.Image)); check.transform.SetParent(rect,false);
            check.GetComponent<RectTransform>().sizeDelta=new Vector2(22,22);
            check.GetComponent<RectTransform>().anchoredPosition=new Vector2(-290,0);
            check.GetComponent<UnityEngine.UI.Image>().color=new Color(.2f,1f,.5f);
            var toggle=go.GetComponent<Toggle>(); toggle.targetGraphic=go.GetComponent<UnityEngine.UI.Image>(); toggle.graphic=check.GetComponent<UnityEngine.UI.Image>(); toggle.isOn=true;
            var label=Instantiate(pauseLabel,rect); label.name="Show Vectors Label"; label.text="EXIBIR VETORES E VALORES";
            label.rectTransform.anchoredPosition=new Vector2(25,0); label.rectTransform.sizeDelta=new Vector2(540,40); label.alignment=TextAlignmentOptions.Left;
            label.raycastTarget=false;
            showVectors=toggle;
        }
        void Hook()
        {
            if (hooked) return;
            hooked = true;
            angle.onValueChanged.AddListener(_=>RefreshValues()); speed.onValueChanged.AddListener(_=>RefreshValues()); height.onValueChanged.AddListener(_=>RefreshValues()); mass.onValueChanged.AddListener(_=>RefreshValues()); gravity.onValueChanged.AddListener(_=>RefreshValues());
            fire.onClick.AddListener(()=>FireRequested?.Invoke()); pause.onClick.AddListener(()=>PauseRequested?.Invoke()); reset.onClick.AddListener(()=>ResetRequested?.Invoke()); RefreshValues();
        }
        void RefreshValues()
        { angleValue.text=angle.value.ToString("0",Portuguese)+"°"; speedValue.text=speed.value.ToString("0",Portuguese)+" m/s"; heightValue.text=height.value.ToString("0.0",Portuguese)+" m"; massValue.text=mass.value.ToString("0.0",Portuguese)+" kg"; gravityValue.text=gravity.value.ToString("0.00",Portuguese)+" m/s²"; }
        public void ShowSample(in FlightSample s) => metrics.text=$"t = {s.Time:0.00} s\nY = {s.Position.y:0.00} m\nYmax = {s.MaximumHeight:0.00} m";
        public void ShowState(SimulationState state)
        {
            displayedState=state;
            bool inFlight=state==SimulationState.Running||state==SimulationState.Paused;
            pauseLabel.text=state==SimulationState.Paused?"RETOMAR":"PAUSAR";
            pause.interactable=controlAuthority&&inFlight; fire.interactable=controlAuthority&&!inFlight; reset.interactable=controlAuthority;
            foreach(var slider in new[]{angle,speed,height,mass,gravity}) slider.interactable=controlAuthority&&!inFlight;
            if(state==SimulationState.Idle) metrics.text="t = 0,00 s    ·    y = 0,00 m";
        }
    }
}
