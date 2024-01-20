using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarRessources : MonoBehaviour
{
    [SerializeField] private float maxHealthOfRessource = 10;
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private Slider delayedSlider;
    [SerializeField] private GameObject sliderGameObject;

    private Camera mainCam;
    private float currentHealth;
    private float _damage;
    private float timerUntilHided = 0.5f;
    private float currentTimer;

    private bool canStopShowingHealthBar = true;
    
    void Start()
    {
        InitializeBar();

        GameManager.ShowHealthBarsChanged += OnShowHealthBarsChanged;
        
        ShowHealthBar(false);
        mainCam = Camera.main;
        LookTowardsCamera();
    }
    
    void OnShowHealthBarsChanged(bool show)
    {
        canStopShowingHealthBar = !show;
        ShowHealthBar(show);
    }

    private void OnDestroy()
    {
        GameManager.ShowHealthBarsChanged -= OnShowHealthBarsChanged;
    }

    public void ShowHealthBar(bool show)
    {
        sliderGameObject.SetActive(show);
    }

    private void InitializeBar()
    {
        healthBarSlider.maxValue = maxHealthOfRessource;
        delayedSlider.maxValue = maxHealthOfRessource;
        healthBarSlider.value = maxHealthOfRessource;
        delayedSlider.value = maxHealthOfRessource;
        
        currentHealth = maxHealthOfRessource;

    }
    

    public void SetMaxHealth(float maxHealth)
    {
        maxHealthOfRessource = maxHealth;

        InitializeBar();
    }

    public void DamageRessource(float damage)
    {
        _damage = damage;
        if (currentHealth - damage <= 0)
        {
            currentHealth = 0;
        }
        else
        {
            currentHealth -= damage;
        }
        
        ShowHealthBar(true);
        ChangeValueOfSlider();
    }
    
    private void ChangeValueOfSlider()
    {
        StartCoroutine(ChangeSlidersCoroutine(currentHealth));
    }

    private IEnumerator ChangeSlidersCoroutine(float valueToChange)
    {
        healthBarSlider.value = valueToChange;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(SlideCoroutine(valueToChange, delayedSlider));
    }
    

    private void LookTowardsCamera()
    {
        transform.LookAt(transform.position + mainCam.transform.rotation * Vector3.forward,
            mainCam.transform.rotation * Vector3.up);
    }
    
    

    private IEnumerator SlideCoroutine(float valueToReach, Slider slider)
    {
        float increment = (slider.value - valueToReach) * Time.deltaTime / 1.5f;
        while (slider.value > valueToReach)
        {
            slider.value -= increment;
            yield return new WaitForEndOfFrame();
        }
    }
    
    

    void Update()
    {
        LookTowardsCamera();
        if (delayedSlider.value <= currentHealth && sliderGameObject.activeSelf)
        {
            currentTimer -= Time.deltaTime;
            if (currentTimer <= 0 && canStopShowingHealthBar)
            {
                ShowHealthBar(false);
            }
        }
        else
        {
            currentTimer = timerUntilHided;
        }
    }
    
}
