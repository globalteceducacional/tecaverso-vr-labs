using NUnit.Framework;
using Tecaverso.Labs.Thermology;

public class IdealGasTests
{
    [Test] public void IsothermalExpansionHalvesPressureAndCompressionReversesWork()
    {
        var gas=new IdealGasModel();gas.SetVolume(40);
        Assert.That(gas.Temperature,Is.EqualTo(300));
        Assert.That(gas.Pressure*gas.Volume,Is.EqualTo(gas.InitialPressure*gas.InitialVolume).Within(1e-7));
        Assert.That(gas.Work,Is.GreaterThan(0));
        gas.SetVolume(12.5);Assert.That(gas.Pressure,Is.EqualTo(gas.InitialPressure*2).Within(1e-7));Assert.That(gas.Work,Is.LessThan(0));
        gas.SetVolume(25);Assert.That(gas.Work,Is.EqualTo(0).Within(1e-8));
    }
    [TestCase(90)] [TestCase(110)] [TestCase(130)]
    public void IsobaricHeatingKeepsPressureAndWorkUsesLitresTimesKpa(double pressure)
    {
        var gas=new IdealGasModel();gas.Reset(GasProcess.Isobaric,pressure);gas.SetTemperature(450);
        Assert.That(gas.Pressure,Is.EqualTo(pressure).Within(1e-7));
        Assert.That(gas.Volume/gas.InitialVolume,Is.EqualTo(1.5).Within(1e-7));
        Assert.That(gas.Work,Is.EqualTo(IdealGasModel.R*150).Within(1e-7));
    }
    [Test] public void IsochoricHeatingRaisesPressureWithoutMechanicalWork()
    {
        var gas=new IdealGasModel();gas.Reset(GasProcess.Isochoric);gas.SetTemperature(450);
        Assert.That(gas.Volume,Is.EqualTo(25));Assert.That(gas.Pressure/gas.InitialPressure,Is.EqualTo(1.5).Within(1e-7));Assert.That(gas.Work,Is.Zero);
        Assert.That(gas.SetVolume(30),Is.False);
    }
    [Test] public void InvalidInputCannotBreakTheThermodynamicState()
    {
        var gas=new IdealGasModel();Assert.That(gas.SetVolume(double.NaN),Is.False);gas.SetVolume(-3);Assert.That(gas.Volume,Is.EqualTo(12));
        Assert.That(gas.SetTemperature(400),Is.False);gas.Reset(GasProcess.Isochoric);Assert.That(gas.SetTemperature(double.PositiveInfinity),Is.False);gas.SetTemperature(0);Assert.That(gas.Temperature,Is.EqualTo(250));
        Assert.Throws<System.ArgumentOutOfRangeException>(()=>gas.Reset(GasProcess.Isobaric,0));
    }
}
