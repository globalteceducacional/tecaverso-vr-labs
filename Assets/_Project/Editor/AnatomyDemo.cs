using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Tecaverso.Interaction;
using Tecaverso.Labs.Biology.Anatomy;

// Editor-only choreography. Uses the room's UI/XR events; no invented anatomical results.
public static class AnatomyDemo
{
    public const float Duration=207;
    static AnatomyLab lab;
    static readonly List<(float time,Action action)> steps=new();
    static readonly List<string> journal=new();
    static int next;
    static float clock,releaseAt,pausedPhase;
    static Vector3 origin,cameraPosition,cameraTarget;
    static string pausedStage;
    static bool reduced;
    static Action<Vector3,Vector3> view;
    static Action<string> say;
    static XRRayInteractor hand;
    static XRDirectInteractor grip;
    static AnatomyOrgan demonstrationOrgan;
    static Vector3 gripOrigin;
    static string flowHeading,lastStage;
    static AnatomyOrgan selected;
    static Selectable pressed;
    static LineRenderer pointer;
    static Material pointerMaterial;
    static void Check(bool condition,string message){if(!condition)throw new InvalidOperationException(message);}
    static void Add(float time,Action action)=>steps.Add((time,action));
    static void Shot(Vector3 position,Vector3 target){Release();flowHeading=null;lastStage=null;cameraPosition=position;cameraTarget=target;view(position,target);}
    static void Wide()=>Shot(new Vector3(0,2,-3.5f),new Vector3(0,1.15f,.5f));
    static void Console()
    {
        var t=lab.transform.Find("Console inclinado");Shot(t.position+t.rotation*Vector3.back*1.22f,t.position);
    }
    static void Body()=>Shot(lab.bodyPivot.position+new Vector3(0,1.3f,-2.1f),lab.bodyPivot.position+new Vector3(0,.98f,0));
    static void SystemDetail()
    {
        var parts=lab.organs.Where(o=>o.system==lab.ActiveSystem).ToArray();var bounds=parts[0].WorldBounds;
        foreach(var part in parts.Skip(1))bounds.Encapsulate(part.WorldBounds);
        float distance=Mathf.Max(.5f,Mathf.Max(bounds.size.y,bounds.size.x/(16f/9))*1.15f);
        // Keep the lowest structure above the recording-only caption band.
        Shot(bounds.center+new Vector3(0,-.06f,-distance),bounds.center-Vector3.up*.06f);
    }
    static void Flow(int system,float angle=0)
    {
        float y=system==0?1.54f:system==2?1.34f:1.39f;
        float distance=system==0?.85f:1.22f;
        float x=system==1?.10f:system==3?-.11f:0;
        var target=lab.bodyPivot.position+new Vector3(x,y,0);
        Shot(target+Quaternion.Euler(0,angle,0)*(Vector3.back*distance),target);
    }
    static void Details()
    {
        var p=lab.consultation.transform.position;Shot(p+new Vector3(0,-.12f,-1.35f),p+Vector3.down*.12f);
    }
    static void BeginOrganInspection()
    {
        lab.Choose(1);
        lab.Close();
        lab.ShowSkin(false);
        demonstrationOrgan=lab.organs.Single(o=>o.system==1&&o.title=="Coração");
        gripOrigin=demonstrationOrgan.WorldBounds.center;
        var target=gripOrigin+new Vector3(.10f,0,-.05f);
        Shot(target+new Vector3(0,.04f,-.62f),target);
        var controller=new GameObject("Temporary organ inspection controller");
        var touch=controller.AddComponent<SphereCollider>();
        touch.isTrigger=true;
        touch.radius=.022f;
        var body=controller.AddComponent<Rigidbody>();
        body.useGravity=false;
        body.isKinematic=true;
        controller.transform.SetPositionAndRotation(gripOrigin,Quaternion.identity);
        grip=controller.AddComponent<XRDirectInteractor>();
        grip.interactionManager=demonstrationOrgan.interactionManager;
        grip.StartManualInteraction((IXRSelectInteractable)demonstrationOrgan);
        Check(demonstrationOrgan.isSelected,"Pega direta não selecionou o coração.");
    }
    static void Point(Vector3 target)
    {
        var forward=(cameraTarget-cameraPosition).normalized;var right=Vector3.Cross(Vector3.up,forward).normalized;
        pointer.SetPosition(0,cameraPosition+forward*.25f+right*.25f-Vector3.up*.2f);pointer.SetPosition(1,target);pointer.enabled=true;releaseAt=clock+.5f;
    }
    static PointerEventData Data()=>new(EventSystem.current){button=PointerEventData.InputButton.Left};
    static void Click(Selectable control)
    {
        Release();Check(control.IsInteractable()&&Tecaverso.UI.UIVisibility.IsVisible(control.gameObject),"Controle indisponível: "+control.name);
        pressed=control;var e=Data();ExecuteEvents.Execute(control.gameObject,e,ExecuteEvents.pointerEnterHandler);
        ExecuteEvents.Execute(control.gameObject,e,ExecuteEvents.pointerDownHandler);ExecuteEvents.Execute(control.gameObject,e,ExecuteEvents.pointerUpHandler);
        ExecuteEvents.Execute(control.gameObject,e,ExecuteEvents.pointerClickHandler);Point(control.transform.position);
    }
    static void Select(string name)
    {
        Release();selected=lab.organs.Single(o=>o.system==lab.ActiveSystem&&o.title==name);
        hand.transform.SetPositionAndRotation(cameraPosition,Quaternion.LookRotation(selected.WorldBounds.center-cameraPosition));
        selected.interactionManager.HoverEnter((IXRHoverInteractor)hand,(IXRHoverInteractable)selected);
        selected.interactionManager.SelectEnter((IXRSelectInteractor)hand,(IXRSelectInteractable)selected);
        Point(selected.WorldBounds.center);Check(lab.Selected==selected,"Seleção XR não abriu consulta: "+name);
    }
    static void Release()
    {
        if(pressed){ExecuteEvents.Execute(pressed.gameObject,Data(),ExecuteEvents.pointerExitHandler);pressed=null;if(EventSystem.current)EventSystem.current.SetSelectedGameObject(null);}
        if(selected)
        {
            if(hand&&hand.interactablesSelected.Contains(selected))selected.interactionManager.SelectExit((IXRSelectInteractor)hand,(IXRSelectInteractable)selected);
            if(hand&&hand.interactablesHovered.Contains(selected))selected.interactionManager.HoverExit((IXRHoverInteractor)hand,(IXRHoverInteractable)selected);
            selected=null;
        }
        if(pointer)pointer.enabled=false;
    }
    static void Route(int system,float time,string organ,string explanation)
    {
        string name=AnatomyDefinitions.Titles[system].ToUpperInvariant();
        Add(time,()=>{Console();Click(lab.systemButtons[system]);Check(lab.ActiveSystem==system,"Troca de sistema falhou");say(name+" · Trocar pelo console\nUm sistema principal por vez, sem perder o alinhamento do corpo.");});
        Add(time+1,()=>{Flow(system);Select(organ);say(name+" · Selecionar "+organ+"\nO contorno acompanha a estrutura e a seleção abre a ficha lateral.");});
        Add(time+3,()=>{Details();say("CONSULTA · "+organ+"\nFunção, contexto e limites do modelo. Fechar mantém o sistema escolhido.");});
        Add(time+10,()=>{Click(lab.close);Console();say(name+" · Observar percurso\n"+explanation);});
        Add(time+11,()=>Click(lab.process));
        Add(time+12,()=>{Flow(system);Check(lab.path.Running&&lab.path.Visible&&lab.path.surfaceMaterial.shader.isSupported,"Percurso não iniciou");flowHeading=name+" · Em movimento";});
        Add(time+20,()=>{Console();Click(lab.process);Check(!lab.path.Running,"Pausa falhou");pausedPhase=lab.path.Phase;pausedStage=lab.path.Legend;say(name+" · Pausar percurso\nA onda sobre o órgão congela na etapa atual.");});
        Add(time+21,()=>{Flow(system);flowHeading=name+" · Pausado nesta etapa";});
        Add(time+25,()=>
        {
            Check(Mathf.Abs(lab.path.Phase-pausedPhase)<.00001f&&lab.path.Legend==pausedStage&&lab.path.Visible,"O percurso mudou durante a pausa");
            Console();Click(lab.process);Check(lab.path.Running,"Retomada falhou");say(name+" · Retomar\nO percurso continua a partir da posição preservada.");
        });
        Add(time+26,()=>{Flow(system,system==1?-30:32);flowHeading=name+" · Vista oblíqua após retomar";});
        Add(time+32,()=>Check(lab.path.Running&&Mathf.Abs(lab.path.Phase-pausedPhase)>.001f,"Percurso não avançou após retomar"));
    }
    public static void Reset()
    {
        lab=UnityEngine.Object.FindFirstObjectByType<AnatomyLab>();Check(lab&&lab.organs.Length==60,"Abra Anatomy em Play Mode.");
        reduced=MotionPreferences.Reduced;MotionPreferences.SetReduced(false);origin=lab.bodyPivot.position;
        lab.Choose(8);lab.Choose(0);lab.Close();lab.skin.isOn=true;lab.ring.value=0;
        steps.Clear();journal.Clear();next=0;
        hand=new GameObject("Temporary anatomy demo ray").AddComponent<XRRayInteractor>();hand.interactionManager=lab.organs[0].interactionManager;
        pointer=new GameObject("Temporary anatomy demo pointer").AddComponent<LineRenderer>();pointer.positionCount=2;pointer.startWidth=.003f;pointer.endWidth=.0015f;
        pointerMaterial=new Material(Shader.Find("Universal Render Pipeline/Unlit"));pointerMaterial.SetColor("_BaseColor",new Color(.2f,.8f,1));pointer.sharedMaterial=pointerMaterial;pointer.enabled=false;
        Add(0,()=>{Wide();say("CORPO HUMANO E SISTEMAS · Visão da sala\nInterações automatizadas pelos controles reais. Legendas e ponteiro são exclusivos deste vídeo.");});
        Add(6,()=>{Console();say("CONSOLE · Nove sistemas, pele e controle de percurso\nO dial Unity gira o corpo no lugar. Uma única consulta aparece quando um órgão é selecionado.");});
        Add(10,()=>{Click(lab.skin);Check(!lab.skinRoot.activeSelf,"Pele não ocultou");say("PELE · Ocultar o contexto semitransparente\nAs estruturas ficam livres para observação; a pele não bloqueia a seleção.");});
        Route(0,12,"Pulmão esquerdo","Ar nas vias e nos dois pulmões: inspiração e expiração, sem animação de contração.");
        Route(1,46,"Coração","Circuitos pulmonar e sistêmico. Azul: menos O2; vermelho: mais O2. O sangue humano não é azul.");
        Route(2,80,"Estômago","Conteúdo da boca aos intestinos, em sentido único. Fígado e pâncreas são órgãos acessórios.");
        Route(3,114,"Encéfalo","Exemplo sensitivo: membro superior > medula > encéfalo. Propagação de sinal, não de matéria.");
        Add(148,()=>{Console();Click(lab.systemButtons[4]);Check(!lab.process.GetComponent<CanvasGroup>().blocksRaycasts&&!lab.path.Running,"Sistema estático manteve percurso");say("ESQUELÉTICO · Exploração anatômica estática\nO comando de percurso desaparece. Não há animação inventada de articulações.");});
        Add(151,()=>{var p=lab.ring.transform.position;Shot(p+new Vector3(0,.65f,-.9f),p);say("DIAL UNITY · Girar sem carregar o corpo\nO modelo permanece ancorado, e todas as peças compartilham o mesmo pivô.");});
        Add(153,()=>{Body();say("ROTAÇÃO · Frente, lateral e retorno\nA orientação muda suavemente; a posição, a altura e o alinhamento das estruturas permanecem iguais.");});
        Add(163,()=>{lab.ring.value=0;Check(Vector3.Distance(origin,lab.bodyPivot.position)<.0001f,"Giro deslocou o corpo");Console();Click(lab.systemButtons[5]);Body();say("MUSCULAR · Mais um sistema no mesmo corpo\nMúsculos preservam suas texturas. O modelo não tem contração ou movimento de marcha.");});
        Add(169,()=>{Console();Click(lab.systemButtons[6]);SystemDetail();say("URINÁRIO · Rins, ureteres, bexiga e conjunto uretra/próstata\nGrupos de consulta locais; a ficha explica limitações e estruturas compartilhadas do acervo.");});
        Add(175,()=>{Console();Click(lab.systemButtons[7]);Body();say("LINFÁTICO · Vasos, linfonodos, baço e timo\nRedes agrupadas para consulta; não há fluxo quantitativo ou células de defesa simuladas.");});
        Add(181,()=>{Console();Click(lab.systemButtons[8]);SystemDetail();say("REPRODUTIVO · Anatomia masculina do acervo\nO modelo feminino não está adaptado nesta entrega. As consultas explicam os grupos disponíveis.");});
        Add(187,()=>{BeginOrganInspection();say("PEGA DIRETA · Coração\nSegure a peça para afastá-la do tórax e observar sua forma.");});
        Add(193,()=>{Check(Vector3.Distance(gripOrigin,demonstrationOrgan.WorldBounds.center)>.14f,"A mão moveu, mas o coração permaneceu no lugar.");say("INSPEÇÃO · Coração na mão\nGire o punho para observar outra face do órgão.");});
        Add(197,()=>{grip.EndManualInteraction();say("SOLTAR · Reposição anatômica\nAo soltar o coração, ele volta ao seu lugar no tórax.");});
        Add(200,()=>{Check(Vector3.Distance(gripOrigin,demonstrationOrgan.WorldBounds.center)<.002f,"O coração não voltou à posição anatômica.");lab.Close();Console();Click(lab.systemButtons[0]);Click(lab.skin);Wide();say("EXPLORAÇÃO LIVRE · Nove sistemas e 60 consultas offline\nPele, giro e consulta independentes. Voltar ao Hub permanece no console.");});
        steps.Sort((a,b)=>a.time.CompareTo(b.time));
    }
    public static void Tick(float t,Action<Vector3,Vector3> camera,Action<string> caption)
    {
        view=camera;say=caption;clock=t;if(t>=releaseAt)Release();
        while(next<steps.Count&&t>=steps[next].time)
        {
            var step=steps[next++];step.action();journal.Add($"{step.time:0.0}s | {AnatomyDefinitions.Titles[lab.ActiveSystem]} | running={lab.path.Running} | phase={lab.path.Phase:0.000000} | selected={(lab.Selected?lab.Selected.title:"-")}");
        }
        if(t>=153&&t<163)lab.ring.value=Mathf.Sin(Mathf.PI*(t-153)/10)*.5f;
        if(flowHeading!=null&&lastStage!=lab.path.Legend)
        {
            lastStage=lab.path.Legend;
            say(flowHeading+"\n"+lastStage);
        }
        if(grip&&t>=187&&t<197)
        {
            float lift=Mathf.SmoothStep(0,1,(t-188)/3);
            float turn=Mathf.SmoothStep(0,1,(t-191)/4);
            grip.transform.SetPositionAndRotation(gripOrigin+new Vector3(.22f,.025f,-.13f)*lift,Quaternion.Euler(0,58*turn,-12*turn));
        }
    }
    public static void Cleanup()
    {
        Release();MotionPreferences.SetReduced(reduced);
        if(grip){if(grip.isPerformingManualInteraction)grip.EndManualInteraction();UnityEngine.Object.Destroy(grip.gameObject);}
        if(hand)UnityEngine.Object.Destroy(hand.gameObject);if(pointer)UnityEngine.Object.Destroy(pointer.gameObject);if(pointerMaterial)UnityEngine.Object.Destroy(pointerMaterial);
        Directory.CreateDirectory("GeneratedMedia/AnatomyVideo");File.WriteAllLines("GeneratedMedia/AnatomyVideo/roteiro-executado.txt",journal);
    }
}
