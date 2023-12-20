using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    private int phaseNumber = 0;
    private bool changingPhase = false;

    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private string[] tutorialTexts;
    [SerializeField] private GameObject mouseUI;
    private Animator mouseAnimator;
    [Header("Phase 1")]
    [SerializeField] private GameObject[] hideObjectsPhase1;
    [SerializeField] private GameObject[] showObjectsPhase1;
    
    [Header("Phase 2")]
    [SerializeField] private GameObject[] hideObjectsPhase2;
    [SerializeField] private GameObject[] showObjectsPhase2;
    
    [Header("Phase 3")]
    [SerializeField] private GameObject[] hideObjectsPhase3;
    [SerializeField] private GameObject[] showObjectsPhase3;
    
    [Header("Phase 4")]
    [SerializeField] private GameObject[] hideObjectsPhase4;
    [SerializeField] private GameObject[] showObjectsPhase4;
    
    [Header("Phase 5")]
    [SerializeField] private GameObject[] hideObjectsPhase5;
    [SerializeField] private GameObject[] showObjectsPhase5;

    [SerializeField] private ThrowLightning throwLightning;
    // Start is called before the first frame update
    void Start()
    {
        mouseAnimator = mouseUI.GetComponentInChildren<Animator>();
        ChangePhase();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator SetChangingPhase(bool value = false, float time = 0)
    {
        yield return new WaitForSeconds(time);
        changingPhase = value;
    }
    private void ChangePhase()
    {
        phaseNumber++;
        StartCoroutine(SetChangingPhase(false, 0.5f));
        tutorialText.text = tutorialTexts[phaseNumber-1];
        switch (phaseNumber)
        {
            case 1:
                foreach (GameObject hideObject in hideObjectsPhase1)
                {
                    hideObject.SetActive(false);
                }
                foreach (GameObject showObject in showObjectsPhase1)
                {
                    showObject.SetActive(true);
                }
                throwLightning.enabled = false;
                mouseAnimator.SetTrigger("Phase1");
                break;
            case 2:
                foreach (GameObject hideObject in hideObjectsPhase2)
                {
                    hideObject.SetActive(false);
                }
                foreach (GameObject showObject in showObjectsPhase2)
                {
                    showObject.SetActive(true);
                }

                throwLightning.enabled = true;
                mouseUI.SetActive(true);
                mouseAnimator.SetTrigger("Phase2");
                break;
            case 3:
                foreach (GameObject hideObject in hideObjectsPhase3)
                {
                    hideObject.SetActive(false);
                }
                foreach (GameObject showObject in showObjectsPhase3)
                {
                    showObject.SetActive(true);
                }

                break;
            case 4:
                foreach (GameObject hideObject in hideObjectsPhase4)
                {
                    hideObject.SetActive(false);
                }
                foreach (GameObject showObject in showObjectsPhase4)
                {
                    showObject.SetActive(true);
                }

                break;
            case 5:
                foreach (GameObject hideObject in hideObjectsPhase5)
                {
                    hideObject.SetActive(false);
                }
                foreach (GameObject showObject in showObjectsPhase5)
                {
                    showObject.SetActive(true);
                }

                break;
                
            case 6:
                PlayerPrefs.SetInt("Tutorial", 1);
                SceneManager.LoadScene("MainScene");
                break;
        }
    }
    public void CompletePhase(int i)
    {
        if((phaseNumber <= 3 && i != phaseNumber) || changingPhase) return;
        if(phaseNumber >= 4 && i != 3) return; // We can only complete phase 4 and above by clicking like phase 3
        
        changingPhase = true;
        mouseUI.SetActive(false);
        ChangePhase();
    }
}
