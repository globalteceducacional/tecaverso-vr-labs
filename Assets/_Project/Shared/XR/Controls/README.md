# Controles 3D oficiais Unity

Os prefabs visuais desta pasta usam as malhas originais **Dial, Lever e Slider** do exemplo UI_3D do XR Interaction Toolkit. Materiais próprios: azul Tecaverso, prata e grafite. Fonte, revisão e licença estão em `Assets/ThirdParty/XRI3DUI/`.

Os modelos não foram remodelados. Os prefabs separam base e peça móvel e fornecem collider e ponto de pega. O pivô da manopla mantém o eixo esperado pelo XRKnob; a orientação da malha adapta a face ao suporte existente. Raízes dos controles, posição dos painéis e referências do experimento são preservadas.

## Aplicação nos laboratórios

| Laboratório | Revisão |
|---|---|
| Termologia | Dial oficial para o controle térmico, ligado ao ThermalKnob existente. |
| Ondulatória | Dois dials para amplitude/frequência; Lever para ligar/desligar pelo movimento; pegadores Slider no puxador e nos dois cursores. As réguas científicas continuam sendo o trilho dos cursores. |
| Associação de resistores | Lever oficial no interruptor existente, preservando eventos de abrir/fechar e reset do circuito. |
| Lançamento oblíquo, Plano inclinado e Gravitação | Sem manoplas/alavancas físicas genéricas equivalentes a substituir. Menus continuam uGUI; objetos científicos mantêm suas representações. |

`Unity3DControlVisuals` é uma ferramenta de autoria pontual e idempotente: substitui o visual e os colliders usados pelo interactor, sem regenerar cenas. Visuais anteriores ficam inativos para preservar referências autorais. Os builders de criação também aplicam os modelos oficiais.

Interações usam os componentes XRKnob/XRLever da Unity já presentes no projeto, com os adaptadores dos laboratórios. Ondulatória passou de alternância ao selecionar para eventos de ativação/desativação da alavanca. Selecionar sem mover não altera o estado. Pausa/modal bloqueiam a alavanca e reset devolve-a à posição desligada.

## Validação

Compilação e console sem erros. Em Play Mode: aquecimento, pausa e reset de Termologia; abrir/fechar/reset em Resistores; seleção sem alternância, movimento entre posições, pausa e reset da alavanca de Ondulatória usando interator XR temporário. Capturas do Editor conferiram os três aparelhos. Validação de alcance e conforto no headset permanece pendente; nenhum build foi gerado.
