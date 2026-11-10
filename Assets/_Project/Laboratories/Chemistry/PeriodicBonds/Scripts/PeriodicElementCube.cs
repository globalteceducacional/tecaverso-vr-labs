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
        Vector3 outlineRest;
        MaterialPropertyBlock block;
        Tween colorTween, outlineTween;

        void Awake()
        {
            block = new MaterialPropertyBlock();
            surface.GetPropertyBlock(block);
            shown = color;
            outlineRest = outline.transform.localScale;
            var xr = GetComponent<XRSimpleInteractable>();
            xr.selectEntered.AddListener(_ => { explorer.Select(number); Pulse(); });
            xr.firstHoverEntered.AddListener(_ => { hover = true; Refresh(); Pulse(); });
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
            outline.SetActive(selected || hover);
            if (!selected && !hover || MotionPreferences.Reduced)
            {
                outlineTween?.Kill();
                outline.transform.localScale = outlineRest;
            }
        }
        void Pulse()
        {
            outlineTween?.Kill();
            outline.transform.localScale = outlineRest;
            if (MotionPreferences.Reduced) return;
            // Only the decoration grows. Cube, text and collider remain in place.
            outlineTween = DOTween.Sequence()
                .Append(outline.transform.DOScale(outlineRest * 1.035f, .09f).SetEase(Ease.OutQuad))
                .Append(outline.transform.DOScale(outlineRest, .13f).SetEase(Ease.OutQuad))
                .SetUpdate(true);
        }
        void OnDisable()
        {
            MotionPreferences.Changed -= Refresh;
            colorTween?.Kill();
            outlineTween?.Kill();
            hover = false;
            if (outline)
            {
                outline.transform.localScale = outlineRest;
                outline.SetActive(selected);
            }
            if (surface && block != null)
                Paint(selected ? Color.Lerp(color, Color.white, .1f) : dim ? Color.Lerp(color, new Color(.035f, .045f, .09f), .72f) : color);
        }
    }
}
