#if UNITY_EDITOR
using System.Collections.Generic;
using Tecaverso.Labs.ObliqueLaunch;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

public static class ObliqueLaunchSceneBuilder
{
    const string ScenePath = "Assets/_Project/Laboratories/Physics/ObliqueLaunch/Scenes/ObliqueLaunch.unity";
    const string MaterialPath = "Assets/_Project/Laboratories/Physics/ObliqueLaunch/Materials/";

    [MenuItem("Tecaverso/Build Oblique Launch MVP")]
    public static void Build()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath);
        var previous = GameObject.Find("Oblique Launch Lab");
        if (previous != null) Object.DestroyImmediate(previous);
        var oldPlane = GameObject.Find("Plane");
        if (oldPlane != null) Object.DestroyImmediate(oldPlane);

        var root = new GameObject("Oblique Launch Lab");
        var simulation = root.AddComponent<ObliqueLaunchSimulation>();
        var lab = root.AddComponent<ObliqueLaunchLab>();

        var ground = Primitive("Ground", PrimitiveType.Cube, root.transform, new Vector3(90f, -.1f, 0f), new Vector3(180f, .2f, 12f), Mat("Ground", new Color(.055f,.08f,.11f)));
        ground.isStatic = true;
        Grid(root.transform);

        var platform = new GameObject("Launch Platform"); platform.transform.SetParent(root.transform);
        var baseCylinder = Primitive("Adjustable Base", PrimitiveType.Cylinder, platform.transform, Vector3.zero, new Vector3(1.2f,.5f,1.2f), Mat("Base", new Color(.12f,.25f,.34f)));
        var pivot = new GameObject("Cannon Pivot").transform; pivot.SetParent(platform.transform);
        var barrel = Primitive("Cannon Barrel", PrimitiveType.Cylinder, pivot, Vector3.up*.75f, new Vector3(.25f,.8f,.25f), Mat("Cannon", new Color(.12f,.14f,.17f)));
        var muzzle = new GameObject("Muzzle").transform; muzzle.SetParent(pivot); muzzle.localPosition=Vector3.up*1.65f;

        var projectile = CreateProjectile("Projectile", root.transform, Mat("Projectile", new Color(.08f,.08f,.09f)), true);
        var body = projectile.gameObject.AddComponent<Rigidbody>(); body.isKinematic=true; body.useGravity=false;

        var snapshotRoot = new GameObject("Temporal Snapshots").transform; snapshotRoot.SetParent(root.transform);
        var snapshotViews = new List<ProjectileView>();
        for(int i=0;i<32;i++) { var view=CreateProjectile($"Snapshot {i:00}",snapshotRoot,Mat("Snapshot",new Color(.35f,.8f,1f,.45f)),false); view.transform.localScale=Vector3.one*.65f; view.gameObject.SetActive(false); snapshotViews.Add(view); }
        var pool = snapshotRoot.gameObject.AddComponent<TrajectorySnapshotPool>(); pool.Bind(snapshotViews.ToArray());

        var trajectory = CreateTrajectory(root.transform,projectile.GetComponent<Renderer>().sharedMaterial);
        var rulers = CreateRulers(root.transform);
        var panel = CreatePanel(root.transform);

        lab.Bind(simulation,panel,projectile,pool,trajectory,rulers,baseCylinder.transform,pivot,muzzle,body);
        SetObjectField(lab,"baseCylinder",baseCylinder.transform);

        var rig=GameObject.Find("XR Origin (XR Rig)");
        if(rig!=null)
        {
            rig.transform.position=new Vector3(2f,.15f,-6f);
            rig.transform.rotation=Quaternion.identity;
            FaceViewer(panel.transform,rig.transform);
        }
        var light=GameObject.Find("Directional Light");
        if(light!=null) Object.DestroyImmediate(light);
        light=new GameObject("Directional Light");
        var lightComponent=light.AddComponent<Light>(); lightComponent.type=LightType.Directional; lightComponent.intensity=1.2f; light.transform.rotation=Quaternion.Euler(45f,-35f,0f);

        Selection.activeGameObject=root;
        EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
        Debug.Log("[Tecaverso] Oblique Launch MVP scene built.");
    }

    static ObliqueLaunchPanel CreatePanel(Transform parent)
    {
        var canvasGo=new GameObject("XR Control Panel",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster),typeof(TrackedDeviceGraphicRaycaster),typeof(Image)); canvasGo.transform.SetParent(parent);
        var canvas=canvasGo.GetComponent<Canvas>(); canvas.renderMode=RenderMode.WorldSpace;
        var rect=canvasGo.GetComponent<RectTransform>(); rect.sizeDelta=new Vector2(920,720); rect.position=new Vector3(-2.5f,2.4f,-2.2f); rect.rotation=Quaternion.identity; rect.localScale=Vector3.one*.0028f;
        canvasGo.GetComponent<Image>().color=new Color(.025f,.045f,.07f,.96f);
        Text("MOVIMENTO OBLÍQUO",rect,new Vector2(0,310),38,Color.white,TextAlignmentOptions.Center,new Vector2(860,55));
        Text("Ajuste os parâmetros e observe os vetores",rect,new Vector2(0,268),22,new Color(.55f,.8f,1f),TextAlignmentOptions.Center,new Vector2(860,38));
        var angle=SliderRow(rect,"ÂNGULO",new Vector2(0,205),0,90,45,out var angleValue);
        var speed=SliderRow(rect,"VELOCIDADE INICIAL",new Vector2(0,125),0,50,18,out var speedValue);
        var height=SliderRow(rect,"ALTURA DA BASE",new Vector2(0,45),0,10,2,out var heightValue);
        var mass=SliderRow(rect,"MASSA",new Vector2(0,-35),1,10,3,out var massValue);
        var gravity=SliderRow(rect,"GRAVIDADE LOCAL",new Vector2(0,-115),.1f,24.8f,9.81f,out var gravityValue);
        Text("Vresult  VERDE     Vx  AZUL     Vy  VERMELHO",rect,new Vector2(0,-155),19,new Color(.72f,.86f,.96f),TextAlignmentOptions.Center,new Vector2(850,32));
        var metrics=Text("t = 0.00 s     Y = 0.00 m     Ymax = 0.00 m",rect,new Vector2(0,-190),24,Color.white,TextAlignmentOptions.Center,new Vector2(850,45));
        var fire=Button(rect,"DISPARAR",new Vector2(-270,-270),new Color(.1f,.65f,.4f),out _);
        var pause=Button(rect,"PAUSAR",new Vector2(0,-270),new Color(.95f,.65f,.1f),out var pauseLabel);
        var reset=Button(rect,"RESETAR",new Vector2(270,-270),new Color(.82f,.2f,.22f),out _);
        var panel=canvasGo.AddComponent<ObliqueLaunchPanel>(); panel.Bind(angle,speed,height,mass,gravity,angleValue,speedValue,heightValue,massValue,gravityValue,metrics,pauseLabel,fire,pause,reset); return panel;
    }

    static Slider SliderRow(RectTransform parent,string label,Vector2 pos,float min,float max,float value,out TMP_Text valueText)
    {
        Text(label,parent,pos+new Vector2(-300,25),22,Color.white,TextAlignmentOptions.Left,new Vector2(310,35));
        valueText=Text("",parent,pos+new Vector2(330,25),22,new Color(.35f,.85f,1f),TextAlignmentOptions.Right,new Vector2(180,35));
        var go=new GameObject(label+" Slider",typeof(RectTransform),typeof(Slider)); go.transform.SetParent(parent,false); var r=go.GetComponent<RectTransform>(); r.anchoredPosition=pos+new Vector2(45,-10); r.sizeDelta=new Vector2(520,30);
        var bg=UIRect("Background",r,Vector2.zero,new Vector2(520,12),new Color(.12f,.18f,.23f));
        var fillArea=new GameObject("Fill Area",typeof(RectTransform)); fillArea.transform.SetParent(r,false); Stretch(fillArea.GetComponent<RectTransform>(),new Vector2(0,0),new Vector2(1,1),new Vector2(8,8),new Vector2(-8,-8));
        var fill=UIRect("Fill",fillArea.GetComponent<RectTransform>(),Vector2.zero,Vector2.zero,new Color(.1f,.65f,.9f)); Stretch(fill.GetComponent<RectTransform>(),Vector2.zero,Vector2.one,Vector2.zero,Vector2.zero);
        var handleArea=new GameObject("Handle Slide Area",typeof(RectTransform)); handleArea.transform.SetParent(r,false); Stretch(handleArea.GetComponent<RectTransform>(),Vector2.zero,Vector2.one,new Vector2(10,0),new Vector2(-10,0));
        var handle=UIRect("Handle",handleArea.GetComponent<RectTransform>(),Vector2.zero,new Vector2(28,28),Color.white);
        var slider=go.GetComponent<Slider>(); slider.minValue=min; slider.maxValue=max; slider.value=value; slider.fillRect=fill.GetComponent<RectTransform>(); slider.handleRect=handle.GetComponent<RectTransform>(); slider.targetGraphic=handle.GetComponent<Image>(); slider.direction=Slider.Direction.LeftToRight; return slider;
    }

    static Button Button(RectTransform parent,string label,Vector2 pos,Color color,out TMP_Text text)
    {
        var go=UIRect(label+" Button",parent,pos,new Vector2(230,66),color); var button=go.AddComponent<Button>(); button.targetGraphic=go.GetComponent<Image>();
        text=Text(label,go.GetComponent<RectTransform>(),Vector2.zero,24,Color.white,TextAlignmentOptions.Center,new Vector2(220,60)); return button;
    }

    static ProjectileTrajectoryLine CreateTrajectory(Transform parent,Material projectileMaterial)
    {
        var root=new GameObject("Projectile Trajectory"); root.transform.SetParent(parent);
        var line=Line("Trajectory Line",root.transform,new Color(.1f,.85f,1f,.85f),.045f,false);
        line.numCornerVertices=3;
        var trajectory=root.AddComponent<ProjectileTrajectoryLine>(); trajectory.Bind(line,projectileMaterial); return trajectory;
    }

    static MeasurementRulers CreateRulers(Transform parent)
    {
        var root=new GameObject("Digital Rulers"); root.transform.SetParent(parent);
        var range=Line("Range Laser",root.transform,new Color(.15f,1f,.45f),.035f,false); range.positionCount=2;
        var height=Line("Height Laser",root.transform,new Color(1f,.3f,.2f),.035f,false); height.positionCount=2;
        var rt=WorldText("Rtotal: 0.00 m",root.transform,new Vector3(0,.12f,0),.14f,new Color(.15f,1f,.45f));
        var ht=WorldText("Ymax: 0.00 m",root.transform,new Vector3(.15f,0,0),.14f,new Color(1f,.3f,.2f));
        var rulers=root.AddComponent<MeasurementRulers>(); rulers.Bind(range,height,rt,ht); return rulers;
    }

    static ProjectileView CreateProjectile(string name,Transform parent,Material material,bool collider)
    {
        var sphere=Primitive(name,PrimitiveType.Sphere,parent,Vector3.zero,Vector3.one*.35f,material); var c=sphere.GetComponent<Collider>(); if(c!=null&&!collider) Object.DestroyImmediate(c);
        var result=Arrow("Vresult",sphere.transform,Mat("Vresult",new Color(.15f,1f,.35f)));
        var vx=Arrow("Vx",sphere.transform,Mat("Vx",new Color(.15f,.65f,1f)));
        var vy=Arrow("Vy",sphere.transform,Mat("Vy",new Color(1f,.35f,.2f)));
        var view=sphere.AddComponent<ProjectileView>(); view.Bind(result,vx,vy); view.HideVectors(); return view;
    }

    static VectorArrowView Arrow(string name,Transform parent,Material material)
    {
        var root=new GameObject(name); root.transform.SetParent(parent,false);
        var shaft=Primitive("Shaft",PrimitiveType.Cylinder,root.transform,Vector3.zero,Vector3.one,material).transform;
        var head=new GameObject("Head",typeof(MeshFilter),typeof(MeshRenderer)).transform; head.SetParent(root.transform,false); head.GetComponent<MeshFilter>().sharedMesh=ConeMesh(); head.GetComponent<MeshRenderer>().sharedMaterial=material;
        var arrow=root.AddComponent<VectorArrowView>(); arrow.Bind(shaft,head); return arrow;
    }

    static void Grid(Transform parent)
    {
        var mat=Mat("Grid",new Color(.12f,.32f,.42f,.55f));
        for(int x=0;x<=90;x+=5){var l=Line("Grid X "+x,parent,mat.color,.015f,false);l.positionCount=2;l.SetPositions(new[]{new Vector3(x,.02f,-6),new Vector3(x,.02f,6)});}
        for(int z=-6;z<=6;z+=2){var l=Line("Grid Z "+z,parent,mat.color,.015f,false);l.positionCount=2;l.SetPositions(new[]{new Vector3(0,.02f,z),new Vector3(90,.02f,z)});}
    }

    static GameObject Primitive(string name,PrimitiveType type,Transform parent,Vector3 pos,Vector3 scale,Material material)
    { var go=GameObject.CreatePrimitive(type);go.name=name;go.transform.SetParent(parent);go.transform.localPosition=pos;go.transform.localScale=scale;go.GetComponent<Renderer>().sharedMaterial=material;return go; }
    static LineRenderer Line(string name,Transform parent,Color color,float width,bool local)
    { var go=new GameObject(name);go.transform.SetParent(parent,false);var l=go.AddComponent<LineRenderer>();l.useWorldSpace=!local;l.sharedMaterial=Mat("Line "+ColorUtility.ToHtmlStringRGBA(color),color);l.startColor=l.endColor=color;l.startWidth=l.endWidth=width;l.numCapVertices=4;return l; }
    static TMP_Text WorldText(string value,Transform parent,Vector3 pos,float size,Color color)
    {var go=new GameObject(value,typeof(TextMeshPro));go.transform.SetParent(parent,false);go.transform.localPosition=pos;go.transform.localRotation=Quaternion.Euler(0,180,0);var t=go.GetComponent<TextMeshPro>();t.text=value;t.fontSize=size;t.color=color;t.alignment=TextAlignmentOptions.Center;t.rectTransform.sizeDelta=new Vector2(4,1);return t;}
    static TMP_Text Text(string value,RectTransform parent,Vector2 pos,float size,Color color,TextAlignmentOptions alignment,Vector2 dimensions)
    {var go=new GameObject(value,typeof(RectTransform),typeof(TextMeshProUGUI));go.transform.SetParent(parent,false);var r=go.GetComponent<RectTransform>();r.anchoredPosition=pos;r.sizeDelta=dimensions;var t=go.GetComponent<TextMeshProUGUI>();t.text=value;t.fontSize=size;t.color=color;t.alignment=alignment;return t;}
    static GameObject UIRect(string name,RectTransform parent,Vector2 pos,Vector2 size,Color color)
    {var go=new GameObject(name,typeof(RectTransform),typeof(Image));go.transform.SetParent(parent,false);var r=go.GetComponent<RectTransform>();r.anchoredPosition=pos;r.sizeDelta=size;go.GetComponent<Image>().color=color;return go;}
    static void Stretch(RectTransform r,Vector2 min,Vector2 max,Vector2 offsetMin,Vector2 offsetMax){r.anchorMin=min;r.anchorMax=max;r.offsetMin=offsetMin;r.offsetMax=offsetMax;}
    static void FaceViewer(Transform spatialUi,Transform viewer)
    {
        var eyePosition=viewer.position+Vector3.up*1.6f;
        spatialUi.LookAt(eyePosition,Vector3.up);
        spatialUi.Rotate(0f,180f,0f,Space.Self);
    }
    static Material Mat(string name,Color color)
    {string path=MaterialPath+name+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);if(m==null){m=new Material(Shader.Find("Universal Render Pipeline/Unlit")){name=name,color=color};if(color.a<1){m.SetFloat("_Surface",1);m.renderQueue=3000;}AssetDatabase.CreateAsset(m,path);}return m;}
    static Mesh ConeMesh()
    {const int n=16;var v=new List<Vector3>{Vector3.zero,Vector3.up};var tris=new List<int>();for(int i=0;i<n;i++)v.Add(new Vector3(Mathf.Cos(i*Mathf.PI*2/n),0,Mathf.Sin(i*Mathf.PI*2/n)));for(int i=0;i<n;i++){int a=2+i,b=2+(i+1)%n;tris.Add(0);tris.Add(b);tris.Add(a);tris.Add(1);tris.Add(a);tris.Add(b);}var m=new Mesh{name="Vector Cone"};m.SetVertices(v);m.SetTriangles(tris,0);m.RecalculateNormals();return m;}
    static void SetObjectField(Object target,string name,Object value){var so=new SerializedObject(target);so.FindProperty(name).objectReferenceValue=value;so.ApplyModifiedPropertiesWithoutUndo();}
}
#endif
