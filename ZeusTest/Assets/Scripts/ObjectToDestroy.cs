using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectToDestroy : MonoBehaviour
{
    public float life;
    public float maxLife;
    [SerializeField] private HealthBarRessources healthBar;

	void Start()
    {
        if(healthBar != null) healthBar.SetMaxHealth(life);
        maxLife = life;
    }	
    
    public void TakeDamage(float damage, bool impactAdorationBarValue = false)
    {
        life -= damage;

        if (life > 0 && healthBar != null)
        {
            healthBar.DamageRessource(damage);
        }

        if(life <= 0)
        {
            healthBar.ShowHealthBar(false);
            Resource resource = GetComponent<Resource>();
            Building building = GetComponent<Building>();
            if(resource != null) //If the object is a resource
            {
                if (resource.canBeHarvested == false) return; // Not already destroyed
                
                if(impactAdorationBarValue) AdorationBar.instance.FindAndChangeNearestAdorationBar(transform.position, AdorationBarEvents.DestroyResource);
                resource.HasBeenHarvested();
                return;
            }
            else if(building != null) //If the object is a building
            {
                if (impactAdorationBarValue)
                {
                    //Find the adorationBarManager of the town corresponding to the building
                    AdorationBarManager adorationBarManager = building.context.gameObject.transform.parent
                        .GetComponent<AdorationBarManager>();
                    adorationBarManager.ChangeAdorationBarValue(AdorationBarEvents.DestroyBuilding);
                }
                if (building.BuildingType == BuildingType.house)
                {
                    building.context.RemoveObjectFromDestination(transform, DestinationType.rest);
                }
                PointDistribution.instance.SetAllInColliderToObstacle(GetComponent<BoxCollider>(), false);
                
                
            }
            else if(GetComponent<NPCController>() != null) //If the object is an AI
            {
                AdorationBar.instance.FindAndChangeAdorationBarNPC(GetComponent<NPCController>(),
                    AdorationBarEvents.KillHuman);
            }
            else if(GetComponent<WolfController>() != null) //If the object is a wolf
            {
                if(impactAdorationBarValue) AdorationBar.instance.FindAndChangeNearestAdorationBar(transform.position, AdorationBarEvents.KillWolf);
            }
            Destroy(gameObject);
            //TODO: Event to tell the GameManager that the player destroyed an object or an AI (spawn more AI ...)
        }
    }
}
