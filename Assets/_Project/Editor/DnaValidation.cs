using System;
using System.Linq;
using Tecaverso.Labs.Biology.Dna;
using Tecaverso.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Object = UnityEngine.Object;

namespace Tecaverso.Editor
{
    public static class DnaValidation
    {
        static void Check(bool value, string issue)
        {
            if (!value) throw new InvalidOperationException(issue);
        }
        public static string Run()
        {
            Check(Application.isPlaying && SceneManager.GetActiveScene().path == DnaAuthoring.ScenePath, "Abra a sala de DNA em Play Mode.");
            var lab = Object.FindFirstObjectByType<DnaLab>();
            Check(lab && lab.sockets.Length == 4 && lab.pieces.Length == 5 && lab.targets.Length == 6, "Kit incompleto.");
            Check(!lab.pairing.HasWork, "Inicie o teste com as duas fitas vazias.");
            var hand = new GameObject("Mão temporária de validação").AddComponent<XRRayInteractor>();
            var manager = lab.sockets[0].interactionManager;
            hand.interactionManager = manager;
            DnaBase Base(char symbol) => lab.pieces.Single(p => p.Letter == symbol);
            void Grab(DnaBase piece)
            {
                hand.transform.SetPositionAndRotation(piece.transform.position, piece.transform.rotation);
                manager.SelectEnter((IXRSelectInteractor)hand, (IXRSelectInteractable)piece);
                Check(piece.InUse && piece.isSelected, "Pega XR não chegou à peça " + piece.symbol);
            }
            void Drop(DnaBase piece, Vector3 position)
            {
                manager.SelectExit((IXRSelectInteractor)hand, (IXRSelectInteractable)piece);
                piece.transform.position = position;
                piece.SendMessage("LateUpdate");
                Check(!piece.InUse, "Soltura XR não chegou à bancada.");
            }
            void Fill()
            {
                for (int i = 0; i < 4; i++)
                {
                    var p = Base(lab.pairing.Expected(i));
                    Grab(p);
                    Drop(p, lab.sockets[i].attachTransform.position);
                    Check(lab.pairing.At(i) == p.Letter && !lab.targets[i].enabled, "Falha ao encaixar " + p.symbol);
                }
            }
            try
            {
                Check(!Base('U').gameObject.activeSelf && Base('T').gameObject.activeSelf, "Bandeja de DNA incorreta.");
                var wrong = Base('A');
                Grab(wrong);
                wrong.transform.position = lab.sockets[0].attachTransform.position;
                wrong.SendMessage("LateUpdate");
                Check(wrong.isSelected && !lab.pairing.HasWork && lab.status.text.Contains("pede T"), "A–A não explicou a recusa durante a pega.");
                lab.ChooseRna();
                Check(!lab.pairing.Rna, "Troca de fita durante a pega.");
                Drop(wrong, lab.sockets[0].attachTransform.position);
                Check(Vector3.Distance(wrong.transform.position, wrong.tray.position) < .001f && !lab.pairing.HasWork, "Recusa consumiu ou perdeu peça.");
                Grab(wrong);
                wrong.transform.position = wrong.tray.position + Vector3.back * 3;
                wrong.SendMessage("LateUpdate");
                wrong.SendMessage("LateUpdate");
                Check(!wrong.isSelected && !wrong.InUse && Vector3.Distance(wrong.transform.position, wrong.tray.position) < .001f,
                    "Peça saiu da bancada sem retornar ao berço.");
                Fill();
                lab.rna.onClick.Invoke();
                Check(lab.pairing.Rna && lab.pairing.Count == 0 && Base('U').gameObject.activeSelf && !Base('T').gameObject.activeSelf, "Separação das atividades falhou.");
                Fill();
                lab.dna.onClick.Invoke();
                Check(lab.pairing.Count == 4 && lab.pairing.At(0) == 'T', "DNA anterior perdido.");
                var t = Base('T');
                Grab(t);
                Check(lab.pairing.Count == 3, "Remoção não liberou a posição.");
                Drop(t, t.tray.position);
                Grab(t); Drop(t, lab.sockets[0].attachTransform.position);
                var target = lab.targets.Single(p => p.targetIndex == 5);
                manager.SelectEnter((IXRSelectInteractor)hand, (IXRSelectInteractable)target);
                manager.SelectExit((IXRSelectInteractor)hand, (IXRSelectInteractable)target);
                Check(lab.consultation.GetComponent<CanvasGroup>().blocksRaycasts && lab.queryTitle.text == "Cromossomo", "Consulta por gesto XR não abriu.");
                lab.restore.onClick.Invoke();
                Check(lab.Modal && UIVisibility.IsVisible(lab.confirmation) && !target.enabled && !t.enabled, "Modal deixou objetos acessíveis.");
                lab.cancel.onClick.Invoke();
                Check(!lab.Modal && lab.pairing.Count == 4 && UIVisibility.IsVisible(lab.consultation), "Cancelamento perdeu fita ou consulta.");
                lab.close.onClick.Invoke();
                lab.rna.onClick.Invoke();
                Check(lab.pairing.Count == 4 && lab.pairing.At(0) == 'U', "RNA anterior perdido.");
                lab.restore.onClick.Invoke();
                lab.confirm.onClick.Invoke();
                Check(!lab.Modal && !lab.pairing.Rna && !lab.pairing.HasWork && !UIVisibility.IsVisible(lab.confirmation), "Restauração incompleta.");
            }
            finally { Object.DestroyImmediate(hand.gameObject); }
            return "OK · 8 encaixes por eventos XR, A–A recusado, retorno à bandeja, remoção/reposição, troca de fita, consulta, cancelamento e restauração.";
        }
    }
}
