# Ondulatória — GDD curto

**Estado:** implementado como laboratório individual em 26/10/2026; veja [uso, modelo e validação](README.md).

**Formato:** nova sala de Física, exploração individual em VR. Substitui a prévia “Ondas” no Hub, mantendo seis simulações na disciplina.  
**Objetivo:** relacionar amplitude, frequência, período, comprimento de onda e velocidade; observar propagação, reflexão e formação de ondas estacionárias.

## Experiência central

Uma corda ampliada, suspensa entre dois suportes, atravessa parte da sala. O aluno produz um pulso com a mão, liga um gerador periódico e observa como a perturbação se propaga e retorna. Pontos destacados permanecem ligados à corda: oscilam verticalmente enquanto a onda percorre o aparelho.

A pergunta inicial é **“O que percorre a corda: o material ou a perturbação?”**. A exploração evolui da ação manual para ajustes controlados e medições. O aluno pode caminhar pelos dois lados, pausar o fenômeno e examinar um ponto sem perder a visão do conjunto.

Referência visual: [storyboard aprovado](Storyboard.png). A imagem orienta ambiente e interações; escalas, rótulos e estados científicos devem seguir o modelo descrito neste GDD.

## Sala, objetos e interações XR

Comprimento inicial proposto da corda: **6–8 m**, com altura de equilíbrio próxima de **1,2 m**, ajustável para uso sentado. Reservar circulação lateral e acesso aos dois suportes. Reutilizar a sala padrão, ampliando-a apenas se necessário. Paredes peroladas, estrutura grafite, piso escuro e detalhes azuis; corda clara com brilho discreto e fundo que preserve seu contraste.

| Elemento | Interação e resposta |
|---|---|
| Gerador mecânico | Haste vertical transmite o movimento à corda. Interruptor físico liga/desliga a excitação periódica; desligar o gerador permite observar as ondas restantes. |
| Puxador azul | Pega XR restrita ao eixo vertical no modo Pulso. Ao levantar e soltar, produz uma perturbação; o retorno é suave e limitado. |
| Manoplas | Amplitude em metros e frequência em hertz, com valores junto aos controles. Disponíveis nos modos periódicos; não duplicar com sliders. |
| Suporte terminal | Fixo: grampo. Livre: anel que desliza verticalmente. Absorvente: módulo identificado que reduz o retorno. A escolha altera geometria e condição de contorno. |
| Corda e pontos de acompanhamento | Malha flexível contínua; pontos selecionáveis por raio XR, sem arrastar a corda fora do puxador. Selecionar um ponto revela seu gráfico de deslocamento × tempo. |
| Régua e dois cursores | Régua paralela à corda. Cursores XR deslizam ao longo dela e mostram a separação horizontal em metros; permitem medir entre cristas durante a pausa. |

## Experiências e controles

| Modo | Ação principal | Observação esperada |
|---|---|---|
| **Pulso e reflexão** | Puxar e soltar; comparar terminação fixa e livre. | Direção de propagação e pulso refletido: invertido na fixa, sem inversão na livre. |
| **Onda periódica** | Ligar o gerador e ajustar uma manopla por vez. Começa com terminação absorvente. | Amplitude muda a excursão; frequência altera o espaçamento entre cristas para o mesmo meio. |
| **Ondas estacionárias** | Usar terminação fixa e variar a frequência do gerador. | Interferência entre ondas incidentes e refletidas, nós e ventres próximos das ressonâncias. |

Painel World Space com Montserrat e cores do projeto. **Botões exclusivos** escolhem o modo; opções nomeadas escolhem a terminação quando aplicável. A configuração da terminação ocorre antes do ensaio. Trocar de modo durante um ensaio pede confirmação para reiniciá-lo.

Controles comuns: **Pausar/Continuar**, **Avançar um passo** durante a pausa, **Reiniciar** e **Voltar ao Hub**. Pausar congela o tempo físico, preservando medição, seleção e menus; puxador e manoplas ficam bloqueados com indicação do motivo. Reiniciar limpa ondas e gráficos, mantendo modo e ajustes escolhidos, com gerador desligado. Voltar ao Hub usa a confirmação padrão e bloqueia interações do aparelho enquanto o modal estiver aberto.

Auxílios opcionais: régua, pontos destacados e marcação de nós/ventres. Controles sem função no modo atual ficam recolhidos. Uma instrução curta orienta a próxima ação; não há formulário obrigatório de parâmetros nem avanço automático entre experiências.

## Informação e feedback científico

- Mostrar amplitude e frequência **do gerador**, velocidade do meio e período `T = 1/f`. Distinguir amplitude imposta e deslocamento observado, sobretudo na ressonância.
- Na onda periódica estabilizada, apresentar `v = λf`. Após mudar a frequência, indicar o transiente: a corda pode conter trechos produzidos com frequências diferentes.
- A régua mede distância espacial. O gráfico do ponto selecionado mede deslocamento no tempo. Identificar os dois referenciais explicitamente.
- Nas estacionárias, marcar nós e ventres somente quando houver padrão suficientemente estável. Durante o transiente, informar “formando o padrão”.
- Setas de propagação e movimento local são auxílios pontuais, sem réplicas temporais de objetos. Cores sempre acompanhadas de rótulos ou símbolos.

## Modelo e limites

Corda unidimensional idealizada, com deslocamento transversal, tensão e densidade linear constantes. Velocidade `v = √(Fₜ/μ)`. Amplitude não altera a velocidade no regime linear. A simulação deve propagar perturbações, refletir conforme a terminação e permitir superposição; não basta desenhar uma senoide uniforme para todos os modos.

Usar um modelo numérico estável de ondas, com passo físico independente da taxa de renderização. A malha representa esse estado; uma cadeia de joints não será a fonte da física. Amortecimento pequeno e explícito limita o crescimento na ressonância. A terminação absorvente é uma aproximação numérica, não uma extremidade livre.

Limitar amplitudes e frequências para manter estabilidade e leitura. As faixas finais serão calibradas com o comprimento e a velocidade escolhidos, incluindo pelo menos três padrões estacionários observáveis. No caso ideal de duas extremidades fixas, `fₙ = nv/(2L)` orienta a validação; o acoplamento do gerador deve ser considerado na comparação.

Primeira versão sem cuba de ondas, som, difração, mudança de meio ou sincronização LAN. Acessibilidade inclui ajuste de altura do aparelho e pausa sem movimentação da câmera.

## Roteiro do aluno

1. Produzir um pulso e acompanhar um ponto marcado.
2. Comparar o retorno em extremidades fixa e livre.
3. Ligar o gerador e variar somente a amplitude.
4. Aumentar a frequência, esperar estabilizar e medir entre cristas.
5. Buscar uma onda estacionária e identificar nós e ventres.
6. Pausar, comparar o perfil da corda com o gráfico de um ponto e repetir o ensaio.

## Critérios de aceite

- Pulso viaja com velocidade consistente; reflexão apresenta o sinal correto; absorção reduz o retorno.
- Frequência, período e comprimento de onda concordam dentro de tolerância documentada; o comportamento não depende da taxa de quadros.
- Nós não são marcados em posições móveis durante transientes; ressonância permanece limitada.
- Puxador, manoplas, seletores e cursores funcionam com XR; pausa, reinício, troca de modo e modal não deixam interações presas.
- Painéis não encobrem a corda; instrumentos ficam alcançáveis e rótulos legíveis em pé e sentado.
- Compilação, testes físicos e revisão visual no Editor; alcance, conforto e desempenho validados separadamente em headset. Builds somente mediante autorização.
