using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanAIBehaviour : MonoBehaviour
{
    [SerializeField] private float speed;
    [Header("For Debug")]
    [SerializeField] private IAConstruction currentConstructionObjective;
    [SerializeField] private TypeOfResources resourceObjective;
    [SerializeField] private GameObject nearestResource; 
    [SerializeField] private List<GameObject> obstaclesToBuild;
    private bool canFindRessource = false;
    private NavMeshAgent _navMeshAgent;
   
    // Start is called before the first frame update
    void Start()
    {
        //Let the time for resources to spawn
        Invoke("FindAnObjective", 1f);
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update() 
    {
        if(canFindRessource)
        {
            if(nearestResource != null)
            {
                MoveTo(nearestResource.transform.position);
            }
            else //Object was destroyed, find a new objective
            {
                FindAnObjective();
            }
        }

        transform.LookAt(GameManager.instance.planet.transform.position );
        transform.Rotate(new Vector3(90,0,0));

    }
    private void FindAnObjective()
    {   
        // Later, we will use a currentConstructionObjective on each human only if they have different tasks.
        currentConstructionObjective = IAConstructionManager.instance.ChooseAConstructionObjective();

        if(currentConstructionObjective == null) return;

        if (obstaclesToBuild.Count > 0)
        {
                nearestResource = obstaclesToBuild[0];
                obstaclesToBuild.RemoveAt(0);
                canFindRessource = true;
                return;
        }

        if(IAConstructionManager.instance.CanBuild(currentConstructionObjective))
        {
            IAConstructionManager.instance.SetConstructionPosition(transform.position);
            obstaclesToBuild = IAConstructionManager.instance.GetAllBuildObstacles(transform.position);
            Debug.Log(obstaclesToBuild.Count);
            if (obstaclesToBuild.Count == 0)
            {
                IAConstructionManager.instance.BuildConstruction(currentConstructionObjective, transform.position);
                canFindRessource = false;
                currentConstructionObjective = null;
                StartCoroutine("WaitBeforeFindingAnObjective");
            }
            else
            {
                nearestResource = obstaclesToBuild[0];
                obstaclesToBuild.RemoveAt(0);
                canFindRessource = true;
            }
        }
        else
        {
            resourceObjective = IAConstructionManager.instance.GetAResourceObjective(currentConstructionObjective);
            nearestResource = FindNearestResource(resourceObjective);
            canFindRessource = true;
        }
    }

    private GameObject FindNearestResource(TypeOfResources resource)
    {
        GameObject[] resources = GameObject.FindGameObjectsWithTag(resource.ToString());
        GameObject nearestResourceTemp = null;
        float nearestDistance = Mathf.Infinity;
        foreach(GameObject resourceObject in resources)
        { //Distance doesn't take in account the fact that its a sphere
            float distance = Vector3.Distance(transform.position, resourceObject.transform.position);
            if(distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestResourceTemp = resourceObject;
            }
        }
        return nearestResourceTemp;
    }
    private void MoveTo(Vector3 position)
    {
        //transform.position = Vector3.MoveTowards(transform.position, position, speed * Time.deltaTime);
        _navMeshAgent.SetDestination(position);

        if (Vector3.Distance(transform.position,position) < 0.1f) // Change this to a distance check or collider check with the resource
        {
            CollectResource();
        }

    }
    IEnumerator WaitBeforeFindingAnObjective()
    {
        yield return new WaitForSeconds(1f);
        FindAnObjective();
    }
    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject == nearestResource)
        {
            CollectResource();
        }
            
    }
    private void CollectResource()
    {
        
        // This would be a good idea, add a component so the resource do all the work about addind 1 to IAResourceManager ... but for now, I do simple
        // nearestResource.GetComponent<Resource>().TakeResource();
        IAResourceManager.instance.ChangeResourceNumber(resourceObjective, 1);
        Destroy(nearestResource);
        nearestResource = null;
        canFindRessource = false;
        // Add a delay before finding a new objective
        StartCoroutine("WaitBeforeFindingAnObjective");
    }
}
