using UnityEngine;

public class CameraClickMove : MonoBehaviour
{
    [Header("Objetos clickeables")]
    public GameObject objeto1;
    public GameObject objeto2;
    public GameObject objeto3;
    public GameObject objeto4;

    [Header("Posiciones y rotaciones objetivo de la cámara")]
    public Transform camPos1;
    public Transform camPos2;
    public Transform camPos3;
    public Transform camPos4;

    [Header("Movimiento de cámara")]
    [Tooltip("Velocidad del movimiento de la cámara")]
    public float moveSpeed = 3f;

    private Camera mainCamera;
    private CameraShake shakeScript;

    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private Vector3 basePosition;
    private Vector3 baseRotationEuler;
    private bool moving = false;

    [Header("Configuración de la Nota")]
    public Transform note;
    public Transform notePos1;
    public Transform notePos2;
    [Tooltip("Velocidad del movimiento de la nota")]
    public float noteMoveSpeed = 4f;

    private Vector3 noteTargetPosition;
    private Quaternion noteTargetRotation;
    private bool noteMoving = false;

    void Start()
    {
        mainCamera = Camera.main;
        shakeScript = mainCamera.GetComponent<CameraShake>();

        if (shakeScript == null)
            Debug.LogWarning("No se encontró el script CameraConstantShake en la cámara principal.");

        // Inicializamos posiciones iniciales
        targetPosition = mainCamera.transform.localPosition;
        targetRotation = mainCamera.transform.localRotation;
        basePosition = targetPosition;
        baseRotationEuler = mainCamera.transform.localEulerAngles;

        // Posición inicial de la nota
        if (note != null && notePos1 != null)
        {
            note.position = notePos1.position;
            note.rotation = notePos1.rotation;
            noteTargetPosition = notePos1.position;
            noteTargetRotation = notePos1.rotation;
        }
    }

    void Update()
    {
        DetectarClick();
        MoverCamara();
        MoverNota();
    }

    void DetectarClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == objeto1)
                {
                    SetNewCameraTarget(camPos1);
                    SetNoteTarget(notePos1);
                }
                else if (hit.collider.gameObject == objeto2)
                {
                    SetNewCameraTarget(camPos2);
                    SetNoteTarget(notePos1);
                }
                else if (hit.collider.gameObject == objeto3)
                {
                    SetNewCameraTarget(camPos3);
                    SetNoteTarget(notePos1);
                }
                else if (hit.collider.gameObject == objeto4)
                {
                    SetNewCameraTarget(camPos4);
                    SetNoteTarget(notePos2);
                }
            }
        }
    }

    // Hice este metodo Publico para poder llamarlo desde otros scripts jej
    public void SetNewCameraTarget(Transform newTarget)
    {
        targetPosition = newTarget.localPosition;
        targetRotation = newTarget.localRotation;

        basePosition = newTarget.localPosition;
        baseRotationEuler = newTarget.localEulerAngles;

        moving = true;
    }

    // Hice este metodo Publico para poder llamarlo desde otros scripts jej2
    public void SetNoteTarget(Transform newTarget)
    {
        if (note == null) return;

        noteTargetPosition = newTarget.position;
        noteTargetRotation = newTarget.rotation;
        noteMoving = true;
    }

    void MoverCamara()
    {
        if (!moving) return;

        Transform camTransform = mainCamera.transform;

        camTransform.localPosition = Vector3.Lerp(camTransform.localPosition, targetPosition, Time.deltaTime * moveSpeed);
        camTransform.localRotation = Quaternion.Lerp(camTransform.localRotation, targetRotation, Time.deltaTime * moveSpeed);

        // Cuando llega al destino, fijamos la base del shake
        if (Vector3.Distance(camTransform.localPosition, targetPosition) < 0.01f &&
            Quaternion.Angle(camTransform.localRotation, targetRotation) < 0.5f)
        {
            moving = false;

            if (shakeScript != null)
            {
                shakeScript.baseRotationEuler = baseRotationEuler;
            }
        }
    }

    void MoverNota()
    {
        if (!noteMoving || note == null) return;

        note.position = Vector3.Lerp(note.position, noteTargetPosition, Time.deltaTime * noteMoveSpeed);
        note.rotation = Quaternion.Slerp(note.rotation, noteTargetRotation, Time.deltaTime * noteMoveSpeed);

        if (Vector3.Distance(note.position, noteTargetPosition) < 0.01f &&
            Quaternion.Angle(note.rotation, noteTargetRotation) < 0.5f)
        {
            noteMoving = false;
        }
    }
}
