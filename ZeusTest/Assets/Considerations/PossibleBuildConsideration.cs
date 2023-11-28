using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PossibleBuildConsideration", menuName = "UtilityAI/Considerations/PossibleBuildConsideration")]
public class PossibleBuildConsideration : Consideration
{
    [SerializeField] private AnimationCurve responseCurve;
    public override float ScoreConsideration(NPCController _NPCController)
    {
        score = responseCurve.Evaluate(Mathf.Clamp01(_NPCController.GetPossibleBuildScore())); 
        return score;
    }
}
