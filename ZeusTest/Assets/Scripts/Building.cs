using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingType
{
    village,
    mine,
    sawmill,
    house,
    babel
}

public class Building : MonoBehaviour
{
    [SerializeField] private BuildingType buildingType;
    [SerializeField] private Material Roof2;
    [SerializeField] private Material Roof3;
    [SerializeField] private Material Stone;
    [SerializeField] private Material Metal;
    [SerializeField] protected GameObject level2;
    [SerializeField] private GameObject level3;
    [SerializeField] private GameObject changingMaterial;
    [SerializeField] private GameObject roof;
    public int level = 0;
    public virtual void levelUp()
    {
        level++;

        switch(level){
        case 2:
            if (level2 != null) level2.SetActive(true);
            if (roof != null) ChangeMaterial(roof,Roof2 );
            if (changingMaterial != null) ChangeMaterial(changingMaterial, Stone);
            break;
        case 3:
            if (level3 != null) level3.SetActive(true);
            if (roof != null) ChangeMaterial(roof, Roof3);
            if (changingMaterial != null) ChangeMaterial(changingMaterial, Metal);
            break;
        }
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

    void ChangeMaterial( GameObject target, Material mat) {
        Renderer[] children;
        children = target.GetComponentsInChildren<Renderer>();
            foreach (var i in children) {
            i.material = mat;
            }
    }
}
