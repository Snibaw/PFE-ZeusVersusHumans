using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBrain : MonoBehaviour
{
    public bool finishedDeciding { get; set; }
    public bool finishedExecutingBestAction { get; set; }
    public Action bestAction { get; set; }
    private NPCController _npcController;
    
    [SerializeField] private Billboard _billboard;
    [SerializeField] private Action[] actionsAvailable;
    [SerializeField] private ThoughtsAndActionManager thoughtsScript;
    
    // Start is called before the first frame update
    void Start()
    {
        _npcController = GetComponent<NPCController>();
        finishedDeciding = false;
        finishedExecutingBestAction = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void DecideBestAction()
    {
        finishedExecutingBestAction = false;
        
        thoughtsScript.ActivateThoughts(true);
        
        float score = 0f;
        int nextBestActionIndex = 0;
        for (int i = 0; i < actionsAvailable.Length; i++)
        {
            if (ScoreAction(actionsAvailable[i]) > score)
            {
                nextBestActionIndex = i;
                score = actionsAvailable[i].score;
            }
        }
        
        bestAction = actionsAvailable[nextBestActionIndex];
        bestAction.SetRequiredDestination(_npcController);

        SetBestActionThought(bestAction);
        Debug.Log("action name : " + bestAction.name);

        finishedDeciding = true;
        _billboard.UpdateBestActionText(bestAction.name);
    }

    private void SetBestActionThought(Action act)
    {
        var actionName = act.name;
        switch (actionName)
        {
            case "Sleep" :
                thoughtsScript.ChangeAction(ActionOfIA.Sleep);
                break;
            case "DropOffResource" :
                thoughtsScript.ChangeAction(ActionOfIA.Home);
                break;
            case "Build" :
                thoughtsScript.ChangeAction(ActionOfIA.Build);
                break;
            case "Upgrade" :
                thoughtsScript.ChangeAction(ActionOfIA.Build);
                break;
            case "Eat" :
                //thoughtsScript.ChangeAction(ActionOfIA.Food);
                break;
            case "Work" :
                if (bestAction.RequiredDestination == null)
                {
                    thoughtsScript.ChangeAction(ActionOfIA.Wood);
                    break;
                }
                Resource resource = bestAction.RequiredDestination.GetComponent<Resource>();
                if (resource == null)
                {
                    thoughtsScript.ChangeAction(ActionOfIA.Wood);
                    break;
                }
                switch (resource.ResourceType)
                {
                    case ResourceType.wood:
                        thoughtsScript.ChangeAction(ActionOfIA.Wood);
                        break;
                    case ResourceType.stone:
                        thoughtsScript.ChangeAction(ActionOfIA.Stone);
                        break;
                    case ResourceType.metal:
                        thoughtsScript.ChangeAction(ActionOfIA.Iron);
                        break;
                }
                break;
            default:
                thoughtsScript.ChangeAction(ActionOfIA.Sleep);
                break;
        }
    }
        
    public float ScoreAction(Action action)
    {
        float score = 1f;
        for (int i = 0; i < action.considerations.Length; i++)
        {
            float considerationScore = action.considerations[i].ScoreConsideration(_npcController);
            score *= considerationScore;

            if (score == 0) // avoid useless calculations
            {
                action.score = 0;
                return action.score; // Avoid useless calculations
            }
        }
        
        //Averaging score
        // Dave Mark's method GDC Talk and Behavioral Mathematics For Game AI
        float originalScore = score;
        if (action.considerations.Length == 0)
        {
            Debug.Log("No considerations for action " + action.name);
            return 0;
        }
        float modFactor = 1 - (1 / action.considerations.Length);
        float makeupValue = (1 - originalScore) * modFactor;
        action.score = originalScore + (makeupValue * originalScore);
        return action.score;
    }

    
    
}
