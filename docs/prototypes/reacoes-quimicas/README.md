# Prévia de Reações químicas

Prévia aprovada de UI e fluxo, revisada para mesa em degraus e termômetro; **não altera nem executa a sala Unity**. Doze reagentes (três ácidos, três bases, três sais, três óxidos), 12 receitas cadastradas, seleção física A/B, dosagem do par, consulta opcional e recusa de terceiro reagente. Ver [GDD](../../gdd-quimica-reacoes.md).

Abra `index.html` em um navegador. Use os sete botões superiores para ver estados ilustrativos; essa navegação e as legendas não pertencem à aplicação. Não é um simulador químico ou uma reprodução do shader. Os valores da ficha exemplificam 100 mL de CaCl₂ + 150 mL de Na₂CO₃, ambos a 0,10 mol/L (aproximadamente 1,00 g de CaCO₃ e excesso de 5,0 mmol de carbonato).

O HTML usa apenas JavaScript/SVG/CSS locais e as fontes Montserrat já versionadas em `Assets/_Project/UI/Fonts/Montserrat/`. Não depende de CDN nem de servidor.

## Capturar as imagens do GDD

Na raiz do repositório:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File docs/prototypes/reacoes-quimicas/capture.ps1
```

O script usa o Chrome Headless Shell já disponível no ambiente local de `Tools/tecaverso-demo/node_modules/.remotion/`. Essa dependência ignorada não está garantida em clones. Para usar uma instalação equivalente existente, informe `-BrowserPath C:/caminho/chrome-headless-shell.exe`. O script não baixa dependências, usa somente HTML próprio e gera PNGs de 1600 × 1000. Fecha o processo de cada captura automaticamente.

Saída: `GeneratedMedia/GDD/Reacoes/01-entrada.png` até `07-temperatura.png`, ignorada pelo Git. Fonte e script são versionáveis. Um clone não contém as imagens: regenere-as localmente ou inclua-as junto do GDD ao exportar/compartilhar. Confira existência e leitura das sete imagens após executar.

As imagens precisam ser revisadas para não encobrir o recipiente ou cortar botões. Mockup não valida tamanho angular, leitura estéreo, alcance ou conforto em headset.
