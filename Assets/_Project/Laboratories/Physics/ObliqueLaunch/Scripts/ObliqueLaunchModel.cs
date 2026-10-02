using UnityEngine;

namespace Tecaverso.Labs.ObliqueLaunch
{
    public readonly struct LaunchParameters
    {
        public readonly float Angle, Speed, Height, Mass, Gravity;
        public LaunchParameters(float angle, float speed, float height, float mass, float gravity)
        {
            Angle = Mathf.Clamp(angle, 0f, 90f);
            Speed = Mathf.Round(Mathf.Clamp(speed, 0f, 30f));
            Height = Mathf.Clamp(height, 0f, 10f);
            Mass = Mathf.Clamp(mass, 1f, 10f);
            Gravity = Mathf.Clamp(gravity, 5f, 20f);
        }
        public Vector3 InitialVelocity
        {
            get
            {
                float radians = Angle * Mathf.Deg2Rad;
                return new Vector3(Speed * Mathf.Cos(radians), Speed * Mathf.Sin(radians), 0f);
            }
        }
    }

    public readonly struct FlightSample
    {
        public readonly float Time, MaximumHeight;
        public readonly Vector3 Position, Velocity;
        public FlightSample(float time, Vector3 position, Vector3 velocity, float maximumHeight)
        { Time = time; Position = position; Velocity = velocity; MaximumHeight = maximumHeight; }
    }

    public static class ProjectileKinematics
    {
        public static FlightSample Evaluate(in LaunchParameters p, float time)
        {
            var v0 = p.InitialVelocity;
            var position = new Vector3(v0.x * time, p.Height + v0.y * time - .5f * p.Gravity * time * time, 0f);
            var velocity = new Vector3(v0.x, v0.y - p.Gravity * time, 0f);
            float peak = p.Height + (v0.y * v0.y) / (2f * p.Gravity);
            return new FlightSample(time, position, velocity, peak);
        }

        public static FlightSample Evaluate(in LaunchParameters p, float time, Vector3 origin)
        {
            var v0 = p.InitialVelocity;
            var position = origin + new Vector3(v0.x * time, v0.y * time - .5f * p.Gravity * time * time, 0f);
            var velocity = new Vector3(v0.x, v0.y - p.Gravity * time, 0f);
            float peak = origin.y + (v0.y * v0.y) / (2f * p.Gravity);
            return new FlightSample(time, position, velocity, peak);
        }
    }
}
