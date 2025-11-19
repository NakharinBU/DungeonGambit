using System.Collections.Generic;
using UnityEngine;
using static EnumData;

public class Inventory : MonoBehaviour
{

    private List<ItemData> items = new List<ItemData>();
    [SerializeField] private int capacity = 20;

    private void Awake()
    {
        if (items == null) items = new List<ItemData>();
    }

    public bool AddItem(ItemData item)
    {
        if (items.Count >= capacity)
        {
            Debug.Log("กระเป๋าเต็ม!");
            return false;
        }
        items.Add(item);
        Debug.Log($"Added item: {item.itemName}. Inventory size: {items.Count}");
        return true;
    }


    public bool RemoveItem(ItemData item)
    {
        return items.Remove(item);
    }


    public ItemData GetItemByType(ItemType type)
    {
        return items.Find(i => i.Type == type);
    }


    public List<ItemData> GetAllItems()
    {
        return new List<ItemData>(items);
    }
}
