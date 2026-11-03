using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
namespace Tecaverso.Labs.Shapes
{
    public class ShapeGrab:XRGrabInteractable
    {
        public ShapeView view; public int vertex=-1; public ShapesLab sliderLab;public Transform rail;
        public override bool IsSelectableBy(IXRSelectInteractor interactor)=> (sliderLab!=null?!sliderLab.Modal&&sliderLab.selected!=null&&!sliderLab.selected.gallery&&sliderLab.selected.CanUnfold: view!=null&&!view.lab.Modal&&!view.gallery&&(vertex>=0?view.lab.mode==2:view.lab.mode==0&&view.open==0))&&base.IsSelectableBy(interactor);
        protected override void OnSelectEntered(SelectEnterEventArgs args){base.OnSelectEntered(args);if(sliderLab!=null)sliderLab.Save();if(view!=null){view.lab.Select(view);if(vertex>=0)view.lab.vertexIndex=vertex;view.lab.Save();view.rotating=false;} }
        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase phase)
        {
            base.ProcessInteractable(phase);if(phase!=XRInteractionUpdateOrder.UpdatePhase.Late||!isSelected)return;
            if(sliderLab!=null){float value=Mathf.InverseLerp(-.35f,.35f,rail.InverseTransformPoint(transform.position).x);transform.position=rail.TransformPoint(new Vector3(Mathf.Lerp(-.35f,.35f,value),0,0));sliderLab.Open(value);}
            else if(vertex>=0){view.Edit(vertex,view.transform.InverseTransformPoint(transform.position));transform.position=view.transform.TransformPoint(view.data.vertices[vertex]);}
        }
    }
}
