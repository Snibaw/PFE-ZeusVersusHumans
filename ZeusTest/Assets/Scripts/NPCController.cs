using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    decide,
    move,
    execute,
    defendFromWolf,
    AttackWolfHuman
}
public class NPCController : MonoBehaviour
{
    private Billboard billboard;
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
    public GameObject buildingToBuild { get; set; }
    public TownBehaviour homeTown { get; set; }

    public ObjectToDestroy objectToDestroy { get; set; }

    private bool isSleeping = false;

    [SerializeField] private WolfController _wolfTarget;
    private NPCController _humanTarget;

    private float _timeLastAttackWolf;
    private float _timeLastAttackHuman;

    [SerializeField] private float _cooldownAttackWolf;
    [SerializeField] private float _cooldownAttackHuman;

    [SerializeField] private UI_Timer uiTimerScript;
    [SerializeField] private ThoughtsAndActionManager thoughtsScript;
    [SerializeField] private bool _canMoveOnWater;
    PointDistribution _pointDistribution;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _rangeAttack;
    private TownRelation homeTownRelation;
    private int townIndex;
    [HideInInspector] public AdorationBarManager adorationBarManager;

    private Animator _animator;



    public bool isExecuting;
    // Start is called before the first frame update
    void Start()
    {
        mover = GetComponent<MoveController>();
        aiBrain = GetComponent<AIBrain>();
        Inventory = GetComponent<NPCInventory>();
        stats = GetComponent<NPCStats>();
        objectToDestroy = GetComponent<ObjectToDestroy>();
        buildManager = context.storage.gameObject.GetComponent<BuildManager>();
        upgradeManager = context.storage.gameObject.GetComponent<UpgradeManager>();
        _pointDistribution = GameObject.FindWithTag("Planet").GetComponent<PointDistribution>();
        billboard = GetComponentInChildren<Billboard>();
        _animator = GetComponentInChildren<Animator>();
        isExecuting = false;
        constructionToBuild = null;
        _canMoveOnWater = false;

        _timeLastAttackWolf = -_cooldownAttackWolf;

        homeTownRelation = homeTown.GetComponent<TownRelation>();
        adorationBarManager = homeTown.GetComponent<AdorationBarManager>();
        townIndex = homeTown.townIndex;
        homeTown.humanAI.Add(this);

        StartCoroutine(HumanDetection());
        
    }

    // Update is called once per frame
    void Update()
    {
        if (stats.energy <= 0) return;
        
        stats.UpdateEnergy(isSleeping);
        // stats.UpdateHunger();
        FSMTick();
    }

    

    public void FSMTick()
    {
        if(billboard != null) billboard.UpdateStateText(currentState.ToString());
        switch (currentState)
        {
            case State.decide:
                _canMoveOnWater = homeTown.canUseBoat;
                aiBrain.DecideBestAction();
                if (Vector3.Distance(aiBrain.bestAction.RequiredDestination.position, this.transform.position) < context.MinDistance)
                {
                    currentState = State.execute;
                }
                else
                {
                    currentState = State.move;
                }
                break;
            
            case State.move:
                if(aiBrain.bestAction.RequiredDestination == null)
                {
                    currentState = State.decide;
                    break;
                }
                
                if (Vector3.Distance(aiBrain.bestAction.RequiredDestination.position, this.transform.position) < context.MinDistance)
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

            case State.defendFromWolf:
                //Debug.Log("Human defendFromWolf: homeTown.wolfPackToAttack= " + homeTown.wolfPackToAttack + " | _wolfTarget=" + _wolfTarget);
                if(homeTown.wolfPackToAttack != null)
                {
                    if (objectToDestroy.life / objectToDestroy.maxLife < 0.3f && Vector3.Distance(transform.position, homeTown.transform.position) > 1f)
                    {
                        //Debug.Log("Human: Retreat");
                        mover.MoveTo(homeTown.transform.position + UnityEngine.Random.insideUnitSphere * 0.5f, false);
                    }
                    else if (objectToDestroy.life / objectToDestroy.maxLife < 0.3f && Vector3.Distance(transform.position, homeTown.transform.position) < 1f)
                    {
                        //Debug.Log("Human: Sleep");
                        StartCoroutine(ExecuteAction("Sleep",0.5f));
                    }
                    else
                    {
                        //Debug.Log("Human: AttackWolf");
                        if (Vector3.Distance(transform.position, homeTown.transform.position) < 1f)
                        {
                            homeTown.wolfPackToAttack = _wolfTarget._wolfPack;
                        }

                        GameObject wolf = homeTown.wolfPackToAttack._wolfs[0];
                        if(wolf != null) mover.MoveTo(wolf.transform.position + UnityEngine.Random.insideUnitSphere * 0.5f, false);
                        AttackWolf();
                    }
                }
                else if(_wolfTarget != null)
                {
                    if (objectToDestroy.life / objectToDestroy.maxLife < 0.3f && Vector3.Distance(transform.position, homeTown.transform.position) > 1f)
                    {
                        //Debug.Log("Human: Retreat");
                        mover.MoveTo(homeTown.transform.position + UnityEngine.Random.insideUnitSphere * 0.5f, false);
                    }
                    else if (objectToDestroy.life / objectToDestroy.maxLife < 0.3f && Vector3.Distance(transform.position, homeTown.transform.position) < 1f)
                    {
                        //Debug.Log("Human: Sleep");
                        StartCoroutine(ExecuteAction("Sleep", 0.5f));
                    }
                    else
                    {
                        //Debug.Log("Human: AttackWolf");
                        if(Vector3.Distance(transform.position, homeTown.transform.position) < 1f)
                        {
                            homeTown.wolfPackToAttack = _wolfTarget._wolfPack;
                        }
                        mover.MoveTo(_wolfTarget.transform.position + UnityEngine.Random.insideUnitSphere * 0.5f, false);
                        AttackWolf();
                    }
                }
                else
                {
                    currentState = State.decide;
                }
                break;
            case State.AttackWolfHuman:
                if(_humanTarget != null)
                {
                    if (objectToDestroy.life / objectToDestroy.maxLife < 0.3f && Vector3.Distance(transform.position, homeTown.transform.position) > 1f)
                    {
                        mover.MoveTo(homeTown.transform.position + UnityEngine.Random.insideUnitSphere * 0.5f, false);
                    }
                    else
                    {
                        mover.MoveTo(_humanTarget.transform.position + UnityEngine.Random.insideUnitSphere * 0.5f, false);
                        AttackHumans();
                    }
                }
                else
                {
                    currentState = State.decide;
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
        //Debug.Log("Doing : " + action);
        StartCoroutine(ExecuteAction(action, time));
    }
    public IEnumerator ExecuteAction(string action, float time)
    {
        if (aiBrain.bestAction == null)
        {
            currentState = State.decide;
            yield break;
        }
        thoughtsScript.ActivateThoughts(false);
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
        //If exhausted, take more time to do action
        float calculatedTime = time * (0.5f - (stats.energy / 200)) / homeTown.GetComponent<Building>().level; // energy between 0 and 100 => 0 to 0.5 => * 0.5 to 0
        uiTimerScript.StartTimer(calculatedTime);
        yield return new WaitForSeconds(calculatedTime);
        
        if (aiBrain.bestAction == null)
        {
            currentState = State.decide;
            yield break;
        }
        
        switch (action)
        {
            case "Work":
                Resource resource = null;
                if (aiBrain.bestAction.RequiredDestination != null) {
                    resource = aiBrain.bestAction.RequiredDestination.GetComponent<Resource>();
                    //Change Action in Timer / Thought bubble
                    if (resource != null)
                    {
                        SetAction(resource.ResourceType);
                        thoughtsScript.ActivateThoughts(false);
                    }
                    
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
                AdorationBar.instance.FindAndChangeAdorationBarNPC(this, AdorationBarEvents.ConstructBuilding);
                break;
            case "Upgrade":
                ExecuteUpgrade();
                CheckBabel();
                break;
        }
        aiBrain.finishedExecutingBestAction = true;
    }

    private void ExecuteBuild()
    {
        if (buildingToBuild == null) {Debug.Log("No Building To Build"); return;}
        Building building = buildingToBuild.GetComponent<Building>();
        building.changeToCivColor(homeTown.townColor);
        building.context = context;
        buildingToBuild.SetActive(true);

        if(aiBrain.bestAction.RequiredDestination != null) Destroy(aiBrain.bestAction.RequiredDestination.gameObject);
        //Tell the context that a new house has been built
        if (buildingToBuild.GetComponent<Building>().BuildingType == BuildingType.house)
        {
            context.AddDestinationTypeBuild(DestinationType.rest, buildingToBuild.transform);
            buildingToBuild.GetComponent<Building>().context = context;
        }

        foreach( var prod in buildingToBuild.GetComponents<ProductionBuilding>())
        {
            prod.storage = homeTown.GetComponent<Storage>();
        }
            
        
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
        if (buildingToUpgrade == null)  return;
        IAConstruction constructionToUpgrade = upgradeManager.BuildingToConstruction(buildingToUpgrade);
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

    private void CheckBabel()
    {
        if (upgradeManager.CheckIfComplete()) GameManager.instance.CanMakeBabel = true;
    }


    public float GetPossibleBuildScore()
    {
        float score =  homeTown.CanNPCBuild( Inventory); //buildManager.HowManyConstructionCanBeBuilt(this, Inventory);
        //Debug.Log(score);
        return Mathf.Clamp01(score);
    }

    public float GetHypotheticalBuildScore()
    {
        float score = buildManager.HowManyConstructionCanBeBuilt(this);
        return Mathf.Clamp01(score);
    }

    public float GetPossibleUpgradeScore()
    {
        float score = homeTown.CanNPCUpgrade();
        return Mathf.Clamp01(score);
    }
    public Transform FindUpgradePosition()
    {
        homeTown.SetNPCUpgrade(this);
        if (buildingToUpgrade == null) return transform;
        return buildingToUpgrade.transform;
    }

    public Transform FindBuildRequiredDestination()
    {
        homeTown.SetNPCBuild(this);
        if (buildingToBuild == null) return transform;
        GraphNode targetNode = _pointDistribution.FindClosestNodeFree(buildingToBuild.transform.position, _canMoveOnWater);
        GameObject Target = new GameObject();
        Target.transform.position = targetNode.Position;
        buildingToBuild.transform.LookAt(targetNode.Position, buildingToBuild.transform.position.normalized);
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

    public bool canMeditate()
    {
        return homeTown.canMeditate;
    }

    public bool canBeAttacked()
    {
        if (homeTown == null) return true;
        return !homeTown.canRepelWolves;
    }
    
    public float GetAdoration()
    {
        return adorationBarManager.adorationValue;
    }
    
    IEnumerator HumanDetection()
    {
        while (true)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position - transform.forward * 0.5f, 2.5f, _layerMask);
            bool isTheirAWolf = false;
            bool isTheirAHuman = false;
            foreach (Collider collider in colliders)
            {
                if(collider.CompareTag("Wolf"))
                {
                    if (collider.transform.parent.gameObject.TryGetComponent<WolfController>(out _wolfTarget))
                    {
                        isTheirAWolf = true;
                        //Reset AiBrain
                        isExecuting = false;
                        aiBrain.bestAction = null;
                        aiBrain.finishedExecutingBestAction = true;
                        currentState = State.defendFromWolf;
                        if (buildingToBuild != null) Destroy(buildingToBuild);
                    }
                }
                if(collider.CompareTag("canBeDestroyed"))
                {
                    NPCController npc = collider.gameObject.GetComponent<NPCController>();
                    if(npc != null)
                    {
                        if (npc.townIndex != townIndex)
                        {
                            if (npc.homeTown != null && npc.adorationBarManager != null)
                            {
                                float newValue = homeTownRelation.UpdateRelation(npc.homeTown.townIndex,
                                    npc.adorationBarManager.adorationValue);
                                if (newValue < -10)
                                {
                                    isTheirAHuman = true;
                                    currentState = State.AttackWolfHuman;
                                    if (buildingToBuild != null) Destroy(buildingToBuild);
                                    _humanTarget = npc;
                                }
                            }
                        }
                    }
                }
            }
            if(currentState == State.defendFromWolf && !isTheirAWolf) currentState = State.decide;
            if(currentState == State.AttackWolfHuman && !isTheirAHuman) currentState = State.decide;

            yield return new WaitForSeconds(0.3f);
        }
        
    }

    void AttackWolf()
    {
        
        if (_wolfTarget == null) return;
        if (Time.time - _timeLastAttackWolf < _cooldownAttackWolf) return;
        if (Vector3.Distance(_wolfTarget.transform.position, transform.position) > _rangeAttack) return;


        //Debug.Log("Detect: AttackWolf Wolf");
        StartCoroutine(StopMovingTime(2));
        int damage = CalculateDamage();
        _wolfTarget.GetComponent<ObjectToDestroy>().TakeDamage(damage);
        _animator.SetTrigger("attack");
        _timeLastAttackWolf = Time.time;
        //Camera.main.transform.parent.GetComponent<CameraMovement>().MoveToObject(_wolfTarget.gameObject);
    }
    void AttackHumans()
    {
        if (_humanTarget == null) return;
        if (Time.time - _timeLastAttackHuman < _cooldownAttackHuman) return;
        if (Vector3.Distance(_humanTarget.transform.position, transform.position) > _rangeAttack) return;

        //Debug.Log("Detect: AttackWolf Human");
        StartCoroutine(StopMovingTime(2));
        int damage = CalculateDamage();
        _humanTarget.GetComponent<ObjectToDestroy>().TakeDamage(damage);
        _animator.SetTrigger("attack");
        _timeLastAttackHuman = Time.time;
        // Camera.main.transform.parent.GetComponent<CameraMovement>().MoveToObject(_humanTarget.gameObject);
    }
    int CalculateDamage()
    {
        int damage;
        switch (homeTown.GetComponent<Building>().level)
        {
            case 1:
                damage = 50;
                break;
            case 2: 
                damage = 100; 
                break;
            case 3:
                damage = 150;
                break;
            default:
                damage = 50;
                break;
        }

        return damage;
    }

    IEnumerator StopMovingTime(float timeSecond)
    {
        float timeBegin = Time.time;
        while (true)
        {
            if(Time.time - timeBegin > timeSecond)
            {
                break;
            }
            else
            {
                mover.StopMoving();
                yield return new WaitForEndOfFrame();
            }
        }
        yield return new WaitForEndOfFrame();
    }



    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position - transform.forward * 1f, 1f);
    }

    private void OnDestroy()
    {
        Debug.Log("Human Tu�");
        if (buildingToBuild != null) Destroy(buildingToBuild);
        homeTown.humanAI.Remove(this);
    }

}

