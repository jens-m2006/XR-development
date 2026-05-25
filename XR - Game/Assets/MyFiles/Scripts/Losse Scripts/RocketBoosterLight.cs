using UnityEngine;

public class RocketBoosterLight : MonoBehaviour
{
    private Light boosterLight;

    [Header("Light Settings")]
    public float intensity = 10f;
    public float range = 10f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 15f;
    public float rotationAmount = 2f;

    private Quaternion startRotation;

    void Awake()
    {
        boosterLight = GetComponentInChildren<Light>();
        startRotation = transform.localRotation;
    }

    void Update()
    {
        if (boosterLight == null) return;

        // Static light (no dimming)
        boosterLight.intensity = intensity;
        boosterLight.range = range;

        // Simple rotation wobble
        float rotX = Mathf.Sin(Time.time * rotationSpeed) * rotationAmount;
        float rotY = Mathf.Cos(Time.time * rotationSpeed * 0.8f) * rotationAmount;
        float rotZ = Mathf.Sin(Time.time * rotationSpeed * 1.2f) * rotationAmount;

        transform.localRotation = startRotation * Quaternion.Euler(rotX, rotY, rotZ);
    }
}