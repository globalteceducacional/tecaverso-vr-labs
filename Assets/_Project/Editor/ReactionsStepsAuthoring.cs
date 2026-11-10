using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.ProBuilder;
using TMPro;
using Tecaverso.Labs.Chemistry.Reactions;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Object=UnityEngine.Object;
namespace Tecaverso.Editor
{
    public static class ReactionsStepsAuthoring
    {
        static Transform Box(Transform parent,string name,Vector3 position,Vector3 size,Material material)
        {
            var p=ShapeGenerator.GenerateCube(PivotLocation.Center,size);p.name=name;p.transform.SetParent(parent,false);p.transform.localPosition=position;p.GetComponent<Renderer>().sharedMaterial=material;
            if(!p.GetComponent<BoxCollider>())p.gameObject.AddComponent<BoxCollider>();return p.transform;
        }
        static Transform Child(Transform parent,string name,Vector3 position)
        {var t=new GameObject(name).transform;t.SetParent(parent,false);t.localPosition=position;return t;}
        static void Pose(ReagentTool t,Vector3 position)
        {t.home.position=position;t.home.rotation=Quaternion.identity;t.transform.SetPositionAndRotation(position,Quaternion.identity);if(t.TryGetComponent<Rigidbody>(out var rb)){rb.position=position;rb.rotation=Quaternion.identity;}}
        public static void Upgrade()
        {
            var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();var lab=Object.FindFirstObjectByType<ReactionsLab>();
            if(EditorApplication.isPlayingOrWillChangePlaymode||scene.isDirty||scene.path!=ReactionsAuthoring.ScenePath||!lab||!lab.bench||lab.bench.thermometer)throw new Exception("Abra Reactions salva, fora de Play e ainda sem a revisão de degraus.");
            System.IO.Directory.CreateDirectory("Temp/ReactionsMigration");System.IO.File.Copy(scene.path,"Temp/ReactionsMigration/Reactions-before-steps.unity",true);
            var b=lab.bench;var root=b.transform;const string f=ReactionsAuthoring.Folder;
            var dark=AssetDatabase.LoadAssetAtPath<Material>(f+"/Materials/Grafite.mat");var blue=AssetDatabase.LoadAssetAtPath<Material>(f+"/Materials/Azul.mat");var metal=AssetDatabase.LoadAssetAtPath<Material>(f+"/Materials/Metal.mat");
            string[] old={"Tampo esquerdo","Tampo direito","Tampo ao fundo"};
            Vector3[] oldSize={new Vector3(.65f,.08f,2.5f),new Vector3(.65f,.08f,2.5f),new Vector3(3.4f,.08f,.65f)};
            for(int k=0;k<3;k++)
            {
                var t=root.Find(old[k]);t.name="Degrau "+(k+1);t.localPosition=new Vector3(0,.88f+k*.25f,1.35f+k*.58f);t.localScale=new Vector3(3.9f/oldSize[k].x,1,.58f/oldSize[k].z);
                Box(root,"Espelho do degrau "+k,new Vector3(0,.77f+k*.25f,1.10f+k*.58f),new Vector3(3.9f,.22f,.035f),dark);
            }
            Box(root,"Área de trabalho livre",new Vector3(0,.88f,.18f),new Vector3(3.9f,.08f,.9f),dark);
            int leg=0;foreach(var t in root.Cast<Transform>().Where(t=>t.name=="Pé da mesa").ToArray())
            {t.localPosition=new Vector3(leg<2?-1.8f:1.8f,.44f,leg%2==0?-.15f:2.50f);leg++;}
            for(int i=0;i<12;i++)
            {
                var r=ReactionWorkbench.Reagents[i];int g=i/3,k=i%3;float x=-1.44f+g*.96f+(k-1)*.16f;
                var t=b.instruments.Single(t=>!t.spoon&&t.reagentId==r.Id);
                Pose(t,new Vector3(x,1.20f+k*.25f,1.35f+k*.58f));
                t.liquid.tint=ReactionsBench.ClearLiquid;t.liquid.opacity=.16f;t.liquid.ResetMotion();
                if(r.Solid)
                {var spoon=b.instruments.Single(s=>s.spoon&&s.reagentId==r.Id);Pose(spoon,t.home.position+new Vector3(.29f,-.23f,-.10f));}
                else{var main=t.droplets.main;main.startColor=new Color(.92f,.92f,.92f,.55f);}
            }
            string[] groups={"ÁCIDOS","BASES","SAIS","ÓXIDOS"};
            for(int g=0;g<4;g++)root.Find("Grupo "+groups[g]).localPosition=new Vector3(-1.44f+g*.96f,2.12f,2.52f);
            var indicator=b.instruments.Single(t=>t.indicator);Pose(indicator,new Vector3(1.24f,1.12f,.03f));indicator.liquid.tint=ReactionsBench.ClearLiquid;indicator.liquid.opacity=.16f;
            // Move only the status plaque whose old location obscured reagent labels.
            b.status.transform.parent.localPosition=new Vector3(-1.5f,1.20f,-.4f);
            ((RectTransform)b.status.transform.parent).sizeDelta=new Vector2(650,200);
            lab.receiver.tint=ReactionsBench.ClearLiquid;lab.receiver.opacity=.16f;
            // Dedicated neutral glass instance; never recolour shared materials in other laboratories.
            string glassPath=f+"/Materials/Vidraria neutra.mat";
            var neutral=AssetDatabase.LoadAssetAtPath<Material>(glassPath);
            var renderer=lab.transform.GetComponentsInChildren<Renderer>(true).First(r=>r.sharedMaterial&&r.sharedMaterial.name=="Vidraria");
            if(!neutral){neutral=new Material(renderer.sharedMaterial){name="Vidraria neutra"};neutral.SetColor("_BaseColor",new Color(.94f,.94f,.94f,.13f));AssetDatabase.CreateAsset(neutral,glassPath);}
            foreach(var r in lab.GetComponentsInChildren<Renderer>(true))if(r.sharedMaterial&&r.sharedMaterial.name=="Vidraria")r.sharedMaterial=neutral;
            var probe=CreateProbe(root,lab,blue,dark,metal);b.thermometer=probe;
            Box(root,"Suporte da sonda",new Vector3(-1.10f,.96f,-.07f),new Vector3(.38f,.08f,.28f),blue);
            Tecaverso.Interaction.InteractionMotionBootstrap.Ensure(probe);
            EditorUtility.SetDirty(b);EditorUtility.SetDirty(lab);EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
        }
        static ReactionThermometer CreateProbe(Transform parent,ReactionsLab lab,Material blue,Material dark,Material metal)
        {
            var root=Child(parent,"Termômetro XR",new Vector3(-1.10f,1.60f,-.07f));
            var rb=root.gameObject.AddComponent<Rigidbody>();rb.useGravity=false;rb.isKinematic=true;
            var tool=root.gameObject.AddComponent<ReactionThermometer>();tool.lab=lab;tool.throwOnDetach=false;tool.useDynamicAttach=true;tool.movementType=XRBaseInteractable.MovementType.Kinematic;
            tool.home=Child(parent,"Retorno termômetro",root.localPosition);
            // Octagonal chamfered housing: one editable ProBuilder mesh with outward winding.
            Vector2[] ring={new(-.13f,-.14f),new(.13f,-.14f),new(.16f,-.11f),new(.16f,.11f),new(.13f,.14f),new(-.13f,.14f),new(-.16f,.11f),new(-.16f,-.11f)};
            var vertices=new List<Vector3>();foreach(float z in new[]{-.035f,.035f})foreach(var p in ring)vertices.Add(new Vector3(p.x,p.y,z));
            var faces=new List<Face>();for(int i=1;i<7;i++){faces.Add(new Face(new[]{0,i+1,i}));faces.Add(new Face(new[]{8,8+i,8+i+1}));}
            for(int i=0;i<8;i++){int j=(i+1)%8;faces.Add(new Face(new[]{i,j,8+j,i,8+j,8+i}));}
            var housing=ProBuilderMesh.Create(vertices,faces);housing.name="Carcaça chanfrada";housing.transform.SetParent(root,false);housing.GetComponent<Renderer>().sharedMaterial=blue;
            var col=root.gameObject.AddComponent<BoxCollider>();col.size=new Vector3(.32f,.28f,.07f);tool.colliders.Add(col);
            var shaft=ShapeGenerator.GenerateCylinder(PivotLocation.Center,16,.008f,.44f,0);shaft.name="Sonda de aço";shaft.transform.SetParent(root,false);shaft.transform.localPosition=new Vector3(0,-.36f,0);shaft.GetComponent<Renderer>().sharedMaterial=metal;
            tool.tip=Child(root,"Ponta de medição",new Vector3(0,-.58f,0));
            // Reuse the existing world-space label layout/font, without adding an application HUD.
            var existing=lab.bench.instruments.First(t=>!t.spoon).label.transform.parent.gameObject;
            var panel=Object.Instantiate(existing,root).GetComponent<RectTransform>();panel.name="Visor da sonda";panel.localPosition=new Vector3(0,0,-.039f);panel.localRotation=Quaternion.identity;panel.localScale=Vector3.one*.0008f;panel.sizeDelta=new Vector2(350,290);
            tool.display=panel.GetComponentInChildren<TMP_Text>();tool.display.text="TERMÔMETRO\nMergulhe a ponta\nno líquido\nEstimativa ideal";tool.display.fontSize=32;tool.display.alignment=TextAlignmentOptions.Center;
            return tool;
        }
    }
}
