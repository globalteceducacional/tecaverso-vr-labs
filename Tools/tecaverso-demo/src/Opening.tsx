import { useText } from "./Locale";
import {
  AbsoluteFill,
  staticFile,
  useVideoConfig,
  useCurrentFrame,
  interpolate,
} from "remotion";
import { Video } from "@remotion/media";
import { base, videoStyle } from "./Design";
export const Opening = () => {
  const { fps } = useVideoConfig();
  const t = useText();
  const frame = useCurrentFrame();
  return (
    <AbsoluteFill style={base}>
      <Video
        src={staticFile("media/03-Gravitacao.mp4")}
        trimBefore={4 * fps}
        muted
        premountFor={fps}
        objectFit="contain"
        style={videoStyle}
      />
      <AbsoluteFill style={{ backgroundColor: "rgba(2,23,50,.78)" }} />
      <div
        style={{
          position: "absolute",
          bottom: 0,
          left: 0,
          right: 0,
          height: 108,
          background: "#021732",
        }}
      />
      <div
        style={{
          position: "absolute",
          left: 112,
          top: 176,
          width: 160,
          height: 8,
          background: "#6F9EFF",
          scale: interpolate(frame, [0, fps], [0, 1], {
            extrapolateRight: "clamp",
          }),
          transformOrigin: "left",
        }}
      />
      <div
        style={{
          position: "absolute",
          left: 112,
          top: 225,
          opacity: interpolate(frame, [0, 0.7 * fps], [0, 1], {
            extrapolateRight: "clamp",
          }),
        }}
      >
        <div
          style={{
            fontSize: 29,
            letterSpacing: 5,
            marginBottom: 45,
            color: "#BBD1FF",
          }}
        >
          {t("LABORATÓRIOS EDUCACIONAIS EM REALIDADE VIRTUAL")}
        </div>
        <div style={{ fontSize: 146, lineHeight: 1.05, fontWeight: 800 }}>
          Tecaverso
          <br />
          <span style={{ color: "#8AABFF" }}>Labs</span>
        </div>
        <div style={{ fontSize: 43, marginTop: 46 }}>
          {t("Experimente. Observe. Compreenda.")}
        </div>
      </div>
      <div
        style={{
          position: "absolute",
          bottom: 85,
          left: 112,
          fontSize: 26,
          letterSpacing: 2,
        }}
      >
        {t("6 EXPERIMENTOS DE FÍSICA")}
      </div>
    </AbsoluteFill>
  );
};
