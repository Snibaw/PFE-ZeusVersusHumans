using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Eat", menuName = "UtilityAI/Actions/Eat")]
public class Eat : Action
{
    public override void Execute(NPCController _npcController)
    {
        Debug.Log("I ate food");
        // Logic for updating everything involved with eating (inventory, hunger stat, etc.)
        _npcController.stats.hunger -= 30;
        // _npcController.OnFinishedAction();
        _npcController.aiBrain.finishedExecutingBestAction = true;
    }

    public override void SetRequiredDestination(NPCController _npcController)
    {
        RequiredDestination = _npcController.gameObject.transform;
        _npcController.mover.destination = RequiredDestination;
    }
}
