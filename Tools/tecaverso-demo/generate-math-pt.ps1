$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot
New-Item -ItemType Directory -Force 'public/voiceover/math-francisca-source' | Out-Null
$scenes = Get-Content -LiteralPath 'math-narration.json' -Raw -Encoding UTF8 | ConvertFrom-Json
foreach ($scene in $scenes) {
  $target = 'public/voiceover/math-francisca-source/' + $scene.id + '.mp3'
  uv tool run --from edge-tts edge-tts --voice pt-BR-FranciscaNeural --rate=-5% --text $scene.text --write-media $target
  if ($LASTEXITCODE -ne 0) { throw ('Falha ao gerar ' + $scene.id) }
  Write-Output ('Gerado: ' + $scene.id)
}


