using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdorationBarManager : MonoBehaviour
{
    private TownBehaviour _townBehaviour;
    public float adorationValue = 50;

    private void Start()
    {
        _townBehaviour = transform.parent.GetComponent<TownBehaviour>();
        InvokeRepeating("PassiveIncrease", 1, 1f);
    }

    public void ChangeAdorationBarValue(AdorationBarEvents adorationBarEventType)
    {
        foreach (AdorationBarEvent adorationEvent in AdorationBar.instance.adorationBarEvent)
        {
            if (adorationEvent.adorationBarEvent == adorationBarEventType)
            {
                adorationValue = Mathf.Clamp(adorationValue + adorationEvent.value, 0, 100);
                _townBehaviour.AdorationBonuses(adorationValue);
                return;
                
            }
        }
    }
    private void PassiveIncrease()
    {
        ChangeAdorationBarValue(AdorationBarEvents.PassivelyIncreasePerSeconds);
    }
}
