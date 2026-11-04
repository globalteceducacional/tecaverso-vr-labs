using NUnit.Framework;
namespace Tecaverso.Labs.Fractions.Tests
{
    public class FractionTests
    {
        [Test] public void DifferentDenominatorsHaveExactEqualTotals(){var s=new FractionSession();s.Add(0,3);s.Add(0,6);s.Add(1,2);Assert.AreEqual(s.Total(0),s.Total(1));Assert.AreEqual("1/2",s.TotalText(0,false));}
        [Test] public void ImproperFractionUsesOneWholeAsPercentReference(){var s=new FractionSession();for(int i=0;i<3;i++)s.Add(0,2);Assert.AreEqual("3/2 = 1 + 1/2\n= 1,5 · = 150%",s.TotalText(0,true));}
        [TestCase(1)] [TestCase(2)] [TestCase(3)] [TestCase(4)] [TestCase(5)] [TestCase(6)] [TestCase(8)] [TestCase(10)] [TestCase(12)]
        public void CapacityIsExact(int d){var s=new FractionSession();for(int i=0;i<2*d;i++)Assert.IsTrue(s.Add(0,d));Assert.IsFalse(s.Add(0,12));Assert.AreEqual(240,s.Total(0));}
        [Test] public void RemoveMiddleAndUndoPreserveOrder(){var s=new FractionSession();s.Add(0,3);s.Add(0,6);s.Add(0,2);s.Remove(0,1);CollectionAssert.AreEqual(new[]{3,2},s.Row(0));s.Undo();CollectionAssert.AreEqual(new[]{3,6,2},s.Row(0));}
        [Test] public void UndoIsBoundedAndDoesNotAlterRecipe(){var s=new FractionSession();for(int i=0;i<12;i++)s.Add(0,12);s.AddToken(0);for(int i=0;i<10;i++)Assert.IsTrue(s.Undo());Assert.IsFalse(s.Undo());Assert.AreEqual(2,s.Row(0).Count);Assert.AreEqual(1,s.Recipe[0]);}
        [Test] public void ReferenceDoesNotAutofillAndRatioIsNotTarget(){var s=new FractionSession();s.Multiplier=2;Assert.AreEqual(0,s.Recipe[0]);s.Multiplier=1;for(int i=0;i<4;i++)s.AddToken(0);for(int i=0;i<6;i++)s.AddToken(1);Assert.IsTrue(s.SameRatio);Assert.IsFalse(s.TargetReached);StringAssert.Contains("2/5 = 40%",s.RecipeText());s.Multiplier=2;Assert.IsTrue(s.TargetReached);}
        [Test] public void EmptySingleCategoryAndLimitsAreDefined(){var s=new FractionSession();StringAssert.Contains("Participações: —",s.RecipeText());for(int i=0;i<10;i++)Assert.IsTrue(s.AddToken(0));Assert.IsFalse(s.AddToken(0));StringAssert.Contains("Receita incompleta",s.RecipeText());StringAssert.Contains("100%",s.RecipeText());Assert.IsFalse(s.RemoveToken(1));}
        [Test] public void RoundingIsMarkedOnlyWhenNeeded(){Assert.AreEqual("≈ 0,333",FractionSession.Rounded(1,3,3));Assert.AreEqual("= 0,125",FractionSession.Rounded(1,8,3));Assert.AreEqual("≈ 16,7",FractionSession.Rounded(1,6,1,100));}
        [Test] public void ClearKeepsRecipeAndCanUndo(){var s=new FractionSession();s.Add(0,4);s.Add(1,2);s.AddToken(1);s.ClearRows();Assert.AreEqual(0,s.Total(0));Assert.AreEqual(1,s.Recipe[1]);s.Undo();Assert.AreEqual(30,s.Total(0));Assert.AreEqual(60,s.Total(1));}
    }
}
