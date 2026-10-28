# Espaço cartesiano

Laboratório individual de Matemática, acessível pelo Hub. Cena: `Scenes/CartesianSpace.unity`.

## Interações

- Criar ponto: nasce na origem e pode ser agarrado diretamente ou pelo raio XR. O rig compartilhado mantém a manipulação de distância pelo analógico.
- Selecionar um ponto apresenta coordenadas, distância à origem e construções vinculadas. Campos numéricos usam o teclado espacial; botões de incremento, passo e travas permitem ajustes precisos.
- Escolher uma ferramenta e selecionar os pontos indicados cria segmento, reta, semirreta, triângulo, plano, caixa alinhada aos eixos, esfera ou tetraedro. A prévia aparece antes do último ponto.
- Mover um vértice atualiza todas as construções dependentes. Selecionar o marcador central e ativar **Mover conjunto** permite translação preservando dimensões e respeitando as travas dos vértices.
- Visualização controla componentes, vetor posição, projeções, grades e o toggle **Magnético** (ligado inicialmente). Durante o arraste de um ponto, cada coordenada a até 0,12 u de um inteiro encaixa nele; fora dessa região, valores fracionários permanecem livres. Desligar o toggle permite posicionamento totalmente livre. Campos numéricos são sempre exatos, e o passo dos botões −/+ é independente. Mover conjunto não aplica magnetismo para preservar a forma. XY, XZ e YZ restringem somente os novos pontos.
- Análise apresenta comprimentos, áreas, volumes, equações e normais conforme a figura. Geometrias degeneradas recebem indicação visual e explicação.
- Desfazer/refazer agrupa cada gesto de movimentação. Excluir um ponto informa as construções dependentes; limpar e voltar ao Hub exigem confirmação.
- Atividades incluem posicionamento, comprimento, área e volume, além de exploração livre.

As grades XZ (horizontal), XY e YZ (verticais) iniciam visíveis. Na aba Visualizar, cada controle Grade alterna independentemente entre VISÍVEL e OCULTA; ocultar uma grade preserva eixos, pontos e construções.

## Escala e limites

Coordenadas entre −5 e 5; uma unidade matemática equivale a 0,5 m no ambiente. Y é vertical. Alterar a altura de visualização não muda coordenadas. Limite de 24 pontos e 16 construções por sessão. Superfícies são translúcidas e não bloqueiam a circulação; a interação ocorre pelos pontos e marcadores centrais. Retas e planos são recortados pelo espaço de visualização.

## Organização

`Model/CartesianModel.cs` concentra geometria, validação, restrições e histórico sem dependência da UI. `Scripts/CartesianLab.cs` coordena interação e apresentação; `CartesianGrab.cs` integra XR; `CartesianGeometryView.cs` reutiliza linhas, malhas e setas. Prefabs e materiais ficam nas respectivas pastas. `CartesianAuthoring` é exclusivamente para primeira instalação e recusa recriar uma cena existente.

A sala reutiliza o rig XR, teclado espacial, ambiente, perfil de renderização e identidade visual do projeto. Não adiciona suporte LAN; a autoridade LAN permanece no laboratório suportado pelo projeto. Construções são temporárias e não persistem ao sair da sala.

## Validação

Onze testes EditMode em `CartesianModelTests` passaram: magnetismo opcional, frações, histórico, degenerescência, dependências, translação, travas, restrições 2D, limites, medidas e recorte de retas. As oito ferramentas, edição numérica, prévias, confirmação de exclusão e navegação Hub/sala foram exercitadas em Play Mode. Um interator XR sintético verificou seleção, deslocamento e desfazer.

Pendente validação física em headset: alcance dos controles, legibilidade à distância, manipulação pelo analógico e conforto de locomoção. Nenhum build foi produzido.
