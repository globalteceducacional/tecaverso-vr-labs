# Tecaverso Labs

Conjunto de laboratórios educacionais em realidade virtual, desenvolvido com Unity 6000.3.25f1, URP, OpenXR e XR Interaction Toolkit. O primeiro laboratório, **Movimento Oblíquo**, tem MVP implementado. O modelo pedagógico compartilhado — experimentos, avaliação e progresso do aluno — ainda será definido na próxima etapa.

## Abrir e executar

1. Instale a versão indicada em `ProjectSettings/ProjectVersion.txt` pelo Unity Hub. Para headsets Android, inclua Android Build Support, SDK, NDK e OpenJDK.
2. Abra este diretório e aguarde a importação dos pacotes.
3. Abra `Assets/_Project/Hub/Scenes/Hub.unity` e entre em Play Mode, com headset ou XR Interaction Simulator.
4. Use o menu de conexão local do template para hospedar uma sala ou informar o IPv4 do host e entrar.
5. Para o laboratório, abra `Assets/_Project/Laboratories/Physics/ObliqueLaunch/Scenes/ObliqueLaunch.unity` e entre em Play Mode. Ajuste os parâmetros no painel espacial e use **DISPARAR**.

O download inicial dos pacotes exige Internet. A sessão multiplayer em execução usa rede local e não exige Unity Cloud, login, Lobby ou Relay.

## Organização

```text
Assets/
  _Project/
    Core/               # Código compartilhado. Hoje só Networking (`LanAddress`);
                        # XR, UI e Audio estão reservados
    Editor/             # Ferramentas de Editor (gerador da cena do laboratório)
    Hub/Scenes/         # Entrada do aplicativo e conexão LAN
    Laboratories/
      Physics/ObliqueLaunch/
        Scenes/ Scripts/ Materials/   # Prefabs/ reservada, ainda vazia
      Chemistry/        # reservadas
      Biology/
      Mathematics/
    Shared/             # Assets reutilizados pelos laboratórios
    Tests/              # Testes EditMode e PlayMode
  ThirdParty/VRMultiplayer/ # Template original com adaptações LAN documentadas
  Samples/              # Samples importados do XR Interaction Toolkit e XR Hands
  XR/ e XRI/            # Configurações gerenciadas pelos pacotes
```

Pastas ainda vazias são reservadas para expansão. `Core` compila no assembly `Tecaverso.Core`; o template referencia esse assembly para utilitários LAN. Não existe ainda um modelo de experimentos, avaliação ou progresso pedagógico.

O Hub deriva da BasicScene do template. As duas cenas habilitadas no build são `Hub` (índice 0) e `ObliqueLaunch` (índice 1), nessa ordem. A demonstração completa permanece em `Assets/ThirdParty/VRMultiplayer/Scenes/SampleScene.unity`, fora do build. A cena do laboratório está no build, mas o Hub **não tem botão que a carregue**: a navegação entre laboratórios será definida junto ao fluxo do produto.

## Laboratório de Movimento Oblíquo

O laboratório segue separação do tipo MVP, com a física em C# independente de `GameObject`:

- `ObliqueLaunchModel` — `LaunchParameters`, `FlightSample` e `ProjectileKinematics` (equações do movimento oblíquo, sem dependência da cena).
- `ObliqueLaunchSimulation` — estado e tempo, na máquina `Idle`, `Running`, `Paused` e `Complete`.
- Views — `ProjectileView`, `VectorArrowView`, `ProjectileTrajectoryLine`, `MeasurementRulers` e `TrajectorySnapshotPool`.
- `ObliqueLaunchPanel` — view passiva da UI; `ObliqueLaunchLab` — fachada que conecta tudo por eventos.

O aluno ajusta ângulo, velocidade inicial, altura da base, massa e gravidade; ao disparar, acompanha o projétil com os vetores Vresult, Vx e Vy, snapshots temporais a cada 0,5 s, arco do ângulo, réguas de alcance (`Rtotal`) e altura máxima (`Ymax`), e até cinco trajetórias anteriores com esmaecimento e marcador de ápice.

A massa é aplicada ao `Rigidbody` representativo e exibida no painel, mas **não altera a trajetória** ideal: sem resistência do ar, a massa não muda o alcance. O comportamento é intencional e reforça o conceito físico correto.

### A cena é gerada por código

`Assets/_Project/Editor/ObliqueLaunchSceneBuilder.cs` constrói a cena inteira pelo menu **`Tecaverso/Build Oblique Launch MVP`**. O gerador destrói o objeto raiz `Oblique Launch Lab` e o reconstrói, e também reposiciona o XR Origin e reorienta o painel para o observador. Portanto **ajustes manuais feitos dentro do laboratório são perdidos no próximo build**: altere o gerador e regenere, ou ajuste o gerador antes de posicionar a UI à mão.

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

No Unity Test Runner, execute `Tecaverso.EditMode.Tests` para validar IPv4 e `Tecaverso.PlayMode.Tests` para conexão UDP local, desconexão e reconexão. São 19 casos: 18 de `LanAddress` (aceitação de IPv4 privado, link-local e loopback; rejeição de entradas inválidas, não locais e `IP:porta`) e 1 de transporte com dois `NetworkManager`. O teste de transporte usa uma porta temporária livre e não depende da Internet.

A rede está coberta; **a física do laboratório ainda não tem teste automatizado**. `ProjectileKinematics` é o alvo mais barato para isso, por ser C# puro sem dependência de `GameObject`.

A plataforma ativa do projeto é **Android**, mas até agora só existe build validada de **Windows x64** (`Builds/LanValidation/`, 264,72 MB, 0 erros). Nenhuma build Android foi gerada nem testada em headset. O histórico da validação de rede está em `docs/validation.md` e antecede o laboratório.

Para validar em hardware, rode a mesma build em dois dispositivos, hospede no primeiro e conecte pelo IP no segundo. Verifique avatares, pegar/soltar objetos, desconectar e reconectar, inclusive com o roteador sem acesso à Internet. O teste de loopback não substitui essa validação em headsets e firewall reais.

## Versionamento

Versione `Assets` (incluindo todos os `.meta`), `Packages`, `ProjectSettings`, README e configurações compartilhadas. `Library`, `Temp`, `Logs`, `UserSettings`, soluções geradas e builds ficam ignorados. Mova assets pelo Unity para preservar GUIDs. Samples importados referenciados pelos prefabs fazem parte das dependências e não devem ser removidos indiscriminadamente.

`ObliqueLaunch.unity` é gerado por código e tem cerca de 1,5 MB, porque as 99 setas de vetor carregam cada uma sua própria cópia da malha do cone serializada na cena, em vez de compartilhar um asset. Evite editar essa cena à mão e considere extrair a malha para um asset ao mexer no gerador.

As adaptações no template estão descritas em `docs/lan-template-changes.md`. Revise-as antes de atualizar o template. O padrão de código do projeto está em `docs/development-guidelines.md` e o histórico de validação em `docs/validation.md`.
