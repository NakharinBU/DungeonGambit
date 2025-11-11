using Solution;
using System.Collections.Generic;
using UnityEngine;

public class TileHighlighter : MonoBehaviour
{
    public GameObject highlightPrefab;
    private List<GameObject> highlights = new List<GameObject>();

    public Color emptyColor = new Color(0f, 0.6f, 1f, 0.4f); // น้ำเงิน
    public Color enemyColor = new Color(1f, 0f, 0f, 0.5f);   // แดง
    public Color exitColor = new Color(0f, 1f, 0f, 0.5f);    // เขียว

    public void ClearHighlights()
    {
        foreach (GameObject obj in highlights)
        {
            Destroy(obj);
        }
        highlights.Clear();
    }

    public void ShowHighlights(Vector2Int playerPos, OOPMapGenerator mapGenerator)
    {
        ClearHighlights();

        // ทั้ง 8 ทิศทางเหมือน King
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(1, 0),
            new Vector2Int(1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, 1)
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int checkPos = playerPos + dir;

            // ✅ แก้ตรงนี้: ใช้ X, Y แทน width, height
            if (checkPos.x < 0 || checkPos.y < 0 ||
                checkPos.x >= mapGenerator.X || checkPos.y >= mapGenerator.Y)
                continue;

            // ตรวจว่าช่องนั้นมี object อะไร
            GameObject tile = Instantiate(highlightPrefab, new Vector3(checkPos.x, checkPos.y, 0f), Quaternion.identity);
            var sr = tile.GetComponent<SpriteRenderer>();

            // เปลี่ยนสีตามประเภท
            var cell = mapGenerator.mapdata[checkPos.x, checkPos.y];
            if (cell == null)
            {
                sr.color = emptyColor;
            }
            else if (cell.CompareTag("Enemy"))
            {
                sr.color = enemyColor;
            }
            else if (cell.CompareTag("Exit"))
            {
                sr.color = exitColor;
            }
            else
            {
                sr.color = Color.gray; // ช่องอื่น ๆ
            }

            highlights.Add(tile);
        }
    }
}
