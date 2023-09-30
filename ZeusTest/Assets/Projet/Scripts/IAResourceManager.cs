using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IAResourceManager : MonoBehaviour
{
    [SerializeField] private int woodNBR;
    [SerializeField] private int stoneNBR;

    [SerializeField] private TMP_Text resourcesText;
        
    public static IAResourceManager instance;
    private void Awake() {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
    }
    private void Start() {
        #if UNITY_EDITOR
        UpdateResourcesText();
        #endif
    }
    // TODO : Add predicted resources (for multiple IA) and choose the resource depending on the number of predicted resources


    public void ChangeResourceNumber(TypeOfResources resource, int value)
    {
        switch(resource)
        {
            case TypeOfResources.wood:
                woodNBR += value;
                break;
            case TypeOfResources.stone:
                stoneNBR += value;
                break;
        }
        #if UNITY_EDITOR
        UpdateResourcesText();
        #endif
    }
    public int GetResourceNumber(TypeOfResources resource)
    {
        switch(resource)
        {
            case TypeOfResources.wood:
                return woodNBR;
            case TypeOfResources.stone:
                return stoneNBR;
            default:
                return -1;
        }
    }
    public void UpdateResourcesText()
    {
        resourcesText.text = "Wood:" + woodNBR + "\n" + "Stone:" + stoneNBR;
    }
}
