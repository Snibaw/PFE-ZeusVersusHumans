using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownRelation : MonoBehaviour
{
    public class OtherTownRelation
    {
        public int index;
        public GameObject town;
        public float relationValue;
        public bool unknown = true;
        
        public OtherTownRelation(int index, GameObject town, float relationValue, bool unknown = true)
        {
            this.index = index;
            this.town = town;
            this.relationValue = relationValue;
            this.unknown = unknown;
        }
    }
    public List<OtherTownRelation> otherTowns = new List<OtherTownRelation>();
    private TownBehaviour _townBehaviour;
    private AdorationBarManager _adorationBarManager;
    private float valueBeforeAngry = 20;
    
    IEnumerator Start()
    {
        //Wait for every town to be initialized
        yield return new WaitForSeconds(0.1f);
        _townBehaviour = GetComponent<TownBehaviour>();
        _adorationBarManager = GetComponent<AdorationBarManager>();
        InitTownRelation();
        
    }

    private void InitTownRelation()
    {
        for(int i=0; i<GameManager.instance.Towns.Count; i++)
        {
            if (GameManager.instance.Towns[i] == gameObject) continue;
            otherTowns.Add(new OtherTownRelation(i, GameManager.instance.Towns[i], 0, true));
        }
    }
    public float UpdateRelation(int index, float value)
    {
        if(index >= _townBehaviour.townIndex) // because the town itself is not in the list
        {
            index--;
        }
        float relationValue = CalculateRelationValue(value);
        otherTowns[index].relationValue = relationValue;
        otherTowns[index].unknown = false;
        return relationValue;
    }
    private float CalculateRelationValue(float value) // Reminder : <-10 :(  |   -10 to 10 :/   |   >10 :)
    { 
        return valueBeforeAngry - Mathf.Abs(_adorationBarManager.adorationValue - value); 
        // If both adoration value are the same, the relation value is 20
        // The more the adoration value is different, the more the relation value is low
    }
}
