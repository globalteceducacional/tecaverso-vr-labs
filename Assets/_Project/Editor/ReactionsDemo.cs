using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Tecaverso.Labs.Chemistry.Reactions;

// Editor-only recording: actual XR grabs, socket release and instrument transfer; no injected chemistry.
public static class ReactionsDemo
{
    sealed class Shot { public float seconds; public string caption; public Action begin; public Action<float> animate; }
    static readonly List<Shot> shots=new();
    static readonly List<string> evidence=new();
    static ReactionsLab lab; static ReactionsBench bench;
    static XRDirectInteractor hand; static ReagentTool held;
    static Action<Vector3,Vector3> view;
    static int previous;
    static Vector3 from; static Quaternion fromRotation;
    static double initialStock,initialVolume;
    public static float Duration=>shots.Sum(s=>s.seconds);
    public static string Evidence=>string.Join("\n",evidence);
    static ReagentTool Tool(string id,bool spoon=false)=>bench.instruments.Single(t=>t.reagentId==id&&t.spoon==spoon);
    static void Add(float sec,string caption,Action begin=null,Action<float> animate=null)=>shots.Add(new Shot{seconds=sec,caption=caption,begin=begin,animate=animate});
    static void Check(bool pass,string message){if(!pass)throw new Exception("Recording: "+message);evidence.Add("PASS: "+message);}
    static void Wide()=>view(new Vector3(-.6f,2.5f,-3.1f),new Vector3(0,1.45f,.6f));
    static void Close()=>view(new Vector3(-.65f,1.95f,-1.15f),new Vector3(0,1.55f,.5f));
    static void Panel(Transform t)=>view(t.position-t.forward*1.15f,t.position+Vector3.down*.04f);
    static void Grab(string id,bool spoon=false)
    {
        held=Tool(id,spoon);hand.transform.SetPositionAndRotation(held.transform.position,held.transform.rotation);
        hand.StartManualInteraction((IXRSelectInteractable)held);Remember();Check(held.Held,"XR grab "+id+(spoon?" spoon":""));
    }
    static void Remember(){from=hand.transform.position;fromRotation=hand.transform.rotation;}
    static void MoveRoot(Vector3 position,Quaternion rotation,float u)
    {u=Mathf.SmoothStep(0,1,Mathf.Clamp01(u));hand.transform.SetPositionAndRotation(Vector3.Lerp(from,position,u),Quaternion.Slerp(fromRotation,rotation,u));}
    static Vector3 RootForMouth(Vector3 mouth,Quaternion rotation)=>mouth-rotation*Vector3.Scale(held.mouth.localPosition,held.transform.lossyScale);
    static void Pose(Vector3 mouth,Quaternion rotation)=>hand.transform.SetPositionAndRotation(RootForMouth(mouth,rotation),rotation);
    static void Drop(bool home=true){if(!held)return;var t=held;if(hand.hasSelection)hand.EndManualInteraction();held=null;if(home)t.ReturnHome();}
    static void Dock(string id,int slot)
    {
        string name=ReactionWorkbench.Find(id).Name;
        Add(4,"ESCOLHER "+(slot==0?"A":"B")+" · "+name+"\nPegue o recipiente e solte no suporte. O par fica registrado para a dosagem.",()=>{Wide();Grab(id);},t=>
        {if(held){MoveRoot(bench.sockets[slot].attachTransform.position,Quaternion.identity,t/2.4f);if(t>2.9f)Drop(false);}});
        Add(2,slot==0?"PARCEIROS DISPONÍVEIS\nOs rótulos indicam quais reagentes podem completar este par.":"PAR PRONTO · Retire os recipientes para dosar\nA seleção permanece mesmo quando os suportes ficam vazios.",()=>
        {Check(bench.sockets[slot].interactablesSelected.Any(x=>x is ReagentTool r&&r.reagentId==id),"Socket accepted "+id);if(slot==1)Check(bench.Model.Ready,"Pair ready");});
    }
    static void Return()
    {
        Add(2,"DEVOLVER O INSTRUMENTO\nEstoque e conteúdo do béquer são preservados.",Remember,t=>{if(held){MoveRoot(held.home.position,held.home.rotation,t/1.5f);if(t>1.7f)Drop();}});
    }
    static void Pour(string id,float ml,string text,bool blocked=false)
    {
        Add(8,text,()=>{Close();Grab(id);initialStock=bench.Model.Stock(id);initialVolume=lab.Session.VolumeMl;},t=>
        {
            var mouth=lab.opening.position+Vector3.up*.3f;
            if(t<1.7f)MoveRoot(RootForMouth(mouth,Quaternion.identity),Quaternion.identity,t/1.7f);
            else if(blocked||initialStock-bench.Model.Stock(id)<ml)Pose(mouth,Quaternion.Euler(0,0,Mathf.Lerp(0,145,Mathf.Clamp01((t-1.7f)/1.0f))));
            else Pose(mouth,Quaternion.identity);
        });
        Add(1,blocked?"TERCEIRO REAGENTE BLOQUEADO\nNenhum jato, consumo de estoque ou alteração da mistura.":"DOSE TRANSFERIDA\nO resultado é calculado a partir da quantidade que realmente entrou.",()=>
        {if(blocked)Check(bench.Model.Stock(id)==initialStock&&lab.Session.VolumeMl==initialVolume,"Third reagent blocked without stock loss");else Check(initialStock-bench.Model.Stock(id)>=ml,"Transferred "+ml+" mL of "+id);});
        Return();
    }
    static void Powder(string id)
    {
        Add(3,"SÓLIDO · Coletar uma dose com a colher dedicada\nA concha retira até 1 g do pote do reagente escolhido.",()=>{Wide();Grab(id,true);},t=>MoveRoot(RootForMouth(Tool(id).collectionPoint.position,Quaternion.identity),Quaternion.identity,t/1.8f));
        Add(3,"LEVAR A COLHER AO BÉQUER\nO sólido é dosado fisicamente; o pote não despeja líquido.",()=>{Check(bench.Model.Spoon(id)>.9,"Spoon loaded "+id);Close();Remember();},t=>MoveRoot(RootForMouth(lab.opening.position+Vector3.up*.3f,Quaternion.identity),Quaternion.identity,t/2));
        Add(3,"INCLINAR E OBSERVAR\nA reação começa conforme o sólido entra em contato com a solução.",null,t=>Pose(lab.opening.position+Vector3.up*.3f,Quaternion.Euler(0,0,Mathf.Lerp(0,140,Mathf.Clamp01(t/.7f)))));
        Return();
    }
    static void Consult(string text)
    {
        Add(7,text,()=>{bench.consult.onClick.Invoke();Panel(bench.sheet.transform);Check(bench.SheetOpen,"Consultation opened");});
        Add(1,"FECHAR A FICHA\nA observação volta a ocupar o centro da experiência.",()=>{bench.close.onClick.Invoke();Wide();});
    }
    static void ResetPair()
    {
        Add(3,"NOVA MISTURA\nConfirme para descartar o conteúdo e repor os reagentes.",()=>{bench.reset.onClick.Invoke();Panel(bench.confirmation.transform);Check(bench.Modal,"Reset confirmation opened");});
        Add(3,"ESTOQUES REPOSTOS · Escolha outro par\nCada tentativa usa somente os dois reagentes selecionados.",()=>{bench.confirm.onClick.Invoke();Wide();});
    }
    static void Measure(bool supported)
    {
        Add(4,"TERMÔMETRO XR · Pegar e mergulhar a ponta\nA leitura aparece somente dentro do líquido.",()=>
        {
            Close();var p=bench.thermometer;hand.transform.SetPositionAndRotation(p.transform.position,p.transform.rotation);
            hand.StartManualInteraction((IXRSelectInteractable)p);Remember();Check(p.Held,"XR thermometer grab");
        },t=>MoveRoot(lab.opening.position+new Vector3(.10f,lab.receiver.SurfaceY-lab.opening.position.y-.015f,0)-bench.thermometer.tip.localPosition,Quaternion.identity,t/2.5f));
        Add(7,supported?"AQUECIMENTO · Temperatura inicial, atual e variação\nEstimativa ideal para HCl/NaOH: o calor depende da quantidade que reagiu.":"LIMITE EXPLÍCITO · Modelo térmico indisponível\nNeste par não há temperatura calculada; isso não significa ausência de calor.",()=>
        {
            var p=bench.thermometer;Check(p.Immersed,"Probe immersed");
            Check(supported?lab.Session.TemperatureC>25:p.display.text.Contains("indisponível"),"Thermal reading matches coverage");
            hand.EndManualInteraction();view(p.transform.position+new Vector3(-.20f,.10f,-.85f),p.transform.position+Vector3.down*.15f);
        });
        Add(3,"RETIRAR A SONDA\nFora do líquido, o visor deixa de mostrar a medida anterior.",()=>
        {
            var p=bench.thermometer;hand.transform.SetPositionAndRotation(p.transform.position,p.transform.rotation);
            hand.StartManualInteraction((IXRSelectInteractable)p);Remember();Close();
        },t=>MoveRoot(from+Vector3.up*.4f,Quaternion.identity,t/1.5f));
        Add(3,"DEVOLVER O TERMÔMETRO\nO instrumento pode ser reutilizado na próxima mistura.",()=>
        {Check(!bench.thermometer.Immersed,"Probe clears reading outside liquid");Remember();},t=>
        {if(hand.hasSelection){MoveRoot(bench.thermometer.home.position,bench.thermometer.home.rotation,t/2);if(t>2.4f){hand.EndManualInteraction();bench.thermometer.ReturnHome();}}});
    }
    public static void Reset()
    {
        Cleanup();shots.Clear();evidence.Clear();previous=-1;
        lab=UnityEngine.Object.FindFirstObjectByType<ReactionsLab>();bench=lab?lab.bench:null;
        if(!bench)throw new Exception("Open the migrated A/B Reactions scene.");
        bench.ResetAll();
        var go=new GameObject("Temporary recording XR hand");var col=go.AddComponent<SphereCollider>();col.isTrigger=true;col.radius=.035f;
        var rb=go.AddComponent<Rigidbody>();rb.isKinematic=true;rb.useGravity=false;
        hand=go.AddComponent<XRDirectInteractor>();hand.interactionManager=UnityEngine.Object.FindFirstObjectByType<XRInteractionManager>();
        Add(6,"REAÇÕES QUÍMICAS · 12 reagentes em três degraus\nDemonstração automatizada de interações XR reais. Legendas exclusivas deste vídeo.",Wide);
        Dock("cacl",0);Dock("carb",1);
        Pour("cacl",80,"1 · PRECIPITAÇÃO · Cloreto de cálcio\nRetire A, aproxime o bico e incline. Apenas um reagente ainda não forma precipitado.");
        Add(3,"PRIMEIRO REAGENTE NO BÉQUER\nObserve o nível de líquido antes da segunda adição.",()=>Check(lab.Session.VolumeMl>75&&lab.Session.ExtentMol==0,"No premature precipitate"));
        Pour("carb",100,"ADICIONAR CARBONATO DE SÓDIO\nA mistura forma carbonato de cálcio sólido, com partículas e depósito branco.");
        Add(4,"PRECIPITADO FORMADO\nO sólido branco é produto da reação; as quantidades dependem das doses.",()=>{Close();Check(lab.Session.ExtentMol>0&&lab.sediment.gameObject.activeSelf,"Precipitate formed");});
        Consult("CONSULTA OPCIONAL · Equação e quantidades\nConfira produtos, reagente limitante, excesso e perdas. A ficha não fica aberta durante a dosagem.");
        Pour("hcl",0,"TENTATIVA FORA DO PAR · Ácido clorídrico\nMesmo inclinado sobre o béquer, um terceiro reagente não entra nesta mistura.",true);
        Add(3,"CANCELAR TAMBÉM É POSSÍVEL\nNova mistura pede confirmação porque há material utilizado.",()=>{initialVolume=lab.Session.VolumeMl;bench.reset.onClick.Invoke();Panel(bench.confirmation.transform);});
        Add(3,"CANCELAMENTO · Conteúdo preservado\nCancelar permite continuar a mesma experiência.",()=>{bench.cancel.onClick.Invoke();Close();Check(lab.Session.VolumeMl==initialVolume,"Cancel preserves contents");});
        ResetPair();Dock("acetic",0);Dock("bicarb",1);
        Pour("acetic",100,"2 · FORMAÇÃO DE GÁS · Ácido acético\nPrepare a solução no béquer e depois use bicarbonato sólido.");
        Powder("bicarb");
        Add(5,"EFERVESCÊNCIA · Produção de CO₂\nAs bolhas acompanham a reação. Sólido em excesso pode permanecer no recipiente.",()=>{Close();Check(lab.Session.ExtentMol>0&&lab.Session.ProductsMol.Last()>0,"CO2 formed");});
        Consult("COMPARAR COM A PRECIPITAÇÃO\nAqui há liberação de gás. O balanço registra produtos, excesso e doses efetivamente recebidas.");
        ResetPair();Dock("naoh",0);Dock("hcl",1);
        Pour("naoh",60,"3 · NEUTRALIZAÇÃO · Hidróxido de sódio\nA solução não fica rosa automaticamente: o indicador é uma ação separada.");
        Add(3,"ADICIONAR INDICADOR EM TRAÇO\nPegue o conta-gotas e leve à abertura do béquer.",()=>{Check(!lab.Session.IndicatorPresent,"No automatic indicator");Grab("indicator");Close();},t=>MoveRoot(RootForMouth(lab.opening.position+Vector3.up*.2f,Quaternion.identity),Quaternion.identity,t/2));
        Add(3,"ATIVAR O CONTA-GOTAS\nA fenolftaleína fica rosa no meio básico deste par validado.",()=>held.activated.Invoke(new ActivateEventArgs()));
        Return();
        Pour("hcl",110,"ADICIONAR ÁCIDO GRADUALMENTE\nO ácido consome a base; o rosa desaparece ao sair da faixa básica.");
        Add(4,"NEUTRALIZAÇÃO E EXCESSO DE ÁCIDO\nSem gás ou precipitado inventado. A cor depende do indicador e do pH calculado.",()=>{Close();Check(lab.Session.IndicatorPresent&&lab.Session.PH<7&&lab.receiver.tint.g>.8f,"Indicator clear after acid excess");});
        Measure(true);
        Consult("RESULTADO · pH e excesso\nO pH numérico está disponível apenas no par HCl/NaOH. Os demais pares têm limites explícitos.");
        ResetPair();Dock("cuo",0);Dock("sulfuric",1);
        Pour("sulfuric",100,"4 · ÓXIDO E ÁCIDO · Ácido sulfúrico diluído\nO sólido foi escolhido em A, mas a solução pode ser adicionada primeiro.");
        Powder("cuo");
        Add(5,"ÓXIDO PRETO → SOLUÇÃO AZUL\nO CuO é consumido conforme a dose de ácido. O azul representa sulfato de cobre dissolvido.",()=>{Close();Check(lab.Session.ExtentMol>0&&lab.receiver.tint.r<.4f,"Copper solution formed");Check(lab.bubbles.particleCount==0&&lab.precipitate.particleCount==0,"No false gas or precipitation");});
        Measure(false);
        Consult("CONSULTAR OS LIMITES DO MODELO\nReceitas locais; sem cinética. Estimativa térmica apenas para HCl com NaOH ou KOH.");
        ResetPair();
        Add(6,"PRONTO PARA UMA NOVA EXPLORAÇÃO\n12 reagentes · 12 receitas · escolha física A/B · consulta opcional · Voltar ao Hub na faixa de comandos.",()=>{Wide();Check(!bench.Model.Ready&&!bench.Model.Used,"Final reset complete");});
    }
    public static void Tick(float time,Action<Vector3,Vector3> camera,Action<string> caption)
    {
        view=camera;float start=0;
        for(int i=0;i<shots.Count;i++)
        {
            var s=shots[i];if(time<start+s.seconds)
            {if(previous!=i){previous=i;s.begin?.Invoke();caption(s.caption);}s.animate?.Invoke(time-start);return;}start+=s.seconds;
        }
    }
    public static void Cleanup(){if(hand){if(hand.hasSelection)hand.EndManualInteraction();UnityEngine.Object.Destroy(hand.gameObject);}hand=null;held=null;}
}
