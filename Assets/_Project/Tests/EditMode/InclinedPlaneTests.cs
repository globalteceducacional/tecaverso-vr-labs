using NUnit.Framework;
using Tecaverso.Labs.InclinedPlane;
using UnityEngine;

public class InclinedPlaneTests
{
    [TestCase(0f)] [TestCase(20f)]
    public void StaticFrictionBalancesOnlyTheRequiredForce(float angle)
    {
        var f=new InclineForces(new InclineParameters(angle,3,9.81f,.65f,.45f));
        Assert.IsFalse(f.Slides);
        Assert.That((f.Weight+f.Normal+f.Friction).magnitude,Is.LessThan(.0001f));
        Assert.That(f.Friction.magnitude,Is.LessThan(f.StaticLimit));
    }
    [Test]
    public void CriticalAngleRemainsInEquilibriumButGreaterAngleSlides()
    {
        float critical=Mathf.Atan(.5f)*Mathf.Rad2Deg;
        Assert.IsFalse(new InclineForces(new InclineParameters(critical,3,9.81f,.5f,.3f)).Slides);
        Assert.IsTrue(new InclineForces(new InclineParameters(critical+.01f,3,9.81f,.5f,.3f)).Slides);
    }
    [Test]
    public void MassScalesForcesButNotAcceleration()
    {
        var light=new InclineForces(new InclineParameters(30,1,9.81f,.25f,.15f));
        var heavy=new InclineForces(new InclineParameters(30,10,9.81f,.25f,.15f));
        Assert.That(heavy.Acceleration,Is.EqualTo(light.Acceleration).Within(.00001f));
        Assert.That(heavy.Normal.magnitude,Is.EqualTo(light.Normal.magnitude*10).Within(.0001f));
        Assert.That((heavy.Weight+heavy.Normal+heavy.Friction-heavy.Resultant).magnitude,Is.LessThan(.0001f));
    }
    [Test]
    public void FrictionlessMotionMatchesAnalyticalSolutionAndExactEndpoint()
    {
        var model=new InclinedPlaneModel();model.Configure(new InclineParameters(30,3,10,0,0));model.Release();
        model.Advance(.5f);
        Assert.That(model.Distance,Is.EqualTo(.625f).Within(.00001f));
        Assert.That(model.Speed,Is.EqualTo(2.5f).Within(.00001f));
        model.Advance(100);
        Assert.AreEqual(InclineState.Complete,model.State);
        Assert.That(model.Distance,Is.EqualTo(InclinedPlaneModel.Travel));
        Assert.That(model.Elapsed,Is.EqualTo(Mathf.Sqrt(2*InclinedPlaneModel.Travel/5)).Within(.00001f));
    }
    [Test]
    public void PauseFreezesMotionAndResetUnlocksParameters()
    {
        var model=new InclinedPlaneModel();var p=new InclineParameters(45,3,10,.5f,.3f);
        model.Configure(p);model.Release();model.Advance(.1f);model.TogglePause();
        float distance=model.Distance;model.Advance(1);
        Assert.AreEqual(distance,model.Distance);Assert.IsFalse(model.Configure(p));
        model.TogglePause();model.Advance(.1f);Assert.Greater(model.Distance,distance);
        model.Reset();Assert.AreEqual(0,model.Distance);Assert.IsTrue(model.Configure(p));
    }
    [Test]
    public void EquilibriumDoesNotDriftAndCoefficientsStayConsistent()
    {
        var model=new InclinedPlaneModel();var p=new InclineParameters(0,3,9.81f,.2f,.9f);
        Assert.AreEqual(.2f,p.KineticFriction);
        model.Configure(p);model.Release();model.Advance(100);
        Assert.AreEqual(InclineState.Equilibrium,model.State);Assert.AreEqual(0,model.Distance);
    }
}
