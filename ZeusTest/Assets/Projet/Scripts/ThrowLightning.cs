using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrowLightning : MonoBehaviour
{
    [SerializeField] private GameObject lightningPrefab;
    [SerializeField] private MeshCollider planetCollider;
    [SerializeField] private GameObject AimCurve;
    private float planetRadius;
    private Camera mainCam;
    private QuadraticCurve curve;
    

    void Start()
    {
        mainCam = Camera.main;
        planetRadius = planetCollider.bounds.size.x/2;
    }

    public void Throw(Vector3 direction, float magnitude, float time) // Magnitude = distance traveled in direction. 1/time = power of the bolt
    {
        GameObject aimCurve = Instantiate(AimCurve, new Vector3(0,0,0), Quaternion.identity);
        curve = aimCurve.GetComponent<QuadraticCurve>();
        FindFinalPointOnPlanet(direction, magnitude);


        GameObject lightning = Instantiate(lightningPrefab, mainCam.transform.position, Quaternion.identity);
        lightning.GetComponent<LightningBehaviour>().InitValues(curve, 1/(3*time));
        Destroy(lightning, 10f);
        
        AdorationBar.instance.ChangeAdorationBarValue(AdorationBarEvents.ThrowLightning);

    }
    private void FindFinalPointOnPlanet(Vector3 direction, float magnitude)
    {
        //Make the direction relative to the rotation of the camera
        direction =  Quaternion.Euler(mainCam.transform.rotation.eulerAngles) * direction;
        
        float magnitudeMax = Screen.height;
        curve.A.position = mainCam.transform.position;
        Vector3 finalPosition = direction.normalized * planetRadius * magnitude * 3 / magnitudeMax;
        finalPosition -= new Vector3(0, planetRadius, 0); // Lower point = planet bottom
        curve.B.position = finalPosition;
        //Change control so its a curve
        curve.Control.position = curve.A.position + (curve.B.position - curve.A.position) / 2 + new Vector3(0, planetRadius/2, 0);
        
    }
}


