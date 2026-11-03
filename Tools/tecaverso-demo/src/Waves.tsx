import { useText } from "./Locale";
import { Series, staticFile, useVideoConfig } from "remotion";
import { Video } from "@remotion/media";
import { LabFrame, videoStyle } from "./Design";
export const Waves = () => {
  const t = useText();
  const { fps } = useVideoConfig();
  return (
    <LabFrame
      number="06"
      title={t("Ondulatória")}
      description={t(
        "Investigue pulsos, ondas periódicas e ondas estacionárias.",
      )}
      note=""
    >
      <Series>
        <Series.Sequence
          name="Trecho 1"
          durationInFrames={4 * fps}
          premountFor={fps}
        >
          <Video
            src={staticFile("media/06-Ondulatoria.mp4")}
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
            src={staticFile("media/06-Ondulatoria.mp4")}
            trimBefore={10 * fps}
            muted
            premountFor={fps}
            objectFit="contain"
            style={videoStyle}
          />
        </Series.Sequence>
        <Series.Sequence
          name="Trecho 3"
          durationInFrames={6 * fps}
          premountFor={fps}
        >
          <Video
            src={staticFile("media/06-Ondulatoria.mp4")}
            trimBefore={22 * fps}
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
