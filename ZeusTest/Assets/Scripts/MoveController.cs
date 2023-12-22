using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveController : MonoBehaviour
{
    //private NavMeshAgent _navMeshAgent;
    protected NPCStats stats;
    [SerializeField] protected float startSpeed;
    protected float exhaustedSpeed;
    protected float lastEnergy = 0;

    protected float currentSpeed;

    protected Coroutine _followPath;
    protected bool isMoving = false;

    private Vector3 _positionLastFrame;
    [SerializeField] private GameObject _boat;

    // public Transform destination;
    // Start is called before the first frame update
    protected void Start()
    {

        _followPath = null;
        stats = GetComponent<NPCStats>();
        exhaustedSpeed = startSpeed / 2;
        currentSpeed = startSpeed;
        _positionLastFrame = transform.position;
    }

   
    public void MoveTo(Vector3 position, bool canMoveOnWater)
    {
        
        
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

    public void SetSpeed(bool isExhausted)
    {
        if (isExhausted)
            currentSpeed = exhaustedSpeed;
        else
            currentSpeed = startSpeed;
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
