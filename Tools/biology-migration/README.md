# Ferramentas locais de migração de Biologia

Não integram o runtime nem são necessárias para executar Células. Python usa somente a biblioteca padrão; captura HTML usa as dependências já presentes em `Tools/tecaverso-demo`.

- `cell_geometry.py`: refaz `GeometryMap.json` a partir dos dois prefabs de origem de Células versionados em `SourceAssets/BiologyLegacy`.
- `audit.py`, `fbx_stats.py`, `preserve.py`: auditoria/preservação do acervo completo. Requerem a pasta antiga ou o acervo bruto local conforme seus caminhos; não presumir que todos os arquivos do manifesto acompanhem o clone. A preservação verifica hashes e não apaga a origem.
- `verify-prototype.cjs`, `serve-prototype.cjs`: testes/capturas e servidor local de revisão dos protótipos aprovados; saídas em pastas ignoradas.
- `write-design.py`: gerador histórico da primeira proposta. **Não executar sobre a documentação atual**: sobrescreve GDDs/protótipos e não incorpora todas as revisões posteriores à implementação.
- `mcp_read.py`, `mcp_cells.py`: ferramentas locais de Editor; sessões e respostas em `tmp`, ignoradas. O cliente de Células confirma a raiz do projeto antes de executar uma ferramenta. Não há chave de provedor ou dependência de rede no aplicativo.

Os instaladores de Editor `CellsAssets`/`CellsAuthoring` consomem os FBX e o mapa em `Assets`, criando assets pela API Unity. A sala existente recebe ajustes pontuais; não regenerá-la para alterar interação ou layout. Sem build automático.
