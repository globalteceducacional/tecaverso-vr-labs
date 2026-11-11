using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Tecaverso.UI;
using Tecaverso.Labs.Chemistry.AtomicModels;

namespace Tecaverso.Editor
{
    // Targeted refinement of the existing room, never a scene replacement.
    public static class AtomicModelsPresentation
    {
        const string Folder="Assets/_Project/Laboratories/Chemistry/AtomicModels";
        static TMP_FontAsset font;
        public static void Apply()
        {
            var lab=Object.FindFirstObjectByType<AtomicModelsLab>();
            if(!lab || lab.gameObject.scene.path!=Folder+"/Scenes/AtomicModels.unity")throw new System.Exception("Open AtomicModels first.");
            font=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Regular SDF.asset");
            foreach(var t in lab.GetComponentsInChildren<TMP_Text>(true))
            {
                t.font=font;t.fontSize=8;t.raycastTarget=false;
                t.text=t.text.Replace("⁺","<sup>+</sup>").Replace("⁻","<sup>−</sup>").Replace("⁰","<sup>0</sup>");
                if(t.name=="Título"||t.name=="Orientação"||t==lab.result||t==lab.warning||t==lab.viewStatus)t.transform.localRotation=Quaternion.identity;
                t.ForceMeshUpdate(true);
            }
            // Keep the existing spawn clear: the rear wall exhibits now sit to either side.
            var exhibits=lab.GetComponentsInChildren<AtomicControl>(true).Where(c=>c.action=="exhibit").ToArray();
            for(int i=0;i<exhibits.Length;i++)
            {
                var e=exhibits[i];e.exhibitImage=AssetDatabase.LoadAssetAtPath<Texture2D>(Folder+"/Art/model-"+i+".png");
                var label=e.transform.Find("Nome").GetComponent<TMP_Text>();label.rectTransform.sizeDelta=new Vector2(11,3);label.fontSize=8;
                if(i==6){e.transform.localPosition=new Vector3(4.8f,1.5f,1.1f);e.transform.localRotation=Quaternion.Euler(0,75,0);e.transform.localScale=Vector3.one*.65f;}
                if(i>=7){e.transform.localPosition=new Vector3(-2.7f+(i-7)*2.7f,1.65f,-5.1f);}
                if(i==5)foreach(var line in e.GetComponentsInChildren<LineRenderer>(true))line.gameObject.SetActive(false);
                // Local illustrations are also visible on the physical exhibits.
                var old=e.transform.Find("Ilustração local");if(old)Object.DestroyImmediate(old.gameObject);
                var picture=GameObject.CreatePrimitive(PrimitiveType.Quad);picture.name="Ilustração local";picture.transform.SetParent(e.transform,false);
                picture.transform.localPosition=new Vector3(0,-.48f,-.55f);picture.transform.localScale=new Vector3(.62f,.36f,1);
                Object.DestroyImmediate(picture.GetComponent<Collider>());
                string path=Folder+"/Art/model-"+i+".mat";var mat=AssetDatabase.LoadAssetAtPath<Material>(path);
                if(!mat){mat=new Material(Shader.Find("Universal Render Pipeline/Unlit"));mat.mainTexture=e.exhibitImage;AssetDatabase.CreateAsset(mat,path);}
                picture.GetComponent<Renderer>().sharedMaterial=mat;
            }
            // TMP sizing is physical; reserve room for all four lines of the result.
            lab.result.rectTransform.sizeDelta=new Vector2(14,5);lab.result.fontSize=7;
            for(int i=0;i<3;i++)
            {
                var label=lab.transform.Find("Contagem "+i);if(label){label.localPosition=new Vector3(-3.3f,1.2f,-.65f+i*.55f);label.localRotation=Quaternion.Euler(65,0,0);label.GetComponent<TMP_Text>().fontSize=6;}
            }
            var mist=AssetDatabase.LoadAssetAtPath<Material>(Folder+"/Materials/Nuvem translúcida.mat");
            foreach(var g in lab.cloudVisuals)g.GetComponent<Renderer>().sharedMaterial=mist;
            // These two views are rebuilt internally only; scene/root transforms stay intact.
            var cp=lab.consultationRoot.transform.localPosition;Object.DestroyImmediate(lab.consultationRoot);
            var panel=Panel(lab.transform,"Ficha contextual · uma por vez",new Vector2(1100,650),cp);
            lab.consultationRoot=panel.gameObject;
            lab.consultationTitle=Label(panel,"Título",new Vector2(0,255),new Vector2(1000,90),40);
            var imageGo=new GameObject("Ilustração ampliada",typeof(RectTransform),typeof(RawImage));imageGo.transform.SetParent(panel,false);
            var rect=imageGo.GetComponent<RectTransform>();rect.sizeDelta=new Vector2(400,240);rect.anchoredPosition=new Vector2(-300,30);
            lab.consultationImage=imageGo.GetComponent<RawImage>();lab.consultationImage.raycastTarget=false;
            lab.consultationText=Label(panel,"Proposta e evidências",new Vector2(210,0),new Vector2(550,390),26);lab.consultationText.alignment=TextAlignmentOptions.TopLeft;
            Button(panel,"Fechar",new Vector2(0,-260),lab.CloseConsultation);
            var fp=lab.confirmationRoot.transform.localPosition;Object.DestroyImmediate(lab.confirmationRoot);
            var confirm=Panel(lab.transform,"Confirmação de limpeza",new Vector2(900,330),fp);lab.confirmationRoot=confirm.gameObject;
            lab.confirmation=Label(confirm,"Pergunta",new Vector2(0,65),new Vector2(820,160),36);
            Button(confirm,"Cancelar",new Vector2(-210,-100),lab.CancelClear);Button(confirm,"Limpar",new Vector2(210,-100),lab.ConfirmClear);
            lab.protons=lab.neutrons=lab.electrons=0;lab.Refresh();lab.CloseConsultation();lab.CancelClear();
            EditorSceneManager.MarkSceneDirty(lab.gameObject.scene);EditorSceneManager.SaveScene(lab.gameObject.scene);AssetDatabase.SaveAssets();
        }
        static RectTransform Panel(Transform parent,string name,Vector2 size,Vector3 pos)
        {
            var go=new GameObject(name,typeof(RectTransform),typeof(Canvas),typeof(CanvasGroup),typeof(TrackedDeviceGraphicRaycaster));go.transform.SetParent(parent,false);
            go.GetComponent<Canvas>().renderMode=RenderMode.WorldSpace;var rt=(RectTransform)go.transform;rt.sizeDelta=size;rt.localPosition=pos;rt.localScale=Vector3.one*.0028f;
            var bg=new GameObject("Fundo",typeof(RectTransform),typeof(Image));bg.transform.SetParent(rt,false);var br=(RectTransform)bg.transform;br.anchorMin=Vector2.zero;br.anchorMax=Vector2.one;br.offsetMin=br.offsetMax=Vector2.zero;bg.GetComponent<Image>().color=new Color(.91f,.90f,.996f);
            return rt;
        }
        static TMP_Text Label(RectTransform parent,string name,Vector2 pos,Vector2 size,float fontSize)
        {
            var g=new GameObject(name,typeof(RectTransform),typeof(TextMeshProUGUI));g.transform.SetParent(parent,false);var t=g.GetComponent<TextMeshProUGUI>();t.font=font;t.fontSize=fontSize;t.color=new Color(.035f,.07f,.18f);t.alignment=TextAlignmentOptions.Center;t.raycastTarget=false;t.rectTransform.anchoredPosition=pos;t.rectTransform.sizeDelta=size;t.textWrappingMode=TextWrappingModes.Normal;return t;
        }
        static void Button(RectTransform parent,string title,Vector2 pos,UnityAction action)
        {
            var g=new GameObject(title,typeof(RectTransform),typeof(Image),typeof(Button));g.transform.SetParent(parent,false);var rt=(RectTransform)g.transform;rt.sizeDelta=new Vector2(340,72);rt.anchoredPosition=pos;g.GetComponent<Image>().color=new Color(.157f,.306f,.627f);
            UnityEventTools.AddPersistentListener(g.GetComponent<Button>().onClick,action);var text=Label(rt,"Rótulo",Vector2.zero,new Vector2(320,64),28);text.text=title;text.color=Color.white;
        }
    }
}
