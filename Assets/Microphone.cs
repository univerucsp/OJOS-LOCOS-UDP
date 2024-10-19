using UnityEngine;

public class MicrophoneInput : MonoBehaviour
{
    private AudioClip microphoneClip;
    private const int sampleSize = 256;

    void Start()
    {
        // Inicia la grabación desde el micrófono
        microphoneClip = Microphone.Start(null, true, 1, AudioSettings.outputSampleRate);
    }

    void Update()
    {
        // Calcula el nivel de ruido
        float[] samples = new float[sampleSize];
        AudioListener.GetSpectrumData(samples, 0, FFTWindow.BlackmanHarris);

        float sum = 0;
        for (int i = 0; i < samples.Length; i++)
        {
            sum += samples[i];
        }
        float noiseLevel = sum / sampleSize;

        // Usa el nivel de ruido como desees
        //Debug.Log("Nivel de Ruido: " + noiseLevel);
    }

    void OnApplicationQuit()
    {
        Microphone.End(null); // Detiene la grabación al salir
    }
}
