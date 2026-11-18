using System;
using System.Linq;
using DG.Tweening;
using Tecaverso.Interaction;
using Tecaverso.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Content.Interaction;

namespace Tecaverso.Labs.Biology.Dna
{
    public sealed class DnaLab : MonoBehaviour
    {
        public DnaBase[] pieces;
        public DnaSocket[] sockets;
        public DnaTarget[] targets;
        public Transform helix;
        public XRKnob dial;
        public GameObject controls, consultation, confirmation;
        public Button dna, rna, restore, back, close, cancel, confirm;
        public TMP_Text status, modeCaption, queryTitle, queryBody;
        public readonly DnaPairing pairing = new();
        public bool Modal { get; private set; }
        DnaTarget inspected;
        int chosen = -1;
        bool leaving;
        Tween queryFade;

        void OnEnable()
        {
            dna.onClick.AddListener(ChooseDna); rna.onClick.AddListener(ChooseRna);
            restore.onClick.AddListener(AskRestore); back.onClick.AddListener(Back);
            close.onClick.AddListener(Close); cancel.onClick.AddListener(Cancel);
            confirm.onClick.AddListener(Restore); dial.onValueChange.AddListener(Rotate);
        }
        void Start()
        {
            UIVisibility.Set(consultation, false);
            UIVisibility.Set(confirmation, false);
            PresentMode();
        }
        void OnDisable()
        {
            queryFade?.Kill();
            dna.onClick.RemoveListener(ChooseDna); rna.onClick.RemoveListener(ChooseRna);
            restore.onClick.RemoveListener(AskRestore); back.onClick.RemoveListener(Back);
            close.onClick.RemoveListener(Close); cancel.onClick.RemoveListener(Cancel);
            confirm.onClick.RemoveListener(Restore); dial.onValueChange.RemoveListener(Rotate);
        }
        void Rotate(float value) => helix.localRotation = Quaternion.Euler(0, value * 360f, 0);
        public void ChooseDna() => ChooseMode(false);
        public void ChooseRna() => ChooseMode(true);
        public void ChooseMode(bool isRna)
        {
            if (Modal || isRna == pairing.Rna) return;
            if (pieces.Any(p => p.InUse)) { Message("Solte a peça antes de trocar de fita.", true); return; }
            pairing.Choose(isRna);
            PresentMode();
        }
        void PresentMode()
        {
            foreach (var p in pieces)
            {
                p.gameObject.SetActive(pairing.Available(p.Letter));
                int index = pairing.Find(p.Letter);
                p.Anchor(index < 0 ? p.tray : sockets[index].attachTransform, false);
            }
            chosen = -1;
            dna.GetComponentInChildren<TMP_Text>().text = pairing.Rna ? "DNA" : "DNA · ativo";
            rna.GetComponentInChildren<TMP_Text>().text = pairing.Rna ? "RNA · ativo" : "RNA";
            Refresh();
            Message(pairing.Count == 4 ? "Fita completa. Pegue uma base encaixada para retirá-la." :
                "Pegue uma base da bandeja e solte-a em frente à letra complementar.");
        }
        void Refresh()
        {
            modeCaption.text = (pairing.Rna ? "RNA" : "DNA") + "    " + pairing.Count + "/4";
            foreach (var t in targets)
            {
                bool slot = t.targetIndex < DnaPairing.Template.Length;
                bool enabledTarget = !Modal && (!slot || pairing.At(t.targetIndex) == '\0');
                t.enabled = enabledTarget;
                foreach (var c in t.colliders) c.enabled = enabledTarget;
                InteractionMotionBootstrap.Ensure(t);
                t.GetComponent<XRInteractionMotion>().SetPersistentSelected(enabledTarget && (t == inspected || t.targetIndex == chosen));
            }
        }
        public bool CanLift(DnaBase piece) => !Modal && pairing.Available(piece.Letter) && !Array.Exists(pieces, p => p != piece && p.InUse);
        public void Lift(DnaBase piece)
        {
            if (Modal) return;
            pairing.Lift(piece.Letter);
            Refresh();
            Message("Base " + piece.symbol + " na mão. Aproxime-a de um encaixe.");
        }
        public int Nearest(Vector3 world)
        {
            int best = -1;
            float distance = .128f * .128f;
            for (int i = 0; i < sockets.Length; i++)
            {
                float d = (sockets[i].attachTransform.position - world).sqrMagnitude;
                if (d <= distance) { best = i; distance = d; }
            }
            return best;
        }
        public bool OutsideWorkArea(Vector3 world)
        {
            Vector3 center = (sockets[0].attachTransform.position + sockets[3].attachTransform.position) * .5f;
            return (world - center).sqrMagnitude > 1.6f * 1.6f;
        }
        string Refusal(int index)
        {
            if (pairing.At(index) != '\0') return "Encaixe ocupado. Retire a base para liberar a posição.";
            return "Molde " + DnaPairing.Template[index] + " pede " + pairing.Expected(index) + " na fita de " + (pairing.Rna ? "RNA." : "DNA.");
        }
        public void Preview(DnaBase piece, int index)
        {
            if (pairing.At(index) == '\0' && piece.Letter == pairing.Expected(index))
                Message("Par " + DnaPairing.Template[index] + "–" + piece.symbol + " compatível. Solte para encaixar.");
            else Message(Refusal(index), true);
        }
        public void Release(DnaBase piece)
        {
            if (Modal) return;
            int index = Nearest(piece.transform.position);
            if (index >= 0 && pairing.Place(index, piece.Letter))
            {
                sockets[index].Seat(piece);
                piece.Anchor(sockets[index].attachTransform, true);
                chosen = -1;
                Refresh();
                Message(pairing.Count == 4 ? "Quatro pares completos. Alterne DNA e RNA para comparar." :
                    "Par " + DnaPairing.Template[index] + "–" + piece.symbol + " encaixado. Escolha a próxima base.");
            }
            else
            {
                bool outside = OutsideWorkArea(piece.transform.position);
                piece.Anchor(piece.tray, true);
                if (outside) Message("A base saiu da área da bancada e voltou à bandeja.");
                else Message(index < 0 ? "Base devolvida à bandeja." : Refusal(index) + " Base devolvida.", index >= 0);
            }
        }
        public void ChooseTarget(DnaTarget target)
        {
            if (Modal) return;
            int index = target.targetIndex;
            if (index < DnaPairing.Template.Length)
            {
                if (pairing.At(index) != '\0') return;
                chosen = index;
                Message("Posição " + (index + 1) + " · molde " + DnaPairing.Template[index] + ". Aproxime a base complementar.");
            }
            else
            {
                bool chromosome = index == 5;
                inspected = target;
                queryTitle.text = chromosome ? "Cromossomo" : "Dupla hélice de DNA";
                queryBody.text = chromosome ?
                    "O DNA se organiza com proteínas nos cromossomos.\n\nO X representa um cromossomo duplicado e condensado. Cada cromátide contém uma molécula de DNA.\n\nAs ampliações dos modelos são independentes." :
                    "Duas fitas antiparalelas formam a dupla hélice. Açúcar e fosfato compõem as laterais. As bases pareiam no interior.\n\nNo DNA, A pareia com T e C com G. Na cópia de RNA, A do molde pareia com U.\n\nCores e distâncias são didáticas.";
                queryFade?.Kill();
                UIVisibility.Set(consultation, true);
                if (!MotionPreferences.Reduced)
                {
                    var group = consultation.GetComponent<CanvasGroup>();
                    group.alpha = 0;
                    queryFade = group.DOFade(1, .14f).SetUpdate(true);
                }
            }
            Refresh();
        }
        public void Close()
        {
            queryFade?.Kill();
            inspected = null;
            UIVisibility.Set(consultation, false);
            Refresh();
        }
        public void AskRestore()
        {
            if (Modal) return;
            if (pieces.Any(p => p.InUse) || dial.isSelected) { Message("Solte a peça ou a manopla antes de restaurar.", true); return; }
            if (!pairing.HasWork) { Message("As duas fitas já estão vazias."); return; }
            SetModal(true);
        }
        void SetModal(bool visible)
        {
            Modal = visible;
            queryFade?.Kill();
            UIVisibility.SetInteraction(controls, !visible);
            UIVisibility.Set(consultation, !visible && inspected != null);
            UIVisibility.Set(confirmation, visible);
            dial.enabled = !visible;
            foreach (var c in dial.colliders) c.enabled = !visible;
            foreach (var p in pieces)
            {
                p.enabled = !visible;
                foreach (var c in p.colliders) c.enabled = !visible;
            }
            Refresh();
        }
        public void Cancel()
        {
            if (Modal) SetModal(false);
        }
        public void Restore()
        {
            if (!Modal) return;
            inspected = null;
            SetModal(false);
            pairing.Reset();
            dial.value = 0;
            PresentMode();
            Message("Duas fitas restauradas. Comece pelo DNA.");
        }
        public void Back()
        {
            if (Modal || leaving) return;
            leaving = true;
            SceneManager.LoadSceneAsync("Assets/_Project/Hub/Scenes/Hub.unity");
        }
        void Message(string text, bool error = false)
        {
            status.text = text;
            status.color = error ? new Color32(143, 44, 49, 255) : new Color32(7, 56, 111, 255);
        }
    }
}
