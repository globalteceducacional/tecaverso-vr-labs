# Formas geométricas

Sala individual de Matemática: uma galeria de onze sólidos com nomes, contagens geométricas, fórmulas de volume e legendas acima das peças. Veja o [GDD aprovado](GDD.md).

## Uso

No Hub, escolha Matemática → Formas geométricas. Todas as peças estão visíveis ao entrar. Caminhe entre as duas fileiras e observe os sólidos. O único painel contém **Exibir vértices**, **Exibir arestas**, **Girar sólidos** e **Voltar ao Hub**. Os estados iniciais são vértices desligados, arestas e rotação ligados. Apontar com um interator XR pausa a rotação da peça; sair retoma quando a rotação está habilitada.

A coleção reúne tetraedro, pirâmide de base quadrada, prismas triangular/pentagonal/hexagonal, cubo, paralelepípedo retângulo, octaedro, dodecaedro, icosaedro e esfera. Os cinco poliedros regulares usam fórmulas correspondentes a sólidos regulares. A esfera não mostra vértices nem arestas da tesselação.

Foram removidos da cena os painéis de edição e comparação A/B, abas, campos, desafios e planificação. Nenhum menu precisa ser usado para disponibilizar uma peça.

Para construir e editar pontos, retas, superfícies e sólidos, use a sala separada de [Espaço cartesiano](../CartesianSpace/README.md). Esta galeria apresenta somente sólidos 3D e fórmulas simbólicas de volume.

## Organização

- `Scenes/GeometricShapes.unity`: cena serializada, com as onze peças disponíveis também em Edit Mode. Ambiente, rig, iluminação e pós-processamento preservados.
- `Model/SolidGalleryCatalog.cs`: seleção de formas e fórmulas. Reutiliza a topologia matemática de `ShapeGeometry`.
- `Scripts/SolidGallery.cs`: três preferências de apresentação e retorno ao Hub.
- `Scripts/SolidExhibit.cs`: rotação da geometria, pausa por hover XR e orientação horizontal do rótulo a partir da posição do observador. O rótulo permanece fora da hierarquia giratória.
- `Meshes/` e `Materials/`: malhas e materiais persistentes. Uma malha de superfície, uma de arestas e uma de marcadores por poliedro; as arestas são únicas e não incluem diagonais internas das faces.
- `Tests/`: medidas, topologia, fórmulas e verificações geométricas.
- `Assets/_Project/Editor/ShapesAuthoring.cs`: migração única do conteúdo antigo, recusada se a galeria já existir. Não usar como atualizador da cena. Refinamentos devem editar os objetos existentes.

Os scripts anteriores `ShapesLab`, `ShapeView`, `ShapeGrab` e `ShapeElement` não são mais referenciados pela cena. Permanecem como código legado, sem execução nesta experiência. A biblioteca de geometria mantém operações utilizadas pelos testes históricos.

## Escala e apresentação

Sala existente preservada. Sete sólidos no fundo e quatro à frente, com aproximadamente 4 m entre fileiras e centros a 1,4–1,75 m do piso. Peças com aproximadamente 1–1,6 m de extensão; as fórmulas são simbólicas, sem calcular volumes a partir de metros de cena. Painel de controles próximo à entrada; rótulos acima de cada sólido. A interface segue a identidade do projeto sem impor a estrutura de formulário dos outros laboratórios.

## Validação — 03/11/2026

- Compilação do Editor sem erros.
- 20 testes EditMode aprovados: 11 existentes e 9 casos novos, cobrindo prismas, pirâmide, paralelepípedo e fórmulas dos poliedros regulares.
- Play Mode: 11 peças, três toggles independentes, um botão; esfera sem destaques indevidos; pausa por hover e retomada da rotação; rótulos com posição estável; retorno pelo botão ao Hub e recarga da sala, com um único rig.
- Inspeção das superfícies: nenhuma face com orientação invertida. TextMeshPro sem overflow nos rótulos e controles. Capturas do ambiente e do painel revisadas.
- A inicialização do OpenXR reportou avisos de referências de scripts ausentes nas configurações do pacote. Nenhum GameObject da sala tem script ausente. Configurações XR pré-existentes foram preservadas; a recarga da sala dentro de Play Mode não gerou novos erros ou avisos.

Ainda requer homologação com headset para legibilidade, alcance dos controles e desempenho. Sala individual, sem sincronização LAN ou persistência. Não foi gerado build.

As malhas em `Meshes/`, os materiais e a cena são recursos necessários ao laboratório e devem ser versionados com seus `.meta`. Capturas de conferência ficam em `Temp/ShapesQA/`, ignorada pelo Git; não são dependências da sala. Consulte a [política de mídias geradas](../../../../../docs/development-guidelines.md#midias-geradas).
