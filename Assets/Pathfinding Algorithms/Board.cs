using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public static Board Instance { get; private set; }
    public static Vector3Int[] Directions = new Vector3Int[4]
    {
        Vector3Int.up,
        Vector3Int.right,
        Vector3Int.down,
        Vector3Int.left,
    };
    public Tilemap tilemap;
    public Vector3Int Size;
    public Dictionary<Vector3Int, TileLogic> Tiles;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
        Tiles = new Dictionary<Vector3Int, TileLogic>();
        CreateTileLogic();
        Debug.Log(Tiles.Count);
    }

    public void PaintTile(Vector3Int position, Color color)
    {
        tilemap.SetColor(position, color);
    }

    private void CreateTileLogic()
    {
        for (int x = 0; x < Size.x; x++)
        {
            for (int y = 0; y < Size.y; y++)
            {
                TileLogic tile = new TileLogic();
                tile.Position = new Vector3Int(x, y, 0);
                Tiles.Add(tile.Position, tile);
                SetTile(tile);
            }
        }
    }

    public static TileLogic GetTile(Vector3Int position)
    {
        TileLogic tile;
        if (Instance.Tiles.TryGetValue(position, out tile)) return tile;
        return null;
    }

    public void ClearSearch()
    {
        foreach (TileLogic t in Tiles.Values)
        {
            t.Distance = int.MaxValue;
            t.Previous = null;
        }
    }

    private void SetTile(TileLogic tile)
    {
        string tileType = tilemap.GetTile(tile.Position).name;
        switch (tileType)
        {
            case "blockedTile":
                tile.Occupied = true;
                break;
        }
    }
}
