using System;
using System.Linq;
using NUnit.Framework;

namespace Tecaverso.Labs.Chemistry.Reactions.Tests
{
    public class ReactionWorkbenchTests
    {
        [Test] public void AllPairsAreSymmetricAndOnlyTwelveAreAvailable()
        {
            var all=ReactionWorkbench.Reagents;Assert.That(all.Length,Is.EqualTo(12));
            Assert.That(all.GroupBy(r=>r.Group).All(g=>g.Count()==3),Is.True);int pairs=0;
            for(int i=0;i<all.Length;i++)
            {
                Assert.That(ReactionWorkbench.Recipe(all[i].Id,all[i].Id),Is.Null);
                Assert.That(all.Any(b=>ReactionWorkbench.Recipe(all[i].Id,b.Id)!=null),Is.True);
                for(int j=i+1;j<all.Length;j++)
                {
                    var forward=ReactionWorkbench.Recipe(all[i].Id,all[j].Id);
                    Assert.That(ReactionWorkbench.Recipe(all[j].Id,all[i].Id),Is.SameAs(forward));
                    var m=new ReactionWorkbench();m.SelectA(all[i].Id);
                    Assert.That(m.SelectB(all[j].Id),Is.EqualTo(forward!=null));if(forward!=null)pairs++;
                }
            }
            Assert.That(pairs,Is.EqualTo(12));
        }
        [TestCase(0)][TestCase(1)][TestCase(2)][TestCase(3)][TestCase(4)][TestCase(5)]
        [TestCase(6)][TestCase(7)][TestCase(8)][TestCase(9)][TestCase(10)][TestCase(11)]
        public void EachRecipeConservesAtomsAndStockInBothOrders(int recipe)
        {
            var d=ReactionDefinition.All[recipe];Assert.That(d.IsBalanced(d.Stoichiometry),Is.True);
            var first=ReactionWorkbench.Reagents.Single(r=>r.Formula==d.Species[0].Formula).Id;
            var second=ReactionWorkbench.Reagents.Single(r=>r.Formula==d.Species[1].Formula).Id;
            foreach(bool reverse in new[]{false,true})
            {
                var m=new ReactionWorkbench();m.SelectA(reverse?second:first);Assert.That(m.SelectB(reverse?first:second),Is.True);
                // Add the second chemical before the first, including solid-first trials.
                if(d.PowderSecond){m.LoadSpoon(second);m.PourPowder(second,.5,true);}else m.PourLiquid(second,120,true);
                Assert.That(m.Session.ExtentMol,Is.Zero);
                m.PourLiquid(first,100,true);m.PourLiquid(first,20,false);
                Assert.That(m.Session.ExtentMol,Is.GreaterThan(0));
                foreach(var e in d.Elements)Assert.That(m.Session.AtomConservationError(e),Is.EqualTo(0).Within(1e-10));
                Assert.That(m.Stock(first),Is.EqualTo(180).Within(1e-8));
                if(d.PowderSecond)Assert.That(m.Stock(second)+m.Spoon(second)+m.Session.AddedMol[1]*d.SolidMolarMass+m.Session.SpilledPowder,Is.EqualTo(6).Within(1e-9));
            }
        }
        [Test] public void ForeignReagentCannotEnterModelOrConsumeStock()
        {
            var m=new ReactionWorkbench();Assert.That(m.PourLiquid("hcl",100,true).Removed,Is.Zero);
            Assert.That(m.SelectB("naoh"),Is.False);m.SelectA("cacl");m.SelectB("carb");m.PourLiquid("cacl",80,true);
            Assert.That(m.PourLiquid("hcl",100,true).Removed,Is.Zero);Assert.That(m.LoadSpoon("mgo"),Is.Zero);
            Assert.That(m.PourPowder("mgo",1,true).Removed,Is.Zero);Assert.That(m.Stock("hcl"),Is.EqualTo(300));
            Assert.That(m.Session.VolumeMl,Is.EqualTo(80));Assert.That(m.SelectA("hcl"),Is.False);Assert.That(m.SelectB("hcl"),Is.False);
        }
        [Test] public void RemovingDraftAAndResettingUsedPairHaveDifferentEffects()
        {
            var m=new ReactionWorkbench();m.SelectA("hcl");m.RemoveA();Assert.That(m.A,Is.Null);
            m.SelectA("hcl");m.SelectB("naoh");m.RemoveA();Assert.That(m.A,Is.EqualTo("hcl"));
            m.PourLiquid("naoh",100,true);Assert.That(m.AddIndicator(),Is.True);Assert.That(m.Used,Is.True);
            m.Reset();Assert.That(m.Ready,Is.False);Assert.That(m.Used,Is.False);Assert.That(m.Stock("naoh"),Is.EqualTo(300));
        }
        [Test] public void IndicatorAndPHAreRestrictedToTheValidatedReferencePair()
        {
            for(int i=0;i<12;i++)
            {
                var s=new ReactionSession(ReactionDefinition.All[i]);s.PourLiquid(0,30,true);
                Assert.That(s.AddIndicator(),Is.EqualTo(i==2));Assert.That(double.IsNaN(s.PH),Is.EqualTo(i!=2));
            }
        }
        [Test] public void LiquidCarbonateGasReactionMustNotConsumePowder()
        {
            var m=new ReactionWorkbench();m.SelectA("hcl");m.SelectB("carb");
            Assert.That(m.LoadSpoon("carb"),Is.Zero);m.PourLiquid("hcl",100,true);m.PourLiquid("carb",100,true);
            Assert.That(m.Session.ExtentMol,Is.EqualTo(.005).Within(1e-10));Assert.That(m.Session.RemainingMol[1],Is.EqualTo(.005).Within(1e-10));
        }
    }
}
