using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Tecaverso.Labs.ObliqueLaunch;

namespace Tecaverso.UI
{
    /// <summary>Presentation state, independent of GameObject/component lifetime.</summary>
    public static class UIVisibility
    {
        public static bool IsVisible(GameObject view)
        {
            if(view==null || !view.activeInHierarchy)return false;
            for(var t=view.transform;t!=null;t=t.parent)
            {
                var group=t.GetComponent<CanvasGroup>();
                if(group!=null && group.alpha<=0f)return false;
            }
            return true;
        }
        public static void Set(GameObject view,bool visible,bool collapse=false)
        {
            if(view==null)return;
            if(!view.TryGetComponent<CanvasGroup>(out var group))group=view.AddComponent<CanvasGroup>();
            if(!visible)
            {
                var selected=EventSystem.current!=null?EventSystem.current.currentSelectedGameObject:null;
                if(selected!=null && selected.transform.IsChildOf(view.transform))EventSystem.current.SetSelectedGameObject(null);
                if(group.alpha>0)foreach(var feedback in view.GetComponentsInChildren<ButtonTweenFeedback>(true))feedback.ResetFeedback();
            }
            group.alpha=visible?1:0;group.interactable=visible;group.blocksRaycasts=visible;
            if(collapse)
            {
                if(!view.TryGetComponent<LayoutElement>(out var item))item=view.AddComponent<LayoutElement>();
                if(item.ignoreLayout==visible) { item.ignoreLayout=!visible; LayoutRebuilder.MarkLayoutForRebuild((RectTransform)view.transform); }
            }
            // Migration compatibility only. Subsequent view transitions do not disable objects.
            if(!view.activeSelf)view.SetActive(true);
        }
        public static void SetInteraction(GameObject view,bool enabled)
        {
            if(!view.TryGetComponent<CanvasGroup>(out var group))group=view.AddComponent<CanvasGroup>();
            enabled=enabled && group.alpha>0f;
            group.interactable=enabled;group.blocksRaycasts=enabled;
        }
    }
}
