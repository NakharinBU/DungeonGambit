using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "AreaDamageSkill", menuName = "Skills/Active/AreaDamageSkill")]
public class AreaDamageSkill : ActiveSkill
{
    public float damageMultiplier = 1f;

    [Header("Targeting Parameters")]
    // รัศมีของ AOE (Area of Effect)
    // 0 = Single Target (1x1)
    // 1 = AOE ขนาด 3x3 (Fireball เดิม)
    // 2 = AOE ขนาด 5x5
    public int aoeRadius = 1;

    public override bool Activate(Player user, Vector2Int target)
    {
        if (!CanActivate(user)) return false;

        PayCost(user);

        float finalDamage = power + (user.stats?.atk * damageMultiplier ?? 0f);

        if (vfxPrefab != null)
        {
            user.DungeonManagerRef?.PlayVFX(vfxPrefab, target, vfxDuration);
        }

        if (sfxClip != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(sfxClip, sfxVolume);
        }


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
                    skillEffect.Apply(targetChar, (int)finalDamage);
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
