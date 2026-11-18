const names = [
  "Respiratório",
  "Circulatório",
  "Digestório",
  "Nervoso",
  "Esquelético",
  "Muscular",
  "Urinário",
  "Linfático",
  "Reprodutivo",
];
const durations = [8, 12, 14, 6];
const initialLegend = [
  "Ar nas vias e nos dois pulmões.",
  "Circuito pulmonar e sistêmico. Azul indica sangue menos oxigenado e vermelho, mais oxigenado.",
  "Caminho do conteúdo pelo tubo digestório.",
  "Sinal sensitivo da mão direita até o encéfalo.",
];
const $ = (s) => document.querySelector(s);
const canvas = $("canvas"),
  ctx = canvas.getContext("2d");
const palette = {
  lung: "#dc99a3",
  vein: "#6392ca",
  artery: "#d3666b",
  nerve: "#dac087",
  bone: "#dbd2bd",
  gut: "#d99d88",
};
const body = new Path2D(
  "M220 92 C205 81 204 54 210 38 Q220 17 240 17 Q260 17 270 38 C276 54 275 81 260 92 L258 108 Q285 110 298 125 L317 188 341 249 354 280 Q361 296 352 295 L335 278 317 260 286 197 279 182 Q273 215 282 258 Q292 283 282 306 L273 388 265 477 269 527 283 541 Q286 548 272 548 L247 545 243 482 242 404 237 316 232 404 223 483 221 545 196 548 Q182 548 185 541 L202 527 206 477 200 388 191 306 Q180 283 193 255 Q202 215 193 182 L182 198 161 251 144 277 129 294 Q117 304 122 286 L136 251 156 196 177 132 Q184 113 221 107 Z",
);
const subjects = [
  {
    name: "Pulmão esquerdo",
    function: "Participa das trocas gasosas entre o ar e o sangue.",
    context:
      "Os brônquios conduzem ar para os pulmões. Observe a posição do pulmão esquerdo junto ao coração.",
    canHold: true,
    part: "M249 139 C261 124 284 151 284 179 Q286 211 263 211 Q246 205 248 188 L257 175 Q250 169 249 163Z",
    color: palette.lung,
  },
  {
    name: "Coração",
    function: "Bombeia sangue para os circuitos pulmonar e sistêmico.",
    context:
      "O lado direito envia sangue aos pulmões. O lado esquerdo envia sangue aos tecidos.",
    canHold: true,
    part: "M236 169 Q227 155 235 151 Q245 147 249 155 Q262 145 269 157 C276 170 263 183 255 194 Q238 184 236 169Z",
    color: "#c66d69",
  },
  {
    name: "Estômago",
    function: "Recebe, armazena e mistura o conteúdo vindo do esôfago.",
    context: "O conteúdo segue do estômago para o intestino delgado.",
    canHold: true,
    part: "M244 174 Q256 181 260 176 Q277 169 278 187 C282 210 263 219 251 209 Q245 201 235 210 L229 203 Q244 190 250 198 Q259 184 244 184Z",
    color: "#e4a691",
  },
  {
    name: "Encéfalo",
    function:
      "Integra informações e participa da coordenação das respostas do corpo.",
    context:
      "O pulso apresentado exemplifica a condução sensitiva do membro superior até o encéfalo.",
    canHold: true,
    part: "M216 56 Q204 46 213 38 Q211 26 226 26 Q234 16 243 23 Q254 19 262 29 Q273 29 269 41 Q277 50 265 56 Q261 69 251 60 Q238 65 233 59 Q216 67 216 56Z",
    color: "#df9b98",
  },
  {
    name: "Crânio",
    function: "O conjunto de ossos do crânio protege o encéfalo.",
    context: "A peça reúne ossos do modelo em um conjunto.",
    canHold: true,
    part: "M212 51 Q207 22 239 21 Q268 20 269 49 Q270 69 256 75 L251 88 228 88 222 73 Q210 68 212 51Z",
    color: palette.bone,
  },
  {
    name: "Musculatura do tronco",
    function: "Participa da sustentação e do movimento do tronco.",
    context: "Grupo de estruturas musculares apresentado na posição anatômica.",
    canHold: false,
    part: "M239 125 Q222 111 195 133 L196 161 Q209 176 238 160 Q266 176 281 161 L284 133 Q260 113 241 125Z",
    color: "#b76f67",
  },
  {
    name: "Rim esquerdo",
    function: "Participa da filtração do sangue e da formação da urina.",
    context: "O ureter conduz a urina do rim à bexiga.",
    canHold: true,
    part: "M259 199 Q266 188 276 194 C288 202 282 224 274 228 Q262 234 262 223 Q273 216 262 210 Q255 208 259 199Z",
    color: "#c78679",
  },
  {
    name: "Baço",
    function: "Filtra o sangue e participa da resposta imunitária.",
    context: "Fica na parte superior esquerda do abdome, próximo ao estômago.",
    canHold: true,
    part: "M263 183 Q281 178 282 193 Q281 211 268 217 Q263 208 264 200 Q258 195 263 183Z",
    color: "#b186af",
  },
  {
    name: "Próstata e uretra — conjunto",
    function: "O modelo agrupa a próstata e parte da uretra.",
    context:
      "A uretra passa pela próstata. Explore as duas estruturas no contexto da pelve.",
    canHold: false,
    part: "M227 278 Q240 269 253 278 Q251 292 240 293 L242 318 236 318 237 293 Q228 293 227 278Z",
    color: "#d59b87",
  },
];
const armR = [
  [130, 281],
  [150, 247],
  [168, 199],
  [190, 141],
  [210, 124],
  [236, 116],
];
const armL = [
  [348, 281],
  [329, 247],
  [312, 199],
  [289, 141],
  [270, 124],
  [246, 121],
];
const leftLung = [
  [244, 154],
  [255, 144],
  [266, 158],
  [269, 185],
  [256, 172],
  [248, 166],
];
const rightLung = [
  [238, 154],
  [224, 144],
  [212, 158],
  [210, 184],
  [223, 173],
  [236, 167],
];
const systemicOut = [
  [250, 167],
  [249, 143],
  [247, 126],
  ...armL.slice().reverse().slice(1),
];
const systemicBack = armL
  .map(([x, y]) => [x - 4, y + 3])
  .concat([
    [239, 138],
    [237, 164],
  ]);
const labelAnchors = [
  [267, 181],
  [252, 171],
  [261, 193],
  [240, 44],
  [240, 56],
  [239, 144],
  [273, 209],
  [272, 198],
  [241, 282],
];
const bolus = [
  [238, 80],
  [238, 113],
  [240, 146],
  [242, 174],
  [261, 188],
  [256, 201],
  [238, 211],
  [215, 219],
  [250, 225],
  [220, 233],
  [250, 240],
  [215, 247],
  [249, 253],
  [211, 259],
  [207, 243],
  [206, 217],
  [232, 216],
  [266, 219],
  [267, 249],
  [256, 265],
  [240, 267],
  [240, 287],
];
const sense = armR.concat([
  [239, 103],
  [239, 82],
  [241, 69],
  [247, 54],
  [257, 45],
]);
const breath = [
  [239, 71],
  [240, 88],
  [240, 113],
  [240, 136],
];
const airBranches = [
  breath.concat([
    [218, 153],
    [211, 178],
    [207, 195],
  ]),
  breath.concat([
    [263, 153],
    [270, 179],
    [273, 195],
  ]),
];
let system = 0,
  skin = true,
  inspected = false,
  held = false,
  returning = false,
  progress = "off",
  phase = 0,
  yaw = 0;
let offset = [0, 0],
  returningFrom,
  returnStart,
  drag = null,
  lastFrame = 0,
  hover = false,
  dialDrag = null;
const reduced = matchMedia("(prefers-reduced-motion: reduce)");
function stroke(points, color, width) {
  ctx.beginPath();
  points.forEach(([x, y], i) => (i ? ctx.lineTo(x, y) : ctx.moveTo(x, y)));
  ctx.lineWidth = width;
  ctx.lineCap = "round";
  ctx.lineJoin = "round";
  ctx.strokeStyle = color;
  ctx.stroke();
}
function shape(path, color, line, width = 1) {
  const p = new Path2D(path);
  ctx.fillStyle = color;
  ctx.fill(p);
  if (line) {
    ctx.strokeStyle = line;
    ctx.lineWidth = width;
    ctx.stroke(p);
  }
}
function ellipse(x, y, rx, ry, color) {
  ctx.fillStyle = color;
  ctx.beginPath();
  ctx.ellipse(x, y, rx, ry, 0, 0, Math.PI * 2);
  ctx.fill();
}
function stem(path, color, width = 3) {
  ctx.strokeStyle = color;
  ctx.lineWidth = width;
  ctx.lineCap = "round";
  ctx.lineJoin = "round";
  ctx.stroke(new Path2D(path));
}
function routesBase() {
  if (system === 0) {
    shape(
      "M230 139 C216 124 195 152 196 188 Q196 211 219 213 Q236 205 231 184Z",
      palette.lung,
    );
    airBranches.forEach((p) => stroke(p, "#b1c8cc", 4));
  } else if (system === 1) {
    for (const p of [leftLung, rightLung]) stroke(p, "#ae7f88", 4);
    stroke(systemicOut, palette.artery, 3);
    stroke(systemicBack, palette.vein, 3);
    for (const side of [-1, 1]) {
      const x = (n) => 240 + n * side;
      stroke(
        [
          [x(2), 166],
          [x(4), 236],
          [x(28), 299],
          [x(23), 374],
          [x(19), 477],
          [x(19), 527],
        ],
        palette.artery,
        3,
      );
      stroke(
        [
          [x(5), 164],
          [x(8), 237],
          [x(34), 298],
          [x(28), 374],
          [x(23), 477],
          [x(22), 527],
        ],
        palette.vein,
        2,
      );
      stroke(
        [
          [x(3), 135],
          [x(11), 97],
          [x(15), 65],
        ],
        palette.artery,
        3,
      );
    }
    stroke(armR, palette.vein, 3);
    stroke(
      armR.map(([x, y]) => [x + 4, y + 1]),
      palette.artery,
      3,
    );
  } else if (system === 2) {
    stroke(bolus, "#cc9787", 8);
    stroke(bolus.slice(0, 4), "#daa997", 7);
    stem(
      "M238 179 Q224 165 202 174 Q196 182 199 201 Q215 201 238 185",
      "#a97869",
      12,
    );
  } else if (system === 3 || system === 7) {
    let color = system === 3 ? palette.nerve : "#97b79c";
    for (const p of [
      armR,
      armL,
      [
        [238, 72],
        [239, 150],
        [240, 261],
      ],
      [
        [240, 248],
        [213, 301],
        [209, 380],
        [215, 477],
        [206, 527],
      ],
      [
        [240, 248],
        [268, 301],
        [268, 380],
        [257, 477],
        [265, 527],
      ],
    ])
      stroke(p, color, 2);
    for (let i = 0; i < 10; i++)
      for (const s of [-1, 1])
        stroke(
          [
            [239, 122 + i * 10],
            [239 + s * 16, 130 + i * 10],
            [239 + s * 32, 136 + i * 10],
          ],
          color,
          1,
        );
    if (system === 7)
      for (const [x, y] of [
        ...armR.slice(2),
        ...armL.slice(2),
        [215, 279],
        [267, 279],
      ])
        ellipse(x, y, 3, 4, "#9fd5ac");
  } else if (system === 4 || system === 5) {
    const color = system === 4 ? palette.bone : "#ac6a62";
    const w = system === 4 ? 6 : 14;
    for (const p of [
      armR,
      armL,
      [
        [240, 94],
        [240, 153],
        [240, 249],
      ],
      [
        [239, 267],
        [211, 292],
        [209, 380],
        [214, 481],
        [208, 529],
      ],
      [
        [241, 267],
        [270, 292],
        [268, 380],
        [260, 481],
        [266, 529],
      ],
    ])
      stroke(p, color, w);
    for (let i = 0; i < 9; i++)
      for (const s of [-1, 1])
        stem(
          "M240 " +
            (132 + i * 7) +
            " Q" +
            (240 + s * 42) +
            " " +
            (120 + i * 7) +
            " " +
            (240 + s * 32) +
            " " +
            (141 + i * 7) +
            " Q" +
            (240 + s * 16) +
            " " +
            (156 + i * 7) +
            " 240 " +
            (150 + i * 7),
          color,
          2.8,
        );
    shape(
      "M207 243 Q223 239 240 254 Q258 236 273 244 Q284 268 258 281 L241 293 221 280 Q204 269 207 243Z",
      color,
    );
  } else if (system === 6) {
    shape(
      "M222 200 Q211 186 201 196 Q191 212 204 225 Q216 234 218 220 Q208 213 219 209Z",
      "#c78679",
    );
    stem(
      "M213 211 Q220 225 223 250 L230 274 M269 212 Q262 227 259 250 L250 274",
      "#dbc6aa",
      3,
    );
    ellipse(240, 275, 14, 12, "#cfa38c");
  } else {
    stem(
      "M215 310 Q197 289 211 267 Q222 247 233 270 M265 310 Q283 289 269 267 Q258 247 247 270",
      "#dba594",
      3,
    );
    ellipse(214, 311, 7, 10, "#c5967f");
    ellipse(266, 311, 7, 10, "#c5967f");
  }
}
function wave(points, q, color, width) {
  const lengths = [0];
  for (let i = 1; i < points.length; i++)
    lengths[i] =
      lengths[i - 1] +
      Math.hypot(
        points[i][0] - points[i - 1][0],
        points[i][1] - points[i - 1][1],
      );
  const L = lengths.at(-1),
    pos = q * L,
    span = Math.min(32, L * 0.55);
  for (let i = 1; i < points.length; i++) {
    const a = lengths[i - 1],
      b = lengths[i],
      steps = Math.ceil((b - a) / 3);
    for (let j = 0; j < steps; j++) {
      let t = j / steps,
        delta = Math.abs(a + (b - a) * t - pos);
      if (delta > span) continue;
      const p = points[i - 1],
        v = points[i],
        lerp = (k) => [p[0] + (v[0] - p[0]) * k, p[1] + (v[1] - p[1]) * k];
      ctx.globalAlpha = (1 - delta / span) ** 1.25;
      stroke([lerp(t), lerp((j + 1) / steps)], color, width);
    }
  }
  ctx.globalAlpha = 1;
}
function stage() {
  if (system === 0) return phase < 0.5 ? "Inspiração" : "Expiração";
  if (system === 1)
    return [
      "Coração direito → pulmões",
      "Pulmões → coração esquerdo",
      "Coração esquerdo → mão esquerda",
      "Retorno ao coração direito",
    ][Math.min(3, Math.floor(phase * 4))];
  if (system === 2)
    return phase < 0.28
      ? "Boca e esôfago"
      : phase < 0.4
        ? "Estômago"
        : phase < 0.7
          ? "Intestino delgado"
          : "Intestino grosso e eliminação";
  return phase < 0.55
    ? "Mão direita e nervos do braço"
    : phase < 0.82
      ? "Chegada à medula cervical"
      : "Tronco encefálico e encéfalo";
}
function drawProcess() {
  if (system === 0)
    airBranches.forEach((p) =>
      wave(p, 1 - Math.abs(2 * phase - 1), "#a6efff", 7),
    );
  if (system === 1) {
    let i = Math.min(3, Math.floor(phase * 4)),
      q = (phase * 4) % 1;
    if (i < 2)
      for (const p of [leftLung, rightLung])
        wave(
          i === 0 ? p.slice(0, 4) : p.slice(3),
          q,
          i === 0 ? "#85c5ff" : "#ffb0a3",
          7,
        );
    else
      wave(
        i === 2 ? systemicOut : systemicBack,
        q,
        i === 2 ? "#ffaba0" : "#92c9ff",
        6,
      );
  }
  if (system === 2) wave(bolus, phase, "#ffdb9a", 7);
  if (system === 3) wave(sense, phase, "#fff1ab", 5);
}
function projection() {
  return (
    Math.sign(Math.cos(yaw) || 1) * Math.max(0.22, Math.abs(Math.cos(yaw)))
  );
}
function draw() {
  ctx.clearRect(0, 0, 480, 620);
  ellipse(240, 558, 100, 14, "#173250");
  ellipse(240, 552, 99, 12, "#40657c");
  ctx.save();
  ctx.translate(240, 0);
  ctx.scale(projection(), 1);
  ctx.translate(-240, 0);
  if (skin) {
    ctx.fillStyle = "#9cb1bf38";
    ctx.fill(body);
    ctx.strokeStyle = "#becddb99";
    ctx.lineWidth = 1.6;
    ctx.stroke(body);
  }
  routesBase();
  ctx.save();
  ctx.translate(offset[0] / projection(), offset[1]);
  const subject = subjects[system];
  shape(
    subject.part,
    subject.color,
    inspected || hover ? "#a1ffed" : "#f0d7ce75",
    inspected || hover ? 3 : 1,
  );
  ctx.restore();
  if (progress !== "off" && !held && !returning) drawProcess();
  ctx.restore();
  if (inspected) {
    const [x, y] = labelAnchors[system];
    const end = [240 + (x - 240) * projection() + offset[0], y + offset[1]];
    const lineY = Math.max(88, Math.min(end[1], 460));
    ctx.font = "bold 13px Montserrat, sans-serif";
    ctx.fillStyle = "#f0f6ff";
    ctx.fillText(subject.name, 9, lineY - 9);
    stroke(
      [
        [9, lineY],
        [77, lineY],
        [end[0] - 8, end[1]],
      ],
      "#c3d9ed",
      1,
    );
  }
}
function sync() {
  const s = subjects[system];
  $("#title").textContent = "Sistema " + names[system].toLowerCase();
  $("#skin").setAttribute("aria-pressed", String(skin));
  document
    .querySelectorAll(".systems button")
    .forEach((b, i) => b.setAttribute("aria-pressed", String(i === system)));
  $("#process").hidden = system > 3;
  $("#process").textContent =
    progress === "running"
      ? "Pausar percurso"
      : progress === "paused"
        ? "Retomar percurso"
        : "Observar percurso";
  $("#query").hidden = !inspected;
  $("#organ").textContent = s.name;
  $("#function").textContent = s.function;
  $("#context").textContent = s.context;
  $("#status").textContent = held
    ? "Solte para devolver à posição anatômica."
    : inspected
      ? s.canHold
        ? "Segure e mova para examinar. Solte para devolver."
        : "Estrutura fixa para consulta."
      : "Selecione uma estrutura para consultar.";
  $("#legend").textContent =
    system > 3
      ? "Exploração anatômica estática."
      : initialLegend[system] +
        (held || returning || progress === "off"
          ? ""
          : " " + stage() + (progress === "paused" ? " · pausado." : "."));
  $("#dial").style.transform = "rotate(" + yaw + "rad)";
  draw();
}
function choose(index) {
  system = index;
  inspected = held = returning = hover = false;
  drag = null;
  offset = [0, 0];
  phase = 0;
  progress = "off";
  sync();
}
function toggle() {
  if (system > 3 || held || returning) return;
  progress = progress === "running" ? "paused" : "running";
  sync();
}
names.forEach((name, i) => {
  const b = document.createElement("button");
  b.textContent = name;
  b.onclick = () => choose(i);
  $(".systems").append(b);
});
$("#skin").onclick = () => {
  skin = !skin;
  sync();
};
$("#process").onclick = toggle;
$("#close").onclick = () => {
  inspected = false;
  sync();
};
$("#back").onclick = () => $("dialog").showModal();
$("#enter").onclick = () => {
  $("dialog").close();
  yaw = 0;
  skin = true;
  choose(0);
};
function pointer(e) {
  const r = canvas.getBoundingClientRect();
  return [
    ((e.clientX - r.left) * 480) / r.width,
    ((e.clientY - r.top) * 620) / r.height,
  ];
}
function hit(p) {
  return ctx.isPointInPath(
    new Path2D(subjects[system].part),
    240 + (p[0] - 240 - offset[0]) / projection(),
    p[1] - offset[1],
  );
}
canvas.onpointerdown = (e) => {
  const p = pointer(e);
  if (!hit(p)) return;
  canvas.setPointerCapture(e.pointerId);
  drag = p;
  inspected = true;
  sync();
};
canvas.onpointermove = (e) => {
  const p = pointer(e);
  if (drag && subjects[system].canHold) {
    const dx = p[0] - drag[0],
      dy = p[1] - drag[1];
    if (Math.hypot(dx, dy) > 5 || held) {
      held = true;
      offset = [dx, dy];
      sync();
    }
  } else {
    hover = hit(p);
    canvas.style.cursor = hover ? "pointer" : "default";
    draw();
  }
};
function release() {
  drag = null;
  if (held) {
    held = false;
    if (reduced.matches) offset = [0, 0];
    else {
      returnStart = performance.now();
      returningFrom = offset;
      returning = true;
    }
  }
  sync();
}
canvas.onpointerup = canvas.onpointercancel = release;
canvas.onpointerleave = () => {
  hover = false;
  draw();
};
canvas.onkeydown = (e) => {
  if (e.key === "Enter") {
    e.preventDefault();
    inspected = true;
    sync();
  }
};
const dial = $("#dial");
dial.onpointerdown = (e) => {
  dial.setPointerCapture(e.pointerId);
  dialDrag = { x: e.clientX, yaw };
};
dial.onpointermove = (e) => {
  if (dialDrag) {
    yaw = dialDrag.yaw + (e.clientX - dialDrag.x) * 0.015;
    sync();
  }
};
dial.onpointerup = dial.onpointercancel = () => (dialDrag = null);
dial.onkeydown = (e) => {
  if (["ArrowLeft", "ArrowRight"].includes(e.key)) {
    e.preventDefault();
    yaw += ((e.key === "ArrowRight" ? 1 : -1) * Math.PI) / 12;
    sync();
  }
};
function render(time) {
  const dt = lastFrame ? Math.min(0.05, (time - lastFrame) / 1000) : 0;
  lastFrame = time;
  if (returning) {
    const t = Math.min(1, (time - returnStart) / 300);
    offset = returningFrom.map((x) => x * (1 - t) ** 3);
    if (t === 1) {
      returning = false;
      sync();
    }
  }
  if (progress === "running" && !held && !returning) {
    const s = stage();
    phase = (phase + dt / durations[system]) % 1;
    if (stage() !== s) sync();
  }
  draw();
  requestAnimationFrame(render);
}
const state = new URLSearchParams(location.search).get("state");
choose({ grab: 1, path: 1, paused: 3, static: 5 }[state] ?? 0);
if (state === "detail" || state === "static") {
  inspected = true;
}
if (state === "grab") {
  inspected = held = true;
  offset = [125, 85];
}
if (state === "path" || state === "paused") {
  skin = true;
  phase = state === "path" ? 0.65 : 0.45;
  progress = state === "paused" ? "paused" : "running";
}
sync();
requestAnimationFrame(render);
