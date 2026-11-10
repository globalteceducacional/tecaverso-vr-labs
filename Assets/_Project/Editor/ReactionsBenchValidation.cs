using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Tecaverso.Labs.Chemistry.Reactions;
using Object=UnityEngine.Object;

namespace Tecaverso.Editor
{
    /// <summary>Editor-only real XRI grab/release into socket triggers, pouring and negative paths.</summary>
    public static class ReactionsBenchValidation
    {
        static ReactionsLab lab;static ReactionsBench bench;static XRDirectInteractor hand;static ReagentTool held;
        static IEnumerator<double> steps;static double deadline;static readonly List<string> report=new List<string>();
        public static string Status {get;private set;}="Not run";
        public static string Report=>string.Join("\n",report);
        static ReagentTool Tool(string id,bool spoon=false)=>bench.instruments.Single(t=>t.reagentId==id&&t.spoon==spoon);
        public static void Start()
        {
            if(!EditorApplication.isPlaying||steps!=null)throw new Exception("Play Mode required; one validation at a time.");
            lab=Object.FindFirstObjectByType<ReactionsLab>();bench=lab.bench;if(!bench)throw new Exception("Migrate Reactions first.");
            bench.ResetAll();var go=new GameObject("Temporary A B validation hand");go.AddComponent<SphereCollider>().isTrigger=true;go.GetComponent<SphereCollider>().radius=.035f;
            var rb=go.AddComponent<Rigidbody>();rb.isKinematic=true;rb.useGravity=false;
            hand=go.AddComponent<XRDirectInteractor>();hand.interactionManager=Object.FindFirstObjectByType<XRInteractionManager>();
            report.Clear();Status="Running";steps=Run();deadline=0;EditorApplication.update+=Tick;
        }
        static void Tick()
        {
            if(!EditorApplication.isPlaying){Finish("Interrupted");return;}if(EditorApplication.timeSinceStartup<deadline)return;
            try{if(!steps.MoveNext()){Finish("Passed");return;}deadline=EditorApplication.timeSinceStartup+steps.Current;}
            catch(Exception e){report.Add(e.ToString());Finish("Failed");Debug.LogError("A/B validation: "+e.Message);}
        }
        static void Finish(string result)
        {
            EditorApplication.update-=Tick;steps?.Dispose();steps=null;
            if(hand){if(hand.hasSelection)hand.EndManualInteraction();Object.Destroy(hand.gameObject);}Status=result;
            System.IO.Directory.CreateDirectory("Temp/ReactionsBenchValidation");System.IO.File.WriteAllText("Temp/ReactionsBenchValidation/report.txt",Status+"\n"+Report);
        }
        static void Check(bool pass,string message){if(!pass)throw new Exception(message);report.Add("PASS: "+message);}
        static void Grab(string id,bool spoon=false)
        {
            held=Tool(id,spoon);hand.transform.SetPositionAndRotation(held.transform.position,held.transform.rotation);hand.StartManualInteraction((IXRSelectInteractable)held);
        }
        static void Pose(Vector3 mouth,Quaternion rotation)
        {hand.transform.SetPositionAndRotation(mouth-rotation*Vector3.Scale(held.mouth.localPosition,held.transform.lossyScale),rotation);}
        static void Drop(bool home=true){hand.EndManualInteraction();if(home)held.ReturnHome();held=null;}
        static IEnumerator<double> Dock(string id,int slot)
        {
            Grab(id);yield return .2;Check(held.Held,"Grab "+id);
            hand.transform.SetPositionAndRotation(bench.sockets[slot].attachTransform.position,Quaternion.identity);yield return .3;
            Drop(false);yield return .5;
            Check(bench.sockets[slot].interactablesSelected.Any(t=>t is ReagentTool r&&r.reagentId==id),"Physical release accepted correct identity in socket "+slot+": "+id);
        }
        static IEnumerator<double> Pair(string a,string b)
        {
            foreach(var x in Enumerate(Dock(a,0)))yield return x;
            foreach(var x in Enumerate(Dock(b,1)))yield return x;
            Check(bench.Model.Ready&&bench.Model.A==a&&bench.Model.B==b,"Pair ready with correct identities "+a+" / "+b);
        }
        static IEnumerable<double> Enumerate(IEnumerator<double> e){using(e){while(e.MoveNext())yield return e.Current;}}
        static IEnumerator<double> Pour(string id,float seconds)
        {
            Grab(id);yield return .2;Check(held.Held,"Can retrieve selected bottle "+id);
            Pose(lab.opening.position+Vector3.up*.3f,Quaternion.identity);yield return .3;
            Pose(lab.opening.position+Vector3.up*.3f,Quaternion.Euler(0,0,145));yield return seconds;
            Drop();yield return .2;
        }
        static IEnumerator<double> Powder(string id)
        {
            Grab(id,true);yield return .2;Pose(Tool(id).collectionPoint.position,Quaternion.identity);yield return .5;
            Check(bench.Model.Spoon(id)>.9,"Dedicated spoon collected "+id);
            Pose(lab.opening.position+Vector3.up*.3f,Quaternion.identity);yield return .3;
            Pose(lab.opening.position+Vector3.up*.3f,Quaternion.Euler(0,0,140));yield return 1.5;
            Drop();yield return .2;
        }
        static IEnumerator<double> Run()
        {
            yield return .5;
            Check(bench.instruments.Count(t=>!t.spoon&&!t.indicator)==12,"12 reagent containers present");
            Check(!bench.SheetOpen&&!bench.Modal&&!bench.Model.Ready,"Clean entry with no expanded UI or active recipe");
            Check(bench.thermometer&&bench.thermometer.GetComponent<Tecaverso.Interaction.XRInteractionMotion>(),"Physical thermometer with shared XR motion");
            foreach(var x in Enumerate(Pour("hcl",.5f)))yield return x;
            Check(bench.Model.Stock("hcl")==300&&!bench.Model.Used,"Pour before choosing pair blocked without stock loss");
            foreach(var x in Enumerate(Dock("hcl",0)))yield return x;
            Grab("hcl");yield return .3;Check(bench.Model.A==null,"Removing draft A clears selection");Drop();yield return .2;
            foreach(var x in Enumerate(Dock("hcl",0)))yield return x;
            Grab("acetic");yield return .2;hand.transform.position=bench.sockets[1].attachTransform.position;yield return .3;Drop(false);yield return .5;
            Check(!bench.Model.Ready&&!bench.sockets[1].hasSelection,"Acid/acid pair rejected by physical socket");
            Tool("acetic").ReturnHome();bench.ResetAll();yield return .3;
            foreach(var x in Enumerate(Pair("cacl","carb")))yield return x;
            foreach(var x in Enumerate(Pour("cacl",.7f)))yield return x;
            Check(lab.Session.VolumeMl>5&&lab.Session.ExtentMol==0,"First reagent transfers but does not precipitate");
            foreach(var x in Enumerate(Pour("carb",1f)))yield return x;
            Check(lab.Session.ExtentMol>0&&lab.sediment.gameObject.activeSelf,"Real XR pouring produces precipitate");
            double before=lab.Session.VolumeMl,extent=lab.Session.ExtentMol;
            foreach(var x in Enumerate(Pour("hcl",.7f)))yield return x;
            Check(lab.Session.VolumeMl==before&&bench.Model.Stock("hcl")==300,"Third liquid blocked before emission and stock consumption");
            foreach(var x in Enumerate(Pour("cacl",.6f)))yield return x;
            Check(lab.Session.VolumeMl>before&&lab.Session.ExtentMol>=extent,"Repeated A dose remains permitted");
            bench.OpenSheet();yield return .2;Check(bench.SheetOpen&&bench.details.text.Contains("CaCO₃"),"Optional sheet displays current products");
            before=lab.Session.VolumeMl;bench.RequestReset();yield return .2;Check(bench.Modal,"Used mixture requires confirmation");bench.Cancel();
            Check(lab.Session.VolumeMl==before&&!bench.Modal&&bench.SheetOpen,"Cancel preserves mixture and restores sheet");
            Grab("hcl");yield return .2;Check(!bench.SheetOpen,"Grab closes consultation");bench.RequestReset();Check(!bench.Modal,"Held instrument prevents reset");Drop();yield return .2;
            bench.RequestReset();bench.Confirm();yield return .3;Check(!bench.Model.Ready&&!bench.Model.Used,"Confirmed reset clears pair and restores supplies");
            foreach(var x in Enumerate(Pair("acetic","bicarb")))yield return x;
            foreach(var x in Enumerate(Pour("acetic",.8f)))yield return x;
            foreach(var x in Enumerate(Powder("bicarb")))yield return x;
            Check(lab.Session.ExtentMol>0&&lab.Session.ProductsMol.Last()>0,"Solid dose creates CO2 in gas recipe");
            foreach(string e in lab.Session.Definition.Elements)Check(Math.Abs(lab.Session.AtomConservationError(e))<1e-10,"Gas conservation "+e);
            bench.ResetAll();yield return .3;
            foreach(var x in Enumerate(Pair("naoh","hcl")))yield return x;
            foreach(var x in Enumerate(Pour("naoh",.6f)))yield return x;
            Check(lab.Session.PH>10&&lab.receiver.tint.g>.8f,"Base is not pink before explicit indicator");
            Grab("indicator");yield return .2;Pose(lab.opening.position+Vector3.up*.2f,Quaternion.identity);yield return .2;
            held.activated.Invoke(new ActivateEventArgs());yield return .3;
            Check(lab.Session.IndicatorPresent&&lab.receiver.tint.r>.8f,"Held dropper activation adds indicator and changes tint");Drop();yield return .2;
            foreach(var x in Enumerate(Pour("hcl",1.3f)))yield return x;
            Check(lab.Session.PH<7&&lab.receiver.tint.g>.8f,"Acid excess removes pink tint");
            foreach(var x in Enumerate(Probe(true)))yield return x;
            bench.ResetAll();yield return .3;
            foreach(var x in Enumerate(Pair("cuo","sulfuric")))yield return x;
            foreach(var x in Enumerate(Pour("sulfuric",.8f)))yield return x;
            foreach(var x in Enumerate(Powder("cuo")))yield return x;
            Check(lab.Session.ExtentMol>0&&lab.receiver.tint.r<.4f,"Copper oxide recipe consumes solid and forms blue solution");
            Check(lab.bubbles.particleCount==0&&lab.precipitate.particleCount==0,"Blue solution does not generate gas or precipitate VFX");
            foreach(var x in Enumerate(Probe(false)))yield return x;
            bench.OpenSheet();yield return .3;
            foreach(var text in bench.GetComponentsInChildren<TMPro.TMP_Text>(true))if(Tecaverso.UI.UIVisibility.IsVisible(text.gameObject))
            {text.ForceMeshUpdate();Check(!text.isTextTruncated,"No truncated text: "+text.transform.parent.name);}
            Check(bench.instruments.All(t=>t.GetComponent<Tecaverso.Interaction.XRInteractionMotion>()),"Shared motion attached to every instrument");
            before=lab.Session.VolumeMl;bench.RequestExit();Check(bench.Modal,"Exit with mixture requests confirmation");bench.Cancel();Check(lab.Session.VolumeMl==before,"Cancel exit preserves mixture");
        }
        static IEnumerator<double> Probe(bool supported)
        {
            var probe=bench.thermometer;bench.OpenSheet();
            hand.transform.SetPositionAndRotation(probe.transform.position,probe.transform.rotation);hand.StartManualInteraction((IXRSelectInteractable)probe);yield return .2;
            Check(probe.Held&&!bench.SheetOpen,"XR probe grab closes optional sheet");bench.RequestReset();Check(!bench.Modal,"Held thermometer prevents reset");
            var point=new Vector3(lab.opening.position.x+.10f,lab.receiver.SurfaceY-.015f,lab.opening.position.z);
            hand.transform.SetPositionAndRotation(point-probe.tip.localPosition,Quaternion.identity);yield return .3;
            probe.RefreshDisplay();Check(probe.Immersed,"Only submerged tip reads mixture");
            Check(supported?probe.display.text.Contains(lab.Session.TemperatureC.ToString("0.00")):probe.display.text.Contains("indisponível"),"Probe displays modeled temperature or explicit unavailable state");
            if(supported)Check(lab.Session.TemperatureC>25&&lab.Session.HeatReleasedJ>0,"Actual XR dosing releases heat according to model");
            hand.EndManualInteraction();yield return .3;Check(probe.Immersed,"Released probe remains in measurement position");
            hand.transform.SetPositionAndRotation(probe.transform.position,probe.transform.rotation);hand.StartManualInteraction((IXRSelectInteractable)probe);yield return .2;
            hand.transform.position+=Vector3.up*.6f;yield return .3;probe.RefreshDisplay();Check(!probe.Immersed&&probe.display.text.Contains("Mergulhe"),"Removing probe clears stale measurement");
            hand.EndManualInteraction();yield return .2;probe.ReturnHome();
        }
    }
}
