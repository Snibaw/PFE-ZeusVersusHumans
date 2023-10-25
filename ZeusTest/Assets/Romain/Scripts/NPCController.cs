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
    
    public State currentState { get; set; }

    private bool isExecuting;
    // Start is called before the first frame update
    void Start()
    {
        mover = GetComponent<MoveController>();
        aiBrain = GetComponent<AIBrain>();
        Inventory = GetComponent<NPCInventory>();
        stats = GetComponent<NPCStats>();
        isExecuting = false;
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
        switch (currentState)
        {
            case State.decide:
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
                if (Vector3.Distance(aiBrain.bestAction.RequiredDestination.position, this.transform.position) < context.MinDistance)
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
        }
        aiBrain.finishedExecutingBestAction = true;
    }

}
