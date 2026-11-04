using System;
using System.Linq;
using NUnit.Framework;
namespace Tecaverso.Labs.Probability.Tests
{
    public class ProbabilityTests
    {
        [Test] public void EmptySeriesHasUndefinedStatistics(){var s=new ProbabilitySeries(Experiment.Dice,2);Assert.IsTrue(double.IsNaN(s.Mean));Assert.IsTrue(double.IsNaN(s.Median));Assert.IsEmpty(s.Modes);}
        [TestCase(1)] [TestCase(2)] [TestCase(17)] [TestCase(100)]
        public void StatisticsMatchIndependentSortedSample(int count)
        {var s=new ProbabilitySeries(Experiment.Dice,123);var values=Enumerable.Range(0,count).Select(_=>s.Draw()+1).OrderBy(x=>x).ToArray();Assert.AreEqual(values.Average(),s.Mean,1e-12);Assert.AreEqual((values[(count-1)/2]+values[count/2])/2.0,s.Median);Assert.AreEqual(count,s.Counts.Sum());Assert.AreEqual(Math.Min(count,10),s.Recent.Count);var grouped=values.GroupBy(x=>x).ToArray();CollectionAssert.AreEqual(grouped.Where(g=>g.Count()==grouped.Max(x=>x.Count())).Select(g=>g.Key),s.Modes);}
        [Test] public void UrnWithoutReplacementExhaustsExactComposition()
        {var s=new ProbabilitySeries(Experiment.Urn,5);Assert.IsTrue(s.Configure(2,8,false));for(int i=0;i<10;i++){Assert.AreEqual((double)s.RemainingA/(s.RemainingA+s.RemainingB),s.Chance(0));Assert.GreaterOrEqual(s.Draw(),0);}CollectionAssert.AreEqual(new[]{2,8},s.Counts);Assert.AreEqual(-1,s.Draw());Assert.IsTrue(double.IsNaN(s.Chance(0)));Assert.AreEqual(10,s.Total);}
        [Test] public void WithReplacementKeepsPopulationAndLocksConfiguration()
        {var s=new ProbabilitySeries(Experiment.Urn,9);s.Configure(0,10,true);for(int i=0;i<100;i++)Assert.AreEqual(1,s.Draw());Assert.AreEqual(10,s.RemainingB);Assert.IsFalse(s.Configure(5,5,false));s.Reset();Assert.AreEqual(0,s.Total);Assert.IsTrue(s.Configure(3,2,false));}
        [Test] public void LimitsAndResetPreserveIntegrity()
        {var s=new ProbabilitySeries(Experiment.Coin,4);for(int i=0;i<10010;i++)s.Draw();Assert.AreEqual(10000,s.Total);Assert.AreEqual(10000,s.Counts.Sum());Assert.AreEqual(10,s.Recent.Count);s.Reset();Assert.AreEqual(0,s.Counts.Sum());Assert.IsEmpty(s.Recent);}
        [TestCase(0,0)] [TestCase(-1,5)] [TestCase(11,1)]
        public void InvalidUrnConfigurationDoesNotMutate(int a,int b){var s=new ProbabilitySeries(Experiment.Urn);Assert.IsFalse(s.Configure(a,b,true));Assert.AreEqual(5,s.RemainingA);Assert.AreEqual(5,s.RemainingB);}
    }
}
