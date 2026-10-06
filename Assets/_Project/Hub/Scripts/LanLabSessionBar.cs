using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Tecaverso.Labs.ObliqueLaunch;

namespace Tecaverso.Hub
{
    public sealed class LanLabSessionBar : MonoBehaviour
    {
        LanLobbyController lobby;
        TMP_Text label, actionLabel;
        bool confirm;
        public static void Create(ObliqueLaunchPanel panel,LanLobbyController service)
        {
            var go=new GameObject("LAN session controls",typeof(RectTransform),typeof(Image),typeof(LanLabSessionBar));
            go.transform.SetParent(panel.transform,false); go.layer=panel.gameObject.layer;
            var rect=go.GetComponent<RectTransform>(); rect.sizeDelta=new Vector2(470,128); rect.anchoredPosition=new Vector2(0,-460);
            go.GetComponent<Image>().color=new Color32(232,230,254,255);
            var bar=go.GetComponent<LanLabSessionBar>(); bar.lobby=service;
            var font=panel.GetComponentInChildren<TMP_Text>(true).font;
            bar.label=Text(rect,"Role",font,new Vector2(0,28),new Vector2(450,44));
            var buttonGO=new GameObject("Return",typeof(RectTransform),typeof(Image),typeof(Button)); buttonGO.transform.SetParent(rect,false); buttonGO.layer=go.layer;
            var buttonRect=buttonGO.GetComponent<RectTransform>(); buttonRect.sizeDelta=new Vector2(440,44); buttonRect.anchoredPosition=new Vector2(0,-28);
            var image=buttonGO.GetComponent<Image>(); image.color=new Color32(40,78,160,255);
            buttonGO.GetComponent<Button>().targetGraphic=image;
            bar.actionLabel=Text(buttonRect,"Action",font,Vector2.zero,new Vector2(430,44)); bar.actionLabel.color=Color.white;
            buttonGO.GetComponent<Button>().onClick.AddListener(bar.Return);
            buttonGO.AddComponent<ButtonTweenFeedback>();
            var layout=go.AddComponent<VerticalLayoutGroup>();layout.padding=new RectOffset(15,15,16,16);layout.spacing=8;
            layout.childControlWidth=layout.childControlHeight=true;layout.childForceExpandWidth=true;layout.childForceExpandHeight=false;
            foreach(var child in new[]{bar.label.gameObject,buttonGO}) { var item=child.AddComponent<LayoutElement>();item.minHeight=item.preferredHeight=44; }
            service.Changed+=bar.Refresh; bar.Refresh();
        }
        static TMP_Text Text(Transform parent,string name,TMP_FontAsset font,Vector2 position,Vector2 size)
        {
            var go=new GameObject(name,typeof(RectTransform),typeof(TextMeshProUGUI)); go.transform.SetParent(parent,false); go.layer=parent.gameObject.layer;
            var text=go.GetComponent<TextMeshProUGUI>(); text.font=font; text.fontSize=19; text.alignment=TextAlignmentOptions.Center;
            text.color=new Color32(22,22,22,255); text.raycastTarget=false; text.rectTransform.sizeDelta=size; text.rectTransform.anchoredPosition=position; return text;
        }
        void Return()
        {
            if(!confirm) { confirm=true; Refresh(); Invoke(nameof(Cancel),5f); return; }
            lobby.ReturnToLobby();
        }
        void Cancel() { confirm=false; Refresh(); }
        void Refresh()
        {
            label.text=lobby.IsHost ? lobby.EveryoneInLab ? "LAN · Você controla o experimento" : "LAN · Aguardando participantes…" : "LAN · Observador · Vetores são locais";
            actionLabel.text=confirm?"CONFIRMAR SAÍDA (5 s)":lobby.IsHost?"VOLTAR TODOS À SALA":"SAIR DA SALA";
        }
        void OnDestroy() { if(lobby!=null) lobby.Changed-=Refresh; }
    }
}
