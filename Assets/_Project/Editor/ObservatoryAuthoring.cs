using System.Linq;
using Tecaverso.Labs.Gravitation;
using Tecaverso.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object=UnityEngine.Object;
namespace Tecaverso.Editor
{
    public static class ObservatoryAuthoring
    {
        static void Height(GameObject go,float h){var e=go.GetComponent<LayoutElement>()??go.AddComponent<LayoutElement>();e.minHeight=e.preferredHeight=h;e.flexibleHeight=0;}
        public static void Configure(GravitationLab lab)
        {
            var p=lab.panel;var s=lab.solar;var inspect=s.GetComponent<PlanetInspection>();
            s.transform.position=new Vector3(0,1.8f,4);
            foreach(var t in s.transform.Cast<Transform>().Where(t=>t.name.StartsWith("Scale tick")||t.name.StartsWith("Ruler mark")||t.name=="Distance ruler"||t.name=="Distance scale"))
                t.gameObject.SetActive(false);
            inspect.hoverCard.SetActive(false);inspect.preview.gameObject.SetActive(false);
            inspect.rotateLeft.gameObject.SetActive(false);inspect.rotateRight.gameObject.SetActive(false);
            var card=inspect.title.GetComponentInParent<Canvas>(true).GetComponent<RectTransform>();
            card.sizeDelta=new Vector2(900,700);
            inspect.title.rectTransform.sizeDelta=new Vector2(820,70);inspect.title.rectTransform.anchoredPosition=new Vector2(0,280);inspect.title.fontSize=34;
            inspect.details.rectTransform.sizeDelta=new Vector2(820,440);inspect.details.rectTransform.anchoredPosition=new Vector2(0,0);inspect.details.fontSize=27;
            var row=(RectTransform)inspect.close.transform.parent;row.sizeDelta=new Vector2(820,80);row.anchoredPosition=new Vector2(0,-290);
            inspect.close.GetComponentInChildren<TMP_Text>(true).text="FECHAR INFORMAÇÕES";
            inspect.Close();
            foreach(var b in inspect.planetButtons)b.transform.parent.gameObject.SetActive(false);
            p.solarViews[0].transform.parent.gameObject.SetActive(false);
            p.solarGuide.transform.SetAsFirstSibling();Height(p.solarGuide.gameObject,72);
            if(p.timeSpeeds==null||p.timeSpeeds.Length!=3)
            {
                var go=new GameObject("Orbital speed presets",typeof(RectTransform),typeof(HorizontalLayoutGroup),typeof(LayoutElement));
                go.transform.SetParent(p.solarControls.transform,false);
                var layout=go.GetComponent<HorizontalLayoutGroup>();layout.spacing=8;layout.childControlHeight=layout.childControlWidth=true;layout.childForceExpandWidth=true;
                Height(go,64);p.timeSpeeds=new Button[3];
                for(int i=0;i<3;i++)
                {
                    var b=Object.Instantiate(p.solarViews[0],go.transform);b.name="Orbital speed "+i;b.onClick=new Button.ButtonClickedEvent();b.gameObject.SetActive(true);
                    b.GetComponentInChildren<TMP_Text>(true).fontSize=20;
                    var e=b.GetComponent<LayoutElement>();e.minWidth=0;e.preferredWidth=-1;e.flexibleWidth=1;Height(b.gameObject,64);
                    p.timeSpeeds[i]=b;
                }
            }
            Height(p.solarControls,152);
            Height(p.scale.gameObject,84);
            var pause=p.actions[1].GetComponent<LayoutElement>();pause.minWidth=0;pause.preferredWidth=-1;pause.flexibleWidth=1;
            var content=p.modes[0].transform.parent.parent;
            content.GetChild(0).GetComponent<TMP_Text>().text="OBSERVATÓRIO · SISTEMA SOLAR";
            // Compact the internal panel; preserve the world Canvas transform.
            var pages=(RectTransform)p.pages[3].transform;
            ((RectTransform)pages.parent).sizeDelta=new Vector2(720,940);
            pages.anchorMin=Vector2.zero;pages.anchorMax=Vector2.one;pages.offsetMin=pages.offsetMax=Vector2.zero;
            var parameters=(RectTransform)p.pages[0].transform;parameters.sizeDelta=new Vector2(720,820);parameters.anchoredPosition=Vector2.zero;
            var surface=(RectTransform)parameters.Find("Surface");surface.anchorMin=Vector2.zero;surface.anchorMax=Vector2.one;surface.offsetMin=new Vector2(2,2);surface.offsetMax=new Vector2(-2,-2);
            var back=(RectTransform)p.actions[5].transform;back.anchorMin=back.anchorMax=new Vector2(0,1);back.pivot=new Vector2(0,1);back.anchoredPosition=new Vector2(0,-844);back.sizeDelta=new Vector2(720,72);
            var modal=(RectTransform)p.pages[2].transform;modal.anchorMin=Vector2.zero;modal.anchorMax=Vector2.one;modal.offsetMin=modal.offsetMax=Vector2.zero;
            var modalSurface=(RectTransform)modal.Find("Surface");modalSurface.anchorMin=Vector2.zero;modalSurface.anchorMax=Vector2.one;modalSurface.offsetMin=new Vector2(2,2);modalSurface.offsetMax=new Vector2(-2,-2);
            lab.ResetAttempt();s.Show(0,true);
            EditorUtility.SetDirty(inspect);EditorUtility.SetDirty(p);EditorUtility.SetDirty(s);EditorUtility.SetDirty(lab);
        }
    }
}
