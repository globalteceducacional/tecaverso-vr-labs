using Tecaverso.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Tecaverso.Labs.Biology.Microbes
{
    public sealed class MicrobesLab : MonoBehaviour
    {
        public MicrobeExhibit[] exhibits;
        public GameObject[] stages;
        public TMP_Text stageCaption, status, queryTitle, queryText;
        public GameObject query, confirmation;
        public Button close, restore, cancel, confirm, back;
        public Toggle labels;
        public XRSimpleInteractable advance;
        public MicrobeExhibit Selected { get; private set; }
        public int Stage { get; private set; }
        public bool Confirming { get; private set; }
        static readonly string[] Captions = {
            "1 / 3    Contato e entrada\nO fago se liga à bactéria e seu material genético entra.",
            "2 / 3    Síntese e montagem\nNo interior da célula, formam-se novos fagos.",
            "3 / 3    Lise e liberação\nA célula se rompe e os novos fagos são liberados."
        };
        const string Hint = "Selecione um exemplar. Gire a manopla para examinar o outro lado.";

        void OnEnable()
        {
            close.onClick.AddListener(Close);
            restore.onClick.AddListener(AskRestore);
            cancel.onClick.AddListener(Cancel);
            confirm.onClick.AddListener(Restore);
            back.onClick.AddListener(Back);
            labels.onValueChanged.AddListener(ShowLabels);
            advance.selectEntered.AddListener(Pressed);
        }

        void Start()
        {
            ShowStage(0);
            Close();
            ShowLabels(false);
            UIVisibility.Set(confirmation, false);
        }

        void OnDisable()
        {
            close.onClick.RemoveListener(Close);
            restore.onClick.RemoveListener(AskRestore);
            cancel.onClick.RemoveListener(Cancel);
            confirm.onClick.RemoveListener(Restore);
            back.onClick.RemoveListener(Back);
            labels.onValueChanged.RemoveListener(ShowLabels);
            advance.selectEntered.RemoveListener(Pressed);
        }

        public void Inspect(MicrobeExhibit exhibit)
        {
            if (Confirming || !exhibit) return;
            if (Selected && Selected != exhibit) Selected.Highlight(false);
            Selected = exhibit;
            Selected.Highlight(true);
            queryTitle.text = exhibit.title;
            queryText.text = exhibit.category + "\n\n" + exhibit.description + "\n\n" + exhibit.note;
            UIVisibility.Set(query, true);
            status.text = "Consulta aberta · " + exhibit.title;
        }

        public void Close()
        {
            if (Confirming) return;
            if (Selected) Selected.Highlight(false);
            Selected = null;
            UIVisibility.Set(query, false);
            status.text = Hint;
        }

        public void ShowLabels(bool value)
        {
            if (Confirming) return;
            foreach (var exhibit in exhibits) exhibit.annotation.SetActive(value);
            labels.SetIsOnWithoutNotify(value);
        }

        void ShowStage(int index)
        {
            Stage = index;
            for (int i = 0; i < stages.Length; i++) stages[i].SetActive(i == index);
            stageCaption.text = Captions[index];
        }

        void Pressed(SelectEnterEventArgs args) => Next();
        public void Next()
        {
            if (Confirming) return;
            ShowStage((Stage + 1) % stages.Length);
            status.text = "Etapa " + (Stage + 1) + " de 3. Avance quando terminar a leitura.";
        }

        void Lock(bool value)
        {
            Confirming = value;
            foreach (var exhibit in exhibits)
            {
                exhibit.enabled = !value;
                exhibit.dial.enabled = !value;
            }
            advance.enabled = labels.interactable = back.interactable = restore.interactable = !value;
            UIVisibility.Set(query, !value && Selected);
            UIVisibility.Set(confirmation, value);
        }
        public void AskRestore() { if (!Confirming) Lock(true); }
        public void Cancel()
        {
            if (!Confirming) return;
            Lock(false);
            if (Selected) Inspect(Selected);
        }
        public void Restore()
        {
            if (!Confirming) return;
            Lock(false);
            Close();
            ShowLabels(false);
            ShowStage(0);
            foreach (var exhibit in exhibits)
            {
                exhibit.dial.value = 0;
                exhibit.turntable.localRotation = Quaternion.identity;
            }
            status.text = "Galeria restaurada. " + Hint;
        }
        public void Back() { if (!Confirming) SceneManager.LoadSceneAsync("Assets/_Project/Hub/Scenes/Hub.unity"); }
    }
}
