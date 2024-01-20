using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class ThrowLightning : MonoBehaviour
{
    [SerializeField] private Vector3 spawnOffset;
    [SerializeField] private GameObject lightningPrefab;
    [SerializeField] private MeshCollider planetCollider;
    [SerializeField] private GameObject AimCurve;
    private float planetRadius;
    private Camera mainCam;
    private QuadraticCurve curve;
    [SerializeField] private GameObject targetPrefab;
    private GameObject _target;
    
    [Header("Throw Lightning Cooldown")]
    [SerializeField] private float cooldownBtwLightning = 3f;
    private float _currentCooldown = 0f;

    [SerializeField] private GameObject[] lightningUI;
    private int _numberOfLightning;
    

    void Start()
    {
        mainCam = Camera.main;
        planetRadius = planetCollider.bounds.size.x/2;
        
        _currentCooldown = cooldownBtwLightning;
        _numberOfLightning = lightningUI.Length;
        for (int i = 0; i < _numberOfLightning; i++)
        {
            lightningUI[i].SetActive(true);
        }
    }

    private void Update()
    {
        if (_numberOfLightning < lightningUI.Length)
        {
            _currentCooldown -= Time.deltaTime;
            if (_currentCooldown <= 0)
            {
                _numberOfLightning++;
                _currentCooldown = cooldownBtwLightning;
                lightningUI[_numberOfLightning-1].SetActive(true);
            }
        }
        
        
    }

    public void Throw(Vector3 direction, float magnitude, float intensity) // Magnitude = distance traveled in direction. 1/time = power of the bolt
    {
        if (_numberOfLightning <= 0) return;
        
        if (ReferenceEquals(curve, null))
        {
            GameObject aimCurve = Instantiate(AimCurve, new Vector3(0,0,0), Quaternion.identity);
            curve = aimCurve.GetComponent<QuadraticCurve>();
        }

        FindFinalPointOnPlanet(direction, magnitude, intensity);


        GameObject lightning = Instantiate(lightningPrefab, mainCam.transform.position, Quaternion.identity);
        lightning.GetComponent<LightningBehaviour>().InitValues(curve, intensity);
        Destroy(lightning, 10f);
        //Cooldown
        _numberOfLightning--;
        lightningUI[_numberOfLightning].SetActive(false);

        AudioManager.instance.PlaySoundEffect(SoundEffects.LightningThrow);

        Destroy(_target);
        _target = null;
        curve = null;
    }

    public void FindFinalPointOnPlanet(Vector3 direction, float magnitude, float intensity)
    {
        if (_numberOfLightning <= 0) return;
        
        if (ReferenceEquals(curve, null))
        {
            GameObject aimCurve = Instantiate(AimCurve, new Vector3(0,0,0), Quaternion.identity);
            curve = aimCurve.GetComponent<QuadraticCurve>();
        }
         //Make the direction relative to the rotation of the camera
         direction =  Quaternion.Euler(mainCam.transform.rotation.eulerAngles) * direction;
         
         Vector3 changeSpawnOffset = new Vector3(spawnOffset.x * UnityEngine.Random.Range(-1, 1), spawnOffset.y, spawnOffset.z);
         Vector3 rotatedSpawnOffset = mainCam.transform.rotation * changeSpawnOffset;
         curve.A.position = mainCam.transform.position + rotatedSpawnOffset;
         float magnitudeMax = Screen.height;
         Vector3 finalPosition = direction.normalized * planetRadius * (magnitude / magnitudeMax) * 3;
         finalPosition -= Quaternion.Euler(mainCam.transform.rotation.eulerAngles) * new Vector3(0, planetRadius, 0);
         curve.B.position = finalPosition;
         //Create a raycast from camera to curve.B.position
         RaycastHit hit;
         if (Physics.Raycast(mainCam.transform.position, curve.B.position - mainCam.transform.position, out hit, Mathf.Infinity))
         {
             curve.B.position = hit.point;
             curve.B.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
         }
         //Change control so its a curve
         curve.Control.position = curve.A.position + (curve.B.position - curve.A.position) / 2 + Quaternion.Euler(mainCam.transform.rotation.eulerAngles) * new Vector3(direction.x / 2, planetRadius / 2, 0);
         
         MoveTarget(intensity);

    }

    private void MoveTarget(float intensity)
    {

        if (ReferenceEquals(_target, null))
        {
            _target = Instantiate(targetPrefab, curve.B.position, curve.B.rotation);
        }
        else
        {
            _target.transform.position = curve.B.position;
            _target.transform.rotation = curve.B.rotation;
        }
        _target.transform.localScale = new Vector3(intensity, 0.1f, intensity);
    }
}


