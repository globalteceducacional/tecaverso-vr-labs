# Validação da preparação LAN — 2026-10-01

## Atualização — entrada e experimento LAN — 2026-10-06

- Nenhum build foi executado nesta etapa. Verificações limitadas ao Unity Editor.
- EditMode: 2/2 testes de `LanLobbyProtocolTests` passaram (nome, versão, capacidade e limites de texto).
- PlayMode: `LanLobbyExperimentTests.DirectLanReplicatesLobbyLateJoinPauseAndReset` passou em UDP loopback com dois NetworkManagers e duas instâncias do laboratório. Cobertura: aprovação/conexão, participantes, pronto, entrada tardia reconstruindo cinco disparos, controle bloqueado no observador, pausa, retomada e reset.
- A primeira execução desse teste identificou um erro na validação do tamanho da mensagem de pronto; corrigido para descontar a posição/cabeçalho do leitor e confirmado pela reexecução.
- Verificação do Hub no Editor: criação de sala, início do laboratório, uma câmera ativa, cinco registros após seis disparos, limpeza no reset e retorno à sala mantendo a sessão.
- Telas adicionadas ao Canvas existente; cenário, posição da UI e transformações salvas do rig/canhão preservados. A UI lateral antiga foi desativada, não apagada.
- Conferência final: navegação individual, rejeição de IPv4 público, criação de sala e confirmação de saída preservada durante atualização do lobby. Canvas em `(0, 1.95, 3.2)`, rotação `(0, 0, 0)`. Editor deixado no Hub, fora de Play Mode e sem sessão ativa.
- Sem erros de compilação nos checks. O console final continha um aviso/erro do Package Manager (`Operation cancelled`), sem stack trace do código do experimento; referências ausentes herdadas do template continuam fora desta alteração.
- Pendente em hardware: ciclo completo em dois headsets, teclado XR/tracking, perda real de Wi-Fi/host, firewall, legibilidade e desempenho. Oito participantes é limite configurado, não capacidade certificada.

## Registro anterior

### Refatoração de UI — 06/10/2026

- Guia `UI_GUIDELINES.md` e encaminhamento em `AGENTS.md` criados.
- Migrados o prefab HubUI, Hub.unity e os painéis de ObliqueLaunch.unity: grids de cards, linhas/colunas de ações e parâmetros, rodapé separado, modal centralizado e CanvasGroups de páginas/abas.
- Transforms externos dos Canvases comparados antes/depois pela migração e preservados.
- Conferidos visualmente entrada, conteúdo, parâmetros e análise. Nove controles visíveis no HUB 2 sem interseções de suas áreas; navegação e bloqueio/restauração do fundo pelo modal exercitados.
- Dois testes EditMode de visibilidade/recolhimento e uma regressão PlayMode LAN passaram. Testes não equivalem à validação de ponteiro/tracking em headset.
- Tween de botão limitado ao filho Visual (até 1,02×), preservando o RectTransform do layout e o alvo de interação.
- As capturas compostas do MCP produziram erro interno de PlayerLoop em ScreenshotUtility; a captura pela câmera explícita funcionou. Não foi identificado stack trace desse erro no código da aplicação.
- Caches dinâmicos de fontes gerados durante a prévia foram limpos, mantendo os assets originais. Nenhum build.

Unity 6000.3.25f1, Windows Editor, via MCP.

- Build Windows x64 concluída: `Builds/LanValidation/Tecaverso Labs.exe`, 264,72 MB, 0 erros e 11 avisos. Duração: aproximadamente 6 minutos. Android ainda não foi compilado nem validado em headset.
  Os avisos do relatório dizem respeito ao pacote ServicesCore sem projeto online vinculado, links temporários da simulação XR e desconexões do MCP durante o processamento da build. Não é necessário vincular Unity Cloud para a sessão LAN.

- EditMode: 18/18 casos passaram para IPv4 privado, link-local, loopback e rejeição de entradas inválidas/públicas.
- PlayMode: dois NetworkManagers com UnityTransport direto e DistributedAuthority sem CMB conectaram, desconectaram e reconectaram por UDP loopback.
- Hub: HostLocalConnection retornou true, avatar de rede foi criado, Connected ficou true e a sessão pôde ser encerrada e hospedada novamente.
- Durante o teste do Hub, UnityServices.State permaneceu Uninitialized; transporte efetivo foi Unity.Netcode.Transports.UTP.UnityTransport; nenhum erro foi encontrado no console.
- Não houve teste em dois headsets físicos. Firewall, isolamento Wi-Fi, tracking, conforto e desempenho precisam ser validados no hardware alvo.

Há referências antigas a um script ausente (GUID `38f25601a5df5c4408328447395e13ea`) nos prefabs `XRControllerLeftModel` e `XRControllerRightModel` do template. Elas não foram introduzidas pela migração. Os componentes não foram apagados ou substituídos sem identificar sua origem. A inspeção dos objetos carregados durante a sessão do Hub não encontrou componentes ausentes.
