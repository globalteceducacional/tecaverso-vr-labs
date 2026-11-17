# Protótipos de Biologia

Propostas de 17/11/2026, ainda não aprovadas nem implementadas em cenas do projeto atual. Abrir [index.html](index.html) no navegador: seis salas navegáveis, sem servidor, CDN ou API. Montserrat é carregada da pasta de fontes local do projeto. Diagramas SVG são representações para testar escolhas e estados; não são renders dos FBX antigos.

## O que experimentar

| Sala | Fluxo representativo | Caso de recuperação |
|---|---|---|
| Células | Selecionar mitocôndria; abrir “Como funciona em RV?”; simular separação, giro e reencaixe | Separar sem organela selecionada é recusado; recompor/cancelar |
| Corpo humano | Alternar nove sistemas, selecionar estrutura, mostrar pele e observar/pausar quatro percursos | Troca de sistema fecha consulta de alvo oculto; partes estáticas sem percurso inventado |
| Microrganismos | Consultar bactérias, vírus e estruturas; avançar três etapas do fago | Modelos próximos não criam infecção; restaurar/cancelar |
| DNA/RNA | Clicar no socket e na base da bandeja; completar DNA e RNA independentemente | Base incompatível preserva a fita; alternar preserva montagem |
| Ecossistemas | Encaixar capim → gafanhoto → sapo; exibir fluxo e consultar matéria/energia | Papel errado mantém a ficha disponível |
| Evolução | Guardar referência, avançar gerações, alternar ambiente e comparar | Ambiente não altera indivíduos imediatamente; seis gerações é limite |

O cabeçalho, os links de troca de sala e os comandos tracejados são navegação e proxies de gestos da revisão HTML. Não representam painéis adicionais da aplicação. O giro comprime a projeção do desenho para comunicar mudança de orientação; não valida a rotação de uma malha 3D em XR. Seis páginas usam o mesmo arquivo com parâmetros `?lab=celulas|anatomia|microbios|dna|ecossistemas|evolucao`.

Os estados `entry`, `action`, `detail`, `error` e `reset` podem ser pré-carregados por `&state=...` para revisão visual. O aviso de erro pré-carregado é uma prévia de mensagem; os fluxos acima permitem provocar recusas reais. Anatomia não possui reset destrutivo no produto.

Para revisar no navegador por localhost, executar `node Tools/biology-migration/serve-prototype.cjs` na raiz e abrir o endereço impresso. O servidor aceita somente leitura dos quatro arquivos do protótipo e das fontes locais; não expõe as demais pastas do projeto. Não é dependência da aplicação Unity ou do HTML aberto como arquivo.

## Capturas e validação

Fonte: `index.html`, `app.js`, `style.css`, `layout.html`. Prévia espacial: [layout.html](layout.html). Capturas em `GeneratedMedia/GDD/biologia/`, ignoradas por Git. Instruções e teste de interação estão em `Tools/biology-migration/verify-prototype.cjs`; requer a instalação já existente em `Tools/tecaverso-demo/node_modules/@remotion/renderer` e seu Chrome Headless Shell. Nenhuma dependência nova é necessária. Esse script verifica o HTML; não compila Unity nem cria build.

Aceite em headset, raycasts, manipulação física, hierarquia semântica dos modelos, materiais URP e custo de renderização permanecem etapas da implementação. Respeitar a distinção entre preferência de motion (feedback) e animações que ensinam processos.
