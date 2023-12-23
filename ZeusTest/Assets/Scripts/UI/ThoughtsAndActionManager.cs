using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionOfIA{Food, Iron, Wood, Stone, Sleep, Home}

public class ThoughtsAndActionManager : MonoBehaviour
{
    [SerializeField] private GameObject thoughtBubble;
    
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
        currentActionOrThought = actionUI.Home; // So current is not null !
        currentActionOrThought.SetActive(false);
    }


    public void ChangeAction(ActionOfIA action)
    {
        switch (action)
        {
            case ActionOfIA.Food :
                Debug.Log("Food");
                currentActionOrThought.SetActive(false);
                currentActionOrThought = actionUI.Food;
                currentActionOrThought.SetActive(true);

                break;
            
            case ActionOfIA.Iron :
                Debug.Log("Iron");
                currentActionOrThought.SetActive(false);
                currentActionOrThought = actionUI.Iron;
                currentActionOrThought.SetActive(true);
                break;
            
            case ActionOfIA.Wood :
                Debug.Log("Wood");
                currentActionOrThought.SetActive(false);
                currentActionOrThought = actionUI.Wood;
                currentActionOrThought.SetActive(true);
                break;
            
            case ActionOfIA.Stone :
                Debug.Log("Stone");
                currentActionOrThought.SetActive(false);
                currentActionOrThought = actionUI.Stone;
                currentActionOrThought.SetActive(true);
                break;
            
            case ActionOfIA.Sleep :
                Debug.Log("Sleep");
                currentActionOrThought.SetActive(false);
                currentActionOrThought = actionUI.Sleep;
                currentActionOrThought.SetActive(true);
                break;
            
            case ActionOfIA.Home :
                Debug.Log("Home");
                currentActionOrThought.SetActive(false);
                currentActionOrThought = actionUI.Home;
                currentActionOrThought.SetActive(true);
                break;
        }
    }

    public void ActivateThoughts(bool isActive)
    {
        thoughtBubble.SetActive(isActive);
    }
}
