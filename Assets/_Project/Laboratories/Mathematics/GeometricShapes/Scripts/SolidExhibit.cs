using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Tecaverso.Labs.Shapes
{
    public sealed class SolidExhibit : MonoBehaviour
    {
        public Transform geometry, caption;
        public GameObject vertices, edges;
        public XRSimpleInteractable hoverTarget;
        public int vertexCount, edgeCount, faceCount;
        public bool rotating = true;
        Transform viewer;

        void Start() { if (Camera.main != null) viewer = Camera.main.transform; }
        void Update()
        {
            // isHovered accounts for multiple hands/rays and resets when an interactor is disabled.
            if (rotating && (hoverTarget == null || !hoverTarget.isHovered))
                geometry.Rotate(Vector3.up, 10 * Time.deltaTime, Space.Self);
        }
        void LateUpdate()
        {
            if (viewer == null || caption == null) return;
            var forward = caption.position - viewer.position;
            forward.y = 0;
            if (forward.sqrMagnitude > .01f) caption.rotation = Quaternion.LookRotation(forward);
        }
        public void ShowVertices(bool value) => vertices.SetActive(value && vertexCount > 0);
        public void ShowEdges(bool value) => edges.SetActive(value && edgeCount > 0);
    }
}
