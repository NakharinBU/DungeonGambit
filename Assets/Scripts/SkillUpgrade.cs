using UnityEngine;

[CreateAssetMenu(fileName = "SkillUpgrade", menuName = "Skills/Upgrade Option")]
public class SkillUpgrade : ScriptableObject
{
    public SkillBase skill; // Skill ที่จะถูกอัพเกรด/เพิ่ม
    public int cost = 5; // FIX: ราคาอัพเกรดด้วย Soul Point
    public EnumData.UpgradeType upgradeType; // ADD, UPGRADE, REPLACE

    public bool ApplyUpgrade(Player player)
    {
        // 1. ตรวจสอบ Soul Point
        int currentSoul = player.currencies.Get(EnumData.CurrencyType.SoulPoint);
        if (currentSoul < cost)
        {
            Debug.Log("Not enough Soul Points to purchase this upgrade!");
            return false;
        }

        // 2. จ่าย Soul Point
        player.currencies.Add(EnumData.CurrencyType.SoulPoint, -cost); // ลบ Soul Point

        // 3. ใช้ Logic อัพเกรด/เพิ่ม
        switch (upgradeType)
        {
            case EnumData.UpgradeType.Skill:
                // ต้องมี List<SkillBase> ใน Player
                // player.skills.Add(Instantiate(skill));
                if (skill is PassiveSkill passive)
                {
                    passive.ApplyOnAcquire(player); // สำหรับ Passive Skill
                }
                break;
                // case EnumData.UpgradeType.UPGRADE:
                //    // Logic การเพิ่ม Level ให้ Skill ที่มีอยู่
                //    break;
        }

        return true;
    }
}
