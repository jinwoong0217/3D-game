using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public Canvas EndCanvas;
    
    [Header("전체 스코어")]
    public int totalScore = 5;
    int enemyCount = 0;

    /// <summary>
    /// 적을 죽이면 스코어가 올라가는 함수
    /// </summary>
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
