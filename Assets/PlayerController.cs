using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    [Header("Movement settings")]
    public int moveRange = 3;
    public float moveSpeed = 5f;

    [Header("References")]
    public GridManager gridManager;
    public BFSPath pathFinder;

    [Header("Debug")]
    public bool showDebugPath = true;

    public Node currentNode { get; private set; }
    public bool IsMoving { get; private set; }
    public bool HasFinishedTurn { get; private set; }

    private List<Node> previewPath = new List<Node>();

    private void Start()
    {
        if (pathFinder == null)
            pathFinder = FindFirstObjectByType<BFSPath>();

        if(gridManager == null)
            gridManager = FindFirstObjectByType<GridManager>();

        InitializePosition();
    }

    public void StartTurn()
    {
        HasFinishedTurn = false;
        // Здесь разрешаем управление игроку
    }

    private void EndTurn()
    {
        HasFinishedTurn = true;
        // вызывать, когда игрок закончил перемещение
    }

    private void InitializePosition()
    {
        currentNode = GetClosestAvailableNode(transform.position);

        if(currentNode != null)
        {
            transform.position = currentNode.worldPosition;
            currentNode.occupant = Node.Occupant.Player;

            if (currentNode.view != null)
                currentNode.view.Highlight(Color.cyan);
            
            Debug.Log($"Player initialized at grid position: {currentNode.gridIndex}");
        }
        else
            Debug.LogError("Failed to find available node for player!");
    }

    private Node GetClosestAvailableNode(Vector3 position)
    {
        Node closest = gridManager.GetClosestNodeByPosition(position);
        if (closest != null) return closest;
        return null;
    }

    public void RequestMove(Node targetNode)
    {
        if(targetNode == null)
        {
            Debug.LogWarning("Target node is Null;");
            return;
        }

        if(IsMoving)
        {
            Debug.LogWarning("Player is moving");
            return;
        }

        if(currentNode == null)
        {
            Debug.LogError("Current node is not set");
            return;
        }

        if(currentNode == targetNode)
        {
            Debug.Log("Already at target node");
            return;
        }

        if(!targetNode.IsAvailable())
        {
            Debug.Log("Target node is not available");
            return;
        }

        List<Node> path = pathFinder.SearchPath(currentNode, targetNode);

        if(path == null || path.Count == 0)
        {
            Debug.Log("No path found");
            return;
        }

        List<Node> limitedPath = LimitPathByRange(path);

        if(limitedPath.Count <= 1)
        {
            Debug.Log("Target is too far or path is invalid");
            return;
        }

        StartCoroutine(FollowPath(limitedPath));
    }

    private List<Node> LimitPathByRange(List<Node> path)
    {
        int maxNodes = Mathf.Min(path.Count, moveRange + 1);
        List<Node> limitedPath = new List<Node>();

        for (int i = 0; i < maxNodes; i++)
        {
            limitedPath.Add(path[i]);
        }

        return limitedPath;
    }

    private IEnumerator FollowPath(List<Node> path)
    {
        IsMoving = true;

        if(showDebugPath)
            VisualizePath(path);

        for (int i = 1; i < path.Count; i++)
        {
            Node targetNode = path[i];
            Vector3 startPosition = transform.position;
            Vector3 endPosition = targetNode.worldPosition;

            float journey = 0f;
            while (journey < 1f)
            {
                journey += Time.deltaTime * moveSpeed;
                transform.position = Vector3.Lerp(startPosition, endPosition, journey);
                yield return null;
            }

            if(i == 1)
            {
                UpdateOccupancy(currentNode, null);
            }

            if(i == path.Count - 1)
            {
                UpdateOccupancy(null, targetNode);
                currentNode = targetNode;
            }
        }

        if (showDebugPath)
            ClearPathVisualization();

        IsMoving = false;

        OnMoveComplete();
    }

    private void OnMoveComplete()
    {
        EndTurn();
        Debug.Log("Player turn complete!");
    }

    private void VisualizePath(List<Node> path)
    {
        ClearPathVisualization();
        previewPath = new List<Node>(path);

        // Подсвечиваем ноды пути (кроме стартовой)
        for (int i = 1; i < path.Count; i++)
        {
            if (path[i].view != null)
            {
                // Последняя нода - ярче
                Color pathColor = (i == path.Count - 1) ? Color.magenta : Color.blue;
                path[i].view.Highlight(pathColor);
            }
        }
    }

    private void ClearPathVisualization()
    {
        foreach (Node node in previewPath)
        {
            if (node != null && node.view != null && node != currentNode)
                node.view.Unhighlight();
        }
        previewPath.Clear();
    }

    private void UpdateOccupancy(Node oldNode, Node newNode)
    {
        if(oldNode != null)
        {
            oldNode.occupant = Node.Occupant.None;
            if (oldNode.view != null)
                oldNode.view.Unhighlight();
        }

        if(newNode != null)
        {
            newNode.occupant = Node.Occupant.Player;
            if (newNode.view != null)
                newNode.view.Highlight(Color.cyan);
        }
    }
}
