# Short GDD — Separação de misturas

**Status:** proposta para aprovação; não implementado. **Disciplina:** Química. **Modo:** exploração individual offline. **Sessão sugerida:** 12–18 minutos.

## Objetivo e pergunta

Qual propriedade permite separar os componentes de uma mistura? O aluno monta uma sequência de equipamentos, prevê o destino dos componentes e compara o resultado com sua previsão. Separação física não é apresentada como reação química.

## Sala e objetos

- Um reservatório inicial e uma estação de montagem ocupam a sala, com circulação entre módulos de **filtração**, **decantação**, **destilação simples** e **cromatografia em papel**. Recipientes finais mostram o material recuperado.
- Funil, filtro, tubos, coletor e condensador possuem formas reconhecíveis, XR Grab Interactables e sockets com silhueta/etiqueta de encaixe. Tubos ou conexões podem usar cabo visual flexível quando a movimentação justificar; não simular corda física para conduzir líquido.
- Um mapa do processo aparece junto ao equipamento montado, com nomes e quantidades dos componentes. Efeitos de líquido, gotejamento, vapor ou pigmento são aplicados conforme a técnica, sem indicar pureza impossível.

## Desafios e fluxo

| Mistura cadastrada | Sequência investigada | Aprendizado principal |
|---|---|---|
| Água + areia | Filtrar e coletar filtrado/resíduo | O filtro retém partículas sólidas não dissolvidas |
| Água + sal + areia | Filtrar; depois destilar parte do filtrado | Filtração não retém sal dissolvido; água condensada e sal têm destinos distintos |
| Tinta com pigmentos cadastrados | Aplicar gota no papel e escolher solvente | Pigmentos migram distâncias diferentes no modelo de cromatografia |

1. O aluno escolhe uma mistura diretamente no reservatório, lê seus componentes e prevê quais saídas conterão cada um.
2. Pega e encaixa os módulos. Sockets aceitam conexões fisicamente compatíveis; um encaixe incompleto é apontado localmente.
3. Abre uma alavanca física para iniciar a transferência, ou aciona o aquecimento virtual no módulo de destilação. **Pausar / Continuar** conserva o estado dos recipientes.
4. Compara a previsão com as quantidades recuperadas; pode inspecionar um corte de partículas no filtro ou no papel. **Reiniciar mistura** restaura o caso e a montagem após confirmação.

A UI principal limita-se à escolha nomeada da mistura, leituras locais, **Exibir partículas**, pausa/reinício e retorno ao Hub. Não usar slider para escolher técnicas. Aquecimento só aparece quando a destilação está montada, com controle físico coerente e estado ligado/desligado visível.

## Modelo, limites e feedback

- Cada mistura tem composição e propriedades necessárias declaradas: estado, tamanho de partícula didático, solubilidade relevante e/ou comportamento de migração. Cada módulo transforma uma corrente de entrada em saídas com balanço de quantidade.
- Para água + sal + areia, o filtrado contém água e sal dissolvido. Destilação simples recupera água condensada; o resíduo concentrado permanece identificado. Não dizer que filtração remove sal dissolvido ou que a destilação é instantânea.
- Vapor, gotas e avanço de pigmento comunicam processo. Eles não determinam massa, temperatura ou separação. Cromatografia usa resultados cadastrados, sem prever pigmentos arbitrários.
- Limitar módulos simultâneos e efeitos recorrentes; usar pooling. Não incluir processos industriais completos nem misturas livres no primeiro escopo. A [RSC reúne filtração, destilação e cromatografia](https://edu.rsc.org/resources/11-14-years/elements-compounds-and-mixtures/separating-mixtures) como técnicas distintas de separação.

## Percurso e aceite

Tentar filtrar água salgada, observar que o sal passa, então montar a sequência para água + sal + areia; finalizar com cromatografia. Aceite: cada saída preserva a composição correta; massa/quantidade contabilizada sem criação ou perda silenciosa; conexões inválidas não iniciam o fluxo; pausa e reinício preservam/restauram estados; texto identifica resíduos e produtos recuperados; operação é possível em XR com alvos acessíveis. Nenhum build sem pedido.
