# Tabela periódica 3D

Sala individual offline em `Scenes/PeriodicBonds.unity`. Ambiente e rig XR preservados. A versão atual substitui a montagem de ligações; suportes, sockets e receitas não participam desta sala.

## Implementado — 09/11/2026

- 118 cubos selecionáveis por raio/toque XR, com número, símbolo e nome.
- Tween DOTween de cor no hover (inclusive em cubos escurecidos), pulso do contorno ao selecionar e atenuação suave dos demais. Seleção persistente; alvos, posições e textos estáveis.
- Ficha com Visão geral, Propriedades, Na natureza e Fontes; limpar seleção e voltar ao Hub.
- Propriedades PubChem locais e textos de ocorrência e aplicações em português para todos os 118 elementos. Diferencia elemento, compostos e usos de isótopos.
- 90 fotografias reais locais, com legenda, autoria, licença e proveniência. Para 28 elementos sem foto validada no acervo, uma explicação substitui a imagem. Não há ilustrações apresentadas como fotografias.
- Carregamento de uma foto por vez via Resources; texturas limitadas a 512 px na importação. Ao mudar de elemento ou página, libera a foto anterior e limpa a referência visual.
- Paginação de texto aparece somente quando necessária; nenhuma redução automática de fonte. Toggle de movimento reduzido com preferência global local persistida.
- Nenhuma requisição de internet durante a execução. URLs na página Fontes são referências, não dependências de carregamento.

## Componentes e dados

`PeriodicTableExplorer` coordena seleção e ficha; `PeriodicElementCube` trata interação/realce; `ElementReferenceView` apresenta conteúdo, fotografia, paginação e preferência de motion. `Data/PubChemPeriodicTable.json` contém propriedades; `Data/ElementReference.json` contém textos, legendas e créditos. Consulte [dados e licenças](Data/README.md).

`ReferenceAndMotionMigration` foi aplicada pontualmente à cena existente. Não executar novamente o builder da sala nem a antiga migração estrutural para atualizar conteúdo. Código legado permanece por referências do roteiro antigo de gravação; não participa do runtime desta sala.

## Validação e limites

Editor: compilação e testes de seleção/consulta dos 118 registros; 90 fotos carregadas e legendas sem overflow; elementos sem foto não conservam imagem anterior. Cobertura de motion verificada nas 13 cenas de laboratório. Hover/seleção não alteram transforms físicos e o modo reduzido mantém contornos estáticos. Capturas em `Assets/Screenshots/` e relatórios em `Temp/`, ignorados pelo Git.

Não houve build ou teste físico de ergonomia/performance em headset. O resumo PubChem não traz todas as condições e incertezas experimentais; a ficha declara essa limitação. Busca, filtros e modelo atômico são opcionais do [GDD](../../../../../docs/gdd-quimica-tabela-e-ligacoes.md) e não estão implementados. Não reutilizar o vídeo antigo de ligações como demonstração desta versão.
