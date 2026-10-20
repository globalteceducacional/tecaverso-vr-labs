# Gravitação — estação orbital

Laboratório de Física **independente**, em `Scenes/Gravitation.unity`.
Entrada: **Explorar sozinho → Física → próxima página → Gravitação → Iniciar simulação**.
O card existente foi habilitado, preservando seu ícone. Lançamento oblíquo e plano inclinado continuam em suas cenas próprias.
Disponível no modo individual; o início coletivo LAN continua restrito ao lançamento oblíquo.

## Modos

- **Atração:** posições fixas para investigar massa e distância. Não integra movimento nem exibe velocidade.
- **Satélite:** escolha Terra, Lua ou Marte, ajuste altitude, velocidade inicial relativa à circular, massa do satélite, massa relativa do astro e tempo simulado por segundo. Lançar, pausar/retomar, reiniciar, força/velocidade/trajetória opcionais.
- **Sistema Solar:** instalação horizontal suspensa com Sol e oito planetas, órbitas elípticas, movimento kepleriano e um relógio comum. Botões alternam **Completo** e **Ampliar rochosos**, preservando tempo e pausa. Somente a velocidade do tempo usa slider; distâncias e contagem de planetas são informações, não controles. Pausa, reinício e opção Exibir órbitas.

### Inspeção dos planetas

- Apontar com o interator XR exibe um cartão acima do planeta: massa, distância instantânea ao Sol (UA e milhões de km) e período orbital em dias. Seu tamanho se adapta à distância, dentro de limites.
- Selecionar o planeta, ou usar um dos oito botões no painel, abre uma cópia ampliada próxima do estudante. Todos os oito continuam acessíveis pela lista na vista dos rochosos.
- A inspeção tem botões **Girar esquerda**, **Girar direita** e **Fechar**. Saturno mantém seus anéis. O painel é colocado uma vez, sem mover a câmera nem ficar preso à cabeça.
- A vista próxima isola visualmente o planeta. O relógio mantém seu estado; fechar restaura as órbitas e as opções de apresentação atuais. Trocar de modo ou abrir a confirmação de saída fecha a inspeção.
- `PlanetPointer` usa `XRSimpleInteractable`, sem agarrar/deslocar os astros. Os colisores são triggers e os casters XR desta cena consultam triggers, permitindo seleção sem bloquear a circulação.

Trocar modo reinicia a tentativa. Parâmetros físicos e escolha do astro ficam bloqueados durante uma tentativa orbital; o relógio e as opções de apresentação continuam disponíveis. Alterar a massa do astro identifica a configuração como **personalizada**. A aba Análise contém fórmulas, valores, escalas e limitações. O retorno ao Hub usa confirmação e bloqueia a interação atrás do modal.

## Física e escalas

`Model/OrbitalModel.cs` é um assembly sem referências a UnityEngine, testável isoladamente.
Usa problema restrito de dois corpos: astro esférico fixo, satélite de massa desprezível para a órbita, sem propulsão, atmosfera ou perturbações de outros corpos. A massa do satélite afeta `F`, mas não sua aceleração.

- Raios em km; parâmetros gravitacionais `GM` em km³/s²; força convertida para newtons.
- Integração velocity Verlet em double, com comprimento normalizado pelo raio do astro e tempo por `sqrt(R³/GM)`. Subpasso máximo de 0,002 unidades normalizadas.
- Colisão: interseção com a superfície, sem penetração; valores congelados imediatamente antes do impacto, sem impulso/rebote.
- Limite de observação: cinco raios do astro. Energia não negativa identifica escape; uma órbita ligada que excede a área recebe outra mensagem, sem ser chamada de escape.
- Raio visual do astro de 0,65 m. A escala km/m aparece no painel. Satélite ampliado para leitura.
- Força e velocidade têm escalas independentes explicitadas. Dois `VectorArrowView` reutilizados com cones persistentes, sem réplicas temporais. O diagrama é projetado à frente do globo para manter as pontas visíveis.
- Trajetória do satélite limitada a 2.048 pontos, preservando as amostras mais recentes sem reduzir sua resolução. Não cria GameObjects por amostra.

O Sistema Solar é um **modelo coplanar horizontal (XZ)**, não uma efeméride. As distâncias são proporcionais, com um único fator linear em cada vista:

- **Completo:** 1 m = 5 UA. Netuno fica aproximadamente a 6,04 m do Sol; os rochosos aparecem agrupados perto dele.
- **Rochosos:** 1 m = 0,25 UA, ampliação de 20×, mostrando Mercúrio, Vênus, Terra e Marte. Os demais planetas ficam ocultos, sem alterar o relógio compartilhado.
- Régua visível e escala em UA no painel. A análise informa semieixo maior e período dos oito planetas. UA é apresentada como aproximadamente 150 milhões de km.
- Plano orbital a 1,65 m do piso, com cerca de 12,2 m de diâmetro na vista completa. A mesa foi desativada para liberar o chão. Somente a instância da sala foi ampliada para aproximadamente 18,2 × 23,8 m; rig e Canvas principal mantêm seus transforms autorais.

`Model/SolarScale.cs` centraliza as conversões. Todos os diâmetros, incluindo o Sol, usam o mesmo fator: o Sol tem diâmetro aproximadamente 9,73 vezes maior que Júpiter. Os corpos são aproximações esféricas com os diâmetros de referência da NASA.

- **Duas escalas (padrão):** diâmetros em 1 m = 14 milhões de km na vista completa e 700 mil km na vista dos rochosos. Todos os corpos são ampliados juntos em aproximadamente 53,4× em relação às distâncias; nenhum planeta recebe um aumento individual. Painel e análise explicitam os dois fatores.
- **Escala única real:** checkbox aplica aos diâmetros a mesma conversão das distâncias (149.597.870,7 km por UA). Corpos ficam muito pequenos na sala, como exige a proporção física.
- Losangos vazados de seleção e alvos de interação auxiliam a localização dos corpos pequenos. Não representam o diâmetro físico. Os oito botões de inspeção permanecem disponíveis.
- A vista próxima informa **zoom individual** e não deve ser usada para comparar tamanhos. O cartão inclui também o diâmetro físico.
- A ampliação de rochosos aplica 20× tanto às posições quanto aos diâmetros. Mudar a escala não altera o tempo nem a pausa.

Fases iniciais são ilustrativas, sem representar posições atuais no céu. Cada elipse preserva sua excentricidade e sua evolução temporal segue a equação de Kepler. Órbitas com 240 pontos são reconstruídas somente ao mudar de escala; durante o movimento atualizam-se as posições dos planetas/rótulos.

## Arte e reutilização

Sala padrão, rig XR, controles uGUI/TMP, Montserrat, feedbacks, confirmação de saída, materiais estruturais e perfil URP compartilhados. A nova sala recebe plataforma de projeção e janela estelar; nenhuma sala anterior é reconstruída.
`Prefabs/ObservationSatellite.prefab` contém corpo, antena e painéis solares.
Os mapas são assets locais, sem downloads em runtime. Texturas importadas em até 2K, com mipmaps e compressão.

`Editor/GravitationAuthoring.cs` é uma instalação inicial e recusa sobrescrever a cena existente. Ajustes posteriores devem editar a cena/componentes.
`Editor/GravitationPresentation.cs` aplica a migração pontual da mesa horizontal e do painel solar sem reconstruir cena, rig ou Canvas.
`Editor/PlanetInspectionAuthoring.cs` acrescenta os alvos, cartões, inspeção e expansão da sala. Não reconstrói as cenas existentes.

## Fontes

- Texturas: [Solar System Scope / INOVE](https://www.solarsystemscope.com/textures/), **CC BY 4.0**. Inventário, links e alterações em [Textures/ATTRIBUTION.md](Textures/ATTRIBUTION.md).
- Massas, períodos, excentricidades e distâncias planetárias aproximadas: [NASA Planetary Fact Sheet](https://nssdc.gsfc.nasa.gov/planetary/factsheet/). Massas arredondadas da tabela em 10²⁴ kg; a distância instantânea é calculada pelo modelo orbital, não é uma efeméride.
- Diâmetros planetários: mesma tabela da NASA. Diâmetro solar de 1.391.400 km, calculado a partir do raio médio de 695.700 km: [NASA Sun Fact Sheet](https://nssdc.gsfc.nasa.gov/planetary/factsheet/sunfact.html).
- Parâmetros gravitacionais arredondados: [JPL Astrodynamic Parameters — DE440](https://ssd.jpl.nasa.gov/astro_par.html). Para Marte usa-se a aproximação do sistema marciano.
- Raios médios: Terra 6.371 km, Lua 1.737,4 km, Marte 3.389,5 km; [JPL Planetary Physical Parameters](https://ssd.jpl.nasa.gov/planets/phys_par.html) e [Moon Fact Sheet](https://nssdc.gsfc.nasa.gov/planetary/factsheet/moonfact.html).

## Validação

Em 20/10/2026, oito testes EditMode passaram: inverso do quadrado e massas, fechamento/energia de órbita circular, independência da massa do satélite e da partição de frames, colisão, escape, órbita ligada fora de vista, pausa/reset e fechamento/velocidade das órbitas planetárias.
Validação em Play Mode inclui botões de lançamento/pausa/reset, presets, modos, toggles, análise, confirmação/cancelamento de saída, um único rig/câmera e dois vetores fixos. Textos do painel conferidos quanto a overflow. Ciclo Gravitação → Hub → Gravitação verificado em Play Mode. O limite de 2.048 amostras foi exercitado com 2.200 posições, confirmando continuidade e resolução da trajetória.
Sem build e sem certificação de desempenho/conforto em headset.

Revisão horizontal: compilação sem erros e verificações em Play Mode de coplanaridade, fator linear comum, ampliação 16×, preservação de tempo/pausa, alternância de 8/4 planetas, órbitas, reset, modal, ausência de overflow e restauração do modo Satélite. Capturas conferidas na altura de observação e de frente para o painel.

Revisão de exploração suspensa: oito seleções/texturas, dados sem overflow, anéis de Saturno, giro/fechamento, callbacks de hover/seleção XR, alvos sem bloqueio físico, ocultação/restauração das órbitas, inspeção dos gigantes na vista dos rochosos, relógio e bloqueio por modal verificados em Play Mode. Compilação e capturas verificadas; interação física e conforto em headset permanecem pendentes.

Revisão de proporções: oito testes adicionais em EditMode verificam razões Sol/planetas nas quatro combinações de vista/escala, conversão única real, zoom comum de 20× e Sol sem interseção com o periélio de Mercúrio no modo ampliado.
