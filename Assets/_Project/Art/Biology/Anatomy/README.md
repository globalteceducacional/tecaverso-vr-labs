# Arte de Corpo humano e sistemas

Migração seletiva da anatomia masculina de `SourceAssets/BiologyLegacy`, sem importar salas, rigs, scripts ou configurações antigas. O modelo feminino permanece preservado no acervo e não é uma opção desta entrega.

- `Models/Male_Full_Body_Anatomy_and_Skin_fbx.FBX`: original, com `.meta`/GUID preservados. SHA-256 `8112552033df268f9b696975dfeebd774103aa4002a595094fd40eeab7118ce8`.
- `AnatomyGeometry.json`: origem dos dez prefabs, pertencimento das 137 malhas únicas, 137 materiais e 274 texturas utilizadas. Algumas malhas são compartilhadas entre sistemas.
- `Textures`: derivados locais dos albedos e normais originais. Maior dimensão de 1024 px para albedo e 512 px para normal, mipmaps e compressão pelo importador Unity. Não são capturas: são dependências do aplicativo, versionáveis com seus `.meta` e Git LFS.
- `Materials`: variantes URP Lit com albedo, normal, cor e acabamento de origem. Não houve recoloração geral dos órgãos com o verde da disciplina.
- `Prefabs/AnatomyVisual.prefab`: geometria no referencial comum do FBX, sem as translações incorretas de alguns prefabs antigos. A escala/posição de apresentação é aplicada ao conjunto na cena.

`AnatomyAssets` prepara derivados no Editor e `AnatomyAuthoring` instala a sala uma única vez. Ambos recusam regenerar assets/cenas existentes. Não utilizar essas ferramentas para sobrescrever ajustes autorais.

Correções semânticas: medula espinal no sistema nervoso; vasos da região brônquica no circulatório; suprarrenais e seios maxilares excluídos dos grupos urinário/muscular. Dentes e discos são contexto do esqueleto, com explicação de que não são ossos. Próstata/uretra compartilham uma malha original, limitação explicitada na consulta. Faringe e cavidade oral são compartilhadas pelos sistemas pertinentes.

Os originais de alta resolução continuam fora de `Assets`; cerca de 1,5 GB de texturas brutas não são necessários para executar a sala. O runtime não lê `PROJETO ANTIGO`, `SourceAssets`, ferramentas de migração ou internet. A pasta antiga não foi apagada. Procedência/licença dos modelos exige registro a partir das aquisições antes de distribuição, conforme a [auditoria](../../../../../docs/biology/legacy-analysis.md).
