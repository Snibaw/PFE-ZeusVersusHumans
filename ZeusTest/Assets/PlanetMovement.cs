using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetMovement : MonoBehaviour
{
    [SerializeField] private float rotationMultiplier = 5f;
    void Update()
    {
        //Rotate planet in x, y and z axis when mouse is dragged
        if(Input.GetMouseButton(0))
        {
            transform.Rotate(Input.GetAxis("Mouse Y") * rotationMultiplier, -Input.GetAxis("Mouse X") * rotationMultiplier, 0, Space.World);
        }
    }
}
