# Formas geométricas — short GDD

Status: revisão aprovada e implementada. Substitui o laboratório de edição e estudo A/B.

## Experiência

Uma galeria de sólidos grandes, azuis, suspensos em duas fileiras, inspirada nas imagens da versão anterior do Tecaverso. O aluno entra e já encontra toda a coleção. Caminha entre as peças, observa suas faces, lê o nome e a fórmula de volume acima delas e liga os destaques que deseja ver.

Não há seleção de peças num catálogo, cópias de estudo ou etapas obrigatórias. A comparação acontece pela observação dos objetos expostos. Esta versão contém somente sólidos 3D.

## Coleção e fórmulas

| Sólido | Faces planas | Vértices | Arestas | Volume |
|---|---:|---:|---:|---|
| Tetraedro regular | 4 | 4 | 6 | V = √2 · a³ / 12 |
| Pirâmide de base quadrada | 5 | 5 | 8 | V = Ab · h / 3 |
| Prisma triangular | 5 | 6 | 9 | V = Ab · h |
| Cubo | 6 | 8 | 12 | V = a³ |
| Paralelepípedo retângulo | 6 | 8 | 12 | V = a · b · c |
| Prisma pentagonal | 7 | 10 | 15 | V = Ab · h |
| Octaedro regular | 8 | 6 | 12 | V = √2 · a³ / 3 |
| Prisma hexagonal | 8 | 12 | 18 | V = Ab · h |
| Dodecaedro regular | 12 | 20 | 30 | V = (15 + 7√5) · a³ / 4 |
| Icosaedro regular | 20 | 12 | 30 | V = 5(3 + √5) · a³ / 12 |
| Esfera | — | 0 | 0 | V = 4πr³ / 3 |

Cada rótulo explica suas variáveis: a é a aresta dos sólidos regulares; a, b e c são as dimensões do paralelepípedo; Ab é a área da base; h é a altura perpendicular à base; r é o raio. A esfera possui superfície curva, sem faces planas, vértices ou arestas. Não mostrar a tesselação gráfica como característica matemática.

As fórmulas são simbólicas. Não há valores numéricos de volume, medição ou edição de dimensões nesta versão.

## Cena e apresentação

- Sete peças na fileira do fundo e quatro à frente, com corredor entre elas. Sala, iluminação, rig XR e pós-processamento existentes são preservados.
- Sólidos com cerca de 1 a 1,6 m de extensão, com centros entre 1,4 e 1,75 m do piso. As escalas são de apresentação, não unidades de uma atividade de cálculo numérico.
- Malhas com faces voltadas para fora e materiais azuis iluminados. Arestas em ciano; vértices em laranja, acompanhados de contagens textuais.
- Nome, contagens, fórmula e legenda acima de cada peça, em blocos opacos e legíveis, usando Montserrat e a paleta do projeto.
- Rótulos permanecem no mesmo lugar, fora do objeto que gira. Orientam-se horizontalmente conforme a posição do observador para permitir leitura ao caminhar ao redor; não acompanham a rotação da cabeça.
- Todas as peças estão serializadas na cena, visíveis também fora de Play Mode. Nenhuma montagem da sala em runtime.

## Interações XR e UI

Uma faixa compacta perto da entrada contém somente:

1. **Exibir vértices** — toggle independente, inicialmente desligado. Mostra os vértices matemáticos dos poliedros.
2. **Exibir arestas** — toggle independente, inicialmente ligado. Destaca cada aresta geométrica uma única vez; não inclui diagonais da triangulação das faces.
3. **Girar sólidos** — toggle independente, inicialmente ligado. Rotação de 10°/s em torno do eixo vertical. Apontar para o sólido com um interator XR pausa aquela peça enquanto houver hover; sair retoma a rotação se a opção estiver ligada.
4. **Voltar ao Hub** — botão de navegação, sem confirmação de perda de trabalho, pois não existe edição nesta sala.

Os controles usam uGUI e raycaster XR, com toda a linha do toggle clicável e marca visível de estado. A navegação reutiliza o rig e a locomoção existentes. As peças não são agarráveis nem deslocadas: os alunos circulam por elas. Toggles alteram apenas a visualização local; não implementam uma sessão coletiva LAN.

## Percurso do aluno

Entrar e reconhecer os sólidos; caminhar até um deles; observar seu volume e ler a fórmula; ligar vértices para contá-los; alternar arestas; apontar para parar a rotação ou desligá-la para toda a galeria; comparar visualmente sólidos próximos; voltar ao Hub.

## Fora do escopo

Geometria plana, edição, estudo A/B, campos numéricos, cópias, seleção de faces, desafios, desfazer/refazer, transparência, planificação e controles de tamanho ou número de lados. Não adicionar esses recursos como consequência de reutilizar um painel de outro laboratório.

## Aceite

- Onze sólidos disponíveis sem menus de seleção.
- Fórmulas corretas para as formas representadas e legendas completas.
- Três toggles independentes, sem afetar a geometria ou a navegação.
- Esfera sem falsos vértices/arestas; poliedros sem diagonais internas nos destaques.
- Rótulos não orbitam com as peças; passagens livres.
- Retorno ao Hub e reentrada funcionais no Editor.
- Leitura, alcance dos controles e desempenho devem ser homologados no headset; testes de Editor não substituem essa etapa.
