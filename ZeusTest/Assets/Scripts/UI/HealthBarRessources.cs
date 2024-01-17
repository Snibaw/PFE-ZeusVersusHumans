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
    private float timeBeforeDeactivate;
    
    void Start()
    {
        InitializeBar();
        
        sliderGameObject.SetActive(false);
        mainCam = Camera.main;
        LookTowardsCamera();
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
        if (currentHealth - damage <= 0)
        {
            currentHealth = 0;
        }
        else
        {
            currentHealth -= damage;
        }
        
        sliderGameObject.SetActive(true);
        ChangeValueOfSlider();
        timeBeforeDeactivate = 4f;
    }

    private IEnumerator ShowHealthBar(float time)
    {
        sliderGameObject.SetActive(true);
        yield return new WaitForSeconds(time);
        sliderGameObject.SetActive(false);
    }

    
    
    private void ChangeValueOfSlider()
    {
        StartCoroutine(ChangeSlidersCoroutine(currentHealth));
    }

    private IEnumerator ChangeSlidersCoroutine(float valueToChange)
    {
        healthBarSlider.value = valueToChange;
        yield return new WaitForSeconds(2f);
        StartCoroutine(SlideCoroutine(valueToChange, delayedSlider));
    }
    

    private void LookTowardsCamera()
    {
        var rotation = mainCam.transform.rotation;
        transform.LookAt(transform.position  + rotation * Vector3.forward);
    }
    
    

    private IEnumerator SlideCoroutine(float valueToReach, Slider slider)
    {
        if (slider.value > valueToReach)
        {
            slider.value -= 15 * Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
            StartCoroutine( SlideCoroutine(valueToReach, slider));
        }
    }
    
    

    void Update()
    {
        LookTowardsCamera();

        if (timeBeforeDeactivate > 0)
        {
            timeBeforeDeactivate -= Time.deltaTime;
        }
        else
        {
            sliderGameObject.SetActive(false);
        }
    }
}
