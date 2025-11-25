using System.Collections.Generic;
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

        List<ItemData> items = player.inventory.GetAllItems();

        for (int i = 0; i < slotUIs.Count; i++)
        {
            if (i < items.Count)
            {
                slotUIs[i].SetupSlot(items[i], 1);
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
