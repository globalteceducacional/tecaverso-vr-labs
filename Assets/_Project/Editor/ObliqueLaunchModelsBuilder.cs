#if UNITY_EDITOR
using System.Collections.Generic;
using Tecaverso.Labs.ObliqueLaunch;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.ProBuilder;

public static class ObliqueLaunchModelsBuilder
{
    const string Folder="Assets/_Project/Laboratories/Physics/ObliqueLaunch/Models";
    static Material metal, pearl, blue, black;
    [MenuItem("Tecaverso/Create Cannon Models")]
    public static void Create()
    {
        if(Application.isPlaying) throw new System.InvalidOperationException("Exit Play Mode first.");
        if(GameObject.Find("Telescopic Base Model")!=null) throw new System.InvalidOperationException("Models already exist.");
        if(!AssetDatabase.IsValidFolder(Folder))AssetDatabase.CreateFolder("Assets/_Project/Laboratories/Physics/ObliqueLaunch","Models");
        metal=Mat("Titanium",new Color(.09f,.13f,.21f),.55f);
        pearl=Mat("Ceramic",new Color(.65f,.72f,.83f),.25f);
        black=Mat("Bore",new Color(.007f,.012f,.023f),.1f);
        blue=new Material(Shader.Find("Universal Render Pipeline/Unlit"));blue.color=new Color(.03f,.45f,1f);AssetDatabase.CreateAsset(blue,Folder+"/Ion Blue.mat");
        var lab=Object.FindFirstObjectByType<ObliqueLaunchLab>();
        var lift=new GameObject("Telescopic Base Model");lift.transform.SetParent(lab.transform,false);
        Ring("Foundation",lift.transform,.72f,.78f,0,.09f,metal);
        Ring("Foundation luminous rim",lift.transform,.74f,.77f,.075f,.095f,blue);
        var stages=new Transform[3];var collars=new Transform[3];
        for(int i=0;i<3;i++)
        {
            float radius=.56f-i*.1f;
            var stage=new GameObject("Lift Stage "+(i+1));stage.transform.SetParent(lift.transform,false);stages[i]=stage.transform;
            Ring("Telescopic sleeve",stage.transform,0,radius,0,1,i==1?metal:pearl);
            for(int side=0;side<4;side++)
            {
                float angle=side*Mathf.PI*.5f;
                var rib=Box("Guide rail",stage.transform,new Vector3(Mathf.Sin(angle)*radius,.5f,Mathf.Cos(angle)*radius),new Vector3(.07f,1,.045f),metal);
                rib.localRotation=Quaternion.Euler(0,side*90,0);
                var strip=Box("Blue status strip",stage.transform,new Vector3(Mathf.Sin(angle)*(radius+.025f),.5f,Mathf.Cos(angle)*(radius+.025f)),new Vector3(.024f,.83f,.012f),blue);
                strip.localRotation=Quaternion.Euler(0,side*90,0);
            }
            var collar=new GameObject("Stage collar "+i);collar.transform.SetParent(lift.transform,false);collars[i]=collar.transform;
            Ring("Seal",collar.transform,0,radius+.045f,0,.075f,metal);
            Ring("Blue seal band",collar.transform,radius+.037f,radius+.047f,.03f,.047f,blue);
        }
        var top=new GameObject("Elevator platform");top.transform.SetParent(lift.transform,false);
        Ring("Platform",top.transform,0,.62f,-.05f,.06f,metal);
        Ring("Platform trim",top.transform,.59f,.625f,.035f,.065f,blue);
        for(int i=0;i<8;i++)
        {float a=i*Mathf.PI/4;Box("Anchor bolt",lift.transform,new Vector3(Mathf.Cos(a)*.67f,.095f,Mathf.Sin(a)*.67f),Vector3.one*.06f,pearl);}
        var liftView=lift.AddComponent<TelescopicLaunchBase>();liftView.Bind(stages,collars,top.transform);
        liftView.SetHeight(Object.FindFirstObjectByType<ObliqueLaunchPanel>().Parameters.Height);

        var pivot=GameObject.Find("Cannon Pivot").transform;
        var cannon=new GameObject("Cannon Model");cannon.transform.SetParent(pivot,false);
        Ring("Breech housing",cannon.transform,0,.28f,0,.4f,metal);
        Ring("Ceramic barrel jacket",cannon.transform,.17f,.235f,.32f,1.35f,pearl);
        Ring("Inner barrel",cannon.transform,.14f,.175f,.34f,1.66f,metal);
        Ring("Muzzle collar",cannon.transform,.14f,.29f,1.38f,1.62f,metal);
        Ring("Muzzle light rim",cannon.transform,.24f,.292f,1.53f,1.57f,blue);
        Ring("Muzzle bevel",cannon.transform,.14f,.25f,1.62f,1.7f,pearl);
        Ring("Dark bore end",cannon.transform,0,.14f,.4f,.41f,black);
        for(int i=0;i<4;i++)
        {
            float a=i*Mathf.PI*.5f;
            var rail=Box("Barrel rail",cannon.transform,new Vector3(Mathf.Sin(a)*.232f,.87f,Mathf.Cos(a)*.232f),new Vector3(.08f,.94f,.065f),metal);rail.localRotation=Quaternion.Euler(0,i*90,0);
            var strip=Box("Barrel blue channel",cannon.transform,new Vector3(Mathf.Sin(a)*.269f,.87f,Mathf.Cos(a)*.269f),new Vector3(.028f,.73f,.012f),blue);strip.localRotation=rail.localRotation;
        }
        for(int side=-1;side<=1;side+=2)
        {
            var cap=new GameObject("Trunnion");cap.transform.SetParent(cannon.transform,false);cap.transform.localPosition=new Vector3(0,.17f,side*.28f);cap.transform.localRotation=Quaternion.Euler(side*90,0,0);
            Ring("Axle cap",cap.transform,0,.19f,0,.07f,metal);
            Ring("Axle luminous ring",cap.transform,.11f,.14f,0,.075f,blue);
        }
        var feedback=cannon.AddComponent<CannonTweenFeedback>();
        PrefabUtility.SaveAsPrefabAsset(lift,Folder+"/TelescopicBase.prefab");
        PrefabUtility.SaveAsPrefabAsset(cannon,Folder+"/Cannon.prefab");
        feedback.Bind(lab);EditorUtility.SetDirty(feedback);
        var so=new SerializedObject(lab);so.FindProperty("telescopicBase").objectReferenceValue=liftView;so.ApplyModifiedPropertiesWithoutUndo();
        // Keep original transforms as references, but replace their primitive visuals.
        foreach(var name in new[]{"Adjustable Base","Cannon Barrel"})
        {
            var old=GameObject.Find(name);old.GetComponent<Renderer>().enabled=false;
            var collider=old.GetComponent<Collider>();if(collider!=null)collider.enabled=false;
            var oldFeedback=old.GetComponent<CannonTweenFeedback>();if(oldFeedback!=null)oldFeedback.enabled=false;
        }
        EditorSceneManager.MarkSceneDirty(lab.gameObject.scene);EditorSceneManager.SaveScene(lab.gameObject.scene);AssetDatabase.SaveAssets();
    }
    static Material Mat(string name,Color color,float metallic)
    {var m=new Material(Shader.Find("Universal Render Pipeline/Lit"));m.color=color;m.SetFloat("_Metallic",metallic);m.SetFloat("_Smoothness",.45f);AssetDatabase.CreateAsset(m,Folder+"/"+name+".mat");return m;}
    static Transform Box(string name,Transform parent,Vector3 position,Vector3 scale,Material mat)
    {var b=ShapeGenerator.GenerateCube(PivotLocation.Center,scale);b.name=name;b.transform.SetParent(parent,false);b.transform.localPosition=position;b.GetComponent<Renderer>().sharedMaterial=mat;var c=b.GetComponent<Collider>();if(c!=null)Object.DestroyImmediate(c);return b.transform;}
    static int meshIndex;
    static void Ring(string name,Transform parent,float inner,float outer,float bottom,float top,Material mat)
    {
        const int sides=32;var v=new List<Vector3>();var t=new List<int>();
        var profile=new[]{new Vector2(inner,bottom),new Vector2(outer,bottom),new Vector2(outer,top),new Vector2(inner,top),new Vector2(inner,bottom)};
        for(int p=0;p<4;p++)for(int i=0;i<sides;i++)
        {
            int start=v.Count;float a=i*Mathf.PI*2/sides,b=(i+1)*Mathf.PI*2/sides;
            foreach(var pair in new[]{new Vector3(profile[p].x*Mathf.Cos(a),profile[p].y,profile[p].x*Mathf.Sin(a)),new Vector3(profile[p].x*Mathf.Cos(b),profile[p].y,profile[p].x*Mathf.Sin(b)),new Vector3(profile[p+1].x*Mathf.Cos(b),profile[p+1].y,profile[p+1].x*Mathf.Sin(b)),new Vector3(profile[p+1].x*Mathf.Cos(a),profile[p+1].y,profile[p+1].x*Mathf.Sin(a))})v.Add(pair);
            t.AddRange(new[]{start,start+2,start+1,start,start+3,start+2});
        }
        var mesh=new Mesh{name=name};mesh.SetVertices(v);mesh.SetTriangles(t,0);mesh.RecalculateNormals();mesh.RecalculateBounds();
        AssetDatabase.CreateAsset(mesh,Folder+"/Part"+(meshIndex++)+".asset");
        var go=new GameObject(name,typeof(MeshFilter),typeof(MeshRenderer));go.transform.SetParent(parent,false);go.GetComponent<MeshFilter>().sharedMesh=mesh;go.GetComponent<MeshRenderer>().sharedMaterial=mat;
    }
}
#endif
