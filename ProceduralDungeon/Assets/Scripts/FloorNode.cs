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
    private bool occupied;

    // caches
    private List<FloorNode> neighbors;
    private FloorNode n;
    private FloorNode e;
    private FloorNode s;
    private FloorNode w;
    private List<FloorNode> corners;
    private FloorNode ne;
    private FloorNode se;
    private FloorNode sw;
    private FloorNode nw;

    // A* Pathfinding
    private FloorNode pathParent;
    private int gCost;
    private int hCost;
    // f cost is calculated in a method

    public FloorNode(Vector3 _worldPos, bool _walkable, Vector2Int _gridPos)
    {
        worldPos = _worldPos;
        walkable = _walkable;
        gridPos = _gridPos;
    }

    public void SetOccupied(bool state)
    {
        occupied = state;
    }

    public bool GetOccupied()
    {
        return occupied;
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

    public void SetN(FloorNode node)
    {
        n = node;
    }

    public void SetE(FloorNode node)
    {
        e = node;
    }

    public void SetS(FloorNode node)
    {
        s = node;
    }

    public void SetW(FloorNode node)
    {
        w = node;
    }

    public List<FloorNode> GetNeighbors()
    {
        return neighbors;
    }

    public FloorNode GetN()
    {
        return n;
    }

    public FloorNode GetE()
    {
        return e;
    }

    public FloorNode GetS()
    {
        return s;
    }

    public FloorNode GetW()
    {
        return w;
    }

    public void SetCorners(List<FloorNode> _corners)
    {
        corners = _corners;
    }

    public void SetNE(FloorNode node)
    {
        ne = node;
    }

    public void SetSE(FloorNode node)
    {
        se = node;
    }

    public void SetNW(FloorNode node)
    {
        nw = node;
    }

    public void SetSW(FloorNode node)
    {
        sw = node;
    }

    public List<FloorNode> GetCorners()
    {
        return corners;
    }

    public FloorNode GetNE()
    {
        return ne;
    }

    public FloorNode GetSE()
    {
        return se;
    }

    public FloorNode GetNW()
    {
        return nw;
    }

    public FloorNode GetSW()
    {
        return sw;
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
