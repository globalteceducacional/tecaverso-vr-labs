# Probabilidade e estatística

Sala individual de Matemática com três estações em U: moeda, dado e urna. Entrada: **Explorar sozinho → Matemática → Probabilidade e estatística**. Cena: `Scenes/Probability.unity`. [GDD](../../../../../docs/gdd-probabilidade-e-estatistica.md).

## Como usar

1. Acione a alavanca oficial Unity ou “Sortear uma vez”. Retorne a alavanca para rearmar; apenas agarrá-la não sorteia.
2. Escolha 10, 100 ou 1.000 tentativas e execute o lote. Pausar conserva o restante; Encerrar conserva os resultados obtidos. Ao afastar-se mais de 6 m da estação ou iniciar outro instrumento, o lote pausa.
3. Compare as barras azuis de frequência observada com as marcas laranja de probabilidade teórica. O eixo percentual permanece entre 0% e 100%.
4. No dado, consulte média, mediana e todas as modas empatadas. Recentes mostra apenas as últimas dez tentativas; os totais incluem toda a série.
5. Na urna, configure A/B entre 0 e 10 por categoria, com pelo menos uma bola, antes da primeira retirada. Compare os modos com e sem reposição. Sem reposição, P representa a próxima retirada e as marcas fixas ficam ocultas.
6. Examine a bola da bandeja usando XR Grab. Devolvê-la fisicamente não muda a composição: a configuração de reposição controla o modelo. Um lote pausa enquanto ela é segurada.
7. Nova série limpa apenas a estação atual e repõe a urna escolhida, mediante confirmação. Voltar ao Hub avisa que os resultados não serão salvos.

## Modelo e arquitetura

`Model/ProbabilitySeries.cs` separa sorteios, população, contagens e estatísticas da apresentação. Usa sorteador computacional uniforme para moeda/dado e ponderado pela composição restante da urna. Animações apresentam os resultados; a força da mão não altera as chances. Lotes agrupam amostras e mostram animações representativas, sem criar milhares de objetos.

`ProbabilityStation` coordena controles, gráficos, animações e pausa. `ProbabilityLab` coordena exclusividade dos lotes, confirmações e retorno ao Hub. Limite de 10.000 resultados por série; séries ficam somente em memória durante a visita. Valores indefinidos aparecem como “—”.

Reutiliza sala, rig, locomoção, renderização, uGUI/TMP e alavancas oficiais compartilhadas. Cúpula, lançador e recipiente possuem malhas persistidas em `Meshes`. `ProbabilityAuthoring` serve para primeira autoria e recusa recriar uma cena existente; alterações posteriores devem preservar os objetos e referências da cena.

## Validação e limites

Em 04/11/2026: 11 testes EditMode aprovados (estatísticas, população, reposição, limites e configurações inválidas). Em Play Mode: lote de 1.000 concluído, pausa/continuação, exclusividade, cancelamento, urna esgotada com composição exata e nova série. Raio XR sintético validou toggle e retorno/cancelamento; interator temporário validou pega e acionamento da alavanca sem duplicação. Ida e volta pelo Hub verificadas com um único rig e séries reiniciadas.

Modo individual offline, sem LAN, persistência ou exportação. Validação física de alcance, leitura e desempenho em headset permanece pendente. Nenhum build foi gerado. Capturas de revisão ficam em `Temp/ProbabilityQA/`, ignorado pelo Git.
