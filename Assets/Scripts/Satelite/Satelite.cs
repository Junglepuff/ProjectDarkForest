using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

// Tipos de estado para el satelite
public enum SateliteState
{
    Enabled,
    Disabled,
    Destroyed
}

public class Satelite : MonoBehaviour
{
    // Datos del satelite
    [Header("Datos del Satelite")]
    [Tooltip("General")]
    [SerializeField] private string sateliteID = "001";
    [SerializeField] private string sateliteName = "Voyager";
    [SerializeField] private string sateliteLocation = "Ofiuco";
    [Tooltip("Estado actual")]
    [SerializeField] private SateliteState state = SateliteState.Enabled;
    [SerializeField] private bool hasAnomaly = false;
    [Tooltip("Cooldown (segundos)")]
    [SerializeField] private float prCooldown = 30f;
    [SerializeField] private bool hasBeenPinged = false;

    // Referencias a TextMeshPro
    [Header("Referencias TextMeshPro")]
    [Tooltip("Referencia al componente TextMeshPro para el nombre")]
    [SerializeField] private TMP_Text nameTextTMP;
    [SerializeField] private TMP_Text nameTextScreenTMP;
    [Tooltip("Referencia al componente TextMeshPro para el estado")]
    [SerializeField] private TMP_Text stateTextTMP;
    [Tooltip("Referencia al componente TextMeshPro para la ubicacion")]
    [SerializeField] private TMP_Text locationTextTMP;
    [Tooltip("Referencia al componente TextMeshPro para el cooldown timer")]
    [SerializeField] private TMP_Text cooldownTextTMP;

    // Referencias a los botones Reset y Ping
    [Header("Referencias Botones")]
    [Tooltip("Referencia al boton de resetear satelite.")]
    [SerializeField] private GameObject resetButton;
    [Tooltip("Referencia al boton de pingear satelite.")]
    [SerializeField] private GameObject pingButton;

    // Referencias a camaras
    [Header("Referencias Camaras")]
    [Tooltip("Referencia al monitor")]
    [SerializeField] private GameObject monitor;
    [Tooltip("Referencia a la posicion de la camara del monitor")]
    [SerializeField] private GameObject cameraMonitorPos;
    private VideoPlayer monitorVideoPlayer;

    // Control de CD
    private Coroutine cooldownRoutine;

    // Prefs helpers
    private string PrefKey(string field) => $"Satelite.{sateliteID}.{field}";
    private string GetPrefString(string field, string fallback) =>
        PlayerPrefs.HasKey(PrefKey(field)) ? PlayerPrefs.GetString(PrefKey(field)) : fallback;
    private int GetPrefInt(string field, int fallback) =>
        PlayerPrefs.HasKey(PrefKey(field)) ? PlayerPrefs.GetInt(PrefKey(field)) : fallback;


    // Propiedades publicas para leer y modificar datos
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public string ID
    {
        get => GetPrefString("ID", sateliteID);
        set
        {
            if (sateliteID == value) return;
            sateliteID = value;
            PersistData();
            UpdateMonitorData();
        }
    }

    public string Name
    {
        get => GetPrefString("Name", sateliteName);
        set
        {
            if (sateliteName == value) return;
            sateliteName = value;
            PersistData();
            UpdateMonitorData();
        }
    }

    public string Location
    {
        get => GetPrefString("Location", sateliteLocation);
        set
        {
            if (sateliteLocation == value) return;
            sateliteLocation = value;
            PersistData();
            UpdateMonitorData();
        }
    }

    public string ScreenText
    {
        get => GetPrefString("ScreenText", nameTextScreenTMP.text);
        set
        {
            if (nameTextScreenTMP.text == value) return;
            nameTextScreenTMP.text = value;
            PersistData();
        }
    }

    public SateliteState State
    {
        get => (SateliteState)GetPrefInt("State", (int)state);
        set
        {
            if (state == value) return;
            state = value;
            PersistData();
            OnStateChanged();
        }
    }

    public bool HasAnomaly
    {
        get => hasAnomaly;
        set
        {
            hasAnomaly = value;
            if (hasAnomaly)
            {
                State = SateliteState.Disabled;
                OnStateChanged();
            }
        }
    }

    public bool HasBeenPinged
    {
        get => hasBeenPinged;
        set => hasBeenPinged = value;
    }

    // Pingea el satelite
    public void PingSatelite()
    {
        var camMover = Camera.main != null ? Camera.main.GetComponent<CameraClickMove>() : null;

        if (cameraMonitorPos == null) return;

        if (camMover == null) return;

        if (cooldownTextTMP == null) return;

        if (!HasBeenPinged)
        {
            HasBeenPinged = true;
            ScreenText = ID + "." + Name;

            // Mover camara hacia el monitor
            if (camMover != null && cameraMonitorPos != null)
            {
                camMover.SetNewCameraTarget(cameraMonitorPos.transform);
            }

            if (!HasAnomaly)
            {
                UpdateMonitorVideo(true);

                // Comenzar cooldown
                if (cooldownRoutine != null) StopCoroutine(cooldownRoutine);
                cooldownRoutine = StartCoroutine(CooldownCoroutine());
            } else
            {
                UpdateMonitorVideo(true);

                // TODO: Apagar monitor y mostrar cooldown del monitor

                State = SateliteState.Destroyed;
            }
        }
    }

    // Resetea el satelite
    public void ResetSatelite()
    {
        if (State == SateliteState.Disabled)
        {
            if (HasAnomaly)
            {
                State = SateliteState.Destroyed;
            }
            else
            {
                State = SateliteState.Enabled;
            }

            // Comenzar cooldown
            if (cooldownRoutine != null) StopCoroutine(cooldownRoutine);
            cooldownRoutine = StartCoroutine(CooldownCoroutine());
        }
    }

    // Metodos privados
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Carga los datos al instanciar el satelite
    private void Awake()
    {
        LoadData();
    }

    // Inicializa el satelite
    private void Start()
    {
        OnStateChanged();
        ScreenText = ID + "." + Name;
        UpdateMonitorVideo(false);
    }

    // Carga los datos del satelite desde PlayerPrefs
    private void LoadData()
    {
        if (PlayerPrefs.HasKey(PrefKey("ID"))) sateliteID = PlayerPrefs.GetString(PrefKey("ID"));
        if (PlayerPrefs.HasKey(PrefKey("Name"))) sateliteName = PlayerPrefs.GetString(PrefKey("Name"));
        if (PlayerPrefs.HasKey(PrefKey("Location"))) sateliteLocation = PlayerPrefs.GetString(PrefKey("Location"));
        if (PlayerPrefs.HasKey(PrefKey("ScreenText"))) nameTextScreenTMP.text = PlayerPrefs.GetString(PrefKey("ScreenText"));
        if (PlayerPrefs.HasKey(PrefKey("State"))) state = (SateliteState)PlayerPrefs.GetInt(PrefKey("State"));
    }

    // Persiste los datos del satelite
    void PersistData()
    {
        PlayerPrefs.SetString(PrefKey("ID"), sateliteID);
        PlayerPrefs.SetString(PrefKey("Name"), sateliteName);
        PlayerPrefs.SetString(PrefKey("Location"), sateliteLocation);
        PlayerPrefs.SetString(PrefKey("ScreenText"), nameTextScreenTMP.text);
        PlayerPrefs.SetInt(PrefKey("State"), (int)state);

        PlayerPrefs.Save();
    }

    // Actualiza los textos en pantalla y visibilidad de botones
    void UpdateMonitorData()
    {
        string idText = ID;
        string nameText = Name;
        string stateText = State.ToString();
        string locationText = Location;

        // Si state es Destroyed ocultar timer y botones
        if (State == SateliteState.Destroyed)
        {
            if (cooldownTextTMP != null) cooldownTextTMP.gameObject.SetActive(false);
            if (resetButton != null) resetButton.gameObject.SetActive(false);
            if (pingButton != null) pingButton.gameObject.SetActive(false);
        } else if (State == SateliteState.Disabled)
        {
            if (resetButton != null) resetButton.gameObject.SetActive(true);
            if (pingButton != null) pingButton.gameObject.SetActive(false);
        } else // Enabled
        {
            if (resetButton != null) resetButton.gameObject.SetActive(false);
            if (pingButton != null) pingButton.gameObject.SetActive(true);
        }
        // Actualiza los textos en pantalla
        if (nameTextTMP != null) nameTextTMP.text = ">> " + idText + "." + nameText + "Sat";
        if (stateTextTMP != null) stateTextTMP.text = stateText;
        if (locationTextTMP != null) locationTextTMP.text = locationText;
    }

    // Se ejecuta cuando el estado del satelite cambia
    void OnStateChanged()
    {
        UpdateMonitorData();

        // Cambia el color del texto segun el estado
        Color c = Color.white;
        switch (State)
        {
            case SateliteState.Enabled:
                c = Color.white;
                break;
            case SateliteState.Disabled:
                c = Color.softRed;
                break;
            case SateliteState.Destroyed:
                c = Color.gray3;
                break;
        }

        if (nameTextTMP != null) nameTextTMP.color = c;
        if (stateTextTMP != null) stateTextTMP.color = c;
        if (locationTextTMP != null) locationTextTMP.color = c;
    }

    // Pilla el VideoPlayer del monitor
    private void EnsureVideoPlayer()
    {
        if (monitorVideoPlayer != null) return;
        if (monitor == null) return;

        monitorVideoPlayer = monitor.GetComponentInChildren<VideoPlayer>() ?? monitor.GetComponent<VideoPlayer>();
    }

    // Asigna el .mp4 segun ID y HasAnomaly
    private void UpdateMonitorVideo(bool play)
    {
        EnsureVideoPlayer();
        if (monitorVideoPlayer == null || monitor == null) return;

        string folder = HasAnomaly ? "ANOMALY" : "NORMAL";

        int idNum = 1;
        if (!int.TryParse(ID, out idNum))
        {
            var trimmed = ID.TrimStart('0');
            if (!int.TryParse(trimmed, out idNum)) idNum = 1;
        }

        string fileName = $"planet{idNum}_{(HasAnomaly ? "ANOMALY" : "NORMAL")}.mp4";
        string fullPath = Path.Combine(Application.dataPath, "Videos", folder, fileName);

        monitorVideoPlayer.source = VideoSource.Url;
        monitorVideoPlayer.url = "file://" + fullPath;

        if (play)
        {
            monitorVideoPlayer.Play();
        }

        // TODO: Correr un timer para reemplazar el video con estatica despues de X segundos
    }

    // Coroutine para controlar el cooldown del ping
    private IEnumerator CooldownCoroutine()
    {
        // Inicio del cooldown
        resetButton.gameObject.SetActive(false);
        pingButton.gameObject.SetActive(false);

        int remaining = Mathf.CeilToInt(prCooldown);

        cooldownTextTMP.gameObject.SetActive(true);

        while (remaining > 0)
        {
            cooldownTextTMP.text = remaining.ToString() + "s";
            yield return new WaitForSeconds(1f);
            remaining--;
        }

        // Final del cooldown
        cooldownTextTMP.gameObject.SetActive(false);
        cooldownTextTMP.text = "";
        hasBeenPinged = false;
        cooldownRoutine = null;
        OnStateChanged();
    }
}
