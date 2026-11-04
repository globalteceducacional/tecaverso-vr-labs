using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Tecaverso.UI;
namespace Tecaverso.Labs.Probability
{
    public sealed class ProbabilityLab:MonoBehaviour
    {
        public ProbabilityStation[] stations;
        public Button back,confirm,cancel;
        public GameObject modal;
        public TMP_Text message;
        Action pending;
        public bool Modal { get; private set; }
        void OnEnable(){back.onClick.AddListener(Back);confirm.onClick.AddListener(Confirm);cancel.onClick.AddListener(Cancel);Cancel();}
        void OnDisable(){back.onClick.RemoveListener(Back);confirm.onClick.RemoveListener(Confirm);cancel.onClick.RemoveListener(Cancel);}
        public void Activate(ProbabilityStation station){foreach(var s in stations)if(s!=station)s.Suspend();}
        public void Ask(string text,Action action,bool needed=true,Transform anchor=null){if(Modal)return;if(!needed){action();return;}pending=action;message.text=text;Modal=true;if(anchor){modal.transform.position=anchor.position-anchor.forward*.1f;modal.transform.rotation=anchor.rotation;}UIVisibility.Set(modal,true);back.interactable=false;foreach(var s in stations){s.Suspend();UIVisibility.SetInteraction(s.controls,false);s.lever.enabled=false;}}
        void Confirm(){var action=pending;Cancel();action?.Invoke();}
        void Cancel(){pending=null;Modal=false;UIVisibility.Set(modal,false);back.interactable=true;foreach(var s in stations){UIVisibility.SetInteraction(s.controls,true);s.lever.enabled=true;}}
        void Back(){Ask("Voltar ao Hub? Os resultados desta visita não serão salvos.",()=>SceneManager.LoadSceneAsync("Assets/_Project/Hub/Scenes/Hub.unity"),Array.Exists(stations,s=>s.Series!=null&&s.Series.Total>0),back.transform.parent);}
    }
}
