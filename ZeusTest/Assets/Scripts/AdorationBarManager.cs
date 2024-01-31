using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AdorationBarManager : MonoBehaviour
{
    private TownBehaviour _townBehaviour;
    public float adorationValue = 50;
    public float adorationModifier;

    private void Start()
    {
        adorationModifier = Random.Range(0.5f, 1f) * (Random.Range(0,2) == 0 ? -1 : 1);
        adorationValue = 15 * adorationModifier + 50;
        _townBehaviour = GetComponent<TownBehaviour>();
        InvokeRepeating("PassiveIncrease", 1, 1f);
    }

    public void ChangeAdorationBarValue(AdorationBarEvents adorationBarEventType)
    {
        foreach (AdorationBarEvent adorationEvent in AdorationBar.instance.adorationBarEvent)
        {
            if (adorationEvent.adorationBarEvent == adorationBarEventType)
            {
                float addValue = adorationEvent.value;
                if (adorationBarEventType == AdorationBarEvents.PassivelyIncreasePerSeconds)
                {
                    addValue *= adorationModifier;
                }
                adorationValue = Mathf.Clamp(adorationValue + addValue, 0, 100);
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
