using TMPro;
using UnityEngine;

namespace Tecaverso.Labs.ObliqueLaunch
{
    public sealed class MeasurementRulers : MonoBehaviour
    {
        [SerializeField] LineRenderer rangeLine, heightLine;
        [SerializeField] TMP_Text rangeLabel, heightLabel;
        Vector3 launchOrigin;
        [SerializeField] LineRenderer baseHeightLine, angleArc, angleBaseline;
        [SerializeField] TMP_Text baseHeightLabel, angleLabel;

        public void ShowLaunchGeometry(Vector3 origin, float angle, float height)
        {
            if(baseHeightLine==null)
            {
                baseHeightLine=Instantiate(heightLine,transform); baseHeightLine.name="Platform Height Ruler";
                angleArc=Instantiate(rangeLine,transform); angleArc.name="Launch Angle Arc";
                angleBaseline=Instantiate(rangeLine,transform); angleBaseline.name="Angle Horizontal Reference";
                baseHeightLabel=Instantiate(heightLabel,transform); baseHeightLabel.name="Platform Height Label";
                angleLabel=Instantiate(rangeLabel,transform); angleLabel.name="Launch Angle Label";
                baseHeightLabel.fontSize=angleLabel.fontSize=2.5f;
                baseHeightLabel.rectTransform.sizeDelta=angleLabel.rectTransform.sizeDelta=new Vector2(2f,.5f);
            }
            // Keep measurements coplanar with the cannon to avoid parallax offsets.
            var center=origin;
            var bottom=new Vector3(center.x-1f,0f,center.z);
            baseHeightLine.useWorldSpace=true;
            baseHeightLine.positionCount=2;
            baseHeightLine.SetPosition(0,bottom); baseHeightLine.SetPosition(1,bottom+Vector3.up*height);
            baseHeightLabel.text=$"{height:0.00} m";
            baseHeightLabel.transform.position=bottom+new Vector3(-.6f,Mathf.Max(.2f,height*.5f),0f);
            const float radius=1.25f;
            angleBaseline.useWorldSpace=true; angleBaseline.positionCount=2;
            angleBaseline.SetPosition(0,center); angleBaseline.SetPosition(1,center+Vector3.right*1.7f);
            angleArc.useWorldSpace=true; angleArc.positionCount=33;
            for(int i=0;i<33;i++)
            {
                float a=angle*Mathf.Deg2Rad*i/32f;
                angleArc.SetPosition(i,center+new Vector3(Mathf.Cos(a),Mathf.Sin(a),0f)*radius);
            }
            angleLabel.text=$"{angle:0}°";
            float middle=angle*Mathf.Deg2Rad*.5f;
            angleLabel.transform.position=center+new Vector3(Mathf.Cos(middle),Mathf.Sin(middle),0f)*1.8f;
            // Text faces the same lateral observer as the experiment, without rotating the UI.
            baseHeightLabel.transform.rotation=angleLabel.transform.rotation=Quaternion.identity;
        }
        public void Bind(LineRenderer range, LineRenderer height, TMP_Text rangeText, TMP_Text heightText)
        { rangeLine = range; heightLine = height; rangeLabel = rangeText; heightLabel = heightText; }
        public void Begin(Vector3 origin) => launchOrigin = origin;
        public void UpdateMeasurements(in FlightSample sample)
        {
            float range = sample.Position.x - launchOrigin.x;
            rangeLine.SetPosition(0, new Vector3(launchOrigin.x, 0f, launchOrigin.z)); rangeLine.SetPosition(1, new Vector3(sample.Position.x, 0f, launchOrigin.z));
            heightLine.SetPosition(0, new Vector3(launchOrigin.x, 0f, launchOrigin.z)); heightLine.SetPosition(1, new Vector3(launchOrigin.x, sample.MaximumHeight, launchOrigin.z));
            rangeLabel.transform.position = new Vector3(launchOrigin.x + range * .5f, .12f, launchOrigin.z);
            heightLabel.transform.position = new Vector3(launchOrigin.x+.15f, sample.MaximumHeight * .5f, launchOrigin.z);
            rangeLabel.text = $"Rtotal: {range:0.00} m";
            heightLabel.text = $"Ymax: {sample.MaximumHeight:0.00} m";
        }
        public void Clear()
        {
            launchOrigin=Vector3.zero; rangeLine.SetPosition(0, Vector3.zero); rangeLine.SetPosition(1, Vector3.zero);
            heightLine.SetPosition(0, Vector3.zero); heightLine.SetPosition(1, Vector3.zero);
            rangeLabel.text = "Rtotal: 0.00 m"; heightLabel.text = "Ymax: 0.00 m";
        }
    }
}
