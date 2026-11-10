using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Tecaverso.Labs.Chemistry.Reactions;
using Object=UnityEngine.Object;

namespace Tecaverso.Editor
{
    /// <summary>Focused Editor smoke test using real XRI selection and pouring. No scene/build changes.</summary>
    public static class ReactionsValidation
    {
        static ReactionsLab lab;
        static XRDirectInteractor hand;
        static IEnumerator<double> steps;
        static double deadline;
        static readonly List<string> report=new List<string>();
        public static string Status {get;private set;}="Not run";
        public static string Report=>string.Join("\n",report);
        public static void Start()
        {
            if(!EditorApplication.isPlaying)throw new Exception("Run in Play Mode.");
            if(steps!=null)throw new Exception("Validation already running.");
            lab=Object.FindFirstObjectByType<ReactionsLab>();if(!lab)throw new Exception("Open Reactions first.");
            if(lab.bench)throw new Exception("Use ReactionsBenchValidation.Start() para a sala A/B de 12 reagentes.");
            var go=new GameObject("Temporary validation hand");
            var col=go.AddComponent<SphereCollider>();col.isTrigger=true;col.radius=.035f;
            var rb=go.AddComponent<Rigidbody>();rb.isKinematic=true;rb.useGravity=false;
            hand=go.AddComponent<XRDirectInteractor>();hand.interactionManager=Object.FindFirstObjectByType<XRInteractionManager>();
            report.Clear();Status="Running";steps=Run();deadline=0;EditorApplication.update+=Tick;
        }
        static void Tick()
        {
            if(!EditorApplication.isPlaying){Finish("Interrupted");return;}
            if(EditorApplication.timeSinceStartup<deadline)return;
            try{if(!steps.MoveNext()){Finish("Passed");return;}deadline=EditorApplication.timeSinceStartup+steps.Current;}
            catch(Exception e){report.Add(e.ToString());Finish("Failed");Debug.LogError("Reaction validation: "+e.Message);}
        }
        static void Finish(string result)
        {EditorApplication.update-=Tick;steps?.Dispose();steps=null;if(hand){if(hand.hasSelection)hand.EndManualInteraction();Object.Destroy(hand.gameObject);}Status=result;}
        static void Check(bool pass,string message){if(!pass)throw new Exception(message);report.Add("PASS: "+message);}
        static void Switch(int kind){lab.RequestCase(kind);if(lab.Modal)lab.ConfirmReset();}
        static void Grab(int index)
        {
            var t=lab.tools[index];hand.transform.SetPositionAndRotation(t.transform.position,t.transform.rotation);
            hand.StartManualInteraction((IXRSelectInteractable)t);
        }
        static void Pose(int index,Vector3 mouth,Quaternion rotation)
        {hand.transform.SetPositionAndRotation(mouth-rotation*lab.tools[index].mouth.localPosition,rotation);}
        static void Drop(int index){hand.EndManualInteraction();lab.tools[index].ReturnHome();}
        static IEnumerator<double> Run()
        {
            Switch(0);yield return .3;
            Grab(0);yield return .2;Check(lab.tools[0].Held,"XR grab selects the reagent");
            Pose(0,lab.opening.position+Vector3.up*.3f,Quaternion.Euler(0,0,145));yield return 1;
            Check(lab.Session.VolumeMl>5&&lab.Session.SpilledMl<.01,"Tilt over aperture transfers actual stock into receiver");
            lab.TogglePause();double before=lab.Session.VolumeMl;yield return .4;
            Check(Math.Abs(lab.Session.VolumeMl-before)<1e-6,"Pause blocks transfer while still held and tilted");
            lab.TogglePause();Pose(0,lab.opening.position+new Vector3(.65f,.3f,0),Quaternion.Euler(0,0,145));yield return .5;
            Check(lab.Session.SpilledMl>1,"Pour outside aperture records spill");Drop(0);yield return .2;
            before=lab.Session.VolumeMl;lab.RequestCase(1);Check(lab.Modal,"Used mixture asks before changing case");lab.CancelReset();
            Check(!lab.Modal&&lab.Session.VolumeMl==before,"Cancel preserves experiment and unlocks interaction");
            Grab(2);yield return .2;Pose(2,lab.powderDish.position,Quaternion.identity);yield return .5;
            Check(lab.Session.SpoonGrams>.9,"Physical spoon collects finite bicarbonate stock");
            Pose(2,lab.opening.position+Vector3.up*.22f,Quaternion.Euler(0,0,135));yield return 1.5;
            Check(lab.Session.ExtentMol>0&&lab.Session.ProductsMol[2]>0,"Spoon dose produces CO2 only after adding acid");Drop(2);yield return .2;
            foreach(var e in lab.Session.Definition.Elements)Check(Math.Abs(lab.Session.AtomConservationError(e))<1e-10,"Gas atom conservation: "+e);
            lab.ShowMolecules(false);Check(!lab.atomGroups[0].gameObject.activeInHierarchy,"Molecular toggle hides 3D atoms as well as text");lab.ShowMolecules(true);
            Switch(1);yield return .3;
            Check(!lab.powderStation.activeSelf&&!lab.tools[2].gameObject.activeSelf&&lab.tools[1].gameObject.activeSelf,"Precipitation exposes only relevant tools");
            Grab(0);yield return .2;Pose(0,lab.opening.position+Vector3.up*.3f,Quaternion.Euler(0,0,145));yield return .8;Drop(0);yield return .2;
            Check(!lab.sediment.gameObject.activeSelf,"One reagent alone cannot create precipitate");
            Grab(1);yield return .2;Pose(1,lab.opening.position+Vector3.up*.3f,Quaternion.Euler(0,0,-145));yield return 1;Drop(1);yield return .3;
            Check(lab.Session.ExtentMol>0&&lab.sediment.gameObject.activeSelf,"XR mixing creates CaCO3 deposit");
            Check(lab.balance.text.Contains("AJUSTE"),"Unbalanced precipitation equation is flagged");lab.ChangeCoefficient(3,1);Check(lab.balance.text.Contains("BALANCEADA"),"Coefficient 2 NaCl balances the equation");
            Switch(2);yield return .3;
            Grab(1);yield return .2;Pose(1,lab.opening.position+Vector3.up*.3f,Quaternion.Euler(0,0,-145));yield return .6;Drop(1);yield return .3;
            Check(lab.Session.PH>10&&lab.receiver.tint.r>.8f,"Base produces pink indicator");
            Grab(0);yield return .2;Pose(0,lab.opening.position+Vector3.up*.3f,Quaternion.Euler(0,0,145));yield return 1.3;Drop(0);yield return .3;
            Check(lab.Session.PH<7&&lab.receiver.tint.r<.7f,"Acid excess returns indicator to reference clear tint");
            Check(!lab.sediment.gameObject.activeSelf&&lab.bubbles.particleCount==0,"Neutralization has no gas or precipitate effects");
            lab.RequestCase(0);Check(lab.Modal,"Reset confirmation shown");lab.ConfirmReset();yield return .2;
            Check(!lab.Session.Used&&lab.Session.SourceMl[0]==300&&!lab.Modal,"Confirmed reset restores supplies and controls");
            Check(lab.tools[0].GetComponent<Tecaverso.Interaction.XRInteractionMotion>()!=null,"Shared XR motion is attached");
        }
    }
}
