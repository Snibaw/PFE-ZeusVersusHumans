using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Work", menuName = "UtilityAI/Actions/Work")]
public class Work : Action
{
    public override void Execute(NPCController _npcController)
    {
        _npcController.DoAction("Work", timeToExecute);
    }

    public override void SetRequiredDestination(NPCController _npcController)
    {
        float distance = Mathf.Infinity;
        Transform nearestResource = null;
        // GameObject targetObject = null;

        ResourceType resourceNeeded = _npcController.context.storage.GetResourceNeeded(_npcController.Inventory);
        List<Transform> resources = _npcController.context.Destinations[DestinationType.resource];
        foreach (Transform resource in resources)
        {
            if (resource != null) 
            {
                Resource resourceComponent = resource.GetComponent<Resource>();
            if(resourceComponent.ResourceType != resourceNeeded || !resourceComponent.canBeHarvested || resourceComponent.isMarked) 
                continue;
            
            float distanceFromResource = Vector3.Distance(_npcController.transform.position, resource.position);
            if (distanceFromResource < distance)
            {
                nearestResource = resource;
                distance = distanceFromResource;
            }
            }
        }
        if(nearestResource == null) // we didn't find any resource to harvest
        {
            _npcController.currentState = State.decide;
            return;
        }

        nearestResource.GetComponent<Resource>().MarkThisResource();
        RequiredDestination = nearestResource;
    }
}
