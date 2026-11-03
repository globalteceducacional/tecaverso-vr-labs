using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Tecaverso.Labs.Shapes;
public class ShapeGeometryTests
{
 [TestCase("Cubo",8,12,6)] [TestCase("Tetraedro",4,6,4)] [TestCase("Octaedro",6,12,8)] [TestCase("Dodecaedro",20,30,12)] [TestCase("Icosaedro",12,30,20)]
 public void Topology(string name,int v,int e,int f){var d=ShapeGeometry.Catalog().First(x=>x.name==name);Assert.AreEqual(v,d.vertices.Length);Assert.AreEqual(e,d.Edges.Length);Assert.AreEqual(f,d.faces.Length);Assert.AreEqual(2,v-e+f);}
 [TestCase("Cubo")] [TestCase("Prisma triangular")] [TestCase("Pirâmide de base quadrada")]
 public void UnfoldPreservesFaces(string name){var d=ShapeGeometry.Catalog().First(x=>x.name==name);foreach(float t in new[]{0,.3f,.7f,1}){var faces=ShapeGeometry.Unfold(d,t);Assert.That(faces.Sum(ShapeGeometry.FaceArea),Is.EqualTo(d.Area).Within(.0001));for(int i=0;i<faces.Length;i++)for(int j=0;j<faces[i].Length;j++){int next=(j+1)%faces[i].Length;Assert.That(Vector3.Distance(faces[i][j],faces[i][next]),Is.EqualTo(Vector3.Distance(d.vertices[d.faces[i][j]],d.vertices[d.faces[i][next]])).Within(.0001));}if(t==1){var all=faces.SelectMany(x=>x).ToArray();Assert.That(all.Max(x=>x.y)-all.Min(x=>x.y),Is.LessThan(.0001));}}}
 [Test] public void RegularityAndConcavity(){var list=ShapeGeometry.Catalog();Assert.IsTrue(ShapeGeometry.Regular(ShapeGeometry.Polygon(6).vertices));Assert.IsFalse(ShapeGeometry.Regular(list.First(x=>x.name=="Retângulo").vertices));var d=list.First(x=>x.name=="Polígono côncavo");Assert.IsFalse(ShapeGeometry.Convex(d.vertices));Assert.IsTrue(ShapeGeometry.ValidPolygon(d.vertices));var triangles=ShapeGeometry.Triangulate(d.vertices);float area=0;for(int i=0;i<triangles.Length;i+=3)area+=ShapeGeometry.FaceArea(new[]{d.vertices[triangles[i]],d.vertices[triangles[i+1]],d.vertices[triangles[i+2]]});Assert.That(area,Is.EqualTo(d.Area).Within(.0001));}
 [Test] public void RejectCrossing(){Assert.IsFalse(ShapeGeometry.ValidPolygon(new[]{new Vector3(-1,-1,0),new Vector3(1,1,0),new Vector3(-1,1,0),new Vector3(1,-1,0)}));}
 [Test] public void AnalyticMeasures(){var cube=ShapeGeometry.Catalog().First(d=>d.name=="Cubo");Assert.That(cube.Volume,Is.EqualTo(1.3f*1.3f*1.3f).Within(.0001));Assert.That(ShapeGeometry.Curved("sphere").Volume,Is.EqualTo(4*Mathf.PI/3).Within(.0001));}
}
