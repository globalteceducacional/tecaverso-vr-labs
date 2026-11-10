using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

namespace Tecaverso.Labs.Chemistry.PeriodicBonds
{
    public sealed class PeriodicBondsLab : MonoBehaviour
    {
        public BondSocket[] sockets;
        public AtomPiece[] atomPrefabs;
        public Transform[] supplyAnchors;
        public BondDisplay display;
        public TMP_Text elementInfo, secondaryElementInfo, assemblyInfo, resultInfo, statusInfo;
        public Toggle electronsToggle;
        public Button dismantleButton, backButton;
        public Button[] tableButtons;
        readonly BondAssembly assembly = new BondAssembly();
        readonly AtomPiece[] mounted = new AtomPiece[BondAssembly.SlotCount];
        Outline selectedElementOutline;
        bool leaving;

        void Start()
        {
            for (int i = 0; i < sockets.Length; i++)
            {
                int index = i;
                sockets[i].selectEntered.AddListener(args => Dock(index, args));
                sockets[i].selectExited.AddListener(args => Undock(index, args));
                sockets[i].hoverEntered.AddListener(args => Hover(index, args));
            }
            for (int i = 0; i < tableButtons.Length; i++)
            {
                int index = i;
                tableButtons[i].onClick.AddListener(() => SelectElement(index));
            }
            electronsToggle.onValueChanged.AddListener(_ => Refresh());
            dismantleButton.onClick.AddListener(Dismantle);
            backButton.onClick.AddListener(Back);
            for (int i = 0; i < atomPrefabs.Length; i++) Restock((ElementKind)i);
            SelectElement(0); Refresh();
        }
        // This laboratory root is scene-owned; listeners and spawned pieces are disposed on scene unload.
        public bool CanDock(int index, AtomPiece piece) =>
            index >= 0 && index < mounted.Length && piece.dockIndex < 0 && assembly.CanPlace(index, piece.element);

        public void Restock(ElementKind element)
        {
            var piece = Instantiate(atomPrefabs[(int)element], supplyAnchors[(int)element].position,
                supplyAnchors[(int)element].rotation, transform);
            piece.name = BondCatalog.Names[(int)element] + " · átomo";
            piece.lab = this; piece.element = element;
            piece.gameObject.SetActive(true);
        }
        void Dock(int index, SelectEnterEventArgs args)
        {
            var piece = args.interactableObject.transform.GetComponent<AtomPiece>();
            if (!piece || !assembly.Place(index, piece.element)) return;
            piece.dockIndex = index; mounted[index] = piece;
            statusInfo.text = "Átomo encaixado. Continue a montagem ou observe o resultado.";
            Refresh();
        }
        void Undock(int index, SelectExitEventArgs args)
        {
            if (!mounted[index] || mounted[index].transform != args.interactableObject.transform) return;
            mounted[index].dockIndex = -1; mounted[index] = null; assembly.Remove(index);
            statusInfo.text = "Átomo retirado. A composição foi atualizada."; Refresh();
        }
        void Hover(int index, HoverEnterEventArgs args)
        {
            var piece = args.interactableObject.transform.GetComponent<AtomPiece>();
            if (piece && !CanDock(index, piece))
                statusInfo.text = "Esta combinação não faz parte das estruturas estudadas aqui.";
        }
        void SelectElement(int index)
        {
            if (index < 0 || index >= PeriodicElementCatalog.All.Length) return;
            if (selectedElementOutline) selectedElementOutline.enabled = false;
            selectedElementOutline = tableButtons[index].GetComponent<Outline>();
            if (!selectedElementOutline) selectedElementOutline = tableButtons[index].gameObject.AddComponent<Outline>();
            selectedElementOutline.effectColor = new Color(1f, .77f, .15f);
            selectedElementOutline.effectDistance = new Vector2(4, -4);
            selectedElementOutline.enabled = true;
            var element = PeriodicElementCatalog.All[index];
            int active = Array.IndexOf(BondCatalog.Symbols, element.symbol);
            string position = element.group == 0
                ? element.IsLanthanide ? "Lantanídeo · 6º período" : "Actinídeo · 7º período"
                : element.period + "º período · grupo " + element.group;
            string detail = active >= 0
                ? "Elétrons de valência no modelo: " + BondCatalog.ValenceElectrons[active] + " · peça disponível"
                : "Elemento consultável · sem peça nesta montagem";
            string content = element.name + " · " + element.symbol + " · Z = " + element.atomicNumber +
                "\n" + position + "\n" + detail;
            elementInfo.text = content;
            if (secondaryElementInfo) secondaryElementInfo.text = content;
        }
        void Refresh()
        {
            assemblyInfo.text = "MONTAGEM · " + BondCatalog.Composition(assembly.Slots);
            var recipe = assembly.Matched;
            display.Show(recipe, electronsToggle.isOn);
            resultInfo.text = recipe == null ? "Encaixe átomos para investigar uma estrutura." :
                recipe.formula + " · " + recipe.name + "\n" + recipe.classification + "\n" + recipe.explanation;
            dismantleButton.interactable = assembly.Count > 0;
        }
        void Dismantle()
        {
            for (int i = 0; i < mounted.Length; i++) if (mounted[i])
            {
                var piece = mounted[i]; mounted[i] = null; piece.dockIndex = -1;
                Destroy(piece.gameObject);
            }
            assembly.Clear(); statusInfo.text = "Montagem desmontada. Peças disponíveis nos suportes."; Refresh();
        }
        void Back()
        {
            if (leaving) return;
            leaving = true; backButton.interactable = false;
            SceneManager.LoadSceneAsync("Assets/_Project/Hub/Scenes/Hub.unity");
        }
    }
}
