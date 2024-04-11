using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] private float tileDiameter;
    private float tileRadius;

    private FloorNode[,] dungeonFloor;
    [SerializeField] private Vector2Int dungeonSize;
    [SerializeField] private Vector2Int minRoomSize;
    private Vector3 worldBottomLeft;

    private Random dungeonRandom;
    private BSP<Room> roomPartition;
    private AStar pathFinder;
    private Prims prims;
    private List<Room> roomsList;

    private HashSet<FloorNode> hallwaysTileSet;
    private HashSet<FloorNode> roomsTileSet;
    private HashSet<GameObject> wallsTileSet;
    private HashSet<GameObject> ceilingsTileSet;

    [SerializeField] private GameObject floorTile;
    [SerializeField] private GameObject wallTile;
    [SerializeField] private GameObject ceilingTile;
    private List<GameObject> worldTiles;

    [Range(0, 100)] [SerializeField] private int randomRoomConnectionChance;
    [Range(0, 100)] [SerializeField] private int farthestRoomConnectionChance;
    [Range(0, 100)] [SerializeField] private int secondRoomConnectionChance;

    [SerializeField] private bool generateWalls;
    [SerializeField] private bool generateCeiling;

    private void Start()
    {
        worldTiles = new List<GameObject>();
        pathFinder = new AStar();
        tileRadius = tileDiameter / 2;
        dungeonRandom = new Random(Guid.NewGuid().GetHashCode());

        roomsTileSet = new HashSet<FloorNode>();
        hallwaysTileSet = new HashSet<FloorNode>();
        wallsTileSet = new HashSet<GameObject>();
        ceilingsTileSet = new HashSet<GameObject>();
        dungeonFloor = new FloorNode[dungeonSize.x, dungeonSize.y];
        roomsList = new List<Room>();


        CreateFloor();
        LinkRooms();
        CreateHallways();
        InstantiateFloor();
        if (generateWalls)
        {
            InstantiateWalls();
        }
        if (generateCeiling)
        {
            InstantiateCeiling();
        }
    }

    private void InstantiateFloor()
    {
        for (int z = 0; z < dungeonSize.y; z++)
        {
            for (int x = 0; x < dungeonSize.x; x++)
            {
                if (!dungeonFloor[x, z].GetWalkable())
                {
                    continue;
                }
                GameObject obj = Instantiate(floorTile, dungeonFloor[x, z].GetWorldPos(), Quaternion.identity);
                worldTiles.Add(obj);
                obj.transform.SetParent(transform);
            }
        }
        hallwaysTileSet.ExceptWith(roomsTileSet);
    }

    private void InstantiateCeiling()
    {
        for (int z = 0; z < dungeonSize.y; z++)
        {
            for (int x = 0; x < dungeonSize.x; x++)
            {
                if (!dungeonFloor[x, z].GetWalkable())
                {
                    continue;
                }
                GameObject obj = Instantiate(ceilingTile, dungeonFloor[x, z].GetWorldPos() + new Vector3(0, tileDiameter, 0), Quaternion.Euler(180, 0, 0));
                worldTiles.Add(obj);
                ceilingsTileSet.Add(obj);
                obj.transform.SetParent(transform);
            }
        }
    }

    private void InstantiateWalls()
    {
        for (int z = 0; z < dungeonSize.y; z++)
        {
            for (int x = 0; x < dungeonSize.x; x++)
            {
                if (!dungeonFloor[x, z].GetWalkable())
                {
                    continue;
                }
                Vector3 currentWorldPos = dungeonFloor[x, z].GetWorldPos();
                bool n = true;
                bool e = true;
                bool s = true;
                bool w = true;
                foreach (FloorNode neighbor in dungeonFloor[x, z].GetNeighbors())
                {
                    if (!neighbor.GetWalkable())
                    {
                        continue;
                    }

                    Vector3 diff = neighbor.GetWorldPos() - currentWorldPos;
                    if (diff == new Vector3(tileDiameter, 0, 0))
                    {
                        e = false;
                    }
                    else if (diff == new Vector3(-tileDiameter, 0, 0))
                    {
                        w = false;
                    }
                    else if (diff == new Vector3(0, 0, tileDiameter))
                    {
                        n = false;
                    }
                    else if (diff == new Vector3(0, 0, -tileDiameter))
                    {
                        s = false;
                    }
                }
                if (n)
                {
                    GameObject obj = Instantiate(wallTile, currentWorldPos + new Vector3(0, tileRadius, tileRadius), Quaternion.Euler(-90, 0, 0));
                    worldTiles.Add(obj);
                    obj.transform.SetParent(transform);
                    wallsTileSet.Add(obj);
                }
                if (e)
                {
                    GameObject obj = Instantiate(wallTile, currentWorldPos + new Vector3(tileRadius, tileRadius, 0), Quaternion.Euler(90, -90, 0));
                    worldTiles.Add(obj);
                    obj.transform.SetParent(transform);
                    wallsTileSet.Add(obj);
                }
                if (s)
                {
                    GameObject obj = Instantiate(wallTile, currentWorldPos + new Vector3(0, tileRadius, -tileRadius), Quaternion.Euler(90, 0, 0));
                    worldTiles.Add(obj);
                    obj.transform.SetParent(transform);
                    wallsTileSet.Add(obj);
                }
                if (w)
                {
                    GameObject obj = Instantiate(wallTile, currentWorldPos + new Vector3(-tileRadius, tileRadius, 0), Quaternion.Euler(90, 90, 0));
                    worldTiles.Add(obj);
                    obj.transform.SetParent(transform);
                    wallsTileSet.Add(obj);
                }
            }
        }
    }

    private void CreateFloor()
    {
        worldBottomLeft = transform.position - Vector3.right * dungeonSize.x / 2f - Vector3.forward * dungeonSize.y / 2f;
        worldBottomLeft *= tileDiameter;
        worldBottomLeft += new Vector3(tileRadius, 0, tileRadius);

        roomPartition = new BSP<Room>(new Room(new Vector2Int(0, 0), new Vector2Int(dungeonSize.x, dungeonSize.y)), minRoomSize);
        foreach (Room room in roomPartition.GetLeaves())
        {
            roomsList.Add(room);
        }

        foreach (Room room in roomsList)
        {
            HashSet<Vector2Int> internalPos = room.GetRoomTilePositions();
            Vector2Int lowerLeft = room.GetLowerLeftPos();
            Vector2Int maxSize = room.GetMaxSize();
            for (int z = lowerLeft.y; z < lowerLeft.y + maxSize.y; z++)
            {
                for (int x = lowerLeft.x; x < lowerLeft.x + maxSize.x; x++)
                {
                    bool state = false;
                    if (internalPos.Contains(new Vector2Int(x, z)))
                    {
                        state = true;
                    }
                    dungeonFloor[x, z] = new FloorNode(worldBottomLeft + tileDiameter * new Vector3(x, 0, z), state, new Vector2Int(x, z));
                    dungeonFloor[x, z].SetParentRoom(room);
                    if (state)
                    {
                        room.AddInternalTile(dungeonFloor[x, z]);
                    }
                    roomsTileSet.UnionWith(room.GetInternalTiles());
                }
            }
        }

        // Calculate approximation
        foreach (Room r in roomsList)
        {
            r.Approximate();
            Vector2 lower = r.GetApproximationLowerLeft();
            Vector2 upper = r.GetApproximationUpperRight();
            for (float z = lower.y; z <= upper.y; z += tileDiameter)
            {
                for (float x = lower.x; x <= upper.x; x += tileDiameter)
                {
                    r.AddApproximationTile(NodeFromWorldPoint(new Vector3(x, 0, z)));
                }
            }
        }

        foreach (FloorNode node in dungeonFloor)
        {
            CacheNeighbors(node);
            CacheCorners(node);
        }
        foreach (Room room in roomsList)
        {
            room.CacheBorderTiles();
        }
    }

    private void LinkRooms()
    {
        List<FloorNode> centers = new List<FloorNode>();
        foreach (Room room in roomsList)
        {
            Vector2Int roomCenter = room.GetCenterPos();
            Vector3 pos = worldBottomLeft + tileDiameter * new Vector3(roomCenter.x, 0, roomCenter.y);
            centers.Add(NodeFromWorldPoint(pos));
        }
        prims = new Prims(centers, randomRoomConnectionChance, farthestRoomConnectionChance, secondRoomConnectionChance);
    }

    private void CreateHallways()
    {
        List<RoomLink> roomLinks = prims.GetRoomLinks();
        for (int i = 0; i < roomLinks.Count; i++)
        {
            HashSet<FloorNode> exclusion = new HashSet<FloorNode>();
            foreach (Room room in roomsList)
            {
                exclusion.UnionWith(room.GetRoomApproximation());
            }
            FloorNode firstCenter = roomLinks[i].GetFirstCenter();
            FloorNode secondCenter = roomLinks[i].GetSecondCenter();
            // rare case in which the dungeon only has one room
            if (secondCenter == null)
            {
                return;
            }
            exclusion.ExceptWith(firstCenter.GetParentRoom().GetRoomApproximation());
            exclusion.ExceptWith(secondCenter.GetParentRoom().GetRoomApproximation());
            HashSet<FloorNode> path = pathFinder.FindPath(firstCenter, secondCenter, exclusion);
            hallwaysTileSet.UnionWith(path);
        }

        foreach (FloorNode tile in hallwaysTileSet)
        {
            tile.SetWalkable(true);
        }
    }

    public void CacheNeighbors(FloorNode node)
    {
        List<FloorNode> list = new List<FloorNode>();
        int targetX = node.GetGridPos().x;
        int targetY = node.GetGridPos().y;
        try
        {
            list.Add(dungeonFloor[targetX - 1, targetY]);
        }
        catch { }
        try
        {
            list.Add(dungeonFloor[targetX + 1, targetY]);
        }
        catch { }
        try
        {
            list.Add(dungeonFloor[targetX, targetY - 1]);
        }
        catch { }
        try
        {
            list.Add(dungeonFloor[targetX, targetY + 1]);
        }
        catch { }
        node.SetNeighbors(list);
    }

    private void CacheCorners(FloorNode node)
    {
        List<FloorNode> list = new List<FloorNode>();
        int targetX = node.GetGridPos().x;
        int targetY = node.GetGridPos().y;
        try
        {
            list.Add(dungeonFloor[targetX - 1, targetY - 1]);
        }
        catch { }
        try
        {
            list.Add(dungeonFloor[targetX - 1, targetY + 1]);
        }
        catch { }
        try
        {
            list.Add(dungeonFloor[targetX + 1, targetY - 1]);
        }
        catch { }
        try
        {
            list.Add(dungeonFloor[targetX + 1, targetY + 1]);
        }
        catch { }
        node.SetCorners(list);
    }

    private FloorNode NodeFromWorldPoint(Vector3 pos)
    {
        float percentX = (pos.x / tileDiameter + dungeonSize.x / 2) / dungeonSize.x;
        float percentY = (pos.z / tileDiameter + dungeonSize.y / 2) / dungeonSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        int x = Mathf.RoundToInt(percentX * (dungeonSize.x - 1));
        int y = Mathf.RoundToInt(percentY * (dungeonSize.y - 1));
        return dungeonFloor[x, y];
    }
}
