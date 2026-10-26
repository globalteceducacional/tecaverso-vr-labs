using System;

namespace Tecaverso.Labs.Waves
{
    public enum WaveEnd { Fixed, Free, Absorbing }
    /// <summary>Centred finite differences for y_tt + 2 gamma y_t = c² y_xx.
    /// Fixed 2 ms step, Courant number 0.24. kinematic source or interior force.
    /// First order Mur outgoing boundary is used for the absorbing termination.</summary>
    public sealed class StringWaveModel
    {
        public const int Count=241;
        public const double Length=6, Speed=3, Dt=.002, Dx=Length/(Count-1);
        public const double Damping=.10;
        double[] y=new double[Count], previous=new double[Count], next=new double[Count];
        double accumulator;
        public double Time {get; private set;}
        public double this[int i]=>y[i];
        public WaveEnd End {get;set;}=WaveEnd.Fixed;
        public double Sample(double x)
        {double p=Math.Clamp(x/Dx,0,Count-1);int i=Math.Min(Count-2,(int)p);return y[i]+(y[i+1]-y[i])*(p-i);}
        public void Reset()
        {Array.Clear(y,0,Count);Array.Clear(previous,0,Count);Array.Clear(next,0,Count);Time=accumulator=0;}
        public void Advance(double elapsed,Func<double,double> source,bool interior=false)
        {
            if(double.IsNaN(elapsed)||double.IsInfinity(elapsed)||elapsed<0) return;
            accumulator+=elapsed;
            while(accumulator+1e-12>=Dt){Step(source(Time+Dt),interior);accumulator-=Dt;}
        }
        public void Step(double source,bool interior=false)
        {
            if(double.IsNaN(source)||double.IsInfinity(source))source=0;
            source=Math.Clamp(source,-.35,.35);
            double r=Speed*Dt/Dx,g=Damping*Dt;
            for(int i=1;i<Count-1;i++)
            {
                double force=interior&&i==12?source*480:0;
                next[i]=(2*y[i]-(1-g)*previous[i]+r*r*(y[i-1]-2*y[i]+y[i+1])+force*Dt*Dt)/(1+g);
            }
            next[0]=interior?0:source;
            if(End==WaveEnd.Fixed)next[Count-1]=0;
            else if(End==WaveEnd.Free)
                next[Count-1]=(2*y[Count-1]-(1-g)*previous[Count-1]+2*r*r*(y[Count-2]-y[Count-1]))/(1+g);
            else next[Count-1]=y[Count-2]+(r-1)/(r+1)*(next[Count-2]-y[Count-1]);
            var spare=previous;previous=y;y=next;next=spare;Time+=Dt;
        }
    }
}
