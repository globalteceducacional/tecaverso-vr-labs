using UnityEngine;

namespace Tecaverso.Labs.InclinedPlane
{
    public sealed class InclinedPlaneLab : MonoBehaviour
    {
        [SerializeField] InclinedPlanePanel panel;
        [SerializeField] InclinedPlaneForcesView forcesView;
        [SerializeField] Transform rampPivot, block, support;
        public InclinedPlaneModel Model { get; } = new InclinedPlaneModel();
        public void Bind(InclinedPlanePanel ui, InclinedPlaneForcesView vectors, Transform pivot, Transform body, Transform supportColumn)
        { panel=ui; forcesView=vectors; rampPivot=pivot; block=body; support=supportColumn; }
        void OnEnable()
        {
            panel.ParametersChanged+=Configure;
            panel.ReleaseRequested+=Release;
            panel.PauseRequested+=Pause;
            panel.ResetRequested+=ResetLab;
            panel.VisibilityChanged+=Present;
        }
        void OnDisable()
        {
            panel.ParametersChanged-=Configure;
            panel.ReleaseRequested-=Release;
            panel.PauseRequested-=Pause;
            panel.ResetRequested-=ResetLab;
            panel.VisibilityChanged-=Present;
        }
        void Start() => Configure();
        void Update() { if(Model.Advance(Time.deltaTime)) Present(); }
        void Configure() { Model.Configure(panel.Parameters); Present(); }
        public void Release() { Model.Release(); Present(); }
        public void Pause() { Model.TogglePause(); Present(); }
        public void ResetLab() { Model.Reset(); Configure(); }
        public void Present()
        {
            var p=Model.Parameters;
            rampPivot.localRotation=Quaternion.Euler(0,0,-p.Angle);
            var top=rampPivot.position-p.DownSlope*InclinedPlaneModel.RampLength;
            block.SetPositionAndRotation(top+p.DownSlope*(InclinedPlaneModel.StartOffset+Model.Distance)+p.OutwardNormal*.3f, rampPivot.rotation);
            support.position=new Vector3(top.x,top.y*.5f,top.z);
            support.localScale=new Vector3(.18f,top.y,.6f);
            forcesView.Show(block.position,p,Model.Forces,panel.VectorsVisible,panel.ResultantVisible,panel.ComponentsVisible);
            panel.Show(Model,forcesView.Scale(p));
        }
    }
}
