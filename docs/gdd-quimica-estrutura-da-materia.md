# Short GDD — Estrutura da matéria

**Status:** proposta para aprovação; não implementado. **Disciplina:** Química. **Modo:** exploração individual offline. **Sessão sugerida:** 10–15 minutos.

## Objetivo e pergunta

Como prótons, nêutrons e elétrons determinam a identidade, a carga e a massa aproximada de um átomo? Ao final, o aluno distingue elemento, isótopo e íon, sem tratar o modelo de camadas como uma fotografia do átomo.

## Sala e objetos

- Um núcleo ampliado flutua no centro, ao alcance visual de vários pontos da sala. Áreas de encaixe das partículas ficam ao redor; um esquema eletrônico e a tabela periódica ficam próximos, sem bloquear a circulação.
- Dispensadores identificados por nome, símbolo e cor oferecem prótons, nêutrons e elétrons. As partículas são XR Grab Interactables; o núcleo e as camadas possuem sockets tolerantes à imprecisão da mão. Uma peça solta fora da área retorna ao dispensador.
- O modelo escolar de camadas e uma nuvem eletrônica estilizada são duas **representações alternativas** do mesmo estado, não dois estados físicos simultâneos. A escala entre partículas e núcleo é declaradamente didática.

## Interações e UI

1. O aluno pega um próton e o encaixa no núcleo; o modelo identifica hidrogênio. Outros prótons mudam o número atômico e o elemento.
2. Nêutrons alteram o número de massa, preservando o elemento; o aluno compara dois isótopos do mesmo elemento.
3. Elétrons ocupam posições do modelo de camadas; removê-los ou acrescentá-los altera a carga. A peça continua agarrável para correção.
4. Um seletor de duas opções alterna **Camadas** / **Nuvem didática**. Toggles independentes mostram contagem e identificação das partículas. **Limpar montagem** pede confirmação somente se houver trabalho; **Voltar ao Hub** segue o fluxo padrão.

Número atômico Z, número de massa A = prótons + nêutrons, carga líquida e nome do elemento aparecem ao lado do modelo e mudam imediatamente. Uma frase fixa esclarece: “Camadas são uma representação escolar; elétrons não percorrem estes círculos como planetas.” A UI não exige selecionar elemento antes de manipular o átomo.

## Modelo e limites

- Primeira versão: elementos leves com dados explícitos de número atômico, símbolo e distribuição escolar; definir a lista ao implementar. Não inventar um elemento quando a configuração exceder o catálogo.
- Z = número de prótons; isótopos têm o mesmo Z e números de nêutrons distintos; íons resultam de ganho/perda de elétrons. A leitura de massa é **número de massa**, não massa atômica média da tabela periódica.
- Não simular orbitais, spins, energias eletrônicas, estabilidade nuclear ou decaimento. A visualização eletrônica não representa escala, trajetória ou probabilidade quantitativa.

## Percurso e aceite

Montar dois isótopos de um elemento; manter o núcleo e produzir um cátion e um ânion; explicar qual alteração muda o elemento. Aceite: contagens e carga corretas em toda retirada/encaixe; nenhuma peça duplicada por socket; modelos alternativos mostram os mesmos dados; textos legíveis sem depender somente da cor; retorno e recuperação de peças funcionam em XR.

Reutilizar rig, sockets, grab interactables, TMP e identidade do [design system](../DESIGN_SYSTEM.md). Sem líquidos ou shader de frasco nesta sala. Validar lógica no Editor e alcance/leitura no headset separadamente; nenhum build sem pedido.
