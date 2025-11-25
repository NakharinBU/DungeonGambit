using UnityEngine;

[CreateAssetMenu(fileName = "SkillCard_", menuName = "System/Skill Card Data")]
public class SkillCardData : ScriptableObject
{
    [Header("Display Info")]
    public string cardName = "New Skill Card";
    public string description = "รายละเอียดของสกิลนี้";
    public Sprite artwork;

    [Header("Cost & Type")]
    public int soulCost = 1;

    public SkillBase skillAsset;
    public SkillType skillType;

    public enum SkillType
    {
        Active,
        Passive
    }
}
