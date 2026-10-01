using TMPro;
using UnityEngine;

namespace Tecaverso.Labs.ObliqueLaunch
{
    public sealed class MeasurementRulers : MonoBehaviour
    {
        [SerializeField] LineRenderer rangeLine, heightLine;
        [SerializeField] TMP_Text rangeLabel, heightLabel;
        public void Bind(LineRenderer range, LineRenderer height, TMP_Text rangeText, TMP_Text heightText)
        { rangeLine = range; heightLine = height; rangeLabel = rangeText; heightLabel = heightText; }
        public void UpdateMeasurements(in FlightSample sample)
        {
            rangeLine.SetPosition(0, Vector3.zero); rangeLine.SetPosition(1, new Vector3(sample.Position.x, 0f, 0f));
            heightLine.SetPosition(0, Vector3.zero); heightLine.SetPosition(1, new Vector3(0f, sample.MaximumHeight, 0f));
            rangeLabel.transform.position = new Vector3(sample.Position.x * .5f, .12f, 0f);
            heightLabel.transform.position = new Vector3(.15f, sample.MaximumHeight * .5f, 0f);
            rangeLabel.text = $"Rtotal: {sample.Position.x:0.00} m";
            heightLabel.text = $"Ymax: {sample.MaximumHeight:0.00} m";
        }
        public void Clear()
        {
            rangeLine.SetPosition(0, Vector3.zero); rangeLine.SetPosition(1, Vector3.zero);
            heightLine.SetPosition(0, Vector3.zero); heightLine.SetPosition(1, Vector3.zero);
            rangeLabel.text = "Rtotal: 0.00 m"; heightLabel.text = "Ymax: 0.00 m";
        }
    }
}
