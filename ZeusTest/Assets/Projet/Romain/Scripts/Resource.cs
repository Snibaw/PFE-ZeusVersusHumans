using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum ResourceType
{
    wood,
    stone,
    metal
}

public class Resource : MonoBehaviour
{
    public bool canBeHarvested = true;
    [SerializeField] private ResourceType resourceType;
    [SerializeField] private GameObject SpawnedModel;
    [SerializeField] private GameObject HarvestedModel;
    [SerializeField] private float timeBeforeRespawn = 5f;
    
    public ResourceType ResourceType
    {
        get
        {
            return resourceType;
        }
        set
        {
            resourceType = value;
        }
    }
    public delegate void ResourceExhausted();
    public event ResourceExhausted OnResourceExhausted;

    // Start is called before the first frame update
    void Start()
    {
        ShowModelDependingOnHarvestedState();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void ShowModelDependingOnHarvestedState()
    {
        SpawnedModel.SetActive(canBeHarvested);
        HarvestedModel.SetActive(!canBeHarvested);
    }
    
    public void HasBeenHarvested()
    {
        canBeHarvested = false;
        ShowModelDependingOnHarvestedState();
        StartCoroutine(WaitBeforeRespawning());
    }

    private IEnumerator WaitBeforeRespawning()
    {
        yield return new WaitForSeconds(timeBeforeRespawn);
        canBeHarvested = true;
        ShowModelDependingOnHarvestedState();
    }
}