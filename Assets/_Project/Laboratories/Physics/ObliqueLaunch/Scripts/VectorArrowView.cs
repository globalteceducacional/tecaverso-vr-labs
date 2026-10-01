using UnityEngine;

namespace Tecaverso.Labs.ObliqueLaunch
{
    public sealed class VectorArrowView : MonoBehaviour
    {
        [SerializeField] Transform shaft;
        [SerializeField] Transform head;
        [SerializeField, Min(.001f)] float unitsPerMeterPerSecond = .06f;

        public void SetVector(Vector3 vector)
        {
            float length = vector.magnitude * unitsPerMeterPerSecond;
            gameObject.SetActive(length > .015f);
            if (!gameObject.activeSelf) return;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, vector.normalized);
            float headLength = Mathf.Min(.16f, length * .35f);
            shaft.localPosition = Vector3.up * (length - headLength) * .5f;
            shaft.localScale = new Vector3(.025f, (length - headLength) * .5f, .025f);
            head.localPosition = Vector3.up * (length - headLength * .5f);
            head.localScale = new Vector3(.07f, headLength * .5f, .07f);
        }

        public void Bind(Transform shaftTransform, Transform headTransform) { shaft = shaftTransform; head = headTransform; }
    }
}
