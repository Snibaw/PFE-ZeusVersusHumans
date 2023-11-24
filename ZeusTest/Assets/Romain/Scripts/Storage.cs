using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : StorageInventory
{
    [SerializeField] private int maxCapacityPerType;
    private BuildManager _buildManager;

    private void Start()
    {
        _buildManager = GetComponent<BuildManager>();
        InitializeInventory();
        MaxCapacity = maxCapacityPerType * Inventory.Count;
    }

    public void SetMaxCapacityPerType(int capacity)
    {
        maxCapacityPerType = capacity;
    }

    public int GetNbResources(ResourceType resource)
    {
        return Inventory[resource];
    }

    public override void AddResource(ResourceType r, int amount)
    {
        int amountInInventory = Inventory[r];
        if (amountInInventory + amount > maxCapacityPerType)
        {
            int amountCanAdd = maxCapacityPerType - amountInInventory;
            Inventory[r] += amountCanAdd;
        }
        else
        {
            Inventory[r] += amount;
        }
    }

    public override void RemoveResource(ResourceType r, int amount)
    {
        if (Inventory[r] - amount < 0)
        {
            Inventory[r] = 0;
        }
        else
        {
            Inventory[r] -= amount;
        }
    }
    public void GetAllResourcesFromNPC(NPCInventory npcInventory)
    {
        foreach (ResourceType r in npcInventory.Inventory.Keys)
        {
            AddResource(r, npcInventory.Inventory[r]);
        }
        npcInventory.RemoveAllResource();
    }

    //Basically : We take the resource of the town + the resource of the NPC asking for it
    // We check which construction needs the least amount of work to be built
    // In this construction, we check the missing resources and we return the one that is the most missing
    public ResourceType GetResourceNeeded(StorageInventory npcInventory)
    {
        ResourceType resourceNeeded = ResourceType.wood;
        
        //Find the cheapest construction to build
        IAConstruction cheapestConstruction = _buildManager.FindTheCheapestConstructionToBuild(this, npcInventory);

        if (cheapestConstruction != null) // we can still build
        {
            // Find the resource that is the most missing
            int maxNumberOfResourcesNeeded = -100000;
            foreach(ResourceType resourceType in System.Enum.GetValues(typeof(ResourceType)))
            {
                int numberOfRessourceNeeded = cheapestConstruction.GetResourceNeeded(resourceType, 0) - (Inventory[resourceType] + npcInventory.Inventory[resourceType]);
                if(numberOfRessourceNeeded > maxNumberOfResourcesNeeded && numberOfRessourceNeeded > 0)
                {
                    maxNumberOfResourcesNeeded = numberOfRessourceNeeded;
                    resourceNeeded = resourceType;
                }
            }
        }
        else
        {
            //Just return the resource that is the most missing
            float minValue = maxCapacityPerType;
            foreach (ResourceType r in Inventory.Keys)
            {
                if (Inventory[r] < minValue)
                {
                    minValue = Inventory[r];
                    resourceNeeded = r;
                }
            }
        }
        return resourceNeeded;
        
    }
}