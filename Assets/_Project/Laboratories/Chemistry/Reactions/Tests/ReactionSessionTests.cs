using NUnit.Framework;
namespace Tecaverso.Labs.Chemistry.Reactions.Tests
{
    public class ReactionSessionTests
    {
        [TestCase(ReactionKind.Gas)][TestCase(ReactionKind.Precipitation)][TestCase(ReactionKind.Neutralization)]
        public void ReferenceEquationsConserveEveryElement(ReactionKind kind){var d=ReactionDefinition.All[(int)kind];Assert.That(d.IsBalanced(d.Stoichiometry),Is.True);}
        [Test] public void IncorrectPrecipitationEquationIsRejected(){Assert.That(ReactionDefinition.All[1].IsBalanced(new[]{1,1,1,1}),Is.False);}
        [Test] public void SpillDoesNotCreateReaction(){var s=new ReactionSession(ReactionKind.Precipitation);s.PourLiquid(0,100,false);s.PourLiquid(1,100,true);Assert.That(s.ExtentMol,Is.Zero);Assert.That(s.SpilledMl,Is.EqualTo(100));Assert.That(s.SourceMl[0],Is.EqualTo(200));}
        [Test] public void OverflowIsAccountedAsIncomingSpill(){var s=new ReactionSession(ReactionKind.Precipitation);s.PourLiquid(0,300,true);var t=s.PourLiquid(1,300,true);Assert.That(s.VolumeMl,Is.EqualTo(500));Assert.That(t.Spilled,Is.EqualTo(100));Assert.That(s.ExtentMol,Is.EqualTo(.02).Within(1e-12));}
        [Test] public void PausedTransferCannotConsumeStock(){var s=new ReactionSession();s.LoadSpoon();s.Paused=true;s.PourLiquid(0,100,true);s.PourPowder(1,true);Assert.That(s.SourceMl[0],Is.EqualTo(300));Assert.That(s.SpoonGrams,Is.EqualTo(1));}
        [Test] public void GasAndExcessRemainInAtomBalance(){var s=new ReactionSession();s.PourLiquid(0,100,true);s.LoadSpoon();s.PourPowder(1,true);Assert.That(s.ProductsMol[2],Is.EqualTo(.01).Within(1e-12));Assert.That(s.RemainingMol[1],Is.GreaterThan(0));foreach(var e in s.Definition.Elements)Assert.That(s.AtomConservationError(e),Is.EqualTo(0).Within(1e-12));}
        [Test] public void NoReactionWithoutSecondReagent(){var s=new ReactionSession();s.PourLiquid(0,300,true);Assert.That(s.ExtentMol,Is.Zero);}
        [Test] public void SpoonReservoirIsFinite(){var s=new ReactionSession();for(int i=0;i<10;i++){s.LoadSpoon();s.PourPowder(1,false);}Assert.That(s.SpilledPowder,Is.EqualTo(6));Assert.That(s.SpoonGrams,Is.Zero);Assert.That(s.PowderStock,Is.Zero);}
        [Test] public void IdealNeutralizationCrossesEquivalence(){var s=new ReactionSession(ReactionKind.Neutralization);s.PourLiquid(1,100,true);Assert.That(s.PH,Is.EqualTo(13).Within(.001));s.PourLiquid(0,100,true);Assert.That(s.PH,Is.EqualTo(7).Within(1e-6));s.PourLiquid(0,100,true);Assert.That(s.PH,Is.LessThan(2));}
        [Test] public void ResetRestoresInventories(){var s=new ReactionSession();s.LoadSpoon();s.PourPowder(1,true);s.Reset(ReactionKind.Precipitation);Assert.That(s.Used,Is.False);Assert.That(s.SpoonGrams,Is.Zero);Assert.That(s.SourceMl[1],Is.EqualTo(300));Assert.That(s.ExtentMol,Is.Zero);}
        [Test] public void DoseCannotBeNegativeOrNonFinite(){var s=new ReactionSession();Assert.Throws<System.ArgumentOutOfRangeException>(()=>s.PourLiquid(0,double.NaN,true));Assert.Throws<System.ArgumentOutOfRangeException>(()=>s.PourPowder(-1,true));}
        [Test] public void IncrementalDoseMatchesSingleDose(){var a=new ReactionSession(ReactionKind.Precipitation);var b=new ReactionSession(ReactionKind.Precipitation);a.PourLiquid(0,160,true);a.PourLiquid(1,230,true);for(int i=0;i<160;i++)b.PourLiquid(0,1,true);for(int i=0;i<230;i++)b.PourLiquid(1,1,true);Assert.That(b.ExtentMol,Is.EqualTo(a.ExtentMol).Within(1e-12));}
    }
}
