using UnityEngine;
using System.Collections.Generic;

public class Chest : InteractableObject
{
    public bool isOpened = false;

    public List<ItemData> contents = new List<ItemData>();

    public Sprite openedSprite;

    public override void Interact(Player player)
    {
        if (isOpened)
        {
            Debug.Log("This chest is already empty.");
            return;
        }


        if (player.inventory == null) // **FIX 3: ป้องกัน Crash ถ้า Inventory หลุดไป**
        {
            Debug.LogError("Player Inventory is Null! Cannot add items from chest.");
            return;
        }


        if (contents.Count > 0)
        {
            Debug.Log($"Player opened the chest and found {contents.Count} items!");

            foreach (ItemData item in contents)
            {
                if (item != null)
                {
                    player.inventory.AddItem(item);
                    Debug.Log($"- Added item: {item.itemName}");
                }
            }

            isOpened = true;

            if (openedSprite != null)
            {
                SpriteRenderer sr = GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = openedSprite;
                }
            }
        }
        else
        {
            Debug.Log("Player opened an empty chest.");
            isOpened = true;
        }
    }
}