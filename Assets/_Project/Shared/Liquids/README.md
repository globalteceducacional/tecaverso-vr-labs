# Apresentação de líquidos

`LiquidPresentation` adapta o Shader Graph original da [amostra Unity](../../../ThirdParty/UnityLiquidSample/README.md). Não calcula química nem transfere material: recebe `fill`, `tint` e `paused` do modelo da sala.

O material da sala usa a variante derivada `TransparentLiquid.shadergraph`, com superfície transparente e `_Opacity` (0–1). Em 10/11, o limiar de alpha clip da variante foi reduzido de 0,5 para 0,001: a máscara geométrica continua descartando o volume acima da superfície, mas líquidos incolores podem usar opacidade 0,16 sem desaparecer. O gráfico do fornecedor permanece intacto em ThirdParty. Essa variação torna o precipitado interno observável; não representa absorção óptica ou refração física. O padrão do componente continua 0,65; apenas os instrumentos de Reactions foram recalibrados.

O instalador gera amostras de volume dentro da malha fechada, por interseções de triângulos. Em execução, a orientação transforma essas amostras e o quantil do volume define o corte horizontal do shader. Assim, inclinar um frasco não aplica simplesmente a altura original no eixo local. O nível é uma aproximação visual discretizada; a quantidade numérica é a fonte de verdade. Atualizações em repouso são reutilizadas, sem criar materiais por frame.

O script reproduz a equação do gráfico Unity: Y relativo remapeado pelos limites mundiais, mais os termos X/Z de oscilação. `ReachesMouth` verifica se a superfície chega ao bico antes do despejo. O wobble é amortecido, pausa junto à experiência e é removido pela preferência `MotionPreferences.Reduced`; o plano do líquido continua respondendo à orientação do recipiente.

Pausar não altera a câmera ou o XR Grab. Não adicionar o `LiquidWobble` original ao mesmo objeto: ele disputaria as propriedades do shader e instanciaria materiais.

Usado inicialmente em Reactions. Compatibilidade visual conferida no Editor URP; validação estéreo, transparência e custo de GPU/CPU em headset ainda pendentes.
