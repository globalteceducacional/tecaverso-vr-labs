# Short GDD — Frações e proporções

**Status:** proposta para revisão; implementação não autorizada por este documento.  
**Data:** 04/11/2026 · Matemática · Exploração individual offline · Sessão sugerida: 10–15 minutos.

## Conceito e objetivo

Uma sala de montagem em que frações são peças que o aluno pode pegar, encaixar e comparar. Reconhecer a unidade de referência, equivalência, soma e subtração por composição e relações entre fração, decimal, porcentagem e razão.

O primeiro escopo usa barras de mesmo comprimento para representar um inteiro. A comparação depende de um referencial constante; nenhuma peça cresce automaticamente para preencher um espaço.

## Sala e objetos

- Duas molduras verticais paralelas, A e B, com início alinhado. Cada moldura comporta dois inteiros consecutivos, claramente separados: 1 inteiro = 1,2 m; capacidade total = 2,4 m por linha.
- As linhas ficam em altura de manipulação, separadas o bastante para não sobrepor mãos, peças ou textos. O aluno pode observar diretamente ou operar pelo raio XR.
- Dispensadores laterais oferecem peças de 1, 1/2, 1/3, 1/4, 1/5, 1/6, 1/8, 1/10 e 1/12. Cada peça traz a fração escrita; cores ajudam a reconhecer denominadores, mas não substituem os rótulos.
- Área de devolução identificada e botão “Recolher peças soltas” para recuperar itens fora de alcance.
- Uma estação lateral de receitas usa fichas grandes e recipientes de encaixe para duas categorias, identificadas por símbolo e nome. Representa quantidades discretas, sem líquido ou mistura física.
- Fórmulas e valores aparecem junto à montagem. Um pequeno conjunto de opções de visualização e retorno ao Hub fica na entrada.

## Montagem de frações e interação XR

O aluno pega uma peça no dispensador; ao retirar, outra fica disponível. A pega pode ser direta ou por raio, usando aproximação/afastamento pelo analógico quando suportado pelo componente compartilhado. Peças têm rotação e posição alinhadas ao encaixar, sem alterar seu comprimento.

As molduras usam zonas de encaixe XR compatíveis com larguras diferentes. Uma prévia indica a posição final. Soltar perto da ponta livre acrescenta a peça à sequência; uma peça incompatível com o espaço restante não encaixa e apresenta “Não cabe neste espaço”. A peça continua disponível na mão ou retorna ao dispensador se abandonada fora da área válida.

Peças encaixadas continuam agarráveis. Ao retirar uma do meio, as seguintes se reorganizam brevemente para a esquerda, preservando ordem, comprimento e soma, sem mover peças que estejam sendo seguradas. A moldura mostra a expressão da composição e seu total simplificado.

Exemplos: 1/4 + 1/4 = 1/2; 1/3 + 1/6 = 1/2; 3/2 = 1 + 1/2. A escrita original das peças permanece visível, mesmo que o total seja simplificado. Comparar A e B destaca se os comprimentos são iguais ou qual deles é maior. Não soma denominadores como se fossem numeradores.

## UI e controles por tarefa

| Controle | Estado inicial | Função |
|---|---|---|
| Exibir divisões | Ligado | Mostra marcas das partes em cada peça; não impõe uma subdivisão comum minúscula a toda a régua |
| Exibir decimal e porcentagem | Desligado | Acrescenta os valores ao total de A e B |
| Mostrar equivalência | Desligado | Liga as extremidades das duas montagens e destaca igualdade ou diferença |
| Desfazer | Disponível após mudança | Reverte a última adição/remoção nas molduras, até 10 ações |
| Recolher peças soltas | Ação local | Devolve somente peças não encaixadas |
| Limpar montagem | Ação local | Limpa A e B após confirmação, preservando a receita lateral |
| Voltar ao Hub | Navegação | Informa perda das montagens da visita e permite cancelar |

Não há slider de numerador ou denominador: as peças são os controles. Não há seletor para escolher uma fração antes de poder enxergá-la. Mostrar resultados próximo das linhas, sem painel geral de edição.

## Estação de razão e proporção

Receita inicial: **2 fichas da categoria A para 3 da categoria B**. Fichas representam contagens de itens, não comprimentos. O aluno pega e encaixa fichas nos dois recipientes, com leitura imediata: A : B, total de fichas e participação de cada categoria no total.

- Botões “1×”, “2×” e “3×” selecionam somente a referência desejada: 2:3, 4:6 ou 6:9. O aluno monta a quantidade; a seleção não preenche automaticamente os recipientes.
- Mostrar a razão reduzida da montagem e a mensagem “Mesma proporção” quando aplicável, distinguindo-a de “Quantidade da referência atingida”. Por exemplo, 4:6 mantém a proporção, mas não corresponde ao alvo 1×.
- Limite de 10 fichas por categoria. Peças são retiradas individualmente ou devolvidas por “Limpar receita”, com confirmação.
- Fórmula local: participação de A = A / (A + B). Em 2:3, A representa 2/5 = 40% do total, e não 2/3 do total.
- Recipientes vazios não recebem porcentagens indefinidas. Se somente uma categoria estiver presente, mostrar 0%/100% e “Receita incompleta”; não dividir pela contagem zero ao reduzir a razão.

As duas estações ficam disponíveis na mesma sala, sem troca de modo ou ocultação de objetos.

## Precisão, limites e feedback

Usar números racionais para cálculos, equivalência e capacidade dos encaixes. A posição gráfica é derivada do valor racional; tolerância física do socket não determina igualdade matemática.

Decimal e porcentagem podem ser arredondados: mostrar “≈” quando necessário, como 1/3 ≈ 0,333 e ≈ 33,3%. O inteiro de referência permanece explícito; 3/2 corresponde a 150% de um inteiro, mesmo ocupando apenas parte da moldura de dois inteiros.

Até 24 peças encaixadas por linha (24 × 1/12 = 2) e poucas peças soltas simultaneamente. Dispensadores usam reposição controlada. Feedback de encaixe combina contorno, som/haptic opcional e texto breve; não exige precisão motora excessiva. Escala de pega pode ser maior que a peça sem sobrepor alvos vizinhos.

## Percurso sugerido

1. Colocar 1/2 na linha A e duas peças de 1/4 em B; ligar equivalência e observar os extremos alinhados.
2. Limpar e comparar 1/3 + 1/6 com 1/2; perceber que denominadores diferentes também compõem o mesmo comprimento.
3. Montar três metades e relacionar 3/2, 1,5 e 150% com o inteiro marcado.
4. Retirar uma metade e observar a subtração e o novo total.
5. Na receita, montar 2:3 e depois 4:6; comparar razão constante, quantidade total e porcentagem de cada categoria.

## Reuso, limites e aceite

Reutilizar rig, locomoção, XR Grab Interactables, sockets compatíveis, controles e materiais compartilhados, TMP e renderização. Seguir [UI](../UI_GUIDELINES.md), [design system](../DESIGN_SYSTEM.md) e [escopo](../README.md). Dimensões e alcance serão conferidos no headset, incluindo uso sentado.

Fora do primeiro escopo: líquidos, discos fracionários, frações negativas, multiplicação/divisão geral de frações, porcentagens financeiras, editor livre de denominadores, persistência e LAN. Atividades são sugestões, sem pontuação ou bloqueio sequencial.

Aceite futuro: unidade comum preservada; encaixes sem sobreposição; peças pequenas acessíveis por raio; igualdade racional e simplificação corretas; frações impróprias e arredondamento bem identificados; receita distingue razão de participação no total; desfazer coerente e recuperação de peças fora de alcance. Validar matemática no Editor e ergonomia física separadamente. Builds somente por solicitação.
