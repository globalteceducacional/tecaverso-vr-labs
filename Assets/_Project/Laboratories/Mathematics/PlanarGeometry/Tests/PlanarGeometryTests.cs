using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Tecaverso.Labs.Shapes;
using Tecaverso.Labs.PlanarGeometry;

public class PlanarGeometryTests
{
    static readonly string[] Names = { "Triângulo equilátero", "Quadrado", "Pentágono regular", "Hexágono regular",
        "Heptágono regular", "Octógono regular", "Triângulo escaleno", "Retângulo", "Losango", "Paralelogramo", "Trapézio", "Polígono em L", "Círculo" };

    [TestCaseSource(nameof(Names))]
    public void GeometryMatchesFormulaAndClassification(string name)
    {
        var d = PlanarCatalog.Create().Single(x => x.shape.name == name); var vertices = d.shape.vertices;
        Assert.That(d.shape.Area, Is.EqualTo(d.formulaArea).Within(.0001f));
        var triangles = ShapeGeometry.Triangulate(vertices); float area = 0;
        for (int i=0;i<triangles.Length;i+=3)
            area += ShapeGeometry.FaceArea(new[] { vertices[triangles[i]], vertices[triangles[i+1]], vertices[triangles[i+2]] });
        Assert.That(area, Is.EqualTo(d.formulaArea).Within(d.IsCircle ? .003f : .0001f));
        Assert.That(vertices.All(v => v.z == 0));
        if (!d.IsCircle)
        {
            Assert.IsTrue(ShapeGeometry.ValidPolygon(vertices));
            Assert.AreEqual(d.regular, ShapeGeometry.Regular(vertices));
            Assert.AreEqual(name != "Polígono em L", ShapeGeometry.Convex(vertices));
            Assert.AreEqual(vertices.Length, d.VertexCount);
        }
        else
        {
            Assert.AreEqual(0, d.VertexCount);
            Assert.That(d.shape.Perimeter, Is.EqualTo(2*Mathf.PI*.95f).Within(.0001f));
        }
    }

    [Test]
    public void HeightsAndApothemsArePerpendicularAndOnBoundary()
    {
        foreach (var d in PlanarCatalog.Create()) foreach (var m in d.dimensions.Where(m=>m.rightAngle))
        {
            Assert.That(Mathf.Abs(Vector3.Dot((m.end-m.start).normalized,m.perpendicular.normalized)), Is.LessThan(.0001f), d.shape.name);
            var v=d.shape.vertices;
            Assert.IsTrue(Enumerable.Range(0,v.Length).Any(i =>
                Mathf.Abs(Vector3.Distance(v[i],m.start)+Vector3.Distance(m.start,v[(i+1)%v.Length])-Vector3.Distance(v[i],v[(i+1)%v.Length]))<.0001f), d.shape.name);
        }
    }

    [Test]
    public void ConcaveCutoutIsEmptyAndMatchesSubtractedRectangle()
    {
        var d=PlanarCatalog.Create().Single(x=>x.shape.name=="Polígono em L");
        Assert.That(ShapeGeometry.FaceArea(d.auxiliaryLoops[0])-ShapeGeometry.FaceArea(d.auxiliaryLoops[1]), Is.EqualTo(d.shape.Area).Within(.0001f));
        var v=d.shape.vertices;var triangles=ShapeGeometry.Triangulate(v);
        for(float x=.05f;x<.9f;x+=.17f)for(float y=-.05f;y<.9f;y+=.17f)
        {
            var p=new Vector2(x,y);
            for(int i=0;i<triangles.Length;i+=3)
            {
                Vector2 a=v[triangles[i]],b=v[triangles[i+1]],c=v[triangles[i+2]];
                bool inside=ShapeGeometry.Cross(b-a,p-a)>=0&&ShapeGeometry.Cross(c-b,p-b)>=0&&ShapeGeometry.Cross(a-c,p-c)>=0;
                Assert.IsFalse(inside,"Render triangles must not fill the missing rectangle.");
            }
        }
    }
}
