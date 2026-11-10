# Conjunto de GDDs — Química

**Status:** seis GDDs definidos; Tabela periódica teve o GDD refeito como exploração 3D dos 118 elementos. Migração estrutural aplicada: cubos, seleção e propriedades offline; montagem de ligações removida da cena. Ocorrência/aplicações dos 118 elementos e 90 fotos reais locais implementadas, com créditos e explicações para 28 ausências. Motion de interação implementado; headset pendente. **Data:** 09/11/2026. **Público:** Ensino Médio. **Modo inicial:** individual offline. Os três mockups anteriores do Hub permanecem.

| Sala | Ação central | Conceito e documento |
|---|---|---|
| 1. Estrutura da matéria | Encaixar partículas | [Elemento, isótopo e íon](gdd-quimica-estrutura-da-materia.md) |
| 2. Tabela periódica | Selecionar cubos 3D e consultar os 118 elementos | [Propriedades, organização e ocorrência na natureza](gdd-quimica-tabela-e-ligacoes.md) |
| 3. Reações químicas | Dosar reagentes e balancear | [Produtos, conservação e reagente limitante](gdd-quimica-reacoes.md) |
| 4. Soluções | Preparar e transferir líquidos | [Concentração, diluição e saturação](gdd-quimica-solucoes.md) |
| 5. Ácidos, bases e pH | Dosar e medir | [Neutralização gradual e indicadores](gdd-quimica-acidos-bases-ph.md) |
| 6. Separação de misturas | Montar processos | [Filtração, destilação e cromatografia](gdd-quimica-separacao-de-misturas.md) |

## Contratos compartilhados propostos

- **Modelo antes do efeito:** cada sala mantém cálculos e estado em C# independentes de Shader Graph, partículas, TMP e GameObjects quando possível. A apresentação recebe estado calculado; VFX não decide volume, concentração, pH, produto, energia ou probabilidade.
- **Transferência de material:** um recipiente possui capacidade, conteúdo e quantidade. Origem, destino, vazão e derramamento têm estados explícitos. Inclinação e proximidade iniciam a transferência; o fluxo visual acompanha o volume realmente transferido. Pausar não duplica nem perde conteúdo.
- **Kit visual de líquidos:** nível, superfície inclinável e oscilação de retorno por Shader Graph; jato e gotas com sistema de partículas; espuma, bolhas, precipitado e vapor como módulos separados. Compatibilidade exata do exemplo da [Unity Asset Store](https://assetstore.unity.com/packages/essentials/tutorial-projects/shader-graph-examples-vfx-sample-project-372864) com o projeto, licenças e custo no headset serão verificados antes de importar. Não presumir que o pacote fornece um shader de líquido pronto.
- **Linguagem molecular:** símbolos e cores consistentes entre salas; partículas ampliadas e contagens gráficas sempre identificadas como modelos didáticos. A vista macroscópica e a microscópica consultam o mesmo estado, mas não precisam ter escala ou tempo físicos idênticos.
- **Interação e UI:** XR Grab Interactables, sockets, controles 3D oficiais da Unity quando adequados, uGUI/TMP e retorno ao Hub compartilhados. Cada controle segue a tarefa: escolha nomeada para casos, botões −/+ para inteiros, toggles para visualizações e medidores para resultados. Sem painel genérico repetido nas seis salas. Seguir [UI Guidelines](../UI_GUIDELINES.md) e [Design System](../DESIGN_SYSTEM.md); Química usa roxo como acento de disciplina, sem pintar todo o ambiente de roxo.
- **Estado e autoridade:** primeira entrega individual. Ao considerar LAN, o anfitrião comandará manipulações que mudam o modelo; visualizações locais poderão ser preferências por participante. Não afirmar sincronização coletiva antes de implementá-la.
- **Performance:** priorizar poucos efeitos simultâneos, pooling, materiais reutilizados, textos atualizados por eventos e leitura em headset. Efeitos não devem esconder resultados, induzir enjoo ou alterar a câmera.

## Limites comuns e validação

Catálogos de elementos, substâncias e misturas são finitos e declarados. Uma combinação não cadastrada não autoriza inventar resultado químico; a mensagem deve indicar o limite do laboratório sem afirmar impossibilidade universal. Unidades, hipóteses e valores educacionais de cada caso precisam ser revisados antes de implementar.

Critérios transversais de aceite: a quantidade exibida coincide com a quantidade calculada; transições de pausa/reinício são determinísticas; interações funcionam por pega ou raio XR conforme o objeto; conteúdo científico permanece legível sem depender apenas de cor; o resultado visual corresponde ao caso selecionado; o frame rate e a leitura são avaliados no headset. Documentação é planejamento, não evidência de execução. Nenhum build sem autorização explícita.

## Próximas etapas de prototipagem

1. **Tabela periódica**: migrar a cena existente para 118 cubos selecionáveis, destaque, fichas completas e aparência/ocorrência com fontes. Remover montagem de ligações; importar dados e imagens revisados para uso offline e validar a interação XR.
2. **Estrutura da matéria** amplia o conjunto visual de partículas e diferencia elemento, isótopo e íon.
3. **Reações químicas** valida o kit de frascos, transferência, líquido e efeitos específicos.
4. **Soluções** e **pH** reutilizam a transferência, com modelos matemáticos próprios.
5. **Separação de misturas** completa o catálogo com processos encadeados.

Essa é uma ordem de implementação sugerida, não uma sequência obrigatória para o aluno.



