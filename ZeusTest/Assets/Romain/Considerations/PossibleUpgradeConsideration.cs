using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PossibleUpgradeConsideration", menuName = "UtilityAI/Considerations/PossibleUpgradeConsideration")]
public class PossibleUpgradeConsideration : Consideration
{
    [SerializeField] private AnimationCurve responseCurve;
    public override float ScoreConsideration(NPCController _NPCController)
    {
        score = responseCurve.Evaluate(Mathf.Clamp01(_NPCController.GetPossibleUpgradeScore())); 
        return score;
    }
}