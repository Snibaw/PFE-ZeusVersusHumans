using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcesSlider : MonoBehaviour
{
    [SerializeField] private Slider woodSlider;
    [SerializeField] private Slider stoneSlider;
    [SerializeField] private Slider ironSlider;

    [SerializeField] private int maxValue = 100;
    private Storage storageScript;

    void Start()
    {
        //Set up max value to 100
        woodSlider.maxValue = maxValue;
        stoneSlider.maxValue = maxValue;
        ironSlider.maxValue = maxValue;
    }

    public void SetBuilding(GameObject building)
    {
        storageScript = building.GetComponent<Storage>();
    }

    public void SetValueOfSliders()
    {
        //Set slider with their values
        woodSlider.value = storageScript.GetNbResources(ResourceType.wood);
        stoneSlider.value = storageScript.GetNbResources(ResourceType.stone);
        ironSlider.value = storageScript.GetNbResources(ResourceType.metal);

    }

    void Update()
    {
        //TODO : if value(resources) d'un village < 100; on set up normal
        //TODO : else ; on met la maxValue du slider à value(resources) et on setup le reste avec cette value en max
    }
}
