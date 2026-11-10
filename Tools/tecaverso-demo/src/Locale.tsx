import { createContext, useContext } from "react";
export const EnglishContext = createContext(false);
const dictionary: Record<string, string> = {
  "Texturas planetárias": "Planetary textures",
  "Mathematics, made tangible.": "Mathematics, made tangible.",
  "Seis laboratórios interativos": "Six interactive virtual labs",
  "Formas geométricas": "Geometric Shapes",
  "Explore propriedades e modelos tridimensionais de formas geométricas.":
    "Explore the properties and three-dimensional models of geometric shapes.",
  "Funções e gráficos": "Functions and Graphs",
  "Explore funções e interprete suas representações no plano cartesiano.":
    "Explore functions and interpret their graphs on the Cartesian plane.",
  "Relacione expressões algébricas às curvas representadas no plano.":
    "Connect algebraic expressions to curves on the plane.",
  "Espaço cartesiano": "Cartesian Plane",
  "Localize pontos e explore coordenadas no plano cartesiano.":
    "Locate points and explore coordinates on the Cartesian plane.",
  "Geometria plana": "Plane Geometry",
  "Compare figuras, medidas e relações da geometria plana.":
    "Compare shapes, measurements, and relationships in plane geometry.",
  "Investigue resultados, distribuições e padrões observados nos dados.":
    "Investigate outcomes, distributions, and patterns in data.",
  "Compare quantidades e descubra relações proporcionais equivalentes.":
    "Compare quantities and discover equivalent proportional relationships.",
  "Probabilidade e estatística": "Probability and Statistics",
  "Investigue dados, distribuições e resultados de experimentos aleatórios.":
    "Investigate data, distributions and outcomes of random experiments.",
  "Frações e proporções": "Fractions and Proportions",
  "Compare frações e explore relações proporcionais com modelos visuais.":
    "Compare fractions and explore proportional relationships with visual models.",
  "Explore. Compare. Understand.": "Explore. Compare. Understand.",
  "Matemática interativa em realidade virtual":
    "Interactive mathematics in virtual reality",
  "LABORATÓRIOS EDUCACIONAIS EM REALIDADE VIRTUAL":
    "EDUCATIONAL LABORATORIES IN VIRTUAL REALITY",
  "MATEMÁTICA EM REALIDADE VIRTUAL": "MATHEMATICS IN VIRTUAL REALITY",
  "Experimente. Observe. Compreenda.": "Experiment. Observe. Understand.",
  "6 EXPERIMENTOS DE FÍSICA": "6 PHYSICS EXPERIMENTS",
  "A Física ganha": "Physics takes on",
  "uma nova dimensão.": "a new dimension.",
  "Explore relações. Compare resultados.":
    "Explore relationships. Compare results.",
  "FÍSICA EM REALIDADE VIRTUAL": "PHYSICS IN VIRTUAL REALITY",
  "Lançamento oblíquo": "Projectile Motion",
  "Compare trajetórias com diferentes ângulos de lançamento.":
    "Compare trajectories at different launch angles.",
  "Velocidade inicial: 8 m/s · reprodução original em câmera lenta (0,5×)":
    "Initial speed: 8 m/s · original footage in slow motion (0.5×)",
  "Plano inclinado": "Inclined Plane",
  "Observe peso, normal e atrito durante o deslizamento.":
    "Observe weight, normal force and friction as the block slides.",
  "Reprodução original em câmera lenta (0,5×)":
    "Original footage in slow motion (0.5×)",
  Gravitação: "Gravitation",
  "Explore as órbitas e as informações dos planetas.":
    "Explore planetary orbits and discover facts about each planet.",
  "Tempo acelerado · tamanhos e distâncias didáticos":
    "Accelerated time · sizes and distances scaled for teaching",
  "Associação de resistores": "Resistor Circuits",
  "Abra e feche o circuito. Observe a corrente e a lâmpada.":
    "Open and close the circuit. Observe the current and the light bulb.",
  Termologia: "Thermodynamics",
  "Relacione pressão, volume e temperatura nas transformações do gás.":
    "Explore how pressure, volume and temperature are related.",
  "Modelo de gás ideal": "Ideal gas model",
  Ondulatória: "Waves",
  "Investigue pulsos, ondas periódicas e ondas estacionárias.":
    "Investigate pulses, periodic waves and standing waves.",
};
export const useText = () => {
  const en = useContext(EnglishContext);
  return (text: string) => (en ? (dictionary[text] ?? text) : text);
};
