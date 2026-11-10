using System;
using NUnit.Framework;
namespace Tecaverso.Labs.Chemistry.Reactions.Tests
{
    public class ReactionThermalTests
    {
        [TestCase(2)][TestCase(7)]
        public void NeutralizationUsesAcceptedMolesAndHeatCapacity(int recipe)
        {
            var s=new ReactionSession(ReactionDefinition.All[recipe]);Assert.That(double.IsNaN(s.TemperatureC),Is.True);
            s.PourLiquid(0,100,true);Assert.That(s.TemperatureC,Is.EqualTo(25));s.PourLiquid(1,100,true);
            Assert.That(s.HeatReleasedJ,Is.EqualTo(558.3).Within(1e-8));
            Assert.That(s.TemperatureC,Is.EqualTo(25+558.3/916).Within(1e-9));
            double heat=s.HeatReleasedJ,temp=s.TemperatureC;s.PourLiquid(0,50,false);
            Assert.That(s.HeatReleasedJ,Is.EqualTo(heat));Assert.That(s.TemperatureC,Is.EqualTo(temp));
            s.PourLiquid(0,50,true);Assert.That(s.HeatReleasedJ,Is.EqualTo(heat));Assert.That(s.TemperatureC,Is.LessThan(temp));
        }
        [Test] public void UnsupportedPairsAreNotReportedAsNoTemperatureChange()
        {for(int i=0;i<12;i++){var s=new ReactionSession(ReactionDefinition.All[i]);s.PourLiquid(0,100,true);Assert.That(s.HasThermalModel,Is.EqualTo(i==2||i==7));if(!s.HasThermalModel)Assert.That(double.IsNaN(s.TemperatureC)&&double.IsNaN(s.HeatReleasedJ),Is.True);}}
        [Test] public void OrderAndIncrementalDosesConserveEnergy()
        {
            var a=new ReactionSession(ReactionDefinition.All[2]);var b=new ReactionSession(ReactionDefinition.All[2]);a.PourLiquid(0,100,true);a.PourLiquid(1,100,true);
            for(int i=0;i<100;i++){b.PourLiquid(1,1,true);b.PourLiquid(0,1,true);}
            Assert.That(b.TemperatureC,Is.EqualTo(a.TemperatureC).Within(1e-9));b.Paused=true;b.PourLiquid(0,20,true);Assert.That(b.VolumeMl,Is.EqualTo(200));
            b.Reset(ReactionDefinition.All[2]);Assert.That(b.HeatReleasedJ,Is.Zero);Assert.That(double.IsNaN(b.TemperatureC),Is.True);
        }
        [Test] public void OverflowDoesNotGenerateHeatForDiscardedReagent()
        {
            var s=new ReactionSession(ReactionDefinition.All[2]);s.PourLiquid(0,300,true);s.PourLiquid(1,300,true);
            Assert.That(s.VolumeMl,Is.EqualTo(500));Assert.That(s.SpilledMl,Is.EqualTo(100));Assert.That(s.HeatReleasedJ,Is.EqualTo(1116.6).Within(1e-8));
        }
    }
}
