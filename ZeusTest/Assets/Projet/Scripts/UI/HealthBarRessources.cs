using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarRessources : MonoBehaviour
{
    private Camera mainCam;
    void Start()
    {
        gameObject.SetActive(false);
        mainCam = Camera.main;
        LookTowardsCamera();
    }

    private void LookTowardsCamera()
    {
        var rotation = mainCam.transform.rotation;
        transform.LookAt(transform.position + rotation * Vector3.back);
    }

    void Update()
    {
        LookTowardsCamera();
    }
}
