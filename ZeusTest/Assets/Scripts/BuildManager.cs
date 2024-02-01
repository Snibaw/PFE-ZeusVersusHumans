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
    [SerializeField] private float randomModifierStrength = 0.3f;

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
            numberOfConstructionBuilt.TryAdd(buildingType, 0);
        }
    }
    public void AddConstructionBuilt(BuildingType buildingType)
    {
        numberOfConstructionBuilt[buildingType] += 1;
    }

        public void DecreaseConstructionBuilt(BuildingType buildingType)
    {
        numberOfConstructionBuilt[buildingType] -= 1;
    }

    public int HowManyConstructionCanBeBuilt(NPCController _npcController, StorageInventory inventory = null)
    {
        if(inventory == null) inventory = storageInventory;

        int returnValue = 0;
        foreach (IAConstruction construction in possibleConstructions)
        {
            if (!CanBuild(construction, _npcController.homeTown)) continue;
            if (!CanBuildMore(construction)) continue;
            
            if (FindBuildPercentage(construction, inventory) == 1)
            {
                returnValue += 1;
                _npcController.constructionToBuild = construction;

                
            }
        }
        return returnValue;
    }

    public float FindBuildPercentage(IAConstruction construction, StorageInventory inventory = null, StorageInventory inventory2 = null)
    {
        float percentage = 0f;
        int values = 0;
        if(inventory == null) inventory = storageInventory;
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
        
        if(numberOfConstructionBuilt.TryGetValue(buildTypeSearch, out int value))
        {
            return value < construction.numberMaxToSpawn;
        }
        numberOfConstructionBuilt.TryAdd(buildTypeSearch, 0);
        return true;
    }

    private bool CanBuild(IAConstruction construction, TownBehaviour town)
    {
        BuildingType buildTypeSearch = construction.prefab.GetComponent<Building>().BuildingType;
        if (buildTypeSearch == BuildingType.lightningRod && !town.canBuildLightningRod) return false;
        return true;
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

    public IAConstruction FindNextConstruction(TownBehaviour town)
    {
        float score = 0;
        IAConstruction bestConstruction = null;
        float currentScore = -1f;
        foreach (IAConstruction construction in possibleConstructions)
        {
            if (!CanBuild(construction, town)) continue;
            if (!CanBuildMore(construction)) continue;
            
                currentScore = construction.AdorationScoreConsideration(town) * UnityEngine.Random.Range(1f - randomModifierStrength, 1f + randomModifierStrength) * 
                    2f * (1f - numberOfConstructionBuilt[construction.prefab.GetComponent<Building>().BuildingType] * 0.1f);
                if ( currentScore > score)
                {
                    score = currentScore;
                    bestConstruction = construction;
                }
                

        }
        if (bestConstruction == null) Debug.Log("All building built!!");
        return bestConstruction;
    }
}
