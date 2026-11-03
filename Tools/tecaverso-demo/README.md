# Tecaverso Labs — demonstrativo

Edição Remotion em português: 1920 × 1080, 30 fps, 2715 quadros (90,5 segundos). Trilha ambiente tecnológica original, instrumental, estéreo, com sintetizadores suaves a 72 BPM. Narração em português brasileiro com a voz Francisca Neural, escolhida pelo usuário após comparar amostras. As seis gravações originais não possuem áudio.

## Prévia

Nesta pasta, execute `npm install` e `npm run dev`. Abra a composição `Tecaverso-Labs`. Cada laboratório também tem sua própria composição em `Cenas`, com cortes independentes na timeline.

As mídias estão em `public/media`. Para restaurá-las, execute `./prepare-media.ps1`. Esse comando copia as gravações e fontes do repositório, preservando os originais.

## Montagem

| Cena | Duração antes da transição | Intervalos usados do original (segundos) |
|---|---:|---|
| Abertura | 6 s | Gravitação: 4–10 |
| Lançamento oblíquo | 14 s | 1,5–7,5; 11,5–15,5; 21,5–25,5 |
| Plano inclinado | 12 s | 1,5–6,5; 12–19 |
| Gravitação | 13 s | 4–12; 19–24 |
| Associação de resistores | 13 s | 2–8; 18–25 |
| Termologia | 15 s | 2–7; 11–16; 21–26 |
| Ondulatória | 15 s | 2–6; 10–15; 22–28 |
| Encerramento | 6 s | Cartela editorial |

Sete transições de 0,5 segundo sobrepõem as cenas. Cortes internos preservam a velocidade das gravações, incluindo a câmera lenta dos dois primeiros experimentos. Títulos editoriais substituem a faixa inferior gravada. A edição não demonstra uso em headset nem multiplayer.

## Exportação

Quando solicitado, exportar com:

```powershell
npx remotion render Tecaverso-Labs out/Tecaverso-Labs-Demonstrativo-Francisca.mp4 --codec=h264 --crf=18 --audio-bitrate=192k
```

## Verificação e créditos

`npm run lint` verifica ESLint e TypeScript. Quadros de conferência ficam em `qa`. Não altera cenas Unity.

Texturas planetárias da gravação: Solar System Scope, https://www.solarsystemscope.com/textures/, CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/). A gravação foi recortada e recebeu sobreposições. Fonte Montserrat: licença em `public/fonts/OFL.txt`.

## Trilha sonora

`public/audio/tecaverso-ambient.wav`: composição sintetizada especificamente para este demonstrativo, sem samples de terceiros. Fonte reproduzível: `create-soundtrack.cjs`. A trilha possui fade de entrada de 3 s e saída de 6 s, com normalização para aproximadamente −23 LUFS antes dos fades.

A exportação com trilha reutiliza o fluxo H.264 do MP4 anterior sem recompressão e acrescenta AAC estéreo a 192 kb/s e 48 kHz. O projeto Remotion também referencia a mesma trilha para futuras renderizações. O arquivo anterior sem áudio foi preservado.

## Narração em português brasileiro

Roteiro e pontos de entrada: `narration.json`. Voz: Microsoft Maria, pt-BR, síntese local do Windows; não é uma gravação humana. Geração: `powershell.exe -NoProfile -File ./generate-narration.ps1`. Preparação e sincronização: `node prepare-narration.cjs`. Os trechos de fala são normalizados a −18 LUFS, com ajustes mínimos de velocidade apenas quando necessários para caber na cena.

A trilha usa ganho de 0,28 durante as falas e 0,65 nas pausas, com transições suaves. O projeto Remotion referencia a narração completa e a trilha ajustada; o MP4 final preserva a imagem Full HD sem recompressão. Versões anteriores permanecem em `out`.

## Versão atual — Francisca Neural

Voz `pt-BR-FranciscaNeural`, serviço online do Edge via edge-tts, ritmo −5%, igual à amostra aprovada. Gerar com `./generate-francisca.ps1` e preparar com `node prepare-narration.cjs --francisca`. O roteiro permanece em `narration.json`; os tempos medidos estão em `qa/francisca-narration-timing.json`. Todas as falas cabem nas respectivas cenas.

A composição Remotion usa `public/voiceover/francisca-tecaverso-narracao.wav` e `public/audio/francisca-tecaverso-ambient-ducked.wav`. A trilha baixa suavemente durante as falas e sobe nas pausas. Exportação: `out/Tecaverso-Labs-Demonstrativo-Francisca.mp4`. As versões anteriores foram preservadas.

## English edition

Composition: `Tecaverso-Labs-English`; export: `out/Tecaverso-Labs-Demo-English.mp4`. The shared scenes use `src/Locale.tsx` to translate all editorial titles, descriptions, notes and credits. Portuguese compositions and exports remain available. Text embedded in the laboratory recordings remains Portuguese.

Narration: Jenny Neural, US English, rate −5%. Script: `narration-en.json`. Generate with `./generate-english.ps1`, then `node prepare-narration.cjs --english`. Music ducking follows the English speech timings. Resolution: 1920 × 1080, 30 fps, duration 90.5 seconds.
