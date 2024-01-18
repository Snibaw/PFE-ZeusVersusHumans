using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject HumanAI;
    [SerializeField] private float timeBtwHumanSpawn = 5f;
    [SerializeField] private Context townContext;
    
    public bool canBuildLightningRod = false;
    public bool canUseBoat = false;
    public bool canMeditate = false;
    public bool canRepelWolves = false;
    [SerializeField] private List<float> adorationCheckpoints;
    public Color townColor;
    public float adorationValue = 50f;
    public bool canSpanwHuman = true;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        while (canSpanwHuman)
        {
            SpawnHuman();
            yield return new WaitForSeconds(timeBtwHumanSpawn);
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SpawnHuman()
    {
        GraphNode spawnNode = PointDistribution.instance.FindClosestNodeFree(transform.position, false);
        GameObject human = Instantiate(HumanAI, spawnNode.Position, Quaternion.identity);
        human.transform.rotation = Quaternion.FromToRotation(Vector3.up, human.transform.position - Vector3.zero);
        NPCController npcController = human.GetComponent<NPCController>();
        if (npcController != null)
        {
            human.GetComponent<NPCController>().homeTown = gameObject.GetComponent<Building>();
            human.GetComponent<NPCController>().context = townContext;
        }
        
        //Change human color depending on the town color
        human.GetComponent<HumanModelModificator>().ChangeHatColor(townColor);

    }

    public void AdorationBonuses(float adoration)
    {
        adorationValue = adoration;
        if (adoration <= 0 || adoration >= 100)
            {
                GameManager.instance.CanMakeBabel = true;
            }
        if (adoration <= adorationCheckpoints[0])
            {
                canBuildLightningRod = true;
            }
        if (adoration <= adorationCheckpoints[1])
            {
                canUseBoat = true;
            }
        if (adoration >= adorationCheckpoints[2])
            {
                canMeditate = true;
            }
        if (adoration >= adorationCheckpoints[3])
            {
                canRepelWolves = true;
            }
        
    }
}
