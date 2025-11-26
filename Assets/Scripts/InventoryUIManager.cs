using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager Instance { get; private set; }

    public GameObject inventoryPanel;

    public List<InventorySlotUI> slotUIs;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        if (inventoryPanel == null) inventoryPanel = gameObject;

        inventoryPanel.SetActive(false);
    }

    public void UpdateInventoryUI()
    {
        Player player = Player.Instance;
        if (player == null || player.inventory == null) return;

        Dictionary<ItemData, int> itemStacks = player.inventory.GetAllItemStacks();

        List<KeyValuePair<ItemData, int>> stacksList = itemStacks.ToList();

        for (int i = 0; i < slotUIs.Count; i++)
        {
            if (i < stacksList.Count)
            {
                ItemData item = stacksList[i].Key;
                int quantity = stacksList[i].Value;

                slotUIs[i].SetupSlot(item, quantity);
            }
            else
            {
                slotUIs[i].SetupSlot(null);
            }
        }
    }

    public void SetPanelActive(bool state)
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(state);
            if (state)
            {
                UpdateInventoryUI();
            }
        }
    }
}
