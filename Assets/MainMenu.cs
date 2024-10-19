using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class MainMenu : MonoBehaviour
{
    private SceneFader sceneFader; // Referencia al script SceneFader

    private string serverIP = "127.0.0.1"; // Dirección IP del servidor Python
    private int serverPort = 65432;        // Puerto de comunicación para enviar
    private int listenPort = 65434;        // Puerto para escuchar mensajes

    private UdpClient udpClient;
    private bool isRunning = true;

    private void Start()
    {
        // Busca el objeto con el tag "SceneFader"
        GameObject faderObject = GameObject.FindWithTag("Fade");
        if (faderObject != null)
        {
            sceneFader = faderObject.GetComponent<SceneFader>();
        }

        if (sceneFader == null)
        {
            Debug.LogWarning("SceneFader no está asignado. Cambiando de escena directamente.");
        }

        // Iniciar el cliente UDP para escuchar mensajes
        udpClient = new UdpClient(listenPort);
        StartListening();
    }

    private void OnDestroy()
    {
        isRunning = false;
        udpClient.Close();
    }

    public void PlayGame()
    {
        SendStartGame();
        if (sceneFader != null)
        {
            sceneFader.FadeToScene("SampleScene");
        }
        else
        {
            SceneManager.LoadScene("SampleScene");
        }
    }

    public void CalibrateGame()
    {
        SendCalibrationMessage();
        if (sceneFader != null)
        {
            sceneFader.FadeToScene("CalibrateGame");
        }
        else
        {
            Debug.LogWarning("SceneFader no está asignado. Cambiando de escena directamente.");
            SceneManager.LoadScene("CalibrateGame");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void BackButton()
    {
        if (sceneFader != null)
        {
            sceneFader.FadeToScene("MainMenu");
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void SendCalibrationMessage()
    {
        SendMessageToServer("activar-calibracion");
    }

    private void SendStartGame()
    {
        SendMessageToServer("iniciar-juego");
    }

    private void SendMessageToServer(string message)
    {
        try
        {
            UdpClient udpClient = new UdpClient();
            udpClient.Connect(serverIP, serverPort);

            byte[] data = Encoding.UTF8.GetBytes(message);
            udpClient.Send(data, data.Length);

            udpClient.Close();
        }
        catch (SocketException ex)
        {
            Debug.LogError("Error al enviar el mensaje: " + ex.Message);
        }
    }

    private async void StartListening()
    {
        while (isRunning)
        {
            try
            {
                UdpReceiveResult result = await udpClient.ReceiveAsync();
                string receivedMessage = Encoding.UTF8.GetString(result.Buffer);
                HandleReceivedMessage(receivedMessage);
            }
            catch (SocketException ex)
            {
                Debug.LogError("Error al recibir el mensaje: " + ex.Message);
            }
        }
    }

    private void HandleReceivedMessage(string message)
    {
        Debug.Log($"Mensaje recibido: {message}");

        switch (message)
        {
            case "iniciar-juego":
                PlayGame();
                break;
            case "calibrar":
                CalibrateGame();
                break;
            case "salir":
                QuitGame();
                break;
            case "volver":
                BackButton();
                break;
            default:
                Debug.Log("Comando no reconocido.");
                break;
        }
    }
}

