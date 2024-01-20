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
    
    private void Awake()
    {
        if (instance == null) instance = this;
        else if(instance != this) Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
        
        soundEffectSource = GetComponent<AudioSource>();
    }
    public void PlaySoundEffect(SoundEffects soundEffect, float volume = 0.5f)
    {
        foreach (AudioClip clip in soundEffectClips)
        {
            if (clip.name == soundEffect.ToString())
            {
                soundEffectSource.PlayOneShot(clip, volume);
                return;
            }
        }
    }
}
