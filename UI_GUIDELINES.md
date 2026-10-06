# Diretrizes de UI — Tecaverso Labs

Versão 1.0 · 06/10/2026. Regras de implementação para pessoas e agentes de IA.
Leia também `DESIGN_SYSTEM.md` (identidade e tokens) e `README.md` (GDD e escopo).

## 1. Regras obrigatórias

- Usar uGUI, TextMeshPro e interação XR já instalada. Recursos próprios em `Assets/_Project/`.
- Preservar posição, rotação e escala autorais dos Canvases World Space, rig e instrumentos. Alterar layout interno não autoriza reposicionar a UI no mundo.
- Inspecionar cena, scripts, referências e prefabs antes de editar. Não regenerar cenas nem executar builders de instalação como atualizadores.
- Reutilizar componentes e ícones existentes. Manter fontes Montserrat e tokens do design system.
- Não alterar física, autoridade LAN ou eventos de domínio para resolver um problema visual.
- Não gerar builds sem pedido explícito. Validar com compilação do Editor, captura e testes focados.

## 2. Hierarquia e responsabilidades

Canvas World Space → páginas independentes → regiões (cabeçalho/conteúdo/rodapé) → grupos → componentes → visual.
Modais são irmãos das páginas, desenhados por último, com bloqueio de fundo.
Controllers possuem estado de navegação; componentes visuais não decidem permissões de rede.
Um componente deve ter um único responsável por posição e tamanho em cada eixo.

## 3. Catálogo de composição

| Caso | Organização | Overflow / cuidados |
|---|---|---|
| Formulário, parâmetros, menu lateral | VerticalLayoutGroup | Alturas mínimas; rolagem se exceder região |
| Linha de campo, ações, toolbar | HorizontalLayoutGroup | LayoutElement; espaçamento explícito; agrupar antes de reduzir fonte |
| Cards uniformes | GridLayoutGroup | Colunas explícitas; definir célula e espaçamento segundo a região |
| Lista longa | ScrollRect + grupo vertical | Viewport mascarado; pooling quando o volume justificar |
| Dropdown | TMP_Dropdown + template rolável | Área XR ampla; popup não pode ser cortado pelo painel; testar abertura/fechamento/foco |
| Tabela uniforme | GridLayoutGroup | Cabeçalho com mesmas dimensões de coluna |
| Tabela com colunas diferentes | Pilha de linhas horizontais | Compartilhar larguras das colunas; não calcular cada linha independentemente |
| Abas | Barra horizontal + páginas sobrepostas | Uma página visível/interativa por vez |
| Modal | Backdrop stretch + painel central | Bloquear página de fundo; ações em linha; foco explícito |
| Tooltip | Ancorado ao alvo com limites | Não depender de hover para funções essenciais; não interceptar ponteiro |
| Notificações | Pilha vertical em região reservada | Limite de quantidade; não encobrir ações |
| Accordion | Pilha vertical com conteúdo recolhível | Ocultar e recolher espaço são operações distintas |
| Slider | Linha rótulo/valor + controle abaixo | Não alterar geometria interna de Fill/Handle como layout de formulário |
| Toggle / seleção exclusiva | Linha horizontal; ToggleGroup se exclusivo | Rótulo clicável; ícone de estado além da cor |
| Teclado virtual | Reutilizar teclado XR existente | Layout por linhas se customização for necessária; nunca duplicar teclado global |
| Menu radial | Layout angular dedicado | Não forçar Grid/HorizontalLayoutGroup |
| Gráficos, vetores, réguas | Geometria calculada pelos dados | Não usar Layout Groups para representar posições científicas |
| Lista + detalhes | Regiões separadas + anchors | Larguras mínimas; não sobrepor ações ao rodapé |

Não criar componentes ainda não necessários só para preencher este catálogo.

## 4. Layouts e dimensões

- Usar LayoutElement para min/preferred/flexible. Espaçamento e padding seguem múltiplos de 8 quando a escala do componente permitir.
- Não escrever anchoredPosition/sizeDelta a cada frame em filhos controlados por LayoutGroup.
- Não colocar ContentSizeFitter no mesmo eixo já controlado pelo layout pai. Usá-lo apenas quando o conteúdo realmente define o tamanho, como um conteúdo rolável.
- GridLayoutGroup não possui reflow responsivo automático: declarar colunas/células ou implementar uma regra específica de redimensionamento.
- Não executar ForceRebuildLayoutImmediate em Update. Reconstrução explícita apenas na autoria, inicialização ou verificação pontual.
- Layouts podem manter dimensões físicas fixas em VR. “Responsivo” não significa reduzir tudo até caber em qualquer tamanho.
- Imagens de fundo/borda ficam fora do fluxo; stretch com insets. Ícones preservam aspecto.

## 5. Anchors e pivots

| Região | Regra |
|---|---|
| Página / backdrop | Stretch nos dois eixos; offsets zero ou margens |
| Cabeçalho | Stretch horizontal no topo; altura definida |
| Rodapé | Stretch horizontal embaixo; altura definida e região reservada |
| Conteúdo | Stretch na região entre cabeçalho e rodapé |
| Modal | Centro/centro, pivot 0.5/0.5, tamanho limitado ao painel |
| Conteúdo rolável | Topo/stretch horizontal, pivot no topo |
| Filho de LayoutGroup | Deixar o grupo dirigir os anchors; não disputar os valores |
| Instrumento no mundo | Referencial explícito do instrumento; não alterar com presets de UI |

Presets não corrigem uma hierarquia errada. Registrar margens e garantir que áreas de conteúdo e rodapé não se cruzam.

## 6. Visibilidade, interação e ciclo de vida

Usar `Tecaverso.UI.UIVisibility` nas páginas, abas, modais e ações condicionais:

- Visível: alpha 1, interactable true, blocksRaycasts true.
- Oculto: alpha 0, interactable false, blocksRaycasts false; limpar seleção dentro da região e cancelar feedbacks.
- Estado de navegação usa visibilidade explícita, nunca `activeSelf` como sinônimo de página aberta.
- Para itens opcionais em LayoutGroup, recolher também via LayoutElement.ignoreLayout. Para páginas sobrepostas, preservar o slot.
- CanvasGroup não interrompe Update, corrotinas ou inscrições em eventos. Controladores devem evitar trabalho desnecessário quando ocultos; não depender de OnDisable para fechar uma tela.
- Um modal bloqueia a interação do fundo, incluindo navegação por teclado/controlador, não somente o clique visual.
- `SetActive` permanece válido para rigs, ambientes 3D, pooling, backups aposentados e desligamento real de subsistemas. Não substituí-lo mecanicamente nesses casos.
- CanvasGroup afeta raycasts de UI, não colisores/raycasts físicos. Não adicionar colliders como atalhos aos controles uGUI.

## 7. Feedback e animação

- O RectTransform que participa do layout e a área clicável permanecem estáveis.
- Tween de escala atua em um filho `Visual`, com margem para a expansão. Não animar o tamanho controlado pelo LayoutGroup.
- Cancelar/restaurar tweens ao ocultar, desabilitar ou destruir; ocultar por CanvasGroup não chama OnDisable.
- Não deslocar câmera, posição de leitura, física ou foco para produzir feedback.
- Respeitar preferência futura de movimento reduzido. Priorizar animações curtas e discretas.

## 8. Texto, foco e XR

- Escolher explicitamente quebra, truncamento, paginação ou rolagem. Nunca usar overflow livre para conteúdo variável.
- Não reduzir fonte indefinidamente. Nomes longos e oito participantes devem caber na estrutura prevista.
- Textos e decorações usam raycastTarget=false. Alvos clicáveis usam uma área estável e sem sobreposição.
- Abas e menus ocultos não podem permanecer focados. Modais mantêm o fundo indisponível.
- Validar com ponteiro XR e teclado virtual; mouse no Editor não certifica usabilidade no headset.
- Proteger fronteiras: nickname máximo, mensagens de erro longas, valores/unidades, estados host/participante e mockups.

## 9. Procedimento de alteração e aceite

1. Registrar cena aberta, alterações existentes, transforms dos Canvases e estado do Editor.
2. Alterar apenas componentes em escopo; manter referências serializadas e listeners.
3. Migrar scripts que consultam activeSelf ou dependem de OnDisable antes de trocar visibilidade.
4. Confirmar compilação, navegação, modal e alternância de papéis/abas.
5. Conferir limites de RectTransforms e capturas: sem sobreposição de controles simultaneamente visíveis, cortes ou cliques invisíveis.
6. Verificar hover, conteúdo longo e ausência de mudanças nos transforms externos.
7. Salvar as cenas/prefabs alterados e registrar limites da validação. Não afirmar validação física sem testar hardware.

Referências: [Unity Auto Layout](https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/UIAutoLayout.html), [CanvasGroup](https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/class-CanvasGroup.html).
