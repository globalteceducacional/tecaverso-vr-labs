# Tecaverso Labs

Laboratórios educacionais em realidade virtual para investigar conceitos por meio de experimentos interativos, visualização de grandezas e comparação de resultados.

Este README reúne o **Game Design Document (GDD)** e as orientações de entrada no projeto. O primeiro experimento funcional é **Física / Lançamento oblíquo**.

**GDD 1.1 · Atualizado em 06/10/2026 · Idioma principal: pt-BR.**

> Este documento distingue implementação de planejamento. **Atual** descreve código/cenas e verificações registradas; **Planejado** define o design a implementar. Nenhum build foi gerado nesta revisão.

## Sumário

- [1. Visão e objetivos](#1-visão-e-objetivos)
- [2. Estado atual](#2-estado-atual)
- [3. Público, plataformas e escopo](#3-público-plataformas-e-escopo)
- [4. Experiência e estrutura educacional](#4-experiência-e-estrutura-educacional)
- [5. Navegação e telas](#5-navegação-e-telas)
- [6. Interação XR e acessibilidade](#6-interação-xr-e-acessibilidade)
- [7. Direção visual, áudio e feedback](#7-direção-visual-áudio-e-feedback)
- [8. Catálogo de disciplinas](#8-catálogo-de-disciplinas)
- [9. Lançamento oblíquo](#9-lançamento-oblíquo)
- [10. Multiplayer LAN](#10-multiplayer-lan)
- [11. Dados e persistência](#11-dados-e-persistência)
- [12. Arquitetura e organização](#12-arquitetura-e-organização)
- [13. Desempenho e validação](#13-desempenho-e-validação)
- [14. Roadmap e critérios de aceite](#14-roadmap-e-critérios-de-aceite)
- [15. Decisões, riscos e dependências](#15-decisões-riscos-e-dependências)
- [16. Abrir, executar e contribuir](#16-abrir-executar-e-contribuir)
- [17. Referências e manutenção](#17-referências-e-manutenção)

## 1. Visão e objetivos

### Conceito

O Tecaverso Labs é um conjunto de salas virtuais de investigação científica. O aluno escolhe uma disciplina, manipula variáveis e relaciona o que observa às representações matemáticas e conceituais.

O ambiente é um laboratório tecnológico de aprendizagem, não uma campanha narrativa. As salas compartilham identidade visual; a instrumentação muda conforme o experimento.

### Pilares

1. **Experimentar para compreender:** prever, manipular, observar, comparar e explicar.
2. **Ciência legível:** unidades, hipóteses do modelo, vetores e resultados explícitos.
3. **VR com propósito:** presença espacial e manipulação ajudam a interpretar o fenômeno; efeitos não escondem informação.
4. **Uso escolar offline:** experiência individual sem login e colaboração pela rede local, sem serviços de nuvem obrigatórios.
5. **Expansão sustentável:** reutilizar salas, componentes e dados; não criar uma arquitetura diferente por laboratório.

### Objetivos de experiência

- Aluno: iniciar uma atividade com pouca orientação, reconhecer o que pode alterar e explicar uma relação entre ação e resultado.
- Professor: conduzir uma sessão local sem administrar contas ou infraestrutura online.
- Desenvolvimento: incorporar novos experimentos com um contrato pequeno de sessão, apresentação e conteúdo educativo.

Não haverá ranking, recompensa por velocidade ou pontuação competitiva. O sucesso é a compreensão e a qualidade da investigação, não o número de disparos.

## 2. Estado atual

| Área | Atual | Ainda falta |
|---|---|---|
| Entrada | `Hub.unity`: explorar sozinho, criar sala e entrar por IP no painel principal | Configurações e tutorial XR |
| HUB 1 | Quatro disciplinas com ícones Figma e navegação | Papéis de anfitrião/participante |
| HUB 2 | Seis conteúdos de Física; anfitrião seleciona o conteúdo da sala | Expandir os conteúdos sincronizados |
| Demais disciplinas | Mockups navegáveis de Química, Biologia e Matemática | Simulações e atividades educativas |
| Ambiente | Sala padrão reutilizável e laboratório com acabamento equivalente | Consolidar recursos compartilhados fora de Física |
| Experimento | Parâmetros, simulação, pausa/reset, vetores, trajetória, réguas e análise | Precisão temporal nos extremos, roteiro e refinamentos de leitura |
| Histórico | Até cinco trajetórias e marcadores de ápice esmaecidos | Comparação identificada e amostras completas por registro |
| LAN | Sala, pronto, entrada coletiva e estado do lançamento oblíquo via UDP direto | Validar headsets físicos, oito dispositivos e Wi-Fi escolar |
| Educação | Estrutura e objetivos decididos neste GDD | Implementar atividades, feedback e conclusão |
| Qualidade | Verificações recentes no Editor/Play Mode | Conforto, desempenho e rede em headsets reais |

**Autoridade atual:** no modo LAN, somente o anfitrião ajusta parâmetros e dispara, pausa ou reseta. Participantes observam a mesma simulação; exibir vetores e alternar abas são preferências locais. Transferência de controle ainda é planejada.

O histórico atual preserva linhas e ápices; as cópias temporais são limpas a cada novo disparo. Não existe progresso educativo persistente.

## 3. Público, plataformas e escopo

### Público definido

- Primário: alunos do Ensino Médio, em atividades mediadas por professor.
- Secundário: introdução científica no Ensino Superior e exploração individual.
- Outros níveis podem receber conteúdo específico sem mudar a experiência-base.
- Primeira versão educativa em português brasileiro. Preparar textos para localização; tradução completa fica fora do MVP.

### Plataformas e equipamento

- Alvo de produto: headset standalone Android compatível com OpenXR e controladores rastreados.
- Desenvolvimento e validação auxiliar: Unity Editor e Windows com XR compatível.
- XR Interaction Simulator é ferramenta de desenvolvimento, não uma versão desktop educacional pronta.
- Hand tracking é complementar; nenhuma ação essencial depende dele.
- Premissa de desempenho: 72 Hz no dispositivo mínimo homologado. É uma meta de planejamento, não resultado medido.
- O modelo do headset, sistema e configuração gráfica serão registrados quando o equipamento estiver disponível. A homologação física não pode ser substituída por uma decisão documental.

### Modos de uso

- **Exploração individual:** livre, offline e sem cadastro.
- **Atividade guiada:** hipótese, investigação e conclusão; planejada.
- **Sala LAN:** grupo conduzido por anfitrião; planejada para os experimentos.

### Fora do escopo inicial

Multiplayer pela Internet, Relay/Lobby online, contas obrigatórias, voz integrada, migração automática de anfitrião, integração LMS, notas oficiais, ranking, mundo aberto, avatares complexos, editor de experimentos pelo aluno e resistência do ar no lançamento oblíquo.

Monetização, publicidade e loja não fazem parte do escopo funcional. Distribuição comercial, se necessária, será uma decisão separada.

## 4. Experiência e estrutura educacional

### Ciclo principal

**Escolher → compreender o objetivo → prever → experimentar → observar → comparar → explicar → concluir ou explorar novamente.**

Atividades guiadas terão duração-alvo de 10–15 minutos, com pausas livres e sem limite punitivo. Exploração livre não exige terminar um roteiro.

### Estrutura de uma atividade

1. **Contexto:** uma pergunta curta sobre o fenômeno.
2. **Objetivo:** o que o aluno deverá conseguir explicar.
3. **Modelo:** hipóteses e simplificações relevantes.
4. **Previsão:** escolher ou registrar uma hipótese antes de manipular.
5. **Investigação:** alterar poucas variáveis e observar tentativas.
6. **Comparação:** evidenciar o que mudou e o que foi mantido constante.
7. **Conclusão:** resposta curta/seleção conceitual com explicação.

Usar cartões breves. Evitar longos textos no headset, digitação extensa e dependência de reconhecimento de fala.

### Feedback e progressão

- Hipóteses incorretas recebem explicação e oportunidade de repetir, sem punição.
- Concluir exige realizar a comparação proposta e responder à pergunta central; assistir à animação não basta.
- Estados: Não iniciada, Em andamento e Concluída, associados à atividade.
- Na sessão coletiva, o experimento é compartilhado, mas respostas e conclusão são individuais. Respostas não bloqueiam os demais participantes.
- O anfitrião poderá visualizar um resumo voluntário de participação, sem ranking.
- Conteúdo deve receber revisão científica/pedagógica antes de distribuição educativa. Não declarar alinhamento curricular formal sem essa revisão.

### Primeira atividade: “O que muda o alcance?”

**Pré-requisitos:** posição, velocidade, unidades e leitura básica de ângulos.

**Objetivos:** decompor a velocidade, reconhecer a aceleração vertical, relacionar ângulo/velocidade/gravidade ao alcance e explicar a independência da massa no modelo ideal.

- Observar Vx e Vy e pausar perto do ápice.
- Com altura zero, comparar 30°, 45° e 60°, mantendo velocidade e gravidade.
- Comparar massas de 1 e 10 kg com as outras condições iguais.
- Comparar gravidades de 5 e 20 m/s².
- Explicar por que as trajetórias mudaram ou permaneceram iguais.

**Conclusões esperadas:** Vx constante; Vy variável e nulo no ápice; massa não altera a trajetória sem arrasto. Apresentar alcance máximo a 45° somente para alturas de lançamento e chegada iguais.

## 5. Navegação e telas

### Fluxo individual planejado

Entrada → Explorar sozinho → HUB 1 / Disciplinas → HUB 2 / Conteúdos → Apresentação do experimento → Exploração livre ou atividade guiada → Resumo opcional → HUB 2.

A apresentação é um painel no laboratório, não uma nova cena. Pode ser dispensada após a primeira visita.

### Fluxo coletivo planejado

- Anfitrião: Entrada → Criar sala → Sala de espera → Selecionar conteúdo → Participantes prontos → Iniciar para todos.
- Participante: Entrada → Entrar por IP → Sala de espera → Aguardar anfitrião → Experimento.
- Anfitrião é um papel operacional; não exige conta de professor.
- Selecionar conteúdo não carrega a cena imediatamente nos clientes. A ação “Iniciar para todos” é explícita.

### Inventário de interfaces

| Interface | Conteúdo e ações | Situação |
|---|---|---|
| Entrada | Explorar sozinho, criar sala, entrar em sala | Implementada; configurações pendentes |
| Criar sala | Apelido, nome da sala, criar/cancelar | Implementado |
| Entrar na sala | Apelido, IPv4, exemplo, conectar/cancelar | Implementado |
| Sala de espera | IP, participantes, capacidade, pronto, sair | Implementada |
| HUB 1 | Quatro disciplinas | Atual; integrar papéis |
| HUB 2 | Conteúdos, descrição, disponibilidade e iniciar | Atual; integrar papéis |
| Parâmetros/Análise | Controles, fórmulas e resultados | Atual no lançamento oblíquo |
| Menu do experimento | Pausa, ajuda, visualização e voltar ao Hub | Parcial; consolidar |
| Roteiro/resumo | Objetivo, etapas e conclusão | Planejado |
| Modais transversais | Conexão, falha, sala cheia, versão incompatível, perda de host e sair | Implementados para LAN; expandir para outras ações |

A UI LAN lateral herdada está desativada, preservada como referência. O painel principal reutiliza os HUBs existentes, sem criar uma cena Unity para cada tela.

### Regras de navegação

- Voltar nunca fecha a aplicação inesperadamente. Sair da aplicação exige confirmação.
- Voltar ao Hub confirma perda de registros não exportados.
- Participante sai individualmente; anfitrião recebe aviso de que encerrar desconecta todos.
- Manter disciplina/conteúdo selecionados ao retornar. Nova atividade começa sem estado transitório da anterior.
- Bloquear cliques repetidos durante carga/conexão. Cancelamento deve retornar a estado consistente quando tecnicamente seguro.
- Erros indicam uma ação concreta: corrigir IP, tentar novamente, voltar ou desconectar.
- Mockups abrem detalhes, mas nunca carregam cenas inexistentes. Exibir “Em breve” no botão desabilitado.

### Complementos no Figma

Criar entrada, modais LAN e sala de espera; adicionar variantes de anfitrião/participante, carregamento, falha e desconexão. Usar componentes e variantes, evitando cópias divergentes. Prever normal, hover, pressionado, selecionado, desabilitado e foco.

## 6. Interação XR e acessibilidade

### Interação

- Apontar e pressionar gatilho é o caminho principal; interação próxima é complementar.
- Operação com qualquer mão; nenhuma função essencial exige duas mãos simultaneamente.
- Sliders têm área de interação maior que a linha visível.
- Adicionar botões − / + para precisão, respeitando os limites e incrementos de cada parâmetro.
- Instruções devem refletir o dispositivo. Teclado/mouse permanecem fallback de desenvolvimento.
- Experimentos laterais apresentam movimento da esquerda para a direita. Interfaces voltadas ao observador; ferramentas não podem reorientar painéis autorais indiscriminadamente.

### Conforto — design-alvo

- Posição inicial útil, modo sentado/em pé e recentralização acessível.
- Teleporte e giro em passos como padrão; deslocamento contínuo opcional.
- Não mover a câmera por recoil, impacto, tween ou troca de tela; não usar camera shake.
- Painéis estáveis no espaço, ajustáveis em altura/distância; evitar acompanhamento permanente da cabeça.
- Separar área de observação e objeto experimental. Não exigir caminhada física para acompanhar toda a trajetória.
- Pausa, ajuda e saída acessíveis durante a atividade.

### Legibilidade e inclusão

- Não identificar grandezas somente por cor: incluir símbolo, nome e unidade.
- Oferecer escala de texto, volume, intensidade de feedback e animações reduzidas.
- Informação sonora também deve ter representação visual.
- Separar visibilidade de trajetória, cópias, vetores, valores e gravidade; manter o toggle global como atalho.
- Priorizar rótulos da amostra selecionada para evitar sobreposição; reduzir detalhes das demais sem apagar seus dados.
- Validar leitura e alcance dos controles sentado/em pé no headset-alvo. Fidelidade ao Figma não comprova conforto em VR.

## 7. Direção visual, áudio e feedback

### Ambiente padrão

Painéis perolados, estruturas grafite, rodapés azuis, faixas luminosas azuis e piso escuro quadriculado. Laboratório organizado, sem cenografia competindo com informações científicas.

Prefab: `Assets/_Project/Environments/StandardRoom/TecaversoStandardRoom.prefab`, 14 × 14 m e 7 m de altura, com geometria e duas luzes sem sombra; não contém rig ou UI.

O laboratório balístico mantém dimensões maiores para acomodar a trajetória. **Padronizar estilo não significa impor o mesmo tamanho a todas as salas.**

### UI

- Montserrat Regular, Bold e ExtraBold, com assets TMP e licença OFL.
- Gotham Ultra aparece na coleção Figma, mas não é utilizada/importada nas telas atuais. Só usar mediante arquivo/licença apropriados.
- Azul primário `#284EA0`; contorno `#07386F`; superfície `#D3D1E8`; fundo/inset `#E8E6FE`; texto `#161616`.
- Física azul, Química roxa, Biologia verde, Matemática laranja.
- Preservar ícones originais e proporções. Screenshots de telas não são assets de UI.
- UGUI/TMP espacial com comportamentos funcionais; adaptar prompts de desktop a XR.

### Feedback e áudio

- Atual: DOTween em botões/sliders e pulso do canhão ao disparar.
- Planejado: sinais breves de foco, confirmação, indisponibilidade e conclusão.
- Hover/press não deve deslocar outros controles nem comprometer a região clicável.
- Animação do canhão afeta somente o visual; não desloca origem, medidas ou parâmetros.
- Sons discretos de interface, disparo e conclusão; ambiente opcional e baixo, sem mascarar a fala presencial.
- Haptics leves e opcionais na confirmação, não vibração contínua de hover.
- UI usa tempo independente da física. Pausar não congela menus.
- Cancelar/restaurar tweens ao desativar ou resetar.
- Sem voz integrada no MVP; não habilitar Vivox como solução para LAN offline.

## 8. Catálogo de disciplinas

Catálogo atual: `Assets/_Project/Hub/HubCatalog.asset`. Caminho de cena vazio significa mockup. Evoluir para identificadores estáveis, evitando dependência do índice visual.

| Disciplina | Conteúdos | Disponibilidade |
|---|---|---|
| Física | Movimento e forças; Eletricidade; Óptica; Termodinâmica; Ondas; Gravitação | Somente lançamento oblíquo é funcional |
| Química | Estrutura da matéria; Reações químicas; Soluções | Mockups |
| Biologia | Células; Corpo humano; Ecossistemas | Mockups |
| Matemática | Geometria; Funções; Medidas | Mockups |

“Movimento e forças” é uma categoria, não uma declaração de que atrito e leis de Newton já foram implementados. A descrição deve refletir o conteúdo realmente disponível.

Novo experimento exige objetivo educativo, modelo científico, controles, visualizações, roteiro mínimo, configuração/prefab, integração ao Hub e critérios de validação.

## 9. Lançamento oblíquo

### Objetivo e cenário

Relacionar condições iniciais com trajetória, velocidade, alcance e altura máxima. Canhão sobre base telescópica, painel de parâmetros/análise e instrumentos espaciais. Base acompanha altura; canhão acompanha ângulo.

### Parâmetros

| Parâmetro | Limites | Interação definida |
|---|---|---|
| Ângulo θ | 0°–90° | Passo de 1° |
| Velocidade v₀ | 0–30 m/s | Somente inteiros |
| Altura inicial h₀ | 0–10 m | Fracionária; passo-alvo 0,1 m nos botões |
| Massa m | 1–10 kg | Fracionária; passo-alvo 0,1 kg nos botões |
| Gravidade g | 5–20 m/s² | Fracionária; passo-alvo 0,01 m/s² nos botões |

Defaults atuais: 45°, 18 m/s, 2 m, 3 kg, 9,81 m/s². Os botões incrementais são planejados; sliders fracionários atuais são contínuos.

Parâmetros bloqueados durante voo/pausa. Finalizar ou resetar antes de reconfigurar. Roteiros poderão travar parâmetros específicos, indicando o motivo.

### Modelo científico

- Movimento ideal em plano vertical, gravidade uniforme e solo plano em y = 0.
- Sem arrasto, vento, rotação ou quique.
- Origem horizontal na base/eixo do canhão, não na boca. Origem vertical em h₀; comprimento/animação do cano não altera condições iniciais.
- Massa não altera a trajetória. Explicar isso na UI e usar a comparação como atividade.
- Rigidbody é representativo; trajetória calculada analiticamente, não por forças no corpo.

Equações, com x₀ na base do canhão:

```text
vx = v₀ cos(θ)
vy(t) = v₀ sen(θ) − g t
x(t) = x₀ + vx t
y(t) = h₀ + v₀ sen(θ) t − ½ g t²
|v(t)| = √(vx² + vy(t)²)
tápice = v₀ sen(θ) / g
Ymáx prevista = h₀ + [v₀ sen(θ)]² / (2g)
timpacto = [v₀ sen(θ) + √([v₀ sen(θ)]² + 2g h₀)] / g
Rtotal = vx timpacto
```

Altura máxima prevista é diferente de altura máxima atingida até agora. A UI atual usa a previsão; a revisão deve nomeá-la e, quando útil, apresentar também a altura atingida.

### Estados e comandos

- **Idle:** editar parâmetros, projétil na origem, vetores dinâmicos ocultos.
- **Running:** tempo progride, atualizando projétil, linha e amostras.
- **Paused:** tempo/amostragem congelados, menus disponíveis.
- **Complete:** impacto encerra o movimento; registros permanecem e novo disparo é permitido.
- **Disparar:** criar registro com parâmetros imutáveis daquela tentativa.
- **Pausar/Retomar:** preservar tempo exato, sem salto.
- **Resetar:** limpar disparos/medidas e voltar à origem, mantendo parâmetros configurados.
- Restaurar parâmetros padrão, se oferecido, é uma ação distinta de resetar.

### Representações

- LineRenderer acompanha o percurso temporal, sem fechar um laço nem retornar do projétil à origem.
- Vetores: resultante, Vx, Vy e gravidade, com valores/unidades.
- Haste cilíndrica extensível e ponta cônica fixa, unidas sem espaço.
- Escala das setas é amplificação didática configurável, não escala física de distância. O multiplicador visual atual de 2× não significa “1 m/s = 2 m”.
- Vetor nulo não deve manter seta residual; Vy = 0 no ápice continua legível no rótulo.
- Alcance medido desde a projeção da base no solo; alturas desde y = 0.
- Arco de ângulo e régua de altura no mesmo plano Z do canhão.
- Não restaurar gráfico Y × t separado atrás do experimento: a representação principal solicitada é a trajetória espacial. Análise mantém fórmulas e métricas.

### Histórico

- Máximo de cinco disparos, incluindo atual; o sexto remove o mais antigo.
- Opacidades-alvo: 100%, 85%, 70%, 55%, 40%.
- Linha, ápice e elementos associados seguem a mesma regra; evitar multiplicação acidental de alpha entre material/vértice.
- Marcador de ápice no instante/posição exatos, em destaque sem alterar a física.
- Cópias a cada 0,5 s com vetores daquele instante, sem geração durante pausa.
- **Atual:** linhas/ápices persistem; amostras temporais são reaproveitadas para o disparo atual.
- **Planejado:** registro contém também amostras e parâmetros. Exibir detalhes do registro selecionado e linhas/ápices dos cinco como contexto, reduzindo poluição visual e custo.
- Identificar tentativas por número; permitir selecionar/comparar duas. Alterar sliders não modifica resultados anteriores.
- Reset limpa todas as tentativas da sessão de laboratório.

### Precisão e casos de borda

- Corrigir impacto para timpacto exato. Hoje o cruzamento do solo é detectado por frame e y limitado a zero, podendo exceder o alcance horizontal.
- Amostrar nos tempos exatos dos intervalos, não no primeiro frame após o limiar.
- Validar v₀ = 0, θ = 0°/90°, h₀ = 0 e gravidades extremas.
- Impacto imediato termina sem atraso, quique ou deslocamento artificial.
- Unidades e formatação pt-BR consistentes.
- Ambiente acomoda toda a faixa de parâmetros; cenografia não interrompe a trajetória ideal.

## 10. Multiplayer LAN

### Papéis e capacidade planejados

- Um anfitrião e até sete participantes: **meta de oito dispositivos**, condicionada a teste; não capacidade certificada.
- Anfitrião escolhe atividade, inicia cena, pausa/reseta e encerra.
- Um operador manipula o experimento por vez; por padrão o anfitrião.
- Controle pode ser concedido/revogado a um participante.
- Observadores ajustam sua visualização e abrem análise localmente, sem alterar a física coletiva.
- Não implementar edição simultânea de sliders no MVP.

| Ação | Anfitrião | Operador delegado | Observador |
|---|---|---|---|
| Escolher/iniciar conteúdo para todos | Sim | Não | Não |
| Alterar parâmetros e disparar | Quando detém controle | Quando autorizado | Não |
| Pausar/retomar/resetar | Sim | Se a atividade autorizar explicitamente | Não |
| Conceder/revogar controle | Sim | Não | Pode solicitar |
| Ajustar visualização/volume local | Sim | Sim | Sim |
| Encerrar a sala | Sim | Não | Pode sair individualmente |

### Arquitetura de rede

**Implementação de 06/10/2026:** `LanLobbyController` gerencia aprovação, versão, limite de oito participantes e estado pronto. `LanExperimentSession` carrega a apresentação do laboratório aditivamente em cada dispositivo, mantendo transporte, avatares e rig XR do Hub. Os objetos didáticos não são `NetworkObject`; não adicionar objetos de rede à cena sem revisar esse ciclo. O rig recebe a posição de entrada já definida no laboratório e recupera sua pose do Hub ao retornar; os transforms salvos não são alterados.

`ProjectileLanSync` envia snapshots confiáveis limitados a cinco lançamentos, a 10 Hz, com revisão de reset e sequência. Clientes reconstroem linhas, ápices, amostras, vetores e métricas pelas equações existentes. Há extrapolação máxima de 250 ms; pausa e fim usam o tempo autoritativo. Controles ficam bloqueados até os participantes confirmarem a carga. O anfitrião pode voltar todos à sala; participantes podem sair individualmente. Não há transferência de controle, descoberta automática ou migração de host.

O template atual usa UnityTransport direto e conserva DistributedAuthority em **DAHost local**, sem CMB/Relay. Isso não significa nuvem.

Preservar a conexão existente e implementar **autoridade lógica do anfitrião sobre o experimento**, validando remetente, permissão, versão e limites dos comandos. Não migrar cegamente a topologia do template.

Para lançamento oblíquo, replicar estado compacto e comandos, não cada seta a cada frame:

- Identificação de sala, experimento e versão de protocolo.
- Operador autorizado e revisão do estado.
- ID de lançamento, parâmetros imutáveis, origem e tempo inicial.
- Estado Running/Paused/Complete e tempo acumulado de pausa.
- Histórico limitado, reset e etapa educativa compartilhada.

Clientes reconstroem pela mesma equação e relógio de sessão. Anfitrião confirma transições e envia snapshots para correção/entrada tardia. Não exigir determinismo bit a bit entre plataformas; estabelecer tolerância nos testes.

Mensagens confiáveis para comandos, permissões e transições; avatares mantêm mecanismo apropriado do template.

### Ciclo da sessão e falhas

- Estados-alvo: Offline, Criando, Conectando, Sala de espera, Carregando, Em experimento, Desconectando e Falha.
- Criar/entrar não inicia conteúdo automaticamente.
- Iniciar para todos exige participantes incluídos prontos. Durante carga, aguardar confirmação; oferecer remover quem falhou ou cancelar.
- Entrada tardia recebe snapshot e entra como observador, sem reiniciar a atividade.
- Saída do operador revoga permissão e devolve controle ao anfitrião.
- Perda do anfitrião encerra sessão coletiva, explica o ocorrido e volta à entrada; sem migração automática.
- Reconexão explícita recebe novo snapshot e não recupera autoridade antiga automaticamente.
- Versão incompatível é rejeitada antes de iniciar a atividade.
- Timeouts-alvo configuráveis: 10 s para conexão; 30 s de carga antes de oferecer recuperação. Ajustar com evidência real, evitando esperas indefinidas.

### Operação da conexão atual

- Mesma rede e versão do aplicativo; porta padrão UDP 7777.
- Host escuta em 0.0.0.0; cliente usa IPv4 da rede compartilhada, como 192.168.1.10.
- 127.0.0.1 somente para processos no mesmo computador.
- Validação atual aceita IPv4 privado/link-local/loopback; rejeita DNS, IPv6, IP público e IP:porta.
- Havendo VPN/várias interfaces, escolher a interface compartilhada.
- Erros devem orientar sobre possível isolamento Wi-Fi/firewall; nenhuma regra de firewall é alterada automaticamente.
- Não abrir portas do roteador nem exigir autenticação Unity Cloud.
- Descoberta automática depois de estabilizar entrada manual.
- Sem PIN/senha ou criptografia de aplicação adicional no MVP; usar redes controladas. “LAN” não deve ser apresentada como garantia de segurança.
- Vivox desativado. Dependências online do template permanecem instaladas, mas não são usadas pelos novos fluxos LAN.

## 11. Dados e persistência

### Política definida

- Atual: tentativas transitórias, sem progresso educativo persistente.
- Padrão planejado: sessão anônima em memória, sem login, analytics remoto ou captura de voz.
- Persistir preferências locais: idioma, volume, locomoção, escala de texto e animações reduzidas.
- Progresso individual opcional por perfil/apelido local, sem nome civil obrigatório.
- Disponibilizar apagar dados e iniciar nova sessão em dispositivo compartilhado.
- Não gravar IPs, apelidos ou respostas em logs por padrão; diagnóstico mínimo e exportação consciente.
- Resetar tentativas, apagar perfil e restaurar configurações são ações distintas.

### Registro planejado

ID/versão do experimento e modelo, ID de tentativa, parâmetros com unidades, tempo de impacto, alcance, altura máxima, amostras relevantes e respostas da atividade.

Separar definições imutáveis de estado da sessão. Não salvar GameObjects como resultados científicos.

### Exportação

Depois de estabilizar o roteiro, oferecer CSV de tentativas e JSON de sessão, localmente e por ação explícita. Mostrar destino e resultado. Não enviar automaticamente ao professor ou serviço externo.

Exportação e recuperação de progresso ainda não existem.

## 12. Arquitetura e organização

### Stack atual

Unity 6000.3.25f1, URP, OpenXR, XR Interaction Toolkit, Input System, UGUI/TMP, DOTween, Netcode for GameObjects e UnityTransport. ProBuilder/ferramentas de Editor apoiam autoria; não reconstruir salas em runtime.

Versões efetivas em `ProjectSettings/ProjectVersion.txt`, `Packages/manifest.json` e `Packages/packages-lock.json`. Não atualizar pacotes incidentalmente em tarefas de UI.

### Estrutura existente

```text
Assets/
  _Project/
    Core/Networking/           # LanAddress; assembly Tecaverso.Core
    Editor/                    # Autoria de cenas, UI e modelos
    Environments/StandardRoom/ # Prefab e malha compartilhada
    Hub/                       # Cena, UI, catálogo, navegação
    Laboratories/
      Physics/ObliqueLaunch/   # Cena, scripts, materiais e modelos
      Chemistry/              # Reservada
      Biology/                # Reservada
      Mathematics/            # Reservada
    UI/                        # Fontes, ícones, identidade
    Shared/                    # Compartilhados em consolidação
    Tests/                     # EditMode e PlayMode
  ThirdParty/VRMultiplayer/    # Template adaptado para LAN
  Samples/                     # Dependências importadas de XR
  XR/ e XRI/                  # Configurações dos pacotes
```

Manter **Assets/_Project/**, não Assets/Tecaverso/. A arquitetura educacional a seguir é planejada, não lista de classes existentes.

### Responsabilidades

- Domínio: parâmetros, equações, amostras, regras educativas e validação.
- Aplicação: comandos, sessão, navegação e coordenação.
- Apresentação: painéis, vetores, modelos, áudio e tweens.
- Infraestrutura: LAN, relógio de sessão, armazenamento e carregamento.

Hoje `ProjectileKinematics` calcula, `ObliqueLaunchSimulation` controla tempo/estado, `ObliqueLaunchLab` coordena, e views apresentam. O modelo evita GameObjects, mas usa tipos matemáticos de UnityEngine; não é independente de toda a biblioteca Unity.

### Evolução escolhida

- Evoluir HubCatalog com IDs estáveis e disponibilidade explícita.
- Introduzir definições de experimento/atividade em ScriptableObjects quando o roteiro for implementado.
- Criar contrato pequeno de sessão para iniciar, pausar, retomar, resetar e obter/aplicar snapshot.
- UI envia os mesmos comandos no individual/coletivo; autoridade decide se podem ser executados.
- Separar estado educativo de estado físico: concluir atividade não é tocar o solo.
- Centralizar navegação assíncrona e recuperação de falhas.
- Extrair materiais/helpers/feedbacks compartilhados para pastas apropriadas, preservando GUIDs. Hoje o Hub depende de alguns recursos dentro de Física: dívida técnica conhecida.
- Interfaces nas fronteiras que realmente variam: relógio, persistência, autoridade e carregamento.
- Não criar um framework genérico antes de validar um segundo experimento.

### Padrão de código

SOLID pragmático, KISS, composição, estados explícitos, eventos com inscrições/desinscrições simétricas e configuração por dados. Pooling para instâncias recorrentes; compartilhar recursos quando houver benefício. Evitar Find por frame e novos singletons sem necessidade.

Detalhes em `docs/development-guidelines.md`. Não introduzir padrões ou dependências apenas por antecipação.

### Autoria segura

Cenas são assets autorais a preservar. **Não executar ObliqueLaunchSceneBuilder.Build() para pequenos ajustes:** ele reconstrói o laboratório e pode sobrescrever rig, UI e alterações manuais.

Instaladores de UI/Hub são migrações iniciais e recusam duplicação. Depois, editar objetos/prefabs existentes. “Build” no nome de um menu antigo de autoria não autoriza reconstruir a cena nem gerar executáveis.

## 13. Desempenho e validação

### Metas, não certificações atuais

- Sustentar a frequência configurada do headset mínimo: referência inicial 72 Hz, aproximadamente 13,9 ms por frame, com margem.
- Sem crescimento contínuo de memória após 20 ciclos de disparo/reset e alternância Hub/laboratório.
- Até cinco registros; pooling e detalhamento seletivo limitam textos/transparências/renderers.
- Reutilizar atlas TMP, sprites, malhas e materiais; evitar alocações por frame quando uma atualização por mudança resolver.
- Poucas luzes em tempo real e sombras apenas necessárias. Pós-processamento permanece somente se couber no orçamento medido.
- Conteúdo instalado executa sem rede externa; importação inicial/distribuição de atualizações pode precisar de Internet.

### Validação econômica

- Documentação: coerência, referências locais e diff; não compilar Unity por Markdown.
- Scripts: compilação/console e teste focado no comportamento alterado.
- UI/cena: captura visual e fluxo representativo, sem capturas repetitivas.
- Fórmulas/entrada: testes pequenos de regras críticas, preferencialmente EditMode.
- Rede: smoke test pertinente e dispositivos reais antes de declarar multiplayer pronto.
- **Não gerar builds sem solicitação/autorização explícita do responsável.**

### Matriz mínima de aceite

1. Individual: entrar, selecionar, iniciar, disparar, pausar/retomar, comparar, resetar e voltar.
2. Física: extremos, ápice/impacto exatos e amostragem independente de frame rate.
3. Histórico: sexto disparo remove só o mais antigo; reset limpa tudo; parâmetros anteriores imutáveis.
4. UI: mockups bloqueados, unidades corretas e controles XR acessíveis.
5. LAN: host/cliente reais, permissões, pausa/reset, entrada tardia, perda do operador e do host.
6. Offline: atividades e rede local funcionando sem acesso à Internet.
7. Hardware: leitura, conforto operacional, memória e desempenho no headset-alvo.

### Evidência disponível

- Validação LAN de 01/10/2026: 18 casos de endereço e um teste de transporte loopback, além de host/reconexão no Hub; ver `docs/validation.md`.
- Esse registro inclui build Windows anterior à proibição atual. Não comprova o estado recente nem valida Android/headsets.
- UI/Hub recentes: navegação das quatro disciplinas, bloqueio de mockups e abertura do laboratório com um rig no Editor/Play Mode, sem erros reportados na verificação.
- Em 06/10/2026, o teste `DirectLanReplicatesLobbyLateJoinPauseAndReset` passou com dois NetworkManagers via UDP loopback: sala/pronto, reconstrução de cinco disparos na entrada tardia, observador sem controle, pausa, retomada e reset. O teste isola a replicação; não substitui o ciclo completo em dois dispositivos.
- Host, entrada e retorno ao laboratório foram exercitados no Editor com um único rig/câmera ativo. Não há validação de conforto em headset ou capacidade real de oito dispositivos. Nenhum build novo foi gerado.

## 14. Roadmap e critérios de aceite

Sem datas prometidas. Fechar um fluxo utilizável antes de expandir.

### M0 — Base atual: protótipo entregue

Hub visual, catálogo/mockups, sala padrão, UI Figma e experimento individual. Navegação até o laboratório exercitada no Editor. Não equivale ao aceite educativo, coletivo ou de hardware.

### M1 — Fluxo individual e UX integrada

- Complementos no Figma, entrada/configurações/estados.
- Retorno ao Hub, carga recuperável e confirmações.
- UI LAN integrada e MVP coletivo implementados; configurações e validação de hardware pendentes.
- Legibilidade/controles no headset e redução da sobreposição de rótulos.
- **Aceite:** usuário completa o percurso individual e retorna sem intervenção do Editor ou perda inesperada de configuração.

### M2 — Ciência e primeira atividade

- Impacto/amostras exatos; nomenclatura previsto/atingido.
- Histórico consistente, transparência e identificação de tentativas.
- Roteiro “O que muda o alcance?”, feedback conceitual e conclusão em memória.
- **Aceite:** extremos corretos, cinco registros consistentes e roteiro com comparação/explicação; revisão científica antes de distribuição educativa.

### M3 — Experimento coletivo LAN

MVP implementado e validado em loopback; aceite em dispositivos reais ainda pendente. Delegação de controle permanece planejada.

- Sala de espera, papéis, delegação e carga coordenada.
- Sincronização de lançamento, tempo, pausa/reset, histórico e entrada tardia.
- Falhas de conexão e incompatibilidade.
- **Aceite:** ao menos dois dispositivos reais executam offline com estado coerente e recuperação sem travar. Só anunciar oito após testar oito.

### M4 — Piloto e desempenho

- Sessão acompanhada com professor/alunos e registro de dificuldades observadas.
- Medições no hardware escolhido, com ajuste de conteúdo/UX.
- **Aceite:** orçamento de desempenho e critérios de usabilidade documentados; problemas impeditivos resolvidos.

### M5 — Expansão controlada

- Perfis locais opcionais, exportação e descoberta LAN conforme necessidade.
- Próximo experimento proposto: Óptica/reflexão e refração, sujeito aos resultados do piloto; aproveita visualização espacial sem exigir um grande novo sistema de interação.
- Expandir para outra disciplina após validar contrato comum em dois experimentos.
- **Aceite:** novo conteúdo reutiliza fluxo, sessão e identidade sem duplicar infraestrutura.

## 15. Decisões, riscos e dependências

| Tema | Decisão/mitigação |
|---|---|
| Muitas disciplinas | Consolidar uma atividade educativa antes de multiplicar simulações |
| UI originada em desktop | Adaptar alvos, prompts e distância; validar no headset |
| Multiplayer incompleto | Indicar modo local e bloquear transição indevida até sincronização |
| Conflito de controle | Um operador por vez, autorizado pelo anfitrião |
| Perda de anfitrião | Encerrar com mensagem; sem migração no MVP |
| Custo dos registros | Cinco tentativas, pooling e detalhamento seletivo |
| Dependências do template | Rastrear referências antes de remover/atualizar pacotes e samples |
| Hub acoplado a Física | Extrair compartilhados gradualmente via Unity |
| Precisão aparente | Tempo analítico e nomenclatura científica explícita |
| Ajustes autorais | Alterações pontuais; não regenerar cenas |
| Hardware indisponível | Metas definidas, homologação condicionada a evidência real |
| Custo de validação | Testes por risco e nenhuma build automática |

Decisões firmadas: Ensino Médio como público primário; pt-BR; individual e LAN; oito dispositivos como meta; um operador; autoridade lógica do anfitrião; sem nuvem/voz/migração; atividades curtas; progresso local opcional; sala tecnológica comum; física ideal sem arrasto.

Dependências externas restantes: equipamento para homologação, revisão educativa/científica, licença Gotham caso seja adotada e autorização de builds futuras. As demais escolhas seguem este GDD até revisão deliberada.

## 16. Abrir, executar e contribuir

### Executar o estado atual

1. Usar a versão em `ProjectSettings/ProjectVersion.txt` no Unity Hub. Para desenvolvimento Android, instalar módulos correspondentes; não é necessário gerar build agora.
2. Abrir o repositório e aguardar importação dos pacotes.
3. Abrir `Assets/_Project/Hub/Scenes/Hub.unity` em Play Mode com configuração XR apropriada.
4. Selecionar **Física → Movimento e forças → Iniciar simulação**, sem sessão LAN ativa.
5. Ajustar parâmetros e usar Disparar, Pausar/Retomar, Reiniciar e Parâmetros/Análise.
6. Para trabalho isolado, abrir diretamente a cena ObliqueLaunch.

No painel principal, escolha **Criar sala local**, informe nome e sala e compartilhe o IPv4 exibido. Nos demais dispositivos, escolha **Entrar em uma sala**. Participantes confirmam **Estou pronto**; o anfitrião seleciona Movimento e forças e usa **Iniciar para todos**. Dentro do laboratório, somente ele controla a simulação. A barra LAN permite retornar à sala (anfitrião) ou sair (participante), com confirmação. O modo individual continua disponível.

### Arquivos principais

- Entrada: `Assets/_Project/Hub/Scenes/Hub.unity`, primeira cena habilitada.
- Laboratório: `Assets/_Project/Laboratories/Physics/ObliqueLaunch/Scenes/ObliqueLaunch.unity`, segunda cena habilitada.
- Demo do template: `Assets/ThirdParty/VRMultiplayer/Scenes/SampleScene.unity`, fora do fluxo principal.
- Catálogo: `Assets/_Project/Hub/HubCatalog.asset`.
- UI: `Assets/_Project/Hub/HubUI.prefab`.
- Sala: `Assets/_Project/Environments/StandardRoom/TecaversoStandardRoom.prefab`.
- Empresa/produto: Tecaverso / Tecaverso Labs; identificador configurado `com.tecaverso.labs`.

### Contribuição

- Versionar Assets com seus .meta, Packages, ProjectSettings e documentação.
- Não versionar Library, Temp, Logs, UserSettings ou builds.
- Mover assets pela Unity para preservar GUIDs.
- Preservar alterações locais anteriores; não fazer limpezas/reset em massa.
- Não remover samples/terceiros sem verificar dependências.
- Atualizar catálogo/GDD quando disponibilidade ou comportamento mudar.
- Registrar o que foi realmente validado: compilar não equivale a funcionar no headset.

## 17. Referências e manutenção

- [Tecaverso Game UX Kit](https://www.figma.com/design/ztbnAS5MZZmqOLbQAfm96E/Tecaverso-Game-UX-Kit): HUB 1 `3053:200`, HUB 2 `3053:238`, Projectile Motion `3051:256`, Projectile Analysis `3051:338`.
- `Assets/_Project/Hub/README.md`: implementação específica do Hub.
- `Assets/_Project/UI/Figma-Implementation.md`: adaptação da identidade e fontes.
- `docs/development-guidelines.md`: diretrizes adaptadas do guia Unity *Level up your code with design patterns and SOLID* fornecido ao projeto.
- `docs/lan-template-changes.md`: alterações LAN do template.
- `docs/validation.md`: evidência histórica, com data/escopo.
- `Assets/_Project/UI/Fonts/Montserrat/OFL.txt`: licença da fonte.

Este README é referência de **design do produto**. Código/cenas e registros de validação são referência de **comportamento implementado e comprovado**. Uma funcionalidade especificada aqui não deve ser marcada como pronta sem implementação e verificação.

### Histórico

- **1.0 — 05/10/2026:** consolidação do GDD; correção do retrato do Hub; decisões de fluxo, educação, LAN, dados, arquitetura, critérios de aceite e roadmap. A arquitetura educacional passa a estar definida como design, ainda pendente de implementação.
