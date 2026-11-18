# Short GDD — Ecossistemas e relações

**Status:** cena implementada em Unity; conforto e alcance em headset pendentes. **Disciplina:** Biologia. **Modo:** individual offline. **Público:** Ensino Médio. **Data:** 17/11/2026. Uma das **seis** salas propostas, conforme [índice](gdd-biologia.md).

## Objetivo e experiência

Construir uma cadeia alimentar simples e distinguir fluxo de energia de ciclagem de matéria. O aluno manipula fichas com representações de organismos e observa setas coerentes com a montagem.

## Acervo, objetos e sala

**Cenas antigas consolidadas:** EcossistemaInterativo.

**Acervo e autoria.** A cena antiga continha a estrutura inicial de ambiente. A sala usa o rig XR, a iluminação e a arquitetura da versão atual. A pradaria elevada, as três fichas e os fungos sobre madeira morta foram modelados para este laboratório. O [modelador dos espécimes](../Assets/_Project/Editor/EcosystemsSpecimens.cs) gera malhas locais com cores por vértice. As representações indicam grupos de organismos, em escalas ampliadas independentes.

**Catálogo fechado desta entrega:** Um cenário terrestre didático; três fichas com relevo/silhueta e nome (capim, gafanhoto, sapo), três sockets de papéis tróficos e um módulo de decomposição fixo. Não usar Salmonella como exemplo obrigatório de decompositor. Esta versão não é simulador populacional ou editor de teias livres.

| Conjunto de objetos | Papel | Interação | Limite |
|---|---|---|---|
| Modelos e peças descritos no catálogo | Fenômeno principal | Seleção direta, pega/rotação ou encaixe conforme o fluxo abaixo | Não disponibilizar partes sem identificação revisada |
| Suportes e sockets da atividade | Ancoragem, retorno e alcance | Controles 3D e filtros de encaixe | Preservar referência espacial e evitar seleção concorrente |
| Consulta e navegação | Apoio opcional | Apontar/tocar e fechar; voltar ao Hub | Uma consulta simultânea; sem HUD na câmera |

**Disposição.** Diorama horizontal com superfície a 1,08 m, largura de 2,67 m e acesso frontal e lateral. Na borda próxima ficam as três fichas em ordem embaralhada. No campo, os três encaixes seguem da esquerda para a direita. A luz solar se encontra à esquerda e o tronco com fungos à direita. A alavanca fica na borda direita. O estado da montagem e as ações de restaurar e voltar estão na frente inclinada da bancada. A consulta e a confirmação compartilham o suporte à direita.

**Controles escolhidos pela tarefa:** Três fichas XR e três encaixes; toggle Exibir fluxo; Restaurar diorama; consulta tocando o módulo de matéria/energia. Sem sliders de população, chuva ou temperatura.

## Inventário de interfaces, rótulos e controles 3D

**HUD na câmera: nenhum.** Uma ficha opcional de consulta. Os nomes ficam nos suportes e junto à estrutura selecionada; nenhum cartão extenso acompanha uma peça na mão. A sala não repete o formulário de Física.

| ID | Tipo e localização | Finalidade e ação | Inicial | Abre/fecha | Simultâneo |
|---|---|---|---|---|---|
| R1 | Nomes nas três posições da bandeja, papéis nos encaixes e legenda acima do campo | Identificar as fichas, seus destinos e o significado das setas | Visíveis | Permanentes | C1, E1, S1 e eventual Q1 ou X1 |
| C1 | Fichas XR na bandeja, encaixes no campo, alavanca na borda direita e Restaurar na frente da bancada | Pegar, encaixar, retirar, exibir fluxo e restaurar | Três fichas disponíveis, encaixes vazios e alavanca desligada | Mão ou raio XR nas peças, alavanca da Unity e botão Restaurar | R1, E1, Q1 e S1. X1 suspende a interação |
| Q1 | Ficha clara fixa à direita da bancada | Explicar a decomposição, o retorno de nutrientes e a dissipação de energia | Oculta | Selecionar os fungos sobre o tronco para abrir. Fechar encerra a leitura | Objetos e C1 visíveis. X1 assume o espaço da consulta |
| E1 | Uma linha no suporte, sem painel flutuante | Confirmar ação ou explicar recusa/limite; não depender apenas de cor | Instrução inicial curta | Atualiza por evento; erro persiste até correção ou outra ação pertinente | R1, C1 e Q1 |
| X1 | Confirmação no espaço da consulta | Cancelar, restaurar ou voltar ao Hub quando houver estado a descartar | Oculta | Restaurar ou Voltar abre a decisão. Cancelar preserva a montagem e a consulta que estava aberta | Objetos ao fundo visíveis. C1, Q1 e S1 aguardam a decisão |
| S1 | Voltar ao Hub na frente da bancada | Retornar ao Hub. Com montagem ou fluxo ativo, X1 permite escolher entre descartar e continuar | Visível | Permanente | Todos, exceto bloqueio por X1 |

O cabeçalho de revisão e a seção “Como isso funciona em RV?” do HTML não são interfaces a implementar na sala. Motion reduzido é preferência geral do produto, exposta no menu padrão; não exige outro painel permanente de Biologia.

## Sequência e casos sem resultado

1. Entrar, reconhecer as peças e a primeira ação em E1.
2. Encaixar capim, gafanhoto e sapo na ordem; ativar fluxo; seguir energia solar → produtor → consumidor, com dissipação de calor. Consultar decompositores e observar retorno de nutrientes ao ambiente. Remontar a cadeia.
3. Fechar a consulta, comparar outro caso e retornar ao estado de exploração.
4. Restaurar quando aplicável: X1 pede confirmação; cancelar preserva os objetos; S1 retorna ao Hub.

**Recusas e recuperação.** Ao aproximar um sapo do produtor ou tentar colocar o herbívoro primeiro, o suporte explica qual papel falta e a ficha continua na mão. Soltar fora de um encaixe válido devolve a ficha à bandeja. Retirar o produtor interrompe todos os percursos. As peças já colocadas nos demais suportes conservam suas posições para a remontagem. A confirmação só abre depois que o aluno solta a peça.

## Modelo científico e limites

As setas vão do recurso alimentar ao consumidor. Energia não circula de volta ao Sol; matéria retorna ao ambiente por processos de decomposição. Sem lei universal de 10%, sem mortalidade automática e sem previsão ecológica real. Todas as imagens e textos locais.

Referência de revisão: [OpenStax — conteúdo relacionado](https://openstax.org/books/biology-2e/pages/46-2-energy-flow-through-ecosystems). Os modelos artísticos precisam de validação semântica; nomes de arquivos não são autoridade taxonômica/anatômica. Reutilização técnica não substitui revisão do conteúdo.

**Leitura dos percursos.** Com a alavanca ligada, luz → capim, capim → gafanhoto e gafanhoto → sapo aparecem à medida que as ligações são completadas. Cada nível conectado possui uma derivação curta em salmão que indica dissipação de calor. Com a cadeia completa, trilhas turquesa unem resíduos dos três níveis à madeira em decomposição e outro trajeto mostra nutrientes do solo voltando ao produtor. Um shader desloca o brilho sobre as fitas aderentes à superfície. As pontas das setas indicam a direção mesmo com motion reduzido.

## Interação, feedback e ergonomia

Pega por proximidade e raio XR conforme a peça; nenhuma ação essencial exige hand tracking. Bases são ancoradas e não são carregadas pela sala. Partes manipuláveis têm suporte/socket de retorno e área de pega distinta do botão de consulta. Não adicionar Rigidbody a cada submalha automaticamente. Alvos amplos, colliders simples e filtros evitam selecionar a base em vez da peça.

Foco: contorno em 0,12–0,16 s. Ação: confirmação de até 0,22 s no filho visual. Estado selecionado persiste; collider, câmera e referência científica permanecem estáveis. Encaixe aceita uma vez por gesto; recusa mantém a peça e explica correção. Reutilizar `Shared/Interaction`, preferência `MotionPreferences.Reduced` e controles oficiais Unity em `Shared/XR/Controls`, sem duplicar tweens. Som curto e haptic podem complementar, nunca substituir informação visual. Não inventar partículas para ornamentar uma galeria: usar somente para representar um processo cadastrado, com legenda e pausa quando temporal.

Modelos têm alcance e altura ajustáveis na autoria; o primeiro protótipo mira controles de pega entre 0,8 e 1,25 m, com consulta a cerca de 2–3 m. Validar sentado/em pé em headset antes de homologar. O controle de motion é a preferência geral da aplicação; no HTML está no cabeçalho de revisão.

## Prévias e estados

**Proposta, não cena Unity implementada.** O [protótipo HTML](prototypes/biologia/index.html?lab=ecossistemas) usa diagramas esquemáticos. Header, navegação de revisão e comandos tracejados de simular pega/giro são externos ao laboratório; não autorizam novos painéis na Unity. Os controles sólidos representam controles da sala. A ficha aparece somente por seleção. O estado `detail` é exemplo de consulta; `action` exemplifica manipulação/resultado; `error` é exemplo visual do aviso, não execução automática de uma falha real.

![Disposição espacial proposta](../GeneratedMedia/GDD/biologia/layout.png)

Visão espacial comum: entrada, suporte elevado, faixa de circulação e consulta lateral. A disposição específica é descrita acima.

![Entrada](../GeneratedMedia/GDD/biologia/ecossistemas-entry.png)

Entrada: R1, C1 e S1 visíveis; Q1 e X1 ocultos. Os objetos são a ação principal.

![Interação e resultado](../GeneratedMedia/GDD/biologia/ecossistemas-action.png)

Manipulação/resultado: C1 altera os objetos; E1 confirma a ação. Fechar Q1 mantém o experimento. Não há interface sobre a câmera.

![Consulta opcional](../GeneratedMedia/GDD/biologia/ecossistemas-detail.png)

Consulta: selecionar objeto ou símbolo da base abre Q1 lateral; Fechar remove o bloqueio visual sem resetar estado.

![Recusa ou limite](../GeneratedMedia/GDD/biologia/ecossistemas-error.png)

Impedimento: E1 descreve a recuperação sem modal. O estado válido anterior é preservado.

![Restauração](../GeneratedMedia/GDD/biologia/ecossistemas-reset.png)

Confirmação: X1 bloqueia manipulação e navegação, Cancelar preserva tudo; Restaurar retorna à entrada. Só existe onde há restauração destrutiva.

Capturas locais ignoradas em `GeneratedMedia/GDD/biologia/`. Para compartilhar o GDD, enviar também essas imagens; elas não acompanham Git. Verificação e reprodução das cinco prévias com [o teste local do protótipo](../Tools/biology-migration/check-ecology.cjs). No diagrama, selecione uma ficha e depois o encaixe. O tronco com fungos abre a consulta.

## Execução e aceite

Modo inicial individual offline; textos, malhas, texturas e metadados embarcados. Sem API em runtime. LAN não implementada: a infraestrutura atual só dá autoridade coletiva ao lançamento oblíquo; não anunciar Biologia multiplayer.

- Completar o fluxo descrito, incluindo seleção, manipulação, consulta, correção e restauração/cancelamento.
- Rótulos e controles correspondem ao inventário; objetos e área de pega ficam legíveis durante consultas.
- Ocultar um objeto cancela foco e interação; não manter colisores ou seleção invisíveis. Voltar ao Hub não exige navegar por telas de estudo.
- Restaurar respeita estado autoral de cada peça; a preferência reduzida não altera o conteúdo científico.
- A [cena](../Assets/_Project/Laboratories/Biology/Ecosystems/Scenes/Ecosystems.unity) usa os materiais e os modelos incluídos em `Ecosystems/Art`. A [validação do Editor](../Assets/_Project/Editor/EcosystemsValidation.cs) exercita encaixe XR, retirada, preservação da cadeia, consulta, cancelamento, restauração, textos e shaders. Alcance, conforto e desempenho em headset exigem verificação no dispositivo.

## Verificação da cena implementada

O teste em Play Mode passou para os três encaixes, a recusa de papel incorreto, a retirada do produtor e a recomposição da cadeia. A consulta abriu por um evento de seleção XR. Cancelar a restauração preservou as peças e o fluxo. Confirmar devolveu os três organismos à bandeja. Os textos da consulta couberam na ficha e os shaders carregaram no Editor.

![Bancada com a cadeia montada, vista frontal no Unity](../GeneratedMedia/BiologyReview/ecossistemas-frente.png)

![Vista oblíqua dos trajetos de alimentação, calor e nutrientes no Unity](../GeneratedMedia/BiologyReview/ecossistemas-cadeia.png)

A vista oblíqua permite seguir os três tipos de seta sobre a pradaria. A revisão substituiu a fileira regular de pedras por pequenas concentrações de rocha e vegetação, aumentou as legendas e afastou a identificação dos fungos da placa do predador.
