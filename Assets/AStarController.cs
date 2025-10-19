using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class AStarController : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 10;
    public int height = 10;
    public float nodeSize = 1f;
    public LayerMask obstacleMask;

    [Header("Movement Settings")]
    public Transform mover;
    public float moveSpeed = 5f;

    [Header("Target Settings")]
    [Range(-30, 30)] public int targetX = 9;
    [Range(-30, 30)] public int targetZ = 9;

    private NodeBase[,] nodes;

    void Start()
    {
        GenerateGrid();
        AssignNeighbors();

        NodeBase startNode = nodes[0, 0];
        NodeBase targetNode = nodes[Mathf.Clamp(targetX, 0, width - 1), Mathf.Clamp(targetZ, 0, height - 1)];

        var path = FindPath(startNode, targetNode);

        if (path != null)
        {
            DrawPath(path);
            StartCoroutine(MoveAlongPath(path, mover, moveSpeed));
        }
    }

    void GenerateGrid()
    {
        nodes = new NodeBase[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 worldPos = new Vector3(x * nodeSize, 0, z * nodeSize);
                bool walkable = !Physics.CheckSphere(worldPos, nodeSize * 0.4f, obstacleMask);
                nodes[x, z] = new NodeBase(worldPos, walkable);
            }
        }
    }

    void AssignNeighbors()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                List<NodeBase> neighbors = new List<NodeBase>();
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dz = -1; dz <= 1; dz++)
                    {
                        if (dx == 0 && dz == 0) continue;
                        int nx = x + dx;
                        int nz = z + dz;
                        if (nx >= 0 && nx < width && nz >= 0 && nz < height)
                            neighbors.Add(nodes[nx, nz]);
                    }
                }
                nodes[x, z].Neighbors = neighbors;
            }
        }
    }

    public List<NodeBase> FindPath(NodeBase startNode, NodeBase targetNode)
    {
        var toSearch = new List<NodeBase>() { startNode };
        var processed = new List<NodeBase>();

        while (toSearch.Any())
        {
            NodeBase current = toSearch[0];
            foreach (var t in toSearch)
            {
                if (t.F < current.F || (t.F == current.F && t.H < current.H))
                    current = t;
            }

            processed.Add(current);
            toSearch.Remove(current);

            if (current == targetNode)
            {
                var currentPathTile = targetNode;
                var path = new List<NodeBase>();
                while (currentPathTile != startNode)
                {
                    path.Add(currentPathTile);
                    currentPathTile = currentPathTile.Connection;
                }
                path.Reverse();
                return path;
            }

            foreach (var neighbor in current.Neighbors.Where(t => t.Walkable && !processed.Contains(t)))
            {
                bool inSearch = toSearch.Contains(neighbor);
                float costToNeighbor = current.G + current.GetDistance(neighbor);

                if (!inSearch || costToNeighbor < neighbor.G)
                {
                    neighbor.SetG(costToNeighbor);
                    neighbor.SetConnection(current);

                    if (!inSearch)
                    {
                        neighbor.SetH(neighbor.GetDistance(targetNode));
                        toSearch.Add(neighbor);
                    }
                }
            }
        }
        return null;
    }

    void DrawPath(List<NodeBase> path)
    {
        for (int i = 0; i < path.Count - 1; i++)
        {
            Debug.DrawLine(path[i].WorldPosition + Vector3.up * 0.1f, path[i + 1].WorldPosition + Vector3.up * 0.1f, Color.green, 10f);
        }
    }

    IEnumerator MoveAlongPath(List<NodeBase> path, Transform obj, float speed)
    {
        foreach (var node in path)
        {
            while (Vector3.Distance(obj.position, node.WorldPosition) > 0.1f)
            {
                obj.position = Vector3.MoveTowards(obj.position, node.WorldPosition, speed * Time.deltaTime);
                yield return null;
            }
        }
    }

    // Визуализация всех нод
    private void OnDrawGizmos()
    {
        if (nodes == null) return;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Gizmos.color = nodes[x, z].Walkable ? Color.white : Color.red;
                Gizmos.DrawCube(nodes[x, z].WorldPosition + Vector3.up * 0.1f, Vector3.one * 0.2f);
            }
        }
    }
}