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

    // เก็บ SkillEffect ไว้เพื่อใช้ในเมธอด Activate/Apply
    protected SkillEffect skillEffect;

    protected virtual void OnEnable()
    {
        // สร้าง SkillEffect เมื่อคลาสถูกเปิดใช้
        skillEffect = new SkillEffect(effectType, power);
    }

    // เมธอดสำหรับเรียกใช้คำอธิบาย
    public virtual string GetDescription()
    {
        return string.Format(description, power, level);
    }
}
