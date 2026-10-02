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
        Vector3 restScale;
        Tween tween;
        bool hovered, pressed, wasInteractable;

        void Awake() { button=GetComponent<Button>(); restScale=transform.localScale; }
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
            float factor=button.IsInteractable() ? (pressed ? pressedScale : hovered ? hoverScale : 1f) : 1f;
            tween=transform.DOScale(restScale*factor,duration).SetEase(Ease.OutQuad).SetUpdate(true);
        }
        void OnDisable()
        { tween?.Kill(); tween=null; hovered=pressed=false; transform.localScale=restScale; }
    }
}
