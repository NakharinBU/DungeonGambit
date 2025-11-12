using UnityEngine;

public class EnumData : MonoBehaviour
{
    // (คุณอาจจะสร้างไฟล์ Enums.cs หรือไฟล์แยกตามประเภท เช่น TileEnums.cs)

    // จาก Tile
    public enum TileType { Floor, Wall, Exit, Pit, Shop }

    // จาก Item
    public enum ItemType { Consumable, Equipment, Material, Key }
    public enum ItemEffect { HealHP, RestoreMP, StatusBoost, Special }

    // จาก Enemy
    public enum EnemyType { Minion, Elite, Boss }
    public enum AIType { Melee, Ranged, Support, Static } // BehaviorType ใน Diagram

    // จาก Skill
    public enum SkillType { Active, Passive }
    public enum SkillEffectType { Damage, Healing, Buff, Debuff, Utility }
    public enum PassiveTrigger { OnTurnStart, OnTurnEnd, OnDamaged, OnAttack }

    // จาก Upgrade/SoulUpgrade
    public enum UpgradeType { Stat, Skill, Item, Passive }
    // ถ้าคุณมี SoulSystem
    public enum SoulUpgradeType { Health, Attack, Mana, Special }

    // จาก TurnManager
    public enum TurnState { PlayerTurn, EnemyTurn, Processing }

    // จาก CurrencyManager
    public enum CurrencyType { Gold, SoulPoint, UndoPoint }
}
