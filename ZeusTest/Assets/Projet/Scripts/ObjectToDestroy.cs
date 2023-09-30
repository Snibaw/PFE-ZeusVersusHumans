using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectToDestroy : MonoBehaviour
{
    [SerializeField] private float life;
    
    public void TakeDamage(float damage)
    {
        #if UNITY_EDITOR 
            GameManager.instance.SetDamageText(damage, life); 
        #endif

        life -= damage;

        if(life <= 0)
        {
            Destroy(gameObject);
        }
    }
}