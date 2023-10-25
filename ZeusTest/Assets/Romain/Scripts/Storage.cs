using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : StorageInventory
{
    [SerializeField] private int maxCapacityPerType;

    private void Start()
    {
        InitializeInventory();
        MaxCapacity = maxCapacityPerType * Inventory.Count;
    }

    public void SetMaxCapacityPerType(int capacity)
    {
        maxCapacityPerType = capacity;
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

    public ResourceType GetResourceNeeded()
    {
        float minValue = maxCapacityPerType;
        ResourceType resourceNeeded = ResourceType.wood;
        
        foreach(ResourceType r in Inventory.Keys)
        {
            Debug.Log("Resource " + r + " has " + Inventory[r] + " items");
            if(Inventory[r] < minValue)
            {
                minValue = Inventory[r];
                resourceNeeded = r;
            }
        }

        Debug.Log(resourceNeeded);
        return resourceNeeded;
    }
}