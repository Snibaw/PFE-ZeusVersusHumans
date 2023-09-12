using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowLightning : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private GameObject explosionPrefab;

    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
