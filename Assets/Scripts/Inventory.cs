using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static EnumData;

public class Inventory : MonoBehaviour
{
    private GameObject currentInventoryUI;
    private bool isOpen = false;
    private List<ItemData> items = new List<ItemData>();
    private void OnEnable() => UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
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
