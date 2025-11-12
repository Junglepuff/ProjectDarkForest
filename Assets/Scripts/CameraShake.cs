using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Tooltip("Rotación base de la cámara.")]
    public Vector3 baseRotationEuler;

    [Header("Shake")]
    [Tooltip("Intensidad del shake de rotación (grados).")]
    [Range(0f, 5f)]
    public float rotationShakeIntensity = 1f;

    [Tooltip("Velocidad del shake.")]
    [Range(0.1f, 10f)]
    public float shakeSpeed = 2f;

    [Tooltip("Suavidad del movimiento del shake.")]
    [Range(0.1f, 20f)]
    public float smoothFactor = 8f;

    private float seedRotX, seedRotY, seedRotZ;
    private Quaternion baseRotation;
    private Quaternion targetShakeRotation;

    void Start()
    {
        seedRotX = Random.Range(0f, 1000f);
        seedRotY = Random.Range(0f, 1000f);
        seedRotZ = Random.Range(0f, 1000f);

        if (baseRotationEuler == Vector3.zero)
            baseRotationEuler = transform.localEulerAngles;

        baseRotation = Quaternion.Euler(baseRotationEuler);
        targetShakeRotation = baseRotation;
    }

    void Update()
    {
        float t = Time.time * shakeSpeed;

        // Genera pequeños offsets suaves con Perlin Noise
        float rotX = (Mathf.PerlinNoise(seedRotX, t) - 0.5f) * 2f * rotationShakeIntensity;
        float rotY = (Mathf.PerlinNoise(seedRotY, t) - 0.5f) * 2f * rotationShakeIntensity;
        float rotZ = (Mathf.PerlinNoise(seedRotZ, t) - 0.5f) * 2f * rotationShakeIntensity;

        // Calculamos la rotación objetivo del shake
        Quaternion shakeOffset = Quaternion.Euler(rotX, rotY, rotZ);
        targetShakeRotation = baseRotation * shakeOffset;

        // Aplicamos suavemente la rotación final mediante Slerp
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetShakeRotation, Time.deltaTime * smoothFactor);
    }

    // Permite actualizar dinámicamente la rotación base desde otros scripts
    public void SetBaseRotation(Vector3 newBaseEuler)
    {
        baseRotationEuler = newBaseEuler;
        baseRotation = Quaternion.Euler(newBaseEuler);
    }
}
