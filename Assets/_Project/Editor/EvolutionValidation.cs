using System;
using System.Linq;
using Tecaverso.Interaction;
using Tecaverso.Labs.Biology.Evolution;
using Tecaverso.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Object = UnityEngine.Object;

namespace Tecaverso.Editor
{
    public static class EvolutionValidation
    {
        static void Check(bool result,string message)
        {
            if (!result) throw new InvalidOperationException(message);
        }

        public static string CheckModel()
        {
            for (int schedule=0;schedule<64;schedule++)
            {
                var population=new SelectionPopulation();
                population.SaveReference();
                for (int generation=0;generation<6;generation++)
                {
                    bool dark=(schedule & 1<<generation)!=0;
                    int previous=population.Light;
                    population.SetGround(dark);
                    Check(population.Light==previous,"Trocar a casca alterou o fenótipo.");
                    Check(population.Advance() && population.Generation==generation+1,"Geração inválida.");
                    Check(population.Light+population.Dark==24 && population.Light>=0 && population.Dark>=0,"Total ou sinal incorreto.");
                    Check(dark ? population.Light<=previous : population.Light>=previous,"Tendência contrária aos pesos.");
                    Check(population.Reference.Value.Light==12 && population.Reference.Value.Generation==0,"Referência mudou.");
                }
                if (schedule==0) Check(population.Light==21,"Esperado 21 claras na G6.");
                if (schedule==63) Check(population.Light==3,"Esperado 3 claras na G6 escura.");
                Check(!population.Advance() && population.Generation==6,"Limite ultrapassado.");
                population.Reset();
                Check(!population.HasChanges && population.Light==12 && !population.DarkGround,"Falha na restauração.");
            }
            return "64 sequências de seis gerações, pesos, ambiente, contagem, referência imutável e restauração.";
        }

        public static string Run()
        {
            Check(Application.isPlaying && SceneManager.GetActiveScene().path==EvolutionAuthoring.ScenePath,"Abra Evolution em Play Mode.");
            CheckModel();
            var lab=Object.FindFirstObjectByType<EvolutionLab>();
            Check(lab && lab.moths.Length==24 && lab.moths.All(m=>m && m.enabled),"População incompleta.");
            Check(lab.moths.Select(m=>m.GetComponent<MeshFilter>().sharedMesh).Distinct().Count()==1,"Malha não instanciada.");
            foreach (var material in new[]{lab.darkMoth,lab.lightMoth,lab.lightBark,lab.darkBark})
                Check(material && material.shader.isSupported && !ShaderUtil.ShaderHasError(material.shader),"Shader inválido: "+material.name);
            var motion=lab.step.GetComponent<XRInteractionMotion>();
            Check(motion && motion.visualTargets.SequenceEqual(new[]{lab.stepCap}),"Silhueta do botão inclui a placa.");
            var rayObject=new GameObject("Temporary evolution test ray");
            bool reduced=MotionPreferences.Reduced;
            try
            {
                lab.PresentInitial();
                var manager=lab.step.interactionManager;
                var ray=rayObject.AddComponent<XRRayInteractor>();
                ray.interactionManager=manager;
                var keyCollider=lab.step.GetComponentInChildren<Collider>();
                Physics.SyncTransforms();
                var frontDeck=lab.habitat.transform.parent.Find("Tampo").GetComponent<Collider>();
                var stage=lab.bark.transform.parent;
                foreach(Transform support in lab.transform)
                {
                    if(support.name!="Suporte") continue;
                    foreach(float x in new[]{-.5f,.5f})
                        foreach(float z in new[]{-.5f,.5f})
                        {
                            Vector3 corner=support.TransformPoint(new Vector3(x,.5f,z));
                            Check(stage.InverseTransformPoint(corner).y < 0f,"O pé atravessa a face da casca.");
                        }
                }
                foreach(float eye in new[]{1.12f,1.65f})
                {
                    Vector3 origin=new Vector3(keyCollider.bounds.center.x,eye,-1.05f);
                    Vector3 reach=keyCollider.bounds.center-origin;
                    Check(Physics.Raycast(origin,reach,out var hit,reach.magnitude+.02f) && hit.collider==keyCollider,
                        "Tampo bloqueia o botão na altura "+eye);
                    // The front edge used to cut across the wings in the standing entry view.
                    Vector3 entry=new Vector3(0,eye,-2.45f);
                    foreach(var moth in lab.moths.Take(6))
                        foreach(var vertex in moth.GetComponent<MeshFilter>().sharedMesh.vertices)
                        {
                            Vector3 sight=moth.transform.TransformPoint(vertex)-entry;
                            Check(!frontDeck.Raycast(new Ray(entry,sight.normalized),out _,sight.magnitude),
                                "O tampo esconde a primeira fileira a "+eye+" m: "+moth.name);
                        }
                }
                MotionPreferences.SetReduced(false);
                lab.save.onClick.Invoke();
                var colors=lab.moths.Select(m=>m.sharedMaterial).ToArray();
                lab.habitat.value=true;
                Check(lab.bark.sharedMaterial==lab.darkBark && lab.moths.Select(m=>m.sharedMaterial).SequenceEqual(colors),
                    "O ambiente recoloriu as mariposas.");
                rayObject.transform.position=keyCollider.bounds.center;
                manager.SelectEnter((IXRSelectInteractor)ray,(IXRSelectInteractable)lab.step);
                if(lab.step.isSelected) manager.SelectExit((IXRSelectInteractor)ray,(IXRSelectInteractable)lab.step);
                Check(lab.InTransition && lab.Population.Generation==1,"O gesto XR não iniciou a geração.");
                lab.Advance();lab.TickTransition(.6f);
                Check(lab.Population.Generation==1 && lab.moths.Count(m=>m.transform.localScale==Vector3.one)<24 &&
                    lab.moths.Select(m=>m.sharedMaterial).SequenceEqual(colors),"Fase dos pais alterou a pigmentação.");
                lab.TickTransition(1.05f);
                Check(lab.status.text.Contains("Descendentes") && lab.moths.All(m=>m.transform.localScale.x>0 && m.transform.localScale.x<1),
                    "Entrada da prole ausente.");
                lab.TickTransition(1f);
                Check(!lab.InTransition && lab.moths.Count(m=>m.sharedMaterial==lab.lightMoth)==10,"G1 escura incorreta.");
                lab.about.onClick.Invoke();
                Check(UIVisibility.IsVisible(lab.consultation),"Consulta não abriu.");
                AssertReadable(lab.consultation);
                lab.restore.onClick.Invoke();
                Check(lab.Confirming && !UIVisibility.IsVisible(lab.consultation) && !lab.habitat.enabled && !lab.step.enabled &&
                    !lab.actions.GetComponent<CanvasGroup>().blocksRaycasts,"Confirmação permite interação de fundo.");
                AssertReadable(lab.confirmation);
                lab.Advance();lab.SetEnvironment(false);lab.SaveReference();
                lab.cancel.onClick.Invoke();
                Check(lab.Population.Generation==1 && lab.Population.DarkGround && lab.Population.Reference.Value.Generation==0 &&
                    UIVisibility.IsVisible(lab.consultation),"Cancelamento perdeu o contexto.");
                MotionPreferences.SetReduced(true);
                for(int i=0;i<5;i++) lab.Advance();
                Check(!lab.InTransition && !lab.step.enabled && lab.stepCaption.text.Contains("Limite") &&
                    lab.moths.Count(m=>m.sharedMaterial==lab.lightMoth)==3,"Movimento reduzido ou limite incorreto.");
                AssertReadable(lab.actions);
                foreach(var text in new[]{lab.counts,lab.reference,lab.status,lab.stepCaption})
                {text.ForceMeshUpdate();Check(!text.isTextOverflowing,"Leitura truncada: "+text.text);}
                lab.restore.onClick.Invoke();lab.confirm.onClick.Invoke();
                Check(!lab.Population.HasChanges && lab.step.enabled && UIVisibility.IsVisible(lab.actions) &&
                    !UIVisibility.IsVisible(lab.confirmation),"Restauração incompleta.");
            }
            finally
            {
                Object.DestroyImmediate(rayObject);
                MotionPreferences.SetReduced(reduced);
                lab.PresentInitial();
            }
            return "OK: 64 sequências, 24 malhas, shaders, acesso ao botão sentado/em pé por raycast, seleção XR, fases de reposição, limite, referência, cancelamento, restauração, texto e movimento reduzido.";
        }
        static void AssertReadable(GameObject root)
        {
            Canvas.ForceUpdateCanvases();
            foreach(var text in root.GetComponentsInChildren<TMP_Text>())
            {
                text.ForceMeshUpdate();
                Check(!text.isTextOverflowing,"Texto truncado: "+text.text);
            }
        }
    }
}
