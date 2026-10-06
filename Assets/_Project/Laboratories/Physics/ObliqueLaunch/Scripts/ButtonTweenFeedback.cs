using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Tecaverso.Labs.ObliqueLaunch
{
    [RequireComponent(typeof(Button)), DisallowMultipleComponent]
    public sealed class ButtonTweenFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField, Range(1f, 1.2f)] float hoverScale = 1.05f;
        [SerializeField, Range(.8f, 1f)] float pressedScale = .96f;
        [SerializeField, Min(.01f)] float duration = .12f;
        Button button;
        [SerializeField] RectTransform visual;
        Vector3 restScale;
        Tween tween;
        bool hovered, pressed, wasInteractable;

        void Awake() { button=GetComponent<Button>(); EnsureVisual(); restScale=visual.localScale; }
        public void EnsureVisual()
        {
            if(visual!=null)return;
            var rect=(RectTransform)transform;
            var children=new System.Collections.Generic.List<Transform>();
            foreach(Transform child in transform)children.Add(child);
            visual=new GameObject("Visual",typeof(RectTransform)).GetComponent<RectTransform>();
            visual.gameObject.layer=gameObject.layer;visual.SetParent(transform,false);
            visual.anchorMin=Vector2.zero;visual.anchorMax=Vector2.one;visual.pivot=rect.pivot;visual.offsetMin=visual.offsetMax=Vector2.zero;
            foreach(var child in children)child.SetParent(visual,false);
            var control=GetComponent<Button>();var background=GetComponent<Image>();
            if(background!=null && control.targetGraphic==background)
            {
                var plate=new GameObject("Background",typeof(RectTransform),typeof(Image));plate.layer=gameObject.layer;plate.transform.SetParent(visual,false);plate.transform.SetAsFirstSibling();
                var rt=(RectTransform)plate.transform;rt.anchorMin=Vector2.zero;rt.anchorMax=Vector2.one;rt.offsetMin=rt.offsetMax=Vector2.zero;
                var copy=plate.GetComponent<Image>();copy.sprite=background.sprite;copy.type=background.type;copy.color=background.color;copy.material=background.material;copy.raycastTarget=false;
                background.color=Color.clear;control.targetGraphic=copy;
            }
        }
        void OnEnable() { wasInteractable=button.IsInteractable(); }
        void Update()
        {
            bool available=button.IsInteractable();
            if(available==wasInteractable) return;
            wasInteractable=available;
            if(!available) pressed=false;
            Refresh();
        }
        public void OnPointerEnter(PointerEventData e) { hovered=true; Refresh(); }
        public void OnPointerExit(PointerEventData e) { hovered=false; pressed=false; Refresh(); }
        public void OnPointerDown(PointerEventData e)
        { if(e.button!=PointerEventData.InputButton.Left || !button.IsInteractable()) return; pressed=true; Refresh(); }
        public void OnPointerUp(PointerEventData e) { pressed=false; Refresh(); }
        void Refresh()
        {
            tween?.Kill();
            float factor=button.IsInteractable() ? (pressed ? pressedScale : hovered ? Mathf.Min(hoverScale,1.02f) : 1f) : 1f;
            tween=visual.DOScale(restScale*factor,duration).SetEase(Ease.OutQuad).SetUpdate(true);
        }
        public void ResetFeedback()
        { tween?.Kill(); tween=null; hovered=pressed=false; if(visual!=null)visual.localScale=restScale==Vector3.zero?Vector3.one:restScale; }
        void OnDisable() => ResetFeedback();
    }
}
