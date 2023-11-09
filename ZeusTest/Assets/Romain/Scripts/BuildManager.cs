using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class BuildManager : MonoBehaviour
{
    [SerializeField] private IAConstruction[] possibleConstructions;
    private StorageInventory storageInventory;

    private void Start()
    {
        storageInventory = GetComponent<StorageInventory>();
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

    private float FindBuildPercentage(IAConstruction construction, StorageInventory inventory)
    {
        float percentage = 0f;
        int values = 0;
        foreach (ResourceType r in ResourceType.GetValues(typeof(ResourceType)))
        {
            float resourceNeeded = construction.GetResourceNeeded(r);
            
            if (resourceNeeded > 0)
            {
                percentage += Mathf.Clamp01(inventory.Inventory[r] / resourceNeeded);
                values += 1;
            }
            
        }
        percentage /= values;

        return percentage;
    }

    private bool CanBuildMore(IAConstruction construction)
    {
        //Later, just keep the number of build for a town
        //Find every construction of the same type
        GameObject[] constructions = GameObject.FindGameObjectsWithTag(construction.prefab.tag);
        if(constructions.Length >= construction.numberMaxToSpawn)
        {
            return false;
        }

        return true;
    }
}
