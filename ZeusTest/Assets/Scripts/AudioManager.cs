using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SoundEffects
{
    LightningImpact,
    LightningThrow,
    Earthquake
}
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;
    private AudioSource soundEffectSource;
    [SerializeField] private AudioClip[] soundEffectClips;
    [SerializeField] private AudioSource musicSource;
    
    private void Awake()
    {
        if (instance == null) instance = this;
        else if(instance != this) Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
        
        soundEffectSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        PlayPauseMusic();
    }

    public void PlaySoundEffect(SoundEffects soundEffect, float volume = 0.5f)
    {
        if(PlayerPrefs.GetInt("Sound") == 0) return;
        foreach (AudioClip clip in soundEffectClips)
        {
            if (clip.name == soundEffect.ToString())
            {
                soundEffectSource.PlayOneShot(clip, volume);
                return;
            }
        }
    }
    public void SetSFVolume(float volume)
    {
        soundEffectSource.volume = volume;
    }
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }
    public void PlayPauseMusic()
    {
        if (PlayerPrefs.GetInt("Sound") == 0)
            musicSource.Pause();
        else musicSource.Play();
    }
}
