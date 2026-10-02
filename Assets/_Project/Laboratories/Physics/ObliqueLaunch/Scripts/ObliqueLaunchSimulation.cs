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
            if (State != SimulationState.Running) return;
            Current = ProjectileKinematics.Evaluate(Parameters, Current.Time + Time.deltaTime, Origin);
            if (Current.Position.y <= 0f && Current.Time > 0f)
            {
                Current = new FlightSample(Current.Time, new Vector3(Current.Position.x, 0f, Origin.z), Current.Velocity, Current.MaximumHeight);
                SetState(SimulationState.Complete);
            }
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
