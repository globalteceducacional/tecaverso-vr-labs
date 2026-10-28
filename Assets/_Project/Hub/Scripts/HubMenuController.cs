using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Tecaverso.UI;

namespace Tecaverso.Hub
{
    public sealed class HubMenuController : MonoBehaviour
    {
        [Serializable] public sealed class ContentCard
        {
            public Button button;
            public Image surface, icon;
            public TMP_Text label;
        }
        [SerializeField] HubCatalog catalog;
        [SerializeField] GameObject disciplinesView, contentsView;
        [SerializeField] Button[] disciplineButtons;
        [SerializeField] ContentCard[] cards;
        [SerializeField] Button back, launch;
        [SerializeField] TMP_Text heading, subtitle, detailTitle, description, objective, launchLabel, status;
        [SerializeField] Image detailIcon;
        [SerializeField] Button previousPage, nextPage;
        [SerializeField] TMP_Text pageLabel;
        [SerializeField] GameObject pagination;
        int pageStart;
        int disciplineIndex, contentIndex;
        bool loading;
        bool selectionOnly;
        public event Action<HubCatalog.Content> ContentChosen;
        public void SetSelectionMode(bool value) { selectionOnly=value; ShowDisciplines(); }
        public bool ShowingContents => UIVisibility.IsVisible(contentsView);
        public void Hide() { UIVisibility.Set(disciplinesView,false); UIVisibility.Set(contentsView,false); UIVisibility.Set(back.gameObject,false,true); }
        public string SelectedContent => catalog.disciplines[disciplineIndex].contents[contentIndex].title;

        public void Bind(HubCatalog data, GameObject disciplines, GameObject contents, Button[] buttons,
            ContentCard[] contentCards, Button backButton, Button launchButton, TMP_Text title,
            TMP_Text sub, TMP_Text detail, TMP_Text body, TMP_Text goal, TMP_Text launchText, TMP_Text feedback, Image image)
        {
            catalog=data; disciplinesView=disciplines; contentsView=contents; disciplineButtons=buttons;
            cards=contentCards; back=backButton; launch=launchButton; heading=title; subtitle=sub;
            detailTitle=detail; description=body; objective=goal; launchLabel=launchText; status=feedback; detailIcon=image;
        }
        void Awake()
        {
            for(int i=0;i<disciplineButtons.Length;i++){int index=i; disciplineButtons[i].onClick.AddListener(()=>OpenDiscipline(index));}
            for(int i=0;i<cards.Length;i++){int index=i; cards[i].button.onClick.AddListener(()=>SelectContent(pageStart+index));}
            back.onClick.AddListener(ShowDisciplines); launch.onClick.AddListener(LaunchSelected);
            if(previousPage!=null)previousPage.onClick.AddListener(()=>ChangePage(-1));
            if(nextPage!=null)nextPage.onClick.AddListener(()=>ChangePage(1));
            ShowDisciplines();
        }
        void Update()
        {
            if(!loading&&Keyboard.current!=null&&Keyboard.current.escapeKey.wasPressedThisFrame&&ShowingContents) ShowDisciplines();
        }
        public void ShowDisciplines()
        {
            if(loading) return;
            UIVisibility.Set(disciplinesView,true); UIVisibility.Set(contentsView,false); UIVisibility.Set(back.gameObject,false,true); status.text="";
        }
        public void OpenDiscipline(int index)
        {
            if(loading||index<0||index>=catalog.disciplines.Length) return;
            disciplineIndex=index;
            pageStart=0;
            var discipline=catalog.disciplines[index];
            heading.text=discipline.title;
            subtitle.text=Array.Exists(discipline.contents,c=>!string.IsNullOrEmpty(c.scenePath))?"SELECIONE O CONTEÚDO DA SIMULAÇÃO":"PRÉVIA DA DISCIPLINA · CONTEÚDOS EM DESENVOLVIMENTO";
            RenderCards();
            UIVisibility.Set(disciplinesView,false); UIVisibility.Set(contentsView,true); UIVisibility.Set(back.gameObject,true,true);
            SelectContent(0);
        }
        void ChangePage(int direction)
        {
            if(loading)return;
            int count=catalog.disciplines[disciplineIndex].contents.Length;
            pageStart=Mathf.Clamp(pageStart+direction*cards.Length,0,((count-1)/cards.Length)*cards.Length);
            RenderCards();SelectContent(pageStart);
        }
        void RenderCards()
        {
            var discipline=catalog.disciplines[disciplineIndex];
            for(int i=0;i<cards.Length;i++)
            {
                int index=pageStart+i;
                bool available=index<discipline.contents.Length;
                UIVisibility.Set(cards[i].button.gameObject,available,true);
                if(!available) continue;
                cards[i].label.text=discipline.contents[index].title; cards[i].icon.sprite=discipline.contents[index].icon;
            }
            if(pagination!=null)UIVisibility.Set(pagination,discipline.contents.Length>cards.Length);
            if(previousPage!=null)previousPage.interactable=pageStart>0;
            if(nextPage!=null)nextPage.interactable=pageStart+cards.Length<discipline.contents.Length;
            if(pageLabel!=null)pageLabel.text=$"{pageStart/cards.Length+1} / {Mathf.CeilToInt(discipline.contents.Length/(float)cards.Length)}";
        }
        public void SelectContent(int index)
        {
            var entries=catalog.disciplines[disciplineIndex].contents;
            if(loading||index<0||index>=entries.Length) return;
            contentIndex=index; var content=entries[index];
            for(int i=0;i<cards.Length;i++)
            {
                cards[i].surface.color=pageStart+i==index?new Color32(40,78,160,255):new Color32(211,209,232,255);
                cards[i].label.color=pageStart+i==index?Color.white:new Color32(22,22,22,255);
            }
            detailTitle.text=content.title; description.text=content.description;
            objective.text="OBJETIVO  ·  "+content.objective; detailIcon.sprite=content.icon;
            launch.interactable=!string.IsNullOrEmpty(content.scenePath);
            launchLabel.text=launch.interactable?(selectionOnly?"SELECIONAR PARA A SALA":"INICIAR SIMULAÇÃO"):"EM BREVE";
            status.text=launch.interactable?"Disponível · "+content.title:"MOCKUP · Esta simulação ainda não está disponível.";
        }
        public void LaunchSelected()
        {
            if(loading||!ShowingContents) return;
            var path=catalog.disciplines[disciplineIndex].contents[contentIndex].scenePath;
            if(string.IsNullOrEmpty(path)) return;
            if(selectionOnly) { ContentChosen?.Invoke(catalog.disciplines[disciplineIndex].contents[contentIndex]); return; }
            if(NetworkManager.Singleton!=null&&NetworkManager.Singleton.IsListening)
            { status.text="Desconecte da sala LAN para abrir este experimento local."; return; }
            if(!Application.CanStreamedLevelBeLoaded(path))
            { status.text="Cena não encontrada na lista de cenas da aplicação."; return; }
            StartCoroutine(Load(path));
        }
        IEnumerator Load(string path)
        {
            loading=true; launch.interactable=false; back.interactable=false;
            foreach(var card in cards) card.button.interactable=false;
            status.text="Carregando o laboratório…";
            yield return SceneManager.LoadSceneAsync(path,LoadSceneMode.Single);
        }
    }
}
