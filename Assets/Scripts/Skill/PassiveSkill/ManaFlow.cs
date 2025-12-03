using UnityEngine;

[CreateAssetMenu(fileName = "Passive_ManaFlow", menuName = "Skills/Passive/Mana Flow on Kill")]
public class ManaFlow : PassiveSkill
{
    public override void OnTrigger(Player user, object context = null)
    {

        if (trigger == EnumData.PassiveTrigger.OnKill)
        {
            if (user.stats != null)
            {
                if (vfxPrefab != null && user.DungeonManagerRef != null)
                {
                    user.DungeonManagerRef.PlayVFX(vfxPrefab, user.position, vfxDuration);
                }

                if (sfxClip != null && AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFX(sfxClip, sfxVolume);
                }

                user.RestoreMana((int)power);

                Debug.Log($"[{skillName}] Activated! Gained {power} Mana on Kill. Current Mana: {user.stats.mp}");
            }
        }
    }
}
