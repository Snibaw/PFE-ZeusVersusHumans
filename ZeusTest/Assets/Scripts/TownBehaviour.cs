using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownBehaviour : MonoBehaviour
{
    public int townIndex;
    [SerializeField] private GameObject HumanAIprefabs;
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

    public List<NPCController> humanAI;
    public WolfPack wolfPackToAttack = null;

    public int townScore
    {
        set
        {
            _townScore = value;
            TownDevelopmentSlider.instance.UpdateSliders(townIndex, value);
        }
        get
        {
            return _townScore;
        }
    }

    private int _townScore;
    
    public List<int> townResourceScore = new List<int>() { 0, 0, 0 };
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        buildManager = townContext.storage.gameObject.GetComponent<BuildManager>();
        upgradeManager = townContext.storage.gameObject.GetComponent<UpgradeManager>();
        _pointDistribution = GameObject.FindWithTag("Planet").GetComponent<PointDistribution>();
        chooseNextConstruction();
        chooseNextUpgrade();
        humanAI = new List<NPCController>();
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
        //if ( nextConstruction==null) chooseNextConstruction();
        if ( nextUpgrade==null) chooseNextUpgrade();
    }
    void SpawnHuman()
    {
        GraphNode spawnNode = PointDistribution.instance.FindClosestNodeFree(transform.position, false);
        GameObject human = Instantiate(HumanAIprefabs, spawnNode.Position, Quaternion.identity);
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

    public void chooseNextConstruction()
    {
        IAConstruction potentialConstruction = buildManager.FindNextConstruction(this);
        GraphNode targetNode = null;
        if (potentialConstruction != null) targetNode = _pointDistribution.FindClosestNodeWithAllFreeInRadius(transform.position, potentialConstruction.prefab.GetComponent<BoxCollider>().size.x * 1.2f);
        if (targetNode != null) 
        {
            nextConstruction = InitializeNextConstruction(potentialConstruction, targetNode.Position);
            nextIAConstruction = potentialConstruction;
        }
    }

    public void chooseNextUpgrade()
    {
        nextUpgrade = upgradeManager.FindNextUpgrade(this);
    }

    public GameObject InitializeNextConstruction(IAConstruction construction, Vector3 pos)
    {
        //Spawn the construction
        GameObject buildingToBuild = Instantiate(construction.prefab, pos, Quaternion.identity);
        _pointDistribution.SetAllInColliderToObstacle(buildingToBuild.GetComponent<BoxCollider>());
        buildingToBuild.SetActive(false);

        
        //Tell the build manager that a new construction has been built
        buildManager.AddConstructionBuilt(buildingToBuild.GetComponent<Building>().BuildingType);
        
        return buildingToBuild;
    }

    public float CanNPCBuild(NPCInventory inventory)
    {
        if (nextConstruction == null) return 0;
        float score = buildManager.FindBuildPercentage(nextIAConstruction, inventory);
        if (score >= 0.99){
            return 1f;
        }
        return 0f;
    }

    public float CanNPCUpgrade()
    {
        if (nextUpgrade == null) return 0;
        float score = upgradeManager.FindUpgradePercentage(nextUpgrade);
        if (score >= 0.99){
            return 1f;
        }
        return 0f;
    }

    public void SetNPCBuild(NPCController _npcController)
    {
        if (nextConstruction == null) return;
        _npcController.buildingToBuild = nextConstruction;
        _npcController.constructionToBuild = nextIAConstruction;
        chooseNextConstruction();
    }

    public void SetNPCUpgrade(NPCController _npcController)
    {
        if (nextUpgrade == null) return;
    
        _npcController.buildingToUpgrade = nextUpgrade;
        chooseNextUpgrade();
    }
}
