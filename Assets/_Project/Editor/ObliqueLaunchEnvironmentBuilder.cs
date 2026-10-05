#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;

// Environment-only authoring tool: never rebuilds the experiment or its UI.
public static class ObliqueLaunchEnvironmentBuilder
{
    const string RootName = "Tecaverso Laboratory Environment";
    const string Folder = "Assets/_Project/Laboratories/Physics/ObliqueLaunch/Materials/Environment";
    static Transform root;

    [MenuItem("Tecaverso/Create Laboratory Environment")]
    public static void Create()
    {
        if(Application.isPlaying) throw new System.InvalidOperationException("Exit Play Mode first.");
        if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name!="ObliqueLaunch")
            throw new System.InvalidOperationException("Open ObliqueLaunch first.");
        if(GameObject.Find(RootName)!=null) throw new System.InvalidOperationException("Environment already exists; edit it in the hierarchy.");
        if(!AssetDatabase.IsValidFolder(Folder)) AssetDatabase.CreateFolder("Assets/_Project/Laboratories/Physics/ObliqueLaunch/Materials","Environment");
        var go=new GameObject(RootName); Undo.RegisterCreatedObjectUndo(go,"Create laboratory environment"); root=go.transform;
        var wall=Material("Pearl Panels",new Color(.57f,.62f,.74f),false,.22f);
        var dark=Material("Structural Graphite",new Color(.065f,.095f,.17f),false,.35f);
        var plinth=Material("Blue Wall Base",new Color(.105f,.16f,.29f),false,.4f);
        var floor=Material("Midnight Floor",new Color(.025f,.05f,.12f),false,.7f);
        floor.SetFloat("_EnvironmentReflections",0f); floor.EnableKeyword("_ENVIRONMENTREFLECTIONS_OFF");
        floor.SetFloat("_SpecularHighlights",0f); floor.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
        floor.shader=Shader.Find("Tecaverso/Laboratory Floor");
        floor.SetColor("_BaseColor",new Color(.015f,.03f,.085f));
        floor.SetColor("_GridColor",new Color(.04f,.25f,.65f)); floor.SetFloat("_CellSize",2f);
        var blue=Material("Electric Blue",new Color(.025f,.3f,1f)*1.6f,true);
        var white=Material("White Light",new Color(.75f,.87f,1f)*1.6f,true);
        var grid=Material("Floor Grid",new Color(.035f,.16f,.36f),true);

        // Clear envelope for v0 <= 30, g >= 5, launch height <= 10.
        Box("Floor",new Vector3(95,-.15f,0),new Vector3(210,.3f,28),floor,true);
        Box("Back upper wall",new Vector3(95,57,14),new Vector3(210,102,.3f),wall,true);
        Box("Front upper wall",new Vector3(95,57,-14),new Vector3(210,102,.3f),wall,true);
        Box("Far end",new Vector3(200,54,0),new Vector3(.3f,108,28),wall,true);
        Box("Left wall",new Vector3(-10,54,0),new Vector3(.3f,108,28),wall,true);
        Box("High ceiling",new Vector3(95,108,0),new Vector3(210,.3f,28),wall);
        foreach(float z in new[]{-13.8f,13.8f})
        {
            for(int x=-10;x<200;x+=10)
            {
                Box("Wall panel",new Vector3(x+5,4.65f,z),new Vector3(9.88f,6.7f,.22f),wall);
                Box("Wall skirting",new Vector3(x+5,.65f,z-.02f*Mathf.Sign(z)),new Vector3(9.88f,1.3f,.32f),plinth);
                Box("Panel seam",new Vector3(x,4.7f,z),new Vector3(.05f,7f,.25f),dark);
                if(x%20==0) Box("Wall pier",new Vector3(x,4.5f,z-.12f*Mathf.Sign(z)),new Vector3(.35f,9f,.45f),wall);
                Box("Luminaire housing",new Vector3(x+5,7.75f,z-.28f*Mathf.Sign(z)),new Vector3(1.3f,.14f,.35f),dark);
                Box("Luminaire",new Vector3(x+5,7.67f,z-.3f*Mathf.Sign(z)),new Vector3(1.12f,.06f,.32f),white);
            }
            Box("Upper graphite rail",new Vector3(95,8.5f,z),new Vector3(210,.7f,.45f),dark);
            Box("Upper blue strip",new Vector3(95,8.5f,z-.25f*Mathf.Sign(z)),new Vector3(210,.14f,.06f),blue);
            Box("Lower blue strip",new Vector3(95,.12f,z-.2f*Mathf.Sign(z)),new Vector3(210,.045f,.05f),blue);
        }
        // Return the reference's panel treatment around the near end.
        for(int z=-12;z<=12;z+=4)
        {
            Box("End panel",new Vector3(-9.8f,4.65f,z),new Vector3(.22f,6.7f,3.88f),wall);
            Box("End skirting",new Vector3(-9.7f,.65f,z),new Vector3(.3f,1.3f,3.88f),plinth);
        }
        Box("End upper rail",new Vector3(-9.6f,8.5f,0),new Vector3(.4f,.7f,28),dark);
        Box("End blue strip",new Vector3(-9.36f,8.5f,0),new Vector3(.06f,.14f,28),blue);
        Box("End lower strip",new Vector3(-9.48f,.12f,0),new Vector3(.06f,.045f,28),blue);
        // A low canopy above the observation aisle, outside the flight plane Z=0.
        Box("Observation canopy",new Vector3(95,9,-9),new Vector3(210,.2f,10),wall);
        Box("Canopy edge",new Vector3(95,8.9f,-4),new Vector3(210,.3f,.3f),dark);
        Box("Canopy light",new Vector3(95,8.72f,-4),new Vector3(210,.06f,.13f),blue);
        // Grid is anti-aliased in the floor shader, without overlapping geometry.
        for(int x=-5;x<=25;x+=10)
        {
            var lamp=new GameObject("Wall wash"); lamp.transform.SetParent(root);
            lamp.transform.position=new Vector3(x,7.4f,12.5f); lamp.transform.rotation=Quaternion.Euler(35,0,0);
            var light=lamp.AddComponent<Light>(); light.type=LightType.Spot; light.color=new Color(.72f,.8f,1f);
            light.intensity=4; light.range=14; light.spotAngle=95; light.shadows=LightShadows.None;
        }
        RenderSettings.ambientMode=AmbientMode.Trilight;
        RenderSettings.ambientSkyColor=new Color(.57f,.65f,.83f);
        RenderSettings.ambientEquatorColor=new Color(.32f,.39f,.55f);
        RenderSettings.ambientGroundColor=new Color(.11f,.15f,.24f);
        // Keep prior environment recoverable and exclude only its visuals/collision.
        foreach(var name in new[]{"Room","Ground"})
        { var old=GameObject.Find(name); if(old!=null){Undo.RecordObject(old,"Replace environment"); old.SetActive(false);} }
        var lab=GameObject.Find("Oblique Launch Lab");
        foreach(Transform child in lab.transform)
            if(child.name.StartsWith("Grid X ")||child.name.StartsWith("Grid Z ")) {Undo.RecordObject(child.gameObject,"Replace grid");child.gameObject.SetActive(false);}
        EditorSceneManager.MarkSceneDirty(go.scene); EditorSceneManager.SaveScene(go.scene); AssetDatabase.SaveAssets();
    }

    static void Box(string name,Vector3 position,Vector3 size,Material material,bool collider=false)
    {
        var mesh=ShapeGenerator.GenerateCube(PivotLocation.Center,size);
        mesh.name=name; mesh.transform.SetParent(root,false); mesh.transform.position=position;
        mesh.GetComponent<MeshRenderer>().sharedMaterial=material;
        var col=mesh.GetComponent<Collider>(); if(col!=null && !collider) Object.DestroyImmediate(col);
        if(collider && col==null) mesh.gameObject.AddComponent<BoxCollider>();
        mesh.gameObject.isStatic=true;
    }
    static Material Material(string name,Color color,bool unlit,float smoothness=0)
    {
        string path=Folder+"/"+name+".mat";
        var mat=AssetDatabase.LoadAssetAtPath<Material>(path);
        if(mat==null){mat=new Material(Shader.Find(unlit?"Universal Render Pipeline/Unlit":"Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(mat,path);}
        mat.color=color; if(!unlit) mat.SetFloat("_Smoothness",smoothness); EditorUtility.SetDirty(mat); return mat;
    }
    static void Grid(Material material)
    {
        var vertices=new List<Vector3>(); var triangles=new List<int>();
        void Strip(float x,float z,float width,float depth)
        {
            int i=vertices.Count;
            vertices.Add(new Vector3(x,.03f,z));vertices.Add(new Vector3(x+width,.03f,z));
            vertices.Add(new Vector3(x+width,.03f,z+depth));vertices.Add(new Vector3(x,.03f,z+depth));
            triangles.AddRange(new[]{i,i+2,i+1,i,i+3,i+2});
        }
        for(int x=-10;x<=200;x+=2) Strip(x,-14,.025f,28);
        for(int z=-14;z<=14;z+=2) Strip(-10,z,210,.025f);
        var mesh=new Mesh{name="Laboratory Grid"};mesh.SetVertices(vertices);mesh.SetTriangles(triangles,0);mesh.RecalculateNormals();
        AssetDatabase.CreateAsset(mesh,Folder+"/LaboratoryGrid.asset");
        var go=new GameObject("Floor grid",typeof(MeshFilter),typeof(MeshRenderer));go.transform.SetParent(root,false);
        go.GetComponent<MeshFilter>().sharedMesh=mesh;go.GetComponent<MeshRenderer>().sharedMaterial=material;go.isStatic=true;
    }
}
#endif
