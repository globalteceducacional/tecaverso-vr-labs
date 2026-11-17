# Padrão de feedback de interação 3D

**Aprovado:** padronização e correção das salas anteriores solicitadas em 17/11/2026. Escopo: Física, Matemática, Química e Biologia. Esta revisão visual não altera os experimentos, gestos ou controles dos GDDs individuais.

**Execução:** implementado no componente e renderer compartilhados, incluindo as salas anteriores. [Validação e capturas reais](interaction-feedback-validation.md). A prévia abaixo continua identificada como proposta esquemática.

## Objetivo

Reconhecer a peça interativa sem exibir caixas dos colliders, encobrir detalhes ou alterar cores científicas. O grupo semântico recebe uma silhueta única; as submalhas não ganham caixas separadas. Hover e seleção não empilham efeitos. Objetos parcialmente ocultos só destacam a parte visível.

## Inventário de interfaces e estados

Nenhum HUD, painel, rótulo, botão ou gesto novo. Permanecem os inventários de cada laboratório.

| ID | Visual e localização | Finalidade / entrada | Inicial / saída | Simultaneidade |
|---|---|---|---|---|
| F1 | Silhueta azul na superfície do alvo XR | Foco por raio/proximidade | Oculta; remove ao sair/desabilitar | Um efeito por alvo; seleção tem prioridade |
| F2 | Silhueta âmbar no objeto consultado | Seleção persistente solicitada pelo domínio | Oculta; fecha/troca consulta | Substitui F1 no mesmo alvo; sem piscar continuamente |
| F3 | Silhueta azul suave na peça em pega | Confirmar manipulação sem cobrir o objeto | Somente durante pega; socket não conta como mão | Menor intensidade; não somar caixa de seleção |
| F4 | Indicação de origem/encaixe já prevista no GDD | Devolução da peça | Condicional ao estado da atividade | Preservar filtros, recusa e texto existente; não introduzir indicação em galerias sem montagem |

Botões uGUI mantêm `ControlMotion` ou feedback próprio. Manoplas/alavancas realçam a peça móvel; frascos não realçam o líquido ou a impressão do rótulo. Instâncias futuras usam o mesmo componente compartilhado e um alvo visual explícito quando o grupo automático não representa a ação.

## Prévia anterior à implementação

Proposta esquemática; não é captura da Unity. Cabeçalho, cartões e botão de teste desta página são ferramentas de revisão externas ao aplicativo.

![Repouso, foco, consulta e manipulação](../GeneratedMedia/GDD/interaction-feedback/states.png)

[Fonte HTML offline](prototypes/interaction-feedback/index.html). Reprodução: `node Tools/interaction-feedback/capture.cjs`; usa o Chrome headless e dependências já presentes em `Tools/tecaverso-demo`. Capturas ignoradas pelo Git; acompanhar o documento ao compartilhar.

## Implementação e preservação

Substituir o visual no componente compartilhado, preservar GUIDs e componentes serializados e cobrir criação runtime. Remover a seleção retangular adicional de Células e a decoração duplicada dos cubos periódicos. Não remover linhas de gráficos, vetores, réguas, órbitas, arestas ou indicações de encaixe.

Renderização deriva da geometria atual, sem alterar materiais, colliders, transforms, Rigidbody, câmera, entrada ou rede. Fade de 0,12 s em tempo não escalado; preferência reduzida usa transição imediata. Ocultar/desabilitar remove foco, pulso e registros ativos. Não alocar materiais por objeto/frame.

Sockets não contam como foco/pega do usuário. Dois ponteiros são contados separadamente. Cobertura da forma e visibilidade são máscaras distintas, evitando destacar o contorno de rótulos e de objetos que a encobrem. Líquidos e superfícies com deformação/recorte científico não devem ser usados como alvo automático de realce.

## Critérios de aceite

- Todas as cenas habilitadas e os objetos criados em execução recebem o padrão, com exceções explícitas para feedback próprio equivalente e teletransporte.
- Ausência das antigas caixas em foco, seleção e soltura; nenhuma duplicação no mesmo alvo.
- Verificar repouso, hover/saída, pega/soltura, seleção persistente, troca de alvo, desabilitação, mudança de cena e motion reduzido.
- Conferir uma peça orgânica, um frasco, um controle mecânico e um objeto matemático; geometria, cores e interação preservadas.
- Editor/console e capturas reais separados da prévia. MSAA e estéreo previstos na implementação; conforto/desempenho em headset exigem teste físico. Sem build nesta tarefa.
