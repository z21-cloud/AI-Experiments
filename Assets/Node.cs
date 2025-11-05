using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node
{
    public Vector2Int gridIndex;
    public Vector3 worldPosition;
    public bool isWalkable;
    public Node parent;
    public List<Node> neighbors;
    public NodeView view;

    public enum Occupant { None, Player, NPC };
    public Occupant occupant = Occupant.None;

    public Node(Vector2Int index, Vector3 position, bool isWalkable = false)
    {
        gridIndex = index;
        worldPosition = position;
        this.isWalkable = isWalkable;
        neighbors = new List<Node>();
        parent = null;
    }

    public bool IsAvailable()
    {
        return isWalkable && occupant == Occupant.None;
    }
}
