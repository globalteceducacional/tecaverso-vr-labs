using System;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class SpectatorWaitingTests
{
    GameObject root;
    Component view;
    Type type, stateType;
    [SetUp] public void Setup()
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/UI/Spectator/SpectatorWaiting.prefab");
        Assert.NotNull(prefab);
        root = UnityEngine.Object.Instantiate(prefab);
        type = Type.GetType("Tecaverso.UI.Spectator.SpectatorWaitingView, Assembly-CSharp", true);
        stateType = Type.GetType("Tecaverso.UI.Spectator.SpectatorWaitingState, Assembly-CSharp", true);
        view = root.GetComponent(type);
    }
    [TearDown] public void Cleanup() { if (root != null) UnityEngine.Object.DestroyImmediate(root); }
    void State(string value) => type.GetMethod("SetState").Invoke(view, new[] { Enum.Parse(stateType, value) });
    CanvasGroup Page => root.transform.Find("Waiting page").GetComponent<CanvasGroup>();
    CanvasGroup Action(string name) => root.transform.Find("Waiting page/Session notice/Actions/" + name).GetComponent<CanvasGroup>();

    [Test] public void UnsupportedActionsStayHiddenAndLiveReleasesInput()
    {
        State("Unconnected");
        Assert.AreEqual(0, Action("Conectar").alpha);
        Assert.AreEqual(0, Action("Assistir").alpha);
        Assert.AreEqual(0, Action("Explorar sozinho").alpha);
        State("Live");
        Assert.AreEqual(0, Page.alpha); Assert.IsFalse(Page.blocksRaycasts);
        Assert.IsTrue(Page.gameObject.activeSelf);
        State("ConnectionLost");
        Assert.AreEqual(1, Page.alpha); Assert.IsTrue(Page.blocksRaycasts);
    }

    [Test] public void WatchRequiresBothCapabilityAndAnAvailableSession()
    {
        type.GetMethod("SetCapabilities").Invoke(view, new object[] { true, true, true });
        State("WaitingForHost"); Assert.AreEqual(0, Action("Assistir").alpha);
        State("SessionAvailable"); Assert.AreEqual(1, Action("Assistir").alpha);
        type.GetMethod("SetCapabilities").Invoke(view, new object[] { true, false, true });
        Assert.AreEqual(0, Action("Assistir").alpha);
    }
}
