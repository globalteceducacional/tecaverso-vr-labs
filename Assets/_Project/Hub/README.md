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
  Associação de resistores, Gravitação and Termologia open their individual labs.
  Ondas and the other disciplines remain mockups.
- Physics has six contents, including Inclined Plane in place of Optics. The Hub
  shows all six cards together; pagination remains hidden for disciplines with
  at most six entries.
- Existing LAN setup is preserved. A connected LAN session is not silently closed
  when opening an individual experiment; the UI asks to disconnect first.
  Only ObliqueLaunch currently supports collective LAN sessions.

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

## Focused verification

Editor compilation and console: no errors. Play Mode: all four disciplines open;
only the implemented Physics content enables Start; Back restores HUB 1.
Invoking the bound Physics/Start buttons loaded ObliqueLaunch with one XR rig.
Visual checks cover both Figma screens and the room. No build or headset/LAN
multi-device test was performed; physical comfort still needs a headset check.
