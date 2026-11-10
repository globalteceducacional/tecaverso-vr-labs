# Base local — Tabela periódica

Revisão: 09/11/2026. Todos os dados e fotografias usados pela sala acompanham o projeto. Não há cliente HTTP no runtime.

| Arquivo | Conteúdo |
|---|---|
| `PubChemPeriodicTable.json` | Resposta local PubChem PUG REST, 118 elementos e propriedades |
| `ElementReference.json` | 118 sínteses originais em português sobre ocorrência e aplicações; legendas e créditos |
| `Resources/PeriodicSamples/` | 90 fotografias reais; texturas importadas com máximo de 512 px e carregadas uma por vez |
| `ScientificSources.json` | Proveniência PubChem PUG View e referências científicas |
| `PhotoProvenance.json` e `PHOTO_CREDITS.md` | Identificação, fonte, autor e licença individual das imagens |
| `CATALOG_LICENSE.md` | Licença CC BY-SA 3.0 do índice complementar Periodic-Table-JSON |

A licença do índice não substitui a licença de cada fotografia. As imagens são CC BY, CC BY-SA ou domínio público conforme a proveniência individual. Créditos acompanham o aplicativo na página Fontes. Imagens de descarga elétrica, material processado, mineral e solução possuem legendas específicas; não são necessariamente a aparência natural do elemento puro. Nenhuma imagem de IA é usada como evidência científica.

## Cobertura e ausências

Textos: 118/118. Fotos: 90/118. Sem foto neste acervo: 61, 84–87, 89–91, 96 e 100–118. A ficha explica falta de fotografia licenciada/validada ou impossibilidade de amostra macroscópica, conforme o caso. Isso não implica ausência do elemento. Não baixar uma imagem com copyright sem autorização nem substituir uma lacuna por uma montagem ilustrativa.

As propriedades resumidas nem sempre especificam condições experimentais e incertezas; campos vazios não significam zero. O catálogo é educativo, não modelo de simulação, especificação de reagentes ou orientação de manipulação. Diferenciar aplicações do elemento, de seus compostos e de isótopos específicos.

## Fontes e manutenção

- https://pubchem.ncbi.nlm.nih.gov/rest/pug/periodictable/JSON
- https://pubchem.ncbi.nlm.nih.gov/rest/pug_view/data/element/1/JSON (número atômico variável)
- https://iupac.github.io/WFChemCookbook/datasources/pubchem_ptable.html
- https://pubchem.ncbi.nlm.nih.gov/docs/copyright
- https://github.com/Bowserinator/Periodic-Table-JSON
- https://images-of-elements.com/ e Wikimedia Commons, com licença por arquivo.
- Complementos RSC, DOE e NASA identificados nas entradas pertinentes e nos créditos.

As ferramentas em `Tools/periodic-elements/` são executadas somente na preparação editorial. `build_catalog.py` recompõe os textos/legendas sem rede, usando a proveniência versionada quando o cache não existe. Downloads e evidência bruta ficam em `Temp/periodic-import/`. Revisar identificação, direitos, tradução, condições e importação antes de substituir o catálogo distribuído. Preservar `.meta` existentes ao atualizar fotos e versionar novos assets usados pelo aplicativo.
