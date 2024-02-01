using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionOfIA{Wood, Stone, Iron, Sleep, Build, Fight, Home}

public class ThoughtsAndActionManager : MonoBehaviour
{
    [SerializeField] private GameObject thoughtBubble;

    private GameObject currentActionOrThought;
    
    [Serializable] public class ActionWithThinking
    {
        [SerializeField] public ActionOfIA action;
        [SerializeField] public GameObject thought;
    }
    
    public ActionWithThinking[] actionsWithThinking;

    private void Start()
    {
        currentActionOrThought = actionsWithThinking[0].thought;
        currentActionOrThought.SetActive(false);
    }


    public void ChangeAction(ActionOfIA action)
    {
        currentActionOrThought.SetActive(false);
        foreach (ActionWithThinking actionWithThinking in actionsWithThinking)
        {
            if (actionWithThinking.action == action)
            {
                currentActionOrThought = actionWithThinking.thought;
                break;
            }
        }
        currentActionOrThought.SetActive(true);
    }

    public void ActivateThoughts(bool isActive)
    {
        thoughtBubble.SetActive(isActive);
    }
    
}
