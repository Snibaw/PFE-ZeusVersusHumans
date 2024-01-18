using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcesSlider : MonoBehaviour
{
    [SerializeField] private GameObject layout;
    [SerializeField] private Slider woodSlider;
    [SerializeField] private Slider stoneSlider;
    [SerializeField] private Slider ironSlider;

    private List<int> maxResourceValue = new List<int>() { 0, 0, 0 };

    [SerializeField] private int maxValue = 100;
    private Storage storageScript;

    void Start()
    {
        //Set up max value to 100
        woodSlider.maxValue = maxValue;
        stoneSlider.maxValue = maxValue;
        ironSlider.maxValue = maxValue;
    }

    public void SetVisible(bool isVisible, Storage _storage = null)
    {
        layout.SetActive(isVisible);
        if (isVisible && _storage != null)
        {
            storageScript = _storage;
            UpdateSlidersValue();
        }
    }

    public void UpdateSlidersValue()
    {
        //Set slider with their values
        
        woodSlider.value = storageScript.GetNbResources(ResourceType.wood);
        stoneSlider.value = storageScript.GetNbResources(ResourceType.stone);
        ironSlider.value = storageScript.GetNbResources(ResourceType.metal);

        UpdateMaxResourceValue();
        
        woodSlider.maxValue = maxResourceValue[(int)ResourceType.wood] > maxValue ? maxResourceValue[(int)ResourceType.wood] : maxValue;
        stoneSlider.maxValue = maxResourceValue[(int)ResourceType.stone] > maxValue ? maxResourceValue[(int)ResourceType.stone] : maxValue;
        ironSlider.maxValue = maxResourceValue[(int)ResourceType.metal] > maxValue ? maxResourceValue[(int)ResourceType.metal] : maxValue;
        
        // foreach (ResourceType r in ResourceType.GetValues(typeof(ResourceType)))
        // {
        //     Debug.Log("Resource " + storageScript.GetNbResources(r));
        //     Debug.Log("Resource Max" + maxResourceValue[(int)r]);
        // }
    }

    private void UpdateMaxResourceValue()
    {
        foreach (GameObject building in GameObject.FindGameObjectsWithTag("Building"))
        {
            if (building.GetComponent<Building>().BuildingType == BuildingType.village)
            {
                Storage currentStorage = building.GetComponent<Storage>();

                foreach (ResourceType r in ResourceType.GetValues(typeof(ResourceType)))
                {
                    UpdateMaxForOneResourceValue(currentStorage, r);
                }
            }
        }
    }

    private void UpdateMaxForOneResourceValue(Storage storageScript, ResourceType r)
    {
        int amountInStorage = storageScript.GetNbResources(r);
        if(amountInStorage > maxResourceValue[(int)r])
        {
            maxResourceValue[(int)r] = amountInStorage;
        }
    }
}
