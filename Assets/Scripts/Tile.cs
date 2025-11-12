using UnityEngine;
using static EnumData;

public class Tile
{
    private Vector2 position;
    private TileType type;
    private bool isWalkable;

    private bool isOccupied = false;

    public Vector2 Position => position;
    public TileType Type => type;
    public bool IsOccupied => isOccupied;

    public Tile(Vector2 pos, TileType t)
    {
        position = pos;
        type = t;
        isWalkable = (t == TileType.Floor || t == TileType.Exit || t == TileType.Shop);
    }

    public void SetOccupied(bool state)
    {
        isOccupied = state;
    }

    public bool IsWalkable()
    {
        return isWalkable;
    }
}
