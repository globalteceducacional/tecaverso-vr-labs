# Associação de resistores

Cena individual `Scenes/ResistorCircuits.unity`, acessível no Hub em Física → Associação de resistores. A sala usa o rig XR, a sala padrão, Montserrat, a paleta de Física e os ajustes de renderização comuns. A cena foi criada separadamente; os laboratórios anteriores não foram recriados.

## Montagem

O espaço contém uma bateria didática de 9 V com contatos de pressão, interruptor de alavanca, lâmpada com soquete e filamento, resistores axiais de 10, 20 e 30 Ω com faixas, dois blocos de derivação com barramento, amperímetro e voltímetro com visor, e 14 cabos com plugues. Bateria, resistores, instrumentos e ambas as pontas de cada cabo usam `XRGrabInteractable`. Terminais usam `XRSocketInteractor`; um socket aceita um plugue. Os blocos de derivação permitem unir vários condutores no mesmo nó. O interruptor usa o `XRLever` do pacote de exemplos XR já presente no projeto; mover a alavanca fecha ou abre o circuito. Os componentes ficam distribuídos em suportes pela sala, com corredor central livre.

Os corpos dos dez dispositivos e a alavanca são malhas **ProBuilder editáveis** na própria cena. Seus perfis, cantos, faixas, contatos e faces de materiais são gerados por `ResistorCircuitsMeshModels`; textos, terminais, colliders e componentes XR permanecem separados para interação. A cúpula da lâmpada é uma segunda malha para receber o brilho calculado sem iluminar o soquete. A geração por Tripo/Meshy/fal do MCP requer uma chave de provedor e não foi usada nesta versão.

O aluno conecta o positivo da fonte ao interruptor, componentes e lâmpada, e fecha o circuito voltando ao negativo. O amperímetro participa do ramo em série; o voltímetro é ligado entre dois nós, usando derivações quando os terminais já estão ocupados. **Soltar cabos** desfaz as conexões e devolve as pontas ao expositor. **Voltar ao Hub** pede confirmação.

Os cabos têm pontas físicas XR e uma curva visual flexível. Não simulam corda com joints. Apenas as conexões feitas nos sockets entram no cálculo; contato casual ou cruzamento visual não une condutores. Marcadores móveis representam o **sentido convencional da corrente** em cada cabo com corrente calculada, sem pretender reproduzir a velocidade dos elétrons.

## Modelo elétrico

`CircuitGraph` resolve os potenciais dos nós de um circuito resistivo DC por análise nodal. O modelo considera fonte ideal de 9 V, fios de 0,01 Ω, amperímetro de 0,01 Ω, interruptor ideal, resistores fixos e lâmpada de resistência constante de 12 Ω. A corrente acende a lâmpada proporcionalmente à potência no modelo. Correntes acima de 10 A disparam uma proteção virtual e desligam a visualização. A resistência real de uma lâmpada incandescente varia com a temperatura; esta simplificação é apresentada como modelo didático.

O controle da cena fica em `CircuitLab`; `CircuitPart` representa cada elemento de dois terminais e `CircuitTerminal` registra eventos de conexão XR. A geometria e os cabos não determinam os valores elétricos.

## Verificação

No Editor, a compilação passou sem erros. O cálculo retornou 0,15 A para resistores de 10, 20 e 30 Ω em série sob 9 V, e 0,90 A/0,45 A para 10/20 Ω em paralelo. Um circuito montado com eventos reais de socket acendeu a lâmpada e indicou cerca de 0,41 A no ramo de 10 Ω com a lâmpada em série; abrir o interruptor zerou a corrente e apagou a luz. O amperímetro em série leu 0,41 A e o voltímetro conectado pelos blocos de derivação leu 4,09 V no resistor. O reset soltou o plugue testado. O modal bloqueou o painel de fundo e o retorno chegou ao Hub com um rig. Cena reaberta com 10 componentes, 14 cabos e 24 terminais serializados; nenhum script ausente e nenhum erro no Play Mode final. A ergonomia e o desempenho precisam ser avaliados em headset; nenhum build foi gerado.
