using UnityEngine;

public class SkillEffect : MonoBehaviour
{
    public EnumData.SkillEffectType effect;
    public float value;
    public int duration;

    public SkillEffect(EnumData.SkillEffectType effectType, float val, int dur = 0)
    {
        effect = effectType;
        value = val;
        duration = dur;
    }

    public void Apply(Character target)
    {
        if (target == null) return;

        switch (effect)
        {
            case EnumData.SkillEffectType.Damage:
                // เรียก CombatSystem.Instance.ProcessDamage(...)
                // (ต้องปรับ CombatSystem ให้รับ Character 2 ตัว หรือจัดการ damage เอง)
                Debug.Log($"{target.characterName} took {value} Damage.");
                target.TakeDamage((int)value);
                break;
            case EnumData.SkillEffectType.Regen:
                if (target is Player player)
                {
                    // ต้องมีเมธอด AddMana ใน Status หรือ Player
                    // player.stats.AddMana((int)value); 
                    Debug.Log($"Restored {value} Mana.");
                }
                break;
                // ... เพิ่ม Effect อื่นๆ ...
        }
    }
}
