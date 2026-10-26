using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Object=UnityEngine.Object;

namespace Tecaverso.Editor
{
    /// <summary>Reskins existing interactables with original Unity UI_3D meshes.
    /// Keeps interactable roots, event bindings and simulation adapters intact.</summary>
    public static class Unity3DControlVisuals
    {
        const string Folder="Assets/_Project/Shared/XR/Controls";
        static void Ensure(string path){if(AssetDatabase.IsValidFolder(path))return;int i=path.LastIndexOf('/');Ensure(path.Substring(0,i));AssetDatabase.CreateFolder(path.Substring(0,i),path.Substring(i+1));}
        static Material Mat(string name,Color color,float metallic=0)
        {
            string path=Folder+"/"+name+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);if(m!=null)return m;
            m=new Material(Shader.Find("Universal Render Pipeline/Lit")){name=name};m.SetColor("_BaseColor",color);m.SetFloat("_Metallic",metallic);m.SetFloat("_Smoothness",.45f);AssetDatabase.CreateAsset(m,path);return m;
        }
        static GameObject Visual(string kind)
        {
            Ensure(Folder);string path=Folder+"/Unity "+kind+" Visual.prefab";var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(path);if(prefab!=null)return prefab;
            var blue=Mat("Tecaverso control blue",new Color32(40,78,160,255));var silver=Mat("Control silver",new Color(.65f,.69f,.74f),.65f);var dark=Mat("Control graphite",new Color(.045f,.06f,.08f));
            var source=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ThirdParty/XRI3DUI/Models/"+kind+".fbx");
            var root=new GameObject("Unity "+kind+" Visual");
            foreach(var mf in source.GetComponentsInChildren<MeshFilter>())
            {
                bool moving=mf.sharedMesh.name==(kind=="Dial"?"Knob":kind=="Lever"?"LeverStick":"Slider");
                var pivot=new GameObject(moving?"Handle":"Base").transform;pivot.SetParent(root.transform,false);
                var part=new GameObject(mf.sharedMesh.name).transform;part.SetParent(pivot,false);
                // Existing lab dials face -Z from a +90X root. Flip mesh only so
                // the original XRKnob handle pivot still rotates about local Y.
                if(kind=="Dial")part.localRotation=Quaternion.Euler(180,0,0);
                part.gameObject.AddComponent<MeshFilter>().sharedMesh=mf.sharedMesh;
                var mats=mf.GetComponent<Renderer>().sharedMaterials.Select(m=>m.name.Contains("accent")||m.name.Contains("shinyBlack")?blue:m.name.Contains("hole")?dark:silver).ToArray();part.gameObject.AddComponent<MeshRenderer>().sharedMaterials=mats;
                if(moving)
                {
                    var c=pivot.gameObject.AddComponent<BoxCollider>();
                    if(kind=="Dial"){c.center=new Vector3(0,-.05f,0);c.size=new Vector3(.115f,.09f,.115f);}
                    else if(kind=="Lever"){c.center=new Vector3(0,.18f,0);c.size=new Vector3(.065f,.14f,.085f);}
                    else{c.center=new Vector3(0,.012f,0);c.size=new Vector3(.05f,.045f,.045f);}
                    var attach=new GameObject("Hand attach").transform;attach.SetParent(pivot,false);attach.localPosition=c.center;
                }
            }
            prefab=PrefabUtility.SaveAsPrefabAsset(root,path);Object.DestroyImmediate(root);return prefab;
        }
        static Transform Instance(Transform root,string kind,float scale)
        {
            var previous=root.Find("Unity "+kind+" Visual");if(previous!=null)return previous;
            var go=(GameObject)PrefabUtility.InstantiatePrefab(Visual(kind),root);go.transform.localPosition=Vector3.zero;go.transform.localRotation=Quaternion.identity;go.transform.localScale=Vector3.one*scale;return go.transform;
        }
        static void Snap(XRBaseInteractable control,Transform handle)
        {
            control.colliders.Clear();handle.GetComponent<Collider>().enabled=true;control.colliders.Add(handle.GetComponent<Collider>());
            var so=new SerializedObject(control);var attach=so.FindProperty("m_InteractorSnapTransform");if(attach!=null)attach.objectReferenceValue=handle.Find("Hand attach");so.ApplyModifiedPropertiesWithoutUndo();EditorUtility.SetDirty(control);
        }
        public static void Knob(XRKnob knob,float scale)
        {
            if(knob.handle!=null&&!knob.handle.name.Equals("Handle"))knob.handle.gameObject.SetActive(false);
            var visual=Instance(knob.transform,"Dial",scale);knob.handle=visual.Find("Handle");Snap(knob,knob.handle);knob.value=knob.value;
        }
        public static void Lever(XRLever lever,float scale)
        {
            foreach(var old in lever.colliders)if(old!=null)old.enabled=false;
            // Retire only previous control visuals, keeping the control root and
            // its persistent activate/deactivate events (e.g. circuit wiring).
            foreach(Transform child in lever.transform)if(child.name!="Unity Lever Visual")child.gameObject.SetActive(false);
            var visual=Instance(lever.transform,"Lever",scale);lever.handle=visual.Find("Handle");lever.minAngle=-40;lever.maxAngle=40;Snap(lever,lever.handle);
            var so=new SerializedObject(lever);so.FindProperty("m_LockToValue").boolValue=true;so.ApplyModifiedPropertiesWithoutUndo();lever.value=lever.value;
        }
        public static void SliderGrip(Transform root,float scale)
        {
            var grip=root.Find("Grip");if(grip!=null)grip.gameObject.SetActive(false);
            var visual=Instance(root,"Slider",scale);visual.Find("Base").gameObject.SetActive(false);
            // The existing scientific rail and constrained grab own the collider.
            visual.Find("Handle").GetComponent<Collider>().enabled=false;
        }
        public static string UpgradeLoadedScene()
        {
            var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();int knobs=0,levers=0,slides=0;
            foreach(var root in scene.GetRootGameObjects())
            {
                foreach(var k in root.GetComponentsInChildren<XRKnob>(true)){Knob(k,k is Tecaverso.Labs.Thermology.ThermalKnob?3.5f:2.7f);knobs++;}
                foreach(var l in root.GetComponentsInChildren<XRLever>(true)){Lever(l,l is Tecaverso.Labs.Waves.WaveSwitch?1.3f:2.2f);levers++;}
                foreach(var s in root.GetComponentsInChildren<Tecaverso.Labs.Waves.WaveSlide>(true)){SliderGrip(s.transform,3.6f);slides++;}
            }
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();return $"{scene.name}: {knobs} dials, {levers} levers, {slides} slider grips.";
        }
    }
}
