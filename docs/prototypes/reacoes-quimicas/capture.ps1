param([string]$BrowserPath)
$ErrorActionPreference = 'Stop'
$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../..'))
$htmlPath = Join-Path $PSScriptRoot 'index.html'
$outputDir = Join-Path $repoRoot 'GeneratedMedia/GDD/Reacoes'
if (-not $BrowserPath) {
    $BrowserPath = Join-Path $repoRoot 'Tools/tecaverso-demo/node_modules/.remotion/chrome-headless-shell/win64/chrome-headless-shell-win64/chrome-headless-shell.exe'
}
if (-not (Test-Path -LiteralPath $BrowserPath)) {
    throw 'Informe -BrowserPath para um Chromium headless local. O script nao instala nem baixa dependencias.'
}
New-Item -ItemType Directory -Force -Path $outputDir | Out-Null
$states = @('entrada','escolha','mistura','consulta','limite','reinicio','temperatura')
for ($i = 0; $i -lt $states.Length; $i++) {
    $target = Join-Path $outputDir ('{0:D2}-{1}.png' -f ($i + 1), $states[$i])
    $pageUrl = ([Uri]$htmlPath).AbsoluteUri + '?estado=' + $states[$i]
    # Executa somente o HTML local em browser headless separado; nao usa perfis pessoais.
    $renderedDom = & $BrowserPath '--no-sandbox' '--disable-gpu' '--disable-background-networking' '--hide-scrollbars' '--window-size=1600,1000' '--virtual-time-budget=1500' '--dump-dom' "--screenshot=$target" $pageUrl
    if ($LASTEXITCODE -ne 0 -or -not (Test-Path -LiteralPath $target) -or ($renderedDom -join "`n") -notmatch 'data-preview-ready="true"') { throw "Falha na captura ou JavaScript: $target" }
    Write-Output $target
}
