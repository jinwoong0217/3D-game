using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    public int totalScore = 5;
    private int enemyCount = 0;

    public TextMeshProUGUI scoreText;
    public Canvas EndCanvas;

    public void KillEnemy()
    {
        enemyCount++;
        UpdateScoreText();

        if(enemyCount >= totalScore)
        {
            Time.timeScale = 0.0f;
            EndCanvas.gameObject.SetActive(true);
        }
    }

    private void UpdateScoreText()
    {
        scoreText.text = enemyCount + "/" + totalScore;
    }
}
