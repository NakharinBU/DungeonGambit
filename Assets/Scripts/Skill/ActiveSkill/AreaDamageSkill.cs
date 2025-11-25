using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AreaDamageSkill", menuName = "Skills/Active/AreaDamageSkill")]
public class AreaDamageSkill : ActiveSkill
{
    // Optional: ตัวคูณดาเมจ (เช่น baseDamage * ผู้ใช้.Attack * damageMultiplier)
    // public float damageMultiplier = 1f; 

    [Header("Targeting Parameters")]
    // รัศมีของ AOE (Area of Effect)
    // 0 = Single Target (1x1)
    // 1 = AOE ขนาด 3x3 (Fireball เดิม)
    // 2 = AOE ขนาด 5x5
    public int aoeRadius = 1;

    public override bool Activate(Player user, Vector2Int target)
    {
        if (!CanActivate(user)) return false;

        // 1. จ่ายค่าร่าย
        PayCost(user);

        // คำนวณดาเมจสุดท้าย (ต้องมี logic ในการคำนวณดาเมจตาม stats ของ user)
        // float finalDamage = baseDamage + (user.stats?.Attack * damageMultiplier ?? 0f);
        float finalDamage = power; // ใช้ค่า baseDamage ไปก่อน ถ้าไม่มี Stats

        // 2. หาเป้าหมายตามรัศมี AOE ที่กำหนด
        for (int x = -aoeRadius; x <= aoeRadius; x++)
        {
            for (int y = -aoeRadius; y <= aoeRadius; y++)
            {
                Vector2Int aoeTarget = target + new Vector2Int(x, y);
                // 3. หา Character ที่ตำแหน่งนั้น
                Character targetChar = user.DungeonManagerRef?.GetCharacterAtPosition(aoeTarget);

                if (targetChar != null && targetChar != user)
                {
                    // 4. ใช้ SkillEffect (คุณอาจต้องปรับปรุง SkillEffect ให้รับค่าดาเมจ)
                    // *RECOMMENDED:* skillEffect.Apply(targetChar, finalDamage);

                    // ใช้ตามรูปแบบเดิม (สมมติว่า skillEffect ดึงค่าดาเมจจาก asset นี้เอง)
                    skillEffect.Apply(targetChar);
                }
            }
        }

        user.DungeonManagerRef?.ResolveEnemyTurn();

        return true;
    }

    public override List<Vector2Int> GetTargetHighlights(Vector2Int center)
    {
        List<Vector2Int> highlights = new List<Vector2Int>();

        for (int x = -aoeRadius; x <= aoeRadius; x++)
        {
            for (int y = -aoeRadius; y <= aoeRadius; y++)
            {
                highlights.Add(center + new Vector2Int(x, y));
            }
        }
        return highlights;
    }
}
