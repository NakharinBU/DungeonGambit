using UnityEngine;

public class SkillBase : ScriptableObject
{
    public string skillName;
    [TextArea] public string description;
    public int level = 1;
    public int power;
    public EnumData.SkillType type;
    public EnumData.SkillEffectType effectType;
    public Sprite icon;

    protected SkillEffect skillEffect;

    protected virtual void OnEnable()
    {
        skillEffect = new SkillEffect(effectType, power);
    }

    public virtual string GetDescription()
    {
        return string.Format(description, power, level);
    }
}
