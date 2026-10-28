using NUnit.Framework;
using UnityEngine;
using Tecaverso.Labs.Cartesian;

public class CartesianModelTests
{
    [Test] public void MagneticDragAttractsOnlyNearbyIntegersAndCanBeDisabled()
    {
        var raw=new Vector3(1.09f,-2.08f,.45f);
        Assert.That(CartesianModel.Magnetize(raw,true),Is.EqualTo(new Vector3(1,-2,.45f)));
        Assert.That(CartesianModel.Magnetize(raw,false),Is.EqualTo(raw));
        Assert.That(CartesianModel.Magnetize(new Vector3(.13f,-.13f,2.5f),true),Is.EqualTo(new Vector3(.13f,-.13f,2.5f)));
        var m=new CartesianModel();var p=m.AddPoint();
        m.Move(p.id,raw);Assert.That(p.position,Is.EqualTo(raw),"Numeric edits retain exact fractions.");
        m.ToggleLock(p.id,0);m.Move(p.id,CartesianModel.Magnetize(new Vector3(2.04f,-3.06f,.5f),true));
        Assert.That(p.position,Is.EqualTo(new Vector3(1.09f,-3,.5f)),"Magnetism respects axis locks.");
    }
    static int Point(CartesianModel m,Vector3 v){var p=m.AddPoint();m.Move(p.id,v);return p.id;}
    [Test] public void GestureIsOneUndoAndNewEditClearsRedo()
    {var m=new CartesianModel();var p=m.AddPoint();m.BeginGesture();for(int i=1;i<50;i++)m.Move(p.id,new Vector3(i*.1f,1,0));m.EndGesture();m.Undo();Assert.That(m.Point(0).position,Is.EqualTo(Vector3.zero));m.Redo();Assert.That(m.Point(0).position.x,Is.EqualTo(4.9f).Within(.0001));m.Undo();m.Move(0,Vector3.one);Assert.That(m.CanRedo,Is.False);}
    [Test] public void NoOpGestureDoesNotConsumeHistory()
    {var m=new CartesianModel();m.AddPoint();m.BeginGesture();m.Move(0,Vector3.zero);m.EndGesture();m.Undo();Assert.That(m.Points,Is.Empty);}
    [Test] public void DegenerateGeometryIsRejectedAndCanBecomeValidAgain()
    {var m=new CartesianModel();int a=Point(m,Vector3.zero),b=Point(m,Vector3.right),c=Point(m,Vector3.right*2),d=Point(m,Vector3.forward);Assert.That(m.Add(ConstructionKind.Plane,new[]{a,b,c},out _),Is.Null);m.Move(c,Vector3.up);var p=m.Add(ConstructionKind.Plane,new[]{a,b,c},out _);Assert.That(p,Is.Not.Null);m.Move(c,Vector3.right*2);Assert.That(m.Validate(p.kind,p.points,out _),Is.False);m.Move(c,Vector3.up);Assert.That(m.Validate(p.kind,p.points,out _),Is.True);Assert.That(m.Add(ConstructionKind.Tetrahedron,new[]{a,b,c,d},out _),Is.Not.Null);m.Move(d,Vector3.one-Vector3.forward);Assert.That(m.Validate(ConstructionKind.Tetrahedron,new[]{a,b,c,d},out _),Is.False);}
    [Test] public void SharedPointDeletionAndUndoRestoreTopology()
    {var m=new CartesianModel();int a=Point(m,Vector3.zero),b=Point(m,Vector3.right),c=Point(m,Vector3.up);m.Add(ConstructionKind.Segment,new[]{a,b},out _);m.Add(ConstructionKind.Triangle,new[]{a,b,c},out _);m.DeletePoint(b);Assert.That(m.Constructions,Is.Empty);m.Undo();Assert.That(m.Constructions.Count,Is.EqualTo(2));Assert.That(m.Point(b).position,Is.EqualTo(Vector3.right));}
    [Test] public void GroupMotionClampsCommonDeltaAndPreservesDistances()
    {var m=new CartesianModel();int a=Point(m,new Vector3(4,0,0)),b=Point(m,new Vector3(2,1,0));float length=Vector3.Distance(m.Point(a).position,m.Point(b).position);var delta=m.Translate(new[]{a,b},new Vector3(3,0,0));Assert.That(delta.x,Is.EqualTo(1));Assert.That(Vector3.Distance(m.Point(a).position,m.Point(b).position),Is.EqualTo(length).Within(.0001));m.ToggleLock(a,1);m.Translate(new[]{a,b},Vector3.up);Assert.That(m.Point(b).position.y,Is.EqualTo(1));}
    [Test] public void PlaneRestrictionAffectsOnlyItsPointAndGroupMotion()
    {var m=new CartesianModel();var a=m.AddPoint(2);m.Move(a.id,new Vector3(2,3,4));var b=m.AddPoint();m.Move(b.id,new Vector3(1,2,3));Assert.That(m.Point(a.id).position.z,Is.Zero);Assert.That(m.Point(b.id).position.z,Is.EqualTo(3));m.Translate(new[]{a.id,b.id},Vector3.forward);Assert.That(m.Point(b.id).position.z,Is.EqualTo(3));}
    [Test] public void CoordinatesRejectNonFiniteAndSnapWithinBounds()
    {var m=new CartesianModel();m.AddPoint();Assert.That(m.Move(0,new Vector3(float.NaN,0,0)),Is.False);Assert.That(m.Move(0,new Vector3(float.PositiveInfinity,0,0)),Is.False);m.Move(0,new Vector3(100,-100,.74f),.5f);Assert.That(m.Point(0).position,Is.EqualTo(new Vector3(5,-5,.5f)));}
    [Test] public void ReferenceAreasAndVolumesAreCorrect()
    {Assert.That(CartesianModel.TriangleArea(Vector3.zero,Vector3.right*3,Vector3.up*4),Is.EqualTo(6).Within(.0001));Assert.That(CartesianModel.TetraVolume(Vector3.zero,Vector3.right*3,Vector3.up*4,Vector3.forward*2),Is.EqualTo(4).Within(.0001));}
    [Test] public void InfiniteLineAndRayClipDifferently()
    {Assert.That(CartesianModel.ClipLine(Vector3.one,Vector3.right,false,out var a,out var b),Is.True);Assert.That(a,Is.EqualTo(new Vector3(-5,1,1)));Assert.That(b,Is.EqualTo(new Vector3(5,1,1)));Assert.That(CartesianModel.ClipLine(Vector3.one,Vector3.right,true,out a,out b),Is.True);Assert.That(a,Is.EqualTo(Vector3.one));Assert.That(CartesianModel.ClipLine(Vector3.zero,Vector3.zero,false,out a,out b),Is.False);}
    [Test] public void CapacityIsBoundedAndClearCanBeUndone()
    {var m=new CartesianModel();for(int i=0;i<CartesianModel.MaxPoints;i++)Assert.That(m.AddPoint(),Is.Not.Null);Assert.That(m.AddPoint(),Is.Null);m.Clear();Assert.That(m.Points,Is.Empty);m.Undo();Assert.That(m.Points.Count,Is.EqualTo(CartesianModel.MaxPoints));}
}
