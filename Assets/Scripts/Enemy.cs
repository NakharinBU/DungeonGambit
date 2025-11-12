using UnityEngine;
using static EnumData;

public abstract class Enemy : Character
{
    public EnemyType enemyType;
    public int visionRange = 5;

    // เมธอด TakeTurn ถูกลบไป เพราะไม่มี TurnManager มาเรียกใช้แล้ว
    public abstract void DecideAction(Player player);
}

public class KnightEnemy : Enemy // Example Subclass
{
    protected override void Awake()
    {
        base.Awake();
        characterName = "Knight";
        stats = new Status(15, 0, 4, 2);
    }

    public override void DecideAction(Player player)
    {
        // Logic นี้จะไม่ถูกเรียกใช้จนกว่าจะมีการสร้าง TurnManager
        if (Vector2Int.Distance(position, player.position) <= 1.5f) Attack(player);
        else MoveTowards(player);
    }

    protected virtual void MoveTowards(Player target)
    {
        Vector2Int direction = new Vector2Int(
            Mathf.Clamp(target.position.x - position.x, -1, 1),
            Mathf.Clamp(target.position.y - position.y, -1, 1)
        );
        base.Move(direction);
    }
}
