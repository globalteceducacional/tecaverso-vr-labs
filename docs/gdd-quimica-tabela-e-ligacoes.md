# Short GDD — Tabela periódica 3D

**Revisão:** 09/11/2026. **Status:** exploração 3D implementada no Editor com 118 cubos, propriedades locais, textos de ocorrência/aplicações dos 118 elementos e 90 fotografias com créditos/licenças. Outros 28 registros explicam a ausência de foto validada; sem imagens inventadas. Motion de hover, seleção, escurecimento e ficha implementado, com preferência reduzida. Montagem antiga removida. Ergonomia e desempenho em headset pendentes. Substitui o GDD de tabela e ligações.

**Disciplina:** Química. **Modo:** exploração individual offline. **Sessão sugerida:** 10–20 minutos.

## 1. Objetivo

Explorar os 118 elementos, reconhecer a organização por número atômico, períodos e famílias e relacionar propriedades com aparência, ocorrência e aplicações. Todos os elementos têm a mesma possibilidade de seleção e consulta. Esta sala não monta moléculas nem simula reações.

## 2. Sala e composição

- Tabela como instalação de 118 cubos físicos 3D, organizados na disposição periódica convencional. Cada frente mostra número atômico, símbolo e nome em TMP; cores identificam categorias, acompanhadas de legenda textual.
- Manter os períodos 1–7 na estrutura principal e séries de lantanídeos/actinídeos em uma extensão próxima, com conexão e identificação explícitas. A estrutura deve permitir reconhecer a tabela completa sem espalhar elementos arbitrariamente pela sala.
- Cubos têm profundidade perceptível, espaçamento e alvos independentes. Não são botões quadrados em um Canvas. Não representam dimensões de átomos nem amostras reais do material.
- Área de consulta próxima à tabela com ficha legível e uma vitrine de aparência/ocorrência. Não exigir caminhar até cada cubo para consultar elementos altos ou distantes.
- Circulação desimpedida, iluminação suficiente e identidade do projeto, com roxo de Química como acento. Dimensionar a instalação após medir a sala e validar a leitura; preservar rig, ambiente e transformações autorais não afetadas pelo redesign.

## 3. Interações XR

### Apontar e selecionar

1. Apontar o raio XR para um cubo mostra um contorno de hover, sem abrir ou trocar a ficha automaticamente.
2. Pressionar o gatilho seleciona o elemento. Contorno persistente e identificação textual confirmam a seleção; o cubo mantém sua posição na tabela.
3. Os demais cubos ficam escurecidos, mas continuam legíveis e selecionáveis. Escurecer não significa bloquear: o aluno pode trocar diretamente de elemento sem fechar a consulta.
4. A ficha e a vitrine passam a mostrar o mesmo elemento. A câmera e o aluno nunca são deslocados automaticamente.
5. Selecionar o mesmo cubo mantém a ficha. **Limpar seleção** restaura a tabela e o estado introdutório. **Voltar ao Hub** fica sempre acessível.

Usar XRSimpleInteractable ou componente equivalente existente para seleção por raio/toque. Os cubos não são peças agarráveis: retirá-los desmontaria a referência espacial da tabela. Hover, seleção e leitura não dependem apenas de cor, áudio ou rastreamento ocular.

### Encontrar um elemento

- A interação principal é apontar para a tabela; não criar uma lista duplicada de 118 botões.
- Busca opcional por nome em português, símbolo ou número atômico, usando o teclado XR compartilhado. Mostrar poucos resultados legíveis; escolher um resultado executa exatamente a mesma seleção do cubo.
- Busca sem resultado explica o estado e permite apagar o texto. Fechar a busca não apaga a seleção atual.
- Filtro por família/categoria apenas realça membros na tabela. Não reorganiza as posições nem torna outros elementos inacessíveis. Seleção de elemento tem destaque mais forte que filtro.
- Busca e filtros são auxiliares; não bloqueiam a entrega da consulta direta completa dos 118 elementos.

## 4. Ficha do elemento e controles

A informação é extensa, portanto a ficha usa páginas de conteúdo com escolhas nomeadas, em vez de reduzir toda a tipografia para caber em uma tela. Cabeçalho persistente com nome, símbolo e número atômico.

| Página | Conteúdo |
|---|---|
| Visão geral | Categoria, grupo/período ou série, massa atômica, estado padrão, descoberta e configuração eletrônica |
| Propriedades | Configuração eletrônica, distribuição por camadas, eletronegatividade com escala, estados de oxidação, densidade e condições, temperaturas de fusão/ebulição e unidades, quando disponíveis |
| Na natureza | Ocorrência, aplicações (elemento/compostos/isótopos), fotografia e identificação da amostra, distinção entre material preparado e ocorrência natural |
| Fontes | Proveniência dos dados, data da revisão, créditos e licença de cada imagem; distinguir valores medidos, estimados e indisponíveis |

Ao trocar de elemento, manter a página escolhida e atualizar todo o conteúdo de forma atômica. Nunca manter foto do elemento anterior sob o nome do novo. Se o conteúdo exceder a região, usar paginação ou rolagem XR explícita e testada. Usar o padrão visual compartilhado sem replicar painel de experimento, sliders, estudo A/B ou botões de montagem.

Controles essenciais: escolha das páginas, limpar seleção e voltar ao Hub. Há preferência global local de movimento reduzido; anterior/próximo texto aparecem apenas quando o conteúdo exige paginação. Só adicionar controles de visualização quando existir uma visualização correspondente. Nenhum slider para selecionar elementos.

## 5. Aparência e visualizações

### Amostra macroscópica

- Priorizar fotografias reais com identificação e licença verificadas, em uma moldura na vitrine. Um modelo 3D é complementar quando houver asset apropriado e validado; não prometer 118 modelos reais a partir de uma API de texto.
- Distinguir explicitamente **elemento puro**, **mineral que contém o elemento** e **representação ilustrativa**. Por exemplo, uma amostra de mineral de urânio não deve ser rotulada como uma pedra de urânio metálico puro. A espécie mineral e sua composição devem ser verificadas na fonte antes de inclusão.
- Não representar todo elemento como pedra: gases, líquidos e materiais sintetizados exigem exemplos adequados. Não usar brilho verde como propriedade visual padrão do urânio.
- Quando não houver foto licenciada ou amostra macroscópica conhecida, informar isso e mostrar uma explicação científica. Não inventar aparência nem deixar uma foto genérica parecer evidência.
- Se houver modelo 3D, permitir girá-lo por pega XR, com escala ilustrativa identificada e ação de restaurar orientação. A manipulação da amostra não move o cubo da tabela. Fotos não precisam de controles de rotação.

### Estrutura atômica

Uma visualização opcional por camadas pode acompanhar a configuração eletrônica. Deve ser identificada como modelo didático, sem apresentar órbitas clássicas como trajetórias reais de elétrons. Contagem, dados e legenda precisam corresponder ao elemento selecionado; não inferir isótopo, número de nêutrons ou valência universal a partir da massa atômica média. Não há criação de ligações nessa visualização.

## 6. Dados, APIs e funcionamento offline

**Fontes importadas na preparação editorial; nenhuma consulta em runtime:**

- [PubChem PUG REST](https://pubchem.ncbi.nlm.nih.gov/docs/pug-rest), com resumo da tabela em `https://pubchem.ncbi.nlm.nih.gov/rest/pug/periodictable/JSON`. A [receita da IUPAC](https://iupac.github.io/WFChemCookbook/datasources/pubchem_ptable.html) documenta acesso programático às propriedades. Base principal local, complementada por PUG View e referências científicas identificadas.
- [Periodic-Table-JSON](https://github.com/Bowserinator/Periodic-Table-JSON): índice complementar de candidatos a fotografia, com atribuição e licença CC BY-SA 3.0 preservadas. Links de imagens não concedem automaticamente direito de reutilização. Filtrar a seleção para os 118 elementos reconhecidos.
- [MediaWiki Imageinfo](https://www.mediawiki.org/wiki/API:Imageinfo): consulta de arquivos e metadados para fotografias do Wikimedia Commons. Revisar autoria, licença, identificação da amostra e atribuição de cada arquivo.

A API alimenta uma etapa de importação/curadoria, não uma requisição obrigatória a cada clique no headset. Distribuir catálogo e mídia aprovados localmente, com versões e atribuições, para uso offline. Falhas de rede durante atualização preservam o catálogo válido anterior. Assets usados pelo aplicativo e seus `.meta` são versionados; capturas de desenvolvimento continuam nas pastas ignoradas.

Separar catálogo científico, seleção/filtros e apresentação XR. Usar número atômico como identificador estável. Textos em pt-BR devem ser revisados; não fazer tradução automática improvisada durante a consulta. Campos ausentes são apresentados como não disponíveis, nunca como zero. Distinguir massa atômica padrão de número de massa de um isótopo e registrar condições/unidades das propriedades. Elementos sintéticos podem ter informações experimentais limitadas; completude significa cobertura dos 118 e transparência sobre lacunas, não números inventados.

## 7. Percurso do aluno

1. Reconhece famílias e períodos observando a instalação completa.
2. Aponta e seleciona um elemento próximo ou distante; vê a tabela escurecer e a ficha correspondente abrir.
3. Consulta propriedades e aparência, distinguindo elemento puro de material que o contém.
4. Seleciona outro elemento diretamente e compara mentalmente seus dados mantendo a mesma página, sem etapa de desmontar ou reiniciar.
5. Explora um elemento sem foto ou propriedade conhecida e entende o limite da informação, sem receber um resultado químico falso.
6. Limpa a seleção para recuperar a visão geral ou retorna ao Hub.

## 8. Migração da implementação existente

- Nome apresentado no Hub e na sala passa a **Tabela periódica**.
- Substituir as células uGUI por cubos 3D com TMP e interação XR, preservando posições científicas e cobertura dos 118 elementos.
- Retirar da experiência os suportes de H/C/O/Na/Cl, quatro sockets, montagem, vitrine de moléculas/rede iônica, botão Desmontar e toggle de elétrons de ligação.
- Desvincular receitas e validação de combinações do fluxo desta sala. Remover código/assets antigos somente após verificar referências, incluindo o roteiro de gravação existente.
- Criar seleção única com destaque/atenuação, ficha paginada e vitrine de ocorrência. Reutilizar componentes XR, fontes, materiais e navegação apropriados.
- Atualizar catálogo do Hub, README e demonstração após a migração. Não marcar a nova sala como implementada somente porque este GDD foi refeito.
- O caminho histórico deste documento e da cena pode permanecer para preservar links/GUIDs; o nome técnico PeriodicBonds não define o novo escopo.

## 9. Critérios de aceite

- Exatamente 118 elementos distintos, todos selecionáveis e com identidade/posição corretas; nenhuma distinção entre cinco ativos e demais apenas decorativos.
- Hover não muda seleção; selecionar outro elemento, repetir seleção e limpar funcionam sem deixar realces ou conteúdo antigos.
- Cubos escurecidos continuam acessíveis; texto permanece legível; nenhuma ampliação de destaque invade alvos vizinhos.
- Todas as páginas correspondem ao elemento selecionado, inclusive durante trocas rápidas e quando faltam dados ou imagens.
- Dados têm fonte/unidade/condição quando pertinentes; créditos e licenças acompanham mídia. Funcionamento básico sem internet.
- Sem sockets de combinação, moléculas montáveis ou mensagens de combinação inválida nesta sala.
- Caminhos de consulta, limpeza e retorno funcionam no Editor; alcance, legibilidade, alvos e conforto são avaliados separadamente em headset, sentado e em pé.
- Nenhuma alteração incidental no rig/ambiente; nenhuma geração de build sem pedido explícito.

## 10. Correspondência obrigatória entre GDD e sala

A execução da sala deve refletir este GDD. A migração só pode ser declarada concluída depois de verificar, em Play Mode, os 118 cubos, seleção/destaque/limpeza, páginas de dados, apresentação de ocorrência, funcionamento offline e retorno ao Hub, além da ausência do fluxo antigo de ligações. Capturas ou gravações devem demonstrar essas interações reais, não substituir seu funcionamento por uma sequência visual roteirizada.

Registrar separadamente funcionalidades implementadas, verificações no Editor e pendências de headset. Recursos descritos como opcionais continuam opcionais; funcionalidades essenciais não podem ser omitidas ou reduzidas silenciosamente para caber na implementação. Se uma fonte não fornecer um campo ou imagem, apresentar a lacuna conforme este GDD; isso não autoriza restringir a seleção a poucos elementos. Qualquer mudança de escopo necessária deve ser explicitada e conciliada com o documento antes do aceite.


## 11. Conteúdo e motion entregues em 09/11/2026

- `Data/ElementReference.json`: ocorrência e aplicações dos 118 elementos em português; `Data/Resources/PeriodicSamples/`: 90 fotos reais. Créditos e proveniência acompanham cada entrada, com índice local `PHOTO_CREDITS.md`.
- Sem fotografia validada no acervo para Z=61, 84–87, 89–91, 96 e 100–118. A ficha informa a lacuna e distingue falta de licença/validação de ausência de amostra macroscópica. Esses elementos continuam totalmente selecionáveis.
- Cubos: tween de cor em 0,16 s; hover ilumina também o cubo atenuado; pulso do contorno em 0,22 s ao focar/selecionar; saída volta ao estado selecionado ou atenuado correto. Não escalar o cubo/collider nem deslocar texto/grade.
- Ficha: entrada de conteúdo em 0,16 s, sem deslocar a leitura; botões e toggle seguem motion compartilhado. Trocas rápidas cancelam o feedback anterior. Movimento reduzido elimina interpolação e pulso, preservando estado visual.
- Fotos carregadas localmente por demanda, uma por vez. Não há fallback remoto. Ao limpar ou mudar para uma entrada sem imagem, a foto anterior é removida.
- Editor: 118 consultas, 90 carregamentos de foto, legendas e estados sem imagem conferidos. Validação no headset permanece separada; busca/filtros/modelo atômico continuam opcionais e não implementados. Condições e incertezas não fornecidas pelo resumo PubChem são declaradas como limite, não inferidas.
