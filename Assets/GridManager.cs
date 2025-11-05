using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GridManager : MonoBehaviour
{
    public GameObject ground;
    public GameObject nodePrefab;
    public Transform nodesParent;
    public float nodeSize;
    public LayerMask obstacle;
    private Dictionary<Vector2Int, Node> nodes;
    private Vector3 originMin;
    private Vector3 originMax;
    private void Awake()
    {
        nodes = new Dictionary<Vector2Int, Node>();
        Bounds bounds = ground.GetComponent<MeshRenderer>().bounds;
        originMin = bounds.min;
        originMax = bounds.max;

        int countX = Mathf.RoundToInt((originMax.x - originMin.x) / nodeSize);
        int countZ = Mathf.RoundToInt((originMax.z - originMin.z) / nodeSize);

        nodes.Clear();

        for (int x = 0; x < countX; x++)
        {
            for (int z = 0; z < countZ; z++)
            {
                Vector2Int gridIndex = new Vector2Int(x, z);

                float worldX = originMin.x + nodeSize * x + nodeSize / 2;
                float worldZ = originMin.z + nodeSize * z + nodeSize / 2;
                float worldY = originMin.y + .5f;

                Vector3 worldPosition = new Vector3(worldX, worldY, worldZ);

                bool isWalkable = !CheckForObstacle(worldPosition);

                GameObject nodeObject = Instantiate(nodePrefab, worldPosition, Quaternion.identity, nodesParent);
                NodeView view = nodeObject.GetComponent<NodeView>();
                view.SetColor(isWalkable ? Color.green : Color.red);

                Node node = new Node(gridIndex, worldPosition, isWalkable);
                view.Init(node);
                node.view = view;
                nodes.Add(gridIndex, node);
            }
        }

        Debug.Log(nodes.Count);

        Vector2Int[] directions =
        {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left
        };

        foreach (Node neighbor in nodes.Values)
        {
            AddNeighbors(directions, neighbor);
        }
    }

    private void AddNeighbors(Vector2Int[] directions, Node neighbor)
    {
        foreach (Vector2Int direction in directions)
        {
            Vector2Int index = neighbor.gridIndex + direction;
            if (nodes.ContainsKey(index))
                neighbor.neighbors.Add(nodes[index]);
        }
    }

    private bool CheckForObstacle(Vector3 worldPosition)
    {
        Collider[] colliders = Physics.OverlapSphere(worldPosition, nodeSize / 2, obstacle);
        return colliders.Length > 0;
    }

    public Node GetClosestNodeByPosition(Vector3 worldPosition)
    {
        /*int localX = Mathf.RoundToInt((worldPosition.x - originMin.x - nodeSize / 2) / nodeSize);
        int localZ = Mathf.RoundToInt((worldPosition.z - originMin.z - nodeSize / 2) / nodeSize);
        Vector2Int index = new Vector2Int(localX, localZ);
        if (nodes.ContainsKey(index))
        {
            Node temp = nodes[index];
            if (!temp.IsAvailable())
            {
                foreach (Node neighbor in temp.neighbors)
                {
                    if (neighbor.IsAvailable()) return neighbor;
                }
            }
        }
        return null;
        */

        Node closest = null;
        float minDistance = float.MaxValue;
        foreach (Node node in nodes.Values)
        {
            float distance = Vector3.Distance(node.worldPosition, worldPosition);
            if(distance < minDistance && node.IsAvailable())
            {
                minDistance = distance;
                closest = node;
            }
        }
        return closest;
    }
    /*private void OnDrawGizmos()
    {
        if (nodes == null) return;
        foreach(var kvp in nodes)
        {
            Node node = kvp.Value;
            Gizmos.color = node.isWalkable ? Color.green : Color.red;
            Gizmos.DrawWireCube(node.worldPosition, Vector3.one * nodeSize * .8f);
        }
    }*/
}
