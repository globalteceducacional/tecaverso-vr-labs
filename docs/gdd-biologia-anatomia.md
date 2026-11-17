# Short GDD — Corpo humano e sistemas

**Status:** aprovado para desenvolvimento; ainda não implementado. **Disciplina:** Biologia. **Modo:** individual offline. **Público:** Ensino Médio. **Data:** 17/11/2026. Uma das **seis** salas propostas, conforme [índice](gdd-biologia.md).

## Objetivo e experiência

Relacionar órgãos, sistemas e sua posição no mesmo corpo. O aluno gira um corpo ancorado, escolhe um sistema e consulta órgãos; observa quatro percursos esquemáticos, sem trocar de sala.

## Acervo, objetos e sala

**Cenas antigas consolidadas:** EsqueletoHumano + SistemaCirculatorio + SistemaDigestorio + SistemaNervoso + SistemaRespiratorio.

**Reaproveitamento comprovado por arquivos:** Body, Skeleton, Muscles, NervousSystem, Sistema Digestorio, Sistema Renal, Sistema Reprodutivo, SistemaCirculatório, Sitema Respiratorio e LymphaticSystem. São conjuntos derivados do FBX masculino. Há FBX feminino separado, preservado para adaptação futura, sem prometer equivalência ou alternância pronta.

**Catálogo fechado desta entrega:** Nove sistemas/camadas além da pele: respiratório, circulatório, digestório, nervoso, esquelético, muscular, urinário, linfático e reprodutivo. Um sistema principal por vez e pele independente. Quatro percursos: ar/vias respiratórias, sangue em circuito, caminho digestório e sinal nervoso ilustrativo. Os demais são exploração anatômica estática.

| Conjunto de objetos | Papel | Interação | Limite |
|---|---|---|---|
| Modelos e peças descritos no catálogo | Fenômeno principal | Seleção direta, pega/rotação ou encaixe conforme o fluxo abaixo | Não disponibilizar partes sem identificação revisada |
| Suportes e sockets da atividade | Ancoragem, retorno e alcance | Controles 3D e filtros de encaixe | Preservar referência espacial e evitar seleção concorrente |
| Consulta e navegação | Apoio opcional | Apontar/tocar e fechar; voltar ao Hub | Uma consulta simultânea; sem HUD na câmera |

**Disposição proposta:** Corpo com aproximadamente 1,7 m de altura, base giratória ancorada no centro; circulação de pelo menos 1,5 m ao redor. Seleção de sistemas no suporte frontal inclinado, abaixo do tórax visual. Consulta lateral fixa. Não copiar o posicionamento antigo de órgãos soltos a vários metros do corpo.

**Controles escolhidos pela tarefa:** Nove botões exclusivos nomeados; toggle Pele; Observar/Pausar percurso somente nos quatro sistemas com percurso; anel físico de rotação. Sem slider de índice de sistema e sem um painel de parâmetros.

## Inventário de interfaces, rótulos e controles 3D

**HUD na câmera: nenhum.** Uma ficha opcional de consulta. Os nomes ficam nos suportes e junto à estrutura selecionada; nenhum cartão extenso acompanha uma peça na mão. A sala não repete o formulário de Física.

| ID | Tipo e localização | Finalidade e ação | Inicial | Abre/fecha | Simultâneo |
|---|---|---|---|---|---|
| R1 | Nomes nos suportes e legenda junto ao modelo | Identificar o objeto e informar ampliação didática; rótulos extras só quando solicitados | Nomes visíveis; extras ocultos | Toggle quando houver; seleção realça uma estrutura | C1, S1; Q1 pode consultar a selecionada |
| C1 | Controles 3D no suporte e objetos da atividade | Nove botões exclusivos nomeados; toggle Pele; Observar/Pausar percurso somente nos quatro sistemas com percurso; anel físico de rotação. Sem slider de índice de sistema e sem um painel de parâmetros. | Visíveis; estado inicial da atividade | Pega/toque/raio, evento de domínio | R1, E1, Q1 e S1; bloqueados por X1 |
| Q1 | Ficha clara fixa à lateral do suporte | Nome, função, contexto, limite do modelo; Fechar | Oculta | Selecionar uma parte ou símbolo de consulta; Fechar ou trocar alvo fecha/substitui | Objetos e C1 permanecem visíveis; uma Q1 por vez |
| E1 | Uma linha no suporte, sem painel flutuante | Confirmar ação ou explicar recusa/limite; não depender apenas de cor | Instrução inicial curta | Atualiza por evento; erro persiste até correção ou outra ação pertinente | R1, C1 e Q1 |
| X1 | Não se aplica | Não há restauração destrutiva nesta sala | Não existe | — | — |
| S1 | Voltar ao Hub, junto à saída e ao suporte | Retorno padrão à disciplina, sem confirmação se não houver estado relevante | Visível | Permanente | Todos, exceto bloqueio por X1 |

O cabeçalho de revisão e a seção “Como isso funciona em RV?” do HTML não são interfaces a implementar na sala. Motion reduzido é preferência geral do produto, exposta no menu padrão; não exige outro painel permanente de Biologia.

## Sequência e casos sem resultado

1. Entrar, reconhecer as peças e a primeira ação em E1.
2. Escolher Respiratório → ocultar pele → selecionar pulmão → consultar função → observar percurso → pausar → escolher Circulatório e repetir no coração. Visão muda no mesmo referencial corporal. Esqueleto e músculos são alternativas na mesma sala.
3. Fechar a consulta, comparar outro caso e retornar ao estado de exploração.
4. Restaurar quando aplicável: X1 pede confirmação; cancelar preserva os objetos; S1 retorna ao Hub.

**Recusas e recuperação:** Trocar sistema fecha a consulta do órgão oculto e interrompe seu percurso, sem deixar collider invisível. Órgãos não mapeados são identificados como indisponíveis para consulta. Não mostrar animação de contração muscular ou articulação se o modelo não foi rigado.

## Modelo científico e limites

Preservar cores anatômicas; azul/vermelho do esquema de vasos não representa cor real do sangue. Ar, sangue, alimento e sinal não compartilham a mesma animação ou legenda. Sem vazão, ECG, diagnóstico, cirurgia ou fisiologia quantitativa. O esqueleto tem grupos de ossos: não prometer seleção dos 206 ossos.

Referência de revisão: [OpenStax — conteúdo relacionado](https://openstax.org/books/biology-2e/pages/1-2-themes-and-concepts-of-biology). Os modelos artísticos precisam de validação semântica; nomes de arquivos não são autoridade taxonômica/anatômica. Reutilização técnica não substitui revisão do conteúdo.

**Critério específico:** Os nove sistemas são selecionáveis sem mudar de cena. Pele, visibilidade e consultas coerentes. Quatro percursos com legendas próprias e pausa. Preservar alinhamento anatômico de origem.

## Interação, feedback e ergonomia

Pega por proximidade e raio XR conforme a peça; nenhuma ação essencial exige hand tracking. Bases são ancoradas e não são carregadas pela sala. Partes manipuláveis têm suporte/socket de retorno e área de pega distinta do botão de consulta. Não adicionar Rigidbody a cada submalha automaticamente. Alvos amplos, colliders simples e filtros evitam selecionar a base em vez da peça.

Foco: contorno em 0,12–0,16 s. Ação: confirmação de até 0,22 s no filho visual. Estado selecionado persiste; collider, câmera e referência científica permanecem estáveis. Encaixe aceita uma vez por gesto; recusa mantém a peça e explica correção. Reutilizar `Shared/Interaction`, preferência `MotionPreferences.Reduced` e controles oficiais Unity em `Shared/XR/Controls`, sem duplicar tweens. Som curto e haptic podem complementar, nunca substituir informação visual. Não inventar partículas para ornamentar uma galeria: usar somente para representar um processo cadastrado, com legenda e pausa quando temporal.

Modelos têm alcance e altura ajustáveis na autoria; o primeiro protótipo mira controles de pega entre 0,8 e 1,25 m, com consulta a cerca de 2–3 m. Validar sentado/em pé em headset antes de homologar. O controle de motion é a preferência geral da aplicação; no HTML está no cabeçalho de revisão.

## Prévias e estados

**Proposta, não cena Unity implementada.** O [protótipo HTML](prototypes/biologia/index.html?lab=anatomia) usa diagramas esquemáticos. Header, navegação de revisão e comandos tracejados de simular pega/giro são externos ao laboratório; não autorizam novos painéis na Unity. Os controles sólidos representam controles da sala. A ficha aparece somente por seleção. O estado `detail` é exemplo de consulta; `action` exemplifica manipulação/resultado; `error` é exemplo visual do aviso, não execução automática de uma falha real.

![Disposição espacial proposta](../GeneratedMedia/GDD/biologia/layout.png)

Visão espacial comum: entrada, suporte elevado, faixa de circulação e consulta lateral. A disposição específica é descrita acima.

![Entrada](../GeneratedMedia/GDD/biologia/anatomia-entry.png)

Entrada: R1, C1 e S1 visíveis; Q1 e X1 ocultos. Os objetos são a ação principal.

![Interação e resultado](../GeneratedMedia/GDD/biologia/anatomia-action.png)

Manipulação/resultado: C1 altera os objetos; E1 confirma a ação. Fechar Q1 mantém o experimento. Não há interface sobre a câmera.

![Consulta opcional](../GeneratedMedia/GDD/biologia/anatomia-detail.png)

Consulta: selecionar objeto ou símbolo da base abre Q1 lateral; Fechar remove o bloqueio visual sem resetar estado.

![Recusa ou limite](../GeneratedMedia/GDD/biologia/anatomia-error.png)

Impedimento: E1 descreve a recuperação sem modal. O estado válido anterior é preservado.

**X1 não se aplica à Anatomia:** a seleção de sistemas e o fechamento de consultas não destroem montagem. Não criar modal de reset por convenção.

Capturas locais ignoradas em `GeneratedMedia/GDD/biologia/`. Para compartilhar o GDD, enviar também essas imagens; elas não acompanham Git. Fonte e reprodução em [README do protótipo](prototypes/biologia/README.md).

## Execução e aceite

Modo inicial individual offline; textos, malhas, texturas e metadados embarcados. Sem API em runtime. LAN não implementada: a infraestrutura atual só dá autoridade coletiva ao lançamento oblíquo; não anunciar Biologia multiplayer.

- Completar o fluxo descrito, incluindo seleção, manipulação, consulta, correção e restauração/cancelamento.
- Rótulos e controles correspondem ao inventário; objetos e área de pega ficam legíveis durante consultas.
- Ocultar um objeto cancela foco e interação; não manter colisores ou seleção invisíveis. Voltar ao Hub não exige navegar por telas de estudo.
- Restaurar respeita estado autoral de cada peça; a preferência reduzida não altera o conteúdo científico.
- Compilação, testes de domínio e captura no Editor serão exigidos na implementação. Alcance, conforto e desempenho em headset são etapas separadas. Nenhum build sem autorização.
