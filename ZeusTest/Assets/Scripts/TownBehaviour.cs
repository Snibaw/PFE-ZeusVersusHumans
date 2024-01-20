using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject HumanAI;
    [SerializeField] private float timeBtwHumanSpawn = 5f;
    [SerializeField] private Context townContext;
    PointDistribution _pointDistribution;
    
    public BuildManager buildManager { get; set; }
    public UpgradeManager upgradeManager { get; set; }

    public bool canBuildLightningRod = false;
    public bool canUseBoat = false;
    public bool canMeditate = false;
    public bool canRepelWolves = false;
    [SerializeField] private List<float> adorationCheckpoints;
    public Color townColor;
    public float adorationValue = 50f;
    public bool canSpanwHuman = true;
    public GameObject nextConstruction;
    public IAConstruction nextIAConstruction;
    public Building nextUpgrade;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        buildManager = townContext.storage.gameObject.GetComponent<BuildManager>();
        upgradeManager = townContext.storage.gameObject.GetComponent<UpgradeManager>();
        _pointDistribution = GameObject.FindWithTag("Planet").GetComponent<PointDistribution>();
        chooseNextAction();
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
            human.GetComponent<NPCController>().homeTown = this;
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

    public void chooseNextAction()
    {
        IAConstruction potentialConstruction = buildManager.FindNextConstruction(this, out float constructionScore);
        Building potentialUpgrade = upgradeManager.FindNextUpgrade(this, out float upgradeScore);
        GraphNode targetNode = _pointDistribution.FindClosestNodeWithAllFreeInRadius(transform.position, potentialConstruction.prefab.GetComponent<BoxCollider>().size.x * 1.1f);
        if ( constructionScore >= upgradeScore&&targetNode != null) 
        {
            nextConstruction = InitializeNextConstruction(potentialConstruction, targetNode.Position);
            nextIAConstruction = potentialConstruction;
            nextUpgrade = null;
        } else {
            nextConstruction = null;
            nextIAConstruction = null;
            nextUpgrade = potentialUpgrade;
        }
    }

    public GameObject InitializeNextConstruction(IAConstruction construction, Vector3 pos)
    {
        //Spawn the construction
        GameObject buildingToBuild = Instantiate(construction.prefab, pos, Quaternion.identity);
        _pointDistribution.SetAllInColliderToObstacle(buildingToBuild.GetComponent<BoxCollider>());
        buildingToBuild.SetActive(false);
        return buildingToBuild;
    }

    public float NPCBuild(NPCController _npcController, NPCInventory inventory)
    {
        if (nextConstruction == null) return 0;
        float score = buildManager.FindBuildPercentage(nextIAConstruction, inventory);
        if (score >= 1){
            _npcController.buildingToBuild = nextConstruction;
            _npcController.constructionToBuild = nextIAConstruction;
            chooseNextAction();
            return 1f;
        }
        return 0f;
    }

    public float NPCUpgrade(NPCController _npcController)
    {
        if (nextUpgrade == null) return 0;
        float score = upgradeManager.FindUpgradePercentage(nextUpgrade);
        if (score >= 1){
            _npcController.buildingToUpgrade = nextUpgrade;
            chooseNextAction();
            return 1f;
        }
        return 0f;
    }
}
