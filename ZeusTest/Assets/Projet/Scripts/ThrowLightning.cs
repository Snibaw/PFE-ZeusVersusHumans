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
        float tempoMagnitude = (magnitude/Screen.height)*2;
        tempoMagnitude = direction.y < 0 ? Mathf.Clamp01(tempoMagnitude) : Mathf.Clamp01(tempoMagnitude*2.5f); // Difficult to hit the bottom of the planet

        // When we rotate the camera we need to rotate the direction too
        Vector3 projectedDirection = Vector3.ProjectOnPlane(direction, mainCam.transform.forward).normalized;

        Vector3 pointToAim = projectedDirection * tempoMagnitude * planetRadius;
        pointToAim = KeepPointOnScreen(pointToAim);

        Vector3 pointOnPlanet = Vector3.zero;
        //Draw Raycast from camera to this point
        RaycastHit hit;
        if(Physics.Raycast(mainCam.transform.position, pointToAim - mainCam.transform.position, out hit, 1000f))
        {
            if(hit.collider.gameObject == planetCollider.gameObject)
            {
                pointOnPlanet = hit.point;
            }
        }
        if(pointOnPlanet == Vector3.zero)
            return; // TODO: If we don't find a point on the planet, we don't throw the lightning or find another point

        //If we find a point on the planet, we draw a curve from the camera to this point
        curve.A.position = mainCam.transform.position;
        curve.B.position = pointOnPlanet;
        curve.Control.position = (curve.A.position + curve.B.position)/2 + projectedDirection * 3;
    }
    private Vector3 KeepPointOnScreen(Vector3 point)
    {
        Vector3 screenPoint = mainCam.WorldToViewportPoint(point);
        screenPoint.x = Mathf.Clamp01(screenPoint.x);
        screenPoint.y = Mathf.Clamp01(screenPoint.y);
        screenPoint.z = Mathf.Clamp01(screenPoint.z);
        return mainCam.ViewportToWorldPoint(screenPoint);
    }
}


