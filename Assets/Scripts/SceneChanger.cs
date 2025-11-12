using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void StartGame()
    {
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
}
