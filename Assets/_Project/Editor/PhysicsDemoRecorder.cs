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
    static readonly System.Collections.Generic.List<(Camera camera,bool enabled,string tag)> previousCameras=new();
    public static string Status { get; private set; }="Idle";
    static T Find<T>() where T:UnityEngine.Object => UnityEngine.Object.FindFirstObjectByType<T>();
    public static void Begin(string name, string output)
    {
        if(!EditorApplication.isPlaying) throw new InvalidOperationException("Enter Play Mode first.");
        lab=name; stage=0; duration=name=="Reactions"?ReactionsDemo.Duration:name=="PeriodicExplorer"?PeriodicExplorerDemo.Duration:name=="PeriodicBonds"?64:name=="Waves"?36:name.StartsWith("Math")?34:30;
        foreach(var c in UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsSortMode.None)){previousCameras.Add((c,c.enabled,c.tag));c.enabled=false;if(c.CompareTag("MainCamera"))c.tag="Untagged";}
        camera=new GameObject("Temporary recording camera").AddComponent<Camera>();camera.tag="MainCamera";
        camera.nearClipPlane=.05f;camera.farClipPlane=150;camera.fieldOfView=65;
        camera.gameObject.AddComponent<UniversalAdditionalCameraData>().renderPostProcessing=true;
        View(new Vector3(0,2,-6.6f),new Vector3(0,1.45f,1));
        string title=name;
        switch(name)
        {
            case "AtomicModels": title="MODELOS ATÔMICOS";AtomicModelsDemo.Reset();duration=AtomicModelsDemo.Duration;break;
            case "Reactions": title="REAÇÕES QUÍMICAS";ReactionsDemo.Reset();duration=ReactionsDemo.Duration;View(new Vector3(-.6f,2.5f,-3.1f),new Vector3(0,1.4f,.5f));break;
            case "PeriodicExplorer": title="TABELA PERIÓDICA 3D";PeriodicExplorerDemo.Reset();View(new Vector3(-.6f,3,-4.8f),new Vector3(0,2.9f,4.2f));break;
            case "PeriodicBonds": title="TABELA PERIÓDICA E LIGAÇÕES";PeriodicBondsDemo.Reset();View(new Vector3(0,2.5f,-1.3f),new Vector3(0,2.5f,4.65f));break;
            case "Waves": title="ONDULATÓRIA";Find<WavesLab>().ResetRun();break;
            case "Thermology": title="TERMOLOGIA";View(new Vector3(.7f,2.6f,-4.6f),new Vector3(.7f,1.7f,1.5f));Find<ThermologyLab>().ResetRun();break;
            case "InclinedPlane": title="PLANO INCLINADO";View(new Vector3(3,4,-6.5f),new Vector3(3,3,0));Time.timeScale=.5f;Find<InclinedPlaneLab>().ResetLab();break;
            case "ObliqueLaunch":title="LANÇAMENTO OBLÍQUO";View(new Vector3(3,3.5f,-6.5f),new Vector3(3,2,0));Time.timeScale=.5f;Find<ObliqueLaunchLab>().ClearRemoteHistory();break;
            case "Gravitation":title="GRAVITAÇÃO";var g=Find<GravitationLab>();var center=g.solar.transform.position;View(center+new Vector3(0,13,-22),center);g.panel.timeSpeeds[2].onClick.Invoke();if(g.SolarPaused)g.Pause();break;
            case "ResistorCircuits":title="ASSOCIAÇÃO DE RESISTORES";View(new Vector3(0,4,-6),new Vector3(0,1.4f,1));WireCircuit();break;
            case "MathCartesian":title="ESPAÇO CARTESIANO";View(new Vector3(-3.6f,4.1f,-2.5f),new Vector3(0,2.7f,1.5f));break;
            case "MathSolids":title="FORMAS GEOMÉTRICAS";View(new Vector3(0,3.2f,-4.8f),new Vector3(0,1.5f,.3f));break;
            case "MathPlanar":title="GEOMETRIA PLANA";View(new Vector3(0,2.5f,-.5f),new Vector3(-5.4f,1.55f,-1f));break;
            case "MathFunctions":title="FUNÇÕES E GRÁFICOS";View(new Vector3(0,2.8f,-.8f),new Vector3(0,2.4f,4.6f));break;
            case "MathProbability":title="PROBABILIDADE E ESTATÍSTICA";View(new Vector3(-.7f,2.4f,0),new Vector3(-4.8f,1.6f,0));break;
            case "MathFractions":title="FRAÇÕES E PROPORÇÕES";View(new Vector3(0,2.5f,-1),new Vector3(0,1.8f,2.5f));break;
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
    static void Finish(){if(recorder!=null&&recorder.IsRecording())recorder.StopRecording();EditorApplication.update-=Tick;if(lab=="AtomicModels")AtomicModelsDemo.Cleanup();if(lab=="Reactions")ReactionsDemo.Cleanup();Time.timeScale=1;if(camera)UnityEngine.Object.Destroy(camera.gameObject);if(caption)UnityEngine.Object.Destroy(caption.canvas.gameObject);foreach(var entry in previousCameras)if(entry.camera){entry.camera.tag=entry.tag;entry.camera.enabled=entry.enabled;}previousCameras.Clear();}
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
                case "AtomicModels": AtomicModelsDemo.Tick(t,View,Say);break;
                case "Reactions": ReactionsDemo.Tick(t,View,Say);break;
                case "PeriodicExplorer": PeriodicExplorerDemo.Tick(t,View,Say);break;
                case "PeriodicBonds": PeriodicBondsDemo.Tick(t,View,Say);break;
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
                case "MathCartesian":
                    var cart=Find<Tecaverso.Labs.Cartesian.CartesianLab>();
                    if(At(t,1,1)){cart.CreatePoint();cart.Model.Move(cart.SelectedPoint,new Vector3(-2,1,0));cart.Sync();Say("ESPAÇO CARTESIANO · Ponto A\nO ponto sai da origem e revela suas componentes x, y e z.");}
                    if(At(t,5,2)){cart.CreatePoint();cart.Model.Move(cart.SelectedPoint,new Vector3(2,1,0));cart.Sync();Say("ESPAÇO CARTESIANO · Ponto B\nDois pontos posicionados no espaço tridimensional.");}
                    if(At(t,8,3)){cart.Select(0,false);cart.BeginTool(Tecaverso.Labs.Cartesian.ConstructionKind.Line);cart.Select(1,false);Say("ESPAÇO CARTESIANO · Reta AB\nOs dois pontos determinam a reta.");}
                    if(At(t,13,4)){cart.CreatePoint();cart.Model.Move(cart.SelectedPoint,new Vector3(0,3,1));cart.Sync();cart.Select(0,false);cart.BeginTool(Tecaverso.Labs.Cartesian.ConstructionKind.Triangle);cart.Select(1,false);cart.Select(2,false);Say("ESPAÇO CARTESIANO · Superfície ABC\nUm terceiro ponto forma o triângulo no plano.");}
                    if(At(t,19,5)){cart.CreatePoint();cart.Model.Move(cart.SelectedPoint,new Vector3(0,1,3));cart.Sync();cart.Select(0,false);cart.BeginTool(Tecaverso.Labs.Cartesian.ConstructionKind.Tetrahedron);cart.Select(1,false);cart.Select(2,false);cart.Select(3,false);Say("ESPAÇO CARTESIANO · Sólido ABCD\nQuatro vértices não coplanares formam um tetraedro.");}
                    if(At(t,26,6)){cart.Model.Move(3,new Vector3(0,2,4));cart.Sync();Say("ESPAÇO CARTESIANO · Geometria dinâmica\nMover D atualiza o sólido e suas medidas.");}break;
                case "MathSolids":
                    var solids=Find<Tecaverso.Labs.Shapes.SolidGallery>();
                    if(At(t,2,1)){solids.verticesToggle.isOn=true;Say("FORMAS GEOMÉTRICAS · Vértices\nObserve os marcadores nos prismas e no cubo.");}
                    if(At(t,7,2)){solids.edgesToggle.isOn=true;Say("FORMAS GEOMÉTRICAS · Arestas\nConte os encontros entre faces planas.");}
                    if(At(t,12,3)){solids.rotationToggle.isOn=true;View(new Vector3(0,3.2f,1.7f),new Vector3(0,2.3f,4.25f));Say("FORMAS GEOMÉTRICAS · Poliedros\nA rotação revela pirâmide, tetraedro e octaedro.");}
                    if(At(t,20,4)){View(new Vector3(3.5f,3.2f,1.8f),new Vector3(4.4f,2.3f,4.25f));Say("FORMAS GEOMÉTRICAS · Mais faces\nDodecaedro, icosaedro e esfera: compare faces, arestas e vértices.");}
                    if(At(t,27,5)){solids.rotationToggle.isOn=false;View(new Vector3(0,5.2f,-2.3f),new Vector3(0,1.7f,2.4f));Say("FORMAS GEOMÉTRICAS · Galeria\nNomes e fórmulas de volume aparecem acima dos sólidos.");}break;
                case "MathPlanar":
                    var planar=Find<Tecaverso.Labs.PlanarGeometry.PlanarGallery>();
                    if(At(t,2,1)){planar.verticesToggle.isOn=true;Say("GEOMETRIA PLANA · Vértices\nMarcadores identificam os vértices das figuras planas.");}
                    if(At(t,7,2)){planar.contoursToggle.isOn=true;View(new Vector3(0,2.5f,-.5f),new Vector3(5.4f,1.55f,-1));Say("GEOMETRIA PLANA · Contornos\nCompare lados e perímetros na parede oposta.");}
                    if(At(t,13,3)){planar.dimensionsToggle.isOn=true;View(new Vector3(0,2.8f,1.3f),new Vector3(0,1.55f,5.45f));Say("GEOMETRIA PLANA · Medidas\nBase, altura e dimensões acompanham as fórmulas de área.");}
                    if(At(t,21,4)){View(new Vector3(1.1f,2.4f,1.2f),new Vector3(2.2f,1.55f,5.45f));Say("GEOMETRIA PLANA · Polígono côncavo\nO formato em L pode ser decomposto para calcular sua área.");}
                    if(At(t,27,5)){View(new Vector3(2.8f,2.3f,1.6f),new Vector3(4.4f,1.55f,5.45f));Say("GEOMETRIA PLANA · Círculo\nÁrea πr²; a borda curva não possui vértices ou arestas.");}break;
                case "MathFunctions":
                    var f=Find<Tecaverso.Labs.Functions.FunctionsLab>();
                    if(At(t,1,1)){f.SelectFamily(0);f.SetCoefficient(0,1);f.SetCoefficient(1,0);f.DragMarker(-2);Say("FUNÇÕES · Reta\nFunção afim: P percorre a curva e mostra suas coordenadas.");}
                    if(At(t,7,2)){f.RequestReference();f.SetCoefficient(0,2);f.SetCoefficient(1,1);f.DragMarker(1);Say("FUNÇÕES · Coeficientes e referência\nInclinação e intercepto mudam; a reta anterior fica como comparação.");}
                    if(At(t,14,3)){f.SelectFamily(1);f.SetCoefficient(0,1);f.SetCoefficient(1,0);f.SetCoefficient(2,-1);f.DragMarker(-1);Say("FUNÇÕES · Parábola\nRaízes em −1 e +1; observe o vértice e o ponto P.");}
                    if(At(t,21,4)){f.SetCoefficient(0,-1);f.SetCoefficient(2,2);f.DragMarker(0);Say("FUNÇÕES · Concavidade\nCoeficiente a negativo inverte a parábola; P vai ao vértice.");}
                    if(At(t,28,5)){f.DragMarker(1.4);Say("FUNÇÕES · Leitura do gráfico\nO deslocamento de P atualiza x, f(x) e as projeções nos eixos.");}break;
                case "MathProbability":
                    var probability=Find<Tecaverso.Labs.Probability.ProbabilityLab>();
                    if(At(t,1,1)){probability.stations[0].Single();Say("PROBABILIDADE · Moeda\nUm lançamento produz cara ou coroa.");}
                    if(At(t,5,2)){probability.stations[0].StartBatch();Say("PROBABILIDADE · Frequência\nVários lançamentos fazem as barras se aproximarem de 50%.");}
                    if(At(t,12,3)){View(new Vector3(0,2.5f,.1f),new Vector3(0,1.6f,4.8f));probability.stations[1].StartBatch();Say("ESTATÍSTICA · Dado\nAs frequências, média, mediana e moda são atualizadas.");}
                    if(At(t,21,4)){View(new Vector3(.7f,2.4f,0),new Vector3(4.8f,1.6f,0));probability.stations[2].Series.Configure(2,8,false);probability.stations[2].Refresh();probability.stations[2].Single();Say("PROBABILIDADE · Urna 2:8\nSem reposição: a chance seguinte muda após cada retirada.");}
                    if(At(t,26,5)){probability.stations[2].StartBatch();Say("PROBABILIDADE · Urna\nA composição restante e as chances são recalculadas.");}break;
                case "MathFractions":
                    var fractions=Find<Tecaverso.Labs.Fractions.FractionsLab>();
                    if(At(t,1,1)){DockFraction(fractions,1,0);Say("FRAÇÕES · Metade\nPegue a peça 1/2 e encaixe na linha A.");}
                    if(At(t,5,2)){DockFraction(fractions,3,1);DockFraction(fractions,3,1);fractions.equivalence.isOn=true;Say("FRAÇÕES · Equivalência\nDuas peças de 1/4 em B têm o mesmo comprimento que 1/2 em A.");}
                    if(At(t,12,3)){fractions.Session.ClearRows();fractions.SendMessage("RestoreRows");DockFraction(fractions,2,0);DockFraction(fractions,5,0);DockFraction(fractions,1,1);Say("FRAÇÕES · Soma\n1/3 + 1/6 = 1/2, comparado com a linha B.");}
                    if(At(t,19,4)){fractions.Session.ClearRows();fractions.SendMessage("RestoreRows");DockFraction(fractions,1,0);DockFraction(fractions,1,0);DockFraction(fractions,1,0);fractions.decimals.isOn=true;Say("FRAÇÕES · Imprópria\n3/2 corresponde a 1,5 inteiro ou 150%.");}
                    if(At(t,26,5)){View(new Vector3(.8f,2.7f,.4f),new Vector3(4,1.8f,.4f));DockFraction(fractions,9,2);DockFraction(fractions,9,2);DockFraction(fractions,10,3);DockFraction(fractions,10,3);DockFraction(fractions,10,3);Say("PROPORÇÕES · Receita 2:3\nDuas peças A e três B atingem a razão proposta.");}break;
            }
            if(t>=duration){Finish();Status="Complete "+lab;Debug.Log(Status);}
        }
        catch(Exception ex){Finish();Status="Failed: "+ex;Debug.LogException(ex);}
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
    static void DockFraction(Tecaverso.Labs.Fractions.FractionsLab lab,int kind,int dock)
    {
        var piece=lab.Pieces.First(p=>p.gameObject.activeSelf&&p.supply&&p.kind==kind);
        lab.Take(piece);piece.transform.position=lab.End(dock);if(!lab.Dock(piece,dock))throw new Exception("Fraction piece could not dock: "+kind+" to "+dock);
    }
}
