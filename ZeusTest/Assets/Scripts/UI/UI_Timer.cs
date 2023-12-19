using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Timer : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject canvasGameObject;
    
    private bool shouldFillSlider = false;
    
    void Start()
    {
        slider.value = 0;
    }

    public void SetMaxValue(int value)
    {
        slider.maxValue = value;
    }

    public void StartTimer(float maxValue)
    {
        slider.value = 0;
        slider.maxValue = maxValue;
        canvasGameObject.SetActive(true);
        shouldFillSlider = true;
    }

    void Update()
    {
        if (shouldFillSlider)
        {
            var newValue = slider.value + Time.deltaTime;
            if (newValue < slider.maxValue)
            {
                slider.value = newValue;
            }
            else
            {
                slider.value = slider.maxValue;
                shouldFillSlider = false;
                canvasGameObject.SetActive(false);
            }
        }
    }
}
