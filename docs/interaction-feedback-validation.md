# Validação do feedback compartilhado — 17/11/2026

**Implementado nas salas anteriores e obrigatório para as próximas**, conforme [padrão e prévia](gdd-interaction-feedback.md). Não é homologação de hardware.

## Escopo verificado

Regressão focada no Editor/Play Mode em **17 cenas habilitadas: 16 laboratórios e Hub**. Inventário total: **262 componentes XR e 501 controles uGUI**, incluindo templates/objetos inativos. Verificação de cobertura não significa execução individual de todas as ações desses controles.

| Disciplina | Cenas |
|---|---|
| Física | Lançamento oblíquo, Plano inclinado, Gravitação, Resistores, Termologia, Ondulatória |
| Matemática | Espaço cartesiano, Formas geométricas, Geometria plana, Funções, Probabilidade, Frações |
| Química | Tabela periódica, Reações químicas, Modelos atômicos |
| Biologia | Células animal e vegetal |

Galerias/salas que só oferecem uGUI não receberam interações 3D artificiais para preencher o teste. Os controles existentes mantêm seu feedback próprio.

## Resultados

- Um componente de feedback por alvo XR; controles uGUI cobertos por `ControlMotion` ou DOTween próprio; nenhum `XR motion outline` legado criado.
- Eventos sintéticos em alvo representativo por cena aplicável: foco, dois ponteiros, saída de um/ambos, hover de socket ignorado, seleção persistente, desabilitação/remoção do registro e motion reduzido.
- Referências dos materiais e transforms do alvo preservadas durante o feedback. Alvos de manopla restritos ao handle; líquidos, TMP e LineRenderer excluídos da máscara.
- Criação real de um ponto cartesiano pelo botão da sala coberta automaticamente pelo registro XR.
- Teste funcional de Células aprovado novamente: seleção XR através do Interaction Manager, afastamento, filtro dos sockets, devolução, consulta, cancelar/recompor e rótulos.
- Shader compilado sem erros; console sem erros ao final. Renderer próprio derivado do anterior; alteração no pipeline limitada à referência do renderer. MSAA 4×, HDR e render scale preservados. Nenhum renderer de terceiros foi alterado.
- Revisão visual encontrou e corrigiu foco disparado por sockets e bordas internas indevidas em rótulos de frascos. Máscaras separadas de cobertura/visibilidade mantêm a silhueta sem desenhar limites de oclusão.

Ferramenta reproduzível: `Assets/_Project/Editor/InteractionFeedbackVerification.cs`, `Start()` em Play Mode. Relatório local completo: `GeneratedMedia/InteractionFeedback/verification.json`. As capturas aproximadas usam câmera temporária somente de revisão; ela não é salva nas cenas.

## Capturas reais do Editor

![Estrutura orgânica selecionada](../GeneratedMedia/InteractionFeedback/Cells-silhouette.png)

![Recipiente com líquido e rótulo preservados](../GeneratedMedia/InteractionFeedback/Reactions-silhouette.png)

![Somente a parte móvel da manopla](../GeneratedMedia/InteractionFeedback/Thermology-silhouette.png)

![Ponto criado em execução](../GeneratedMedia/InteractionFeedback/CartesianSpace-silhouette.png)

Imagens e relatório são ignorados pelo Git. Para compartilhar este documento, levar também as mídias. A prévia esquemática do padrão permanece identificada separadamente.

## Limites

Não houve build, gravação ou teste em headset. Estéreo físico, conforto, espessura percebida e custo GPU/CPU precisam de medição no equipamento de destino. Shaders científicos que deformam a geometria ou recortam a superfície exigem alvo rígido adequado ou máscara específica; a máscara compartilhada não reproduz arbitrariamente todos os shaders. As equações e os fluxos completos dos 16 laboratórios não foram retestados por esta mudança visual. A preferência de movimento reduzido foi restaurada ao valor anterior após a regressão.
