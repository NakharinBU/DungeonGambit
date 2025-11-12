using UnityEngine;
using static EnumData;

public class ItemDrop : InteractableObject
{
    public int value = 10;

    public override void Interact(Player player)
    {
        Debug.Log($"Player collected {value} Gold!");
        player.currencies.Add(CurrencyType.Gold,value);
        DestroySelf(); // เก็บแล้วหายไป
    }
}
