using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Content.Interaction;
using Tecaverso.Labs.Chemistry.AtomicModels;

// Editor-only recording script; all results come from the lab's interaction handlers.
public static class AtomicModelsDemo
{
    public const float Duration=112;
    static AtomicModelsLab lab;
    static readonly List<(float time,Action run)> steps=new();
    static int next;
    static XRSimpleInteractable pressed;
    static float releaseAt,clock;
    static LineRenderer pointer;
    static Material material;
    static XRDirectInteractor hand;
    static Action<Vector3,Vector3> view;
    static Action<string> say;
    static AtomicControl Control(string action,int kind=0,int delta=0)=>lab.GetComponentsInChildren<AtomicControl>(true).First(c=>c.action==action&&(action!="change"||(c.kind==kind&&c.delta==delta)));
    static void Press(AtomicControl c)
    {
        Release();pressed=c.GetComponent<XRSimpleInteractable>();
        pressed.firstHoverEntered.Invoke(new HoverEnterEventArgs());pressed.selectEntered.Invoke(new SelectEnterEventArgs());
        pointer.enabled=true;pointer.SetPosition(0,new Vector3(.55f,1.4f,-3));pointer.SetPosition(1,c.transform.position);
        releaseAt=clock+.22f;
    }
    static void Release(){if(pressed){pressed.selectExited.Invoke(new SelectExitEventArgs());pressed.lastHoverExited.Invoke(new HoverExitEventArgs());pressed=null;}if(pointer)pointer.enabled=false;}
    static void Front(){view(new Vector3(0,1.9f,-3.6f),new Vector3(0,1.3f,-1.1f));}
    static void Exhibit(string name)
    {
        var c=lab.GetComponentsInChildren<AtomicControl>(true).First(c=>c.action=="exhibit"&&c.exhibitTitle.StartsWith(name));
        Press(c);pointer.enabled=false;view(c.transform.position-c.transform.forward*4.4f+Vector3.up*.15f,c.transform.position+Vector3.up*.12f);
    }
    static void Add(float t,Action action)=>steps.Add((t,action));
    public static void Reset()
    {
        lab=UnityEngine.Object.FindFirstObjectByType<AtomicModelsLab>();if(!lab)throw new Exception("Open AtomicModels.");
        lab.ConfirmClear();lab.SetView(false);lab.CloseConsultation();steps.Clear();next=0;
        pointer=new GameObject("Temporary atomic demo pointer").AddComponent<LineRenderer>();material=new Material(Shader.Find("Universal Render Pipeline/Unlit"));material.color=new Color(.2f,.8f,1);pointer.sharedMaterial=material;pointer.positionCount=2;pointer.startWidth=.006f;pointer.endWidth=.003f;pointer.enabled=false;
        Add(0,()=>{Front();say("MODELOS ATÔMICOS · Oficina central e museus laterais\nDemonstração automatizada pelos controles da sala · funcionamento offline.");});
        Add(5,()=>say("MONTAGEM · Um toque adiciona uma partícula\nPrótons definem o elemento; nêutrons alteram A; elétrons alteram a carga."));
        for(int i=0;i<18;i++){int kind=i%3;Add(6+i*.45f,()=>Press(Control("change",kind,1)));}
        Add(15,()=>{if(lab.protons!=6||lab.neutrons!=6||lab.electrons!=6)throw new Exception("C12 failed");say("CARBONO-12 · 6 prótons, 6 nêutrons, 6 elétrons\nZ = 6 · A = 12 · carga zero. As órbitas são uma representação didática.");});
        Add(20,()=>{Press(Control("change",1,1));say("ISÓTOPO · Um nêutron a mais: carbono-13\nO elemento permanece carbono porque o número de prótons não mudou.");});
        Add(25,()=>{Press(Control("change",2,-1));say("ÍON · Retirando um elétron\nA placa passa a indicar cátion +1; o núcleo continua sendo carbono-13.");});
        Add(30,()=>{lab.GetComponentInChildren<XRLever>().value=true;say("NUVEM · Mesma montagem, outra representação\nNuvem qualitativa; não é uma previsão quantitativa de orbitais.");});
        Add(35,()=>{lab.GetComponentInChildren<XRLever>().value=false;hand=new GameObject("Temporary rotation hand").AddComponent<XRDirectInteractor>();var grab=lab.atomPivot.GetComponent<XRGrabInteractable>();grab.interactionManager.SelectEnter((IXRSelectInteractor)hand,(IXRSelectInteractable)grab);say("GIRAR · Selecionar e girar o modelo no lugar\nA posição do núcleo e os controles permanecem fixos.");});
        Add(41,()=>{var grab=lab.atomPivot.GetComponent<XRGrabInteractable>();grab.interactionManager.SelectExit((IXRSelectInteractor)hand,(IXRSelectInteractable)grab);UnityEngine.Object.Destroy(hand.gameObject);hand=null;Press(Control("recenter"));say("RECENTRALIZAR · Retorno suave à orientação inicial\nAs contagens permanecem iguais.");});
        Add(46,()=>{Press(Control("clear"));say("LIMPAR · Confirmação na própria estação\nO resultado cede espaço à decisão; os outros controles ficam bloqueados.");});
        Add(49,()=>{lab.confirmationRoot.GetComponentsInChildren<UnityEngine.UI.Button>().First(b=>b.name=="Cancelar").onClick.Invoke();say("CANCELAR · A montagem foi preservada");});
        Add(52,()=>{Press(Control("clear"));});
        Add(53,()=>{lab.confirmationRoot.GetComponentsInChildren<UnityEngine.UI.Button>().First(b=>b.name=="Limpar").onClick.Invoke();say("RECOMEÇAR · Nova montagem até o cálcio\nAdições aceleradas, pelos mesmos controles +/−.");});
        for(int i=0;i<60;i++){int kind=i%3;Add(54+i*.15f,()=>Press(Control("change",kind,1)));}
        Add(64,()=>{Release();if(lab.protons!=20||lab.neutrons!=20||lab.electrons!=20)throw new Exception("Ca40 failed");say("CÁLCIO-40 · Quatro camadas didáticas: 2, 8, 8 e 2\n20 prótons e 20 elétrons · estabilidade dos núcleos não é avaliada.");});
        Add(68,()=>{Press(Control("change",0,1));say("LIMITE · A oficina vai de hidrogênio a cálcio\nTentar acrescentar o 21º próton mantém a contagem e explica o limite.");});
        Add(72,()=>{Exhibit("Rutherford");say("ALA ESQUERDA · História dos modelos atômicos\nRutherford: proposta, evidência experimental, avanço e limite.");});
        Add(83,()=>{Exhibit("Modelo quântico");say("MODELO QUÂNTICO · A evolução da explicação\nImagem local e texto contextual; uma única ficha aberta por vez.");});
        Add(93,()=>{Exhibit("Próton");say("ALA DIREITA · Partículas do átomo\nPróton: dois quarks up e um down; glúons no esquema da interação forte.");});
        Add(104,()=>{lab.CloseConsultation();Front();say("EXPLORAÇÃO LIVRE · Oficina, história e partículas\nA montagem permanece disponível. Voltar ao Hub fica junto à entrada.");});
        steps.Sort((a,b)=>a.time.CompareTo(b.time));
    }
    public static void Tick(float t,Action<Vector3,Vector3> camera,Action<string> caption)
    {
        view=camera;say=caption;clock=t;if(pressed&&t>=releaseAt)Release();
        while(next<steps.Count&&t>=steps[next].time)steps[next++].run();
        if(hand)hand.transform.rotation=Quaternion.Euler(Mathf.Sin((t-35)*.7f)*22,(t-35)*20,0);
    }
    public static void Cleanup(){Release();if(hand)UnityEngine.Object.Destroy(hand.gameObject);if(pointer)UnityEngine.Object.Destroy(pointer.gameObject);if(material)UnityEngine.Object.Destroy(material);}
}
