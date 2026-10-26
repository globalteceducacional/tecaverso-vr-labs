using System;
using NUnit.Framework;
using Tecaverso.Labs.Waves;

public class StringWaveTests
{
    static double Pulse(double t)=>t<.4?.2*Math.Pow(Math.Sin(Math.PI*t/.4),2):0;
    [TestCase(WaveEnd.Fixed,-1)]
    [TestCase(WaveEnd.Free,1)]
    public void PulseTravelsAndReflectsWithCorrectSign(WaveEnd end,int sign)
    {
        var m=new StringWaveModel{End=end};double incident=0,reflected=0,arrival=0;
        for(int i=0;i<2000;i++)
        {
            m.Advance(StringWaveModel.Dt,Pulse);double y=m.Sample(3);
            if(m.Time<2&&y>incident){incident=y;arrival=m.Time;}
            if(m.Time>2.5&&Math.Abs(y)>Math.Abs(reflected))reflected=y;
        }
        Assert.That(arrival,Is.EqualTo(1.2).Within(.025));
        Assert.That(incident,Is.GreaterThan(.15));Assert.That(reflected*sign,Is.GreaterThan(.1));
    }
    [Test]
    public void AbsorbingEndSuppressesReturn()
    {
        var m=new StringWaveModel{End=WaveEnd.Absorbing};double reflection=0;
        for(int i=0;i<2000;i++){m.Advance(StringWaveModel.Dt,Pulse);if(m.Time>2.5)reflection=Math.Max(reflection,Math.Abs(m.Sample(3)));}
        Assert.That(reflection,Is.LessThan(.015));
    }
    [Test]
    public void FramePartitionDoesNotChangeSolution()
    {
        var a=new StringWaveModel();var b=new StringWaveModel();Func<double,double> drive=t=>.1*Math.Sin(Math.PI*t);
        for(int i=0;i<180;i++)a.Advance(1.0/30,drive);
        for(int i=0;i<540;i++)b.Advance(1.0/90,drive);
        for(int i=0;i<StringWaveModel.Count;i++)Assert.That(a[i],Is.EqualTo(b[i]).Within(1e-10));
    }
    [TestCase(.25,1)]
    [TestCase(.5,2)]
    [TestCase(.75,3)]
    public void InteriorDriverProducesResonantNodes(double frequency,int harmonic)
    {
        var m=new StringWaveModel();var max=new double[StringWaveModel.Count];
        for(int j=0;j<30000;j++)
        {
            m.Step(.08*Math.Sin(2*Math.PI*frequency*(m.Time+StringWaveModel.Dt)),true);
            if(j>28000)for(int i=0;i<max.Length;i++)max[i]=Math.Max(max[i],Math.Abs(m[i]));
        }
        double belly=max[(int)Math.Round(120.0/harmonic)];
        Assert.That(belly,Is.GreaterThan(.08).And.LessThan(1));
        for(int n=1;n<harmonic;n++)Assert.That(max[n*240/harmonic],Is.LessThan(belly*.15));
        Assert.That(m[0],Is.Zero);Assert.That(m[240],Is.Zero);
    }
    [Test]
    public void WavelengthMatchesSpeedOverFrequencyAndAmplitudeIsLinear()
    {
        var a=new StringWaveModel{End=WaveEnd.Absorbing};var b=new StringWaveModel{End=WaveEnd.Absorbing};
        for(int i=0;i<10000;i++){double drive=.1*Math.Sin(2*Math.PI*.75*(a.Time+StringWaveModel.Dt));a.Step(drive);b.Step(2*drive);}
        var zeros=new System.Collections.Generic.List<double>();
        for(int i=1;i<240;i++)
        {
            Assert.That(b[i],Is.EqualTo(2*a[i]).Within(1e-8));
            if(a[i]*a[i+1]<0)zeros.Add((i+Math.Abs(a[i])/(Math.Abs(a[i])+Math.Abs(a[i+1])))*StringWaveModel.Dx);
        }
        Assert.That(zeros.Count,Is.GreaterThanOrEqualTo(2));
        Assert.That(2*(zeros[1]-zeros[0]),Is.EqualTo(StringWaveModel.Speed/.75).Within(.06));
    }
}
