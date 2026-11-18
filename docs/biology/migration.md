# Migração consciente dos assets de Biologia

**Etapa atual, 17/11/2026:** seis GDDs aprovados; primeira sala, [Células animal e vegetal](../../Assets/_Project/Laboratories/Biology/Cells/README.md), implementada em cena nova e verificada no Editor. Auditoria, preservação e protótipos precederam a implementação. Não houve build ou importação em massa de fontes.

## Fontes preservadas e exclusão da pasta antiga

`SourceAssets/BiologyLegacy/` contém 1.568 arquivos copiados e conferidos por SHA-256: modelos, materiais/texturas, metadados, 21 prefabs, dez cenas e scripts históricos. [Manifesto](../../SourceAssets/BiologyLegacy/manifest.json). As configurações históricas têm função documental; nunca substituem as atuais. A pasta está fora de `Assets`, evitando importação de ~1,88 GB antes da escolha de conteúdo.

A preservação cobre o **acervo de Biologia auditado**, não todo o projeto anterior ou suas demais disciplinas. Cenas preservadas não são cópia executável da aplicação antiga: ambiente, lightmaps, rigs e pacotes ficam fora do escopo. A subcópia Unity 6.0 não contém os modelos autorais de Biologia da cópia Unity 6.3 e não foi duplicada.

`/PROJETO ANTIGO/` está ignorada pelo Git e permanece no disco. Nenhuma deleção foi feita. Para removê-la depois, fechar primeiro o Editor que a utiliza; os modelos e montagens de Biologia preservados não dependem dessa pasta. Se desejar conservar outras disciplinas da versão antiga, manter backup separado antes de removê-la.

## Importação por laboratório, após aprovação

1. Selecionar somente os modelos/sistemas do GDD aprovado e dependências pelo manifesto. Originais continuam em `SourceAssets`; criar runtime em `Assets/_Project/Art/Biology/` e prefabs de apresentação por sala.
2. Comparar GUIDs do lote com `Assets` e pacotes instalados. Preservar `.meta` quando não houver colisão. Em colisão, distinguir recurso idêntico de outro asset; não sobrescrever. Remapeamentos precisam de relatório.
3. Materiais URP: preservar originais, criar variantes apenas quando necessário; conferir normal, roughness/smoothness, textura linear/sRGB e transparência. GUIDs de URP Lit e XRGrab encontrados no legado existem nos pacotes atuais; não significa que todos os materiais estejam visualmente homologados.
4. Analisar submalhas e nomes. Criar IDs semânticos próprios e relação `ID → caminho original/fileID`; não identificar espécies/órgãos por cor ou sequência ObjectXX. Manter transforms locais e alinhamento entre sistemas.
5. Otimizar cópias derivadas: recortar sistemas não usados do modelo completo, rever LOD, texturas e materiais. Inspeção binária estima ~1,35 M triângulos no FBX masculino e ~1,57 M no feminino; estes números não medem custo real de cena. O leitor não cobre toda variante FBX e não certifica as células. Nenhuma malha foi decimada automaticamente.
6. Criar prefabs visuais com root ancorado, pivô validado, collider simples e área de pega; adicionar XR atual de forma seletiva. Scripts antigos não são copiados como lógica de domínio. Separar domínio, comandos, apresentação e metadados locais.
7. Nova sala usa ambiente/prefabs atuais, `Shared/Interaction`, controles Unity e navegação Hub. Não rodar builders sobre cenas existentes nem copiar ProjectSettings/Packages da aplicação anterior.
8. Verificar compilação/console e fluxo no Editor. Depois validar headset e performance por laboratório. Só disponibilizar no catálogo ao funcionar. LAN permanece fora desta primeira entrega de Biologia.

## Versionamento e licenças

Modelos e texturas preservados possuem regras de LFS restritas a `SourceAssets/BiologyLegacy/` (`.png`, `.fbx` e `.FBX`). O versionamento é feito por lote: esta entrega inclui os modelos/prefabs de origem de Células e o manifesto; o restante do acervo bruto continua local, fora do commit de Células. Não inferir que todo caminho do manifesto acompanha um clone. Não ignorar globalmente imagens de runtime. Capturas/roteiros exportados em `GeneratedMedia`, já ignorados.

Licenças/autorias dos modelos não foram localizadas nos diretórios de modelos fornecidos. Registrar compra/origem e atribuição antes de distribuição; não inferir licença livre pela presença no projeto. Não buscar arquivos privados ou credenciais para completar essa informação.

## Limites de independência

Após a remoção da pasta antiga, a auditoria e os protótipos continuam disponíveis. Para repetir `audit.py`, é necessário recolocar a fonte antiga; para usar o acervo e conferir sua integridade, basta o manifesto da pasta preservada. O conector `mcp_read.py` é uma ferramenta local de auditoria do Editor aberto, não dependência da aplicação final. Sessões/respostas ficam em `tmp`, ignoradas.

## Lote implementado: Células

Foram importados somente `Animal_Cell.fbx` e `Plant_Cell.fbx`, com GUIDs originais conferidos sem colisão. `GeometryMap.json` conserva a montagem visual dos prefabs antigos. Novos prefabs visuais e materiais URP não carregam scripts, interface, física ou ambiente antigos. O mapa de oito alvos consta no README da sala; seis categorias foram associadas a grupos semânticos existentes, sem inferir organelas por nomes genéricos.

`Cells.unity` reutiliza ambiente, rig, componentes XR, controles oficiais Unity e feedback compartilhado do projeto atual. Está habilitada no catálogo de seis conteúdos de Biologia. Corpo humano foi implementado posteriormente; quatro entradas ainda não apontam para cenas. Os FBX de runtime também têm regras LFS restritas à pasta de arte de Biologia. `SourceAssets` e `PROJETO ANTIGO` não são lidos pela aplicação. Headset, otimização medida e revisão científica independente continuam etapas de homologação.

## Lote implementado: Corpo humano e sistemas

Um FBX masculino importado com GUID/hash originais. O pertencimento das peças vem dos dez prefabs históricos; as poses vêm do referencial comum do FBX, evitando raízes deslocadas de prefabs antigos. `AnatomyGeometry.json` registra fontes, materiais, texturas e correções semânticas. Apenas o conjunto recebe escala de 1,7 m, orientação de apresentação e posição na sala.

Albedos e normais foram derivados pelo Editor em PNGs menores, preservando originais fora de `Assets`. Não foi necessário importar cerca de 1,5 GB de texturas brutas, nem o segundo modelo corporal. A regra LFS de arte de Biologia agora também cobre os PNGs runtime; capturas continuam ignoradas. Os derivados são suficientes num clone para executar a sala. Reproduzir sua autoria exige as fontes correspondentes do acervo local preservado.

`Anatomy.unity` tem nove sistemas, pele independente, 60 grupos de consulta XR, dial Unity e quatro percursos didáticos pausáveis. O aluno pode afastar e girar 24 peças e devolvê-las à pose anatômica ao soltar. Verificações e limitações em [validation.md](validation.md).
