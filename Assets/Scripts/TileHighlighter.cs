using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TileHighlighter : MonoBehaviour
{
    public GameObject highlightPrefab;
    private List<GameObject> highlights = new List<GameObject>();
    private DungeonManager dungeonManager; // เพิ่ม Reference ถึง DungeonManager

    [Header("Highlight Colors")]
    public Color emptyColor = new Color(0f, 0.6f, 1f, 0.4f); // น้ำเงิน (เดินได้)
    public Color enemyColor = new Color(1f, 0f, 0f, 0.5f);  // แดง (โจมตี)
    public Color exitColor = new Color(0f, 1f, 0f, 0.5f);   // เขียว (ทางออก)
    public Color wallColor = new Color(0.5f, 0.5f, 0.5f, 0.2f); // เทา (กำแพง/เดินไม่ได้)

    private void Start()
    {
        // รับ Instance ของ DungeonManager เมื่อเกมเริ่ม
        dungeonManager = DungeonManager.Instance;
        if (dungeonManager == null)
        {
            Debug.LogError("DungeonManager not found! Cannot highlight tiles.");
        }
    }

    public void ClearHighlights()
    {
        foreach (GameObject obj in highlights)
        {
            Destroy(obj);
        }
        highlights.Clear();
    }

    // ปรับปรุงเมธอดให้รับ Player Object แทน MapGenerator
    public void ShowHighlights(Player player)
    {
        if (dungeonManager == null) return;

        ClearHighlights();

        Vector2Int playerPos = player.position;

        // 8 ทิศทาง (แบบ King)
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(1, 0),
            new Vector2Int(1, -1), new Vector2Int(0, -1), new Vector2Int(-1, -1),
            new Vector2Int(-1, 0), new Vector2Int(-1, 1)
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int checkPos = playerPos + dir;

            // 1. ตรวจสอบข้อมูล Tile ใน DungeonManager
            Tile tileData = dungeonManager.GetTile(checkPos.x, checkPos.y);

            // ถ้าพ้นขอบเขตของแผนที่ (GetTile จะคืนค่า null) หรือเป็น Tile ที่เดินไม่ได้เลย (เช่น Pit) ให้ข้าม
            if (tileData == null) continue;

            // 2. สร้าง Visual Highlight
            GameObject highlight = Instantiate(highlightPrefab,
                                               new Vector3(checkPos.x, checkPos.y, 0f), // ตำแหน่งเดียวกับ Tile
                                               Quaternion.identity);
            var sr = highlight.GetComponent<SpriteRenderer>();

            // 3. กำหนดสีตามสถานะของ Tile

            // ตรวจสอบว่ามี Actor อยู่หรือไม่
            Character characterAtPos = dungeonManager.GetCharacterAtPosition(checkPos);

            if (characterAtPos != null && characterAtPos is Enemy)
            {
                // ช่องมีศัตรู -> โจมตีได้
                sr.color = enemyColor;
            }
            else if (!tileData.IsWalkable())
            {
                // ถ้า Tile Data บอกว่าเดินไม่ได้ (Wall, Pit)
                sr.color = wallColor;
            }
            else if (tileData.IsWalkable())
            {
                // ถ้า Tile Data บอกว่าเดินได้ และไม่มี Actor
                // *คุณอาจต้องตรวจสอบ TileType.Exit ด้วย
                // (ปัจจุบัน Tile Data ไม่มี public property สำหรับ TileType, แต่สมมติว่ามี)

                // if (tileData.Type == TileType.Exit) sr.color = exitColor; // ต้องมีการเข้าถึง TileType
                // else sr.color = emptyColor;
                sr.color = emptyColor; // Default
            }

            // เพิ่มความโปร่งใสเล็กน้อยเพื่อให้เห็นพื้นหลัง
            Color c = sr.color;
            c.a = 0.5f;
            sr.color = c;

            highlights.Add(highlight);
        }
    }
}