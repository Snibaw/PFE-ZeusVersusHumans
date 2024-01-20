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
    [SerializeField] private float randomModifierStrength = 0.3f;
    

    private void Start()
    {
        storageInventory = GetComponent<StorageInventory>();
        AddBuildingBuilt(gameObject.GetComponent<Building>());
    }
    public void AddBuildingBuilt(Building building)
    {
        possibleUpgrades.Add(building);
    }

    public IAConstruction BuildingToConstruction(Building building)
    {
        return constructionValue[Array.IndexOf(buildingToConstruction, building.BuildingType)];
    }

    public int HowManyBuildingCanBeUpgraded(NPCController _npcController, StorageInventory inventory = null)
    {
        if(inventory == null) inventory = storageInventory;
        
        int returnValue = 0;
        for (int i = 0; i< possibleUpgrades.Count; i++)
        {
            Building building = possibleUpgrades[i];
            if (building == null) {possibleUpgrades.RemoveAt(i); i--;continue;}
            if (!CanUpgrade(building)) continue;
            
            if (FindUpgradePercentage(building, inventory) == 1)
            {
                var construction = BuildingToConstruction(building);
                _npcController.buildingToUpgrade = building;

            }
        }
        return returnValue;
    }

    public float FindUpgradePercentage(Building building, StorageInventory inventory = null, StorageInventory inventory2 = null)
    {
        
        float percentage = 0f;
        int values = 0;
        if(inventory == null) inventory = storageInventory;
        foreach (ResourceType r in ResourceType.GetValues(typeof(ResourceType)))
        {
            float resourceNeeded = BuildingToConstruction(building).GetResourceNeeded(r, building.level);
            
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
        return building.level < BuildingToConstruction(building).maxLevel;
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
            if (building.level < BuildingToConstruction(building).maxLevel) return false;
        }
        return true;
    }

    public Building FindNextUpgrade(TownBehaviour town, out float score)
    {
        score = 0;
        Building bestBuilding = null;
        float currentScore = -1f;
        foreach (Building building in possibleUpgrades)
        {
            if (!CanUpgrade(building)) continue;

            var construction = BuildingToConstruction(building);
            
                currentScore = construction.AdorationScoreConsideration(town) * UnityEngine.Random.Range(1f- randomModifierStrength, 1f + randomModifierStrength) * 
                    2f;
                if ( currentScore > score)
                {
                    score = currentScore;
                    bestBuilding = building;
                }
                

        }
        return bestBuilding;
    }
}
