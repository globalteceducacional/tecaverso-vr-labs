# Modelos atômicos e estrutura atômica

Sala individual offline: `Scenes/AtomicModels.unity`, disponível em Química → Modelos atômicos no Hub.

## Uso

- Percorra as seis estações históricas e o marco de Chadwick; selecione uma vitrine para abrir imagem local, proposta, evidência, avanço e limite. Fechar preserva a montagem.
- Na parede oposta, consulte elétron, próton e nêutron; os esquemas mostram quarks uud/udd e glúons.
- Pressione +/− para adicionar/remover prótons, nêutrons ou elétrons; manter selecionado repete. Limites: 20/24/20.
- Segure o modelo central com a seleção XR para girá-lo no lugar. Recentralizar restaura a orientação. O seletor Unity alterna Camadas/Nuvem.
- Limpar solicita confirmação e bloqueia os controles do mundo até Cancelar/Limpar. Voltar ao Hub fica junto à entrada.

## Implementação

`AtomicModelsLab` controla contagens, visuais e consulta. `AtomicControl` recebe seleção XR e repetição; `AtomicRotateInteractable` restringe a manipulação à rotação; `AtomicViewLever` conecta o XRLever. Modelos de botão PushButton e alavanca são dos exemplos oficiais Unity, com materiais locais. Feedback usa Shared/Interaction e respeita a preferência de movimento reduzido; órbitas científicas continuam animadas.

As imagens em `Art/` são ilustrações esquemáticas próprias, derivadas dos SVGs do protótipo aprovado, usadas pelo aplicativo e versionadas com seus `.meta`. Não são fotografias nem documentos históricos. Não há acesso de rede em runtime. As capturas de validação ficam em `GeneratedMedia/Validation/AtomicModels/`, ignoradas pelo Git.

## Limites científicos

Z=p; A=p+n; carga=p−e. Identificação H–Ca. Isótopos/íons montados não implicam estabilidade ou ocorrência natural. Z=0 não é elemento. Camadas 2/8/8/2 e inclinações são uma aproximação escolar, não configurações calculadas de todos os íons. A nuvem é qualitativa; não representa orbitais/probabilidades calculados. Tamanhos e velocidades não estão em escala real. Sem sincronização LAN.

## Verificação em 11/11/2026

Compilação sem erros. Play Mode: C12 neutro, C13 cátion, Ca40, limites inferiores/superiores, confirmação bloqueando ajuste/giro, cancelamento, limpeza, Camadas/Nuvem, zero prótons, órbitas em movimento, recentralização suave e reduzida, imagens locais e motion compartilhado. Eventos XR exercitados com interator temporário no Editor: seleção adiciona uma unidade, pressão mantida repete e giro de 25° rotaciona o átomo mantendo sua posição. Capturas conferidas da oficina e consultas de Rutherford e próton. A cena não contém componentes com script ausente. Avisos de scripts ausentes são emitidos na inicialização OpenXR (`FeatureHelpers.cs:332`, `XRGeneralSettings.AttemptInitializeXRSDKOnLoad`), nas configurações preexistentes preservadas; não há script ausente na hierarquia desta sala.

Validação repetível: abrir a cena em Play Mode e executar `Tecaverso.Editor.AtomicModelsAuthoring.Validate()`; reinicia a montagem em runtime. Não executar builders de instalação para atualizar a sala. Headset, alcance físico, interação de mãos, conforto e desempenho ainda pendentes. Nenhum build produzido.


## Bancada única — revisão ergonômica

A oficina foi concentrada em um tampo de aproximadamente 1,8 × 0,8 m, inclinado a 45°, usando a malha de mesa Unity já importada. Três grupos +/− ficam na faixa frontal; Recentralizar, seletor e Limpar ficam atrás. Resultados acima do tampo e átomo acima/atrás da leitura, com escala visual 0,45. A confirmação substitui temporariamente o resultado e bloqueia o restante. Os dois pedestais antigos estão desativados; museus, saída e rig foram preservados.

Verificados no Editor: montagem até Ca40, limites, alternância, confirmação/cancelamento e ausência de sobreposição entre os seis alvos. Alvos +/− com largura aproximada de 22 cm; inclinação e distância são pontos iniciais para teste no headset, não certificação de ergonomia. Captura local: GeneratedMedia/Validation/AtomicModels/bancada-final.png.

## Disposição lateral e vídeo — 11/11/2026

A bancada foi deslocada 1,5 m para o centro. A leitura agora é inclinada abaixo da linha do átomo. História ocupa a esquerda; partículas, a direita. A ficha usa a orientação da vitrine, permitindo consultas laterais sem abrir um painel diante da montagem. Rig e retorno ao Hub preservados.

Roteiro reproduzível: `AtomicModelsDemo` integrado ao `PhysicsDemoRecorder`, 112 s, 1920×1080, 30 fps, sem áudio. Usa seleção dos botões, XRLever e interator de giro temporário; resultados vêm do laboratório. Fluxo: C12, C13, cátion +1, nuvem, giro, recentralização, cancelar/confirmar limpeza, Ca40, limite, Rutherford, modelo quântico e próton. Legendas são somente da gravação e não foram adicionadas à sala.

Saída local ignorada: `GeneratedMedia/Recordings/AtomicModels/modelos-atomicos-bancada-central-final.mp4`. Validar enquadramentos da oficina e das duas alas antes de entregar; isso não certifica usabilidade em headset.

