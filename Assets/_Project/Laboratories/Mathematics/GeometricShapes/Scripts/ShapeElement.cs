using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
namespace Tecaverso.Labs.Shapes
{
    public class ShapeElement:MonoBehaviour
    {
        public ShapeView view; public string kind; public int index;
        void Start(){var x=GetComponent<XRSimpleInteractable>();x.hoverEntered.AddListener(_=>{if(!view.lab.Modal){view.Hover(true);if(view.lab.mode==1)view.Inspect(kind,index,false);}});x.hoverExited.AddListener(_=>view.Hover(false));x.selectEntered.AddListener(_=>{if(!view.lab.Modal){view.lab.Select(view);if(view.lab.mode==1)view.Inspect(kind,index,true);var r=GetComponent<Renderer>();if(r!=null)ShapeView.Tint(r,Color.yellow);}});}
    }
}
