using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HowFullIsMyInventory", menuName = "UtilityAI/Considerations/HowFullIsMyInventory")]
public class HowFullIsMyInventory : Consideration
{
    [SerializeField] private AnimationCurve responseCurve;
    
    public override float ScoreConsideration(NPCController _npcController)
    {
        score = responseCurve.Evaluate(Mathf.Clamp01(_npcController.Inventory.HowFullIsStorage()));
        return score;
    }
}
