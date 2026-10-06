using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;

namespace Tecaverso.Hub
{
    /// <summary>Presentation only. Networking and experiment lifecycle live in separate services.</summary>
    public sealed class HubEntryController : MonoBehaviour
    {
        public LanLobbyController lobby;
        public HubMenuController menu;
        public GameObject entryPage, formPage, lobbyPage, errorPage, connectingPage;
        public Button solo, create, join, submit, formBack, chooseContent, start, ready, leave, cancelConnection, retry, errorBack, selectionBack, confirmLeave, cancelLeave;
        public GameObject leaveDialog;
        public TMP_InputField nickname, roomOrAddress;
        public TMP_Text formTitle, fieldLabel, fieldHint, formInfo, formError, submitLabel, roomTitle, roomAddress, participants, contentTitle, lobbyHint, readyLabel, errorTitle, errorBody, connectionText;
        bool hosting, selecting, readyValue;
        GameObject[] originalViews;
        void Start()
        {
            originalViews=transform.parent.Cast<Transform>().Where(t=>t!=transform).Select(t=>t.gameObject).ToArray();
            solo.onClick.AddListener(()=>Select(false)); create.onClick.AddListener(()=>Form(true)); join.onClick.AddListener(()=>Form(false));
            submit.onClick.AddListener(Submit); formBack.onClick.AddListener(Entry);
            chooseContent.onClick.AddListener(()=>Select(true)); start.onClick.AddListener(lobby.StartExperiment);
            ready.onClick.AddListener(()=>lobby.SetReady(!readyValue));
            leave.onClick.AddListener(()=>leaveDialog.SetActive(true));
            cancelLeave.onClick.AddListener(()=>leaveDialog.SetActive(false));
            confirmLeave.onClick.AddListener(()=>{leaveDialog.SetActive(false);lobby.Leave();Entry();});
            cancelConnection.onClick.AddListener(()=>{lobby.Leave();Form(hosting);});
            retry.onClick.AddListener(()=>Form(hosting)); errorBack.onClick.AddListener(()=>{lobby.Leave();Entry();});
            selectionBack.onClick.AddListener(()=>{selecting=false;if(lobby.IsHost)Refresh();else Entry();});
            nickname.onSelect.AddListener(_=>Keyboard(nickname)); roomOrAddress.onSelect.AddListener(_=>Keyboard(roomOrAddress));
            nickname.onValueChanged.AddListener(_=>Validate()); roomOrAddress.onValueChanged.AddListener(_=>Validate());
            menu.ContentChosen+=Chosen; lobby.Changed+=Refresh;
            nickname.text=PlayerPrefs.GetString("Tecaverso.DisplayName","Estudante");
            Entry();
        }
        void OnDestroy() { if(lobby!=null)lobby.Changed-=Refresh; if(menu!=null)menu.ContentChosen-=Chosen; }
        static void Keyboard(TMP_InputField field) { if(GlobalNonNativeKeyboard.instance!=null) GlobalNonNativeKeyboard.instance.ShowKeyboard(field); }
        void Page(GameObject page)
        {
            // Periodic lobby snapshots must not dismiss an open confirmation dialog.
            bool changed=page==null || !page.activeSelf;
            // Do not render two coplanar world-space screens on top of each other.
            if(page!=null && originalViews!=null) foreach(var view in originalViews)view.SetActive(false);
            foreach(var view in new[]{entryPage,formPage,lobbyPage,errorPage,connectingPage}) view.SetActive(view==page);
            selectionBack.gameObject.SetActive(page==null);
            if(changed) leaveDialog.SetActive(false);
        }
        void Entry() { selecting=false; Page(entryPage); }
        public void Form(bool host)
        {
            hosting=host; selecting=false;
            formTitle.text=host?"Criar sala local":"Entrar em uma sala";
            fieldLabel.text=host?"NOME DA SALA":"IP DO ANFITRIÃO";
            fieldHint.text=host?"Um nome para identificar sua turma.":"Exemplo: 192.168.1.10 · Porta UDP 7777";
            roomOrAddress.characterLimit=host?40:15; roomOrAddress.text=host?"Laboratório da turma":"";
            submitLabel.text=host?"CRIAR SALA":"CONECTAR";
            formInfo.text=host?"Aprenda em grupo, na mesma rede\n\nAté 8 participantes, incluindo você.\nCompartilhe o IP exibido na sala.\n\nVocê será o anfitrião e controlará o experimento.":"Conecte-se à mesma rede do anfitrião\n\nPeça o endereço IP da sala.\nNão é necessário login ou internet.\n\nRedes de convidados podem impedir a conexão entre dispositivos.";
            Page(formPage); Validate();
        }
        void Validate()
        {
            bool name=Tecaverso.Networking.LanLobbyProtocol.CleanName(nickname.text).Length>0;
            bool address=hosting||Tecaverso.Networking.LanAddress.TryNormalize(roomOrAddress.text,out _);
            submit.interactable=name&&address;
            formError.text=!name?"Informe seu nome.":!address&&roomOrAddress.text.Length>0?"Informe um IPv4 de rede local válido.":"";
        }
        void Submit()
        {
            Validate(); if(!submit.interactable)return;
            PlayerPrefs.SetString("Tecaverso.DisplayName",nickname.text);
            if(hosting)lobby.Host(nickname.text,roomOrAddress.text);else lobby.Join(nickname.text,roomOrAddress.text);
            Refresh();
        }
        void Select(bool forRoom) { selecting=forRoom; Page(null); menu.SetSelectionMode(forRoom); }
        void Chosen(HubCatalog.Content content) { lobby.SelectContent(content); selecting=false; Refresh(); }
        public void Refresh()
        {
            if(lobby.State==LanLobbyController.ConnectionState.Connecting)
            { connectionText.text=hosting?"Criando sua sala local…":"Conectando a "+lobby.Address+"…\nVerifique se os dispositivos estão na mesma rede."; Page(connectingPage); return; }
            if(lobby.State==LanLobbyController.ConnectionState.Failed)
            { selecting=false; Error(lobby.ErrorCode); Page(errorPage); return; }
            if(lobby.State!=LanLobbyController.ConnectionState.Lobby) { Entry();return; }
            if(selecting)return;
            Page(lobbyPage);
            var snapshot=lobby.Snapshot;
            roomTitle.text=snapshot.room;
            roomAddress.text="IP LOCAL  ·  "+lobby.Address+"\nUDP 7777 · "+snapshot.participants.Length+" / 8 participantes";
            participants.text=string.Join("\n\n",snapshot.participants.Select(p=>p.name+(p.id==lobby.LocalId?" (você)":"")+"    ·    "+(p.host?"ANFITRIÃO":p.ready?"PRONTO":"AGUARDANDO")));
            contentTitle.text=string.IsNullOrEmpty(snapshot.contentTitle)?"Selecione um experimento":snapshot.contentTitle;
            chooseContent.gameObject.SetActive(lobby.IsHost); start.gameObject.SetActive(lobby.IsHost); ready.gameObject.SetActive(!lobby.IsHost);
            start.interactable=lobby.CanStart;
            readyValue=snapshot.participants.Any(p=>p.id==lobby.LocalId&&p.ready);
            readyLabel.text=readyValue?"PRONTO · CANCELAR":"ESTOU PRONTO";
            lobbyHint.text=snapshot.experimentActive?"Carregando laboratório…":!snapshot.supportsMultiplayer?"Este conteúdo ainda não oferece modo compartilhado.":lobby.IsHost?lobby.CanStart?"Tudo pronto. Inicie para todos.":"Aguardando todos confirmarem que estão prontos.":"O anfitrião controla o experimento. Confirme quando estiver pronto.";
        }
        void Error(string code)
        {
            errorTitle.text=code=="ROOM_FULL"?"Sala cheia":code=="VERSION_MISMATCH"?"Versões diferentes":code=="HOST_LOST"?"O anfitrião desconectou":"Não foi possível conectar";
            errorBody.text=code=="ROOM_FULL"?"A sala já tem 8 participantes. Aguarde uma vaga ou crie outra sala.":code=="VERSION_MISMATCH"?"Todos precisam usar a mesma versão do Tecaverso Labs. Atualize os dispositivos e tente novamente.":code=="HOST_LOST"?"A sessão foi encerrada. Você voltou ao Hub. Crie outra sala ou reconecte ao anfitrião.":"Confira o IP, a mesma rede Wi-Fi e a permissão de firewall para UDP 7777.\n\nRedes de convidados ou isolamento de clientes podem bloquear a conexão.\nCódigo: "+code;
        }
    }
}
