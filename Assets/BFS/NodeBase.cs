using UnityEngine;
using System.Collections.Generic;

public class NodeBase
{
    public Vector2Int nodePosition;
    public List<NodeBase> neighbors;
    public bool isObstacle;
    public NodeView view;
    public NodeBase parent;

    public NodeBase(Vector2Int position, bool isObstacle = false)
    {
        nodePosition = position;
        isObstacle = this.isObstacle;
        neighbors = new List<NodeBase>();
        parent = null;
        view = null;
    }
}
