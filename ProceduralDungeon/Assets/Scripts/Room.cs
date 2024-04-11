using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Rooms are space allocations for floor tiles to be contained in.
 * Rooms use random walk to create floor.
 */
public class Room
{
    private Vector2Int lowerLeft;
    private Vector2Int maxSize;
    private Vector2Int center;

    // these two variables indicate internal cutoffs for room random walk growth
    // the primary purpose being to guarantee hallway space
    private Vector2Int minInternalRoom;
    private Vector2Int maxInternalRoom;

    // internal tile positions is the hashset of actual walkable tiles in the room
    private HashSet<Vector2Int> internalTilePositions;
    private HashSet<FloorNode> roomFloorTiles;
    private HashSet<FloorNode> borderFloorTiles;

    // room approximation contains a rectangle of all walkable room tiles
    private HashSet<FloorNode> roomApproximation;
    private Vector2 approximationLowerLeft;
    private Vector2 approximationUpperRight;

    public Room(Vector2Int _lowerLeft, Vector2Int _maxSize)
    {
        lowerLeft = _lowerLeft;
        maxSize = _maxSize;

        // for max internal position, an additional node is taken away to ensure hallway space
        minInternalRoom = new Vector2Int(lowerLeft.x, lowerLeft.y);
        maxInternalRoom = new Vector2Int(lowerLeft.x + maxSize.x - 2, lowerLeft.y + maxSize.y - 2);

        center = lowerLeft + new Vector2Int(maxSize.x / 2, maxSize.y / 2);

        roomFloorTiles = new HashSet<FloorNode>();
        internalTilePositions = RandomWalk(center, maxSize.x, maxSize.y);
        borderFloorTiles = new HashSet<FloorNode>();
        roomApproximation = new HashSet<FloorNode>();
    }

    public Vector2Int GetLowerLeftPos()
    {
        return lowerLeft;
    }

    public Vector2Int GetMaxSize()
    {
        return maxSize;
    }

    public Vector2Int GetCenterPos()
    {
        return center;
    }

    public HashSet<Vector2Int> GetRoomTilePositions()
    {
        return internalTilePositions;
    }

    public HashSet<FloorNode> GetInternalTiles()
    {
        return roomFloorTiles;
    }

    public HashSet<FloorNode> GetBorderTiles()
    {
        return borderFloorTiles;
    }

    public void AddInternalTile(FloorNode tile)
    {
        roomFloorTiles.Add(tile);
    }

    public void AddApproximationTile(FloorNode tile)
    {
        roomApproximation.Add(tile);
    }

    public HashSet<FloorNode> GetRoomApproximation()
    {
        return roomApproximation;
    }

    public Vector2 GetApproximationLowerLeft()
    {
        return approximationLowerLeft;
    }

    public Vector2 GetApproximationUpperRight()
    {
        return approximationUpperRight;
    }

    public void Approximate()
    {
        float minX = int.MaxValue, maxX = int.MinValue, minZ = int.MaxValue, maxZ = int.MinValue;
        foreach (FloorNode node in roomFloorTiles)
        {
            Vector3 pos = node.GetWorldPos();
            if (pos.x < minX)
            {
                minX = pos.x;
            }
            if (pos.x > maxX)
            {
                maxX = pos.x;
            }
            if (pos.z < minZ)
            {
                minZ = pos.z;
            }
            if (pos.z > maxZ)
            {
                maxZ = pos.z;
            }
        }
        approximationLowerLeft = new Vector2(minX - 4, minZ - 4);
        approximationUpperRight = new Vector2(maxX + 4, maxZ + 4);
    }

    public void CacheBorderTiles()
    {
        foreach (FloorNode floorTile in roomFloorTiles)
        {
            foreach (FloorNode neighbor in floorTile.GetNeighbors())
            {
                if (!neighbor.GetWalkable())
                {
                    borderFloorTiles.Add(neighbor);
                }
            }
            foreach (FloorNode corner in floorTile.GetCorners())
            {
                if (!corner.GetWalkable())
                {
                    borderFloorTiles.Add(corner);
                }
            }
        }
    }

    private HashSet<Vector2Int> RandomWalk(Vector2Int startPos, int iterations, int walkLength)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        for (int k = 0; k < iterations; k++)
        {
            Vector2Int lastPos = startPos;
            for (int i = 0; i < walkLength; i++)
            {
                Vector2Int newPos = lastPos + RandomWalkDir();
                if (newPos.x > minInternalRoom.x && newPos.x < maxInternalRoom.x && newPos.y > minInternalRoom.y && newPos.y < maxInternalRoom.y)
                {
                    path.Add(newPos);
                    lastPos = newPos;
                }
            }
        }
        return path;
    }

    private Vector2Int RandomWalkDir()
    {
        Vector2Int[] dirs = { new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1) };
        return dirs[Random.Range(0, dirs.Length)];
    }
}
