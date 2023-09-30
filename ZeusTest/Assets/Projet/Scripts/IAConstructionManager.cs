using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IAConstructionManager : MonoBehaviour
{
    [SerializeField] private IAConstruction[] constructions;
    public IAConstruction currentConstructionObjective;
    [SerializeField] private TMP_Text constructionText;
    public static IAConstructionManager instance;

    private void Awake() {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
    }
    
    public IAConstruction GetAConstructionObjective()
    {
        if(currentConstructionObjective == null) // TODO : Check if the number of construction of the current objective is not already reached
        {
            ChooseAConstructionObjective();
        }
        return currentConstructionObjective;
    }
    public TypeOfResources GetAResourceObjective()
    {
        int maxResourcesNeeded = -100000;
        TypeOfResources resourceMostNeeded = TypeOfResources.wood;
        //Browse threw every resources possibles
        foreach(TypeOfResources resource in System.Enum.GetValues(typeof(TypeOfResources)))
        {
            //If we find a resource more needed than the previous one, we change the resourceMostNeeded
            int resourceNeeded = currentConstructionObjective.GetResourceNeeded(resource) - IAResourceManager.instance.GetResourceNumber(resource);
            if(resourceNeeded > maxResourcesNeeded)
            {
                maxResourcesNeeded = resourceNeeded;
                resourceMostNeeded = resource;
            }
        }
        Debug.Log("Resource most needed:" + resourceMostNeeded);
        return resourceMostNeeded;
    }

    private void ChooseAConstructionObjective() // TODO : Choose a construction depending on the priority, the resources, the type of human (religious or army...
    {
        currentConstructionObjective = constructions[0];
        #if UNITY_EDITOR
        UpdateConstructionText();
        #endif
    }
    private void UpdateConstructionText()
    {
        constructionText.text = "Construction: " + currentConstructionObjective.name + "\n" + "woodCost:" + currentConstructionObjective.woodCost + "\n" + "stoneCost:" + currentConstructionObjective.stoneCost;
    }
}
