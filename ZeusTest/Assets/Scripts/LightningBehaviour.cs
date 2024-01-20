using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class LightningBehaviour : MonoBehaviour
{
    public QuadraticCurve curve;
    public float speed = 0.1f;
    [SerializeField] private float speedAugmentationOverTime = 0.01f;
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
            Destroy(curve.gameObject);
            Destroy(gameObject);
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
        //Create a circle around the lightning depending on the intensity
        //If the circle collides with an object with the CanBeDestroyed Layer, access the script and do damage

        bool needToBeDestroyed = (other.CompareTag("Ground") || other.CompareTag("Water"));
        
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
                needToBeDestroyed = true;
            }
        }


        if (needToBeDestroyed)
        {
            Vibrator.Vibrate((long)(300 * intensity));
            CameraShaker.Instance.ShakeOnce(intensity * magnitudeMultiplier, roughness, fadeInTime, fadeOutTime);
            AudioManager.instance.PlaySoundEffect(SoundEffects.LightningImpact, Mathf.Clamp01(intensity));
            Destroy(curve.gameObject);
            Destroy(gameObject);
        }
    }
    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, intensity);
    }
}
