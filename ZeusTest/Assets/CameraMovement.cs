using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform rotateAroundObject;
    [SerializeField] private float rotationSpeed = 5f;

    public void RotateAround(float xInput, float yInput)
    {
        transform.RotateAround(rotateAroundObject.position, Vector3.up, xInput * rotationSpeed * Time.deltaTime);
        transform.RotateAround(rotateAroundObject.position, transform.right, -yInput * rotationSpeed * Time.deltaTime);
    }
}
