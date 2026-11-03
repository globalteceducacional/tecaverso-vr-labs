import { useText } from "./Locale";
import { Series, staticFile, useVideoConfig } from "remotion";
import { Video } from "@remotion/media";
import { LabFrame, videoStyle } from "./Design";
export const Incline = () => {
  const t = useText();
  const { fps } = useVideoConfig();
  return (
    <LabFrame
      number="02"
      title={t("Plano inclinado")}
      description={t("Observe peso, normal e atrito durante o deslizamento.")}
      note={t("Reprodução original em câmera lenta (0,5×)")}
    >
      <Series>
        <Series.Sequence
          name="Trecho 1"
          durationInFrames={5 * fps}
          premountFor={fps}
        >
          <Video
            src={staticFile("media/02-Plano-inclinado.mp4")}
            trimBefore={1.5 * fps}
            muted
            premountFor={fps}
            objectFit="contain"
            style={videoStyle}
          />
        </Series.Sequence>
        <Series.Sequence
          name="Trecho 2"
          durationInFrames={7 * fps}
          premountFor={fps}
        >
          <Video
            src={staticFile("media/02-Plano-inclinado.mp4")}
            trimBefore={12 * fps}
            muted
            premountFor={fps}
            objectFit="contain"
            style={videoStyle}
          />
        </Series.Sequence>
      </Series>
    </LabFrame>
  );
};
