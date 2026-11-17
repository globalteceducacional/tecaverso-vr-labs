using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Tecaverso.Interaction;

// Focused editor regression: existing scenes, no saves/builds; generated evidence stays ignored.
public static class InteractionFeedbackVerification
{
    [Serializable]sealed class Row {public string scene,target,capture,error;public int xr,ui,surfaces;}
    [Serializable]sealed class Report {public List<Row> scenes=new();}
    static Report report;
    static string[] scenes;
    static int index,phase,frame;
    static double due;
    static AsyncOperation loading;
    static Camera reviewCamera;
    static bool reduced;
    static Row row;
    public static string Status {get;private set;}="Idle";
    const string Output="GeneratedMedia/InteractionFeedback";
    public static void Start()
    {
        if(!Application.isPlaying)throw new InvalidOperationException("Entre em Play Mode.");
        scenes=EditorBuildSettings.scenes.Where(s=>s.enabled).Select(s=>s.path).ToArray();
        Directory.CreateDirectory(Output);report=new Report();index=-1;reduced=MotionPreferences.Reduced;
        EditorApplication.update-=Tick;EditorApplication.update+=Tick;Next();
    }
    static void Next()
    {
        if(reviewCamera)UnityEngine.Object.Destroy(reviewCamera.gameObject);
        index++;
        if(index==scenes.Length){Finish();return;}
        Status="Checking "+Path.GetFileNameWithoutExtension(scenes[index]);phase=0;
        loading=SceneManager.LoadSceneAsync(scenes[index],LoadSceneMode.Single);due=EditorApplication.timeSinceStartup+1.2;frame=Time.frameCount;
    }
    static void Tick()
    {
        if(!Application.isPlaying){EditorApplication.update-=Tick;return;}
        if(!loading.isDone || EditorApplication.timeSinceStartup<due || Time.frameCount<=frame+2)return;
        try
        {
            if(phase==0)
            {
                var cart=UnityEngine.Object.FindFirstObjectByType<Tecaverso.Labs.Cartesian.CartesianLab>();
                if(cart && cart.Model.Points.Count==0){cart.create.onClick.Invoke();phase=1;frame=Time.frameCount;due=EditorApplication.timeSinceStartup+.25;return;}
                phase=1;
            }
            if(phase==1)
            {
                row=CheckScene();report.scenes.Add(row);phase=2;
                frame=Time.frameCount;due=EditorApplication.timeSinceStartup+.3;
                return;
            }
            if(phase==2 && reviewCamera)
            {
                ScreenCapture.CaptureScreenshot(row.capture);phase=3;frame=Time.frameCount;due=EditorApplication.timeSinceStartup+.35;return;
            }
            if(phase==3 && !File.Exists(row.capture))return;
            Next();
        }
        catch(Exception e)
        {
            report.scenes.Add(new Row{scene=scenes[index],error=e.Message});Next();
        }
    }
    static void Assert(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    static Row CheckScene()
    {
        var scene=SceneManager.GetActiveScene();var roots=scene.GetRootGameObjects();
        var controls=roots.SelectMany(r=>r.GetComponentsInChildren<Selectable>(true)).ToArray();
        var interactables=roots.SelectMany(r=>r.GetComponentsInChildren<XRBaseInteractable>(true)).Where(x=>!x.GetType().Name.Contains("Teleportation")).ToArray();
        foreach(var x in interactables)Assert(x.GetComponents<XRInteractionMotion>().Length==1,"Cobertura XR ausente/duplicada: "+x.name);
        foreach(var c in controls)Assert(c.GetComponent<ControlMotion>() || c.GetComponent<Tecaverso.Labs.ObliqueLaunch.ButtonTweenFeedback>() || c.GetComponent<Tecaverso.Labs.ObliqueLaunch.SliderTweenFeedback>(),"Controle sem feedback: "+c.name);
        Assert(!roots.SelectMany(r=>r.GetComponentsInChildren<Transform>(true)).Any(t=>t.name=="XR motion outline"),"Caixa de bounds antiga presente.");
        foreach(var cell in UnityEngine.Object.FindObjectsByType<Tecaverso.Labs.Biology.Cells.CellStructure>(FindObjectsSortMode.None))Assert(!cell.selectionOutline || !cell.selectionOutline.enabled,"Caixa de seleção de Células ativa.");
        var result=new Row{scene=scene.path,xr=interactables.Length,ui=controls.Length};
        var candidates=interactables.Where(x=>x.isActiveAndEnabled).Select(x=>x.GetComponent<XRInteractionMotion>()).ToArray();
        foreach(var c in candidates)c.RefreshVisuals();
        var target=candidates.FirstOrDefault(x=>x.name=="animal.mitocondria") ?? candidates.FirstOrDefault(x=>x.GetComponent<Tecaverso.Labs.Chemistry.Reactions.ReagentTool>()) ?? candidates.FirstOrDefault(x=>x.GetComponent<XRKnob>()) ?? candidates.FirstOrDefault(x=>x.Surfaces.Any(r=>r && r.enabled && r.gameObject.activeInHierarchy));
        if(!target)return result;
        CheckStates(target);result.target=target.name;result.surfaces=target.Surfaces.Count;
        Assert(target.Surfaces.All(r=>!(r is LineRenderer)&&!r.GetComponent<TMP_Text>()),"Texto/linha científica incluída na máscara.");
        var liquid=target.GetComponent<Tecaverso.Labs.Chemistry.Reactions.ReagentTool>();
        if(liquid && liquid.liquid)Assert(!target.Surfaces.Contains(liquid.liquid.surface),"Líquido incluído no realce do frasco.");
        if(target.GetComponent<XRKnob>() is XRKnob knob)Assert(target.Surfaces.All(r=>r.transform.IsChildOf(knob.handle)),"Dial realça base em vez da peça móvel.");
        var visible=target.Surfaces.Where(r=>r && r.enabled && r.gameObject.activeInHierarchy).ToArray();
        if(visible.Length>0)
        {
            target.SetPersistentSelected(true);
            bool representative=scene.name=="Cells"||scene.name=="Reactions"||scene.name=="Thermology"||scene.name=="CartesianSpace";
            if(representative)
            {
                var bounds=visible[0].bounds;foreach(var r in visible.Skip(1))bounds.Encapsulate(r.bounds);
                var source=Camera.main;reviewCamera=new GameObject("Feedback review camera").AddComponent<Camera>();reviewCamera.CopyFrom(source);
                reviewCamera.depth=source.depth+10;reviewCamera.targetTexture=null;reviewCamera.fieldOfView=42;reviewCamera.nearClipPlane=.01f;
                var additional=reviewCamera.GetUniversalAdditionalCameraData();additional.renderPostProcessing=true;
                float distance=Mathf.Max(.23f,bounds.size.magnitude*1.65f);
                reviewCamera.transform.position=bounds.center+new Vector3(.12f,.12f,-1).normalized*distance;
                reviewCamera.transform.rotation=Quaternion.LookRotation(bounds.center-reviewCamera.transform.position);
                result.capture=Output+"/"+scene.name+"-silhouette.png";
            }
        }
        return result;
    }
    static void CheckStates(XRInteractionMotion effect)
    {
        var xr=effect.GetComponent<XRBaseInteractable>();var position=xr.transform.localPosition;var scale=xr.transform.localScale;var rotation=xr.transform.localRotation;
        var renderers=effect.Surfaces.ToArray();var materials=renderers.Select(r=>r.sharedMaterials).ToArray();
        bool old=MotionPreferences.Reduced;var temporary=new GameObject("Temporary feedback pointers");
        try
        {
            var a=temporary.AddComponent<XRRayInteractor>();var b=new GameObject("Second pointer").AddComponent<XRRayInteractor>();b.transform.SetParent(temporary.transform);
            var socket=new GameObject("Socket").AddComponent<XRSocketInteractor>();socket.transform.SetParent(temporary.transform);
            MotionPreferences.SetReduced(true);effect.ResetFeedback();
            xr.hoverEntered.Invoke(new HoverEnterEventArgs{interactorObject=socket,interactableObject=xr});effect.SendMessage("LateUpdate");Assert(effect.Amount==0,"Socket acende foco do usuário.");
            xr.hoverEntered.Invoke(new HoverEnterEventArgs{interactorObject=a,interactableObject=xr});effect.SendMessage("LateUpdate");Assert(effect.Amount==1,"Hover não aparece no modo reduzido.");
            xr.hoverEntered.Invoke(new HoverEnterEventArgs{interactorObject=b,interactableObject=xr});
            xr.hoverExited.Invoke(new HoverExitEventArgs{interactorObject=a,interactableObject=xr});effect.SendMessage("LateUpdate");Assert(effect.Amount==1,"Saída de um ponteiro apagou o outro.");
            xr.hoverExited.Invoke(new HoverExitEventArgs{interactorObject=b,interactableObject=xr});effect.SendMessage("LateUpdate");Assert(effect.Amount==0,"Foco permanece após saída.");
            effect.SetPersistentSelected(true);effect.SendMessage("LateUpdate");Assert(effect.Amount==1&&effect.PersistentSelected,"Seleção não persiste.");
            effect.enabled=false;Assert(effect.Amount==0&&!XRInteractionMotion.Instances.Contains(effect),"Desabilitar deixou efeito ativo.");effect.enabled=true;
            Assert(xr.transform.localPosition==position&&xr.transform.localScale==scale&&xr.transform.localRotation==rotation,"Feedback alterou transform.");
            for(int i=0;i<renderers.Length;i++)Assert(materials[i].SequenceEqual(renderers[i].sharedMaterials),"Feedback substituiu materiais científicos.");
        }
        finally{MotionPreferences.SetReduced(old);UnityEngine.Object.DestroyImmediate(temporary);effect.ResetFeedback();}
    }
    static void Finish()
    {
        EditorApplication.update-=Tick;MotionPreferences.SetReduced(reduced);
        File.WriteAllText(Output+"/verification.json",JsonUtility.ToJson(report,true));
        Status="Finished: "+report.scenes.Count+" scenes; errors="+report.scenes.Count(r=>!string.IsNullOrEmpty(r.error));
        SceneManager.LoadSceneAsync(Tecaverso.Editor.CellsAuthoring.ScenePath,LoadSceneMode.Single);
    }
}
