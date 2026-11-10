using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Tecaverso.Interaction;

// Editor-only smoke test: opens existing labs in Play Mode without saving or rebuilding them.
public static class InteractionMotionVerification
{
    static string[] scenes; static int index; static double due;
    static readonly List<string> results = new();
    public static void Start()
    {
        if(!EditorApplication.isPlaying)throw new InvalidOperationException("Enter Play Mode first.");
        scenes=EditorBuildSettings.scenes.Where(s=>s.enabled&&s.path.Contains("/Laboratories/")).Select(s=>s.path).ToArray();
        results.Clear();index=-1;EditorApplication.update-=Tick;EditorApplication.update+=Tick;Next();
    }
    static void Next()
    {
        index++;
        if(index>=scenes.Length){Finish();return;}
        SceneManager.LoadScene(scenes[index]);due=EditorApplication.timeSinceStartup+2;
    }
    static void Tick()
    {
        if(!EditorApplication.isPlaying){EditorApplication.update-=Tick;return;}
        if(EditorApplication.timeSinceStartup<due)return;
        try {
            var roots=SceneManager.GetActiveScene().GetRootGameObjects();
            var ui=roots.SelectMany(r=>r.GetComponentsInChildren<Selectable>(true)).ToArray();
            var xr=roots.SelectMany(r=>r.GetComponentsInChildren<XRBaseInteractable>(true)).ToArray();
            var uncoveredUI=ui.Where(c=>!c.GetComponent<ControlMotion>()&&!c.GetComponent<Tecaverso.Labs.ObliqueLaunch.ButtonTweenFeedback>()&&!c.GetComponent<Tecaverso.Labs.ObliqueLaunch.SliderTweenFeedback>()).Select(c=>c.name).ToArray();
            var uncoveredXR=xr.Where(c=>!c.GetComponent<XRInteractionMotion>()&&!c.GetComponent<Tecaverso.Labs.Chemistry.PeriodicBonds.PeriodicElementCube>()&&!c.GetType().Name.Contains("Teleportation")).Select(c=>c.name).ToArray();
            results.Add(SceneManager.GetActiveScene().name+": UI="+ui.Length+" XR="+xr.Length+" missingUI=["+string.Join(",",uncoveredUI)+"] missingXR=["+string.Join(",",uncoveredXR)+"]");
        }catch(Exception e){results.Add("ERROR "+e);}
        Next();
    }
    static void Finish()
    {
        EditorApplication.update-=Tick;
        System.IO.Directory.CreateDirectory("Temp");
        System.IO.File.WriteAllLines("Temp/interaction-motion-verification.txt",results);
        SceneManager.LoadScene("Assets/_Project/Laboratories/Chemistry/PeriodicBonds/Scenes/PeriodicBonds.unity");
        Debug.Log("Interaction motion verification complete: Temp/interaction-motion-verification.txt");
    }
}
