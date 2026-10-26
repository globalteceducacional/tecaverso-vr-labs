# Ondulatória

Laboratório individual em **Explorar sozinho → Física → Ondulatória**. Cena: `Scenes/Waves.unity`. Mantém seis conteúdos de Física no Hub, substituindo a prévia Ondas. Referências: [GDD](GDD.md) e [storyboard](Storyboard.png).

## Uso

- **Pulso:** escolha a terminação antes do ensaio, pegue o puxador azul e solte. Compare a reflexão fixa, livre e absorvente.
- **Periódica:** acione o interruptor junto ao gerador. Ajuste amplitude e frequência pelas duas manoplas XR. A terminação inicial é absorvente.
- **Estacionária:** o gerador excita a corda perto da extremidade esquerda, com ambas as pontas fixas. Experimente 0,25, 0,50 e 0,75 Hz. Espere o transiente diminuir para ver as marcações de nós e ventres.
- Pause para medir com os dois cursores laranja da régua. O botão Passo avança 0,05 s, mantendo a pausa. Selecione um ponto da corda para ver seus últimos oito segundos no gráfico, inclusive durante a pausa.
- Reiniciar limpa as ondas e desliga o gerador, preservando modo e ajustes. A troca de modo durante um ensaio pede confirmação. Voltar ao Hub também confirma o descarte.
- Réguas, pontos e marcações são opcionais. Baixar/Elevar ajusta a altura da montagem entre 0,85 e 1,55 m, sem mover a câmera. Painéis ficam ao lado do aparelho para preservar a vista da corda.

## Física e apresentação

Corda com comprimento de 6 m, tensão de 9 N e densidade linear de 1 kg/m: velocidade de 3 m/s. Modelo linear transversal com 241 amostras, espaçamento de 0,025 m e passo de 0,002 s. O número de Courant é 0,24. A malha tubular acompanha o estado calculado, sem joints nem senoide global imposta.

O integrador usa diferenças finitas centradas para `y_tt + 2γ y_t = c² y_xx`, com `γ = 0,10 s⁻¹`. A terminação fixa impõe deslocamento zero; a livre usa gradiente nulo; a absorvente usa a condição de saída de Mur de primeira ordem. A absorção é aproximada e o amortecimento reduz amplitudes ao longo do percurso.

Amplitude do acionamento: **0,03–0,20 m**. Frequência: **0,15–1,00 Hz**, arredondada em passos de 0,01 Hz. No modo periódico, o acionamento desloca a ponta esquerda. No estacionário, as pontas ficam fixas e o acionamento produz uma força localizada a 0,30 m da ponta esquerda: aceleração da amostra igual a 480 s⁻² vezes o deslocamento do acionamento. Assim, amplitude do gerador e amplitude observada na corda são diferentes. O movimento do acionamento retorna suavemente ao repouso quando desligado.

O pulso manual limita o deslocamento a ±0,30 m; ao soltar, o puxador retorna a 0,65 m/s. O cálculo usa tempo fixo e a renderização acompanha os estados. Frames superiores a 0,25 s têm recuperação limitada, desacelerando o tempo simulado para evitar travamentos prolongados.

Nós e ventres são encontrados nos mínimos e máximos da envoltória medida. Só aparecem após pelo menos 12 s e duas janelas estáveis, com duração de pelo menos dois períodos cada. A convergência pode levar dezenas de segundos. Alterar amplitude, frequência ou ligar/desligar o gerador invalida as marcações. Não há identificação de nós durante o transiente.

O gráfico mantém 201 estados amostrados a 25 Hz, permitindo selecionar outro ponto durante a pausa. Mostra deslocamento em metros, escala vertical adaptativa e últimos oito segundos. A régua mede separação horizontal; o aluno deve alinhar os cursores com cristas consecutivas. `λ = v/f` é uma referência de regime periódico, não uma medida automática durante transientes.

## Organização

- `Model/StringWaveModel.cs`: física independente de Unity.
- `Scripts/WavesLab.cs`: estado da experiência, comandos, tubo, envoltória e histórico.
- `WaveKnob`: reaproveita o XRKnob existente; `WaveSlide`: pegador e cursores limitados por eixo; `WavePoint`: seleção XR; `WaveSwitch`: XRLever com eventos de ativar/desativar pelo movimento.
- Manoplas, alavanca e pegadores usam as malhas oficiais Unity UI_3D e os prefabs compartilhados em `Assets/_Project/Shared/XR/Controls/`.
- `WaveTrace`: gráfico uGUI/TMP; painéis seguem os componentes e cores existentes.
- `WavesAuthoring`: autoria inicial com malhas ProBuilder e sala/rig reutilizados; recusa sobrescrever uma cena existente. Ajustes posteriores são feitos em componentes da cena.

Sem sincronização LAN, som, difração, cuba de ondas ou variação das propriedades do meio nesta versão.

## Validação

Oito casos EditMode de `StringWaveTests` passaram: chegada do pulso, sinais das reflexões, absorção, independência da partição de frames, três ressonâncias e relação entre velocidade, frequência, comprimento de onda e amplitude. Tolerâncias: chegada ±0,025 s; reflexão absorvente menor que 0,015 m para pulso de 0,20 m; comprimento de onda ±0,06 m; nós internos abaixo de 15% do ventre após estabilização.

Verificações em Play Mode, com interator XR temporário: seleção do puxador, liberação ao pausar, bloqueio da manipulação, interruptor, cursores disponíveis na pausa, passo de 0,05 s, manoplas, confirmação/cancelamento de modo, histórico por ponto, reset e retorno ao Hub. Painéis e aparelho revisados por capturas do Editor.

Alcance, conforto em pé/sentado e desempenho físico ainda precisam de validação com headset. Nenhum build foi gerado.
