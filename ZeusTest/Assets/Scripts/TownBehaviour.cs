using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject HumanAI;
    [SerializeField] private float timeBtwHumanSpawn = 5f;
    [SerializeField] private Context townContext;
    
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
        GraphNode spawnNode = PointDistribution.instance.FindClosestNodeFree(transform.position);
        GameObject human = Instantiate(HumanAI, spawnNode.Position, Quaternion.identity);
        human.transform.rotation = Quaternion.FromToRotation(Vector3.up, human.transform.position - Vector3.zero);
    }
}
