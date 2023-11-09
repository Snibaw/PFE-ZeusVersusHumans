using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New IAConstruction", menuName = "IAConstruction")]
public class IAConstruction : ScriptableObject
{
    public GameObject prefab;
    public int numberMaxToSpawn;
    public int woodCost;
    public int stoneCost;
    public int metalCost;
    public int GetResourceNeededOld(TypeOfResources resource)
    {
        switch(resource)
        {
            case TypeOfResources.wood:
                return woodCost;
            case TypeOfResources.stone:
                return stoneCost;
            default:
                return -1;
        }
    }
    public int GetResourceNeeded(ResourceType resource)
    {
        switch(resource)
        {
            case ResourceType.wood:
                return woodCost;
            case ResourceType.stone:
                return stoneCost;
            case ResourceType.metal:
                return metalCost;
            default:
                return 0;
        }
    }
}
