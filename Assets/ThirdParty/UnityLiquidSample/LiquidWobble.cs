using UnityEngine;

//This implementation is inspired by common liquid wobble techniques used in real-time rendering.
public class LiquidWobble : MonoBehaviour
{
    private Renderer objectRenderer;

    private Vector3 previousPosition;
    private Vector3 previousRotation;

    private float wobbleX;
    private float wobbleZ;

    private float wobbleVelocityX;
    private float wobbleVelocityZ;

    private float elapsedTime;

    [Header("Wobble Settings")]
    public float wobbleStrength = 0.03f;
    public float wobbleSpeed = 1f;
    public float recoverySpeed = 1f;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();

        previousPosition = transform.position;
        previousRotation = transform.rotation.eulerAngles;
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;
        elapsedTime += deltaTime;

        // Gradually reduce wobble over time
        wobbleVelocityX = Mathf.Lerp(wobbleVelocityX, 0f, deltaTime * recoverySpeed);
        wobbleVelocityZ = Mathf.Lerp(wobbleVelocityZ, 0f, deltaTime * recoverySpeed);

        // Create oscillation using sine wave
        float wave = Mathf.PI * 2f * wobbleSpeed;

        wobbleX = wobbleVelocityX * Mathf.Sin(wave * elapsedTime);
        wobbleZ = wobbleVelocityZ * Mathf.Sin(wave * elapsedTime);

        // Apply to shader
        objectRenderer.material.SetFloat("_WobbleX", wobbleX);
        objectRenderer.material.SetFloat("_WobbleZ", wobbleZ);

        // Calculate movement
        Vector3 velocity = (transform.position - previousPosition) / deltaTime;
        Vector3 rotationDelta = transform.rotation.eulerAngles - previousRotation;

        // Convert movement into wobble force
        float wobbleForceX = velocity.x + rotationDelta.z * 0.2f;
        float wobbleForceZ = velocity.z + rotationDelta.x * 0.2f;

        wobbleVelocityX += Mathf.Clamp(wobbleForceX * wobbleStrength, -wobbleStrength, wobbleStrength);
        wobbleVelocityZ += Mathf.Clamp(wobbleForceZ * wobbleStrength, -wobbleStrength, wobbleStrength);

        // Store values for next frame
        previousPosition = transform.position;
        previousRotation = transform.rotation.eulerAngles;
    }
}