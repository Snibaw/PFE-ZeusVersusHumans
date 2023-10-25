using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sleep", menuName = "UtilityAI/Actions/Sleep")]
public class Sleep : Action
{
    public override void Execute(NPCController _npcController) // dependency injection
    {
        _npcController.DoSleep(3); 
    }

    public override void SetRequiredDestination(NPCController _npcController)
    {
        RequiredDestination = _npcController.context.home.transform;
        _npcController.mover.destination = RequiredDestination;
    }
}
