using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sleep", menuName = "UtilityAI/Actions/Sleep")]
public class Sleep : Action
{
    public override void Execute(NPCController _npcController) // dependency injection
    {
        _npcController.DoAction("Sleep", _npcController.canMeditate() ? timeToExecute / 2.0f : timeToExecute); 
    }

    public override void SetRequiredDestination(NPCController _npcController)
    {
        RequiredDestination = _npcController.canMeditate() ? _npcController.transform : _npcController.context.FindClosestRestPosition(_npcController.transform.position);
    }
}
