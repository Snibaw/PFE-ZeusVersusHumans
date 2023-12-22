using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    decide,
    move,
    execute
}
public class NPCController : MonoBehaviour
{
    public MoveController mover { get; set; }
    public AIBrain aiBrain { get; set; }
    public NPCInventory Inventory { get; set; }
    public NPCStats stats { get; set; }
    [HideInInspector] public Context context;
    public BuildManager buildManager { get; set; }
    public UpgradeManager upgradeManager { get; set; }
    public State currentState { get; set; }
    public IAConstruction constructionToBuild { get; set; }
    public Building buildingToUpgrade {get; set; }
    public IAConstruction constructionToUpgrade {get; set; }
    public GameObject buildingToBuild { get; set; }
    private bool isSleeping = false;

    [SerializeField] private UI_Timer uiTimerScript;
    [SerializeField] private ThoughtsAndActionManager thoughtsScript;
    [SerializeField] private bool _canMoveOnWater;
    PointDistribution _pointDistribution;

    

    public bool isExecuting;
    // Start is called before the first frame update
    void Start()
    {
        mover = GetComponent<MoveController>();
        aiBrain = GetComponent<AIBrain>();
        Inventory = GetComponent<NPCInventory>();
        stats = GetComponent<NPCStats>();
        context = GameManager.instance.context;
        buildManager = context.storage.gameObject.GetComponent<BuildManager>();
        upgradeManager = context.storage.gameObject.GetComponent<UpgradeManager>();
        _pointDistribution = GameObject.FindWithTag("Planet").GetComponent<PointDistribution>();
        isExecuting = false;
        constructionToBuild = null;
        _canMoveOnWater = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        stats.UpdateEnergy(isSleeping);
        // stats.UpdateHunger();
        FSMTick();
    }

    

    public void FSMTick()
    {
        float MinDistanceModifier = constructionToBuild == null ? 1f : 0.5f;
        switch (currentState)
        {
            case State.decide:
                aiBrain.DecideBestAction();
                MinDistanceModifier = constructionToBuild == null ? 1f : 0.5f;
                if (Vector3.Distance(aiBrain.bestAction.RequiredDestination.position, this.transform.position) < context.MinDistance * MinDistanceModifier)
                {
                    currentState = State.execute;
                }
                else
                {
                    currentState = State.move;
                }
                break;
            
            case State.move:
                if (Vector3.Distance(aiBrain.bestAction.RequiredDestination.position, this.transform.position) < context.MinDistance * MinDistanceModifier)
                {
                    mover.StopMoving();
                    currentState = State.execute;
                }
                else
                {
                    mover.MoveTo(aiBrain.bestAction.RequiredDestination.position, _canMoveOnWater);
                }    
                
                break;
            
            case State.execute:
                if (!isExecuting)
                {
                    aiBrain.bestAction.Execute(this);
                    isExecuting = true;
                }
                else
                {
                    if (aiBrain.finishedExecutingBestAction)
                    {
                        currentState = State.decide;
                        isExecuting = false;
                    }
                }
                break;
            
        }
    }
    public void OnFinishedAction()
    {
        aiBrain.DecideBestAction();
    }
    public void DoAction(string action, float time)
    {
        Debug.Log("Doing : " + action);
        StartCoroutine(ExecuteAction(action, time));
    }
    private IEnumerator ExecuteAction(string action, float time)
    {

        //Some exceptions are managed here
        
        //Lose energy only when not sleeping
        isSleeping = action == "Sleep";
        if (!isSleeping)
        {
            stats.energy -= context.energyLostPerAction;
            if(stats.energy == 0) time = time*2;
        }
        //If the resource being harvested is not harvestable anymore, we stop the action
        if(action == "Work" && !aiBrain.bestAction.RequiredDestination.GetComponent<Resource>().canBeHarvested)
        {
            aiBrain.finishedExecutingBestAction = true;
            yield break;
        }
        uiTimerScript.StartTimer(time);
        yield return new WaitForSeconds(time);
        
        switch (action)
        {
            case "Work":
                Resource resource = null;
                if (aiBrain.bestAction.RequiredDestination != null) {
                    resource = aiBrain.bestAction.RequiredDestination.GetComponent<Resource>();
                    //Change Action in Timer / Thought bubble
                    SetAction(resource.ResourceType);
                    thoughtsScript.ActivateThoughts(true);
                }
                
                if (resource != null && resource.canBeHarvested)
                {
                    Inventory.AddResource(resource.ResourceType, 1);
                    resource.HasBeenHarvested();
                }
                break;
            case "Sleep":
                thoughtsScript.ChangeAction(ActionOfIA.Sleep);
                thoughtsScript.ActivateThoughts(false);
                
                stats.energy += 100;
                break;
            // case "Eat":
            //     stats.hunger -= 30;
            //     break;
            case "DropOffResource":
                thoughtsScript.ChangeAction(ActionOfIA.Home);
                thoughtsScript.ActivateThoughts(false);
                
                context.storage.GetAllResourcesFromNPC(Inventory);
                stats.resource = 0;
                break;
            case "FetchResource":
                // TO do later if buildings needs a lot of resources.
                break;
            case "Build":
                ExecuteBuild();
                AdorationBar.instance.ChangeAdorationBarValue(AdorationBarEvents.ConstructBuilding);
                break;
            case "Upgrade":
                ExecuteUpgrade();
                break;
        }
        aiBrain.finishedExecutingBestAction = true;
    }

    private void ExecuteBuild()
    {
        buildingToBuild.SetActive(true);

        Destroy(aiBrain.bestAction.RequiredDestination.gameObject);
        //Tell the context that a new house has been built
        if(buildingToBuild.GetComponent<Building>().BuildingType == BuildingType.house)
            context.AddDestinationTypeBuild(DestinationType.rest, buildingToBuild.transform);
        //Tell the build manager that a new construction has been built
        buildManager.AddConstructionBuilt(buildingToBuild.GetComponent<Building>().BuildingType);
        //Tell the upgrade manager that a new construction has been built
        upgradeManager.AddBuildingBuilt(buildingToBuild.GetComponent<Building>());
        //delete resources from the inventory
        foreach (ResourceType r in ResourceType.GetValues(typeof(ResourceType)))
        {
            Inventory.RemoveResource(r, constructionToBuild.GetResourceNeeded(r,0));
        }
        constructionToBuild = null;
        buildingToBuild = null;
    }
    private void ExecuteUpgrade()
    {
        //delete resources from the inventory
        foreach (ResourceType r in ResourceType.GetValues(typeof(ResourceType)))
        {
            context.storage.RemoveResource(r, constructionToUpgrade.GetResourceNeeded(r,buildingToUpgrade.level));
        }

        //raise the level of the building
        buildingToUpgrade.levelUp();

        constructionToUpgrade = null;
        buildingToUpgrade = null;
    }
    public float GetPossibleBuildScore()
    {
        float score = buildManager.HowManyConstructionCanBeBuilt(this, Inventory);
        return Mathf.Clamp01(score);
    }

    public float GetHypotheticalBuildScore()
    {
        float score = buildManager.HowManyConstructionCanBeBuilt(this);
        return Mathf.Clamp01(score);
    }

    public float GetPossibleUpgradeScore()
    {
        float score = upgradeManager.HowManyBuildingCanBeUpgraded(this);
        return Mathf.Clamp01(score);
    }
    public Transform FindUpgradePosition()
    {
        return buildingToUpgrade.transform;
    }

    public Transform FindBuildPosition()
    {
        GraphNode targetNode = _pointDistribution.FindClosestNodeWithAllFreeInRadius(transform.position, constructionToBuild.prefab.GetComponent<BoxCollider>().size.x * 1.2f);
        var positionToBuild = targetNode.Position;
        //Spawn the construction
        buildingToBuild = Instantiate(constructionToBuild.prefab, positionToBuild, Quaternion.identity);
        buildingToBuild.transform.LookAt(transform.position, positionToBuild.normalized);
        _pointDistribution.SetAllInColliderToObstacle(buildingToBuild.GetComponent<BoxCollider>());
        buildingToBuild.SetActive(false);

        GameObject Target = new GameObject();
        Target.transform.position = targetNode.Position;
        
        return  Target.transform;
    }

    public Transform FindBuildRequiredDestination()
    {
        GraphNode targetNode = _pointDistribution.FindClosestNodeFree(buildingToBuild.transform.position, _canMoveOnWater);
        GameObject Target = new GameObject();
        Target.transform.position = targetNode.Position;
        return  Target.transform;
    }

    private void SetAction(ResourceType resource)
    {
        switch (resource)
        {
            case ResourceType.metal :
                thoughtsScript.ChangeAction(ActionOfIA.Iron);
                break;
            case ResourceType.wood :
                thoughtsScript.ChangeAction(ActionOfIA.Wood);
                break;
            case ResourceType.stone :
                thoughtsScript.ChangeAction(ActionOfIA.Iron);
                break;
        }
    }


    

    /*
    public Transform FindBuildPosition()
    {
        Vector3[] CheckDirections = new Vector3[9];
        int index = 0;
        for (int i =-1; i<2; i++)
        {
            for (int j =-1; j<2; j++)
            {
                CheckDirections[index] = new Vector3((float)i, (float)j, 0f);
                index++;
            }
        }

        Debug.Log("Size of construction to build : " + constructionToBuild.prefab.GetComponent<BoxCollider>().size);
        
        float SphereSize = constructionToBuild.prefab.GetComponent<BoxCollider>().size.x * 1.2f;


        // Check if we can just build here first
        if (checkResourcesInSphere(transform.position, SphereSize) == 0)
        {
            positionToBuild = transform.position;
            return transform;
        }

        Vector3 BuildPosition;
        GameObject Target = new GameObject();
        Vector3 OriginVector = new Vector3(0f,0f, -1f);

        // Then immediately around here
        Vector3 Direction = new Vector3(0f, 1f, 0f); // Replace by transform.position when the planet is added.
        for (int j = 0; j< CheckDirections.Length; j++)
        {
            BuildPosition = transform.position + Quaternion.FromToRotation(OriginVector, Direction) * CheckDirections[j]; 
            if (checkResourcesInSphere(BuildPosition, SphereSize) == 0)
            {
                Target.transform.position = BuildPosition;
                positionToBuild = BuildPosition;
                return Target.transform;
            
            }
        }


        // If it's still no good, we look around the resources
        GameObject[] Resources = GameObject.FindGameObjectsWithTag("Resources");
        Array.Sort(Resources, 0, Resources.Length, new SortDistance(transform));
        for(int i = 0; i < Resources.Length; i++)
        {
            Direction = new Vector3(0f, 1f, 0f); // Replace by Resources[i].transform.position when the planet is added.
            for (int j = 0; j< CheckDirections.Length; j++)
            {
                BuildPosition = Resources[i].transform.position + (Quaternion.FromToRotation(OriginVector, Direction) * CheckDirections[j]).normalized * SphereSize * 1.5f; 
                if (checkResourcesInSphere(BuildPosition, SphereSize) == 0)
                {
                    Target.transform.position = BuildPosition;
                    positionToBuild = BuildPosition;
                    return Target.transform;
                }
            }
        }
        Debug.Log("No Pos found!");
        return Target.transform;
    }
    
    public int checkResourcesInSphere(Vector3 SpherePos, float SphereSize)
    {
        int resourcesUnder = 0;
        Collider[] unfiltered = Physics.OverlapSphere(SpherePos, SphereSize);
        foreach (Collider collider in unfiltered)
        {
            //Debug.Log("Found at " + SpherePos + " : " + collider.gameObject.tag);
            if (collider.gameObject.tag == "Resources")
            {
                resourcesUnder++;
            }
            if (collider.gameObject.tag == "Building")
            {
                Debug.Log("Building Found!");
                return 9999;
            }
        }
        return resourcesUnder;
    }


    class SortDistance : IComparer<GameObject>
    {
        static IComparer<GameObject> comparer;
        
        private Transform charPos;
        
        public SortDistance(Transform charP)
        {
            charPos = charP;
            comparer = this;
        }

        

        public float Distance(GameObject a, Transform charac)
        {
            return (a.transform.position - charac.position).sqrMagnitude;
        }

        public int Compare(GameObject a, GameObject b)
        {
            return Comparer<float>.Default.Compare(Distance(a, charPos), Distance(b, charPos));
        }

        public static IComparer<GameObject> Comparer
        {
            get { return comparer; }
        }
    }
    */
}

