using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInventory : StorageInventory
{
    private NPCController _npcController;
    [SerializeField] private int _maxCapacity;
    [SerializeField] private Billboard InventoryUI;
    private NPCStats _npcStats;

    public delegate void InventoryChangedHandler();
    public event InventoryChangedHandler OnInventoryChanged;

    // Start is called before the first frame update
    void Start()
    {
        _npcController = GetComponent<NPCController>();
        _npcStats = GetComponent<NPCStats>();
        InitializeInventory();
        SetMaxCapacity(_maxCapacity);
        SetResourceStat();
    }

    private void OnEnable()
    {
        OnInventoryChanged += InventoryChanged;
    }

    private void OnDisable()
    {
        OnInventoryChanged -= InventoryChanged;
    }


    public void SetMaxCapacity(int capacity)
    {
        MaxCapacity = capacity;
    }

    public void SetUI(Billboard b)
    {
        InventoryUI = b;
    }

    public override void AddResource(ResourceType r, int amount)
    {
        //Update town score and resource score
        _npcController.homeTown.townScore += amount;
        _npcController.homeTown.townResourceScore[(int)r] += amount;
        
        int amountInInventory = CheckInventoryCount();
        if (amountInInventory + amount > MaxCapacity)
        {
            int amountCanAdd = MaxCapacity - amountInInventory;
            Inventory[r] += amountCanAdd;
        }
        else
        {
            Inventory[r] += amount;
        }

        SetResourceStat();
        OnInventoryChanged?.Invoke();
    }

    public void RemoveAllResource()
    {
        List<ResourceType> types = new List<ResourceType>();
        foreach(ResourceType r in Inventory.Keys)
        {
            types.Add(r);
        }

        foreach(ResourceType r in types)
        {
            Inventory[r] = 0;
        }
        SetResourceStat();
        OnInventoryChanged?.Invoke();
    }

    public override void RemoveResource(ResourceType r, int amount)
    {
        //Update town score and resource score
        _npcController.homeTown.townResourceScore[(int)r] -= amount;
        
        if (Inventory[r] - amount < 0)
        {
            Inventory[r] = 0;
        }
        else
        {
            Inventory[r] -= amount;
        }
        SetResourceStat();
        OnInventoryChanged?.Invoke();
    }        

    public void InventoryChanged()
    {
        InventoryUI.UpdateInventoryText(Inventory[ResourceType.wood], Inventory[ResourceType.stone], Inventory[ResourceType.metal]);
    }
    private void SetResourceStat()
    {
        _npcStats.resource = CheckInventoryCount();
    }
}
