using UnityEngine;

public class EnumData : MonoBehaviour
{
    public enum TileType { Floor, Wall, Exit, Pit, Shop }

    public enum ItemType { Consumable, Equipment, Material, Key }
    public enum ItemEffect { HealHP, RestoreMP, StatusBoost, Special }

    public enum EnemyType { Minion, Elite, Boss }
    public enum AIType { Melee, Ranged, Support, Static }

    public enum SkillType { Active, Passive }
    public enum SkillEffectType { Damage, Regen, Buff, Debuff, Utility }
    public enum PassiveTrigger { OnTurnStart, OnTurnEnd, OnKill, OnAttack }

    public enum UpgradeType { Stat, Skill, Item, Passive }

    public enum StatusUpgradeType { None, MaxHP, MaxMP, Attack }

    public enum SoulUpgradeType { Health, Attack, Mana, Special }

    public enum TurnState { PlayerTurn, EnemyTurn, Processing }

    public enum CurrencyType { Gold, SoulPoint, UndoPoint }
}
