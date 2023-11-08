using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBehaviour : MonoBehaviour
{
    private QuadraticCurve curve;
    [SerializeField] private float speed = 1;
    private float sampleTime = 0;
    private float intensity = 1;


    private void Start()
    {
        sampleTime = 0;
    }
    private void Update() {
        sampleTime += Time.deltaTime * speed;
        transform.position = curve.evaluate(sampleTime);
        transform.forward = curve.evaluate(sampleTime + 0.01f) - transform.position;

        if(sampleTime > 1) Destroy(gameObject);
    }
    
    public void InitValues(QuadraticCurve _curve, float _intensity)
    {
        curve = _curve;
        intensity = _intensity;
        transform.localScale = Vector3.one * intensity;
    }

    private void OnTriggerEnter(Collider other) {
        //Create a circle around the lightning depending on the intensity
        //If the circle collides with an object with the CanBeDestroyed Layer, access the script and do damage
        Collider[] colliders = Physics.OverlapSphere(transform.position, intensity);
        foreach(Collider collider in colliders)
        {
            if(collider.gameObject.layer == LayerMask.NameToLayer("CanBeDestroyed"))
            {
                if (collider.GetComponent<ObjectToDestroy>() == null)
                {
                    Debug.Log("Object" + collider.gameObject.name + " has no ObjectToDestroy script");
                    continue;
                }
                collider.GetComponent<ObjectToDestroy>().TakeDamage(Mathf.Clamp01(intensity/5)*100);
            }
        }
        


        Destroy(gameObject);
    }
    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, intensity);
    }
}
