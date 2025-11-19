using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TileHighlighter : MonoBehaviour
{
    public GameObject highlightPrefab;
    private List<GameObject> highlights = new List<GameObject>();
    private DungeonManager dungeonManager;

    private DungeonManager DungeonManagerRef
    {
        get
        {
            if (dungeonManager == null)
            {
                dungeonManager = DungeonManager.Instance;
            }
            return dungeonManager;
        }
    }


    [Header("Highlight Colors")]
    public Color emptyColor = new Color(0f, 0.6f, 1f, 0.4f); // น้ำเงิน (เดินได้)
    public Color enemyColor = new Color(1f, 0f, 0f, 0.5f);  // แดง (โจมตี)
    public Color exitColor = new Color(0f, 1f, 0f, 0.5f);   // เขียว (ทางออก)
    public Color wallColor = new Color(0.5f, 0.5f, 0.5f, 0.2f); // เทา (กำแพง/เดินไม่ได้)

    public Color enemyPathColor = new Color(1f, 0.5f, 0f, 0.4f); // ส้ม (เส้นทางเดิน)

    public void ClearHighlights()
    {
        foreach (GameObject obj in highlights)
        {
            Destroy(obj);
        }
        highlights.Clear();
    }

    public void ShowHighlights(Player player)
    {
        if (DungeonManagerRef == null) return;

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

            Tile tileData = DungeonManagerRef.GetTile(checkPos.x, checkPos.y);

            if (tileData == null) continue;

            GameObject highlight = Instantiate(highlightPrefab,
                                               new Vector3(checkPos.x, checkPos.y, 0f), // ตำแหน่งเดียวกับ Tile
                                               Quaternion.identity);
            var sr = highlight.GetComponent<SpriteRenderer>();

            // ตรวจสอบว่ามี Actor อยู่หรือไม่
            Character characterAtPos = DungeonManagerRef.GetCharacterAtPosition(checkPos);

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
                sr.color = emptyColor; // Default
            }
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

        if (distance <= 1.5f)
        {
            GameObject attackHighlight = Instantiate(highlightPrefab, new Vector3(targetPos.x, targetPos.y, 0f), Quaternion.identity);
            attackHighlight.GetComponent<SpriteRenderer>().color = enemyColor;
            highlights.Add(attackHighlight);
            return;
        }

        if (distance <= enemy.visionRange)
        {
            Vector2Int finalPos = SimulateKnightMove(enemy, targetPos);

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

        int moveX = (direction.x > 0) ? 1 : (direction.x < 0) ? -1 : 0;
        int moveY = (direction.y > 0) ? 1 : (direction.y < 0) ? -1 : 0;

        Vector2Int potentialMove = new Vector2Int(moveX, moveY);

        if (potentialMove != Vector2Int.zero)
        {
            if (CanEnemyMoveTo(finalPos + potentialMove))
            {
                return finalPos + potentialMove;
            }
        }

        if (moveX != 0)
        {
            Vector2Int xMove = new Vector2Int(moveX, 0);
            if (CanEnemyMoveTo(finalPos + xMove))
            {
                return finalPos + xMove;
            }
        }

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

        return targetTile != null && targetTile.IsWalkable() && charAtTarget == null;
    }
}