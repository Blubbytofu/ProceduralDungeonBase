using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class FloorNode
{
    // identity variables
    private Vector3 worldPos;
    private bool walkable;
    private Vector2Int gridPos;
    private Room parentRoom;

    // caches
    private List<FloorNode> neighbors;
    public FloorNode n;
    public FloorNode e;
    public FloorNode s;
    public FloorNode w;
    private List<FloorNode> corners;
    public FloorNode ne;
    public FloorNode se;
    public FloorNode sw;
    public FloorNode nw;

    // A* Pathfinding
    private FloorNode pathParent;
    private int gCost;
    private int hCost;

    public FloorNode(Vector3 _worldPos, bool _walkable, Vector2Int _gridPos)
    {
        worldPos = _worldPos;
        walkable = _walkable;
        gridPos = _gridPos;
    }

    public Vector3 GetWorldPos()
    {
        return worldPos;
    }

    public Vector2Int GetGridPos()
    {
        return gridPos;
    }

    public void SetWalkable(bool _walkable)
    {
        walkable = _walkable;
    }

    public bool GetWalkable()
    {
        return walkable;
    }

    public void SetNeighbors(List<FloorNode> _neighbors)
    {
        neighbors = _neighbors;
    }

    public List<FloorNode> GetNeighbors()
    {
        return neighbors;
    }

    public void SetCorners(List<FloorNode> _corners)
    {
        corners = _corners;
    }

    public List<FloorNode> GetCorners()
    {
        return corners;
    }

    public void SetParentRoom(Room _parentRoom)
    {
        parentRoom = _parentRoom;
    }

    public Room GetParentRoom()
    {
        return parentRoom;
    }

    public void SetPathParent(FloorNode _pathParent)
    {
        pathParent = _pathParent;
    }

    public FloorNode GetPathParent()
    {
        return pathParent;
    }

    public void SetGCost(int _gCost)
    {
        gCost = _gCost;
    }

    public int GetGCost()
    {
        return gCost;
    }

    public void SetHCost(int _hCost)
    {
        hCost = _hCost;
    }

    public int GetHCost()
    {
        return hCost;
    }

    public int GetFCost()
    {
        return gCost + hCost;
    }

    public int DistTo(FloorNode other)
    {
        return (int)Mathf.Abs(worldPos.x - other.worldPos.x) + (int)Mathf.Abs(worldPos.z - other.worldPos.z);
    }
}
