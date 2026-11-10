# Short GDD — Soluções

**Status:** proposta para aprovação; não implementado. **Disciplina:** Química. **Modo:** exploração individual offline. **Sessão sugerida:** 10–15 minutos.

## Objetivo e pergunta

Como massa de soluto e volume de solução determinam a concentração? O aluno prepara, compara e dilui soluções, distinguindo material dissolvido de sólido que permanece no fundo.

## Sala e objetos

- Bancada aberta e circulável com frascos graduados agarráveis, água, dispensadores de solutos cadastrados, espátula ou doseador, agitador XR e área de comparação A/B. Recipientes têm nome, capacidade e marcas em mL.
- Cada frasco usa o sistema visual compartilhado: nível de enchimento, superfície que oscila ao ser movida e partículas de despejo. Grãos sólidos usam uma apresentação distinta. Uma lupa molecular fixa junto à área de comparação mostra a dispersão relativa das partículas.
- Instrumentos e dados acompanham os frascos; não há painel de parâmetros que substitua a manipulação. Frascos vazios e cheios mantêm pontos de pega identificáveis.

## Fluxo e interações

1. O aluno escolhe um soluto do catálogo, adiciona água ao frasco A e doses conhecidas de soluto. Agitar acelera a apresentação da mistura; a quantidade dissolvida é governada pelo modelo, não pela força da mão.
2. Prepara B com outra massa ou volume e aproxima os frascos para comparar concentrações. Rótulos locais mostram soluto, massa total adicionada, massa dissolvida, volume da solução e concentração.
3. Transfere uma porção de A para B e acrescenta solvente. O modelo conserva a quantidade de soluto efetivamente transferida e recalcula a concentração; o líquido não duplica ao passar entre frascos.
4. Em um caso com dados disponíveis, altera a temperatura por um controle físico graduado e observa como o limite de solubilidade muda. Esse controle só aparece nessa atividade e exibe °C.

Um seletor de atividades nomeadas oferece **Preparar**, **Comparar** e **Diluir** como instruções opcionais, sem bloquear a exploração. Toggles locais exibem **Vista de partículas** e **Marcas de volume**. Botões **Esvaziar frasco** e **Voltar ao Hub** seguem confirmação proporcional à perda de trabalho. Não usar slider para escolher soluto ou para definir quantidades já manipuladas fisicamente.

## Modelo, leitura e limites

- No caso simples, concentração em massa `C = m_soluto dissolvido / V_solução`, com unidade explícita, por exemplo g/L. Não confundir volume de solvente com volume final da solução quando os dados os distinguirem.
- Diluição: a massa de soluto dissolvido na porção transferida é conservada ao adicionar solvente, enquanto o volume final cresce. Mostrar os valores antes e depois.
- Saturação depende de dados tabelados por soluto e temperatura. Uma quantidade adicional pode permanecer sólida; não aumentar indefinidamente a concentração exibida.
- Cor do líquido é recurso de identificação visual, não medidor universal de concentração. A lupa molecular é qualitativa e não representa número real de partículas ou dinâmica molecular.

Primeiro escopo: poucos solutos e faixas de temperatura com dados definidos; sem mistura arbitrária de vários solutos, reação química, densidade real variável ou fluido computacional completo. O sistema de líquidos é compartilhado com [Reações](gdd-quimica-reacoes.md) e [pH](gdd-quimica-acidos-bases-ph.md), enquanto o modelo de concentração permanece próprio desta sala.

## Percurso e aceite

Preparar duas soluções de volumes iguais e massas diferentes; diluir uma porção; adicionar soluto até observar material não dissolvido. Aceite: conservação de volume/soluto nas transferências dentro da simplificação declarada; unidade e fórmula coerentes; saturação não excede o dado cadastrado; frascos não transbordam sem feedback; vista de partículas acompanha o estado calculado; interação e leitura verificadas em XR. Nenhum build sem pedido.
