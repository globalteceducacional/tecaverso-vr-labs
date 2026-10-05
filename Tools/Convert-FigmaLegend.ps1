# Rasterizes the supplied Figma circle SVGs for UGUI, preserving their geometry/colors.
Add-Type -AssemblyName System.Drawing
$assetDirectory = Join-Path $PSScriptRoot '../Assets/_Project/UI/Art/Figma'
foreach ($name in @('Result', 'Horizontal', 'Vertical')) {
    [xml]$svg = Get-Content -LiteralPath (Join-Path $assetDirectory "$name.svg")
    $circle = $svg.svg.circle
    $scale = 4
    $bitmap = [System.Drawing.Bitmap]::new(72, 72)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $brush = [System.Drawing.SolidBrush]::new([System.Drawing.ColorTranslator]::FromHtml($circle.fill))
    $radius = [float]$circle.r
    $graphics.FillEllipse($brush, ([float]$circle.cx - $radius)*$scale, ([float]$circle.cy - $radius)*$scale, 2*$radius*$scale, 2*$radius*$scale)
    $bitmap.Save((Join-Path $assetDirectory "$name.png"), [System.Drawing.Imaging.ImageFormat]::Png)
    $brush.Dispose(); $graphics.Dispose(); $bitmap.Dispose()
}
