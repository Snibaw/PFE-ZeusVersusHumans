using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class PointDistribution : MonoBehaviour
{
    public static PointDistribution instance = null;

    [SerializeField] private bool showSpheres = true;
    [SerializeField] private float sphereScale = 0.001f;
    [SerializeField] private int numberOfPointsOnSphere = 128;
    [SerializeField] private GameObject sphereParent;

    [SerializeField] private GameObject _follower;

    [SerializeField] private GameObject _startNode;
    [SerializeField] private GameObject _endNode;
    private GraphNode[] _nodesClicked = new GraphNode[2];
    private float _distanceBtw2Points;
    public GraphNode[] nodes;
    public Dictionary<GraphNode, List<GraphNode>> graph;

    [HideInInspector]
    public List<GameObject> uspheres = new List<GameObject>();
    private float scaling;
    
    SpawnResources spawnResources;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if(instance != this) Destroy(gameObject);
    }

    public void Start()
    {

        ClearSphereFolder();
        uspheres = new List<GameObject>();
        scaling = transform.localScale.x;
        nodes = PointsOnSphere(numberOfPointsOnSphere);
        
        _distanceBtw2Points = Vector3.Distance(nodes[0].Position, nodes[1].Position);
         graph = CreateGraph(nodes, 1.5f * _distanceBtw2Points);

         if (showSpheres)
         {
             foreach (GraphNode point in nodes)
             {
                 GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                 sphere.transform.parent = sphereParent.transform;
                 sphere.transform.position = point.Position;
                 sphere.transform.localScale = new Vector3(sphereScale, sphereScale, sphereScale);
                 sphere.tag = "Sphere";
            
                 if(point.IsObstacle)
                     sphere.GetComponent<Renderer>().material.color = Color.blue;
            
                 uspheres.Add(sphere);
             }
         }
        
        
        spawnResources = GetComponent<SpawnResources>();
        spawnResources.InitSpawnResourcesOnPlanet();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Raycast from the mouse position to the world, check if it hits a sphere
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.collider.gameObject.CompareTag("Sphere"))
                {
                    _nodesClicked[0] = _nodesClicked[1];
                    _nodesClicked[1] = nodes[uspheres.IndexOf(hit.collider.gameObject)];
                    
                    Debug.Log("Clicked on sphere " + _nodesClicked[1].Position + " is obstacle : " + _nodesClicked[1].IsObstacle);
                    if (_nodesClicked[0] != null && _nodesClicked[1] != null)
                    {

                        MoveController follower = Instantiate(_follower).GetComponent<MoveController>();
                        follower.transform.position = _nodesClicked[0].Position;
                        follower.MoveTo(_nodesClicked[1].Position);
                        
                    }
                }
            }
        }
    }

    public void FindPath()
    {
        Random.InitState(System.DateTime.Now.Second);
        if (_startNode == null && _endNode == null) CalculatePath(nodes[Random.Range(0, nodes.Length)], nodes[Random.Range(0, nodes.Length)]);
        else if (_startNode == null) CalculatePath(nodes[Random.Range(0, nodes.Length)], nodes[uspheres.IndexOf(_endNode)]);
        else if (_endNode == null) CalculatePath(nodes[uspheres.IndexOf(_startNode)], nodes[Random.Range(0, nodes.Length)]);
        else CalculatePath(nodes[uspheres.IndexOf(_startNode)], nodes[uspheres.IndexOf(_endNode)]);
    }
    
    public List<Vector3> CalculatePath(GraphNode startNode, GraphNode endNode)
    {        
        AStar aStar = new AStar();

        List<Vector3> path = aStar.FindPath(graph, startNode, endNode);
        
        for (int i = 0; i < path.Count - 1; i++)
        {
            Debug.DrawLine(path[i], path[i + 1], Color.red, 5f);
        }

        return path;
    }

    GraphNode[] PointsOnSphere(int n)
    {
        List<GraphNode> upts = new List<GraphNode>();
        float inc = Mathf.PI * (3 - Mathf.Sqrt(5));
        float off = 2.0f / n;
        float x, y, z, r, phi;

        for (var k = 0; k < n; k++)
        {
            y = k * off - 1 + (off / 2);
            r = Mathf.Sqrt(1 - y * y);
            phi = k * inc;
            x = Mathf.Cos(phi) * r;
            z = Mathf.Sin(phi) * r;

            GraphNode g = new GraphNode(new Vector3(x, y, z) * scaling * 1.3f, k, true);
            upts.Add(g);
        }
        return upts.ToArray();
    }

    Dictionary<GraphNode, List<GraphNode>> CreateGraph(GraphNode[] nodes, float connectionThreshold)
    {
        Dictionary<GraphNode, List<GraphNode>> graph = new Dictionary<GraphNode, List<GraphNode>>();

        for (int i = 0; i < nodes.Length; i++)
        {
            List<GraphNode> neighbors = new List<GraphNode>();

            for (int j = 0; j < nodes.Length; j++)
            {
                if (i != j && Vector3.Distance(nodes[i].Position, nodes[j].Position) < connectionThreshold)
                {
                    neighbors.Add(nodes[j]);
                }
            }

            graph.Add(nodes[i], neighbors);
        }

        return graph;
    }


    public GraphNode FindTheClosestGraphNode(Vector3 position)
    {
        GraphNode closestNode = null;
        float closestDistance = float.MaxValue;
        
        foreach(var node in graph.Keys)
        {
            float distance = Vector3.Distance(node.Position, position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNode = node;
            }
        }

        return closestNode;

    }


    public void ClearSphereFolder()
    {
        for (int i = sphereParent.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(sphereParent.transform.GetChild(i).gameObject);
        }

        nodes = null;
        uspheres = null;
    }

    public GraphNode FindClosestNodeFree(Vector3 position)
    {
        GraphNode closestNode = null;
        float closestDistance = float.MaxValue;
        
        foreach(var node in graph.Keys)
        {
            if(node.IsObstacle) continue;
            float distance = Vector3.Distance(node.Position, position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNode = node;
            }
        }
        return closestNode;
    }
    public GraphNode FindClosestNodeWithAllFreeInRadius(Vector3 position, float radius)
    {
        GraphNode closestNode = null;
        float closestDistance = float.MaxValue;
        
        foreach(var node in graph.Keys)
        {
            if(node.IsObstacle) continue;
            bool neighborIsObstacle = false;
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].IsObstacle && Vector3.Distance(nodes[i].Position, node.Position) < radius)
                {
                    neighborIsObstacle = true;
                    break;
                }
            }
            if(neighborIsObstacle) continue;
            float distance = Vector3.Distance(node.Position, position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNode = node;
            }
        }
        return closestNode;
    }

    public void SetAllInRadiusToObstacle(GraphNode positionNode, float radius)
    {
        for (int i = 0; i < nodes.Length; i++)
            {
                if (Vector3.Distance(nodes[i].Position, positionNode.Position) < radius)
                {
                    nodes[i].IsObstacle = true;
                }
            }
    }


}