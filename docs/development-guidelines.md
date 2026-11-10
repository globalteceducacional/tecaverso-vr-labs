# Diretrizes de desenvolvimento do Tecaverso Labs

Estas diretrizes adaptam ao projeto os princípios do guia da Unity sobre SOLID e padrões de projeto.

- Comece com KISS. Um padrão só entra quando resolve um problema real e recorrente.
- Cada componente deve ter uma responsabilidade e um motivo claro para mudar. Separe cálculo, estado, apresentação, entrada e persistência.
- Mantenha a física e as regras centrais em C# independente de GameObjects quando possível. Isso torna os cálculos legíveis e reutilizáveis.
- Dependa de contratos pequenos e dados explícitos. Evite localizar dependências repetidamente com `Find*` durante a simulação.
- Prefira composição de componentes Unity a hierarquias profundas de herança.
- Abra extensão por configuração, prefabs e estratégias quando houver variações reais; não antecipe variações hipotéticas.
- Use eventos para mudanças discretas de estado. Remova inscrições simetricamente e evite cadeias de eventos difíceis de rastrear.
- Use máquina de estados quando transições são parte do domínio, como Idle, Running, Paused e Complete.
- Use pooling para objetos criados repetidamente durante a experiência, como amostras temporais e efeitos.
- Use MVP ou separação equivalente em telas com lógica relevante: modelo sem UI, view passiva e um coordenador explícito.
- Use ScriptableObjects para dados compartilhados de configuração quando houver múltiplas cenas ou variantes. Não use como estado global mutável por conveniência.
- Evite singletons novos. Quando um serviço global for inevitável, limite sua API e ciclo de vida.
- Otimize depois de medir. Dirty flags, flyweights e DOTS só entram quando o perfil justificar a complexidade.
- Prefira nomes que expressem intenção, campos serializados privados e namespaces por laboratório.
- Para cada alteração, faça a menor validação que cobre o risco: documentação exige revisão de conteúdo, links e diff; scripts exigem compilação e console; Play Mode para comportamento; testes automatizados apenas para regras críticas ou regressões prováveis; builds somente quando solicitadas.

## UI conforme a atividade

Leia [UI_GUIDELINES.md](../UI_GUIDELINES.md) e [DESIGN_SYSTEM.md](../DESIGN_SYSTEM.md) antes de modificar interfaces. Reutilize identidade e componentes; a tarefa define se um painel, slider ou fluxo de edição é necessário.

O Espaço cartesiano tem ferramentas de construção e edição. Formas geométricas é uma galeria de observação: nomes e fórmulas junto às peças, três toggles independentes e retorno ao Hub. Não reintroduzir estudo A/B, abas ou campos numéricos na galeria por copiar a estrutura do outro laboratório. O GDD específico aprovado delimita o escopo.

<a id="midias-geradas"></a>
## Mídias geradas e versionamento

As regras ficam no [`.gitignore`](../.gitignore). Use os destinos abaixo para saídas locais:

| Saída | Destino |
|---|---|
| Gravações Unity Recorder | `Recordings/`, incluindo `Recordings/Physics/` |
| Capturas do Editor/MCP e validações | `Temp/`, `tmp/`, `Captures/` ou `Screenshots/` |
| Storyboards, conceitos e outras prévias geradas | `GeneratedMedia/` |
| Capturas que precisem ser importadas no Editor | `Assets/Screenshots/`, `Assets/Captures/`, `Assets/Recordings/` ou `Assets/GeneratedMedia/` |
| Exportações e quadros da demonstração Remotion | `Tools/tecaverso-demo/out/` e `Tools/tecaverso-demo/qa/` |
| Cópias de gravações, trilhas e narrações da demonstração | `Tools/tecaverso-demo/public/media/`, `public/audio/` e `public/voiceover/` |

As pastas de saída sob `Assets/` e seus `.meta` de pasta são ignorados. Não referencie esses arquivos locais em cenas, prefabs ou materiais que precisem funcionar em outro checkout.

Texturas, ícones, malhas, materiais e áudio usados pelo aplicativo são dependências do projeto, mesmo quando gerados com auxílio de IA: devem ficar em suas pastas definitivas e ser versionados com seus `.meta`, licenças e atribuições pertinentes. Não adicionar regras globais para `*.png`, `*.jpg`, `*.mp4`, `*.wav` ou `*.mat`.

Versionar os scripts de geração, roteiros e configurações permite reproduzir as saídas. As gravações locais precisam existir para montar o demonstrativo; um clone do repositório não as recupera automaticamente.

Antes de commitar, conferir `git status --short`, `git diff --cached --check` e usar `git check-ignore -v -- caminho` para verificar a regra aplicada. Para arquivos já rastreados, `git check-ignore --no-index -v -- caminho` verifica somente a regra. `.gitignore` não retira arquivos do índice nem apaga o histórico: imagens antigas já versionadas, inclusive em `tmp/`, permanecem rastreadas até uma limpeza específica. Não reescrever histórico automaticamente.

Ao revisar assets grandes, conferir `git lfs fsck --pointers` e `git lfs fsck --objects`. Mídia de demonstração deve ser ignorada, não enviada ao LFS como alternativa. As verificações são locais e não comprovam disponibilidade dos objetos no remoto.

## Movimento oblíquo

O MVP separa `ProjectileKinematics` (equações), `ObliqueLaunchSimulation` (estado e tempo), views de projétil/vetores/gráfico/réguas, pool de snapshots, painel e `ObliqueLaunchLab` como fachada coordenadora. A massa é exibida e aplicada ao Rigidbody representativo, mas não altera a trajetória cinemática ideal sem resistência do ar, reforçando o conceito físico correto.

### Orientação espacial padrão

- A apresentação lateral, da esquerda para a direita, é própria do lançamento oblíquo. Escolher a disposição conforme o fenômeno: sistema solar horizontal, espaço cartesiano tridimensional e galeria com circulação entre sólidos.
- O XR Origin deve ficar voltado para a área do experimento.
- Canvas e demais interfaces espaciais devem ser orientados para a posição dos olhos do jogador, sem depender de rotações fixas específicas da cena.
- Geradores de cena devem preservar ajustes autorais sempre que possível e nunca reaplicar uma orientação fixa sobre uma UI já posicionada manualmente.

## Aceite de motion

Toda nova interação deve aplicar a diretriz de motion do Design System. Testar normal, hover/foco, ação, saída, ocultação, desabilitação e preferência reduzida. Confirmar que transforms/colliders físicos e valores científicos não mudam pelo feedback. Reutilizar `Shared/Interaction`; não adicionar loops decorativos em todas as peças. Estado selecionado precisa continuar evidente sem animação.

`InteractionMotionVerification.Start()` no Editor em Play Mode percorre as cenas de laboratório habilitadas e registra cobertura de Selectables e interactables em `Temp/interaction-motion-verification.txt`; não salva nem regenera cenas. Esse teste não substitui interação XR real ou medição de desempenho no dispositivo.
