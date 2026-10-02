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
- Para cada alteração, faça a menor validação que cobre o risco: compilação e console sempre; Play Mode para comportamento; testes automatizados apenas para regras críticas ou regressões prováveis; builds somente quando solicitadas.

## Movimento oblíquo

O MVP separa `ProjectileKinematics` (equações), `ObliqueLaunchSimulation` (estado e tempo), views de projétil/vetores/gráfico/réguas, pool de snapshots, painel e `ObliqueLaunchLab` como fachada coordenadora. A massa é exibida e aplicada ao Rigidbody representativo, mas não altera a trajetória cinemática ideal sem resistência do ar, reforçando o conceito físico correto.

### Orientação espacial padrão

- Os experimentos são apresentados lateralmente, com o movimento principal ocorrendo da esquerda para a direita no campo de visão do aluno.
- O XR Origin deve ficar voltado para a área do experimento.
- Canvas e demais interfaces espaciais devem ser orientados para a posição dos olhos do jogador, sem depender de rotações fixas específicas da cena.
- Geradores de cena devem preservar ajustes autorais sempre que possível e nunca reaplicar uma orientação fixa sobre uma UI já posicionada manualmente.
