import { useText } from "./Locale";
import { Series, staticFile, useVideoConfig } from "remotion";
import { Video } from "@remotion/media";
import { LabFrame, videoStyle } from "./Design";
export const Gravity = () => {
  const t = useText();
  const { fps } = useVideoConfig();
  return (
    <LabFrame
      number="03"
      title={t("Gravitação")}
      description={t("Explore as órbitas e as informações dos planetas.")}
      note={t("Tempo acelerado · tamanhos e distâncias didáticos")}
    >
      <Series>
        <Series.Sequence
          name="Trecho 1"
          durationInFrames={8 * fps}
          premountFor={fps}
        >
          <Video
            src={staticFile("media/03-Gravitacao.mp4")}
            trimBefore={4 * fps}
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
            src={staticFile("media/03-Gravitacao.mp4")}
            trimBefore={19 * fps}
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
