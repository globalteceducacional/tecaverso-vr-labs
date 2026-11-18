# Short GDD — Corpo humano e sistemas

**Biologia · Ensino Médio · individual offline.** Segunda das [seis salas de Biologia](gdd-biologia.md). A revisão de 17/11/2026 recupera a inspeção manual de órgãos e corrige a apresentação do giro e dos quatro processos. O protótipo abaixo especifica a revisão. As verificações anteriores e as capturas da primeira versão estão identificadas separadamente.

## Objetivo e sala

Onde fica o órgão, a que sistema pertence e como ele participa de um processo? O aluno examina um corpo masculino de aproximadamente 1,7 m, separa uma peça para ver seu verso e a devolve à posição original. Pode então seguir uma etapa do processo no corpo recomposto e pausá-la para explicar o que está vendo.

Na entrada, o corpo está de frente, com o sistema respiratório selecionado e a pele semitransparente visível. A base fica no centro. À frente e à esquerda, o console deixa o tórax e a pelve livres no campo visual. A extensão direita do mesmo tampo inclinado sustenta o dial. A face azul, o indicador e o nome **Girar corpo** ficam voltados para cima e para o aluno. É possível contornar a base e operar por raio a partir de uma posição sentada. A ficha tem posição própria à direita do corpo. A faixa livre prevista em torno da base é de pelo menos 1,5 m.

## Acervo e interações

O corpo reúne as antigas salas EsqueletoHumano, SistemaCirculatorio, SistemaDigestorio, SistemaNervoso e SistemaRespiratorio. Pele e sistemas usam a pose comum do FBX masculino. O acervo feminino continua reservado para adaptação futura. Cada consulta corresponde a uma unidade identificada no acervo. Quando a unidade reúne mais de uma malha, elas permanecem juntas durante o exame.

| Sistema | Consultas | Unidades que o aluno pode afastar |
|---|---:|---|
| Respiratório | 9 | Laringe, traqueia, diafragma e os dois pulmões individualmente |
| Circulatório | 5 | Coração |
| Digestório | 11 | Esôfago, estômago, intestino delgado, intestino grosso, fígado, pâncreas e vesícula biliar |
| Nervoso | 3 | Encéfalo como conjunto |
| Esquelético | 14 | Crânio, pelve e caixa torácica, cada qual como conjunto |
| Muscular | 4 | Consulta das regiões musculares na posição anatômica |
| Urinário | 6 | Cada rim e ureter individualmente e a bexiga |
| Linfático | 4 | Baço e timo |
| Reprodutivo | 4 | Consulta das estruturas agrupadas na pelve |

**60 consultas, 24 unidades destacáveis.** Redes extensas, conjuntos distribuídos e estruturas agrupadas para contexto compõem os outros 36 alvos fixos. A pele fornece contexto e deixa o raio chegar ao órgão interno.

## Inventário de interfaces, rótulos e controles 3D

Sem HUD na cabeça. Há um console de operação e uma ficha de consulta opcional. O aluno solicita informação diretamente no órgão.

| ID e localização | Finalidade e gesto | Inicial | Entrada e saída | Simultâneo |
|---|---|---|---|---|
| R1 · título e legenda no corpo | Identificar o sistema e ler a etapa em curso. A legenda acompanha o fenômeno e conserva a etapa na pausa | Sistema respiratório e contexto didático | Corrige o conteúdo ao escolher sistema ou mudar de etapa | C1, G1, E1, S1 e, se aberta, Q1 |
| R2 · nome à esquerda do corpo e linha-guia | Ligar o nome escolhido à unidade observada, inclusive durante a inspeção | Oculto | Selecionar revela. Fechar a consulta ou trocar sistema recolhe | R1, C1, G1, Q1, E1, S1 |
| C1 · grade e ações no tampo esquerdo | Escolher um dos nove sistemas na grade 3 × 3, alternar Pele e iniciar, pausar ou continuar o percurso. O texto da ação indica o próximo passo | Respiratório, pele ligada e percurso parado | Sistemas anatômicos sem processo recolhem a ação de percurso. Durante o afastamento a apresentação do percurso é suspensa | R1, G1, E1, S1, Q1 e R2 quando houver seleção |
| G1 · dial Unity ao lado direito da grade | Segurar o manípulo e girar o conjunto em torno do eixo vertical. O corpo mantém posição e altura | Disponível com face e nome legíveis da frente | Pegar ou soltar controla o giro. A silhueta aparece somente na peça móvel | Todos |
| Q1 · ficha lateral direita | Ler nome, função, contexto e limites. **Fechar consulta** encerra a leitura | Oculta | Seleção direta ou por raio abre. Outro alvo substitui o conteúdo. Trocar sistema fecha | Uma ficha por vez, R2 e controles |
| E1 · linha de instrução no console | Explicar o gesto de exame, a soltura, a estrutura fixa ou o estado do percurso | Convite para selecionar uma estrutura | Atualiza com a ação. A tentativa em um grupo fixo preserva sua consulta e posição | C1, G1, R1 e Q1 durante consulta |
| S1 · Voltar ao Hub no rodapé do console | Retornar à disciplina | Visível | Ação direta a qualquer momento | Todos |

Não há confirmação destrutiva. A soltura já oferece saída reversível. Legendas da gravação e instruções de mouse do protótipo são externas à sala.

## Prévias da revisão

[Experimentar a proposta em HTML](prototypes/biologia/anatomy-layout.html). O desenho representa a anatomia em projeção frontal simplificada. No navegador, clique no órgão ilustrado para consultar, mantenha pressionado e arraste para examinar, solte para devolver. Arraste a manopla ou use as setas do teclado com o dial em foco. O protótipo demonstra um alvo representativo por sistema. Os totais da tabela acima se referem à sala Unity.

**Entrada · R1, C1, G1, E1, S1.** De frente para o console, a face de pega se vê sobre o prolongamento do tampo. O corpo fica no centro. Escolher um sistema muda o conteúdo no mesmo lugar.

![Proposta de entrada e manopla visível na face superior do suporte](../GeneratedMedia/GDD/biologia/anatomia-layout-entry.png)

**Consulta · Q1 e R2.** Uma seleção breve no pulmão esquerdo abre a ficha, mantendo as mãos e a região torácica livres. Fechar recolhe a ficha e o nome.

![Proposta da consulta ao pulmão esquerdo](../GeneratedMedia/GDD/biologia/anatomia-layout-detail.png)

**Manipulação · Q1, R2 e E1.** Segurar e mover afasta o coração. A ficha fica na estação. O modelo não recebe uma cópia flutuante, um cartão na mão ou um novo painel. Ao soltar, o coração volta à pose que ocupa no corpo.

![Proposta do coração retirado para inspeção e instrução de devolução](../GeneratedMedia/GDD/biologia/anatomia-layout-grab.png)

**Em movimento · R1 e C1.** O avanço luminoso está na região do trajeto, com o sistema arterial e o retorno venoso distinguíveis. A etapa é acompanhada na legenda.

![Proposta de destaque em trajeto vascular](../GeneratedMedia/GDD/biologia/anatomia-layout-path.png)

**Pausa · R1 e C1.** O aluno para a frente de propagação do sinal e pode continuar a partir daquele lugar. A orientação do corpo pode mudar durante a pausa.

![Proposta do sistema nervoso com etapa e sinal pausados](../GeneratedMedia/GDD/biologia/anatomia-layout-paused.png)

**Consulta a grupo fixo · E1, Q1 e R2.** Selecionar a musculatura permite ler a ficha. E1 explica que a estrutura é fixa. O sistema muscular oferece exploração estática e o console recolhe o controle de percurso.

![Proposta da consulta a grupo fixo no sistema muscular](../GeneratedMedia/GDD/biologia/anatomia-layout-static.png)

As seis imagens são capturas da proposta HTML. Para reproduzir, executar `node Tools/biology-migration/capture-anatomy.cjs` com as dependências locais de `Tools/tecaverso-demo`. As imagens ficam na pasta ignorada `GeneratedMedia/GDD/biologia` e devem acompanhar o Markdown quando compartilhado. O [protótipo original](prototypes/biologia/index.html?lab=anatomia) documenta a primeira proposta.

## Sequência do aluno e feedback

1. Escolher **Circulatório** e ocultar Pele se quiser ver o coração com mais contraste. Foco por raio ou proximidade mostra uma única silhueta sobre o órgão.
2. Selecionar o coração para ler a ficha. Mantendo a seleção, mover a mão ou o ponto de pega do raio para afastar a peça e girá-la. Um pequeno limiar de deslocamento distingue a consulta breve da retirada deliberada. A malha conserva cor e escala.
3. Soltar para devolver automaticamente. O destino é a pose local no corpo, inclusive se o aluno tiver girado o conjunto. Uma peça pode ficar afastada por vez. Movimento reduzido faz uma restituição imediata. Não há arremesso ou perda de órgão no chão.
4. Iniciar o circuito, pausar no membro superior e girar o corpo para ver a mesma etapa de outro ângulo. Se examinar uma das peças durante um percurso, suspender a visualização enquanto estiver afastada. A devolução recupera a fase e o estado anterior de reprodução, inclusive a pausa manual.
5. Escolher **Nervoso**. A consulta anterior fecha, a peça volta à origem e o percurso anterior some. Observar a propagação da mão ao encéfalo. Escolher **Muscular** para comparar uma estrutura fixa ou usar **Voltar ao Hub**.

[Feedback de interação compartilhado](gdd-interaction-feedback.md) para foco, ação e retenção da silhueta. O retorno é curto, desacelera ao chegar à origem e deixa os vizinhos imóveis. Com preferência reduzida, foco e seleção permanecem legíveis de forma estática. O processo científico só avança após solicitação e pode ser pausado. Câmera e foco visual do aluno ficam estáveis.

## Percursos e limites científicos

| Sistema | O que observar | Como ler a pausa |
|---|---|---|
| Respiratório | Destaque ciano progride pelas vias até os dois pulmões durante a inspiração e volta na expiração | Identificar sentido e região. Sem modelo individual de alvéolos ou cálculo de volume |
| Circulatório | O coração direito envia sangue menos oxigenado aos pulmões. A representação muda para sangue mais oxigenado, retorna ao coração esquerdo e segue pelas artérias até a mão esquerda. O sangue retorna pelas veias | Distinguir direção e etapa pelo texto e pela frente luminosa. Azul e vermelho codificam oxigenação. As redes brônquicas não substituem o circuito pulmonar |
| Digestório | Uma onda de destaque acompanha boca, faringe, esôfago, estômago, intestino delgado e intestino grosso até a região de eliminação | Distinguir por onde passa o conteúdo. Fígado, pâncreas e vesícula são órgãos acessórios |
| Nervoso | Uma frente luminosa sensitiva progride dos nervos da mão direita e do braço à medula cervical, ao tronco encefálico e ao encéfalo | Identificar uma etapa de propagação no tecido. O exemplo representa uma sequência regional, sem partícula material ou velocidade de condução |

A luz é didática. Os trechos esquemáticos respeitam a posição e a profundidade das estruturas. A sala apresenta a sequência funcional macroscópica, sem simular vazão, pressão, peristaltismo quantitativo, ECG ou um trato nervoso específico. Ossos e redes agrupados conservam o agrupamento da malha de origem. Os modelos e textos precisam de revisão científica independente.

Referências de autoria: OpenStax *Anatomy and Physiology 2e* — [vias respiratórias](https://openstax.org/books/anatomy-and-physiology-2e/pages/22-1-organs-and-structures-of-the-respiratory-system), [coração](https://openstax.org/books/anatomy-and-physiology-2e/pages/19-1-heart-anatomy), [circuitos circulatórios](https://openstax.org/books/anatomy-and-physiology-2e/pages/20-5-circulatory-pathways), [digestão](https://openstax.org/books/anatomy-and-physiology-2e/pages/23-1-overview-of-the-digestive-system) e [organização nervosa](https://openstax.org/books/anatomy-and-physiology-2e/pages/12-1-basic-structure-and-function-of-the-nervous-system). O aplicativo usa apenas dados locais.

## Verificação e critérios de aceite

O vídeo de 3min14s anterior à revisão fornece a linha de base. No quadro de **1min**, os marcadores circulatórios seguem diagonais fora dos vasos no abdome. Em **2min08s**, a linha nervosa cruza o tronco e ultrapassa o encéfalo. Em **2min30s**, a manopla aparece por baixo da placa e encobre parte do nome. Esses três enquadramentos devem ser repetidos após a correção.

| Verificar no Editor | Critério observável |
|---|---|
| Manopla de frente, em três quartos e de cima | Indicador, face de pega e nome legíveis. O collider acompanha a manopla. Girar muda somente a orientação do corpo |
| Consulta rápida, retirada, giro e soltura | A consulta abre sem deslocamento acidental. Afastar uma unidade destacável movimenta apenas aquela peça. Soltar, trocar sistema e sair nunca deixam alvos ou seleções invisíveis |
| Quatro percursos de frente, a 45° e de perfil | Objeto, luz e etapa coincidem ao girar. Sem trilhos flutuando diante do corpo. Pausar mantém avanço e cor, retomar continua da mesma etapa |
| Pausa + retirada e modo reduzido | Recuperar o estado de processo vigente ao devolver. Sem pulsação decorativa, salto de câmera ou perda de referência da peça |
| Sistemas estáticos, grupos fixos e Hub | A ação de percurso fica recolhida e o grupo fixo continua consultável. A troca limpa os estados anteriores. Entrada e saída funcionam no modo individual |

A primeira versão já foi verificada para nove sistemas exclusivos, 60 consultas, retorno ao Hub e operação offline. Seu [registro de validação](biology/validation.md) e a [gravação inicial](recording-anatomy.md) constituem histórico. O protótipo serve para decidir a disposição e os gestos. Alcance sentado/em pé, legibilidade, conforto e desempenho em headset precisam de verificação física.

### Revisão implementada na Unity

No Play Mode, a pega direta e a seleção por raio foram exercitadas com interatores XR. O exame do coração incluiu retirada, rotação, retorno após girar o corpo, cancelamento ao trocar o sistema e preservação da pausa. As imagens abaixo mostram a execução que sucedeu as prévias HTML.

![Manopla do dial e identificação no mesmo tampo](../GeneratedMedia/Anatomy/RevisaoFinal/dial-em-pe.png)

![Troca de oxigenação junto às redes da mão](../GeneratedMedia/Anatomy/RevisaoFinal/circulacao-mao.png)

![Etapa cervical vista por trás do corpo](../GeneratedMedia/Anatomy/RevisaoFinal/nervoso-medula.png)
