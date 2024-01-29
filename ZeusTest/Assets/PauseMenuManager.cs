using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsParent;
    [SerializeField] private GameObject pauseParent;
    
    [Header("Settings")]
    [SerializeField] private Sprite[] soundSprites;
    [SerializeField] private Image soundImage;
    [SerializeField] private TMP_Text musicVolumeText;
    [SerializeField] private TMP_Text sfVolumeText;
    

    private void Start()
    {
        pauseMenu.SetActive(false);
        
        soundImage.sprite = soundSprites[PlayerPrefs.GetInt("Sound")];
        SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume"));
        SetSFVolume(PlayerPrefs.GetFloat("SFVolume"));
    }

    public void OpenPauseMenu()
    {
        if(pauseMenu.activeSelf) return;
        
        settingsParent.SetActive(false);
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }
    public void ClosePauseMenu()
    {
        if(!pauseMenu.activeSelf) return;
        
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
    public void OpenSettingsMenu()
    {
        if(settingsParent.activeSelf) return;
        settingsParent.SetActive(true);
        pauseParent.SetActive(false);
    }
    public void CloseSettingsMenu()
    {
        if(!settingsParent.activeSelf) return;
        settingsParent.SetActive(false);
        pauseParent.SetActive(true);
    }

    public void SoundOnOff()
    {
        int newValue = 1 - PlayerPrefs.GetInt("Sound");
        PlayerPrefs.SetInt("Sound", newValue);
        soundImage.sprite = soundSprites[newValue];
        AudioManager.instance.PlayPauseMusic();
    }

    public void ClickOnMusicVolume()
    {
        SetMusicVolume(Random.Range(0, 1f));
    }
    public void ClickOnSFVolume()
    {
        SetSFVolume(Random.Range(0, 1f));
    }
    private void SetSFVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFVolume", volume);
        AudioManager.instance.SetSFVolume(volume);
        sfVolumeText.text = Mathf.RoundToInt(volume * 100).ToString();
    }
    private void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        AudioManager.instance.SetMusicVolume(volume);
        musicVolumeText.text = Mathf.RoundToInt(volume * 100).ToString();
    }
}
