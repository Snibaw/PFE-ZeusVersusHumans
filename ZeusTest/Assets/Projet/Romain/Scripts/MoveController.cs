using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveController : MonoBehaviour
{
    //private NavMeshAgent _navMeshAgent;
    private NPCStats stats;
    [SerializeField] private float startSpeed;
    private float exhaustedSpeed;
    private float lastEnergy = 0;

    private float currentSpeed;

    private Coroutine _followPath;
    private bool isMoving = false;

    // public Transform destination;
    // Start is called before the first frame update
    void Start()
    {

        _followPath = null;
        stats = GetComponent<NPCStats>();
        exhaustedSpeed = startSpeed / 2;
        currentSpeed = startSpeed;
    }
    public void MoveTo(Vector3 position)
    {
        transform.rotation = Quaternion.FromToRotation(Vector3.up, transform.position - Vector3.zero);
        
        if (isMoving) return;

        isMoving = true;
        StartCoroutine(FollowPath(PointDistribution.instance.CalculatePath(
            PointDistribution.instance.FindTheClosestGraphNode(transform.position), 
            PointDistribution.instance.FindTheClosestGraphNode(position))));
        
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

    // Update is called once per frame
    void Update()
    {
    }


    public IEnumerator FollowPath(List<Vector3> path)
    {
        //transform.position = path[0];
        float distanceRequire = 0.25f;
        int index = 0;

        while (Vector3.Distance(transform.position, path[path.Count - 1]) > distanceRequire)
        {
            if (Vector3.Distance(transform.position, path[index]) <= distanceRequire)
            {
                index++;
            }
            else
            {
                Vector3 direction = (path[index] - transform.position).normalized;
                transform.position += direction * currentSpeed * Time.deltaTime;
            }
            yield return new WaitForEndOfFrame();
        }
        isMoving = false;
    }
}
