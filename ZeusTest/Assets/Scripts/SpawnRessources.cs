using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnResources : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private MeshCollider planetCollider;
    private float planetRadius;
    private Vector3 planetCenter;
    [SerializeField] private int maxIterationToFindAPosition = 100;

    [Header("ResourcesToSpawn")]
    [SerializeField] private GameObject folderParentOfResources;

    [SerializeField] private ResourceToSpawn[] ResourcesToSpawn;
    [SerializeField] private GameObject townPrefab;
    [SerializeField] private int numberOfTownToSpawn;
    [SerializeField] private Color[] townColors;

    [SerializeField] private GameObject babelPrefab;
    [SerializeField] private GameObject _wolfPackPrefab;
    PointDistribution _pointDistribution;

    List<GraphNode> nodesWithMostSpaceAround = new List<GraphNode>();

    private List<GraphNode> _constructionToAvoidWhenSpawnTown = new List<GraphNode>();


    private void Awake() 
    {
        _pointDistribution = GetComponent<PointDistribution>();
        planetRadius = planetCollider.bounds.extents.x;
        planetCenter = planetCollider.bounds.center;
    }

    private void Start()
    {
         
    }


    public void InitSpawnResourcesOnPlanet()
    {
        //First spawn the bigger object (town)
        GraphNode centerNode = null;
        centerNode = FindRandomNodeWithMostSpaceAround();
        

        if(centerNode == null) return;


        centerNode.IsObstacle = true;
        var babelSpawned = InstantiateResource(babelPrefab, centerNode);
        _pointDistribution.SetAllInColliderToObstacle(babelSpawned.GetComponent<BoxCollider>());
        
        _constructionToAvoidWhenSpawnTown.Add(centerNode);
        
        SpawnTowns(babelSpawned);
        
        foreach (ResourceToSpawn Resource in ResourcesToSpawn)
        {
            if(Resource.spawnOnGroup)
            {
                for(int i = 0; i < Resource.numberOfGroupToSpawn; i++)
                {
                    SpawnMultipleResources(Resource);
                }
            }
            else
            {
                //Spawn the Resource independently
                for(int i = 0; i< Resource.numberToSpawn; i++)
                {
                    SpawnSingleResource(Resource.prefab);
                }
            }
        }

        StartCoroutine(SpawnSingleResourceEveryCycleOfTimeWithAMaximumOfElement(_wolfPackPrefab, 60, 3));

    }

    private void SpawnTowns(GameObject babelSpawned)
    {
        for(int i = 0; i < numberOfTownToSpawn; i++)
        {
            GraphNode centerNode = null;
            centerNode = FindRandomNodeWithMostSpaceAround(true);
            if(centerNode != null)
            { //Spawn town the furthest from the other towns, then add town to the list of spawned towns
                centerNode.IsObstacle = true;
                var townSpawned = InstantiateResource(townPrefab, centerNode);
                _pointDistribution.SetAllInColliderToObstacle(townSpawned.GetComponent<BoxCollider>());

                townSpawned.GetComponent<UpgradeManager>().AddBuildingBuilt(babelSpawned.GetComponent<Building>());
                townSpawned.GetComponent<TownBehaviour>().townColor = townColors[i];
                townSpawned.GetComponent<TownBehaviour>().townIndex = i;
                townSpawned.GetComponent<Building>().changeToCivColor(townColors[i]);
                _constructionToAvoidWhenSpawnTown.Add(centerNode);

                GameManager.instance.Towns.Add(townSpawned);
            }
        }
    }
    private GameObject SpawnSingleResource(GameObject prefab)
    {
        GraphNode randomNode = null;
        GameObject resource = null;
        // Vector3 spawnPoint;
        // SearchAPositionToSpawn
        for(int i = 0; i < maxIterationToFindAPosition ; i++)
        {
            int index = Random.Range(0, _pointDistribution.nodes.Length);
            randomNode = _pointDistribution.nodes[index]; // Random node
            if (randomNode.IsObstacle) continue;
            if (randomNode.IsWater) continue;// If the node is an obstacle, we continue
            _pointDistribution.nodes[index].IsObstacle = true; // We set the node as an obstacle
            break;
        } 
        if(randomNode != null)
        {
            resource = InstantiateResource(prefab, randomNode);
        }
        return resource;
    }

    private void SpawnMultipleResources(ResourceToSpawn Resource)
    {
        GraphNode centerNode = null;
        // SearchAPositionToSpawn
        centerNode = FindRandomNodeWithMostSpaceAround();
        if(centerNode != null)
        {
            //Find the nodes around the centerNode
            List<GraphNode> nodesAroundFree = new List<GraphNode>();
            nodesAroundFree.Add(centerNode);
            foreach (GraphNode neighbor in _pointDistribution.graph[centerNode])
            {
                if(neighbor.IsObstacle) continue;
                if (neighbor.IsWater) continue;
                foreach (GraphNode neighbor2 in _pointDistribution.graph[neighbor])
                {
                    if(neighbor2.IsObstacle) continue;
                    if (neighbor2.IsWater) continue;
                    if (!nodesAroundFree.Contains(neighbor2)) nodesAroundFree.Add(neighbor2);
                }
            }
            
            //Now build randomly around the centerNode
            for(int i = 0; i < Resource.numberToSpawn-1; i++)
            {
                if(nodesAroundFree.Count == 0) break;
                int index = Random.Range(0, nodesAroundFree.Count);
                GraphNode nodeToBuild = nodesAroundFree[index];
                nodesAroundFree.RemoveAt(index);
                nodeToBuild.IsObstacle = true; 
                InstantiateResource(Resource.prefab, nodeToBuild);
            }
        }
    }

    private GraphNode FindRandomNodeWithMostSpaceAround(bool AvoidListOfNodes = false)
    {
        // Find the node with the most space around in the graph
        int maxSpaceAround = 0;
        foreach (GraphNode node in _pointDistribution.nodes)
        {
            if (node.IsObstacle) continue;
            if (node.IsWater) continue;
            int spaceAround = 0;
            foreach (GraphNode neighbor in _pointDistribution.graph[node]) // We check the neighbors
            {
                if (neighbor.IsObstacle) continue;
                if (neighbor.IsWater) continue;
                foreach (GraphNode neighbor2 in _pointDistribution.graph[neighbor]) // We check the neighbors of the neighbors
                {
                    if (neighbor2.IsObstacle) continue;
                    if (neighbor2.IsWater) continue;
                    spaceAround += 1;
                }
            }
            
            if (spaceAround > maxSpaceAround) // We find a node with more space around
            {
                maxSpaceAround = spaceAround;
                nodesWithMostSpaceAround.Clear();
                nodesWithMostSpaceAround.Add(node);
            }
            else if(spaceAround == maxSpaceAround) // We find a node with the same space around
            {
                nodesWithMostSpaceAround.Add(node);
            }
        }
        if ( maxSpaceAround < 5) return null;
        // Return a random node with the most space around
        GraphNode nodeToReturn = null;
        if (!AvoidListOfNodes)
        {
            int index = Random.Range(0, nodesWithMostSpaceAround.Count);
            nodeToReturn = nodesWithMostSpaceAround[index];
            nodesWithMostSpaceAround.RemoveAt(index);
        }
        else
        {
            nodeToReturn = FindFurthestNodeFromList(nodesWithMostSpaceAround);
            nodesWithMostSpaceAround.Remove(nodeToReturn);
        }
        return nodeToReturn;
    }
    private GraphNode FindFurthestNodeFromList(List<GraphNode> nodes)
    {
        // We want the furthest node from the already constructed buildings. We just find the node where the nearest building is the furthest.
        GraphNode furthestNode = null;
        float furthestDistance = 0;
        foreach (GraphNode node in nodes)
        {
            float minDistance = Mathf.Infinity;
            foreach (GraphNode constructNodes in _constructionToAvoidWhenSpawnTown) 
            {
                float distance = Vector3.Distance(node.Position, constructNodes.Position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                }
            }
            if (minDistance > furthestDistance)
            {
                furthestDistance = minDistance;
                furthestNode = node;
            }
        }
        return furthestNode;
    }
    private GameObject InstantiateResource(GameObject prefab, GraphNode node)
    {
        if(_pointDistribution.uspheres.Count > 0) _pointDistribution.uspheres[node.index].GetComponent<MeshRenderer>().material.color = Color.blue;
        GameObject objSpawned = Instantiate(prefab, node.Position, Quaternion.identity);
        SetRotationAndParent(objSpawned);
        return objSpawned;
    }
    public void SetRotationAndParent(GameObject objSpawned)
    {
        objSpawned.transform.rotation = Quaternion.FromToRotation(Vector3.up, objSpawned.transform.position - planetCenter); // Set the rotation
        objSpawned.transform.parent = folderParentOfResources.transform; // Set the parent
    }


    IEnumerator SpawnSingleResourceEveryCycleOfTimeWithAMaximumOfElement(GameObject prefab,  float timeCycle, int maxElement)
    {

        while (true)
        {

            if(GameManager.instance.WolfPacks.Count < maxElement)
            {
                prefab.GetComponentInChildren<WolfPack>().wolfsSkin = GameManager.instance.WolfPacks.Count;
                

                GameManager.instance.WolfPacks.Add(SpawnSingleResource(prefab).GetComponentInChildren<WolfPack>());
            }
            



            yield return new WaitForSeconds(timeCycle);
        }

        
    }

}
