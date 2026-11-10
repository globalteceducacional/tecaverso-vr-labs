using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Tecaverso.Labs.Chemistry.PeriodicBonds;

// Editor-only demonstration through the actual interaction events. No domain result is fabricated.
public static class PeriodicExplorerDemo
{
    public const float Duration = 146;
    static int previous=-1;
    static PeriodicTableExplorer lab;
    static LineRenderer pointer;
    static Material pointerMaterial;
    static readonly int[] numbers={6,26,60,92};
    static readonly string[] names={"Carbono · não metal","Ferro · metal de transição","Neodímio · lantanídeo","Urânio · actinídeo"};
    public static void Reset()
    {
        previous=-1;lab=UnityEngine.Object.FindFirstObjectByType<PeriodicTableExplorer>();
        if(!lab||lab.cubes.Length!=118||!lab.reference||lab.reference.Count!=118)throw new InvalidOperationException("Abra a tabela 3D com o acervo local completo.");
        lab.reference.reducedMotion.isOn=false;
        lab.clear.onClick.Invoke();lab.pages[0].onClick.Invoke();
        pointer=new GameObject("Temporary demonstration pointer").AddComponent<LineRenderer>();
        pointerMaterial=new Material(Shader.Find("Universal Render Pipeline/Unlit"));pointerMaterial.SetColor("_BaseColor",new Color(.2f,.8f,1));
        pointer.sharedMaterial=pointerMaterial;pointer.startWidth=.007f;pointer.endWidth=.004f;pointer.positionCount=2;pointer.enabled=false;
    }
    static void Hover(XRSimpleInteractable xr,bool value)
    {
        if(value){xr.firstHoverEntered.Invoke(new HoverEnterEventArgs());xr.hoverEntered.Invoke(new HoverEnterEventArgs());}
        else {xr.hoverExited.Invoke(new HoverExitEventArgs());xr.lastHoverExited.Invoke(new HoverExitEventArgs());}
    }
    static void Select(int number)
    {
        var xr=lab.cubes[number-1].GetComponent<XRSimpleInteractable>();
        xr.selectEntered.Invoke(new SelectEnterEventArgs());xr.selectExited.Invoke(new SelectExitEventArgs());
        if(lab.SelectedNumber!=number)throw new Exception("Seleção não aplicada: "+number);
    }
    static void Details(Action<Vector3,Vector3> view)
    {
        pointer.enabled=false;var panel=lab.heading.canvas.transform;
        view(panel.position-panel.forward*3.5f+Vector3.up*.12f,panel.position+Vector3.up*.12f);
    }
    public static void Tick(float t,Action<Vector3,Vector3> view,Action<string> say)
    {
        if(t<6){if(previous==-1){view(new Vector3(-.6f,3,-4.8f),new Vector3(0,2.9f,4.2f));say("TABELA PERIÓDICA 3D · Fotos e informações offline\nDemonstração automatizada pelos eventos da sala · 118 elementos selecionáveis.");previous=0;}return;}
        if(t>=126)
        {
            int step=t<133?90:t<136?91:t<139?92:93;if(previous==step)return;previous=step;
            if(step==90){Select(118);lab.pages[2].onClick.Invoke();Details(view);if(lab.reference.photo.texture)throw new Exception("Foto anterior permaneceu indevidamente.");say("OGANESSÔNIO · Informação também quando não existe foto de amostra\nA consulta explica a limitação e remove a fotografia anterior, sem inventar uma aparência.");}
            else if(step==91){lab.reference.reducedMotion.isOn=true;say("MOVIMENTO REDUZIDO · Preferência local\nO realce permanece, sem pulsos ou transições de escala. A opção vale para as outras salas.");}
            else if(step==92){lab.reference.reducedMotion.isOn=false;lab.clear.onClick.Invoke();say("LIMPAR SELEÇÃO · Conteúdo anterior removido\nA ficha volta às instruções iniciais e a tabela recupera as cores das categorias.");}
            else {view(new Vector3(-.6f,3,-4.8f),new Vector3(0,2.9f,4.2f));say("EXPLORAÇÃO CONCLUÍDA · Todos os elementos continuam disponíveis\n118 fichas de ocorrência e aplicações · 90 fotos reais locais com créditos e licenças.");}
            return;
        }
        int index=Mathf.Clamp((int)((t-6)/30),0,3);float local=t-6-index*30;
        int part=local<1?0:local<2.5f?1:local<3?2:local<4?3:local<6?4:local<10?5:local<14?6:local<26?7:8;
        int stage=1+index*9+part;if(stage==previous)return;previous=stage;
        var cube=lab.cubes[numbers[index]-1];var xr=cube.GetComponent<XRSimpleInteractable>();
        if(part==0)
        {
            lab.pages[0].onClick.Invoke();var target=cube.transform.position;var position=target+new Vector3(-.35f,.3f,-2.4f);
            view(position,target);pointer.SetPosition(0,position+new Vector3(.45f,-.4f,.2f));pointer.SetPosition(1,target+Vector3.back*.17f);pointer.enabled=false;
            say("APROXIMAR · "+names[index]+"\nO cubo é um alvo 3D independente, com número atômico, símbolo e nome.");
        }
        else if(part==1||part==3){pointer.enabled=true;Hover(xr,true);say("HOVER · "+names[index]+"\nCor e contorno respondem suavemente ao foco, sem trocar a seleção.");}
        else if(part==2){pointer.enabled=false;Hover(xr,false);}
        else if(part==4){Select(numbers[index]);Hover(xr,false);say("SELECIONAR · "+names[index]+"\nPulso de confirmação e escurecimento suave dos demais; posição e área de clique permanecem fixas.");}
        else
        {
            Details(view);int page=part-5;lab.pages[page].onClick.Invoke();
            if(page==2&&!lab.reference.photo.texture)throw new Exception("Fotografia local não carregou: "+numbers[index]);
            say(names[index]+" · "+new[]{"Visão geral","Propriedades","Na natureza e aplicações","Fontes e créditos"}[page]+"\n"+new[]{"Identidade, posição na tabela, massa e configuração eletrônica.","Valores disponíveis, unidades e limites dos dados de referência.","Fotografia real identificada, ocorrência e usos; todo o conteúdo já está salvo localmente.","Fontes científicas, identificação da amostra, autoria e licença da fotografia."}[page]);
        }
    }
}
