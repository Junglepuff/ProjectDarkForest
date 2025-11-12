using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Renderer))]
public class ColorCycleWithBloomSmooth : MonoBehaviour
{
    [Header("Colores disponibles")]
    [Tooltip("Lista de colores posibles para la emisión.")]
    public List<Color> colorList = new List<Color>();

    [Header("Material base (opcional)")]
    public Material baseMaterial;

    [Header("Intervalo de cambio de color (segundos)")]
    public float minChangeInterval = 5f;
    public float maxChangeInterval = 120f;

    [Header("Emisión")]
    [Tooltip("Rango de intensidad aleatoria para la emisión del objeto.")]
    public float minEmissionIntensity = 1f;
    public float maxEmissionIntensity = 3f;

    [Header("Bloom (Post Processing Volume)")]
    [Tooltip("Volume que contiene el efecto Bloom.")]
    public Volume postProcessVolume;

    [Header("Transición")]
    [Tooltip("Duración de la interpolación entre colores (segundos)")]
    public float transitionDuration = 2f;

    private Renderer rend;
    private Material instanceMaterial;
    private Color currentColor;
    private float currentEmissionIntensity;
    private Bloom bloomEffect;

    void Start()
    {
        rend = GetComponent<Renderer>();
        instanceMaterial = baseMaterial != null ? new Material(baseMaterial) : new Material(rend.material);
        rend.material = instanceMaterial;

        if (postProcessVolume != null && postProcessVolume.profile != null)
        {
            postProcessVolume.profile.TryGet(out bloomEffect);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: No se ha asignado un Volume con Bloom en el inspector.");
        }

        if (colorList == null || colorList.Count == 0)
        {
            Debug.LogWarning($"{gameObject.name}: No hay colores en la lista de colores.");
            return;
        }

        currentColor = colorList[Random.Range(0, colorList.Count)];
        currentEmissionIntensity = Random.Range(minEmissionIntensity, maxEmissionIntensity);

        ApplyEmission(currentColor, currentEmissionIntensity);
        StartCoroutine(ColorChangeRoutine());
    }

    IEnumerator ColorChangeRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minChangeInterval, maxChangeInterval);
            yield return new WaitForSeconds(waitTime);

            Color newColor = colorList[Random.Range(0, colorList.Count)];
            while (newColor == currentColor && colorList.Count > 1)
            {
                newColor = colorList[Random.Range(0, colorList.Count)];
            }

            float newIntensity = Random.Range(minEmissionIntensity, maxEmissionIntensity);
            yield return StartCoroutine(TransitionToNewColor(newColor, newIntensity));

            currentColor = newColor;
            currentEmissionIntensity = newIntensity;
        }
    }

    IEnumerator TransitionToNewColor(Color targetColor, float targetIntensity)
    {
        Color startColor = currentColor;
        float startIntensity = currentEmissionIntensity;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / transitionDuration);

            Color lerpedColor = Color.Lerp(startColor, targetColor, t);
            float lerpedIntensity = Mathf.Lerp(startIntensity, targetIntensity, t);

            ApplyEmission(lerpedColor, lerpedIntensity);

            yield return null;
        }

        ApplyEmission(targetColor, targetIntensity);
    }

    void ApplyEmission(Color color, float intensity)
    {
        if (instanceMaterial == null) return;

        instanceMaterial.color = color;
        instanceMaterial.EnableKeyword("_EMISSION");
        instanceMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

        instanceMaterial.SetColor("_EmissionColor", color * Mathf.LinearToGammaSpace(intensity));
        DynamicGI.SetEmissive(rend, color * intensity);

        if (bloomEffect != null)
        {
            bloomEffect.intensity.value = intensity;
        }
    }
}
