using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Tecaverso.Labs.Biology.Cells;
using Tecaverso.UI;
using Object=UnityEngine.Object;

namespace Tecaverso.Editor
{
    // Focused Editor verification, never a runtime/demo dependency.
    public static class CellsValidation
    {
        static void Check(bool condition,string failure){if(!condition)throw new InvalidOperationException(failure);}
        public static string Run()
        {
            Check(Application.isPlaying && SceneManager.GetActiveScene().path==CellsAuthoring.ScenePath,"Abra Cells em Play Mode.");
            var lab=Object.FindFirstObjectByType<CellsLab>();
            Check(lab.structures.Length==8 && lab.structures.Count(p=>p.detachable)==6,"Catálogo deve ter oito instâncias e seis organelas destacáveis.");
            Check(Object.FindObjectsByType<Unity.XR.CoreUtils.XROrigin>(FindObjectsSortMode.None).Length==1,"Mais de um rig ativo.");
            foreach(var p in lab.structures)
            {
                Check(p.GetComponent<Tecaverso.Interaction.XRInteractionMotion>() && p.home && p.caption,"Feedback ou referência ausente.");
                if(p.detachable)Check(p.socket && p.socket.target==p,"Socket sem filtro de identidade.");
            }
            var mito=lab.structures.Single(p=>p.structureId=="animal.mitocondria");
            var other=lab.structures.Single(p=>p.structureId=="vegetal.mitocondria");
            Check(!mito.socket.CanSelect((IXRSelectInteractable)other),"Socket aceita outra organela.");
            mito.docked=false;mito.transform.position=mito.home.position+Vector3.left*.2f;
            lab.Inspect(mito);Check(lab.consultationRoot.GetComponent<CanvasGroup>().blocksRaycasts && lab.Inspecting && mito.GetComponent<Tecaverso.Interaction.XRInteractionMotion>().PersistentSelected,"Consulta/seleção não abriu.");
            var pose=mito.transform.position;lab.CloseConsultation();Check(Vector3.Distance(pose,mito.transform.position)<.001f && !mito.docked,"Fechar consulta alterou a peça.");
            lab.RequestRestore();Check(lab.Modal && UIVisibility.IsVisible(lab.confirmationRoot),"Confirmação não abriu.");
            lab.Cancel();Check(!lab.Modal && !mito.docked && Vector3.Distance(pose,mito.transform.position)<.001f,"Cancelar alterou a peça.");
            var scale=mito.transform.localScale;var target=new Pose(mito.home.position+Vector3.one*3,Quaternion.identity);
            mito.GetComponent<CellGrabLimit>().Process(mito,XRInteractionUpdateOrder.UpdatePhase.Dynamic,ref target,ref scale);
            Check(Vector3.Distance(target.position,mito.home.position)<=.4001f,"Limite de pega não aplicado ao alvo físico.");
            Check(lab.Return(mito,false) && mito.docked && Vector3.Distance(mito.transform.position,mito.home.position)<.001f,"Retorno ao socket falhou.");
            // Drive real XR select events with a temporary interactor, without modifying input actions.
            var test=new GameObject("Temporary cells validation hand");
            var ray=test.AddComponent<XRRayInteractor>();
            try
            {
                ray.transform.position=mito.home.position;
                var manager=mito.interactionManager;
                manager.SelectEnter((IXRSelectInteractor)ray,(IXRSelectInteractable)mito);
                Check(mito.Held && !mito.docked && lab.Inspecting,"Pega XR não comunicou seleção.");
                lab.RequestRestore();Check(!lab.Modal,"Confirmação aberta com uma peça na mão.");
                manager.SelectExit((IXRSelectInteractor)ray,(IXRSelectInteractable)mito);
            }
            finally{Object.DestroyImmediate(test);}
            lab.CloseConsultation();lab.Return(mito,false);
            lab.labels.isOn=true;Check(lab.structures.All(p=>p.caption.gameObject.activeSelf),"Toggle de rótulos não atualizou.");
            lab.RequestRestore();lab.Confirm();Check(lab.structures.All(p=>p.docked) && !lab.Inspecting && !lab.Modal,"Recomposição incompleta.");
            Check(lab.labels.isOn,"Recomposição apagou a preferência de rótulos.");
            lab.labels.isOn=false;
            Check(lab.structures.All(p=>!p.caption.gameObject.activeSelf),"Rótulos não foram ocultos.");
            return "OK: inventário, rig único, motion, filtros, consulta, cancelamento, limite físico, socket, pega XR e recomposição.";
        }
    }
}
