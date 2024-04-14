using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

/**
 * A specialized binary space partitioning algorithm using a special kind of tree.
 */
public class BSP<T> where T : Room
{
    private Node<T> root;
    private Random partitionRandom;
    private Vector2Int minSplitSize;

    public BSP(T _root, Vector2Int _minSplitSize)
    {
        root = new Node<T>(_root);
        partitionRandom = new Random(Guid.NewGuid().GetHashCode());
        minSplitSize = _minSplitSize;
        Subdivide();
    }

    public HashSet<T> GetLeaves()
    {
        if (root == null)
        {
            return null;
        }
        return root.GetLeaves();
    }

    private void Subdivide()
    {
        Queue<Node<T>> nodeQ = new Queue<Node<T>>();
        nodeQ.Enqueue(root);

        while (nodeQ.Count > 0)
        {
            Node<T> current = nodeQ.Dequeue();
            Vector2Int currentSize = current.room.GetMaxSize();
            if (currentSize.x < minSplitSize.x || currentSize.y < minSplitSize.y)
            {
                continue;
            }

            int randSplitDir = partitionRandom.Next(0, 2);
            if (randSplitDir == 0)
            {
                if (currentSize.y >= minSplitSize.y * 2)
                {
                    SplitHorizontal(current, minSplitSize.y, nodeQ);
                }
                else if (currentSize.x >= minSplitSize.x * 2)
                {
                    SplitVertical(current, minSplitSize.x, nodeQ);
                }
            }
            else
            {
                if (currentSize.x >= minSplitSize.x * 2)
                {
                    SplitVertical(current, minSplitSize.x, nodeQ);
                }
                else if (currentSize.y >= minSplitSize.y * 2)
                {
                    SplitHorizontal(current, minSplitSize.y, nodeQ);
                }
            }
        }
    }

    /**
     * A vertical split means the cutting line is drawn vertically.
     */
    private void SplitVertical(Node<T> node, int minX, Queue<Node<T>> nodeQ)
    {
        Room room = node.room;
        Vector2Int roomSize = room.GetMaxSize();
        int rand = partitionRandom.Next(minX, roomSize.x - minX + 1);
        Vector2Int lowerLeft = room.GetLowerLeftPos();
        Room left = new Room(lowerLeft, new Vector2Int(rand + 1, roomSize.y));
        Room right = new Room(lowerLeft + new Vector2Int(rand + 1, 0), new Vector2Int(roomSize.x - rand - 1, roomSize.y));
        node.left = new Node<T>((T)left);
        node.right = new Node<T>((T)right);

        nodeQ.Enqueue(node.left);
        nodeQ.Enqueue(node.right);
    }

    /**
     * A horizontal split means the cutting line is drawn horizontally.
     */
    private void SplitHorizontal(Node<T> node, int minY, Queue<Node<T>> nodeQ)
    {
        Room room = node.room;
        Vector2Int roomSize = room.GetMaxSize();
        int rand = partitionRandom.Next(minY, roomSize.y - minY + 1);
        Vector2Int lowerLeft = room.GetLowerLeftPos();
        Room left = new Room(lowerLeft, new Vector2Int(roomSize.x, rand + 1));
        Room right = new Room(lowerLeft + new Vector2Int(0, rand + 1), new Vector2Int(roomSize.x, roomSize.y - rand - 1));
        node.left = new Node<T>((T)left);
        node.right = new Node<T>((T)right);

        nodeQ.Enqueue(node.left);
        nodeQ.Enqueue(node.right);
    }

    /**
     * A private Node class which contains the recursive GetLeaves method.
     */
    class Node<D> where D : Room
    {
        public D room;
        public Node<D> left;
        public Node<D> right;

        public Node(D _room)
        {
            room = _room;
            left = null;
            right = null;
        }

        public HashSet<D> GetLeaves()
        {
            HashSet<D> children = new HashSet<D>();
            if (left != null)
            {
                children.UnionWith(left.GetLeaves());
            }
            if (right != null)
            {
                children.UnionWith(right.GetLeaves());
            }
            if (left == null && right == null)
            {
                children.Add(room);
            }
            return children;
        }
    }
}
