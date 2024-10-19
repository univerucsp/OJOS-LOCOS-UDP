using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public Image fadeImage; // Imagen que se usará para el fade
    public float fadeDuration = 1f; // Duración del fade

    private void Start()
    {
        // Realizar un fade in cuando se carga la escena
        StartCoroutine(FadeIn());
    }

    public void FadeToScene(string sceneName)
    {
        // Iniciar el fade out y luego cargar la nueva escena
        StartCoroutine(FadeOut(sceneName));
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        color.a = 1f; // Comenzar con la imagen completamente opaca

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color;
    }

    private IEnumerator FadeOut(string sceneName)
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        color.a = 0f; // Comenzar con la imagen completamente transparente

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;

        // Cargar la nueva escena después del fade out
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}

