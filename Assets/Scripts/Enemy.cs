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

        Player player = Player.Instance; // ใช้ Singleton

        if (player != null)
        {
            // 2. เรียก Passive Trigger สำหรับเหตุการณ์ OnKill
            // Passive Skill ที่ต้องการ Mana/HP เมื่อฆ่าจะทำงานที่นี่
            player.CheckPassiveTrigger(EnumData.PassiveTrigger.OnKill, player, this);

            // Note: ถ้าคุณต้องการให้ Player.Die() ล้าง Highlight
            // คุณต้องลบ FindFirstObjectByType<Player>() ใน base.Die() ออก
            // หรือย้าย Logic นี้ไปไว้ใน Player.cs แทน
        }

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

