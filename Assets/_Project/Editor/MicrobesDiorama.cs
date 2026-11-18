using System.Collections.Generic;
using System.Linq;
using Tecaverso.Labs.Biology.Microbes;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using static Tecaverso.Editor.MicrobesAuthoring;

namespace Tecaverso.Editor
{
    internal static class MicrobesDiorama
    {
        public static void Create(MicrobesLab lab,Material graphite,Material accent)
        {
            var stand=Node(lab.transform,"Diorama do bacteriófago",new(5.27f,0,-2.20f));
            stand.localRotation=Quaternion.Euler(0,72,0);
            Shape(stand,"Coluna",PrimitiveType.Cylinder,new(0,.53f,0),new(.24f,.53f,.24f),graphite);
            Shape(stand,"Tampo",PrimitiveType.Cube,new(0,1.035f,-.10f),new(1.44f,.07f,.49f),graphite);
            Shape(stand,"Fundo do diorama",PrimitiveType.Cube,new(0,1.60f,.11f),new(1.50f,1.05f,.045f),graphite);
            Label(stand,"Ciclo de um bacteriófago",new(0,2.03f,.075f),930,74,38);
            var tint=MaterialFor("Traço do fago",new Color32(218,178,241,255));
            lab.stages=new GameObject[3];
            for(int stage=0;stage<3;stage++)
            {
                var layer=Node(stand,"Etapa "+(stage+1),new(0,1.58f,.056f));
                lab.stages[stage]=layer.gameObject;
                var points=new List<Vector2>();
                for(int i=0;i<=48;i++)
                {
                    float a=Mathf.Lerp(stage==2?.47f:0,stage==2?Mathf.PI*2-.47f:Mathf.PI*2,i/48f);
                    points.Add(new Vector2(Mathf.Cos(a)*.395f-.048f,Mathf.Sin(a)*.166f));
                }
                Stroke(layer,"Contorno da célula",accent,.014f,points.ToArray());
                if(stage==0)
                {
                    Phage(layer,new(-.12f,.228f),.8f,tint);
                    Stroke(layer,"Material genético",tint,.009f,new(-.12f,.18f),new(-.12f,.115f),new(-.08f,.09f),new(-.125f,.054f),new(-.10f,.023f));
                }
                else
                {
                    Phage(layer,new(-.23f,.027f),.53f,tint);
                    Phage(layer,new(-.050f,-.06f),.47f,tint);
                    if(stage==1) Phage(layer,new(.13f,.046f),.49f,tint);
                    else
                    {
                        Phage(layer,new(.39f,.034f),.52f,tint);
                        Phage(layer,new(.55f,.13f),.52f,tint);
                    }
                }
                layer.gameObject.SetActive(stage==0);
            }
            lab.stageCaption=Label(stand,"1 / 3    Contato e entrada\nO fago se liga à bactéria e seu material genético entra.",new(0,1.23f,.071f),920,136,26);
            var button=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/Samples/XR Interaction Toolkit/3.5.1/Starter Assets/DemoAssets/Prefabs/Interactables/Push Button.prefab"),stand);
            button.transform.localPosition=new(-.47f,1.16f,-.16f);button.transform.localScale=Vector3.one*1.18f;
            lab.advance=button.GetComponent<XRSimpleInteractable>();
            foreach(var r in button.GetComponentsInChildren<MeshRenderer>())
            {
                r.sharedMaterials=r.sharedMaterials.Select(m=>r.name=="EmergencyStop"?accent:graphite).ToArray();
                PrefabUtility.RecordPrefabInstancePropertyModifications(r);
            }
            Label(stand,"Próxima etapa",new(.05f,1.09f,-.30f),525,65,28);
        }

        static void Phage(Transform parent,Vector2 center,float scale,Material ink)
        {
            var icon=Node(parent,"Fago",new(center.x,center.y,-.01f));
            icon.localScale=Vector3.one*scale;
            var head=Enumerable.Range(0,7).Select(i=>new Vector2(Mathf.Cos(i*Mathf.PI/3)*.050f,
                .048f+Mathf.Sin(i*Mathf.PI/3)*.051f)).ToArray();
            Stroke(icon,"Cabeça",ink,.010f,head);
            Stroke(icon,"Cauda",ink,.012f,new(0,0),new(0,-.060f));
            Stroke(icon,"Fibras",ink,.010f,new(-.051f,-.095f),new(-.035f,-.057f),new(0,-.046f),new(.035f,-.057f),new(.051f,-.095f));
        }

        static void Stroke(Transform parent,string name,Material ink,float width,params Vector2[] points)
        {
            var line=Node(parent,name,Vector3.zero).gameObject.AddComponent<LineRenderer>();
            line.useWorldSpace=false;line.alignment=LineAlignment.TransformZ;
            line.sharedMaterial=ink;line.widthMultiplier=width;line.numCornerVertices=3;line.numCapVertices=2;
            line.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;line.receiveShadows=false;
            line.positionCount=points.Length;line.SetPositions(points.Select(p=>new Vector3(p.x,p.y,0)).ToArray());
        }
    }
}
