using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Tecaverso.UI;
using Tecaverso.Interaction;
using DG.Tweening;

namespace Tecaverso.Labs.Biology.Cells
{
    public sealed class CellsLab : MonoBehaviour
    {
        public CellStructure[] structures;
        public CellDisplay[] displays;
        public Toggle labels;
        public Button restore, back, close, cancel, confirm;
        public GameObject controlsRoot, consultationRoot, confirmationRoot;
        public TMP_Text consultationTitle, consultationBody, status, confirmationText, confirmLabel;
        public bool Modal { get; private set; }
        public bool Inspecting => selected != null;
        public bool HasLoose => structures.Any(s => s.detachable && !s.docked);
        public bool HasHeld => structures.Any(s => s.Held);
        CellStructure selected;
        bool leaving, exitConfirmation;
        Tween fade;

        void OnEnable()
        {
            labels.onValueChanged.AddListener(ShowLabels);
            restore.onClick.AddListener(RequestRestore); back.onClick.AddListener(Back);
            close.onClick.AddListener(CloseConsultation); cancel.onClick.AddListener(Cancel); confirm.onClick.AddListener(Confirm);
        }
        void Start()
        {
            UIVisibility.Set(consultationRoot, false); UIVisibility.Set(confirmationRoot, false);
            ShowLabels(labels.isOn);
            Message("Aponte para uma estrutura. Segure uma organela para afastar e reencaixe na origem.");
        }
        void OnDisable()
        {
            fade?.Kill();
            labels.onValueChanged.RemoveListener(ShowLabels);
            restore.onClick.RemoveListener(RequestRestore); back.onClick.RemoveListener(Back);
            close.onClick.RemoveListener(CloseConsultation); cancel.onClick.RemoveListener(Cancel); confirm.onClick.RemoveListener(Confirm);
        }
        public bool OtherHeld(CellStructure target) => structures.Any(s => s != target && s.Held);
        public void Inspect(CellStructure structure)
        {
            if (Modal || !structures.Contains(structure)) return;
            selected = structure;
            consultationTitle.text = structure.title;
            consultationBody.text = structure.function + "\n\n" + structure.context + "\n\n<size=85%>Modelo ampliado; cores didáticas. Separar peças explora o modelo, não uma célula em funcionamento.</size>";
            fade?.Kill(); UIVisibility.Set(consultationRoot, true);
            if (!MotionPreferences.Reduced)
            {
                var group = consultationRoot.GetComponent<CanvasGroup>(); group.alpha = 0;
                fade = group.DOFade(1, .14f).SetUpdate(true);
            }
            RefreshSelection();
            Message(structure.detachable ? "Consulta aberta. Afastamento limitado a 0,4 m; solte perto do encaixe original para devolver." : "Estrutura fixa para consulta. Feche a ficha para girar a célula.");
        }
        public void CloseConsultation()
        {
            fade?.Kill(); selected = null; UIVisibility.Set(consultationRoot, false); RefreshSelection();
        }
        void RefreshSelection()
        {
            foreach (var p in structures)
            {
                if (p.selectionOutline) p.selectionOutline.enabled = false; // Legacy decoration retained only for serialized compatibility.
                InteractionMotionBootstrap.Ensure(p);
                p.GetComponent<XRInteractionMotion>().SetPersistentSelected(p == selected);
                if (p.caption) p.caption.gameObject.SetActive(labels.isOn || p == selected);
            }
        }
        public void ShowLabels(bool _) => RefreshSelection();
        public void Release(CellStructure part)
        {
            if (Modal || !part.detachable) return;
            if (Vector3.Distance(part.transform.position, part.home.position) <= .12f) Return(part, false);
            else
            {
                var other = structures.FirstOrDefault(p => p != part && Vector3.Distance(part.transform.position, p.home.position) < .1f);
                Message(other ? "Esse encaixe pertence a outra estrutura. Devolva a peça à sua origem." : "Peça separada. Reencaixe na origem; peças sem uso retornam em 15 s.", other != null);
            }
        }
        public bool Return(CellStructure part, bool automatic)
        {
            if (part.Held) return false;
            var socket = part.socket;
            if (socket && socket.interactionManager)
            {
                socket.capture = true;
                try
                {
                    socket.interactionManager.SelectEnter((IXRSelectInteractor)socket, (IXRSelectInteractable)part);
                    socket.interactionManager.SelectExit((IXRSelectInteractor)socket, (IXRSelectInteractable)part);
                }
                finally { socket.capture = false; }
            }
            part.RestorePose();
            Message(automatic ? part.title + " retornou ao suporte por estar fora da área ou sem uso." : part.title + " reencaixada na origem.");
            return true;
        }
        public void RequestRestore() => Ask(false);
        void Ask(bool exit)
        {
            if (Modal) return;
            if (HasHeld) { Message("Solte a peça antes de restaurar ou sair.", true); return; }
            exitConfirmation = exit; Modal = true; fade?.Kill();
            UIVisibility.Set(consultationRoot, false); UIVisibility.SetInteraction(controlsRoot, false);
            confirmationText.text = exit ? "Voltar ao Hub?\nA disposição atual das peças será descartada." : "Recompor as duas células?\nPeças e orientação voltarão à origem.";
            confirmLabel.text = exit ? "Voltar ao Hub" : "Recompor";
            UIVisibility.Set(confirmationRoot, true);
        }
        public void Cancel()
        {
            Modal = false; UIVisibility.Set(confirmationRoot, false); UIVisibility.SetInteraction(controlsRoot, true);
            if (selected) UIVisibility.Set(consultationRoot, true);
        }
        public void Confirm()
        {
            if (!Modal) return;
            if (exitConfirmation) { leaving = true; SceneManager.LoadSceneAsync("Assets/_Project/Hub/Scenes/Hub.unity"); return; }
            Cancel(); CloseConsultation();
            foreach (var display in displays) display.Restore();
            foreach (var part in structures) part.RestorePose();
            Message("Células recompostas. A preferência de rótulos foi preservada.");
        }
        public void Back()
        {
            if (leaving || Modal) return;
            if (HasLoose || HasHeld) { Ask(true); return; }
            leaving = true; SceneManager.LoadSceneAsync("Assets/_Project/Hub/Scenes/Hub.unity");
        }
        public void Message(string text, bool error = false)
        {
            status.text = text;
            status.color = error ? new Color32(154, 43, 57, 255) : new Color32(7, 56, 111, 255);
        }
    }
}
