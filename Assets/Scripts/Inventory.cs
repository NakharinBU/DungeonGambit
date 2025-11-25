using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static EnumData;

public class Inventory : MonoBehaviour
{

    private List<ItemData> items = new List<ItemData>();
    [SerializeField] private int capacity = 12;

    private void Awake()
    {
        if (items == null) items = new List<ItemData>();
    }

    public bool AddItem(ItemData item)
    {
        if (items.Count >= capacity)
        {
            Debug.Log("¡ÃÐà»ëÒàµçÁ!");
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

    public GameObject inventoryUI;

    private bool isOpen = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }
    void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryUI.SetActive(isOpen);

        // หยุด/ปล่อยการควบคุม Player เมื่อเปิดเมนู
        if (isOpen)
            Time.timeScale = 0f; // หยุดเกมถ้าต้องการ
        else
            Time.timeScale = 1f;

        // ปิดเมาส์ล็อค ถ้ามีระบบ FPS
        //Cursor.visible = isOpen;
        //Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
