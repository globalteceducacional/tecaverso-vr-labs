using TMPro;
using UnityEngine;

namespace Tecaverso.Labs.ObliqueLaunch
{
    public sealed class HeightTimeGraph : MonoBehaviour
    {
        [SerializeField] LineRenderer curve, progress;
        [SerializeField] TMP_Text title;
        float maxTime = 1f, maxHeight = 1f;
        float nextPlotTime;
        public void Bind(LineRenderer curveLine, LineRenderer progressLine, TMP_Text titleText)
        { curve = curveLine; progress = progressLine; title = titleText; }

        public void Begin(in LaunchParameters parameters)
        {
            curve.positionCount = 0; nextPlotTime = 0f;
            var v = parameters.InitialVelocity;
            maxTime = Mathf.Max(.5f, (v.y + Mathf.Sqrt(v.y * v.y + 2f * parameters.Gravity * parameters.Height)) / parameters.Gravity);
            maxHeight = Mathf.Max(1f, parameters.Height + v.y * v.y / (2f * parameters.Gravity));
            title.text = "POSIÇÃO Y × TEMPO";
        }

        public void Plot(in FlightSample sample)
        {
            Vector3 p = new(Mathf.Clamp01(sample.Time / maxTime) * 2.6f, Mathf.Clamp01(sample.Position.y / maxHeight) * 1.25f, -.01f);
            if (sample.Time >= nextPlotTime || sample.Position.y <= 0f)
            {
                int index = curve.positionCount;
                curve.positionCount = index + 1;
                curve.SetPosition(index, p);
                nextPlotTime += .025f;
            }
            progress.SetPosition(0, new Vector3(p.x, 0f, -.015f)); progress.SetPosition(1, new Vector3(p.x, 1.25f, -.015f));
        }

        public void Clear() { curve.positionCount = 0; nextPlotTime = 0f; progress.SetPosition(0, Vector3.zero); progress.SetPosition(1, Vector3.zero); }
    }
}
