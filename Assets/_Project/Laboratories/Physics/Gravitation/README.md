# Gravitação — observatório do Sistema Solar

Cena independente: `Scenes/Gravitation.unity`.
Entrada: **Explorar sozinho → Física → Gravitação → Iniciar simulação**.
Modo individual; a autoridade LAN do lançamento oblíquo não foi alterada.

## Experiência atual — 23/10/2026

- Oito planetas sempre visíveis, com texturas locais e nome TMP acima de cada astro.
- Apontar destaca o nome; selecionar diretamente o planeta abre suas informações ao lado dele. Selecionar novamente fecha; selecionar outro troca o único cartão aberto.
- Cartão com diâmetro, massa, distância média ao Sol (UA e milhões de km) e período orbital. Dados reais, independentes das dimensões ilustrativas.
- Selecionar pausa as órbitas; fechar não retoma automaticamente. A ação de iniciar órbitas fecha o cartão. O globo selecionado também para de girar para facilitar a observação.
- Não há lista de planetas, cópia ampliada, controles de escala, alternância completo/rochosos ou slider de parâmetros.
- Painel compacto: velocidades **1, 10 e 30 dias por segundo**, **Iniciar/Pausar órbitas** e **Voltar ao Hub**, com confirmação.
- O observatório começa com órbitas pausadas. Os globos giram lentamente (8°/s), de maneira ilustrativa e independente do calendário orbital.
- Abertura da confirmação de saída fecha o cartão e bloqueia seleção dos planetas. Cancelar retorna ao observatório.

## Representação didática

`Model/ObservatoryScale.cs` define as dimensões da exposição. O Sol tem 3 m de diâmetro. Os planetas variam de 0,4 a 1,4 m, por compressão logarítmica dos diâmetros de referência. A ordem de tamanho é preservada; as razões visuais NÃO são as razões físicas.

Órbitas circulares projetadas a 15 cm do piso, abaixo dos globos, têm raios de 2,7; 4; 5,3; 6,5; 8; 10,1; 12,2 e 14 m. Sua distribuição preserva a ordem planetária e oferece circulação, mas NÃO representa distâncias proporcionais. O ângulo orbital é obtido do modelo kepleriano existente, preservando os períodos relativos; a geometria apresentada é ilustrativa e coplanar. Fases iniciais não representam o céu atual. A rotação axial não representa dias siderais, inclinações ou sentido real de cada planeta.

Sala virtual de 32 × 40 m. Centro do sistema em (0; 1,8; 4) m; plano horizontal, sem mesa nem colisores sólidos nos planetas. Rig e Canvas principal mantêm seus transforms autorais; o layout interno foi compactado. A dimensão virtual não exige uma área física equivalente: usa-se a locomoção XR instalada.

`SolarScale.cs` e o modelo de satélite são preservados como referências de cálculo e testes anteriores. Seus modos antigos não estão expostos na UI deste observatório.

## Interação e autoria

- `PlanetPointer` usa hover/select de `XRSimpleInteractable`, com collider trigger. Casters XR consultam triggers.
- `PlanetInspection` agora controla apenas o cartão informativo junto ao astro; não oculta o sistema nem cria modelos ampliados.
- `SolarSystemView` mantém oito globos, oito trajetórias e nomes independentes da rotação. Não cria objetos por frame.
- `ObservatoryAuthoring.Configure` é uma migração pontual da apresentação. Os objetos antigos de lista, preview e régua ficam aposentados/inativos para preservar referências de autoria; não participam da experiência.
- O instalador inicial continua recusando sobrescrever cenas existentes. Nunca regenerar a cena para ajustes visuais.
- UI baseada em uGUI/TMP, Montserrat, componentes e cores compartilhados. Escolha dos controles segue `DESIGN_SYSTEM.md` §7.0 e `UI_GUIDELINES.md`.

## Fontes

- Céu estrelado: [Real Stars Skybox Lite](https://assetstore.unity.com/packages/3d/environments/sci-fi/real-stars-skybox-lite-116333), importado pelo usuário. Material `CubemapStars` preservado; este asset é separado das texturas planetárias abaixo.

- Texturas: [Solar System Scope / INOVE](https://www.solarsystemscope.com/textures/), **CC BY 4.0**. Inventário e atribuição em [Textures/ATTRIBUTION.md](Textures/ATTRIBUTION.md). Assets locais até 2K, com mipmaps e compressão.
- Diâmetros, massas, semieixos e períodos: [NASA Planetary Fact Sheet](https://nssdc.gsfc.nasa.gov/planetary/factsheet/).
- Sol: [NASA Sun Fact Sheet](https://nssdc.gsfc.nasa.gov/planetary/factsheet/sunfact.html).
- Modelo orbital anterior: [JPL Astrodynamic Parameters](https://ssd.jpl.nasa.gov/astro_par.html).

## Validação

### Ambiente do observatório

Acabamento local da cena por `ObservatoryEnvironmentAuthoring.Configure`: plataforma de grafite, guarda-corpo com sinalização luminosa e três arcos abertos permitem ver o skybox ao redor do sistema. Os renderizadores das paredes e teto originais ficam ocultos apenas nesta instância; piso e colisores de limite são preservados. Materiais novos ficam na pasta `Materials` deste laboratório, sem alterar o prefab compartilhado.

Luz principal quente, preenchimento suave e luz pontual junto ao Sol destacam os globos. Ambiente em três cores mantém as faces menos iluminadas visíveis; luzes sem sombras e guias discretas evitam competir com os nomes e cartões. Rig, Canvas e skybox autorais permanecem preservados.

Verificação visual em Play Mode: entrada, circulação e cartão junto à Terra; piso com colisão, quatro limites laterais ativos, um único rig e console sem erros. Conforto e desempenho precisam ser conferidos em headset.

O histórico anterior de testes de física e escalas não certifica a nova apresentação do observatório. A validação atual deve cobrir os oito planetas, colisores de seleção, nomes, cartões, pausa/retomada, presets de tempo, modal e retorno ao Hub. Conferir tamanho/legibilidade e conforto em headset separadamente. Não gerar builds sem autorização.


Validação em 23/10/2026: dois testes EditMode do observatório passaram (tamanhos/ordem e limites/separação). Em Play Mode, oito callbacks XR de seleção, cartões sem overflow, pausa automática, fechamento/retomada, presets, bloqueio pelo modal e retorno ao Hub com um rig foram verificados. Capturas do painel, cartão e confirmação conferidas. Console sem erros na execução final. Testes não substituem interação física, conforto e desempenho em headset; nenhum build foi gerado.
