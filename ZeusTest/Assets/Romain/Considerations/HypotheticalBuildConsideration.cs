using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HypotheticalBuildConsideration", menuName = "UtilityAI/Considerations/HypotheticalBuildConsideration")]
public class HypotheticalBuildConsideration : Consideration
{
    [SerializeField] private AnimationCurve responseCurve;
    public override float ScoreConsideration(NPCController _NPCController)
    {
        score = responseCurve.Evaluate(Mathf.Clamp01(_NPCController.GetHypotheticalBuildScore()/2)); 
        return score;
    }
}
