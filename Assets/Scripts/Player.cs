using System.Collections;
using UnityEngine;

public class Player : Character
{
    public Inventory inventory = new Inventory();
    public CurrencyManager currencies = new CurrencyManager();
    public bool isAutoMoving = false;
    public TileHighlighter highlighter;

    private void Start()
    {
        base.Start();
        highlighter = GetComponent<TileHighlighter>();
    }

    private void Update()
    {
        if (!isAutoMoving)
        {
            if (highlighter != null)
            {
                highlighter.ShowHighlights(this);
            }

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) Move(Vector2Int.up);
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) Move(Vector2Int.down);
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) Move(Vector2Int.left);
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) Move(Vector2Int.right);

            if (Input.GetMouseButtonDown(0))
            {
                Vector2Int targetPos = GetGridPositionFromMouse();
                Vector2Int direction = targetPos - position;

                // จำกัดการเคลื่อนที่ให้เป็น 1 ช่อง (หรือแนวทแยง 1 ช่อง)
                if (direction.magnitude > 0 && Mathf.Abs(direction.x) <= 1 && Mathf.Abs(direction.y) <= 1)
                {
                    // Normalize direction (เช่น ถ้าคลิกไปที่ (x+1, y+1) จะ Move(1, 1))
                    direction.x = Mathf.Clamp(direction.x, -1, 1);
                    direction.y = Mathf.Clamp(direction.y, -1, 1);
                    Move(direction);
                }
            }
        }
    }

    private Vector2Int GetGridPositionFromMouse()
    {
        // แปลงตำแหน่งเมาส์บนหน้าจอเป็นตำแหน่งใน World
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // แปลงเป็นตำแหน่ง Grid (ปัดเศษใกล้เคียง)
        return new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y));
    }

    public override void Move(Vector2Int direction)
    {
        Vector2Int targetPos = position + direction;

        Character targetChar = dungeonManager.GetCharacterAtPosition(targetPos);
        if (targetChar != null && targetChar is Enemy enemy)
        {
            Attack(enemy);
            return;
        }

        Tile targetTile = dungeonManager.GetTile(targetPos.x, targetPos.y);
        if (targetTile == null || !targetTile.IsWalkable() || targetTile.IsOccupied) return;

        base.Move(direction);

        Vector3 targetWorldPos = new Vector3(position.x, position.y, transform.position.z);
        StartCoroutine(MoveSmoothly(targetWorldPos));
    }

    IEnumerator MoveSmoothly(Vector3 target)
    {
        Vector3 start = transform.position;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 5f;
            transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }
        transform.position = target;
    }
}
