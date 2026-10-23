using NUnit.Framework;
using Tecaverso.Labs.Gravitation;

public sealed class SolarScaleTests
{
    [Test]
    public void WalkthroughDoublesDistancesAndDiametersWithoutChangingExaggeration()
    {
        foreach(bool inner in new[]{false,true})
        {
            Assert.That(SolarScale.MetresPerAU(inner),Is.EqualTo((inner?4:.2)*2));
            Assert.That(SolarScale.BodyDiameter(SolarScale.DiameterKm[2],inner,false),
                Is.EqualTo(SolarScale.DiameterKm[2]/(inner?700000:14000000)*2).Within(1e-12));
        }
    }
    [TestCase(false,false)]
    [TestCase(false,true)]
    [TestCase(true,false)]
    [TestCase(true,true)]
    public void EveryBodyPreservesItsReferenceRatio(bool inner,bool exact)
    {
        double sun=SolarScale.BodyDiameter(SolarScale.SunDiameterKm,inner,exact);
        foreach(double diameter in SolarScale.DiameterKm)
        {
            double body=SolarScale.BodyDiameter(diameter,inner,exact);
            Assert.That(sun/body,Is.EqualTo(SolarScale.SunDiameterKm/diameter).Within(1e-9));
            Assert.That(sun,Is.GreaterThan(body));
        }
    }
    [TestCase(false)]
    [TestCase(true)]
    public void ExactModeUsesTheSameUnitForDiametersAndDistances(bool inner)
    {
        double unit=SolarScale.MetresPerAU(inner)/SolarScale.KilometresPerAU;
        Assert.That(SolarScale.BodyDiameter(SolarScale.SunDiameterKm,inner,true),
            Is.EqualTo(SolarScale.SunDiameterKm*unit).Within(1e-12));
        Assert.That(SolarScale.BodyKmPerMetre(inner,true),Is.EqualTo(SolarScale.DistanceKmPerMetre(inner)));
    }
    [Test]
    public void BothViewsUseTheSameMagnificationForAllBodies()
    {
        foreach(bool exact in new[]{false,true})
            foreach(double diameter in SolarScale.DiameterKm)
                Assert.That(SolarScale.BodyDiameter(diameter,true,exact)/SolarScale.BodyDiameter(diameter,false,exact),
                    Is.EqualTo(20).Within(1e-10));
        Assert.That(SolarScale.DistanceKmPerMetre(false)/SolarScale.BodyKmPerMetre(false,false),
            Is.EqualTo(SolarScale.DistanceKmPerMetre(true)/SolarScale.BodyKmPerMetre(true,false)).Within(1e-10));
    }
    [Test]
    public void EnlargedSunDoesNotIntersectMercuryAtPerihelion()
    {
        foreach(bool inner in new[]{false,true})
        {
            var mercury=PlanetOrbit.Planets[0];
            double separation=mercury.SemiMajorAU*(1-mercury.Eccentricity)*SolarScale.MetresPerAU(inner);
            double radii=(SolarScale.BodyDiameter(SolarScale.SunDiameterKm,inner,false)+
                SolarScale.BodyDiameter(SolarScale.DiameterKm[0],inner,false))/2;
            Assert.That(separation,Is.GreaterThan(radii));
        }
    }
}
