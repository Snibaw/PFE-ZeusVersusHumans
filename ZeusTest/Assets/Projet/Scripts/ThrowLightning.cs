using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrowLightning : MonoBehaviour
{
    public QuadraticCurve curve;
    [SerializeField] private GameObject lightningPrefab;
    [SerializeField] private MeshCollider planetCollider;
    private float planetRadius;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        planetRadius = planetCollider.bounds.size.x/2;
    }

    public void Throw(Vector3 direction, float magnitude, float time) // Magnitude = distance traveled in direction. 1/time = power of the bolt
    {
        FindFinalPointOnPlanet(direction, magnitude);


        GameObject lightning = Instantiate(lightningPrefab, mainCam.transform.position, Quaternion.identity);
        lightning.GetComponent<LightningBehaviour>().InitValues(curve, 1/(3*time));
        Destroy(lightning, 10f);

    }
    private void FindFinalPointOnPlanet(Vector3 direction, float magnitude)
    {
        //Make the direction relative to the rotation of the camera
        direction =  Quaternion.Euler(mainCam.transform.rotation.eulerAngles) * direction;
        curve.A.position = mainCam.transform.position;
        curve.B.position = direction.normalized  * planetRadius;
        curve.Control.position = (curve.A.position + curve.B.position) / 2;
        
    }
}


