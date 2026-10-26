using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
namespace Tecaverso.Labs.Waves
{
    public sealed class WavePoint:XRSimpleInteractable
    {
        public WavesLab lab;
        public int sampleIndex;
        protected override void Awake(){base.Awake();selectEntered.AddListener(_=>lab.SelectPoint(sampleIndex));}
        public override bool IsSelectableBy(IXRSelectInteractor interactor)=>lab!=null&&!lab.Modal&&base.IsSelectableBy(interactor);
    }
}
