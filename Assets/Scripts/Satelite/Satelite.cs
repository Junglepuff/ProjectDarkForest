using UnityEngine;
using TMPro;
using System.Collections;

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
    private string sateliteScreenText = "";
    [Tooltip("Estado actual")]
    [SerializeField] private SateliteState state = SateliteState.Enabled;
    [SerializeField] private bool hasAnomaly = false;
    [Tooltip("Cooldown (segundos)")]
    [SerializeField] private float prCooldown = 30f;
    [SerializeField] private bool hasBeenPinged = false;

    // Referencias a TextMeshPro
    [Header("Referencias TextMeshPro")]
    [Tooltip("Referencia al componente TextMeshPro para el nombre.")]
    [SerializeField] private TextMeshPro nameTextTMP;
    [SerializeField] private TextMeshPro nameTextScreeTMP;
    [Tooltip("Referencia al componente TextMeshPro para el estado.")]
    [SerializeField] private TextMeshPro stateTextTMP;
    [Tooltip("Referencia al componente TextMeshPro para la ubicacion.")]
    [SerializeField] private TextMeshPro locationTextTMP;
    [Tooltip("Referencia al componente TextMeshPro para el cooldown timer.")]
    [SerializeField] private TextMeshPro cooldownTextTMP;

    // Referencias a los botones Reset y Ping
    [Header("Referencias Botones")]
    [Tooltip("Referencia al boton de resetear satelite.")]
    [SerializeField] private GameObject resetButton;
    [Tooltip("Referencia al boton de pingear satelite.")]
    [SerializeField] private GameObject pingButton;

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
            UpdateTexts();
            PersistData();
        }
    }

    public string Name
    {
        get => GetPrefString("Name", sateliteName);
        set
        {
            if (sateliteName == value) return;
            sateliteName = value;
            UpdateTexts();
            PersistData();
        }
    }

    public string Location
    {
        get => GetPrefString("Location", sateliteLocation);
        set
        {
            if (sateliteLocation == value) return;
            sateliteLocation = value;
            UpdateTexts();
            PersistData();
        }
    }

    public string ScreenText
    {
        get => GetPrefString("ScreenText", sateliteScreenText);
        set
        {
            if (sateliteScreenText == value) return;
            sateliteScreenText = value;
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
            OnStateChanged();
            PersistData();
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
        if (cooldownTextTMP == null) return;

        if (!HasBeenPinged)
        {
            HasBeenPinged = true;
            ScreenText = ID + "." + Name;
            // Mover camara hacia el monitor

            if (!HasAnomaly)
            {
                // TODO: Mostrar video normal del satelite

                // Comenzar cooldown
                if (cooldownRoutine != null) StopCoroutine(cooldownRoutine);
                cooldownRoutine = StartCoroutine(CooldownCoroutine());
            } else
            {
                // TODO: Mostrar video de anomalia del satelite
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
    // Inicializa el satelite
    private void Start()
    {
        OnStateChanged();
        ScreenText = ID + "." + Name;
    }

    // Persiste los datos del satelite
    void PersistData()
    {
        PlayerPrefs.SetString(PrefKey("ID"), sateliteID);
        PlayerPrefs.SetString(PrefKey("Name"), sateliteName);
        PlayerPrefs.SetString(PrefKey("Location"), sateliteLocation);
        PlayerPrefs.SetString(PrefKey("ScreenText"), sateliteScreenText);
        PlayerPrefs.SetInt(PrefKey("State"), (int)state);

        PlayerPrefs.Save();
    }

    // Actualiza los textos en pantalla
    void UpdateTexts()
    {
        string idText = ID;
        string nameText = Name;
        string stateText = State.ToString();
        string locationText = Location;

        if (nameTextTMP != null) nameTextTMP.text = ">> " + idText + "." + nameText + "Sat";
        if (stateTextTMP != null) stateTextTMP.text = stateText;
        if (locationTextTMP != null) locationTextTMP.text = locationText;
    }

    // Se ejecuta cuando el estado del satelite cambia
    void OnStateChanged()
    {
        UpdateTexts();

        // Cambia el color del texto segun el estado
        Color c = Color.white;
        switch (state)
        {
            case SateliteState.Enabled:
                c = Color.white;
                break;
            case SateliteState.Disabled:
                c = Color.red;
                break;
            case SateliteState.Destroyed:
                c = Color.grey;
                break;
        }

        if (nameTextTMP != null) nameTextTMP.color = c;
        if (stateTextTMP != null) stateTextTMP.color = c;
        if (locationTextTMP != null) locationTextTMP.color = c;
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
        resetButton.gameObject.SetActive(true);
        pingButton.gameObject.SetActive(true);
    }
}
