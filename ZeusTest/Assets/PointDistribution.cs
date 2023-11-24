using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GraphNode
{
    public Vector3 Position { get; set; }
    public bool IsObstacle { get; set; }

    private void CheckGroundAndAdaptPosition()
    {
        RaycastHit hit;
        //Raycast from above the point to the center of the sphere (0,0,0)
        if (Physics.Raycast(Position*1.1f, (Vector3.zero - Position*1.1f), out hit, 2f)) // 2f : experimental
        {
            if (hit.collider.gameObject.CompareTag("Water"))
            {
                IsObstacle = true;
            }
            else if (hit.collider.gameObject.CompareTag("Ground")) // Adapt the position to the ground
            {
                Position = hit.point;
            }
        }
        else // If the raycast doesn't hit anything, it's an obstacle
        {
            IsObstacle = true;
        }
    }
    public GraphNode(Vector3 position, bool AdaptPosition = false)
    {
        Position = position;
        IsObstacle = false; // Default to not being an obstacle
        if(AdaptPosition)
            CheckGroundAndAdaptPosition();
    }
}
public class PointDistribution : MonoBehaviour
{
    [SerializeField] private float sphereScale = 0.001f;
    [SerializeField] private int numberOfPointsOnSphere = 128;
    [SerializeField] private GameObject sphereParent;
    private GraphNode[] _nodesClicked = new GraphNode[2];
    private float _distanceBtw2Points;
    private GraphNode[] nodes;
    private Dictionary<GraphNode, List<GraphNode>> graph;
    
    private List<GameObject> uspheres = new List<GameObject>();
    private float scaling;

    void Start()
    {
        scaling = transform.localScale.x;
        nodes = PointsOnSphere(numberOfPointsOnSphere);
        
        _distanceBtw2Points = Vector3.Distance(nodes[0].Position, nodes[1].Position);
         graph = CreateGraph(nodes, 1.5f * _distanceBtw2Points);
        
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
                    
                    Debug.Log("Clicked on sphere " + _nodesClicked[1].Position);
                    if (_nodesClicked[0] != null && _nodesClicked[1] != null)
                    {
                        CalculatePath(_nodesClicked[0], _nodesClicked[1]);
                    }
                }
            }
        }
    }
    
    void CalculatePath(GraphNode startNode, GraphNode endNode)
    {        
        AStar aStar = new AStar();
        
        List<Vector3> path = aStar.FindPath(graph, startNode, endNode);
        
        for (int i = 0; i < path.Count - 1; i++)
        {
            Debug.DrawLine(path[i], path[i + 1], Color.red, 5f);
        }
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

            upts.Add(new GraphNode(new Vector3(x, y, z) * scaling, true));
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
}