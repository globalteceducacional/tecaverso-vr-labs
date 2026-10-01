# Adaptações LAN do template VR Multiplayer

Base mantida em `Assets/ThirdParty/VRMultiplayer`. Assets movidos preservam seus GUIDs e `.meta`.

- `SessionManager`: política LocalOnly, transporte UDP direto, CMB desativado, entradas de sessão online bloqueadas e shutdown local.
- `XRINetworkGameManager`: caminhos dos botões antigos direcionados à LAN; identidade local; status Connected após spawn; host em todas as interfaces; porta 7777; timeout de 10 segundos; endereços obtidos das interfaces, sem o antigo probe para 8.8.8.8. Importação de UnityEditor protegida para builds.
- `AuthenticationManager`: curto-circuito local sem inicializar Unity Services.
- `VoiceChatManager`: desativação antes de assinar eventos de autenticação; callbacks de volume/voz não acessam Vivox no modo local.
- `LobbyUI`: IPv4 validado sincronamente, sem DNS; bloqueio de Join com entrada inválida; painel de conexão concluído por evento real e mensagens com IP/porta.
- Prefabs dos managers: sessão local e voz desativada; endereço de escuta aberto às interfaces locais.
- `VRMP.asmdef`: referência a `Tecaverso.Core`, que contém `LanAddress`.

O uso de DAHost local é intencional: o template depende de mudanças de propriedade entre jogadores, `CurrentSessionOwner` e RPCs para o proprietário. Trocar apenas o enum para ClientServer quebraria essas interações. NGO permite DAHost com UnityTransport direto sem CMB; o teste PlayMode verifica o handshake e a reconexão nesse modo.

Pacotes de Authentication, Multiplayer Services e Vivox continuam instalados por compatibilidade de compilação com os tipos públicos do template. Sua remoção exigiria uma refatoração mais ampla das interfaces do template, não necessária para operar offline.

Arquitetura educacional, experiência de cada laboratório, navegação do Hub, descoberta automática e voz local permanecem para etapas específicas de produto.
