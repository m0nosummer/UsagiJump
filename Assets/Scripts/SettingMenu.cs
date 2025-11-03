using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class SettingMenu : MonoBehaviour
{
    [SerializeField] private GameObject userPrivacyInfo;
    [SerializeField] private GameObject imageMusicOn;
    [SerializeField] private GameObject imageMusicOff;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private AudioMixer audioMixer;

    private void Awake()
    {
        soundToggle.isOn = PlayerPrefs.GetInt("SoundToggle", 1) == 1;
        ToggleSound();
    }

    public void ToggleSound()
    {
        soundManager.PlayUISound();
        if (audioMixer != null)
        {
            bool isOn = soundToggle.isOn;
            
            if (isOn)
            {
                imageMusicOn.SetActive(true);
                imageMusicOff.SetActive(false);
                audioMixer.SetFloat("MasterVolume", 0f);
                audioMixer.SetFloat("SFXVolume", 0f);
            }
            else
            {
                imageMusicOn.SetActive(false);
                imageMusicOff.SetActive(true);
                audioMixer.SetFloat("MasterVolume", -80f);
                audioMixer.SetFloat("SFXVolume", -80f);
            }
            PlayerPrefs.SetInt("SoundToggle", isOn ? 1 : 0);
        }
    }
    public void ShowPrivacyInfo()
    {
        string url = "https://sites.google.com/view/usagi-jump/%ED%99%88?authuser=4";
        Application.OpenURL(url);
    }
}