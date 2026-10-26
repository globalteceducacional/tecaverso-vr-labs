using System;
using System.Linq;
using System.Globalization;
using Tecaverso.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Tecaverso.Labs.Waves
{
    public sealed class WavesLab:MonoBehaviour
    {
        public Transform apparatus,rod,freeRing,switchHandle;
        public MeshFilter rope;
        public WaveSlide pull,cursorA,cursorB;
        public WaveKnob amplitudeKnob,frequencyKnob;
        public WaveSwitch powerSwitch;
        public WavePoint[] points;
        public Transform[] markers;
        public GameObject ruler,knobAssembly,pullAssembly,fixedEnd,freeEnd,absorber;
        public GameObject controlsPage,analysisPage,dialog,endRow;
        public Button[] modeButtons,endButtons;
        public Button pauseButton,stepButton,resetButton,backButton,confirmButton,cancelButton,rulerButton,pointsButton,nodesButton,lowerButton,raiseButton;
        public TMP_Text instruction,metrics,status,measurement,pointLabel,amplitudeLabel,frequencyLabel,switchLabel,dialogText,graphScale,endLabel;
        public WaveTrace trace;
        public StringWaveModel Model {get;}=new();
        public int Mode {get;private set;}
        public bool Modal {get;private set;}
        public bool Paused {get;private set;}
        public bool Powered {get;private set;}
        public bool CanDrive=>!Paused&&!Modal;
        public float Amplitude {get;private set;}=.15f;
        public float Frequency {get;private set;}=.5f;
        public float DriverDisplacement {get;private set;}
        public bool PatternStable {get;private set;}
        int selected=120,pendingMode=-1;
        bool dirtyRun,showPoints=true,showRuler=true,showNodes=true,setting;
        double phase,accumulator,changedAt,windowTime;
        float uiTimer;
        double[] envelope=new double[StringWaveModel.Count],oldEnvelope=new double[StringWaveModel.Count];
        int stableWindows;
        readonly float[][] history=new float[201][];
        readonly float[] historyTimes=new float[201];
        int historyHead,historyCount;
        double lastSample;
        Mesh mesh; Vector3[] vertices,normals;int[] triangles;
        static readonly CultureInfo Pt=CultureInfo.GetCultureInfo("pt-BR");
        static string N(double v,string format="0.00")=>v.ToString(format,Pt);
        void Awake()
        {
            for(int i=0;i<history.Length;i++)history[i]=new float[StringWaveModel.Count];
            for(int i=0;i<3;i++){int j=i;modeButtons[i].onClick.AddListener(()=>RequestMode(j));endButtons[i].onClick.AddListener(()=>SetEnd(j));}
            amplitudeKnob.onValueChange.AddListener(SetAmplitude);frequencyKnob.onValueChange.AddListener(SetFrequency);
            powerSwitch.onLeverActivate.AddListener(()=>SetGenerator(true));
            powerSwitch.onLeverDeactivate.AddListener(()=>SetGenerator(false));
            pauseButton.onClick.AddListener(TogglePause);stepButton.onClick.AddListener(StepOnce);resetButton.onClick.AddListener(ResetRun);
            backButton.onClick.AddListener(()=>OpenDialog(-1));confirmButton.onClick.AddListener(Confirm);cancelButton.onClick.AddListener(Cancel);
            rulerButton.onClick.AddListener(()=>{showRuler=!showRuler;ruler.SetActive(showRuler);Present();});
            pointsButton.onClick.AddListener(()=>{showPoints=!showPoints;foreach(var p in points)p.gameObject.SetActive(showPoints);Present();});
            nodesButton.onClick.AddListener(()=>{showNodes=!showNodes;Present();});
            lowerButton.onClick.AddListener(()=>Height(-.1f));raiseButton.onClick.AddListener(()=>Height(.1f));
            UIVisibility.Set(dialog,false);CreateMesh();ResetRun();
        }
        void CreateMesh()
        {
            const int sides=8;vertices=new Vector3[StringWaveModel.Count*sides];normals=new Vector3[vertices.Length];triangles=new int[(StringWaveModel.Count-1)*sides*6];
            int k=0;for(int i=0;i<StringWaveModel.Count-1;i++)for(int j=0;j<sides;j++){int a=i*sides+j,b=i*sides+(j+1)%sides,c=b+sides,d=a+sides;triangles[k++]=a;triangles[k++]=b;triangles[k++]=c;triangles[k++]=a;triangles[k++]=c;triangles[k++]=d;}
            mesh=new Mesh{name="Dynamic wave tube"};mesh.MarkDynamic();mesh.vertices=vertices;mesh.triangles=triangles;rope.sharedMesh=mesh;
        }
        void OnDestroy(){if(mesh!=null)Destroy(mesh);}
        void Update()
        {
            if(CanDrive){accumulator+=Math.Min(Time.deltaTime,.25f);while(accumulator>=StringWaveModel.Dt){Advance();accumulator-=StringWaveModel.Dt;}}
            Draw();uiTimer+=Time.unscaledDeltaTime;if(uiTimer>.1f){uiTimer=0;Present();}
        }
        void Advance()
        {
            if(Mode==0){if(!pull.isSelected)DriverDisplacement=Mathf.MoveTowards(DriverDisplacement,0,.65f*(float)StringWaveModel.Dt);}
            else
            {
                phase+=2*Math.PI*Frequency*StringWaveModel.Dt;
                float target=Powered?Amplitude*(float)Math.Sin(phase):0;
                DriverDisplacement=Mathf.MoveTowards(DriverDisplacement,target,2*(float)StringWaveModel.Dt);
            }
            Model.Step(DriverDisplacement,Mode==2);
            for(int i=0;i<envelope.Length;i++)envelope[i]=Math.Max(envelope[i],Math.Abs(Model[i]));
            windowTime+=StringWaveModel.Dt;
            if(windowTime>=Math.Max(2,2/Frequency))
            {
                double difference=0,peak=0;for(int i=0;i<envelope.Length;i++){difference=Math.Max(difference,Math.Abs(envelope[i]-oldEnvelope[i]));peak=Math.Max(peak,envelope[i]);}
                stableWindows=difference<Math.Max(.006,peak*.07)&&peak>.035?stableWindows+1:0;
                PatternStable=Mode==2&&Powered&&Model.Time-changedAt>12&&stableWindows>=2;
                Array.Copy(envelope,oldEnvelope,envelope.Length);Array.Clear(envelope,0,envelope.Length);windowTime=0;
            }
            if(Model.Time-lastSample>=.04-1e-9)
            {
                lastSample=Model.Time;historyTimes[historyHead]=(float)Model.Time;
                for(int i=0;i<StringWaveModel.Count;i++)history[historyHead][i]=(float)Model[i];
                historyHead=(historyHead+1)%history.Length;historyCount=Math.Min(historyCount+1,history.Length);
                trace.Record((float)Model.Time,(float)Model[selected]);
            }
        }
        public void Pull(float value){if(CanDrive&&Mode==0){DriverDisplacement=Mathf.Clamp(value,-.3f,.3f);dirtyRun=true;}}
        public void ToggleGenerator(){SetGenerator(!Powered);}
        public void SetGenerator(bool on)
        {
            if(setting)return;
            if(!CanDrive||Mode==0){setting=true;powerSwitch.value=Powered;setting=false;return;}
            Powered=on;setting=true;powerSwitch.value=Powered;setting=false;dirtyRun=true;InvalidatePattern();Present();
        }
        public void SetAmplitude(float v){if(setting||!CanDrive||Mode==0)return;Amplitude=Mathf.Lerp(.03f,.2f,v);InvalidatePattern();Present();}
        public void SetFrequency(float v){if(setting||!CanDrive||Mode==0)return;Frequency=Mathf.Round(Mathf.Lerp(.15f,1f,v)*100)/100;InvalidatePattern();Present();}
        void InvalidatePattern(){changedAt=Model.Time;PatternStable=false;stableWindows=0;windowTime=0;Array.Clear(envelope,0,envelope.Length);Array.Clear(oldEnvelope,0,oldEnvelope.Length);}
        public void RequestMode(int mode)
        {if(Modal||mode==Mode||mode<0||mode>2)return;if(dirtyRun)OpenDialog(mode);else ApplyMode(mode);}
        void ApplyMode(int mode){Release();Mode=mode;Model.End=mode==1?WaveEnd.Absorbing:WaveEnd.Fixed;ResetRun();}
        public void SetEnd(int end){if(Modal||dirtyRun||Mode==2||end<0||end>2)return;Model.End=(WaveEnd)end;ResetRun();}
        public void TogglePause(){if(Modal)return;Release();Paused=!Paused;Present();}
        public void StepOnce(){if(!Paused||Modal)return;for(int i=0;i<25;i++)Advance();Draw();Present();}
        public void ResetRun()
        {
            Release();Model.Reset();Paused=Powered=dirtyRun=false;phase=accumulator=0;DriverDisplacement=0;InvalidatePattern();trace.Clear();historyHead=historyCount=0;lastSample=0;
            setting=true;powerSwitch.value=false;amplitudeKnob.value=Mathf.InverseLerp(.03f,.2f,Amplitude);frequencyKnob.value=Mathf.InverseLerp(.15f,1,Frequency);setting=false;
            pull.coordinate=0;cursorA.coordinate=1;cursorB.coordinate=3;Present();
        }
        public void SelectPoint(int index)
        {
            if(Modal)return;selected=Mathf.Clamp(index,0,StringWaveModel.Count-1);trace.Clear();
            for(int i=0;i<historyCount;i++){int slot=(historyHead-historyCount+i+history.Length)%history.Length;trace.Record(historyTimes[slot],history[slot][selected]);}
            Present();
        }
        void Height(float delta){if(Modal)return;Release();var p=apparatus.localPosition;p.y=Mathf.Clamp(p.y+delta,.85f,1.55f);apparatus.localPosition=p;Present();}
        void Release()
        {
            foreach(var item in new XRBaseInteractable[]{pull,amplitudeKnob,frequencyKnob,cursorA,cursorB,powerSwitch})
                if(item!=null&&item.interactionManager!=null)foreach(var hand in item.interactorsSelecting.ToArray())item.interactionManager.SelectExit(hand,item);
        }
        void OpenDialog(int mode){Release();pendingMode=mode;Modal=true;dialogText.text=mode<0?"Voltar ao Hub?\nO ensaio será descartado.":"Trocar a experiência?\nO ensaio será reiniciado.";UIVisibility.Set(dialog,true);UIVisibility.SetInteraction(controlsPage,false);UIVisibility.SetInteraction(analysisPage,false);}
        void Cancel(){Modal=false;UIVisibility.Set(dialog,false);UIVisibility.SetInteraction(controlsPage,true);UIVisibility.SetInteraction(analysisPage,true);}
        void Confirm()
        {
            if(pendingMode>=0){int mode=pendingMode;Cancel();ApplyMode(mode);return;}
            const string path="Assets/_Project/Hub/Scenes/Hub.unity";
            if(Application.CanStreamedLevelBeLoaded(path))SceneManager.LoadSceneAsync(path);else{Cancel();status.text="Hub indisponível.";}
        }
        void Draw()
        {
            for(int i=0;i<StringWaveModel.Count;i++)
            {
                var tangent=new Vector3(2*(float)StringWaveModel.Dx,(float)(Model[Math.Min(i+1,240)]-Model[Math.Max(i-1,0)]),0).normalized;
                var up=new Vector3(-tangent.y,tangent.x,0);
                for(int j=0;j<8;j++){float a=j*Mathf.PI/4;int k=i*8+j;var n=up*Mathf.Cos(a)+Vector3.forward*Mathf.Sin(a);vertices[k]=new Vector3(i*(float)StringWaveModel.Dx,(float)Model[i],0)+n*.022f;normals[k]=n;}
            }
            mesh.vertices=vertices;mesh.normals=normals;mesh.RecalculateBounds();
            foreach(var p in points)p.transform.localPosition=new Vector3(p.sampleIndex*(float)StringWaveModel.Dx,(float)Model[p.sampleIndex],0);
            rod.localPosition=new Vector3(Mode==2?.3f:0,DriverDisplacement,-.08f);
            freeRing.localPosition=new Vector3(6,(float)Model[240],0);
            int used=0;
            if(showNodes&&PatternStable)
            {
                double peak=oldEnvelope.Max();
                for(int i=4;i<237&&used<markers.Length;i++)
                {
                    bool node=oldEnvelope[i]<peak*.12&&oldEnvelope[i]<=oldEnvelope[i-1]&&oldEnvelope[i]<oldEnvelope[i+1];
                    bool belly=oldEnvelope[i]>peak*.65&&oldEnvelope[i]>=oldEnvelope[i-1]&&oldEnvelope[i]>oldEnvelope[i+1];
                    if(!node&&!belly)continue;
                    var m=markers[used++];m.gameObject.SetActive(true);m.localPosition=new Vector3(i*(float)StringWaveModel.Dx,belly?(float)oldEnvelope[i]+.17f:.15f,0);m.GetComponent<TMP_Text>().text=node?"NÓ":"VENTRE";i+=5;
                }
            }
            for(int i=used;i<markers.Length;i++)markers[i].gameObject.SetActive(false);
        }
        public void Present()
        {
            if(instruction==null)return;
            instruction.text=Mode==0?"Puxe o pegador azul para cima e solte.\nO ponto se move com a onda ou apenas oscila?":Mode==1?"Ligue o interruptor e gire uma manopla por vez.\nEspere a onda se estabelecer antes de medir.":"Ligue o gerador e procure ressonâncias.\nExperimente 0,25 · 0,50 · 0,75 Hz.";
            metrics.text=Mode==0?"CORDA · 6,00 m\nVELOCIDADE · 3,00 m/s\nTensão: 9 N · densidade linear: 1 kg/m":$"GERADOR  A = {N(Amplitude)} m · f = {N(Frequency)} Hz\nT = {N(1/Frequency)} s · v = 3,00 m/s\nλ = v/f = {N(3/Frequency)} m (regime periódico)";
            status.text=Paused?"PAUSADO · medição e seleção disponíveis":Mode==0?"PULSO · levante e solte o puxador":!Powered?"GERADOR DESLIGADO · ondas podem continuar":Mode==2?(PatternStable?"PADRÃO ESTÁVEL · observe nós e ventres":"FORMANDO O PADRÃO · aguarde estabilizar"):Model.Time-changedAt<4?"TRANSIENTE · frequências podem coexistir":"ONDA PERIÓDICA · compare cristas";
            amplitudeLabel.text=$"AMPLITUDE\n{N(Amplitude)} m";frequencyLabel.text=$"FREQUÊNCIA\n{N(Frequency)} Hz";switchLabel.text=Mode==0?"GERADOR · MODO PULSO":Powered?"GERADOR LIGADO":"GERADOR DESLIGADO";
            measurement.text=$"CURSORES · Δx = {N(Math.Abs(cursorA.coordinate-cursorB.coordinate))} m\nPosicione-os sob duas cristas consecutivas.";
            pointLabel.text=$"PONTO x = {N(selected*StringWaveModel.Dx)} m\ny = {N(Model[selected])} m · tempo {N(Model.Time)} s";
            graphScale.text=$"DESLOCAMENTO (m) · ±{N(trace.Range)}\nTEMPO (s) · últimos 8 s, presente à direita";
            endLabel.text="EXTREMIDADE · "+new[]{"FIXA","LIVRE","ABSORVENTE"}[(int)Model.End];
            fixedEnd.SetActive(Model.End==WaveEnd.Fixed);freeEnd.SetActive(Model.End==WaveEnd.Free);absorber.SetActive(Model.End==WaveEnd.Absorbing);
            knobAssembly.SetActive(Mode!=0);pullAssembly.SetActive(Mode==0);
            UIVisibility.Set(endRow,Mode!=2);endRow.GetComponent<LayoutElement>().ignoreLayout=Mode==2;
            for(int i=0;i<3;i++){modeButtons[i].GetComponent<Image>().color=i==Mode?new Color(.15f,.31f,.63f):new Color(.45f,.45f,.58f);endButtons[i].interactable=!dirtyRun;endButtons[i].GetComponentInChildren<TMP_Text>().text=(i==(int)Model.End?"✓ ":"")+new[]{"FIXA","LIVRE","ABSORVENTE"}[i];}
            pauseButton.GetComponentInChildren<TMP_Text>().text=Paused?"CONTINUAR":"PAUSAR";stepButton.interactable=Paused;
            rulerButton.GetComponentInChildren<TMP_Text>().text="RÉGUA "+(showRuler?"✓":"—");pointsButton.GetComponentInChildren<TMP_Text>().text="PONTOS "+(showPoints?"✓":"—");nodesButton.GetComponentInChildren<TMP_Text>().text="NÓS "+(showNodes?"✓":"—");nodesButton.interactable=Mode==2;
        }
    }
}
