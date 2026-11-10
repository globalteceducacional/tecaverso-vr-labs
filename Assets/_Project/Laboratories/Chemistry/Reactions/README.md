# Reações químicas

Sala individual offline: `Scenes/Reactions.unity`, Hub → Química → Reações químicas. Reformulação aprovada do [GDD](../../../../../docs/gdd-quimica-reacoes.md): **12 reagentes, seleção física A/B, 12 receitas e consulta opcional**. Sem API ou sincronização LAN.

## Como usar

1. Explore os quatro grupos da mesa: três ácidos, três bases, três sais e três óxidos em quatro colunas e três degraus. Fórmula, nome e concentração/estado estão impressos em rótulos curvos no material dos recipientes. Estoque e disponibilidade ficam na borda fixa da prateleira; colheres têm fórmula no cabo, sem painel transportado.
2. Pegue um frasco ou pote com grip, por raio ou proximidade, e **solte no suporte A**. Os parceiros cadastrados recebem realce e marca “B disponível”.
3. Encaixe um parceiro no suporte B. Depois de fechar o par, pode retirar ambos e dosar em qualquer ordem; A e B permanecem registrados nos suportes.
4. Incline o frasco sobre o béquer de 500 mL. A vazão didática chega a 40 mL/s. Use a colher dedicada para sólidos: encoste a concha na abertura do pote, ainda na posição de coleta, e depois incline sobre o béquer. Capacidade de 1 g, vazão 0,8 g/s.
5. Consulte a ficha quando desejar ler equação, doses, produtos, excesso/limitante e perdas. Fechar ou pegar um instrumento recolhe a ficha. Não há painel de coeficientes nem visor molecular permanente.
6. No par HCl/NaOH, pegue o conta-gotas, aproxime o bico do béquer e use **ativar** para adicionar indicador em traço. Antes disso a base não fica rosa. Indicador indisponível nos demais pares.
7. Trocar par funciona sem confirmação antes de qualquer uso. Nova mistura, depois de consumo/perda/indicador, pede confirmação de descarte e reposição. Cancelar conserva tudo. Devolva os instrumentos antes de reiniciar ou sair.

Antes de B, retirar A desfaz a escolha. Após fechar A/B, retirar um frasco libera a dosagem e preserva o par. Um terceiro reagente, uma colher de outra substância ou um par fora do catálogo são bloqueados **antes de consumir estoque/emitir fluxo**. Isso indica o limite do simulador, não impossibilidade química. Somente reagentes habilitados vertidos fora do recipiente contam como perda.

Instrumentos soltos retornam após 10 s, preservando estoque. Frascos segurados ou encaixados não são teleportados. Reinício espera a conclusão da liberação dos sockets antes de reativá-los, evitando recaptura por contatos antigos. Toda interação reutiliza Shared/Interaction e movimento reduzido.

## Identificação física dos reagentes

Frascos e colheres não carregam Canvas/HUD. Treze rótulos (12 reagentes e indicador) usam materiais opacos sobre uma faixa curva aderente ao vidro, sem exceder a silhueta. Cinco colheres têm identificação impressa no cabo. As antigas referências de texto dos frascos apontam agora para placas fixas na borda dos degraus; a massa coletada aparece na placa do pote correspondente. Estado do líquido, pivôs, colliders, pontos de pega e fluxo A/B foram preservados.

Texturas de runtime em Art/Labels, com seus materiais, malhas e .meta, devem ser versionadas. Gerador offline: Tools/generate-reagent-labels.ps1, usando a base local e Montserrat. ReactionsLabelAuthoring.Upgrade() executa a migração uma vez e preserva uma cópia anterior em Temp/ReactionsMigration. Prévias documentais continuam ignoradas pelo Git.

## Bancada e termômetro

Três degraus de 3,9 × 0,58 m, a 0,92 / 1,17 / 1,42 m. Quatro colunas por categoria, com deslocamento lateral entre níveis para reduzir sobreposição. Frente de trabalho e circulação lateral; posições do béquer, A/B, rig e ficha preservadas. Sonda com carcaça chanfrada e haste modeladas em ProBuilder, visor no próprio instrumento e pega XR por raio/proximidade.

Pegue a sonda e mergulhe apenas a ponta metálica. O visor mostra temperatura atual, inicial e variação; fora do líquido, apaga valores e pede imersão. Pode soltá-la imersa para dosar sem segurá-la. Não é um reagente e não entra nos suportes A/B. Reinício/saída são bloqueados enquanto estiver segurada. Movimento e preferência reduzida reutilizam Shared/Interaction.

**Cobertura térmica:** HCl/NaOH e HCl/KOH apenas. Outros pares mostram “Modelo térmico indisponível”; não se afirma ausência de calor. T₀ = 25 °C; Q = 55.830 × extensão (mol), em joules; T = 25 + Q/(80 + 4,18 × volume aceito em mL). Densidade aproximada 1 g/mL, capacidade térmica didática do béquer 80 J/K, soluções a 25 °C. Estimativa ideal sem perdas para o ambiente, calor de diluição ou cinética; não há vapor, resfriamento animado ou pH corrigido por temperatura. 100 mL de cada solução a 0,10 mol/L resultam em aproximadamente 25,61 °C. Perdas não geram calor no recipiente, e adição em excesso pode reduzir a elevação de temperatura por aumentar a capacidade térmica.

Fonte local documentada: OpenStax Chemistry 2e, [Apêndice G](https://openstax.org/books/chemistry-2e/pages/g-standard-thermodynamic-properties-for-selected-substances): ΔHf° de H₂O(l) = −285,83; OH−(aq) = −230,0; H+(aq) = 0 kJ/mol. ΔH° da neutralização = −55,83 kJ/mol. Nenhuma consulta à internet ocorre em execução.

## Catálogo

| Grupo | Reagentes |
|---|---|
| Ácidos | CH₃COOH, HCl, H₂SO₄ diluído |
| Bases | NaOH, KOH, Mg(OH)₂ |
| Sais | NaHCO₃, CaCl₂, Na₂CO₃ |
| Óxidos | MgO, CuO, ZnO |

Sete soluções a 0,10 mol/L, 300 mL cada; cinco sólidos de 6 g, com colheres dedicadas. Mg(OH)₂ é sólido pouco solúvel; não se oferece solução fictícia. CuO é preto; os outros sólidos de partida são brancos. As 12 equações e os parceiros por identidade estão no GDD e na base local. Cada substância participa de ao menos uma receita; não há mistura multicomponente ou reaproveitamento de produtos.

## Modelo e limites

- Extensão estequiométrica, conservação de átomos, estoques, doses aceitas, perdas e excesso. Volume aditivo; sem deslocamento de sólidos, contração, cinética ou equilíbrio detalhado.
- O líquido excedente de entrada é contabilizado como perda; não se simula escoamento da mistura já formada. Gás liberado permanece no balanço. Água formada exclui solvente.
- pH ideal a 25 °C **somente para HCl/NaOH**, incluindo autoionização. Fenolftaleína explícita, em traço, transição 8,2–10. Repetir a ativação não intensifica a cor. Não mostrar pH fictício nos demais pares.
- H₂SO₄/NaOH: balanço formal da neutralização completa; a ficha usa equivalentes ácidos/básicos restantes e informa que o equilíbrio bissulfato/sulfato não foi modelado. Não é uma curva de titulação.
- CuO/H₂SO₄: consumo do sólido preto, solução azul; sem precipitado azul nem bolhas. Tempo de dissolução e intensidade de cor são didáticos, sem aquecimento ou cristalização.
- Reutilizam-se as malhas, o Shader Graph Unity e os efeitos existentes. O volume controla nível/wobble; VFX não calcula química. As sete soluções de partida têm aparência incolor, com opacidade 0,16; o azul aparece apenas na formação de solução de cobre. A suspensão branca do precipitado se dissipa visualmente (escala didática de 3 s), deixando depósito distinguível do líquido. A mudança de cor e a transparência não constituem espectrofotometria.

## Arquitetura

- `ReactionDefinition`: 12 receitas, espécies, átomos, coeficientes, evidência e massa molar do sólido. Os primeiros três índices preservam compatibilidade do modelo original.
- `ReactionSession`: cálculo de uma receita; independente de Unity.
- `ReactionWorkbench`: catálogo, escolha A/B e barreira de entrada por identidade. **Todas as transferências da nova sala passam por essa API**.
- `ReactionThermometer`: detecção física de imersão e leitura do modelo térmico; sem dosagem química.
- `ReagentSocket`: encaixe físico filtrado, recusa explicada e callbacks de seleção.
- `ReagentTool`: XR Grab, bico, concha, fluxo existente e retorno. Na sala migrada delega a interação ao coordenador.
- `ReactionsBench`: coordena UI compacta, instrumentos, confirmação, indicador e apresentação por estado.
- `ReactionsLab`: mantém referências serializadas do béquer/efeitos e compatibilidade do autor original; na sala A/B delega ao coordenador. A UI antiga foi retirada da cena.
- `Shared/Liquids/LiquidPresentation`: adaptador visual reutilizado; variante transparente com limiar de alpha clip 0,001 para permitir soluções de opacidade 0,16. O gráfico original Unity e o cálculo de nível/wobble foram preservados.
- `ReactionsAuthoring.UpgradeToBench()`: migração única e focada; recusa cenas sujas, Play Mode e segunda execução. Preserva ambiente/rig/béquer; backup local em `Temp/ReactionsMigration/`. Ajustes posteriores são pontuais.

## Verificação

- Modelo: **36 testes Edit Mode aprovados**, incluindo receitas, 66 pares distintos, igualdade A/B, entradas inválidas, conservação, estoques, pH, indicador e energia térmica.
- Integração XR: roteiro aprovado em Play Mode em 10/11/2026, sem erros no Console, com quatro receitas, casos inválidos e leitura sem truncamento. Executar `Tecaverso.Editor.ReactionsBenchValidation.Start()` para repetir. Resultado e evidências em `Temp/ReactionsBenchValidation/report.txt`. Inclui pega, imersão, retirada e repouso da sonda; calor nas doses XR de HCl/NaOH e indisponibilidade explícita em CuO/H₂SO₄. Inclui liberação física nos sockets, identidade aceita, tentativa ácido/ácido, terceiro reagente, transferência, colher, indicador, consulta e confirmações. Retorno real ao Hub pelo botão também verificado em Play Mode.
- A validação antiga `ReactionsValidation` e o vídeo de três casos não certificam o novo fluxo. `ReactionsDemo` foi atualizado para a seleção A/B: precipitação, gás com colher, indicador explícito e consumo de CuO, além de terceiro reagente bloqueado, consulta e cancelamento/reinício.
- Leitura estéreo, alcance físico e desempenho continuam pendentes em headset. Nenhum build autorizado/executado nesta reformulação.

## Gravação reproduzível

O roteiro atualizado inclui a bancada em degraus e a sonda: leitura térmica após HCl/NaOH, retirada da ponta e indisponibilidade explícita em CuO/H₂SO₄. As gravações A/B anteriores não demonstram esses recursos. A gravação Rotulos-Fisicos mostra a versão corrigida, sem Canvas nos frascos ou colheres. O vídeo Degraus-Termometro é anterior a essa correção.

Em Play Mode nesta cena, executar `PhysicsDemoRecorder.Begin("Reactions", "Recordings/Chemistry/ReacoesQuimicas-Rotulos-Fisicos")`. O Unity Recorder captura 1920 × 1080 a 30 fps por 284 s, com câmera temporária e legendas exclusivas da gravação. O roteiro automatiza um interator XR: transporta, solta nos sockets, inclina frascos e dosa pela lógica real dos instrumentos; comandos da ficha usam os eventos dos botões. Não injeta quantidades ou resultados químicos e não representa captura de uma pessoa com headset. `ReactionsDemo.Evidence` registra verificações durante a execução; uma falha interrompe a captura. Câmeras e objetos temporários são restaurados/removidos ao terminar, sem salvar a encenação na cena.

Vídeos, roteiro com tempos e quadros de revisão ficam em `Recordings/Chemistry/`, ignorado pelo Git. A revisão deve conferir o arquivo MP4, não apenas uma captura do Editor.

## Créditos

As fontes científicas de cada receita estão no [GDD](../../../../../docs/gdd-quimica-reacoes.md). [Créditos da amostra Unity](../../../../ThirdParty/UnityLiquidSample/README.md). [Prévias de interface aprovadas](../../../../../docs/prototypes/reacoes-quimicas/README.md). Capturas, gravações e relatórios locais ficam em pastas ignoradas pelo Git.
