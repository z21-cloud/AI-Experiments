using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Tilemaps;

public class Dijkstra : MonoBehaviour
{
    public Vector3Int InitialPosition;
    public float AlgorithmTimeLength = 1 ;
    public int SearchLength;
    private List<TileLogic> tilesSearch;
    public void StarSearch()
    {
        StartCoroutine(Search(Board.GetTile(InitialPosition)));
    }

    private IEnumerator Search(TileLogic start)
    {
        tilesSearch = new List<TileLogic>();
        tilesSearch.Add(start);

        Board.Instance.ClearSearch();

        Queue<TileLogic> checkNow = new Queue<TileLogic>();
        Queue<TileLogic> checkNext = new Queue<TileLogic>();

        start.Distance = 0;
        checkNow.Enqueue(start);
        
        while(checkNow.Count > 0)
        {
            TileLogic tile = checkNow.Dequeue();
            Board.Instance.PaintTile(tile.Position, Color.green);
            for (int i = 0; i < Board.Directions.Length; i++)
            {
                TileLogic next = Board.GetTile(tile.Position + Board.Directions[i]);
                yield return new WaitForSeconds(AlgorithmTimeLength);

                if (next == null || next.Distance <= tile.Distance + 1 || next.Occupied) continue;
                next.Distance = tile.Distance + 1;
                if(ValidateMovement(tile, next))
                {
                    checkNext.Enqueue(next);
                    next.Previous = tile;
                    tilesSearch.Add(next);
                    Board.Instance.PaintTile(next.Position, Color.yellow);
                }
            }

            if (checkNow.Count == 0)
            {
                SwapReferences(ref checkNow, ref checkNext);
            }
        }
    }

    private bool ValidateMovement(TileLogic from, TileLogic to)
    {
        if (to.Distance > SearchLength) return false;
        return true;
    }

    private void SwapReferences(ref Queue<TileLogic> checkNow, ref Queue<TileLogic> checkNext)
    {
        Queue<TileLogic> temp = checkNow;
        checkNow = checkNext;
        checkNext = temp;
    }
}
