using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Context : MonoBehaviour
{       
    public Storage storage;
    public GameObject home;
    public string resourceTag = "resource";
    public float MinDistance = 5f;
    public int energyLostPerAction = 5;
    public Dictionary<DestinationType, List<Transform>> Destinations { get; private set; }

    public static Context instance;

    private void Start()
    {
        List<Transform> restDestinations = new List<Transform>() { home.transform };
        List<Transform> storageDestinations = new List<Transform>() { storage.transform };
        List<Transform> resourceDestinations = GetAllResources();

        instance = this;

        Destinations = new Dictionary<DestinationType, List<Transform>>()
        {
            { DestinationType.rest, restDestinations},
            { DestinationType.storage, storageDestinations },
            { DestinationType.resource, resourceDestinations }
        };
    }

    private List<Transform> GetAllResources()
    {
        Transform[] gameObjects = FindObjectsOfType<Transform>() as Transform[];
        List<Transform> resources = new List<Transform>();
        foreach (Transform go in gameObjects)
        {
            if(go.gameObject.tag == resourceTag)
            {
                resources.Add(go);
            }
        }
        return resources;
    }

}
