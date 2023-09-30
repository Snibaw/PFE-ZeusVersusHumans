using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanAIBehaviour : MonoBehaviour
{
    [SerializeField] private float speed;
    [Header("For Debug")]
    [SerializeField] private IAConstruction currentConstructionObjective;
    [SerializeField] private TypeOfResources resourceObjective;
    [SerializeField] private GameObject nearestResource;
    // Start is called before the first frame update
    void Start()
    {
        //Let the time for resources to spawn
        Invoke("FindAnObjective", 1f);
    }
    private void Update() 
    {
        if(nearestResource != null)
        {
            MoveTo(nearestResource.transform.position);
        }
    }
    private void FindAnObjective()
    {   
        currentConstructionObjective = IAConstructionManager.instance.GetAConstructionObjective();
        resourceObjective = IAConstructionManager.instance.GetAResourceObjective();
        nearestResource = FindNearestResource(resourceObjective);
        Debug.Log("Nearest resource:" + nearestResource);
    }
    private GameObject FindNearestResource(TypeOfResources resource)
    {
        GameObject[] resources = GameObject.FindGameObjectsWithTag(resource.ToString());
        Debug.Log("resource:" + resource.ToString());
        Debug.Log("Resources:" + resources.Length);
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
        transform.position = Vector3.MoveTowards(transform.position, position, speed * Time.deltaTime);
        
        if(transform.position == position) // Change this to a distance check or collider check with the resource
        {
            Debug.Log("Resource reached " + nearestResource.name);
            // This would be a good idea, add a component so the resource do all the work about addind 1 to IAResourceManager ... but for now, I do simple
            // nearestResource.GetComponent<Resource>().TakeResource();
            IAResourceManager.instance.ChangeResourceNumber(resourceObjective, 1);
            Destroy(nearestResource);
            nearestResource = null;
            // Add a delay before finding a new objective
            StartCoroutine("WaitBeforeFindingAnObjective");
        }

    }
    IEnumerator WaitBeforeFindingAnObjective()
    {
        yield return new WaitForSeconds(1f);
        FindAnObjective();
    }
}
