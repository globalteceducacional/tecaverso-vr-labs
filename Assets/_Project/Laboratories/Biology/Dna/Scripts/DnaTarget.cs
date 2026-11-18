using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Tecaverso.Labs.Biology.Dna
{
    public sealed class DnaTarget : XRSimpleInteractable
    {
        public DnaLab lab;
        // 0..3 are empty sockets. The two remaining targets are museum exhibits.
        public int targetIndex;
        protected override void OnSelectEntered(SelectEnterEventArgs e)
        {
            base.OnSelectEntered(e);
            if (lab) lab.ChooseTarget(this);
        }
    }
}
