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
    protected CombatSystem combatSystem;

    protected virtual void Awake()
    {
        if (stats == null) stats = new Status(10, 5, 3, 1);
    }

    protected virtual void Start()
    {
        //dungeonManager = DungeonManager.Instance;
        combatSystem = CombatSystem.Instance();
    }

    public virtual void Move(Vector2Int direction)
    {
        Debug.Log($"{characterName} moved to {position + direction}");
        position += direction;
        OnTurnEnd();
    }

    public virtual void Attack(Character target)
    {
        if (target == null) return;
        int damage = combatSystem.CalculateDamage(this, target);
        Debug.Log($"{characterName} โจมตี {target.characterName} สร้างความเสียหาย {damage}!");
        target.TakeDamage(damage);
        OnTurnEnd();
    }

    public virtual void TakeDamage(int amount)
    {
        stats.hp -= amount;
        if (stats.hp <= 0) Die();
    }

    protected virtual void Die()
    {
        Debug.Log($"{characterName} ถูกกำจัดแล้ว!");
        // Logic: Free Tile, Remove from list, Destroy GameObject
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
