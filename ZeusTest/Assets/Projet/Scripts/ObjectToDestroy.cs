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
            Destroy(gameObject);
            //TODO: Event to tell the GameManager that the player destroyed an object or an AI (spawn more AI ...)
        }
    }
}
