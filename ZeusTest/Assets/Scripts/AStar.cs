using System.Collections.Generic;
using UnityEngine;

public class AStar
{
    public List<Vector3> FindPath(Dictionary<GraphNode, List<GraphNode>> graph, GraphNode startNode, GraphNode goalNode, bool canMoveOnWater)
    {
        PriorityQueue<GraphNode> openSet = new PriorityQueue<GraphNode>();
        HashSet<GraphNode> closedSet = new HashSet<GraphNode>();
        Dictionary<GraphNode, GraphNode> cameFrom = new Dictionary<GraphNode, GraphNode>();
        Dictionary<GraphNode, float> gScore = new Dictionary<GraphNode, float>();
        Dictionary<GraphNode, float> fScore = new Dictionary<GraphNode, float>();
        
        if(startNode.IsObstacle)
        {
            startNode = PointDistribution.instance.FindClosestNodeFree(startNode.Position, canMoveOnWater);
        }

        openSet.Enqueue(startNode, 0);
        if (goalNode.IsObstacle)
        {
            goalNode = PointDistribution.instance.FindClosestNodeFree(goalNode.Position, canMoveOnWater);
        }
        
        gScore[startNode] = 0;
        fScore[startNode] = HeuristicCostEstimate(startNode.Position, goalNode.Position);

        while (openSet.Count > 0)
        {
            GraphNode current = openSet.Dequeue();

            if (current == goalNode)
            {
                return ReconstructPath(cameFrom, current);
            }

            closedSet.Add(current);

            foreach (GraphNode neighbor in graph[current])
            {
                if (closedSet.Contains(neighbor) || neighbor.IsObstacle)
                    continue;
                if (closedSet.Contains(neighbor) || (neighbor.IsWater && !canMoveOnWater))
                    continue;

                float tentativeGScore = gScore[current] + Vector3.Distance(current.Position, neighbor.Position);

                if (!openSet.Contains(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor.Position, goalNode.Position);

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Enqueue(neighbor, fScore[neighbor]);
                    }
                }
            }
        }
        Debug.Log("No path found between " + startNode.Position + " and " + goalNode.Position +"whiche are obstacles?" + startNode.IsObstacle + " " + goalNode.IsObstacle);
        DebugFunction(startNode, goalNode);
        return null;
    }

    private void DebugFunction(GraphNode startNode, GraphNode goalNode)
    {
        foreach (var sphere in PointDistribution.instance.uspheres)
        {
            if (sphere.transform.position == startNode.Position)
            {
                sphere.GetComponent<Renderer>().material.color = Color.red;
            }
            if (sphere.transform.position == goalNode.Position)
            {
                sphere.GetComponent<Renderer>().material.color = Color.green;
            }
        }
    }
private static List<Vector3> ReconstructPath(Dictionary<GraphNode, GraphNode> cameFrom, GraphNode current)
    {
        List<Vector3> path = new List<Vector3>();
        while (cameFrom.ContainsKey(current))
        {
            path.Insert(0, current.Position);
            current = cameFrom[current];
        }

        path.Insert(0, current.Position);
        return path;
    }

    private static float HeuristicCostEstimate(Vector3 start, Vector3 goal)
    {
        // Replace this with your specific heuristic function.
        // Here, we use Euclidean distance as a simple heuristic.
        return Vector3.Distance(start, goal);
    }

    private class PriorityQueue<T>
    {
        private List<T> elements = new List<T>();
        private Dictionary<T, float> priorities = new Dictionary<T, float>();

        public int Count
        {
            get { return elements.Count; }
        }

        public void Enqueue(T item, float priority)
        {
            elements.Add(item);
            priorities[item] = priority;
            elements.Sort((a, b) => priorities[a].CompareTo(priorities[b]));
        }

        public T Dequeue()
        {
            T item = elements[0];
            elements.RemoveAt(0);
            priorities.Remove(item);
            return item;
        }

        public bool Contains(T item)
        {
            return priorities.ContainsKey(item);
        }   
    }
}