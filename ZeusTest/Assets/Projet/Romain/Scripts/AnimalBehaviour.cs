using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


public class AnimalBehaviour : MonoBehaviour
{
    [SerializeField] private float[] timeBorderBetweenMovements = new[] { 1f, 3f };
    [SerializeField] private float[] timeBorderInMovements = new[] { 3f, 5f };
    private float _timerBtwMovements = 0f;
    private float _timerInMovements = 0f;
    private bool isArrived = true;

    private void Start()
    {
        _timerBtwMovements = Random.Range(timeBorderBetweenMovements[0], timeBorderBetweenMovements[1]);
    }

    private void Update()
    {
        if(isArrived) _timerBtwMovements -= Time.deltaTime;
        
        if (_timerBtwMovements <= 0)
        {
            _timerBtwMovements = Random.Range(timeBorderBetweenMovements[0], timeBorderBetweenMovements[1]);
            MoveToRandomPosition();
        }

        Movement();
    }

    private void Movement()
    {
        if (_timerInMovements <= 0)
        {
            isArrived = true;
            return;
        }
        _timerInMovements -= Time.deltaTime;
        transform.Translate(Vector3.forward * Time.deltaTime);
    }

    private void MoveToRandomPosition()
    {
        transform.Rotate(0, Random.Range(0, 360), 0);
        _timerInMovements = Random.Range(timeBorderInMovements[0], timeBorderInMovements[1]);
        isArrived = false;
    }
}