using System;
using System.Linq;
using Tecaverso.Hub;
using Tecaverso.Labs.ObliqueLaunch;
using TMPro;
using Unity.Netcode;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using static Tecaverso.Editor.ObliqueLaunchFigmaUIBuilder;

namespace Tecaverso.Editor
{
    /// <summary>Adds entry pages without rebuilding the existing room, rig, canvas or content UI.</summary>
    public static class HubEntryUIBuilder
    {
        static readonly Color Ink=new Color32(22,22,22,255),Blue=new Color32(40,78,160,255),Navy=new Color32(7,56,111,255),Surface=new Color32(211,209,232,255),Background=new Color32(232,230,254,255);
        static TMP_FontAsset regular,bold,extra;
        static TMP_Text Label(Transform parent,string value,float x,float y,float w,float h,float size=28,bool heavy=false)
        { var text=Text(parent,value,x,y,w,h,size,heavy?bold:regular,Ink); text.textWrappingMode=TextWrappingModes.Normal; return text; }
        static RectTransform Card(Transform parent,string name,float x,float y,float w,float h)
        { var border=Image(parent,name,x,y,w,h,Navy,true); Image(border.transform,"Surface",2,2,w-4,h-4,Surface); return border.rectTransform; }
        static Button Action(Transform parent,string value,float x,float y,float w,out TMP_Text text,bool secondary=false)
        {
            var image=Image(parent,value,x,y,w,88,secondary?Navy:Blue,true); var button=image.gameObject.AddComponent<Button>();button.targetGraphic=image;
            button.navigation=new Navigation{mode=Navigation.Mode.None};
            var colors=button.colors;colors.disabledColor=new Color(.65f,.65f,.72f,1);button.colors=colors;
            text=Label(image.transform,value,16,0,w-32,88,28,true);text.alignment=TextAlignmentOptions.Center;text.color=Color.white;
            image.gameObject.AddComponent<ButtonTweenFeedback>(); return button;
        }
        static RectTransform Page(Transform parent,string name,string title,string subtitle)
        {
            var root=Image(parent,name,0,0,2560,1440,Background,true).rectTransform;
            Label(root,"TECAVERSO LABS",96,96,800,48,28,true);
            Label(root,"APRENDA. EXPERIMENTE. DESCUBRA.",96,148,1500,40,24);
            var heading=Label(root,title,96,230,2368,140,80,true); heading.font=extra; heading.name="Heading";
            Label(root,subtitle,96,376,2368,60,28);
            return root;
        }
        static void Icon(Transform parent,string name,float x,float y,float size)
        {
            var icon=Image(parent,name,x,y,size,size,Color.white);
            icon.sprite=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/UI/Art/Figma/Entry/"+name+".png"); icon.preserveAspect=true;
        }
        static TMP_InputField Input(Transform parent,string name,float x,float y,float w,int limit)
        {
            var image=Image(parent,name,x,y,w,88,Background,true);
            var field=image.gameObject.AddComponent<TMP_InputField>();field.targetGraphic=image;
            var viewport=Rect(image.transform,"Viewport",24,0,w-48,88);viewport.gameObject.AddComponent<RectMask2D>();
            var text=Label(viewport,"",0,0,w-48,88,28);text.alignment=TextAlignmentOptions.MidlineLeft;text.textWrappingMode=TextWrappingModes.NoWrap;
            field.textViewport=viewport;field.textComponent=text;field.characterLimit=limit;field.lineType=TMP_InputField.LineType.SingleLine;
            field.navigation=new Navigation{mode=Navigation.Mode.None}; return field;
        }
        public static string Apply()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Exit Play mode first.");
            var canvas=GameObject.Find("Tecaverso Hub UI");var menu=UnityEngine.Object.FindFirstObjectByType<HubMenuController>();
            if(canvas==null||menu==null)throw new InvalidOperationException("Open the existing Hub scene.");
            if(canvas.transform.Find("LAN Entry Flow")!=null)return "Already installed; no existing UI changed.";
            regular=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Regular SDF.asset");
            bold=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Bold SDF.asset");
            extra=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-ExtraBold SDF.asset");
            foreach(var name in new[]{"Solo","Create","Group","Error"})
            {
                var importer=(TextureImporter)AssetImporter.GetAtPath("Assets/_Project/UI/Art/Figma/Entry/"+name+".png");
                importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.alphaIsTransparency=true;importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.SaveAndReimport();
            }
            Undo.RegisterFullObjectHierarchyUndo(canvas,"Add LAN entry flow");
            var manager=UnityEngine.Object.FindFirstObjectByType<NetworkManager>();
            var service=manager.GetComponent<LanLobbyController>()??Undo.AddComponent<LanLobbyController>(manager.gameObject);
            var catalogPath=AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:HubCatalog").First());
            service.Bind(manager,AssetDatabase.LoadAssetAtPath<HubCatalog>(catalogPath));EditorUtility.SetDirty(service);
            var root=Rect(canvas.transform,"LAN Entry Flow",0,0,2560,1440);
            var ui=root.gameObject.AddComponent<HubEntryController>();ui.lobby=service;ui.menu=menu;
            var entry=Page(root,"Entry","Como você quer aprender?","Escolha sua experiência. Todas as salas funcionam na rede local.");ui.entryPage=entry.gameObject;
            string[] titles={"Explorar sozinho","Criar sala local","Entrar em uma sala"},icons={"Solo","Create","Group"};
            string[] bodies={"Explore os laboratórios no seu ritmo.\nNão é necessário conectar-se a uma sala.","Seja o anfitrião e conduza o experimento com sua turma na mesma rede.","Participe de uma sala usando o IP compartilhado pelo anfitrião."};
            var buttons=new Button[3];
            for(int i=0;i<3;i++)
            { var card=Card(entry,titles[i],96+i*768,480,720,520);Icon(card,icons[i],48,48,72);Label(card,titles[i],48,156,624,72,36,true);Label(card,bodies[i],48,252,624,156);buttons[i]=Action(card,i==0?"EXPLORAR":i==1?"CRIAR SALA":"ENTRAR",48,384,624,out _); }
            ui.solo=buttons[0];ui.create=buttons[1];ui.join=buttons[2];
            Label(entry,"SEM LOGIN · SEM NUVEM · CONEXÃO LOCAL",96,1096,2200,60,24,true);
            var form=Page(root,"Room form","Criar sala local","Prepare sua sessão com os dispositivos na mesma rede.");ui.formPage=form.gameObject;ui.formTitle=form.Find("Heading").GetComponent<TMP_Text>();
            var fields=Card(form,"Room details",96,480,980,680);Label(fields,"SEU NOME",48,48,820,40,28,true);ui.nickname=Input(fields,"Nickname",48,108,884,24);
            ui.fieldLabel=Label(fields,"NOME DA SALA",48,252,884,40,28,true);ui.roomOrAddress=Input(fields,"Room or IP",48,312,884,40);
            ui.fieldHint=Label(fields,"",48,424,884,80,24);ui.formError=Label(fields,"",48,532,884,100,24);ui.formError.color=new Color32(170,36,53,255);
            var info=Card(form,"Local network information",1140,480,1324,680);Icon(info,"Group",48,48,80);ui.formInfo=Label(info,"",48,176,1228,440,28);
            ui.formBack=Action(form,"VOLTAR",96,1232,360,out _,true);ui.submit=Action(form,"CRIAR SALA",480,1232,596,out ui.submitLabel);
            var room=Page(root,"Lobby","Sua sala está pronta","Organize a turma, escolha o conteúdo e prepare-se para experimentar.");ui.lobbyPage=room.gameObject;
            ui.roomTitle=room.Find("Heading").GetComponent<TMP_Text>();
            var people=Card(room,"Participants",96,480,1240,696);Label(people,"PARTICIPANTES",48,32,1144,56,36,true);
            // Eight rows fit the panel at the 24 px secondary-text token without hidden participants.
            ui.participants=Label(people,"",48,116,1144,464,24);ui.participants.richText=false;
            ui.roomAddress=Label(people,"",48,588,1144,88,24,true);
            var content=Card(room,"Selected experiment",1384,480,1080,696);
            var projectile=Image(content,"Projectile Motion",48,48,128,128,Color.white);projectile.preserveAspect=true;projectile.sprite=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/UI/Art/Figma/Hub/ProjectileMotion.png");
            ui.contentTitle=Label(content,"Lançamento oblíquo",208,56,824,128,36,true);
            ui.lobbyHint=Label(content,"",48,224,984,160,28);
            ui.chooseContent=Action(content,"TROCAR CONTEÚDO",48,440,984,out _,true);
            ui.start=Action(content,"INICIAR PARA TODOS",48,560,984,out _);
            ui.ready=Action(content,"ESTOU PRONTO",48,560,984,out ui.readyLabel);
            ui.leave=Action(room,"SAIR DA SALA",96,1232,440,out _,true);
            var connecting=Page(root,"Connecting","Conectando…","A sessão é local. Nenhum serviço em nuvem é utilizado.");ui.connectingPage=connecting.gameObject;
            var progress=Card(connecting,"Connection progress",96,480,2368,440);Icon(progress,"Group",48,48,80);ui.connectionText=Label(progress,"",48,176,2240,176,36,true);ui.cancelConnection=Action(connecting,"CANCELAR",96,1040,440,out _,true);
            var error=Page(root,"Connection failure","Precisamos de sua atenção","Você pode tentar novamente ou voltar ao início.");ui.errorPage=error.gameObject;
            var dialog=Card(error,"Connection error",96,480,1600,656);Icon(dialog,"Error",48,48,64);ui.errorTitle=Label(dialog,"",144,48,1408,96,48,true);ui.errorBody=Label(dialog,"",48,184,1504,280,28);
            ui.retry=Action(dialog,"TENTAR NOVAMENTE",48,520,720,out _);ui.errorBack=Action(dialog,"VOLTAR AO INÍCIO",792,520,760,out _,true);
            ui.selectionBack=Action(root,"VOLTAR À ENTRADA / SALA",1760,1232,704,out _,true);
            var shade=Image(root,"Leave confirmation",0,0,2560,1440,new Color(0,0,0,.65f),true);ui.leaveDialog=shade.gameObject;
            var confirmation=Card(shade.transform,"Confirm leave",600,480,1360,456);
            Label(confirmation,"Sair da sala?",48,48,1264,80,48,true);Label(confirmation,"Se você for o anfitrião, a sessão será encerrada para todos.",48,160,1264,88,28);
            ui.cancelLeave=Action(confirmation,"CONTINUAR NA SALA",48,304,620,out _,true);ui.confirmLeave=Action(confirmation,"SAIR",692,304,620,out _);
            foreach(var page in new[]{ui.formPage,ui.lobbyPage,ui.connectingPage,ui.errorPage,ui.leaveDialog,ui.selectionBack.gameObject})page.SetActive(false);
            var old=GameObject.Find("Connection Canvas");if(old!=null){Undo.RecordObject(old,"Retire side connection UI");old.SetActive(false);}
            EditorUtility.SetDirty(ui);EditorSceneManager.MarkSceneDirty(canvas.scene);EditorSceneManager.SaveScene(canvas.scene);
            return "Entry, forms, lobby, connection states and confirmation installed. Existing transforms preserved.";
        }
    }
}
