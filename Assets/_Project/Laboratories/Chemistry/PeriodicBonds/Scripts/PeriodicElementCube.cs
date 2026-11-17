using DG.Tweening;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Tecaverso.Interaction;

namespace Tecaverso.Labs.Chemistry.PeriodicBonds
{
    [RequireComponent(typeof(XRSimpleInteractable))]
    public sealed class PeriodicElementCube : MonoBehaviour
    {
        public int number;
        public PeriodicTableExplorer explorer;
        public Renderer surface;
        public GameObject outline;
        public Color color;
        bool selected, hover, dim;
        Color shown;
        MaterialPropertyBlock block;
        Tween colorTween;
        XRInteractionMotion feedback;

        void Awake()
        {
            block = new MaterialPropertyBlock();
            surface.GetPropertyBlock(block);
            shown = color;
            var xr = GetComponent<XRSimpleInteractable>();
            InteractionMotionBootstrap.Ensure(xr);feedback=GetComponent<XRInteractionMotion>();
            feedback.visualTargets=new[]{surface};feedback.RefreshVisuals();
            if(outline)outline.SetActive(false);
            xr.selectEntered.AddListener(_ => { explorer.Select(number); });
            xr.firstHoverEntered.AddListener(_ => { hover = true; Refresh(); });
            xr.lastHoverExited.AddListener(_ => { hover = false; Refresh(); });
        }
        void OnEnable()
        {
            MotionPreferences.Changed += Refresh;
            Refresh();
        }
        public void SetState(bool active, bool dimmed)
        {
            selected = active;
            dim = dimmed;
            Refresh();
        }
        void Paint(Color value)
        {
            shown = value;
            block.SetColor("_BaseColor", value);
            surface.SetPropertyBlock(block);
        }
        void Refresh()
        {
            colorTween?.Kill();
            Color target = dim ? Color.Lerp(color, new Color(.035f, .045f, .09f), .72f) : color;
            // Hover remains visible even while another element is selected.
            if (hover) target = Color.Lerp(color, Color.white, .22f);
            else if (selected) target = Color.Lerp(color, Color.white, .1f);
            if (MotionPreferences.Reduced || !isActiveAndEnabled) Paint(target);
            else colorTween = DOTween.To(() => shown, Paint, target, .16f).SetEase(Ease.OutQuad).SetUpdate(true);
            if(outline)outline.SetActive(false);
            if(feedback)feedback.SetPersistentSelected(selected);
        }
        void OnDisable()
        {
            MotionPreferences.Changed -= Refresh;
            colorTween?.Kill();
            hover = false;
            if(outline)outline.SetActive(false);
            if(feedback)feedback.ResetFeedback();
            if (surface && block != null)
                Paint(selected ? Color.Lerp(color, Color.white, .1f) : dim ? Color.Lerp(color, new Color(.035f, .045f, .09f), .72f) : color);
        }
    }
}
