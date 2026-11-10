# Short GDD — Reações químicas: escolher, misturar e observar

**Status:** reformulação aprovada e implementada no Editor em 10/11/2026: 12 reagentes em degraus, suportes A/B, sonda térmica XR e consulta opcional. Substitui o fluxo de experiências selecionadas por painel. Uso, arquitetura e verificações estão no [README técnico](../Assets/_Project/Laboratories/Chemistry/Reactions/README.md); alcance e leitura em headset continuam pendentes. **Disciplina:** Química. **Modo:** individual, offline. **Sessão sugerida:** 12–18 minutos.

## 1. Experiência e objetivo

**Correção de usabilidade — 10/11/2026:** retirar os Canvases transportados pelos frascos e colheres. Frascos/potes usam rótulo impresso em material numa faixa curva aderente ao vidro (fórmula, nome, concentração/estado), sem ultrapassar a silhueta. Identificação completa, estoque, carga da colher e marca de parceiro disponível ficam na borda fixa do degrau. Colheres recebem identificação no cabo. O visor do termômetro pertence ao instrumento; a ficha opcional continua sendo a única consulta ampliada. Não pintar os líquidos para identificar categorias.

![Revisão de identificação e dosagem sem painéis nos frascos](../GeneratedMedia/GDD/Reacoes/08-rotulos.png)

Fonte reproduzível: [prévia dos rótulos](prototypes/reacoes-quimicas/rotulos.html). Esta revisão substitui a representação dos rótulos nas prévias anteriores.

**Revisão aprovada e implementada — degraus e medição térmica (10/11/2026):** substituir o U por três níveis e quatro colunas de categorias, com frascos desencontrados e área de trabalho livre à frente. Preservar o béquer, suportes A/B, três comandos e ficha opcional. Soluções de partida incolores; branco/preto para os sólidos reais. Sonda térmica XR no suporte de instrumentos: leitura somente com ponta imersa; retirar apaga a leitura. Mede estimativa ideal em HCl/NaOH e HCl/KOH; demais pares mostram “Modelo térmico indisponível”, sem inventar 25 °C como resultado. Sem novo painel ou slider.

![Prévia aprovada da mesa em degraus e visor no instrumento](../GeneratedMedia/GDD/Reacoes/07-temperatura.png)

UI-07 · **Visor da sonda**, preso ao instrumento: estado (mergulhe / imersa / sem modelo), temperatura atual, inicial e variação. Valores exemplificados para 100 mL de cada solução 0,10 mol/L. Pega direta/raio e motion compartilhado; repouso dentro do béquer permite dosar com ambas as mãos livres. Reinício e saída exigem devolver também a sonda. Consulta continua única e opcional.

Modelo térmico inicial: T₀ = 25 °C; soluções recebidas à mesma temperatura; densidade 1 g/mL e calor específico 4,18 J/(g·K); capacidade térmica didática do béquer 80 J/K. H⁺(aq) + OH⁻(aq) → H₂O(l), ΔH° = −55,83 kJ/mol, calculado de −285,83 − (0 − 230,0), conforme [OpenStax, Apêndice G](https://openstax.org/books/chemistry-2e/pages/g-standard-thermodynamic-properties-for-selected-substances). Q = −ΔH°·extensão; T = T₀ + Q/(80 + 4,18·volume em mL). Calor e capacidade usam apenas material aceito; excesso dilui o aquecimento, perdas não aquecem o béquer. Sem troca com o ambiente, calor de diluição, resposta temporal do sensor ou cinética. pH continua aproximação a 25 °C, separada da estimativa térmica. Não emitir vapor ou alterar cor para comunicar calor.

Uma mesa com reagentes agrupados em **ácidos, bases, sais e óxidos**. O aluno escolhe um par pelos próprios frascos, dosa com as mãos e observa a transformação no recipiente. Compara formação de gás, precipitação, neutralização e consumo de sólido, relacionando quantidades, produtos e excesso.

**Preservar o visual aprovado:** vidraria, líquido com nível e wobble, jato, gotas, bolhas e precipitado. Reutilizar os assets existentes sem redesenhar shaders ou partículas. Esta revisão muda disposição, escolha de reagentes e apresentação das informações. Ampliar o catálogo exige trabalho no modelo; não é apenas esconder painéis.

## 2. Prévias antes da implementação

O [protótipo HTML navegável](prototypes/reacoes-quimicas/index.html) apresenta sete estados. São esquemas de disposição e interface, não renders finais nem capturas da Unity. A navegação superior e as legendas externas pertencem à documentação, não à aplicação.

### Entrada — a mesa apresenta as opções

![Entrada: grupos de reagentes e suportes A/B, sem análise aberta](../GeneratedMedia/GDD/Reacoes/01-entrada.png)

### Escolha — o primeiro reagente revela os parceiros cadastrados

![HCl em A e destaque dos sete parceiros disponíveis em B](../GeneratedMedia/GDD/Reacoes/02-escolha.png)

### Manipulação — frascos livres depois da escolha do par

![Dosagem livre do par cloreto de cálcio e carbonato de sódio](../GeneratedMedia/GDD/Reacoes/03-mistura.png)

### Consulta — uma única ficha opcional

![Ficha lateral com equação e resultado, mantendo o recipiente visível](../GeneratedMedia/GDD/Reacoes/04-consulta.png)

### Limite — impedir uma terceira substância sem consumir estoque

![Terceiro reagente bloqueado antes do despejo e explicação local](../GeneratedMedia/GDD/Reacoes/05-limite.png)

### Reinício — confirmação e cancelamento explícitos

![Confirmação de nova mistura](../GeneratedMedia/GDD/Reacoes/06-reinicio.png)

As imagens são saídas locais ignoradas pelo Git, conforme a política do projeto. O HTML e o [script de captura](prototypes/reacoes-quimicas/capture.ps1) permitem reconstruí-las sem rede; consultar o [README do protótipo](prototypes/reacoes-quimicas/README.md). Os estados precisam acompanhar qualquer mudança de fluxo antes da implementação.

## 3. Disposição e objetos

- **Mesa em degraus**, 3,9 m de largura: três tampos de 0,58 m de profundidade, alturas de 0,92 / 1,17 / 1,42 m. Quatro colunas por categoria, frascos desencontrados lateralmente em 0,16 m entre níveis. Frente de trabalho a 0,92 m; suportes A/B e béquer mantidos. Alcance sentado/em pé a validar; interação por raio complementa o acesso lateral. Não esticar rig ou frascos.
- Quatro regiões sinalizadas com nome, sem depender só de cor, **três recipientes por grupo, 12 ao todo**, presentes desde a entrada. Distribuir os grupos em quatro colunas dos degraus, mantendo espaço de pega entre frascos e acesso pela circulação. Não reduzir objetos/rótulos para acomodar o catálogo. Rótulos impressos no material trazem nome, fórmula, estado e concentração. Estoque/carga da colher e parceiro disponível ficam na borda fixa do degrau. A faixa de categoria não colore artificialmente a solução.
- **Dois suportes físicos A e B**, com XR Socket Interactor, escolhem o par. Não são áreas de estudo, cópias de amostras ou estações de comparação.
- **Um béquer fixo de 500 mL**, com graduação, área central livre e contenção para perdas. Não há mistura entre vários recipientes nesta entrega.
- Cinco potes de sólido com colheres dedicadas e identificadas, guardadas junto de seus potes. Encaixa-se o pote para escolher a substância; usa-se sua colher para dosar. O pote não despeja como frasco de líquido.
- Suporte lateral para termômetro XR e conta-gotas de indicador para HCl/NaOH: auxiliar explícito, não reagente universal.
- Faixa compacta de comandos na lateral acessível da mesa. Uma ficha de leitura abre no espaço lateral reservado, sem atravessar o béquer ou a trajetória da mão.

## 4. Escolha controlada sem catálogo de botões

**Regra central: dois reagentes por tentativa; várias adições desses mesmos reagentes.** A disponibilidade depende de uma receita cadastrada por identidade dos compostos, não de regras simplistas como “ácido nunca mistura com ácido”.

| Estado | O aluno faz | Resposta da sala |
|---|---|---|
| Escolher A | Pega qualquer frasco/pote e encaixa no suporte A | Mostra nome/fórmula. Parceiros cadastrados recebem contorno e marca “B disponível”. Outros continuam visíveis e legíveis. Não despejar ainda. |
| Escolher B | Encaixa um parceiro no suporte B | Aceita somente identidade diferente que forme receita com A. Mesma disponibilidade nas duas ordens de escolha. |
| Par pronto | Retira recipientes para manipular | O par permanece registrado nos suportes, mesmo vazios. Não cria duplicatas. Encaixar B válido prepara a tentativa automaticamente; sem botão Iniciar. |
| Dosar | Inclina A/B ou usa a colher dedicada | Transferência gradual. A→B→A→B é permitido, com estoque finito, conservação, reagente limitante e excesso. |
| Tentar outro reagente | Aproxima ou tenta verter C | Não aceita trocar o par nem entrar terceiro reagente. Bloqueia antes de emitir jato/partículas e consumir estoque. Explica como iniciar outra mistura. |
| Trocar sem uso | Aciona Trocar par, instrumentos devolvidos e nenhuma dose/perda | Libera seleção e suportes, sem confirmação desnecessária. |
| Nova mistura | Solicita reinício depois de qualquer consumo, perda ou indicador | Confirma descarte e reposição. Cancelar conserva par, estoques e conteúdo. Confirmar volta à escolha A. |

**Detalhes XR:** pega por proximidade ou raio; encaixe deliberado ao soltar sobre o socket. O suporte não captura instrumentos segurados. Enquanto só A foi escolhido, retirá-lo desfaz a escolha e os destaques; após B completar o par, retirar ambos permite dosar sem perder a seleção. B fica indisponível enquanto A está vazio. A mesma substância nos dois suportes não constitui nova receita.

**Ácido com ácido, base com base e outros pares fora do catálogo:** B não aceita o encaixe; a peça permanece solta/segurada normalmente, sem empurrão ou teletransporte. Mensagem: “Esta combinação não está disponível neste laboratório.” É uma restrição do modelo, **não uma afirmação de inexistência de reação na natureza**. Não emitir efeitos nem simular “nenhuma reação” como conclusão científica para um par desconhecido.

**Sequência sal → base → ácido → óxido:** não é permitida na mesma tentativa desta versão. Exigiria química multicomponente, equilíbrios, produtos intermediários e nova validação. O aluno explora esses grupos em tentativas separadas. Uma futura atividade de etapas encadeadas precisa de roteiro e modelo próprios.

**Sem desperdício involuntário:** antes de fechar A/B, nenhum instrumento transfere. Depois, apenas A/B estão habilitados; a regra vale também para colheres e para qualquer caminho de entrada no modelo. Para A/B válidos, verter fora do recipiente continua sendo perda contabilizada. Aviso de combinação inválida prevalece sobre capacidade; nenhuma ação bloqueada altera estoque.

## 5. Catálogo implementado

**12 reagentes: 3 ácidos, 3 bases, 3 sais e 3 óxidos.** Sete soluções de 300 mL a 0,10 mol/L e cinco sólidos de 6 g por tentativa. Colheres de até 1 g, dosagem gradual e estoque explícito. Todos participam de pelo menos uma receita, sem itens apenas decorativos. O indicador é auxiliar e não entra na contagem.

| Grupo | Composto | Fórmula | Apresentação | Parceiros cadastrados |
|---|---|---|---|---|
| Ácidos | Ácido acético | CH₃COOH | Solução aquosa | NaHCO₃; NaOH |
| Ácidos | Ácido clorídrico | HCl | Solução aquosa | NaOH; KOH; Mg(OH)₂; NaHCO₃; Na₂CO₃; MgO; ZnO |
| Ácidos | Ácido sulfúrico diluído | H₂SO₄ | Solução aquosa | NaOH; CuO |
| Bases | Hidróxido de sódio | NaOH | Solução aquosa | HCl; CH₃COOH; H₂SO₄ |
| Bases | Hidróxido de potássio | KOH | Solução aquosa | HCl |
| Bases | Hidróxido de magnésio | Mg(OH)₂ | Sólido branco | HCl |
| Sais | Bicarbonato de sódio | NaHCO₃ | Sólido branco | CH₃COOH; HCl |
| Sais | Cloreto de cálcio | CaCl₂ | Solução aquosa | Na₂CO₃ |
| Sais | Carbonato de sódio | Na₂CO₃ | Solução aquosa | CaCl₂; HCl |
| Óxidos | Óxido de magnésio | MgO | Sólido branco | HCl |
| Óxidos | Óxido de cobre(II) | CuO | Sólido preto | H₂SO₄ |
| Óxidos | Óxido de zinco | ZnO | Sólido branco | HCl |

MgO e CuO são óxidos básicos; ZnO é anfótero. Ficam no setor Óxidos; os hidróxidos ficam no setor Bases. O caráter anfótero do ZnO pode constar na consulta, mas suas reações com bases não estão cadastradas nesta versão. Mg(OH)₂ é pouco solúvel e é oferecido como sólido, não como solução fictícia de 0,10 mol/L. Não criar grupo de elementos puros nesta sala.

O indicador fica fora desses 12 reagentes. Continua habilitado somente no par HCl/NaOH (R3); ativar o conta-gotas sobre o béquer adiciona traço de fenolftaleína, registrado como “indicador presente”. Repetições não intensificam artificialmente a cor. Volume/consumo estequiométrico do traço são desprezados e declarados. Fora desse par, o instrumento informa sua finalidade e não modifica o recipiente. É a única exceção auxiliar à seleção A/B; ampliar a medição de pH para outros pares exige validar o modelo correspondente.

## 6. Doze receitas e evidências

| ID | Equação balanceada | O que observar | Implementação e limites |
|---|---|---|---|
| R1 | CH₃COOH + NaHCO₃ → CH₃COONa + H₂O + CO₂ | Bolhas durante produção de gás; sólido em excesso pode permanecer | Implementado; modelo preservado |
| R2 | CaCl₂ + Na₂CO₃ → CaCO₃(s) + 2 NaCl | Formação e depósito de sólido branco | Implementado; modelo preservado |
| R3 | HCl + NaOH → NaCl + H₂O | Sem bolhas/precipitado; indicador opcional revela mudança de pH | Implementado; indicador explícito |
| R4 | HCl + NaHCO₃ → NaCl + H₂O + CO₂ | Gás e consumo do sólido conforme dose | Implementado |
| R5 | 2 HCl + Na₂CO₃ → 2 NaCl + H₂O + CO₂ | Gás; comparar proporção molar com R4 | Implementado |
| R6 | CH₃COOH + NaOH → CH₃COONa + H₂O | Pode não haver mudança macroscópica evidente; ficha explica transformação | Implementado; não reutilizar pH de ácido forte |
| R7 | MgO + 2 HCl → MgCl₂ + H₂O | Consumo de sólido branco; sem geração de gás | Implementado; dissolução didática |
| R8 | HCl + KOH → KCl + H₂O | Neutralização sem mudança visual obrigatória; consultar produtos | Implementado |
| R9 | Mg(OH)₂ + 2 HCl → MgCl₂ + 2 H₂O | Consumo de sólido branco; comparar base pouco solúvel com NaOH | Implementado; sem simular pH/solubilidade detalhados |
| R10 | H₂SO₄ + 2 NaOH → Na₂SO₄ + 2 H₂O | Comparar proporção de neutralização completa 1:2 com HCl/NaOH 1:1 | Implementado; balanço global, não curva de titulação |
| R11 | CuO + H₂SO₄ → CuSO₄ + H₂O | Consumo de sólido preto e formação de solução azul; sem bolhas | Implementado; não gerar cristais automaticamente |
| R12 | ZnO + 2 HCl → ZnCl₂ + H₂O | Consumo de sólido branco; sem geração de gás | Implementado; dissolução didática |

R11 reutiliza os shaders e sistemas de partículas atuais, configurando a cor do sólido e da solução pelo estado calculado. Não redesenhar o acabamento aprovado. Cinética, aquecimento e cristalização não são simulados; a duração visual não reproduz o tempo real de dissolução. O sal dissolvido azul não deve aparecer como precipitado azul.

Não adicionar cor ou efervescência para tornar toda reação espetacular. “Não vi bolhas” não significa “não reagiu”; R6 oferece esse contraste. Equações globais não representam sais como moléculas intactas em solução.

Estas são todas as receitas aceitas. Outros pares, mesmo quimicamente possíveis, ficam fora desta versão. Produtos são resultados contabilizados, não frascos para alimentar outra etapa. Destaques, sockets, mensagens e cálculo devem consultar **a mesma base local de receitas**.

## 7. Inventário de interfaces e visibilidade

**Nenhum HUD preso à cabeça. Uma faixa pequena; no máximo uma ficha expandida.** Rótulos e suportes fazem a descoberta. Não há seleção de casos por botões, visor molecular permanente, abas de edição ou balanceamento interativo nesta revisão.

| ID / interface | Tipo/local | Função e ações exatas | Entrada | Abre / fecha / simultaneidade |
|---|---|---|---|---|
| UI-01 · Sinalização | Texto estático acima da mesa e nos setores | Nome da sala; uma instrução curta; nomes dos quatro grupos. Leitura | Visível | Permanente, sem placas duplicadas de instrução |
| UI-02 · Identificação | Material curvo nos frascos, impressão nos cabos, placas fixas nos degraus e suportes | Nome/fórmula, estado e concentração no material; estoque/carga da colher e marca B disponível no móvel; A/B e graduação mantidos | Visível | Zero Canvas nos reagentes/colheres; não há painel acompanhando a mão. Foco realça um alvo sem abrir fichas |
| UI-03 · Situação local | Uma pequena placa lateral aos suportes, fora do volume de observação | Próxima ação, par pronto, evidência ou impedimento e recuperação. Sem botões | “Escolha A, depois B” | Um texto por vez. Aviso persiste enquanto tentativa inválida continuar, substituindo a dica |
| UI-04 · Comandos | Faixa na borda acessível da mesa | **Consultar**, **Trocar par / Nova mistura**, **Voltar ao Hub**. Segundo botão depende de uso | Visível | Permanente; confirmação bloqueia fundo. Sem slider ou pausa por padrão |
| UI-05 · Consulta | Painel espacial opaco lateral ao béquer | Antes do par: ajuda A/B. Par pronto: nomes, estados, proporção molar. Após dosagem: evidência, equação, doses, produtos, limitante/excesso, perdas; pH só R3. **Fechar ficha** | Oculta | Abre só por Consultar; fecha por Fechar ou ao pegar instrumento para dosagem. Não pausa nem modifica dados |
| UI-06 · Confirmação | Modal na área de consulta | Descarte/reposição ou saída com perda; **Cancelar** e **Confirmar reinício / Sair** | Oculta | Substitui UI-05. Bloqueia novas pegas/transferências e UI de fundo, preserva locomoção. Só abre após devolver instrumentos; cancelar restaura estado anterior |
| UI-07 · Sonda | Visor no termômetro XR | Temperaturas inicial/atual e variação estimadas; imersão ou modelo indisponível | Pede imersão | Leitura apenas dentro do líquido; apaga ao retirar. Modelo térmico em HCl/NaOH e HCl/KOH |

**Controles 3D:** A/B são sockets. Frascos/potes são XR Grab Interactables; colheres transferem sólidos; conta-gotas tem ativação discreta sobre o béquer. Não acrescentar painel de catálogo para essas ações.

**Entrada:** objetos, sinalização/rótulos, dica curta e três comandos; zero análise aberta. **Manipulação:** graduação, identificação A/B e no máximo uma mensagem local. **Consulta:** um painel; a mensagem local se recolhe salvo impedimento urgente. **Confirmação:** uma decisão substitui a ficha, sem outras camadas. Legendas de gravação não fazem parte da aplicação.

Montserrat, fundo lavanda opaco, ações azuis e química como acento. Ficha com referência inicial de 0,65 × 0,60 m, a ajustar por leitura no headset, sem reduzir fonte para caber. Se necessário paginar, atualizar o mockup antes. Posicionamento estável, sem seguir cabeça. Alvos de referência ≥1,5° e corpo 0,5°–0,8°, conforme Design System; validar no dispositivo. Alcance por raio complementa pega direta.

## 8. Fluxo de sessão

1. Reconhecer os grupos e ler os frascos sem abrir menu.
2. Encaixar CaCl₂ em A; identificar Na₂CO₃ como parceiro e encaixá-lo em B.
3. Retirar e despejar em qualquer ordem. Observar o sólido; adicionar mais A/B e comparar quantidade produzida e estoque.
4. Consultar equação/excesso e fechar. Ao tentar HCl, receber orientação antes de consumir material.
5. Confirmar Nova mistura, escolher CH₃COOH + NaHCO₃ e usar colher; comparar gás com precipitado.
6. Explorar HCl + NaOH com indicador e depois HCl + MgO para observar óxido. Não exigir todas as receitas numa sessão.
7. Voltar ao Hub pelos comandos padrão.

Motion compartilhado em foco, encaixe aceito, rejeição, comandos e estados. Rejeição usa contorno e explicação, sem sacudir objeto segurado. Respeitar movimento reduzido e restauração ao ocultar. Não animar câmera, colliders ou posições de dosagem para dar feedback.

## 9. Modelo e limites

- Base local: 12 identidades, apresentação, estoques, estequiometria, produtos e evidências. Doze receitas fechadas, nenhuma API em execução.
- Conservação de átomos, doses, perdas, extensão estequiométrica, limitante/excesso. Toda transferência verifica par ativo **no domínio**, além do filtro XR; nenhuma chamada de ferramenta ou UI pode ignorar a regra.
- Capacidade de 500 mL; volume soma soluções aceitas, sem contração/deslocamento de sólidos. Excesso de dose é perda do reagente de entrada, como no modelo atual. CO₂ sai do sistema aberto e permanece contabilizado.
- Concentração fixa, quantidades finitas. Estimativa térmica somente para R3 e R8, conforme a seção 1. Sem cinética, solubilidade detalhada ou equilíbrios multicomponentes. R7, R9, R11 e R12 não prometem tempo real de dissolução nem efeito de aquecimento.
- R10 representa a estequiometria global de neutralização completa. Em doses parciais, a consulta deve indicar equivalentes ácidos/básicos restantes e o limite do modelo; não apresentar o balanço formal como identificação completa de espécies ou ignorar silenciosamente bissulfato/sulfato. Não calcular pH nem inventar curva de titulação com a hipótese de ácido monoprótico.
- pH ideal de ácido/base forte somente em R3. Não aplicar esse método a ácido acético, carbonatos ou MgO; R6 não recebe viragem inventada.
- Preservar transparência, wobble e calibração de nível. VFX apresenta estado, não calcula quantidades.
- Retorno de objetos soltos preserva estoques/par/mistura, sem duplicação. Reinício repõe estoques explicitamente; encaixar de volta não repõe. Nada teletransporta uma peça segurada.

## 10. Aceite e verificação da migração

A cena recebeu migração pontual da mesa, dos instrumentos e da interface, preservando ambiente, rig, béquer e recursos de líquidos/partículas. O modelo ampliado passou em 36 testes Edit Mode, incluindo os 66 pares distintos, receitas nas duas ordens e bloqueios por identidade. A integração usa instrumentos e encaixes XR em Play Mode; o roteiro e o relatório estão descritos no README técnico. Verificações anteriores dos três casos não substituem esses testes. Os itens abaixo definem o aceite contínuo; leitura, alcance e estéreo no dispositivo ainda exigem validação em headset.

- Migrar pontualmente a cena existente, preservando rig, referências, assets líquidos e ajustes autorais. Retirar antigas placas/casos no escopo aprovado; não regenerar a sala inteira.
- Verificar cada receita nas duas ordens de escolha e dosagem, alternância A/B, excesso, falta, estoque esgotado, sólidos e perdas; conservar cada elemento.
- Para 12 reagentes: verificar 66 pares distintos e 12 casos de mesma identidade. Exatamente 12 pares recebem receita; os demais têm recusa explicada. Conferir simetria A/B, pelo menos um parceiro para cada substância e três itens por categoria. Recusa não equivale a conclusão química.
- Testar ácido/ácido, B antes de A, retirada de A antes/depois de fechar par, terceiro reagente/colher e chamadas diretas ao modelo fora do catálogo. Nenhuma recusa consome estoque, emite fluxo ou contamina estado.
- Testar reinício/saída e cancelamento. Perda, colher e indicador contam como uso. Não reiniciar com objeto segurado. Fechar ficha não deixa raycast invisível.
- Comparar sete vistas reais com imagens aprovadas: nada diante do líquido, uma ficha, três comandos, mensagens sem duplicação. Capturas devem incluir falhas.
- Validar leitura sentado/em pé, XR direto/raio, partículas e transparência no headset. Sem build não solicitado.

## Referências científicas

- R1: [RSC — vinagre e bicarbonato](https://edu.rsc.org/primary-science/irreversible-changes-and-the-freaky-hand/4013614.article).
- R2: [University of Colorado Boulder — precipitação de CaCO₃](https://www.colorado.edu/lab/lecture-demo-manual/general-reactions/s130-chemical-rxns-precipitation-cacl2-na2co3-caco3).
- R3: [RSC — titulação HCl/NaOH](https://edu.rsc.org/concentration-of-solutions-and-titration/titrating-sodium-hydroxide-with-hydrochloric-acid/697.article).
- R4/R5: [UTSA — carbonato e bicarbonato com HCl](https://utsa.pressbooks.pub/chemistry1004/chapter/lab-7-reaction-stoichiometry-lab/).
- R7: [estudo experimental de dissolução de MgO em HCl](https://www.sciencedirect.com/science/article/pii/S0304386X03001142). A adaptação didática não reproduz a cinética experimental.
- R8/R10: [RSC — ácidos, bases e neutralização](https://edu.rsc.org/resources/acids-and-alkalis-problem-solving-challenges/840.article). As equações aqui são balanços globais, sem modelo de equilíbrio para o ácido sulfúrico.
- R9: [University of Wisconsin–Madison — neutralização, incluindo Mg(OH)₂ sólido](https://wisc.pb.unizin.org/minimisgenchem/chapter/m3q3-4-acids-bases-neutralization-and-gas-forming-reactions/).
- R11: [RSC — CuO e ácido sulfúrico](https://edu.rsc.org/experiments/reacting-copperii-oxide-with-sulfuric-acid/1917.article) e [UW–Madison — sólido em água versus formação de solução azul em ácido](https://demolab.chem.wisc.edu/aqueous-equilibrium-and-precipitation-reactions-solubility-of-copper-oxide-in-water-vs-acid/).
- R12: [estudo experimental de dissolução de ZnO em ácidos](https://www.jstage.jst.go.jp/article/matertrans/44/12/44_12_2489/_article/-char/en).

R6 aplica a neutralização global do ácido acético por NaOH, com estequiometria e conservação verificadas pelos testes do modelo; não calcula pH de ácido fraco. Fontes apoiam transformações; seleção A/B, limites operacionais e interfaces são decisões do design aprovado.
