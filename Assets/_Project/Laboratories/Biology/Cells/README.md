# Células animal e vegetal

Primeira sala individual offline de Biologia, implementada em 17/11/2026 conforme o [GDD aprovado](../../../../../docs/gdd-biologia-celulas.md). Cena: `Scenes/Cells.unity`. Entrada: **Explorar sozinho → Biologia → Células animal e vegetal**. Corpo humano e sistemas também está disponível; os outros quatro conteúdos permanecem em desenvolvimento.

## Uso

As duas células ficam visíveis em suportes elevados, com escala de observação de aproximadamente 1 m na maior dimensão. O corte inicia voltado ao aluno, levemente inclinado. As cores são didáticas; não há escala real comum entre células e ambiente.

Selecione uma estrutura por raio ou proximidade para abrir uma única ficha lateral de função/contexto. Segure uma das seis organelas destacáveis e afaste até **0,4 m** de sua origem. Um indicador mostra o encaixe. Soltar a até **0,12 m** da origem devolve a peça ao socket correspondente. Outro socket nunca aceita a peça. Peças abandonadas retornam após **15 s**, ou quando saem da área; o motivo aparece na linha de estado.

Membrana e parede são fixas para consulta. Fechar a ficha não reencaixa a organela. Use **Exibir rótulos** para identificação adicional: os nomes ficam nas laterais, com linhas apontando para as estruturas, e permanecem legíveis durante o giro. Os dials Unity giram os modelos ancorados; ficam indisponíveis enquanto há consulta, peça na mão ou peça separada. Fechar a consulta e reencaixar libera o giro.

**Recompor células** abre confirmação e restaura peças/orientação; cancelar preserva a disposição. A preferência de rótulos é mantida. Se houver peça na mão, solte antes de recompor/sair. **Voltar ao Hub** pede confirmação quando há peça separada. Não há HUD na cabeça, montagem A/B ou formulário de parâmetros.

## Estruturas implementadas

| ID | Grupo original | Consulta | Pega |
|---|---|---|---|
| animal.nucleo | Animal_Cell/Nucleo | Núcleo | Destacável |
| animal.mitocondria | Animal_Cell/Mitocondria_1 | Mitocôndria | Destacável |
| animal.membrana | Animal_Cell/MembranaCelular | Membrana plasmática | Fixa |
| vegetal.nucleo | Plant_Cell/Nucleo | Núcleo | Destacável |
| vegetal.mitocondria | Plant_Cell/Mitocondria_1 | Mitocôndria | Destacável |
| vegetal.cloroplasto | Plant_Cell/Cloroplasto_1 | Cloroplasto | Destacável |
| vegetal.vacuolo | Plant_Cell/Vacuolo | Vacúolo central | Destacável |
| vegetal.parede | Plant_Cell/ParedeCelular | Parede celular | Fixa |

São **seis categorias e oito alvos**, incluindo núcleo/mitocôndria nas duas células. As demais peças dos modelos permanecem visuais. Não foram transformados nomes genéricos em organelas inventadas. A membrana existe nos dois tipos celulares; o alvo desta entrega usa o grupo semântico do modelo animal. A consulta da parede explica sua relação com a membrana.

## Arte e dependências

Somente os dois FBX foram importados para `Assets/_Project/Art/Biology/Cells/Models/`, com seus GUIDs/metadados preservados. `GeometryMap.json` registra as transformações e referências de malhas/materiais dos prefabs antigos. `CellsAssets` reconstrói apenas geometria, sem menu, callouts, colliders ou scripts antigos; novos prefabs visuais mantêm a hierarquia semântica. Materiais URP derivados preservam as cores dos materiais embutidos. Fontes originais completas permanecem em `SourceAssets/BiologyLegacy`, fora da importação runtime.

A aplicação não lê `SourceAssets`, `PROJETO ANTIGO`, scripts Python, servidor MCP ou Internet durante a execução. A origem/licença dos modelos permanece conforme a auditoria do acervo; registrar antes de distribuição.

Ambiente, rig, EventSystem e renderização foram reutilizados da sala padrão já usada em Geometria plana. A cena nova é aberta isoladamente, com um rig. O instalador em `Editor/CellsAuthoring.cs` recusa uma cena existente: não deve ser usado para atualizar/regenerar a sala. `RefinePresentation` é um ajuste específico de apresentação; preserve alterações autorais posteriores.

## Componentes

- `CellsLab`: seleção, consulta, rótulos, retorno, confirmação e Hub.
- `CellStructure`: XRGrab com partes fixas/destacáveis e recuperação.
- `CellGrabLimit`: limita o alvo XR antes de aplicar movimento físico.
- `CellReturnSocket`: filtro por identidade e captura apenas ao soltar.
- `CellDisplay`: dial Unity e giro suavizado, sem translação do modelo.
- `Shared/Interaction`: silhueta da peça em foco/ação/seleção persistente, sem caixas de bounds ou contorno retangular adicional. A indicação de origem permanece contextual. Sockets não acendem foco do usuário. Preferência reduzida respeitada; nenhuma animação de câmera. [Padrão global e prévia](../../../../../docs/gdd-interaction-feedback.md).

## Validação e limites

Verificação focada no Editor/Play Mode: inventário, um rig, motion compartilhado, filtro de sockets, consulta, fechamento sem alterar peça, cancelar/recompor, alvo físico limitado, eventos reais de seleção XR, rótulos e preservação de preferência. Raycasts a partir da câmera inicial atingiram os oito alvos correspondentes. Consulta de 247 caracteres sem overflow TMP. Console sem erros na verificação.

Evidências locais em `GeneratedMedia/Cells/` (ignoradas). `CellsValidation.Run()` é ferramenta de teste do Editor; não integra o runtime. Os testes usam interator temporário e não substituem controladores físicos. Conforto, leitura, alcance e desempenho em headset continuam pendentes. Não foi gerado build; LAN não implementada nesta sala.
