# Termologia

Sala individual acessível em **Explorar sozinho → Física → Termologia**.
Cena: `Scenes/Thermology.unity`. Proposta aprovada: [GDD](GDD.md).

## Uso

1. Escolha Isotérmica, Isobárica ou Isométrica. Opcionalmente registre uma previsão sobre a pressão.
2. Na isobárica, pegue uma carga de 90, 110 ou 130 kPa e encaixe-a sobre o pistão antes de iniciar.
3. Pressione Iniciar. Na isotérmica, arraste o puxador azul verticalmente. Nas demais, gire a manopla térmica.
4. Observe partículas, instrumentos, gráfico P × V e trabalho acumulado desde o estado inicial.
5. Pause para observar, continue ou reinicie para mudar de transformação. Voltar ao Hub pede confirmação.

Reiniciar devolve cargas soltas ao suporte; a carga encaixada permanece. Trocar de transformação libera o encaixe e restaura todas as cargas. Há uma opção de reduzir as partículas de 96 para 32.

## Modelo e limites

- Gás ideal fechado, n = 1 mol, PV = nRT; cálculos com temperatura absoluta.
- Isotérmica: T = 300 K, V entre 12 e 45 L; W = nRT ln(Vf/Vi).
- Isobárica: P definida pela carga; T entre 250 e 450 K, V = nRT/P; W = P ΔV.
- Isométrica/isocórica: V = 25 L; T entre 250 e 450 K; W = 0.
- Trabalho positivo na expansão, negativo na compressão; kPa × L = J. O gráfico representa o trabalho líquido desde o início do ensaio.
- Transformações idealizadas quase estáticas. A temperatura aproxima-se do alvo a 35 K/s para permitir observação.
- Partículas são uma amostra visual, com velocidade proporcional a √T, reflexão nas paredes e pulsos nas colisões. Não calculam pressão por colisões, não reproduzem distribuição molecular nem escala temporal real.
- Jaqueta e setas indicam o sentido qualitativo do calor; não há calorimetria quantitativa.
- Aparelho em escala didática. As cargas indicam pressão externa efetiva; não são massas em quilogramas.
- Não implementa sincronização LAN. A infraestrutura XR e o ambiente reutilizam o padrão dos laboratórios individuais.

## Componentes

`Model/IdealGasModel.cs` contém a física sem dependência de Unity. `ThermologyLab` coordena estado, UI e instrumentos. `ThermalPiston`, `ThermalKnob`, `ThermalLoad` e `ThermalLoadSocket` controlam as interações XR. `GasParticles` representa moléculas e `GasPVGraph` desenha o gráfico.

O aparelho usa malhas ProBuilder de revolução, materiais próprios e a sala padrão. `ThermologyAuthoring` cria somente a cena inicial e recusa sobrescrever uma cena existente; ajustes posteriores devem preservar a cena autoral.

## Validação em 26/10/2026

- Seis testes EditMode de `IdealGasTests` passaram: invariantes, trabalho, unidades, limites e entradas inválidas.
- Verificações em Play Mode: três transformações, encaixe de carga via XRI, pausa, bloqueio pelo modal, cancelamento e retorno ao Hub.
- Compilação e console sem erros após as correções; revisão visual dos painéis e aparelho no Editor.
- A manipulação física, alcance dos controles e legibilidade ainda precisam de validação com headset. Nenhum build foi gerado.
