using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Tecaverso.Labs.Thermology
{
    public sealed class ThermalKnob : XRKnob
    {
        public ThermologyLab lab;
        public override bool IsSelectableBy(IXRSelectInteractor interactor) => lab != null && lab.CanHeat && base.IsSelectableBy(interactor);
    }
}
