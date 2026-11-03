using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tecaverso.Labs.Shapes
{
    public sealed class SolidDefinition
    {
        public readonly ShapeData shape;
        public readonly string formula, legend;
        public SolidDefinition(ShapeData shape, string formula, string legend)
        { this.shape = shape; this.formula = formula; this.legend = legend; }
    }

    /// <summary>The fixed educational collection; rendering triangles are not mathematical edges.</summary>
    public static class SolidGalleryCatalog
    {
        public static List<SolidDefinition> Create()
        {
            var source = ShapeGeometry.Catalog();
            ShapeData Get(string name) => source.First(s => s.name == name).Copy();
            var box = Get("Cubo");
            box.name = "Paralelepípedo retângulo";
            box.vertices = box.vertices.Select(v => Vector3.Scale(v, new Vector3(1.25f, 1, .65f))).ToArray();
            var hex = ShapeGeometry.Prism(6); hex.name = "Prisma hexagonal";
            const string edge = "a: comprimento da aresta";
            const string prism = "Ab: área da base\nh: altura perpendicular à base";
            return new List<SolidDefinition>
            {
                new(Get("Tetraedro"), "V = √2 · a³ / 12", edge),
                new(ShapeGeometry.Prism(4, true), "V = Ab · h / 3", prism),
                new(ShapeGeometry.Prism(3), "V = Ab · h", prism),
                new(Get("Cubo"), "V = a³", edge),
                new(box, "V = a · b · c", "a, b, c: comprimento, largura e altura"),
                new(ShapeGeometry.Prism(5), "V = Ab · h", prism),
                new(Get("Octaedro"), "V = √2 · a³ / 3", edge),
                new(hex, "V = Ab · h", prism),
                new(Get("Dodecaedro"), "V = (15 + 7√5) · a³ / 4", edge),
                new(Get("Icosaedro"), "V = 5(3 + √5) · a³ / 12", edge),
                new(Get("Esfera"), "V = 4πr³ / 3", "r: raio da esfera\nSem vértices ou arestas")
            };
        }
    }
}
