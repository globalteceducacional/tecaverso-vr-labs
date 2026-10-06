using UnityEngine;

namespace Tecaverso.Labs.ObliqueLaunch
{
    public sealed class ObliqueLaunchLab : MonoBehaviour
    {
        public event System.Action Launched;
        public event System.Action ResetRequested;
        [SerializeField] ObliqueLaunchSimulation simulation;
        [SerializeField] ObliqueLaunchPanel panel;
        [SerializeField] ProjectileView projectile;
        [SerializeField] TrajectorySnapshotPool snapshots;
        [SerializeField] ProjectileTrajectoryLine trajectory;
        [SerializeField] MeasurementRulers rulers;
        [SerializeField] Transform baseCylinder, cannonPivot, muzzle;
        [SerializeField] Rigidbody projectileBody;
        [SerializeField] TelescopicLaunchBase telescopicBase;
        public ObliqueLaunchSimulation Simulation => simulation;
        public ObliqueLaunchPanel Panel => panel;
        bool controlAuthority = true;
        public void SetControlAuthority(bool value) { if(controlAuthority==value)return; controlAuthority=value; panel.SetControlAuthority(value); }
        public void ApplyRemoteParameters(LaunchParameters value) { panel.ApplyParameters(value); ApplyControls(); }
        public void BeginRemoteLaunch(LaunchParameters value)
        { ApplyRemoteParameters(value); BeginLaunch(); }
        public void ClearRemoteHistory() => ClearLab();
        public void Bind(ObliqueLaunchSimulation sim, ObliqueLaunchPanel ui, ProjectileView projectileView, TrajectorySnapshotPool pool, ProjectileTrajectoryLine trajectoryView, MeasurementRulers rulerView, Transform cylinder, Transform pivot, Transform muzzleTransform, Rigidbody body)
        { simulation=sim; panel=ui; projectile=projectileView; snapshots=pool; trajectory=trajectoryView; rulers=rulerView; baseCylinder=cylinder; cannonPivot=pivot; muzzle=muzzleTransform; projectileBody=body; }
        void OnEnable()
        {
            panel.FireRequested+=Fire; panel.PauseRequested+=Pause; panel.ResetRequested+=ResetLab;
            panel.VectorVisibilityChanged+=SetVectorVisibility;
            simulation.SampleChanged+=OnSample; simulation.StateChanged+=panel.ShowState; panel.ShowState(simulation.State); ApplyControls();
        }
        void OnDisable()
        { panel.FireRequested-=Fire; panel.PauseRequested-=Pause; panel.ResetRequested-=ResetLab; panel.VectorVisibilityChanged-=SetVectorVisibility; simulation.SampleChanged-=OnSample; simulation.StateChanged-=panel.ShowState; }
        void Pause() { if(controlAuthority) simulation.TogglePause(); }
        void SetVectorVisibility(bool visible)
        {
            foreach(var view in GetComponentsInChildren<ProjectileView>(true)) view.SetVectorsVisible(visible);
            if(simulation.State==SimulationState.Idle || simulation.State==SimulationState.Complete) projectile.HideVectors();
        }
        void Update() { if(simulation.State==SimulationState.Idle||simulation.State==SimulationState.Complete) ApplyControls(); }
        void ApplyControls()
        {
            var p=panel.Parameters; float h=Mathf.Max(.1f,p.Height);
            baseCylinder.localScale=new Vector3(1f,h*.5f,1f); baseCylinder.localPosition=new Vector3(0f,h*.5f,0f);
            if(telescopicBase!=null) telescopicBase.SetHeight(p.Height);
            cannonPivot.position=LaunchOrigin; cannonPivot.rotation=Quaternion.Euler(0f,0f,p.Angle-90f);
            rulers.ShowLaunchGeometry(LaunchOrigin,p.Angle,p.Height);
            projectileBody.mass=p.Mass; if(simulation.State==SimulationState.Idle) projectile.transform.position=LaunchOrigin;
        }
        Vector3 LaunchOrigin => new Vector3(baseCylinder.position.x,panel.Parameters.Height,baseCylinder.position.z);
        void Fire()
        { if(controlAuthority) BeginLaunch(); }
        void BeginLaunch()
        {
            snapshots.Clear(); rulers.Clear(); ApplyControls(); projectile.HideVectors();
            var parameters=panel.Parameters;
            trajectory.Begin(LaunchOrigin,parameters);
            rulers.Begin(new Vector3(baseCylinder.position.x,0f,baseCylinder.position.z));
            simulation.Launch(parameters,LaunchOrigin);
            Launched?.Invoke();
        }
        void ResetLab()
        { if(controlAuthority) ClearLab(); }
        void ClearLab()
        { simulation.ResetSimulation(); snapshots.Clear(); trajectory.Clear(); rulers.Clear(); projectile.HideVectors(); ApplyControls(); panel.ShowState(SimulationState.Idle); ResetRequested?.Invoke(); }
        void OnSample(FlightSample sample)
        {
            if(simulation.State==SimulationState.Idle){projectile.transform.position=LaunchOrigin;projectile.HideVectors();return;}
            projectile.transform.position=sample.Position;
            if(simulation.State==SimulationState.Complete) projectile.HideVectors(); else projectile.Show(sample,simulation.Parameters.Gravity);
            panel.ShowSample(sample); snapshots.Record(sample,simulation.Parameters.Gravity); trajectory.Plot(sample); rulers.UpdateMeasurements(sample);
        }
    }
}
