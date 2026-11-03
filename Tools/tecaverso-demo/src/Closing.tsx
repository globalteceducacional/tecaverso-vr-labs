import { useText } from "./Locale";
import {
  AbsoluteFill,
  interpolate,
  useCurrentFrame,
  useVideoConfig,
} from "remotion";
import { base } from "./Design";
export const Closing = () => {
  const t = useText();
  const frame = useCurrentFrame();
  const { fps } = useVideoConfig();
  return (
    <AbsoluteFill
      style={{
        ...base,
        justifyContent: "center",
        padding: 112,
        boxSizing: "border-box",
      }}
    >
      <div
        style={{
          opacity: interpolate(frame, [0, 0.6 * fps], [0, 1], {
            extrapolateRight: "clamp",
          }),
        }}
      >
        <div
          style={{
            fontSize: 28,
            letterSpacing: 5,
            color: "#8AABFF",
            marginBottom: 48,
          }}
        >
          TECAVERSO LABS
        </div>
        <div style={{ fontSize: 105, fontWeight: 800, lineHeight: 1.12 }}>
          {t("A Física ganha")}
          <br />
          {t("uma nova dimensão.")}
        </div>
        <div
          style={{
            height: 7,
            width: 160,
            background: "#284EA0",
            margin: "48px 0",
          }}
        />
        <div style={{ fontSize: 40, color: "#E8E6FE" }}>
          {t("Explore relações. Compare resultados.")}
        </div>
      </div>
      <div
        style={{
          position: "absolute",
          bottom: 64,
          left: 112,
          fontSize: 20,
          color: "#AAC3EE",
        }}
      >
        {t("Texturas planetárias")}: Solar System Scope · CC BY 4.0 ·
        solarsystemscope.com/textures
      </div>
    </AbsoluteFill>
  );
};
