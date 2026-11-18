# Fontes de Biologia preservadas

Cópia seletiva de `PROJETO ANTIGO/tecaverso-vr`, realizada em 17/11/2026. O [manifesto](manifest.json) registra caminhos, tamanhos e SHA-256 dos 1.568 arquivos copiados, incluindo `.meta`. A pasta original não foi apagada.

Esta pasta fica **fora de Assets** para evitar importação automática de aproximadamente 1,88 GB de fontes. Não é um segundo projeto Unity executável: cenas/prefabs são registros de montagem e dependem do ambiente e de pacotes não copiados. `ProjectSettings/ProjectVersion.txt` e `Packages/manifest.json` são referências históricas, nunca devem substituir as configurações atuais.

Preservados: 14 modelos FBX (células animal/vegetal, anatomia masculina/feminina, DNA e nove modelos de micróbios/estruturas), 249 materiais, 465 texturas, 21 prefabs, dez cenas e scripts de referência. A anatomia feminina estava disponível como modelo, mas não era a base dos dez prefabs de sistemas masculinos.

Antes de importar uma seleção para `Assets/_Project/Art/Biology/`, seguir o [plano de migração](../../docs/biology/migration.md): comparar GUIDs, selecionar malhas e materiais usados, validar normais, pivôs, escala e colliders; adaptar prefabs de interação ao XR atual e medir custo. Não copiar configurações, rigs, scripts ou salas antigas por cima dos novos. Não executar cenas históricas desta pasta como se fossem os novos laboratórios.

O versionamento é seletivo por laboratório. A entrega de Células inclui os dois FBX, os dois prefabs de origem e seus `.meta`, além deste README e do manifesto completo. Os FBX de origem e runtime são idênticos e compartilham os mesmos objetos no Git LFS. O restante do acervo bruto preservado fica local até a migração dos próximos laboratórios: sua presença no manifesto não significa que todos os arquivos acompanhem um clone. Os 1.568 arquivos continuam preservados neste workspace.

Os hashes do manifesto registram a cópia original local. Git pode normalizar finais de linha dos arquivos textuais ao versioná-los; isso não altera GUIDs ou conteúdo semântico, mas pode alterar seu hash de bytes em outro checkout. Os FBX no LFS preservam os bytes originais e seus hashes foram conferidos.

Corpo humano e sistemas agora usa uma cópia do FBX masculino e derivados menores de 274 texturas em `Assets/_Project/Art/Biology/Anatomy`, com rastreio em `AnatomyGeometry.json`. O runtime não precisa dos PNGs brutos desta pasta. A migração não altera nem elimina essas fontes; o modelo feminino continua preservado para possível adaptação futura.

Os binários desta pasta têm regras próprias de Git LFS em `.gitattributes`. Capturas e vídeos permanecem em `GeneratedMedia/`, ignorados. Nenhum push é necessário para usar a sala localmente.

Não foram encontrados arquivos de licença/autoria dos modelos dentro das pastas de modelos auditadas. Isso não determina que sejam livres ou proibidos: a origem/licença deve ser registrada a partir das aquisições do projeto antes de distribuição. As fontes são mantidas sem alterar os originais. Não apresentar o autor do Tecaverso como autor dos modelos de terceiros.
