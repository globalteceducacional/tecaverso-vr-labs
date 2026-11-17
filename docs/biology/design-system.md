# Aplicação do Design System à Biologia

**Proposta de 17/11/2026.** Complementa [DESIGN_SYSTEM.md](../../DESIGN_SYSTEM.md) e [UI_GUIDELINES.md](../../UI_GUIDELINES.md); não cria uma identidade concorrente. Os [seis GDDs](../gdd-biologia.md) determinam os controles de cada sala.

## Ambiente e apresentação

Sala tecnológica atual: painéis perolados, base grafite, piso escuro e acentos azuis. Acento de disciplina `#3ED39C` em faixa, suporte e seleção contextual. Ações primárias azuis `#284EA0`; consultas claras `#E8E6FE`, texto `#161616`, contorno `#07386F`; Montserrat local, cantos retos. Não importar céu, paredes, logotipo luminoso gigante ou materiais de ambiente da versão antiga.

O modelo é o protagonista. Órgãos, membranas e material genético conservam cores biológicas/didáticas distintas. Cada legenda esclarece ampliação e cores convencionais. Não usar o mesmo tamanho visual para insinuar proporção real entre célula, bactéria, vírus e corpo. Sem bloom sobre texto, pele ou tecido; iluminação neutra suficiente para ler relevo.

## Organização espacial

![Disposição espacial](../../GeneratedMedia/GDD/biologia/layout.png)

Referências iniciais de autoria: sala em torno de 8 × 8 m, circulação livre ≥ 1,5 m, suportes elevados a 1,1–1,3 m e controles frequentes a 0,8–1,25 m. Corpo humano ~1,7 m; modelos microscópicos ampliados para ~0,35–1,0 m conforme a tarefa. Consulta à lateral, nunca entre a cabeça e o modelo. Ajustes finais dependem de alcance/legibilidade em headset; não mover automaticamente cenas atuais para impor essas medidas.

Não há bancada obrigatória para toda sala. Células usa dois suportes; Anatomia um corpo central; Microrganismos galeria; DNA uma montagem; Ecossistemas e Evolução dioramas. Suporte inclinado pode reutilizar modelos Unity XR, preservando pivô, collider e eventos. A região de leitura não gira junto com uma organela na mão.

## Inventário compartilhado, sem painéis redundantes

Rótulo curto e fixo no suporte; destaque da estrutura selecionada; uma ficha lateral opcional com Nome/Função/Contexto/Limite/Fechar; uma linha local para instrução/erro; retorno padrão ao Hub. Confirmação apenas para ações que perdem montagem ou população. Anatomia não recebe reset/modal por hábito. Controle de movimento reduzido fica nas preferências padrão.

Peças trazem letra, nome ou símbolo na superfície. Seleção não depende só de hover; pode ser feita por toque ou raio. Um alvo oculto perde foco e deixa de disputar raycasts. Consulta não move o modelo nem a câmera. O aluno não precisa escolher um objeto em um painel antes de apontar para ele.

## Estados, motion e feedback científico

| Ação/estado | Visual | Som/haptic proposto | Regra |
|---|---|---|---|
| Foco | Contorno curto em 0,12–0,16 s | Opcional, sem som contínuo | Área de pega permanece estável |
| Consulta | Estrutura destacada; ficha abre lateralmente | Clique breve opcional | Uma ficha por vez; Fechar não reseta |
| Pega | Realce de pega e socket de retorno | Pulso breve | Não escalar a peça ou collider para “juice” |
| Encaixe aceito | Contorno de confirmação e acomodação breve | Pulso/clique único | Cientificamente correto antes de confirmar |
| Recusa | Contorno e texto do motivo; peça permanece na mão | Sinal curto distinto | Sem penalidade, sumiço ou reação inventada |
| Etapa/processo | Movimento dirigido e legendado | Opcional, sem exigir áudio | Pausável ou avanço por etapa |
| Restauração | Confirmação local e retorno ao estado inicial | Confirmação breve | Cancelar preserva; restaurar cancela tweens |

Reutilizar `ControlMotion`, `XRInteractionMotion` e `MotionPreferences.Reduced`; sem empilhar efeitos de DOTween. Preferência reduzida elimina pulsos, escalas e fades de feedback, mas não interrompe automaticamente o conteúdo científico. Para processos científicos, oferecer pausa/etapas. Sem oscilar câmera ou mover ponto de leitura.

Partículas: percurso respiratório/circulatório com símbolos/legendas distintos; etapas do fago com entrada/montagem/liberação; energia e matéria com caminhos separados no diorama. Não adicionar bolhas, fumaça ou faíscas a órgãos e células por decoração. Usar pooling, limites de emissão e ausência de partículas sobre texto.

## Proveniência e revisão

Preservar fontes e seus `.meta`; variantes para URP e malhas otimizadas terão rastreio. Cor de modelo não valida taxonomia; nomes como Plant_Cell_XX não autorizam inventar organela. O acervo bruto não comprova função, rig ou possibilidade de corte. Nada de recorte anatômico, fisiologia ou animação de articulação sem modelagem e validação próprias.

HTML é ferramenta de revisão: diagramas são esquemáticos, navegação superior e botões de “simular gesto” não integram a UI final. As imagens dos GDDs mostram estados propostos, não evidências da implementação Unity. Novas interfaces exigem atualização do inventário e das imagens antes da implementação.
