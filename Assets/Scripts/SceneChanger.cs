using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void StartGame()
    {
        ResetSatelitePlayerPrefsToDefaults();
        PlayerPrefs.SetInt("DAY", 0);
        SceneManager.LoadScene(1);
    }

    public void OptionsMenu()
    {
        // Implementar luego
        return;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    // Resetea los PlayerPrefs de todos los satelites a sus valores por defecto
    private void ResetSatelitePlayerPrefsToDefaults()
    {
        var sats = FindObjectsByType<Satelite>(FindObjectsSortMode.None);
        if (sats == null || sats.Length == 0) return;

        foreach (var sat in sats)
        {
            string id = sat.ID;
            string prefix = $"Satelite.{id}.";

            PlayerPrefs.SetString(prefix + "ID", sat.ID);
            PlayerPrefs.SetString(prefix + "Name", sat.Name);
            PlayerPrefs.SetString(prefix + "Location", sat.Location);
            PlayerPrefs.SetString(prefix + "ScreenText", sat.ScreenText ?? "");
            PlayerPrefs.SetInt(prefix + "State", (int)sat.State);
        }

        PlayerPrefs.Save();
    }
}
