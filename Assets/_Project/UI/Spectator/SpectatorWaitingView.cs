using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Tecaverso.UI.Spectator
{
    public enum SpectatorWaitingState { Unconnected, Connecting, WaitingForHost, SessionAvailable, ConnectionLost, Live }

    /// <summary>Presentation only. The session owner supplies facts and wires supported actions.</summary>
    public sealed class SpectatorWaitingView : MonoBehaviour
    {
        [Header("Presentation")]
        [SerializeField] GameObject page;
        [SerializeField] TMP_Text status, detail, mediaLabel;
        [SerializeField] Button connect, watch, explore;
        [SerializeField] VideoPlayer player;
        [SerializeField] RawImage videoSurface;
        [SerializeField] AspectRatioFitter videoAspect;
        [Header("Optional prerecorded preview")]
        [Tooltip("PC: VideoClip or URL. Web: supply a browser-accessible URL (same origin recommended).")]
        [SerializeField] VideoClip previewClip;
        [SerializeField] string previewUrl;
        [SerializeField, Min(1)] float preparationTimeout = 15;
        [Header("Wire actions before enabling their capabilities")]
        public UnityEvent connectRequested = new();
        public UnityEvent watchRequested = new();
        public UnityEvent exploreRequested = new();

        public SpectatorWaitingState State { get; private set; }
        public bool IsShowing => page != null && UIVisibility.IsVisible(page);
        bool canConnect, canWatch, canExplore, preparing, failed;
        float deadline;

        public void Bind(GameObject root, TMP_Text heading, TMP_Text body, TMP_Text media,
            Button connectButton, Button watchButton, Button exploreButton,
            VideoPlayer video, RawImage surface, AspectRatioFitter aspect)
        {
            page = root; status = heading; detail = body; mediaLabel = media;
            connect = connectButton; watch = watchButton; explore = exploreButton;
            player = video; videoSurface = surface; videoAspect = aspect;
        }

        void Awake()
        {
            connect.onClick.AddListener(RequestConnect);
            watch.onClick.AddListener(RequestWatch);
            explore.onClick.AddListener(RequestExplore);
            player.playOnAwake = false;
            player.isLooping = true;
            player.audioOutputMode = VideoAudioOutputMode.None;
            player.renderMode = VideoRenderMode.APIOnly;
            player.prepareCompleted += Prepared;
            player.errorReceived += VideoFailed;
            SetState(SpectatorWaitingState.Unconnected);
        }

        void OnEnable() { if (page != null && IsShowing) StartPreview(); }
        void OnDisable() => StopPreview();
        void OnDestroy()
        {
            if (player == null) return;
            player.prepareCompleted -= Prepared;
            player.errorReceived -= VideoFailed;
        }
        void Update()
        {
            if (preparing && Time.realtimeSinceStartup >= deadline)
                VideoFailed(player, "Preparation timed out");
        }

        public void SetCapabilities(bool connectionAvailable, bool watchingAvailable, bool explorationAvailable)
        {
            canConnect = connectionAvailable; canWatch = watchingAvailable; canExplore = explorationAvailable;
            RefreshActions();
        }

        public void SetState(SpectatorWaitingState state)
        {
            bool wasShowing = IsShowing;
            State = state;
            UIVisibility.Set(page, state != SpectatorWaitingState.Live);
            switch (state)
            {
                case SpectatorWaitingState.Unconnected:
                    status.text = "Nenhuma sala conectada";
                    detail.text = "Conheça o Tecaverso enquanto aguarda. A busca automática de salas ainda não está disponível.";
                    break;
                case SpectatorWaitingState.Connecting:
                    status.text = "Conectando à sala…";
                    detail.text = "Aguarde a confirmação da conexão. A imagem de fundo não é uma sessão ao vivo.";
                    break;
                case SpectatorWaitingState.WaitingForHost:
                    status.text = "Aguardando o anfitrião";
                    detail.text = "Você está na sala. O experimento ainda não foi iniciado.";
                    break;
                case SpectatorWaitingState.SessionAvailable:
                    status.text = "Sessão disponível";
                    detail.text = "Selecione Assistir para acompanhar o experimento.";
                    break;
                case SpectatorWaitingState.ConnectionLost:
                    status.text = "A conexão com a sala foi encerrada";
                    detail.text = "Você não está acompanhando uma sessão ao vivo. Conecte novamente quando disponível.";
                    break;
            }
            RefreshActions();
            if (!IsShowing) StopPreview();
            else if (!wasShowing && isActiveAndEnabled) StartPreview();
        }

        void RefreshActions()
        {
            bool busy = State == SpectatorWaitingState.Connecting;
            UIVisibility.Set(connect.gameObject, canConnect, true);
            UIVisibility.Set(watch.gameObject, canWatch && State == SpectatorWaitingState.SessionAvailable, true);
            UIVisibility.Set(explore.gameObject, canExplore, true);
            connect.interactable = canConnect && !busy;
            watch.interactable = canWatch && !busy;
            explore.interactable = canExplore && !busy;
        }
        void RequestConnect() { if (canConnect && State != SpectatorWaitingState.Connecting) connectRequested.Invoke(); }
        void RequestWatch() { if (canWatch && State == SpectatorWaitingState.SessionAvailable) watchRequested.Invoke(); }
        void RequestExplore() { if (canExplore && State != SpectatorWaitingState.Connecting) exploreRequested.Invoke(); }

        public void RetryPreview() { failed = false; StartPreview(); }
        void StartPreview()
        {
            if (!Application.isPlaying || player == null || !IsShowing || preparing || player.isPlaying || failed) return;
            videoSurface.enabled = false;
            if (!string.IsNullOrWhiteSpace(previewUrl))
            { player.source = VideoSource.Url; player.url = previewUrl; }
            else if (previewClip != null && Application.platform != RuntimePlatform.WebGLPlayer)
            { player.source = VideoSource.VideoClip; player.clip = previewClip; }
            else
            {
                mediaLabel.text = "ESPAÇO PARA PRÉVIA GRAVADA · Vídeo ainda não configurado";
                return;
            }
            mediaLabel.text = "PRÉVIA GRAVADA · Carregando vídeo…";
            preparing = true;
            deadline = Time.realtimeSinceStartup + preparationTimeout;
            player.Prepare();
        }
        void Prepared(VideoPlayer source)
        {
            if (!preparing || !IsShowing || !isActiveAndEnabled) return;
            preparing = false;
            videoSurface.texture = source.texture;
            videoAspect.aspectRatio = source.height > 0 ? (float)source.width / source.height : 16f / 9f;
            videoSurface.enabled = true;
            mediaLabel.text = "PRÉVIA GRAVADA · Não é uma sessão ao vivo";
            source.Play();
        }
        void VideoFailed(VideoPlayer source, string reason)
        {
            failed = true;
            StopPreview();
            mediaLabel.text = "Prévia indisponível · A conexão com a sala não depende do vídeo";
        }
        void StopPreview()
        {
            preparing = false;
            if (player != null) player.Stop();
            if (videoSurface != null) { videoSurface.enabled = false; videoSurface.texture = null; }
        }
    }
}
