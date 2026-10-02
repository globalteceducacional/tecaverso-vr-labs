using UnityEngine;

namespace Tecaverso.Labs.ObliqueLaunch
{
    public sealed class VectorArrowView : MonoBehaviour
    {
        [SerializeField] Transform shaft;
        [SerializeField] Transform head;
        [SerializeField, Min(.001f)] float unitsPerMeterPerSecond = .06f;
        [SerializeField, Min(.1f)] float lengthMultiplier = 2f;
        [SerializeField, Min(.001f)] float shaftDiameter = .05f;
        [SerializeField, Min(.001f)] float headRadius = .10f;
        [SerializeField, Min(.001f)] float headLength = .22f;

        public void SetVector(Vector3 vector)
        {
            float length = vector.magnitude * unitsPerMeterPerSecond * lengthMultiplier;
            gameObject.SetActive(vector.magnitude * unitsPerMeterPerSecond > .015f);
            if (!gameObject.activeSelf) return;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, vector.normalized);
            // Unity's cylinder spans -1..1 in Y; the cone mesh spans 0..1.
            // Only the shaft changes length; its end is exactly the cone's base.
            shaft.localPosition = Vector3.up * length * .5f;
            shaft.localRotation = Quaternion.identity;
            shaft.localScale = new Vector3(shaftDiameter, length * .5f, shaftDiameter);
            head.localPosition = Vector3.up * length;
            head.localRotation = Quaternion.identity;
            head.localScale = new Vector3(headRadius, headLength, headRadius);
        }

        public void Bind(Transform shaftTransform, Transform headTransform) { shaft = shaftTransform; head = headTransform; }
    }
}
