# Motion de interação compartilhado

Aplicado em 09/11/2026 aos controles de Física, Matemática, Tabela periódica e Hub, preservando os feedbacks DOTween existentes.

- `ControlMotion`: contorno decorativo sem raycasts, transição de 0,12 s e pulso de 0,22 s. Não reparenta texto nem escala a área clicável. Recebe pointer, foco e mudanças de Button/Toggle/Slider/Dropdown; restaura por `UIVisibility` e OnDisable. Em repouso não percorre ancestrais nem recalcula o contorno.
- `XRInteractionMotion`: estado compartilhado de foco, pega e seleção persistente. Reúne as malhas da peça semântica; Knob/Lever usam o handle. Não cria LineRenderer nem caixas de bounds. Sockets não acendem foco do usuário; dois ponteiros mantêm referências independentes. Limpa ao desabilitar e respeita motion reduzido.
- `InteractionSilhouetteFeature`/`InteractionSilhouette.shader`: Render Graph URP, máscara do conjunto e máscara de visibilidade, contorno fino antes dos transparentes/UI. Separar cobertura de visibilidade evita bordas falsas nos rótulos de frascos e nos limites de oclusão. MSAA/HDR atuais preservados; texturas herdam dimensão/slices XR. Não há materiais por objeto/frame ou alteração dos materiais originais. Os passes só são enfileirados quando existe realce ativo.
- `InteractionMotionBootstrap`: instala no carregamento de cena e na inscrição de interactables XR criados durante a execução. Não faz varreduras por frame. Novos Selectables criados após a carga precisam de `Ensure(control)` ou de prefab com feedback.
- `MotionPreferences`: preferência local persistida. O modo reduzido mantém realces instantâneos. Na Tabela periódica há um toggle; a opção permanece nas outras salas. Não pausa movimento científico.

`PeriodicElementCube` mantém DOTween para cor/escurecimento, mas usa a mesma silhueta compartilhada para foco/seleção; a decoração antiga permanece inativa. Células envia seleção persistente e desliga o LineRenderer retangular legado. Gravitação comunica o planeta selecionado. A grade, o texto e os alvos continuam estáveis. `ButtonTweenFeedback` e `SliderTweenFeedback` de uGUI permanecem próprios, sem duplicação.

A ferramenta Editor `ReferenceAndMotionMigration` aplica componentes pontualmente em cenas existentes. Não usar builders de sala para atualizar feedback. `InteractionMotionVerification` verifica cobertura das cenas em Play Mode, sem gravar alterações. A validação no Editor não comprova ergonomia ou desempenho em headset.

## Autoria e aceite — 17/11/2026

Todas as disciplinas seguem [o mesmo padrão e sua prévia](../../../../docs/gdd-interaction-feedback.md). `InteractionFeedbackMigration.Install()` deriva o renderer existente para `Shared/Rendering/TecaversoVRRenderer.asset`, instala o feature e aponta o pipeline próprio para ele. Não altera o renderer de terceiros, cenas, posições ou parâmetros científicos. Componentes antigos conservam o GUID; o bootstrap cobre também novas instâncias XR.

O alvo automático exclui TMP, LineRenderer, partículas, líquido de `LiquidPresentation` e malhas com nomes de rótulo. Se uma importação usar nomes genéricos ou shaders que deformam geometria, definir `visualRoot`/`visualTargets` explicitamente e revisar a silhueta. Não transformar essa limitação em caixas de fallback. Use `SetPersistentSelected(bool)` para consulta persistente; a pega não cria uma seleção permanente sozinha.

Execute `InteractionFeedbackVerification.Start()` no Editor em Play Mode: cobertura das cenas habilitadas, eventos sintéticos de foco, hover de socket, dois ponteiros, saída, seleção, desabilitação, integridade de materiais/transforms e criação runtime de ponto cartesiano. Capturas aproximadas usam câmera transitória exclusiva da verificação, não uma câmera nova do aplicativo. Evidências em `GeneratedMedia/InteractionFeedback/`, ignoradas. Testes de headset/estéreo e custo GPU continuam separados; nenhum build nesta alteração.
