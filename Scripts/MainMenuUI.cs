using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private TextMeshProUGUI textHighScore;
    
    private bool _isPaused = false;

    private void Start()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        textHighScore.text = highScore.ToString();
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void LoadInGame()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1f;
        _isPaused = false;
    }
    public void ShowSetting()
    {
        soundManager.PlayUISound();
        settingPanel.SetActive(true);
    }

    public void CloseSetting()
    {
        soundManager.PlayUISound();
        settingPanel.SetActive(false);
    }
    public void ShowMainPanel()
    {
        soundManager.PlayUISound();
        mainPanel.SetActive(true);
    }
    public void CloseMainPanel()
    {
        soundManager.PlayUISound();
        mainPanel.SetActive(false);
    }
    public void ShowLeaderBoard()
    {
        soundManager.PlayUISound();
    }
}