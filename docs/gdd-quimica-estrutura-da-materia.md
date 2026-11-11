# Short GDD — Modelos atômicos e estrutura atômica

**Status:** cena implementada no Unity; validação funcional no Editor concluída, conforto XR no headset pendente. **Disciplina:** Química. **Modo:** individual offline. **Data:** 11/11/2026.

**Cena:** `Assets/_Project/Laboratories/Chemistry/AtomicModels/Scenes/AtomicModels.unity`. A entrada “Modelos atômicos” do Hub abre esta sala. Os controles são objetos físicos com `XRSimpleInteractable`; o átomo usa `XRGrabInteractable` apenas para rotação. As vitrines usam maquetes 3D didáticas de cada proposta e partículas, com ilustrações locais e uma ficha contextual. As imagens abaixo são mockups do GDD, não capturas da cena.

## Objetivo e experiência

O aluno percorre duas alas: modelos históricos e partículas do átomo. No centro, monta átomos pressionando controles 3D de +/− para prótons, nêutrons e elétrons. Na vista **Camadas**, elétrons orbitam continuamente em anéis inclinados, como visualização inspirada em Bohr. O aluno gira o modelo no lugar com a mão ou raio XR para observar os anéis de outros ângulos, sem deslocar a plataforma ou a placa de resultados. Descobre que prótons definem o elemento, nêutrons distinguem isótopos e elétrons alteram a carga. Os modelos são representações didáticas, sem escala real; órbitas circulares não são trajetórias físicas no modelo quântico.

## Sala, objetos e catálogo

Seis estações históricas ocupam uma ala junto à parede. Na parede oposta, três vitrines menores mostram elétron, próton e nêutron; nas duas últimas, uma ampliação esquemática revela quarks e glúons. O aluno caminha entre as alas e em torno da oficina central. Três pares de botões físicos +/− ficam em um console ao lado da entrada; a placa de resultado permanece fixa junto à plataforma, e Voltar ao Hub fica próximo à saída. No protótipo plano, “Virar” simula olhar para a parede oposta; **não é um botão previsto na sala VR**.

| Objeto | Quantidade/opções | Interação XR | Resultado/limite |
|---|---|---|---|
| Museu | 6 estações: Leucipo/Demócrito, Dalton, Thomson, Rutherford, Bohr, modelo quântico | Apontar ou aproximar e selecionar | Placas curtas; uma ficha expandida por vez |
| Marco de Chadwick | 1 placa entre Bohr e contemporâneo | Selecionar | Descoberta do nêutron em 1932, não um modelo completo separado |
| Ala das partículas | 3 estações: elétron, próton, nêutron | Caminhar até a parede oposta e selecionar | Ilustração ampliada; elétron elementar, próton uud e nêutron udd; glúons explicados, não manipuláveis |
| Prótons | Par de botões +/−; 0–20 visíveis | Pressionar por unidade ou manter pressionado para repetição | Z de 1 a 20 identifica H a Ca; Z=0 não é elemento |
| Nêutrons | Par de botões +/−; 0–24 visíveis | Pressionar ou manter pressionado | Altera A, preserva Z; estabilidade não avaliada |
| Elétrons | Par de botões +/−; 0–20 visíveis | Pressionar ou manter pressionado | Altera carga; distribuição escolar automática em até quatro camadas |
| Plataforma | Núcleo e representação eletrônica ampliados | Segurar o átomo e girar em torno do núcleo; sem translação | Mostra contagens; anéis têm inclinações visuais distintas e giram juntos com a vista |
| Seletor | Camadas / Nuvem | Controle físico de duas posições | Troca a representação sem alterar a montagem |
| Recentralizar | 1 botão físico junto ao seletor | Pressionar | Retorna suavemente à orientação inicial sem alterar o átomo |
| Limpar | 1 botão físico | Pressionar | Confirma se houver peças; Cancelar preserva |

Cada estação histórica tem maquete 3D didática e placa curta. A ficha contextual ao lado da maquete mostra **proposta, evidência, avanço e limite**. A Antiguidade é hipótese filosófica, não modelo experimental. Rutherford mostra espalhamento e núcleo; Bohr, níveis discretos; o modelo contemporâneo, nuvem qualitativa. As vitrines não fingem que cientistas anteriores conheciam partículas descobertas depois. Na ala oposta, a ficha usa **composição, descoberta, papel no átomo e limite**. Quarks e glúons ficam dentro da explicação de próton/nêutron; fótons e neutrinos podem ser citados, mas não aparecem como componentes permanentes a encaixar. As imagens SVG atuais são didáticas, não reproduções documentais originais.

## Inventário de interfaces e HUD

**HUD de câmera:** nenhum. **Painel de edição genérico:** nenhum. Os rótulos ficam no console fixo; partículas representadas no modelo não precisam ser clicáveis.

| ID / interface | Tipo e localização | Finalidade | Conteúdo e ações | Inicial | Abre / fecha | Simultâneo |
|---|---|---|---|---|---|---|
| M1 · vitrines históricas | Maquete 3D e rótulo físico, uma por estação na primeira parede | Reconhecer cada proposta sem consulta obrigatória | Maquete do modelo, nome, época; selecionar abre M2 | Visíveis na primeira parede | Permanentes | M3, M4, A1–A5, S1 |
| M2 · ficha contextual | Painel ao lado da estação selecionada | Ampliar o modelo/partícula e explicar seu significado | Na ala histórica: proposta, evidência, avanço, limite; na ala de partículas: composição, descoberta, papel, limite; maquete adjacente e Fechar | Oculta | Selecionar M1/M4; Fechar ou nova seleção | M1, M3, M4, A1–A5, S1; só uma M2 |
| M3 · marco Chadwick | Placa física entre estações históricas | Contextualizar o nêutron | 1932, descoberta; consulta via M2 | Visível | Permanente | M1, M4, A1–A5, S1 |
| M4 · vitrines de partículas | Três maquetes 3D e rótulos físicos na parede oposta | Distinguir constituintes do átomo e estrutura dos núcleons | Elétron e⁻, próton p⁺/uud, nêutron n⁰/udd; seleção abre M2 | Visíveis na parede oposta | Permanentes | M1, M3, A1–A5, S1 |
| A1 · console de partículas | Três pares de botões físicos +/− ao lado da oficina | Alterar contagens sem manipular muitas peças pequenas em VR | Próton p⁺, nêutron n⁰, elétron e⁻; toque muda uma unidade, manter pressionado repete após breve espera | Visível, todas as contagens em 0 | Permanente; toque/raio XR nos botões | M1, M3, M4, A2–A5, S1 |
| A2 · estado | Placa fixa ao lado da plataforma | Verificar resultado imediato | Nome/símbolo, p/n/e, Z, A, carga, neutro/cátion/ânion | “Nenhum elemento” | Atualiza após cada pressão | M1, M3, M4, A1, A3–A5, S1 |
| A3 · vista | Seletor físico na borda | Comparar representações | Camadas com elétrons orbitando / Nuvem; posição ativa; aviso de movimento didático | Camadas | Alterna por toque/raio XR | M1, M3, M4, A1, A2, A4, A5, S1 |
| A4 · Limpar | Botão físico na borda | Recomeçar com segurança | Limpar montagem | Visível | Abre C1 apenas com peças | M1, M3, M4, A1–A3, A5, S1 |
| A5 · giro e Recentralizar | Modelo central selecionável para rotação; botão na borda | Inspecionar anéis inclinados sem mover a montagem nem a leitura | Segurar e girar no lugar; soltura com desaceleração curta; Recentralizar volta à orientação inicial | Visível, orientação padrão | Permanentes; não abrem painel | M1, M3, M4, A1–A4, S1 |
| C1 · confirmação | Pequeno modal sobre plataforma | Evitar apagamento acidental | Cancelar / Limpar e consequência | Oculto | A4 abre; escolha fecha | Bloqueia interação com todos os controles do mundo; Cancelar / Limpar continuam acessíveis |
| E1 · limite | Aviso local junto ao console | Explicar pressão sem mudança | Mínimo 0 ou máximo da oficina; contagem preservada | Oculto | Pressionar − em 0 ou + no máximo; fecha após leitura | A1–A5, S1; não empilha com C1 |
| S1 · Voltar ao Hub | Botão padrão na saída | Navegação | Voltar ao Hub | Visível | Permanente | Todos exceto C1 ativo |

**Entrada:** seis ilustrações históricas à frente, três vitrines de partículas na parede oposta, console +/−, plataforma vazia, A2–A5 e S1; nenhuma ficha aberta. **Manipulação:** cada toque muda uma unidade no modelo e em A2; elétrons orbitam continuamente em Camadas; A5 gira apenas a visualização, mantendo resultados e controles imóveis. **Consulta:** M2 abre com imagem ampliada e quatro campos, fecha explicitamente e preserva a montagem. **Confirmação:** C1 impede os botões +/− até escolha. Texto e forma complementam cor.

## Prévias visuais de UI/HUD

**Propostas conceituais, não capturas de cena implementada.** Fonte: [protótipo HTML interativo](prototypes/modelos-atomicos/index.html). Nele, clique/toque e pressão prolongada nos botões +/− simulam controles 3D; arrastar o átomo simula segurá-lo para girar, e “Virar” simula caminhar até a parede oposta. “Virar”, “Como usar” e a faixa de passos são recursos explicativos do navegador, não interfaces previstas na sala VR. Capturas locais em `GeneratedMedia/GDD/modelos-atomicos/` são ignoradas pelo Git; enviar junto do documento ao compartilhá-lo. Os desenhos SVG das vitrines são ilustrações didáticas feitas para o GDD, não modelos 3D finais nem imagens documentais originais. Cada imagem tem 1600×900.

### Entrada — M1, M3, A1–A4, S1

![Visão do museu e da oficina](../GeneratedMedia/GDD/modelos-atomicos/entrance.png)

Entrar; selecionar uma estação ou pressionar + no console. O estado vazio permanece visível.

### Consulta — M1, M2, M3, A1–A4, S1

![Ficha histórica de Rutherford](../GeneratedMedia/GDD/modelos-atomicos/museum.png)

Selecionar Rutherford; Fechar ou selecionar outra estação. A oficina permanece.

### Ala das partículas — M4, A1–A5, S1

![Vitrines de elétron, próton e nêutron](../GeneratedMedia/GDD/modelos-atomicos/particles.png)

No VR, caminhar até a parede oposta; no protótipo, acionar “Virar”. A montagem central permanece.

### Consulta de partícula — M2, M4, A1–A5, S1

![Ficha ampliada do próton com quarks e glúons](../GeneratedMedia/GDD/modelos-atomicos/proton.png)

Selecionar próton; a ficha mostra composição, descoberta, papel no átomo e limite. Fechar retorna à observação sem alterar a montagem.

### Manipulação — M1, M3, A1–A4, S1

![Carbono 12 montado](../GeneratedMedia/GDD/modelos-atomicos/assembly.png)

Pressionar + seis vezes em cada linha para formar C-12 neutro; usar − para comparar.

### Giro do modelo — A1–A5, S1

![Átomo girado para ver planos inclinados das órbitas](../GeneratedMedia/GDD/modelos-atomicos/rotation.png)

Segurar e girar o átomo no lugar; a soltura desacelera suavemente. Recentralizar restaura a orientação inicial. Contagens e posição da plataforma não mudam.

### Comparação de isótopo e íon — M1, M3, A1–A4, S1

![Carbono 13 após acrescentar um nêutron](../GeneratedMedia/GDD/modelos-atomicos/isotope.png)
![Cátion após retirar um elétron](../GeneratedMedia/GDD/modelos-atomicos/ion.png)

Acrescentar 1 nêutron, depois retirar 1 elétron. Repor as peças reverte cada estado.

### Vista quântica didática — M1, M3, A1–A4, S1

![Mesma montagem em nuvem qualitativa](../GeneratedMedia/GDD/modelos-atomicos/cloud.png)

Acionar Nuvem; Camadas retorna às órbitas animadas. Contagens permanecem.

### Limite superior do catálogo — M1, M3, A1–A4, S1

![Cálcio 40 com quatro camadas didáticas](../GeneratedMedia/GDD/modelos-atomicos/calcium.png)

Montar Ca-40 com 20 prótons, 20 nêutrons e 20 elétrons. A placa informa o símbolo Ca e as quatro camadas mostram 2, 8, 8 e 2 elétrons.

### Limite — E1, A1–A4, S1

![Aviso local de limite](../GeneratedMedia/GDD/modelos-atomicos/error.png)

Pressionar + no limite de 20 prótons; a contagem não muda. A prévia mostra Ca-40 com 20p, 20n e 20e.

### Confirmação — C1 com controles de fundo bloqueados

![Confirmação para limpar](../GeneratedMedia/GDD/modelos-atomicos/clear.png)

Acionar Limpar; Cancelar preserva e Limpar esvazia.

**Reprodução:** abrir `index.html` para interagir desde a entrada. Para reproduzir uma captura, usar `?demo=entrance|museum|particles|proton|assembly|rotation|isotope|ion|cloud|calcium|error|clear`; mesmo nesses estados, os controles continuam ativos. Avaliar leitura e alcance no headset; o protótipo plano não prova conforto XR.

## Fluxo e interações XR

1. O aluno percorre M1, consulta M2 em Dalton, Rutherford e modelo quântico e fecha cada ficha. Caminha à parede oposta para M4 e consulta elétron, próton e nêutron; compara a composição dos núcleons.
2. Pressiona + em Próton no console A1: A2 mostra H. Usa + em Nêutron e Elétron, compara H-1 e H-2.
3. Monta C-12 com seis pressões de + em cada linha; acrescenta um nêutron (C-13) e usa − em Elétron (carga +1). + repõe o elétron.
4. Observa os elétrons orbitando em planos inclinados na vista Camadas. Segura A5 para girar todo o arranjo orbital em torno do núcleo e solta para uma desaceleração curta. Recentraliza a orientação; alterna A3 e explica a diferença entre esse esquema histórico animado e a nuvem qualitativa.
5. Experimenta E1 e A4/C1 ou retorna pelo S1.

Foco dá contorno/brilho discreto; botão pressionado recua brevemente e a contagem e a partícula correspondente se atualizam imediatamente; ficha aparece com fade discreto; limite dá feedback estático e texto. A5 gira somente a representação, sem translação, com inércia breve e amortecimento; Recentralizar anima a volta. Em movimento reduzido, a soltura interrompe a rotação e Recentralizar salta à posição padrão. Restauração ao ocultar/desselecionar é obrigatória. Reutilizar Shared/Interaction sem alterar colliders, transformações científicas nem câmera. Movimento decorativo nas vitrines não desloca texto ou pontos de interação.

## Modelo e limites

- `Z = p`; H a Ca para `1 ≤ p ≤ 20`; `p = 0` mostra “nenhum elemento”.
- `A = p + n`, número de massa inteiro, diferente da massa atômica média.
- Carga em unidades elementares: `q/e = p − e`. Positiva: cátion; negativa: ânion; zero: neutro.
- Catálogo: 0–20 prótons, 0–24 nêutrons, 0–20 elétrons. Estados dentro do intervalo têm contagens válidas, sem afirmar estabilidade, abundância natural, radioatividade ou ocorrência. C-13 e Ca-40 são notações de isótopo, não juízos automáticos de estabilidade.
- Camadas escolares até 20 elétrons, distribuídos no protótipo como 2, 8, 8, 2. Na vista Camadas, os elétrons percorrem quatro órbitas didáticas contínuas, com velocidades visuais distintas; é animação pedagógica, não trajetória física afirmada pelo modelo quântico. Os planos têm inclinações **escolhidas para leitura 3D**, não ângulos previstos por uma lei para o átomo montado. O modelo de Bohr quantiza níveis/energia e momento angular, mas não fornece esses quatro ângulos. O número quântico magnético descreve orientação de orbitais quânticos e não torna estes anéis trajetórias reais. A vista Nuvem não mostra órbitas nem probabilidade quantitativa. Não simular orbitais individuais, spin, energias, decaimento, força elétrica ou escala real. Preferência de movimento reduzido suprime inércia e transições decorativas; as órbitas continuam animadas por serem conteúdo científico, conforme DESIGN_SYSTEM.md.
- A5 permite girar a visualização ao redor do núcleo; não pega, desloca, escala nem duplica o átomo. O núcleo é pivô fixo. A1/A2 ficam ancorados. Nesta versão não há cópia portátil.
- M4 diferencia elétron elementar de próton e nêutron compostos. Próton: uud; nêutron: udd; glúons aparecem como parte esquemática da interação forte. Fótons e neutrinos não são adicionados à montagem nem apresentados como componentes permanentes do átomo comum. Conteúdo e ilustrações ficam locais.
- Cada pressão altera no máximo uma unidade. − em zero e + no limite preservam a contagem e mostram E1. C1 bloqueia novos ajustes.
- Dados históricos e lista de elementos ficam locais, versionados. Sem API em runtime. LAN futura terá anfitrião como autoridade para montagens; não afirmar sincronização antes de implementar.

## Critérios de aceite e verificação

- Museu cobre as seis estações históricas e a ala de três partículas, com imagens e fichas coerentes; Chadwick é marco, não um “novo modelo” completo.
- Cena, inventário e imagens concordam; nenhum painel extra entra sem revisar GDD e mockups.
- Seleção nas duas alas, botões +/−, alternância, retirada por − e fechamento funcionam em XR sem bloquear circulação.
- A5 gira em torno do núcleo com amortecimento, sem translação da plataforma; Recentralizar retorna à orientação padrão. Movimento reduzido interrompe a inércia.
- Z, A, carga, símbolo e nome se atualizam em H, C-12/C-13, Ne, K e Ca-40, íons e zero prótons; nenhuma duplicação.
- Confirmação/cancelamento, limite e motion com movimento reduzido funcionam.
- Validar lógica no Editor e conforto, contraste e performance no headset. Não fazer build sem autorização.

**Verificado no Editor:** importação e compilação da cena; 10 vitrines, seis controles de contagem, C-12 neutro, C-13 com carga +1, alternância Camadas/Nuvem, Cancelar/Limpar e registro no Hub. **Ainda requer teste presencial em XR:** alcance e leitura das fichas nas duas paredes, orientação confortável ao girar o átomo, feedback de foco e desempenho. A cena não foi gravada nem compilada em build nesta etapa.

## Referências

- [Só Química — Modelos atômicos](https://www.soquimica.com.br/conteudos/em/modelosatomicos/).
- [LibreTexts — The Structure of the Atom](https://chem.libretexts.org/Courses/can/CHEM_210_General_Chemistry_I_(Puenzo)/02:_Atoms_and_Elements/2.05:_The_Structure_of_The_Atom).
- [OpenStax Chemistry 2e — Atomic Structure and Symbolism](https://openstax.org/books/chemistry-2e/pages/2-3-atomic-structure-and-symbolism).
- [OpenStax Chemistry 2e — Electronic Structure](https://openstax.org/books/chemistry-2e/pages/6-4-electronic-structure-of-atoms-electron-configurations).
- [OpenStax Chemistry — O modelo de Bohr](https://openstax.org/books/chemistry/pages/6-2-the-bohr-model) e [desenvolvimento da teoria quântica](https://openstax.org/books/chemistry/pages/6-3-development-of-quantum-theory): níveis, momento angular e orientação de orbitais.
- [CERN — Modelo Padrão](https://home.cern/science/physics/standard-model/) e [estrutura da matéria em ALICE](https://home.cern/science/experiments/alice/): elétron, quarks e glúons.

## Implementação e evidências — 11/11/2026

A sala usa botões 3D com malha PushButton oficial Unity para +/− e XRLever para Camadas/Nuvem. Fichas e confirmação são Canvases World Space com UIVisibility e ponteiro XR; o restante permanece no mundo. Dez ilustrações estão em AtomicModels/Art e funcionam offline. Consulta inclui imagem ampliada e texto; não há HUD preso à câmera. Os mockups acima continuam conceituais; a disposição final reserva corredores entre oficina e museus.

Ver [guia de uso, arquitetura e validação](../Assets/_Project/Laboratories/Chemistry/AtomicModels/README.md). Capturas reais locais:

![Oficina em funcionamento](../GeneratedMedia/Validation/AtomicModels/carbono.png)

![Consulta histórica implementada](../GeneratedMedia/Validation/AtomicModels/museum-final.png)

## Revisão ergonômica — bancada única (11/11/2026)

Esta revisão substitui a dispersão de A1–A5 entre pedestais. Um único tampo inclinado a 45° agrupa os seis botões +/− na faixa frontal e Recentralizar, Camadas/Nuvem e Limpar na faixa posterior. Todos continuam visíveis na estação. A2 permanece fixo acima do tampo, com o átomo acima/atrás da leitura, sem HUD na câmera. C1 aparece na mesma região de leitura, ocultando A2 durante a decisão; Cancelar/Limpar fecham C1 e restauram A2. E1 fica junto à leitura. As fichas M1–M4 e o retorno S1 permanecem em suas posições.

Tampo inicial: cerca de 1,8 × 0,8 m; inclinação de 45°, altura frontal próxima de 0,8 m. A bancada fica à frente da posição de entrada, sem deslocar o rig. A representação do átomo é reduzida para caber acima da estação; contagens, órbitas relativas e equações permanecem iguais. O raio XR permite selecionar os controles e girar o átomo sem esticar o corpo. Alcance sentado e em pé devem ser validados no headset; medidas não são certificação de ergonomia.

![Proposta de bancada única e controles](../GeneratedMedia/GDD/modelos-atomicos/bancada.png)

Prévia conceitual anterior à alteração Unity. O HTML mostra áreas funcionais; na cena, +/− e alavanca reutilizam os modelos oficiais Unity. Nenhum painel adicional foi introduzido. Imagens anteriores documentam a primeira disposição, substituída por esta revisão.

### Bancada implementada

![Bancada única na Unity com Ca40](../GeneratedMedia/Validation/AtomicModels/bancada-final.png)

Captura de validação no Editor. Museu e rig preservados; modelos Unity reutilizados; seis alvos de partículas sem sobreposição. Conforto físico e alcance sentado/em pé permanecem pendentes de teste no headset.

## Revisão de disposição — alas laterais

Bancada deslocada 1,5 m em direção ao centro. A1/A3/A4/A5 permanecem no mesmo tampo. A2 torna-se leitura inclinada, abaixo do átomo, sem um painel vertical entre olhos e modelo. C1 ocupa temporariamente essa região e continua bloqueando controles. A montagem mantém acesso por raio XR; usuário pode aproximar-se da estação sem mover automaticamente a câmera.

M1 e M3 ocupam a parede esquerda; M4, a direita. A ficha M2 usa a orientação da própria vitrine e abre diante dela, afastada da oficina; apenas uma ficha por vez. Título/instrução acompanham a ala histórica. S1 e rig permanecem. Não há novos painéis ou controles.

![Prévia espacial das alas e oficina](../GeneratedMedia/GDD/modelos-atomicos/disposicao.png)

Proposta de disposição anterior à alteração da cena; não é captura Unity. O vídeo demonstrará montagem C12, C13, íon, Ca40, limites, Camadas/Nuvem, giro/recentralização, consulta histórica e partículas, cancelamento e limpeza. Demonstração automatizada via eventos/controles reais, não teste humano em headset.
