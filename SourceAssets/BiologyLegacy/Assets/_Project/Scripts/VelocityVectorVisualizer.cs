using UnityEngine;

[ExecuteAlways]
public class VectorVisualizer : MonoBehaviour
{
    public enum VectorType { X, Y, Z, Resultant }

    [Header("Fonte de Dados")]
    public Rigidbody targetRb;

    [Header("Tipo de Vetor")]
    public VectorType vectorType = VectorType.Resultant;

    [Header("Escala")]
    public float scaleMultiplier = 0.1f;

    [Header("Opções")]
    public float zeroEpsilon = 1e-4f;

    Vector3 _baseScale;

    void Reset()
    {
        vectorType = VectorType.Resultant;
        TryAutoAssignRigidbody();
    }

    void OnEnable()
    {
        TryAutoAssignRigidbody();
        _baseScale = transform.localScale;
    }

    void TryAutoAssignRigidbody()
    {
        if (targetRb == null) targetRb = GetComponent<Rigidbody>();
        if (targetRb == null) targetRb = GetComponentInParent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (Application.isPlaying) UpdateTransform();
    }

    void UpdateTransform()
    {
        if (targetRb == null) return;

        Vector3 v = targetRb.linearVelocity;
        float value = 0f;
        Vector3 dir = Vector3.up;

        switch (vectorType)
        {
            case VectorType.X:
                value = v.x;
                dir = value >= 0f ? Vector3.right : Vector3.left;
                break;

            case VectorType.Y:
                value = v.y;
                dir = value >= 0f ? Vector3.up : Vector3.down;
                break;

            case VectorType.Z:
                value = v.z;
                dir = value >= 0f ? Vector3.forward : Vector3.back;
                break;

            case VectorType.Resultant:
                value = v.magnitude;
                if (value <= zeroEpsilon) { transform.localScale = Vector3.zero; return; }
                dir = v.normalized;
                break;
        }

        Quaternion rot = Quaternion.FromToRotation(Vector3.up, dir);
        transform.rotation = rot;

        float length = Mathf.Abs(value) * scaleMultiplier;
        Vector3 newScale = new Vector3(_baseScale.x, length, _baseScale.z);
        transform.localScale = newScale;

        transform.position = targetRb.position + dir.normalized * (length /** 0.5f*/);
    }
}
