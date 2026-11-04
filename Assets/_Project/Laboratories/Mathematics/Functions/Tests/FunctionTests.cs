using System;
using NUnit.Framework;
namespace Tecaverso.Labs.Functions.Tests
{
    public class FunctionTests
    {
        [TestCase(1,0,-1,2)] [TestCase(1,0,0,1)] [TestCase(1,0,1,0)]
        [TestCase(0,2,-4,1)] [TestCase(0,0,3,0)] [TestCase(.1,.2,.1,1)]
        public void RootsSatisfyOriginalEquation(double a,double b,double c,int count)
        {
            var p=new Polynomial(FunctionFamily.Quadratic,a,b,c);var roots=FunctionMath.Roots(p);
            Assert.AreEqual(count,roots.Length);foreach(var x in roots)Assert.That(p.Evaluate(x),Is.EqualTo(0).Within(1e-9));
        }
        [Test] public void ZeroAndNonzeroConstantsAreDistinct()
        {
            Assert.IsTrue(FunctionMath.InfiniteRoots(new Polynomial(FunctionFamily.Affine,0,0)));
            Assert.IsFalse(FunctionMath.InfiniteRoots(new Polynomial(FunctionFamily.Affine,0,1)));
            Assert.IsFalse(FunctionMath.Vertex(new Polynomial(FunctionFamily.Quadratic,0,2,3),out _));
        }
        [Test] public void VertexIsAtExtremum()
        {
            var p=new Polynomial(FunctionFamily.Quadratic,2,-4,-1);
            Assert.IsTrue(FunctionMath.Vertex(p,out var v));Assert.AreEqual(1,v.x);Assert.AreEqual(-3,v.y);
            Assert.Greater(p.Evaluate(.99),v.y);Assert.Greater(p.Evaluate(1.01),v.y);
        }
        [Test] public void ClippingSeparatesBranchesAndNeverIncludesOffscreenSamples()
        {
            var p=new Polynomial(FunctionFamily.Quadratic,1,0,-5);var intervals=FunctionMath.Intervals(p);
            Assert.AreEqual(2,intervals.Count);Assert.That(intervals[0].y,Is.EqualTo(-1).Within(1e-5));Assert.That(intervals[1].x,Is.EqualTo(1).Within(1e-5));
            foreach(var range in intervals)for(int i=1;i<100;i++)
            {double x=range.x+(range.y-range.x)*i/100;Assert.IsTrue(FunctionMath.Visible(x,p.Evaluate(x)));}
            Assert.IsTrue(FunctionMath.ClosestVisibleX(p,0,out var nearest));Assert.That(Math.Abs(nearest),Is.EqualTo(1).Within(1e-5));
        }
        [Test] public void OffscreenConstantDoesNotProduceFalseBorderCurve()
        {
            var p=new Polynomial(FunctionFamily.Affine,0,5);Assert.IsEmpty(FunctionMath.Intervals(p));Assert.IsFalse(FunctionMath.ClosestVisibleX(p,0,out _));
        }
        [Test] public void TangencyToWindowKeepsSingleVisiblePoint()
        {var p=new Polynomial(FunctionFamily.Quadratic,1,0,4);var ranges=FunctionMath.Intervals(p);Assert.AreEqual(1,ranges.Count);Assert.AreEqual(0,ranges[0].x);Assert.AreEqual(0,ranges[0].y);}
        [Test] public void ConstantOnBorderIsVisible()
        {var ranges=FunctionMath.Intervals(new Polynomial(FunctionFamily.Affine,0,4));Assert.AreEqual(1,ranges.Count);Assert.AreEqual(-6,ranges[0].x);Assert.AreEqual(6,ranges[0].y);}
        [Test] public void ClippedDragEndpointRemainsVisibleAfterFloatConversion()
        {
            var p=new Polynomial(FunctionFamily.Quadratic,.3,1.7,-2.9);
            foreach(var target in new[]{-6.0,6.0})
            {Assert.IsTrue(FunctionMath.ClosestVisibleX(p,target,out var x));Assert.IsTrue(FunctionMath.Visible(x,p.Evaluate(x)));}
        }
        [Test] public void FamilySwitchReferenceAndRestoreKeepIndependentValues()
        {
            var s=new FunctionSession();s.Set(0,2);s.Set(1,3);s.SaveReference();s.Select(FunctionFamily.Quadratic);s.Set(2,-1);
            Assert.AreEqual(2,s.Reference.Value.a);Assert.AreEqual(3,s.Reference.Value.b);
            s.Restore();Assert.AreEqual(0,s.Current.c);s.UndoRestore();Assert.AreEqual(-1,s.Current.c);
            s.Select(FunctionFamily.Affine);Assert.AreEqual(2,s.Current.a);Assert.AreEqual(3,s.Current.b);
            s.Set(0,-2);Assert.AreEqual(2,s.Reference.Value.a);
        }
        [Test] public void CoefficientsClampAndSnapToTenths()
        {var s=new FunctionSession();s.Set(0,10);Assert.AreEqual(3,s.Current.a);s.Set(1,-100);Assert.AreEqual(-5,s.Current.b);s.Set(0,.26);Assert.AreEqual(.3,s.Current.a);}
        [Test] public void MarkerKeepsItsCoordinateWhenFunctionLeavesViewport()
        {var s=new FunctionSession();s.StepMarker(5);s.Set(0,3);Assert.AreEqual(5,s.MarkerX);Assert.IsFalse(FunctionMath.Visible(s.MarkerX,s.Current.Evaluate(s.MarkerX)));s.StepMarker(100);Assert.AreEqual(6,s.MarkerX);}
    }
}
