using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveController : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;

    public Transform destination;
    // Start is called before the first frame update
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }
    public void MoveTo(Vector3 position)
    {
        _navMeshAgent.destination = position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
