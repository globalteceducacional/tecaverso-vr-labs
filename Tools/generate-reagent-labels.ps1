# Offline, deterministic runtime textures, not documentation screenshots.
$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$out=Join-Path $root 'Assets/_Project/Laboratories/Chemistry/Reactions/Art/Labels'
New-Item -ItemType Directory -Force -Path $out | Out-Null
$fonts=[Drawing.Text.PrivateFontCollection]::new()
$fonts.AddFontFile((Join-Path $root 'Assets/_Project/UI/Fonts/Montserrat/Montserrat-Regular.ttf'))
$fonts.AddFontFile((Join-Path $root 'Assets/_Project/UI/Fonts/Montserrat/Montserrat-Bold.ttf'))
$family=$fonts.Families[0]
$blue=[Drawing.SolidBrush]::new([Drawing.ColorTranslator]::FromHtml('#284EA0'))
$ink=[Drawing.SolidBrush]::new([Drawing.ColorTranslator]::FromHtml('#161616'))
$white=[Drawing.SolidBrush]::new([Drawing.Color]::White)
$fmt=[Drawing.StringFormat]::new();$fmt.Alignment='Center';$fmt.LineAlignment='Center'
function Write-Label($id,$formula,$name,$kind) {
    $bmp=[Drawing.Bitmap]::new(512,512);$g=[Drawing.Graphics]::FromImage($bmp)
    $g.Clear([Drawing.ColorTranslator]::FromHtml('#F2F0EB'));$g.TextRenderingHint='AntiAliasGridFit'
    $g.FillRectangle($blue,0,0,512,60)
    $small=[Drawing.Font]::new($family,22,[Drawing.FontStyle]::Regular,[Drawing.GraphicsUnit]::Pixel)
    $large=[Drawing.Font]::new($family,68,[Drawing.FontStyle]::Bold,[Drawing.GraphicsUnit]::Pixel)
    $body=[Drawing.Font]::new($family,32,[Drawing.FontStyle]::Regular,[Drawing.GraphicsUnit]::Pixel)
    $g.DrawString('TECAVERSO · QUÍMICA',$small,$white,[Drawing.RectangleF]::new(10,0,492,60),$fmt)
    $g.DrawString($formula,$large,$blue,[Drawing.RectangleF]::new(15,90,482,110),$fmt)
    $g.DrawString($name,$body,$ink,[Drawing.RectangleF]::new(35,205,442,145),$fmt)
    $g.DrawString($kind,$small,$ink,[Drawing.RectangleF]::new(20,385,472,70),$fmt)
    $bmp.Save((Join-Path $out ($id+'.png')),[Drawing.Imaging.ImageFormat]::Png)
    $g.Dispose();$bmp.Dispose();$small.Dispose();$large.Dispose();$body.Dispose()
}
$model=[IO.File]::ReadAllText((Join-Path $root 'Assets/_Project/Laboratories/Chemistry/Reactions/Model/ReactionWorkbench.cs'))
foreach($m in [regex]::Matches($model,'new ReagentDefinition\("([^"]+)","([^"]+)","([^"]+)","([^"]+)"(,true)?\)')) {
    $solid=$m.Groups[5].Success
    $kind=if($solid){'SÓLIDO'}else{'SOLUÇÃO · 0,10 mol/L'}
    Write-Label $m.Groups[1].Value $m.Groups[2].Value $m.Groups[3].Value $kind
    if($solid){
        $bmp=[Drawing.Bitmap]::new(512,128);$g=[Drawing.Graphics]::FromImage($bmp);$g.Clear([Drawing.ColorTranslator]::FromHtml('#284EA0'));$g.TextRenderingHint='AntiAliasGridFit'
        $f=[Drawing.Font]::new($family,64,[Drawing.FontStyle]::Bold,[Drawing.GraphicsUnit]::Pixel)
        $g.DrawString($m.Groups[2].Value,$f,$white,[Drawing.RectangleF]::new(0,0,512,128),$fmt)
        $bmp.Save((Join-Path $out ($m.Groups[1].Value+'-spoon.png')),[Drawing.Imaging.ImageFormat]::Png)
        $f.Dispose();$g.Dispose();$bmp.Dispose()
    }
}
Write-Label 'indicator' 'INDICADOR' 'Fenolftaleína' 'TRAÇO · HCl / NaOH'
$fmt.Dispose();$blue.Dispose();$white.Dispose();$ink.Dispose();$fonts.Dispose()
Write-Output '18 texturas de rótulo e identificação geradas localmente.'
