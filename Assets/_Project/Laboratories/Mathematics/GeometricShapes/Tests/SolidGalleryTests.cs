using System.Linq;
using NUnit.Framework;
using Tecaverso.Labs.Shapes;
using UnityEngine;

public class SolidGalleryTests
{
    [TestCase("Prisma triangular", 6, 9, 5)]
    [TestCase("Prisma pentagonal", 10, 15, 7)]
    [TestCase("Prisma hexagonal", 12, 18, 8)]
    [TestCase("Pirâmide de base quadrada", 5, 8, 5)]
    [TestCase("Paralelepípedo retângulo", 8, 12, 6)]
    public void GalleryPolyhedraHaveClosedGeometricFaces(string name, int v, int e, int f)
    {
        var shape = SolidGalleryCatalog.Create().First(d => d.shape.name == name).shape;
        Assert.AreEqual(v, shape.vertices.Length);
        Assert.AreEqual(e, shape.Edges.Length);
        Assert.AreEqual(f, shape.faces.Length);
        foreach (var edge in shape.Edges)
            Assert.AreEqual(2, shape.faces.Count(face => Enumerable.Range(0, face.Length).Any(i =>
                (face[i] == edge.Item1 && face[(i + 1) % face.Length] == edge.Item2) ||
                (face[i] == edge.Item2 && face[(i + 1) % face.Length] == edge.Item1))));
        float minY = shape.vertices.Min(p => p.y), maxY = shape.vertices.Max(p => p.y);
        var baseFace = shape.faces.First(face => face.All(i => Mathf.Abs(shape.vertices[i].y - minY) < .001f));
        float baseArea = ShapeGeometry.FaceArea(baseFace.Select(i => shape.vertices[i]).ToArray());
        float volume = baseArea * (maxY - minY) / (name.StartsWith("Pirâmide") ? 3 : 1);
        Assert.That(shape.Volume, Is.EqualTo(volume).Within(.0001f));
    }

    [TestCase("Tetraedro")]
    [TestCase("Octaedro")]
    [TestCase("Dodecaedro")]
    [TestCase("Icosaedro")]
    public void RegularSolidVolumeMatchesDisplayedFormula(string name)
    {
        var shape = SolidGalleryCatalog.Create().First(d => d.shape.name == name).shape;
        float a = Vector3.Distance(shape.vertices[shape.Edges[0].Item1], shape.vertices[shape.Edges[0].Item2]);
        float coefficient = name switch
        {
            "Tetraedro" => Mathf.Sqrt(2) / 12,
            "Octaedro" => Mathf.Sqrt(2) / 3,
            "Dodecaedro" => (15 + 7 * Mathf.Sqrt(5)) / 4,
            _ => 5 * (3 + Mathf.Sqrt(5)) / 12
        };
        Assert.That(shape.Volume, Is.EqualTo(coefficient * a * a * a).Within(.0001f));
    }
}
