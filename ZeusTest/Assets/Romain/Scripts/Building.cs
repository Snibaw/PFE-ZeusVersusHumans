using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingType
{
    village,
    mine,
    sawmill,
    house
}

public class Building : MonoBehaviour
{
    [SerializeField] private BuildingType buildingType;
    public BuildingType BuildingType
    {
        get
        {
            return buildingType;
        }
        set
        {
            buildingType = value;
        }
    }
}
