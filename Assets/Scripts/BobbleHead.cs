using UnityEngine;

public class BobbleHead : MonoBehaviour
{
    [Header("Configuración del movimiento")]
    [Tooltip("Eje de rotación principal del cabeceo (por ejemplo, Vector3.right para adelante/atrás).")]
    public Vector3 rotationAxis = Vector3.right;

    [Tooltip("Intensidad máxima del movimiento inicial al clic.")]
    public float initialShakeAngle = 15f;

    [Tooltip("Velocidad del rebote del cabeceo.")]
    public float shakeFrequency = 5f;

    [Tooltip("Amortiguación (cuánto tarda en detenerse el movimiento).")]
    [Range(0f, 5f)]
    public float damping = 1.5f;

    [Tooltip("Habilitar clic directo en este objeto.")]
    public bool clickable = true;

    private float shakeAmount = 0f;
    private float shakeTime = 0f;
    private Quaternion baseRotation;

    void Start()
    {
        baseRotation = transform.localRotation;
    }

    void Update()
    {
        // Si hay movimiento activo
        if (shakeAmount > 0.001f)
        {
            shakeTime += Time.deltaTime * shakeFrequency;

            // Movimiento amortiguado tipo resorte (seno atenuado)
            float currentAngle = Mathf.Sin(shakeTime) * shakeAmount;

            // Aplica rotación relativa al eje
            transform.localRotation = baseRotation * Quaternion.AngleAxis(currentAngle, rotationAxis);

            // Reduce el movimiento gradualmente (amortiguación)
            shakeAmount = Mathf.Lerp(shakeAmount, 0f, Time.deltaTime * damping);
        }
    }

    void OnMouseDown()
    {
        if (!clickable) return;

        // Reinicia el tiempo y aplica nueva intensidad
        shakeTime = 0f;
        shakeAmount = initialShakeAngle;
    }

    // Puedes llamarlo desde otros scripts también:
    public void TriggerBobble()
    {
        shakeTime = 0f;
        shakeAmount = initialShakeAngle;
    }
}
