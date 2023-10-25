using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceConsideration", menuName = "UtilityAI/Considerations/ResourceConsideration")]
public class ResourceConsideration : Consideration
{
    [SerializeField] private AnimationCurve responseCurve;
    public override float ScoreConsideration(NPCController _NPCController)
    {
        score = responseCurve.Evaluate(Mathf.Clamp01(_NPCController.stats.resource /
                                                     _NPCController.Inventory.MaxCapacity));
        Debug.Log(Mathf.Clamp01(_NPCController.stats.resource /
                                _NPCController.Inventory.MaxCapacity));
        return score;
    }
}
