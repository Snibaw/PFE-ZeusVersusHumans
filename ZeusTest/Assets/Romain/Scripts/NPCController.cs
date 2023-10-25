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
    // Start is called before the first frame update
    void Start()
    {
        mover = GetComponent<MoveController>();
        aiBrain = GetComponent<AIBrain>();
        Inventory = GetComponent<NPCInventory>();
        stats = GetComponent<NPCStats>();
    }

    // Update is called once per frame
    void Update()
    {
        // if (aiBrain.finishedDeciding)
        // {
        //     aiBrain.finishedDeciding = false;
        //     aiBrain.bestAction.Execute(this);
        // }
        //
        // stats.UpdateEnergy(AmIAtRestDestination());
        // stats.UpdateHunger();
        FSMTick();
    }

    public void FSMTick()
    {
        switch (currentState)
        {
            case State.decide:
                aiBrain.DecideBestAction();

                if (Vector3.Distance(aiBrain.bestAction.RequiredDestination.position, this.transform.position) < 2f)
                {
                    currentState = State.execute;
                }
                else
                {
                    currentState = State.move;
                }
                break;
            
            case State.move:
                
                if(Vector3.Distance(aiBrain.bestAction.RequiredDestination.position, this.transform.position) < 2f)
                {
                    currentState = State.execute;
                }
                else
                {
                    mover.MoveTo(aiBrain.bestAction.RequiredDestination.position);
                }    
                
                break;
            
            case State.execute:

                if (aiBrain.finishedExecutingBestAction == false)
                {
                    aiBrain.bestAction.Execute(this);
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

    public bool AmIAtRestDestination()
    {
        return Vector3.Distance(this.transform.position, context.home.transform.position) <= context.MinDistance;
    }
    #region Coroutine

    public void DoWork(int time) // Time = time to harvest one resource
    {
        StartCoroutine(WorkCoroutine(time));
    }

    public void DoSleep(int time)
    {
        StartCoroutine(SleepCoroutine(time));
    }

    IEnumerator WorkCoroutine(int time)
    {
        int counter = time;
        while (counter > 0)
        {
            yield return new WaitForSeconds(1);
            counter--;
        }
        Debug.Log("Harvest 1 resource done");
        //Logic to update things involved with work (inventory, UI, etc.)
        Inventory.AddResource(ResourceType.wood, 1);
        //Decide our new best action after we've finished working
        //OnFinishedAction();
        aiBrain.finishedExecutingBestAction = true;
    }
    IEnumerator SleepCoroutine(int time)
    {
        int counter = time;
        while (counter > 0)
        {
            yield return new WaitForSeconds(1);
            counter--;
        }
        Debug.Log("Sleep done");
        //Logic to update energy
        stats.energy += 10;
        //OnFinishedAction();
        aiBrain.finishedExecutingBestAction = true;
    }
    
    #endregion
}
