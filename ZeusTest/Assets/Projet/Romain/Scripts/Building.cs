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
    public int level = 0;
    public void levelUp()
    {
        level++;
    }
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
