# Geometria plana

Sala individual disponível em **Explorar sozinho → Matemática → Geometria plana**. Cena: `Scenes/PlanarGeometry.unity`. [GDD aprovado](../../../../../docs/gdd-geometria-plana.md).

## Uso

As treze figuras estão expostas em U, com espaço central para caminhar. Cada peça tem nome, classificação, contagem de lados/vértices e fórmula de área acima dela. Figuras e rótulos permanecem estáveis, sem rotação.

- **Exibir vértices:** mostra marcadores nos polígonos. Inicialmente desligado.
- **Destacar contornos:** destaca os lados e a circunferência e mostra a relação de perímetro abaixo da peça. Inicialmente ligado.
- **Exibir medidas auxiliares:** mostra bases, alturas perpendiculares, diagonais, apótemas e raio conforme a fórmula. Inicialmente desligado. No L, o retângulo externo aparece em amarelo e o recorte em coral, com linhas tracejadas.
- **Voltar ao Hub:** navegação direta, sem confirmação de perda de edição.

Todos os toggles são independentes e podem ser acionados pelo raio XR na linha inteira, incluindo o rótulo. Não há seleção obrigatória de peças, campos, cópias de estudo ou ferramentas de edição.

## Coleção e modelo

Triângulo equilátero, quadrado e polígonos regulares de cinco a oito lados; triângulo escaleno, retângulo, losango, paralelogramo e trapézio irregulares convexos; polígono côncavo em L e círculo. São treze figuras ao todo.

As fórmulas são simbólicas. A escala visual não é uma unidade de exercício numérico. Regulares têm todos os lados e ângulos internos iguais; irregularidade não implica concavidade. O círculo não é um polígono: não recebe marcadores dos vértices de renderização nem contagem de lados. Sua circunferência usa `C = 2πr`.

O círculo é representado por 96 segmentos somente para renderização; área e circunferência são calculadas analiticamente. A triangulação dos polígonos reutiliza `ShapeGeometry`, preservando o recorte do L. A malha de preenchimento possui faces nos dois sentidos; diagonais internas não aparecem como lados. As medidas auxiliares usam uma camada separada dos contornos.

## Organização e reutilização

- `Model/PlanarCatalog.cs`: dados das treze figuras, fórmulas, classificação e segmentos auxiliares. Assembly `Tecaverso.PlanarGeometry.Model`, dependente de `Tecaverso.Shapes.Model`.
- `Scripts/PlanarGallery.cs`: preferências de apresentação e saída. Inscrições de eventos simétricas, sem atualização por frame.
- `Scripts/PlanarExhibit.cs`: controla grupos de geometria e visibilidade explícita dos textos com `UIVisibility`.
- `Meshes/`: superfícies, contornos, vértices e linhas auxiliares serializados. `Materials/`: cores das medidas; azul, ciano e laranja reutilizados da galeria de sólidos.
- `Tests/`: fórmulas, triangulação, classificações e medidas auxiliares.
- `Assets/_Project/Editor/PlanarGeometryAuthoring.cs`: instalador inicial, protegido contra sobrescrever a cena. Reutiliza a sala/rig e clona os componentes de controles e legendas da galeria de sólidos. Ajustes posteriores devem editar os objetos existentes, sem executar novamente o instalador.

As peças têm aproximadamente 1,2–1,8 m de extensão e centro a 1,55 m do piso. A sala, o rig e a iluminação são derivados da cena existente sem modificar a cena de origem. As figuras usam dados próprios para posições de medidas e fórmulas; não há escalas artificiais de coordenadas numéricas.

## Validação — 03/11/2026

- 15 testes EditMode aprovados: área/formulação e classificação de cada figura, triangulação, alturas/apótemas perpendiculares e recorte do L sem preenchimento indevido.
- Play Mode: 13 peças, três toggles e um botão; alternância independente de geometria e textos; círculo sem vértices; uma única instância do rig.
- Raio XR sintético atingiu a área de texto de cada toggle e acionou a mudança de estado. O mesmo caminho de raycast/evento acionou o botão de retorno.
- Retorno ao Hub e abertura de **Matemática → Geometria plana** pelo controlador do menu confirmados; estados iniciais restaurados ao reentrar.
- Capturas do ambiente, painel e figuras com medidas revisadas; TextMeshPro sem overflow. Compilação e console sem erros ao final da verificação de navegação.

Esses testes não substituem homologação em headset de legibilidade, alcance, circulação e desempenho. A sala é individual, sem sincronização LAN ou persistência de progresso. Não foi gerado build.

Capturas ficam em `Temp/PlanarGeometryQA/`, ignorada pelo Git. Cena, malhas, materiais, scripts e seus `.meta` são recursos necessários ao aplicativo e devem ser versionados.
