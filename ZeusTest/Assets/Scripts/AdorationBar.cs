using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

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
    private GameObject adorationBarUI;
    public static AdorationBar instance = null;
    public AdorationBarEvent[] adorationBarEvent;
    private float _adorationValue = 0;
    private Slider slider;
    private AdorationBarManager _adorationBarManager;
    
    private void Awake()
    {
        if (instance == null) instance = this;
        else if(instance != this) Destroy(gameObject);
    }

    private void Start()
    {
        slider = GetComponentInChildren<Slider>();
        adorationBarUI = transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        if(adorationBarUI.activeSelf) UpdateSliderValue();
    }


    public void SetVisible(bool visible, AdorationBarManager adorationBarManager = null)
    {
        adorationBarUI.SetActive(visible);
        if (visible)
        {
            _adorationBarManager = adorationBarManager;
            UpdateSliderValue();
        }
    }
    private void UpdateSliderValue()
    {
        if (_adorationBarManager == null) return;
        _adorationValue = _adorationBarManager.adorationValue;
        slider.value = _adorationValue;
    }

    public void FindAndChangeAdorationBarNPC(NPCController _npcController, AdorationBarEvents adorationBarEventType)
    {
        AdorationBarManager adorationBarManager = _npcController.homeTown.GetComponentInChildren<AdorationBarManager>();
        if (adorationBarManager == null) return;
        adorationBarManager.ChangeAdorationBarValue(adorationBarEventType);
    }

    public void FindAndChangeNearestAdorationBar(Vector3 objPosition, AdorationBarEvents adorationBarEventType)
    {
        //Find nearest Town
        GameObject nearestTown = null;
        float nearestDistance = Mathf.Infinity;
        foreach (GameObject building in GameObject.FindGameObjectsWithTag("Building"))
        {
            if (building.GetComponent<Building>().BuildingType == BuildingType.village)
            {
                float _distance = Vector3.Distance(objPosition, building.transform.position);
                if (nearestTown == null)
                {
                    nearestTown = building;
                    nearestDistance = _distance;
                }
                else
                {
                    if(_distance < nearestDistance)
                    {
                        nearestTown = building;
                        nearestDistance = _distance;
                    }
                }
            }
        }
        if (nearestTown == null) return;
        AdorationBarManager adorationBarManager = nearestTown.GetComponentInChildren<AdorationBarManager>();
        if (adorationBarManager == null) return;
        adorationBarManager.ChangeAdorationBarValue(adorationBarEventType);
    }
}
