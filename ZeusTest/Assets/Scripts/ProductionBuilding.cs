using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionBuilding : MonoBehaviour
{
    //Context context;

    [SerializeField] ResourceType resourceType;
    [SerializeField] float timeBetweenResources;
    float timeSinceLastResource;
    public Storage storage;



    // Start is called before the first frame update
    void Start()
    {
        timeSinceLastResource = 0f;
        //context = Context.instance;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastResource += Time.deltaTime;
        if (timeSinceLastResource > timeBetweenResources)
        {
            if (storage!= null) storage.AddResource(resourceType, 1);
            timeSinceLastResource = 0f;
        }
        
    }
}
