using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GraphNode
{
    public Vector3 Position { get; }
    public bool IsObstacle { get; set; }

    public GraphNode(Vector3 position)
    {
        Position = position;
        IsObstacle = false; // Default to not being an obstacle
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

    void Start()
    {
        float scaling = transform.localScale.x;
        nodes = PointsOnSphere(numberOfPointsOnSphere);
        
        _distanceBtw2Points = Vector3.Distance(nodes[0].Position, nodes[1].Position);
         graph = CreateGraph(nodes, 1.5f * _distanceBtw2Points);
         
         GraphNode startNode = nodes[Random.Range(0, nodes.Length - 1)];
         GraphNode endNode = nodes[Random.Range(0, nodes.Length - 1)];
         
         AStar aStar = new AStar();
         
         List<Vector3> path = aStar.FindPath(graph, startNode, endNode);
        
        foreach (GraphNode point in nodes)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.parent = sphereParent.transform;
            sphere.transform.position = point.Position * scaling;
            sphere.transform.localScale = new Vector3(sphereScale, sphereScale, sphereScale);
            sphere.tag = "Sphere";

            if (path.Contains(point.Position))
            {
                sphere.GetComponent<Renderer>().material.color = Color.red;
                sphere.transform.localScale *= 3;
            }
            
            uspheres.Add(sphere);
        }
    }
    
    void CalculatePath(GraphNode startNode, GraphNode endNode)
    {        
        AStar aStar = new AStar();

        // Call the FindPath method on the AStar instance
        List<Vector3> path = aStar.FindPath(graph, startNode, endNode);
        Debug.Log("FindPath");
        
        // Draw the path
        for (int i = 0; i < path.Count - 1; i++)
        {
            Debug.DrawLine(path[i], path[i + 1], Color.red, 10f);
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

            upts.Add(new GraphNode(new Vector3(x, y, z)));
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