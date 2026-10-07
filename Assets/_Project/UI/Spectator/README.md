# Tela de espera do espectador — 07/10/2026

Componente de apresentação PC/Web, isolado do fluxo VR. Não é ainda um cliente multiplayer desktop/web.

## Abrir e configurar

- Abra `SpectatorWaitingPreview.unity` para visualizar a tela isoladamente (não incluída nas cenas de build).
- Edite `SpectatorWaiting.prefab`, componente `SpectatorWaitingView`.
- `Preview Clip`: vídeo local para PC. `Preview Url`: URL do vídeo, prioritária sobre o clip e necessária no Web.
- Nenhum vídeo do cenário foi fornecido. O fundo estático identifica explicitamente o espaço reservado.
- No Web, servir vídeo por HTTP(S) compatível com a página; preferir mesma origem e verificar codec, MIME, CORS e políticas do navegador na homologação. Não usar caminhos locais do Windows.
- Vídeo em loop e sem áudio. Proporção preservada, timeout de preparação de 15 s e fundo estático em caso de falha. `RetryPreview()` permite tentativa explícita; não há repetição infinita de downloads.

## Contrato com o futuro cliente espectador

O dono da sessão chama `SetState` com fatos confirmados: `Unconnected`, `Connecting`, `WaitingForHost`, `SessionAvailable`, `ConnectionLost`, `Live`.
`Unconnected` significa somente que não existe conexão local confirmada, NÃO que não existe sala na rede.
`Live` oculta a página por CanvasGroup e para a reprodução. Perda de conexão reapresenta a tela, sem reconectar ou explorar automaticamente.

Conectar callbacks em `connectRequested`, `watchRequested`, `exploreRequested` e então liberar apenas capacidades implementadas em `SetCapabilities`. As ações começam ocultas; a cena de prévia não inventa salas nem simula uma conexão bem-sucedida.

Pendências separadas: cliente sem XR, papéis de espectador, transporte WebSocket para Web, seleção/descoberta de sala, câmeras e HUD ao vivo, exploração desktop e perfis de build. Não ativar automaticamente esta UI sobre o Hub VR; integrá-la à entrada desktop quando existir.

Layout: uGUI Screen Space Overlay, Montserrat, tokens Tecaverso, aviso no canto inferior direito, layouts vertical/horizontal e CanvasGroups. Visualização alvo landscape 16:9/16:10; mobile portrait não homologado.

## Validação desta etapa

- Editor Unity 6000.3.25f1: scripts compilados sem erros; dois testes EditMode de visibilidade/capacidades aprovados.
- Captura 1280 × 720 conferida: fundo reservado e aviso sem sobreposição.
- Sem vídeo fornecido: decodificação, loop real e comportamento de autoplay em navegadores ainda não foram testados.
- Nenhum build gerado; nenhuma alteração intencional às cenas VR, configurações XR ou transporte LAN.
