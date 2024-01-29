using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveController : MonoBehaviour
{
    protected NPCStats stats;
    [SerializeField] protected float startSpeed;
    protected float exhaustedSpeed;
    protected float lastEnergy = 0;

    protected float currentSpeed;

    protected Coroutine _followPath;
    protected bool isMoving = false;

    private Vector3 _positionLastFrame;
    [SerializeField] private GameObject _boat;
    protected void Start()
    {

        _followPath = null;
        //stats = GetComponent<NPCStats>();

        if(!TryGetComponent<NPCStats>(out stats))stats = null;

        exhaustedSpeed = startSpeed / 2;
        currentSpeed = startSpeed;
        _positionLastFrame = transform.position;
    }

    public void AdaptSpeedToEnergy(float energy)
    {
        currentSpeed = startSpeed * (0.5f + (energy / 200)); // Energy between 0 and 100 => 0 to 0.5 => * btw 0.5 and 1
    }

    public void MoveTo(Vector3 position, bool canMoveOnWater)
    {
        if(stats != null) if(stats.energy <= 0) return;
        if (isMoving) return;

        isMoving = true;
        StartCoroutine(FollowPath(PointDistribution.instance.CalculatePath(
            PointDistribution.instance.FindTheClosestGraphNode(transform.position), 
            PointDistribution.instance.FindTheClosestGraphNode(position), canMoveOnWater)));
        
    }

    public void StopMoving()
    {
        if (_followPath != null) StopCoroutine(_followPath);
    }

    public IEnumerator FollowPath(List<Vector3> path)
    {
        float distanceRequire = 0.25f;
        int index = 0;
        if(path != null)
        {
            while (Vector3.Distance(transform.position, path[path.Count - 1]) > distanceRequire)
            {
                if (Vector3.Distance(transform.position, path[index]) <= distanceRequire)
                {
                    index++;
                }
                else
                {
                    Vector3 direction = (path[index] - transform.position).normalized;
                    transform.rotation = Quaternion.FromToRotation(Vector3.up, transform.position);
                    Vector3 directionToAdd = direction * currentSpeed * Time.deltaTime;
                    transform.LookAt(transform.position - direction, transform.up);
                    transform.position += directionToAdd;
                    if (PointDistribution.instance.FindTheClosestGraphNode(transform.position).IsWater)
                    {
                        if(_boat != null) _boat.SetActive(true);
                    }
                    else
                    {
                        if (_boat != null) _boat.SetActive(false);
                    }
                    
                }
                yield return new WaitForEndOfFrame();
            }
        }
        
        isMoving = false;
    }
}
