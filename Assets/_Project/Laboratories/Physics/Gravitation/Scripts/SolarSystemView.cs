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
        public TMP_Text rockyLabel, rulerLabel;
        public TMP_Text[] rulerTicks;
        public LineRenderer[] selectionMarkers;
        public bool RealScale { get; private set; }
        public bool InnerSystem { get; private set; }
        bool geometryReady;
        bool inspecting, pathsVisible=true;
        public void SetInspectionVisible(bool visible)
        {
            inspecting=visible;
            for(int i=0;i<planets.Length;i++)
            {
                foreach(var renderer in planets[i].GetComponentsInChildren<Renderer>(true))renderer.enabled=!visible;
                var collider=planets[i].GetComponent<Collider>();if(collider!=null)collider.enabled=!visible;
                labels[i].enabled=!visible;
                orbits[i].enabled=!visible&&pathsVisible&&(!InnerSystem||i<4);
                if(selectionMarkers!=null&&i<selectionMarkers.Length)selectionMarkers[i].enabled=!visible&&(!InnerSystem||i<4);
            }
            if(sun!=null)sun.GetComponent<Renderer>().enabled=!visible;
            if(rockyLabel!=null)rockyLabel.enabled=!visible;
        }
        // A single linear scale applies to every position within either view.
        public static float MetresPerAU(bool inner)=>(float)SolarScale.MetresPerAU(inner);
        public static float DisplaySemiMajor(int index)=>(float)PlanetOrbit.Planets[index].SemiMajorAU*MetresPerAU(false);
        public void SetView(bool inner,bool realScale=false)
        {
            if(geometryReady&&InnerSystem==inner&&RealScale==realScale)return;
            InnerSystem=inner;RealScale=realScale;geometryReady=true;
            float scale=MetresPerAU(inner);
            if(sun!=null)sun.localScale=Vector3.one*(float)SolarScale.BodyDiameter(SolarScale.SunDiameterKm,inner,realScale);
            for(int i=0;i<planets.Length;i++)
            {
                float size=(float)SolarScale.BodyDiameter(SolarScale.DiameterKm[i],inner,realScale);
                planets[i].localScale=Vector3.one*size;
                // Selection assistance is distinct from the body's rendered physical diameter.
                var target=planets[i].GetComponent<SphereCollider>();
                if(target!=null)target.radius=Mathf.Max(.5f,.04f/size);
                planets[i].gameObject.SetActive(!inner||i<4);
                labels[i].gameObject.SetActive(inner?i<4:i>=4);
                labels[i].fontSize=inner?1.35f:1;
                labels[i].rectTransform.sizeDelta=new Vector2(1.5f,.3f);
                orbits[i].positionCount=240;
                orbits[i].startWidth=orbits[i].endWidth=!inner&&i<4?.002f:.008f;
                for(int j=0;j<240;j++)
                {
                    var p=PlanetOrbit.Planets[i].EllipseAt(j*2*System.Math.PI/240);
                    orbits[i].SetPosition(j,new Vector3((float)p.X,0,(float)p.Y)*scale);
                }
            }
            if(rockyLabel!=null)
            {
                rockyLabel.gameObject.SetActive(true);
                rockyLabel.text="SOL · Ø 1.391.400 km";
                rockyLabel.transform.localPosition=Vector3.up*(sun.localScale.x*.5f+.25f);
            }
            if(rulerLabel!=null)rulerLabel.text=inner?"DISTÂNCIAS · 1 m = 0,25 UA":"DISTÂNCIAS · 1 m = 5 UA";
            if(rulerTicks!=null)for(int i=0;i<rulerTicks.Length;i++)
                rulerTicks[i].text=(i/scale).ToString("0.##",System.Globalization.CultureInfo.GetCultureInfo("pt-BR"))+" UA";
        }
        public void Show(double days,bool showPaths)
        {
            pathsVisible=showPaths;
            if(!geometryReady)SetView(InnerSystem,RealScale);
            for(int i=0;i<planets.Length;i++)
            {
                var data=PlanetOrbit.Planets[i];
                var position=data.PositionAt(days,i*.79);
                planets[i].localPosition=new Vector3((float)position.X,0,(float)position.Y)*MetresPerAU(InnerSystem);
                if(selectionMarkers!=null&&i<selectionMarkers.Length)
                {
                    selectionMarkers[i].transform.localPosition=planets[i].localPosition;
                    selectionMarkers[i].enabled=!inspecting&&(!InnerSystem||i<4);
                }
                labels[i].transform.localPosition=planets[i].localPosition+new Vector3(0,.23f,0);
                orbits[i].enabled=!inspecting&&showPaths&&(!InnerSystem||i<4);
            }
        }
        void LateUpdate()
        {
            var camera=Camera.main;if(camera==null)return;
            // Face the student's location, not head rotation; labels stay stable when standing still.
            foreach(var label in labels)
            {
                if(!label.gameObject.activeInHierarchy)continue;
                var direction=Vector3.ProjectOnPlane(label.transform.position-camera.transform.position,Vector3.up);
                if(direction.sqrMagnitude>.01f)label.transform.rotation=Quaternion.LookRotation(direction);
            }
            if(selectionMarkers!=null)foreach(var marker in selectionMarkers)
            {
                var direction=marker.transform.position-camera.transform.position;
                if(direction.sqrMagnitude>.01f)marker.transform.rotation=Quaternion.LookRotation(direction);
            }
        }
    }
}
