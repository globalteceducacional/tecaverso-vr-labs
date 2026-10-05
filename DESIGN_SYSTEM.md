# Tecaverso Labs — Design System VR

**Versão 1.0 · 05/10/2026 · Português brasileiro**

Referência visual e de interação para interfaces, instrumentos e salas do Tecaverso Labs. Complementa o GDD em [README.md](README.md): o GDD define o produto; este documento define como ele deve se apresentar e responder visualmente.

> **Atual** indica padrões já presentes nos assets/cenas. **Diretriz** estabelece o padrão para novos componentes e revisões, sem afirmar que tudo já está implementado. Valores de conforto são pontos de partida para validação no headset, não certificações de acessibilidade.

## 1. Princípios

1. **Clareza científica:** fenômeno, valores, unidades e controles são mais importantes que decoração.
2. **Consistência:** mesma função usa o mesmo componente e vocabulário em todas as disciplinas.
3. **Presença sem distração:** laboratório tecnológico sóbrio, com iluminação azul como acento.
4. **Interação previsível:** seleção, disponibilidade, ação e resultado precisam ser reconhecíveis.
5. **Conforto espacial:** interfaces estáveis, legíveis e sem movimento involuntário da câmera.
6. **Economia visual e técnica:** reutilizar recursos; limitar transparências, textos simultâneos e efeitos.

## 2. Referência e fonte de verdade

- Design: [Tecaverso Game UX Kit no Figma](https://www.figma.com/design/ztbnAS5MZZmqOLbQAfm96E/Tecaverso-Game-UX-Kit), página Examples.
- HUB 1: `3053:200`; HUB 2: `3053:238`.
- Projectile Motion: `3051:256`; Projectile Analysis: `3051:338`.
- Fontes: `Assets/_Project/UI/Fonts/Montserrat/`.
- Ícones: `Assets/_Project/UI/Art/Figma/`, com os do Hub em `Hub/`.
- UI Hub: `Assets/_Project/Hub/HubUI.prefab`.
- Sala: `Assets/_Project/Environments/StandardRoom/TecaversoStandardRoom.prefab`.

O Figma define identidade e composição; Unity define interação e apresentação espacial. Divergências necessárias para VR devem ser documentadas e depois refletidas no design. Não usar screenshots das telas como UI.

Os nomes de tokens neste documento são **contratos de design**. Ainda não existe um serviço de tema ou asset central que implemente todos eles; parte dos valores está nos componentes e ferramentas de autoria.

## 3. Cores

### 3.1 Paleta de marca

Valores hexadecimais abaixo são referências sRGB para UI.

| Token | Cor | Uso |
|---|---|---|
| `brand.100` | `#284EA0` | Primária, ações principais, conteúdo selecionado |
| `brand.200` | `#004389` | Variação secundária da coleção Tecaverso |
| `brand.300` | `#02356C` | Variação profunda da marca |
| `brand.400` | `#062E61` | Ênfase escura |
| `brand.500` | `#021732` | Superfícies escuras de marca |
| `brand.600` | `#02041B` | Tom mais profundo; uso restrito |
| `neutral.white` | `#FFFFFF` | Conteúdo sobre preenchimento de marca |
| `neutral.black` | `#000000` | Referência neutra, não texto padrão |

### 3.2 Tokens semânticos das telas atuais

| Token | Cor | Aplicação |
|---|---|---|
| `ui.canvas` | `#E8E6FE` | Fundo dos Hubs e blocos internos de fórmulas |
| `ui.surface` | `#D3D1E8` | Cards e painéis de parâmetros/análise |
| `ui.border` | `#07386F` | Contornos das telas Examples |
| `ui.text.primary` | `#161616` | Títulos, descrições e conteúdo principal |
| `ui.text.accent` | `#284EA0` | Valores e títulos de seção destacados |
| `ui.text.onBrand` | `#FFFFFF` | Texto sobre botão/card azul |
| `ui.track` | `#BDBBD4` | Trilha inativa de slider |
| `ui.action.primary` | `#284EA0` | Disparar, iniciar e ações de avanço |
| `ui.action.secondary` | `#07386F` | Ação secundária preenchida |

`ui.border` reproduz os Examples; não é idêntico a `brand.300`. Não substituir cores parecidas silenciosamente. A coleção Figma possui modos Claro/Escuro, mas **as telas implementadas seguem os Examples claros**. Um tema escuro completo ainda exige especificação e validação; não basta inverter cores.

### 3.3 Disciplinas e estados

| Semântica | Cor | Regra |
|---|---|---|
| Física | `#284EA0` | Faixa/acento do card |
| Química | `#7648E8` | Faixa/acento do card |
| Biologia | `#3ED39C` | Faixa/acento do card |
| Matemática | `#E7811B` | Faixa/acento do card |
| Positivo, coleção | `#3FCF92` | Confirmação acompanhada de texto/símbolo |
| Negativo, coleção | `#E64848` | Erro acompanhado de explicação |
| Atenção, diretriz | `#9A7000` | Avisos sobre superfície clara; não texto sobre azul |

A cor de disciplina identifica uma categoria, não sucesso/erro. Não pintar a interface inteira com a cor da disciplina. Os tokens de positivo/negativo pertencem à coleção; não implicam que todos os seus estados já estejam aplicados.

### 3.4 Contraste e uso

- Preferir texto `ui.text.primary` em superfícies claras e branco em preenchimentos azuis.
- Não usar verde claro, amarelo ou azul elétrico como texto pequeno sobre lavanda.
- Estado não pode depender apenas de cor: incluir rótulo, borda, símbolo ou alteração de conteúdo.
- Validar contraste no material/render real, com iluminação e configuração do headset. Não declarar conformidade com uma norma apenas pela paleta.
- Painéis informativos são opacos. Reservar transparência para dados históricos e efeitos de estado, não para texto sobre cenários variáveis.

## 4. Tipografia

### 4.1 Família e pesos

**Atual:** Montserrat Regular (400), Bold (700) e ExtraBold (800), com fontes TTF e assets SDF do TextMeshPro. Usar o arquivo do peso correto, não simular negrito indiscriminadamente.

- Regular: descrições, instruções e apoio.
- Bold: nomes de componentes, ações, parâmetros e fórmulas.
- ExtraBold: títulos principais e métricas de destaque.
- Gotham Ultra consta no token de display da coleção, mas não é usada nas telas atuais nem está importada. Sua adoção depende de arquivo/licença apropriados.
- Estilos antigos de texto no Figma ainda citam Inter. Não introduzir Inter por esse motivo: os Examples implementados usam Montserrat.
- Preservar a licença `Assets/_Project/UI/Fonts/Montserrat/OFL.txt`.

### 4.2 Escala de referência

Os números são **unidades de layout do Canvas/Figma**, não milímetros nem tamanho físico garantido.

| Papel | Tamanho de referência | Peso | Aplicação |
|---|---|---|---|
| Display Hub | 100 | ExtraBold | Escolher disciplina; nome da disciplina |
| Métrica principal | 52 | ExtraBold | Alcance e altura máxima |
| Título de detalhe | 42 | Bold | Conteúdo selecionado no HUB 2 |
| Título de disciplina | 38 | Bold | Cards do HUB 1 |
| Corpo Hub | 24–26 | Regular | Descrições e instruções |
| Ação XR ampliada | 28 | Bold | Iniciar/voltar no Hub |
| Título de painel | 24 | ExtraBold | Parâmetros e Análise |
| Ação de experimento | 24 | Bold | Disparar, reiniciar e pausa |
| Fórmula | 25 | Bold | Equações de referência |
| Valor de parâmetro | 22 | Bold | Valor e unidade do slider |
| Legenda de vetor | 22 | Regular | Nome das componentes |
| Nome de parâmetro | 20 | Bold | Ângulo, velocidade etc. |
| Título de seção | 18 | Bold | Vetores, fórmulas |
| Apoio de fórmula | 17 | Regular | Significado da equação |

Diretriz de entrelinha: 1,25–1,35× para leitura, com espaço adicional quando há subscritos. Evitar auto-size que reduza silenciosamente o texto para caber; primeiro revisar frase, quebra ou espaço.

### 4.3 Regras editoriais

- Títulos de navegação em caixa normal; caixa alta somente em comandos curtos e rótulos de seção.
- Corpo alinhado à esquerda. Centralizar apenas títulos/cards curtos e botões.
- Descrições de cards com quebra de linha; não comprimir fonte para uma linha.
- Sem parágrafos longos em VR: dividir em objetivo, instrução e resultado.
- Valores pt-BR: `9,81 m/s²`, `2,0 m`, `45°`; manter unidade junto ao valor.
- Variações: Vx/Vy nos rótulos; subscritos nas fórmulas quando disponíveis.
- Garantir acentos, θ, °, ², sinal de menos, multiplicação e subscritos. Configurar fallback deliberado; não aceitar quadrados de glifos ausentes.
- Não misturar resultado previsto e atingido sob o mesmo rótulo “máximo”.

## 5. Espaçamento e composição

### 5.1 Escala para novos componentes

Base de **8 unidades**, com 4 como ajuste fino:

| Token | Valor | Uso típico |
|---|---|---|
| `space.1` | 4 | Ajuste interno fino |
| `space.2` | 8 | Separação compacta |
| `space.3` | 12 | Ícone e rótulo |
| `space.4` | 16 | Elementos diretamente relacionados |
| `space.6` | 24 | Conteúdos de um grupo |
| `space.8` | 32 | Grupos e padding compacto |
| `space.12` | 48 | Padding de painel |
| `space.16` | 64 | Separação de seções |
| `space.24` | 96 | Separação de grandes blocos |

Esta escala é diretriz nova. Os Examples possuem medidas próprias, como 58, 115 e 30 unidades. **Preservar layouts existentes**; não arredondar suas medidas automaticamente apenas para encaixar na escala.

### 5.2 Geometria visual

- Cantos retos como padrão atual; não introduzir cards arredondados em telas isoladas.
- Contorno padrão de 2 unidades; seleção forte usa 7–8 nos cards de referência.
- Não depender de linhas ultrafinas: ajustar espessura física caso o contorno desapareça no headset.
- Superfícies planas, sem sombras pesadas, glassmorphism ou gradientes decorativos novos.
- Agrupar por proximidade e hierarquia; evitar caixas dentro de caixas sem função.
- Layouts novos devem usar containers/layout groups quando adequado. Composições fixas do Figma podem conservar coordenadas de referência, desde que a escala física seja explícita.

### 5.3 Templates atuais

| Template | Referência |
|---|---|
| HUB 1 | Canvas 2560 × 1440; quatro cards 430 × 650; intervalo horizontal de 50 |
| HUB 2 | Canvas 2560 × 1440; grade 3 × 2 de cards 360 × 250; intervalos de 30; detalhe 710 × 410 |
| Parâmetros | Card 620 × 1010 |
| Análise | Card 720 × 1010 |
| Fórmula | Bloco 620 × 96 |

Não inserir controles extras espremendo os elementos existentes. Usar uma faixa de ações, aba, painel auxiliar ou modal coerente com a tarefa.

## 6. Escala física e posicionamento em VR

### 6.1 Unidades

Uma unidade de mundo é tratada como um metro no projeto. Tamanho físico resulta da dimensão do RectTransform e da escala mundial acumulada, inclusive dos pais.

Exemplos atuais de autoria:

- Hub: 2560 × 1440, escala 0,002 → aproximadamente 5,12 × 2,88 m, sem escala adicional dos pais.
- Experimento: Canvas em escala 0,0028 e conteúdo em 0,64 → card de Parâmetros aproximadamente 1,11 × 1,81 m, sem escala adicional.

Esses valores descrevem a montagem inicial, não limites universais. Preservar ajustes autorais posteriores e medir `lossyScale`/bounds antes de alterar.

### 6.2 Diretrizes de conforto

- Começar a validação com painéis principais aproximadamente a 2–3 m do observador, ajustando ao espaço e à tarefa. Não mover automaticamente painéis existentes para impor essa distância.
- Posicionar ações frequentes próximas ao campo de visão confortável, sem exigir olhar continuamente para cima/baixo.
- Oferecer ajuste de altura/distância e recentralização; não bloquear uso sentado.
- Um painel ativo principal por tarefa; não cobrir canhão, trajetória ou instrumento relevante.
- Usar orientação voltada ao ponto de observação. Evitar billboard que gira continuamente com a cabeça.
- Rótulos científicos permanecem ligados ao fenômeno; não seguem o jogador de forma que percam sua referência espacial.
- Medir tamanho angular, não só pixels. Como ponto inicial de teste, buscar altura visível de letras de corpo na ordem de 0,5°–0,8° e alvos interativos de pelo menos 1,5°. São metas ajustáveis, não garantias de leitura.
- Relação útil: ângulo visual = `2 × atan(tamanho físico / (2 × distância))`. A altura real do glifo é menor que a caixa do texto.
- Área clicável pode exceder o desenho do controle, mas não invadir a área de outro controle.

## 7. Componentes

### 7.1 Botões

- Primário: azul `ui.action.primary`, texto branco; uma ação principal por contexto.
- Secundário: azul profundo; retorno e ações auxiliares.
- Destrutivo: label explícito e confirmação quando há perda de trabalho/sessão. Não tornar “Resetar” visualmente mais dominante que “Disparar”.
- Ícone nunca substitui sozinho um comando importante.
- Desabilitado: aparência reduzida, sem interação; explicar a causa perto da ação quando não for evidente.
- Loading: bloquear repetição, manter indicação de progresso e recuperação.
- Atual: botões do experimento de 235 × 72, pausa de 235 × 52; ações ampliadas no Hub. São dimensões de referência, não tamanhos mínimos em VR.

### 7.2 Cards de disciplina e conteúdo

- Disciplina: faixa de cor, imagem, nome, descrição e ação.
- Conteúdo: imagem, título e seleção vinculada ao painel de detalhes.
- Card inteiro pode ser interativo, evitando pequenos botões aninhados com ações conflitantes.
- Selecionado não significa hover. Preservar a seleção após o ponteiro sair.
- Diretriz: mover o contorno/acento de seleção para o card realmente selecionado; não manter “Física” destacada como se fosse uma escolha ativa em todos os estados.
- “Em breve” no detalhe e no botão de início bloqueado; card continua acessível para consultar o mockup.
- Imagens com proporção preservada, sem esticar ou cortar conteúdo significativo.

### 7.3 Sliders e ajustes numéricos

- Ordem: nome e valor na mesma linha; trilha abaixo.
- Referência atual: trilha de 6 unidades; handle 12 × 32; região de interação de 64 unidades de altura.
- Trilha ativa azul e inativa `ui.track`; handle com contorno azul e centro branco.
- A região de interação não deve mover com a animação do handle.
- Mostrar unidade e respeitar passos do domínio. Velocidade inicial é inteira; gravidade aceita fração.
- Botões − / + são uma melhoria planejada para precisão.
- Durante voo/pausa, parâmetros indisponíveis continuam legíveis, com motivo indicado; não alterar silenciosamente o lançamento em andamento.

### 7.4 Toggles

- Exibir estado marcado/desmarcado com forma e rótulo; não usar apenas diferença sutil de cor.
- Rótulo descritivo: “Exibir vetores”, “Exibir valores”, “Animações reduzidas”.
- Área clicável inclui rótulo e caixa.
- Atual: toggle global de vetores. Controles separados de visualização são planejados.

### 7.5 Abas

- Alternam contexto sem perder os dados da simulação.
- Parâmetros e Análise compartilham posição/orientação do painel.
- Aba selecionada precisa ter indicação própria. A implementação atual usa também indisponibilidade do botão selecionado; uma revisão deve distinguir visualmente “selecionado” de “indisponível”.
- Troca de aba não pausa, dispara ou reseta implicitamente.

### 7.6 Métricas, fórmulas e legendas

- Métrica: nome curto, número dominante e unidade junto ao número.
- Diferenciar valor instantâneo, previsto e final.
- Fórmula: bloco claro contornado, expressão em azul e descrição em corpo menor.
- Legenda associa cor, símbolo e significado. Símbolos coerentes entre UI e setas.
- Não reduzir fonte automaticamente quando um valor ganha dígitos; reservar espaço para o maior valor plausível.

### 7.7 Modais, mensagens e formulários — planejados

- Modal central à interface ativa; uma pergunta/decisão por vez.
- Bloquear interação com o painel atrás, mantendo orientação espacial visível.
- Confirmar/cancelar claramente; cancelar é sempre uma saída segura.
- Erro: o que aconteceu e como recuperar, sem stack trace ou códigos técnicos como único conteúdo.
- Formulário LAN: label persistente, exemplo de IP, validação junto ao campo e conectar/cancelar. Placeholder não substitui label.
- Carregamento: nunca ficar indefinido sem timeout e recuperação.
- Toasts só para informação não crítica. Falhas de conexão e perda da sala exigem mensagem persistente até reconhecimento.

## 8. Estados e feedback de interação

| Estado | Regra |
|---|---|
| Normal | Aparência base e função reconhecível |
| Hover/foco XR | Realce leve e estável, sem acionar a função |
| Pressionado | Confirmação visual imediata |
| Selecionado | Permanece distinto após o hover |
| Desabilitado | Não reage como ação disponível; conteúdo ainda legível |
| Carregando | Impede repetição; mostra progresso/estado |
| Erro | Explicação e recuperação |
| Sucesso | Confirmação breve sem interromper o fluxo |

### DOTween: referências atuais

| Feedback | Valores padrão no código |
|---|---|
| Botão hover | Escala 1,05× |
| Botão pressionado | Escala 0,96× |
| Botão duração | 0,12 s, OutQuad |
| Handle hover | Escala 1,15× |
| Handle pressionado | Escala 1,25× |
| Handle duração | 0,12 s, OutQuad |
| Canhão disparando | Expansão transversal de 16%; 0,07 s de expansão e 0,18 s de retorno |

Valores serializados podem variar por componente. Para novos componentes, animar preferencialmente o filho visual, preservando layout e alvo de interação. Não aplicar grandes expansões aos cards próximos entre si.

- Pausar a simulação não congela menus: feedback de UI usa tempo independente.
- Cancelar tweens concorrentes e restaurar escala ao desativar/resetar.
- Nunca animar câmera, referência de medição ou origem física para reforçar feedback.
- Animações reduzidas substituem escala/pulso por mudança discreta de cor/contorno; opção ainda planejada.
- Sons e haptics são opcionais, breves e acompanhados por sinal visual. Não há voz integrada no MVP.

## 9. Visualização científica

### Paleta dos vetores

| Grandeza | Cor atual | Identificação |
|---|---|---|
| Velocidade resultante | `#284EA0` | V / velocidade resultante |
| Componente horizontal | `#3ED39C` | Vx |
| Componente vertical | `#D93645` | Vy |
| Gravidade no mundo | Amarelo `#FFFF00` | g e m/s² |
| Gravidade na legenda clara | Ocre `#9A7000` | Seta para baixo e texto |

O amarelo do vetor não deve virar texto pequeno sobre fundo claro. O azul da resultante pode perder contraste em partes escuras do ambiente: prever revisão com contorno/apoio visual, sem mudar apenas a legenda e deixar a seta inconsistente.

### Geometria e hierarquia

- Haste cilíndrica extensível; cone de tamanho fixo com base unida à ponta da haste.
- Espessura suficiente para leitura à distância, sem encobrir a bolinha e os demais vetores.
- Escala visual de comprimento é didática/configurável. Não confundir unidades de velocidade com metros do cenário.
- Vetor nulo não apresenta haste residual; seu valor pode permanecer em texto.
- Réguas/arco no plano do canhão; medir desde a base/solo conforme o modelo.
- Trajetória representa percurso em ordem temporal, sem voltar à origem ou formar fechamento indevido.
- Textos de amostras devem evitar colisão visual; priorizar a selecionada.

### Histórico e transparência

- Cinco lançamentos no máximo; opacidades-alvo 100%, 85%, 70%, 55%, 40%.
- Aplicar por idade de lançamento, não por distância do jogador.
- A mesma tentativa deve manter aparência coerente entre trajetória e ápice.
- Evitar multiplicar alpha em material e vértice, tornando a transparência maior que a especificada.
- Atual: trajetória/ápice persistem; cópias temporais são recicladas a cada disparo. Preservar amostras por tentativa e exibir detalhes seletivamente é evolução prevista no GDD.
- Transparência histórica não deve impedir acesso aos valores: o registro selecionado precisa de leitura clara no painel.

## 10. Ambientes e modelos 3D

### Materiais e acabamento

- Painéis perolados de grandes superfícies, rodapés azul-escuros e estrutura grafite.
- Faixas azul-elétrico restritas a juntas, rodapés, contornos e detalhes funcionais.
- Piso escuro com grade regular; grade menos dominante que trajetória e instrumentos.
- Materiais do cenário são diferentes dos tokens sRGB de UI. Preservar shader, espaço de cor e intensidades HDR/emissivas ao reutilizar; não converter uma cor de emissão diretamente em hex de texto.
- Os materiais compartilhados atualmente estão em `Assets/_Project/Laboratories/Physics/ObliqueLaunch/Materials/Environment/`. Migração para pasta comum é planejada, preservando GUIDs.

### Forma

- Modelos com silhueta clara, superfícies simples e detalhes moderados.
- Canhão/base: metal escuro, carenagens claras e acentos azuis coerentes com a sala.
- Movimento mecânico comunica a função: base telescópica muda altura, canhão muda ângulo.
- Não escalar detalhes do modelo de forma que deslocamento visual altere a referência científica.
- Espaços de circulação/observação livres de objetos decorativos desnecessários.

### Iluminação

- Luz ambiente suficiente para identificar objetos; azul como acento, não iluminação monocromática total.
- Priorizar iluminação de baixo custo e poucas luzes em tempo real.
- Sombras, bloom e reflexos dependem de medição no headset. Não são requisitos para a identidade visual.
- Evitar cintilação, estrobos, brilho excessivo e superfícies sobrepostas com z-fighting.

## 11. Implementação Unity e desempenho

- UGUI/TMP em Canvas World Space com interação XR; não misturar sistemas de UI sem necessidade.
- Reutilizar componentes, prefabs, fonts SDF, sprites e materiais existentes antes de criar variantes.
- Elementos decorativos/textos com `raycastTarget` desligado; só superfícies interativas devem disputar o ponteiro.
- Separar modelo, comandos e apresentação; botões não implementam fórmulas físicas nem permissões de rede.
- Uma ação coletiva passa pela autoridade de sessão; hover, escala de texto e preferência de vetores são locais.
- Atualizar textos quando dados mudarem; evitar reconstruir layout/malhas ou criar materiais a cada frame.
- Pooling para cópias/efeitos recorrentes; limitar rótulos simultâneos.
- Aplicar mudanças pontuais. Não regenerar a cena para trocar cor ou tamanho de texto.
- Preservar transformação autoral do Canvas e do rig. Ferramentas de instalação não são comandos de atualização genéricos.
- Todos os recursos próprios permanecem sob `Assets/_Project/`.

## 12. Critérios de revisão visual

Antes de considerar um componente pronto:

- [ ] Usa fonte/peso e cores previstos, sem fonte substituta silenciosa.
- [ ] Ícones originais corretos, com proporção e transparência preservadas.
- [ ] Hierarquia clara: título, conteúdo, ação principal, apoio.
- [ ] Não há corte, sobreposição, glifo ausente ou quebra inadequada de unidade.
- [ ] Tamanho físico e distância foram considerados, não apenas resolução da imagem.
- [ ] Normal, hover, pressão, seleção e indisponibilidade são distinguíveis.
- [ ] Área interativa não conflita com os controles vizinhos.
- [ ] Função disponível por ponteiro XR, não somente teclado/mouse.
- [ ] Não depende apenas de cor ou som.
- [ ] Tween não desloca câmera, física ou layout; restaura estado ao desativar.
- [ ] Estado científico e unidades correspondem à simulação.
- [ ] Nenhuma mudança de posição/orientação autoral foi aplicada incidentalmente.
- [ ] Comparação visual no Editor concluída; validação em headset registrada separadamente.

Para documentação, revisar coerência e referências; não compilar ou gerar build. Para alterações visuais, usar captura e fluxo representativo. **Builds somente com autorização explícita.**

## 13. Evolução e governança

1. Reutilizar tokens/componentes antes de adicionar novos.
2. Uma nova variante precisa ter propósito e estados documentados.
3. Atualizar Figma, implementação e este documento ao mudar um padrão compartilhado.
4. Não propagar alterações em massa sem verificar impacto nas cenas existentes.
5. Consolidar futuramente tokens em configuração compartilhada, evitando valores divergentes em scripts de autoria.
6. Revisões de conforto podem adaptar o layout do Figma; registrar a razão e validar, sem tratar fidelidade de pixels como prioridade superior à legibilidade em VR.

Pendências visuais prioritárias: fluxo LAN integrado, distinção consistente de seleção/desabilitado, ajuste de precisão nos sliders, controles separados de visualização, redução de sobreposição dos rótulos e opções de texto/animação acessíveis.

### Histórico

- **1.0 — 05/10/2026:** consolidação da identidade já aplicada, tokens de UI, tipografia, escala de espaçamento, componentes, estados, direção 3D e critérios de VR. Melhorias futuras são identificadas como diretrizes, não como funcionalidades prontas.
