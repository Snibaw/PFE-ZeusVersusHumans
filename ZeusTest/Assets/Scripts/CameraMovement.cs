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
        Vector3 targetPosition = rotateAroundObject.position + directionToObject.normalized * distanceToKeep;
        StartCoroutine(MoveCameraSmoothly(targetPosition, 1.2f));
    }

    private IEnumerator MoveCameraSmoothly(Vector3 targetPosition, float moveTime)
    {
        float elapsedTime = 0f;
        Vector3 initialPosition = transform.position;

        while (elapsedTime < moveTime)
        {
            Vector3 currentPos = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / moveTime);
            transform.position = rotateAroundObject.position + (currentPos - rotateAroundObject.position).normalized * 16f;
            transform.LookAt(rotateAroundObject.position);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
        transform.LookAt(rotateAroundObject.position);
    }
}
