# Funções e gráficos

Sala individual de Matemática: funções afins e quadráticas em um plano XY de 6 × 4 m, com 0,5 m por unidade, janela x ∈ [−6, 6], y ∈ [−4, 4] e origem a 2,4 m de altura. A profundidade da sala não representa outra variável matemática.

Entrada: **Explorar sozinho → Matemática → Funções e gráficos**. Cena: `Scenes/Functions.unity`. [GDD](../../../../../docs/gdd-funcoes-e-graficos.md).

## Como usar

1. Escolha Afim ou Quadrática no console lateral. Cada família conserva seus coeficientes durante a visita.
2. Gire as manoplas oficiais Unity UI_3D ou use − / + para passos de 0,1. O controle c aparece somente na quadrática. Limites: a entre −3 e 3; b e c entre −5 e 5.
3. Agarre o ponto P com interação XR direta ou à distância e mova-o horizontalmente. O valor y acompanha f(x). Os botões x − / + permitem ajustar a abscissa em passos de 0,1.
4. Alterne a grade e os destaques de raízes/vértice. Resultados fora da janela são informados no console; a escala não muda automaticamente.
5. Use Guardar referência para fixar uma curva tracejada e sua expressão. Alterar a função ativa não altera essa cópia. Substituir a referência requer confirmação.
6. Restaurar função recupera o exemplo inicial da família e oferece Desfazer restauração. Voltar ao Hub informa a perda dos ajustes quando há trabalho na visita.

Quando P sai da janela após alterar os coeficientes, sua coordenada permanece no console e o marcador é ocultado; não é deslocado artificialmente até a borda. As legendas de P e dos pontos analíticos acompanham suas posições. Projeções tracejadas ligam P aos eixos.

## Modelo e reuso

- `Model/FunctionModel.cs`: coeficientes, avaliação, raízes analíticas, vértice, recorte em intervalos visíveis, estado por família, referência e desfazer restauração. Assembly independente dos componentes de apresentação.
- `Scripts/FunctionsLab.cs`: comandos comuns a botões/manoplas, leituras e confirmações. Bloqueia UI e instrumentos durante modais.
- `Scripts/FunctionPlot.cs`: atualização da geometria científica, projeções e rótulos; malhas reutilizadas durante a sessão e liberadas ao sair.
- `FunctionMarker` deriva de XRGrabInteractable; usa o referencial mundial do gráfico mesmo quando o Toolkit remove temporariamente o pai durante a pega.
- `FunctionKnob` reutiliza XRKnob e o visual oficial em `Shared/XR/Controls`, com o adaptador visual existente. Não há manopla feita de primitivas substituindo o modelo oficial.
- Sala, rig, locomoção, iluminação/renderização e componentes uGUI/TMP reaproveitados. O antigo mockup Funções foi atualizado no catálogo; a disciplina continua com cinco entradas e quatro salas implementadas.
- `FunctionsAuthoring` é um instalador de primeira autoria e recusa uma sala já existente. Ajustes posteriores devem editar os objetos na cena, preservando transformações e referências.

Curvas são amostradas para desenho; raízes e vértice são calculados analiticamente. O recorte divide a curva nas interseções com a janela, sem criar segmentos falsos ligando ramos separados. Tangências à borda preservam o ponto de contato. Valores são arredondados apenas na leitura.

Com a = 0, a quadrática passa a afim/constante e perde o vértice. A função nula possui todos os reais como raízes; uma constante não nula não possui raízes. A aparência da curva de referência é tracejada e identificada por texto, além da cor.

## Escopo e validação

Modo individual offline. Sem sincronização LAN, editor livre de expressões, cálculo diferencial, histórico múltiplo ou progresso persistente. A autoridade LAN existente permanece inalterada.

Validação no Editor em 04/11/2026: 16 testes matemáticos aprovados em `Tests/FunctionTests.cs`; verificações de estados em Play Mode; seleção de família, toggles e incrementos via raycast XR; seleção/arraste de P e giro da manopla com interator XR temporário; confirmações e ida/volta pelo Hub com um único rig. Capturas locais em `Temp/FunctionsQA/`, ignoradas pelo Git.

A validação no Editor não certifica alcance, leitura, desempenho ou conforto em headset. Esses testes físicos permanecem pendentes. Nenhum build foi gerado.
