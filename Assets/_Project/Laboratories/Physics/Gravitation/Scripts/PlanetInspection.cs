using System;
using System.Globalization;
using Tecaverso.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Tecaverso.Labs.Gravitation
{
    public sealed class PlanetInspection : MonoBehaviour
    {
        static readonly double[] MassKg={.330e24,4.87e24,5.97e24,.642e24,1898e24,568e24,86.8e24,102e24};
        static readonly CultureInfo Pt=CultureInfo.GetCultureInfo("pt-BR");
        public SolarSystemView solar;
        public GravitationLab lab;
        public GameObject hoverCard,inspectionRoot;
        public TMP_Text hoverText,title,details;
        // Legacy authoring references; retired objects stay inactive after migration.
        public Transform preview,previewRing;
        public Button[] planetButtons;
        public Button close,rotateLeft,rotateRight;
        public Vector2 roomMin=new Vector2(-14.5f,-14.5f),roomMax=new Vector2(14.5f,22.5f);
        int hovered=-1,selected=-1;
        public int Selected=>selected;
        public int Hovered=>hovered;
        public bool IsOpen=>inspectionRoot!=null&&inspectionRoot.activeSelf;
        void Awake(){close.onClick.AddListener(Close);Close();if(hoverCard!=null)UIVisibility.Set(hoverCard,false);}
        void OnDestroy(){if(close!=null)close.onClick.RemoveListener(Close);}
        void OnDisable(){Close();hovered=-1;if(solar!=null)solar.Highlight(-1,-1);}
        public string Facts(int index)
        {
            var data=PlanetOrbit.Planets[index];
            int exponent=(int)Math.Floor(Math.Log10(MassKg[index]));
            return "Diâmetro: "+SolarScale.DiameterKm[index].ToString("N0",Pt)+" km\n"+
                "Massa: "+(MassKg[index]/Math.Pow(10,exponent)).ToString("0.###",Pt)+" × 10<sup>"+exponent+"</sup> kg\n"+
                "Distância média ao Sol: "+data.SemiMajorAU.ToString("0.00",Pt)+" UA\n"+
                "("+ (data.SemiMajorAU*149.5978707).ToString("N1",Pt)+" milhões de km)\n"+
                "Período orbital: "+data.PeriodDays.ToString("N1",Pt)+" dias";
        }
        bool Available(int i)=>i>=0&&i<solar.planets.Length&&lab.panel.Mode==GravityMode.SolarSystem&&!UIVisibility.IsVisible(lab.panel.pages[2]);
        public void ShowHover(int index){if(!Available(index))return;hovered=index;solar.Highlight(hovered,selected);}
        public void HideHover(int index){if(hovered!=index)return;hovered=-1;solar.Highlight(hovered,selected);}
        public void Focus(int index)
        {
            if(!Available(index))return;
            if(selected==index){Close();return;}
            var camera=Camera.main;if(camera==null)return;
            if(lab.SolarRunning&&!lab.SolarPaused)lab.Pause();
            selected=index;solar.Highlight(hovered,selected);
            var body=solar.planets[index];
            var facing=Vector3.ProjectOnPlane(body.position-camera.transform.position,Vector3.up).normalized;
            if(facing.sqrMagnitude<.01f)facing=Vector3.forward;
            var right=Vector3.Cross(Vector3.up,facing);
            float factor=Mathf.Clamp(Vector3.Distance(camera.transform.position,body.position)/3,1,1.5f);
            var card=title.GetComponentInParent<Canvas>(true).transform;
            card.localScale=Vector3.one*(.0015f*factor);
            float offset=body.lossyScale.x*.5f+.3f+.675f*factor;
            if(index==5)offset+=.6f; // Keep Saturn's rings unobscured.
            var at=body.position+right*offset;
            if(at.x<roomMin.x||at.x>roomMax.x||at.z<roomMin.y||at.z>roomMax.y)at=body.position-right*offset;
            at.x=Mathf.Clamp(at.x,roomMin.x,roomMax.x);at.z=Mathf.Clamp(at.z,roomMin.y,roomMax.y);
            inspectionRoot.transform.SetPositionAndRotation(at,Quaternion.LookRotation(facing));

            title.text=PlanetOrbit.Planets[index].Name;
            details.text=Facts(index)+"\n\nValores reais · cenário didático\nÓrbitas pausadas para leitura.";
            inspectionRoot.SetActive(true);
        }
        public void Close(){selected=-1;if(inspectionRoot!=null)inspectionRoot.SetActive(false);if(solar!=null)solar.Highlight(hovered,-1);}
        void LateUpdate()
        {
            if(UIVisibility.IsVisible(lab.panel.pages[2])){Close();hovered=-1;solar.Highlight(-1,-1);return;}
            if(selected>=0&&Camera.main!=null)
            {
                var d=Vector3.ProjectOnPlane(inspectionRoot.transform.position-Camera.main.transform.position,Vector3.up);
                if(d.sqrMagnitude>.01f)inspectionRoot.transform.rotation=Quaternion.LookRotation(d);
            }
        }
    }
}
