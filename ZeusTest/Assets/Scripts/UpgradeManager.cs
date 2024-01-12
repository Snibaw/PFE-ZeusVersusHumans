using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements.Experimental;
using System.Linq;

public class UpgradeManager : MonoBehaviour
{
    public List<Building> possibleUpgrades;
    private StorageInventory storageInventory;
    public BuildingType[] buildingToConstruction;
    public IAConstruction[] constructionValue;

    

    private void Start()
    {
        storageInventory = GetComponent<StorageInventory>();
        AddBuildingBuilt(gameObject.GetComponent<Building>());
    }
    public void AddBuildingBuilt(Building building)
    {
        possibleUpgrades.Add(building);
    }

    public int HowManyBuildingCanBeUpgraded(NPCController _npcController, StorageInventory inventory = null)
    {
        if(inventory == null) inventory = storageInventory;

        int returnValue = 0;
        foreach (Building building in possibleUpgrades)
        {
            if (!CanUpgrade(building)) continue;
            
            if (FindUpgradePercentage(building, inventory) == 1)
            {
                returnValue += 1;
                _npcController.buildingToUpgrade = building;
                _npcController.constructionToUpgrade = constructionValue[Array.IndexOf(buildingToConstruction, building.BuildingType)];
            }
        }
        return returnValue;
    }

    private float FindUpgradePercentage(Building building, StorageInventory inventory, StorageInventory inventory2 = null)
    {
        float percentage = 0f;
        int values = 0;
        foreach (ResourceType r in ResourceType.GetValues(typeof(ResourceType)))
        {
            float resourceNeeded = constructionValue[Array.IndexOf(buildingToConstruction, building.BuildingType)].GetResourceNeeded(r, building.level);
            
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

    private bool CanUpgrade(Building building)
    {
        if (building.BuildingType == BuildingType.babel && !GameManager.instance.CanMakeBabel) return false;
        return building.level < constructionValue[Array.IndexOf(buildingToConstruction, building.BuildingType)].maxLevel;
    }

    public Building FindTheCheapestBuildingToUpgrade(StorageInventory townInventory, StorageInventory npcInventory)
    {
        float maxPercentageOfResourcesAvailable = -1f;
        Building cheapestBuilding = null;
        
        foreach(Building building in possibleUpgrades)
        {
            if (!CanUpgrade(building)) continue;
            float percentage = FindUpgradePercentage(building, townInventory, npcInventory);
            
            if(percentage < 1 && percentage > maxPercentageOfResourcesAvailable) // IF it can be upgraded, the upgrade script will take care of it
            {
                maxPercentageOfResourcesAvailable = percentage;
                cheapestBuilding = building;
            }
        }
        return cheapestBuilding;
    }

    public bool CheckIfComplete()
    {
        foreach(Building building in possibleUpgrades)
        {
            if (building.BuildingType == BuildingType.babel) continue;
            if (building.level < constructionValue[Array.IndexOf(buildingToConstruction, building.BuildingType)].maxLevel) return false;
            return true;
        }
        return true;
    }
}
