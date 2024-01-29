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
    
    [Header("Shake")]
    private float shakePower = 0;
    [SerializeField] private float shakeTime = 0.5f;
    [SerializeField] private GameObject shakeText;
    private bool _waitingForShake = false;
    private bool _shakeForMusic = false;

    private void Start()
    {
        pauseMenu.SetActive(false);
        
        soundImage.sprite = soundSprites[PlayerPrefs.GetInt("Sound")];
        SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume"));
        SetSFVolume(PlayerPrefs.GetFloat("SFVolume"));
    }

    private void Update()
    {
        if(_waitingForShake && Input.acceleration.magnitude > 1.5f)
        {
            _waitingForShake = false;
            shakeText.SetActive(false);
            StartCoroutine(RecordShake(_shakeForMusic));
        }
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
        
        _waitingForShake = false;
        shakeText.SetActive(false);
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
        
        _waitingForShake = false;
        shakeText.SetActive(false);
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

    public void ClickOnVolume(bool isMusic)
    {
        _shakeForMusic = isMusic;
        _waitingForShake = true;
        shakeText.SetActive(true);
    }
    private IEnumerator RecordShake(bool isMusic)
    {
        shakePower = 0;
        float elapsedTime = 0;
        while (elapsedTime < shakeTime)
        {
            shakePower += Input.acceleration.magnitude;
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        shakePower = Mathf.Clamp01((shakePower-30) / 100);
        Debug.Log("shakePower = " + shakePower);
        if(isMusic) SetMusicVolume(shakePower);
        else SetSFVolume(shakePower);
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
