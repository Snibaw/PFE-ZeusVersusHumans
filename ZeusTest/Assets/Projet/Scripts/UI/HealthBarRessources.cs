using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarRessources : MonoBehaviour
{
    [SerializeField] private float maxHealthOfRessource = 10;
    [SerializeField] private Slider healthBarSlider;

    private Camera mainCam;
    private float currentHealth;
    
    void Start()
    {
        healthBarSlider.maxValue = maxHealthOfRessource;
        currentHealth = maxHealthOfRessource;
        ChangeValueOfSlider();
        
        gameObject.SetActive(false);
        mainCam = Camera.main;
        LookTowardsCamera();
    }

    public void ActivateHealthBar()
    {
        gameObject.SetActive(true);
    }

    public void SetMaxHealth(float maxHealth)
    {
        maxHealthOfRessource = maxHealth;
        
        healthBarSlider.maxValue = maxHealthOfRessource;
        currentHealth = maxHealthOfRessource;
        ChangeValueOfSlider();
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
        ChangeValueOfSlider();
    }

    private void ChangeValueOfSlider()
    {
        healthBarSlider.value = currentHealth;
    }
    

    private void LookTowardsCamera()
    {
        var rotation = mainCam.transform.rotation;
        transform.LookAt(transform.position + rotation * Vector3.back);
    }

    void Update()
    {
        LookTowardsCamera();
    }
}
