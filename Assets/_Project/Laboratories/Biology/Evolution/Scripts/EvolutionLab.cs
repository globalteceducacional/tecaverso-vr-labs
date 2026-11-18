using Tecaverso.Interaction;
using Tecaverso.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Tecaverso.Labs.Biology.Evolution
{
    public sealed class EvolutionLab : MonoBehaviour
    {
        public MeshRenderer[] moths;
        public Material lightMoth, darkMoth, lightBark, darkBark;
        public MeshRenderer bark;
        public XRLever habitat;
        public XRSimpleInteractable step;
        public Renderer stepCap;
        public Material stepAvailable, stepUnavailable;
        public TMP_Text counts, reference, status, stepCaption;
        public Button save, restore, about, back, close, confirm, cancel;
        public GameObject actions, consultation, confirmation;
        public SelectionPopulation Population { get; } = new SelectionPopulation();
        public bool InTransition { get; private set; }
        public bool Confirming { get; private set; }

        const string Intro = "Guarde uma referência e avance uma geração.";
        const string End = "Seis gerações concluídas. Compare o registro ou restaure a população.";
        const float Depart = .48f, Observe = 1.10f, Exchange = 1.40f, Arrive = 1.98f;
        readonly bool[] keep = new bool[SelectionPopulation.Size];
        int previousLight;
        bool offspringVisible, restoreConsultation, leaving;
        float elapsed;

        void OnEnable()
        {
            step.selectEntered.AddListener(Pressed);
            habitat.onLeverActivate.AddListener(SetDark);
            habitat.onLeverDeactivate.AddListener(SetLight);
            save.onClick.AddListener(SaveReference);
            restore.onClick.AddListener(RequestRestore);
            about.onClick.AddListener(OpenConsultation);
            back.onClick.AddListener(ReturnToHub);
            close.onClick.AddListener(CloseConsultation);
            confirm.onClick.AddListener(ConfirmRestore);
            cancel.onClick.AddListener(CancelRestore);
            MotionPreferences.Changed += MotionChanged;
        }

        void OnDisable()
        {
            MotionPreferences.Changed -= MotionChanged;
            step.selectEntered.RemoveListener(Pressed);
            habitat.onLeverActivate.RemoveListener(SetDark);
            habitat.onLeverDeactivate.RemoveListener(SetLight);
            save.onClick.RemoveListener(SaveReference);
            restore.onClick.RemoveListener(RequestRestore);
            about.onClick.RemoveListener(OpenConsultation);
            back.onClick.RemoveListener(ReturnToHub);
            close.onClick.RemoveListener(CloseConsultation);
            confirm.onClick.RemoveListener(ConfirmRestore);
            cancel.onClick.RemoveListener(CancelRestore);
            if (InTransition) FinishGeneration();
        }

        void Start() => PresentInitial();

        // Also called once during authoring so the saved scene shows the actual entry state.
        public void PresentInitial()
        {
            Population.Reset();
            InTransition = Confirming = false;
            habitat.value = false;
            ApplyGround();
            PaintGeneration();
            foreach (var moth in moths) moth.transform.localScale = Vector3.one;
            UIVisibility.Set(consultation, false);
            UIVisibility.Set(confirmation, false);
            RefreshReadout();
            status.text = Intro;
            SetControls();
        }

        static string Ground(bool dark) => dark ? "escura" : "clara";
        static int Rank(int slot, int generation) => (slot * 11 + generation * 7) % SelectionPopulation.Size;

        void SetDark() => SetEnvironment(true);
        void SetLight() => SetEnvironment(false);
        void ApplyGround() => bark.sharedMaterial = Population.DarkGround ? darkBark : lightBark;

        public void SetEnvironment(bool dark)
        {
            if (Confirming || InTransition || Population.DarkGround == dark) return;
            Population.SetGround(dark);
            habitat.value = dark;
            ApplyGround();
            RefreshReadout();
            status.text = Population.Generation == SelectionPopulation.Limit ? End :
                "A casca mudou. As mariposas mantêm sua pigmentação.";
        }

        void Pressed(SelectEnterEventArgs _) => Advance();
        public void Advance()
        {
            if (Confirming || InTransition || Population.Generation >= SelectionPopulation.Limit) return;
            previousLight = Population.Light;
            int previousGeneration = Population.Generation;
            Population.Advance();
            if (MotionPreferences.Reduced) { FinishGeneration(); return; }

            // Only the departing parents are dimmed out. Each successor enters with its own pigment.
            int lightSurvivors = Population.DarkGround ? Mathf.RoundToInt(previousLight * .7f) : previousLight;
            int darkParents = SelectionPopulation.Size - previousLight;
            int darkSurvivors = Population.DarkGround ? darkParents : Mathf.RoundToInt(darkParents * .7f);
            for (int i = 0; i < keep.Length; i++)
            {
                int rank = Rank(i, previousGeneration);
                keep[i] = rank < previousLight ? rank < lightSurvivors : rank - previousLight < darkSurvivors;
            }
            InTransition = true;
            offspringVisible = false;
            elapsed = 0f;
            status.text = "Seleção · observe quais fenótipos deixam mais descendentes.";
            SetControls();
        }

        void Update()
        {
            if (InTransition) TickTransition(Time.unscaledDeltaTime);
        }

        public void TickTransition(float seconds)
        {
            if (!InTransition) return;
            elapsed += Mathf.Max(0f, seconds);
            if (elapsed >= Arrive) { FinishGeneration(); return; }
            if (elapsed >= Exchange && !offspringVisible)
            {
                offspringVisible = true;
                PaintGeneration();
                RefreshReadout();
                status.text = "Descendentes · cada mariposa já nasce com seu fenótipo.";
            }
            for (int i = 0; i < moths.Length; i++)
            {
                float size;
                if (offspringVisible) size = Mathf.InverseLerp(Exchange, Arrive, elapsed);
                else if (keep[i]) size = 1f - Mathf.InverseLerp(Observe, Exchange, elapsed);
                else size = 1f - Mathf.InverseLerp(0f, Depart, elapsed);
                moths[i].transform.localScale = Vector3.one * Mathf.SmoothStep(0f, 1f, size);
            }
        }

        void PaintGeneration()
        {
            for (int i = 0; i < moths.Length; i++)
                moths[i].sharedMaterial = Rank(i, Population.Generation) < Population.Light ? lightMoth : darkMoth;
        }
        void MotionChanged()
        {
            if (MotionPreferences.Reduced && InTransition) FinishGeneration();
        }
        void FinishGeneration()
        {
            InTransition = false;
            PaintGeneration();
            foreach (var moth in moths) moth.transform.localScale = Vector3.one;
            RefreshReadout();
            status.text = Population.Generation == SelectionPopulation.Limit ? End :
                $"Geração {Population.Generation} · compare os descendentes com a referência.";
            SetControls();
        }

        void RefreshReadout()
        {
            counts.text = $"Geração {Population.Generation} de 6   ·   Casca {Ground(Population.DarkGround)}\n" +
                          $"{Population.Light} claras     {Population.Dark} escuras";
            var saved = Population.Reference;
            reference.text = saved.HasValue
                ? $"Referência G{saved.Value.Generation} · casca {Ground(saved.Value.DarkGround)}\n" +
                  $"{saved.Value.Light} claras     {SelectionPopulation.Size - saved.Value.Light} escuras"
                : "Referência\nSem registro";
        }

        public void SaveReference()
        {
            if (Confirming || InTransition) return;
            Population.SaveReference();
            RefreshReadout();
            status.text = Population.Generation == SelectionPopulation.Limit ? End : "Referência guardada para comparação.";
        }
        public void OpenConsultation()
        {
            if (!Confirming) UIVisibility.Set(consultation, true);
        }
        public void CloseConsultation() => UIVisibility.Set(consultation, false);
        public void RequestRestore()
        {
            if (Confirming || InTransition) return;
            if (!Population.HasChanges) { status.text = "A população já está no estado inicial."; return; }
            restoreConsultation = UIVisibility.IsVisible(consultation);
            CloseConsultation();
            Confirming = true;
            UIVisibility.Set(confirmation, true);
            SetControls();
        }
        public void ConfirmRestore() { if (Confirming) PresentInitial(); }
        public void CancelRestore()
        {
            if (!Confirming) return;
            Confirming = false;
            UIVisibility.Set(confirmation, false);
            if (restoreConsultation) OpenConsultation();
            SetControls();
        }
        void SetControls()
        {
            bool edit = !Confirming && !InTransition;
            bool limit = Population.Generation == SelectionPopulation.Limit;
            habitat.enabled = edit;
            step.enabled = edit && !limit;
            stepCap.sharedMaterial = step.enabled ? stepAvailable : stepUnavailable;
            stepCaption.text = limit && !InTransition ? "Limite · 6 gerações" : "Avançar geração";
            save.interactable = restore.interactable = edit;
            UIVisibility.SetInteraction(actions, !Confirming);
        }
        public void ReturnToHub()
        {
            if (leaving || Confirming) return;
            leaving = true;
            SceneManager.LoadSceneAsync("Assets/_Project/Hub/Scenes/Hub.unity");
        }
    }
}
