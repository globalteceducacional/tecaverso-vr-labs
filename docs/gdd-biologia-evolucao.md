# Short GDD — Evolução e seleção natural

**Status:** implementado na [cena Evolution](../Assets/_Project/Laboratories/Biology/Evolution/Scenes/Evolution.unity). **Disciplina:** Biologia. **Modo:** individual offline. **Público:** Ensino Médio. **Data:** 17/11/2026. Uma das **seis** salas propostas, conforme [índice](gdd-biologia.md).

## Objetivo e experiência

Observar mudanças de frequência entre gerações sob seleção. O aluno alterna um ambiente, guarda uma referência e compara populações; indivíduos não mudam a própria característica ao trocar o ambiente.

## Acervo, objetos e sala

**Cenas antigas consolidadas:** EvolucaoEspecies.

**Reaproveitamento comprovado por arquivos:** A cena antiga é um esboço de ambiente, sem uma simulação de evolução encontrada. Suporte e população de organismos estilizados são novos. Não converter variantes coloridas de bactérias em espécies distintas sem justificativa.

**Catálogo fechado desta entrega:** 24 mariposas estilizadas da mesma espécie fictícia, inicialmente 12 claras e 12 escuras; dois ambientes (claro/escuro); seis gerações por execução; uma referência guardada. Característica herdável binária simplificada, sem sexualidade ou genótipos explícitos nesta entrega.

| Conjunto de objetos | Papel | Interação | Limite |
|---|---|---|---|
| Terrário | Casca clara ou escura com relevo suave, inclinada para a entrada. As 24 mariposas têm asas, tórax, antenas e duas pigmentações | Observar de frente, de lado e por cima | Os insetos ficam no terrário |
| Console frontal | Comparar a mesma população sob ambientes diferentes | Alavanca física e botão de geração alcançáveis na borda, por mão ou raio | Uma geração por gesto |
| Registro e consulta | Contagens de indivíduos claros e escuros na população e na referência salva | Guardar uma referência e consultar o processo | Um registro por vez |

**Disposição proposta.** Terrário elevado de aproximadamente 2 × 1,2 m. O fundo inclinado torna visíveis os insetos das fileiras posteriores. As mariposas alternam a orientação e a posição na casca. O visor de contagem ocupa a borda traseira. As ações ficam à frente e a consulta abre à direita do terrário.

## Inventário de interfaces, rótulos e controles 3D

| ID | Tipo e localização | Finalidade e ação | Inicial | Abre/fecha | Simultâneo |
|---|---|---|---|---|---|
| R1 | Visor fixo no fundo do terrário | Ler ambiente, geração e contagens de ambos os fenótipos. A linha de referência registra geração, ambiente e contagens anteriores | G0, ambiente claro, 12 claras e 12 escuras, referência vazia | Atualiza a cada ação | C1, E1, Q1, S1 |
| C1 | Alavanca Claro/Escuro e botão físico Avançar geração na borda frontal | Inclinar a alavanca para trocar a casca. Pressionar o botão uma vez para avançar | Visíveis, ambiente claro | O botão fica indisponível ao atingir G6 | R1, E1, Q1, S1 |
| C2 | Faixa de ações no mesmo suporte | Guardar referência, abrir consulta ou pedir restauração | Visível | A referência nova substitui a anterior | R1, C1, E1, Q1, S1 |
| Q1 | Ficha lateral fixa | Ler como a camuflagem afeta a contribuição de cada fenótipo à próxima geração e como o arredondamento afeta as contagens | Oculta | Ação Sobre a seleção abre, Fechar recolhe | R1, C1, C2, E1, S1 |
| E1 | Linha fixa sob as contagens | Mostrar instrução, fase de transição e motivo do limite | Sugere guardar a população inicial | Atualiza por evento | R1, C1, C2, Q1, S1 |
| X1 | Confirmação no lugar de Q1 | Cancelar ou restaurar população, ambiente e referência | Oculta | Restaurar pede confirmação quando existe alteração | R1 e E1 permanecem visíveis; demais ações bloqueadas |
| S1 | Voltar ao Hub na faixa de ações | Retornar à disciplina | Visível | Permanente | Todos, salvo X1 |

A preferência geral de movimento reduzido mantém as contagens e substitui a transição visual por troca imediata. Sem HUD na câmera ou ficha presa a um indivíduo.

## Sequência e casos sem resultado

1. Entrar, reconhecer as peças e a primeira ação em E1.
2. Guardar a população inicial → avançar três gerações no ambiente claro → comparar → alternar ambiente → avançar até seis → observar a tendência. A animação distingue sobrevivência e nascimento dos descendentes; mudança de ambiente não recolore indivíduos.
3. Fechar a consulta, comparar outro caso e retornar ao estado de exploração.
4. Restaurar quando aplicável: X1 pede confirmação; cancelar preserva os objetos; S1 retorna ao Hub.

**Recusas e recuperação:** Ao atingir seis gerações, Avançar fica indisponível com motivo. Guardar referência substitui apenas a referência, sem perder população. Cancelar restauração preserva geração, ambiente, frequência e referência.

## Modelo científico e limites

Modelo didático determinístico: pesos relativos 1 para fenótipo favorecido e 0,7 para o outro. n_claros′ = round(24 × n_claros × w_claro / (n_claros × w_claro + n_escuros × w_escuro)); n_escuros′ = 24 − n_claros′. Pesos são escolhas do modelo, não dados de uma espécie. Sem deriva, mutação, migração, especiação, genética mendeliana ou previsão real. Contagens arredondadas podem estabilizar e isso deve ser explicado.

Referência de revisão: [OpenStax — conteúdo relacionado](https://openstax.org/books/biology-2e/pages/19-3-adaptive-evolution). Os modelos artísticos precisam de validação semântica; nomes de arquivos não são autoridade taxonômica/anatômica. Reutilização técnica não substitui revisão do conteúdo.

**Critério específico:** Ambiente sozinho não muda contagens ou cor. Geração mantém total 24; tendência acompanha pesos. Referência é cópia imutável; seis é limite explícito. Não chamar evolução de melhoria ou intenção.

## Interação, feedback e ergonomia

A alavanca usa o modelo da Unity instalado no projeto. O jogador segura o cabo e inclina para a esquerda ou direita. O botão de geração usa o prefab Push Button do Starter Assets do XRI. O alvo de foco é a tecla móvel. O controle tem uma legenda própria que passa a “Limite · 6 gerações” no fim do percurso. O visor, o tampo e as mariposas permanecem ancorados.

Ao avançar, a reposição dura cerca de dois segundos. Primeiro sai de cena uma parte das mariposas do fenótipo desfavorecido. As demais ficam visíveis por um instante. Depois os pais saem e surge a geração de descendentes. A pigmentação só é trocada entre coortes. A seleção visual de sobreviventes ilustra o processo; a fórmula de contribuição calcula a composição da nova população a partir das contagens inteiras anteriores. Cada avanço exige nova ação do aluno.

A fase em curso aparece no visor. A alavanca, o registro e o avanço ficam bloqueados durante a reposição para manter a comparação coerente. A ficha lateral e a saída permanecem acessíveis. Com movimento reduzido, a nova geração aparece de imediato. A leitura, a câmera e os colliders continuam fixos.

O primeiro roteiro de comparação parte de 12 claras e 12 escuras. Três avanços no ambiente claro levam a 18 claras e 6 escuras. Mudar para casca escura mantém as mesmas 18 claras até o próximo avanço. Após três avanços nesse ambiente, o visor registra 12 claras e 12 escuras novamente. Um segundo percurso de seis gerações em ambiente sempre claro produz 21 claras e 3 escuras.

## Prévias e estados

**Imagens do protótipo de revisão.** O [protótipo HTML](prototypes/biologia/index.html?lab=evolucao) usa diagramas esquemáticos. Header, navegação de revisão e comandos tracejados de simular pega/giro são externos ao laboratório; não autorizam novos painéis na Unity. Os controles sólidos representam controles da sala. A ficha aparece somente por seleção. O estado `detail` é exemplo de consulta; `action` exemplifica manipulação/resultado; `error` é exemplo visual do aviso, não execução automática de uma falha real.

![Disposição espacial proposta](../GeneratedMedia/GDD/biologia/layout.png)

Visão espacial comum: entrada, suporte elevado, faixa de circulação e consulta lateral. A disposição específica é descrita acima.

![Entrada](../GeneratedMedia/GDD/biologia/evolucao-entry.png)

Entrada: R1, C1 e S1 visíveis; Q1 e X1 ocultos. Os objetos são a ação principal.

![Interação e resultado](../GeneratedMedia/GDD/biologia/evolucao-action.png)

Manipulação/resultado: C1 altera os objetos; E1 confirma a ação. Fechar Q1 mantém o experimento. Não há interface sobre a câmera.

![Consulta opcional](../GeneratedMedia/GDD/biologia/evolucao-detail.png)

Consulta: selecionar objeto ou símbolo da base abre Q1 lateral; Fechar remove o bloqueio visual sem resetar estado.

![Recusa ou limite](../GeneratedMedia/GDD/biologia/evolucao-error.png)

Impedimento: E1 descreve a recuperação sem modal. O estado válido anterior é preservado.

![Restauração](../GeneratedMedia/GDD/biologia/evolucao-reset.png)

Confirmação: X1 bloqueia manipulação e navegação, Cancelar preserva tudo; Restaurar retorna à entrada. Só existe onde há restauração destrutiva.

Capturas locais ignoradas em `GeneratedMedia/GDD/biologia/`. Para compartilhar o GDD, enviar também essas imagens; elas não acompanham Git. Fonte e reprodução em [README do protótipo](prototypes/biologia/README.md).

## Execução e aceite

Modo inicial individual offline; textos, malhas, texturas e metadados embarcados. Sem API em runtime. LAN não implementada: a infraestrutura atual só dá autoridade coletiva ao lançamento oblíquo; não anunciar Biologia multiplayer.

- Completar o fluxo descrito, incluindo seleção, manipulação, consulta, correção e restauração/cancelamento.
- Rótulos e controles correspondem ao inventário; objetos e área de pega ficam legíveis durante consultas.
- Ocultar um objeto cancela foco e interação; não manter colisores ou seleção invisíveis. Voltar ao Hub não exige navegar por telas de estudo.
- Restaurar respeita estado autoral de cada peça; a preferência reduzida não altera o conteúdo científico.
- Verificação de domínio executada na classe C# usada pelo laboratório. As 64 sequências de seis escolhas de ambiente conservaram o total de 24, a referência e o limite de seis gerações. O roteiro HTML também passou pela troca de ambiente, pelo limite, pela referência e pelo cancelamento. A verificação de cena está em [EvolutionValidation](../Assets/_Project/Editor/EvolutionValidation.cs). Alcance, conforto e desempenho em headset dependem de validação física.

### Revisão no Editor

O percurso em Play Mode acionou o botão pelo XR Interaction Manager, alternou as cascas, guardou a referência e verificou a seleção dos pais e a entrada dos descendentes. A consulta, o bloqueio durante a confirmação, o cancelamento, a restauração e a preferência de movimento reduzido passaram pela mesma verificação.

Na captura frontal da G3 sobre casca escura, havia 6 claras e 18 escuras. Na referência G0 ainda havia 12 de cada. A primeira revisão revelou que a borda frontal tapava a ponta de algumas asas. A correção de autoria eleva a amostra e o visor em 10 cm, amplia as duas contagens e mantém os controles na mesma altura. O shader mistura sulcos curtos e manchas de casca com detalhe filtrado à distância.

A vista oblíqua também revelou as quinas dos pés atravessando a casca inclinada. Os apoios ajustados ficam pelo menos 4,9 cm abaixo do plano da casca. O teste do Editor verifica as oito quinas e lança raios para os vértices das seis mariposas da frente a partir de duas alturas de observação, 1,12 m e 1,65 m. Nas capturas finais de 18/11, a primeira fileira aparece por inteiro na vista de entrada e as partes da casca que cobrem os pés estão contínuas na vista oblíqua.

![Vista frontal da população e da referência](../GeneratedMedia/BiologyReview/evolucao-frente.png)

![Vista aproximada das asas e da casca](../GeneratedMedia/BiologyReview/evolucao-coorte.png)
