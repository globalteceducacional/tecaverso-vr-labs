# Auditoria de Biologia da versão anterior

**Data:** 17/11/2026. **Escopo:** aplicação antiga, cenas/conteúdo de Biologia e suas dependências de arte. **Entrega atual:** documentação, preservação e protótipos; nenhuma nova cena Unity disponível ainda.

## Fonte e método

Há duas cópias aninhadas: `PROJETO ANTIGO/tecaverso-vr` (Unity **6000.3.17f1**) e `PROJETO ANTIGO/tecaverso-vr/tecaverso-vr` (**6000.0.32f1**). A cópia externa é a referência: dez cenas de Biologia, prefabs especializados e a pasta de modelos. A interna é mais antiga, tem seis cenas no catálogo, mas não contém o acervo autoral de modelos/prefabs da cópia mais recente.

Foram lidos os YAML das dez cenas e dos 21 prefabs, referências por GUID, scripts autorais compartilhados, materiais/remapeamentos e arquivos de modelo. O inventário reprodutível está em [legacy-audit.json](legacy-audit.json), gerado por `Tools/biology-migration/audit.py`. O fechamento de dependências leva em conta materiais e `.meta`, não apenas os FBX.

Inspeção visual pela interface do Editor antigo: **CelulaAnimalVegetal, SistemaRespiratorio e Microrganismos**. Foram observados os modelos coloridos, sua distribuição e os suportes/ambiente anteriores; não houve alteração/salvamento dos assets. O MCP depois confirmou a raiz `.../PROJETO ANTIGO/tecaverso-vr`, Unity 6000.3.17f1, Editor ocioso fora de Play e a hierarquia de Microrganismos. Instância selecionada: `tecaverso-vr`; a instância do projeto atual ficou fora das consultas. Não alegar que todas as cenas foram testadas em Play Mode.

## Conteúdo por cena e destino

| Cena antiga | Evidência de conteúdo | Interação encontrada/limite | Destino |
|---|---|---|---|
| CelulaAnimalVegetal | Animal_Cell e Plant_Cell | Partes com colliders/Rigidbody e XRGrab; nomes parcialmente semânticos, muitos ObjectXX/Plant_Cell_XX | Células |
| EsqueletoHumano | Body e Skeleton | Grupos ósseos separáveis; não comprova 206 ossos individualizados ou biomecânica | Corpo humano |
| SistemaCirculatorio | Body e SistemaCirculatório | Subconjuntos arterial/venoso, coração/vasos | Corpo humano |
| SistemaDigestorio | Body e Sistema Digestorio | Estômago, intestinos, fígado, pâncreas e outros agrupamentos | Corpo humano |
| SistemaNervoso | Body e NervousSystem | Encéfalo, lobos e componentes nervosos | Corpo humano |
| SistemaRespiratorio | Body e Sitema Respiratorio | Pulmões, vias, diafragma; corpo e conjunto exibidos separados na cena | Corpo humano |
| Microrganismos | Nove prefabs/FBX | MCP confirma XRGrabInteractable e XRGeneralGrabTransformer nos exemplares; galeria em fileira a y≈1,5 m | Microrganismos e vírus |
| DnaRna | dna_strand_set, partes de hélices | Observação/pega de estruturas; não foi encontrado sistema autoral de síntese ou pareamento | DNA/RNA |
| EcossistemaInterativo | Ambiente/lighting inicial | Não foi encontrado diorama/organismos ou modelo de relações próprio | Ecossistemas, conteúdo novo |
| EvolucaoEspecies | Ambiente/mesa inicial | Não foi encontrado modelo populacional, seleção ou gerações | Evolução, conteúdo novo |

A contagem de GameObjects declarados diretamente na cena não mede sua complexidade: muitas peças estão em instâncias de prefabs e referências importadas. Por isso o relatório relaciona dependências, não conclui “sala vazia” apenas pelo número de objetos raiz.

## Acervo recuperável

**14 modelos FBX, 249 materiais e 465 texturas**, cerca de **1,875 GB decimais** de arte. Preservação total com metadados, cenas/prefabs/scripts de referência: **1.568 arquivos**. Sem alterar os originais. [Manifesto e hashes](../../SourceAssets/BiologyLegacy/manifest.json).

| Grupo | Arte e estrutura | Avaliação de reaproveitamento |
|---|---|---|
| Células | 2 FBX; Animal_Cell com 72 GameObjects, Plant_Cell com 78 | Bom valor visual; mapear organelas e escolher peças destacáveis sem mover colliders de leitura |
| Anatomia masculina | 1 FBX e 10 prefabs de Body/sistemas; 155 nós de geometria no leitor binário | Grande valor; manter alinhamento dos sistemas e importar seletivamente |
| Anatomia feminina | 1 FBX adicional | Preservado como acervo; não há prefabs equivalentes de sistemas comprovados. Não oferecer alternância pronta |
| DNA | 1 FBX com hélices/partes | Útil à observação; montagem e RNA serão novos |
| Micróbios/estruturas | 9 FBX e 9 prefabs | Sete exemplares artísticos e duas estruturas; revisar taxonomia e nomes antes de ensinar |

Os dez prefabs anatômicos incluem Body, Skeleton (18 GameObjects), Muscles (27), NervousSystem (35), Digestório (13), Renal (9), Reprodutivo (6), Circulatório (18), Respiratório (12) e Linfático (15). Grupos de renderização não são sinônimos de órgãos individuais ou células funcionais. O inventário mantém inclusive erros de nomes do legado para rastrear a origem.

O modelo `Animal_Cell` é uma representação de célula animal genérica; não foi encontrado um catálogo de tipos celulares humanos especializados. Não renomeá-lo como neurônio, hemácia ou outra célula específica sem modelo e revisão correspondentes.

## Arquitetura e comportamento anterior

As cenas usam ambiente compartilhado (`SalaBiologia`), XR Interaction Manager, EventSystem e recursos do template XR. O fluxo autoral de navegação encontrado é `SceneLoader.LoadScene`, com menu de punho em `WristUI` (retomar, recarregar cena e voltar ao MainMenu). `AffordanceToggle` exibe/oculta o filho `Affordance Callout` durante a pega; isso dá identificação contextual, mas depende do nome do filho e do grab no mesmo objeto. `ObjectTransformDisplay` atualiza posição/rotação/escala em TMP; é ferramenta genérica, não um modelo biológico.

Não foi encontrado, nos scripts autorais do projeto, um domínio de fisiologia, genética, ecossistemas ou evolução equivalente aos modelos e comandos atuais. O que existe de mais reaproveitável é o **conteúdo visual e sua separação em peças**, somado a interações genéricas XR. Não descrever a versão anterior como simulador biológico completo. A ausência de callbacks persistentes nos YAML não elimina os eventos internos do XR; os prefabs efetivamente possuem componentes de pega.

O projeto atual separa domínio, comandos, apresentação e infraestrutura; usa sala atual, navegação Hub, controles Unity e `Shared/Interaction`. A migração aproveitará arte, hierarquias e posições locais; não copiará menu de punho, teleporte, rigs, input actions e scripts antigos sobre os novos. Biologia começa offline individual; nada encontrado nesta análise autoriza prometer LAN nela.

## Qualidade, dependências e custo

- Os dois GUIDs de script mais recorrentes são de `XRGrabInteractable` e `XRGeneralGrabTransformer`, disponíveis nos pacotes atuais. URP Lit e metadado de versão URP também foram resolvidos no PackageCache atual. Isso não certifica os prefabs em uma importação real.
- A leitura binária estima **1.352.344 triângulos** no modelo masculino e **1.567.476** no feminino. [Detalhe técnico](model-topology.json), `Tools/biology-migration/fbx_stats.py`. O leitor não cobre todas as variantes de FBX: zero/não analisado nas células não significa malha sem geometria. Números são contagem estática, não benchmark.
- Não há validação recente de LOD, draw calls, transparências ou memória de GPU em headset. As texturas e o corpo completo exigem revisão antes de importação em runtime. Não recolorir tudo de azul para esconder incompatibilidades.
- Colisores simples e pegas limitadas são preferíveis a um MeshCollider/Rigidbody por submalha. As partes anatômicas preservam orientação e escala de origem; qualquer ajuste de proporção deve ser explícito.
- Licenças/autorias não foram localizadas nas pastas de modelos auditadas. Registrar procedência antes de distribuição. Nenhum download de modelos externos ou compra foi realizado.

## Resultado da consolidação

As dez cenas tornam-se [seis propostas](../gdd-biologia.md): Células; Corpo humano e sistemas; Microrganismos e vírus; DNA/RNA; Ecossistemas; Evolução. Cinco cenas anatômicas e os sistemas extras dos prefabs se unem no mesmo laboratório. Ecossistemas e Evolução continuam propostas de conteúdo novo; sua existência como arquivo `.unity` antigo não é evidência de funcionalidade.

Arquivos antigos estão ignorados por Git. A cópia seletiva permite remover a pasta antiga sem perder o acervo de Biologia listado no manifesto, após fechar seu Editor. Outras disciplinas, configurações e pacotes antigos não foram preservados como aplicação executável. Veja o [plano de migração](migration.md).
