using UnityEngine;

[CreateAssetMenu(fileName = "New Potion Item", menuName = "Inventory/Consumable/Potion")]
public class PotionItem : ItemData
{
    public override void Use(Player target)
    {
        // ... Logic พิเศษของ Potion
        base.Use(target); // เรียก Logic ฐานด้วย
    }
    
}