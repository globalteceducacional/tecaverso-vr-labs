using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Tecaverso.Interaction;
using Tecaverso.UI;
using UnityEngine.UI;
using DG.Tweening;
using Tecaverso.Labs.Chemistry.PeriodicBonds;

namespace Tecaverso.Labs.Chemistry.AtomicModels
{
    public sealed class AtomicModelsLab : MonoBehaviour
    {
        public int protons, neutrons, electrons;
        public TMP_Text result, warning, confirmation, viewStatus;
        public GameObject confirmationRoot, consultationRoot;
        public TMP_Text consultationTitle, consultationText;
        public Transform atomPivot, electronRoot, cloudRoot;
        public GameObject[] protonVisuals, neutronVisuals, electronVisuals, orbitVisuals, cloudVisuals;
        public bool cloud;
        public AtomicRotateInteractable rotation;
        public RawImage consultationImage;
        public bool IsConfirming { get; private set; }
        Tween panelTween;
        float warningUntil;
        static readonly int[] Caps = { 2, 8, 8, 2 };
        static readonly float[] Tilts = { 0, 28, -31, 52 };
        void Start() { Refresh(); CloseConsultation(); CancelClear(); }
        void Update()
        {
            if (warning && warning.gameObject.activeSelf && Time.unscaledTime > warningUntil) warning.gameObject.SetActive(false);
            if (electronVisuals == null || cloud) return;
            int used=0;
            for(int shell=0;shell<4;shell++)
            {
                int count=Mathf.Min(Mathf.Max(electrons-used,0),Caps[shell]);
                float radius=.52f+shell*.29f;
                Quaternion tilt=Quaternion.Euler(Tilts[shell],shell*23f,12f);
                for(int j=0;j<count;j++)
                {
                    float angle=(j/(float)count)*Mathf.PI*2f+Time.time*(.9f-shell*.12f);
                    electronVisuals[used+j].transform.localPosition=tilt*new Vector3(Mathf.Cos(angle)*radius,0,Mathf.Sin(angle)*radius);
                }
                used+=count;
            }
        }
        public void Change(int kind,int delta)
        {
            if(IsConfirming || kind<0 || kind>2 || delta==0)return;
            delta=Math.Sign(delta);
            int current=kind==0?protons:kind==1?neutrons:electrons;
            int limit=kind==1?24:20;
            int next=Mathf.Clamp(current+delta,0,limit);
            if(next==current){Warn(delta>0?"Limite máximo desta oficina.":"A contagem já está em zero.");return;}
            if(kind==0)protons=next;else if(kind==1)neutrons=next;else electrons=next;
            Refresh();
        }
        void Warn(string text){if(!warning)return;warning.text=text;warning.gameObject.SetActive(true);warningUntil=Time.unscaledTime+2.8f;}
        public void Refresh()
        {
            protons=Mathf.Clamp(protons,0,20);neutrons=Mathf.Clamp(neutrons,0,24);electrons=Mathf.Clamp(electrons,0,20);
            if(result)
            {
                string identity=protons==0?"Nenhum elemento":PeriodicElementCatalog.All[protons-1].name+" · "+PeriodicElementCatalog.All[protons-1].symbol+"-"+(protons+neutrons);
                string charge=protons==0?"sem núcleo atômico":protons==electrons?"neutro":protons>electrons?"cátion +"+(protons-electrons):"ânion −"+(electrons-protons);
                result.text=identity+"\nZ "+protons+"   A "+(protons+neutrons)+"   "+charge+"\np<sup>+</sup> "+protons+"    n<sup>0</sup> "+neutrons+"    e<sup>−</sup> "+electrons+"\n<size=65%>Modelo didático · estabilidade não avaliada</size>";
            }
            if(viewStatus)viewStatus.text=cloud?"NUVEM · qualitativa":"CAMADAS · órbitas didáticas";
            SetCount(protonVisuals,protons);SetCount(neutronVisuals,neutrons);SetCount(electronVisuals,cloud?0:electrons);
            // Pack visible nucleons around the fixed origin, independent of catalogue indices.
            int total=protons+neutrons;
            for(int i=0;i<total;i++)
            {
                var particle=i<protons?protonVisuals[i]:neutronVisuals[i-protons];
                float y=1-2*(i+.5f)/Mathf.Max(total,1),a=i*2.399963f;
                float r=total==1?0:.09f+.035f*Mathf.Pow(total,1f/3f);
                particle.transform.localPosition=new Vector3(Mathf.Cos(a)*Mathf.Sqrt(1-y*y),y,Mathf.Sin(a)*Mathf.Sqrt(1-y*y))*r;
            }
            SetCount(cloudVisuals,cloud?electrons:0);
            int used=0;
            for(int i=0;i<4;i++){int count=Mathf.Min(Mathf.Max(electrons-used,0),Caps[i]);if(orbitVisuals!=null && i<orbitVisuals.Length && orbitVisuals[i])orbitVisuals[i].SetActive(!cloud && count>0);used+=count;}
        }
        static void SetCount(GameObject[] objects,int count){if(objects==null)return;for(int i=0;i<objects.Length;i++)if(objects[i])objects[i].SetActive(i<count);}
        public void ToggleView(){SetView(!cloud);}
        public void SetView(bool value){if(IsConfirming)return;cloud=value;Refresh();}
        public void Recenter(){if(!IsConfirming && rotation)rotation.Recenter();}
        void BlockWorld(bool block)
        {
            foreach(var xr in GetComponentsInChildren<XRBaseInteractable>(true))xr.enabled=!block;
            if(rotation)rotation.enabled=!block;
        }
        public void RequestClear(){if(protons+neutrons+electrons==0)return;CloseConsultation();IsConfirming=true;BlockWorld(true);if(result)result.gameObject.SetActive(false);UIVisibility.Set(confirmationRoot,true);if(warning)warning.gameObject.SetActive(false);if(confirmation)confirmation.text="Limpar a montagem atual?\n<size=70%>Prótons, nêutrons e elétrons voltarão a zero.</size>";}
        public void CancelClear(){IsConfirming=false;UIVisibility.Set(confirmationRoot,false);if(result)result.gameObject.SetActive(true);BlockWorld(false);}
        public void ConfirmClear(){protons=neutrons=electrons=0;CancelClear();Refresh();}
        public void OpenConsultation(string title,string body,Transform exhibit,Texture image=null)
        {
            if(!consultationRoot || IsConfirming)return;
            if(exhibit)
            {
                consultationRoot.transform.position=exhibit.position+Vector3.up*.10f-exhibit.forward*1.14f;
                consultationRoot.transform.rotation=exhibit.rotation;
            }
            panelTween?.Kill();UIVisibility.Set(consultationRoot,true);consultationTitle.text=title.Replace("⁺","<sup>+</sup>").Replace("⁻","<sup>−</sup>").Replace("⁰","<sup>0</sup>");consultationText.text=body;
            if(consultationImage)consultationImage.texture=image;
            if(!MotionPreferences.Reduced){var group=consultationRoot.GetComponent<CanvasGroup>();group.alpha=0;panelTween=group.DOFade(1,.16f).SetUpdate(true);}
        }
        public void CloseConsultation(){panelTween?.Kill();UIVisibility.Set(consultationRoot,false);}
        void OnDisable(){panelTween?.Kill();}
        public void Back(){if(!IsConfirming)SceneManager.LoadSceneAsync("Assets/_Project/Hub/Scenes/Hub.unity");}
    }

}

