using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectToDestroy : MonoBehaviour
{
    [SerializeField] private float life;
    [SerializeField] private HealthBarRessources healthBar;

	void Start()
{
}	
    
    public void TakeDamage(float damage)
    {
        #if UNITY_EDITOR 
            GameManager.instance.SetDamageText(damage, life); 
        #endif
        
        // if (!healthBar.gameObject.activeSelf)
        // {
        //     healthBar.ActivateHealthBar();
        // }
        healthBar.SetMaxHealth(life);
        healthBar.DamageRessource(damage);

        life -= damage;

        if(life <= 0)
        {
            Resource resource = GetComponent<Resource>();
            if(resource != null) //If the object is a resource
            {
                if (resource.canBeHarvested == false) return;
                resource.HasBeenHarvested();
                return;
            }
            else if(GetComponent<NPCController>() != null) //If the object is an AI
            {
                AdorationBar.instance.ChangeAdorationBarValue(AdorationBarEvents.KillHuman);
            }
            Destroy(gameObject);
            //TODO: Event to tell the GameManager that the player destroyed an object or an AI (spawn more AI ...)
        }
    }
}
