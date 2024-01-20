using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New IAConstruction", menuName = "IAConstruction")]
public class IAConstruction : ScriptableObject
{
    public GameObject prefab;
    public int numberMaxToSpawn;
    public List<int> woodCost;
    public List<int> stoneCost;
    public List<int> metalCost;
    public int maxLevel;
    [SerializeField] private AnimationCurve adorationModifierCurve;

    public int GetResourceNeededOld(TypeOfResources resource)
    {
        switch(resource)
        {
            case TypeOfResources.wood:
                return woodCost[0];
            case TypeOfResources.stone:
                return stoneCost[0];
            default:
                return -1;
        }
    }
    public int GetResourceNeeded(ResourceType resource, int level)
    {
        switch(resource)
        {
            case ResourceType.wood:
                return level < woodCost.Count ? woodCost[level] : 999;
            case ResourceType.stone:
                return level < stoneCost.Count ? stoneCost[level] : 999;
            case ResourceType.metal:
                return level < metalCost.Count ? metalCost[level] : 999;
            default:
                return 0;
        }
    }

    public float AdorationScoreConsideration(TownBehaviour town)
    {
        float score = adorationModifierCurve.Evaluate(Mathf.Clamp01(town.adorationValue / 100f));
        return score;
    }
}
