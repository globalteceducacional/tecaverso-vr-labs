// Original, deterministic ambient score. No samples or third-party recordings.
// Stereo 48 kHz; D major / B minor, 72 BPM; gentle pads and delayed plucks.
const fs = require('node:fs');
const path = require('node:path');
const rate = 48000;
const duration = 90.5;
const count = Math.round(rate * duration);
const left = new Float32Array(count);
const right = new Float32Array(count);
const tau = 2 * Math.PI;
const beat = 60 / 72;
const chordLength = 8 * beat;
const progression = [
  [50, 57, 61, 64, 69], // Dmaj9
  [45, 57, 59, 64, 69], // Aadd9
  [47, 54, 57, 62, 66], // Bm7
  [43, 54, 57, 62, 66], // Gmaj9
];
const hz = (note) => 440 * 2 ** ((note - 69) / 12);
function add(start, length, pan, signal) {
  const offset = Math.round(start * rate);
  const n = Math.round(length * rate);
  const l = Math.cos((pan + 1) * Math.PI / 4);
  const r = Math.sin((pan + 1) * Math.PI / 4);
  for (let i = 0; i < n && offset + i < count; i++) {
    if (offset + i < 0) continue;
    const value = signal(i / rate, length);
    left[offset + i] += value * l;
    right[offset + i] += value * r;
  }
}
for (let c = 0; c * chordLength < duration; c++) {
  const chord = progression[c % progression.length];
  chord.forEach((note, voice) => {
    const frequency = hz(note);
    add(c * chordLength, chordLength + 3, (voice - 2) * .27, (t, length) => {
      const envelope = Math.min(1, t / 2.4) * Math.min(1, (length - t) / 3);
      const drift = .85 + .15 * Math.sin(tau * .085 * t + voice);
      return .032 * envelope * drift * (
        Math.sin(tau * frequency * t) +
        .3 * Math.sin(tau * frequency * 1.0017 * t + .25) +
        .12 * Math.sin(tau * frequency * 2 * t)
      );
    });
  });
  // A restrained low pulse under the sustained harmony.
  for (let b = 0; b < 8; b += 2) {
    const frequency = hz(chord[0] - 12);
    add(c * chordLength + b * beat, 1.8, 0, (t) =>
      .035 * (1 - Math.exp(-t * 12)) * Math.exp(-t * 2.4) * Math.sin(tau * frequency * t));
  }
  // Sparse, warm digital plucks, with alternating stereo echoes.
  const pattern = [2, 3, 4, 3, 1, 3, 4, 2];
  for (let step = 0; step < 16; step++) {
    if (step % 4 === 3) continue;
    const start = c * chordLength + step * beat / 2;
    const frequency = hz(chord[pattern[step % pattern.length]] + 12);
    const intensity = .022 * (step % 4 === 0 ? 1 : .65);
    for (let echo = 0; echo < 4; echo++) {
      add(start + echo * beat * .75, 3.5, ((step + echo) % 2 ? .55 : -.55), (t) => {
        const envelope = (1 - Math.exp(-t * 70)) * Math.exp(-t * 2.3);
        return intensity * .36 ** echo * envelope * (
          Math.sin(tau * frequency * t) + .15 * Math.exp(-t * 4) * Math.sin(tau * frequency * 3 * t));
      });
    }
  }
}
// Small, diffuse stereo reflections soften the synths without a harsh beat.
for (const [delay, gain] of [[.173,.12],[.293,.09],[.419,.06]]) {
  const shift = Math.round(delay * rate);
  for (let i = count - 1; i >= shift; i--) {
    left[i] += right[i - shift] * gain;
    right[i] += left[i - shift] * gain;
  }
}
const wav = Buffer.alloc(44 + count * 4);
wav.write('RIFF', 0); wav.writeUInt32LE(wav.length - 8, 4);
wav.write('WAVEfmt ', 8); wav.writeUInt32LE(16, 16);
wav.writeUInt16LE(1, 20); wav.writeUInt16LE(2, 22);
wav.writeUInt32LE(rate, 24); wav.writeUInt32LE(rate * 4, 28);
wav.writeUInt16LE(4, 32); wav.writeUInt16LE(16, 34);
wav.write('data', 36); wav.writeUInt32LE(count * 4, 40);
let peak = 0;
for (let i = 0; i < count; i++) peak = Math.max(peak, Math.abs(left[i]), Math.abs(right[i]));
for (let i = 0; i < count; i++) {
  wav.writeInt16LE(Math.round(left[i] / peak * .5 * 32767), 44 + i * 4);
  wav.writeInt16LE(Math.round(right[i] / peak * .5 * 32767), 46 + i * 4);
}
fs.mkdirSync(path.join(__dirname, 'qa'), {recursive:true});
fs.writeFileSync(path.join(__dirname, 'qa/ambient-source.wav'), wav);
console.log('Created original stereo ambient score: 90.5 seconds, 48 kHz.');
