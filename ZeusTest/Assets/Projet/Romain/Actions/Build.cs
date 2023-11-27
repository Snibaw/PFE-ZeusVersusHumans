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
        Vector3 currentConstructionPosition = _npcController.FindBuildPosition().position;
        Vector3 IAposition = _npcController.gameObject.transform.position;
        Vector3 IAforward = _npcController.gameObject.transform.forward;
        GameObject Target = new GameObject();
        // Debug.Log("Distance : " + Vector3.Distance(IAposition, currentConstructionPosition));
        if(currentConstructionPosition == Vector3.zero || Vector3.Distance(IAposition, currentConstructionPosition) < 0.1f) 
        {
            Target.transform.position = IAposition - IAforward*1.5f;
            // Debug.Log("Close");
        }
        else 
        {
            Target.transform.position = currentConstructionPosition + ( IAposition - currentConstructionPosition).normalized*1.5f;
            // Debug.Log("Far");
        }
        RequiredDestination = Target.transform;
        
    }
}