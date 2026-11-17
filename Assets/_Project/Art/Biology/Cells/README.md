# Arte de Células

Migração seletiva de dois modelos e sua geometria autoral, sem importar o projeto antigo como aplicação. Origem: `SourceAssets/BiologyLegacy/Assets/_Project/Models/Biologia/Celulas Animal e Vegetal/` e os dois prefabs de Células preservados.

- `Models`: FBX originais e `.meta` com GUIDs preservados.
- `GeometryMap.json`: IDs, hierarquia, posição/rotação/escala e referências originais dos 72/78 GameObjects. Não é um inventário científico de todas as peças.
- `Prefabs`: geometria reconstruída pelo Editor, sem os scripts/callouts/physics antigos.
- `Materials`: variantes URP dos materiais embutidos; as cores de origem foram mantidas.

As dependências runtime ficam dentro de Assets; a pasta antiga pode ser removida posteriormente sem quebrar a sala. Não remover este acervo de runtime ou seus `.meta` com regras genéricas de exclusão de imagens. Os FBX desta pasta estão configurados para Git LFS. Capturas e vídeos ficam em `GeneratedMedia`, ignorados pelo Git.

Rastreio da migração e situação de autoria/licenças: [auditoria](../../../../../docs/biology/legacy-analysis.md). Metadados científicos e limites da atividade: [README da sala](../../../Laboratories/Biology/Cells/README.md).
