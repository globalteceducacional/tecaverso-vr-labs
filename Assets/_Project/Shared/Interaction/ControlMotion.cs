using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace Tecaverso.Interaction
{
    // Animates an independent decoration. Layout, hit targets, text and values never move.
    [DisallowMultipleComponent,RequireComponent(typeof(Selectable))]
    public sealed class ControlMotion : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler,IPointerUpHandler,ISelectHandler,IDeselectHandler
    {
        Selectable control;RectTransform frame;CanvasGroup opacity;
        bool hover,focus,pressed;float pulse,amount;
        void Awake()
        {
            control=GetComponent<Selectable>();
            var go=new GameObject("Interaction motion",typeof(RectTransform),typeof(CanvasGroup),typeof(LayoutElement));go.layer=gameObject.layer;
            frame=(RectTransform)go.transform;frame.SetParent(transform,false);frame.anchorMin=Vector2.zero;frame.anchorMax=Vector2.one;frame.offsetMin=frame.offsetMax=Vector2.zero;
            go.GetComponent<LayoutElement>().ignoreLayout=true;opacity=go.GetComponent<CanvasGroup>();opacity.blocksRaycasts=false;opacity.interactable=false;opacity.alpha=0;
            for(int i=0;i<4;i++){
                var bar=new GameObject("Focus edge",typeof(RectTransform),typeof(Image));bar.layer=gameObject.layer;var rt=(RectTransform)bar.transform;rt.SetParent(frame,false);
                rt.anchorMin=i<2?new Vector2(i,0):new Vector2(0,i-2);rt.anchorMax=i<2?new Vector2(i,1):new Vector2(1,i-2);rt.sizeDelta=i<2?new Vector2(3,0):new Vector2(0,3);rt.anchoredPosition=Vector2.zero;
                var image=bar.GetComponent<Image>();image.color=new Color(.15f,.68f,1);image.raycastTarget=false;
            }
            if(control is Button b)b.onClick.AddListener(Pulse);
            if(control is Toggle t)t.onValueChanged.AddListener(Toggled);
            if(control is Slider s)s.onValueChanged.AddListener(Slid);
            if(control is TMPro.TMP_Dropdown d)d.onValueChanged.AddListener(Chosen);
        }
        void Toggled(bool _)=>Pulse();void Slid(float _)=>Pulse();void Chosen(int _)=>Pulse();
        public void Pulse(){pulse=.22f;}
        void LateUpdate()
        {
            if(!hover&&!focus&&!pressed&&pulse<=0&&amount<=0)return;
            bool available=control.IsInteractable()&&Tecaverso.UI.UIVisibility.IsVisible(gameObject);
            if(!available){ResetFeedback();return;}
            pulse=Mathf.Max(0,pulse-Time.unscaledDeltaTime);
            float target=hover||focus||pressed||pulse>0?1:0;
            amount=MotionPreferences.Reduced?target:Mathf.MoveTowards(amount,target,Time.unscaledDeltaTime/0.12f);
            opacity.alpha=amount;
            float scale=MotionPreferences.Reduced?1:1+amount*.012f+(pulse>0?Mathf.Sin(pulse/.22f*Mathf.PI)*.018f:0);
            frame.localScale=Vector3.one*scale;
        }
        public void ResetFeedback(){hover=focus=pressed=false;pulse=amount=0;if(opacity)opacity.alpha=0;if(frame)frame.localScale=Vector3.one;}
        public void OnPointerEnter(PointerEventData e){hover=true;}public void OnPointerExit(PointerEventData e){hover=pressed=false;}
        public void OnPointerDown(PointerEventData e){if(e.button==PointerEventData.InputButton.Left&&control.IsInteractable()){pressed=true;Pulse();}}
        public void OnPointerUp(PointerEventData e){pressed=false;}public void OnSelect(BaseEventData e){focus=true;}public void OnDeselect(BaseEventData e){focus=false;}
        void OnDisable()=>ResetFeedback();
    }
}
