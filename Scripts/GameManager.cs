using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Sprite orangeIdleSprite;
    public Sprite orangeJumpPreparationSprite;
    public Sprite orangeJumpingSprite;
    
    public Sprite greenIdleSprite;
    public Sprite greenJumpPreparationSprite;
    public Sprite greenJumpingSprite;
    
    public TextMeshProUGUI scoreText;
    public bool isGreen = true;
    public int score = 0;
    public int carrotCnt = 0;

    public void Update()
    {
        scoreText.text = score.ToString("0") + "m";
    }
    
    public void UpdateScore()
    {
        if (score > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", score);
            SubmitToLeaderboard(score);
        }
    }

    private void SubmitToLeaderboard(int highScore)
    {
        Social.ReportScore(highScore, GPGSIds.leaderboard_top_rankings,(bool success)=>{});
    }
}