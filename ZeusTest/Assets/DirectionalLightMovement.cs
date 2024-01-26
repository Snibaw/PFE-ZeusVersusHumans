using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalLightMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 1;

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(Vector3.zero, Vector3.right, _speed * Time.deltaTime);
    }
}
