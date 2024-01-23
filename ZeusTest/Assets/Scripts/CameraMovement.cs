using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform rotateAroundObject;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] float distanceToKeep = 16f;

    public void RotateAround(float xInput, float yInput)
    {
        transform.RotateAround(rotateAroundObject.position, transform.up, xInput * rotationSpeed * Time.deltaTime);
        transform.RotateAround(rotateAroundObject.position, transform.right, -yInput * rotationSpeed * Time.deltaTime);
    }
    public void MoveToObject(GameObject obj)
    {
        Vector3 directionToObject = obj.transform.position - rotateAroundObject.position;
        Vector3 newPosition = rotateAroundObject.position + directionToObject.normalized * distanceToKeep;
        transform.position = newPosition;
        transform.LookAt(obj.transform);
    }
}
