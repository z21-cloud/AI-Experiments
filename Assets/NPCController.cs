using System.Collections;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public int moveRange = 3;
    public Transform playerTransform;
    private BFSPath pathfinder;
    private GridManager gridManager;

    public bool HasFinishedTurn { get; private set; }


    private void Start()
    {
        pathfinder = FindFirstObjectByType<BFSPath>();
        gridManager = FindFirstObjectByType<GridManager>();
    }

    public void TakeTurn()
    {
        HasFinishedTurn = false;
        StartCoroutine(MoveTowardsPlayer());
    }

    private IEnumerator MoveTowardsPlayer()
    {
        Node startNode = gridManager.GetClosestNodeByPosition(transform.position);
        Node targetNode = gridManager.GetClosestNodeByPosition(playerTransform.position);

        var path = pathfinder.SearchPath(startNode, targetNode);

        if (path == null || path.Count == 0)
            yield break;

        // ограничиваем путь диапазоном движения
        int steps = Mathf.Min(moveRange, path.Count - 1);
        for (int i = 1; i <= steps; i++)
        {
            Node targetPosNode = gridManager.GetClosestNodeByPosition(path[i].worldPosition);
            transform.position = targetPosNode.worldPosition;
            yield return new WaitForSeconds(0.2f);
        }

        // проверка на достижение игрока
        if (Vector3.Distance(transform.position, playerTransform.position) <= 1.1f)
        {
            Debug.Log("NPC reached the player!");
        }

        yield return new WaitForSeconds(0.5f);
        HasFinishedTurn = true;
    }
}
