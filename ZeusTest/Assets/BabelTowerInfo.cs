using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BabelTowerInfo : MonoBehaviour
{
    [SerializeField] private GameObject babelUI;
    [SerializeField] private TMP_Text levelText;
    
    private void Start()
    {
        babelUI.SetActive(false);
    }
    public void SetVisible(bool visible, int level = 0, int maxLevel = 10)
    {
        babelUI.SetActive(visible);
        levelText.text = level + "/" + maxLevel;
    }
}
