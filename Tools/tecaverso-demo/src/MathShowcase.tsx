import { Audio, Video } from "@remotion/media";
import {
  AbsoluteFill,
  Series,
  staticFile,
  useCurrentFrame,
  useVideoConfig,
} from "remotion";
import { LabFrame, base, videoStyle } from "./Design";
import { EnglishContext, useText } from "./Locale";
import { useContext } from "react";

const captionsPt = [
  [0.4, 5.371, "Conheça os seis laboratórios de matemática do Tecaverso Labs."],
  [6.5, 13.535, "Explore formas geométricas e descubra como suas propriedades aparecem em modelos tridimensionais."],
  [19.5, 26.055, "Observe funções e gráficos: conecte expressões algébricas às curvas que elas produzem."],
  [32.5, 39.775, "Investigue probabilidade e estatística. Compare resultados, distribuições e padrões nos dados."],
  [45.5, 51.887, "No espaço cartesiano, localize pontos e relacione coordenadas à posição no plano."],
  [58.5, 64.623, "Explore a geometria plana e compare formas, medidas e relações entre figuras."],
  [71.5, 77.359, "Com frações e proporções, compare quantidades e reconheça relações equivalentes."],
  [84.5, 89.351, "Aprenda matemática de forma interativa com Tecaverso Labs."],
] as const;
const captionsEn = [
  [0.4, 4.699, "Discover six mathematics labs from Tecaverso Labs."],
  [6.5, 12.311, "Explore geometric shapes and see how their properties appear in three-dimensional models."],
  [19.5, 26.343, "Explore functions and graphs. Connect algebraic expressions to the curves they produce."],
  [32.5, 40.495, "Investigate probability and statistics. Compare outcomes, distributions, and patterns in data."],
  [45.5, 51.287, "On the Cartesian plane, locate points and connect coordinates to their positions."],
  [58.5, 65.031, "Explore plane geometry and compare shapes, measurements, and relationships between figures."],
  [71.5, 77.959, "With fractions and proportions, compare quantities and identify equivalent relationships."],
  [84.5, 88.751, "Learn mathematics interactively with Tecaverso Labs."],
] as const;

const Subtitle = () => {
  const frame = useCurrentFrame();
  const { fps } = useVideoConfig();
  const english = useContext(EnglishContext);
  const captions = english ? captionsEn : captionsPt;
  const active = captions.find(([start, end]) => frame / fps >= start && frame / fps <= end);
  if (!active) return null;
  return (
    <div
      style={{
        position: "absolute",
        bottom: 198,
        left: 180,
        right: 180,
        textAlign: "center",
        background: "rgba(2, 23, 50, 0.93)",
        padding: "14px 24px",
        borderRadius: 12,
        color: "#fff",
        fontSize: 30,
        fontWeight: 700,
        lineHeight: 1.25,
        textShadow: "0 2px 5px #000",
      }}
    >
      {active[2]}
    </div>
  );
};

const MathClip = ({
  file,
  title,
  description,
  number,
}: {
  file: string;
  title: string;
  description: string;
  number: string;
}) => {
  const { fps } = useVideoConfig();
  const t = useText();
  return (
    <LabFrame
      number={number}
      title={t(title)}
      description={t(description)}
      header="MATEMÁTICA EM REALIDADE VIRTUAL"
    >
        <Video
          src={staticFile(`media/mathematics/${file}`)}
          trimBefore={1 * fps}
          durationInFrames={13 * fps}
          muted
          premountFor={fps}
          objectFit="contain"
          style={videoStyle}
        />
    </LabFrame>
  );
};

export const MathShowcase = () => {
  const { fps } = useVideoConfig();
  const t = useText();
  const english = useContext(EnglishContext);
  return (
    <AbsoluteFill style={base}>
      <Audio src={staticFile(`audio/math-${english ? "en" : "pt"}-tecaverso-ambient-ducked.wav`)} volume={1} premountFor={fps} />
      <Audio src={staticFile(`voiceover/math-${english ? "en" : "pt"}-tecaverso-narracao.wav`)} volume={1} premountFor={fps} />
      <Series>
        <Series.Sequence name="Opening" durationInFrames={6 * fps} premountFor={fps}>
          <AbsoluteFill style={{ ...base, justifyContent: "center", padding: 112, boxSizing: "border-box" }}>
            <div style={{ fontSize: 30, letterSpacing: 5, color: "#8AABFF" }}>TECAVERSO LABS</div>
            <div style={{ fontSize: 100, fontWeight: 800, marginTop: 32 }}>{t("Mathematics, made tangible.")}</div>
            <div style={{ fontSize: 40, marginTop: 32, color: "#E8E6FE" }}>{t("Seis laboratórios interativos")}</div>
          </AbsoluteFill>
        </Series.Sequence>
        <Series.Sequence name="Formas geométricas" durationInFrames={13 * fps} premountFor={fps}><MathClip number="01" file="02-Formas-Geometricas.mp4" title="Formas geométricas" description="Explore propriedades e modelos tridimensionais de formas geométricas." /></Series.Sequence>
        <Series.Sequence name="Funções e gráficos" durationInFrames={13 * fps} premountFor={fps}><MathClip number="02" file="03-Funcoes-e-Graficos.mp4" title="Funções e gráficos" description="Relacione expressões algébricas às curvas representadas no plano." /></Series.Sequence>
        <Series.Sequence name="Probabilidade e estatística" durationInFrames={13 * fps} premountFor={fps}><MathClip number="03" file="04-Probabilidade-e-Estatistica.mp4" title="Probabilidade e estatística" description="Investigue resultados, distribuições e padrões observados nos dados." /></Series.Sequence>
        <Series.Sequence name="Espaço cartesiano" durationInFrames={13 * fps} premountFor={fps}><MathClip number="04" file="05-Espaco-Cartesiano.mp4" title="Espaço cartesiano" description="Localize pontos e explore coordenadas no plano cartesiano." /></Series.Sequence>
        <Series.Sequence name="Geometria plana" durationInFrames={13 * fps} premountFor={fps}><MathClip number="05" file="06-Geometria-Plana.mp4" title="Geometria plana" description="Compare figuras, medidas e relações da geometria plana." /></Series.Sequence>
        <Series.Sequence name="Frações e proporções" durationInFrames={13 * fps} premountFor={fps}><MathClip number="06" file="07-Fracoes-e-Proporcoes.mp4" title="Frações e proporções" description="Compare quantidades e descubra relações proporcionais equivalentes." /></Series.Sequence>
        <Series.Sequence name="Closing" durationInFrames={6 * fps} premountFor={fps}>
          <AbsoluteFill style={{ ...base, justifyContent: "center", padding: 112, boxSizing: "border-box" }}>
            <div style={{ fontSize: 30, letterSpacing: 5, color: "#8AABFF" }}>TECAVERSO LABS</div>
            <div style={{ fontSize: 88, fontWeight: 800, marginTop: 36 }}>{t("Explore. Compare. Understand.")}</div>
            <div style={{ fontSize: 34, marginTop: 28, color: "#E8E6FE" }}>{t("Interactive mathematics in virtual reality")}</div>
          </AbsoluteFill>
        </Series.Sequence>
      </Series>
      <Subtitle />
    </AbsoluteFill>
  );
};
