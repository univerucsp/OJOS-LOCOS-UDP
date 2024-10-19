using UnityEngine;
using TMPro; // Asegúrate de usar TextMeshPro
using System.Collections;

public class TextMeshProFader : MonoBehaviour
{
    public TextMeshProUGUI text1; // Asigna en el inspector
    public TextMeshProUGUI text2; // Asigna en el inspector
    public TextMeshProUGUI text3; // Asigna en el inspector

    private void Start()
    {
        // Hacer los textos visibles al inicio
        text1.gameObject.SetActive(true);
        text2.gameObject.SetActive(true);
        text3.gameObject.SetActive(true);
        
        // Iniciar la coroutine para desvanecer los textos
        StartCoroutine(FadeOutTexts());
    }

    private IEnumerator FadeOutTexts()
    {
        // Espera 10 segundos
        yield return new WaitForSeconds(10f);

        // Duración para desvanecer
        float duration = 2f;
        float timeElapsed = 0f;

        // Recorrido para desvanecer
        Color startColor1 = text1.color;
        Color startColor2 = text2.color;
        Color startColor3 = text3.color;

        while (timeElapsed < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, timeElapsed / duration);

            // Aplicar el nuevo color con el valor alpha
            text1.color = new Color(startColor1.r, startColor1.g, startColor1.b, alpha);
            text2.color = new Color(startColor2.r, startColor2.g, startColor2.b, alpha);
            text3.color = new Color(startColor3.r, startColor3.g, startColor3.b, alpha);

            timeElapsed += Time.deltaTime;
            yield return null; // Espera un frame
        }

        // Asegurarse de que el alpha sea 0 al final
        text1.color = new Color(startColor1.r, startColor1.g, startColor1.b, 0f);
        text2.color = new Color(startColor2.r, startColor2.g, startColor2.b, 0f);
        text3.color = new Color(startColor3.r, startColor3.g, startColor3.b, 0f);

        // Desactivar los textos después de que se hayan desvanecido
        text1.gameObject.SetActive(false);
        text2.gameObject.SetActive(false);
        text3.gameObject.SetActive(false);
    }
}

