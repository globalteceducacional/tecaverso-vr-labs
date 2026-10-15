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

        /// <summary>Explicit scale for other physical quantities. Does not change the legacy velocity rendering.</summary>
        public void SetScaledVector(Vector3 vector, float worldUnitsPerUnit)
        {
            float length = vector.magnitude * Mathf.Max(0f, worldUnitsPerUnit);
            gameObject.SetActive(length > .0001f);
            if (!gameObject.activeSelf) return;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, vector.normalized);
            float tip = Mathf.Min(headLength, length * .6f);
            float stem = length - tip;
            shaft.localPosition = Vector3.up * stem * .5f;
            shaft.localRotation = Quaternion.identity;
            // Keep very small arrows tapered instead of letting the cylinder hide the cone.
            float radius = Mathf.Min(headRadius, tip * .5f);
            float diameter = Mathf.Min(shaftDiameter, radius * .65f);
            shaft.localScale = new Vector3(diameter, stem * .5f, diameter);
            head.localPosition = Vector3.up * stem;
            head.localRotation = Quaternion.identity;
            head.localScale = new Vector3(radius, tip, radius);
        }
    }
}
