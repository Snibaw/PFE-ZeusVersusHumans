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
    public int GetResourceNeeded(TypeOfResources resource)
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
}
