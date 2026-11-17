# Short GDD — DNA, RNA e informação

**Status:** aprovado para desenvolvimento; ainda não implementado. **Disciplina:** Biologia. **Modo:** individual offline. **Público:** Ensino Médio. **Data:** 17/11/2026. Uma das **seis** salas propostas, conforme [índice](gdd-biologia.md).

## Objetivo e experiência

Investigar complementaridade e orientação de fitas. O aluno monta uma fita complementar de DNA e um trecho de RNA a partir de uma fita molde de DNA; compreende a diferença entre T e U.

## Acervo, objetos e sala

**Cenas antigas consolidadas:** DnaRna.

**Reaproveitamento comprovado por arquivos:** dna_strand_set.FBX e Chromosome FBX. A hélice antiga serve à observação. Bases com encaixes semânticos e suporte de montagem são novos: o acervo não comprova um kit funcional de pareamento ou um modelo de RNA.

**Catálogo fechado desta entrega:** Uma hélice de observação, um cromossomo em vitrine e duas montagens independentes de quatro posições. Molde 3′–A T G C–5′. DNA complementar 5′–T A C G–3′; RNA 5′–U A C G–3′. Bandeja com cinco tipos de bases A/T/G/C/U e reposição limitada aos quatro encaixes de cada atividade.

| Conjunto de objetos | Papel | Interação | Limite |
|---|---|---|---|
| Modelos e peças descritos no catálogo | Fenômeno principal | Seleção direta, pega/rotação ou encaixe conforme o fluxo abaixo | Não disponibilizar partes sem identificação revisada |
| Suportes e sockets da atividade | Ancoragem, retorno e alcance | Controles 3D e filtros de encaixe | Preservar referência espacial e evitar seleção concorrente |
| Consulta e navegação | Apoio opcional | Apontar/tocar e fechar; voltar ao Hub | Uma consulta simultânea; sem HUD na câmera |

**Disposição proposta:** Hélice ampliada ancorada à esquerda; montagem em bancada inclinada frontal, quatro sockets alcançáveis sem braços elevados. Vitrine do cromossomo à direita. Consulta lateral sem cobrir bandeja. Não exigir pegar a hélice inteira.

**Controles escolhidos pela tarefa:** Escolha exclusiva DNA/RNA, apenas dois estados; cinco tipos de peças com letras gravadas; quatro sockets; Restaurar fitas; anel de rotação na hélice. Nenhum slider de bases. Os botões de bases no HTML são proxies da bandeja física.

## Inventário de interfaces, rótulos e controles 3D

**HUD na câmera: nenhum.** Uma ficha opcional de consulta. Os nomes ficam nos suportes e junto à estrutura selecionada; nenhum cartão extenso acompanha uma peça na mão. A sala não repete o formulário de Física.

| ID | Tipo e localização | Finalidade e ação | Inicial | Abre/fecha | Simultâneo |
|---|---|---|---|---|---|
| R1 | Nomes nos suportes e legenda junto ao modelo | Identificar o objeto e informar ampliação didática; rótulos extras só quando solicitados | Nomes visíveis; extras ocultos | Toggle quando houver; seleção realça uma estrutura | C1, S1; Q1 pode consultar a selecionada |
| C1 | Controles 3D no suporte e objetos da atividade | Escolha exclusiva DNA/RNA, apenas dois estados; cinco tipos de peças com letras gravadas; quatro sockets; Restaurar fitas; anel de rotação na hélice. Nenhum slider de bases. Os botões de bases no HTML são proxies da bandeja física. | Visíveis; estado inicial da atividade | Pega/toque/raio, evento de domínio | R1, E1, Q1 e S1; bloqueados por X1 |
| Q1 | Ficha clara fixa à lateral do suporte | Nome, função, contexto, limite do modelo; Fechar | Oculta | Selecionar uma parte ou símbolo de consulta; Fechar ou trocar alvo fecha/substitui | Objetos e C1 permanecem visíveis; uma Q1 por vez |
| E1 | Uma linha no suporte, sem painel flutuante | Confirmar ação ou explicar recusa/limite; não depender apenas de cor | Instrução inicial curta | Atualiza por evento; erro persiste até correção ou outra ação pertinente | R1, C1 e Q1 |
| X1 | Confirmação no espaço da consulta | Cancelar/Restaurar quando houver perda de montagem/população | Oculta | Ação Restaurar; decisão fecha | Objetos ao fundo visíveis; C1, Q1 e S1 sem interação |
| S1 | Voltar ao Hub, junto à saída e ao suporte | Retorno padrão à disciplina, sem confirmação se não houver estado relevante | Visível | Permanente | Todos, exceto bloqueio por X1 |

O cabeçalho de revisão e a seção “Como isso funciona em RV?” do HTML não são interfaces a implementar na sala. Motion reduzido é preferência geral do produto, exposta no menu padrão; não exige outro painel permanente de Biologia.

## Sequência e casos sem resultado

1. Entrar, reconhecer as peças e a primeira ação em E1.
2. Selecionar socket vazio → pegar base → aproximar → encaixar se compatível. Completar DNA, alternar para RNA e montar; voltar para DNA recupera a primeira montagem. Consultar cromossomo e relacionar as escalas de organização.
3. Fechar a consulta, comparar outro caso e retornar ao estado de exploração.
4. Restaurar quando aplicável: X1 pede confirmação; cancelar preserva os objetos; S1 retorna ao Hub.

**Recusas e recuperação:** Pareamento incompatível não altera a fita, não consome peça e mantém a base na mão; E1 explica a regra. Remover uma base permite corrigir a posição. Troca de atividade preserva ambas as montagens. Confirmação apenas ao restaurar.

## Modelo científico e limites

Kit de complementaridade, não síntese molecular completa. Relações antiparalelas e 5′/3′ explícitas; açúcar/fosfato descritos na consulta. Sem tradução, códons, aminoácidos, mutações ou simulação atômica nesta entrega. A hélice importada pode precisar remalhamento para partes interativas.

Referência de revisão: [OpenStax — conteúdo relacionado](https://openstax.org/books/biology-2e/pages/14-2-dna-structure-and-sequencing). Os modelos artísticos precisam de validação semântica; nomes de arquivos não são autoridade taxonômica/anatômica. Reutilização técnica não substitui revisão do conteúdo.

**Critério específico:** Rejeitar A–A; aceitar A–T no DNA e A–U na transcrição. Completar quatro posições, remover, recolocar e preservar ao alternar. Não mostrar U em DNA.

## Interação, feedback e ergonomia

Pega por proximidade e raio XR conforme a peça; nenhuma ação essencial exige hand tracking. Bases são ancoradas e não são carregadas pela sala. Partes manipuláveis têm suporte/socket de retorno e área de pega distinta do botão de consulta. Não adicionar Rigidbody a cada submalha automaticamente. Alvos amplos, colliders simples e filtros evitam selecionar a base em vez da peça.

Foco: contorno em 0,12–0,16 s. Ação: confirmação de até 0,22 s no filho visual. Estado selecionado persiste; collider, câmera e referência científica permanecem estáveis. Encaixe aceita uma vez por gesto; recusa mantém a peça e explica correção. Reutilizar `Shared/Interaction`, preferência `MotionPreferences.Reduced` e controles oficiais Unity em `Shared/XR/Controls`, sem duplicar tweens. Som curto e haptic podem complementar, nunca substituir informação visual. Não inventar partículas para ornamentar uma galeria: usar somente para representar um processo cadastrado, com legenda e pausa quando temporal.

Modelos têm alcance e altura ajustáveis na autoria; o primeiro protótipo mira controles de pega entre 0,8 e 1,25 m, com consulta a cerca de 2–3 m. Validar sentado/em pé em headset antes de homologar. O controle de motion é a preferência geral da aplicação; no HTML está no cabeçalho de revisão.

## Prévias e estados

**Proposta, não cena Unity implementada.** O [protótipo HTML](prototypes/biologia/index.html?lab=dna) usa diagramas esquemáticos. Header, navegação de revisão e comandos tracejados de simular pega/giro são externos ao laboratório; não autorizam novos painéis na Unity. Os controles sólidos representam controles da sala. A ficha aparece somente por seleção. O estado `detail` é exemplo de consulta; `action` exemplifica manipulação/resultado; `error` é exemplo visual do aviso, não execução automática de uma falha real.

![Disposição espacial proposta](../GeneratedMedia/GDD/biologia/layout.png)

Visão espacial comum: entrada, suporte elevado, faixa de circulação e consulta lateral. A disposição específica é descrita acima.

![Entrada](../GeneratedMedia/GDD/biologia/dna-entry.png)

Entrada: R1, C1 e S1 visíveis; Q1 e X1 ocultos. Os objetos são a ação principal.

![Interação e resultado](../GeneratedMedia/GDD/biologia/dna-action.png)

Manipulação/resultado: C1 altera os objetos; E1 confirma a ação. Fechar Q1 mantém o experimento. Não há interface sobre a câmera.

![Consulta opcional](../GeneratedMedia/GDD/biologia/dna-detail.png)

Consulta: selecionar objeto ou símbolo da base abre Q1 lateral; Fechar remove o bloqueio visual sem resetar estado.

![Recusa ou limite](../GeneratedMedia/GDD/biologia/dna-error.png)

Impedimento: E1 descreve a recuperação sem modal. O estado válido anterior é preservado.

![Restauração](../GeneratedMedia/GDD/biologia/dna-reset.png)

Confirmação: X1 bloqueia manipulação e navegação, Cancelar preserva tudo; Restaurar retorna à entrada. Só existe onde há restauração destrutiva.

Capturas locais ignoradas em `GeneratedMedia/GDD/biologia/`. Para compartilhar o GDD, enviar também essas imagens; elas não acompanham Git. Fonte e reprodução em [README do protótipo](prototypes/biologia/README.md).

## Execução e aceite

Modo inicial individual offline; textos, malhas, texturas e metadados embarcados. Sem API em runtime. LAN não implementada: a infraestrutura atual só dá autoridade coletiva ao lançamento oblíquo; não anunciar Biologia multiplayer.

- Completar o fluxo descrito, incluindo seleção, manipulação, consulta, correção e restauração/cancelamento.
- Rótulos e controles correspondem ao inventário; objetos e área de pega ficam legíveis durante consultas.
- Ocultar um objeto cancela foco e interação; não manter colisores ou seleção invisíveis. Voltar ao Hub não exige navegar por telas de estudo.
- Restaurar respeita estado autoral de cada peça; a preferência reduzida não altera o conteúdo científico.
- Compilação, testes de domínio e captura no Editor serão exigidos na implementação. Alcance, conforto e desempenho em headset são etapas separadas. Nenhum build sem autorização.
