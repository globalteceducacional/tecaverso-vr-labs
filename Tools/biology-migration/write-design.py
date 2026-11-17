"""Generate the six proposal GDDs from an explicit, reviewable design inventory."""
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]
items=[
dict(id='celulas',name='Células animal e vegetal',old='CelulaAnimalVegetal',goal='Comparar estruturas comuns e exclusivas, relacionando forma e função. O aluno seleciona, gira e afasta organelas; identifica por que cloroplasto, parede e vacúolo central não devem aparecer como características da célula animal.',assets='Animal_Cell.fbx e Plant_Cell.fbx; prefabs com 72 e 78 GameObjects, respectivamente. Parte dos nomes são genéricos: requerem mapa semântico manual. Núcleo, mitocôndria e lisossomo já têm nomes reconhecíveis no prefab animal.',catalog='Duas células visíveis simultaneamente. Na primeira entrega, seis estruturas consultáveis: núcleo, mitocôndria, membrana plasmática, parede celular, cloroplasto e vacúolo central. Núcleo e mitocôndria devem ser identificáveis nos dois modelos; estruturas da seleção atual são realçadas, sem apagar o outro exemplar.',space='Dois suportes vizinhos a 1,10–1,30 m de altura, modelos com cerca de 0,8–1,0 m de largura. Faixa de circulação frontal de 1,5 m. Consulta à direita e afastada dos modelos. A escala é de observação, com aviso de ampliação.',controls='Toggle Rótulos; Recompor células. Dois anéis ancorados de rotação e sockets de retorno das organelas. Não há seletor obrigatório Animal/Vegetal: ambas ficam expostas.',action='Selecionar mitocôndria → abrir Q1 → segurar a peça → afastá-la até 0,4 m do conjunto → aproximar do socket original → soltar. Contorno do socket e breve encaixe confirmam retorno. Retirar uma organela é exploração de um modelo desmontável, não uma célula sobrevivendo sem ela.',failure='Uma membrana ou parede não destacável permanece no conjunto; peças não entram em sockets de outra organela. Retorno automático por perda/queda fora da área é anunciado em E1. Não aceitar identificação inventada a partir de nomes ObjectXX.',model='Cores e tamanhos internos são didáticos. Não há metabolismo quantitativo, divisão celular ou escala comum entre célula e corpo humano. Movimento decorativo em repouso é opcional; rotação automática pausa durante seleção, pega e consulta.',source='https://openstax.org/books/biology-2e/pages/4-3-eukaryotic-cells',verify='Núcleo e mitocôndria nas duas células; cloroplasto/parede/vacúolo central somente na vegetal representada. Separar e reencaixar a peça correta; consulta nunca cobre o ponto de pega.'),
dict(id='anatomia',name='Corpo humano e sistemas',old='EsqueletoHumano + SistemaCirculatorio + SistemaDigestorio + SistemaNervoso + SistemaRespiratorio',goal='Relacionar órgãos, sistemas e sua posição no mesmo corpo. O aluno gira um corpo ancorado, escolhe um sistema e consulta órgãos; observa quatro percursos esquemáticos, sem trocar de sala.',assets='Body, Skeleton, Muscles, NervousSystem, Sistema Digestorio, Sistema Renal, Sistema Reprodutivo, SistemaCirculatório, Sitema Respiratorio e LymphaticSystem. São conjuntos derivados do FBX masculino. Há FBX feminino separado, preservado para adaptação futura, sem prometer equivalência ou alternância pronta.',catalog='Nove sistemas/camadas além da pele: respiratório, circulatório, digestório, nervoso, esquelético, muscular, urinário, linfático e reprodutivo. Um sistema principal por vez e pele independente. Quatro percursos: ar/vias respiratórias, sangue em circuito, caminho digestório e sinal nervoso ilustrativo. Os demais são exploração anatômica estática.',space='Corpo com aproximadamente 1,7 m de altura, base giratória ancorada no centro; circulação de pelo menos 1,5 m ao redor. Seleção de sistemas no suporte frontal inclinado, abaixo do tórax visual. Consulta lateral fixa. Não copiar o posicionamento antigo de órgãos soltos a vários metros do corpo.',controls='Nove botões exclusivos nomeados; toggle Pele; Observar/Pausar percurso somente nos quatro sistemas com percurso; anel físico de rotação. Sem slider de índice de sistema e sem um painel de parâmetros.',action='Escolher Respiratório → ocultar pele → selecionar pulmão → consultar função → observar percurso → pausar → escolher Circulatório e repetir no coração. Visão muda no mesmo referencial corporal. Esqueleto e músculos são alternativas na mesma sala.',failure='Trocar sistema fecha a consulta do órgão oculto e interrompe seu percurso, sem deixar collider invisível. Órgãos não mapeados são identificados como indisponíveis para consulta. Não mostrar animação de contração muscular ou articulação se o modelo não foi rigado.',model='Preservar cores anatômicas; azul/vermelho do esquema de vasos não representa cor real do sangue. Ar, sangue, alimento e sinal não compartilham a mesma animação ou legenda. Sem vazão, ECG, diagnóstico, cirurgia ou fisiologia quantitativa. O esqueleto tem grupos de ossos: não prometer seleção dos 206 ossos.',source='https://openstax.org/books/biology-2e/pages/1-2-themes-and-concepts-of-biology',verify='Os nove sistemas são selecionáveis sem mudar de cena. Pele, visibilidade e consultas coerentes. Quatro percursos com legendas próprias e pausa. Preservar alinhamento anatômico de origem.'),
dict(id='microbios',name='Microrganismos e vírus',old='Microrganismos',goal='Distinguir bactérias, vírus e estruturas celulares; relacionar um bacteriófago com uma célula hospedeira. A galeria é observável diretamente, sem catálogo de seleção obrigatório.',assets='Nove FBX e nove prefabs: Bacillus bacteria, Bacterium, Green bacterium, Salmonella, Virus, Light Virus, Bacteriophage Virus, Flagellas e Chromosome. Sete são exemplares artísticos; flagelo e cromossomo são estruturas complementares, não organismos.',catalog='Galeria de sete exemplares: quatro representações bacterianas e três virais. Duas vitrines de estruturas: flagelo e cromossomo. Diorama novo de ciclo lítico simplificado do fago com três etapas: adsorção/entrada, síntese/montagem, lise/liberação. Não inferir espécies ou hospedeiros pelo arquivo.',space='Galeria em semicírculo com suportes elevados, cerca de 1,2 m. Exemplares 0,35–0,65 m; escala independente por exemplar, com indicação explícita. Diorama menor à lateral; consulta fixa junto à galeria. Caminho central livre.',controls='Toggle Rótulos de estruturas; botão físico Próxima etapa do fago; Restaurar galeria. Anéis de giro nos suportes. Os exemplares são selecionados diretamente por raio ou toque.',action='Selecionar bactéria → consultar organização celular → fechar → selecionar vírus → comparar dependência de hospedeiro → selecionar flagelo e reconhecer que é parte de organismo → percorrer as três etapas do fago. A etapa não avança sozinha enquanto o aluno lê.',failure='Sem cruzamento livre de qualquer vírus com qualquer bactéria. A associação fago–hospedeiro do diorama é ilustrativa e cadastrada. Colocar dois modelos próximos não inventa infecção. A consulta esclarece nomes genéricos e o cromossomo em X.',model='Modelos artísticos não são microscopia ou identificação taxonômica. Vírus são acelulares; replicação depende de hospedeiro. A forma em X representa cromossomo duplicado condensado, não todo DNA bacteriano. Sem cultivo, patogenicidade calculada, tratamento ou transmissão clínica.',source='https://openstax.org/books/biology-2e/pages/21-2-virus-infections-and-hosts',verify='Separar sete exemplares de duas estruturas nos rótulos. Ciclo do fago só no diorama. Avanço, retorno e restauração consistentes; não extrapolar espécie pela cor.'),
dict(id='dna',name='DNA, RNA e informação',old='DnaRna',goal='Investigar complementaridade e orientação de fitas. O aluno monta uma fita complementar de DNA e um trecho de RNA a partir de uma fita molde de DNA; compreende a diferença entre T e U.',assets='dna_strand_set.FBX e Chromosome FBX. A hélice antiga serve à observação. Bases com encaixes semânticos e suporte de montagem são novos: o acervo não comprova um kit funcional de pareamento ou um modelo de RNA.',catalog='Uma hélice de observação, um cromossomo em vitrine e duas montagens independentes de quatro posições. Molde 3′–A T G C–5′. DNA complementar 5′–T A C G–3′; RNA 5′–U A C G–3′. Bandeja com cinco tipos de bases A/T/G/C/U e reposição limitada aos quatro encaixes de cada atividade.',space='Hélice ampliada ancorada à esquerda; montagem em bancada inclinada frontal, quatro sockets alcançáveis sem braços elevados. Vitrine do cromossomo à direita. Consulta lateral sem cobrir bandeja. Não exigir pegar a hélice inteira.',controls='Escolha exclusiva DNA/RNA, apenas dois estados; cinco tipos de peças com letras gravadas; quatro sockets; Restaurar fitas; anel de rotação na hélice. Nenhum slider de bases. Os botões de bases no HTML são proxies da bandeja física.',action='Selecionar socket vazio → pegar base → aproximar → encaixar se compatível. Completar DNA, alternar para RNA e montar; voltar para DNA recupera a primeira montagem. Consultar cromossomo e relacionar as escalas de organização.',failure='Pareamento incompatível não altera a fita, não consome peça e mantém a base na mão; E1 explica a regra. Remover uma base permite corrigir a posição. Troca de atividade preserva ambas as montagens. Confirmação apenas ao restaurar.',model='Kit de complementaridade, não síntese molecular completa. Relações antiparalelas e 5′/3′ explícitas; açúcar/fosfato descritos na consulta. Sem tradução, códons, aminoácidos, mutações ou simulação atômica nesta entrega. A hélice importada pode precisar remalhamento para partes interativas.',source='https://openstax.org/books/biology-2e/pages/14-2-dna-structure-and-sequencing',verify='Rejeitar A–A; aceitar A–T no DNA e A–U na transcrição. Completar quatro posições, remover, recolocar e preservar ao alternar. Não mostrar U em DNA.'),
dict(id='ecossistemas',name='Ecossistemas e relações',old='EcossistemaInterativo',goal='Construir uma cadeia alimentar simples e distinguir fluxo de energia de ciclagem de matéria. O aluno manipula fichas com representações de organismos e observa setas coerentes com a montagem.',assets='A cena antiga contém ambiente/estrutura inicial, sem um ecossistema funcional identificado. Diorama, capim, gafanhoto e sapo são conteúdo novo a modelar ou obter com procedência registrada. Os modelos bacterianos existentes podem ilustrar decompositores, com identificação genérica e ressalva de escala.',catalog='Um cenário terrestre didático; três fichas com relevo/silhueta e nome (capim, gafanhoto, sapo), três sockets de papéis tróficos e um módulo de decomposição fixo. Não usar Salmonella como exemplo obrigatório de decompositor. Esta versão não é simulador populacional ou editor de teias livres.',space='Diorama horizontal elevado a 1,1 m, com largura aproximada de 2,4 m, acessível pelo lado frontal e laterais. Bandeja na borda frontal, controles em suporte lateral. Decomposição em seção lateral distinta; sem mesa ocupando todo o espaço de caminhada.',controls='Três fichas XR e três encaixes; toggle Exibir fluxo; Restaurar diorama; consulta tocando o módulo de matéria/energia. Sem sliders de população, chuva ou temperatura.',action='Encaixar capim, gafanhoto e sapo na ordem; ativar fluxo; seguir energia solar → produtor → consumidor, com dissipação de calor. Consultar decompositores e observar retorno de nutrientes ao ambiente. Remontar a cadeia.',failure='Ficha em papel trófico errado é recusada com explicação local, permanece na mão e pode ser tentada novamente. Partículas de transferência só aparecem nas conexões completas; nenhuma cadeia inexistente é animada.',model='As setas vão do recurso alimentar ao consumidor. Energia não circula de volta ao Sol; matéria retorna ao ambiente por processos de decomposição. Sem lei universal de 10%, sem mortalidade automática e sem previsão ecológica real. Todas as imagens e textos locais.',source='https://openstax.org/books/biology-2e/pages/46-2-energy-flow-through-ecosystems',verify='Recusar predador no socket produtor. Montagem completa gera setas corretas. Matéria e energia têm legendas e percursos distintos; energia não aparece como ciclo fechado.'),
dict(id='evolucao',name='Evolução e seleção natural',old='EvolucaoEspecies',goal='Observar mudanças de frequência entre gerações sob seleção. O aluno alterna um ambiente, guarda uma referência e compara populações; indivíduos não mudam a própria característica ao trocar o ambiente.',assets='A cena antiga é um esboço de ambiente, sem uma simulação de evolução encontrada. Suporte e população de organismos estilizados são novos. Não converter variantes coloridas de bactérias em espécies distintas sem justificativa.',catalog='24 indivíduos estilizados, inicialmente 12 claros e 12 escuros; dois ambientes (claro/escuro); seis gerações por execução; uma referência guardada. Característica herdável binária simplificada, sem sexualidade ou genótipos explícitos nesta entrega.',space='Diorama elevado de aproximadamente 2,0 × 1,2 m, população inteira visível. Ambiente alternado por alavanca na borda; visor na base mostra geração e contagens. Referência em faixa fixa pequena, ao lado do visor, sem segundo laboratório ou câmera.',controls='Alavanca Claro/Escuro; botão físico Avançar geração; Guardar referência; Restaurar população. Consulta sobre seleção por toque no símbolo informativo da base. Sem slider de gerações, velocidade ou número de indivíduos.',action='Guardar a população inicial → avançar três gerações no ambiente claro → comparar → alternar ambiente → avançar até seis → observar a tendência. A animação distingue sobrevivência e nascimento dos descendentes; mudança de ambiente não recolore indivíduos.',failure='Ao atingir seis gerações, Avançar fica indisponível com motivo. Guardar referência substitui apenas a referência, sem perder população. Cancelar restauração preserva geração, ambiente, frequência e referência.',model='Modelo didático determinístico: pesos relativos 1 para fenótipo favorecido e 0,7 para o outro. n_claros′ = round(24 × n_claros × w_claro / (n_claros × w_claro + n_escuros × w_escuro)); n_escuros′ = 24 − n_claros′. Pesos são escolhas do modelo, não dados de uma espécie. Sem deriva, mutação, migração, especiação, genética mendeliana ou previsão real. Contagens arredondadas podem estabilizar e isso deve ser explicado.',source='https://openstax.org/books/biology-2e/pages/19-3-adaptive-evolution',verify='Ambiente sozinho não muda contagens ou cor. Geração mantém total 24; tendência acompanha pesos. Referência é cópia imutável; seis é limite explícito. Não chamar evolução de melhoria ou intenção.')
]

common='''## Interação, feedback e ergonomia

Pega por proximidade e raio XR conforme a peça; nenhuma ação essencial exige hand tracking. Bases são ancoradas e não são carregadas pela sala. Partes manipuláveis têm suporte/socket de retorno e área de pega distinta do botão de consulta. Não adicionar Rigidbody a cada submalha automaticamente. Alvos amplos, colliders simples e filtros evitam selecionar a base em vez da peça.

Foco: contorno em 0,12–0,16 s. Ação: confirmação de até 0,22 s no filho visual. Estado selecionado persiste; collider, câmera e referência científica permanecem estáveis. Encaixe aceita uma vez por gesto; recusa mantém a peça e explica correção. Reutilizar `Shared/Interaction`, preferência `MotionPreferences.Reduced` e controles oficiais Unity em `Shared/XR/Controls`, sem duplicar tweens. Som curto e haptic podem complementar, nunca substituir informação visual. Não inventar partículas para ornamentar uma galeria: usar somente para representar um processo cadastrado, com legenda e pausa quando temporal.

Modelos têm alcance e altura ajustáveis na autoria; o primeiro protótipo mira controles de pega entre 0,8 e 1,25 m, com consulta a cerca de 2–3 m. Validar sentado/em pé em headset antes de homologar. O controle de motion é a preferência geral da aplicação; no HTML está no cabeçalho de revisão.

## Prévias e estados

**Proposta, não cena Unity implementada.** O [protótipo HTML](prototypes/biologia/index.html?lab=SLUG) usa diagramas esquemáticos. Header, navegação de revisão e comandos tracejados de simular pega/giro são externos ao laboratório; não autorizam novos painéis na Unity. Os controles sólidos representam controles da sala. A ficha aparece somente por seleção. O estado `detail` é exemplo de consulta; `action` exemplifica manipulação/resultado; `error` é exemplo visual do aviso, não execução automática de uma falha real.

![Disposição espacial proposta](../GeneratedMedia/GDD/biologia/layout.png)

Visão espacial comum: entrada, suporte elevado, faixa de circulação e consulta lateral. A disposição específica é descrita acima.

![Entrada](../GeneratedMedia/GDD/biologia/SLUG-entry.png)

Entrada: R1, C1 e S1 visíveis; Q1 e X1 ocultos. Os objetos são a ação principal.

![Interação e resultado](../GeneratedMedia/GDD/biologia/SLUG-action.png)

Manipulação/resultado: C1 altera os objetos; E1 confirma a ação. Fechar Q1 mantém o experimento. Não há interface sobre a câmera.

![Consulta opcional](../GeneratedMedia/GDD/biologia/SLUG-detail.png)

Consulta: selecionar objeto ou símbolo da base abre Q1 lateral; Fechar remove o bloqueio visual sem resetar estado.

![Recusa ou limite](../GeneratedMedia/GDD/biologia/SLUG-error.png)

Impedimento: E1 descreve a recuperação sem modal. O estado válido anterior é preservado.

![Restauração](../GeneratedMedia/GDD/biologia/SLUG-reset.png)

Confirmação: X1 bloqueia manipulação e navegação, Cancelar preserva tudo; Restaurar retorna à entrada. Só existe onde há restauração destrutiva.

Capturas locais ignoradas em `GeneratedMedia/GDD/biologia/`. Para compartilhar o GDD, enviar também essas imagens; elas não acompanham Git. Fonte e reprodução em [README do protótipo](prototypes/biologia/README.md).

## Execução e aceite

Modo inicial individual offline; textos, malhas, texturas e metadados embarcados. Sem API em runtime. LAN não implementada: a infraestrutura atual só dá autoridade coletiva ao lançamento oblíquo; não anunciar Biologia multiplayer.

- Completar o fluxo descrito, incluindo seleção, manipulação, consulta, correção e restauração/cancelamento.
- Rótulos e controles correspondem ao inventário; objetos e área de pega ficam legíveis durante consultas.
- Ocultar um objeto cancela foco e interação; não manter colisores ou seleção invisíveis. Voltar ao Hub não exige navegar por telas de estudo.
- Restaurar respeita estado autoral de cada peça; a preferência reduzida não altera o conteúdo científico.
- Compilação, testes de domínio e captura no Editor serão exigidos na implementação. Alcance, conforto e desempenho em headset são etapas separadas. Nenhum build sem autorização.
'''

def write():
    for x in items:
        text=f'''# Short GDD — {x['name']}

**Status:** proposta para revisão, não implementada. **Disciplina:** Biologia. **Modo:** individual offline. **Público:** Ensino Médio. **Data:** 17/11/2026. Uma das **seis** salas propostas, conforme [índice](gdd-biologia.md).

## Objetivo e experiência

{x['goal']}

## Acervo, objetos e sala

**Cenas antigas consolidadas:** {x['old']}.

**Reaproveitamento comprovado por arquivos:** {x['assets']}

**Catálogo fechado desta entrega:** {x['catalog']}

| Conjunto de objetos | Papel | Interação | Limite |
|---|---|---|---|
| Modelos e peças descritos no catálogo | Fenômeno principal | Seleção direta, pega/rotação ou encaixe conforme o fluxo abaixo | Não disponibilizar partes sem identificação revisada |
| Suportes e sockets da atividade | Ancoragem, retorno e alcance | Controles 3D e filtros de encaixe | Preservar referência espacial e evitar seleção concorrente |
| Consulta e navegação | Apoio opcional | Apontar/tocar e fechar; voltar ao Hub | Uma consulta simultânea; sem HUD na câmera |

**Disposição proposta:** {x['space']}

**Controles escolhidos pela tarefa:** {x['controls']}

## Inventário de interfaces, rótulos e controles 3D

**HUD na câmera: nenhum.** Uma ficha opcional de consulta. Os nomes ficam nos suportes e junto à estrutura selecionada; nenhum cartão extenso acompanha uma peça na mão. A sala não repete o formulário de Física.

| ID | Tipo e localização | Finalidade e ação | Inicial | Abre/fecha | Simultâneo |
|---|---|---|---|---|---|
| R1 | Nomes nos suportes e legenda junto ao modelo | Identificar o objeto e informar ampliação didática; rótulos extras só quando solicitados | Nomes visíveis; extras ocultos | Toggle quando houver; seleção realça uma estrutura | C1, S1; Q1 pode consultar a selecionada |
| C1 | Controles 3D no suporte e objetos da atividade | {x['controls']} | Visíveis; estado inicial da atividade | Pega/toque/raio, evento de domínio | R1, E1, Q1 e S1; bloqueados por X1 |
| Q1 | Ficha clara fixa à lateral do suporte | Nome, função, contexto, limite do modelo; Fechar | Oculta | Selecionar uma parte ou símbolo de consulta; Fechar ou trocar alvo fecha/substitui | Objetos e C1 permanecem visíveis; uma Q1 por vez |
| E1 | Uma linha no suporte, sem painel flutuante | Confirmar ação ou explicar recusa/limite; não depender apenas de cor | Instrução inicial curta | Atualiza por evento; erro persiste até correção ou outra ação pertinente | R1, C1 e Q1 |
| X1 | Confirmação no espaço da consulta | Cancelar/Restaurar quando houver perda de montagem/população | Oculta | Ação Restaurar; decisão fecha | Objetos ao fundo visíveis; C1, Q1 e S1 sem interação |
| S1 | Voltar ao Hub, junto à saída e ao suporte | Retorno padrão à disciplina, sem confirmação se não houver estado relevante | Visível | Permanente | Todos, exceto bloqueio por X1 |

O cabeçalho de revisão e a seção “Como isso funciona em RV?” do HTML não são interfaces a implementar na sala. Motion reduzido é preferência geral do produto, exposta no menu padrão; não exige outro painel permanente de Biologia.

## Sequência e casos sem resultado

1. Entrar, reconhecer as peças e a primeira ação em E1.
2. {x['action']}
3. Fechar a consulta, comparar outro caso e retornar ao estado de exploração.
4. Restaurar quando aplicável: X1 pede confirmação; cancelar preserva os objetos; S1 retorna ao Hub.

**Recusas e recuperação:** {x['failure']}

## Modelo científico e limites

{x['model']}

Referência de revisão: [OpenStax — conteúdo relacionado]({x['source']}). Os modelos artísticos precisam de validação semântica; nomes de arquivos não são autoridade taxonômica/anatômica. Reutilização técnica não substitui revisão do conteúdo.

**Critério específico:** {x['verify']}

'''+common.replace('SLUG',x['id'])
        # Anatomy has no destructive restore; gallery content can be switched without reset.
        if x['id']=='anatomia':
            text=text.replace('![Restauração](../GeneratedMedia/GDD/biologia/anatomia-reset.png)\n\nConfirmação: X1 bloqueia manipulação e navegação, Cancelar preserva tudo; Restaurar retorna à entrada. Só existe onde há restauração destrutiva.','**X1 não se aplica à Anatomia:** a seleção de sistemas e o fechamento de consultas não destroem montagem. Não criar modal de reset por convenção.')
            text=text.replace('| X1 | Confirmação no espaço da consulta | Cancelar/Restaurar quando houver perda de montagem/população | Oculta | Ação Restaurar; decisão fecha | Objetos ao fundo visíveis; C1, Q1 e S1 sem interação |','| X1 | Não se aplica | Não há restauração destrutiva nesta sala | Não existe | — | — |')
        (ROOT/f'docs/gdd-biologia-{x["id"]}.md').write_text(text,encoding='utf-8')
if __name__=='__main__':write()
