using Tecaverso.Labs.ObliqueLaunch;
using TMPro;
using UnityEngine;

namespace Tecaverso.Labs.Gravitation
{
    public sealed class GravitationLab : MonoBehaviour
    {
        public const float PlanetRadius=.65f;
        public GravitationPanel panel;
        public Transform orbitalRoot, planet, satellite, vectorAnchor;
        public Renderer planetRenderer;
        public Material[] bodyMaterials;
        public VectorArrowView forceArrow, velocityArrow;
        public TMP_Text forceLabel, velocityLabel, stationLabel;
        public LineRenderer trajectory;
        public SolarSystemView solar;
        public GameObject[] satelliteFurniture;
        public OrbitalModel Model { get; } = new();
        readonly Vector3[] samples=new Vector3[2048];
        int sampleCount;
        bool solarRunning,solarPaused;
        double solarDays;
        float nextReadout;
        public bool SolarRunning=>solarRunning;
        public bool SolarPaused=>solarPaused;
        public double SolarDays=>solarDays;
        void OnEnable()
        {
            panel.SettingsChanged+=ResetAttempt; panel.ViewChanged+=Present;
            panel.LaunchRequested+=Launch;panel.PauseRequested+=Pause;panel.ResetRequested+=ResetAttempt;
        }
        void OnDisable()
        {
            panel.SettingsChanged-=ResetAttempt;panel.ViewChanged-=Present;
            panel.LaunchRequested-=Launch;panel.PauseRequested-=Pause;panel.ResetRequested-=ResetAttempt;
        }
        void Start()=>ResetAttempt();
        void Update()
        {
            if(panel.Mode==GravityMode.SolarSystem)
            {
                if(solarRunning&&!solarPaused){solarDays+=Time.deltaTime*panel.Rate;solar.Show(solarDays,panel.PathsVisible);}
            }
            else if(panel.Mode==GravityMode.Satellite&&Model.State==OrbitState.Running)
            {
                Model.Advance(Time.deltaTime*panel.Rate); UpdateSatellite(); AppendSample();
            }
            if(Time.unscaledTime>=nextReadout){nextReadout=Time.unscaledTime+.1f;Readout();}
        }
        public void ResetAttempt()
        {
            Model.Reset();
            Model.Configure(CelestialBody.Presets[panel.BodyIndex],panel.controls[0].value,panel.controls[1].value,
                panel.controls[2].value,panel.controls[3].value);
            solarDays=0;solarRunning=solarPaused=false;sampleCount=0;
            planetRenderer.sharedMaterial=bodyMaterials[panel.BodyIndex];
            trajectory.positionCount=0;
            Present();
            if(panel.Mode==GravityMode.Satellite)AppendSample();
        }
        public void Launch()
        {
            if(panel.Mode==GravityMode.SolarSystem)solarRunning=true;
            else if(panel.Mode==GravityMode.Satellite)Model.Launch();
            Present();
        }
        public void Pause()
        {
            if(panel.Mode==GravityMode.SolarSystem&&solarRunning)solarPaused=!solarPaused;
            else Model.TogglePause();
            Present();
        }
        public void Present()
        {
            bool overview=panel.Mode==GravityMode.SolarSystem;
            orbitalRoot.gameObject.SetActive(!overview);solar.gameObject.SetActive(overview);
            foreach(var furniture in satelliteFurniture)if(furniture!=null)furniture.SetActive(!overview);
            stationLabel.text=overview?"SISTEMA SOLAR · ÓRBITAS DOS PLANETAS":"ESTAÇÃO ORBITAL · "+Model.Body.Name.ToUpperInvariant();
            if(overview){solar.SetView(panel.InnerSystem,panel.realScale!=null&&panel.realScale.isOn);solar.Show(solarDays,panel.PathsVisible);}
            else UpdateSatellite();
            Readout();
        }
        void Readout()
        {
            panel.Show(Model,solarRunning,solarPaused,solarDays);
            forceLabel.text="F = "+Model.ForceNewtons.ToString("N0",System.Globalization.CultureInfo.GetCultureInfo("pt-BR"))+" N";
            velocityLabel.text="v = "+Model.SpeedKm.ToString("0.00",System.Globalization.CultureInfo.GetCultureInfo("pt-BR"))+" km/s";
        }
        void UpdateSatellite()
        {
            var p=Model.Position;var v=Model.Velocity;
            satellite.localPosition=new Vector3((float)p.X,(float)p.Y,0)*PlanetRadius;
            if(v.Length>.0001)satellite.localRotation=Quaternion.Euler(0,0,(float)System.Math.Atan2(v.Y,v.X)*Mathf.Rad2Deg-90);
            // Project the diagram in front of the globe, so its surface cannot hide arrowheads.
            vectorAnchor.localPosition=satellite.localPosition+Vector3.back*(PlanetRadius+.13f);
            bool attraction=panel.Mode==GravityMode.Attraction;
            forceArrow.gameObject.SetActive(panel.ForceVisible);
            if(panel.ForceVisible)forceArrow.SetScaledVector(new Vector3(-(float)p.X,-(float)p.Y,0).normalized*(float)Model.ForceNewtons,(float)(.8/Model.SurfaceForce));
            velocityArrow.gameObject.SetActive(panel.VelocityVisible&&!attraction);
            if(panel.VelocityVisible&&!attraction)velocityArrow.SetScaledVector(new Vector3((float)v.X,(float)v.Y,0),1);
            forceLabel.gameObject.SetActive(panel.ForceVisible);
            velocityLabel.gameObject.SetActive(panel.VelocityVisible&&!attraction);
            // Separate the labels even when gravity and velocity nearly align.
            forceLabel.transform.localPosition=new Vector3(-.45f,-.34f,-.02f);
            velocityLabel.transform.localPosition=new Vector3(.45f,.34f,-.02f);
            trajectory.enabled=panel.PathsVisible&&!attraction;
        }
        void AppendSample()
        {
            Vector3 p=satellite.localPosition;
            if(sampleCount>0&&(samples[sampleCount-1]-p).sqrMagnitude<.000225f&&Model.State==OrbitState.Running)return;
            if(sampleCount==samples.Length)
            {
                // Keep recent samples at their original resolution: repeated decimation
                // eventually draws chords across a closed orbit after many revolutions.
                System.Array.Copy(samples,1,samples,0,samples.Length-1);
                samples[samples.Length-1]=p;
                trajectory.SetPositions(samples);
                return;
            }
            samples[sampleCount++]=p;trajectory.positionCount=sampleCount;trajectory.SetPosition(sampleCount-1,p);
        }
    }
}
