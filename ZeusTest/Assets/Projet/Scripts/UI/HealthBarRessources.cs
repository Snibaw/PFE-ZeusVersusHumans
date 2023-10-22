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
        
        healthBarSlider.gameObject.SetActive(false);
        mainCam = Camera.main;
        LookTowardsCamera();
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
        StartCoroutine(ShowHealthBar(2f));
    }

    private IEnumerator ShowHealthBar(float time)
    {
        healthBarSlider.gameObject.SetActive(true);
        yield return new WaitForSeconds(time);
        healthBarSlider.gameObject.SetActive(false);
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
