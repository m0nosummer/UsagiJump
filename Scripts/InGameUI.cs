using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private JumpController jumpController;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private GameManager gameManager;


    public void Awake()
    {
        if (PlayerPrefs.GetInt("Test6HasSeenTutorialPanel", 0) == 0)
        {
            jumpController.EnableInput(false);
            Time.timeScale = 0f;
            tutorialPanel.SetActive(true);
        }
    }

    public void LoadMainMenu()
    {
        jumpController.EnableInput(false);
        gameManager.UpdateScore();
        SceneManager.LoadScene(0);
    }
    public void LoadInGame()
    {
        gameManager.UpdateScore();
        SceneManager.LoadScene(1);
        Time.timeScale = 1f;
        jumpController.EnableInput(true);
        
    }
    public void PauseGame()
    {
        soundManager.PlayUISound();
        pausePanel.SetActive(true);
        Time.timeScale = 0f; // 게임의 시간을 멈춤
        jumpController.EnableInput(false);
    }
    public void ResumeGame()
    {
        soundManager.PlayUISound();
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        StartCoroutine(ResumeGameAfterDelay(0.1f));
    }

    private IEnumerator ResumeGameAfterDelay(float delay)
    {
        
        yield return new WaitForSecondsRealtime(delay); // 실제 시간 기준으로 대기
        Time.timeScale = 1f;
        jumpController.EnableInput(true);
    }
    public void ShowSetting()
    {
        soundManager.PlayUISound();
        settingPanel.SetActive(true);
    }
    public void ShowTutorial()
    {
        tutorialPanel.SetActive(true);
    }
    public void CloseSetting()
    {
        settingPanel.SetActive(false);
    }
    public void ClosePause()
    {
        pausePanel.SetActive(false);
    }

    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
        if (PlayerPrefs.GetInt("Test6HasSeenTutorialPanel") == 0)
        {
            jumpController.EnableInput(true);
            Time.timeScale = 1f;
            PlayerPrefs.SetInt("Test6HasSeenTutorialPanel", 1); // 패널을 본 것으로 표시
        }
    }
    public void ShowGameOver()
    {
        Time.timeScale = 0f; // 게임의 시간을 멈춤
        gameOverPanel.SetActive(true);
    }
    public void FinishWithScoreX2()
    {
        soundManager.PlayUISound();
        jumpController.EnableInput(false);
        gameManager.score *= 2;
        gameManager.UpdateScore();
        SceneManager.LoadScene(0);
    }
}