using UnityEngine;

public class Bishop : Enemy
{
    public float healRange = 1f;          // ระยะ heal
    public float avoidPlayerRange = 3f;   // ไม่เข้าใกล้ Player

    protected override void Awake()
    {
        base.Awake();
        characterName = "Bishop";
        stats = GetComponent<Status>();
    }

    public override void DecideAction(Player player)
    {
        if (player == null) return;

        // ค้นหาเพื่อนที่ใกล้ที่สุด
        Enemy ally = FindClosestAlly();
        if (ally == null) return;

        float distPlayer = Vector2Int.Distance(position, player.position);

        // 1. ถ้าผู้เล่นเข้าใกล้เกินไป → ถอย
        if (distPlayer < avoidPlayerRange)
        {
            MoveAwayFromPlayer(player.position);
            return;
        }

        float distAlly = Vector2Int.Distance(position, ally.position);

        // 2. ถ้าอยู่ในระยะ Heal → Heal
        if (distAlly <= healRange)
        {
            Heal(ally);
        }
        else
        {
            // 3. เดินเข้าใกล้เพื่อน (แต่ต้องไม่เข้าใกล้ Player)
            MoveTowardsAllySafely(ally.position, player.position);
        }
    }

    // -----------------------------------------------------
    // Heal ตาม atk ของ Bishop
    // -----------------------------------------------------
    private void Heal(Enemy target)
    {
        Status targetStatus = target.GetComponent<Status>();
        if (targetStatus == null) return;

        int healValue = stats.atk;  // Heal = ATK

        targetStatus.hp = Mathf.Min(targetStatus.hp + healValue, targetStatus.maxHp);

        Debug.Log($"{characterName} heals {target.characterName} for {healValue} HP");
    }

    // -----------------------------------------------------
    // ค้นหาเพื่อนที่ใกล้ที่สุด
    // -----------------------------------------------------
    private Enemy FindClosestAlly()
    {
        Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        Enemy closest = null;
        float minDist = Mathf.Infinity;

        foreach (Enemy e in allEnemies)
        {
            if (e == this) continue;

            float dist = Vector2Int.Distance(position, e.position);

            if (dist < minDist)
            {
                minDist = dist;
                closest = e;
            }
        }

        return closest;
    }

    // -----------------------------------------------------
    // หนี Player
    // -----------------------------------------------------
    private void MoveAwayFromPlayer(Vector2Int playerPos)
    {
        Vector2Int dir = position - playerPos;
        dir.x = Mathf.Clamp(dir.x, -1, 1);
        dir.y = Mathf.Clamp(dir.y, -1, 1);

        if (!base.Move(dir))
        {
            // ถ้าเดินไม่ได้ ลองทิศอื่น
            Vector2Int[] dirs =
            {
                new Vector2Int(1,0), new Vector2Int(-1,0),
                new Vector2Int(0,1), new Vector2Int(0,-1),
                new Vector2Int(1,1), new Vector2Int(-1,1),
                new Vector2Int(1,-1), new Vector2Int(-1,-1)
            };

            foreach (var d in dirs)
            {
                if (base.Move(d)) return;
            }
        }
    }

    // -----------------------------------------------------
    // เดินหาเพื่อนแบบปลอดภัย (ไม่เข้าใกล้ Player)
    // -----------------------------------------------------
    private void MoveTowardsAllySafely(Vector2Int allyPos, Vector2Int playerPos)
    {
        Vector2Int direction = allyPos - position;

        int moveX = direction.x > 0 ? 1 : direction.x < 0 ? -1 : 0;
        int moveY = direction.y > 0 ? 1 : direction.y < 0 ? -1 : 0;

        Vector2Int[] moves =
        {
            new Vector2Int(moveX, moveY),
            new Vector2Int(moveX, 0),
            new Vector2Int(0, moveY)
        };

        foreach (var move in moves)
        {
            Vector2Int next = position + move;

            // ห้ามเดินถ้าเข้าใกล้ Player เกินไป
            if (Vector2Int.Distance(next, playerPos) < avoidPlayerRange)
                continue;

            if (base.Move(move))
                return;
        }
    }

    // -----------------------------------------------------
    // บังคับ override ให้คลาส Enemy ไม่ error
    // -----------------------------------------------------
    public override bool MoveTowards(Vector2Int targetPos)
    {
        // Bishop ไม่ใช้ฟังก์ชันนี้โดยตรง
        // แต่ต้องใส่เพื่อ override abstract
        return false;
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
