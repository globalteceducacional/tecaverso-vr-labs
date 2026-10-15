using UnityEngine;

namespace Tecaverso.Labs.InclinedPlane
{
    public readonly struct InclineParameters
    {
        public readonly float Angle, Mass, Gravity, StaticFriction, KineticFriction;
        public InclineParameters(float angle, float mass, float gravity, float staticFriction, float kineticFriction)
        {
            Angle = Mathf.Clamp(angle, 0f, 60f);
            Mass = Mathf.Clamp(mass, 1f, 10f);
            Gravity = Mathf.Clamp(gravity, 5f, 20f);
            StaticFriction = Mathf.Clamp01(staticFriction);
            KineticFriction = Mathf.Clamp(kineticFriction, 0f, StaticFriction);
        }
        public Vector3 DownSlope => new Vector3(Mathf.Cos(Angle * Mathf.Deg2Rad), -Mathf.Sin(Angle * Mathf.Deg2Rad), 0);
        public Vector3 OutwardNormal => new Vector3(Mathf.Sin(Angle * Mathf.Deg2Rad), Mathf.Cos(Angle * Mathf.Deg2Rad), 0);
    }

    public readonly struct InclineForces
    {
        public readonly Vector3 Weight, Normal, Friction, ParallelWeight, PerpendicularWeight, Resultant;
        public readonly float Acceleration, StaticLimit;
        public readonly bool Slides;
        public InclineForces(InclineParameters p)
        {
            Weight = Vector3.down * p.Mass * p.Gravity;
            float normal = p.Mass * p.Gravity * Mathf.Cos(p.Angle * Mathf.Deg2Rad);
            float parallel = p.Mass * p.Gravity * Mathf.Sin(p.Angle * Mathf.Deg2Rad);
            StaticLimit = p.StaticFriction * normal;
            // Relative tolerance avoids numerical slip exactly at the critical angle.
            Slides = parallel > StaticLimit + 1e-6f * p.Mass * p.Gravity;
            Normal = p.OutwardNormal * normal;
            ParallelWeight = p.DownSlope * parallel;
            PerpendicularWeight = -Normal;
            Friction = -p.DownSlope * (Slides ? p.KineticFriction * normal : parallel);
            Acceleration = Slides ? Mathf.Max(0, (parallel - p.KineticFriction * normal) / p.Mass) : 0;
            Resultant = p.DownSlope * (p.Mass * Acceleration);
        }
    }

    public enum InclineState { Ready, Equilibrium, Sliding, Paused, Complete }

    /// <summary>Release from rest on a fixed slope. No rolling or collision impulse is simulated.</summary>
    public sealed class InclinedPlaneModel
    {
        public const float RampLength = 10f, StartOffset = .4f, Travel = RampLength - 2f * StartOffset;
        public InclineParameters Parameters { get; private set; }
        public InclineForces Forces { get; private set; }
        public InclineState State { get; private set; } = InclineState.Ready;
        public float Elapsed { get; private set; }
        public float Distance { get; private set; }
        public float Speed { get; private set; }
        public bool Configure(InclineParameters parameters)
        {
            if (State != InclineState.Ready) return false;
            Parameters = parameters;
            Forces = new InclineForces(parameters);
            return true;
        }
        public void Release()
        {
            if (State == InclineState.Ready) State = Forces.Slides ? InclineState.Sliding : InclineState.Equilibrium;
        }
        public void TogglePause()
        {
            if (State == InclineState.Sliding) State = InclineState.Paused;
            else if (State == InclineState.Paused) State = InclineState.Sliding;
        }
        public void Reset()
        {
            State = InclineState.Ready;
            Elapsed = Distance = Speed = 0;
        }
        public bool Advance(float deltaTime)
        {
            if (State != InclineState.Sliding || !float.IsFinite(deltaTime) || deltaTime <= 0) return false;
            float finish = Mathf.Sqrt(2f * Travel / Forces.Acceleration);
            Elapsed = Mathf.Min(Elapsed + deltaTime, finish);
            Distance = Mathf.Min(.5f * Forces.Acceleration * Elapsed * Elapsed, Travel);
            Speed = Forces.Acceleration * Elapsed;
            if (Elapsed >= finish) { Distance = Travel; State = InclineState.Complete; }
            return true;
        }
    }
}
