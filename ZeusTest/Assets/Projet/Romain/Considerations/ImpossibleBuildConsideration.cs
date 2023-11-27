using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ImpossibleBuildConsideration", menuName = "UtilityAI/Considerations/ImpossibleBuildConsideration")]
public class ImpossibleBuildConsideration : Consideration
{
    [SerializeField] private AnimationCurve responseCurve;
    public override float ScoreConsideration(NPCController _NPCController)
    {
        score = responseCurve.Evaluate(1 - Mathf.Clamp01(_NPCController.GetPossibleBuildScore())); 
        return score;
    }
}
