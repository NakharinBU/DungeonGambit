using UnityEngine;
using System;
using UnityEditor;

public class Archer : Enemy
{
    public float extraRange = 2f;   // ระยะโจมตีเพิ่มขึ้น 2 ช่อง
    private float attackRange = 1.5f;

    protected override void Awake()
    {
        base.Awake();
        characterName = "Archer";
        stats = GetComponent<Status>();

        attackRange += extraRange; // รวมระยะโจมตีใหม่
    }

    public override bool MoveTowards(Vector2Int targetPos)
    {
        Vector2Int direction = targetPos - position;

        int moveX = (direction.x > 0) ? 1 : (direction.x < 0) ? -1 : 0;
        int moveY = (direction.y > 0) ? 1 : (direction.y < 0) ? -1 : 0;

        Vector2Int potentialMove = new Vector2Int(moveX, moveY);

        // เดินทแยงก่อน
        if (potentialMove != Vector2Int.zero)
        {
            if (base.Move(potentialMove))
                return true;
        }

        // เดินแกน X
        if (moveX != 0)
        {
            if (base.Move(new Vector2Int(moveX, 0)))
                return true;
        }

        // เดินแกน Y
        if (moveY != 0)
        {
            if (base.Move(new Vector2Int(0, moveY)))
                return true;
        }

        return false;
    }

    public override void DecideAction(Player player)
    {
        if (player == null) return;

        Vector2Int playerPos = player.position;
        float distance = Vector2Int.Distance(position, playerPos);

        // 1. ACTION: ยิงระยะไกล
        if (distance <= attackRange)
        {
            Attack(player);
            return;
        }

        // 2. ACTION: เข้าใกล้เพื่อหาองศายิง (แต่ไม่มาก)
        if (distance <= visionRange)
        {
            MoveTowards(playerPos);
        }
    }

    public void ShowIntent(Player player, TileHighlighter highlighter)
    {
        if (player == null || highlighter == null) return;

        Vector2Int playerPos = player.position;
        float distance = Vector2Int.Distance(position, playerPos);

        if (distance <= visionRange)
        {
            highlighter.ShowEnemyHighlights(this, player);
        }
    }
}
