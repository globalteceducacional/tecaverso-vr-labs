using System.Linq;
using Tecaverso.Labs.Gravitation;
using Tecaverso.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Tecaverso.Editor
{
    /// <summary>Targeted presentation migration; never rebuilds the room, rig or canvas.</summary>
    public static class GravitationPresentation
    {
        static void Height(GameObject go,float height)
        {
            var item=go.GetComponent<LayoutElement>()??go.AddComponent<LayoutElement>();
            item.minHeight=item.preferredHeight=height;item.flexibleHeight=0;
        }
        public static void Configure(GravitationLab lab)
        {
            var p=lab.panel;
            var content=p.modes[0].transform.parent.parent;
            p.introduction=content.GetChild(1).GetComponent<TMP_Text>();
            if(p.solarControls==null)
            {
                var group=new GameObject("Solar exploration",typeof(RectTransform),typeof(VerticalLayoutGroup),typeof(LayoutElement));
                group.transform.SetParent(content,false);group.transform.SetSiblingIndex(4);
                var layout=group.GetComponent<VerticalLayoutGroup>();
                layout.spacing=16;layout.childControlWidth=layout.childControlHeight=true;
                layout.childForceExpandWidth=true;layout.childForceExpandHeight=false;
                Height(group,360);
                var row=Object.Instantiate(p.bodies[0].transform.parent.gameObject,group.transform);
                row.name="View scale";Height(row,64);
                var buttons=row.GetComponentsInChildren<Button>(true);
                Object.DestroyImmediate(buttons[2].gameObject);
                p.solarViews=new[]{buttons[0],buttons[1]};
                string[] names={"SISTEMA COMPLETO","ROCHOSOS · AMPLIAR"};
                for(int i=0;i<2;i++)
                {
                    var b=buttons[i];b.name=names[i];b.onClick=new Button.ButtonClickedEvent();
                    b.interactable=true;Height(b.gameObject,64);
                    b.GetComponentInChildren<TMP_Text>(true).text=names[i];
                }
                p.solarGuide=Object.Instantiate(p.metrics,group.transform);
                p.solarGuide.name="Solar distance guide";p.solarGuide.fontSize=22;
                p.solarGuide.color=new Color32(22,22,22,255);
                p.solarGuide.textWrappingMode=TextWrappingModes.Normal;
                Height(p.solarGuide.gameObject,280);
                p.solarControls=group;
            }
            foreach(var button in p.solarViews)
            {
                var size=button.GetComponent<LayoutElement>();
                size.minWidth=0;size.preferredWidth=-1;size.flexibleWidth=1;
                var text=button.GetComponentInChildren<TMP_Text>(true);
                text.fontSize=20;
                var colors=button.colors;colors.disabledColor=Color.white;button.colors=colors;
            }
            p.solarViews[0].transform.parent.GetComponent<HorizontalLayoutGroup>().childForceExpandWidth=true;
            UIVisibility.Set(p.solarControls,false,true);
            lab.satelliteFurniture=new[]{"Observation platform","Projection pedestal","Projector rim","Scale reminder"}
                .Select(n=>lab.transform.Find(n).gameObject).ToArray();
            var solar=lab.solar;var root=solar.transform;
            root.position=new Vector3(1.5f,.8f,0);root.rotation=Quaternion.identity;
            solar.sun=root.Find("Sun");
            if(root.Find("Orbital table")==null)
            {
                var table=GameObject.CreatePrimitive(PrimitiveType.Cylinder);table.name="Orbital table";
                table.transform.SetParent(root,false);table.transform.localPosition=new Vector3(0,-.22f,0);
                table.transform.localScale=new Vector3(8.4f,.08f,8.4f);
                Object.DestroyImmediate(table.GetComponent<Collider>());
                table.GetComponent<Renderer>().sharedMaterial=lab.transform.Find("Observation platform").GetComponent<Renderer>().sharedMaterial;
            }
            TMP_Text Label(string name,string text,Vector3 at,float width)
            {
                var existing=root.Find(name);
                var t=existing!=null?existing.GetComponent<TMP_Text>():Object.Instantiate(solar.labels[0],root);
                t.name=name;t.text=text;t.transform.localPosition=at;t.transform.localRotation=Quaternion.identity;
                t.rectTransform.sizeDelta=new Vector2(width,.45f);t.fontSize=1.2f;
                return t;
            }
            solar.rockyLabel=Label("Rocky group label","SOL + ROCHOSOS\nAmplie no painel",new Vector3(0,.42f,0),2);
            solar.rulerLabel=Label("Distance scale","DISTÂNCIAS",new Vector3(0,.02f,-4.55f),4);
            solar.rulerTicks=new TMP_Text[4];
            for(int i=0;i<4;i++)solar.rulerTicks[i]=Label("Scale tick "+i,"",new Vector3(i-1.5f,0,-4.32f),.65f);
            var rule=root.Find("Distance ruler");
            if(rule==null)
            {
                var line=Object.Instantiate(solar.orbits[7],root);line.name="Distance ruler";
                line.loop=false;line.startWidth=line.endWidth=.012f;line.positionCount=2;
                line.SetPositions(new[]{new Vector3(-1.5f,0,-4.1f),new Vector3(1.5f,0,-4.1f)});
                for(int i=0;i<4;i++)
                {
                    var tick=Object.Instantiate(line,root);tick.name="Ruler mark "+i;
                    tick.SetPositions(new[]{new Vector3(i-1.5f,0,-4.04f),new Vector3(i-1.5f,0,-4.16f)});
                }
            }
            solar.SetView(false);solar.Show(0,true);
            PlanetInspectionAuthoring.Configure(lab);
            EditorUtility.SetDirty(p);EditorUtility.SetDirty(solar);EditorUtility.SetDirty(lab);
        }
    }
}
