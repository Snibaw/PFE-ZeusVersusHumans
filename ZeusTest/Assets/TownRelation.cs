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
    
    IEnumerator Start()
    {
        //Wait for every town to be initialized
        yield return new WaitForSeconds(0.1f);
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
}
