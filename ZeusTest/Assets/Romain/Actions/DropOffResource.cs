using System.Collections;
using System.Collections.Generic;
using Unity.Android.Types;
using UnityEngine;

[CreateAssetMenu(fileName = "DropOffResource", menuName = "UtilityAI/Actions/DropOffResource")]
public class DropOffResource : Action
{
    public override void Execute(NPCController _npcController)
    {
        _npcController.DoAction("DropOffResource", timeToExecute);
    }

    public override void SetRequiredDestination(NPCController _npcController)
    {
        RequiredDestination = _npcController.context.storage.transform;
    }
}
