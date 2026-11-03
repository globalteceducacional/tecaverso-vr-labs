import { useText } from "./Locale";
import {
  AbsoluteFill,
  interpolate,
  useCurrentFrame,
  useVideoConfig,
  staticFile,
} from "remotion";
import { loadFont } from "@remotion/fonts";
import type { ReactNode } from "react";

Promise.all([
  loadFont({
    family: "Montserrat",
    url: staticFile("fonts/Montserrat-Regular.ttf"),
    weight: "400",
  }),
  loadFont({
    family: "Montserrat",
    url: staticFile("fonts/Montserrat-Bold.ttf"),
    weight: "700",
  }),
  loadFont({
    family: "Montserrat",
    url: staticFile("fonts/Montserrat-ExtraBold.ttf"),
    weight: "800",
  }),
]);
export const base = {
  backgroundColor: "#021732",
  color: "#FFFFFF",
  fontFamily: "Montserrat",
};
export const videoStyle = { width: "100%", height: "100%" };

export const LabFrame = ({
  children,
  number,
  title,
  description,
  note,
}: {
  children: ReactNode;
  number: string;
  title: string;
  description: string;
  note?: string;
}) => {
  const t = useText();
  const frame = useCurrentFrame();
  const { fps } = useVideoConfig();
  return (
    <AbsoluteFill style={base}>
      {children}
      <div
        style={{
          position: "absolute",
          top: 48,
          left: 64,
          background: "#021732",
          padding: "15px 24px",
          fontSize: 25,
          fontWeight: 800,
          letterSpacing: 2,
        }}
      >
        TECAVERSO <span style={{ fontWeight: 400 }}>LABS</span>
      </div>
      <div
        style={{
          position: "absolute",
          top: 48,
          right: 64,
          background: "#021732",
          padding: "15px 24px",
          fontSize: 24,
        }}
      >
        {t("FÍSICA EM REALIDADE VIRTUAL")}
      </div>
      <div
        style={{
          position: "absolute",
          bottom: 0,
          height: 184,
          left: 0,
          right: 0,
          background: "#021732",
          borderTop: "4px solid #284EA0",
          display: "flex",
          alignItems: "center",
          gap: 36,
          padding: "24px 64px",
          boxSizing: "border-box",
        }}
      >
        <div
          style={{
            fontSize: 52,
            fontWeight: 800,
            color: "#8AABFF",
            borderRight: "2px solid #284EA0",
            paddingRight: 36,
          }}
        >
          {number}
          <span style={{ fontSize: 22, color: "#E8E6FE" }}> / 06</span>
        </div>
        <div
          style={{
            opacity: interpolate(frame, [0, 0.45 * fps], [0, 1], {
              extrapolateRight: "clamp",
            }),
          }}
        >
          <div style={{ fontSize: 44, fontWeight: 800, marginBottom: 12 }}>
            {title}
          </div>
          <div style={{ fontSize: 29, color: "#E8E6FE" }}>{description}</div>
          {note ? (
            <div style={{ fontSize: 20, color: "#AAC3EE", marginTop: 8 }}>
              {note}
            </div>
          ) : null}
        </div>
      </div>
    </AbsoluteFill>
  );
};
