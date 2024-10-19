using System.Collections.Generic;
using UnityEngine;

public class Colission : MonoBehaviour
{
    public static int totalScore;

    public AudioSource audioSource;
    public AudioClip collisionSound;

    public float normalTimeLimit = 1.5f; // Tiempo límite normal
    public float reducedTimeLimit = 0.5f; // Tiempo límite reducido (rápido)
    public float slowTimeLimit = 2.0f; // Tiempo límite más largo (despacio)
    private float currentTimeLimit; // Almacenará el tiempo límite actual
    private float timer = 0f;
    private float activationTimer = 0f; // Temporizador para la activación del objetivo
    private bool isActive = false;

    private int score = 0;
    private GameObject currentTarget;
    private Light targetLight;
    public float maxLightIntensity = 50f;

    public float areaWidth = 10f;  // Rango de aparición en el eje X
    public float areaDepth = 10f;  // Rango de aparición en el eje Z

    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>(); // Para almacenar posiciones ocupadas

    private Light directionalLight; // Para la luz direccional
    private float originalDirectionalIntensity; // Para guardar la intensidad original de la luz direccional
    public float reducedDirectionalIntensity = 0.5f; // Intensidad de la luz direccional cuando está en modo rápido
    public float lightChangeSpeed = 2f; // Velocidad de cambio de la luz direccional

    void Start()
    {
        // Inicializar el objetivo
        currentTarget = GameObject.FindGameObjectWithTag("Objetivo");
        currentTarget.SetActive(false); // Asegúrate de que el objetivo esté inicialmente desactivado
        SetupLight();
        currentTimeLimit = normalTimeLimit; // Establecer el tiempo límite inicial

        // Obtener la luz direccional
        directionalLight = FindObjectOfType<Light>();
        if (directionalLight != null && directionalLight.type == LightType.Directional)
        {
            originalDirectionalIntensity = directionalLight.intensity; // Guardar la intensidad original
        }
    }

    void Update()
    {
        activationTimer += Time.deltaTime; // Actualiza el temporizador de activación

        // Cambiar el tiempo límite según los intervalos de tiempo
        if (activationTimer >= 25f && activationTimer < 41f) // Despacio
        {
            currentTimeLimit = slowTimeLimit;
            ChangeLightColor(Color.green); // Cambia la luz a verde
            SetDirectionalLightIntensitySmooth(originalDirectionalIntensity); // Vuelve a la intensidad original suavemente
        }
        else if (activationTimer >= 41f && activationTimer < 70f) // Rápido
        {
            currentTimeLimit = reducedTimeLimit;
            ChangeLightColor(Color.yellow); // Cambia la luz a amarillo
            SetDirectionalLightIntensitySmooth(reducedDirectionalIntensity); // Oscurece la luz direccional suavemente
        }
        else if (activationTimer >= 70f && activationTimer < 86f) // Normal
        {
            currentTimeLimit = normalTimeLimit;
            ChangeLightColor(Color.blue); // Cambia la luz a azul
            SetDirectionalLightIntensitySmooth(originalDirectionalIntensity); // Vuelve a la intensidad original suavemente
        }
        else if (activationTimer >= 86f && activationTimer < 102f) // Despacio
        {
            currentTimeLimit = slowTimeLimit;
            ChangeLightColor(Color.green); // Cambia la luz a verde
            SetDirectionalLightIntensitySmooth(originalDirectionalIntensity); // Vuelve a la intensidad original suavemente
        }
        else if (activationTimer >= 102f && activationTimer < 118f) // Rápido
        {
            currentTimeLimit = reducedTimeLimit;
            ChangeLightColor(Color.yellow); // Cambia la luz a amarillo
            SetDirectionalLightIntensitySmooth(reducedDirectionalIntensity); // Oscurece la luz direccional suavemente
        }
        else
        {
            currentTimeLimit = normalTimeLimit; // Vuelve al tiempo límite normal fuera de los intervalos especificados
            ChangeLightColor(Color.blue); // Cambia la luz a azul
            SetDirectionalLightIntensitySmooth(originalDirectionalIntensity); // Vuelve a la intensidad original suavemente
        }

        if (activationTimer >= 11f) // Comienza a activar el objetivo después de 7 segundos
        {
            if (!isActive)
            {
                ActivateTarget();
            }

            if (isActive)
            {
                timer += Time.deltaTime; // Solo actualiza el temporizador si el objetivo está activo

                if (targetLight != null)
                {
                    float intensity = Mathf.Lerp(maxLightIntensity, 0f, timer / currentTimeLimit);
                    targetLight.intensity = intensity;
                }

                if (timer >= currentTimeLimit)
                {
                    isActive = false;
                    MoveToRandomPosition(); // Cambia a una nueva posición al finalizar el tiempo
                    DeactivateTarget();
                }
            }
        }
    }

    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Objetivo") && isActive)
        {
            score++;
            totalScore = score;
            Debug.Log("Puntos: " + score); // Cambiado a Debug.Log para consistencia

            if (audioSource != null && collisionSound != null)
            {
                audioSource.PlayOneShot(collisionSound);
            }

            isActive = false;
            MoveToRandomPosition();
            DeactivateTarget();
        }
    }

    void ActivateTarget()
    {
        if (currentTarget != null)
        {
            currentTarget.SetActive(true);
            isActive = true;
            timer = 0f;

            if (targetLight != null)
            {
                targetLight.intensity = maxLightIntensity;
            }

            Debug.Log("Objetivo activado en la posición: " + currentTarget.transform.position);
        }
        else
        {
            Debug.LogWarning("No se pudo activar el objetivo porque no se encontró.");
        }
    }

    void DeactivateTarget()
    {
        if (currentTarget != null)
        {
            currentTarget.SetActive(false);
            Debug.Log("Objetivo desactivado.");
            Invoke("ActivateTarget", 1f); // Reaparece después de 1 segundo
        }
    }

    void MoveToRandomPosition()
    {
        if (currentTarget != null)
        {
            Vector3 newPosition;
            int attempts = 0;
            do
            {
                // Generar una posición aleatoria dentro del rango de -10 a 10 para X y Z
                float randomX = Random.Range(-areaWidth, areaWidth); // -10 a 10
                float randomZ = Random.Range(-areaDepth, areaDepth + 10); // -10 a 10
                newPosition = new Vector3(randomX, 2f, randomZ);
                attempts++;
            } while (occupiedPositions.Contains(newPosition) && attempts < 100); // Intenta hasta 100 veces

            // Si no hay posiciones ocupadas en 100 intentos, se utiliza la nueva posición
            occupiedPositions.Add(newPosition);
            currentTarget.transform.position = newPosition;
            Debug.Log("Objetivo teletransportado a la nueva posición: " + currentTarget.transform.position);
        }
    }

    private void SetupLight()
    {
        if (targetLight == null) // Si la luz no está configurada, la buscamos
        {
            GameObject lightObject = GameObject.FindGameObjectWithTag("Light");
            if (lightObject != null)
            {
                targetLight = lightObject.GetComponent<Light>();
                lightObject.transform.SetParent(currentTarget.transform);
                lightObject.transform.localPosition = new Vector3(0f, 1f, 0f);
                targetLight.intensity = 1000f;
            }
            else // Si no hay luz, creamos una nueva
            {
                lightObject = new GameObject("TargetLight");
                lightObject.transform.SetParent(currentTarget.transform);
                lightObject.transform.localPosition = Vector3.zero;

                targetLight = lightObject.AddComponent<Light>();
                targetLight.type = LightType.Point;
                targetLight.range = 10f;
                targetLight.intensity = 100f;
                targetLight.color = Color.yellow; // Color inicial
            }
        }
    }

    // Método para cambiar el color de la luz según el tiempo límite
    private void ChangeLightColor(Color newColor)
    {
        if (targetLight != null)
        {
            targetLight.color = newColor;
        }
    }

    // Método para establecer la intensidad de la luz direccional suavemente
    private void SetDirectionalLightIntensitySmooth(float targetIntensity)
    {
        if (directionalLight != null)
        {
            directionalLight.intensity = Mathf.Lerp(directionalLight.intensity, targetIntensity, lightChangeSpeed * Time.deltaTime);
        }
    }
}

