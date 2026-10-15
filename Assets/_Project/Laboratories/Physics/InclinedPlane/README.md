# Plano inclinado

Laboratório individual: **Hub → Explorar sozinho → Física → Plano inclinado**.
Cena: `Scenes/InclinedPlane.unity`. Não há suporte LAN para este experimento nesta etapa; o lobby mantém o início coletivo bloqueado para esse conteúdo.

## Experiência

- Uma caixa de carga de madeira desliza da esquerda para a direita sobre uma rampa de 10 m, com percurso útil de 9,20 m e margens de 0,40 m nas extremidades.
- Controles: inclinação de 0° a 60°, massa de 1 a 10 kg, gravidade de 5 a 20 m/s² e coeficientes de atrito entre 0 e 1. O cinético não ultrapassa o estático; reduzir o estático limita também o cinético.
- Presets de superfície: sem atrito, baixo atrito (0,25/0,15) e alto atrito (0,65/0,45).
- Liberar, pausar/retomar, reiniciar, abas Parâmetros/Análise e retorno ao Hub com confirmação. Reiniciar preserva os parâmetros e recoloca o bloco no início.
- Antes da liberação, as forças exibem explicitamente a previsão ao liberar. Após a liberação os parâmetros ficam bloqueados até reiniciar.
- Ao chegar ao fim, a visualização congela os valores imediatamente anteriores à parada; não representa uma colisão ou uma força de batente.

## Forças e modelo

Peso vertical, normal perpendicular à rampa e atrito paralelo à superfície. Atrito estático equilibra apenas a componente necessária até `μsN`; quando esse limite é superado, a descida usa `μkN`. Massa modifica as forças, mas não a aceleração nem o ângulo crítico neste modelo.

A resultante é opcional. Decompor o peso substitui sua seta por `Ppar` e `Pperp`, em amarelo claro, sem somar componentes como forças adicionais. Os nomes evitam glifos não disponíveis na fonte atual; a aba Análise explica seu significado.

Há um único conjunto de seis views reutilizadas, ancorado na frente do bloco. **Não há pool, clones temporais ou trilha de vetores.** Setas nulas são ocultadas, preservando o rótulo numérico. A mesma escala se aplica a todas as forças; a escala máxima de 0,035 m/N é reduzida em conjunto para limitar o peso a 1,20 m de seta, e aparece explicitamente na interface. Comparações quantitativas de massas devem usar os valores em N e a escala indicada.

## Organização e reaproveitamento

- `Model/InclinedPlaneModel.cs`: cálculo analítico, equilíbrio, tempo, pausa e limite exato do percurso. Assembly próprio para testes sem GameObjects.
- `Scripts/InclinedPlaneLab.cs`: coordenação entre modelo, rampa, bloco e apresentação.
- `Scripts/InclinedPlanePanel.cs`: entrada, estados dos controles e navegação.
- `Scripts/InclinedPlaneForcesView.cs`: apresentação das forças, unidades e visibilidade.
- `Prefabs/ForceArrow.prefab`: geometria de haste/cone do lançamento oblíquo com `VectorArrowView.SetScaledVector`. O método original de velocidade permanece inalterado.
- Sala padrão, materiais de ambiente, rig XR, fontes Montserrat, controles e feedbacks existentes reutilizados. As transformações do rig e dos painéis foram preservadas. A sala desta cena foi ampliada para acomodar a rampa até 60°; o prefab compartilhado da sala permanece intacto.
- `Editor/InclinedPlaneAuthoring.cs`: instalação inicial, que recusa sobrescrever a cena existente. Refinamentos devem editar os objetos existentes.
- Catálogo de Física com paginação de seis cards, preservando os conteúdos anteriores.

## Validação — 15/10/2026

- Compilação no Editor sem erros.
- Sete testes EditMode aprovados: equilíbrio, limiar de atrito, efeito da massa, solução sem atrito, fim exato, pausa/reset e coerência dos coeficientes.
- Play Mode: liberar/pausar, controles bloqueados, opções de vetores, análise, reset, equilíbrio com alto atrito, fim sem atrito e confirmação de saída.
- Percurso laboratório → Hub → paginação → laboratório, com um único rig XR e sessão reiniciada.
- Capturas da cena e interfaces conferidas. Sem build e sem validação de conforto em headset.

## Refinamento visual — 15/10/2026

- Caixa de carga reutilizável em `Prefabs/WoodenCargoCrate.prefab`, com textura de madeira, tábuas, reforços diagonais e cantoneiras. Duas malhas/renderers compartilhados; dimensões e contato do modelo analítico preservados.
- `Meshes/ForceArrowCone.asset` mantém a malha do cone persistente. O prefab anterior tinha a referência da ponta vazia. Setas pequenas afunilam a haste para manter o cone visível; o diagrama fica à frente do corrimão, evitando ocultação do peso.
- Rampa de 10 m, régua a cada 0,5 m e percurso útil calculado como comprimento menos as duas margens. Na configuração inicial (30°, μk = 0,30, g = 9,81), a descida leva aproximadamente 2,79 s, contra 1,89 s anteriormente, sem modificar o relógio físico.
- Perfil gráfico comum em `Shared/Rendering`, com MSAA 4× e pós-processamento discreto. Detalhes e reutilização no README dessa pasta.
- Verificação após o refinamento: sete testes EditMode aprovados; Play Mode confirmou descida, pausa/retomada, fim em 9,20 m, reset, equilíbrio horizontal, montagem a 60°, seis views fixas e cones presentes na decomposição com resultante. Nenhuma malha ou script ausente na cena; console sem erros. Rig e Canvas mantêm os transforms anteriores.
