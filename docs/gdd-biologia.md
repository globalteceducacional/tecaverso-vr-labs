# Biologia — seis laboratórios propostos

**Status:** GDDs aprovados. Células e Corpo humano implementados/verificados no Editor; quatro salas pendentes de implementação. **Data:** 17/11/2026. **Modo inicial:** individual offline. Química fica em pausa por orientação do responsável; seus arquivos existentes foram preservados.

## Distribuição e consolidação

| Nova sala | Cenas antigas absorvidas | Experiência principal | Conteúdo a criar |
|---|---|---|---|
| [1. Células animal e vegetal](gdd-biologia-celulas.md) | CelulaAnimalVegetal | Selecionar, girar, separar e reencaixar organelas; comparar duas células presentes | Mapa semântico das organelas, sockets de retorno e consultas |
| [2. Corpo humano e sistemas](gdd-biologia-anatomia.md) | EsqueletoHumano, SistemaCirculatorio, SistemaDigestorio, SistemaNervoso, SistemaRespiratorio | Um corpo com nove sistemas e pele, consulta de 60 estruturas, inspeção manual de 24 unidades e quatro percursos pausáveis | Controle de camadas e conteúdo científico; sistemas extras dos prefabs entram aqui |
| [3. Microrganismos e vírus](gdd-biologia-microbios.md) | Microrganismos | Sete exemplares e duas estruturas; comparação e ciclo lítico simplificado do fago | Identificação revisada, consulta, diorama de hospedeiro e etapas |
| [4. DNA, RNA e informação](gdd-biologia-dna.md) | DnaRna | Hélice de observação e encaixe de bases em duas fitas independentes | Peças semânticas, RNA, encaixes e validação de complementaridade |
| [5. Ecossistemas e relações](gdd-biologia-ecossistemas.md) | EcossistemaInterativo | Montar cadeia e distinguir energia de matéria | Diorama, organismos/fichas e fluxo; legado era cena inicial |
| [6. Evolução e seleção natural](gdd-biologia-evolucao.md) | EvolucaoEspecies | Comparar frequências em seis gerações e dois ambientes | População estilizada e modelo didático; legado era esboço |

São **exatamente seis salas**. Esqueleto, músculos, sistemas urinário, linfático e reprodutivo não criam novos slots: integram Corpo humano. Flagelo e cromossomo não viram “espécies” da galeria. Não há uma sala por órgão ou por modelo importado.

## Documentação e revisão visual

- [Análise do projeto anterior](biology/legacy-analysis.md): evidências, funcionamento encontrado e limites da auditoria.
- [Design de Biologia](biology/design-system.md): aplicação da identidade vigente, interfaces e feedback.
- [Protótipo interativo das seis salas](prototypes/biologia/index.html): ações, consultas, erros e restauração.
- [Plano de migração](biology/migration.md): fontes preservadas, GUIDs, materiais, desempenho e etapas.
- [Validação desta etapa](biology/validation.md): o que foi conferido e o que permanece pendente.
- [Fontes preservadas](../SourceAssets/BiologyLegacy/README.md): manifesto com hashes, independente da pasta antiga.

![Grade das seis propostas](../GeneratedMedia/GDD/biologia/hub.png)

Prévia HTML, não catálogo Unity atualizado. Cada GDD contém imagens de entrada, ação, consulta e estados de recuperação pertinentes; não basta aprovar apenas a grade. O [template de GDD](gdd-template.md) rege o inventário de interfaces. Capturas são locais e ignoradas por Git; fontes HTML e documentos ficam versionáveis.

## Ordem sugerida de implementação

Células → Corpo humano → Microrganismos → DNA/RNA → Ecossistemas → Evolução. As três primeiras aproveitam a maior parte do acervo; DNA adiciona construção; as últimas duas precisam de conteúdo novo. Células e Corpo humano já abrem suas cenas pelo Hub. Os demais quatro conteúdos permanecem em desenvolvimento.

## Regras comuns

Sala, iluminação, Montserrat e controles do Tecaverso atual. Os modelos biológicos conservam cores úteis; verde é acento da disciplina, não material aplicado a todo órgão. Sem HUD na cabeça e sem formulário repetido. No máximo uma consulta contextual por tarefa. Os controles são escolhidos pela atividade: objetos/sockets para montagem; botões exclusivos para sistemas; alavanca para ambiente; toggles para visibilidade.

Interação usa feedback discreto compartilhado, seleção persistente, retorno previsível e preferência de motion. Partículas somente em processos com significado, direção e legenda. Todos os recursos da versão final serão locais. Sem anúncio de LAN, de desempenho em headset ou de simulação fisiológica universal.

## Referências científicas

Referências para a revisão e a autoria dos textos, sem dependência de rede na aplicação: [células eucarióticas](https://openstax.org/books/biology-2e/pages/4-3-eukaryotic-cells), [níveis de organização](https://openstax.org/books/biology-2e/pages/1-2-themes-and-concepts-of-biology), [vírus e hospedeiros](https://openstax.org/books/biology-2e/pages/21-2-virus-infections-and-hosts), [estrutura do DNA](https://openstax.org/books/biology-2e/pages/14-2-dna-structure-and-sequencing), [fluxo de energia](https://openstax.org/books/biology-2e/pages/46-2-energy-flow-through-ecosystems) e [seleção natural](https://openstax.org/books/biology-2e/pages/19-3-adaptive-evolution), OpenStax Biology 2e. Textos breves do protótipo são redação própria, não transcrições do livro.
