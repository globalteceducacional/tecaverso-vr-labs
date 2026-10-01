using UnityEngine;

namespace Tecaverso.Labs.ObliqueLaunch
{
    public sealed class ObliqueLaunchLab : MonoBehaviour
    {
        [SerializeField] ObliqueLaunchSimulation simulation;
        [SerializeField] ObliqueLaunchPanel panel;
        [SerializeField] ProjectileView projectile;
        [SerializeField] TrajectorySnapshotPool snapshots;
        [SerializeField] HeightTimeGraph graph;
        [SerializeField] MeasurementRulers rulers;
        [SerializeField] Transform baseCylinder, cannonPivot, muzzle;
        [SerializeField] Rigidbody projectileBody;
        public void Bind(ObliqueLaunchSimulation sim, ObliqueLaunchPanel ui, ProjectileView projectileView, TrajectorySnapshotPool pool, HeightTimeGraph graphView, MeasurementRulers rulerView, Transform cylinder, Transform pivot, Transform muzzleTransform, Rigidbody body)
        { simulation=sim; panel=ui; projectile=projectileView; snapshots=pool; graph=graphView; rulers=rulerView; baseCylinder=cylinder; cannonPivot=pivot; muzzle=muzzleTransform; projectileBody=body; }
        void OnEnable()
        {
            panel.FireRequested+=Fire; panel.PauseRequested+=simulation.TogglePause; panel.ResetRequested+=ResetLab;
            simulation.SampleChanged+=OnSample; simulation.StateChanged+=panel.ShowState; panel.ShowState(simulation.State); ApplyControls();
        }
        void OnDisable()
        { panel.FireRequested-=Fire; panel.PauseRequested-=simulation.TogglePause; panel.ResetRequested-=ResetLab; simulation.SampleChanged-=OnSample; simulation.StateChanged-=panel.ShowState; }
        void Update() { if(simulation.State==SimulationState.Idle||simulation.State==SimulationState.Complete) ApplyControls(); }
        void ApplyControls()
        {
            var p=panel.Parameters; float h=Mathf.Max(.1f,p.Height);
            baseCylinder.localScale=new Vector3(1f,h*.5f,1f); baseCylinder.localPosition=new Vector3(0f,h*.5f,0f);
            cannonPivot.position=new Vector3(0f,p.Height+.35f,0f); cannonPivot.localRotation=Quaternion.Euler(0f,0f,p.Angle-90f);
            projectileBody.mass=p.Mass; if(simulation.State!=SimulationState.Running&&simulation.State!=SimulationState.Paused) projectile.transform.position=muzzle.position;
        }
        void Fire()
        { snapshots.Clear(); graph.Clear(); rulers.Clear(); ApplyControls(); graph.Begin(panel.Parameters); simulation.Launch(panel.Parameters); }
        void ResetLab()
        { simulation.ResetSimulation(); snapshots.Clear(); graph.Clear(); rulers.Clear(); ApplyControls(); panel.ShowState(SimulationState.Idle); }
        void OnSample(FlightSample sample)
        { if(simulation.State==SimulationState.Idle){projectile.transform.position=muzzle.position;return;} projectile.Show(sample); panel.ShowSample(sample); snapshots.Record(sample); graph.Plot(sample); rulers.UpdateMeasurements(sample); }
    }
}
