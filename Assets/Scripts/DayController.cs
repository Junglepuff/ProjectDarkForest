using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Collections;
using System.Security.Principal; // para obtener el nombre del usuario en Windows

public class DayController : MonoBehaviour
{
    [Header("Fade Settings")]
    public Image fadeImage;
    public float fadeDuration = 2f;
    public int nextScene = 2;

    [Header("Timer Settings")]
    public TextMeshProUGUI timerText;
    public float totalTime = 480f;

    [Header("Fecha Settings")]
    public TextMeshProUGUI fechaText;

    [Header("Worker Settings")]
    public TextMeshProUGUI workerIDText;

    private float elapsedTime = 0f;

    void Start()
    {
        int dia = PlayerPrefs.GetInt("DIA", 0);
        System.DateTime baseDate = new System.DateTime(1988, 11, 6);
        System.DateTime currentDate = baseDate.AddDays(dia);
        fechaText.text = currentDate.ToString("dd/MM/yyyy");

        string workerID = "workerID_" + GetSystemUserOrRandom();
        if (workerIDText != null)
            workerIDText.text = workerID;
        else
            Debug.Log(workerID);

        StartCoroutine(DayCycleRoutine());
    }

    IEnumerator DayCycleRoutine()
    {
        yield return StartCoroutine(FadeImage(1f, 0f));
        yield return StartCoroutine(StartTimer());
        yield return StartCoroutine(FadeImage(0f, 1f));

        int diaActual = PlayerPrefs.GetInt("DIA", 0);
        PlayerPrefs.SetInt("DIA", diaActual + 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene(nextScene);
    }

    IEnumerator FadeImage(float startAlpha, float endAlpha)
    {
        float t = 0f;
        Color color = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / fadeDuration);
            color.a = Mathf.Lerp(startAlpha, endAlpha, normalized);
            fadeImage.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadeImage.color = color;
    }

    IEnumerator StartTimer()
    {
        float nextUpdate = 0f;

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= nextUpdate)
            {
                int minutes = Mathf.FloorToInt(elapsedTime / 60);
                int seconds = Mathf.FloorToInt(elapsedTime % 60);
                timerText.text = $"{minutes:00}:{seconds:00}";
                nextUpdate += 15f;
            }
            yield return null;
        }

        timerText.text = "08:00";
    }

    string GetSystemUserOrRandom()
    {
        try
        {
            string userName = Environment.UserName;

            if (string.IsNullOrEmpty(userName))
            {
                userName = Environment.GetEnvironmentVariable("USERNAME");
                if (string.IsNullOrEmpty(userName))
                    userName = Environment.GetEnvironmentVariable("USER");
            }

            if (string.IsNullOrEmpty(userName))
            {
                int randomID = UnityEngine.Random.Range(100, 1000);
                return randomID.ToString();
            }

            return userName;
        } catch
        {
            int randomID = UnityEngine.Random.Range(100, 1000);
            return randomID.ToString();
        }
    }
}