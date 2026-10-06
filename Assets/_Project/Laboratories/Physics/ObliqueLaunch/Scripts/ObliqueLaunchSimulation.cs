using System;
using UnityEngine;

namespace Tecaverso.Labs.ObliqueLaunch
{
    public enum SimulationState { Idle, Running, Paused, Complete }

    public sealed class ObliqueLaunchSimulation : MonoBehaviour
    {
        public SimulationState State { get; private set; } = SimulationState.Idle;
        public FlightSample Current { get; private set; }
        public LaunchParameters Parameters { get; private set; }
        public Vector3 Origin { get; private set; }
        public bool RemoteDriven { get; set; }
        public event Action<FlightSample> SampleChanged;
        public event Action<SimulationState> StateChanged;

        public void Launch(LaunchParameters parameters, Vector3 origin)
        {
            Parameters = parameters;
            Origin = origin;
            Current = ProjectileKinematics.Evaluate(parameters, 0f, Origin);
            SetState(SimulationState.Running);
            SampleChanged?.Invoke(Current);
        }

        public void TogglePause()
        {
            if (State == SimulationState.Running) SetState(SimulationState.Paused);
            else if (State == SimulationState.Paused) SetState(SimulationState.Running);
        }

        public void ResetSimulation()
        {
            Current = default;
            SetState(SimulationState.Idle);
            SampleChanged?.Invoke(Current);
        }

        void Update()
        {
            if (RemoteDriven || State != SimulationState.Running) return;
            ApplyTime(Current.Time + Time.deltaTime, SimulationState.Running);
        }

        // Both peers evaluate the same analytical flight, including the exact ground intersection.
        public void ApplyTime(float time, SimulationState state)
        {
            float vy = Parameters.InitialVelocity.y;
            float landing = (vy + Mathf.Sqrt(vy * vy + 2f * Parameters.Gravity * Mathf.Max(0f, Origin.y))) / Parameters.Gravity;
            time = Mathf.Clamp(time, 0f, landing);
            Current = ProjectileKinematics.Evaluate(Parameters, time, Origin);
            if (time >= landing)
            {
                Current = new FlightSample(Current.Time, new Vector3(Current.Position.x, 0f, Origin.z), Current.Velocity, Current.MaximumHeight);
                state = SimulationState.Complete;
            }
            SetState(state);
            SampleChanged?.Invoke(Current);
        }

        void SetState(SimulationState value)
        {
            if (State == value) return;
            State = value;
            StateChanged?.Invoke(value);
        }
    }
}
