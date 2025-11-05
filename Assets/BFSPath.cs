using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BFSPath : MonoBehaviour
{
    public List<Node> SearchPath(Node startNode, Node goalNode)
    {
        if (startNode == null || goalNode == null)
        {
            Debug.LogWarning("Start or goal is null");
            return null;
        }

        if (startNode == goalNode)
            return new List<Node> { startNode };

        Queue<Node> queue = new Queue<Node>();
        HashSet<Node> discovered = new HashSet<Node>();

        queue.Enqueue(startNode);
        discovered.Add(startNode);
        startNode.parent = null;

        while (queue.Count > 0)
        {
            Node current = queue.Dequeue();

            if (current == goalNode) 
                return CreatePath(current);

            if (!current.IsAvailable() && current != startNode) continue;

            foreach (Node neighbor in current.neighbors)
            {
                if (discovered.Contains(neighbor)) continue;

                if(neighbor.IsAvailable() && neighbor.occupant == Node.Occupant.None)
                {
                    neighbor.parent = current;
                    discovered.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        return null;
    }

    private List<Node> CreatePath(Node current)
    {
        List<Node> path = new List<Node>();
        Node temp = current;
        while(temp != null)
        {
            path.Add(temp);
            temp = temp.parent;
        }
        path.Reverse();
        return path;
    }
}
