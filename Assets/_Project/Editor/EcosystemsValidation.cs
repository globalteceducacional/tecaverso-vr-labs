using System;
using System.Linq;
using Tecaverso.Interaction;
using Tecaverso.Labs.Biology.Ecosystems;
using Tecaverso.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Object=UnityEngine.Object;

namespace Tecaverso.Editor
{
    public static class EcosystemsValidation
    {
        static void Check(bool condition,string cause)
        {
            if(!condition)throw new InvalidOperationException(cause);
        }
        static void Fit(TMP_Text text)
        {
            Canvas.ForceUpdateCanvases();text.ForceMeshUpdate();
            Check(!text.isTextOverflowing,"Texto truncado: "+text.text);
        }
        static bool Drop(EcosystemsLab lab,int piece,int socket)
        {
            lab.tokens[piece].transform.position=lab.sockets[socket].attachTransform.position;
            return lab.Release(lab.tokens[piece]);
        }
        public static string CheckPlay()
        {
            Check(Application.isPlaying,"Execute em Play Mode.");
            var lab=Object.FindFirstObjectByType<EcosystemsLab>();
            Check(lab && lab.gameObject.scene.path==EcosystemsAuthoring.ScenePath,"Sala incorreta.");
            bool reduced=MotionPreferences.Reduced;
            try
            {
                MotionPreferences.SetReduced(true);
                lab.flowLever.value=false;foreach(var token in lab.tokens)token.Place(false);
                lab.Close();lab.Refresh();
                Check(lab.tokens.Length==3 && lab.sockets.Length==3 && lab.Connected==0,"Disposição inicial.");
                Check(lab.tokens.All(p=>p.GetComponentsInChildren<MeshRenderer>().Any(r=>
                    r.sharedMaterial.shader.name=="Tecaverso/Biology/FieldSpecimens" && r.sharedMaterial.shader.isSupported &&
                    !ShaderUtil.ShaderHasError(r.sharedMaterial.shader))),"Modelo ou shader de espécime inválido.");
                Check(lab.energy.All(r=>r.sharedMaterial.shader.isSupported && !ShaderUtil.ShaderHasError(r.sharedMaterial.shader)),"Shader das setas inválido.");
                lab.flowLever.value=true;
                Check(lab.energy.All(r=>!r.enabled)&&lab.matter.All(r=>!r.enabled),"Percurso sem montagem.");
                Check(!Drop(lab,2,0)&&!lab.tokens[2].Docked&&lab.tokens[2].transform.position==lab.tokens[2].home.position,"Predador aceito como produtor.");
                Fit(lab.status);
                Check(!Drop(lab,1,1),"Montagem sem produtor.");
                for(int i=0;i<3;i++)
                {
                    int enter=0;lab.sockets[i].selectEntered.AddListener(Count);
                    void Count(UnityEngine.XR.Interaction.Toolkit.SelectEnterEventArgs _) { enter++; }
                    try{Check(Drop(lab,i,i)&&enter==1,"Encaixe XR ausente no papel "+i);}
                    finally{lab.sockets[i].selectEntered.RemoveListener(Count);}
                    Check(lab.Connected==i+1&&lab.energy.Count(r=>r.enabled)==i+1&&lab.heat.Count(r=>r.enabled)==i+1,"Energia incoerente.");
                    Check(lab.matter.All(r=>r.enabled==(i==2)),"Retorno de matéria incompleto.");
                }
                var hand=new GameObject("Verificação de raio XR");
                try
                {
                    var ray=hand.AddComponent<XRRayInteractor>();
                    var piece=lab.tokens[0];var manager=piece.interactionManager;
                    manager.SelectEnter((IXRSelectInteractor)ray,(IXRSelectInteractable)piece);
                    Check(piece.Held && !piece.Docked && lab.Connected==0&&lab.energy.All(r=>!r.enabled),"Retirada não interrompeu os percursos.");
                    lab.Preview(piece,2);Check(piece.Held,"Recusa retirou a ficha da mão.");
                    lab.Restore();Check(!lab.Modal,"Modal abriu com ficha na mão.");
                    manager.SelectExit((IXRSelectInteractor)ray,(IXRSelectInteractable)piece);
                    Check(Drop(lab,0,0)&&lab.Connected==3,"Cadeia não se recompôs.");
                    manager.SelectEnter((IXRSelectInteractor)ray,(IXRSelectInteractable)lab.decomposers);
                    manager.SelectExit((IXRSelectInteractor)ray,(IXRSelectInteractable)lab.decomposers);
                }
                finally{Object.DestroyImmediate(hand);}
                Check(lab.Inspecting && UIVisibility.IsVisible(lab.consultation),"Consulta XR não abriu.");
                foreach(var t in lab.consultation.GetComponentsInChildren<TMP_Text>())Fit(t);
                lab.restore.onClick.Invoke();
                Check(lab.Modal&&!lab.decomposers.enabled&&!lab.flowLever.enabled&&!UIVisibility.IsVisible(lab.consultation),"Modal não isolou os controles.");
                lab.cancel.onClick.Invoke();Check(lab.Inspecting&&lab.Connected==3&&lab.flowLever.value,"Cancelar alterou a montagem.");
                lab.Restore();lab.confirm.onClick.Invoke();
                Check(!lab.Modal&&!lab.Inspecting&&lab.Connected==0&&!lab.flowLever.value,"Restauração incompleta.");
                Check(lab.tokens.All(p=>p.transform.position==p.home.position),"Ficha fora da bandeja.");
                Fit(lab.status);
                return "OK · 3 encaixes XR, predador recusado, ordem, retirada, consulta, cancelamento, restauração, shaders e texto.";
            }
            finally{MotionPreferences.SetReduced(reduced);}
        }
    }
}
