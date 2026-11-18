using System;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Content.Interaction;
using Tecaverso.Interaction;
using Tecaverso.UI;

namespace Tecaverso.Labs.Biology.Anatomy
{
    public sealed class AnatomyLab : MonoBehaviour
    {
        public GameObject[] systems;
        public GameObject skinRoot,consultation;
        public AnatomyOrgan[] organs;
        public Transform bodyPivot;
        public XRKnob ring;
        public Button[] systemButtons;
        public Toggle skin;
        public Button process,back,close;
        public TMP_Text systemTitle,legend,status,queryTitle,queryBody,processLabel,selectedLabel;
        public LineRenderer leader;
        public AnatomyPath path;
        public int ActiveSystem {get;private set;}=-1;
        public AnatomyOrgan Selected {get;private set;}
        public AnatomyOrgan InspectionOrgan { get; private set; }
        UnityAction[] actions;
        Tween fade;
        float desiredYaw;
        bool leaving;
        const string PullHint = "Segure e mova para examinar. Solte para devolver.";
        const string FixedHint = "Estrutura fixa para consulta.";
        const string HeldHint = "Gire a peça na mão. Solte para devolver.";
        string SystemHint => organs.Any(o => o.system == ActiveSystem && o.IsLiftable)
            ? "Selecione para consultar. Segure e afaste uma peça destacável."
            : "Selecione uma estrutura para consultar sua função e localização.";
        static readonly Color Blue=new Color32(40,78,160,255),Paper=new Color32(211,209,232,255);
        void OnEnable()
        {
            actions=new UnityAction[systemButtons.Length];
            for(int i=0;i<actions.Length;i++){int index=i;actions[i]=()=>Choose(index);systemButtons[i].onClick.AddListener(actions[i]);}
            skin.onValueChanged.AddListener(ShowSkin);process.onClick.AddListener(TogglePath);back.onClick.AddListener(Back);close.onClick.AddListener(Close);
            ring.onValueChange.AddListener(Rotate);
        }
        void Start(){UIVisibility.Set(consultation,false);selectedLabel.gameObject.SetActive(false);leader.enabled=false;Choose(0);ShowSkin(skin.isOn);}
        void OnDisable()
        {
            fade?.Kill();
            for(int i=0;i<actions.Length;i++)systemButtons[i].onClick.RemoveListener(actions[i]);
            skin.onValueChanged.RemoveListener(ShowSkin);process.onClick.RemoveListener(TogglePath);back.onClick.RemoveListener(Back);close.onClick.RemoveListener(Close);ring.onValueChange.RemoveListener(Rotate);
            if(Selected)Selected.GetComponent<XRInteractionMotion>()?.SetPersistentSelected(false);
        }
        public void Choose(int index)
        {
            if(index<0||index>=systems.Length||index==ActiveSystem)return;
            Close();path.Choose(index);ActiveSystem=index;
            for(int i=0;i<systems.Length;i++)
            {
                systems[i].SetActive(i==index);
                systemButtons[i].targetGraphic.color=Color.white;
                var colors=systemButtons[i].colors;colors.normalColor=colors.selectedColor=i==index?Blue:Paper;colors.highlightedColor=i==index?new Color(.2f,.4f,.78f):new Color(.82f,.88f,1);systemButtons[i].colors=colors;
                systemButtons[i].GetComponentInChildren<TMP_Text>().color=i==index?Color.white:new Color32(7,56,111,255);
            }
            systemTitle.text="SISTEMA "+AnatomyDefinitions.Titles[index].ToUpperInvariant();
            UIVisibility.Set(process.gameObject,index<4,true);processLabel.text="Observar percurso";
            legend.text=path.Legend;
            status.text=SystemHint;
        }
        public void ShowSkin(bool show){skinRoot.SetActive(show);}
        public bool BeginInspection(AnatomyOrgan organ)
        {
            if (!organ || !organ.IsLiftable || organ.system != ActiveSystem || (InspectionOrgan && InspectionOrgan != organ)) return false;
            InspectionOrgan = organ;
            if (Selected != organ) Inspect(organ);
            path.SetInspectionSuspended(true);
            process.interactable = false;
            status.text = HeldHint;
            return true;
        }

        public void EndInspection(AnatomyOrgan organ)
        {
            if (InspectionOrgan != organ) return;
            InspectionOrgan = null;
            if (leaving) return;
            if (path) path.SetInspectionSuspended(false);
            if (process) process.interactable = true;
            if (status && isActiveAndEnabled && ActiveSystem == organ.system)
                status.text = path && path.Running ? "Peça recolocada. Percurso retomado." : "Peça recolocada no corpo.";
        }

        public void Inspect(AnatomyOrgan organ)
        {
            if(!organ||organ.system!=ActiveSystem||!organ.gameObject.activeInHierarchy||(InspectionOrgan&&InspectionOrgan!=organ))return;
            Close();Selected=organ;
            InteractionMotionBootstrap.Ensure(organ);organ.GetComponent<XRInteractionMotion>().SetPersistentSelected(true);
            var hint = organ.IsLiftable ? PullHint : FixedHint;
            queryTitle.text=organ.title;queryBody.text=organ.function+"\n\n"+organ.context+"\n\n<size=85%>Modelo anatômico didático. "+hint+"</size>";
            selectedLabel.text=organ.title;selectedLabel.gameObject.SetActive(true);leader.enabled=true;
            UIVisibility.Set(consultation,true);
            if(!MotionPreferences.Reduced){var group=consultation.GetComponent<CanvasGroup>();group.alpha=0;fade=group.DOFade(1,.14f).SetUpdate(true);}
            status.text=InspectionOrgan ? HeldHint : hint;
        }
        public void Close()
        {
            fade?.Kill();if(Selected)Selected.GetComponent<XRInteractionMotion>()?.SetPersistentSelected(false);Selected=null;
            UIVisibility.Set(consultation,false);selectedLabel.gameObject.SetActive(false);leader.enabled=false;
            if(ActiveSystem>=0)status.text=InspectionOrgan ? HeldHint : SystemHint;
        }
        public void TogglePath()
        {
            if(!process.interactable||!path.Available||path.InspectionSuspended)return;
            path.Toggle();
            processLabel.text=path.Running?"Pausar percurso":"Retomar percurso";
            legend.text=path.Legend;
            status.text=path.Running?"Percurso em movimento. Pause para observar.":"Percurso pausado. A etapa permanece visível.";
        }
        void Rotate(float value){desiredYaw=value*360;}
        void LateUpdate()
        {
            if(path.Visible && legend.text!=path.Legend)legend.text=path.Legend;
            var goal=Quaternion.Euler(0,desiredYaw,0);bodyPivot.localRotation=MotionPreferences.Reduced?goal:Quaternion.Slerp(bodyPivot.localRotation,goal,1-Mathf.Exp(-12*Time.unscaledDeltaTime));
            if(!Selected)return;
            var bounds=Selected.WorldBounds;var anchor=selectedLabel.transform.parent;
            anchor.position=new Vector3(-.78f,Mathf.Clamp(bounds.center.y,.62f,1.95f),bodyPivot.position.z-.3f);
            leader.SetPosition(0,anchor.position+Vector3.right*.23f);leader.SetPosition(1,bounds.center);
        }
        public void Back(){if(leaving)return;leaving=true;path.Choose(-1);SceneManager.LoadSceneAsync("Assets/_Project/Hub/Scenes/Hub.unity");}
    }
}
