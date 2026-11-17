# Short GDD — Células animal e vegetal

**Status:** aprovado; implementado e verificado no Editor em 17/11/2026. Validação física em headset pendente. **Disciplina:** Biologia. **Modo:** individual offline. **Público:** Ensino Médio. **Data:** 17/11/2026. Uma das **seis** salas propostas, conforme [índice](gdd-biologia.md).

## Objetivo e experiência

Comparar estruturas comuns e exclusivas, relacionando forma e função. O aluno seleciona, gira e afasta organelas; identifica por que cloroplasto, parede e vacúolo central não devem aparecer como características da célula animal.

## Acervo, objetos e sala

**Cenas antigas consolidadas:** CelulaAnimalVegetal.

**Reaproveitamento comprovado por arquivos:** Animal_Cell.fbx e Plant_Cell.fbx; prefabs com 72 e 78 GameObjects, respectivamente. Parte dos nomes são genéricos: requerem mapa semântico manual. Núcleo, mitocôndria e lisossomo já têm nomes reconhecíveis no prefab animal.

**Catálogo fechado desta entrega:** Duas células visíveis simultaneamente. Na primeira entrega, seis estruturas consultáveis: núcleo, mitocôndria, membrana plasmática, parede celular, cloroplasto e vacúolo central. Núcleo e mitocôndria devem ser identificáveis nos dois modelos; estruturas da seleção atual são realçadas, sem apagar o outro exemplar.

| Conjunto de objetos | Papel | Interação | Limite |
|---|---|---|---|
| Modelos e peças descritos no catálogo | Fenômeno principal | Seleção direta, pega/rotação ou encaixe conforme o fluxo abaixo | Não disponibilizar partes sem identificação revisada |
| Suportes e sockets da atividade | Ancoragem, retorno e alcance | Controles 3D e filtros de encaixe | Preservar referência espacial e evitar seleção concorrente |
| Consulta e navegação | Apoio opcional | Apontar/tocar e fechar; voltar ao Hub | Uma consulta simultânea; sem HUD na câmera |

**Disposição proposta:** Dois suportes vizinhos a 1,10–1,30 m de altura, modelos com cerca de 0,8–1,0 m de largura. Faixa de circulação frontal de 1,5 m. Consulta à direita e afastada dos modelos. A escala é de observação, com aviso de ampliação.

**Controles escolhidos pela tarefa:** Toggle Rótulos; Recompor células. Dois anéis ancorados de rotação e sockets de retorno das organelas. Não há seletor obrigatório Animal/Vegetal: ambas ficam expostas.

## Inventário de interfaces, rótulos e controles 3D

**HUD na câmera: nenhum.** Uma ficha opcional de consulta. Os nomes ficam nos suportes e junto à estrutura selecionada; nenhum cartão extenso acompanha uma peça na mão. A sala não repete o formulário de Física.

| ID | Tipo e localização | Finalidade e ação | Inicial | Abre/fecha | Simultâneo |
|---|---|---|---|---|---|
| R1 | Nomes nos suportes e legenda junto ao modelo | Identificar o objeto e informar ampliação didática; rótulos extras só quando solicitados | Nomes visíveis; extras ocultos | Toggle quando houver; seleção realça uma estrutura | C1, S1; Q1 pode consultar a selecionada |
| C1 | Controles 3D no suporte e objetos da atividade | Toggle Rótulos; Recompor células. Dois anéis ancorados de rotação e sockets de retorno das organelas. Não há seletor obrigatório Animal/Vegetal: ambas ficam expostas. | Visíveis; estado inicial da atividade | Pega/toque/raio, evento de domínio | R1, E1, Q1 e S1; bloqueados por X1 |
| Q1 | Ficha clara fixa à lateral do suporte | Nome, função, contexto, limite do modelo; Fechar | Oculta | Selecionar uma parte ou símbolo de consulta; Fechar ou trocar alvo fecha/substitui | Objetos e C1 permanecem visíveis; uma Q1 por vez |
| E1 | Uma linha no suporte, sem painel flutuante | Confirmar ação ou explicar recusa/limite; não depender apenas de cor | Instrução inicial curta | Atualiza por evento; erro persiste até correção ou outra ação pertinente | R1, C1 e Q1 |
| X1 | Confirmação no espaço da consulta | Cancelar/Restaurar quando houver perda de montagem/população | Oculta | Ação Restaurar; decisão fecha | Objetos ao fundo visíveis; C1, Q1 e S1 sem interação |
| S1 | Voltar ao Hub, junto à saída e ao suporte | Retorno padrão à disciplina, sem confirmação se não houver estado relevante | Visível | Permanente | Todos, exceto bloqueio por X1 |

O cabeçalho de revisão e a seção “Como isso funciona em RV?” do HTML não são interfaces a implementar na sala. Motion reduzido é preferência geral do produto, exposta no menu padrão; não exige outro painel permanente de Biologia.

## Sequência e casos sem resultado

1. Entrar, reconhecer as peças e a primeira ação em E1.
2. Selecionar mitocôndria → abrir Q1 → segurar a peça → afastá-la até 0,4 m do conjunto → aproximar do socket original → soltar. Contorno do socket e breve encaixe confirmam retorno. Retirar uma organela é exploração de um modelo desmontável, não uma célula sobrevivendo sem ela.
3. Fechar a consulta, comparar outro caso e retornar ao estado de exploração.
4. Restaurar quando aplicável: X1 pede confirmação; cancelar preserva os objetos; S1 retorna ao Hub.

**Recusas e recuperação:** Uma membrana ou parede não destacável permanece no conjunto; peças não entram em sockets de outra organela. Retorno automático por perda/queda fora da área é anunciado em E1. Não aceitar identificação inventada a partir de nomes ObjectXX.

## Modelo científico e limites

Cores e tamanhos internos são didáticos. Não há metabolismo quantitativo, divisão celular ou escala comum entre célula e corpo humano. Movimento decorativo em repouso é opcional; rotação automática pausa durante seleção, pega e consulta.

Referência de revisão: [OpenStax — conteúdo relacionado](https://openstax.org/books/biology-2e/pages/4-3-eukaryotic-cells). Os modelos artísticos precisam de validação semântica; nomes de arquivos não são autoridade taxonômica/anatômica. Reutilização técnica não substitui revisão do conteúdo.

**Critério específico:** Núcleo e mitocôndria nas duas células; cloroplasto/parede/vacúolo central somente na vegetal representada. Separar e reencaixar a peça correta; consulta nunca cobre o ponto de pega.

## Interação, feedback e ergonomia

Pega por proximidade e raio XR conforme a peça; nenhuma ação essencial exige hand tracking. Bases são ancoradas e não são carregadas pela sala. Partes manipuláveis têm suporte/socket de retorno e área de pega distinta do botão de consulta. Não adicionar Rigidbody a cada submalha automaticamente. Alvos amplos, colliders simples e filtros evitam selecionar a base em vez da peça.

Foco: silhueta em 0,12–0,16 s, conforme o [padrão de todas as disciplinas](gdd-interaction-feedback.md). Sem caixa de collider ou segundo contorno de seleção. Sockets não acendem hover. Ação: confirmação de até 0,22 s no visual. Estado selecionado persiste; collider, câmera e referência científica permanecem estáveis. Encaixe aceita uma vez por gesto; recusa mantém a peça e explica correção. Reutilizar `Shared/Interaction`, preferência `MotionPreferences.Reduced` e controles oficiais Unity em `Shared/XR/Controls`, sem duplicar tweens. Som curto e haptic podem complementar, nunca substituir informação visual. Não inventar partículas para ornamentar uma galeria: usar somente para representar um processo cadastrado, com legenda e pausa quando temporal.

Modelos têm alcance e altura ajustáveis na autoria; o primeiro protótipo mira controles de pega entre 0,8 e 1,25 m, com consulta a cerca de 2–3 m. Validar sentado/em pé em headset antes de homologar. O controle de motion é a preferência geral da aplicação; no HTML está no cabeçalho de revisão.

## Prévias e estados

**Proposta, não cena Unity implementada.** O [protótipo HTML](prototypes/biologia/index.html?lab=celulas) usa diagramas esquemáticos. Header, navegação de revisão e comandos tracejados de simular pega/giro são externos ao laboratório; não autorizam novos painéis na Unity. Os controles sólidos representam controles da sala. A ficha aparece somente por seleção. O estado `detail` é exemplo de consulta; `action` exemplifica manipulação/resultado; `error` é exemplo visual do aviso, não execução automática de uma falha real.

![Disposição espacial proposta](../GeneratedMedia/GDD/biologia/layout.png)

Visão espacial comum: entrada, suporte elevado, faixa de circulação e consulta lateral. A disposição específica é descrita acima.

![Entrada](../GeneratedMedia/GDD/biologia/celulas-entry.png)

Entrada: R1, C1 e S1 visíveis; Q1 e X1 ocultos. Os objetos são a ação principal.

![Interação e resultado](../GeneratedMedia/GDD/biologia/celulas-action.png)

Manipulação/resultado: C1 altera os objetos; E1 confirma a ação. Fechar Q1 mantém o experimento. Não há interface sobre a câmera.

![Consulta opcional](../GeneratedMedia/GDD/biologia/celulas-detail.png)

Consulta: selecionar objeto ou símbolo da base abre Q1 lateral; Fechar remove o bloqueio visual sem resetar estado.

![Recusa ou limite](../GeneratedMedia/GDD/biologia/celulas-error.png)

Impedimento: E1 descreve a recuperação sem modal. O estado válido anterior é preservado.

![Restauração](../GeneratedMedia/GDD/biologia/celulas-reset.png)

Confirmação: X1 bloqueia manipulação e navegação, Cancelar preserva tudo; Restaurar retorna à entrada. Só existe onde há restauração destrutiva.

Capturas locais ignoradas em `GeneratedMedia/GDD/biologia/`. Para compartilhar o GDD, enviar também essas imagens; elas não acompanham Git. Fonte e reprodução em [README do protótipo](prototypes/biologia/README.md).

## Execução e aceite

Modo inicial individual offline; textos, malhas, texturas e metadados embarcados. Sem API em runtime. LAN não implementada: a infraestrutura atual só dá autoridade coletiva ao lançamento oblíquo; não anunciar Biologia multiplayer.

- Completar o fluxo descrito, incluindo seleção, manipulação, consulta, correção e restauração/cancelamento.
- Rótulos e controles correspondem ao inventário; objetos e área de pega ficam legíveis durante consultas.
- Ocultar um objeto cancela foco e interação; não manter colisores ou seleção invisíveis. Voltar ao Hub não exige navegar por telas de estudo.
- Restaurar respeita estado autoral de cada peça; a preferência reduzida não altera o conteúdo científico.
- Compilação, testes de domínio e captura no Editor serão exigidos na implementação. Alcance, conforto e desempenho em headset são etapas separadas. Nenhum build sem autorização.

## Implementação desta entrega — 17/11/2026

[Uso, componentes e mapa semântico](../Assets/_Project/Laboratories/Biology/Cells/README.md). Oito alvos representam as seis categorias, com seis organelas destacáveis. Membrana e parede são fixas. Nenhuma nomenclatura foi inferida apenas de ObjectXX. Dois FBX e materiais foram importados seletivamente, sem dependência runtime da pasta antiga.

Controles compactos em Canvas World Space no suporte; giro por dials oficiais Unity. Rótulos extras inicialmente ocultos; ao serem exibidos, ficam em colunas laterais fixas com linhas até suas estruturas, sem girar com o modelo nem cobrir as organelas. Uma consulta lateral por vez. Indicador do encaixe aparece enquanto a peça está separada. A rotação é bloqueada durante consulta/pega/separação. Recomposição preserva a preferência dos rótulos. Retorno ao Hub confirma quando há peça separada; peça na mão deve ser solta antes de abrir a confirmação.

Limites aplicados: 0,4 m de afastamento; retorno ao soltar até 0,12 m da origem; recuperação por abandono após 15 s. Testes de Editor cobriram seleção XR, filtro dos sockets, consulta, cancelamento, recomposição, motion e limite físico. Sem build, LAN ou homologação em headset. As imagens anteriores continuam identificadas como proposta; capturas reais desta entrega estão abaixo.

Capturas atualizadas após a revisão compartilhada de silhueta de 17/11/2026; a consulta e os controles são os mesmos. As caixas de bounds/seleção foram substituídas pelo padrão comum às quatro disciplinas.

![Sala implementada no Editor](../GeneratedMedia/InteractionFeedback/cells-entry.png)

![Consulta com a organela selecionada](../GeneratedMedia/InteractionFeedback/cells-selected-1.png)

![Identificação lateral das estruturas](../GeneratedMedia/InteractionFeedback/cells-labels.png)
