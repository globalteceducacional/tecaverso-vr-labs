$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Runtime.WindowsRuntime
[Windows.Media.SpeechSynthesis.SpeechSynthesizer, Windows.Media.SpeechSynthesis, ContentType=WindowsRuntime] | Out-Null
$asTask = [System.WindowsRuntimeSystemExtensions].GetMethods() | Where-Object { $_.Name -eq 'AsTask' -and $_.IsGenericMethod -and $_.GetParameters().Count -eq 1 -and $_.GetParameters()[0].ParameterType.Name -eq 'IAsyncOperation`1' } | Select-Object -First 1
$synth = New-Object Windows.Media.SpeechSynthesis.SpeechSynthesizer
$synth.Voice = [Windows.Media.SpeechSynthesis.SpeechSynthesizer]::AllVoices | Where-Object { $_.Language -eq 'pt-BR' -and $_.DisplayName -match 'Maria' } | Select-Object -First 1
$synth.Options.SpeakingRate = 1.10
$scenes = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'narration.json') -Raw -Encoding UTF8 | ConvertFrom-Json
foreach ($scene in $scenes) {
  $operation = $synth.SynthesizeTextToStreamAsync($scene.text)
  $task = $asTask.MakeGenericMethod([Windows.Media.SpeechSynthesis.SpeechSynthesisStream]).Invoke($null, @($operation))
  $task.Wait()
  $audioStream = [System.IO.WindowsRuntimeStreamExtensions]::AsStreamForRead($task.Result)
  $target = Join-Path $PSScriptRoot ('qa/' + $scene.id + '-raw.wav')
  $file = [System.IO.File]::Create($target)
  try { $audioStream.CopyTo($file) } finally { $file.Dispose(); $audioStream.Dispose(); $task.Result.Dispose() }
  Write-Output ('Gerado: ' + $scene.id)
}
$synth.Dispose()

