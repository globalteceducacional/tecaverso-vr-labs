using System.Linq;
using Tecaverso.Labs.Gravitation;
using Tecaverso.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Casters;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Object=UnityEngine.Object;

namespace Tecaverso.Editor
{
    public static class PlanetInspectionAuthoring
    {
        static void Height(GameObject go,float value)
        {var e=go.GetComponent<LayoutElement>()??go.AddComponent<LayoutElement>();e.minHeight=e.preferredHeight=value;e.flexibleHeight=0;}
        static RectTransform Rect(string name,Transform parent,Vector2 size,Vector2 at)
        {
            var t=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();t.SetParent(parent,false);
            t.sizeDelta=size;t.anchoredPosition=at;return t;
        }
        static TMP_Text Text(string name,Transform parent,TMP_FontAsset font,string value,Vector2 size,Vector2 at,float fontSize)
        {
            var t=Rect(name,parent,size,at).gameObject.AddComponent<TextMeshProUGUI>();
            t.font=font;t.fontSize=fontSize;t.color=new Color32(22,22,22,255);t.text=value;
            t.textWrappingMode=TextWrappingModes.Normal;t.overflowMode=TextOverflowModes.Truncate;t.raycastTarget=false;
            return t;
        }
        static RectTransform CanvasCard(string name,Transform parent,Vector2 size,bool interactive)
        {
            var t=Rect(name,parent,size,Vector2.zero);t.localScale=Vector3.one*.0015f;
            var c=t.gameObject.AddComponent<Canvas>();c.renderMode=RenderMode.WorldSpace;c.sortingOrder=30;
            var bg=t.gameObject.AddComponent<Image>();bg.color=new Color32(232,230,254,255);bg.raycastTarget=interactive;
            if(interactive)t.gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();
            return t;
        }
        static Button Button(Button source,Transform parent,string label)
        {
            var b=Object.Instantiate(source,parent);b.name=label;b.onClick=new Button.ButtonClickedEvent();
            b.interactable=true;b.GetComponentInChildren<TMP_Text>(true).text=label;return b;
        }
        public static void Configure(GravitationLab lab)
        {
            var solar=lab.solar;var p=lab.panel;var root=solar.transform;
            root.position=new Vector3(1,1.65f,4.5f);
            var table=root.Find("Orbital table");if(table!=null)table.gameObject.SetActive(false);
            // Only the room instance is expanded; authored rig and main canvas are retained.
            var room=lab.gameObject.scene.GetRootGameObjects().Single(g=>g.name=="TecaversoStandardRoom");
            room.transform.localScale=new Vector3(1.3f,1.2f,1.7f);
            PrefabUtility.RecordPrefabInstancePropertyModifications(room.transform);
            var window=lab.transform.Find("Star observation window");
            window.localPosition=new Vector3(0,4.2f,11.2f);window.localScale=new Vector3(17.6f,7.6f,1);
            foreach(var frame in lab.transform.Cast<Transform>().Where(t=>t.name=="Window frame"))
            {
                var at=frame.localPosition;at.z=11.1f;
                if(frame.localScale.x<1){at.x=Mathf.Sign(at.x)*8.85f;}
                else frame.localScale=new Vector3(17.8f,.1f,.1f);
                frame.localPosition=at;
            }
            // Keep the scale reference low and outside the central walking area.
            foreach(var t in root.Cast<Transform>().Where(t=>t.name.StartsWith("Scale tick")||t.name.StartsWith("Ruler mark")||t.name=="Distance ruler"||t.name=="Distance scale"))
            {var at=t.localPosition;at.y=-1.4f;t.localPosition=at;}
            var inspection=solar.GetComponent<PlanetInspection>();
            if(inspection==null)
            {
                inspection=solar.gameObject.AddComponent<PlanetInspection>();inspection.solar=solar;inspection.lab=lab;
                Height(p.solarGuide.gameObject,112);
                var grid=Rect("Planet selection",p.solarControls.transform,new Vector2(624,128),Vector2.zero);
                var layout=grid.gameObject.AddComponent<GridLayoutGroup>();layout.constraint=GridLayoutGroup.Constraint.FixedColumnCount;
                layout.constraintCount=4;layout.cellSize=new Vector2(150,56);layout.spacing=new Vector2(8,8);Height(grid.gameObject,120);
                inspection.planetButtons=new Button[8];
                for(int i=0;i<8;i++)
                {
                    var b=Button(p.bodies[0],grid,PlanetOrbit.Planets[i].Name);
                    b.GetComponentInChildren<TMP_Text>(true).fontSize=20;inspection.planetButtons[i]=b;
                    var body=solar.planets[i].gameObject;
                    var collider=body.GetComponent<SphereCollider>();if(collider==null)collider=body.AddComponent<SphereCollider>();
                    collider.isTrigger=true;collider.radius=.65f;
                    var interactable=body.AddComponent<XRSimpleInteractable>();interactable.colliders.Clear();interactable.colliders.Add(collider);
                    var pointer=body.AddComponent<PlanetPointer>();pointer.inspection=inspection;pointer.index=i;
                }
                var hover=CanvasCard("Planet hover",root,new Vector2(700,320),false);
                inspection.hoverCard=hover.gameObject;
                inspection.hoverText=Text("Planet facts",hover,p.metrics.font,"",new Vector2(652,280),Vector2.zero,25);
                var inspect=new GameObject("Planet close inspection");inspect.transform.SetParent(root,false);
                inspection.inspectionRoot=inspect;
                var card=CanvasCard("Inspection panel",inspect.transform,new Vector2(1280,760),true);
                inspection.title=Text("Title",card,p.captions[0].font,"",new Vector2(1160,70),new Vector2(0,310),36);
                inspection.details=Text("Details",card,p.metrics.font,"",new Vector2(640,440),new Vector2(280,20),26);
                var actions=Rect("Inspection actions",card,new Vector2(1160,80),new Vector2(0,-300));
                var h=actions.gameObject.AddComponent<HorizontalLayoutGroup>();h.spacing=16;h.childControlWidth=h.childControlHeight=true;h.childForceExpandWidth=true;
                var buttons=new[]{Button(p.actions[2],actions,"GIRAR ESQUERDA"),Button(p.actions[2],actions,"GIRAR DIREITA"),Button(p.actions[2],actions,"FECHAR")};
                foreach(var b in buttons){var e=b.GetComponent<LayoutElement>();e.minWidth=0;e.preferredWidth=-1;e.flexibleWidth=1;Height(b.gameObject,80);}
                inspection.rotateLeft=buttons[0];inspection.rotateRight=buttons[1];inspection.close=buttons[2];
                var preview=GameObject.CreatePrimitive(PrimitiveType.Sphere);preview.name="Inspection globe";
                Object.DestroyImmediate(preview.GetComponent<Collider>());preview.transform.SetParent(inspect.transform,false);
                preview.transform.localPosition=new Vector3(-.52f,.02f,-.35f);preview.transform.localScale=Vector3.one*.62f;
                inspection.preview=preview.transform;
                inspection.previewRing=Object.Instantiate(solar.planets[5].Find("Rings"),preview.transform,false);
                inspection.previewRing.gameObject.SetActive(false);inspect.SetActive(false);UIVisibility.Set(hover.gameObject,false);
            }
            foreach(var caster in Object.FindObjectsByType<CurveInteractionCaster>(FindObjectsInactive.Include,FindObjectsSortMode.None))
            {
                if(caster.gameObject.scene!=lab.gameObject.scene)continue;
                caster.raycastTriggerInteraction=QueryTriggerInteraction.Collide;
                EditorUtility.SetDirty(caster);PrefabUtility.RecordPrefabInstancePropertyModifications(caster);
            }
            solar.SetView(false);solar.Show(0,true);
            ConfigureScales(lab);
            ConfigureWalkthrough(lab);
            ObservatoryAuthoring.Configure(lab);
            EditorUtility.SetDirty(inspection);EditorUtility.SetDirty(solar);
        }
        // Targeted update for the expanded exhibit; does not recreate objects or UI.
        public static void ConfigureWalkthrough(GravitationLab lab)
        {
            var solar=lab.solar;
            solar.transform.position=new Vector3(1,2.1f,8);
            var room=lab.gameObject.scene.GetRootGameObjects().Single(g=>g.name=="TecaversoStandardRoom");
            room.transform.position=new Vector3(0,0,4);
            room.transform.localScale=new Vector3(32f/14f,1.2f,40f/14f);
            PrefabUtility.RecordPrefabInstancePropertyModifications(room.transform);
            var window=lab.transform.Find("Star observation window");
            window.localPosition=new Vector3(0,4.2f,22.4f);window.localScale=new Vector3(30,7.6f,1);
            foreach(var frame in lab.transform.Cast<Transform>().Where(t=>t.name=="Window frame"))
            {
                var at=frame.localPosition;at.z=22.3f;
                if(frame.localScale.x<1)at.x=Mathf.Sign(at.x)*15.05f;
                else frame.localScale=new Vector3(30.2f,.1f,.1f);
                frame.localPosition=at;
            }
            foreach(var t in solar.transform.Cast<Transform>().Where(t=>t.name.StartsWith("Scale tick")||t.name.StartsWith("Ruler mark")||t.name=="Distance ruler"||t.name=="Distance scale"))
            {
                var at=t.localPosition;at.y=-1.85f;
                // Place the existing metre ruler near the entrance; retain its one-metre spacing.
                at.z=t.name=="Distance scale"?-9.9f:t.name.StartsWith("Scale tick")?-8.72f:-4.4f;
                t.localPosition=at;
            }
            var inspection=solar.GetComponent<PlanetInspection>();
            inspection.roomMin=new Vector2(-14.5f,-14.5f);inspection.roomMax=new Vector2(14.5f,22.5f);
            solar.SetView(false);solar.Show(0,true);
            EditorUtility.SetDirty(inspection);EditorUtility.SetDirty(solar);
        }
        public static void ConfigureScales(GravitationLab lab)
        {
            var p=lab.panel;var solar=lab.solar;
            if(p.realScale==null)
            {
                p.realScale=Object.Instantiate(p.options[2],p.options[2].transform.parent);
                p.realScale.name="Unified physical scale";
                p.realScale.onValueChanged=new Toggle.ToggleEvent();
                p.realScale.SetIsOnWithoutNotify(false);
                p.realScale.GetComponentInChildren<TMP_Text>(true).text="ESCALA ÚNICA REAL · corpos e distâncias";
                Height(p.realScale.gameObject,48);
                UIVisibility.Set(p.realScale.gameObject,false,true);
            }
            if(solar.selectionMarkers==null||solar.selectionMarkers.Length!=8)
            {
                solar.selectionMarkers=new LineRenderer[8];
                for(int i=0;i<8;i++)
                {
                    var marker=Object.Instantiate(solar.orbits[i],solar.transform);
                    marker.name=PlanetOrbit.Planets[i].Name+" selection target";
                    marker.useWorldSpace=false;marker.loop=true;marker.positionCount=4;
                    marker.startWidth=marker.endWidth=.002f;
                    marker.SetPositions(new[]{new Vector3(-.04f,0,0),new Vector3(0,.04f,0),new Vector3(.04f,0,0),new Vector3(0,-.04f,0)});
                    solar.selectionMarkers[i]=marker;
                }
            }
            solar.rockyLabel.rectTransform.sizeDelta=new Vector2(3,.3f);
            solar.SetView(false);solar.Show(0,true);
            EditorUtility.SetDirty(p);EditorUtility.SetDirty(solar);
        }
    }
}
