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

    public Color enemyPathColor = new Color(1f, 0.5f, 0f, 0.4f); // ส้ม (เส้นทางเดิน)

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
    public void ShowEnemyHighlights(Enemy enemy, Player player)
    {


        Vector2Int targetPos = player.position;
        float distance = Vector2Int.Distance(enemy.position, targetPos);

        // 1. ถ้าอยู่ในระยะโจมตี -> Highlight ช่อง Player เป็นสีแดง (Attack Highlight)
        if (distance <= 1.5f)
        {
            // วาด Highlight สีแดงบนช่อง Player
            GameObject attackHighlight = Instantiate(highlightPrefab, new Vector3(targetPos.x, targetPos.y, 0f), Quaternion.identity);
            attackHighlight.GetComponent<SpriteRenderer>().color = enemyColor;
            highlights.Add(attackHighlight);
            return;
        }

        // 2. ถ้าอยู่ในระยะมองเห็น -> จำลองการเดิน (Path Highlight)
        if (distance <= enemy.visionRange)
        {
            Vector2Int finalPos = SimulateKnightMove(enemy, targetPos);

            // 3. วาด Highlight หากมีการเคลื่อนที่
            if (finalPos != enemy.position)
            {
                GameObject pathHighlight = Instantiate(highlightPrefab, new Vector3(finalPos.x, finalPos.y, 0f), Quaternion.identity);
                pathHighlight.GetComponent<SpriteRenderer>().color = enemyPathColor; // สีส้ม
                highlights.Add(pathHighlight);
            }
        }
    }

    private Vector2Int SimulateKnightMove(Enemy enemy, Vector2Int targetPos)
    {
        Vector2Int finalPos = enemy.position;
        Vector2Int direction = targetPos - finalPos;

        // หา Int Sign
        int moveX = (direction.x > 0) ? 1 : (direction.x < 0) ? -1 : 0;
        int moveY = (direction.y > 0) ? 1 : (direction.y < 0) ? -1 : 0;

        Vector2Int potentialMove = new Vector2Int(moveX, moveY);

        // 1. ลองเดินแนวทแยงก่อน
        if (potentialMove != Vector2Int.zero)
        {
            if (CanEnemyMoveTo(finalPos + potentialMove))
            {
                return finalPos + potentialMove;
            }
        }

        // 2. ถ้าเดินแนวทแยงไม่สำเร็จ ให้ลองเดินแกน X
        if (moveX != 0)
        {
            Vector2Int xMove = new Vector2Int(moveX, 0);
            if (CanEnemyMoveTo(finalPos + xMove))
            {
                return finalPos + xMove;
            }
        }

        // 3. ถ้าเดินแกน X ไม่สำเร็จ ให้ลองเดินแกน Y
        if (moveY != 0)
        {
            Vector2Int yMove = new Vector2Int(0, moveY);
            if (CanEnemyMoveTo(finalPos + yMove))
            {
                return finalPos + yMove;
            }
        }

        return finalPos; // ไม่สามารถเคลื่อนที่ได้
    }


    private bool CanEnemyMoveTo(Vector2Int pos)
    {
        Tile targetTile = dungeonManager.GetTile(pos.x, pos.y);
        Character charAtTarget = dungeonManager.GetCharacterAtPosition(pos);

        // Enemy สามารถเดินไปในช่องที่ว่างและเดินได้เท่านั้น
        return targetTile != null && targetTile.IsWalkable() && charAtTarget == null;
    }
}