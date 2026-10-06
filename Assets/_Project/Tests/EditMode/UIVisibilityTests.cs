using System;
using NUnit.Framework;
using UnityEngine;

public class UIVisibilityTests
{
    GameObject root;
    static Type Visibility=>Type.GetType("Tecaverso.UI.UIVisibility, Assembly-CSharp",true);
    static void Show(GameObject view,bool visible,bool collapse=false)=>Visibility.GetMethod("Set").Invoke(null,new object[]{view,visible,collapse});
    [TearDown] public void Cleanup() { if(root!=null)UnityEngine.Object.DestroyImmediate(root); }
    [Test] public void HiddenPageStaysActiveButCannotInterceptInput()
    {
        root=new GameObject("Visibility test",typeof(RectTransform));Show(root,false);
        var group=root.GetComponent<CanvasGroup>();
        Assert.IsTrue(root.activeSelf);Assert.AreEqual(0,group.alpha);Assert.IsFalse(group.interactable);Assert.IsFalse(group.blocksRaycasts);
        Visibility.GetMethod("SetInteraction").Invoke(null,new object[]{root,true});
        Assert.IsFalse(group.interactable);Assert.IsFalse(group.blocksRaycasts,"Closing a modal must not reactivate an invisible page.");
        Show(root,true);Assert.AreEqual(1,group.alpha);Assert.IsTrue(group.interactable);Assert.IsTrue(group.blocksRaycasts);
    }
    [Test] public void OptionalItemReleasesLayoutSpaceAndHonorsHiddenParent()
    {
        root=new GameObject("Parent",typeof(RectTransform));var child=new GameObject("Item",typeof(RectTransform));child.transform.SetParent(root.transform,false);
        Show(child,false,true);
        var item=child.GetComponent("LayoutElement");var ignored=item.GetType().GetProperty("ignoreLayout");
        Assert.IsTrue((bool)ignored.GetValue(item));Show(child,true,true);Assert.IsFalse((bool)ignored.GetValue(item));
        Show(root,false);Assert.IsFalse((bool)Visibility.GetMethod("IsVisible").Invoke(null,new object[]{child}));
    }
}
