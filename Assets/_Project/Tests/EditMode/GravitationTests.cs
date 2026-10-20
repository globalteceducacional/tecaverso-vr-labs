using System;
using NUnit.Framework;
using Tecaverso.Labs.Gravitation;

public class GravitationTests
{
    static OrbitalModel Orbit(double altitude=.5,double speed=1,double mass=500,double multiplier=1)
    {
        var m=new OrbitalModel();m.Configure(CelestialBody.Presets[0],altitude,speed,mass,multiplier);return m;
    }
    [Test]
    public void ForceObeysInverseSquareAndScalesWithBothMasses()
    {
        var near=Orbit(1);var far=Orbit(3);
        Assert.That(near.ForceNewtons/far.ForceNewtons,Is.EqualTo(4).Within(1e-10));
        Assert.That(Orbit(1,1,1000).ForceNewtons/near.ForceNewtons,Is.EqualTo(2).Within(1e-10));
        Assert.That(Orbit(1,1,500,2).ForceNewtons/near.ForceNewtons,Is.EqualTo(2).Within(1e-10));
    }
    [Test]
    public void CircularOrbitClosesAndPreservesEnergy()
    {
        var m=Orbit();var start=m.Position;double energy=m.SpecificEnergy;
        double period=2*Math.PI*Math.Pow(start.Length,1.5)*m.TimeUnit;
        m.Launch();
        for(int i=0;i<1000;i++)m.Advance(period/1000);
        Assert.AreEqual(OrbitState.Running,m.State);
        Assert.That((m.Position-start).Length,Is.LessThan(2e-5));
        Assert.That(m.SpecificEnergy,Is.EqualTo(energy).Within(1e-8));
    }
    [Test]
    public void OrbitDoesNotDependOnSatelliteMassOrFramePartition()
    {
        var a=Orbit(mass:100);var b=Orbit(mass:2000);
        a.Launch();b.Launch();
        for(int i=0;i<100;i++)a.Advance(10);
        for(int i=0;i<1000;i++)b.Advance(1);
        Assert.That((a.Position-b.Position).Length,Is.LessThan(1e-5));
    }
    [Test]
    public void ZeroLaunchSpeedHitsSurfaceWithoutPenetration()
    {
        var m=Orbit(speed:0);m.Launch();
        for(int i=0;i<1000&&m.State==OrbitState.Running;i++)m.Advance(10);
        Assert.AreEqual(OrbitState.Collision,m.State);
        Assert.That(m.Position.Length,Is.EqualTo(1).Within(1e-10));
        var p=m.Position;m.Advance(100);Assert.AreEqual(p,m.Position);
    }
    [Test]
    public void AboveEscapeSpeedLeavesObservationArea()
    {
        var m=Orbit(speed:1.6);Assert.Greater(m.SpecificEnergy,0);m.Launch();
        for(int i=0;i<1000&&m.State==OrbitState.Running;i++)m.Advance(10);
        Assert.AreEqual(OrbitState.Escaped,m.State);
        Assert.GreaterOrEqual(m.Position.Length,OrbitalModel.DisplayRadius);
    }
    [Test]
    public void BoundOrbitOutsideDisplayIsNotMislabelledAsEscape()
    {
        var m=Orbit(altitude:2,speed:1.2);m.Launch();
        for(int i=0;i<10000&&m.State==OrbitState.Running;i++)m.Advance(10);
        Assert.AreEqual(OrbitState.OutsideView,m.State);Assert.Less(m.SpecificEnergy,0);
    }
    [Test]
    public void PauseAndResetKeepSimulationStateExplicit()
    {
        var m=Orbit();m.Launch();m.Advance(10);m.TogglePause();
        var p=m.Position;m.Advance(100);Assert.AreEqual(p,m.Position);
        Assert.Throws<InvalidOperationException>(()=>m.Configure(CelestialBody.Presets[1],1,1,500,1));
        m.Reset();m.Configure(CelestialBody.Presets[1],1,1,500,1);
        Assert.AreEqual(OrbitState.Ready,m.State);Assert.AreEqual(0,m.ElapsedSeconds);
        Assert.That(m.EscapeSpeedKm/m.CircularSpeedKm,Is.EqualTo(Math.Sqrt(2)).Within(1e-10));
    }
    [Test]
    public void PlanetPeriodsCloseEllipsesAndPerihelionIsFaster()
    {
        foreach(var p in PlanetOrbit.Planets)
        {
            Assert.That((p.PositionAt(p.PeriodDays)-p.PositionAt(0)).Length,Is.LessThan(1e-8));
            Assert.That(p.PositionAt(0).Length,Is.EqualTo(p.SemiMajorAU*(1-p.Eccentricity)).Within(1e-8));
            Assert.That(p.PositionAt(p.PeriodDays/2).Length,Is.EqualTo(p.SemiMajorAU*(1+p.Eccentricity)).Within(1e-8));
            double step=p.PeriodDays/10000;
            Assert.Greater((p.PositionAt(step)-p.PositionAt(0)).Length,
                (p.PositionAt(p.PeriodDays/2+step)-p.PositionAt(p.PeriodDays/2)).Length);
        }
    }
}
