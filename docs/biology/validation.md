# Validação de Biologia — 17/11/2026

## Auditoria e protótipos anteriores à implementação

**Escopo inicial:** auditoria/preservação de fontes, documentação e protótipos HTML. Os resultados desta seção não validam as seis salas na Unity.

- Cópia seletiva: 1.568 arquivos conferidos por SHA-256 durante a preservação; manifesto em `SourceAssets/BiologyLegacy/manifest.json`.
- MCP local: duas instâncias detectadas; consultas fixadas no projeto antigo `tecaverso-vr`, raiz confirmada, Editor fora de Play e pronto para ferramentas. Inspecionados hierarquia de Microrganismos e metadados de Animal_Cell; nenhuma mutação de cena/asset pelo MCP.
- Inspeção visual no Editor antigo: Células, Sistema respiratório e Microrganismos. Não foram executados testes funcionais completos das dez cenas antigas.
- HTML: **dez verificações focadas aprovadas** em `Tools/biology-migration/verify-prototype.cjs`: grade de seis, pareamento/recusa/persistência DNA/RNA, cancelamento de reset, cadeia alimentar, limite/referência de gerações, consulta preservando peça destacada, separação/reencaixe, troca de sistema sem percurso oculto, etapas do fago e ausência de overflow horizontal em consultas a 375/768 px.
- **31 capturas** reproduzíveis em `GeneratedMedia/GDD/biologia/`; revisão visual de entrada/ação/consulta/erro e disposição. As imagens dos GDDs são conceituais, não capturas de implementação Unity.
- Documentação: seis GDDs individuais; 37 links de imagens e referências locais dos documentos de Biologia conferidos, sem destino ausente.
- Git: pasta antiga e mídias de revisão ignoradas; regras LFS confirmadas para FBX/FBX e texturas da pasta de fontes. Isso não equivale a upload ou validação de um commit dessas fontes. Nenhum commit/push nesta etapa.

Os seis GDDs foram aprovados posteriormente. Células e Corpo humano foram implementados conforme as seções seguintes. As outras quatro continuam pendentes, assim como procedência/licença para distribuição, revisão científica e homologação em headset. Não houve build, substituição de cenas atuais ou importação em massa do legado.

## Células animal e vegetal — Editor e Play Mode

- Cena nova `Assets/_Project/Laboratories/Biology/Cells/Scenes/Cells.unity`, isolada com um rig. Dois modelos antigos importados seletivamente e convertidos em apresentação atual.
- Oito alvos de seis categorias: núcleo e mitocôndria em ambas as células; membrana animal; cloroplasto, vacúolo central e parede vegetal. Seis alvos destacáveis; membrana/parede fixos.
- `CellsValidation.Run()` aprovado: inventário e referências, motion compartilhado, filtros por identidade dos sockets, consulta, fechar sem reencaixar, cancelar preservando estado, recompor, limite físico de 0,4 m, entrada/saída de seleção XR real através de interator temporário, rótulos e preservação de preferência.
- Raycasts da câmera inicial atingiram corretamente os oito colliders correspondentes. Consulta testada com 247 caracteres sem overflow no título, corpo ou botão.
- Revisão visual dos estados de entrada, rótulos e consulta. Giro de 90° confirmado, com rótulos fixos na orientação de leitura. Limites projetados da ficha e do rótulo selecionado separados, sem interseção; texto da consulta sem overflow. Linhas apontam para os colliders das estruturas.
- Navegação real: botão Voltar da sala carregou Hub com um rig; Explorar sozinho → Biologia mostrou seis entradas, Células disponível; Iniciar simulação carregou a cena habilitada.
- Console sem erros nas verificações. Evidências e capturas reais em `GeneratedMedia/Cells/`, ignoradas; fonte dos testes em `Assets/_Project/Editor/CellsValidation.cs`.

Essas verificações não substituem controladores físicos. Permanecem pendentes leitura angular, conforto sentado/em pé, alcance e desempenho em headset, além de revisão científica independente. Não foi gerado build e a sala não oferece LAN. Fontes TMP compartilhadas já estavam alteradas antes desta etapa; a renderização no Editor também atualizou seus caches de glifos, sem reverter alterações anteriores do usuário.

## Revisão transversal de interação

Caixas de bounds e seleção de Células substituídas pelo [padrão comum das quatro disciplinas](../gdd-interaction-feedback.md). Os testes funcionais acima passaram novamente com o novo componente. [Regressão das 17 cenas e capturas](../interaction-feedback-validation.md). As capturas antigas de `GeneratedMedia/Cells` são históricas; o GDD agora aponta para a apresentação atual em `GeneratedMedia/InteractionFeedback`.

## Corpo humano e sistemas — Editor e Play Mode

- Nova cena `Assets/_Project/Laboratories/Biology/Anatomy/Scenes/Anatomy.unity`, um rig, sem scripts ausentes na hierarquia. Ambiente atual reutilizado de Células; nenhum rig, UI ou script antigo importado. Catálogo mantém seis conteúdos de Biologia.
- FBX masculino original com SHA-256 conferido; 137 malhas únicas mapeadas nos sistemas/pele, 137 materiais URP e 274 texturas derivadas locais. Albedos até 1024 px e normais até 512 px: 204,7 MiB de PNGs derivados, frente a aproximadamente 1,5 GB de texturas brutas. Não é uma medição de memória GPU nem homologação de performance.
- Correção de pertencimento de medula espinal e vasos brônquicos; exclusão das suprarrenais e seios maxilares dos grupos inadequados. Textos explicitam limites de grupos, dentes/discos e malha conjunta próstata/uretra. Modelo feminino não adaptado.
- `AnatomyValidation.Run()` aprovado: nove sistemas exclusivos; 60 consultas sem overflow TMP; pele independente sem collider; altura de 1,7 m; colliders de sistemas ocultos inativos; feedback compartilhado em todos os alvos; quatro percursos/pausa; troca encerra consulta e partículas; evento real de seleção XR por interator temporário e cancelamento ao trocar sistema; catálogo de seis entradas.
- Raycasts da câmera inicial acertaram traqueia e ambos os pulmões. Dial em 0,25 resultou em giro de 90°, sem alterar altura corporal; consulta e rótulos continuam legíveis. Peças e percursos compartilham pivô e não são recentrados por sistema.
- Percursos avançaram entre frames; pausa manteve a fase respiratória exatamente em 0,485090643 entre chamadas separadas. Partículas preservadas ao pausar e limpas na troca. Dois ramos respiratórios, sangue com transição simbólica de oxigenação, digestão em sentido único e sinal nervoso recebem legendas próprias.
- Revisão visual de entrada, consulta, quatro percursos, giro e órgãos em 45°. Corrigida a face do console inicialmente encoberta pelo tampo e o contraste dos botões não selecionados. Ficha lateral não ocupa a projeção do corpo. Corpo/base afastados rigidamente 0,9 m do console, deixando folga frontal de 1,51 m, sem separar sistemas. Após esse ajuste, a verificação funcional passou novamente.
- Conferidos os 137 materiais (albedo e normal presentes) e os 274 importadores (limites de resolução e leitura CPU desabilitada). As 567 dependências da cena não apontam para `SourceAssets` ou `PROJETO ANTIGO`.
- Navegação real: Voltar ao Hub → Explorar sozinho → Biologia → Corpo humano e sistemas → Iniciar carregou a sala com um rig.
- Console sem erros de execução na verificação final. Há avisos globais anteriores de referências de features OpenXR durante a inicialização do XR (`FeatureHelpers.cs`); a cena não contém MonoBehaviours ausentes. As configurações OpenXR existentes do usuário não foram reescritas.

Capturas reais em `GeneratedMedia/Anatomy/`, ignoradas pelo Git, separadas das propostas HTML. GDD e READMEs atualizados. FBX/PNGs de runtime têm filtro LFS; não são gravações e devem acompanhar o aplicativo com seus `.meta`. Não houve commit/push ou build nesta entrega. Interatores sintéticos e capturas não substituem teste sentado/em pé, alcance, desempenho e legibilidade em headset, nem revisão científica independente.

### Revisão de pega, percursos e dial

- Na gravação inicial, a circulação saía da região vascular aos 60 s, o traçado nervoso passava acima do encéfalo aos 128 s e a manopla ficava sob a placa aos 150 s. Nessa versão a seleção abria a consulta e mantinha o órgão preso ao corpo.
- Na verificação XR da revisão, mover o controlador 5 mm manteve o coração no lugar. A retirada de 22 cm e a rotação de 40° movimentaram o órgão. Girar a base durante o exame e soltar restituiu a pose local do coração. Verificadas também disputa entre mãos, consulta de rede fixa, troca de sistema durante a pega e devolução imediata com movimento reduzido.
- A suíte percorreu as 60 fichas e as 22 etapas dos quatro processos. A retirada suspendeu a representação enquanto a malha esteve deslocada. A devolução recuperou a fase. Um percurso pausado pelo aluno continuou pausado.
- O shader URP passou na compilação do Editor. As ondas seguem as superfícies anatômicas. Em duas rodadas de capturas, a primeira revelou excesso de opacidade nas regiões amplas e o rótulo do dial atrás do tampo. Após a correção, as texturas permaneceram legíveis e o rótulo apareceu na face de operação mesmo depois de salvar e reabrir a cena.
- O teste de raio alcançou primeiro o collider da manopla a partir de alturas de olhos de 1,15 m e 1,65 m. Um interator direto acionou o XRKnob por torção. Capturas frontal, oblíqua e dorsal ajudam a inspecionar sobreposições e profundidade. Os arquivos estão em [RevisaoFinal](../../GeneratedMedia/Anatomy/RevisaoFinal/).

A avaliação ocorreu no Editor, com interatores de teste. Conforto, desempenho no headset e revisão independente da anatomia permanecem pendentes.
