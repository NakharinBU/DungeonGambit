using System.Collections;
using UnityEngine;

public class Player : Character
{

    public Inventory inventory = new Inventory();
    public CurrencyManager currencies = new CurrencyManager();
    public bool isAutoMoving = false;

    protected override void Awake()
    {
        base.Awake();
        characterName = "Player";
    }

    private void Update()
    {
        // ไม่มี TurnManager แล้ว เราจะเช็คแค่ว่าสามารถขยับได้ (currentState = PlayerTurn)
        // เพื่อให้สามารถทดสอบการเคลื่อนที่ได้ต่อเนื่อง (ไม่ใช่ Turn-Based)
        if (!isAutoMoving)
        {
            // Input Logic: WASD
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) Move(Vector2Int.up);
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) Move(Vector2Int.down);
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) Move(Vector2Int.left);
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) Move(Vector2Int.right);
        }
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

        // *** ไม่มีการเรียก TurnManager.Instance.EndPlayerTurn() ***
    }

    public override void Attack(Character target)
    {
        base.Attack(target);
        // *** ไม่มีการเรียก TurnManager.Instance.EndPlayerTurn() ***
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
