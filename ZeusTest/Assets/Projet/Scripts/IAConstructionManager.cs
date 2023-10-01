using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IAConstructionManager : MonoBehaviour
{
    [SerializeField] private IAConstruction[] constructions;
    public IAConstruction currentConstructionObjective;
    [SerializeField] private TMP_Text constructionText;
    [SerializeField] private SpawnResources spawnResources;
    public static IAConstructionManager instance;

    private void Awake() {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
    }
    public bool CanBuild(IAConstruction buildObjective)
    {
        foreach(TypeOfResources resource in System.Enum.GetValues(typeof(TypeOfResources)))
        {
            if(buildObjective.GetResourceNeeded(resource) > IAResourceManager.instance.GetResourceNumber(resource))
            {
                return false;
            }
        }
        return true;
    }
    public void BuildConstruction(IAConstruction construction, Vector3 IAposition)
    {
        // For now : Build the construction where the IA is
        // Later : Build the construction where in an empty space near the IA
        GameObject objSpawned = Instantiate(construction.prefab, IAposition, Quaternion.identity);
        spawnResources.SetRotationAndParent(objSpawned);

        ConstructionWasBuilt(construction);        
    }
    private void ConstructionWasBuilt(IAConstruction construction)
    {
        // Reduce the number of resources depending on the resources needed
        foreach(TypeOfResources resource in System.Enum.GetValues(typeof(TypeOfResources)))
        {
            IAResourceManager.instance.ChangeResourceNumber(resource, -construction.GetResourceNeeded(resource));
        }
    }
    public TypeOfResources GetAResourceObjective(IAConstruction buildObjective)
    {
        int maxResourcesNeeded = -100000;
        TypeOfResources resourceMostNeeded = TypeOfResources.wood;
        //Browse threw every resources possibles
        foreach(TypeOfResources resource in System.Enum.GetValues(typeof(TypeOfResources)))
        {
            //If we find a resource more needed than the previous one, we change the resourceMostNeeded
            int resourceNeeded = buildObjective.GetResourceNeeded(resource) - IAResourceManager.instance.GetResourceNumber(resource);
            if(resourceNeeded > maxResourcesNeeded)
            {
                maxResourcesNeeded = resourceNeeded;
                resourceMostNeeded = resource;
            }
        }
        Debug.Log("Resource most needed:" + resourceMostNeeded);
        return resourceMostNeeded;
    }

    public IAConstruction ChooseAConstructionObjective() // TODO : Choose a construction depending on the priority, the resources, the type of human (religious or army...
    {
        foreach(IAConstruction construction in constructions)
        {
            if(HasReachedMaxNumber(construction) == false) // Check every construction one by one, not a good way to find the best objective but it's simple
            {
                currentConstructionObjective = construction;
                #if UNITY_EDITOR
                UpdateConstructionText();
                #endif
                return construction;
            }
        }
        Debug.Log("No construction objective found");
        return null;
    }
    private bool HasReachedMaxNumber(IAConstruction construction)
    {
        string targetTag = construction.prefab.tag;
        Debug.Log("targetTag:" + targetTag);
        GameObject[] constructions = GameObject.FindGameObjectsWithTag(targetTag);
        Debug.Log("constructions.Length:" + constructions.Length);
        if(constructions.Length >= construction.numberMaxToSpawn)
        {
            return true;
        }
        return false;
    }
    private void UpdateConstructionText()
    {
        constructionText.text = "Construction: " + currentConstructionObjective.name + "\n" + "woodCost:" + currentConstructionObjective.woodCost + "\n" + "stoneCost:" + currentConstructionObjective.stoneCost;
    }
}
