using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;

public class UI_Town : MonoBehaviour
{
    private Context contextScript;
    [SerializeField] private List<ResourceAndTxt> resourceAndTxtList;
    
    [Serializable]
    private struct ResourceAndTxt
    {
        [SerializeField] public ResourceType type;
        [SerializeField] public TMP_Text numberTxt;
    }

    public void SetResourcesNb()
    {
        if(contextScript == null) contextScript = GameManager.instance.context;
        
        foreach (var resourceAndTxt in resourceAndTxtList)
        {
            int nbResource = contextScript.storage.GetNbResources(resourceAndTxt.type);
            resourceAndTxt.numberTxt.text = nbResource.ToString();
        }
    }
}
