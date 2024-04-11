using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Prim's algorithm finds the minimum spanning tree between rooms
 */
public class Prims
{
    private List<RoomLink> roomLinks;
    private List<FloorNode> allNodes;

    private int randomFirstConnectionChance;
    private int farthestConnectionChance;
    private int secondConnectionChance;

    public Prims(List<FloorNode> _allNodes, int _randFirstConChance, int _farthestConnectionChance, int _secondConnectionChance)
    {
        roomLinks = new List<RoomLink>();
        allNodes = _allNodes;
        randomFirstConnectionChance = _randFirstConChance;
        farthestConnectionChance = _farthestConnectionChance;
        secondConnectionChance = _secondConnectionChance;
        MST();
    }

    public List<RoomLink> GetRoomLinks()
    {
        return roomLinks;
    }

    /**
     * Create room links between all given rooms;
     * Default connections are to the closest unconnected room;
     * However can also use change to connect to the farthest unconnected room;
     * Or a random unconnected room;
     * In addition, can also get a second connection to any unconnected room
     */
    private void MST()
    {
        HashSet<FloorNode> visitedNodes = new HashSet<FloorNode>();

        FloorNode current = allNodes[Random.Range(0, allNodes.Count)];
        while (visitedNodes.Count < allNodes.Count)
        {
            HashSet<FloorNode> possibles = new HashSet<FloorNode>(allNodes);
            visitedNodes.Add(current);
            possibles.ExceptWith(visitedNodes);

            FloorNode other;
            int rand = Random.Range(0, 100);
            if (rand < randomFirstConnectionChance)
            {
                other = RandomNode(possibles);
            }
            else if (rand < randomFirstConnectionChance + farthestConnectionChance)
            {
                other = FarthestNode(current, possibles);
            }
            else
            {
                other = ClosestNode(current, possibles);
            }
            visitedNodes.Add(other);

            RoomLink link = new RoomLink(current, other);
            roomLinks.Add(link);

            if (Random.Range(0, 100) < secondConnectionChance)
            {
                RoomLink anotherLink = new RoomLink(current, RandomNode(possibles));
                roomLinks.Add(anotherLink);
            }

            current = other;
        }
    }

    private FloorNode ClosestNode(FloorNode node, HashSet<FloorNode> possibles)
    {
        FloorNode closest = null;
        foreach (FloorNode target in possibles)
        {
            if (closest == null)
            {
                closest = target;
            }

            if (Vector3.Distance(target.GetWorldPos(), node.GetWorldPos()) < Vector3.Distance(closest.GetWorldPos(), node.GetWorldPos()))
            {
                closest = target;
            }
        }
        return closest;
    }

    private FloorNode FarthestNode(FloorNode node, HashSet<FloorNode> possibles)
    {
        FloorNode farthest = null;
        foreach (FloorNode target in possibles)
        {
            if (farthest == null)
            {
                farthest = target;
            }

            if (Vector3.Distance(target.GetWorldPos(), node.GetWorldPos()) > Vector3.Distance(farthest.GetWorldPos(), node.GetWorldPos()))
            {
                farthest = target;
            }
        }
        return farthest;
    }

    private FloorNode RandomNode(HashSet<FloorNode> possibles)
    {
        int i = Random.Range(0, possibles.Count);
        int counter = 0;
        foreach (FloorNode target in possibles)
        {
            if (counter == i)
            {
                return target;
            }
            counter++;
        }
        return null;
    }
}
