import { useText } from "./Locale";
import { Series, staticFile, useVideoConfig } from "remotion";
import { Video } from "@remotion/media";
import { LabFrame, videoStyle } from "./Design";
export const Circuits = () => {
  const t = useText();
  const { fps } = useVideoConfig();
  return (
    <LabFrame
      number="04"
      title={t("Associação de resistores")}
      description={t(
        "Abra e feche o circuito. Observe a corrente e a lâmpada.",
      )}
      note=""
    >
      <Series>
        <Series.Sequence
          name="Trecho 1"
          durationInFrames={6 * fps}
          premountFor={fps}
        >
          <Video
            src={staticFile("media/04-Associacao-de-resistores.mp4")}
            trimBefore={2 * fps}
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
            src={staticFile("media/04-Associacao-de-resistores.mp4")}
            trimBefore={18 * fps}
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
