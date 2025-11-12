using UnityEngine;

public class ButtonPressEffect : MonoBehaviour
{
    public float pressDepth = 0.05f;
    public float pressSpeed = 10f;
    private Vector3 originalPosition;

    private bool isPressed = false;
    private bool returning = false;

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    void OnMouseDown()
    {
        if (!isPressed && !returning)
        {
            StartCoroutine(PressAnimation());
        }
    }

    private System.Collections.IEnumerator PressAnimation()
    {
        isPressed = true;
        Vector3 targetPosition = originalPosition - new Vector3(0, pressDepth, 0);

        while (Vector3.Distance(transform.localPosition, targetPosition) > 0.001f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * pressSpeed);
            yield return null;
        }

        yield return new WaitForSeconds(0.05f);

        returning = true;
        while (Vector3.Distance(transform.localPosition, originalPosition) > 0.001f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * pressSpeed);
            yield return null;
        }

        transform.localPosition = originalPosition;
        isPressed = false;
        returning = false;
    }
}
