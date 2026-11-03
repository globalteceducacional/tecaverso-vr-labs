# Geometria plana — short GDD

Status: aprovado e implementado em 03/11/2026. Nova sala individual de Matemática, disponível no Hub. Uso, arquitetura e limites de validação no [README do laboratório](../Assets/_Project/Laboratories/Mathematics/PlanarGeometry/README.md).

## Conceito e objetivo

Galeria de figuras planas grandes, prontas para observar. O aluno caminha pela sala, compara contornos, distingue polígonos regulares e irregulares e relaciona cada figura à sua fórmula de área. Vértices, perímetro e medidas auxiliares tornam a estrutura e as fórmulas visíveis no próprio objeto.

Complementa [Formas geométricas](../Assets/_Project/Laboratories/Mathematics/GeometricShapes/GDD.md), dedicada aos sólidos. Construção e edição continuam no [Espaço cartesiano](../Assets/_Project/Laboratories/Mathematics/CartesianSpace/README.md).

## Coleção inicial: 13 figuras

| Grupo | Figuras | Área apresentada |
|---|---|---|
| Regulares | Triângulo equilátero | A = b · h / 2 |
| Regulares | Quadrado | A = l² |
| Regulares | Pentágono, hexágono, heptágono e octógono regulares | A = P · ap / 2 |
| Irregulares convexos | Triângulo escaleno | A = b · h / 2 |
| Irregulares convexos | Retângulo não quadrado | A = b · h |
| Irregulares convexos | Losango não quadrado | A = D · d / 2 |
| Irregulares convexos | Paralelogramo oblíquo | A = b · h |
| Irregulares convexos | Trapézio com apenas um par de lados paralelos | A = (B + b) · h / 2 |
| Irregular côncavo | Polígono em L, com seis vértices | A = B · H − b · h |
| Figura curva | Círculo | A = πr² |

**Significado das letras:** A: área; P: perímetro; ap: apótema; l: lado; b/B: bases ou larguras indicadas; h/H: alturas perpendiculares; D/d: diagonais do losango; r: raio. No polígono em L, B e H delimitam o retângulo externo, e b e h delimitam o recorte.

Um polígono regular possui todos os lados e ângulos internos iguais. Regularidade e convexidade são propriedades distintas: um polígono irregular pode ser convexo. O círculo não é polígono e não apresenta vértices ou lados retos. Seu contorno é a circunferência.

## Disposição e visual

- Coleção organizada em U: regulares de um lado, irregulares do outro e figuras de comparação ao fundo. A entrada e a circulação central ficam livres. Todas as figuras estão disponíveis desde a entrada.
- Reutilizar a sala padrão, iluminação, materiais azuis, Montserrat, rig XR e perfil de renderização do projeto. Cada figura tem aproximadamente 1,2–1,8 m de extensão e centro próximo de 1,5 m de altura; a distribuição final deve ser conferida na cena e no headset.
- Figuras verticais, orientadas para a área de observação, com preenchimento azul e contorno legível. São representações de superfícies planas; eventual espessura gráfica não representa um prisma nem participa do cálculo.
- Nome e fórmula de área em TMPro acima da figura. Uma linha curta identifica a classificação e o número de lados/vértices, quando aplicável. Letras auxiliares ficam junto às medidas correspondentes.
- Posição e orientação estáveis. A rotação automática não faz parte desta proposta, para manter bases, alturas e fórmulas fáceis de acompanhar. A rotação da cabeça não move os objetos.
- Superfícies visíveis pelos dois lados; rótulos não devem atravessar a figura nem encobrir a peça vizinha. Triângulos internos da malha ficam ocultos.

## Controles e interações XR

Uma faixa compacta junto à entrada apresenta três toggles independentes e um botão:

1. **Exibir vértices**, inicialmente desligado: marcadores nos vértices matemáticos dos polígonos. Não produz pontos na tesselação do círculo.
2. **Destacar contornos**, inicialmente ligado: destaca os lados dos polígonos e a circunferência. Mostra junto à peça a relação de perímetro: P = soma dos lados; P = n · l nos regulares; C = 2πr no círculo. Desligar remove o destaque e essa informação, mantendo a figura e sua fórmula de área.
3. **Exibir medidas auxiliares**, inicialmente desligado: mostra os segmentos e letras necessários à fórmula de cada peça. Inclui bases, alturas perpendiculares com marca de ângulo reto, diagonais do losango, apótema dos regulares e raio do círculo. No L, evidencia o retângulo externo e o recorte por cores e linhas tracejadas, sem alterar o contorno original.
4. **Voltar ao Hub**: encerra a exploração individual. Sem confirmação de perda de trabalho, pois a galeria não possui edição.

Os toggles usam uGUI com raycaster XR e toda a linha clicável. Nome, figura e área não dependem de hover ou seleção. O aluno usa a locomoção existente para aproximar-se e comparar as peças; não precisa agarrá-las ou levá-las para uma bancada. As preferências de visualização são locais.

## Percurso do aluno

Entrar e reconhecer as figuras; comparar quadrado e retângulo; ativar vértices e contornos para contar e observar; ligar medidas auxiliares para relacionar a altura perpendicular à fórmula do triângulo ou do paralelogramo; comparar um polígono regular com um irregular; observar a reentrância do L e entender sua área por subtração; finalizar no círculo e relacionar raio, área e circunferência.

As relações são simbólicas nesta primeira versão. As dimensões de apresentação na Unity não são valores de um exercício numérico. Não há respostas digitadas, pontuação ou sequência obrigatória de atividades.

## Implementação e escopo

Cena própria em `Assets/_Project/Laboratories/Mathematics/PlanarGeometry/Scenes/PlanarGeometry.unity`, com entrada **Matemática → Geometria plana**. É o quinto conteúdo do catálogo de Matemática, dentro do limite de seis; a galeria de sólidos permanece separada.

`PlanarCatalog` reutiliza `ShapeGeometry` para polígonos, áreas e triangulação. O rig, ambiente, renderização, materiais azuis e componentes uGUI/XR vêm da galeria existente. As malhas são persistidas na autoria, incluindo o contorno e recorte corretos do L; não são montadas em runtime. Não foi necessário ProBuilder para essas superfícies matemáticas.

Ficam fora desta versão: edição de vértices, criação de figuras pelo aluno, estações A/B, campos numéricos, sliders, abas, planificação, rotação, sincronização LAN e persistência de progresso. O controle de medidas auxiliares não vira um modo de edição.

## Critérios de aceite

- Treze figuras expostas, legíveis e acessíveis pela circulação da sala.
- Classificações, contagens e fórmulas correspondem às figuras efetivamente modeladas.
- Alturas são perpendiculares às bases; apótemas chegam perpendicularmente a um lado; o raio vai do centro à circunferência.
- Destaques não expõem diagonais de renderização como lados. O círculo não recebe vértices ou contagem fictícia de lados.
- No polígono em L, área da malha e decomposição por subtração coincidem; o recorte não é preenchido por triangulação incorreta.
- Toggles independentes, retorno ao Hub e uma única instância do rig por sala.
- Testes focados de geometria e verificações de UI/Play Mode no Editor. Registrar separadamente a validação física de leitura, alcance, circulação e desempenho no headset.
- Capturas de conferência ficam nas pastas ignoradas pelo Git. Nenhum build sem solicitação explícita.
