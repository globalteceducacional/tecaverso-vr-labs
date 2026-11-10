using System;

namespace Tecaverso.Labs.Chemistry.Reactions
{
    /// <summary>Stoichiometric educational model. No particle collisions, kinetics or networking.</summary>
    public sealed class ReactionSession
    {
        public const double CapacityMl=500, StockMl=300, PowderGrams=6, SpoonCapacityGrams=1;
        public const double BicarbonateMolarMass=84.0066, CarbonateMolarMass=100.0869;
        public ReactionDefinition Definition {get; private set;}
        public double[] SourceMl {get;}=new double[2];
        public double[] AddedMol {get;}=new double[2];
        public double[] RemainingMol {get;}=new double[2];
        public double[] SpilledMol {get;}=new double[2];
        public double[] ProductsMol {get; private set;}
        public double VolumeMl {get; private set;}
        public double SpilledMl {get; private set;}
        public double PowderStock {get; private set;}
        public double SpoonGrams {get; private set;}
        public double SpilledPowder {get; private set;}
        public double ExtentMol {get; private set;}
        public const double InitialTemperatureC=25, SolutionHeatCapacity=4.18, VesselHeatCapacity=80;
        public bool HasThermalModel=>!double.IsNaN(Definition.ReactionEnthalpyKJPerMol);
        public double HeatReleasedJ=>HasThermalModel?-Definition.ReactionEnthalpyKJPerMol*1000*ExtentMol:double.NaN;
        public double TemperatureC=>HasThermalModel&&VolumeMl>0?InitialTemperatureC+HeatReleasedJ/(VesselHeatCapacity+VolumeMl*SolutionHeatCapacity):double.NaN;
        public double TemperatureChangeC=>TemperatureC-InitialTemperatureC;
        public bool Paused {get;set;}
        public bool IndicatorPresent {get;private set;}
        public bool AddIndicator(){if(Paused||!Definition.SupportsPH)return false;IndicatorPresent=true;return true;}
        public bool Used=>VolumeMl>0||SpilledMl>0||PowderStock<PowderGrams||AddedMol[1]>0||IndicatorPresent;
        public ReactionSession(ReactionKind kind=ReactionKind.Gas)=>Reset(kind);
        public void Reset(ReactionKind kind)
        {
            if((int)kind<0||(int)kind>=ReactionDefinition.All.Length)throw new ArgumentOutOfRangeException(nameof(kind));
            Reset(ReactionDefinition.All[(int)kind]);
        }
        public ReactionSession(ReactionDefinition definition)=>Reset(definition);
        public void Reset(ReactionDefinition definition)
        {
            Definition=definition??throw new ArgumentNullException(nameof(definition));
            SourceMl[0]=StockMl;SourceMl[1]=Definition.PowderSecond?0:StockMl;
            Array.Clear(AddedMol,0,2);Array.Clear(RemainingMol,0,2);Array.Clear(SpilledMol,0,2);
            ProductsMol=new double[Definition.Species.Length-2];
            VolumeMl=SpilledMl=SpoonGrams=SpilledPowder=ExtentMol=0;PowderStock=PowderGrams;Paused=false;IndicatorPresent=false;
        }
        public readonly struct Transfer
        {
            public readonly double Removed,Accepted,Spilled,Reacted;
            public Transfer(double removed,double accepted,double spilled,double reacted){Removed=removed;Accepted=accepted;Spilled=spilled;Reacted=reacted;}
        }
        static void Check(double amount){if(double.IsNaN(amount)||double.IsInfinity(amount)||amount<0)throw new ArgumentOutOfRangeException(nameof(amount));}
        public Transfer PourLiquid(int source,double requestedMl,bool hitsReceiver)
        {
            Check(requestedMl);
            if(source<0||source>1)throw new ArgumentOutOfRangeException(nameof(source));
            if(Paused||source==1&&Definition.PowderSecond)return default;
            double removed=Math.Min(requestedMl,SourceMl[source]);
            double accepted=hitsReceiver?Math.Min(removed,Math.Max(0,CapacityMl-VolumeMl)):0;
            double spilled=removed-accepted;
            SourceMl[source]-=removed;VolumeMl+=accepted;SpilledMl+=spilled;
            double moles=accepted*.001*ReactionDefinition.Concentration;
            AddedMol[source]+=moles;RemainingMol[source]+=moles;SpilledMol[source]+=spilled*.001*ReactionDefinition.Concentration;
            return new Transfer(removed,accepted,spilled,React());
        }
        public double LoadSpoon()
        {
            if(Paused||!Definition.PowderSecond)return 0;
            double amount=Math.Min(PowderStock,Math.Max(0,SpoonCapacityGrams-SpoonGrams));
            PowderStock-=amount;SpoonGrams+=amount;return amount;
        }
        public Transfer PourPowder(double requestedGrams,bool hitsReceiver)
        {
            Check(requestedGrams);if(Paused||!Definition.PowderSecond)return default;
            double removed=Math.Min(requestedGrams,SpoonGrams);SpoonGrams-=removed;
            double accepted=hitsReceiver?removed:0;double spilled=removed-accepted;
            SpilledPowder+=spilled;SpilledMol[1]+=spilled/Definition.SolidMolarMass;
            AddedMol[1]+=accepted/Definition.SolidMolarMass;RemainingMol[1]+=accepted/Definition.SolidMolarMass;
            return new Transfer(removed,accepted,spilled,React());
        }
        double React()
        {
            double extent=Math.Min(RemainingMol[0]/Definition.Stoichiometry[0],RemainingMol[1]/Definition.Stoichiometry[1]);
            for(int i=0;i<2;i++)RemainingMol[i]=Math.Max(0,RemainingMol[i]-extent*Definition.Stoichiometry[i]);
            for(int i=0;i<ProductsMol.Length;i++)ProductsMol[i]+=extent*Definition.Stoichiometry[i+2];
            ExtentMol+=extent;return extent;
        }
        // Ideal dilute strong acid/base at 25 C, including water auto-ionization near equivalence.
        public double PH
        {
            get {
                if(!Definition.SupportsPH||VolumeMl<=0)return double.NaN;
                double excess=(RemainingMol[0]-RemainingMol[1])/(VolumeMl*.001);
                double radical=Math.Sqrt(excess*excess+4e-14);
                double hydrogen=excess>=0?(excess+radical)/2:2e-14/(radical-excess);
                return -Math.Log10(hydrogen);
            }
        }
        public double AtomConservationError(string element)
        {
            double before=0,after=0;
            for(int i=0;i<2;i++){before+=AddedMol[i]*Definition.AtomCount(i,element);after+=RemainingMol[i]*Definition.AtomCount(i,element);}
            for(int i=0;i<ProductsMol.Length;i++)after+=ProductsMol[i]*Definition.AtomCount(i+2,element);
            return after-before;
        }
    }
}
