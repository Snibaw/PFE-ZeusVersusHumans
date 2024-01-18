using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;

public class UI_Town : MonoBehaviour
{
    private Context _contextScript;
    [SerializeField] private List<ResourceAndTxt> resourceAndTxtList;
    
    [Serializable]
    private struct ResourceAndTxt
    {
        [SerializeField] public ResourceType type;
        [SerializeField] public TMP_Text numberTxt;
    }

    private void Update()
    {
        if (_contextScript != null)
        {
            UpdateResourcesUI();
        }
    }

    public void SetResourcesNb(Context contextScript)
    {
        _contextScript = contextScript;
        UpdateResourcesUI();
    }

    private void UpdateResourcesUI()
    {
        foreach (var resourceAndTxt in resourceAndTxtList)
        {
            int nbResource = _contextScript.storage.GetNbResources(resourceAndTxt.type);
            resourceAndTxt.numberTxt.text = nbResource.ToString();
        }
    }
    
}
