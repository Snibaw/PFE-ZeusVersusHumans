using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveController : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private NPCStats stats;
    private float startSpeed;
    private float exhaustedSpeed;
    private float lastEnergy = 0;

    // public Transform destination;
    // Start is called before the first frame update
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        stats = GetComponent<NPCStats>();
        startSpeed = _navMeshAgent.speed;
        exhaustedSpeed = startSpeed / 2;
    }
    public void MoveTo(Vector3 position)
    {
        _navMeshAgent.destination = position;
    }

    public void StopMoving()
    {
        _navMeshAgent.destination = this.transform.position;
    }

    public void SetSpeed(bool isExhausted)
    {
        if (isExhausted)
            _navMeshAgent.speed = exhaustedSpeed;
        else
            _navMeshAgent.speed = startSpeed;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
