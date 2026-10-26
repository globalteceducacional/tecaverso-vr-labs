using System;

namespace Tecaverso.Labs.Thermology
{
    public enum GasProcess { Isothermal, Isobaric, Isochoric }

    /// <summary>One mole, quasistatic ideal-gas processes. kPa * L = J; all temperature calculations use K.</summary>
    public sealed class IdealGasModel
    {
        public const double R = 8.31446261815324;
        public const double MinVolume = 12, MaxVolume = 45, MinTemperature = 250, MaxTemperature = 450;
        public GasProcess Process { get; private set; }
        public double Temperature { get; private set; }
        public double Volume { get; private set; }
        public double Pressure => R * Temperature / Volume;
        public double InitialVolume { get; private set; }
        public double InitialTemperature { get; private set; }
        public double InitialPressure => R * InitialTemperature / InitialVolume;
        public double Work => Process == GasProcess.Isochoric ? 0 : Process == GasProcess.Isothermal
            ? R * InitialTemperature * Math.Log(Volume / InitialVolume)
            : InitialPressure * (Volume - InitialVolume);

        public IdealGasModel() { Reset(GasProcess.Isothermal); }
        public void Reset(GasProcess process, double pressure = 100)
        {
            if (!Finite(pressure) || pressure < 90 || pressure > 130) throw new ArgumentOutOfRangeException(nameof(pressure));
            Process = process;
            InitialTemperature = Temperature = 300;
            InitialVolume = Volume = process == GasProcess.Isobaric ? R * Temperature / pressure : 25;
        }
        public bool SetVolume(double volume)
        {
            if (Process != GasProcess.Isothermal || !Finite(volume)) return false;
            double next = Clamp(volume, MinVolume, MaxVolume);
            if (Math.Abs(next - Volume) < 1e-8) return false;
            Volume = next; return true;
        }
        public bool SetTemperature(double temperature)
        {
            if (Process == GasProcess.Isothermal || !Finite(temperature)) return false;
            double low = MinTemperature, high = MaxTemperature;
            if (Process == GasProcess.Isobaric)
            {
                low = Math.Max(low, InitialPressure * MinVolume / R);
                high = Math.Min(high, InitialPressure * MaxVolume / R);
            }
            double next = Clamp(temperature, low, high);
            if (Math.Abs(next - Temperature) < 1e-8) return false;
            Temperature = next;
            if (Process == GasProcess.Isobaric) Volume = R * Temperature / InitialPressure;
            return true;
        }
        public double PressureAtVolume(double volume) => Process == GasProcess.Isothermal ? R * InitialTemperature / volume : InitialPressure;
        static bool Finite(double x) => !double.IsNaN(x) && !double.IsInfinity(x);
        static double Clamp(double x, double lo, double hi) => Math.Max(lo, Math.Min(hi, x));
    }
}
