using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject HumanAI;
    [SerializeField] private float timeBtwHumanSpawn = 5f;
    [SerializeField] private Context townContext;
    
    [SerializeField] private bool canBuildLightningRod = false;
    [SerializeField] private bool canUseBoat = false;
    [SerializeField] private bool canMeditate = false;
    [SerializeField] private bool canRepelWolves = false;
    [SerializeField] private List<float> adorationCheckpoints;


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
        
    }

    public void AdorationBonuses(float adoration)
    {
        if (adoration <= 0 || adoration >= 100)
            {
                GameManager.instance.CanMakeBabel = true;
            }
        if (adoration <= adorationCheckpoints[0])
            {
                canRepelWolves = true;
            }
        if (adoration <= adorationCheckpoints[1])
            {
                canMeditate = true;
            }
        if (adoration >= adorationCheckpoints[2])
            {
                canUseBoat = true;
            }
        if (adoration >= adorationCheckpoints[3])
            {
                canBuildLightningRod = true;
            }
        
    }
}
