using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using System.Linq;

public class BuildManager : MonoBehaviour
{
    public IAConstruction[] possibleConstructions;
    private StorageInventory storageInventory;
    private Dictionary<BuildingType, int> numberOfConstructionBuilt = new Dictionary<BuildingType, int>();
    

    private void Start()
    {
        storageInventory = GetComponent<StorageInventory>();
        
        InitNumberOfConstructionBuilt();
        AddConstructionBuilt(BuildingType.village);
    }

    private void InitNumberOfConstructionBuilt()
    {
        foreach (BuildingType buildingType in Enum.GetValues(typeof(BuildingType)))
        {
            numberOfConstructionBuilt.Add(buildingType, 0);
        }
    }
    public void AddConstructionBuilt(BuildingType buildingType)
    {
        numberOfConstructionBuilt[buildingType] += 1;
    }

    public int HowManyConstructionCanBeBuilt(NPCController _npcController, StorageInventory inventory = null)
    {
        if(inventory == null) inventory = storageInventory;

        int returnValue = 0;
        foreach (IAConstruction construction in possibleConstructions)
        {
            if (!CanBuildMore(construction)) continue;
            
            if (FindBuildPercentage(construction, inventory) == 1)
            {
                returnValue += 1;
                _npcController.constructionToBuild = construction;
            }
        }
        return returnValue;
    }

    private float FindBuildPercentage(IAConstruction construction, StorageInventory inventory, StorageInventory inventory2 = null)
    {
        float percentage = 0f;
        int values = 0;
        foreach (ResourceType r in ResourceType.GetValues(typeof(ResourceType)))
        {
            float resourceNeeded = construction.GetResourceNeeded(r, 0);
            
            if (resourceNeeded > 0)
            {
                if(inventory2 == null) percentage += Mathf.Clamp01(inventory.Inventory[r] / resourceNeeded);
                else percentage += Mathf.Clamp01((inventory.Inventory[r] + inventory2.Inventory[r]) / resourceNeeded);
                values += 1;
            }
            
        }
        percentage /= values;

        return percentage;
    }

    private bool CanBuildMore(IAConstruction construction)
    {
        BuildingType buildTypeSearch = construction.prefab.GetComponent<Building>().BuildingType;
        return numberOfConstructionBuilt[buildTypeSearch] < construction.numberMaxToSpawn;
    }

    public IAConstruction FindTheCheapestConstructionToBuild(StorageInventory townInventory, StorageInventory npcInventory)
    {
        float maxPercentageOfResourcesAvailable = -1f;
        IAConstruction cheapestConstruction = null;
        
        foreach(IAConstruction construction in possibleConstructions)
        {
            if (!CanBuildMore(construction)) continue;
            float percentage = FindBuildPercentage(construction, townInventory, npcInventory);
            percentage -= numberOfConstructionBuilt[construction.prefab.GetComponent<Building>().BuildingType] * 0.1f; // 10% less per building of the same type
            
            if(percentage < 1 && percentage > maxPercentageOfResourcesAvailable) // IF it can be build, the build script will take care of it
            {
                maxPercentageOfResourcesAvailable = percentage;
                cheapestConstruction = construction;
            }
        }
        return cheapestConstruction;
    }
}
