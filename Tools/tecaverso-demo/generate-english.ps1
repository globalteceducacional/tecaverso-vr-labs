$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot
New-Item -ItemType Directory -Force 'public/voiceover/english-source' | Out-Null
$scenes = Get-Content -LiteralPath 'narration-en.json' -Raw -Encoding UTF8 | ConvertFrom-Json
foreach ($scene in $scenes) {
  $target = 'public/voiceover/english-source/' + $scene.id + '.mp3'
  uv tool run --from edge-tts edge-tts --voice en-US-JennyNeural --rate=-5% --text $scene.text --write-media $target
  if ($LASTEXITCODE -ne 0) { throw ('Falha ao gerar ' + $scene.id) }
  Write-Output ('Gerado: ' + $scene.id)
}
