# Short GDD — Ácidos, bases e pH

**Status:** proposta para aprovação; não implementado. **Disciplina:** Química. **Modo:** exploração individual offline. **Sessão sugerida:** 10–15 minutos.

## Objetivo e pergunta

Como a adição gradual de uma solução ácida ou básica altera o pH? O aluno mede, prevê uma faixa, dosa e relaciona o valor numérico à resposta de um indicador. Esta sala investiga **medição e neutralização gradual**, enquanto [Reações químicas](gdd-quimica-reacoes.md) compara diferentes tipos de transformação.

## Sala e objetos

- Um recipiente transparente central com escala de volume, frascos graduados identificados, conta-gotas ou bureta XR, agitador, amostras de indicador e medidor de pH agarrável com suporte próprio.
- Um gráfico vertical próximo ao recipiente registra pH medido versus volume adicionado. O instrumento mostra leitura numérica; uma faixa próxima informa a cor esperada do indicador selecionado. A representação molecular opcional mostra espécies relevantes do modelo, sem simular colisões individuais.
- O líquido usa nível e oscilação por Shader Graph; gotas e fluxo são efeitos visuais. A cor do indicador é derivada do pH calculado e de sua faixa cadastrada, não da quantidade de partículas renderizadas.

## Fluxo e interações

1. O aluno escolhe uma experiência cadastrada, mede o pH inicial colocando a ponta do medidor no líquido e seleciona um indicador identificado.
2. Dosando por conta-gotas, compara pequenas adições. A bureta oferece volume maior com leitura graduada; ambos transferem volume real do modelo ao recipiente.
3. Após cada adição, o gráfico acrescenta um ponto e o medidor atualiza sua leitura. O aluno pode usar **Pausar leitura** para examinar valor, cor e composição sem perder a mistura.
4. Um alvo opcional apresenta uma **faixa de pH**, não um único valor exato. O aluno tenta alcançá-la e explica por que excesso de reagente ultrapassa a faixa. **Reiniciar amostra** permite repetir com passos menores.

Controles: escolha exclusiva da experiência e do indicador, toggle **Exibir partículas**, **Pausar / Continuar**, **Reiniciar amostra** e **Voltar ao Hub**. O volume é dosado com o instrumento, não por slider duplicado. Um gráfico de resultados não recebe interação de arraste que altere a química.

## Modelo e limites científicos

- Primeira versão: uma neutralização forte–forte com soluções de concentração conhecida para cálculo quantitativo; indicador com faixa de transição cadastrada. Outros casos só entram com modelo químico e dados próprios.
- O cálculo considera mols de ácido/base efetivamente adicionados, volume total e faixa de pH válida do caso. Definir temperatura de referência e hipóteses do modelo na implementação. Nunca converter diretamente a cor do indicador em um pH exato.
- O indicador apenas informa uma faixa; o medidor apresenta o valor numérico do modelo. Em caso de medidor fora do líquido, exibir “Fora da amostra”, não congelar um número sem contexto.
- Fora do primeiro escopo: ácidos fracos, tamponamento, misturas universais, atividade iônica, calibração real do eletrodo e segurança de laboratório físico. Todos os reagentes são virtuais. A [RSC usa mudanças de cor de indicadores](https://edu.rsc.org/experiments/testing-acids-and-bases-on-a-microscale/564.article) e [dosagem gradual para estudar neutralização](https://edu.rsc.org/experiments/neutralising-an-acidic-solution/1756.article).

## Percurso e aceite

Medir a amostra inicial, adicionar base em etapas, anotar a faixa de viragem, ultrapassar o alvo e repetir com doses menores. Aceite: volume transferido e eixo do gráfico concordam; leitura pH não depende do VFX; indicador e número têm semânticas distintas; reinício limpa mistura e série; leitura permanece compreensível sem depender só de cor; medidor e frascos são acessíveis por pega e raio XR. Validar matemática e ergonomia separadamente; nenhum build sem pedido.
