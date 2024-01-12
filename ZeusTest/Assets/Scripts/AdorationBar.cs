using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AdorationBarEvents
{
    DestroyResource,
    KillWolf,
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
    public TownBehaviour town;
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
                town.AdorationBonuses(adorationValue);
                return;
                
            }
        }
    }
}
