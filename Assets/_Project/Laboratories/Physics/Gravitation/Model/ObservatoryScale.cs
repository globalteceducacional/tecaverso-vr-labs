using System;
namespace Tecaverso.Labs.Gravitation
{
    /// <summary>Exhibit metres, deliberately not a physical scale.</summary>
    public static class ObservatoryScale
    {
        public const double SunDiameter=3;
        public static readonly double[] OrbitRadius={2.7,4,5.3,6.5,8,10.1,12.2,14};
        public static double Diameter(int i)=>.4+Math.Log(SolarScale.DiameterKm[i]/4879)/Math.Log(142984.0/4879);
    }
}
