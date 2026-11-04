using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
namespace Tecaverso.Labs.Functions
{
    public sealed class FunctionMarker : XRGrabInteractable
    {
        public FunctionsLab lab;
        public override bool IsSelectableBy(IXRSelectInteractor interactor) => lab && !lab.Modal && base.IsSelectableBy(interactor);
        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase phase)
        {
            base.ProcessInteractable(phase);
            if(!lab || !isSelected || lab.Modal) return;
            if(phase==XRInteractionUpdateOrder.UpdatePhase.Dynamic)
                lab.DragMarker(lab.plot.transform.InverseTransformPoint(transform.position).x/FunctionMath.WorldScale);
            else
            {
                double x=lab.Session.MarkerX;
                transform.position=lab.plot.transform.TransformPoint(new Vector3((float)x*.5f,(float)lab.Session.Current.Evaluate(x)*.5f,-.1f));
                transform.rotation=lab.plot.transform.rotation;
            }
        }
    }
}
