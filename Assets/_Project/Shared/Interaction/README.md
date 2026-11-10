# Motion de interação compartilhado

Aplicado em 09/11/2026 aos controles de Física, Matemática, Tabela periódica e Hub, preservando os feedbacks DOTween existentes.

- `ControlMotion`: contorno decorativo sem raycasts, transição de 0,12 s e pulso de 0,22 s. Não reparenta texto nem escala a área clicável. Recebe pointer, foco e mudanças de Button/Toggle/Slider/Dropdown; restaura por `UIVisibility` e OnDisable. Em repouso não percorre ancestrais nem recalcula o contorno.
- `XRInteractionMotion`: contorno dos bounds dos colliders no hover/seleção/soltura. Não altera o objeto, Rigidbody, domínio ou collider. Não é aplicado às superfícies de teletransporte ou aos cubos que possuem implementação específica.
- `InteractionMotionBootstrap`: instala no carregamento de cena e na inscrição de interactables XR criados durante a execução. Não faz varreduras por frame. Novos Selectables criados após a carga precisam de `Ensure(control)` ou de prefab com feedback.
- `MotionPreferences`: preferência local persistida. O modo reduzido mantém realces instantâneos. Na Tabela periódica há um toggle; a opção permanece nas outras salas. Não pausa movimento científico.

`PeriodicElementCube` usa DOTween para cor, escurecimento e pulso do contorno. A grade, o texto e os alvos continuam estáveis. `ButtonTweenFeedback` e `SliderTweenFeedback` também respeitam a preferência; o bootstrap evita duplicá-los.

A ferramenta Editor `ReferenceAndMotionMigration` aplica componentes pontualmente em cenas existentes. Não usar builders de sala para atualizar feedback. `InteractionMotionVerification` verifica cobertura das cenas em Play Mode, sem gravar alterações. A validação no Editor não comprova ergonomia ou desempenho em headset.
