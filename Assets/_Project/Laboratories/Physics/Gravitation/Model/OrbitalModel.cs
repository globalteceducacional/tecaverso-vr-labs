using System;

namespace Tecaverso.Labs.Gravitation
{
    public readonly struct Double2
    {
        public readonly double X, Y;
        public Double2(double x, double y) { X=x; Y=y; }
        public double Length => Math.Sqrt(X*X+Y*Y);
        public static Double2 operator +(Double2 a, Double2 b) => new(a.X+b.X,a.Y+b.Y);
        public static Double2 operator -(Double2 a, Double2 b) => new(a.X-b.X,a.Y-b.Y);
        public static Double2 operator *(Double2 a, double s) => new(a.X*s,a.Y*s);
        public static double Dot(Double2 a, Double2 b) => a.X*b.X+a.Y*b.Y;
    }

    public readonly struct CelestialBody
    {
        public readonly string Name;
        public readonly double RadiusKm, MuKm;
        public CelestialBody(string name,double radiusKm,double muKm)
        { Name=name; RadiusKm=radiusKm; MuKm=muKm; }
        // Rounded mean radii and gravitational parameters; sources in README.
        public static readonly CelestialBody[] Presets = {
            new("Terra",6371,398600.435), new("Lua",1737.4,4902.8), new("Marte",3389.5,42828.3)
        };
    }

    public enum OrbitState { Ready, Running, Paused, Collision, Escaped, OutsideView }

    /// <summary>
    /// Restricted two-body model. Central body fixed, massless orbiting test particle
    /// (its mass affects force only). No drag or thrust. Integration uses radius/time
    /// normalized units and double precision velocity Verlet.
    /// </summary>
    public sealed class OrbitalModel
    {
        public CelestialBody Body { get; private set; }
        public double MassMultiplier { get; private set; } = 1;
        public double SatelliteMass { get; private set; } = 500;
        public Double2 Position { get; private set; }
        public Double2 Velocity { get; private set; }
        public double ElapsedSeconds { get; private set; }
        public OrbitState State { get; private set; } = OrbitState.Ready;
        public const double DisplayRadius = 5;
        public double Mu => Body.MuKm*MassMultiplier;
        public double TimeUnit => Math.Sqrt(Body.RadiusKm*Body.RadiusKm*Body.RadiusKm/Mu);
        public double VelocityUnit => Math.Sqrt(Mu/Body.RadiusKm);
        public double RadiusKm => Position.Length*Body.RadiusKm;
        public double SpeedKm => Velocity.Length*VelocityUnit;
        public double ForceNewtons => SatelliteMass*Mu*1000/(RadiusKm*RadiusKm);
        public double SurfaceForce => SatelliteMass*Mu*1000/(Body.RadiusKm*Body.RadiusKm);
        public double SpecificEnergy => .5*Velocity.Length*Velocity.Length-1/Position.Length;
        public double CircularSpeedKm => Math.Sqrt(Mu/RadiusKm);
        public double EscapeSpeedKm => Math.Sqrt(2)*CircularSpeedKm;

        public void Configure(CelestialBody body,double altitudeR,double speedFactor,double mass,double multiplier)
        {
            if(State==OrbitState.Running||State==OrbitState.Paused)
                throw new InvalidOperationException("Reset the attempt before changing its parameters.");
            if(body.RadiusKm<=0||body.MuKm<=0||!Finite(altitudeR)||!Finite(speedFactor)||!Finite(mass)||!Finite(multiplier))
                throw new ArgumentOutOfRangeException();
            Body=body;
            SatelliteMass=Math.Clamp(mass,100,2000);
            MassMultiplier=Math.Clamp(multiplier,.25,2);
            double radius=1+Math.Clamp(altitudeR,.1,3);
            Position=new Double2(radius,0);
            Velocity=new Double2(0,Math.Clamp(speedFactor,0,1.8)/Math.Sqrt(radius));
            ElapsedSeconds=0; State=OrbitState.Ready;
        }
        public void Launch() { if(State==OrbitState.Ready)State=OrbitState.Running; }
        public void TogglePause()
        {
            if(State==OrbitState.Running)State=OrbitState.Paused;
            else if(State==OrbitState.Paused)State=OrbitState.Running;
        }
        public void Reset() { State=OrbitState.Ready; ElapsedSeconds=0; }
        static bool Finite(double x) => !double.IsNaN(x)&&!double.IsInfinity(x);
        static Double2 Acceleration(Double2 p) => p*(-1/Math.Pow(p.Length,3));
        public void Advance(double seconds)
        {
            if(State!=OrbitState.Running||!Finite(seconds)||seconds<=0)return;
            double remaining=seconds/TimeUnit;
            while(remaining>1e-12 && State==OrbitState.Running)
            {
                double dt=Math.Min(.002,remaining);
                var a=Acceleration(Position);
                var next=Position+Velocity*dt+a*(.5*dt*dt);
                var nextVelocity=Velocity+(a+Acceleration(next))*(.5*dt);
                if(next.Length<=1)
                {
                    // Locate surface intersection rather than displaying a point below ground.
                    var d=next-Position;
                    double aa=Double2.Dot(d,d),bb=2*Double2.Dot(Position,d),cc=Double2.Dot(Position,Position)-1;
                    double fraction=aa>0?Math.Clamp((-bb-Math.Sqrt(Math.Max(0,bb*bb-4*aa*cc)))/(2*aa),0,1):0;
                    Position=Position+d*fraction;
                    Velocity=Velocity+(nextVelocity-Velocity)*fraction;
                    ElapsedSeconds+=dt*TimeUnit*fraction;
                    State=OrbitState.Collision;
                    return;
                }
                Position=next; Velocity=nextVelocity;
                ElapsedSeconds+=dt*TimeUnit; remaining-=dt;
                if(Position.Length>=DisplayRadius)
                    State=SpecificEnergy>=0?OrbitState.Escaped:OrbitState.OutsideView;
            }
        }
    }

    public readonly struct PlanetOrbit
    {
        public readonly string Name;
        public readonly double SemiMajorAU, Eccentricity, PeriodDays;
        public PlanetOrbit(string name,double a,double e,double days)
        { Name=name; SemiMajorAU=a; Eccentricity=e; PeriodDays=days; }
        public static readonly PlanetOrbit[] Planets = {
            new("Mercúrio",.387,.206,88.0), new("Vênus",.723,.007,224.7),
            new("Terra",1,.017,365.2), new("Marte",1.524,.094,687.0),
            new("Júpiter",5.203,.049,4331), new("Saturno",9.572,.052,10747),
            new("Urano",19.165,.047,30589), new("Netuno",30.18,.010,59800)
        };
        // Keplerian educational orbits; coplanar, arbitrary initial phases, not ephemerides.
        public Double2 PositionAt(double days,double initialPhase=0)
        {
            double mean=(days/PeriodDays*2*Math.PI+initialPhase)%(2*Math.PI);
            double eccentric=mean;
            for(int i=0;i<8;i++)eccentric-=(eccentric-Eccentricity*Math.Sin(eccentric)-mean)/(1-Eccentricity*Math.Cos(eccentric));
            return EllipseAt(eccentric);
        }
        public Double2 EllipseAt(double eccentricAnomaly) =>
            new(SemiMajorAU*(Math.Cos(eccentricAnomaly)-Eccentricity),
                SemiMajorAU*Math.Sqrt(1-Eccentricity*Eccentricity)*Math.Sin(eccentricAnomaly));
    }
}
