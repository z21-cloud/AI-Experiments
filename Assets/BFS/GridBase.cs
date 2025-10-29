using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GridBase : MonoBehaviour
{
    public int weight = 10;
    public int height = 10;
    public float nodeSize = 1;
    public NodeBase startNode;
    public NodeBase goalNode;
    public Vector2Int startPosition = new Vector2Int(0,0);
    public Vector2Int goalPosition = new Vector2Int(9, 9);
    public GameObject nodePrefab;

    private Dictionary<Vector2Int, NodeBase> grid = new Dictionary<Vector2Int, NodeBase>();

    /*private void Start()
    {
        GenerateNode();
    }*/

    public void GenerateNode()
    {
        grid.Clear();
        for (int x = 0; x < weight; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                NodeBase newNode = new NodeBase(pos);
                grid[pos] = newNode;

                Vector3 positionNode = new Vector3(x, y);
                GameObject viewObj = Instantiate(nodePrefab, positionNode, Quaternion.identity);
                NodeView view = viewObj.GetComponent<NodeView>();
                view.Init(newNode, Color.white);
            }
        }

        foreach(NodeBase node in grid.Values)
        {
            AddHeighbors(node);
        }

        startNode = GetNodeAt(startPosition);
        goalNode = GetNodeAt(goalPosition);

        if (startNode != null) startNode.view.SetColor(Color.blue);
        if (goalNode != null) goalNode.view.SetColor(Color.red);
    }

    private void AddHeighbors(NodeBase node)
    {
        Vector2Int[] direcions = {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left
        };

        foreach(Vector2Int dir in direcions)
        {
            Vector2Int neighborPos = node.nodePosition + dir;
            if(grid.ContainsKey(neighborPos))
            {
                node.neighbors.Add(grid[neighborPos]);
            }
        }
    }

    public NodeBase GetNodeAt(Vector2Int pos)
    {
        grid.TryGetValue(pos, out NodeBase node);
        return node;
    }
}
