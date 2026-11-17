# Short GDD — Eletroquímica: construindo uma pilha

**Status:** proposta para discussão, não implementada. **Disciplina:** Química. **Modo inicial:** individual offline. **Público:** Ensino Médio. **Data:** 11/11/2026.

## Objetivo e experiência

Pergunta central: como uma reação espontânea gera diferença de potencial e corrente em um circuito fechado? O aluno monta duas semicélulas, conecta a ponte salina e o circuito externo, mede a tensão e compara três pares de metais. Identifica oxidação no ânodo, redução no cátodo, elétrons no fio e íons na ponte. A sala investiga a **origem química da fonte**; Associação de resistores investiga o circuito alimentado por uma fonte pronta.

## Sala, objetos e catálogo

Área central de aproximadamente 5 × 4 m para montagem, com dois recipientes transparentes fixos a uma estação. Estante de peças à esquerda e placa de medição fixa junto à estação. Passagem livre ao redor; nada suspenso diante dos recipientes. Eletrodos, ponte, cabos e instrumento possuem suportes de retorno. A lâmpada e o voltímetro permanecem na estação; as pontas dos cabos são peças físicas, o cabo é visual flexível.

| Objeto | Quantidade/opções | Interação XR | Resultado/limite |
|---|---|---|---|
| Recipientes | 2 fixos, um por semicélula | Observar, selecionar a borda para consultar | Volume e concentração fixos nesta versão; não há mistura livre |
| Eletrodos | 2 Zn, 2 Fe, 2 Cu; identificados na própria peça | XR Grab + Socket de cada recipiente | Um metal por semicélula; não encaixa na solução de outro metal |
| Soluções preparadas | 2 cartuchos de ZnSO₄, 2 de FeSO₄, 2 de CuSO₄ | Trocar cartucho de solução em Socket, com rótulo físico | 1 mol/L nominal, 25 °C; sem dosagem ou transferência de líquido |
| Ponte salina | 1 tubo em U com eletrólito inerte | XR Grab; encaixe duplo entre recipientes | Fecha o caminho iônico; orientação indiferente |
| Cabos | 2 com pontas físicas | XR Grab nas pontas; encaixe entre cada eletrodo e um borne da estação | Cabo flexível acompanha as pontas; sem cadeia de joints em cada segmento |
| Circuito externo | 1 interruptor XR Lever e 1 lâmpada fixa, ligados internamente aos bornes | Acionar alavanca; conectar cabos | Lâmpada responde somente a circuito válido e fechado |
| Voltímetro | 1 instrumento fixo, ligado em paralelo aos bornes, com seletor físico de polaridade das ponteiras | Inverter ponteiras nos bornes positivo/negativo | Polaridade assinada; visor não é um painel solto |
| Carga didática | Elétrons no fio, íons na ponte e soluções | Toggle físico único Exibir fluxo | Movimento com sentido e legendas; não representa velocidades reais |

**Combinações válidas com semicélulas correspondentes:** Zn/Cu: E° = 1,10 V; Zn/Fe: 0,32 V; Fe/Cu: 0,78 V. Valores de referência arredondados calculados de potenciais padrão de redução, a 25 °C e atividades idealizadas. Inverter a posição física das semicélulas inverte o sentido espacial do fluxo, mas não cria outra reação. Inverter as ponteiras do voltímetro inverte apenas o sinal lido. Eletrodos iguais em semicélulas de mesma composição: diferença de potencial ideal nula e lâmpada apagada. Pares sem cartuchos correspondentes, ponte ausente ou circuito aberto têm estado incompleto; não se inventa uma reação. Catálogo futuro exige GDD e revisão científica.

## Inventário de interfaces e HUD

**HUD na câmera:** nenhum. **Painel genérico de parâmetros:** nenhum. Peças identificadas por inscrições na própria superfície. Só uma placa de medição fica visível durante a montagem.

| ID | Tipo/localização | Finalidade | Conteúdo/ação | Inicial | Abre/fecha | Simultâneo |
|---|---|---|---|---|---|---|
| R1 | Rótulos aderentes a peças/estante | Diferenciar metal e solução | Zn, Fe, Cu, soluções, ponte, cabos | Visíveis | Permanentes | Todos |
| C1 | Sockets visíveis nos recipientes e circuito | Guiar montagem | Foco, encaixe, remoção; forma e texto indicam correspondência | Visíveis | Permanentes | Todos salvo modal |
| C2 | Lever do interruptor, na estação | Abrir/fechar circuito | Estado ABERTO/FECHADO gravado no suporte | Visível, aberto | Alavanca alterna | R1, C1, M1, V1 |
| C3 | Toggle físico Exibir fluxo | Mostrar/esconder partículas didáticas | ON/OFF textual; não altera cálculo | Visível, desligado | Pressão | R1, C1, M1, V1 |
| M1 | Visor do voltímetro no instrumento | Ler diferença de potencial | Valor com sinal e unidade V, ou “—” durante montagem incompleta | Visível “— V” | Atualiza por evento | R1, C1, C2, C3, V1 |
| V1 | Placa pequena fixa ao lado da estação | Explicar estado e polos | Par, ânodo/cátodo, estado e próxima ação útil | Visível “Monte duas semicélulas” | Atualiza por evento | R1, C1, C2, C3, M1 |
| Q1 | Ficha opcional no suporte da estação | Consultar equações sem esconder célula | Oxidação, redução, equação global, E° e hipóteses; Fechar | Oculta | Selecionar placa V1; Fechar | Peças e M1; oculta V1 enquanto aberta |
| E1 | Mensagem local na placa V1 | Explicar montagem inválida | Falta ponte, cabo, cartucho compatível ou circuito aberto; ação de correção | Oculta | Surge ao tentar fechar/iniciar; some ao corrigir | R1, C1–C3, M1 |
| X1 | Reiniciar, botão físico na borda | Repetir comparação | Abre X2; não altera antes de confirmar | Visível | Pressão | R1, C1–C3, M1, V1 |
| X2 | Confirmação sobre a placa | Evitar perda da montagem | Cancelar/Reiniciar; consequência explícita | Oculta | X1 abre; escolha fecha | Bloqueia montagem e C2/C3 até decisão |
| S1 | Voltar ao Hub próximo à saída | Navegação | Voltar ao Hub | Visível | Permanente | Todos salvo X2 |

**Entrada:** duas células vazias, estante de peças, visor “— V” e placa com primeira ação. **Manipulação:** rótulos e medição permanecem junto à estação; sem ficha aberta. **Consulta:** Q1 substitui V1 e pode ser fechada sem alterar montagem. **Confirmação:** X2 ocupa V1, bloqueia os objetos e preserva a montagem ao cancelar. **Gravação futura:** câmera, legendas e ponteiro de demonstração não são UI da sala.

## Prévias visuais — propostas, não capturas Unity

Protótipo [HTML interativo](prototypes/eletroquimica/index.html), executável offline no navegador. Os seletores simulam os encaixes XR; ponte, cabos, alavanca, polaridade, fluxo, consulta e reinício são clicáveis. A lâmpada acende somente com circuito válido e diferença de potencial; elétrons e íons animados indicam sentidos, com representação estática sob preferência de movimento reduzido. As imagens abaixo foram capturadas dos estados do protótipo; modelos 3D, leitura XR e dimensões serão validados antes de implementar.

![Entrada: peças, células e instrução inicial](../GeneratedMedia/GDD/eletroquimica/entry.png)

Entrada, R1/C1/M1/V1/S1: pegar eletrodos e cartuchos; peças continuam disponíveis na estante.

![Montagem incompleta sem ponte salina](../GeneratedMedia/GDD/eletroquimica/incomplete.png)

Montagem incompleta, E1: tentar fechar sem ponte; aviso local indica a peça faltante, sem acender lâmpada.

![Pilha Zn/Cu completa](../GeneratedMedia/GDD/eletroquimica/complete.png)

Circuito completo, C2/M1/V1: encaixar ponte/cabos e fechar; polos, fluxo e tensão de referência aparecem.

![Consulta opcional das semirreações](../GeneratedMedia/GDD/eletroquimica/detail.png)

Consulta, Q1: selecionar V1, ler semirreações e fechar; montagem permanece.

![Confirmação de reinício](../GeneratedMedia/GDD/eletroquimica/reset.png)

Confirmação, X2: Cancelar mantém tudo; Reiniciar devolve peças aos suportes.

As capturas em `GeneratedMedia/` são ignoradas pelo Git e devem acompanhar o documento ao compartilhá-lo. O mockup não prova alcance ou legibilidade em headset.

## Fluxo XR

1. Escolher Zn/ZnSO₄ e Cu/CuSO₄ em R1; inserir nos respectivos C1. Foco dá realce discreto; encaixe produz feedback breve via Shared/Interaction.
2. Conectar dois cabos externos, ponte salina e ponteiras de M1. Tentar fechar sem ponte gera E1; completar a ponte remove o bloqueio.
3. Fechar C2: M1 indica tensão assinada conforme ponteiras, V1 identifica ânodo Zn e cátodo Cu; lâmpada acende. C3 revela elétrons Zn→Cu pelo fio e compensação iônica na ponte de KNO₃: NO₃⁻ rumo à semicélula de Zn e K⁺ rumo à de Cu. O sentido de corrente convencional é oposto ao dos elétrons.
4. Consultar Q1 e observar Zn→Zn²⁺+2e⁻ e Cu²⁺+2e⁻→Cu; Fechar retorna à estação. Reabrir C2 apaga lâmpada e pausa fluxo.
5. Trocar Zn por Fe e o cartucho correspondente para comparar Fe/Cu; inverter ponteiras de M1 para observar o sinal. Comparar Zn/Fe. Reiniciar por X1/X2 ou sair por S1.

Pega por proximidade para peças; raio XR para sockets distantes e consultas. Suportes tornam peças alcançáveis sentado ou em pé, sujeitos a teste físico. O feedback de foco/ação/estado usa Shared/Interaction; movimento reduzido remove pulsos decorativos, sem alterar o modelo químico. Sem movimento da câmera.

## Modelo, resultados e limites

Semirreações e potenciais ficam em dados locais. `E°cel = E°redução(cátodo) − E°redução(ânodo)`; fontes de referência: [OpenStax, células galvânicas](https://openstax.org/books/chemistry/pages/17-2-galvanic-cells) e [potenciais padrão](https://openstax.org/books/chemistry/pages/17-3-standard-reduction-potentials). Para Zn/Cu: [1,10 V nas condições padrão](https://chem.libretexts.org/Bookshelves/Analytical_Chemistry/Supplemental_Modules_%28Analytical_Chemistry%29/Electrochemistry/Redox_Potentials/Standard_Potentials). Zn/Fe e Fe/Cu são inferências da diferença entre potenciais padrão arredondados. A tensão no visor será **ideal didática**; resistência interna, queda sob carga, concentração variável e esgotamento não são simulados. A lâmpada indica circuito completo, sem prometer brilho proporcional a uma potência medida. Partículas e mudanças visuais nos eletrodos são esquemáticas e não alteram cálculo. Sem API em runtime, sem LAN nesta versão.

## Critérios de aceite

- Montar três pares válidos e receber tensão/polos coerentes; inverter ponteiras muda apenas o sinal.
- Ponte ou cabo ausente, solução incompatível, metal repetido e circuito aberto têm estados claros; lâmpada e fluxo não indicam corrente nesses casos.
- Encaixar/remover peças, abrir/fechar C2, alternar C3, consultar Q1 e cancelar/confirmar X2 preservam o estado esperado.
- A célula fica visível durante uso; painéis, rótulos e controles coincidem com inventário e imagens.
- Modelo e interações testados no Editor; alcance, leitura, frame rate e conforto avaliados no headset separadamente. Não gerar build sem autorização.
