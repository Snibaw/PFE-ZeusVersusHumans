using System.Collections;
using System.Collections.Generic;
using Unity.Android.Types;
using UnityEngine;

[CreateAssetMenu(fileName = "DropOffResource", menuName = "UtilityAI/Actions/DropOffResource")]
public class DropOffResource : Action
{
    public override void Execute(NPCController _npcController)
    {
        Debug.Log("Dropped Off Resource");
        _npcController.Inventory.RemoveAllResource();
        _npcController.stats.resource = 0;
        _npcController.aiBrain.finishedExecutingBestAction = true;
    }

    public override void SetRequiredDestination(NPCController _npcController)
    {
        RequiredDestination = _npcController.context.storage.transform;
        _npcController.mover.destination = RequiredDestination;
    }
}
