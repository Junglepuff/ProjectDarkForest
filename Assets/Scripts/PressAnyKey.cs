using UnityEngine;
using UnityEngine.SceneManagement;


public class PressAnyKey : MonoBehaviour
{
    void Update()
    {
        if(Input.anyKey)
        {
            SceneManager.LoadScene(2);
        }        
    }
}
