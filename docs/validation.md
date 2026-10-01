# Validação da preparação LAN — 2026-10-01

Unity 6000.3.25f1, Windows Editor, via MCP.

- Build Windows x64 concluída: `Builds/LanValidation/Tecaverso Labs.exe`, 264,72 MB, 0 erros e 11 avisos. Duração: aproximadamente 6 minutos. Android ainda não foi compilado nem validado em headset.
  Os avisos do relatório dizem respeito ao pacote ServicesCore sem projeto online vinculado, links temporários da simulação XR e desconexões do MCP durante o processamento da build. Não é necessário vincular Unity Cloud para a sessão LAN.

- EditMode: 18/18 casos passaram para IPv4 privado, link-local, loopback e rejeição de entradas inválidas/públicas.
- PlayMode: dois NetworkManagers com UnityTransport direto e DistributedAuthority sem CMB conectaram, desconectaram e reconectaram por UDP loopback.
- Hub: HostLocalConnection retornou true, avatar de rede foi criado, Connected ficou true e a sessão pôde ser encerrada e hospedada novamente.
- Durante o teste do Hub, UnityServices.State permaneceu Uninitialized; transporte efetivo foi Unity.Netcode.Transports.UTP.UnityTransport; nenhum erro foi encontrado no console.
- Não houve teste em dois headsets físicos. Firewall, isolamento Wi-Fi, tracking, conforto e desempenho precisam ser validados no hardware alvo.

Há referências antigas a um script ausente (GUID `38f25601a5df5c4408328447395e13ea`) nos prefabs `XRControllerLeftModel` e `XRControllerRightModel` do template. Elas não foram introduzidas pela migração. Os componentes não foram apagados ou substituídos sem identificar sua origem. A inspeção dos objetos carregados durante a sessão do Hub não encontrou componentes ausentes.
