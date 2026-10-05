using System;
using System.Globalization;
using System.Linq;
using TMPro;
using Tecaverso.Labs.ObliqueLaunch;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;

namespace Tecaverso.Editor
{
    /// <summary>One-time, targeted migration. Never recreates the lab or moves its Canvas.</summary>
    public static class ObliqueLaunchFigmaUIBuilder
    {
        const string FontPath="Assets/_Project/UI/Fonts/Montserrat/";
        const string ArtPath="Assets/_Project/UI/Art/Figma/";
        static readonly Color Ink=Hex("161616"), Blue=Hex("284EA0"), Navy=Hex("07386F"), Surface=Hex("D3D1E8"), Inset=Hex("E8E6FE");
        static TMP_FontAsset regular, bold, extraBold;

        [MenuItem("Tecaverso/Oblique Launch/Apply Figma Experiment UI")]
        public static void Apply()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode) throw new InvalidOperationException("Exit Play mode first.");
            var panel=UnityEngine.Object.FindFirstObjectByType<ObliqueLaunchPanel>();
            var simulation=UnityEngine.Object.FindFirstObjectByType<ObliqueLaunchSimulation>();
            if(panel==null||simulation==null) throw new InvalidOperationException("Open the existing ObliqueLaunch scene first.");
            if(panel.transform.Find("Figma Experiment UI")!=null) throw new InvalidOperationException("Figma UI already installed; edit its existing objects.");
            regular=Font("Regular"); bold=Font("Bold"); extraBold=Font("ExtraBold");
            foreach(string name in new[]{"Result","Horizontal","Vertical"}) ImportSprite(name);
            var values=panel.Parameters;
            Undo.RegisterFullObjectHierarchyUndo(panel.gameObject,"Apply Figma experiment UI");
            var oldChildren=panel.transform.Cast<Transform>().ToArray();
            var backup=new GameObject("Legacy UI (inactive backup)",typeof(RectTransform));
            backup.transform.SetParent(panel.transform,false); backup.SetActive(false);
            foreach(var child in oldChildren) child.SetParent(backup.transform,true);
            panel.GetComponent<Image>().enabled=false;

            var root=Rect(panel.transform,"Figma Experiment UI",0,0,720,1120);
            root.anchorMin=root.anchorMax=root.pivot=new Vector2(.5f,.5f);
            root.anchoredPosition=Vector2.zero; root.localScale=Vector3.one*.64f;
            var parametersTab=Button(root,"Parâmetros",50,0,300,64,Blue,out _);
            var analysisTab=Button(root,"Análise",370,0,300,64,Navy,out _);
            var motion=Card(root,"Experiment - Projectile Motion",50,90,620,1010);
            var analysis=Card(root,"Experiment - Projectile Analysis",0,90,720,1010);
            Text(motion,"PARÂMETROS",58,46,500,36,24,extraBold,Ink);
            string[] labels={"ÂNGULO","VELOCIDADE","ALTURA INICIAL","MASSA","GRAVIDADE"};
            float[] min={0,0,0,1,5}, max={90,30,10,10,20}, initial={values.Angle,values.Speed,values.Height,values.Mass,values.Gravity};
            var sliders=new Slider[5]; var valueLabels=new TMP_Text[5];
            for(int i=0;i<5;i++)
            {
                float y=118+115*i;
                Text(motion,labels[i],58,y,355,32,20,bold,Ink);
                string unit=i==0?"°":i==1?" m/s":i==2?" m":i==3?" kg":" m/s²";
                string format=i<2?"0":i==4?"0.00":"0.0";
                valueLabels[i]=Text(motion,initial[i].ToString(format,CultureInfo.GetCultureInfo("pt-BR"))+unit,422,y,150,32,22,bold,Blue);
                valueLabels[i].alignment=TextAlignmentOptions.MidlineRight;
                sliders[i]=Slider(motion,labels[i],58,y+20,500,64,min[i],max[i],initial[i],i<2);
            }
            var pause=Button(motion,"PAUSAR",58,677,235,52,Navy,out var pauseText);
            pause.interactable=false;
            var toggle=VectorToggle(motion,323,681);
            var fire=Button(motion,"DISPARAR",58,758,235,72,Blue,out _);
            var reset=Button(motion,"REINICIAR",323,758,235,72,Navy,out _);
            Text(motion,"Arraste os controles e dispare para testar.",58,868,510,34,20,regular,Ink);
            var metrics=Text(motion,"t = 0,00 s    ·    y = 0,00 m",58,922,500,74,17,regular,Navy);
            panel.Bind(sliders[0],sliders[1],sliders[2],sliders[3],sliders[4],valueLabels[0],valueLabels[1],valueLabels[2],valueLabels[3],valueLabels[4],metrics,pauseText,fire,pause,reset);
            panel.BindVectorToggle(toggle);

            Text(analysis,"ANÁLISE",48,43,620,36,24,extraBold,Ink);
            Text(analysis,"ALCANCE",48,103,280,28,18,bold,Blue);
            Text(analysis,"ALTURA MÁXIMA",378,103,295,28,18,bold,Blue);
            var range=Text(analysis,"0,0 m",48,133,300,76,52,extraBold,Ink);
            var peak=Text(analysis,"0,0 m",378,133,294,76,52,extraBold,Ink);
            Text(analysis,"VETORES",48,248,620,30,18,bold,Ink);
            Legend(analysis,"Result","Velocidade resultante",298);
            Legend(analysis,"Horizontal","Componente horizontal · Vx",353);
            Legend(analysis,"Vertical","Componente vertical · Vy",408);
            Text(analysis,"↓",45,447,28,32,26,bold,Hex("9A7000"));
            Text(analysis,"Gravidade · g",80,447,580,32,22,regular,Ink);
            Text(analysis,"FÓRMULAS DE REFERÊNCIA",48,503,620,32,18,bold,Ink);
            string[] formulas={"x = v<sub>0</sub> cos(θ) · t","y = h<sub>0</sub> + v<sub>0</sub> sen(θ) · t − ½gt²","v<sub>y</sub> = v<sub>0</sub> sen(θ) − gt"};
            string[] captions={"Posição horizontal","Posição vertical","Velocidade vertical"};
            for(int i=0;i<3;i++)
            {
                var box=Card(analysis,"Formula "+i,48,548+i*120,620,96,Inset);
                Text(box,formulas[i],20,12,580,40,25,bold,Blue);
                Text(box,captions[i],20,55,580,30,17,regular,Ink);
            }
            var telemetry=Text(analysis,"PRONTO PARA DISPARAR\nt = 0,00 s    ·    y = 0,00 m",48,925,620,64,18,regular,Navy);
            var presenter=root.gameObject.AddComponent<ProjectileAnalysisPanel>();
            presenter.Bind(simulation,motion.gameObject,analysis.gameObject,parametersTab,analysisTab,range,peak,telemetry);
            analysis.gameObject.SetActive(false); parametersTab.interactable=false;
            SetVectorColor("Vresult",Blue); SetVectorColor("Vx",Hex("3ED39C")); SetVectorColor("Vy",Hex("D93645"));
            EditorUtility.SetDirty(panel); EditorUtility.SetDirty(presenter);
            EditorSceneManager.MarkSceneDirty(panel.gameObject.scene);
            EditorSceneManager.SaveScene(panel.gameObject.scene);
            AssetDatabase.SaveAssets();
            Selection.activeGameObject=panel.gameObject;
        }

        static TMP_FontAsset Font(string weight)
        {
            string path=FontPath+"Montserrat-"+weight+" SDF.asset";
            var asset=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
            if(asset!=null) return asset;
            var source=AssetDatabase.LoadAssetAtPath<UnityEngine.Font>(FontPath+"Montserrat-"+weight+".ttf");
            if(source==null) throw new InvalidOperationException("Import Montserrat "+weight+" first.");
            asset=TMP_FontAsset.CreateFontAsset(source,64,8,GlyphRenderMode.SDFAA,1024,1024,AtlasPopulationMode.Dynamic,true);
            asset.name="Montserrat-"+weight+" SDF";
            asset.fallbackFontAssetTable=new System.Collections.Generic.List<TMP_FontAsset>();
            if(TMP_Settings.defaultFontAsset!=null) asset.fallbackFontAssetTable.Add(TMP_Settings.defaultFontAsset);
            string characters=new string(Enumerable.Range(32,224).Select(i=>(char)i).ToArray())+"θ−↓";
            asset.TryAddCharacters(characters,out string missing);
            AssetDatabase.CreateAsset(asset,path);
            AssetDatabase.AddObjectToAsset(asset.material,asset);
            foreach(var atlas in asset.atlasTextures) AssetDatabase.AddObjectToAsset(atlas,asset);
            EditorUtility.SetDirty(asset);
            return asset;
        }
        static void ImportSprite(string name)
        {
            var importer=(TextureImporter)AssetImporter.GetAtPath(ArtPath+name+".png");
            if(importer==null) throw new InvalidOperationException("Missing Figma asset "+name);
            importer.textureType=TextureImporterType.Sprite;
            importer.spriteImportMode=SpriteImportMode.Single;
            importer.alphaIsTransparency=true; importer.mipmapEnabled=false;
            importer.textureCompression=TextureImporterCompression.Uncompressed; importer.SaveAndReimport();
        }
        public static RectTransform Rect(Transform parent,string name,float x,float y,float w,float h)
        {
            var go=new GameObject(name,typeof(RectTransform)); go.layer=parent.gameObject.layer;
            var rt=go.GetComponent<RectTransform>(); rt.SetParent(parent,false);
            rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(0,1);
            rt.anchoredPosition=new Vector2(x,-y); rt.sizeDelta=new Vector2(w,h); return rt;
        }
        public static Image Image(Transform parent,string name,float x,float y,float w,float h,Color color,bool hit=false)
        {
            var im=Rect(parent,name,x,y,w,h).gameObject.AddComponent<Image>();
            im.color=color; im.raycastTarget=hit; return im;
        }
        static RectTransform Card(Transform parent,string name,float x,float y,float w,float h,Color? fill=null)
        {
            var border=Image(parent,name,x,y,w,h,Navy,true);
            Image(border.transform,"Surface",2,2,w-4,h-4,fill??Surface);
            return border.rectTransform;
        }
        public static TMP_Text Text(Transform parent,string content,float x,float y,float w,float h,float size,TMP_FontAsset font,Color color)
        {
            var text=Rect(parent,content,x,y,w,h).gameObject.AddComponent<TextMeshProUGUI>();
            text.font=font; text.fontSize=size; text.color=color; text.text=content;
            text.alignment=TextAlignmentOptions.TopLeft; text.textWrappingMode=TextWrappingModes.NoWrap;
            text.raycastTarget=false; return text;
        }
        static Button Button(Transform parent,string name,float x,float y,float w,float h,Color fill,out TMP_Text label)
        {
            var image=Image(parent,name,x,y,w,h,fill,true);
            var button=image.gameObject.AddComponent<Button>(); button.targetGraphic=image;
            var colors=button.colors; colors.highlightedColor=new Color(.85f,.9f,1); colors.pressedColor=new Color(.65f,.75f,.92f); colors.disabledColor=new Color(.7f,.7f,.75f,.75f); button.colors=colors;
            button.navigation=new Navigation{mode=Navigation.Mode.None};
            label=Text(image.transform,name,0,0,w,h,24,bold,Color.white); label.alignment=TextAlignmentOptions.Center;
            image.gameObject.AddComponent<ButtonTweenFeedback>(); return button;
        }
        static Slider Slider(Transform parent,string name,float x,float y,float w,float h,float min,float max,float value,bool integers)
        {
            var hit=Image(parent,name+" Slider",x,y,w,h,Color.clear,true);
            Image(hit.transform,"Track",0,28,w,6,Hex("BDBBD4"));
            var fillArea=Rect(hit.transform,"Fill Area",0,28,w,6);
            var fill=Image(fillArea,"Active",0,0,w,6,Blue);
            fill.rectTransform.sizeDelta=Vector2.zero;
            fill.rectTransform.pivot=new Vector2(0,.5f);
            var handleArea=Rect(hit.transform,"Handle Area",0,15,w,32);
            var handle=Image(handleArea,"Handle",0,0,12,32,Blue,true);
            handle.rectTransform.sizeDelta=new Vector2(12,0);
            handle.rectTransform.pivot=new Vector2(.5f,.5f);
            Image(handle.transform,"White inset",3,3,6,26,Color.white);
            var slider=hit.gameObject.AddComponent<Slider>();
            slider.fillRect=fill.rectTransform; slider.handleRect=handle.rectTransform; slider.targetGraphic=handle;
            slider.direction=UnityEngine.UI.Slider.Direction.LeftToRight;
            slider.minValue=min; slider.maxValue=max; slider.wholeNumbers=integers; slider.SetValueWithoutNotify(value);
            slider.navigation=new Navigation{mode=Navigation.Mode.None};
            hit.gameObject.AddComponent<SliderTweenFeedback>(); return slider;
        }
        static Toggle VectorToggle(Transform parent,float x,float y)
        {
            var hit=Image(parent,"Exibir vetores",x,y,235,44,Color.clear,true);
            var box=Image(hit.transform,"Toggle border",0,6,32,32,Navy);
            Image(box.transform,"Toggle surface",2,2,28,28,Inset);
            var check=Image(box.transform,"Check",7,7,18,18,Blue);
            Text(hit.transform,"VETORES",46,6,185,32,20,bold,Ink);
            var toggle=hit.gameObject.AddComponent<Toggle>(); toggle.targetGraphic=box; toggle.graphic=check; toggle.isOn=true;
            toggle.navigation=new Navigation{mode=Navigation.Mode.None}; return toggle;
        }
        static void Legend(Transform parent,string asset,string label,float y)
        {
            var icon=Image(parent,label+" Icon",48,y,18,18,Color.white);
            icon.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath+asset+".png");
            Text(parent,label,80,y-6,585,34,22,regular,Ink);
        }
        static void SetVectorColor(string name,Color color)
        {
            var material=AssetDatabase.LoadAssetAtPath<Material>("Assets/_Project/Laboratories/Physics/ObliqueLaunch/Materials/"+name+".mat");
            if(material==null) return;
            Undo.RecordObject(material,"Match Figma vector legend");
            material.SetColor("_BaseColor",color); EditorUtility.SetDirty(material);
        }
        static Color Hex(string hex) { ColorUtility.TryParseHtmlString("#"+hex,out var color); return color; }
    }
}
