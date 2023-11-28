using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class TestMovement : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    [SerializeField] private Transform _guide;

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        _navMeshAgent.SetDestination(_guide.position);
    }
}
