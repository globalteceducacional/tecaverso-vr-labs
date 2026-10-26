# Termologia — GDD curto

**Estado:** implementado como laboratório individual em 26/10/2026; veja [uso, limites e validação](README.md).  
**Objetivo:** relacionar pressão, volume e temperatura de um gás ideal nas transformações isotérmica, isobárica e isométrica (isocórica), e interpretar o trabalho realizado pelo gás.

## Experiência central

O aluno entra em uma sala no padrão visual de Física e caminha ao redor de uma **câmara cilíndrica transparente com pistão**. Ela fica sobre uma base central, com altura próxima à do observador; há espaço livre para contorná-la. Partículas luminosas ocupam o interior. Um manômetro, um termômetro e uma régua de volume estão ligados fisicamente ao cilindro. Um painel próximo mostra o processo escolhido, os valores e um gráfico. A sala usa a geometria reutilizável do projeto, com iluminação suficiente para ler os instrumentos e observar as partículas.

A escala do aparelho é didática: o volume exibido em litros não corresponde ao tamanho físico da sala. Um número fixo de partículas visíveis representa uma amostra do gás, não todas as moléculas. A velocidade visual cresce aproximadamente com a raiz da temperatura absoluta; a densidade aparente muda com o volume. Pequenos pulsos nas paredes sugerem colisões, sem usá-las para calcular a pressão.

## Objetos e interações 3D/XR

| Objeto | Interação e resposta |
|---|---|
| Pistão e puxador | `XRGrabInteractable` com movimento restrito ao eixo do cilindro. O aluno o arrasta na isotérmica; os limites de volume são visíveis. Nos outros modos, o puxador não aceita arraste e o motivo aparece no painel. |
| Jaqueta térmica | Faixa ao redor da câmara que indica entrada ou saída de calor com luz quente/fria e setas discretas. Na isotérmica, representa o reservatório que mantém a temperatura constante. |
| Controle térmico | Manopla XR para aquecer ou resfriar nas transformações isobárica e isométrica. Mostra o valor-alvo em kelvin e °C, sem competir com outro controle de temperatura no painel. |
| Carga do pistão | Peso calibrado encaixado antes da isobárica para definir a pressão externa constante. Durante a execução, o pistão se move como consequência da mudança de temperatura. |
| Trava mecânica | Encaixa-se visualmente na isométrica e mantém o volume fixo; não pode ser solta enquanto o processo estiver rodando. |
| Instrumentos | Manômetro em kPa, termômetro em K/°C e régua em L respondem continuamente ao mesmo estado calculado. |

## Modos e controles de UI

O painel usa Montserrat, cores e componentes do [design system](../../../../../DESIGN_SYSTEM.md). Os três modos são **botões de seleção exclusiva**, não posições de um slider. A ação principal de cada modo fica evidente; controles que não se aplicam são recolhidos. Há **Iniciar, Pausar e Reiniciar**, mais **Voltar ao Hub** com o fluxo padrão.

| Modo | Ação do aluno | Grandeza mantida | Feedback principal |
|---|---|---|---|
| Isotérmica | Arrastar o pistão para comprimir ou expandir. | Temperatura | Pressão varia em sentido oposto ao volume; a jaqueta mostra a troca de calor necessária. |
| Isobárica | Escolher a carga antes de iniciar e girar a manopla térmica. | Pressão | O pistão se desloca e o volume acompanha a temperatura. |
| Isométrica/isocórica | Girar a manopla térmica com o pistão travado. | Volume | A pressão muda enquanto a régua de volume permanece fixa. |

O painel apresenta **P, V e T com unidades**, destaca a grandeza constante e oferece uma previsão curta antes da ação (“a pressão vai aumentar, diminuir ou permanecer igual?”). Após o movimento, confirma a tendência observada e permite repetir o ensaio. Um toggle opcional reduz partículas/efeitos sem alterar o cálculo.

**Trabalho de um gás é uma análise transversal**, não uma quarta transformação. O gráfico P × V registra a trajetória do processo e destaca a área correspondente a W = ∫ P dV. A convenção exibida é trabalho **positivo na expansão**, **negativo na compressão** e **zero na isométrica**.

## Modelo e limites do primeiro escopo

O estado físico vem de `PV = nRT`, com quantidade de gás fixa. Na isotérmica, `T` é constante e `W = nRT ln(Vf/Vi)`; na isobárica, `P` é constante e `W = P ΔV`; na isométrica, `V` é constante e `W = 0`. A temperatura do cálculo é sempre em kelvin. O fluxo de calor pode aparecer qualitativamente; valores numéricos de calor e energia interna exigem definir capacidade térmica e composição do gás em uma etapa posterior.

O `ParticleSystem` ou renderização instanciada apresenta as moléculas, mas **não é a fonte dos valores de P, V, T ou W**. Essa separação mantém gráficos e instrumentos coerentes mesmo com poucos elementos visuais em um headset standalone. O primeiro escopo cobre exploração individual e as três transformações; sincronização LAN, mudanças de quantidade de gás e outros processos ficam fora desta versão.

## Roteiro de uma sessão

1. Escolher o processo e prever o que muda.
2. Preparar o aparelho (carga, quando aplicável) e iniciar.
3. Manipular pistão ou controle térmico; caminhar ao redor para observar partículas, instrumentos e trava.
4. Pausar e comparar valores, trajetória no gráfico e sinal do trabalho.
5. Reiniciar e testar a transformação seguinte com condições iniciais equivalentes.
