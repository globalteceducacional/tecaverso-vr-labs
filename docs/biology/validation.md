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

Os seis GDDs foram aprovados posteriormente. A primeira sala foi implementada conforme a seção seguinte. As outras cinco continuam pendentes, assim como procedência/licença para distribuição, revisão científica e homologação em headset. Não houve build, substituição de cenas atuais ou importação em massa do legado.

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
