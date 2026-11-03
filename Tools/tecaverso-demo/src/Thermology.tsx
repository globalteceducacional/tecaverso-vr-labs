import { useText } from "./Locale";
import { Series, staticFile, useVideoConfig } from "remotion";
import { Video } from "@remotion/media";
import { LabFrame, videoStyle } from "./Design";
export const Thermology = () => {
  const t = useText();
  const { fps } = useVideoConfig();
  return (
    <LabFrame
      number="05"
      title={t("Termologia")}
      description={t(
        "Relacione pressão, volume e temperatura nas transformações do gás.",
      )}
      note={t("Modelo de gás ideal")}
    >
      <Series>
        <Series.Sequence
          name="Trecho 1"
          durationInFrames={5 * fps}
          premountFor={fps}
        >
          <Video
            src={staticFile("media/05-Termologia.mp4")}
            trimBefore={2 * fps}
            muted
            premountFor={fps}
            objectFit="contain"
            style={videoStyle}
          />
        </Series.Sequence>
        <Series.Sequence
          name="Trecho 2"
          durationInFrames={5 * fps}
          premountFor={fps}
        >
          <Video
            src={staticFile("media/05-Termologia.mp4")}
            trimBefore={11 * fps}
            muted
            premountFor={fps}
            objectFit="contain"
            style={videoStyle}
          />
        </Series.Sequence>
        <Series.Sequence
          name="Trecho 3"
          durationInFrames={5 * fps}
          premountFor={fps}
        >
          <Video
            src={staticFile("media/05-Termologia.mp4")}
            trimBefore={21 * fps}
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
