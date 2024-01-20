using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Build", menuName = "UtilityAI/Actions/Build")]
public class Build : Action
{
    public override void Execute(NPCController _npcController)
    {
        _npcController.DoAction("Build", timeToExecute);
    }

    public override void SetRequiredDestination(NPCController _npcController)
    {
        RequiredDestination = _npcController.FindBuildRequiredDestination();
        
    }
}
