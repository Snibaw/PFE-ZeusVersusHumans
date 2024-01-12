using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectToDestroy : MonoBehaviour
{
    [SerializeField] private float life;
    [SerializeField] private HealthBarRessources healthBar;

	void Start()
{
    healthBar.SetMaxHealth(life);
}	
    
    public void TakeDamage(float damage, bool impactAdorationBarValue = false)
    {
        #if UNITY_EDITOR 
            GameManager.instance.SetDamageText(damage, life); 
        #endif

        life -= damage;

        if (life > 0)
        {
            healthBar.DamageRessource(damage);
        }

        if(life <= 0)
        {
            Resource resource = GetComponent<Resource>();
            Building building = GetComponent<Building>();
            if(resource != null) //If the object is a resource
            {
                if (resource.canBeHarvested == false) return; // Not already destroyed
                
                if(impactAdorationBarValue) AdorationBar.instance.ChangeAdorationBarValue(AdorationBarEvents.DestroyResource);
                resource.HasBeenHarvested();
                return;
            }
            else if(building != null) //If the object is a building
            {
                if(impactAdorationBarValue) AdorationBar.instance.ChangeAdorationBarValue(AdorationBarEvents.DestroyBuilding);
                if (building.BuildingType == BuildingType.house)
                {
                    GameManager.instance.context.RemoveObjectFromDestination(transform, DestinationType.rest);
                }
                
                
            }
            else if(GetComponent<NPCController>() != null) //If the object is an AI
            {
                if(impactAdorationBarValue) AdorationBar.instance.ChangeAdorationBarValue(AdorationBarEvents.KillHuman);
            }
            else if(GetComponent<WolfController>() != null) //If the object is a wolf
            {
                if(impactAdorationBarValue) AdorationBar.instance.ChangeAdorationBarValue(AdorationBarEvents.KillWolf);
            }
            Destroy(gameObject);
            //TODO: Event to tell the GameManager that the player destroyed an object or an AI (spawn more AI ...)
        }
    }
}
