using System.Collections.Generic;

// A* pathfinding algorithm
public class AStar
{
    /**
     * Returns a path hashset of floor nodes from FloorNode A to B.
     * @param   startNode the first FloorNode in question
     * @param   endNode   the second FloorNode in question
     * @param   exclusion the hashset of floor nodes that should be ignored while pathfinding
     * @return  a hashset of floor nodes
     */
    public HashSet<FloorNode> FindPath(FloorNode startNode, FloorNode endNode, HashSet<FloorNode> exclusion)
    {
        List<FloorNode> toSearch = new List<FloorNode>();
        HashSet<FloorNode> processed = new HashSet<FloorNode>();
        processed.UnionWith(exclusion);
        toSearch.Add(startNode);

        while (toSearch.Count > 0)
        {
            FloorNode current = toSearch[0];
            for (int i = 0; i < toSearch.Count; i++)
            {
                if (toSearch[i].GetFCost() < current.GetFCost() || toSearch[i].GetFCost() == current.GetFCost() && toSearch[i].GetHCost() < current.GetHCost())
                {
                    current = toSearch[i];
                }
            }

            toSearch.Remove(current);
            processed.Add(current);

            if (current == endNode)
            {
                HashSet<FloorNode> path = new HashSet<FloorNode>();
                FloorNode cur = endNode;
                while (cur != startNode)
                {
                    path.Add(cur);
                    cur = cur.GetPathParent();
                }
                path.Add(startNode);
                return path;
            }

            foreach (FloorNode neighbor in current.GetNeighbors())
            {
                if (processed.Contains(neighbor))
                {
                    continue;
                }

                int newMoveCost = current.GetGCost() + current.DistTo(neighbor);
                if (newMoveCost < neighbor.GetGCost() || !toSearch.Contains(neighbor))
                {
                    neighbor.SetGCost(newMoveCost);
                    neighbor.SetHCost(neighbor.DistTo(endNode));
                    neighbor.SetPathParent(current);
                    if (!toSearch.Contains(neighbor))
                    {
                        toSearch.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }
}
