using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EZCameraShake;

public class EarthButton : MonoBehaviour
{
    private TMP_Text _timerText;
    private float _time;
    private Button _earthButton;
    [SerializeField] private GameObject shakePhoneUI;
    [SerializeField] private float timeToShake = 0.5f;
    [SerializeField] private float thresholdShake = 25f;
    [SerializeField] private float cooldown = 120f;
    //CameraShake
    private float damageDiviser = 5, roughness = 6, fadeInTime = 0.1f, fadeOutTime= 1f;
    private bool _recordShake = false;
    private bool _waitForThreshold = false;
    private float _shakeMagnitude = 0f;
    // Start is called before the first frame update

    private void Awake()
    {
        _earthButton = GetComponentInParent<Button>();
        _earthButton.enabled = true;
        shakePhoneUI.SetActive(false);
        _timerText = GetComponentInChildren<TMP_Text>();
        _timerText.text = "";
    }

    private void Update()
    {
        if (_waitForThreshold)
        {
            if (Input.acceleration.magnitude > thresholdShake)
            {
                _waitForThreshold = false;
                _recordShake = true;
                _shakeMagnitude = Input.acceleration.magnitude;
                shakePhoneUI.SetActive(false);
                StartCoroutine(WaitBeforeStopRecordingShake());
            } 
        }
        else if (_recordShake)
        {
            _shakeMagnitude += Input.acceleration.magnitude;
        }
    }
    // Update is called once per frame
    public void ClickOnEarthButton()
    {
        _earthButton.enabled = false;
        _waitForThreshold = true;
        shakePhoneUI.SetActive(true);
    }
    IEnumerator WaitBeforeStopRecordingShake()
    {
        yield return new WaitForSeconds(timeToShake);

        float damageValue = Mathf.Clamp(_shakeMagnitude - 30, 10, 80);
        
        ObjectToDestroy[] objectsToDestroy = FindObjectsOfType<ObjectToDestroy>();
        foreach (ObjectToDestroy objectToDestroy in objectsToDestroy)
        {
            objectToDestroy.TakeDamage(damageValue, false);
        }
        AudioManager.instance.PlaySoundEffect(SoundEffects.Earthquake, damageValue/50);
        CameraShaker.Instance.ShakeOnce(damageValue/damageDiviser, roughness, fadeInTime, fadeOutTime);
        
        _recordShake = false;
        StartTimer(cooldown);
        
    }
    private void StartTimer(float startTime)
    {
        _time = startTime;
        StartCoroutine(UpdateTimer());
    }
    private IEnumerator UpdateTimer()
    {
        while (_time > 0)
        {
            //Show time in minutes and seconds in the format 00:00
            _time--;
            _timerText.text = string.Format("{0:00}:{1:00}", Mathf.Floor(_time / 60), _time % 60);
            yield return new WaitForSeconds(1);
        }
        _timerText.text = "";
        _earthButton.enabled = true;
    }
}
