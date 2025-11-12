using UnityEngine;
using static EnumData;

public class Tile
{
    private Vector2 position;   // - position : Vector2
    private TileType type;      // - type : TileType
    private bool isWalkable;    // - isWalkable : bool

    // สถานะถูกยึดครอง (สำคัญสำหรับการเคลื่อนที่)
    private bool isOccupied = false;

    public Vector2 Position => position;
    public TileType Type => type;
    public bool IsOccupied => isOccupied;

    public Tile(Vector2 pos, TileType t)
    {
        position = pos;
        type = t;
        // กำหนด isWalkable ตามประเภทของ Tile
        isWalkable = (t == TileType.Floor || t == TileType.Exit || t == TileType.Shop);
    }

    // --- Methods (ตาม Diagram) ---

    // + SetOccupied(state : bool) : void
    public void SetOccupied(bool state)
    {
        isOccupied = state;
    }

    // + IsWalkable() : bool
    public bool IsWalkable()
    {
        return isWalkable;
    }
}
