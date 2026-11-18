using DG.Tweening;
using Tecaverso.Interaction;
using Tecaverso.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Tecaverso.Labs.Biology.Ecosystems
{
    public sealed class EcosystemsLab : MonoBehaviour
    {
        public static readonly string[] Names={"Capim","Gafanhoto","Sapo"};
        public static readonly string[] Roles={"produtor","herbívoro","predador"};
        static readonly string[] RoleClues={
            "O capim usa luz para produzir matéria orgânica.",
            "O gafanhoto obtém alimento consumindo capim.",
            "O sapo deste modelo se alimenta de gafanhotos."};
        public const string Invitation="Pegue o capim e encaixe no suporte Produtor.";
        public OrganismToken[] tokens;
        public TrophicSocket[] sockets;
        public XRLever flowLever;
        public XRSimpleInteractable decomposers;
        public Renderer[] energy, heat, matter;
        public GameObject controls, consultation, confirmation;
        public Button back, restore, close, cancel, confirm;
        public TMP_Text status, question, confirmLabel;
        public bool Modal { get; private set; }
        public bool Inspecting { get; private set; }
        public int Connected
        {
            get {int i=0;while(i<tokens.Length && tokens[i].Docked)i++;return i;}
        }
        bool exit, leaving;
        float phase;
        MaterialPropertyBlock ink;
        int phaseProperty;
        Tween fade;

        void Awake()
        {
            ink=new MaterialPropertyBlock();
            phaseProperty=Shader.PropertyToID("_Phase");
        }
        void OnEnable()
        {
            back.onClick.AddListener(Back);restore.onClick.AddListener(Restore);
            close.onClick.AddListener(Close); cancel.onClick.AddListener(Cancel); confirm.onClick.AddListener(Confirm);
            flowLever.onLeverActivate.AddListener(FlowChanged);flowLever.onLeverDeactivate.AddListener(FlowChanged);
            decomposers.selectEntered.AddListener(InspectBySelect);
            MotionPreferences.Changed+=Refresh;
        }
        void Start()
        {
            UIVisibility.Set(consultation,false);UIVisibility.Set(confirmation,false);
            Say(Invitation);Refresh();
        }
        void OnDisable()
        {
            back.onClick.RemoveListener(Back);restore.onClick.RemoveListener(Restore);
            close.onClick.RemoveListener(Close);cancel.onClick.RemoveListener(Cancel);confirm.onClick.RemoveListener(Confirm);
            flowLever.onLeverActivate.RemoveListener(FlowChanged);flowLever.onLeverDeactivate.RemoveListener(FlowChanged);
            decomposers.selectEntered.RemoveListener(InspectBySelect); MotionPreferences.Changed-=Refresh; fade?.Kill();
        }
        void Update()
        {
            if(!flowLever.value || Connected==0 || Modal || MotionPreferences.Reduced)return;
            phase=Mathf.Repeat(phase+Time.deltaTime*.12f,19);
            ApplyPhase();
        }
        void ApplyPhase()
        {
            ink.SetFloat(phaseProperty,phase);
            foreach(var r in energy)if(r.enabled)r.SetPropertyBlock(ink);
            foreach(var r in heat)if(r.enabled)r.SetPropertyBlock(ink);
            foreach(var r in matter)if(r.enabled)r.SetPropertyBlock(ink);
        }
        public void Refresh()
        {
            int connected=Connected;
            for(int i=0;i<3;i++)
            {
                energy[i].enabled=heat[i].enabled=flowLever.value && i<connected;
            }
            foreach(var r in matter)r.enabled=flowLever.value && connected==3;
            ApplyPhase();
        }
        public void PickedUp(OrganismToken token)
        {
            Say(Names[token.role]+" na mão. Procure o encaixe de "+Roles[token.role]+".");Refresh();
        }
        public int NearSocket(Vector3 position)
        {
            for(int i=0;i<sockets.Length;i++)
                if(Vector3.Distance(position,sockets[i].attachTransform.position)<=.185f)return i;
            return -1;
        }
        string Refusal(OrganismToken token,int index)
        {
            if(token.role!=index)return "Este encaixe pede "+Roles[index]+". "+RoleClues[index];
            if(index>Connected)return "Complete a cadeia a partir do produtor. Falta encaixar "+Names[Connected]+".";
            return null;
        }
        public void Preview(OrganismToken token,int index)
        {
            string reason=Refusal(token,index);
            Say(reason??("Solte para encaixar "+Names[index]+"."),reason!=null);
        }
        public bool Release(OrganismToken token)
        {
            int slot=NearSocket(token.transform.position);
            string reason=slot<0?null:Refusal(token,slot);
            bool accept=slot>=0 && reason==null && !Modal && !token.Held;
            if(token.Held)return false;
            if(accept)sockets[slot].Seat(token);
            token.Place(accept);Refresh();
            if(!accept)Say((reason??"Solte sobre o suporte correspondente.")+" Ficha devolvida à bandeja.",true);
            else if(Connected==3)Say(flowLever.value?"Cadeia completa. Siga a energia e o retorno de nutrientes.":"Cadeia completa. Acione Exibir fluxo para seguir as setas.");
            else Say(Names[slot]+" encaixado. Agora procure "+Names[Connected]+".");
            return accept;
        }
        void FlowChanged()
        {
            if(Modal)return;
            Refresh();
            Say(!flowLever.value?"Fluxo oculto. A montagem foi preservada.":Connected==0?Invitation:
                "Energia em dourado, calor em salmão e matéria em turquesa.");
        }
        void InspectBySelect(SelectEnterEventArgs _) => Inspect();
        public void Inspect()
        {
            if(Modal)return;
            Inspecting=true;FadeIn(consultation);MarkModule(true);
        }
        void FadeIn(GameObject view)
        {
            fade?.Kill();UIVisibility.Set(view,true);
            if(MotionPreferences.Reduced)return;
            var group=view.GetComponent<CanvasGroup>();group.alpha=0;
            fade=group.DOFade(1,.14f).SetUpdate(true);
        }
        void MarkModule(bool value)
        {
            InteractionMotionBootstrap.Ensure(decomposers);
            decomposers.GetComponent<XRInteractionMotion>().SetPersistentSelected(value);
        }
        public void Close()
        {
            fade?.Kill();Inspecting=false;MarkModule(false);UIVisibility.Set(consultation,false);
        }
        bool HasState()
        {
            foreach(var t in tokens)if(t.Docked || t.Held)return true;
            return flowLever.value;
        }
        void Ask(bool leave)
        {
            if(Modal || leaving)return;
            foreach(var t in tokens)if(t.Held){Say("Solte a ficha antes de continuar.",true);return;}
            Modal=true;exit=leave;fade?.Kill();
            question.text=leave?"Voltar ao Hub e descartar a montagem?":"Devolver as três fichas à bandeja e ocultar o fluxo?";
            confirmLabel.text=leave?"Voltar ao Hub":"Restaurar";UIVisibility.Set(consultation,false);
            UIVisibility.SetInteraction(controls,false);flowLever.enabled=false;decomposers.enabled=false;
            UIVisibility.Set(confirmation,true);
        }
        public void Back()
        {
            if(Modal || leaving)return;
            if(HasState())Ask(true);else Leave();
        }
        public void Restore(){if(HasState())Ask(false);else Say(Invitation);}
        void Leave(){leaving=true;SceneManager.LoadSceneAsync("Assets/_Project/Hub/Scenes/Hub.unity");}
        public void Cancel()
        {
            if(!Modal)return;
            Modal=false;UIVisibility.Set(confirmation,false);UIVisibility.SetInteraction(controls,true);
            flowLever.enabled=true;decomposers.enabled=true;
            if(Inspecting){UIVisibility.Set(consultation,true);MarkModule(true);}
        }
        public void Confirm()
        {
            if(!Modal)return;
            if(exit){Leave();return;}
            Cancel();Close();foreach(var t in tokens)t.Place(false);
            flowLever.value=false;phase=0;Refresh();Say(Invitation);
        }
        void Say(string message,bool error=false)
        {
            status.text=message;status.color=error?new Color32(145,51,44,255):new Color32(7,56,111,255);
        }
    }
}
