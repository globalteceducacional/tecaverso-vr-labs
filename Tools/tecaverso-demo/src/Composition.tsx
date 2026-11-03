import {
  AbsoluteFill,
  Composition,
  Folder,
  useVideoConfig,
  staticFile,
} from "remotion";
import { Audio } from "@remotion/media";
import { useContext } from "react";
import { EnglishContext } from "./Locale";
import { TransitionSeries, linearTiming } from "@remotion/transitions";
import { fade } from "@remotion/transitions/fade";
import { Opening } from "./Opening";
import { Launch } from "./Launch";
import { Incline } from "./Incline";
import { Gravity } from "./Gravity";
import { Circuits } from "./Circuits";
import { Thermology } from "./Thermology";
import { Waves } from "./Waves";
import { Closing } from "./Closing";
export const Demo = () => {
  const english = useContext(EnglishContext);
  const { fps } = useVideoConfig();
  return (
    <AbsoluteFill style={{ background: "#021732" }}>
      <Audio
        name="Trilha ambiente tecnológica"
        src={staticFile(
          english
            ? "audio/english-tecaverso-ambient-ducked.wav"
            : "audio/francisca-tecaverso-ambient-ducked.wav",
        )}
        volume={1}
        premountFor={fps}
      />
      <Audio
        name="Narração em português brasileiro — Francisca Neural"
        src={staticFile(
          english
            ? "voiceover/english-tecaverso-narracao.wav"
            : "voiceover/francisca-tecaverso-narracao.wav",
        )}
        volume={1}
        premountFor={fps}
      />
      <TransitionSeries>
        <TransitionSeries.Sequence
          name="Opening"
          durationInFrames={6 * fps}
          premountFor={fps}
        >
          <Opening />
        </TransitionSeries.Sequence>
        <TransitionSeries.Transition
          presentation={fade()}
          timing={linearTiming({ durationInFrames: 15 })}
        />
        <TransitionSeries.Sequence
          name="Launch"
          durationInFrames={14 * fps}
          premountFor={fps}
        >
          <Launch />
        </TransitionSeries.Sequence>
        <TransitionSeries.Transition
          presentation={fade()}
          timing={linearTiming({ durationInFrames: 15 })}
        />
        <TransitionSeries.Sequence
          name="Incline"
          durationInFrames={12 * fps}
          premountFor={fps}
        >
          <Incline />
        </TransitionSeries.Sequence>
        <TransitionSeries.Transition
          presentation={fade()}
          timing={linearTiming({ durationInFrames: 15 })}
        />
        <TransitionSeries.Sequence
          name="Gravity"
          durationInFrames={13 * fps}
          premountFor={fps}
        >
          <Gravity />
        </TransitionSeries.Sequence>
        <TransitionSeries.Transition
          presentation={fade()}
          timing={linearTiming({ durationInFrames: 15 })}
        />
        <TransitionSeries.Sequence
          name="Circuits"
          durationInFrames={13 * fps}
          premountFor={fps}
        >
          <Circuits />
        </TransitionSeries.Sequence>
        <TransitionSeries.Transition
          presentation={fade()}
          timing={linearTiming({ durationInFrames: 15 })}
        />
        <TransitionSeries.Sequence
          name="Thermology"
          durationInFrames={15 * fps}
          premountFor={fps}
        >
          <Thermology />
        </TransitionSeries.Sequence>
        <TransitionSeries.Transition
          presentation={fade()}
          timing={linearTiming({ durationInFrames: 15 })}
        />
        <TransitionSeries.Sequence
          name="Waves"
          durationInFrames={15 * fps}
          premountFor={fps}
        >
          <Waves />
        </TransitionSeries.Sequence>
        <TransitionSeries.Transition
          presentation={fade()}
          timing={linearTiming({ durationInFrames: 15 })}
        />
        <TransitionSeries.Sequence
          name="Closing"
          durationInFrames={6 * fps}
          premountFor={fps}
        >
          <Closing />
        </TransitionSeries.Sequence>
      </TransitionSeries>
    </AbsoluteFill>
  );
};
export const EnglishDemo = () => (
  <EnglishContext.Provider value={true}>
    <Demo />
  </EnglishContext.Provider>
);
export const MyComposition = () => (
  <>
    <Composition
      id="Tecaverso-Labs-English"
      component={EnglishDemo}
      durationInFrames={2715}
      fps={30}
      width={1920}
      height={1080}
    />
    <Composition
      id="Tecaverso-Labs"
      component={Demo}
      durationInFrames={2715}
      fps={30}
      width={1920}
      height={1080}
    />
    <Folder name="Cenas">
      <Composition
        id="Opening"
        component={Opening}
        durationInFrames={180}
        fps={30}
        width={1920}
        height={1080}
      />
      <Composition
        id="Launch"
        component={Launch}
        durationInFrames={420}
        fps={30}
        width={1920}
        height={1080}
      />
      <Composition
        id="Incline"
        component={Incline}
        durationInFrames={360}
        fps={30}
        width={1920}
        height={1080}
      />
      <Composition
        id="Gravity"
        component={Gravity}
        durationInFrames={390}
        fps={30}
        width={1920}
        height={1080}
      />
      <Composition
        id="Circuits"
        component={Circuits}
        durationInFrames={390}
        fps={30}
        width={1920}
        height={1080}
      />
      <Composition
        id="Thermology"
        component={Thermology}
        durationInFrames={450}
        fps={30}
        width={1920}
        height={1080}
      />
      <Composition
        id="Waves"
        component={Waves}
        durationInFrames={450}
        fps={30}
        width={1920}
        height={1080}
      />
      <Composition
        id="Closing"
        component={Closing}
        durationInFrames={180}
        fps={30}
        width={1920}
        height={1080}
      />
    </Folder>
  </>
);
