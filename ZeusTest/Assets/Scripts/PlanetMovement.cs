using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetMovement : MonoBehaviour
{
    [SerializeField] private float rotationMultiplier = 5f; // 5 for PC, 0.2 for Android
    public void rotate(float x, float y)
    {
        transform.Rotate(y * rotationMultiplier, -x * rotationMultiplier, 0, Space.World);
    }
}
