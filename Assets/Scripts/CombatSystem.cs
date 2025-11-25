using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void ProcessAttack(Character attacker, Character target)
    {
        if (target.stats.hp <= 0) return;

        int damage = attacker.stats.atk;

        target.TakeDamage(damage);

        Debug.Log($"{attacker.characterName} attacked {target.characterName} for {damage} damage.");
        
    }
}