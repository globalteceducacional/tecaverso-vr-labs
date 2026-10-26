using System;
using System.Linq;
using Tecaverso.Labs.ResistorCircuits;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using Object=UnityEngine.Object;

namespace Tecaverso.Editor
{
    /// <summary>Scene-local visual finishing. Existing electrical terminals and authored transforms stay intact.</summary>
    public static class ResistorCircuitsPresentation
    {
        const string Materials="Assets/_Project/Laboratories/Physics/ResistorCircuits/Materials/";
        static Material navy,silver,ceramic,gold,red,orange,black,glass,display;
        static TMP_FontAsset font;
        static Material Material(string name,Color color,bool emit=false)
        {
            var path=Materials+name+".mat";
            var material=AssetDatabase.LoadAssetAtPath<Material>(path);
            if(material!=null)return material;
            material=new Material(Shader.Find("Universal Render Pipeline/Lit")){name=name};
            material.SetColor("_BaseColor",color);
            material.SetFloat("_Smoothness",name.Contains("Metal")?.72f:.24f);
            if(emit){material.EnableKeyword("_EMISSION");material.SetColor("_EmissionColor",color*.6f);}
            AssetDatabase.CreateAsset(material,path);return material;
        }
        static Transform Shape(Transform parent,string name,PrimitiveType type,Vector3 point,Vector3 scale,Material material,Vector3? rotation=null)
        {
            var go=GameObject.CreatePrimitive(type);go.name=name;go.transform.SetParent(parent,false);
            go.transform.localPosition=point;go.transform.localScale=scale;
            if(rotation.HasValue)go.transform.localEulerAngles=rotation.Value;
            go.GetComponent<Renderer>().sharedMaterial=material;
            Object.DestroyImmediate(go.GetComponent<Collider>());
            return go.transform;
        }
        static TextMeshPro Label(Transform parent,string name,string content,Vector3 point,Vector2 bounds,float size,Color color)
        {
            var label=new GameObject(name,typeof(TextMeshPro)).GetComponent<TextMeshPro>();
            label.transform.SetParent(parent,false);label.transform.localPosition=point;
            label.rectTransform.sizeDelta=bounds;label.font=font;label.fontSize=size;
            label.text=content;label.color=color;label.alignment=TextAlignmentOptions.Center;
            label.raycastTarget=false;return label;
        }
        static Transform Begin(CircuitPart part)
        {
            var previous=part.transform.Find("Housing");
            if(previous!=null){previous.GetComponent<Renderer>().enabled=false;previous.GetComponent<Collider>().enabled=false;}
            return new GameObject("Visual model").transform.Apply(t=>t.SetParent(part.transform,false));
        }
        static void Battery(CircuitPart part)
        {
            var model=Begin(part);
            Shape(model,"Nine-volt battery case",PrimitiveType.Cube,new Vector3(0,.14f,0),new Vector3(.70f,.72f,.42f),navy);
            Shape(model,"Battery rim",PrimitiveType.Cube,new Vector3(0,.51f,0),new Vector3(.73f,.055f,.45f),silver);
            Shape(model,"Positive snap",PrimitiveType.Cylinder,new Vector3(-.18f,.59f,0),new Vector3(.10f,.055f,.10f),silver);
            Shape(model,"Negative snap",PrimitiveType.Cylinder,new Vector3(.18f,.59f,0),new Vector3(.075f,.055f,.075f),silver);
            Shape(model,"Front brand plate",PrimitiveType.Cube,new Vector3(0,.12f,-.218f),new Vector3(.62f,.38f,.015f),Material("Battery label blue",new Color32(40,78,160,255)));
            Label(model,"Battery voltage","9 V",new Vector3(0,.18f,-.231f),new Vector2(.54f,.22f),.28f,Color.white);
            Label(model,"Battery legend","FONTE DC",new Vector3(0,-.045f,-.231f),new Vector2(.58f,.14f),.12f,Color.white);
            foreach(var sign in new[]{part.transform.Find("Polarity +"),part.transform.Find("Polarity -")})if(sign!=null)sign.gameObject.SetActive(false);
            Shape(model,"Positive side jack",PrimitiveType.Cylinder,new Vector3(-.45f,0,0),new Vector3(.14f,.10f,.14f),red,new Vector3(0,0,90));
            Shape(model,"Negative side jack",PrimitiveType.Cylinder,new Vector3(.45f,0,0),new Vector3(.14f,.10f,.14f),black,new Vector3(0,0,90));
        }
        static void Resistor(CircuitPart part)
        {
            var model=Begin(part);
            Shape(model,"Ceramic resistor body",PrimitiveType.Cylinder,Vector3.zero,new Vector3(.15f,.35f,.15f),ceramic,new Vector3(0,0,90));
            foreach(float side in new[]{-1f,1f})
            {
                Shape(model,"Crimped metal cap",PrimitiveType.Cylinder,new Vector3(side*.34f,0,0),new Vector3(.157f,.055f,.157f),silver,new Vector3(0,0,90));
                Shape(model,"Axial lead",PrimitiveType.Cylinder,new Vector3(side*.45f,0,0),new Vector3(.035f,.12f,.035f),silver,new Vector3(0,0,90));
            }
            var first=part.resistance<15?Material("Resistor brown",new Color32(111,57,30,255)):part.resistance<25?red:orange;
            var bands=new[]{first,black,black,gold};
            for(int i=0;i<4;i++)Shape(model,"Value band "+(i+1),PrimitiveType.Cylinder,new Vector3(-.21f+i*.14f,0,0),new Vector3(.155f,.012f,.155f),bands[i],new Vector3(0,0,90));
            if(part.display!=null){part.display.transform.localPosition=new Vector3(0,-.28f,-.06f);part.display.fontSize=.26f;part.display.color=new Color(.85f,.92f,1);}
        }
        static void Lamp(CircuitPart part)
        {
            var model=Begin(part);
            Shape(model,"Lamp socket lower",PrimitiveType.Cylinder,new Vector3(0,.02f,0),new Vector3(.31f,.16f,.31f),navy);
            Shape(model,"Lamp socket ferrule",PrimitiveType.Cylinder,new Vector3(0,.21f,0),new Vector3(.23f,.08f,.23f),silver);
            Shape(model,"Glass neck",PrimitiveType.Cylinder,new Vector3(0,.34f,0),new Vector3(.16f,.10f,.16f),glass);
            var bulb=part.lampRenderer.transform;
            bulb.localPosition=new Vector3(0,.66f,0);bulb.localScale=Vector3.one*.64f;
            bulb.GetComponent<Collider>().enabled=false;
            part.lampRenderer.sharedMaterial=glass;
            part.lampLight.transform.localPosition=Vector3.zero;
            foreach(float side in new[]{-1f,1f})
                Shape(model,"Filament support",PrimitiveType.Cylinder,new Vector3(side*.09f,.56f,-.20f),new Vector3(.012f,.14f,.012f),silver);
            Shape(model,"Glowing filament",PrimitiveType.Cylinder,new Vector3(0,.69f,-.20f),new Vector3(.012f,.11f,.012f),gold,new Vector3(0,0,90));
        }
        static void Meter(CircuitPart part,bool voltmeter)
        {
            var model=Begin(part);
            Shape(model,"Rubber instrument case",PrimitiveType.Cube,Vector3.zero,new Vector3(.96f,.58f,.44f),navy);
            Shape(model,"Screen bezel",PrimitiveType.Cube,new Vector3(0,.07f,-.235f),new Vector3(.78f,.36f,.025f),silver);
            Shape(model,"LCD",PrimitiveType.Cube,new Vector3(0,.07f,-.252f),new Vector3(.70f,.28f,.012f),display);
            if(part.display!=null)
            {
                part.display.transform.localPosition=new Vector3(0,.06f,-.266f);
                part.display.rectTransform.sizeDelta=new Vector2(.66f,.23f);
                part.display.fontSize=.30f;part.display.color=new Color32(20,54,66,255);
            }
            Shape(model,"Selector knob",PrimitiveType.Cylinder,new Vector3(0,-.19f,-.25f),new Vector3(.08f,.035f,.08f),black,new Vector3(90,0,0));
            Label(model,"Meter type",voltmeter?"V":"A",new Vector3(.33f,-.19f,-.25f),new Vector2(.18f,.15f),.16f,Color.white);
            foreach(float side in new[]{-1f,1f})
                Shape(model,"Probe socket bezel",PrimitiveType.Cylinder,new Vector3(side*.58f,0,0),new Vector3(.13f,.07f,.13f),side<0?red:black,new Vector3(0,0,90));
        }
        static void Junction(CircuitPart part)
        {
            var model=Begin(part);
            Shape(model,"Four-way terminal plate",PrimitiveType.Cube,Vector3.zero,new Vector3(.74f,.64f,.35f),navy);
            Shape(model,"Exposed bus bar",PrimitiveType.Cube,new Vector3(0,0,-.19f),new Vector3(.13f,.45f,.025f),silver);
            Shape(model,"Bus bar crosspiece",PrimitiveType.Cube,new Vector3(0,0,-.205f),new Vector3(.53f,.065f,.025f),silver);
            foreach(var terminal in new[]{part.a,part.b}.Concat(part.additionalTerminals??Array.Empty<CircuitTerminal>()))
                Shape(model,"Socket surround",PrimitiveType.Cylinder,terminal.transform.localPosition*.78f,new Vector3(.125f,.035f,.125f),silver,new Vector3(0,0,90));
        }
        static void Switch(CircuitPart part)
        {
            var model=Begin(part);
            Shape(model,"Switch mounting plate",PrimitiveType.Cube,new Vector3(0,-.12f,0),new Vector3(.90f,.18f,.46f),navy);
            Shape(model,"Switch raised base",PrimitiveType.Cylinder,new Vector3(0,.05f,0),new Vector3(.25f,.07f,.25f),silver);
            foreach(float x in new[]{-.36f,.36f})
                Shape(model,"Mounting screw",PrimitiveType.Cylinder,new Vector3(x,-.012f,-.17f),new Vector3(.035f,.009f,.035f),silver);
            var old=part.transform.Find("Handle");if(old!=null)old.gameObject.SetActive(false);
            var interaction=new GameObject("XR lever").transform;interaction.SetParent(part.transform,false);
            interaction.localPosition=new Vector3(0,.18f,0);
            var pivot=new GameObject("Lever pivot").transform;pivot.SetParent(interaction,false);
            Shape(pivot,"Chrome shaft",PrimitiveType.Cylinder,new Vector3(0,.23f,0),new Vector3(.048f,.25f,.048f),silver);
            var knob=Shape(pivot,"Insulated grip",PrimitiveType.Sphere,new Vector3(0,.50f,0),Vector3.one*.22f,black);
            var knobCollider=knob.gameObject.AddComponent<SphereCollider>();knobCollider.radius=.5f;
            var attach=new GameObject("Hand snap").transform;attach.SetParent(knob,false);
            var lever=interaction.gameObject.AddComponent<XRLever>();
            lever.handle=pivot;lever.minAngle=-40;lever.maxAngle=40;lever.lockToValue=true;
            lever.colliders.Add(knobCollider);
            var so=new SerializedObject(lever);
            so.FindProperty("m_InteractorSnapTransform").objectReferenceValue=attach;
            so.FindProperty("m_LockToValue").boolValue=true;
            so.ApplyModifiedPropertiesWithoutUndo();
            UnityEventTools.AddPersistentListener(lever.onLeverActivate,part.CloseSwitch);
            UnityEventTools.AddPersistentListener(lever.onLeverDeactivate,part.OpenSwitch);
            lever.value=false;
            Label(model,"On label","I",new Vector3(0,.03f,-.25f),new Vector2(.2f,.18f),.15f,Color.white);
        }
        static void Plugs(CircuitLab lab)
        {
            foreach(var cable in lab.cables)
            foreach(var end in new[]{cable.first,cable.second})
            {
                var parent=end.transform;
                var grip=parent.GetComponent<Renderer>();grip.sharedMaterial=navy;
                Shape(parent,"Metal banana contact",PrimitiveType.Cylinder,new Vector3(0,.15f,0),new Vector3(.045f,.10f,.045f),silver);
                Shape(parent,"Finger collar",PrimitiveType.Cylinder,new Vector3(0,-.07f,0),new Vector3(.09f,.035f,.09f),end==cable.first?red:black);
            }
        }
        public static string Configure()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Exit Play Mode first.");
            var lab=Object.FindFirstObjectByType<CircuitLab>();if(lab==null)throw new InvalidOperationException("Open the Resistor Circuits scene.");
            if(lab.transform.Find("Visual model finish")!=null)throw new InvalidOperationException("Visual models already applied; edit the authored objects directly.");
            font=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Bold SDF.asset");
            navy=Material("Device navy",new Color32(17,31,54,255));silver=Material("Brushed metal",new Color32(159,173,190,255));
            ceramic=Material("Resistor ceramic",new Color32(213,193,157,255));gold=Material("Contact gold",new Color32(220,164,65,255),true);
            red=Material("Indicator red",new Color32(190,48,55,255));orange=Material("Indicator orange",new Color32(225,107,40,255));
            black=Material("Soft rubber",new Color32(28,33,43,255));glass=Material("Bulb opal glass",new Color32(241,219,170,255),true);
            display=Material("Instrument LCD",new Color32(190,222,216,255),true);
            foreach(var part in lab.parts)
            {
                switch(part.role)
                {
                    case ElectricalRole.Source:Battery(part);break;
                    case ElectricalRole.Switch:Switch(part);break;
                    case ElectricalRole.Lamp:Lamp(part);break;
                    case ElectricalRole.Resistor:Resistor(part);break;
                    case ElectricalRole.Junction:Junction(part);break;
                    case ElectricalRole.Ammeter:Meter(part,false);break;
                    case ElectricalRole.Voltmeter:Meter(part,true);break;
                }
            }
            Plugs(lab);
            foreach(var control in lab.GetComponentsInChildren<XRLever>(true))Unity3DControlVisuals.Lever(control,2.2f);
            var marker=new GameObject("Visual model finish");marker.transform.SetParent(lab.transform,false);
            AssetDatabase.SaveAssets();
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            return "Finished 3D models and XR lever in the existing circuit scene.";
        }
        static T Apply<T>(this T value,Action<T> action){action(value);return value;}
    }
}
