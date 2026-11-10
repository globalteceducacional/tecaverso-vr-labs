using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Tecaverso.Interaction;
using Tecaverso.Labs.Chemistry.PeriodicBonds;
public static class ReferenceAndMotionMigration
{
    const string Folder="Assets/_Project/Laboratories/Chemistry/PeriodicBonds/Data";
    public static void UpgradeReference()
    {
        var lab=UnityEngine.Object.FindFirstObjectByType<PeriodicTableExplorer>();if(!lab)throw new Exception("Abra a tabela periódica.");
        var r=lab.GetComponent<ElementReferenceView>();if(!r)r=lab.gameObject.AddComponent<ElementReferenceView>();lab.reference=r;
        r.content=AssetDatabase.LoadAssetAtPath<TextAsset>(Folder+"/ElementReference.json");if(!r.content)throw new Exception("Base local ausente.");
        var panel=lab.heading.canvas.transform;
        r.photo=Rect(panel,"Foto da amostra",new Vector2(430,120),new Vector2(500,500)).GetComponent<RawImage>()??panel.Find("Foto da amostra").gameObject.AddComponent<RawImage>();r.photo.raycastTarget=false;r.photo.enabled=false;
        r.photoCaption=Label(panel,"Legenda da amostra",new Vector2(430,-300),new Vector2(500,330),28,lab.body.font);r.photoCaption.text="";
        r.previous=Button(panel,"Texto anterior",new Vector2(-440,-510),new Vector2(420,70),lab.body.font);
        r.next=Button(panel,"Próximo texto",new Vector2(440,-510),new Vector2(420,70),lab.body.font);
        r.readingPage=Label(panel,"Página do texto",new Vector2(0,-510),new Vector2(300,70),27,lab.body.font);r.readingPage.text="";
        var row=Rect(panel,"Movimento reduzido",new Vector2(0,-585),new Vector2(700,58));
        r.reducedMotion=row.GetComponent<Toggle>()??row.gameObject.AddComponent<Toggle>();
        var bg=Rect(row,"Caixa",new Vector2(-310,0),new Vector2(42,42));var image=bg.GetComponent<Image>()??bg.gameObject.AddComponent<Image>();image.color=new Color(.16f,.31f,.63f);
        var mark=Rect(bg,"Marca",Vector2.zero,new Vector2(24,24));var tick=mark.GetComponent<Image>()??mark.gameObject.AddComponent<Image>();tick.color=Color.white;tick.raycastTarget=false;
        r.reducedMotion.targetGraphic=image;r.reducedMotion.graphic=tick;
        var hit=row.GetComponent<Image>()??row.gameObject.AddComponent<Image>();hit.color=Color.clear;
        Label(row,"Reduzir movimento",new Vector2(40,0),new Vector2(600,58),27,lab.body.font);
        r.reducedMotion.SetIsOnWithoutNotify(MotionPreferences.Reduced);
        lab.body.overflowMode=TextOverflowModes.Page;
        EditorUtility.SetDirty(lab);EditorUtility.SetDirty(r);EditorSceneManager.MarkSceneDirty(lab.gameObject.scene);EditorSceneManager.SaveScene(lab.gameObject.scene);
    }
    public static string UpgradeMotion()
    {
        int controls=0,objects=0;var previous=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        foreach(var bs in EditorBuildSettings.scenes.Where(s=>s.enabled&&s.path.StartsWith("Assets/_Project/"))){
            var scene=UnityEngine.SceneManagement.SceneManager.GetSceneByPath(bs.path);bool opened=!scene.isLoaded;if(opened)scene=EditorSceneManager.OpenScene(bs.path,OpenSceneMode.Additive);
            foreach(var root in scene.GetRootGameObjects()){
                foreach(var c in root.GetComponentsInChildren<Selectable>(true)){bool had=c.GetComponent<ControlMotion>();InteractionMotionBootstrap.Ensure(c);if(!had&&c.GetComponent<ControlMotion>())controls++;}
                foreach(var x in root.GetComponentsInChildren<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>(true)){bool had=x.GetComponent<XRInteractionMotion>();InteractionMotionBootstrap.Ensure(x);if(!had&&x.GetComponent<XRInteractionMotion>())objects++;}
            }
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);if(opened)EditorSceneManager.CloseScene(scene,true);
        }
        UnityEngine.SceneManagement.SceneManager.SetActiveScene(previous);return $"Added {controls} UI motion components and {objects} XR motion components.";
    }
    static RectTransform Rect(Transform parent,string name,Vector2 position,Vector2 size){var existing=parent.Find(name);var r=existing?(RectTransform)existing:new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);r.anchoredPosition=position;r.sizeDelta=size;return r;}
    static TMP_Text Label(Transform parent,string name,Vector2 position,Vector2 size,float fontSize,TMP_FontAsset font){var r=Rect(parent,name,position,size);var t=r.GetComponent<TextMeshProUGUI>()??r.gameObject.AddComponent<TextMeshProUGUI>();t.font=font;t.fontSize=fontSize;t.text=name;t.color=new Color(.086f,.086f,.086f);t.alignment=TextAlignmentOptions.Center;t.raycastTarget=false;t.overflowMode=TextOverflowModes.Truncate;return t;}
    static Button Button(Transform parent,string name,Vector2 position,Vector2 size,TMP_FontAsset font){var r=Rect(parent,name,position,size);var image=r.GetComponent<Image>()??r.gameObject.AddComponent<Image>();image.color=new Color(.16f,.31f,.63f);var button=r.GetComponent<Button>()??r.gameObject.AddComponent<Button>();Label(r,"Rótulo",Vector2.zero,size,26,font).text=name;r.GetComponentInChildren<TMP_Text>().color=Color.white;return button;}
}
