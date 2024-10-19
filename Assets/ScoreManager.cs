using UnityEngine;
using System.Collections;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // Referencia al texto donde mostrarás el puntaje

    private void Start()
    {
    	Colission.totalScore *= 10;
        // Establecer el puntaje a 5 al inicio
        SetFinalScore(Colission.totalScore);
    }

    // Este método se llamará para establecer el puntaje final
    public void SetFinalScore(int score)
    {
        UpdateScoreDisplay(score);
    }

    private void UpdateScoreDisplay(int score)
    {
        scoreText.text = score.ToString();
    }
}

