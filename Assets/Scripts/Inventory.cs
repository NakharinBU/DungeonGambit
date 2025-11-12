using System.Collections.Generic;
using UnityEngine;
using static EnumData;

public class Inventory : MonoBehaviour
{

    private List<Item> items = new List<Item>();
    private int capacity;

    public Inventory(int initialCapacity = 20)
    {
        capacity = initialCapacity;
    }


    public bool AddItem(Item item)
    {
        if (items.Count >= capacity)
        {
            Debug.Log("กระเป๋าเต็ม!");
            return false;
        }
        items.Add(item);
        return true;
    }


    public bool RemoveItem(Item item)
    {
        return items.Remove(item);
    }


    public Item GetItemByType(ItemType type)
    {
        return items.Find(i => i.Type == type);
    }


    public List<Item> GetAllItems()
    {
        return new List<Item>(items);
    }
}
