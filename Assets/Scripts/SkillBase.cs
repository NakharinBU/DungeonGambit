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

    public GameObject vfxPrefab;
    public float vfxDuration = 0.5f;

    public AudioClip sfxClip;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    protected virtual void OnEnable()
    {
        skillEffect = new SkillEffect(effectType, power);
    }

    public virtual string GetDescription()
    {
        return string.Format(description, power, level);
    }
}
