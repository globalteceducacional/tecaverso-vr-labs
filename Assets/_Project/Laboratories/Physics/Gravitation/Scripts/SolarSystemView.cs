using TMPro;
using UnityEngine;
namespace Tecaverso.Labs.Gravitation
{
    public sealed class SolarSystemView : MonoBehaviour
    {
        public Transform[] planets;
        public LineRenderer[] orbits;
        public TMP_Text[] labels;
        public Transform sun;
        public TMP_Text rockyLabel,rulerLabel;
        public TMP_Text[] rulerTicks;
        public LineRenderer[] selectionMarkers;
        public bool RealScale=>false;
        public bool InnerSystem=>false;
        bool geometryReady;
        int hovered=-1,selected=-1;
        public static float MetresPerAU(bool inner)=>(float)SolarScale.MetresPerAU(inner);
        public static float DisplaySemiMajor(int index)=>(float)ObservatoryScale.OrbitRadius[index];
        // Kept for the original installer; selection no longer hides the exhibit.
        public void SetInspectionVisible(bool visible) { }
        public void Highlight(int hover,int selection){hovered=hover;selected=selection;}
        public void SetView(bool inner,bool realScale=false)
        {
            if(geometryReady)return;geometryReady=true;
            sun.localScale=Vector3.one*(float)ObservatoryScale.SunDiameter;
            for(int i=0;i<planets.Length;i++)
            {
                planets[i].localScale=Vector3.one*(float)ObservatoryScale.Diameter(i);
                planets[i].gameObject.SetActive(true);
                var c=planets[i].GetComponent<SphereCollider>();if(c!=null){c.radius=.55f;c.enabled=true;}
                labels[i].gameObject.SetActive(true);labels[i].enabled=true;labels[i].fontSize=1.5f;
                labels[i].rectTransform.sizeDelta=new Vector2(2.8f,.5f);
                orbits[i].loop=true;orbits[i].positionCount=240;orbits[i].startWidth=orbits[i].endWidth=.008f;
                for(int j=0;j<240;j++)
                {float a=j*2*Mathf.PI/240;orbits[i].SetPosition(j,new Vector3(Mathf.Cos(a)*DisplaySemiMajor(i),-1.65f,Mathf.Sin(a)*DisplaySemiMajor(i)));}
                if(selectionMarkers!=null&&i<selectionMarkers.Length)selectionMarkers[i].enabled=false;
            }
            if(rockyLabel!=null){rockyLabel.text="Sol";rockyLabel.transform.localPosition=Vector3.up*1.8f;}
        }
        public void Show(double days,bool showPaths)
        {
            SetView(false);
            for(int i=0;i<planets.Length;i++)
            {
                var p=PlanetOrbit.Planets[i].PositionAt(days,i*.79);
                planets[i].localPosition=new Vector3((float)p.X,0,(float)p.Y).normalized*DisplaySemiMajor(i);
                labels[i].transform.localPosition=planets[i].localPosition+Vector3.up*(planets[i].localScale.x*.5f+.24f);
                orbits[i].enabled=true;
            }
        }
        void Update()
        {
            // Illustrative axial spin, independent of the orbital calendar.
            for(int i=0;i<planets.Length;i++)if(i!=selected)planets[i].Rotate(Vector3.up,8*Time.deltaTime,Space.Self);
        }
        void LateUpdate()
        {
            var camera=Camera.main;if(camera==null)return;
            for(int i=0;i<labels.Length;i++)
            {
                labels[i].color=i==hovered||i==selected?Color.white:new Color32(190,218,255,255);
                string value=i==selected||i==hovered?"<u>"+PlanetOrbit.Planets[i].Name+"</u>":PlanetOrbit.Planets[i].Name;
                if(labels[i].text!=value)labels[i].text=value;
                Face(labels[i].transform,camera.transform.position);
            }
            if(rockyLabel!=null)Face(rockyLabel.transform,camera.transform.position);
        }
        static void Face(Transform t,Vector3 viewer)
        {var d=Vector3.ProjectOnPlane(t.position-viewer,Vector3.up);if(d.sqrMagnitude>.001f)t.rotation=Quaternion.LookRotation(d);}
    }
}
