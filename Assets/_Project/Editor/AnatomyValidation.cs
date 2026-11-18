using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Tecaverso.Interaction;
using Tecaverso.Labs.Biology.Anatomy;
using Tecaverso.UI;
using Object=UnityEngine.Object;

namespace Tecaverso.Editor
{
    // Focused Editor checks. Does not enter the player or automate a recorded demo.
    public static class AnatomyValidation
    {
        static void Check(bool condition,string message){if(!condition)throw new InvalidOperationException(message);}
        static void CheckDial(AnatomyLab lab)
        {
            var deck=lab.transform.Find("Console inclinado");
            var handle=lab.ring.handle.GetComponent<BoxCollider>();
            var surfaceNormal=deck.TransformDirection(Vector3.back);
            var centre=handle.transform.TransformPoint(handle.center);
            var baseToHandle=centre-lab.ring.transform.position;
            Check(Vector3.Dot(baseToHandle,surfaceNormal)>.06f,"A manopla precisa ficar acima e à frente da base.");
            var label=lab.GetComponentsInChildren<TMP_Text>(true).Single(t=>t.text=="Girar corpo");
            Check(Vector3.Dot(-label.transform.forward,surfaceNormal)>.98f,"Legenda do dial fora do plano de leitura.");
            var mount=deck.Find("Extensão do tampo · giro");
            Check(mount.InverseTransformPoint(label.transform.position).z < -.5f,"Legenda do dial atrás do tampo.");
            Physics.SyncTransforms();
            // Front approaches to the console at seated and standing eye heights.
            foreach(float eye in new[]{1.15f,1.65f})
            {
                var observer=new Vector3(centre.x-.1f,eye,centre.z-.58f);
                Check(Physics.Raycast(observer,centre-observer,out var hit,Vector3.Distance(observer,centre)+.02f)
                    &&hit.collider==handle,"O suporte bloqueia a manopla a "+eye+" m.");
            }
            var controller=new GameObject("Temporary dial interaction");
            var trigger=controller.AddComponent<SphereCollider>();
            trigger.isTrigger=true;
            trigger.radius=.015f;
            var rigidbody=controller.AddComponent<Rigidbody>();
            rigidbody.isKinematic=true;
            rigidbody.useGravity=false;
            var hand=controller.AddComponent<XRDirectInteractor>();
            var dial=lab.ring;
            var previousValue=dial.value;
            try
            {
                dial.value=.5f;
                hand.interactionManager=dial.interactionManager;
                controller.transform.SetPositionAndRotation(centre,dial.transform.rotation);
                hand.StartManualInteraction((IXRSelectInteractable)dial);
                controller.transform.rotation=dial.transform.rotation*Quaternion.Euler(0,40,0);
                dial.ProcessInteractable(UnityEngine.XR.Interaction.Toolkit.XRInteractionUpdateOrder.UpdatePhase.Dynamic);
                Check(Mathf.Abs(dial.value-.5f)>.04f,"A torção do controlador não alterou o dial.");
            }
            finally
            {
                if(hand.isPerformingManualInteraction)hand.EndManualInteraction();
                dial.value=previousValue;
                Object.DestroyImmediate(controller);
            }
        }
        static void CheckOrganHandling(AnatomyLab lab)
        {
            Check(lab.organs.Count(o => o.IsLiftable) == 24, "Catálogo de peças destacáveis incorreto.");
            bool reduced = MotionPreferences.Reduced;
            var rootRotation = lab.bodyPivot.localRotation;
            var left = new GameObject("Temporary inspection hand").AddComponent<XRRayInteractor>();
            var right = new GameObject("Temporary second inspection hand").AddComponent<XRRayInteractor>();
            var phase = UnityEngine.XR.Interaction.Toolkit.XRInteractionUpdateOrder.UpdatePhase.Dynamic;
            void Grip(AnatomyOrgan part)
            {
                left.transform.SetPositionAndRotation(part.WorldBounds.center, Quaternion.identity);
                part.interactionManager.SelectEnter((IXRSelectInteractor)left, (IXRSelectInteractable)part);
                Check(part.isSelected && lab.Selected == part, "O gesto XR não abriu a consulta.");
            }
            void Release(AnatomyOrgan part) =>
                part.interactionManager.SelectExit((IXRSelectInteractor)left, (IXRSelectInteractable)part);
            void Pull(AnatomyOrgan part)
            {
                left.transform.position += Vector3.right * .22f;
                left.transform.rotation *= Quaternion.Euler(0, 40, 0);
                part.ProcessInteractable(phase);
            }
            try
            {
                MotionPreferences.SetReduced(false);
                lab.Choose(0);
                lab.Choose(1);
                var heart = lab.organs.Single(o => o.title == "Coração");
                var arteries = lab.organs.Single(o => o.title == "Rede arterial");
                heart.transform.GetLocalPositionAndRotation(out var home, out var homeRotation);
                var centre = heart.WorldBounds.center;
                lab.TogglePath();
                lab.path.Advance(.9f);
                float frozen = lab.path.Phase;
                Grip(heart);
                left.transform.position += Vector3.right * .005f;
                heart.ProcessInteractable(phase);
                Check(!heart.IsDisplaced && !lab.InspectionOrgan && heart.transform.localPosition == home, "Consulta breve retirou o órgão.");
                Pull(heart);
                Check(heart.IsDisplaced && lab.InspectionOrgan == heart && Vector3.Distance(centre, heart.WorldBounds.center) > .16f,
                    "O coração não acompanhou a mão.");
                Check(Quaternion.Angle(heart.transform.localRotation, homeRotation) > 30f, "A peça não girou na mão.");
                Check(!heart.IsSelectableBy((IXRSelectInteractor)right) && !arteries.IsSelectableBy((IXRSelectInteractor)right) &&
                    !arteries.IsHoverableBy((IXRHoverInteractor)right), "Outra mão pode disputar a inspeção.");
                lab.path.Advance(1);
                Check(lab.path.Running && !lab.path.Visible && lab.path.Phase == frozen && !lab.process.interactable, "Percurso ativo fora do corpo.");
                lab.bodyPivot.localRotation *= Quaternion.Euler(0, 60, 0);
                Release(heart);
                Check(heart.IsDisplaced && !heart.isSelected, "Soltura normal não iniciou a devolução.");
                Check(Time.unscaledDeltaTime > 0, "O teste de retorno requer frames em Play Mode.");
                int framesToSettle = Mathf.CeilToInt(.32f / Time.unscaledDeltaTime) + 1;
                for (int frame = 0; heart.IsDisplaced && frame < framesToSettle; frame++) heart.ProcessInteractable(phase);
                Check(!heart.IsDisplaced && !lab.InspectionOrgan && heart.transform.localPosition == home &&
                    Quaternion.Angle(heart.transform.localRotation, homeRotation) < .001f, "Retorno fora da pose anatômica após giro.");
                Check(lab.path.Visible && lab.path.Running && lab.path.Phase == frozen && lab.process.interactable, "A devolução perdeu a fase em execução.");
                lab.TogglePath();
                MotionPreferences.SetReduced(true);
                Grip(heart); Pull(heart); Release(heart);
                Check(!heart.IsDisplaced && heart.transform.localPosition == home && lab.path.Visible &&
                    !lab.path.Running && lab.path.Phase == frozen, "Movimento reduzido ou pausa manual perdidos na devolução.");
                Grip(arteries); Pull(arteries);
                Check(!arteries.IsDisplaced && lab.Selected == arteries && !lab.path.InspectionSuspended, "Rede fixa retirada do corpo.");
                Release(arteries);
                Grip(heart); Pull(heart);
                lab.Choose(0);
                Check(!heart.isSelected && !heart.IsDisplaced && !lab.Selected && !lab.InspectionOrgan &&
                    !lab.path.Visible && !lab.path.InspectionSuspended && heart.transform.localPosition == home,
                    "Trocar de sistema deixou órgão ou percurso pendente.");
            }
            finally
            {
                Object.DestroyImmediate(left.gameObject);
                Object.DestroyImmediate(right.gameObject);
                lab.bodyPivot.localRotation = rootRotation;
                MotionPreferences.SetReduced(reduced);
                lab.Choose(0);
                lab.Close();
            }
        }

        public static string Run()
        {
            Check(Application.isPlaying&&SceneManager.GetActiveScene().path==AnatomyAuthoring.ScenePath,"Abra Anatomy em Play Mode.");
            var lab=Object.FindFirstObjectByType<AnatomyLab>();
            Check(lab.systems.Length==9&&lab.organs.Length==60,"Inventário incorreto.");
            Check(Object.FindObjectsByType<Unity.XR.CoreUtils.XROrigin>(FindObjectsSortMode.None).Length==1,"Rig duplicado.");
            Check(lab.skinRoot.GetComponentsInChildren<Collider>(true).Length==0,"Pele bloqueia interação.");
            Check(Mathf.Abs(lab.skinRoot.GetComponentInChildren<Renderer>().bounds.size.y-1.7f)<.005f,"Altura corporal incorreta.");
            Check(lab.organs.All(o=>o.surfaces.Length>0&&o.colliders.Count==o.surfaces.Length&&o.GetComponent<XRInteractionMotion>()),"Malha, collider ou feedback ausente.");
            CheckDial(lab);
            CheckOrganHandling(lab);
            var catalog=AssetDatabase.LoadAssetAtPath<Tecaverso.Hub.HubCatalog>("Assets/_Project/Hub/HubCatalog.asset").disciplines.Single(d=>d.title=="Biologia");
            Check(catalog.contents.Length==6&&catalog.contents.Single(c=>c.title=="Corpo humano e sistemas").scenePath==AnatomyAuthoring.ScenePath,"Catálogo incorreto.");
            bool reduced=MotionPreferences.Reduced;
            try
            {
                // Avoid a fade hiding the same-frame layout inspection; restore the actual preference.
                MotionPreferences.SetReduced(true);
                lab.Choose(lab.ActiveSystem==8?7:8);
                for(int i=0;i<9;i++)
                {
                    lab.systemButtons[i].onClick.Invoke();Canvas.ForceUpdateCanvases();
                    Check(lab.ActiveSystem==i&&lab.systems.Count(s=>s.activeInHierarchy)==1,"Sistema não exclusivo: "+i);
                    Check(!lab.Selected&&!UIVisibility.IsVisible(lab.consultation)&&!lab.path.Running&&!lab.path.Visible,"Troca manteve consulta/percurso.");
                    Check(lab.process.GetComponent<CanvasGroup>().blocksRaycasts==(i<4),"Percurso oferecido para sistema estático.");
                    Check(lab.organs.Where(o=>o.system!=i).All(o=>!o.gameObject.activeInHierarchy&&o.colliders.All(c=>!c.gameObject.activeInHierarchy)),"Collider invisível ativo.");
                    foreach(var part in lab.organs.Where(o=>o.system==i))
                    {
                        lab.Inspect(part);Canvas.ForceUpdateCanvases();
                        Check(lab.Selected==part&&UIVisibility.IsVisible(lab.consultation)&&part.GetComponent<XRInteractionMotion>().PersistentSelected,"Consulta/feedback: "+part.title);
                        foreach(var text in lab.consultation.GetComponentsInChildren<TMP_Text>()){text.ForceMeshUpdate();Check(!text.isTextOverflowing,"Consulta truncada: "+part.title+" / "+text.name);}
                        var position=part.transform.position;lab.Close();Check(!lab.Selected&&!UIVisibility.IsVisible(lab.consultation)&&position==part.transform.position&&!part.GetComponent<XRInteractionMotion>().PersistentSelected,"Fechar alterou a peça.");
                    }
                    if(i<4)
                    {
                        lab.process.onClick.Invoke();Check(lab.path.Running&&lab.path.Visible&&lab.path.CurrentStage==0,"Percurso sem etapa visível.");
                        lab.status.ForceMeshUpdate();Check(!lab.status.isTextOverflowing,"Estado em movimento truncado.");
                        int stages=1;
                        for(int sample=1;sample<100;sample++)
                        {
                            int before=lab.path.CurrentStage;
                            lab.path.Advance(lab.path.Duration/100);
                            if(lab.path.CurrentStage==before)continue;
                            Check(lab.path.CurrentStage==before+1,"Etapa pulada no sistema "+i);
                            stages++;
                            lab.legend.text=lab.path.Legend;lab.legend.ForceMeshUpdate();
                            Check(!lab.legend.isTextOverflowing,"Etapa truncada: "+lab.path.Legend);
                        }
                        Check(stages==(i==3?4:6),"Ciclo incompleto no sistema "+i);
                        lab.process.onClick.Invoke();
                        float frozen=lab.path.Phase;
                        lab.path.Advance(2);
                        Check(!lab.path.Running&&lab.path.Visible&&lab.path.Phase==frozen,"Pausa alterou a onda.");
                        lab.process.onClick.Invoke();
                        lab.path.SetInspectionSuspended(true);lab.path.Advance(2);
                        Check(!lab.path.Visible&&lab.path.Running&&lab.path.Phase==frozen,"Retirada deixou o percurso avançar.");
                        lab.path.SetInspectionSuspended(false);lab.path.Advance(lab.path.Duration*.04f);
                        Check(lab.path.Visible&&lab.path.CurrentStage==0&&lab.path.Phase<frozen,"Falha ao restaurar ou repetir o ciclo.");
                    }
                }
                lab.Choose(0);var lung=lab.organs.Single(o=>o.system==0&&o.title=="Pulmão esquerdo");
                var hand=new GameObject("Temporary anatomy validation ray");var ray=hand.AddComponent<XRRayInteractor>();
                try
                {
                    hand.transform.position=lung.WorldBounds.center;var manager=lung.interactionManager;
                    manager.SelectEnter((IXRSelectInteractor)ray,(IXRSelectInteractable)lung);
                    Check(lab.Selected==lung,"Evento XR não abriu consulta.");
                    lab.Choose(1);
                    Check(!lung.isSelected&&!lab.Selected&&!lung.gameObject.activeInHierarchy,"Troca manteve seleção XR no órgão oculto.");
                }
                finally{Object.DestroyImmediate(hand);}
                lab.Close();lab.skin.isOn=false;Check(!lab.skinRoot.activeSelf,"Pele não oculta.");lab.skin.isOn=true;Check(lab.skinRoot.activeSelf,"Pele não restaura.");
                lab.Choose(1);Check(!lung.gameObject.activeInHierarchy,"Pulmão permanece ativo no circulatório.");lab.Choose(0);
                Check(lab.path.surfaceMaterial&&lab.path.surfaceMaterial.shader.isSupported&&lab.path.surfaceMaterial.shader.name=="Tecaverso/Biology/AnatomicalFlow","Shader dos percursos indisponível.");
                foreach(var text in lab.systemButtons.SelectMany(b=>b.GetComponentsInChildren<TMP_Text>()).Concat(new[]{lab.status,lab.legend,lab.processLabel}))
                {text.ForceMeshUpdate();Check(!text.isTextOverflowing,"Controle truncado: "+text.text);}
            }
            finally{MotionPreferences.SetReduced(reduced);}
            return "OK: nove sistemas, 60 consultas sem overflow, rig único, altura, pele, colliders, silhuetas, quatro percursos/pausa, troca limpa, evento XR e catálogo de seis.";
        }
    }
}
