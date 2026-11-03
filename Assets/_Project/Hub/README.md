# Hub

Entry scene: `Scenes/Hub.unity` (first enabled scene).

- `HUB 1`: Figma Examples / Discipline Selection / Image Icons HUB 1, node 3053:200.
- `HUB 2`: Content Selection / Image Icons HUB 2, node 3053:238.
- Source: https://www.figma.com/design/ztbnAS5MZZmqOLbQAfm96E/Tecaverso-Game-UX-Kit
- Original local PNG assets: `_Project/UI/Art/Figma/Hub`; the Inclined Plane illustration is in `_Project/UI/Art/Hub`. Montserrat TMP fonts reused.
- The fixed 2560×1440 composition is adapted to a world-space XR canvas. Actual
  ray-clickable buttons replace desktop keyboard-only hints. Escape also goes back.
- `HubCatalog.asset` is the editable catalog; an empty scene path marks a mockup.
- Physics / Movimento e forças opens ObliqueLaunch. Plano inclinado,
  Associação de resistores, Gravitação, Termologia and Ondulatória open their individual labs.
  Mathematics / Espaço cartesiano, Formas geométricas and Geometria plana open individual labs.
  Mathematics / Funções and Medidas, and all Chemistry/Biology entries, remain mockups.
- Physics has six contents, including Inclined Plane in place of Optics. The Hub
  shows all six cards together; pagination remains hidden for disciplines with
  at most six entries.
- Existing LAN setup is preserved. A connected LAN session is not silently closed
  when opening an individual experiment; the UI asks to disconnect first.
  Only ObliqueLaunch currently supports collective LAN sessions.

## Mathematics

- **Espaço cartesiano:** construct points, lines, surfaces and solids; optional integer
  snapping, 0.5 m per mathematical unit and independent XY/XZ/YZ grid visibility.
  See the [lab README](../Laboratories/Mathematics/CartesianSpace/README.md).
- **Formas geométricas:** eleven solids already displayed in the room, with names,
  topology counts and volume formulas. Only vertex, edge and rotation toggles and
  Back to Hub are exposed. No editing, study A/B, challenges or unfolding UI.
  See the [lab README](../Laboratories/Mathematics/GeometricShapes/README.md) and
  [approved GDD](../Laboratories/Mathematics/GeometricShapes/GDD.md).

- **Geometria plana:** thirteen fixed planar figures with area formulas, vertex,
  contour and auxiliary-measure toggles. No editing or automatic rotation.
  See the [lab README](../Laboratories/Mathematics/PlanarGeometry/README.md).

The current catalog contains six Physics cards and five Mathematics cards. Six is
the maximum per discipline, not a requirement to add placeholder content. Keep
`HubCatalog.asset` and `ProjectSettings/EditorBuildSettings.asset` consistent.

## Standard visual room

Reuse `_Project/Environments/StandardRoom/TecaversoStandardRoom.prefab` for future
rooms. It is 14×14 m, 7 m high, with pearl panels, graphite rails, blue trims and
the original laboratory grid-floor shader/materials. It contains geometry and two
shadowless lights, not an XR rig or UI. The ballistic hall keeps its larger envelope.
Materials are shared with the existing ObliqueLaunch environment; do not delete them.
The template walls/floor/props are disabled, not deleted, in Hub. The XR rig is unchanged.

Editor installer: Tecaverso > Hub > Apply Standard Room and Figma UI.
It refuses a second installation; edit the existing prefabs/scene for adjustments.
No player builds are part of this workflow.

## Recorded verification

The original Hub verification covered all four disciplines, mockup blocking and
Back restoring HUB 1. At that stage, only Physics had implemented content.
Invoking the bound Physics/Start buttons loaded ObliqueLaunch with one XR rig.
Visual checks cover both Figma screens and the room. No build or headset/LAN
multi-device test was performed; physical comfort still needs a headset check.

The Mathematics READMEs record subsequent Editor checks. The solids gallery was
verified with eleven exhibits, three toggles, one Back button and a Hub round trip
with one XR rig. Its initialization reported OpenXR configuration warnings; no
missing scripts were found on scene GameObjects. These are historical results,
not new tests performed during this documentation revision.

Geometria plana was subsequently tested with fifteen EditMode cases, independent
toggles and Back through a synthetic XR ray, and entry through the Mathematics
catalog. All thirteen figures and a single XR rig were present on reentry. This
does not constitute headset validation.
