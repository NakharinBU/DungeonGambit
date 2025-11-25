using System.Collections.Generic;
using UnityEngine;

public abstract class ActiveSkill : SkillBase
{
    public int mpCost;
    public int range;

    public abstract List<Vector2Int> GetTargetHighlights(Vector2Int targetPos);

    // เมธอดหลักที่ Player จะเรียกใช้
    public abstract bool Activate(Player user, Vector2Int target);

    protected bool CanActivate(Player user)
    {

        if (user.stats.mp < mpCost) 
        {
            return false;
        }
        return true;
    }

    protected void PayCost(Player user)
    {
        user.UseMana(mpCost);
    }
}
