using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionOfIA{Food, Iron, Wood, Stone, Sleep, Home}

public class ThoughtsAndActionManager : MonoBehaviour
{
    [SerializeField] private GameObject thoughtBubble;
    [SerializeField] private GameObject timerSlider;
    
    [SerializeField] private ActionUI actionUI;

    private GameObject currentActionOrThought;
    
    [Serializable] private class ActionUI
    {
        [SerializeField] public GameObject Food;
        [SerializeField] public GameObject Iron;
        [SerializeField] public GameObject Wood; 
        [SerializeField] public GameObject Stone;
        [SerializeField] public GameObject Sleep;
        [SerializeField] public GameObject Home;

    }

    private void Start()
    {
        currentActionOrThought = actionUI.Food; // So current is not null !
    }


    public void ChangeAction(ActionOfIA action)
    {
        switch (action)
        {
            case ActionOfIA.Food :
                currentActionOrThought.gameObject.SetActive(false);
                actionUI.Food.SetActive(true);
                currentActionOrThought = actionUI.Food;
                break;
            
            case ActionOfIA.Iron :
                currentActionOrThought.gameObject.SetActive(false);
                actionUI.Food.SetActive(true);
                currentActionOrThought = actionUI.Iron;
                break;
            
            case ActionOfIA.Wood :
                currentActionOrThought.gameObject.SetActive(false);
                actionUI.Food.SetActive(true);
                currentActionOrThought = actionUI.Wood;
                break;
            
            case ActionOfIA.Stone :
                currentActionOrThought.gameObject.SetActive(false);
                actionUI.Food.SetActive(true);
                currentActionOrThought = actionUI.Stone;
                break;
            
            case ActionOfIA.Sleep :
                currentActionOrThought.gameObject.SetActive(false);
                actionUI.Food.SetActive(true);
                currentActionOrThought = actionUI.Sleep;
                break;
            
            case ActionOfIA.Home :
                currentActionOrThought.gameObject.SetActive(false);
                actionUI.Food.SetActive(true);
                currentActionOrThought = actionUI.Home;
                break;
        }
    }

    public void ActivateThoughts(bool isActive)
    {
        thoughtBubble.SetActive(isActive);
    }
}
