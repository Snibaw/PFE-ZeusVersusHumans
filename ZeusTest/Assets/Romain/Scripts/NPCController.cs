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
    public Context context;
    public BuildManager buildManager { get; set; }
    
    public State currentState { get; set; }
    public IAConstruction constructionToBuild { get; set; }
    public Vector3 positionToBuild { get; set; }

    [SerializeField] private UI_Timer uiTimerScript;

    private bool isExecuting;
    // Start is called before the first frame update
    void Start()
    {
        mover = GetComponent<MoveController>();
        aiBrain = GetComponent<AIBrain>();
        Inventory = GetComponent<NPCInventory>();
        stats = GetComponent<NPCStats>();
        buildManager = context.storage.gameObject.GetComponent<BuildManager>();
        isExecuting = false;
        constructionToBuild = null;
    }

    // Update is called once per frame
    void Update()
    {
        stats.UpdateEnergy(AmIAtRestDestination());
        // stats.UpdateHunger();
        FSMTick();
    }

    public void FSMTick()
    {
        float MinDistanceModifier = constructionToBuild == null ? 1f : 0.1f;
        switch (currentState)
        {
            case State.decide:
                aiBrain.DecideBestAction();
                MinDistanceModifier = constructionToBuild == null ? 1f : 0.1f;
                Debug.Log("Distance: " + Vector3.Distance(aiBrain.bestAction.RequiredDestination.position, this.transform.position));
                Debug.Log(context.MinDistance * MinDistanceModifier);
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
                    mover.MoveTo(aiBrain.bestAction.RequiredDestination.position);
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

    public bool AmIAtRestDestination()
    {
        return Vector3.Distance(this.transform.position, context.home.transform.position) <= context.MinDistance;
    }
    public void DoAction(string action, float time)
    {
        Debug.Log("Doing : " + action);
        StartCoroutine(ExecuteAction(action, time));
    }
    private IEnumerator ExecuteAction(string action, float time)
    {
        if (action != "Sleep")
        {
            stats.energy -= context.energyLostPerAction;
            if(stats.energy == 0) time = time*2;
        }
        
        yield return new WaitForSeconds(time);
        
        switch (action)
        {
            case "Work":
                Inventory.AddResource(aiBrain.bestAction.RequiredDestination.GetComponent<Resource>().ResourceType, 1);
                uiTimerScript.StartTimer((int)time);
                break;
            case "Sleep":
                stats.energy += 100;
                break;
            // case "Eat":
            //     stats.hunger -= 30;
            //     break;
            case "DropOffResource":
                context.storage.GetAllResourcesFromNPC(Inventory);
                stats.resource = 0;
                break;
            case "FetchResource":
                // TO do later if buildings needs a lot of resources.
                break;
            case "Build":
                ExecuteBuild();
                break;
        }
        aiBrain.finishedExecutingBestAction = true;
    }

    private void ExecuteBuild()
    {
        //Spawn the construction
        Instantiate(constructionToBuild.prefab, positionToBuild, Quaternion.identity);
        Debug.Log(positionToBuild);
        //delete resources from the inventory
        foreach (ResourceType r in ResourceType.GetValues(typeof(ResourceType)))
        {
            Inventory.RemoveResource(r, constructionToBuild.GetResourceNeeded(r));
        }
        constructionToBuild = null;
        positionToBuild = Vector3.zero;
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
        
        float SphereSize = constructionToBuild.prefab.GetComponent<Collider>().bounds.extents.x / 2f;

        // Check if we can just build here first
        if (checkRessourcesInSphere(transform.position, SphereSize) == 0)
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
            if (checkRessourcesInSphere(BuildPosition, SphereSize) == 0)
            {
                Target.transform.position = BuildPosition;
                positionToBuild = BuildPosition;
                return Target.transform;
            
            }
        }


        // If it's still no good, we look around the ressources
        GameObject[] Ressources = GameObject.FindGameObjectsWithTag("Resources");
        Array.Sort(Ressources, 0, Ressources.Length, new SortDistance(transform));
        for(int i = 0; i < Ressources.Length; i++)
        {
            Direction = new Vector3(0f, 1f, 0f); // Replace by Ressources[i].transform.position when the planet is added.
            for (int j = 0; j< CheckDirections.Length; j++)
            {
                BuildPosition = Ressources[i].transform.position + (Quaternion.FromToRotation(OriginVector, Direction) * CheckDirections[j]).normalized * SphereSize * 1.5f; 
                if (checkRessourcesInSphere(BuildPosition, SphereSize) == 0)
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
    
    public int checkRessourcesInSphere(Vector3 SpherePos, float SphereSize)
    {
        int ressourcesUnder = 0;
        Collider[] unfiltered = Physics.OverlapSphere(SpherePos, SphereSize);
        foreach (Collider collider in unfiltered)
        {
            if (collider.gameObject.tag == "Resources")
            {
                ressourcesUnder++;
            }
            if (collider.gameObject.tag == "Building")
            {
                return 9999;
            }
        }
        return ressourcesUnder;
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

}

