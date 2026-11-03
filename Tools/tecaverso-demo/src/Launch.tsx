import { useText } from "./Locale";
import { Series, staticFile, useVideoConfig } from "remotion";
import { Video } from "@remotion/media";
import { LabFrame, videoStyle } from "./Design";
export const Launch = () => {
  const t = useText();
  const { fps } = useVideoConfig();
  return (
    <LabFrame
      number="01"
      title={t("Lançamento oblíquo")}
      description={t(
        "Compare trajetórias com diferentes ângulos de lançamento.",
      )}
      note={t(
        "Velocidade inicial: 8 m/s · reprodução original em câmera lenta (0,5×)",
      )}
    >
      <Series>
        <Series.Sequence
          name="Trecho 1"
          durationInFrames={6 * fps}
          premountFor={fps}
        >
          <Video
            src={staticFile("media/01-Lancamento-obliquo.mp4")}
            trimBefore={1.5 * fps}
            muted
            premountFor={fps}
            objectFit="contain"
            style={videoStyle}
          />
        </Series.Sequence>
        <Series.Sequence
          name="Trecho 2"
          durationInFrames={4 * fps}
          premountFor={fps}
        >
          <Video
            src={staticFile("media/01-Lancamento-obliquo.mp4")}
            trimBefore={11.5 * fps}
            muted
            premountFor={fps}
            objectFit="contain"
            style={videoStyle}
          />
        </Series.Sequence>
        <Series.Sequence
          name="Trecho 3"
          durationInFrames={4 * fps}
          premountFor={fps}
        >
          <Video
            src={staticFile("media/01-Lancamento-obliquo.mp4")}
            trimBefore={21.5 * fps}
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
