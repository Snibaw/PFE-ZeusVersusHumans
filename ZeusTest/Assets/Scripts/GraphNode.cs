using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GraphNode
{
    public Vector3 Position { get; set; }
    public bool IsObstacle { get; set; }

    public bool IsWater { get; set; }
    public int index { get; set; }

    private void CheckGroundAndAdaptPosition()
    {
        RaycastHit hit;
        //Raycast from above the point to the center of the sphere (0,0,0)
        if (Physics.Raycast(Position, (Vector3.zero - Position), out hit, Position.magnitude)) // 2f : experimental
        {
            Position = hit.point;
            if (hit.collider.gameObject.CompareTag("Water"))
            {
                IsWater = true;
            }
        }
        else // If the raycast doesn't hit anything, it's an obstacle
        {
            IsObstacle = true;
        }
    }
    public GraphNode(Vector3 position, int _index, bool AdaptPosition = false)
    {
        Position = position;
        index = _index;
        IsObstacle = false; // Default to not being an obstacle
        if (AdaptPosition)
            CheckGroundAndAdaptPosition();
    }
}
