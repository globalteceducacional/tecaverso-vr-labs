# Frações e proporções

Sala individual de Matemática. Entrada: **Explorar sozinho → Matemática → Frações e proporções**. Cena: `Scenes/Fractions.unity`. [GDD](../../../../../docs/gdd-fracoes-e-proporcoes.md).

## Montagem e controles

As duas linhas A/B compartilham o mesmo inteiro: **1 inteiro = 1,2 m**. Cada linha comporta 2 inteiros (2,4 m), até 24 peças. Os nove dispensadores oferecem 1, 1/2, 1/3, 1/4, 1/5, 1/6, 1/8, 1/10 e 1/12. Retirar uma peça repõe o dispensador, respeitando o limite de seis peças soltas simultâneas.

Pegue uma peça com XR direto ou por raio e solte a até 0,65 m da ponta livre da linha. A prévia verde mostra a colocação final; comprimento e soma não dependem da tolerância de encaixe. Peças que excedem a capacidade são recusadas. Pegue qualquer peça encaixada para retirá-la; as seguintes se alinham preservando a ordem.

- **Exibir divisões:** marcas nas bordas de cada parcela, sem subdividir todo o inteiro em uma grade comum minúscula.
- **Decimal e porcentagem:** leituras relativas a um inteiro, incluindo 150% para 3/2; arredondamentos são marcados com ≈.
- **Equivalência:** liga os extremos de A/B e mostra igualdade ou comparação exata.
- **Desfazer:** restaura até dez alterações das linhas. Disponível após soltar as peças; não altera a receita.
- **Recolher peças soltas:** preserva peças encaixadas ou na mão. Solte uma peça na bandeja DEVOLVER para retorno imediato; peças abandonadas retornam após oito segundos ou ao cair abaixo do piso útil.
- **Limpar montagem:** confirmação; preserva a receita e permite desfazer.

A UI de opções fica na lateral da entrada. Resultados permanecem junto às linhas e à receita. Não há sliders de numerador/denominador nem seleção prévia de frações.

## Receita

Fichas A/B são contagens, não comprimentos. Encaixe no recipiente correspondente; até dez por categoria. A referência é 2 A para 3 B. Os botões 1×, 2× e 3× mudam a referência para 2:3, 4:6 e 6:9, sem preencher os recipientes.

A leitura diferencia “Mesma proporção” de “Quantidade da referência atingida”. Em 2:3, A representa 2/5 = 40% do total. Uma receita vazia exibe —; uma única categoria mostra 0%/100% e receita incompleta. Limpar receita pede confirmação e preserva as linhas.

## Arquitetura e reuso

- `FractionSession`: domínio independente de Unity; cada inteiro tem 120 unidades racionais, divisíveis por todos os denominadores oferecidos. Capacidade, igualdade e simplificação usam inteiros. Histórico limitado a dez snapshots de A/B.
- `FractionsLab`: comandos, pool, reposição, encaixes, organização das peças, resultados e confirmações. As séries permanecem somente em memória.
- `FractionPiece`: XRGrabInteractable com callbacks de pega/soltura, alinhamento após encaixe e recuperação de abandono. Peças físicas mantêm comprimento, colliders e rótulos.
- `FractionSocket`: XRSocketInteractor cuja seleção é autorizada ao soltar, após validar categoria e capacidade. O socket não captura uma peça que continua na mão; transfere a peça para a montagem, que permanece agarrável.
- Rig, locomoção, interação por raio, sala, renderização, botões/toggles e fontes vêm dos componentes existentes. Aproximação pelo analógico depende da configuração compartilhada do interator.
- `FractionsAuthoring` instala uma cena nova e recusa sobrescrever uma existente. Ajustes posteriores são pontuais; prefabs das onze peças ficam em `Prefabs/`.

Modo individual offline, sem LAN, persistência, líquidos ou operações gerais de multiplicação/divisão de frações. Voltar ao Hub confirma a perda da montagem quando há trabalho.

## Validação

Em 04/11/2026: **17 testes EditMode aprovados**, cobrindo igualdade, capacidade para todos os denominadores, frações impróprias, arredondamento, ordem, desfazer limitado, razão/participação e estados vazios. Play Mode: pega e reposição com interator XR temporário, soltura nos sockets, equivalência 1/2 = 1/4 + 1/4, receita 2:3, rejeição por capacidade, remoção/desfazer, recuperação e confirmações. Controles testados por raio XR sintético, leituras conferidas com 24 peças por linha e ida/volta pelo Hub com um único rig.

Capturas locais em `Temp/FractionsQA/`, ignoradas pelo Git. Testes no Editor não certificam ergonomia, leitura ou desempenho em headset; validação física, inclusive uso sentado, permanece pendente. Nenhum build gerado.
