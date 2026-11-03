// Editor-only reproducible demonstrations. All staging exists only in Play Mode.
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Encoder;
using UnityEditor.Recorder.Input;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Tecaverso.Labs.Waves;
using Tecaverso.Labs.Thermology;
using Tecaverso.Labs.Gravitation;
using Tecaverso.Labs.InclinedPlane;
using Tecaverso.Labs.ObliqueLaunch;
using Tecaverso.Labs.ResistorCircuits;

public static class PhysicsDemoRecorder
{
    static RecorderController recorder;
    static Camera camera;
    static TMP_Text caption;
    static string lab;
    static int firstFrame, lastFrame, stage;
    static float duration;
    public static string Status { get; private set; }="Idle";
    static T Find<T>() where T:UnityEngine.Object => UnityEngine.Object.FindFirstObjectByType<T>();
    public static void Begin(string name, string output)
    {
        if(!EditorApplication.isPlaying) throw new InvalidOperationException("Enter Play Mode first.");
        lab=name; stage=0; duration=name=="Waves"?36:30;
        foreach(var c in UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsSortMode.None)){c.enabled=false;if(c.CompareTag("MainCamera"))c.tag="Untagged";}
        camera=new GameObject("Temporary recording camera").AddComponent<Camera>();camera.tag="MainCamera";
        camera.nearClipPlane=.05f;camera.farClipPlane=150;camera.fieldOfView=65;
        camera.gameObject.AddComponent<UniversalAdditionalCameraData>().renderPostProcessing=true;
        View(new Vector3(0,2,-6.6f),new Vector3(0,1.45f,1));
        string title=name;
        switch(name)
        {
            case "Waves": title="ONDULATÓRIA";Find<WavesLab>().ResetRun();break;
            case "Thermology": title="TERMOLOGIA";View(new Vector3(.7f,2.6f,-4.6f),new Vector3(.7f,1.7f,1.5f));Find<ThermologyLab>().ResetRun();break;
            case "InclinedPlane": title="PLANO INCLINADO";View(new Vector3(3,4,-6.5f),new Vector3(3,3,0));Time.timeScale=.5f;Find<InclinedPlaneLab>().ResetLab();break;
            case "ObliqueLaunch":title="LANÇAMENTO OBLÍQUO";View(new Vector3(3,3.5f,-6.5f),new Vector3(3,2,0));Time.timeScale=.5f;Find<ObliqueLaunchLab>().ClearRemoteHistory();break;
            case "Gravitation":title="GRAVITAÇÃO";var g=Find<GravitationLab>();var center=g.solar.transform.position;View(center+new Vector3(0,13,-22),center);g.panel.timeSpeeds[2].onClick.Invoke();if(g.SolarPaused)g.Pause();break;
            case "ResistorCircuits":title="ASSOCIAÇÃO DE RESISTORES";View(new Vector3(0,4,-6),new Vector3(0,1.4f,1));WireCircuit();break;
        }
        var canvas=new GameObject("Temporary recording captions",typeof(Canvas),typeof(CanvasScaler));
        var cv=canvas.GetComponent<Canvas>();cv.renderMode=RenderMode.ScreenSpaceCamera;cv.worldCamera=camera;cv.planeDistance=.5f;cv.sortingOrder=100;
        var scaler=canvas.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1920,1080);
        var panel=new GameObject("Caption background",typeof(RectTransform),typeof(Image));panel.transform.SetParent(canvas.transform,false);
        var rect=(RectTransform)panel.transform;rect.anchorMin=new Vector2(0,0);rect.anchorMax=new Vector2(1,0);rect.pivot=new Vector2(.5f,0);rect.sizeDelta=new Vector2(0,105);panel.GetComponent<Image>().color=new Color(.015f,.035f,.09f,.94f);
        var text=new GameObject("Caption",typeof(RectTransform),typeof(TextMeshProUGUI));text.transform.SetParent(panel.transform,false);
        caption=text.GetComponent<TMP_Text>();caption.font=Resources.FindObjectsOfTypeAll<TMP_FontAsset>().FirstOrDefault(f=>f.name.Contains("Montserrat")&&!f.name.Contains("Bold"))??TMP_Settings.defaultFontAsset;
        caption.fontSize=26;caption.color=Color.white;caption.alignment=TextAlignmentOptions.MidlineLeft;
        var tr=(RectTransform)text.transform;tr.anchorMin=Vector2.zero;tr.anchorMax=Vector2.one;tr.offsetMin=new Vector2(35,8);tr.offsetMax=new Vector2(-35,-8);
        caption.text=title+"\nDemonstração do laboratório em funcionamento";
        Directory.CreateDirectory(Path.GetDirectoryName(output));
        var settings=ScriptableObject.CreateInstance<RecorderControllerSettings>();
        var movie=ScriptableObject.CreateInstance<MovieRecorderSettings>();movie.name=name;movie.Enabled=true;
        movie.EncoderSettings=new CoreEncoderSettings{EncodingQuality=CoreEncoderSettings.VideoEncodingQuality.High,Codec=CoreEncoderSettings.OutputCodec.MP4};
        movie.CaptureAlpha=false;movie.CaptureAudio=false;
        movie.ImageInputSettings=new CameraInputSettings{Source=ImageSource.MainCamera,OutputWidth=1920,OutputHeight=1080,CaptureUI=true};movie.OutputFile=output;
        settings.AddRecorderSettings(movie);settings.SetRecordModeToManual();settings.FrameRate=30;
        recorder=new RecorderController(settings);recorder.PrepareRecording();if(!recorder.StartRecording())throw new Exception("Recorder did not start.");
        firstFrame=Time.frameCount;lastFrame=-1;Status="Recording "+name;
        EditorApplication.update-=Tick;EditorApplication.update+=Tick;
    }
    static void View(Vector3 position,Vector3 target){camera.transform.SetPositionAndRotation(position,Quaternion.LookRotation(target-position));}
    static void Say(string text){caption.text=text;}
    static bool At(float t,float threshold,int next){if(t<threshold||stage>=next)return false;stage=next;return true;}
    static void Tick()
    {
        if(!EditorApplication.isPlaying){EditorApplication.update-=Tick;return;}
        if(Time.frameCount==lastFrame)return;lastFrame=Time.frameCount;
        float t=(Time.frameCount-firstFrame)/30f;
        try
        {
            Status=$"Recording {lab}: {t:0.0}/{duration} s";
            switch(lab)
            {
                case "Waves":
                    var w=Find<WavesLab>();
                    if(t>2&&t<2.6f){w.Pull(.2f*Mathf.Sin((t-2)/.6f*Mathf.PI));Say("ONDULATÓRIA · Pulso\nPerturbação da corda, propagação e reflexão na extremidade.");}
                    if(At(t,9,1)){w.ResetRun();w.RequestMode(1);w.frequencyKnob.value=.7f;w.powerSwitch.value=true;Say("ONDULATÓRIA · Onda periódica\nGerador ligado: frequência e amplitude controlam a oscilação.");}
                    if(At(t,21,2)){w.ResetRun();w.RequestMode(2);w.frequencyKnob.value=(.75f-.15f)/.85f;w.powerSwitch.value=true;w.TogglePause();for(int i=0;i<1000;i++)w.StepOnce();w.TogglePause();Say("ONDULATÓRIA · Onda estacionária\nRegime previamente estabilizado: observe os nós e ventres.");}
                    if(At(t,31,3)){w.TogglePause();w.SelectPoint(80);Say("ONDULATÓRIA · Análise pausada\nSeleção de um ponto da corda para observar seu movimento no gráfico.");}break;
                case "Thermology":
                    var h=Find<ThermologyLab>();
                    if(At(t,2,1)){h.ChooseMode(0);h.StartRun();Say("TERMOLOGIA · Transformação isotérmica\nMovimento do pistão: volume e pressão variam, temperatura constante.");}
                    if(t>2&&t<10)h.MovePiston(Mathf.Lerp(.875f,1.4f,Mathf.Sin((t-2)/8*Mathf.PI)));
                    if(At(t,11,2)){h.ResetRun();h.ChooseMode(2);h.StartRun();h.knob.value=1;Say("TERMOLOGIA · Transformação isométrica\nAquecimento com volume fixo: maior agitação molecular e pressão.");}
                    if(At(t,21,3)){h.ResetRun();h.ChooseMode(1);h.loadSocket.interactionManager.SelectEnter((IXRSelectInteractor)h.loadSocket,(IXRSelectInteractable)h.weights[1]);h.StartRun();h.knob.value=1;Say("TERMOLOGIA · Transformação isobárica\nAquecimento sob carga: expansão do gás e trabalho do pistão.");}break;
                case "InclinedPlane":
                    var p=Find<InclinedPlaneLab>();if(At(t,2,1)){p.Release();Say("PLANO INCLINADO\nCaixa liberada: observe os vetores das forças durante o movimento.");}
                    if(At(t,5,2)){p.Pause();Say("PLANO INCLINADO · Pausa para análise\nForças representadas no bloco, sem réplicas temporais.");}
                    if(At(t,12,3)){p.Pause();Say("PLANO INCLINADO · Movimento retomado\nDeslizamento da caixa com vetores de peso, normal e atrito.");}if(At(t,23,4)){p.ResetLab();p.Release();Say("PLANO INCLINADO\nReinício do experimento · reprodução em câmera lenta (0,5×).");}break;
                case "ObliqueLaunch":
                    var o=Find<ObliqueLaunchLab>();if(At(t,2,1)||At(t,12,2)||At(t,22,3)){float angle=stage==1?45:stage==2?60:35;o.BeginRemoteLaunch(new LaunchParameters(angle,8,1,1,9.81f));Say($"LANÇAMENTO OBLÍQUO · Ângulo {angle}°\nTrajetória e vetores · velocidade inicial 8 m/s · câmera lenta (0,5×).");}break;
                case "Gravitation":
                    var g=Find<GravitationLab>();if(At(t,2,1))Say("GRAVITAÇÃO · Observatório\nOito planetas em órbita · tempo acelerado · tamanhos didáticos.");
                    if(At(t,19,2)){var body=g.solar.planets[4];var from=(g.solar.transform.position-body.position).normalized;var right=Vector3.Cross(Vector3.up,-from);View(body.position+from*4+Vector3.up,body.position+right*.7f);Find<PlanetInspection>().Focus(4);Say("GRAVITAÇÃO · Júpiter selecionado\nÓrbitas pausadas para leitura das informações do planeta.");}break;
                case "ResistorCircuits":
                    var c=Find<CircuitLab>();var s=c.parts.First(x=>x.role==ElectricalRole.Switch);
                    if(At(t,3,1)||At(t,24,3)){SetSwitch(s,true);Say("ASSOCIAÇÃO DE RESISTORES · Circuito fechado\nCorrente nos fios, lâmpada acesa e leituras dos instrumentos.");}
                    if(At(t,19,2)){SetSwitch(s,false);Say("ASSOCIAÇÃO DE RESISTORES · Circuito aberto\nAlavanca desligada: a corrente cessa e a lâmpada apaga.");}break;
            }
            if(t>=duration){recorder.StopRecording();EditorApplication.update-=Tick;Time.timeScale=1;Status="Complete "+lab;Debug.Log(Status);}
        }
        catch(Exception ex){recorder.StopRecording();EditorApplication.update-=Tick;Time.timeScale=1;Status="Failed: "+ex;Debug.LogException(ex);}
    }
    static void SetSwitch(CircuitPart part,bool value)
    {
        var lever=part.GetComponentsInChildren<MonoBehaviour>().FirstOrDefault(x=>x.GetType().Name.Contains("Lever"));
        var prop=lever?.GetType().GetProperty("value");if(prop!=null)prop.SetValue(lever,value);else if(value)part.CloseSwitch();else part.OpenSwitch();
    }
    static void WireCircuit()
    {
        var c=Find<CircuitLab>();CircuitPart Part(ElectricalRole role)=>c.parts.First(x=>x.role==role);
        var source=Part(ElectricalRole.Source);var sw=Part(ElectricalRole.Switch);var r=Part(ElectricalRole.Resistor);var a=Part(ElectricalRole.Ammeter);var lamp=Part(ElectricalRole.Lamp);var v=Part(ElectricalRole.Voltmeter);var j=c.parts.Where(x=>x.role==ElectricalRole.Junction).ToArray();
        var pairs=new[]{(source.a,sw.a),(sw.b,r.a),(r.b,a.a),(a.b,j[0].a),(j[0].b,lamp.a),(lamp.b,j[1].a),(j[1].b,source.b),(v.a,j[0].additionalTerminals[0]),(v.b,j[1].additionalTerminals[0])};
        for(int i=0;i<pairs.Length;i++){Connect(c.cables[i].first,pairs[i].Item1);Connect(c.cables[i].second,pairs[i].Item2);}SetSwitch(sw,false);c.Recalculate();
    }
    static void Connect(CableEnd end,CircuitTerminal terminal)
    {
        end.transform.position=terminal.transform.position;
        var socket=terminal.GetComponent<XRSocketInteractor>();socket.interactionManager.SelectEnter((IXRSelectInteractor)socket,(IXRSelectInteractable)end.GetComponent<XRGrabInteractable>());
    }
}



