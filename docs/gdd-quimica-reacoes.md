# Short GDD — Reações químicas

**Status:** proposta para aprovação; não implementado. **Disciplina:** Química. **Modo:** exploração individual offline. **Sessão sugerida:** 12–18 minutos.

## Objetivo e pergunta

O que muda quando substâncias reagem e o que permanece conservado? O aluno relaciona evidências observáveis, reorganização de átomos, equação balanceada e reagente limitante em **reações selecionadas**.

## Sala e objetos

- Recipiente de reação transparente no centro, com marcas de volume e área livre para observação. Suportes laterais guardam frascos identificados, colher para sólido, tampa opcional e bandeja de retorno.
- Frascos e colher são XR Grab Interactables. O bico do frasco define a origem do fluxo; o recipiente possui volume receptor, sem exigir encaixe perfeito para cada gota. Um visor molecular fica ao lado do recipiente, sem cobrir o líquido.
- Shader Graph apresenta nível, transparência e oscilação do líquido. Partículas representam o jato, gotas, bolhas e precipitado conforme o caso; elas não fazem o cálculo químico. Cor, brilho e intensidade devem continuar legíveis em VR sem esconder o recipiente.

## Experiências iniciais

| Caso | Ação | Evidência | Equação/modelo |
|---|---|---|---|
| Vinagre + bicarbonato de sódio | Despejar ácido acético diluído e adicionar o sólido | Bolhas e CO₂ recolhido ou liberado | CH₃COOH + NaHCO₃ → CH₃COONa + H₂O + CO₂ |
| Precipitação de carbonato de cálcio | Misturar soluções de CaCl₂ e Na₂CO₃ | Formação e depósito de sólido | CaCl₂ + Na₂CO₃ → CaCO₃(s) + 2 NaCl |
| Neutralização de referência | Dosar HCl e NaOH com indicador | Leitura de pH e mudança do indicador | HCl + NaOH → NaCl + H₂O |

A neutralização aqui serve para comparar **tipos de reação**; a investigação quantitativa do pH pertence à sala [Ácidos, bases e pH](gdd-quimica-acidos-bases-ph.md).

## Fluxo, UI e regras

1. O aluno escolhe uma das três experiências por botões nomeados no suporte de reagentes. Apenas os frascos daquela experiência ficam operacionais; a seleção informa reagentes e objetivo.
2. Inclina um frasco sobre o recipiente. A transferência ocorre gradualmente enquanto bico, inclinação e posição permitem o despejo. Uma quantidade fora do recipiente é registrada como derramada e não entra no cálculo da mistura.
3. Adiciona o segundo reagente, observa o efeito e usa **Pausar / Continuar** para inspecionar o visor molecular. A visualização distingue reagentes presentes, produtos e excesso.
4. Ajusta coeficientes inteiros com botões − / + na equação. A contagem de átomos por elemento mostra os dois lados; o balanceamento é confirmado somente quando todos coincidem. O balanceamento não modifica retroativamente a mistura já dosada.
5. **Reiniciar experiência** restaura frascos, recipiente e cálculos, com confirmação se houver mistura. Trocar de caso requer reinício explícito. Retorno ao Hub segue a navegação padrão.

O modelo químico usa substâncias e quantidades cadastradas, calcula extensão máxima pela estequiometria e conserva a contagem de átomos. Definir unidades exibidas (mL, mol e concentração) e composições iniciais por caso; não inferir mol diretamente do volume sem concentração. Em sistema aberto, CO₂ pode sair do recipiente; a leitura de conservação deve considerar o gás liberado. O efeito visual não autoriza afirmar velocidade de reação ou energia térmica não modeladas.

## Limites e aceite

Misturas fora do catálogo não produzem reação inventada. Sem dinâmica molecular geral, cinética real, toxicologia ou todas as combinações da tabela. Reações e propriedades devem ser validadas antes de virar assets de dados. A [RSC documenta a produção de CO₂ com vinagre e bicarbonato](https://edu.rsc.org/primary-science/irreversible-changes-and-the-freaky-hand/4013614.article).

Aceite: volume transferido igual à mudança de nível; derramamento não entra no recipiente; nenhum efeito de precipitado/bolha em caso indevido; reagente limitante e excesso coerentes; equações balanceadas e contagem de cada elemento corretas; textos legíveis durante efeitos. Validar no Editor e em headset; nenhum build sem pedido.
