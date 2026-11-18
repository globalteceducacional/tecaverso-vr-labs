using System;
using System.Linq;
using Tecaverso.Labs.Biology.Microbes;
using Tecaverso.UI;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Tecaverso.Editor
{
    public static class MicrobesValidation
    {
        public static string Run()
        {
            var lab = UnityEngine.Object.FindFirstObjectByType<MicrobesLab>();
            if (!Application.isPlaying || !lab) throw new InvalidOperationException("Abra a galeria em Play Mode.");
            void Check(bool value, string issue) { if (!value) throw new InvalidOperationException(issue); }
            Check(lab.exhibits.Length == 9 && lab.exhibits.Count(e => e.category.Contains("Estrutura")) == 2, "Acervo ou identificação incompletos.");
            var temp = new GameObject("Raio de validação");
            try
            {
                var ray = temp.AddComponent<XRRayInteractor>();
                var manager = lab.advance.interactionManager;
                ray.interactionManager = manager;
                void Select(XRBaseInteractable target)
                {
                    manager.SelectEnter((IXRSelectInteractor)ray,(IXRSelectInteractable)target);
                    manager.SelectExit((IXRSelectInteractor)ray,(IXRSelectInteractable)target);
                }
                foreach(var exhibit in lab.exhibits)
                {
                    var surfaces = exhibit.turntable.GetComponentsInChildren<MeshRenderer>();
                    // Compare in exhibit coordinates. Rotating a world-space bounding box
                    // enlarges its diagonal even though the specimen itself never changes size.
                    var extent = new Bounds();
                    bool first = true;
                    foreach(var surface in surfaces)
                    {
                        var source = surface.localBounds;
                        var toExhibit = exhibit.turntable.worldToLocalMatrix * surface.localToWorldMatrix;
                        for(int corner=0;corner<8;corner++)
                        {
                            var signs = new Vector3((corner&1)==0?-1:1,(corner&2)==0?-1:1,(corner&4)==0?-1:1);
                            var point = toExhibit.MultiplyPoint3x4(source.center+Vector3.Scale(source.extents,signs));
                            if(first){extent=new Bounds(point,Vector3.zero);first=false;}
                            else extent.Encapsulate(point);
                        }
                    }
                    float span = Mathf.Max(extent.size.x,extent.size.y,extent.size.z);
                    Check(!first && extent.center.magnitude<.09f && span>.54f && span<.68f,"Escala ou centro incoerente em "+exhibit.title);
                    Select(exhibit);
                    Check(lab.Selected==exhibit && UIVisibility.IsVisible(lab.query),"Consulta ausente em "+exhibit.title);
                    Canvas.ForceUpdateCanvases();lab.queryText.ForceMeshUpdate();
                    Check(!lab.queryText.isTextOverflowing,"Consulta truncada em "+exhibit.title);
                }
                lab.close.onClick.Invoke();
                lab.labels.isOn=true;
                Check(lab.exhibits.All(e=>e.annotation.activeInHierarchy),"Rótulos de estruturas ausentes.");
                for(int i=1;i<=3;i++)
                {
                    Select(lab.advance);
                    Check(lab.Stage==i%3 && lab.stages.Count(s=>s.activeSelf)==1 && lab.stages[lab.Stage].activeSelf,"Avanço ambíguo de etapa.");
                }
                Select(lab.advance);Select(lab.exhibits[6]);
                lab.exhibits[6].dial.value=.5f;
                lab.restore.onClick.Invoke();lab.Next();
                Check(lab.Confirming && !lab.exhibits[0].enabled && !lab.advance.enabled && !lab.back.interactable &&
                    !UIVisibility.IsVisible(lab.query) && lab.Stage==1,"A confirmação perdeu o bloqueio.");
                lab.cancel.onClick.Invoke();
                Check(lab.Stage==1 && lab.labels.isOn && UIVisibility.IsVisible(lab.query) && lab.exhibits[6].dial.value==.5f,"Cancelamento perdeu o estado.");
                lab.AskRestore();lab.confirm.onClick.Invoke();
                Check(!lab.Confirming && lab.Stage==0 && !lab.Selected && !lab.labels.isOn && lab.exhibits.All(e=>e.dial.value==0),"Restauração incompleta.");
                return "OK · nove consultas por eventos XR, escalas, rótulos, três etapas, cancelamento e restauração.";
            }
            finally { UnityEngine.Object.DestroyImmediate(temp); }
        }
    }
}
