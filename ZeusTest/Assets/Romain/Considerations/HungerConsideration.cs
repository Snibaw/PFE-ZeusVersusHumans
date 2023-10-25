using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HungerConsideration", menuName = "UtilityAI/Considerations/HungerConsideration")]
public class HungerConsideration : Consideration
{
    [SerializeField] private AnimationCurve responseCurve;
    public override float ScoreConsideration(NPCController _NPCController)
    {
        score = responseCurve.Evaluate(Mathf.Clamp01(_NPCController.stats.hunger/ 100f));
        return score;
    }
}
