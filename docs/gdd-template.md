# Modelo de short GDD — laboratório

**Status:** proposta / aprovado / implementado / revisão em discussão. **Disciplina:** preencher. **Modo:** individual ou LAN, explicitando o que existe. **Data:** preencher.

## Objetivo e experiência do aluno

Qual pergunta o laboratório permite investigar? Descrever em poucas linhas o que o aluno faz com as mãos e o que consegue concluir. Diferenciar observar, construir e editar.

## Sala, objetos e catálogo

Descrever a disposição e a circulação. Listar objetos manipuláveis, instrumentos, suportes e elementos apenas visuais. Informar quantos e quais materiais/figuras/casos existem; seleção livre ou conjuntos predefinidos; quantidades/unidades e limites do modelo.

| Objeto/material | Quantidade ou opções | Papel | Interação XR | Resultado/limite |
|---|---|---|---|---|
| Preencher | Preencher | Manipulável, controle ou visualização | Pegar, inclinar, encaixar, girar, apontar etc. | Preencher |

Quando houver combinações, listar as suportadas, a ação que as produz e o comportamento para ausência de resultado ou combinação não cadastrada. Não prometer simulação universal por disponibilizar um catálogo amplo.

## Inventário de interfaces e HUD — obrigatório

Incluir também pequenos rótulos e controles 3D; contar painéis separados pela experiência de uso, não pela quantidade técnica de Canvases. Declarar explicitamente quando não houver HUD ou painel. Nenhuma linha da tabela exige criar um componente que o experimento não precise.

| ID / interface | Tipo e localização | Finalidade e justificativa | Conteúdo e ações exatas | Visibilidade inicial | Abre / fecha quando | O que aparece simultaneamente |
|---|---|---|---|---|---|---|
| Preencher | Painel espacial, HUD, rótulo no objeto, modal ou controle 3D | Por que precisa existir? O objeto poderia resolver? | Botões, toggles, valores ou textos | Visível / oculto / contextual | Gesto ou estado, incluindo saída | IDs correspondentes |

**Visão de entrada:** descrever o que o aluno vê ao chegar, incluindo quantidade de painéis e informação junto aos objetos.

**Durante a manipulação:** explicar quais informações permanecem, quais se ocultam e como a interface deixa o experimento visível.

**Durante a consulta:** explicar como pedir detalhes, onde aparecem e como voltar à manipulação. Justificar qualquer análise mostrada por padrão.

**Durante uma confirmação:** explicar o que bloqueia, como cancelar e o que é preservado.

**Gravações:** legendas, ponteiros e câmeras exclusivos da demonstração não são UI do laboratório; documentá-los separadamente quando houver roteiro.

## Fluxo e interações XR

Antes de preencher o fluxo, criar a seção visual abaixo e conferir sua coerência com o inventário.

### Prévias visuais de UI/HUD — obrigatórias antes de implementar

Incorporar imagens reais de mockups (HTML capturado ou imagem conceitual) com legenda de estado; um link para HTML sozinho não basta. Incluir visão espacial da sala, interfaces legíveis e, conforme o laboratório, escolha, manipulação, consulta, impedimento e confirmação. Se não houver HUD, mostrar rótulos/controles junto aos objetos. Não criar interfaces apenas para preencher exemplos.

Para cada imagem: estado, IDs de interfaces presentes, gesto/ação de entrada, saída e conteúdo que permanece visível. Marcar **proposta não implementada**; identificar anotações e navegação externas ao aplicativo. As imagens e o inventário precisam descrever o mesmo fluxo.

Registrar links para fonte do protótipo/prompt e instruções de reprodução; guardar capturas em `GeneratedMedia/GDD/<laboratorio>/`, conforme as diretrizes. Incorporar no Markdown com caminho relativo e conferir existência local. Ao compartilhar o documento, incluir as imagens, pois saídas ignoradas não viajam com Git.

**Revisão visual:** verificar legibilidade, controles cortados, área do experimento livre e quantidade simultânea de interfaces. Atualizar prévias quando a proposta mudar; registrar aprovação antes da implementação e validação em headset separadamente.

### Sequência do aluno

Descrever uma sequência curta: entrar, descobrir a ação, manipular, observar, comparar e reiniciar/sair. Referenciar os IDs da interface para não introduzir controles que não aparecem no inventário. Especificar pega por proximidade/raio, encaixes, limites físicos e feedback de foco/ação/estado conforme Shared/Interaction. Respeitar movimento reduzido.

## Modelo, resultados e limites

Definir as grandezas calculadas, as representações didáticas, unidades, resultados esperados e casos sem resultado. Separar efeito visual de cálculo. Declarar dependências offline, autoridade LAN quando aplicável e conteúdo não implementado.

## Critérios de aceite

- O aluno consegue identificar e executar a ação central pelos objetos e instruções previstas.
- O catálogo, as combinações e as interações implementadas correspondem ao escopo descrito.
- A visão inicial e cada estado correspondem ao inventário de interfaces; não há painel ou controle adicional sem finalidade documentada.
- O GDD inclui imagens das interfaces antes de implementar; mockups, inventário e fluxo concordam. Não confundir protótipo com evidência da cena executada.
- Objetos e resultados relevantes ficam visíveis durante a manipulação. Detalhes opcionais têm acesso e fechamento claros.
- Foco, seleção, confirmação/cancelamento, reinício e retorno funcionam sem estado invisível bloqueando interação.
- Separar validação do modelo, Editor, headset e performance. Não afirmar testes não realizados; nenhum build sem autorização.
