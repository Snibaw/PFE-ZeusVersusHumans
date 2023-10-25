using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Work", menuName = "UtilityAI/Actions/Work")]
public class Work : Action
{
    public override void Execute(NPCController _npcController)
    {
        _npcController.DoWork(3); // dependency injection
    }

    public override void SetRequiredDestination(NPCController _npcController)
    {
        float distance = Mathf.Infinity;
        Transform nearestResource = null;

        List<Transform> resources = _npcController.context.Destinations[DestinationType.resource];
        foreach (Transform resource in resources)
        {
            float distanceFromResource = Vector3.Distance(_npcController.transform.position, resource.position);
            if (distanceFromResource < distance)
            {
                nearestResource = resource;
                distance = distanceFromResource;
            }
        }
        RequiredDestination = nearestResource;
        _npcController.mover.destination = RequiredDestination;
    }
}
