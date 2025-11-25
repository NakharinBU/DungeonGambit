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
                Debug.Log($"{target.characterName} took {value} Damage.");
                target.TakeDamage((int)value);
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
