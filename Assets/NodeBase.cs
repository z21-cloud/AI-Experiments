using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NodeBase
{
    public Vector3 WorldPosition;
    public bool Walkable;
    public NodeBase Connection;
    public float G;
    public float H;
    public float F => G + H;
    public List<NodeBase> Neighbors;

    public NodeBase(Vector3 worldPos, bool walkable)
    {
        WorldPosition = worldPos;
        Walkable = walkable;
        Neighbors = new List<NodeBase>();
    }

    public void SetConnection(NodeBase node) => Connection = node;
    public void SetG(float g) => G = g;
    public void SetH(float h) => H = h;

    public float GetDistance(NodeBase other) => Vector3.Distance(WorldPosition, other.WorldPosition);
}
