using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnergyConsideration", menuName = "UtilityAI/Considerations/EnergyConsideration")]
public class EnergyConsideration : Consideration
{
    [SerializeField] private AnimationCurve responseCurve;

    public override float ScoreConsideration(NPCController _NPCController)
    {
        score = responseCurve.Evaluate(Mathf.Clamp01(_NPCController.stats.energy / 100f));
        return score;
    }
}
