using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace Tecaverso.Editor
{
    // Authoring helpers for compact World Space biology interfaces; runtime uses shared UI/XR components.
    public static class BiologyWorldUI
    {
        public static readonly Color Paper=new Color32(232,230,254,255),Blue=new Color32(40,78,160,255),Ink=new Color32(7,56,111,255);
        static TMP_FontAsset Font(bool bold)=>AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-"+(bold?"Bold":"Regular")+" SDF.asset");
        public static RectTransform Rect(Transform parent,string name){var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);return r;}
        public static void Height(RectTransform r,float height){var e=r.gameObject.GetComponent<LayoutElement>()??r.gameObject.AddComponent<LayoutElement>();e.minHeight=e.preferredHeight=height;}
        public static RectTransform Panel(Transform parent,string name,Vector3 position,Vector2 size,float scale,Quaternion rotation)
        {
            var r=Rect(parent,name);r.sizeDelta=size;r.localScale=Vector3.one*scale;r.localRotation=rotation;
            r.gameObject.AddComponent<Canvas>().renderMode=RenderMode.WorldSpace;r.gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();
            var image=r.gameObject.AddComponent<Image>();image.color=Paper;image.raycastTarget=false;r.anchoredPosition3D=position;return r;
        }
        public static RectTransform Body(RectTransform panel,int margin=22)
        {
            var r=Rect(panel,"Conteúdo");r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=Vector2.one*margin;r.offsetMax=-Vector2.one*margin;
            var l=r.gameObject.AddComponent<VerticalLayoutGroup>();l.spacing=10;l.childControlWidth=l.childControlHeight=true;l.childForceExpandHeight=false;return r;
        }
        public static TMP_Text Text(Transform parent,string text,float height,float size,bool bold=false,Color? color=null)
        {
            var r=Rect(parent,"Texto");Height(r,height);var t=r.gameObject.AddComponent<TextMeshProUGUI>();t.font=Font(bold);t.fontSize=size;t.color=color??Ink;t.text=text;t.raycastTarget=false;
            t.alignment=TextAlignmentOptions.Center;t.textWrappingMode=TextWrappingModes.Normal;t.overflowMode=TextOverflowModes.Ellipsis;return t;
        }
        public static Button Button(Transform parent,string title,float height=58)
        {
            var r=Rect(parent,title);Height(r,height);var image=r.gameObject.AddComponent<Image>();image.color=Blue;var button=r.gameObject.AddComponent<Button>();button.targetGraphic=image;
            var label=Text(r,title,height,24,true,Color.white);label.rectTransform.anchorMin=Vector2.zero;label.rectTransform.anchorMax=Vector2.one;label.rectTransform.offsetMin=new Vector2(8,2);label.rectTransform.offsetMax=new Vector2(-8,-2);return button;
        }
        public static Toggle Toggle(Transform parent,string title)
        {
            var r=Rect(parent,title);Height(r,58);var image=r.gameObject.AddComponent<Image>();image.color=Blue;var toggle=r.gameObject.AddComponent<Toggle>();toggle.targetGraphic=image;
            var mark=Rect(r,"Marca");mark.anchorMin=mark.anchorMax=new Vector2(0,.5f);mark.anchoredPosition=new Vector2(20,0);mark.sizeDelta=new Vector2(21,21);var graphic=mark.gameObject.AddComponent<Image>();graphic.color=new Color32(62,211,156,255);toggle.graphic=graphic;
            var label=Text(r,title,58,24,false,Color.white);label.rectTransform.anchorMin=Vector2.zero;label.rectTransform.anchorMax=Vector2.one;label.rectTransform.offsetMin=new Vector2(38,2);label.rectTransform.offsetMax=new Vector2(-5,-2);toggle.SetIsOnWithoutNotify(true);return toggle;
        }
        public static TMP_Text Caption(Transform parent,string text,Vector3 position,Vector2 size,float fontSize,bool bold=true)
        {
            var canvas=Rect(parent,text);canvas.localScale=Vector3.one*.0015f;canvas.sizeDelta=size;canvas.gameObject.AddComponent<Canvas>().renderMode=RenderMode.WorldSpace;
            canvas.anchoredPosition3D=position;var t=Text(canvas,text,size.y,fontSize,bold,Color.white);t.rectTransform.anchorMin=Vector2.zero;t.rectTransform.anchorMax=Vector2.one;t.rectTransform.offsetMin=t.rectTransform.offsetMax=Vector2.zero;return t;
        }
    }
}
