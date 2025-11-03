using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip eatSound;
    [SerializeField] private AudioClip uiSound;
    [SerializeField] private AudioMixerGroup soundEffectsMixerGroup;
    
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.outputAudioMixerGroup = soundEffectsMixerGroup;
    }

    public void PlayJumpSound()
    {
        _audioSource.PlayOneShot(jumpSound);
    }
    public void PlayEatSound()
    {
        _audioSource.PlayOneShot(eatSound);
    }
    public void PlayUISound()
    {
        _audioSource.PlayOneShot(uiSound);
    }
    
}