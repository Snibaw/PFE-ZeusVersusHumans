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
    public bool isMarked = false; // Is someone going to harvest this resource ?
    [SerializeField] private ResourceType resourceType;
    [SerializeField] private GameObject SpawnedModel;
    [SerializeField] private GameObject HarvestedModel;
    [SerializeField] private float timeBeforeRespawn = 5f;
    private Animator _animator;

    [Header("Chose Model Randomly")] [SerializeField]
    private GameObject[] possibleModel;
    
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
    // public delegate void ResourceExhausted();
    // public event ResourceExhausted OnResourceExhausted;

    // Start is called before the first frame update
    void Start()
    {
        ChoseModelRandomly();
        ShowModelDependingOnHarvestedState();

        if (resourceType == ResourceType.wood)
        {
            _animator = GetComponentInChildren<Animator>();
            InvokeRepeating("MakeTreeMove", Random.Range(5f, 10f), Random.Range(10f, 15f));
        }

    }

    void MakeTreeMove()
    {
        _animator.SetTrigger("Move");
    }
    private void ChoseModelRandomly()
    {
        if (possibleModel.Length > 0)
        {
            int random = Random.Range(0, possibleModel.Length);
            foreach (var model in possibleModel)
            {
                model.SetActive(false);
            }
            possibleModel[random].SetActive(true);
        }
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
        isMarked = false;
        ShowModelDependingOnHarvestedState();
    }
    public void MarkThisResource()
    {
        isMarked = true;
        StartCoroutine(WaitBeforeUnmarking(5f));
    }
    private IEnumerator WaitBeforeUnmarking(float time)
    {
        yield return new WaitForSeconds(time);
        isMarked = false;
    }
}