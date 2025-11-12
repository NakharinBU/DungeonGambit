using UnityEngine;

public class EnumData : MonoBehaviour
{
    public enum TileType { Floor, Wall, Exit, Pit, Shop }

    public enum ItemType { Consumable, Equipment, Material, Key }
    public enum ItemEffect { HealHP, RestoreMP, StatusBoost, Special }

    public enum EnemyType { Minion, Elite, Boss }
    public enum AIType { Melee, Ranged, Support, Static }

    public enum SkillType { Active, Passive }
    public enum SkillEffectType { Damage, Healing, Buff, Debuff, Utility }
    public enum PassiveTrigger { OnTurnStart, OnTurnEnd, OnDamaged, OnAttack }

    public enum UpgradeType { Stat, Skill, Item, Passive }
    

    public enum SoulUpgradeType { Health, Attack, Mana, Special }

    public enum TurnState { PlayerTurn, EnemyTurn, Processing }

    public enum CurrencyType { Gold, SoulPoint, UndoPoint }
}
