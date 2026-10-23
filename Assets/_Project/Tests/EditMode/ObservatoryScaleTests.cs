using NUnit.Framework;
using Tecaverso.Labs.Gravitation;

public sealed class ObservatoryScaleTests
{
    [Test]
    public void PlanetsAreReadableAndPreserveSizeOrdering()
    {
        for(int i=0;i<8;i++)
        {
            Assert.That(ObservatoryScale.Diameter(i),Is.InRange(.4,1.4000001));
            for(int j=0;j<8;j++)if(SolarScale.DiameterKm[i]>SolarScale.DiameterKm[j])
                Assert.That(ObservatoryScale.Diameter(i),Is.GreaterThan(ObservatoryScale.Diameter(j)));
        }
        Assert.That(ObservatoryScale.SunDiameter,Is.GreaterThan(ObservatoryScale.Diameter(4)));
    }
    [Test]
    public void OrbitsAndBodiesFitWithinRoomAndDoNotIntersect()
    {
        for(int i=0;i<8;i++)
        {
            double radius=ObservatoryScale.Diameter(i)/2;
            Assert.That(ObservatoryScale.OrbitRadius[i]+radius,Is.LessThan(15));
            if(i==0)Assert.That(ObservatoryScale.OrbitRadius[i]-radius,Is.GreaterThan(ObservatoryScale.SunDiameter/2));
            else Assert.That(ObservatoryScale.OrbitRadius[i]-ObservatoryScale.OrbitRadius[i-1],
                Is.GreaterThan(radius+ObservatoryScale.Diameter(i-1)/2));
        }
    }
}
