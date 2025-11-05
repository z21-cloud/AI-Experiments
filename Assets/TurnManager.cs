using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    public PlayerController player;
    public NPCController npc;
    public bool IsPlayerTurn { get; private set; } = true;

    private bool isPlayerTurn = true;

    private void Start()
    {
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        while (true)
        {
            if (isPlayerTurn)
            {
                Debug.Log("Player...");
                player.StartTurn();

                // ждем, пока игрок закончит ход
                yield return new WaitUntil(() => player.HasFinishedTurn);
                isPlayerTurn = false;
            }
            else
            {
                Debug.Log("NPC...");
                npc.TakeTurn();

                // ждем, пока NPC завершит движение
                yield return new WaitUntil(() => npc.HasFinishedTurn);
                isPlayerTurn = true;
            }
        }
    }
}

