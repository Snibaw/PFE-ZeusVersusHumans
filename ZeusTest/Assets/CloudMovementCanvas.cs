using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloudMovementCanvas : MonoBehaviour
{
    private float length, startpos;
    [SerializeField] private float parallaxEffect;
    // Start is called before the first frame update
    void Start()
    {
        startpos = transform.position.x;
        length = GetComponent<Image>().sprite.bounds.size.x * 100;
        Debug.Log(length);
    }
    void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x + Time.deltaTime * parallaxEffect, transform.position.y, transform.position.z);
        if(transform.position.x > startpos + length) transform.position = new Vector3(startpos, transform.position.y, transform.position.z);
        if(transform.position.x < startpos - length) transform.position = new Vector3(startpos, transform.position.y, transform.position.z);
    }
}
