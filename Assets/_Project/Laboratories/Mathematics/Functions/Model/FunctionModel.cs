using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tecaverso.Labs.Functions
{
    public enum FunctionFamily { Affine, Quadratic }
    [Serializable]
    public struct Polynomial : IEquatable<Polynomial>
    {
        public FunctionFamily family;
        public double a, b, c;
        public Polynomial(FunctionFamily family, double a = 1, double b = 0, double c = 0)
        { this.family = family; this.a = a; this.b = b; this.c = c; }
        public double A => family == FunctionFamily.Quadratic ? a : 0;
        public double B => family == FunctionFamily.Quadratic ? b : a;
        public double C => family == FunctionFamily.Quadratic ? c : b;
        public double Evaluate(double x) => (A * x + B) * x + C;
        public bool Equals(Polynomial other) => family == other.family && a == other.a && b == other.b && c == other.c;
        public Polynomial With(int index, double value)
        {
            var next = this;
            value = Math.Round(Math.Clamp(value, index == 0 ? -3 : -5, index == 0 ? 3 : 5) * 10, MidpointRounding.AwayFromZero) / 10;
            if(index == 0) next.a = value; else if(index == 1) next.b = value; else next.c = value;
            return next;
        }
        public double Coefficient(int i) => i == 0 ? a : i == 1 ? b : c;
    }

    public static class FunctionMath
    {
        public const double XMin = -6, XMax = 6, YMin = -4, YMax = 4;
        public const float WorldScale = .5f;
        // The accepted coefficients are multiples of 0.1. Tolerance only absorbs arithmetic noise.
        const double Epsilon = 1e-10;
        public static bool InfiniteRoots(Polynomial p) => p.A == 0 && p.B == 0 && p.C == 0;
        public static double[] Roots(Polynomial p, double level = 0)
        {
            double a=p.A, b=p.B, c=p.C-level;
            if(a == 0) return b == 0 ? Array.Empty<double>() : new[] { -c/b };
            double d=b*b-4*a*c;
            if(d < -Epsilon) return Array.Empty<double>();
            if(Math.Abs(d) <= Epsilon) return new[] { -b/(2*a) };
            // Stable quadratic formula, including b = 0.
            double q=-.5*(b+(b>=0?1:-1)*Math.Sqrt(d));
            double r1=q/a, r2=c/q;
            return r1<r2 ? new[]{r1,r2} : new[]{r2,r1};
        }
        public static bool Vertex(Polynomial p, out Vector2 value)
        {
            value=default;
            if(p.A == 0) return false;
            double x=-p.B/(2*p.A); value=new Vector2((float)x,(float)p.Evaluate(x)); return true;
        }
        // Rendering/XR coordinates are floats; absorb their round-off at exact clipping endpoints.
        public static bool Visible(double x, double y) => x >= XMin-1e-5 && x <= XMax+1e-5 && y >= YMin-1e-5 && y <= YMax+1e-5;

        // Split at exact intersections with the viewport. Never connect disjoint branches along the border.
        public static List<Vector2> Intervals(Polynomial p)
        {
            var cuts=new List<double>{XMin,XMax};
            foreach(double level in new[]{YMin,YMax}) foreach(double root in Roots(p,level))
                if(root>XMin && root<XMax) cuts.Add(root);
            cuts.Sort(); var result=new List<Vector2>();
            for(int i=0;i<cuts.Count-1;i++)
            {
                double l=cuts[i],r=cuts[i+1];
                if(r-l>Epsilon && Visible((l+r)/2,p.Evaluate((l+r)/2))) result.Add(new Vector2((float)l,(float)r));
            }
            // A tangent can touch the window at one point without any visible interval.
            foreach(double x in cuts)
                if(Visible(x,p.Evaluate(x))&&!result.Exists(r=>x>=r.x-1e-5&&x<=r.y+1e-5))result.Add(new Vector2((float)x,(float)x));
            result.Sort((left,right)=>left.x.CompareTo(right.x));
            return result;
        }
        public static bool ClosestVisibleX(Polynomial p, double target, out double x)
        {
            x=target; double distance=double.PositiveInfinity;
            foreach(var interval in Intervals(p))
            { double candidate=Math.Clamp(target,interval.x,interval.y), d=Math.Abs(target-candidate); if(d<distance){distance=d;x=candidate;} }
            return !double.IsPositiveInfinity(distance);
        }
    }

    /// <summary>State shared by all input methods, with immutable reference snapshots and per-family values.</summary>
    public sealed class FunctionSession
    {
        readonly Polynomial[] functions={new Polynomial(FunctionFamily.Affine),new Polynomial(FunctionFamily.Quadratic)};
        Polynomial? undo;
        public FunctionFamily Family { get; private set; }
        public Polynomial Current => functions[(int)Family];
        public Polynomial? Reference { get; private set; }
        public double MarkerX { get; private set; }
        public bool CanUndo => undo.HasValue && undo.Value.family == Family;
        public bool HasWork => Reference.HasValue || MarkerX != 0 || !functions[0].Equals(new Polynomial(FunctionFamily.Affine)) || !functions[1].Equals(new Polynomial(FunctionFamily.Quadratic));
        public void Select(FunctionFamily family) { Family=family; }
        public void Set(int i,double value) { functions[(int)Family]=Current.With(i,value); undo=null; }
        public void StepMarker(double step) => MarkerX=Math.Round(Math.Clamp(MarkerX+step,FunctionMath.XMin,FunctionMath.XMax)*10)/10;
        public void DragMarker(double x) { if(FunctionMath.ClosestVisibleX(Current,x,out var nearest)) MarkerX=nearest; }
        public void SaveReference() => Reference=Current;
        public void RemoveReference() => Reference=null;
        public void Restore() { undo=Current; functions[(int)Family]=new Polynomial(Family); }
        public void UndoRestore() { if(!CanUndo)return; functions[(int)Family]=undo.Value;undo=null; }
    }
}
