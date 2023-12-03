using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AdorationBarEvents
{
    ThrowLightning,
    DestroyBuilding,
    KillHuman,
    ConstructBuilding,
    PassivelyIncreasePerSeconds,
}
[Serializable]
public class AdorationBarEvent
{
    public AdorationBarEvents adorationBarEvent;
    public float value;
}
public class AdorationBar : MonoBehaviour
{
    public static AdorationBar instance = null;
    [SerializeField] private AdorationBarEvent[] adorationBarEvent;
    private float adorationValue = 0;
    private Slider slider;
    
    private void Awake()
    {
        if (instance == null) instance = this;
        else if(instance != this) Destroy(gameObject);
    }

    private void Start()
    {
        slider = GetComponent<Slider>();
        adorationValue = slider.maxValue / 2;
        slider.value = adorationValue;
        
        InvokeRepeating("PassiveIncrease", 1, 1f);
    }

    public void SetValue(int value)
    {
        slider = GetComponent<Slider>();
        adorationValue = value;
        slider.value = adorationValue;
    }

    private void PassiveIncrease()
    {
        ChangeAdorationBarValue(AdorationBarEvents.PassivelyIncreasePerSeconds);
    }
    
    public void ChangeAdorationBarValue(AdorationBarEvents adorationBarEventType)
    {
        foreach (AdorationBarEvent adorationEvent in this.adorationBarEvent)
        {
            if (adorationEvent.adorationBarEvent == adorationBarEventType)
            {
                adorationValue = Mathf.Clamp(adorationValue + adorationEvent.value, 0, 100);
                slider.value = adorationValue;
                SerialHandler.instance.SetLed(adorationValue > 50);
            }
        }
    }
}
