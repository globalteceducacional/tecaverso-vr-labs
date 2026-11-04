# Registros de validação — Tecaverso Labs

Este documento reúne evidências históricas e aponta para os registros específicos de cada laboratório. A revisão documental de 03/11/2026 conferiu catálogo, cenas habilitadas, links e regras de mídia; não reexecutou testes nem gerou builds. Aprovação no Editor não certifica conforto ou desempenho em headset.

## Matemática — registros consolidados em 04/11/2026

| Laboratório | Evidência registrada | Limites |
|---|---|---|
| [Espaço cartesiano](../Assets/_Project/Laboratories/Mathematics/CartesianSpace/README.md) | 11 testes EditMode; ferramentas, coordenadas, prévias, histórico e navegação em Play Mode; interator XR sintético | Headset, alcance, manipulação pelo analógico e desempenho pendentes; sem LAN |
| [Formas geométricas](../Assets/_Project/Laboratories/Mathematics/GeometricShapes/README.md) | 20 testes EditMode; 11 peças, três toggles, saída ao Hub, pausa por hover e retomada; superfícies sem orientação invertida e textos sem overflow | Headset pendente; sem LAN. Avisos de configuração OpenXR na inicialização, sem scripts ausentes nos GameObjects da sala |
| [Geometria plana](../Assets/_Project/Laboratories/Mathematics/PlanarGeometry/README.md) | 15 testes EditMode; 13 figuras e três toggles independentes; toggles e retorno acionados por raio XR sintético; entrada pela seleção do Hub e reentrada com um rig; sem overflow | Headset pendente; sem LAN. Implementação e verificações posteriores à revisão documental inicial, em 03/11/2026 |
| [Frações e proporções](../Assets/_Project/Laboratories/Mathematics/Fractions/README.md) | 17 testes EditMode em 04/11/2026; Play Mode com pega/reposição, sockets, equivalência, receita, capacidade, desfazer e confirmações; UI por raio XR, 24 peças por linha e Hub com um rig | Headset pendente; sem LAN ou build |
| [Probabilidade e estatística](../Assets/_Project/Laboratories/Mathematics/Probability/README.md) | 11 testes EditMode em 04/11/2026; Play Mode: lote de 1.000, pausa, exclusividade, cancelamento, urna esgotada e reset; toggle e confirmação por raio XR sintético; alavanca por interator temporário | Headset pendente; sem LAN ou build |
| [Funções e gráficos](../Assets/_Project/Laboratories/Mathematics/Functions/README.md) | 16 testes EditMode em 04/11/2026; casos degenerados, recorte/tangência e estado; Play Mode com famílias, referência, restauração, confirmações; UI por raio XR sintético e pega/giro por interator temporário; Hub com um rig | Headset pendente; sem LAN ou build. A validação XR identificou e corrigiu a mudança de referencial do ponto durante a pega |

O GDD vigente de Formas geométricas descreve a galeria simplificada. Verificações antigas de edição A/B ou planificação não são critérios de aceite da sala atual.

## Física — registros por laboratório

- Lançamento oblíquo e LAN: registros históricos abaixo.
- [Plano inclinado](../Assets/_Project/Laboratories/Physics/InclinedPlane/README.md).
- [Gravitação](../Assets/_Project/Laboratories/Physics/Gravitation/README.md).
- [Associação de resistores](../Assets/_Project/Laboratories/Physics/ResistorCircuits/README.md).
- [Termologia](../Assets/_Project/Laboratories/Physics/Thermology/README.md).
- [Ondulatória](../Assets/_Project/Laboratories/Physics/Waves/README.md).

## Capturas e integridade Git — 03/11/2026

Na preparação do commit `85d450c`, `git lfs fsck --pointers` e `git lfs fsck --objects` passaram. São verificações do armazenamento local, sem push ou comprovação dos objetos remotos.

As regras de mídia foram conferidas com `git check-ignore`, incluindo pastas de capturas, exportações, áudios de demonstração e `.meta` de pastas sob `Assets/`. Ícones, texturas e materiais de runtime permanecem versionáveis. Imagens antigas já rastreadas não foram retiradas do índice. Destinos e procedimento em [mídias geradas](development-guidelines.md#midias-geradas).

## Histórico LAN — iniciado em 01/10/2026

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
