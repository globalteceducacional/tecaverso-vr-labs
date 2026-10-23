using System.Linq;
using Tecaverso.Labs.Gravitation;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Object=UnityEngine.Object;

namespace Tecaverso.Editor
{
    /// <summary>Scene-local observatory finish. Never changes the shared room prefab or skybox.</summary>
    public static class ObservatoryEnvironmentAuthoring
    {
        const string Folder="Assets/_Project/Laboratories/Physics/Gravitation/Materials";
        static Material Material(string name,Color color,bool emissive=false)
        {
            string path=Folder+"/"+name+".mat";
            var m=AssetDatabase.LoadAssetAtPath<Material>(path);
            if(m==null){m=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(m,path);}
            m.SetColor("_BaseColor",color);m.SetFloat("_Smoothness",.22f);m.SetFloat("_Metallic",emissive?0:.25f);
            if(emissive){m.EnableKeyword("_EMISSION");m.SetColor("_EmissionColor",color*1.2f);}
            EditorUtility.SetDirty(m);return m;
        }
        static Transform Child(Transform parent,string name)
        {
            var t=parent.Find(name);if(t==null){t=new GameObject(name).transform;t.SetParent(parent,false);}return t;
        }
        static void Box(Transform root,string name,Vector3 at,Vector3 size,Material material)
        {
            var t=root.Find(name);
            if(t==null){var go=GameObject.CreatePrimitive(PrimitiveType.Cube);go.name=name;t=go.transform;t.SetParent(root,false);Object.DestroyImmediate(go.GetComponent<Collider>());}
            t.localPosition=at;t.localRotation=Quaternion.identity;t.localScale=size;
            var r=t.GetComponent<Renderer>();r.sharedMaterial=material;r.shadowCastingMode=ShadowCastingMode.Off;
        }
        static void Line(Transform root,string name,Vector3[] points,float width,Material material)
        {
            var t=Child(root,name);var line=t.GetComponent<LineRenderer>();if(line==null)line=t.gameObject.AddComponent<LineRenderer>();
            line.useWorldSpace=false;line.loop=false;line.positionCount=points.Length;line.SetPositions(points);
            line.startWidth=line.endWidth=width;line.numCornerVertices=3;line.numCapVertices=3;
            line.sharedMaterial=material;line.shadowCastingMode=ShadowCastingMode.Off;line.receiveShadows=false;
        }
        public static void Configure(GravitationLab lab)
        {
            if(Application.isPlaying)throw new System.InvalidOperationException("Apply environment in Edit Mode.");
            var room=lab.gameObject.scene.GetRootGameObjects().Single(g=>g.name=="TecaversoStandardRoom");
            var floor=Material("Observatory deck",new Color(.055f,.075f,.105f));
            var metal=Material("Observatory graphite",new Color(.11f,.15f,.2f));
            var edge=Material("Observatory guidance",new Color(.12f,.4f,.55f),true);
            var muted=Material("Observatory deck seams",new Color(.09f,.14f,.19f),true);
            // Retain the authored floor and boundary colliders. Only this prefab instance changes.
            foreach(var r in room.GetComponentsInChildren<Renderer>(true))
            {
                r.enabled=r.name=="Floor";
                if(r.name=="Floor")r.sharedMaterial=floor;
                PrefabUtility.RecordPrefabInstancePropertyModifications(r);
            }
            foreach(var light in room.GetComponentsInChildren<Light>(true))
            {light.enabled=false;PrefabUtility.RecordPrefabInstancePropertyModifications(light);}
            foreach(var t in lab.transform.Cast<Transform>().Where(t=>t.name=="Star observation window"||t.name=="Window frame"))
                t.gameObject.SetActive(false);
            lab.stationLabel.gameObject.SetActive(false);

            var root=Child(lab.transform,"Observatory architecture");
            // Boundaries match the existing 32 x 40 m floor and physical walls.
            for(int side=0;side<4;side++)
            {
                bool alongX=side<2;
                float fixedAxis=side==0?-15.3f:side==1?23.3f:side==2?-15.3f:15.3f;
                Vector3 at=alongX?new Vector3(0,.14f,fixedAxis):new Vector3(fixedAxis,.14f,4);
                Vector3 size=alongX?new Vector3(30.6f,.28f,.22f):new Vector3(.22f,.28f,38.6f);
                Box(root,"Perimeter base "+side,at,size,metal);
                at.y=1.05f;size.y=.10f;Box(root,"Handrail "+side,at,size,metal);
                at.y=1.115f;size.y=.025f;Box(root,"Handrail guide "+side,at,size,edge);
                at.y=.04f;
                if(alongX){at.z+=side==0?.32f:-.32f;size=new Vector3(30.3f,.018f,.045f);}
                else {at.x+=side==2?.32f:-.32f;size=new Vector3(.045f,.018f,38.3f);}
                Box(root,"Floor boundary light "+side,at,size,edge);
                for(int j=0;j<=8;j++)
                {
                    float a=Mathf.Lerp(alongX?-15.3f:-15.3f,alongX?15.3f:23.3f,j/8f);
                    var pos=alongX?new Vector3(a,.6f,fixedAxis):new Vector3(fixedAxis,.6f,a);
                    Box(root,"Railing post "+side+" "+j,pos,new Vector3(.10f,.9f,.10f),metal);
                }
            }
            // Sparse roof ribs suggest a dome while leaving the stellar background visible.
            for(int rib=0;rib<3;rib++)
            {
                var points=new Vector3[49];float z=-9+rib*13;
                for(int i=0;i<points.Length;i++)
                {float a=i*Mathf.PI/(points.Length-1);points[i]=new Vector3(-15.3f*Mathf.Cos(a),1.1f+6.6f*Mathf.Sin(a),z);}
                Line(root,"Open dome rib "+rib,points,.14f,metal);
                var inset=points.Select(v=>v+Vector3.down*.085f).ToArray();
                Line(root,"Dome accent "+rib,inset,.018f,edge);
            }
            // Sparse deck seams, subordinate to the orbital paths.
            for(int i=-3;i<=3;i++)
                Line(root,"Deck seam "+i,new[]{new Vector3(i*4,.012f,-15),new Vector3(i*4,.012f,23)},.008f,muted);
            RenderSettings.ambientMode=AmbientMode.Trilight;
            RenderSettings.ambientSkyColor=new Color(.32f,.35f,.4f);
            RenderSettings.ambientEquatorColor=new Color(.22f,.25f,.3f);
            RenderSettings.ambientGroundColor=new Color(.12f,.14f,.18f);
            RenderSettings.reflectionIntensity=.25f;
            RenderSettings.fog=false;
            var key=lab.gameObject.scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<Light>(true)).First(l=>l.type==LightType.Directional);
            key.enabled=true;key.color=new Color(1,.94f,.85f);key.intensity=1.05f;key.shadows=LightShadows.None;
            key.transform.rotation=Quaternion.Euler(35,-35,0);EditorUtility.SetDirty(key);
            var fill=Child(root,"Soft neutral fill");var fillLight=fill.GetComponent<Light>();if(fillLight==null)fillLight=fill.gameObject.AddComponent<Light>();
            fill.rotation=Quaternion.Euler(25,145,0);fillLight.type=LightType.Directional;
            fillLight.color=new Color(.75f,.85f,1);fillLight.intensity=.45f;fillLight.shadows=LightShadows.None;
            var source=Child(root,"Solar warm light");source.position=lab.solar.sun.position;
            var sunLight=source.GetComponent<Light>();if(sunLight==null)sunLight=source.gameObject.AddComponent<Light>();
            sunLight.type=LightType.Point;sunLight.range=32;sunLight.intensity=5;sunLight.color=new Color(1,.8f,.56f);sunLight.shadows=LightShadows.None;
            foreach(var camera in Object.FindObjectsByType<Camera>(FindObjectsInactive.Include,FindObjectsSortMode.None))
                if(camera.gameObject.scene==lab.gameObject.scene){camera.clearFlags=CameraClearFlags.Skybox;EditorUtility.SetDirty(camera);PrefabUtility.RecordPrefabInstancePropertyModifications(camera);}
            EditorUtility.SetDirty(lab);
            AssetDatabase.SaveAssets();
        }
    }
}
