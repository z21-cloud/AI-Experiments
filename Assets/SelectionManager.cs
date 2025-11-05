using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour
{
    [Header("References")]
    public PlayerController playerController;
    public BFSPath pathFinder;
    public TurnManager turnManager;

    [Header("Visual Settings")]
    public Color hoverColor = Color.yellow;
    public Color validMoveColor = Color.green;
    public Color invalidMoveColor = Color.red;
    public bool showPathPreview = true;

    // Текущее состояние
    private NodeView currentHoveredNode;
    private NodeView previousHoveredNode;
    private List<Node> previewPath = new List<Node>();

    private void Start()
    {
        // Получаем ссылки, если не назначены
        if (playerController == null)
            playerController = FindFirstObjectByType<PlayerController>();

        if (pathFinder == null)
            pathFinder = FindFirstObjectByType<BFSPath>();

        if (playerController == null)
            Debug.LogError("PlayerController not found! Assign it in the inspector.");
    }

    private void Update()
    {
        HandleMouseHover();
        HandleMouseClick();
    }

    /// <summary>
    /// Обработка наведения мыши (подсветка ноды)
    /// </summary>
    private void HandleMouseHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            NodeView hitNode = hit.collider.GetComponent<NodeView>();

            // Если навели на новую ноду
            if (hitNode != null && hitNode != currentHoveredNode)
            {
                // Очищаем предыдущую подсветку
                ClearHoverHighlight();

                // Сохраняем текущую наведённую ноду
                currentHoveredNode = hitNode;
                previousHoveredNode = hitNode;

                // Подсвечиваем ноду и показываем превью пути
                HighlightHoveredNode(hitNode);
            }
        }
        else
        {
            // Курсор не над нодой - очищаем подсветку
            ClearHoverHighlight();
        }
    }

    /// <summary>
    /// Подсветка наведённой ноды и превью пути
    /// </summary>
    private void HighlightHoveredNode(NodeView nodeView)
    {
        if (nodeView == null) return;

        Node targetNode = nodeView.GetNode();
        if (targetNode == null) return;

        // Если игрок движется - не показываем превью
        if (playerController.IsMoving)
        {
            nodeView.Highlight(hoverColor);
            return;
        }

        // Если это текущая позиция игрока - просто подсвечиваем
        if (targetNode == playerController.currentNode)
        {
            nodeView.Highlight(Color.cyan);
            return;
        }

        // Проверяем, можно ли туда переместиться
        bool canMove = CanMoveToNode(targetNode, out List<Node> path);

        if (canMove && showPathPreview)
        {
            // Показываем превью пути
            ShowPathPreview(path);
            nodeView.Highlight(validMoveColor);
        }
        else
        {
            // Подсвечиваем как недоступную
            nodeView.Highlight(invalidMoveColor);
        }
    }

    /// <summary>
    /// Проверяет, можно ли переместиться к ноде
    /// </summary>
    private bool CanMoveToNode(Node targetNode, out List<Node> path)
    {
        path = null;

        if (targetNode == null || playerController.currentNode == null)
            return false;

        // Проверяем доступность ноды
        if (!targetNode.IsAvailable())
            return false;

        // Ищем путь
        List<Node> fullPath = pathFinder.SearchPath(playerController.currentNode, targetNode);

        if (fullPath == null || fullPath.Count <= 1)
            return false;

        // Ограничиваем путь по дальности
        int maxNodes = Mathf.Min(fullPath.Count, playerController.moveRange + 1);
        path = fullPath.GetRange(0, maxNodes);

        // Проверяем, достигаем ли мы цели с учётом ограничения
        Node lastReachableNode = path[path.Count - 1];
        return lastReachableNode == targetNode;
    }

    /// <summary>
    /// Показывает превью пути
    /// </summary>
    private void ShowPathPreview(List<Node> path)
    {
        // Очищаем старое превью
        ClearPathPreview();

        if (path == null || path.Count <= 1)
            return;

        previewPath = new List<Node>(path);

        // Подсвечиваем все ноды пути (кроме первой - текущей позиции)
        for (int i = 1; i < path.Count - 1; i++)
        {
            Node node = path[i];
            if (node != null && node.view != null)
            {
                node.view.Highlight(Color.blue * 0.5f); // Полупрозрачный синий
            }
        }
    }

    /// <summary>
    /// Очищает превью пути
    /// </summary>
    private void ClearPathPreview()
    {
        foreach (Node node in previewPath)
        {
            if (node != null && node.view != null &&
                node != playerController.currentNode)
            {
                node.view.Unhighlight();
            }
        }
        previewPath.Clear();
    }

    /// <summary>
    /// Очищает подсветку наведения
    /// </summary>
    private void ClearHoverHighlight()
    {
        ClearPathPreview();

        if (currentHoveredNode != null)
        {
            Node node = currentHoveredNode.GetNode();

            // Если это текущая позиция игрока - оставляем подсветку
            if (node != null && node == playerController.currentNode)
            {
                currentHoveredNode.Highlight(Color.cyan);
            }
            else
            {
                currentHoveredNode.Unhighlight();
            }

            currentHoveredNode = null;
        }
    }

    /// <summary>
    /// Обработка кликов мыши
    /// </summary>
    private void HandleMouseClick()
    {
        // Левый клик - информация (опционально)
        if (Input.GetMouseButtonDown(0))
        {
            HandleLeftClick();
        }

        // Правый клик - перемещение
        if (Input.GetMouseButtonDown(1) && turnManager.IsPlayerTurn)
        {
            HandleRightClick();
        }
    }

    /// <summary>
    /// Обработка левого клика (информация о ноде)
    /// </summary>
    private void HandleLeftClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            NodeView hitNode = hit.collider.GetComponent<NodeView>();
            if (hitNode != null)
            {
                Node node = hitNode.GetNode();
                ShowNodeInfo(node);
            }
        }
    }

    /// <summary>
    /// Обработка правого клика (перемещение игрока)
    /// </summary>
    private void HandleRightClick()
    {
        // Если игрок уже движется - игнорируем клик
        if (playerController.IsMoving)
        {
            Debug.Log("Player is already moving!");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            NodeView hitNode = hit.collider.GetComponent<NodeView>();

            if (hitNode != null)
            {
                Node targetNode = hitNode.GetNode();

                if (targetNode != null)
                {
                    // Запрашиваем перемещение у PlayerController
                    playerController.RequestMove(targetNode);

                    // Очищаем все превью после клика
                    ClearHoverHighlight();
                }
            }
        }
    }

    /// <summary>
    /// Показывает информацию о ноде в консоли (для отладки)
    /// </summary>
    private void ShowNodeInfo(Node node)
    {
        if (node == null) return;

        string info = $"Node Info:\n" +
                     $"Grid Index: {node.gridIndex}\n" +
                     $"World Position: {node.worldPosition}\n" +
                     $"Walkable: {node.isWalkable}\n" +
                     $"Occupant: {node.occupant}\n" +
                     $"Available: {node.IsAvailable()}\n" +
                     $"Neighbors: {node.neighbors.Count}";

        Debug.Log(info);

        // Можно добавить UI панель с информацией вместо Debug.Log
    }

    /// <summary>
    /// Показывает радиус движения игрока (опционально)
    /// </summary>
    public void ShowMovementRange()
    {
        // Можно реализовать визуализацию всех клеток в радиусе moveRange
        if (playerController == null || playerController.currentNode == null)
            return;

        List<Node> reachableNodes = GetReachableNodes(
            playerController.currentNode,
            playerController.moveRange
        );

        foreach (Node node in reachableNodes)
        {
            if (node != null && node.view != null)
            {
                node.view.Highlight(Color.green * 0.3f);
            }
        }
    }

    /// <summary>
    /// Получает все доступные ноды в радиусе движения (BFS с ограничением глубины)
    /// </summary>
    private List<Node> GetReachableNodes(Node startNode, int maxDistance)
    {
        List<Node> reachable = new List<Node>();
        Queue<Node> queue = new Queue<Node>();
        Dictionary<Node, int> distances = new Dictionary<Node, int>();

        queue.Enqueue(startNode);
        distances[startNode] = 0;

        while (queue.Count > 0)
        {
            Node current = queue.Dequeue();
            int currentDistance = distances[current];

            if (currentDistance >= maxDistance)
                continue;

            foreach (Node neighbor in current.neighbors)
            {
                if (!distances.ContainsKey(neighbor) && neighbor.IsAvailable())
                {
                    distances[neighbor] = currentDistance + 1;
                    reachable.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        return reachable;
    }

    private void OnDisable()
    {
        // Очистка при выключении
        ClearHoverHighlight();
    }
}