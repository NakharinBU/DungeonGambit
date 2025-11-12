using UnityEngine;
using System;
using UnityEditor;

public class Knight : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        characterName = "Knight";
        stats = new Status(5, 0, 4, 2);
    }

    public override bool MoveTowards(Vector2Int targetPos)
    {
        Vector2Int direction = targetPos - position;

        if (direction.magnitude <= 1.5f) return false;

        int moveX = (direction.x > 0) ? 1 : (direction.x < 0) ? -1 : 0;
        int moveY = (direction.y > 0) ? 1 : (direction.y < 0) ? -1 : 0;

        Vector2Int potentialMove = new Vector2Int(moveX, moveY);

        // 1. ลองเดินแนวทแยงก่อน
        if (potentialMove != Vector2Int.zero)
        {
            if (base.Move(potentialMove))
            {
                return true;
            }
        }

        // 2. ถ้าเดินแนวทแยงไม่สำเร็จ ให้ลองเดินแกน X
        if (moveX != 0)
        {
            if (base.Move(new Vector2Int(moveX, 0)))
            {
                return true;
            }
        }

        // 3. ถ้าเดินแกน X ไม่สำเร็จ ให้ลองเดินแกน Y
        if (moveY != 0)
        {
            if (base.Move(new Vector2Int(0, moveY)))
            {
                return true;
            }
        }

        return false; // เดินไม่ได้
    }

    public override void DecideAction(Player player)
    {
        if (player == null) return;

        TileHighlighter highlighter = player.GetComponent<TileHighlighter>();

        Vector2Int playerPos = player.position;
        float distance = Vector2Int.Distance(position, playerPos);

        // 1. ACTION: โจมตี
        if (distance <= 1.5f)
        {
            Attack(player);
        }
        // 2. ACTION: เคลื่อนที่
        else if (distance <= visionRange)
        {
            MoveTowards(playerPos);
        }
        else
        {

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