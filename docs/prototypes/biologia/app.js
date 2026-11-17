"use strict";
const $ = (id) => document.getElementById(id);
const labs = [
  {
    id: "celulas",
    title: "Células animal e vegetal",
    verb: "Observe, separe e compare organelas.",
    question: "O que as duas células têm em comum e o que as diferencia?",
    legacy: "2 modelos celulares desmontáveis",
    xr: "Aponte para consultar uma organela. Segure uma peça para separá-la dentro da área de estudo; solte no seu socket para reencaixar. Um anel na base gira o conjunto, mantendo a posição. Duas células permanecem lado a lado.",
  },
  {
    id: "anatomia",
    title: "Corpo humano e sistemas",
    verb: "Um corpo, várias camadas e percursos.",
    question: "Como órgãos e sistemas se organizam e trabalham juntos?",
    legacy: "10 conjuntos anatômicos + corpo feminino de referência",
    xr: "Gire o corpo pelo anel da base. Escolha um sistema nos botões físicos do suporte, aponte para um órgão e consulte uma única ficha lateral. A pele é uma camada independente. Os percursos são animações esquemáticas, não fisiologia quantitativa.",
  },
  {
    id: "microbios",
    title: "Microrganismos e vírus",
    verb: "Compare estruturas e explore um bacteriófago.",
    question:
      "Como distinguir uma bactéria de um vírus e de uma estrutura celular?",
    legacy: "7 exemplares + 2 estruturas complementares",
    xr: "Selecione diretamente um exemplar da galeria. Use a pega na base para girar sem carregá-lo pela sala. No diorama do bacteriófago, avance três etapas por botão físico. Flagelo e cromossomo são estruturas, não organismos.",
  },
  {
    id: "dna",
    title: "DNA, RNA e informação",
    verb: "Encaixe bases e descubra a complementaridade.",
    question: "Quais bases podem se parear na cópia e na transcrição?",
    legacy: "Hélice DNA + cromossomo",
    xr: "Pegue uma base da bandeja e aproxime do socket da fita. Uma base incompatível recebe contorno de recusa e permanece na mão. DNA complementar e RNA são duas montagens independentes; alternar não apaga a outra. Gire a hélice pela base.",
  },
  {
    id: "ecossistemas",
    title: "Ecossistemas e relações",
    verb: "Construa uma cadeia e siga energia e matéria.",
    question: "Por onde passam energia e matéria em uma cadeia alimentar?",
    legacy: "Cena inicial; diorama e fichas novos",
    xr: "Pegue três fichas da bandeja e encaixe na ordem produtor → herbívoro → predador. As setas apontam do recurso para o consumidor. O módulo dos decompositores mostra retorno de matéria ao ambiente; a energia não retorna em ciclo.",
  },
  {
    id: "evolucao",
    title: "Evolução e seleção natural",
    verb: "Compare populações ao longo de gerações.",
    question:
      "O ambiente muda indivíduos ou a frequência de características na população?",
    legacy: "Cena inicial; modelo populacional novo",
    xr: "Mude o ambiente pela alavanca Claro/Escuro e avance uma geração por botão físico. Marque uma referência para comparar frequências. A reprodução é representada por descendentes; os indivíduos existentes não mudam de cor por vontade.",
  },
];
const info = {
  nutrients: [
    "Matéria e energia",
    "A matéria pode circular entre componentes do ecossistema. A energia entra, é transferida e se dissipa como calor.",
    "As setas da cadeia apontam do recurso para o consumidor.",
  ],
  selection: [
    "Seleção natural",
    "Diferenças de sobrevivência e reprodução podem alterar frequências na população ao longo das gerações.",
    "Este modelo é determinístico e não simula deriva genética ou mutação.",
  ],
  nucleus: [
    "Núcleo",
    "Abriga a maior parte do DNA da célula e participa do controle da expressão gênica.",
    "Presente nas células animal e vegetal.",
  ],
  mito: [
    "Mitocôndria",
    "Participa da respiração celular e da produção de ATP.",
    "Também existe na célula vegetal.",
  ],
  chloro: [
    "Cloroplasto",
    "Realiza fotossíntese em células fotossintéticas vegetais.",
    "Não aparece na célula animal deste modelo.",
  ],
  vacuole: [
    "Vacúolo central",
    "Armazenamento e manutenção do turgor na célula vegetal.",
    "Representação ampliada; forma e cor são didáticas.",
  ],
  membrane: [
    "Membrana plasmática",
    "Delimita a célula e participa do controle das trocas com o meio.",
    "Não confundir com parede celular.",
  ],
  wall: [
    "Parede celular",
    "Camada externa que contribui para sustentação e proteção.",
    "Na célula vegetal, a parede contém celulose.",
  ],
  heart: [
    "Coração",
    "Bombeia o sangue pelos circuitos pulmonar e sistêmico.",
    "Artérias e veias são definidas pelo sentido do fluxo em relação ao coração.",
  ],
  lungs: [
    "Pulmões",
    "A troca de gases ocorre entre o ar alveolar e o sangue capilar.",
    "Respiração pulmonar e respiração celular são processos diferentes.",
  ],
  stomach: [
    "Estômago",
    "Participa da digestão, com ações mecânicas e químicas.",
    "O diagrama não calcula absorção, pH ou tempo digestivo.",
  ],
  brain: [
    "Encéfalo e nervos",
    "Participam da integração de informações e do controle de respostas.",
    "Percurso visual simplificado; não representa diagnóstico.",
  ],
  skeleton: [
    "Esqueleto",
    "Sustentação, proteção e participação nos movimentos.",
    "Os grupos do modelo não equivalem a ossos individuais selecionáveis.",
  ],
  muscles: [
    "Músculos",
    "Produzem forças e participam dos movimentos em conjunto com o esqueleto.",
    "Não há rig biomecânico confirmado no acervo.",
  ],
  renal: [
    "Rins e vias urinárias",
    "Participam da formação e condução da urina e da regulação do meio interno.",
    "A forma do modelo não é uma simulação de filtração.",
  ],
  lymph: [
    "Sistema linfático",
    "Relaciona-se ao retorno de líquido intersticial e à defesa do organismo.",
    "Cores e espessuras são representações anatômicas.",
  ],
  reproductive: [
    "Sistema reprodutivo",
    "Estruturas relacionadas à reprodução.",
    "Consulta anatômica educativa, com vocabulário científico.",
  ],
  bacteria: [
    "Bactéria · modelo genérico",
    "Organismo celular procarionte. Possui membrana, citoplasma, ribossomos e material genético.",
    "A aparência artística não permite identificar uma espécie.",
  ],
  salmonella: [
    "Salmonella · modelo do acervo",
    "Representação artística de uma bactéria identificada no arquivo como Salmonella.",
    "Identificação e estruturas requerem revisão científica; sem cultivo ou diagnóstico.",
  ],
  bacillus: [
    "Bacilo · morfologia",
    "Bactéria com forma alongada. O nome de arquivo não valida espécie ou gênero taxonômico.",
    "Não classificar apenas pela cor do asset.",
  ],
  virus: [
    "Vírus · modelo genérico",
    "Entidade acelular. A replicação depende de células hospedeiras.",
    "Cor, brilho e espículas do modelo não validam uma espécie.",
  ],
  phage: [
    "Bacteriófago",
    "Vírus que infecta bactérias. O diorama explora um ciclo lítico simplificado.",
    "A associação do diorama é ilustrativa; não prova compatibilidade com Salmonella.",
  ],
  flagellum: [
    "Flagelo · estrutura",
    "Estrutura associada à motilidade em muitas bactérias.",
    "É parte de um organismo, não um microrganismo separado.",
  ],
  chromosome: [
    "Cromossomo · estrutura",
    "Organização do material genético. Este asset tem forma de X.",
    "A forma de X representa um cromossomo duplicado condensado; não é a forma universal do DNA.",
  ],
};
let lab = null,
  s = {};
function defaults() {
  return {
    labels: false,
    selected: null,
    detached: false,
    skin: false,
    system: "resp",
    playing: false,
    phase: 0,
    angle: 0,
    mode: "DNA",
    slot: 0,
    dna: ["", "", "", ""],
    rna: ["", "", "", ""],
    chain: [],
    energy: false,
    dark: false,
    generation: 0,
    counts: [12, 12],
    reference: null,
  };
}
function msg(t, error = false) {
  $("message").textContent = t;
  $("message").classList.toggle("error", error);
}
function select(key) {
  s.selected = key;
  const d = info[key] || [
    key,
    "Objeto selecionado.",
    "Representação conceitual.",
  ];
  $("detailTitle").textContent = d[0];
  $("detailText").textContent = d[1];
  $("detailExtra").textContent = d[2];
  $("detail").hidden = false;
  renderScene();
}
function close() {
  s.selected = null;
  $("detail").hidden = true;
  renderScene();
}
function bg() {
  return `<path d="M0 0H900V350L780 435H120L0 350Z" fill="#52627c"/><path d="M0 350L120 435H780L900 350V510H0" fill="#061125"/><path d="M0 350L120 435H780L900 350M0 45H900" fill="none" stroke="#1688ee" stroke-width="4"/><path d="M150 0V375M750 0V375M0 350H900" fill="none" stroke="#24374e" stroke-width="5"/><path d="M100 510L320 435M800 510L580 435M0 480H900" stroke="#284ea0" fill="none"/><ellipse class="plinth" cx="450" cy="426" rx="330" ry="35"/><path d="M120 426V452Q450 510 780 452V426" fill="#19283e"/>`;
}
function part(key, shape, label, x, y) {
  return `<g role="button" tabindex="0" data-part="${key}" aria-label="Consultar ${info[key]?.[0] || key}"><g transform="${s.detached === key ? "translate(-40 -20)" : ""}" class="part ${s.selected === key ? "selected" : ""}" stroke="${s.selected === key ? "#ffe795" : "#07386f"}">${shape}</g>${s.labels ? `<text x="${x}" y="${y}" text-anchor="middle">${label}</text>` : ""}</g>`;
}
info.nucleusA = info.nucleus;
info.nucleusV = info.nucleus;
info.mitoV = info.mito;
function drawCells() {
  const detached = 0;
  return `<text x="250" y="92" text-anchor="middle">Célula animal</text><text x="650" y="92" text-anchor="middle">Célula vegetal</text><g transform="translate(0,0)"><ellipse cx="250" cy="268" rx="155" ry="128" fill="#dca58b" stroke="#f5bda4" stroke-width="7"/><ellipse cx="250" cy="268" rx="142" ry="115" fill="#80465b"/>${part("nucleusA", `<circle cx="${225 - detached}" cy="250" r="45" fill="#8558c0"/>`, "Núcleo", 225, 240)}${part("mito", '<ellipse cx="320" cy="307" rx="35" ry="18" fill="#e1a85c"/><path d="M293 307l12-9 10 15 11-14 14 10" stroke="#794d3c" fill="none"/>', "Mitocôndria", 320, 347)}${part("membrane", '<path d="M130 206Q98 265 134 319" stroke="#ffc8af" stroke-width="14" fill="none"/>', "Membrana", 143, 375)}<rect x="485" y="143" width="310" height="255" rx="40" fill="#6bac8a" stroke="#a0c79a" stroke-width="8"/><rect x="501" y="159" width="278" height="222" rx="25" fill="#3f856e"/>${part("vacuole", '<ellipse cx="655" cy="270" rx="76" ry="80" fill="#81c7d9"/>', "Vacúolo", 655, 280)}${part("chloro", '<ellipse cx="548" cy="222" rx="24" ry="36" fill="#b3d862"/><path d="M535 205h27M535 216h27M535 227h27M535 238h27" stroke="#3c7550"/>', "Cloroplasto", 548, 185)}${part("nucleusV", '<circle cx="741" cy="312" r="28" fill="#8558c0"/>', "Núcleo", 741, 366)}${part("mitoV", '<ellipse cx="545" cy="327" rx="25" ry="14" fill="#e1a85c"/>', "Mitocôndria", 545, 365)}${part("wall", '<path d="M790 200V340" stroke="#b9d598" stroke-width="13"/>', "Parede", 810, 185)}</g>`;
}
const systems = [
  ["resp", "Respiratório", "lungs"],
  ["circ", "Circulatório", "heart"],
  ["dig", "Digestório", "stomach"],
  ["nerv", "Nervoso", "brain"],
  ["skel", "Esquelético", "skeleton"],
  ["muscle", "Muscular", "muscles"],
  ["renal", "Urinário", "renal"],
  ["lymph", "Linfático", "lymph"],
  ["repro", "Reprodutivo", "reproductive"],
];
function drawAnatomy() {
  let key = systems.find((x) => x[0] === s.system)[2],
    shape = "";
  if (s.system === "resp")
    shape =
      '<path d="M450 172V230M450 216l-30 25M450 216l30 25" stroke="#edd5bd" stroke-width="12" fill="none"/><ellipse cx="414" cy="257" rx="28" ry="50" fill="#d59ca7"/><ellipse cx="486" cy="257" rx="28" ry="50" fill="#d59ca7"/>';
  else if (s.system === "circ")
    shape =
      '<path d="M450 200V365M450 248L390 315M450 248L510 315" stroke="#e998a5" stroke-width="9" fill="none"/><path d="M454 232C426 203 421 249 456 263C490 239 480 210 454 232Z" fill="#c34a5d"/>';
  else if (s.system === "dig")
    shape =
      '<path d="M450 174V250q-35 4-18 35q25 25 47-9" fill="#da9b6b" stroke="#e8c399" stroke-width="10"/><path d="M421 302h60v12h-60v12h60v12h-60" fill="none" stroke="#d39d9b" stroke-width="10"/>';
  else if (s.system === "nerv")
    shape =
      '<ellipse cx="450" cy="139" rx="28" ry="24" fill="#dcbd92"/><path d="M450 164V354M450 215L392 308M450 215L508 308M450 300L429 404M450 300L471 404" fill="none" stroke="#ebd56d" stroke-width="7"/>';
  else if (s.system === "skel")
    shape =
      '<path d="M450 165V340M405 208H495M415 225H485M415 242H485M417 259H483M400 207L382 305M500 207L518 305M450 330L423 414M450 330L477 414" stroke="#e8e0c8" stroke-width="10" fill="none"/>';
  else if (s.system === "muscle")
    shape =
      '<path d="M414 205L397 291M486 205L503 291M433 340L423 407M467 340L477 407" stroke="#cb7c7c" stroke-width="23" fill="none"/>';
  else if (s.system === "renal")
    shape =
      '<ellipse cx="425" cy="278" rx="17" ry="24" fill="#b27983"/><ellipse cx="475" cy="278" rx="17" ry="24" fill="#b27983"/><path d="M425 300L448 340M475 300L452 340" stroke="#ecd9a8" fill="none" stroke-width="6"/><circle cx="450" cy="345" r="15" fill="#e1bb8a"/>';
  else if (s.system === "lymph")
    shape =
      '<path d="M450 190V355M450 220L395 310M450 220L505 310" stroke="#8dd49b" fill="none" stroke-width="8"/>';
  else shape = '<path d="M436 331Q450 310 464 331V360H436Z" fill="#d9ada9"/>';
  return `<text x="450" y="85" text-anchor="middle">${systems.find((x) => x[0] === s.system)[1]} · um mesmo corpo</text><g transform="translate(0,0)"><circle cx="450" cy="138" r="35" fill="#bbaa9a"/><path d="M410 182Q450 168 490 182L530 310L509 320L482 251V325L493 421H467L450 346L433 421H407L418 325V251L391 320L370 310Z" fill="${s.skin ? "#bcae9e" : "#30445c"}" stroke="#c2bdac" stroke-width="3"/>${part(key, `<g opacity="${s.skin ? 0.35 : 1}" class="${s.playing ? "scientific" : ""}">${shape}</g>`, info[key][0], 450, 465)}${s.playing ? '<path class="flowpath" d="M450 180V350" stroke="#70e0e5" stroke-width="7"/>' : ""}</g>`;
}
const microbes = [
  ["virus", "Vírus genérico"],
  ["salmonella", "Salmonella"],
  ["virus", "Vírus luminoso"],
  ["bacteria", "Bactéria verde"],
  ["flagellum", "Flagelo"],
  ["chromosome", "Cromossomo"],
  ["bacteria", "Bactéria genérica"],
  ["phage", "Bacteriófago"],
  ["bacillus", "Bacilo"],
];
function drawMicrobes() {
  let shapes = microbes
    .map(([key, name], i) => {
      let x = 160 + (i % 5) * 145,
        y = i < 5 ? 210 : 355,
        shape;
      if (key === "phage")
        shape = `<polygon points="${x - 20},${y - 32} ${x + 20},${y - 32} ${x + 30},${y} ${x},${y + 18} ${x - 30},${y}" fill="#b296d6"/><path d="M${x} ${y + 18}v35l-32 20m32-20 32 20" stroke="#b3c8e1" stroke-width="6" fill="none"/>`;
      else if (key === "virus")
        shape =
          `<circle cx="${x}" cy="${y}" r="32" fill="#b66f9b"/>` +
          Array.from({ length: 8 }, (_, a) => {
            let ang = (a * Math.PI) / 4;
            return `<path d="M${x + 32 * Math.cos(ang)} ${y + 32 * Math.sin(ang)}l${12 * Math.cos(ang)} ${12 * Math.sin(ang)}" stroke="#dca1c0" stroke-width="7"/>`;
          }).join("");
      else if (key === "chromosome")
        shape = `<path d="M${x - 22} ${y - 35}l44 70m0-70-44 70" stroke="#b9a5ce" stroke-width="14"/>`;
      else if (key === "flagellum")
        shape = `<path d="M${x - 45} ${y}q25-50 45 0t45 0" stroke="#e1c18b" stroke-width="6" fill="none"/>`;
      else
        shape = `<rect x="${x - 39}" y="${y - 20}" width="78" height="40" rx="20" fill="${key === "salmonella" ? "#cd9e79" : "#7cae91"}"/><path d="M${x + 39} ${y}q30 30 40 0" stroke="#a4d9bd" stroke-width="3" fill="none"/>`;
      info[key + "_" + i] = info[key];
      return (
        part(key + "_" + i, shape, name, x, y + 65) +
        `<text class="small" x="${x}" y="${y - 57}" text-anchor="middle">${name}</text>`
      );
    })
    .join("");
  return `<text x="450" y="86" text-anchor="middle">Galeria · 7 exemplares e 2 estruturas</text>${shapes}${s.phase ? `<text x="450" y="478" text-anchor="middle">Ciclo lítico · ${["", "1. Adsorção e entrada", "2. Síntese e montagem", "3. Lise e liberação"][s.phase]}</text><path class="flowpath" d="M520 365H650" stroke="#86e0d3" stroke-width="7"/>` : ""}`;
}
function drawDNA() {
  let a = s.mode === "DNA" ? s.dna : s.rna;
  return (
    `<text x="450" y="85" text-anchor="middle">${s.mode === "DNA" ? "Fita complementar de DNA" : "RNA complementar à fita molde de DNA"}</text><path d="M310 140Q200 270 310 400M590 140Q700 270 590 400" stroke="#9ab1df" stroke-width="12" fill="none"/>` +
    Array.from("ATGC")
      .map((b, i) => {
        let y = 170 + i * 63;
        return `<text x="330" y="${y}" text-anchor="middle">${b}</text><path d="M355 ${y - 7}H535" stroke="#5c769c" stroke-width="5"/><g role="button" tabindex="0" data-slot="${i}" aria-label="Socket ${i + 1}"><rect class="part ${s.slot === i ? "selected" : ""}" x="545" y="${y - 32}" width="52" height="45" rx="5" fill="${a[i] ? "#368169" : "#213958"}"/><text x="571" y="${y}" text-anchor="middle">${a[i] || "?"}</text></g>`;
      })
      .join("") +
    `<text class="small" x="450" y="455" text-anchor="middle">Fita molde DNA: 3′ — A T G C — 5′</text><text class="small" x="450" y="482" text-anchor="middle">${s.mode === 'DNA' ? 'Complementar DNA' : 'RNA'}: 5′ — ${a.map(x => x || '?').join(' ')} — 3′</text>`
  );
}
function drawEco() {
  const names = ["Capim", "Gafanhoto", "Sapo"];
  return (
    `<text x="450" y="85" text-anchor="middle">Cadeia alimentar · diorama didático</text><path d="M130 395Q300 350 420 390T770 390" fill="none" stroke="#7fa98c" stroke-width="35"/>` +
    names
      .map((n, i) => {
        let x = 230 + i * 220;
        return `<rect x="${x - 60}" y="220" width="120" height="100" fill="${s.chain[i] ? "#578775" : "#213958"}" stroke="#a6c8dc" stroke-width="3"/><text x="${x}" y="260" text-anchor="middle">${s.chain[i] || "Encaixe " + (i + 1)}</text><text class="small" x="${x}" y="300" text-anchor="middle">${["Produtor", "Herbívoro", "Predador"][i]}</text>`;
      })
      .join("") +
    `<path d="M300 265h65m-10-10 10 10-10 10M520 265h65m-10-10 10 10-10 10" stroke="#e8c67c" fill="none" stroke-width="5" class="${s.energy ? "flowpath" : ""}"/><text x="450" y="388" text-anchor="middle">Decompositores → nutrientes no ambiente → produtores</text>${s.energy ? '<path d="M200 150L230 210M440 320L440 355M670 320L670 355" stroke="#f0cf86" stroke-width="5" class="flowpath"/><text class="small" x="450" y="150" text-anchor="middle">Entrada de energia solar · dissipação de calor nas transferências</text>' : ""}`
  );
}
function drawEvolution() {
  let total = s.counts[0] + s.counts[1];
  return (
    `<text x="450" y="80" text-anchor="middle">Geração ${s.generation} · ${s.dark ? "ambiente escuro" : "ambiente claro"}</text><rect x="155" y="125" width="590" height="275" fill="${s.dark ? "#26344a" : "#aa9e88"}" stroke="#9ab2ca" stroke-width="3"/>` +
    Array.from({ length: total }, (_, i) => {
      let x = 200 + (i % 8) * 70,
        y = 180 + Math.floor(i / 8) * 75;
      return `<ellipse cx="${x}" cy="${y}" rx="19" ry="12" fill="${i < s.counts[0] ? "#f0d9a5" : "#28333c"}" stroke="#0b1930" stroke-width="2"/>`;
    }).join("") +
    `<text x="450" y="445" text-anchor="middle">Claros ${s.counts[0]} · escuros ${s.counts[1]} · total 24</text>${s.reference ? `<text class="small" x="450" y="475" text-anchor="middle">Referência G${s.reference.g}: claros ${s.reference.c[0]} · escuros ${s.reference.c[1]}</text>` : ""}`
  );
}
function renderScene() {
  if (!lab) return;
  $("scene").innerHTML =
    bg() +
    `<g transform="translate(450 260) scale(${["celulas", "anatomia", "microbios", "dna"].includes(lab.id) ? Math.max(0.4, Math.abs(Math.cos((s.angle * Math.PI) / 6))) : 1} 1) translate(-450 -260)">` +
    {
      celulas: drawCells,
      anatomia: drawAnatomy,
      microbios: drawMicrobes,
      dna: drawDNA,
      ecossistemas: drawEco,
      evolucao: drawEvolution,
    }[lab.id]() +
    `</g>`;
  $("scene").classList.toggle("paused", !s.playing);
  if (["dna", "ecossistemas", "evolucao"].includes(lab.id)) {
    const key = {
      dna: "chromosome",
      ecossistemas: "nutrients",
      evolucao: "selection",
    }[lab.id];
    $("scene").insertAdjacentHTML(
      "beforeend",
      part(
        key,
        '<circle cx="800" cy="115" r="24" fill="#284ea0"/><text x="800" y="122" text-anchor="middle">i</text>',
        "",
        800,
        145,
      ),
    );
  }
  for (const n of document.querySelectorAll("[data-part]")) {
    n.onclick = () => select(n.dataset.part);
    n.onkeydown = (e) => {
      if (e.key === "Enter" || e.key === " ") {
        e.preventDefault();
        select(n.dataset.part);
      }
    };
  }
  for (const n of document.querySelectorAll("[data-slot]")) {
    n.onclick = () => {
      s.slot = +n.dataset.slot;
      renderScene();
      msg(
        "Socket " + (s.slot + 1) + " selecionado. Pegue uma base na bandeja.",
      );
    };
    n.onkeydown = (e) => {
      if (e.key === "Enter" || e.key === " ") {
        e.preventDefault();
        n.onclick();
      }
    };
  }
}
function control(text, action, pressed, id) {
  let b = document.createElement("button");
  b.textContent = text;
  if (id) b.id = id;
  if (pressed !== undefined) b.setAttribute("aria-pressed", pressed);
  b.onclick = () => {
    action();
    renderScene();
    renderControls();
  };
  $("controls").append(b);
}
function gesture(text, fn) {
  let b = document.createElement("button");
  b.textContent = text;
  b.onclick = () => {
    fn();
    renderScene();
  };
  $("gestureControls").append(b);
}
function renderControls() {
  $("controls").replaceChildren();
  $("gestureControls").replaceChildren();
  if (lab.id === "celulas") {
    control("Rótulos", () => (s.labels = !s.labels), s.labels);
    control("Recompor células", askReset);
    gesture("Simular pega e afastamento da organela", () => {
      if (
        ![
          "nucleus",
          "mito",
          "chloro",
          "vacuole",
          "nucleusA",
          "nucleusV",
          "mitoV",
        ].includes(s.selected)
      )
        return msg(
          "Selecione núcleo, mitocôndria, cloroplasto ou vacúolo para separar.",
          true,
        );
      s.detached = s.selected;
      msg(
        "Peça afastada dentro do volume de estudo. Reencaixe no socket original.",
      );
    });
    gesture("Simular reencaixe", () => {
      s.detached = false;
      msg("Peça reencaixada; forma e posição originais restauradas.");
    });
  }
  if (lab.id === "anatomia") {
    systems.forEach(([id, title]) =>
      control(
        title,
        () => {
          s.system = id;
          s.playing = false;
          close();
          msg(
            "Sistema " +
              title.toLowerCase() +
              " selecionado. Aponte para consultar.",
          );
        },
        s.system === id,
      ),
    );
    control("Pele", () => (s.skin = !s.skin), s.skin);
    if (["resp", "circ", "dig", "nerv"].includes(s.system))
      control(
        s.playing ? "Pausar percurso" : "Observar percurso",
        () => {
          s.playing = !s.playing;
          msg(
            s.playing
              ? "Percurso esquemático em movimento."
              : "Percurso pausado.",
          );
        },
        s.playing,
      );
  }
  if (lab.id === "microbios") {
    control("Rótulos de estruturas", () => (s.labels = !s.labels), s.labels);
    control("Próxima etapa do fago", () => {
      s.phase = (s.phase % 3) + 1;
      s.playing = true;
      select("phage_7");
      msg("Etapa " + s.phase + " do ciclo lítico simplificado.");
    });
    control("Restaurar galeria", askReset);
  }
  if (lab.id === "dna") {
    for (let mode of ["DNA", "RNA"])
      control(
        mode,
        () => {
          s.mode = mode;
          s.slot = Math.max(
            0,
            (mode === "DNA" ? s.dna : s.rna).findIndex((x) => !x),
          );
          msg(
            "Montagem " + mode + " recuperada. Escolha um socket e uma base.",
          );
        },
        s.mode === mode,
      );
    for (let base of ["A", "T", "G", "C", "U"])
      control("Base " + base, () => placeBase(base));
    control("Restaurar fitas", askReset);
  }
  if (lab.id === "ecossistemas") {
    for (let n of ["Capim", "Gafanhoto", "Sapo"])
      control("Encaixar " + n, () => {
        const target = ["Capim", "Gafanhoto", "Sapo"][s.chain.length];
        if (!target)
          return msg(
            "Cadeia completa. Observe o fluxo ou restaure para remontar.",
          );
        if (n !== target)
          return msg(
            "Este encaixe pede " +
              ["um produtor", "um herbívoro", "um predador"][s.chain.length] +
              ". A ficha continua disponível.",
            true,
          );
        s.chain.push(n);
        msg("Ficha encaixada: " + n + ".");
      });
    control(
      "Exibir fluxo",
      () => {
        s.energy = !s.energy;
        s.playing = s.energy;
        msg(
          "Energia flui; matéria pode retornar ao ambiente por decomposição.",
        );
      },
      s.energy,
    );
    control("Restaurar diorama", askReset);
  }
  if (lab.id === "evolucao") {
    control(
      "Ambiente " + (s.dark ? "escuro" : "claro"),
      () => {
        s.dark = !s.dark;
        msg("Ambiente alterado. Os indivíduos mantêm suas características.");
      },
      s.dark,
    );
    control("Avançar geração", () => {
      if (s.generation >= 6)
        return msg(
          "Limite de seis gerações. Compare a referência ou restaure.",
          true,
        );
      let n = s.counts[0];
      n = Math.round(
        (24 * (n * (s.dark ? 0.7 : 1))) /
          (n * (s.dark ? 0.7 : 1) + (24 - n) * (s.dark ? 1 : 0.7)),
      );
      s.counts = [n, 24 - n];
      s.generation++;
      msg(
        (s.generation === 6 ? "Limite de seis gerações atingido. " : "") +
          "Descendentes da geração " +
          s.generation +
          ". Vantagem de sobrevivência altera frequências; indivíduos não mudaram de cor.",
      );
    });
    control("Guardar referência", () => {
      s.reference = { g: s.generation, c: [...s.counts] };
      msg("Referência guardada para comparação.");
    });
    control("Restaurar população", askReset);
  }
  if (lab.id === "evolucao" && s.generation >= 6) {
    const b = [...$("controls").children].find(
      (x) => x.textContent === "Avançar geração",
    );
    b.disabled = true;
    b.title = "Limite de seis gerações; restaure para repetir.";
  }
  if (["celulas", "anatomia", "microbios", "dna"].includes(lab.id))
    gesture("Simular giro de 30° no anel da base", () => {
      s.angle++;
      msg(
        "Giro ancorado: o modelo muda de orientação e a base permanece fixa.",
      );
    });
}
function placeBase(base) {
  let expected = (s.mode === "DNA" ? "TACG" : "UACG")[s.slot];
  if (base !== expected)
    return msg(
      "Base " +
        base +
        " recusada neste socket. " +
        (s.mode === "DNA"
          ? "DNA usa A–T e C–G."
          : "Na transcrição, A do molde corresponde a U no RNA.") +
        " A peça permanece na mão.",
      true,
    );
  const arr = s.mode === "DNA" ? s.dna : s.rna;
  arr[s.slot] = base;
  const next = arr.findIndex((x) => !x);
  if (next >= 0) s.slot = next;
  msg(
    next < 0
      ? "Fita " + s.mode + " concluída. As bases são complementares."
      : "Encaixe confirmado. Próximo socket selecionado.",
  );
}
function askReset() {
  $("resetDialog").showModal();
}
function openLab(id, preset) {
  lab = labs.find((x) => x.id === id);
  if (!lab) return;
  s = defaults();
  $("hub").hidden = true;
  $("lab").hidden = false;
  $("number").textContent = "BIOLOGIA · " + (labs.indexOf(lab) + 1) + "/6";
  $("title").textContent = lab.title;
  $("question").textContent = lab.question;
  $("xrText").textContent = lab.xr;
  $("legend").textContent =
    lab.legacy +
    " · Escalas e cores didáticas, sem proporção real entre exemplares.";
  close();
  renderControls();
  msg(
    "Observe os objetos e experimente uma ação. Consulta abre pela seleção direta.",
  );
  if (preset === "detail")
    select(
      {
        celulas: "mito",
        anatomia: "lungs",
        microbios: "phage_7",
        dna: "chromosome",
        ecossistemas: "nutrients",
        evolucao: "selection",
      }[id],
    );
  if (preset === "action") {
    if (id === "celulas") {
      select("nucleusA");
      s.detached = "nucleusA";
    }
    if (id === "anatomia") {
      s.playing = true;
      select("lungs");
    }
    if (id === "microbios") {
      s.phase = 2;
      s.playing = true;
      select("phage_7");
    }
    if (id === "dna") s.dna = ["T", "A", "C", "G"];
    if (id === "ecossistemas") {
      s.chain = ["Capim", "Gafanhoto", "Sapo"];
      s.energy = true;
      s.playing = true;
    }
    if (id === "evolucao") {
      s.generation = 3;
      s.counts = [18, 6];
      s.reference = { g: 0, c: [12, 12] };
    }
    msg("Estado de ação · resultado da interação.");
  }
  if (preset === "error") {
    if (id === "dna") {
      s.slot = 0;
    }
    if (id === "evolucao") {
      s.generation = 6;
      s.counts = [21, 3];
    }
    msg(
      id === "dna"
        ? "Base incompatível. A peça permanece na mão; tente a complementar."
        : {
            celulas:
              "Selecione uma organela destacável antes de separar. Estado preservado.",
            anatomia:
              "Um órgão oculto não recebe seleção. Escolha um sistema visível.",
            microbios:
              "Aproximar exemplares não inicia infecção. Use o diorama cadastrado.",
            ecossistemas:
              "O primeiro encaixe pede um produtor; a ficha permanece na mão.",
            evolucao:
              "Limite de seis gerações. Compare a referência ou restaure.",
          }[id],
      true,
    );
  }
  renderScene();
  renderControls();
  if (preset === "reset") askReset();
}
$("cards").innerHTML = labs
  .map(
    (l, i) =>
      `<a class="card" href="?lab=${l.id}"><svg viewBox="0 0 300 150" aria-hidden="true"><ellipse cx="150" cy="125" rx="95" ry="12" fill="#24456e"/><g stroke="#3ed39c" fill="none" stroke-width="4">${
        [
          '<ellipse cx="150" cy="72" rx="58" ry="40"/><circle cx="137" cy="70" r="17"/><ellipse cx="178" cy="82" rx="13" ry="7"/>',
          '<circle cx="150" cy="35" r="14"/><path d="M132 55h36l20 40m-56-40-20 40m20-40v28l-10 35m46-63v28l10 35"/>',
          '<circle cx="150" cy="70" r="30"/><path d="M150 25v15m0 60v15m-45-45h15m60 0h15m-77-32 11 11m42 42 11 11m0-64-11 11m-42 42-11 11"/>',
          '<path d="M118 22q64 25 0 55t0 45m64-100q-64 25 0 55t0 45M127 35h45m-34 20h24m-24 40h24m-35 20h45M120 77h60"/>',
          '<path d="M100 100h45m-10-10 10 10-10 10m20-10h45m-10-10 10 10-10 10M100 95V60m-10 25 10-20 10 20"/><circle cx="175" cy="62" r="13"/>',
          '<ellipse cx="113" cy="60" rx="12" ry="8"/><ellipse cx="143" cy="60" rx="12" ry="8"/><path d="M100 90h90m-12-10 12 10-12 10"/><ellipse cx="190" cy="60" rx="12" ry="8"/><ellipse cx="175" cy="35" rx="12" ry="8"/>',
        ][i]
      }</g></svg><div class="card-text"><span class="tag">0${i + 1} · PROPOSTA</span><h2>${l.title}</h2><p>${l.verb}</p></div></a>`,
  )
  .join("");
$("hubButton").onclick = () => location.assign("?");
$("closeDetail").onclick = close;
$("cancel").onclick = () => $("resetDialog").close();
$("confirm").onclick = () => {
  $("resetDialog").close();
  s = defaults();
  close();
  renderControls();
  msg("Experiência restaurada.");
};
$("reduced").onclick = () => {
  let on = !document.body.classList.contains("reduced");
  document.body.classList.toggle("reduced", on);
  $("reduced").setAttribute("aria-pressed", on);
  try {
    localStorage.setItem("biologyReduced", on ? "1" : "0");
  } catch {}
};
try {
  if (localStorage.getItem("biologyReduced") === "1") $("reduced").click();
} catch {}
const query = new URLSearchParams(location.search);
if (query.has("lab")) openLab(query.get("lab"), query.get("state"));
