# Unity Liquid sample — subconjunto utilizado

Origem: Unity Technologies, **Shader Graph Examples — VFX Sample Project**, Asset Store ID **372864**, versão **1.1**, importado em 10/11/2026.

- [Página do pacote](https://assetstore.unity.com/packages/essentials/tutorial-projects/shader-graph-examples-vfx-sample-project-372864)
- [Tutorial do shader com movimento](https://learn.unity.com/course/get-started-with-shader-graph/unit/unit-5-three-types-of-liquid-shaders/tutorial/create-a-physics-driven-liquid-shader?version=6.3)
- Uso conforme a licença do pacote na Asset Store / Unity Asset Store EULA; estes arquivos não estão relicenciados pelo projeto.

Arquivos originais preservados, com os GUIDs da importação:

- `LiquidWScript.shadergraph`: nível horizontal, cores da superfície e oscilação X/Z.
- `BottleChunk.fbx`: malhas do frasco, volume interno e rolha.
- `TankCylinder.fbx`: malhas do recipiente cilíndrico e volume interno.
- `LiquidWobble.cs`: implementação de referência do fornecedor, preservada para consulta. Não é anexada aos frascos da aplicação.

O adaptador da aplicação é `Assets/_Project/Shared/Liquids/LiquidPresentation.cs`: usa MaterialPropertyBlock, quantidade calculada pelo laboratório, amostras volumétricas, pausa e preferência compartilhada de movimento reduzido. As malhas foram usadas para criar prefabs XR próprios; não importamos a interação de mouse/câmera da demonstração.

`Assets/_Project/Shared/Liquids/TransparentLiquid.shadergraph` é uma variante derivada do gráfico original: superfície transparente e multiplicação da máscara alpha pela propriedade `_Opacity`. Mantém o recorte do volume e as equações de wobble; permite enxergar o sólido através do líquido. O original nesta pasta permanece intacto e a licença do conteúdo de origem continua aplicável à variante.

Dependências verificadas pelo AssetDatabase: Shader Graph e URP já instalados. Nenhuma dependência das cenas, texturas, UI ou scripts restantes da amostra. O restante da importação está fora de Assets, no backup local ignorado `Temp/ShaderImportBackup/`.

A importação sobrescreveu `Assets/DefaultVolumeProfile.asset` e o `UniversalRenderPipelineGlobalSettings.asset` de VRMultiplayer, incluindo metadados. Esses dois assets e seus `.meta` foram restaurados à versão anterior. `Packages/manifest.json` e `packages-lock.json` não foram alterados. Nenhum ProjectSettings do exemplo foi adotado. A inclusão da sala Reactions no EditorBuildSettings é uma alteração própria do laboratório, não um build.
