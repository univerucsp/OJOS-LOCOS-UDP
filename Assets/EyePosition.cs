using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class EyeTrackingClient : MonoBehaviour
{
    UdpClient udpClient;
    public int listenPort = 65432;

    public GameObject redDot; // Prefab del punto rojo
    public GameObject circle; // Prefab del círculo
    public Transform plane; // Referencia al plano en la escena
    public float circleRadius = 0.5f; // Radio del círculo
    public float planeWidth = 1f; // Ancho del plano
    public float planeHeight = 1f; // Alto del plano

    private Vector3 circlePosition; // Posición actual del círculo

    void Start()
    {
        udpClient = new UdpClient(listenPort);
        SpawnRandomCircle(); // Crear el círculo al inicio
        Debug.Log("Escuchando en el puerto: " + listenPort);
    }

    void Update()
    {
        ReceiveData();
    }

    void ReceiveData()
    {
        if (udpClient.Available > 0)
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, listenPort);
            byte[] data = udpClient.Receive(ref remoteEndPoint);
            string message = Encoding.UTF8.GetString(data);
            string[] values = message.Split(',');

            if (values.Length == 2)
            {
                if (float.TryParse(values[0], out float irisX) && float.TryParse(values[1], out float irisY))
                {
                    // Mover el punto rojo según la posición del iris
                    Vector3 redDotPosition = new Vector3(irisX, 0, irisY);
                    redDot.transform.position = redDotPosition;

                    // Mostrar la posición del eye tracking en la consola
                    Debug.Log($"Posición del iris - X: {irisX}, Y: {irisY}");

                    // Comprobar si el punto rojo está dentro del círculo
                    CheckCollisionWithCircle(redDotPosition);
                }
            }
        }
    }

    void SpawnRandomCircle()
    {
        // Generar una nueva posición aleatoria para el círculo frente al plano
        float randomX = UnityEngine.Random.Range(-3,-planeWidth / 2);//, planeWidth / 2);
        float randomY = UnityEngine.Random.Range(-3,-planeHeight / 2);//, planeHeight / 2);
        circlePosition = new Vector3(randomX, 0, randomY); // Asegúrate de que la Y sea 0 para que esté en el plano

        // Mover el círculo a la nueva posición
        circle.transform.position = circlePosition;
    }

    void CheckCollisionWithCircle(Vector3 redDotPosition)
    {
        // Comprobar si el punto rojo está dentro del círculo
        float distance = Vector3.Distance(redDotPosition, circlePosition);
        if (distance < circleRadius)
        {
            // Si está dentro, mover el círculo a otra posición aleatoria
            SpawnRandomCircle();
            Debug.Log("El punto rojo está dentro del círculo. Generando nuevo círculo.");
        }
    }

    private void OnApplicationQuit()
    {
        udpClient?.Close();
    }
}
