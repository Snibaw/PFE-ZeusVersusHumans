using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class LightningBehaviour : MonoBehaviour
{
    public QuadraticCurve curve;
    public float speed = 0.1f;
    [SerializeField] private float speedAugmentationOverTime = 0.01f;
    [SerializeField] private GameObject lightningImpactPrefab;
    private float sampleTime = 0;
    private float intensity = 1;
    private float magnitudeMultiplier = 3, roughness = 4, fadeInTime = 0.2f, fadeOutTime = 0.5f;


    private void Start()
    {
        sampleTime = 0;
    }
    private void Update()
    {
        speed = Mathf.Min(speed + speedAugmentationOverTime, 2);
        sampleTime += Time.deltaTime * speed;
        transform.position = curve.evaluate(sampleTime);
        transform.forward = curve.evaluate(sampleTime + 0.01f) - transform.position;

        if (sampleTime > 1)
        {
            LightningImpact();
            Destroy(curve.gameObject);
            Destroy(gameObject,0.2f);
        }
    }
    
    public void InitValues(QuadraticCurve _curve, float _intensity)
    {
        curve = _curve;
        intensity = _intensity;
        transform.localScale = intensity * 3f * Vector3.one;
    }

    private void OnTriggerEnter(Collider other)
    {
        LightningImpact();
    }

    private void LightningImpact()
    {
        //Create a circle around the lightning depending on the intensity
        //If the circle collides with an object with the CanBeDestroyed Layer, access the script and do damage
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2*intensity);
        foreach(Collider collider in colliders)
        {
            if(collider.gameObject.layer == LayerMask.NameToLayer("CanBeDestroyed"))
            {
                if (collider.GetComponent<ObjectToDestroy>() == null)
                {
                    Debug.LogWarning("Object" + collider.gameObject.name + " has no ObjectToDestroy script");
                    continue;
                }
                collider.GetComponent<ObjectToDestroy>().TakeDamage(Mathf.Clamp01(intensity)*100, true);
            }
        }

        Vibrator.Vibrate((long)(300 * intensity));
        CameraShaker.Instance.ShakeOnce(intensity * magnitudeMultiplier, roughness, fadeInTime, fadeOutTime);
        AudioManager.instance.PlaySoundEffect(SoundEffects.LightningImpact, Mathf.Clamp01(intensity));
        
        RaycastHit hit;
        GameObject impact;
        if (Physics.Raycast(transform.position, -transform.position, out hit, 100f, LayerMask.GetMask("Default")))
        {
            Debug.Log("Ray hit " + hit.collider.gameObject.name);
            impact = Instantiate(lightningImpactPrefab, hit.point, Quaternion.identity);
            impact.transform.up =  - transform.position;
        }
        else
        {
            impact = Instantiate(lightningImpactPrefab, transform.position, Quaternion.identity);
        }
        impact.transform.localScale = intensity * Vector3.one;
        Destroy(impact, 2f);
        Destroy(curve.gameObject);
        Destroy(gameObject);
    }
}
