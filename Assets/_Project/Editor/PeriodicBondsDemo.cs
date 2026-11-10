using System;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Tecaverso.Labs.Chemistry.PeriodicBonds;

// Automated demonstration through the same UI events and XR sockets used by the student.
public static class PeriodicBondsDemo
{
    static int stage;
    static AtomPiece moving;
    static Vector3 start;
    static float moveStart;
    static int socketIndex;
    public static void Reset(){stage=0;moving=null;}
    public static void Tick(float t,Action<Vector3,Vector3> view,Action<string> say)
    {
        var lab=UnityEngine.Object.FindFirstObjectByType<PeriodicBondsLab>();
        if(moving)
        {
            float progress=Mathf.Clamp01((t-moveStart)/1.5f);
            moving.transform.position=Vector3.Lerp(start,lab.sockets[socketIndex].transform.position,progress)+Vector3.up*Mathf.Sin(progress*Mathf.PI)*.35f;
            if(progress>=1){var socket=lab.sockets[socketIndex];socket.interactionManager.SelectEnter((IXRSelectInteractor)socket,(IXRSelectInteractable)moving.GetComponent<XRGrabInteractable>());moving=null;}
        }
        if(stage==0){say("TABELA PERIÓDICA · 118 elementos\nDemonstração automatizada dos controles e encaixes XR do laboratório.");stage++;}
        if(stage==1&&t>=3){lab.tableButtons[78].onClick.Invoke();say("CONSULTA · Ouro (Au)\nSelecione qualquer célula para consultar nome, número atômico, período e grupo.");stage++;}
        if(stage==2&&t>=8){view(new Vector3(1.8f,2.3f,1.3f),new Vector3(5.1f,2.3f,1.3f));lab.tableButtons[91].onClick.Invoke();say("SÉRIES LATERAIS · Urânio (U)\nLantanídeos e actinídeos também podem ser consultados.");stage++;}
        if(stage==3&&t>=14){view(new Vector3(-1.8f,2.5f,-3),new Vector3(0,1.15f,.15f));say("MONTAGEM · Água (H₂O)\nLeve um oxigênio e dois hidrogênios aos encaixes.");Move(lab,ElementKind.O,0,t);stage++;}
        if(stage==4&&t>=17){Move(lab,ElementKind.H,1,t);stage++;}
        if(stage==5&&t>=20){Move(lab,ElementKind.H,2,t);stage++;}
        if(stage==6&&t>=23){view(new Vector3(3.75f,2,-1.6f),lab.display.transform.position);lab.electronsToggle.isOn=true;say("H₂O · Molécula angular\nDuas ligações simples; o toggle revela os pares de elétrons didáticos.");stage++;}
        if(stage==7&&t>=29){lab.dismantleButton.onClick.Invoke();view(new Vector3(-1.8f,2.5f,-3),new Vector3(0,1.15f,.15f));say("NOVA MONTAGEM · Dióxido de carbono (CO₂)\nDesmontar limpa os encaixes para outra composição.");Move(lab,ElementKind.C,0,t);stage++;}
        if(stage==8&&t>=32){Move(lab,ElementKind.O,1,t);stage++;}
        if(stage==9&&t>=35){Move(lab,ElementKind.O,2,t);stage++;}
        if(stage==10&&t>=38){view(new Vector3(3.75f,2,-2),lab.display.transform.position);say("CO₂ · Molécula linear\nCompare as duas ligações duplas com a geometria angular da água.");stage++;}
        if(stage==11&&t>=44){lab.dismantleButton.onClick.Invoke();view(new Vector3(-1.8f,2.5f,-3),new Vector3(0,1.15f,.15f));say("LIGAÇÃO IÔNICA · NaCl\nEncaixe sódio e cloro para observar a rede de íons.");Move(lab,ElementKind.Na,0,t);stage++;}
        if(stage==12&&t>=47){Move(lab,ElementKind.Cl,1,t);stage++;}
        if(stage==13&&t>=50){view(new Vector3(3.75f,2,-1.7f),lab.display.transform.position);lab.electronsToggle.isOn=false;say("NaCl · Rede cristalina\nÍons Na⁺ e Cl⁻ alternados: um sólido iônico, não uma molécula isolada.");stage++;}
        if(stage==14&&t>=57){say("EXPLORAÇÃO · Girar a estrutura\nA vitrine permite observar o modelo por diferentes ângulos.");stage++;}
        if(t>=57)lab.display.model.localRotation=Quaternion.Euler(0,(t-57)*12,0);
    }
    static void Move(PeriodicBondsLab lab,ElementKind element,int slot,float t)
    {
        moving=UnityEngine.Object.FindObjectsByType<AtomPiece>(FindObjectsSortMode.None).First(p=>p.element==element&&p.dockIndex<0&&!p.GetComponent<XRGrabInteractable>().isSelected);
        start=moving.transform.position;moveStart=t;socketIndex=slot;
    }
}
