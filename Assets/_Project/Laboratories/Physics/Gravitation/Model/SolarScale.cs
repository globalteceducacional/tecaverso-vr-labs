namespace Tecaverso.Labs.Gravitation
{
    /// <summary>Shared, linear scale contracts. No per-body visual size overrides.</summary>
    public static class SolarScale
    {
        public const double KilometresPerAU=149597870.7;
        public const double SunDiameterKm=1391400;
        // NASA Planetary Fact Sheet reference diameters, in planetary order.
        public static readonly double[] DiameterKm={4879,12104,12756,6792,142984,120536,51118,49528};
        public const double WalkthroughMagnification=2;
        public static double MetresPerAU(bool inner)=>(inner?4:.2)*WalkthroughMagnification;
        public static double DistanceKmPerMetre(bool inner)=>KilometresPerAU/MetresPerAU(inner);
        public static double BodyKmPerMetre(bool inner,bool realScale)=>
            realScale?DistanceKmPerMetre(inner):(inner?700000:14000000)/WalkthroughMagnification;
        public static double BodyDiameter(double diameterKm,bool inner,bool realScale)=>
            diameterKm/BodyKmPerMetre(inner,realScale);
    }
}
