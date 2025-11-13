using UnityEngine;
using TMPro;

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
    [SerializeField] private float pingCooldown = 30f;
    [SerializeField] private bool hasBeenPinged = false;

    // Referencias a TextMeshPro
    [Header("Referencias TextMeshPro")]
    [Tooltip("Referencia al componente TextMeshPro para el nombre.")]
    [SerializeField] private TextMeshPro nameTextTMP;
    [Tooltip("Referencia al componente TextMeshPro para el estado.")]
    [SerializeField] private TextMeshPro stateTextTMP;
    [Tooltip("Referencia al componente TextMeshPro para la ubicacion.")]
    [SerializeField] private TextMeshPro locationTextTMP;


    // Propiedades publicas para leer y modificar datos
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public string ID
    {
        get => sateliteID;
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
        get => sateliteName;
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
        get => sateliteLocation;
        set
        {
            if (sateliteLocation == value) return;
            sateliteLocation = value;
            UpdateTexts();
            PersistData();
        }
    }

    public SateliteState State
    {
        get => state;
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
            // Si tiene anomalia, establecer como deshabilitado
            if (hasAnomaly)
            {
                State = SateliteState.Disabled;
                OnStateChanged();
            }
            PersistData();
        }
    }

    public bool HasBeenPinged
    {
        get => hasBeenPinged;
        set => hasBeenPinged = value;
    }

    // Hacer ping al satelite
    public void PingSatelite()
    {
        // TODO: Si el satelite esta habilitado y no esta en cooldown:
        //      Mover camara hacia el monitor
        //      Mostrar la camara del satelite
        //          - Si tiene anomalia, mostrar video anomalia
        //              - Apagar monitor despues de X segundos
        //              - Poner en cooldown el monitor
        //          - Si no tiene anomalia, mostrar video normal
        //              - Poner en cooldown el ping

    }

    // Resetear el satelite
    public void ResetSatelite()
    {
        // TODO: Si el satelite esta deshabilitado:
        //      - Si tiene anomalia, destruir satelite
        //      - Si no tiene anomalia, habilitar satelite
    }

    // Metodos privados
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    private void Start()
    {
        OnStateChanged();
    }

    void PersistData()
    {
        // TODO: Persistir cambio en datos con PlayerPrefs
    }

    void UpdateTexts()
    {
        string idText = sateliteID;
        string nameText = sateliteName;
        string stateText = state.ToString();
        string locationText = sateliteLocation;

        // Actualiza los textos en pantalla para el satelite
        if (nameTextTMP != null) nameTextTMP.text = ">> " + idText + "." + nameText + "Sat";
        if (stateTextTMP != null) stateTextTMP.text = stateText;
        if (locationTextTMP != null) locationTextTMP.text = locationText;
    }

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
}
