using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Character : MonoBehaviour
{
    
    public event Action<int, int> OnHealthChanged;
    public event Action<int, int> OnManaChanged;
    public event Action<int> OnAttackChanged;

    [Header("Character Info")]
    public string characterName = "Base Character";
    public Status stats;
    public Vector2Int position;

    public Status Stats => stats;

    protected DungeonManager dungeonManager;

    protected virtual void Awake()
    {
        if (stats == null)
        {
            stats = new Status();
        }
        stats.hp = stats.maxHp;
        stats.mp = 0;
    }

    protected virtual void Start()
    {
        dungeonManager = DungeonManager.Instance;
    }

    public virtual bool Move(Vector2Int direction)
    {
        Vector2Int targetPos = position + direction;

        Tile targetTile = dungeonManager.GetTile(targetPos.x, targetPos.y);
        Character charAtTarget = dungeonManager.GetCharacterAtPosition(targetPos);

        if (targetTile == null || !targetTile.IsWalkable() || charAtTarget != null)
        {
            return false;
        }

        dungeonManager.GetTile(position.x, position.y)?.SetOccupied(false);
        position = targetPos;
        dungeonManager.GetTile(position.x, position.y)?.SetOccupied(true);

        transform.position = new Vector3(position.x, position.y, transform.position.z);

        return true;
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

        OnHealthChanged?.Invoke(stats.hp, stats.maxHp);

        if (stats.hp <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        
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

        OnManaChanged?.Invoke(stats.mp, stats.maxMp);

        return true;
    }

    public virtual void RestoreMana(int amount)
    {
        stats.mp = Mathf.Min(stats.mp + amount, stats.maxMp);
        OnManaChanged?.Invoke(stats.mp, stats.maxMp);
    }

    public virtual void Heal(int amount)
    {
        stats.hp = Mathf.Min(stats.hp + amount, stats.maxHp);
        OnHealthChanged?.Invoke(stats.hp, stats.maxHp);
    }

    public void UpgradeMaxHP(int amount)
    {
        if (stats != null) stats.maxHp += amount;
        OnHealthChanged?.Invoke(stats.hp, stats.maxHp);
    }

    public void UpgradeMaxMP(int amount)
    {
        if (stats != null) stats.maxMp += amount;
        OnManaChanged?.Invoke(stats.mp, stats.maxMp);
    }

    public void UpgradeBaseAttack(int amount)
    {
        if (stats != null) stats.atk += amount;
        OnAttackChanged.Invoke(stats.atk);
    }
}
