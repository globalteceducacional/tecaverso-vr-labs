using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Tecaverso.Labs.Chemistry.Reactions
{
    /// <summary>Physical probe; no chemical transfer. Unknown thermal data is not a zero temperature change.</summary>
    public sealed class ReactionThermometer : XRGrabInteractable
    {
        public ReactionsLab lab;
        public Transform tip,home;
        public TMP_Text display;
        public bool Held=>isSelected;
        public bool Immersed
        {
            get
            {
                if(!lab||lab.Session==null||lab.Session.VolumeMl<=0||!tip)return false;
                var p=tip.position;var delta=p-lab.opening.position;delta.y=0;
                return delta.sqrMagnitude<Mathf.Pow(lab.openingRadius-.025f,2)&&p.y>lab.receiver.transform.position.y-.31f&&p.y<lab.receiver.SurfaceY-.003f;
            }
        }
        float abandoned,refresh;
        public override bool IsSelectableBy(IXRSelectInteractor interactor)=>lab&&!lab.Modal&&base.IsSelectableBy(interactor);
        void Update()
        {
            if(Held||Immersed)abandoned=0;
            else if(!lab.Modal){abandoned+=Time.deltaTime;if(abandoned>10||transform.position.y<.15f)ReturnHome();}
            refresh+=Time.unscaledDeltaTime;if(refresh>=.1f){refresh=0;RefreshDisplay();}
        }
        public void RefreshDisplay()
        {
            if(!display||!lab)return;
            string text;
            if(!Immersed)text="TERMÔMETRO\nMergulhe a ponta\nno líquido\nEstimativa ideal";
            else if(!lab.Session.HasThermalModel)text="SONDA IMERSA\nModelo térmico\nindisponível\npara este par";
            else text=$"{lab.Session.TemperatureC:0.00} °C\nInicial: {ReactionSession.InitialTemperatureC:0.00} °C\nVariação: {lab.Session.TemperatureChangeC:+0.00;-0.00;0.00} °C\nEstimativa ideal";
            if(display.text!=text)display.text=text;
        }
        public void ReturnHome()
        {
            if(isSelected||!home)return;
            transform.SetPositionAndRotation(home.position,home.rotation);
            if(TryGetComponent<Rigidbody>(out var rb)){rb.position=home.position;rb.rotation=home.rotation;rb.linearVelocity=Vector3.zero;rb.angularVelocity=Vector3.zero;}
            abandoned=0;RefreshDisplay();
        }
    }
}
