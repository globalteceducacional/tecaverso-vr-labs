using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Tecaverso.Interaction;
namespace Tecaverso.Labs.Chemistry.PeriodicBonds
{
    /// <summary>Local-only descriptive content, provenance and one lazy-loaded photograph.</summary>
    public sealed class ElementReferenceView : MonoBehaviour
    {
        [Serializable] public class Catalog { public Entry[] elements; }
        [Serializable] public class Entry {
            public int number;public string occurrence,applications,caption,resource,photoStatus,author,license,licenseUrl,photoSource,sources;
        }
        public TextAsset content;
        public RawImage photo;
        public TMP_Text photoCaption,readingPage;
        public Button previous,next;
        public Toggle reducedMotion;
        Catalog catalog;Texture2D loaded;string loadedPath;
        TMP_Text body;CanvasGroup fade;float reveal=1;int textPage=1;
        public int Count=>Entries.elements.Length;
        Catalog Entries=>catalog??(catalog=JsonUtility.FromJson<Catalog>(content.text));
        void Start()
        {
            previous.onClick.AddListener(()=>Turn(-1));next.onClick.AddListener(()=>Turn(1));
            reducedMotion.SetIsOnWithoutNotify(MotionPreferences.Reduced);reducedMotion.onValueChanged.AddListener(MotionPreferences.SetReduced);
        }
        void Turn(int delta){if(!body)return;textPage=Mathf.Clamp(textPage+delta,1,Mathf.Max(1,body.textInfo.pageCount));body.pageToDisplay=textPage;UpdatePagination();}
        public Entry Get(int number)=>Entries.elements[number-1];
        public void Prepare(TMP_Text target,int number,int page)
        {
            body=target;if(!fade){fade=body.GetComponent<CanvasGroup>();if(!fade)fade=body.gameObject.AddComponent<CanvasGroup>();}
            reveal=MotionPreferences.Reduced?1:0;fade.alpha=MotionPreferences.Reduced?1:.3f;
            bool nature=number>0&&page==2;
            body.rectTransform.anchoredPosition=new Vector2(nature?-285:0,-25);
            body.rectTransform.sizeDelta=new Vector2(nature?760:1340,820);
            body.overflowMode=TextOverflowModes.Page;body.pageToDisplay=textPage=1;
            photo.enabled=false;photo.texture=null;photoCaption.text="";
            string resource=nature?Get(number).resource:null;
            if(loadedPath!=resource){if(loaded)Resources.UnloadAsset(loaded);loaded=null;loadedPath=resource;}
            if(!nature)return;
            var entry=Get(number);
            if(!string.IsNullOrEmpty(resource)){
                if(!loaded)loaded=Resources.Load<Texture2D>(resource);
                photo.texture=loaded;photo.enabled=loaded;
                if(loaded){float aspect=(float)loaded.width/loaded.height;photo.rectTransform.sizeDelta=aspect>1?new Vector2(500,500/aspect):new Vector2(500*aspect,500);}
            }
            photoCaption.text=loaded?entry.caption+"\n\n"+entry.license+" · créditos na aba Fontes":entry.photoStatus;
        }
        public void Complete(){body.ForceMeshUpdate();UpdatePagination();}
        void UpdatePagination(){int pages=Mathf.Max(1,body.textInfo.pageCount);readingPage.text=pages>1?textPage+" / "+pages:"";previous.interactable=textPage>1;next.interactable=textPage<pages;Tecaverso.UI.UIVisibility.Set(previous.gameObject,pages>1);Tecaverso.UI.UIVisibility.Set(next.gameObject,pages>1);}
        void Update(){if(fade&&reveal<1){reveal=MotionPreferences.Reduced?1:Mathf.Clamp01(reveal+Time.unscaledDeltaTime/.16f);fade.alpha=Mathf.Lerp(.3f,1,1-Mathf.Pow(1-reveal,2));}}
        void OnDisable(){if(fade)fade.alpha=1;reveal=1;}
        void OnDestroy(){if(loaded)Resources.UnloadAsset(loaded);}
    }
}

