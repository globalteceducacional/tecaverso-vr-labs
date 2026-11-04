using System;
using System.Collections.Generic;
using System.Linq;
namespace Tecaverso.Labs.Probability
{
    public enum Experiment { Coin, Dice, Urn }
    public sealed class ProbabilitySeries
    {
        public readonly Experiment Kind;
        public readonly int[] Counts;
        public int InitialA { get; private set; }=5;
        public int InitialB { get; private set; }=5;
        public int RemainingA { get; private set; }=5;
        public int RemainingB { get; private set; }=5;
        public bool Replacement { get; private set; }=true;
        public int Total { get; private set; }
        public readonly Queue<int> Recent=new();
        readonly Random random;
        public ProbabilitySeries(Experiment kind,int? seed=null){Kind=kind;Counts=new int[kind==Experiment.Dice?6:2];random=seed.HasValue?new Random(seed.Value):new Random();}
        public bool CanDraw=>Total<10000&&(Kind!=Experiment.Urn||RemainingA+RemainingB>0);
        public bool Configure(int a,int b,bool replacement)
        {
            if(Total!=0||a<0||a>10||b<0||b>10||a+b==0)return false;
            InitialA=RemainingA=a;InitialB=RemainingB=b;Replacement=replacement;return true;
        }
        public double Chance(int category)=>Kind==Experiment.Urn?(RemainingA+RemainingB==0?double.NaN:(double)(category==0?RemainingA:RemainingB)/(RemainingA+RemainingB)):1.0/Counts.Length;
        public int Draw()
        {
            if(!CanDraw)return -1;
            int result=Kind==Experiment.Urn?(random.Next(RemainingA+RemainingB)<RemainingA?0:1):random.Next(Counts.Length);
            Counts[result]++;Total++;Recent.Enqueue(result);if(Recent.Count>10)Recent.Dequeue();
            if(Kind==Experiment.Urn&&!Replacement){if(result==0)RemainingA--;else RemainingB--;}
            return result;
        }
        public void Reset(){Array.Clear(Counts,0,Counts.Length);Recent.Clear();Total=0;RemainingA=InitialA;RemainingB=InitialB;}
        public double Mean=>Total==0?double.NaN:Counts.Select((n,i)=>(double)n*(i+1)).Sum()/Total;
        public double Median
        {
            get{if(Total==0)return double.NaN;int lower=(Total-1)/2,upper=Total/2,cumulative=0;double a=0,b=0;for(int i=0;i<Counts.Length;i++){int next=cumulative+Counts[i];if(cumulative<=lower&&next>lower)a=i+1;if(cumulative<=upper&&next>upper)b=i+1;cumulative=next;}return(a+b)/2;}
        }
        public int[] Modes=>Total==0?Array.Empty<int>():Counts.Select((n,i)=>(n,i)).Where(x=>x.n==Counts.Max()).Select(x=>x.i+1).ToArray();
    }
}
