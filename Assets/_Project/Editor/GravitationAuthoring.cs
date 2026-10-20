using System;
using System.Linq;
using Tecaverso.Hub;
using Tecaverso.Labs.Gravitation;
using Tecaverso.Labs.InclinedPlane;
using Tecaverso.Labs.ObliqueLaunch;
using Tecaverso.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object=UnityEngine.Object;

namespace Tecaverso.Editor
{
    /// <summary>One-time installation of a separate lab; refuses to overwrite an authored scene.</summary>
    public static class GravitationAuthoring
    {
        public const string Folder="Assets/_Project/Laboratories/Physics/Gravitation";
        public const string ScenePath=Folder+"/Scenes/Gravitation.unity";
        const string Incline="Assets/_Project/Laboratories/Physics/InclinedPlane";
        static TMP_FontAsset font;
        static T Ref<T>(Object o,string field) where T:Object => (T)new SerializedObject(o).FindProperty(field).objectReferenceValue;
        static T[] Refs<T>(Object o,string field) where T:Object
        {var p=new SerializedObject(o).FindProperty(field);return Enumerable.Range(0,p.arraySize).Select(i=>(T)p.GetArrayElementAtIndex(i).objectReferenceValue).ToArray();}
        static void Ensure(string folder){if(!AssetDatabase.IsValidFolder(folder)){int slash=folder.LastIndexOf('/');Ensure(folder.Substring(0,slash));AssetDatabase.CreateFolder(folder.Substring(0,slash),folder.Substring(slash+1));}}
        static Material Env(string name)=>AssetDatabase.LoadAssetAtPath<Material>("Assets/_Project/Laboratories/Physics/ObliqueLaunch/Materials/Environment/"+name+".mat");
        static Material Mat(string name,Color color,string texture=null,bool unlit=false)
        {
            var mat=new Material(Shader.Find(unlit?"Universal Render Pipeline/Unlit":"Universal Render Pipeline/Lit")){name=name,enableInstancing=true};
            mat.SetColor("_BaseColor",color);
            if(texture!=null)mat.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(Folder+"/Textures/"+texture));
            if(!unlit)mat.SetFloat("_Smoothness",.15f);
            AssetDatabase.CreateAsset(mat,Folder+"/Materials/"+name+".mat");return mat;
        }
        static Transform Shape(string name,Transform parent,PrimitiveType type,Vector3 p,Vector3 scale,Material material,bool collider=false)
        {
            var go=GameObject.CreatePrimitive(type);go.name=name;go.transform.SetParent(parent,false);go.transform.localPosition=p;go.transform.localScale=scale;
            go.GetComponent<Renderer>().sharedMaterial=material;
            if(!collider)Object.DestroyImmediate(go.GetComponent<Collider>());
            return go.transform;
        }
        static Transform Group(string name,Transform parent,Vector3 p)
        {var t=new GameObject(name).transform;t.SetParent(parent,false);t.localPosition=p;return t;}
        static TMP_Text Label(string name,Transform parent,string text,Vector3 p,float size,float width,Color color)
        {
            var t=new GameObject(name,typeof(TextMeshPro)).GetComponent<TextMeshPro>();
            t.transform.SetParent(parent,false);t.transform.localPosition=p;t.font=font;t.fontSize=size;t.color=color;t.text=text;
            t.rectTransform.sizeDelta=new Vector2(width,.3f);t.alignment=TextAlignmentOptions.Center;t.raycastTarget=false;
            t.textWrappingMode=TextWrappingModes.NoWrap;t.overflowMode=TextOverflowModes.Truncate;return t;
        }
        static LineRenderer Line(string name,Transform parent,Material mat,float width,bool loop=false)
        {
            var line=new GameObject(name,typeof(LineRenderer)).GetComponent<LineRenderer>();line.transform.SetParent(parent,false);
            line.useWorldSpace=false;line.sharedMaterial=mat;line.startWidth=line.endWidth=width;line.loop=loop;
            line.numCapVertices=4;line.numCornerVertices=2;line.positionCount=0;line.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;line.receiveShadows=false;return line;
        }
        static void Caption(Button button,string text){button.name=text;button.GetComponentInChildren<TMP_Text>(true).text=text;}
        static void Clear(Button button)=>button.onClick=new Button.ButtonClickedEvent();
        static GravitationPanel Panel(Transform parent,InclinedPlanePanel source)
        {
            var copy=Object.Instantiate(source.gameObject,parent);copy.name="Gravitation Controls";
            copy.transform.SetPositionAndRotation(source.transform.position,source.transform.rotation);copy.transform.localScale=source.transform.lossyScale;
            var old=copy.GetComponent<InclinedPlanePanel>();
            var sliders=Refs<Slider>(old,"controls");var values=Refs<TMP_Text>(old,"values");var bodies=Refs<Button>(old,"presets");
            var actions=new[]{"release","pause","reset","parametersTab","analysisTab","back","confirmBack","cancelBack"}.Select(f=>Ref<Button>(old,f)).ToArray();
            var options=new[]{"vectors","resultant","components"}.Select(f=>Ref<Toggle>(old,f)).ToArray();
            var pages=new[]{"parametersPage","analysisPage","leaveDialog","pages"}.Select(f=>Ref<GameObject>(old,f)).ToArray();
            var status=Ref<TMP_Text>(old,"status");var metrics=Ref<TMP_Text>(old,"metrics");var analysis=Ref<TMP_Text>(old,"analysis");var scale=Ref<TMP_Text>(old,"scaleLabel");
            Object.DestroyImmediate(old);
            var modeRow=Object.Instantiate(bodies[0].transform.parent.gameObject,bodies[0].transform.parent.parent);
            modeRow.name="Experiment modes";modeRow.transform.SetSiblingIndex(2);
            var modes=modeRow.GetComponentsInChildren<Button>(true);
            string[] modeNames={"ATRAÇÃO","SATÉLITE","SISTEMA SOLAR"},bodyNames={"TERRA","LUA","MARTE"};
            for(int i=0;i<3;i++){Clear(modes[i]);Clear(bodies[i]);Caption(modes[i],modeNames[i]);Caption(bodies[i],bodyNames[i]);}
            foreach(var action in actions)Clear(action);
            string[] fieldNames={"ALTITUDE","VELOCIDADE INICIAL","MASSA DO SATÉLITE","MASSA DO ASTRO","ESCALA DE TEMPO"};
            float[] min={.1f,0,100,.25f,1},max={3,1.8f,2000,2,1000},initial={.5f,1,500,1,300};
            var captions=new TMP_Text[5];
            for(int i=0;i<5;i++)
            {
                var slider=sliders[i];slider.onValueChanged=new Slider.SliderEvent();slider.wholeNumbers=i==2||i==4;
                slider.minValue=min[i];slider.maxValue=max[i];slider.SetValueWithoutNotify(initial[i]);
                captions[i]=slider.transform.parent.GetComponentsInChildren<TMP_Text>(true).First(t=>t!=values[i]);
                captions[i].text=fieldNames[i];slider.name=fieldNames[i];
            }
            string[] optionNames={"FORÇA","VELOCIDADE","TRAJETÓRIAS"};
            for(int i=0;i<3;i++){options[i].onValueChanged=new Toggle.ToggleEvent();options[i].SetIsOnWithoutNotify(true);options[i].GetComponentInChildren<TMP_Text>(true).text=optionNames[i];}
            foreach(var text in copy.GetComponentsInChildren<TMP_Text>(true))
            {
                if(text.text=="PLANO INCLINADO")text.text="GRAVITAÇÃO";
                if(text.text.StartsWith("Libere a caixa"))text.text="Coloque um satélite em órbita.\nTrocar modo ou astro reinicia a tentativa.";
                if(text.text=="FORÇAS NO BLOCO")text.text="ANÁLISE GRAVITACIONAL";
            }
            var details=pages[1].transform.Find("Analysis content").GetComponentsInChildren<TMP_Text>(true);
            var formula=details.First(t=>t.text.StartsWith("P = mg"));
            var explanation=details.First(t=>t.text.StartsWith("AMARELO"));
            var panel=copy.AddComponent<GravitationPanel>();
            panel.controls=sliders;panel.values=values;panel.captions=captions;panel.modes=modes;panel.bodies=bodies;panel.actions=actions;panel.options=options;panel.pages=pages;
            panel.status=status;panel.metrics=metrics;panel.analysis=analysis;panel.scale=scale;panel.formula=formula;panel.explanation=explanation;
            panel.launchLabel=actions[0].GetComponentInChildren<TMP_Text>(true);panel.pauseLabel=actions[1].GetComponentInChildren<TMP_Text>(true);
            UIVisibility.Set(pages[0],true);UIVisibility.Set(pages[1],false);UIVisibility.Set(pages[2],false);UIVisibility.SetInteraction(pages[3],true);
            actions[3].interactable=false;actions[4].interactable=true;
            return panel;
        }
        public static string Create()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Exit Play Mode first.");
            if(AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath)!=null)throw new InvalidOperationException("Scene already exists; edit it instead of regenerating.");
            for(int i=0;i<SceneManager.sceneCount;i++)if(SceneManager.GetSceneAt(i).isDirty)throw new InvalidOperationException("An open scene has unsaved authored changes.");
            foreach(var dir in new[]{"Scenes","Materials","Prefabs","Meshes"})Ensure(Folder+"/"+dir);
            foreach(var path in AssetDatabase.FindAssets("t:Texture2D",new[]{Folder+"/Textures"}).Select(AssetDatabase.GUIDToAssetPath))
            {
                var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.maxTextureSize=2048;importer.mipmapEnabled=true;
                importer.wrapMode=TextureWrapMode.Repeat;importer.filterMode=FilterMode.Trilinear;importer.anisoLevel=2;
                importer.textureCompression=TextureImporterCompression.Compressed;importer.SaveAndReimport();
            }
            font=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Bold SDF.asset");
            var source=EditorSceneManager.OpenScene(Incline+"/Scenes/InclinedPlane.unity",OpenSceneMode.Additive);
            var sourcePanel=source.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<InclinedPlanePanel>(true)).Single();
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);SceneManager.SetActiveScene(scene);
            foreach(string name in new[]{"XR Origin (XR Rig)","XR Interaction Manager","EventSystem","Directional Light"})
            {var copy=Object.Instantiate(source.GetRootGameObjects().Single(r=>r.name==name));copy.name=name;}
            var room=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Environments/StandardRoom/TecaversoStandardRoom.prefab"),scene);
            room.transform.localScale=new Vector3(1,1.2f,1);PrefabUtility.RecordPrefabInstancePropertyModifications(room.transform);
            var root=new GameObject("Gravitation Lab").transform;
            var graphite=Env("Structural Graphite");var blue=Env("Blue Wall Base");var pearl=Env("Pearl Panels");
            var station=Group("Orbital station",root,new Vector3(2.6f,3.8f,1.5f));
            Shape("Observation platform",root,PrimitiveType.Cylinder,new Vector3(2.6f,.18f,1.5f),new Vector3(7,.18f,3),graphite,true);
            Shape("Projection pedestal",root,PrimitiveType.Cylinder,new Vector3(2.6f,.65f,2.6f),new Vector3(1.4f,.45f,1.4f),pearl,true);
            Shape("Projector rim",root,PrimitiveType.Cylinder,new Vector3(2.6f,1.12f,2.6f),new Vector3(1.5f,.035f,1.5f),blue);
            var stars=Mat("Observation stars",new Color(.55f,.55f,.65f),"2k_stars.jpg",true);
            Shape("Star observation window",root,PrimitiveType.Quad,new Vector3(0,4.2f,6.3f),new Vector3(12.8f,7.6f,1),stars);
            foreach(float x in new[]{-6.45f,6.45f})Shape("Window frame",root,PrimitiveType.Cube,new Vector3(x,4.2f,6.2f),new Vector3(.1f,7.8f,.1f),graphite);
            foreach(float y in new[]{.35f,8.05f})Shape("Window frame",root,PrimitiveType.Cube,new Vector3(0,y,6.2f),new Vector3(13,.1f,.1f),graphite);
            var orbital=Group("Satellite experiment",station,Vector3.zero);
            string[] textureNames={"mercury","venus_atmosphere","earth_daymap","mars","jupiter","saturn","uranus","neptune"};
            var planetMats=textureNames.Select(n=>Mat(n,Color.white,"2k_"+n+".jpg")).ToArray();
            var moon=Mat("Moon",Color.white,"2k_moon.jpg");
            var planet=Shape("Central body",orbital,PrimitiveType.Sphere,Vector3.zero,Vector3.one*GravitationLab.PlanetRadius*2,planetMats[2]);
            planet.localRotation=Quaternion.Euler(0,220,0);
            var sat=Group("Satellite",orbital,Vector3.zero);
            Shape("Service module",sat,PrimitiveType.Cube,Vector3.zero,new Vector3(.13f,.19f,.13f),pearl);
            Shape("Antenna",sat,PrimitiveType.Cylinder,new Vector3(0,.15f,0),new Vector3(.015f,.065f,.015f),graphite);
            foreach(float side in new[]{-1f,1f})
            {
                Shape("Array spar",sat,PrimitiveType.Cube,new Vector3(side*.18f,0,0),new Vector3(.30f,.015f,.015f),graphite);
                Shape("Solar array",sat,PrimitiveType.Cube,new Vector3(side*.26f,0,0),new Vector3(.25f,.28f,.025f),blue);
                for(int j=0;j<3;j++)Shape("Cell divider",sat,PrimitiveType.Cube,new Vector3(side*.26f,(j-1)*.085f,-.015f),new Vector3(.24f,.006f,.006f),pearl);
            }
            var satellitePrefab=PrefabUtility.SaveAsPrefabAsset(sat.gameObject,Folder+"/Prefabs/ObservationSatellite.prefab");
            Object.DestroyImmediate(sat.gameObject);sat=((GameObject)PrefabUtility.InstantiatePrefab(satellitePrefab,orbital)).transform;
            var yellow=Mat("Gravitational force",new Color32(255,208,70,255),null,true);
            var velocity=Mat("Velocity",new Color32(90,160,255,255),null,true);
            var pathMat=Mat("Satellite trajectory",new Color32(62,211,156,255),null,true);
            var vectorRoot=Group("Satellite vectors",orbital,Vector3.zero);
            var arrows=new VectorArrowView[2];
            for(int i=0;i<2;i++)
            {
                var go=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Incline+"/Prefabs/ForceArrow.prefab"),vectorRoot);
                go.name=i==0?"Gravity":"Velocity";arrows[i]=go.GetComponent<VectorArrowView>();
                foreach(var r in go.GetComponentsInChildren<Renderer>(true)){r.sharedMaterial=i==0?yellow:velocity;PrefabUtility.RecordPrefabInstancePropertyModifications(r);}
            }
            var forceLabel=Label("Force value",vectorRoot,"F",Vector3.zero,1,1.5f,new Color32(255,208,70,255));
            var speedLabel=Label("Velocity value",vectorRoot,"v",Vector3.zero,1,1.5f,new Color32(90,160,255,255));
            var trail=Line("Satellite trajectory",orbital,pathMat,.014f);
            var overview=Group("Solar System",station,Vector3.zero);
            Shape("Sun",overview,PrimitiveType.Sphere,Vector3.zero,Vector3.one*.48f,Mat("Sun",Color.white,"2k_sun.jpg",true));
            var solar=overview.gameObject.AddComponent<SolarSystemView>();
            solar.planets=new Transform[8];solar.labels=new TMP_Text[8];solar.orbits=new LineRenderer[8];
            float[] sizes={.10f,.14f,.15f,.12f,.32f,.27f,.21f,.20f};
            for(int i=0;i<8;i++)
            {
                var data=PlanetOrbit.Planets[i];
                solar.planets[i]=Shape(data.Name,overview,PrimitiveType.Sphere,Vector3.zero,Vector3.one*sizes[i],planetMats[i]);
                var line=Line(data.Name+" orbit",overview,Mat("Orbit "+i,Color.Lerp(new Color(.25f,.45f,.65f),new Color(.55f,.75f,.85f),i/7f),null,true),.009f,true);
                line.positionCount=160;
                for(int j=0;j<160;j++){var p=data.EllipseAt(j*2*Math.PI/160);line.SetPosition(j,new Vector3((float)p.X,(float)p.Y,.025f)*(SolarSystemView.DisplaySemiMajor(i)/(float)data.SemiMajorAU));}
                solar.orbits[i]=line;solar.labels[i]=Label(data.Name+" label",overview,data.Name,Vector3.zero,1,1.2f,new Color(.8f,.88f,1));
            }
            SaturnRing(solar.planets[5]);
            var panel=Panel(root,sourcePanel);
            var lab=root.gameObject.AddComponent<GravitationLab>();
            lab.panel=panel;lab.orbitalRoot=orbital;lab.planet=planet;lab.planetRenderer=planet.GetComponent<Renderer>();lab.satellite=sat;
            lab.bodyMaterials=new[]{planetMats[2],moon,planetMats[3]};lab.vectorAnchor=vectorRoot;lab.forceArrow=arrows[0];lab.velocityArrow=arrows[1];
            lab.forceLabel=forceLabel;lab.velocityLabel=speedLabel;lab.trajectory=trail;lab.solar=solar;
            lab.stationLabel=Label("Station title",root,"GRAVITAÇÃO",new Vector3(2.6f,7.4f,1.5f),1.5f,7,new Color(.8f,.88f,1));
            Label("Scale reminder",root,"MODELO DIDÁTICO · ESCALAS NA ABA ANÁLISE",new Vector3(2.6f,.40f,-.1f),1,6,new Color(.8f,.88f,1));
            GravitationPresentation.Configure(lab);
            lab.ResetAttempt();
            Canvas.ForceUpdateCanvases();
            foreach(var layout in panel.GetComponentsInChildren<LayoutGroup>(true).Reverse())LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)layout.transform);
            EditorSceneManager.SaveScene(scene,ScenePath);
            var catalog=AssetDatabase.LoadAssetAtPath<HubCatalog>("Assets/_Project/Hub/HubCatalog.asset");
            var entry=catalog.disciplines.First(d=>d.title=="Física").contents.Single(c=>c.title=="Gravitação");
            entry.scenePath=ScenePath;entry.description="Investigue a atração gravitacional, lance um satélite e observe as órbitas dos oito planetas. Exploração individual.";
            entry.objective="Relacionar massa, distância, força gravitacional e movimento orbital.";
            EditorUtility.SetDirty(catalog);AssetDatabase.SaveAssetIfDirty(catalog);
            var scenes=EditorBuildSettings.scenes.ToList();if(!scenes.Any(s=>s.path==ScenePath)){scenes.Add(new EditorBuildSettingsScene(ScenePath,true));EditorBuildSettings.scenes=scenes.ToArray();}
            EditorSceneManager.CloseScene(source,true);
            // Saved source scenes are not changed. Present this new lab alone for Play Mode.
            for(int i=SceneManager.sceneCount-1;i>=0;i--){var other=SceneManager.GetSceneAt(i);if(other!=scene&&!other.isDirty)EditorSceneManager.CloseScene(other,true);}
            Selection.activeGameObject=root.gameObject;
            return "Created Gravitation scene, orbital station, satellite, 8 planets and Hub entry.";
        }
        static void SaturnRing(Transform planet)
        {
            const int n=96;var vertices=new Vector3[(n+1)*2];var uv=new Vector2[vertices.Length];var triangles=new int[n*6];
            for(int i=0;i<=n;i++)
            {
                float a=i*2*Mathf.PI/n;var d=new Vector3(Mathf.Cos(a),0,Mathf.Sin(a));
                vertices[i*2]=d*.65f;vertices[i*2+1]=d*1.15f;uv[i*2]=new Vector2(0,.5f);uv[i*2+1]=new Vector2(1,.5f);
                if(i<n){int k=i*6,b=i*2;triangles[k]=b;triangles[k+1]=b+2;triangles[k+2]=b+1;triangles[k+3]=b+1;triangles[k+4]=b+2;triangles[k+5]=b+3;}
            }
            var mesh=new Mesh{name="Saturn annulus"};mesh.vertices=vertices;mesh.uv=uv;mesh.triangles=triangles;mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,Folder+"/Meshes/SaturnRing.asset");
            var mat=Mat("Saturn rings",Color.white,"2k_saturn_ring_alpha.png",true);mat.SetFloat("_Cull",0);
            mat.SetFloat("_AlphaClip",1);mat.SetFloat("_Cutoff",.2f);mat.EnableKeyword("_ALPHATEST_ON");mat.renderQueue=2450;EditorUtility.SetDirty(mat);AssetDatabase.SaveAssetIfDirty(mat);
            var ring=Group("Rings",planet,Vector3.zero);ring.localRotation=Quaternion.Euler(35,0,20);
            ring.gameObject.AddComponent<MeshFilter>().sharedMesh=mesh;ring.gameObject.AddComponent<MeshRenderer>().sharedMaterial=mat;
        }
    }
}
