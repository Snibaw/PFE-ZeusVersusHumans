using System.Collections;
using System.Collections.Generic;
using Unity.Android.Types;
using UnityEngine;

[CreateAssetMenu(fileName = "FetchResource", menuName = "UtilityAI/Actions/FetchResource")]
public class FetchResource : Action
{
    public override void Execute(NPCController _npcController)
    {
        _npcController.DoAction("FetchResource", timeToExecute);
    }

    public override void SetRequiredDestination(NPCController _npcController)
    {
        RequiredDestination = _npcController.context.storage.transform;
    }
}
