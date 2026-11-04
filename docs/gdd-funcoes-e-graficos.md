# Short GDD — Funções e gráficos

**Status:** aprovado pelo responsável e implementado em 04/11/2026; validação física em headset pendente.  
**Data:** 04/11/2026 · Matemática · Exploração individual offline · Sessão sugerida: 10–15 minutos.

## Conceito e objetivo

Uma sala onde o aluno transforma uma expressão algébrica e vê seu gráfico mudar imediatamente em escala espacial. Investigar como os coeficientes afetam inclinação, posição, concavidade, raízes e vértice. A relação central é **alterar → prever → observar → explicar**.

Primeiro escopo: função afim, f(x) = ax + b, e quadrática, f(x) = ax² + bx + c. São gráficos em um plano XY inserido na sala 3D; profundidade não representa uma terceira variável.

## Sala e objetos

- Plano cartesiano vertical com aproximadamente 6 m de largura e 4 m de altura, origem a cerca de 2,4 m do piso. Janela inicial x ∈ [−6, 6], y ∈ [−4, 4], com 0,5 m por unidade em ambos os eixos. Dimensões sujeitas à validação de leitura no headset.
- Eixos graduados, grade discreta, curva espessa e rótulos fora da região de circulação. O aluno observa do centro e pode aproximar-se pelas laterais.
- Console baixo ao lado do gráfico com seletores de família e manoplas oficiais Unity UI_3D, usando materiais do projeto. Cada manopla tem letra, valor e sentido de ajuste identificados.
- Na sala implementada, o console de seleção e leitura fica à esquerda; as três manoplas ficam na lateral direita, deixando a frente do gráfico livre.
- Um marcador XR preso à curva, com coordenadas (x, f(x)) e projeções tracejadas para os eixos.
- Expressão atual acima do gráfico; raízes e vértice identificados junto aos pontos quando solicitados. Legendas explicam os símbolos sem depender somente de cores.

## Interações XR e UI

| Elemento | Ação do aluno | Resposta |
|---|---|---|
| Seleção Afim / Quadrática | Apontar e selecionar | Exibe somente os coeficientes pertinentes; conserva os ajustes de cada família |
| Manoplas a, b e c | Agarrar e girar | Atualiza valor, expressão e curva enquanto gira |
| Botões − / + junto ao valor | Clicar com raio XR | Ajuste exato de 0,1, alternativa à pega direta |
| Marcador da curva | Agarrar diretamente ou à distância e deslocar horizontalmente | A coordenada x controla o ponto; y é calculado pela função |
| Exibir grade | Toggle, inicialmente ligado | Alterna a grade; eixos permanecem |
| Exibir raízes e vértice | Toggle, inicialmente desligado | Mostra os resultados aplicáveis à família atual |
| Guardar referência | Botão | Fixa uma cópia da curva e da expressão para comparação |
| Remover referência | Botão contextual | Retira a única referência guardada |
| Restaurar função | Botão | Retorna a família atual ao exemplo inicial; permite desfazer a restauração |
| Voltar ao Hub | Botão padrão | Sai; havendo ajustes/referência, informa que não serão salvos e permite cancelar |

Coeficientes: a entre −3 e 3; b e c entre −5 e 5, passo 0,1. Valores são adimensionais. Manopla e botões representam o mesmo estado; nenhum deles mantém valor independente. Padrões: afim a = 1, b = 0; quadrática a = 1, b = 0, c = 0.

O marcador usa a posição do alvo projetada no plano para obter x, limitado à janela e aos trechos visíveis. Não é um objeto solto, nem muda os coeficientes. Ao soltar, permanece no ponto. Uma pequena faixa com x − / +, passo 0,1, permite leitura precisa sem depender da destreza manual. Se o ponto sair da janela após editar a função, sua coordenada continua no console e surge “Ponto fora da janela”; nunca desenhar um valor falso na borda.

A referência permanece fixa enquanto a curva ativa muda; ambas têm rótulos e estilos de linha diferentes. Limite de uma referência. Substituir exige uma ação explícita. Trocar de família conserva os valores, remove os destaques inaplicáveis e mantém a referência identificada.

## Regras matemáticas e apresentação

- Calcular resultados a partir dos coeficientes, sem inferir raízes ou vértice pela malha renderizada.
- Quadrática com a ≠ 0: Δ = b² − 4ac; x do vértice = −b/(2a); y do vértice = f(x). Distinguir zero, uma ou duas raízes reais.
- Com a = 0, explicar que a expressão deixou de ser quadrática. Resolver como função afim ou constante; não dividir por zero nem manter um vértice fictício.
- Função constante não nula não tem raízes; função identicamente nula tem todos os reais como raízes. Na afim, b é o intercepto vertical e a determina a inclinação.
- Recortar curvas na janela fixa, sinalizando continuidade nas bordas. Raízes fora dela aparecem como coordenadas na leitura auxiliar. Não reescalar automaticamente e alterar a percepção da inclinação.
- Mostrar cálculos com precisão interna suficiente e arredondamento somente na apresentação. Desenho é amostrado; resultados numéricos são analíticos.

## Percurso sugerido

1. Começar com f(x) = x; mover o marcador e relacionar entrada e saída.
2. Alterar b para 2 e observar o deslocamento vertical.
3. Guardar essa curva; alterar a para −1 e comparar as inclinações.
4. Escolher Quadrática; variar a e observar abertura e concavidade.
5. Usar a = 1, b = 0, c = −1; ativar raízes e vértice e localizar as duas interseções com o eixo x.
6. Alterar c para 1 e explicar por que não há raízes reais.

O percurso é uma sugestão de mediação, sem bloqueio de etapas, avaliação automática ou pontuação.

## Reuso, limites e aceite

Reutilizar sala, rig, locomoção, controles XR oficiais compartilhados, uGUI/TMP, Montserrat e renderização. Seguir [diretrizes de UI](../UI_GUIDELINES.md) e [design system](../DESIGN_SYSTEM.md). Interface concentrada nos instrumentos, sem abas de edição ou estudo A/B.

Fora do primeiro escopo: digitação livre de expressões, funções exponenciais/logarítmicas/trigonométricas, cálculo diferencial, superfícies 3D, histórico múltiplo, persistência e LAN. A [autoridade coletiva atual](../README.md) permanece restrita ao lançamento oblíquo.

Critérios de aceite: coeficientes e marcador operáveis por XR; correspondência entre curva e fórmula; casos degenerados corretos; referência imutável; recorte sem segmentos falsos; estados preservados ao trocar família; retorno ao Hub. Verificar matemática no Editor e leitura/alcance no headset separadamente. Não gerar builds sem solicitação.

Implementação, uso e evidências no [README do laboratório](../Assets/_Project/Laboratories/Mathematics/Functions/README.md). Probabilidade e estatística e Frações e proporções continuam como propostas separadas; esta implementação não altera essas salas nem o controle LAN.
