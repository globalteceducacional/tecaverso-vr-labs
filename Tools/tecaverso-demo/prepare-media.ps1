$ErrorActionPreference = 'Stop'
$repositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$mediaDestination = Join-Path $PSScriptRoot 'public/media'
$fontDestination = Join-Path $PSScriptRoot 'public/fonts'
New-Item -ItemType Directory -Force $mediaDestination, $fontDestination | Out-Null
Copy-Item -Path (Join-Path $repositoryRoot 'Recordings/Physics/*.mp4') -Destination $mediaDestination
Copy-Item -Path (Join-Path $repositoryRoot 'Assets/_Project/UI/Fonts/Montserrat/*.ttf') -Destination $fontDestination
Copy-Item -LiteralPath (Join-Path $repositoryRoot 'Assets/_Project/UI/Fonts/Montserrat/OFL.txt') -Destination $fontDestination
