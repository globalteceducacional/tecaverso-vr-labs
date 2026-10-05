# Tecaverso experiment UI

Source: https://www.figma.com/design/ztbnAS5MZZmqOLbQAfm96E/Tecaverso-Game-UX-Kit

Examples: Projectile Motion `3051:256`; Projectile Analysis `3051:338`.

The Tecaverso collection has Claro/Escuro modes, primary blue #284EA0,
secondary #004389 and Montserrat body / Gotham Ultra display tokens.
Legacy shared text styles still name Inter, but both requested Examples explicitly
use Montserrat Regular, Bold and ExtraBold. The implementation follows these frames:
surface #D3D1E8, inset #E8E6FE, outline #07386F, text #161616, track #BDBBD4.

Montserrat static TTFs come from https://github.com/JulietaUla/Montserrat,
with the supplied OFL license alongside. Gotham was not downloaded or substituted:
it is not used in these two frames and a licensed font file would be required.
TMP assets embed prepopulated SDF atlases and retain the source fonts for dynamic glyphs.
Legend PNGs are converted from the original Figma SVGs using Tools/Convert-FigmaLegend.ps1.

XR adaptation: switchable tabs on the existing Canvas preserve its world transform.
The cards retain their 620/720 x 1010 proportions, scaled together for the existing
physical footprint. Pause, vector visibility, gravity legend and live telemetry fill
unused areas. Slider hit areas are larger than their visible 6 px tracks.
The world background remains the laboratory, not the flat Figma presentation backdrop.
All values are live; the range is travelled distance until impact and total afterwards.
Maximum height is the kinematic prediction for the active launch, as in the existing model.
Controls are locked in flight/paused to avoid displaying parameters not used by the shot.

Editor installer: Tecaverso > Oblique Launch > Apply Figma Experiment UI.
It targets only the existing panel, archives previous visual children (inactive),
and refuses duplicate application. It does not reconstruct the laboratory or run a build.
