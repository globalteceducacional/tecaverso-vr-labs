# Conjunto de GDDs — Química

**Status:** seis GDDs definidos; Tabela periódica e Reações químicas implementadas no Editor. Tabela: 118 cubos, propriedades, ocorrência/aplicações e 90 fotos reais locais, com créditos e explicações para 28 ausências. Reações: 12 reagentes em degraus, sonda XR (estimativa térmica em HCl/NaOH e HCl/KOH), 12 receitas, suportes A/B, frascos/colheres XR e consulta opcional; líquido Unity, transferência e perdas preservados. Motion compartilhado; headset pendente. **Data:** 10/11/2026. **Público:** Ensino Médio. **Modo inicial:** individual offline. Estrutura da matéria e Soluções continuam mockups no Hub; pH e Separação ainda são GDDs.

| Sala | Ação central | Conceito e documento |
|---|---|---|
| 1. Modelos atômicos e estrutura atômica | Percorrer alas históricas e de partículas; montar e girar um átomo com botões 3D +/− | [Modelos, partículas, elemento, isótopo e íon; GDD e mockups em revisão](gdd-quimica-estrutura-da-materia.md) |
| 2. Tabela periódica | Selecionar cubos 3D e consultar os 118 elementos | [Propriedades, organização e ocorrência na natureza](gdd-quimica-tabela-e-ligacoes.md) |
| 3. Reações químicas | Implementado: 12 reagentes em degraus e sonda térmica XR, escolha física A/B, dosagem e consulta opcional | [Produtos, conservação e reagente limitante; GDD e mockups aprovados](gdd-quimica-reacoes.md) |
| 4. Soluções | Preparar e transferir líquidos | [Concentração, diluição e saturação](gdd-quimica-solucoes.md) |
| 5. Ácidos, bases e pH | Dosar e medir | [Neutralização gradual e indicadores](gdd-quimica-acidos-bases-ph.md) |
| 6. Separação de misturas | Montar processos | [Filtração, destilação e cromatografia](gdd-quimica-separacao-de-misturas.md) |

## Contratos compartilhados propostos

- **Modelo antes do efeito:** cada sala mantém cálculos e estado em C# independentes de Shader Graph, partículas, TMP e GameObjects quando possível. A apresentação recebe estado calculado; VFX não decide volume, concentração, pH, produto, energia ou probabilidade.
- **Transferência de material:** um recipiente possui capacidade, conteúdo e quantidade. Origem, destino, vazão e derramamento têm estados explícitos. Inclinação e proximidade iniciam a transferência; o fluxo visual acompanha o volume realmente transferido. Pausar não duplica nem perde conteúdo.
- **Kit visual de líquidos:** nível, superfície horizontal e oscilação por Shader Graph; jato e gotas, com efeitos específicos conforme a sala. Reações já usa o shader e duas malhas da [amostra Unity](https://assetstore.unity.com/packages/essentials/tutorial-projects/shader-graph-examples-vfx-sample-project-372864), isolados em ThirdParty/UnityLiquidSample, com adaptação em Shared/Liquids. Cenas e configurações do exemplo foram retiradas. URP verificado no Editor; custo e estéreo em headset pendentes. Espuma e vapor de outros GDDs não estão implementados por consequência dessa integração.
- **Linguagem molecular:** símbolos e cores consistentes entre salas; partículas ampliadas e contagens gráficas sempre identificadas como modelos didáticos. A vista macroscópica e a microscópica consultam o mesmo estado, mas não precisam ter escala ou tempo físicos idênticos.
- **Interação e UI:** XR Grab Interactables, sockets, controles 3D oficiais da Unity quando adequados, uGUI/TMP e retorno ao Hub compartilhados. Cada controle segue a tarefa: escolha nomeada para casos, botões −/+ para inteiros, toggles para visualizações e medidores para resultados. Sem painel genérico repetido nas seis salas. Seguir [UI Guidelines](../UI_GUIDELINES.md) e [Design System](../DESIGN_SYSTEM.md); Química usa roxo como acento de disciplina, sem pintar todo o ambiente de roxo.
- **Estado e autoridade:** primeira entrega individual. Ao considerar LAN, o anfitrião comandará manipulações que mudam o modelo; visualizações locais poderão ser preferências por participante. Não afirmar sincronização coletiva antes de implementá-la.
- **Performance:** priorizar poucos efeitos simultâneos, pooling, materiais reutilizados, textos atualizados por eventos e leitura em headset. Efeitos não devem esconder resultados, induzir enjoo ou alterar a câmera.

## Limites comuns e validação

Catálogos de elementos, substâncias e misturas são finitos e declarados. Uma combinação não cadastrada não autoriza inventar resultado químico; a mensagem deve indicar o limite do laboratório sem afirmar impossibilidade universal. Unidades, hipóteses e valores educacionais de cada caso precisam ser revisados antes de implementar.

Critérios transversais de aceite: a quantidade exibida coincide com a quantidade calculada; transições de pausa/reinício são determinísticas; interações funcionam por pega ou raio XR conforme o objeto; conteúdo científico permanece legível sem depender apenas de cor; o resultado visual corresponde ao caso selecionado; o frame rate e a leitura são avaliados no headset. Documentação é planejamento, não evidência de execução. Nenhum build sem autorização explícita.

## Próximas etapas de prototipagem

1. **Tabela periódica**: migrar a cena existente para 118 cubos selecionáveis, destaque, fichas completas e aparência/ocorrência com fontes. Remover montagem de ligações; importar dados e imagens revisados para uso offline e validar a interação XR.
2. **Modelos atômicos e estrutura atômica** propõe seis estações históricas, três vitrines de partículas e uma oficina com controles 3D +/− e modelo girável; GDD e mockups estão em revisão, sem cena implementada.
3. **Reações químicas** valida o kit de frascos, transferência, líquido e efeitos específicos.
4. **Soluções** e **pH** reutilizam a transferência, com modelos matemáticos próprios.
5. **Separação de misturas** completa o catálogo com processos encadeados.

Essa é uma ordem de implementação sugerida, não uma sequência obrigatória para o aluno.



