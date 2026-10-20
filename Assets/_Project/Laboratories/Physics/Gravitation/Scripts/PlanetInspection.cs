using System;
using System.Globalization;
using Tecaverso.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Tecaverso.Labs.Gravitation
{
    public sealed class PlanetInspection : MonoBehaviour
    {
        // NASA Planetary Fact Sheet, mass in kg (rounded source values).
        static readonly double[] MassKg={.330e24,4.87e24,5.97e24,.642e24,1898e24,568e24,86.8e24,102e24};
        static readonly CultureInfo Pt=CultureInfo.GetCultureInfo("pt-BR");
        public SolarSystemView solar;
        public GravitationLab lab;
        public GameObject hoverCard, inspectionRoot;
        public TMP_Text hoverText, title, details;
        public Transform preview, previewRing;
        public Button[] planetButtons;
        public Button close, rotateLeft, rotateRight;
        UnityAction[] choices;
        int hovered=-1, selected=-1;
        float nextReadout;
        public int Selected=>selected;
        public int Hovered=>hovered;
        public bool IsOpen=>inspectionRoot!=null&&inspectionRoot.activeSelf;
        void Awake()
        {
            choices=new UnityAction[planetButtons.Length];
            for(int i=0;i<choices.Length;i++){int index=i;choices[i]=()=>Focus(index);planetButtons[i].onClick.AddListener(choices[i]);}
            close.onClick.AddListener(Close);rotateLeft.onClick.AddListener(RotateLeft);rotateRight.onClick.AddListener(RotateRight);
            Close();UIVisibility.Set(hoverCard,false);
        }
        void OnDestroy()
        {
            if(choices==null)return;
            for(int i=0;i<choices.Length;i++)planetButtons[i].onClick.RemoveListener(choices[i]);
            close.onClick.RemoveListener(Close);rotateLeft.onClick.RemoveListener(RotateLeft);rotateRight.onClick.RemoveListener(RotateRight);
        }
        void OnDisable(){Close();hovered=-1;if(hoverCard!=null)UIVisibility.Set(hoverCard,false);}
        public string Facts(int index)
        {
            var data=PlanetOrbit.Planets[index];
            double distance=data.PositionAt(lab.SolarDays,index*.79).Length;
            int exponent=(int)Math.Floor(Math.Log10(MassKg[index]));
            return "Diâmetro: "+SolarScale.DiameterKm[index].ToString("N0",Pt)+" km\n"+
                "Massa: "+(MassKg[index]/Math.Pow(10,exponent)).ToString("0.###",Pt)+" × 10<sup>"+exponent+"</sup> kg\n"+
                "Ao Sol agora: "+distance.ToString("0.00",Pt)+" UA\n"+
                "("+ (distance*149.5978707).ToString("N1",Pt)+" milhões de km)\n"+
                "Período orbital: "+data.PeriodDays.ToString("N1",Pt)+" dias";
        }
        bool Available(int i)=>i>=0&&i<solar.planets.Length&&lab.panel.Mode==GravityMode.SolarSystem&&!UIVisibility.IsVisible(lab.panel.pages[2]);
        public void ShowHover(int index)
        {
            if(!Available(index)||IsOpen)return;
            hovered=index;hoverText.text=PlanetOrbit.Planets[index].Name+"\n"+Facts(index)+"\nSELECIONE PARA VER DE PERTO";
            UIVisibility.Set(hoverCard,true);
            PlaceHover();
            var camera=Camera.main;
            if(camera!=null)hoverCard.transform.rotation=Quaternion.LookRotation(hoverCard.transform.position-camera.transform.position,Vector3.up);
        }
        public void HideHover(int index){if(hovered!=index)return;hovered=-1;UIVisibility.Set(hoverCard,false);}
        void PlaceHover()
        {
            var camera=Camera.main;
            var position=solar.planets[hovered].position;
            float factor=camera==null?1:Mathf.Clamp(Vector3.Distance(camera.transform.position,position)/3f,1,3);
            hoverCard.transform.localScale=Vector3.one*(.0015f*factor);
            hoverCard.transform.position=position+Vector3.up*(.4f+.24f*factor);
            if(camera!=null)hoverCard.transform.rotation=Quaternion.LookRotation(hoverCard.transform.position-camera.transform.position,Vector3.up);
        }
        public void Focus(int index)
        {
            if(!Available(index))return;
            var camera=Camera.main;if(camera==null)return;
            selected=index;hovered=-1;UIVisibility.Set(hoverCard,false);
            var forward=Vector3.ProjectOnPlane(camera.transform.forward,Vector3.up).normalized;
            if(forward.sqrMagnitude<.1f)forward=Vector3.forward;
            var at=camera.transform.position+forward*2.1f;
            at.x=Mathf.Clamp(at.x,-8f,8f);at.z=Mathf.Clamp(at.z,-10.8f,10.8f);
            if(Vector3.ProjectOnPlane(at-camera.transform.position,Vector3.up).magnitude<1.2f)
                at=camera.transform.position+Vector3.ProjectOnPlane(-camera.transform.position,Vector3.up).normalized*2.1f;
            at.y=Mathf.Max(.9f,at.y-.05f);
            var facing=Vector3.ProjectOnPlane(at-camera.transform.position,Vector3.up).normalized;
            inspectionRoot.transform.SetPositionAndRotation(at,Quaternion.LookRotation(facing));
            preview.GetComponent<Renderer>().sharedMaterial=solar.planets[index].GetComponent<Renderer>().sharedMaterial;
            preview.localRotation=Quaternion.identity;
            preview.localScale=Vector3.one*(index==5?.30f:.55f);
            previewRing.gameObject.SetActive(index==5);
            title.text=PlanetOrbit.Planets[index].Name+" · zoom individual";
            details.text=Facts(index)+"\n\nAmpliação independente da cena.\nNão compare tamanhos nesta vista.";
            inspectionRoot.SetActive(true);
            solar.SetInspectionVisible(true);
        }
        public void Close(){selected=-1;if(inspectionRoot!=null)inspectionRoot.SetActive(false);if(solar!=null)solar.SetInspectionVisible(false);}
        void RotateLeft()=>preview.Rotate(Vector3.up,-30,Space.Self);
        void RotateRight()=>preview.Rotate(Vector3.up,30,Space.Self);
        void LateUpdate()
        {
            if(UIVisibility.IsVisible(lab.panel.pages[2]))
            {Close();if(hovered>=0)HideHover(hovered);return;}
            if(hovered>=0)
            {
                if(!solar.planets[hovered].gameObject.activeInHierarchy)HideHover(hovered);
                else PlaceHover();
            }
            if(Time.unscaledTime<nextReadout)return;nextReadout=Time.unscaledTime+.2f;
            if(hovered>=0)hoverText.text=PlanetOrbit.Planets[hovered].Name+"\n"+Facts(hovered)+"\nSELECIONE PARA VER DE PERTO";
            if(selected>=0)details.text=Facts(selected)+"\n\nAmpliação independente da cena.\nNão compare tamanhos nesta vista.";
        }
    }
}
