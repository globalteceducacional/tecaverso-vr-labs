using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Tecaverso.Labs.Shapes;

namespace Tecaverso.Labs.PlanarGeometry
{
    public sealed class PlanarDimension
    {
        public string symbol;
        public Vector3 start, end, labelOffset, perpendicular;
        public bool rightAngle;
        public float Length => Vector3.Distance(start, end);
        public PlanarDimension(string symbol, Vector3 start, Vector3 end, Vector3 labelOffset,
            bool rightAngle = false, Vector3 perpendicular = default)
        {
            this.symbol = symbol; this.start = start; this.end = end; this.labelOffset = labelOffset;
            this.rightAngle = rightAngle; this.perpendicular = perpendicular;
        }
    }

    public sealed class PlanarDefinition
    {
        public ShapeData shape;
        public string formula, legend, perimeterFormula;
        public float formulaArea;
        public bool regular;
        public readonly List<PlanarDimension> dimensions = new();
        public Vector3[][] auxiliaryLoops = new Vector3[0][];
        public bool IsCircle => shape.curved == "circle";
        public int VertexCount => IsCircle ? 0 : shape.vertices.Length;
        public string Classification => IsCircle ? "Figura curva · não é polígono" :
            (regular ? "Regular" : "Irregular") + (ShapeGeometry.Convex(shape.vertices) ? " · convexo" : " · côncavo");
        public string Counts => IsCircle ? "Sem lados retos ou vértices" : $"{VertexCount} lados · {VertexCount} vértices";
    }

    public static class PlanarCatalog
    {
        static Vector3 V(float x, float y) => new(x, y, 0);
        static PlanarDefinition Polygon(string name, string formula, string legend, float area, params Vector3[] vertices)
        {
            return new PlanarDefinition
            {
                shape = new ShapeData { name = name, planar = true, vertices = vertices, faces = new[] { Enumerable.Range(0, vertices.Length).ToArray() } },
                formula = formula, legend = legend, formulaArea = area, perimeterFormula = "P = soma dos lados"
            };
        }
        static void BaseHeight(PlanarDefinition d, Vector3 left, Vector3 right, Vector3 top)
        {
            d.dimensions.Add(new PlanarDimension("b", left, right, Vector3.down * .16f));
            d.dimensions.Add(new PlanarDimension("h", V(top.x, left.y), top, Vector3.right * .16f, true, Vector3.right));
        }
        public static List<PlanarDefinition> Create()
        {
            var list = new List<PlanarDefinition>();
            for (int n = 3; n <= 8; n++)
            {
                var s = ShapeGeometry.Polygon(n, .95f);
                if (n == 4) s.vertices = new[] { V(-.8f,-.8f), V(.8f,-.8f), V(.8f,.8f), V(-.8f,.8f) };
                s.name = n == 3 ? "Triângulo equilátero" : n == 4 ? "Quadrado" : s.name + " regular";
                var a = s.vertices[0]; var b = s.vertices[1]; var midpoint = (a + b) / 2;
                var d = new PlanarDefinition { shape = s, regular = true, formula = "A = P · ap / 2",
                    legend = "P: perímetro · ap: apótema", perimeterFormula = $"P = {n} · l   (l: lado)", formulaArea = s.Perimeter * midpoint.magnitude / 2 };
                if (n == 3)
                {
                    d.formula = "A = b · h / 2"; d.legend = "b: base · h: altura perpendicular";
                    BaseHeight(d, s.vertices[1], s.vertices[2], s.vertices[0]);
                    d.formulaArea = d.dimensions[0].Length * d.dimensions[1].Length / 2;
                }
                else if (n == 4)
                {
                    d.formula = "A = l²"; d.legend = "l: comprimento do lado"; d.formulaArea = 1.6f * 1.6f;
                    d.dimensions.Add(new PlanarDimension("l", a, b, Vector3.down * .16f));
                }
                else
                {
                    d.dimensions.Add(new PlanarDimension("l", a, b, midpoint.normalized * .15f));
                    // Start at the foot: the right-angle marker is perpendicular to this actual side.
                    d.dimensions.Add(new PlanarDimension("ap", midpoint, Vector3.zero, (b-a).normalized * .16f, true, (b-a).normalized));
                }
                list.Add(d);
            }
            var triangle = Polygon("Triângulo escaleno", "A = b · h / 2", "b: base · h: altura perpendicular", 1.45f,
                V(-1,-.65f), V(1,-.65f), V(-.35f,.8f));
            BaseHeight(triangle, triangle.shape.vertices[0], triangle.shape.vertices[1], triangle.shape.vertices[2]); list.Add(triangle);
            var rectangle = Polygon("Retângulo", "A = b · h", "b: base · h: altura perpendicular", 2.6f,
                V(-1,-.65f), V(1,-.65f), V(1,.65f), V(-1,.65f));
            BaseHeight(rectangle, rectangle.shape.vertices[0], rectangle.shape.vertices[1], V(.65f,.65f)); list.Add(rectangle);
            var rhombus = Polygon("Losango", "A = D · d / 2", "D: diagonal maior · d: diagonal menor", 1.98f,
                V(0,-.9f), V(1.1f,0), V(0,.9f), V(-1.1f,0));
            rhombus.dimensions.Add(new PlanarDimension("D", V(-1.1f,0), V(1.1f,0), V(.45f,.16f)));
            rhombus.dimensions.Add(new PlanarDimension("d", V(0,-.9f), V(0,.9f), V(.16f,.45f))); list.Add(rhombus);
            var parallelogram = Polygon("Paralelogramo", "A = b · h", "b: base · h: altura perpendicular", 1.885f,
                V(-1,-.65f), V(.45f,-.65f), V(1,.65f), V(-.45f,.65f));
            BaseHeight(parallelogram, parallelogram.shape.vertices[0], parallelogram.shape.vertices[1], parallelogram.shape.vertices[3]); list.Add(parallelogram);
            var trapezoid = Polygon("Trapézio", "A = (B + b) · h / 2", "B, b: bases · h: altura perpendicular", 2.1f,
                V(-1,-.7f), V(1,-.7f), V(.55f,.7f), V(-.45f,.7f));
            BaseHeight(trapezoid, trapezoid.shape.vertices[0], trapezoid.shape.vertices[1], V(0,.7f));
            trapezoid.dimensions[0].symbol = "B";
            trapezoid.dimensions.Add(new PlanarDimension("b", trapezoid.shape.vertices[3], trapezoid.shape.vertices[2], Vector3.up * .16f)); list.Add(trapezoid);
            var concave = Polygon("Polígono em L", "A = B · H − b · h", "B, H: retângulo externo\nb, h: retângulo retirado", 2.14f,
                V(-.9f,-.9f), V(.9f,-.9f), V(.9f,-.2f), V(-.1f,-.2f), V(-.1f,.9f), V(-.9f,.9f));
            concave.dimensions.Add(new PlanarDimension("B", V(-.9f,-.9f), V(.9f,-.9f), Vector3.down * .16f));
            concave.dimensions.Add(new PlanarDimension("H", V(-.9f,-.9f), V(-.9f,.9f), Vector3.left * .16f));
            concave.dimensions.Add(new PlanarDimension("b", V(-.1f,.9f), V(.9f,.9f), Vector3.up * .16f));
            concave.dimensions.Add(new PlanarDimension("h", V(.9f,-.2f), V(.9f,.9f), Vector3.right * .16f));
            concave.auxiliaryLoops = new[] { new[] { V(-.9f,-.9f), V(.9f,-.9f), V(.9f,.9f), V(-.9f,.9f) },
                new[] { V(-.1f,-.2f), V(.9f,-.2f), V(.9f,.9f), V(-.1f,.9f) } }; list.Add(concave);
            const int segments = 96;
            var circle = Polygon("Círculo", "A = πr²", "r: raio · C: comprimento da circunferência", Mathf.PI * .95f * .95f,
                Enumerable.Range(0, segments).Select(i => V(Mathf.Cos(i*2*Mathf.PI/segments), Mathf.Sin(i*2*Mathf.PI/segments)) * .95f).ToArray());
            circle.shape.curved = "circle"; circle.shape.radius = .95f; circle.perimeterFormula = "C = 2πr";
            circle.dimensions.Add(new PlanarDimension("r", Vector3.zero, Vector3.right * .95f, Vector3.up * .16f)); list.Add(circle);
            return list;
        }
    }
}
