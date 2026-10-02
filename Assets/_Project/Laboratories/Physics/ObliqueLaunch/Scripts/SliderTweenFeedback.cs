using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Tecaverso.Labs.ObliqueLaunch
{
    [RequireComponent(typeof(Slider)), DisallowMultipleComponent]
    public sealed class SliderTweenFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField, Range(1f,1.5f)] float hoverScale=1.15f;
        [SerializeField, Range(1f,1.5f)] float pressedScale=1.25f;
        [SerializeField, Min(.01f)] float duration=.12f;
        Slider slider;
        Transform handle;
        Vector3 restScale;
        Tween tween;
        bool hovered, pressed, available;

        void Start()
        {
            slider=GetComponent<Slider>(); handle=slider.handleRect;
            if(handle!=null) restScale=handle.localScale;
            available=slider.IsInteractable();
        }
        void Update()
        {
            bool next=slider.IsInteractable();
            if(next==available) return;
            available=next; if(!next) pressed=false; Refresh();
        }
        public void OnPointerEnter(PointerEventData e) { hovered=true; Refresh(); }
        public void OnPointerExit(PointerEventData e) { hovered=false; Refresh(); }
        public void OnPointerDown(PointerEventData e)
        { if(e.button!=PointerEventData.InputButton.Left) return; pressed=true; Refresh(); }
        public void OnPointerUp(PointerEventData e) { pressed=false; Refresh(); }
        void Refresh()
        {
            if(handle==null) return;
            tween?.Kill();
            float factor=slider.IsInteractable() ? (pressed ? pressedScale : hovered ? hoverScale : 1f) : 1f;
            tween=handle.DOScale(restScale*factor,duration).SetEase(Ease.OutQuad).SetUpdate(true);
        }
        void OnDisable()
        {
            tween?.Kill(); tween=null; hovered=pressed=false;
            if(handle!=null) handle.localScale=restScale;
        }
    }
}
