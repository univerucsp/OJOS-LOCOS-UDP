using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI; // Asegúrate de incluir esta línea para usar UI
using System.Collections;
using UnityEngine.SceneManagement; // Asegúrate de incluir esta línea para cambiar de escena

using TMPro; // Agrega esta línea al principio del script

public class EyeTrackingClient : MonoBehaviour
{
    private SceneFader sceneFader; // Referencia al script SceneFader
    
    UdpClient udpClient;
    public int listenPort = 65433;

    public GameObject directionIndicator; // Prefab de la flecha de dirección
    public GameObject redDot; // Prefab del punto rojo
    public GameObject circle; // Prefab del círculo
    public Transform plane; // Referencia al plano en la escena
    public float circleRadius = 5.0f; // Radio del círculo
    public float planeWidth = 10f; // Ancho del plano
    public float planeHeight = 10f; // Alto del plano

    public AudioSource warningSound; // AudioSource para reproducir el sonido de advertencia
    public AudioSource pointSound; // AudioSource para el sonido de ganar puntos
    public AudioSource musicSource; // AudioSource para la música de fondo
    public TextMeshProUGUI countdownText; // Texto para mostrar el contador (opcional)
    private int score = 0; // Variable para llevar la puntuación

    private Vector3 circlePosition; // Posición actual del círculo

    void Start()
    {
        udpClient = new UdpClient(listenPort);
        SpawnRandomCircle(); // Crear el círculo al inicio
        directionIndicator.SetActive(false); // Asegúrate de que esté desactivado al inicio
        Debug.Log("Escuchando en el puerto: " + listenPort);

        // Iniciar el contador antes de comenzar el juego
        StartCoroutine(StartCountdown(3f)); // Cambia 3f por el tiempo que quieras
    }

    IEnumerator StartCountdown(float countdownTime)
    {
        // Mutea la música al iniciar el contador
        musicSource.mute = true;

        int countdown = Mathf.FloorToInt(countdownTime);
        
        // Mostrar el contador en pantalla
        while (countdown > 0)
        {
            if (countdownText != null)
            {
                countdownText.text = countdown.ToString();
            }
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        // Esconder el texto del contador una vez que termine
        if (countdownText != null)
        {
            countdownText.text = "";
        }

        // Desmutea y reproduce la música después del retraso
        musicSource.mute = false;
        musicSource.Play();
    }
    
    void Update()
	{
	    ReceiveData();

	    // Verifica si el redDot está fuera de la vista
	    Vector3 screenPoint = Camera.main.WorldToViewportPoint(redDot.transform.position);
	    bool isVisible = screenPoint.x >= 0 && screenPoint.x <= 1 && screenPoint.y >= 0 && screenPoint.y <= 1;

	    if (!isVisible)
	    {
		// Mostrar el indicador de dirección
		directionIndicator.SetActive(true);

		// Calcular la dirección
		Vector3 direction = (redDot.transform.position - Camera.main.transform.position).normalized;

		// Calcular el ángulo y rotar el indicador
		float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
		directionIndicator.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -angle));

		// Colocar el indicador en la pantalla
		Vector3 screenPos = Camera.main.WorldToScreenPoint(redDot.transform.position);

		// Ajustar la posición para que no se salga de la vista
		float padding = 20f; // Espacio desde los bordes de la pantalla
		float screenWidth = Screen.width;
		float screenHeight = Screen.height;

		// Limitar la posición del indicador dentro de los bordes de la pantalla
		screenPos.x = Mathf.Clamp(screenPos.x, padding, screenWidth - padding);
		screenPos.y = Mathf.Clamp(screenPos.y, padding, screenHeight - padding);

		directionIndicator.transform.position = new Vector3(screenPos.x, screenPos.y, 0);
	    }
	    else
	    {
		// Ocultar el indicador si el redDot está visible
		directionIndicator.SetActive(false);
	    }

	    // Verifica si la música ha terminado
	    if (!musicSource.isPlaying)
	    {
		if (sceneFader != null)
		{
		    sceneFader.FadeToScene("ScoreScene");
		}
		else
		{
		    SceneManager.LoadScene("ScoreScene");
		}
	    }
	}

    
    
    /*
    void Update()
	{
	    ReceiveData();

	    // Verifica si el redDot está fuera de la vista
	    Vector3 screenPoint = Camera.main.WorldToViewportPoint(redDot.transform.position);
	    bool isVisible = screenPoint.x >= 0 && screenPoint.x <= 1 && screenPoint.y >= 0 && screenPoint.y <= 1;

	    if (!isVisible)
	    {
		// Mostrar el indicador de dirección
		directionIndicator.SetActive(true);

		// Calcular la dirección
		Vector3 direction = (redDot.transform.position - Camera.main.transform.position).normalized;

		// Calcular el ángulo y rotar el indicador
		float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
		directionIndicator.transform.rotation = Quaternion.Euler(new Vector3(0, -angle, 0));

		// Colocar el indicador en la pantalla
		Vector3 screenPos = Camera.main.WorldToScreenPoint(redDot.transform.position);
		directionIndicator.transform.position = new Vector3(screenPos.x, screenPos.y, 0);
	    }
	    else
	    {
		// Ocultar el indicador si el redDot está visible
		directionIndicator.SetActive(false);
	    }

	    // Verifica si la música ha terminado
	    if (!musicSource.isPlaying)
	    {
		if (sceneFader != null)
		{
		    sceneFader.FadeToScene("ScoreScene");
		}
		else
		{
		    SceneManager.LoadScene("ScoreScene");
		}
	    }
	}
	*/
    




	/*
    void Update()
    {
        // Simplemente recibir y procesar los datos
        ReceiveData();
        
        // Verifica si la música ha terminado
        if (!musicSource.isPlaying)
        {
            // Cargar la escena de puntaje y pasar el puntaje actual
            //SceneManager.LoadScene("ScoreScene");
            if (sceneFader != null)
            {
            	sceneFader.FadeToScene("ScoreScene");
            }
            else
            {
            	SceneManager.LoadScene("ScoreScene");
            }
        }
        
    }*/

    void ReceiveData()
    {
        if (udpClient.Available > 0)  // Solo recibe si hay datos disponibles
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, listenPort);
            byte[] data = udpClient.Receive(ref remoteEndPoint);
            string message = Encoding.UTF8.GetString(data).Trim();
            //Debug.Log("Posición recibida: " + message);

            if (message == "no_eyes_detected")
            {
                Debug.Log("No se detectaron ojos. Reproduciendo sonido de advertencia.");
                
                // Reproducir sonido de advertencia
                if (!warningSound.isPlaying)
                {
                    warningSound.Play();
                }

                return; // No seguir procesando más
            }

            // Si se reciben datos válidos, procesarlos
            string[] values = message.Split(',');

            if (values.Length == 2)
            {
                if (float.TryParse(values[0], out float irisX) && float.TryParse(values[1], out float irisY))
                {
                    // Detener el sonido si estaba sonando
                    if (warningSound.isPlaying)
                    {
                        warningSound.Stop();
                    }

                    // Ajustar las coordenadas según el tamaño de la escena
                    irisX = irisX * 100.0f;
                    irisY = irisY * 100.0f;

                    // Nueva posición del punto rojo
                    Vector3 targetPosition = new Vector3(irisX, 0, irisY);

                    // Mover el punto rojo según la posición del iris
                    float smoothSpeed = 5.0f; // Ajusta este valor para controlar la velocidad de suavizado
                    redDot.transform.position = Vector3.Lerp(redDot.transform.position, targetPosition, smoothSpeed * Time.deltaTime);
                }
            }
        }
    }

    void SpawnRandomCircle()
    {
        Vector3 newCirclePosition;
        do
        {
            // Generar una nueva posición aleatoria para el círculo
            float randomX = UnityEngine.Random.Range(-planeWidth, planeWidth);
            float randomY = UnityEngine.Random.Range(-planeHeight, planeHeight);
            newCirclePosition = new Vector3(randomX, 0, randomY);
        }
        // Repetir hasta que la nueva posición esté lo suficientemente lejos del punto rojo
        while (Vector3.Distance(newCirclePosition, redDot.transform.position) < circleRadius * 2);

        // Actualizar la posición del círculo
        circlePosition = newCirclePosition;
        circle.transform.position = circlePosition;
    }

    private void OnApplicationQuit()
    {
        udpClient?.Close();
    }
}

