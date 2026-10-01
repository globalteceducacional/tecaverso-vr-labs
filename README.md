# Tecaverso Labs

Conjunto de laboratórios educacionais em realidade virtual, desenvolvido com Unity 6000.3.25f1, URP, OpenXR e XR Interaction Toolkit. A arquitetura pedagógica será definida na próxima etapa.

## Abrir e executar

1. Instale a versão indicada em `ProjectSettings/ProjectVersion.txt` pelo Unity Hub. Para headsets Android, inclua Android Build Support, SDK, NDK e OpenJDK.
2. Abra este diretório e aguarde a importação dos pacotes.
3. Abra `Assets/_Project/Hub/Scenes/Hub.unity` e entre em Play Mode, com headset ou XR Interaction Simulator.
4. Use o menu de conexão local do template para hospedar uma sala ou informar o IPv4 do host e entrar.

O download inicial dos pacotes exige Internet. A sessão multiplayer em execução usa rede local e não exige Unity Cloud, login, Lobby ou Relay.

## Organização

```text
Assets/
  _Project/
    Core/               # Código compartilhado: Networking, XR, UI e Audio
    Hub/Scenes/         # Entrada do aplicativo e conexão LAN
    Laboratories/
      Physics/ObliqueLaunch/
        Scenes/ Scripts/ Prefabs/ Materials/
      Chemistry/
      Biology/
      Mathematics/
    Shared/             # Assets reutilizados pelos laboratórios
    Tests/              # Testes EditMode e PlayMode
  ThirdParty/VRMultiplayer/ # Template original com adaptações LAN documentadas
  Samples/              # Samples importados do XR Interaction Toolkit e XR Hands
  XR/ e XRI/            # Configurações gerenciadas pelos pacotes
```

Pastas ainda vazias são reservadas para expansão. `Core` compila no assembly `Tecaverso.Core`; o template referencia esse assembly para utilitários LAN. Não existe ainda um modelo de experimentos, avaliação ou progresso pedagógico.

O Hub deriva da BasicScene do template. `ObliqueLaunch` ainda é um cenário inicial, sem experimento implementado. Ambos estão habilitados no build, nessa ordem. A demonstração completa permanece em `Assets/ThirdParty/VRMultiplayer/Scenes/SampleScene.unity`, fora do build. A navegação entre laboratórios será definida junto ao fluxo do produto; incluir a cena no build não cria automaticamente um botão de navegação.

## Multiplayer local

- Um dispositivo seleciona **Host**; os outros informam um dos IPv4 exibidos pelo host e selecionam **Join**.
- Todos precisam estar na mesma rede e usar a mesma versão do aplicativo e os mesmos assets de rede. Porta: **UDP 7777**.
- O host escuta em `0.0.0.0`. Para outro dispositivo, use seu endereço Wi-Fi/Ethernet, como `192.168.1.10`. `127.0.0.1` só serve para duas instâncias na mesma máquina.
- A UI aceita IPv4 privado ou link-local; não aceita nomes DNS, IPv6, IP público ou `IP:porta`. Não há descoberta automática de salas.
- Se houver VPN ou várias interfaces, o host exibe os endereços locais disponíveis. Escolha o da rede compartilhada pelos participantes.
- Autorize o aplicativo no firewall do host para a rede privada e confira se o roteador não isola os clientes Wi-Fi. Nenhuma regra de firewall é alterada automaticamente.
- Se o host encerrar, os participantes perdem a conexão. Não há migração de host entre dispositivos.

O transporte é `UnityTransport` direto, sem Relay e sem CMB. A topologia `DistributedAuthority` do NGO é preservada em modo **DAHost local**, pois os objetos do template usam suas regras de propriedade. O nome da topologia não significa que o aplicativo utiliza o serviço de nuvem Distributed Authority.

Vivox está desativado: voz pela LAN não está implementada. Os pacotes de serviços permanecem como dependências do template, mas os caminhos de autenticação/sessão/voz online estão bloqueados no modo local. Não habilite componentes ou APIs de nuvem ao criar novos laboratórios.

Identidade inicial: empresa `Tecaverso`, produto `Tecaverso Labs`, identificador `com.tecaverso.labs` para Android e Standalone.

## Testes e validação

No Unity Test Runner, execute `Tecaverso.EditMode.Tests` para validar IPv4 e `Tecaverso.PlayMode.Tests` para conexão UDP local, desconexão e reconexão. O teste de transporte usa uma porta temporária livre e não depende da Internet.

Para validar em hardware, rode a mesma build em dois dispositivos, hospede no primeiro e conecte pelo IP no segundo. Verifique avatares, pegar/soltar objetos, desconectar e reconectar, inclusive com o roteador sem acesso à Internet. O teste de loopback não substitui essa validação em headsets e firewall reais.

## Versionamento

Versione `Assets` (incluindo todos os `.meta`), `Packages`, `ProjectSettings`, README e configurações compartilhadas. `Library`, `Temp`, `Logs`, `UserSettings`, soluções geradas e builds ficam ignorados. Mova assets pelo Unity para preservar GUIDs. Samples importados referenciados pelos prefabs fazem parte das dependências e não devem ser removidos indiscriminadamente.

As adaptações no template estão descritas em `docs/lan-template-changes.md`. Revise-as antes de atualizar o template.
