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

    public void Apply(Character target, int damage)
    {
        if (target == null) return;

        switch (effect)
        {
            case EnumData.SkillEffectType.Damage:
                Debug.Log($"{target.characterName} took {damage} Damage.");
                target.TakeDamage(damage);
                break;
            case EnumData.SkillEffectType.Regen:
                if (target is Player player)
                {

                    Debug.Log($"Restored {value} Mana.");
                }
                break;
        }
    }
}
