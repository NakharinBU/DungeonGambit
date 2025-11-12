using UnityEngine;
using UnityEngine.UIElements;

public class Character : MonoBehaviour
{
    [Header("Character Info")]
    public string characterName = "Base Character";
    public Status stats;
    public Vector2Int position;

    public Status Stats => stats;

    protected DungeonManager dungeonManager;

    protected virtual void Awake()
    {
        
    }

    protected virtual void Start()
    {
        dungeonManager = DungeonManager.Instance;
    }

    public virtual bool Move(Vector2Int direction)
    {
        Vector2Int targetPos = position + direction;

        Tile targetTile = dungeonManager.GetTile(targetPos.x, targetPos.y);
        Character charAtTarget = dungeonManager.GetCharacterAtPosition(targetPos); // ตรวจสอบว่ามีตัวละครอื่นอยู่หรือไม่

        // ตรวจสอบ: 1) อยู่ในขอบเขต, 2) เดินได้, 3) ไม่มีใครอยู่ (สำคัญ)
        if (targetTile == null || !targetTile.IsWalkable() || charAtTarget != null)
        {
            return false; // เดินไม่ได้
        }

        // อัปเดตสถานะ Tile
        dungeonManager.GetTile(position.x, position.y)?.SetOccupied(false);
        position = targetPos;
        dungeonManager.GetTile(position.x, position.y)?.SetOccupied(true);

        // อัปเดตตำแหน่งใน World Space
        transform.position = new Vector3(position.x, position.y, transform.position.z);

        return true; // เดินสำเร็จ
    }

    public virtual void Attack(Character target)
    {
        if (CombatSystem.Instance != null)
        {
            CombatSystem.Instance.ProcessAttack(this, target);
        }
        else
        {
            Debug.LogError("CombatSystem not found! Attack failed.");
        }
    }

    public virtual void TakeDamage(int amount)
    {
        stats.hp -= amount;

        Debug.Log($"{characterName} took {amount} damage. Remaining HP: {stats.hp}");

        if (stats.hp <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        Player player = FindFirstObjectByType<Player>();
        player?.GetComponent<TileHighlighter>()?.ClearHighlights();
        Debug.Log($"{characterName} ถูกกำจัดแล้ว!");

        Destroy(gameObject);
    }

    public virtual void OnTurnEnd()
    {
        if (this is Player)
        {
            // GameManager.Instance.turnManager.EndPlayerTurn(); // จะเรียกใน Player.OnTurnEnd()
        }
    }

    public virtual bool UseMana(int cost)
    {
        if (stats.mp < cost) return false;
        stats.mp -= cost;
        return true;
    }

    public virtual void RestoreMana(int amount)
    {
        stats.mp = Mathf.Min(stats.mp + amount, stats.maxMp);
    }

    public virtual void Heal(int amount)
    {
        stats.hp = Mathf.Min(stats.hp + amount, stats.maxHp);
    }
}
