# Curadoria offline da tabela periódica

O runtime usa somente `Assets/_Project/Laboratories/Chemistry/PeriodicBonds/Data/`. Estas ferramentas Python 3 são de preparação editorial, sem dependências adicionais.

- `content-pt.tsv`: 118 textos originais em português, ocorrência e aplicações separados por `|`.
- `build_catalog.py`: recompõe catálogo, legendas e créditos **sem internet**, usando as fontes versionadas; cache editorial em Temp é opcional. Executar `python Tools/periodic-elements/build_catalog.py`.
- `import_catalog.py --photos`: restaura apenas fotos ausentes a partir do manifesto local revisado. Requer rede; nunca substitui fotos existentes. Aborta em erros/limites de acesso, sem repetir indiscriminadamente requisições.
- `import_catalog.py --evidence`: consulta PubChem PUG View somente na preparação e armazena a evidência em Temp. Atualizações científicas exigem revisão humana do texto antes de alterar o catálogo.

Importação inicial: candidatos do Periodic-Table-JSON; licenças verificadas em Wikimedia Commons e nas páginas de Images of Elements. Foram descartadas montagens, ilustrações e fotos sem licença de redistribuição confirmada. Dados finais: 118 elementos, 90 fotos reais, 28 explicações de ausência. Fontes complementares RSC/DOE/NASA estão nas entradas correspondentes.

Adicionar uma foto exige identificação científica, fonte, autor, licença e legenda; atualizar `PhotoProvenance.json` e a curadoria, não apenas colar uma URL. Unity importa as fotos com máximo de 512 px. Os assets e `.meta` são versionados; evidência bruta, prévias e capturas ficam ignoradas em Temp. Créditos exibidos no aplicativo e `PHOTO_CREDITS.md` devem continuar sincronizados.
