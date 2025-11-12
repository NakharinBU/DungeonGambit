using UnityEngine;
using static EnumData;

public abstract class Enemy : Character
{
    public EnemyType enemyType;
    public int visionRange = 5;

    public abstract void DecideAction(Player player);
    public abstract bool MoveTowards(Vector2Int targetPos);
}

