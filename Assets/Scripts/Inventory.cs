using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static EnumData;

public class Inventory : MonoBehaviour
{
    private GameObject currentInventoryUI;
    private bool isOpen = false;
    private Dictionary<ItemData, int> itemStacks = new Dictionary<ItemData, int>();

    private void OnEnable() => UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    [SerializeField] private int capacity = 12;

    private void Awake()
    {
        if (itemStacks == null) itemStacks = new Dictionary<ItemData, int>();
    }

    public bool AddItem(ItemData item, int quantity)
    {
        if (itemStacks.ContainsKey(item))
        {
            itemStacks[item] += quantity;
            Debug.Log($"Stacked item: {item.itemName}. New quantity: {itemStacks[item]}");
            return true;
        }
        
        if (itemStacks.Count >= capacity)
        {
            Debug.LogWarning("Inventory is full (Max unique items reached). Cannot add new unique item.");
            return false;
        }

        itemStacks.Add(item, quantity);
        Debug.Log($"Added new item: {item.itemName} x{quantity}. Unique items: {itemStacks.Count}");
        return true;
    }


    public bool RemoveItem(ItemData item, int quantityToRemove = 1)
    {
        if (!itemStacks.ContainsKey(item)) return false;

        int currentQuantity = itemStacks[item];
        if (currentQuantity < quantityToRemove) return false;

        int newQuantity = currentQuantity - quantityToRemove;

        if (newQuantity <= 0)
        {
            itemStacks.Remove(item);
            Debug.Log($"Removed item: {item.itemName}. All stacks consumed.");
        }
        else
        {
            itemStacks[item] = newQuantity;
            Debug.Log($"Reduced quantity of {item.itemName}. Remaining: {newQuantity}");
        }

        return true;
    }


    public ItemData GetItemByType(ItemType type)
    {
        return itemStacks.Keys.FirstOrDefault(i => i.Type == type);
    }


    public Dictionary<ItemData, int> GetAllItemStacks()
    {
        return new Dictionary<ItemData, int>(itemStacks);
    }

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

        if (InventoryUIManager.Instance != null)
        {
            InventoryUIManager.Instance.SetPanelActive(isOpen);
        }
        else
        {
            Debug.LogError("InventoryUIManager Instance is NULL. Did you forget to place it in the Scene?");
        }

        if (isOpen)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        currentInventoryUI = GameObject.Find("InventoryPanelRoot");

        if (currentInventoryUI != null)
        {
            currentInventoryUI.SetActive(false);
            Debug.Log("Inventory UI successfully rebound in new scene.");
        }
    }
}
