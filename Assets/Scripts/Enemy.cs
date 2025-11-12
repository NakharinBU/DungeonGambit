using UnityEngine;
using static EnumData;

public abstract class Enemy : Character
{
    public EnemyType enemyType;
    public int visionRange = 5;

    // เมธอดหลักในการตัดสินใจ (ถูกเรียกโดย TurnManager)
    public abstract void DecideAction(Player player);

    // *** MODIFIED: MoveTowards ถูกเปลี่ยนเป็น Abstract ***
    // ทุก Subclass ของ Enemy ต้องกำหนด Logic การเคลื่อนที่เอง
    public abstract bool MoveTowards(Vector2Int targetPos);
}

