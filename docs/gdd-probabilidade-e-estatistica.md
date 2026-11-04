# Short GDD — Probabilidade e estatística

**Status:** proposta para revisão; implementação não autorizada por este documento.  
**Data:** 04/11/2026 · Matemática · Exploração individual offline · Sessão sugerida: 10–15 minutos.

## Conceito e objetivo

Um laboratório de sorteios com três instrumentos: moeda, dado e urna. O aluno prevê resultados, realiza tentativas e acompanha frequências em gráficos grandes. Entender a diferença entre probabilidade teórica e frequência observada, a variabilidade das amostras e medidas de tendência central.

## Sala e objetos

- Três estações em U, com centro livre: lançador de moeda, cúpula de dado e urna de bolas. Instrumentos reconhecíveis, com carenagens azuis, mecanismos visíveis e alavancas Unity UI_3D.
- Cada estação possui um gráfico vertical atrás dela, legível a partir da área de operação. Colunas têm categoria, contagem e frequência relativa; eixo vertical percentual fixo de 0% a 100% permite comparar amostras.
- Moeda com faces distintas por símbolo e texto; dado com faces 1 a 6; bolas com cor e símbolo redundantes.
- A urna mostra sua composição antes do sorteio. A extração ocorre em uma câmara que oculta qual bola será escolhida; o aluno não escolhe visualmente a próxima bola.
- Pequena bandeja mostra o último resultado. Um registro local lista as últimas dez tentativas; os totais incluem todas as tentativas da série.
- Controles e fórmulas junto a cada instrumento. Há um único botão de retorno ao Hub próximo da entrada.

## Experimentos e modelo

| Estação | Modelo inicial | Leituras |
|---|---|---|
| Moeda | Cara/coroa, p = 1/2 para cada resultado | Contagens e frequências por face |
| Dado | Seis resultados equiprováveis, p = 1/6 | Contagens, frequências, média, mediana e moda dos valores obtidos |
| Urna | Duas categorias, inicialmente 5 bolas de cada, com reposição | Composição, probabilidades atuais e frequências observadas |

Um sorteador computacional determina o resultado; a animação física o apresenta. Essa escolha é explicada em uma frase junto ao instrumento. O movimento da mão não altera a probabilidade. Não anunciar que a física do dado ou da moeda garante equiprobabilidade.

Fórmulas junto aos gráficos: frequência relativa = ocorrências / tentativas; na urna, P(categoria) = quantidade da categoria / total disponível. No dado, média = soma dos valores / número de resultados. Média e mediana não se aplicam às categorias da urna ou às faces nomeadas da moeda.

## Interações XR e controles

- **Sortear uma vez:** mover a alavanca entre os batentes e retornar para rearmá-la. Um ciclo registra um resultado; agarrar a alavanca, por si só, não sorteia. Botão equivalente disponível pelo raio XR.
- **Lote:** seleção exclusiva de 10, 100 ou 1.000 tentativas, seguida de “Executar lote”. Quantidade discreta, sem slider.
- **Pausar / Continuar:** disponível durante lote. A pausa conserva contagens e trabalho restante. “Encerrar lote” mantém os resultados já obtidos e cancela o restante.
- **Exibir probabilidades teóricas:** toggle inicialmente ligado. Mostra marcas de referência e valores separados das frequências observadas.
- **Nova série:** limpa somente a estação atual, com confirmação quando já houver resultados. Não altera as outras estações.
- **Urna:** botões − / + por categoria, entre 0 e 10 bolas, total mínimo 1 e máximo 20. Toggle “Com reposição”, inicialmente ligado.
- **Voltar ao Hub:** encerra a exploração; se houver resultados ou lote ativo, oferece cancelar ou sair, informando que a série não será salva.

Configuração da urna fica bloqueada depois da primeira tentativa, com indicação “Inicie nova série para mudar a composição”. Na nova série, repor a composição inicial escolhida e liberar edição. Não misturar resultados de modelos diferentes.

As bolas retiradas podem ser examinadas por pega XR na bandeja, mas recolocá-las fisicamente não altera a população estatística. A reposição é responsabilidade explícita do experimento. Sair da área de operação pausa um lote antes de iniciar outro instrumento; só um lote executa por vez. Cada estação mantém sua série em memória durante a visita.

## Ritmo, resultados e casos especiais

Tentativas isoladas mostram uma animação completa. Lotes usam atualizações agrupadas, com progresso “340 / 1.000” e até poucas animações representativas; não instanciar milhares de objetos nem simular visualmente cada lançamento. O total exibido corresponde a todas as amostras calculadas, e o agrupamento é informado.

Limite proposto de 10.000 resultados por série. Ao atingir o limite, oferecer nova série. Cancelar um lote não descarta as amostras concluídas. Antes da primeira tentativa, frequências e estatísticas exibem “—”, não divisões por zero.

Sem reposição, a urna atualiza as probabilidades a cada retirada. Os sorteios tornam-se dependentes; não comparar sua frequência acumulada com uma linha fixa como se a probabilidade não mudasse. Mostrar composição inicial, composição restante e “probabilidade da próxima retirada”. Um lote encerra ao esvaziar a urna, informando quantas tentativas realizou. “Repor urna” inicia uma nova série mediante confirmação.

Na moeda e no dado, a probabilidade da próxima tentativa não muda após uma sequência de resultados iguais. Frequências não precisam aproximar-se da teoria a cada tentativa nem coincidir exatamente em amostras grandes.

No dado, ordenar os resultados para a mediana; com quantidade par, usar a média dos dois valores centrais. Empates de maior frequência mostram todas as modas; quando todas as categorias observadas têm a mesma frequência, indicar ausência de moda única. Não arredondar a média para uma face possível do dado.

## Percurso sugerido

1. Prever quantas caras aparecerão em dez lançamentos; acionar a moeda dez vezes e comparar com a previsão.
2. Executar mais 100 tentativas e observar a frequência relativa, sem exigir resultado exatamente igual a 50%.
3. Sortear o dado; observar como média, mediana e moda descrevem a mesma série de formas diferentes.
4. Configurar a urna com 2 bolas de uma categoria e 8 da outra; prever e testar com reposição.
5. Iniciar nova série sem reposição; observar como cada retirada altera a próxima probabilidade até a urna ficar vazia.

## Reuso, limites e aceite

Reutilizar rig, locomoção, alavancas e botões oficiais compartilhados, identidade, uGUI/TMP e renderização. Seguir [UI](../UI_GUIDELINES.md), [design system](../DESIGN_SYSTEM.md) e [escopo do produto](../README.md). Os gráficos são resultados, nunca sliders.

Fora do primeiro escopo: dados viciados configuráveis, combinatória geral, testes de hipótese, regressão, edição livre de dados, exportação, persistência e LAN. Não há ranking ou acerto obrigatório.

Aceite futuro: amostras e contagens coerentes; sorteio ponderado pela composição real; pausa sem perda/duplicação; urna vazia tratada; estatísticas corretas em séries conhecidas e empates; gráficos distinguindo teoria e observação; ausência de crescimento ilimitado de objetos. Testes usam sementes controladas, sem exigir coincidência exata de frequências aleatórias. Validar interação e leitura no headset separadamente; builds somente por solicitação.
