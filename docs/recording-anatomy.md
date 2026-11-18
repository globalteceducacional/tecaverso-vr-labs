# Gravação — Corpo humano e sistemas

Demonstração automatizada no Editor com `com.unity.recorder`, em H.264/MP4, 1920 × 1080, 30 fps e sem áudio. A versão com inspeção do coração dura **3min27s**. Câmera, ponteiro e legendas pertencem à demonstração. A gravação inicial está identificada no histórico ao final.

## Roteiro

| Tempo | Ação observável |
|---|---|
| 00:00 | Visão da sala, corpo, console e navegação |
| 00:06 | Console: nove sistemas, pele, percurso e dial Unity |
| 00:10 | Ocultar pele para observar as estruturas |
| 00:12 | Respiratório: seleção XR do pulmão, consulta, ar em movimento, pausa e retomada |
| 00:46 | Circulatório: coração, consulta, circuito simbólico com oxigenação, pausa e retomada |
| 01:20 | Digestório: estômago, consulta, percurso em sentido único, pausa e retomada |
| 01:54 | Nervoso: encéfalo, consulta, sinal sensitivo ilustrativo, pausa e retomada |
| 02:28 | Esqueleto: comando de percurso oculto; dial e rotação ancorada até 180° e retorno |
| 02:43 | Muscular, em plano de corpo inteiro |
| 02:49 | Urinário, com enquadramento próximo das estruturas |
| 02:55 | Linfático, com a rede no referencial corporal |
| 03:01 | Reprodutivo masculino, com enquadramento próximo |
| 03:07 | Pega direta do coração em close. A peça se afasta do tórax e gira com o controlador |
| 03:17 | Soltura e devolução à pose original. Em seguida, volta ao respiratório com a pele visível |

Há sete segundos reservados para cada ficha dos quatro primeiros sistemas. A pausa é mostrada em close, e a retomada precede a próxima troca. As estruturas pequenas não usam o mesmo enquadramento do corpo inteiro. O roteiro não abre outras UIs ou muda a arquitetura da aplicação.

## Reprodução

Abra a cena `Assets/_Project/Laboratories/Biology/Anatomy/Scenes/Anatomy.unity` e entre em Play Mode. No MCP for Unity, fixado neste projeto, execute:

```csharp
PhysicsDemoRecorder.Begin("Anatomy", "Recordings/Biology/02-Corpo-humano-e-sistemas-inspecao");
```

Consulte `PhysicsDemoRecorder.Status` até `Complete Anatomy`. Não modificar scripts nem trocar de cena durante a gravação. Depois saia de Play Mode. Nenhuma câmera ou transformação do cenário é salva pela rotina. Os controles são acionados por eventos uGUI; as consultas usam seleção pelo XR Interaction Manager. O dial escreve na propriedade do próprio XRKnob, seguindo seu evento normal de giro.

`AnatomyDemo` verifica abertura das consultas, shader de percurso, etapa e fase congeladas em cada pausa, avanço após a retomada, visibilidade nos sistemas estáticos e posição ancorada da base. Na última sequência, um interator direto seleciona o coração. O roteiro verifica que a peça se deslocou e voltou à origem. A preferência de movimento é restaurada ao terminar. [Registro da execução](../GeneratedMedia/AnatomyVideo/roteiro-executado.txt)

## Revisão da saída

Com Node e as dependências/Chrome headless locais de `Tools/tecaverso-demo`, executar:

```powershell
node Tools/biology-migration/review-anatomy-video.cjs Recordings/Biology/02-Corpo-humano-e-sistemas-inspecao.mp4
```

O script usa o ffmpeg local para decodificar o vídeo inteiro. Para a versão com inspeção foram extraídos 38 quadros e revisadas três folhas de contato. As folhas e o relatório ficam em uma subpasta com o nome da gravação em `GeneratedMedia/AnatomyVideo`. A nova tomada integral mostra os nove sistemas, ondas de superfície e ângulos oblíquos após a retomada. Aos 3min12s e 3min15s o coração está na mão em duas orientações, e aos 3min18s está de volta ao corpo.

Gravações, quadros e relatórios são locais e ignorados pelo Git. Fontes do roteiro e da revisão ficam versionáveis; o MP4 não acompanha um clone. Não houve build para produzir a demonstração.

## Histórico da primeira demonstração

Arquivo local: `Recordings/Biology/02-Corpo-humano-e-sistemas-final.mp4`, H.264, 1920 × 1080, 30 fps, 5.819 quadros, 193,97 s. Decodificação integral sem erros e revisão visual dos 29 quadros/duas folhas de contato. Todos os sistemas aparecem; as quatro consultas e os quatro percursos têm pausa e retomada. As imagens do esqueleto em diferentes orientações confirmam o giro.

A entrega reúne a captura principal do Unity Recorder com o trecho a partir de 02:49 regravado pelo mesmo Recorder, para ampliar urinário/reprodutivo e deixar as estruturas acima da faixa de legendas. A montagem local usou ffmpeg; nenhuma imagem de órgão ou resultado foi sintetizada para substituir a execução. O código atual do roteiro já incorpora os enquadramentos corrigidos, permitindo uma nova tomada integral. Registro completo e registro do trecho final permanecem separados em `GeneratedMedia/AnatomyVideo`.
