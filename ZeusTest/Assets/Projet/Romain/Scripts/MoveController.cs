using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveController : MonoBehaviour
{
    private NPCStats stats;
    private float startSpeed;
    private float exhaustedSpeed;
    private float lastEnergy = 0;

    // public Transform destination;
    // Start is called before the first frame update
    void Start()
    {
        stats = GetComponent<NPCStats>();
        // startSpeed = _navMeshAgent.speed;
        exhaustedSpeed = startSpeed / 2;
    }
    public void MoveTo(Vector3 position)
    {
        // TODO : Louan :)
    }

    public void StopMoving()
    {
        // TODO : Louan :)
    }

    public void SetSpeed(bool isExhausted)
    {
        // TODO : Louan :)
        // if (isExhausted)
        // else
    }

    // Update is called once per frame
    void Update()
    {
    }
}
