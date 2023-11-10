using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject HumanAI;
    [SerializeField] private float timeBtwHumanSpawn = 5f;
    [SerializeField] private Context townContext;
    private bool canSpanwHuman = true;
    // Start is called before the first frame update
    IEnumerator Start()
    {
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
        GameObject human = Instantiate(HumanAI, transform.position, Quaternion.identity);
        human.GetComponent<NPCController>().context = townContext;
    }
}
