using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcesSlider : MonoBehaviour
{
    [SerializeField] private Animator bgAnimator;
    [SerializeField] private Slider[] _sliders;
    private bool visibility = false;
    int currentTownIndex = -1;
    

    private List<int> maxResourceValue = new List<int>() { 0, 0, 0 };

    void Start()
    {
        foreach (Slider slider in _sliders)
        {
            slider.value = 1;
            slider.maxValue = 3;
        }
        
        InvokeRepeating("UpdateSlidersValue", 0f, 1f);
    }

    public void SetVisible(bool isVisible, int townIndex = -1)
    {
        visibility = isVisible;
        currentTownIndex = townIndex;
        if(isVisible) bgAnimator.gameObject.SetActive(true);
        StartCoroutine(BGAnimationBeforeVisibility(isVisible));
        if (isVisible && currentTownIndex != -1)
        {
            UpdateSlidersValue();
        }
    }
    private IEnumerator BGAnimationBeforeVisibility(bool isVisible)
    {
        string triggerName = isVisible ? "Open" : "Close";
        bgAnimator.SetTrigger(triggerName);
        yield return new WaitForSeconds(0.4f);
        bgAnimator.gameObject.SetActive(visibility);
    }

    public void UpdateSlidersValue()
    {
        Debug.Log("UpdateSlidersValue");
        bool isThisTheTown = false;
        //Reset maxResourceValue
        foreach (ResourceType r in ResourceType.GetValues(typeof(ResourceType)))
        {
            maxResourceValue[(int)r] = 0;
        }
        //update maxResourceValue
        foreach (GameObject town in GameManager.instance.Towns)
        {
            TownBehaviour townBehaviour = town.GetComponent<TownBehaviour>();
            isThisTheTown = townBehaviour.townIndex == currentTownIndex;
            
            Storage storageScript = town.GetComponent<Storage>();
            foreach (ResourceType r in ResourceType.GetValues(typeof(ResourceType)))
            {
                maxResourceValue[(int)r] += townBehaviour.townResourceScore[(int)r];
                if ((isThisTheTown))
                {
                    _sliders[(int)r].value = townBehaviour.townResourceScore[(int)r];
                }
            }
        }
        //Update the slider max value
        foreach (ResourceType r in ResourceType.GetValues(typeof(ResourceType)))
        {
            _sliders[(int)r].maxValue = maxResourceValue[(int)r];
        }
    }
}
