using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelationBtwCiv : MonoBehaviour
{
    [System.Serializable]
    public class FeelingsStats
    {
        public string Name;
        public Color Color;
        public Sprite Feeling;
        public float minValue = -Mathf.Infinity;
    }
    [SerializeField] private FeelingsStats[] feelings;
    [SerializeField] private GameObject layout;
    [SerializeField] private Image[] townsImages;
    [SerializeField] private Image[] feelingsImages;
    public void SetVisible(bool visible, GameObject town = null)
    {
        layout.SetActive(visible);

        if (visible && town != null)
        {
            SetTownRelation(town);
        }
    }
    private void SetTownRelation(GameObject town)
    {
        TownRelation townRelation = town.GetComponent<TownRelation>();
        for(int i=0; i < townRelation.otherTowns.Count; i++)
        {
            townsImages[i].color = townRelation.otherTowns[i].town.GetComponent<TownBehaviour>().townColor;
            SetFeelingImage(i, townRelation.otherTowns[i].relationValue, townRelation.otherTowns[i].unknown);
        }
    }

    private void SetFeelingImage(int index, float relationValue, bool isUnknown = true)
    {
        if(isUnknown)
        {
            feelingsImages[index].sprite = feelings[0].Feeling;
            feelingsImages[index].color = feelings[0].Color;
            return;
        }
        
        for (int i = 1; i < feelings.Length; i++)
        {
            if(relationValue >= feelings[i].minValue)
            {
                feelingsImages[index].sprite = feelings[i].Feeling;
                feelingsImages[index].color = feelings[i].Color;
                return;
            }
        }
        feelingsImages[index].sprite = feelings[^1].Feeling;
        feelingsImages[index].color = feelings[^1].Color;
    }
}
