using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "UtilityAI/Actions/Upgrade")]
public class Upgrade : Action
{
    public override void Execute(NPCController _npcController)
    {
        _npcController.DoAction("Upgrade", timeToExecute);
    }

    public override void SetRequiredDestination(NPCController _npcController)
    {
        RequiredDestination = _npcController.FindUpgradePosition();
    }
}
