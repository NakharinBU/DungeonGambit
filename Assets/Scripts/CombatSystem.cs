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
        // 1. ตรวจสอบว่า target ยังมีชีวิตอยู่หรือไม่
        if (target.stats.hp <= 0) return;

        // 2. คำนวณความเสียหาย
        int damage = attacker.stats.atk;

        // 3. เรียกเมธอดรับความเสียหาย
        target.TakeDamage(damage);

        // 4. แสดงผลลัพธ์
        Debug.Log($"{attacker.characterName} attacked {target.characterName} for {damage} damage.");

        // 5. ตรวจสอบการตาย
        if (target.stats.hp <= 0)
        {
            target.Die();
        }
    }
}