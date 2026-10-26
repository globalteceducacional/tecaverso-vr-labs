using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
namespace Tecaverso.Labs.Waves
{
    public sealed class WaveSwitch:XRLever
    {
        public WavesLab lab;
        public override bool IsSelectableBy(IXRSelectInteractor interactor)=>lab!=null&&lab.CanDrive&&lab.Mode!=0&&base.IsSelectableBy(interactor);
    }
}
