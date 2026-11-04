using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
namespace Tecaverso.Labs.Functions
{
    public sealed class FunctionKnob : XRKnob
    {
        public FunctionsLab lab;
        public int index;
        public override bool IsSelectableBy(IXRSelectInteractor interactor) => lab && !lab.Modal && (index<2 || lab.Session.Family==FunctionFamily.Quadratic) && base.IsSelectableBy(interactor);
    }
}
