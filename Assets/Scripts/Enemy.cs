using UnityEngine;
using static EnumData;

[System.Serializable]
public struct LootDrop
{
    public InteractableObject itemPrefab;
    [Range(0f, 1f)] public float dropChance;
}

public abstract class Enemy : Character
{
    public EnemyType enemyType;
    public int visionRange = 5;
    public LootDrop[] possibleDrops;
    public float dropChance = 0.5f;

    public override void Die()
    {
        
        dungeonManager.RemoveCharacter(this);

        foreach (var loot in possibleDrops)
        {
            if (loot.itemPrefab != null && UnityEngine.Random.value < loot.dropChance)
            {
                dungeonManager.SpawnInteractable(loot.itemPrefab, position);
            }
        }

        base.Die();

        dungeonManager.CheckLevelCompletion();
    }
    public abstract void DecideAction(Player player);
    public abstract bool MoveTowards(Vector2Int targetPos);
}

