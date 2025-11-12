using System.Collections;
using UnityEngine;
using TMPro;

public class TypeWriterLoop : MonoBehaviour
{
    [Header("Componentes")]
    public TMP_Text textMeshPro;

    [Header("Configuración del efecto")]
    [TextArea]
    public string fullText;
    public float typingSpeed = 0.05f;
    public float holdTime = 2f;

    private void Start()
    {
        if (textMeshPro == null)
            textMeshPro = GetComponent<TMP_Text>();

        StartCoroutine(TypewriterEffect());
    }

    private IEnumerator TypewriterEffect()
    {
        while (true)
        {
            textMeshPro.text = "";

            for (int i = 0; i < fullText.Length; i++)
            {
                textMeshPro.text += fullText[i];
                yield return new WaitForSeconds(typingSpeed);
            }

            yield return new WaitForSeconds(holdTime);
            yield return new WaitForSeconds(0.2f);
        }
    }
}
