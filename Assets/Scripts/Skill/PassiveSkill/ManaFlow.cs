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
                user.stats.mp += power;

                user.stats.mp = Mathf.Min(user.stats.mp, user.stats.maxMp);

                Debug.Log($"[{skillName}] Activated! Gained {power} Mana on Kill. Current Mana: {user.stats.mp}");
            }
        }
    }
}
